using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台订单控制器类
    /// </summary>
    public class OrderController : BaseMallAdminController
    {
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="accountName">账户名</param>
        /// <param name="consignee">收货人</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult OrderList(string storeName, string osn, string accountName, string consignee, string sortColumn, string sortDirection, int storeId = -1, int orderState = 0, int pageSize = 15, int pageNumber = 1)
        {
            //获取用户id
            int uid = -1;
            if (!string.IsNullOrWhiteSpace(accountName))
                uid = Users.GetUidByAccountName(accountName);

            string condition = AdminOrders.GetOrderListCondition(storeId, osn, uid, consignee, orderState);
            string sort = AdminOrders.GetOrderListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrders.GetOrderCount(condition));

            OrderListModel model = new OrderListModel()
            {
                OrderList = AdminOrders.GetOrderList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                OSN = osn,
                AccountName = accountName,
                Consignee = consignee,
                OrderState = orderState
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&storeName={6}&OSN={7}&AccountName={8}&Consignee={9}&OrderState={10}",
                                                          Url.Action("OrderList"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, storeName,
                                                          osn, accountName, consignee, orderState));

            List<SelectListItem> orderStateList = new List<SelectListItem>();
            orderStateList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            orderStateList.Add(new SelectListItem() { Text = "已提交", Value = "10" });
            orderStateList.Add(new SelectListItem() { Text = "等待付款", Value = "30" });
            orderStateList.Add(new SelectListItem() { Text = "待确认", Value = "50" });
            orderStateList.Add(new SelectListItem() { Text = "已确认", Value = "70" });
            orderStateList.Add(new SelectListItem() { Text = "备货中", Value = "90" });
            orderStateList.Add(new SelectListItem() { Text = "已发货", Value = "110" });
            orderStateList.Add(new SelectListItem() { Text = "已完成", Value = "140" });
            orderStateList.Add(new SelectListItem() { Text = "已锁定", Value = "160" });
            orderStateList.Add(new SelectListItem() { Text = "已取消", Value = "180" });
            orderStateList.Add(new SelectListItem() { Text = "已退货", Value = "200" });
            ViewData["orderStateList"] = orderStateList;

            return View(model);
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public ActionResult OrderInfo(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");

            OrderInfoModel model = new OrderInfoModel();
            model.OrderInfo = orderInfo;
            model.RegionInfo = Regions.GetRegionById(orderInfo.RegionId);
            model.UserInfo = Users.GetUserById(orderInfo.Uid);
            model.UserRankInfo = AdminUserRanks.GetUserRankById(model.UserInfo.UserRid);
            model.OrderProductList = AdminOrders.GetOrderProductList(oid);
            model.OrderActionList = OrderActions.GetOrderActionList(oid);

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.ProductShowThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        [HttpGet]
        public ActionResult UpdateOrderShipFee(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "不能修改订单配送费用！");

            UpdateOrderShipFeeModel model = new UpdateOrderShipFeeModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderShipFee(UpdateOrderShipFeeModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "不能订单修改配送费用！");

            if (ModelState.IsValid)
            {
                decimal change = model.ShipFee - orderInfo.ShipFee;
                Orders.UpdateOrderShipFee(orderInfo.Oid, model.ShipFee, orderInfo.OrderAmount + change, orderInfo.SurplusMoney + change);
                CreateOrderAction(oid, OrderActionType.UpdateShipFee, "您订单的配送费用已经修改");
                AddMallAdminLog("更新订单配送费用", "更新订单配送费用,订单ID为:" + oid);
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "更新订单配送费用成功！");
            }
            return View(model);
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        [HttpGet]
        public ActionResult UpdateOrderDiscount(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "不能修改订单折扣！");

            UpdateOrderDiscountModel model = new UpdateOrderDiscountModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderDiscount(UpdateOrderDiscountModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "不能修改订单折扣！");

            if (model.Discount > orderInfo.SurplusMoney)
                ModelState.AddModelError("Discount", "折扣不能大于需支付金额");

            if (ModelState.IsValid)
            {
                decimal change = model.Discount - orderInfo.Discount;
                Orders.UpdateOrderDiscount(orderInfo.Oid, model.Discount, orderInfo.OrderAmount + change, orderInfo.SurplusMoney + change);
                CreateOrderAction(oid, OrderActionType.UpdateDiscount, "您订单的折扣已经修改");
                AddMallAdminLog("更新订单折扣", "更新订单折扣,订单ID为:" + oid);
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "更新订单折扣成功！");
            }
            return View(model);
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.Confirming)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "买家还未付款，不能确认订单！");

            AdminOrders.ConfirmOrder(orderInfo);
            CreateOrderAction(oid, OrderActionType.Confirm, "您的订单已经确认,正在备货中");
            AddMallAdminLog("确认订单", "确认订单,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "确认订单成功");
        }

        /// <summary>
        /// 备货
        /// </summary>
        public ActionResult PreOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.Confirmed)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单还未确认，请先确认！");

            AdminOrders.PreProduct(orderInfo);
            CreateOrderAction(oid, OrderActionType.PreProduct, "您的订单已经备货完成");
            AddMallAdminLog("备货", "备货,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "备货成功");
        }

        /// <summary>
        /// 发货
        /// </summary>
        [HttpGet]
        public ActionResult SendOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.PreProducting)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单还未完成备货,不能发货！");

            SendOrderProductModel model = new SendOrderProductModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 发货
        /// </summary>
        [HttpPost]
        public ActionResult SendOrderProduct(SendOrderProductModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.PreProducting)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单还未完成备货,不能发货！");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                AdminOrders.SendOrder(oid, OrderState.Sended, model.ShipSN, model.ShipCoId, shipCompanyInfo.Name, DateTime.Now);
                CreateOrderAction(oid, OrderActionType.Send, "您订单的已经发货,发货方式为:" + shipCompanyInfo.Name);
                AddMallAdminLog("发货", "发货,订单ID为:" + oid);
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "发货成功！");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        public ActionResult CompleteOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单还未发货，不能完成订单！");

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            AdminOrders.CompleteOrder(ref partUserInfo, orderInfo, DateTime.Now, WorkContext.IP);
            CreateOrderAction(oid, OrderActionType.Complete, "订单已完成，感谢您在" + WorkContext.MallConfig.MallName + "购物，欢迎您再次光临");
            AddMallAdminLog("完成订单", "完成订单,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "完成订单成功");
        }

        /// <summary>
        /// 退货
        /// </summary>
        public ActionResult ReturnOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.Sended && orderInfo.OrderState != (int)OrderState.Completed)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单当前不能退货！");

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            AdminOrders.ReturnOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            CreateOrderAction(oid, OrderActionType.Return, "订单已退货");
            AddMallAdminLog("退货", "退货,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "退货成功");
        }

        /// <summary>
        /// 锁定订单
        /// </summary>
        public ActionResult LockOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单当前不能锁定！");

            AdminOrders.LockOrder(orderInfo);
            CreateOrderAction(oid, OrderActionType.Lock, "订单已锁定");
            AddMallAdminLog("锁定", "锁定,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "锁定成功");
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public ActionResult CancelOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("OrderInfo", new { oid = oid }), "订单当前不能取消！");

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            AdminOrders.CancelOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            CreateOrderAction(oid, OrderActionType.Cancel, "订单已取消");
            AddMallAdminLog("取消订单", "取消订单,订单ID为:" + oid);
            return PromptView(Url.Action("OrderInfo", new { oid = oid }), "取消订单成功");
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public ActionResult PrintOrder(int oid)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在！");

            PrintOrderModel model = new PrintOrderModel()
            {
                OrderInfo = orderInfo,
                RegionInfo = Regions.GetRegionById(orderInfo.RegionId),
                OrderProductList = AdminOrders.GetOrderProductList(oid),
            };

            return View(model);
        }

        /// <summary>
        /// 订单退款列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult RefundList(string storeName, string osn, int storeId = -1, int pageSize = 15, int pageNumber = 1)
        {
            string condition = AdminOrderRefunds.GetOrderRefundListCondition(storeId, osn);
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrderRefunds.GetOrderRefundCount(condition));

            OrderRefundListModel model = new OrderRefundListModel()
            {
                OrderRefundList = AdminOrderRefunds.GetOrderRefundList(pageModel.PageSize, pageModel.PageNumber, condition),
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "选择店铺" : storeName,
                OSN = osn
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&OSN={5}",
                                                          Url.Action("RefundList"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          storeId, storeName, osn));


            return View(model);
        }

        private void CreateOrderAction(int oid, OrderActionType orderActionType, string actionDes)
        {
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = AdminUsers.GetUserDetailById(WorkContext.Uid).RealName,
                ActionType = (int)orderActionType,
                ActionTime = DateTime.Now,
                ActionDes = actionDes
            });
        }
    }
}
