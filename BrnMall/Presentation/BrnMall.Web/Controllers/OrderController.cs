using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.Models;

using fastJSON;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 订单控制器类
    /// </summary>
    public class OrderController : BaseWebController
    {
        private static object _locker = new object();//锁对象

        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrder()
        {
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid);
            if (orderProductList.Count < 1)
                return PromptView("购物车中没有商品，请先选择商品");

            //配送地址id
            int saId = GetRouteInt("saId");
            if (saId == 0)
                saId = WebHelper.GetQueryInt("saId");
            //支付插件名称
            string payName = GetRouteString("payName");
            if (payName.Length == 0)
                payName = WebHelper.GetQueryString("payName");

            ConfirmOrderModel model = new ConfirmOrderModel();

            if (saId > 0)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            if (model.DefaultFullShipAddressInfo == null)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetDefaultFullShipAddress(WorkContext.Uid);

            if (payName.Length > 0)
                model.DefaultPayPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            if (model.DefaultPayPluginInfo == null)
                model.DefaultPayPluginInfo = Plugins.GetDefaultPayPlugin();

            model.PayCreditName = Credits.PayCreditName;
            model.UserPayCredits = WorkContext.PartUserInfo.PayCredits;
            model.MaxUsePayCredits = Credits.GetOrderMaxUsePayCredits(WorkContext.PartUserInfo.PayCredits);

            int allShipFee = 0;
            decimal amount = 0M;
            List<StoreOrder> storeOrderList = new List<StoreOrder>();
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);
            foreach (KeyValuePair<StoreInfo, List<OrderProductInfo>> item in tidiedOrderProductList)
            {
                StoreOrder storeOrder = new StoreOrder();
                storeOrder.StoreInfo = item.Key;
                storeOrder.OrderProductList = item.Value;
                storeOrder.ProductAmount = Orders.SumOrderProductAmount(item.Value);
                storeOrder.FullCut = Orders.SumFullCut(item.Value);
                storeOrder.ShipFee = model.DefaultFullShipAddressInfo != null ? Orders.GetShipFee(model.DefaultFullShipAddressInfo.ProvinceId, model.DefaultFullShipAddressInfo.CityId, item.Value) : 0;
                storeOrder.TotalCount = Orders.SumOrderProductCount(item.Value);
                storeOrder.TotalWeight = Orders.SumOrderProductWeight(item.Value);
                storeOrderList.Add(storeOrder);

                allShipFee += storeOrder.ShipFee;
                amount += storeOrder.ProductAmount - storeOrder.FullCut;
            }
            model.StoreOrderList = storeOrderList;

            IPayPlugin payPlugin = (IPayPlugin)model.DefaultPayPluginInfo.Instance;
            model.PayFee = payPlugin.GetPayFee(amount, DateTime.Now, WorkContext.PartUserInfo);
            model.AllOrderAmount = amount + allShipFee + model.PayFee;

            model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);

            return View(model);
        }

        /// <summary>
        /// 支付插件列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PayPluginList()
        {
            List<PluginInfo> payPluginList = Plugins.GetPayPluginList();

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (PluginInfo pluginInfo in payPluginList)
            {
                sb.AppendFormat("{0}\"systemname\":\"{1}\",\"friendname\":\"{2}\",\"description\":\"{3}\"{4},", "{", pluginInfo.SystemName, pluginInfo.FriendlyName, pluginInfo.Description, "}");
            }
            if (payPluginList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 获得有效的优惠劵列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidCouponList()
        {
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            List<int> storeIdList = Orders.GetStoreIdList(orderProductList);
            decimal orderAmount = Orders.SumOrderProductAmount(orderProductList) - Orders.SumFullCut(orderProductList);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (DataRow row in Coupons.GetUnUsedCouponList(WorkContext.Uid).Rows)
            {
                int storeId = TypeHelper.ObjectToInt(row["storeid"]);
                if (TypeHelper.ObjectToInt(row["state"]) == 0)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) < 1 && TypeHelper.ObjectToDateTime(row["usestarttime"]) > DateTime.Now)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) < 1 && TypeHelper.ObjectToDateTime(row["useendtime"]) <= DateTime.Now)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["userranklower"]) > WorkContext.UserRid)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["orderamountlower"]) > orderAmount)
                {
                    break;
                }
                else if (!storeIdList.Exists(x => x == storeId))
                {
                    break;
                }
                else
                {
                    int limitStoreCid = TypeHelper.ObjectToInt(row["limitstorecid"]);
                    if (limitStoreCid > 0)
                    {
                        foreach (OrderProductInfo orderProductInfo in orderProductList)
                        {
                            if (orderProductInfo.StoreId == storeId && orderProductInfo.Type == 0 && orderProductInfo.StoreCid != limitStoreCid)
                            {
                                break;
                            }
                        }
                    }

                    if (TypeHelper.ObjectToInt(row["limitproduct"]) == 1)
                    {
                        List<OrderProductInfo> commonOrderProductList = Orders.GetCommonOrderProductList(orderProductList);
                        string pidList = Orders.GetRecordIdList(commonOrderProductList);
                        if (!Coupons.IsSameCouponType(TypeHelper.ObjectToInt(row["coupontypeid"]), pidList))
                        {
                            break;
                        }
                    }
                }
                sb.AppendFormat("{0}\"couponid\":\"{1}\",\"name\":\"{2}\"{3},", "{", row["couponid"], row["name"], "}");
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 验证优惠劵编号
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCouponSN()
        {
            string couponSN = WebHelper.GetQueryString("couponSN");//优惠劵编号
            if (couponSN.Length == 0)
                return AjaxResult("emptycouponsn", "请输入优惠劵编号");
            else if (couponSN.Length != 16)
                return AjaxResult("errorcouponsn", "优惠劵编号不正确");

            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
            if (couponInfo == null)//不存在
            {
                return AjaxResult("noexist", "优惠劵不存在");
            }
            else
            {
                int result = 0;
                List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
                List<int> storeIdList = Orders.GetStoreIdList(orderProductList);
                int limitId = Coupons.VerifyCoupon(out result, couponInfo, false, WorkContext.PartUserInfo, storeIdList, orderProductList);
                switch (result)
                {
                    case 0:
                        return AjaxResult("success", "此优惠劵可以正常使用");
                    case 1:
                        return AjaxResult("nocoupontype", "优惠劵类型不存在");
                    case 2:
                        return AjaxResult("closecoupontype", "优惠劵类型已关闭");
                    case 3:
                        return AjaxResult("unstartcoupon", "此优惠劵还未到使用时间");
                    case 4:
                        return AjaxResult("expiredcoupon", "此优惠劵已过期");
                    case 5:
                        return AjaxResult("userranklowercoupon", "你的用户等级太低，不能使用此优惠劵");
                    case 6:
                        return AjaxResult("nomutcoupon", "此优惠劵不能叠加使用");
                    case 7:
                        return AjaxResult("orderamountlowercoupon", "订单金额太低，不能使用此优惠劵");
                    case 8:
                        return AjaxResult("wrongstorecoupon", "此优惠劵只能在购买指定店铺的商品时使用");
                    case 9:
                        return AjaxResult("limitstoreclasscoupon", "此优惠劵只能在购买指定店铺分类的商品时使用");
                    case 10:
                        return AjaxResult("limitproductcoupon", "此优惠劵只能在购买指定商品时使用");
                    default:
                        return AjaxResult("exception", "验证失败,请联系管理员");
                }
            }
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitOrder()
        {
            lock (_locker)
            {
                //验证验证码
                if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
                {
                    string verifyCode = WebHelper.GetFormString("verifyCode");//验证码
                    if (string.IsNullOrWhiteSpace(verifyCode))
                    {
                        return AjaxResult("emptyverifycode", "验证码不能为空");
                    }
                    else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                    {
                        return AjaxResult("wrongverifycode", "验证码不正确");
                    }
                }

                string buyerRemark = WebHelper.GetFormString("buyerRemark");//买家备注
                //验证买家备注的内容长度
                if (StringHelper.GetStringLength(buyerRemark) > 125)
                    return AjaxResult("muchbuyerremark", "备注最多填写125个字");

                int saId = WebHelper.GetFormInt("saId");//配送地址id
                //验证配送地址是否为空
                FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
                if (fullShipAddressInfo == null)
                    return AjaxResult("emptysaid", "请选择配送地址");

                string payName = WebHelper.GetFormString("payName");//支付方式名称
                //验证支付方式是否为空
                PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
                if (payPluginInfo == null)
                    return AjaxResult("empaypay", "请选择支付方式");

                List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid);//购物车商品列表
                //验证购物车是否为空
                if (orderProductList.Count < 1)
                    return AjaxResult("emptyshopcart", "购物车中没有商品");

                List<int> storeIdList = Orders.GetStoreIdList(orderProductList);
                List<StoreInfo> storeList = new List<StoreInfo>(storeIdList.Count);//店铺列表
                //验证店铺状态
                foreach (int storeId in storeIdList)
                {
                    StoreInfo storeInfo = Stores.GetStoreById(storeId);
                    if (storeInfo.State != (int)StoreState.Open)
                        return AjaxResult("storeclose", "店铺" + storeInfo.Name + "已经关闭");
                    storeList.Add(storeInfo);
                }

                int payCreditCount = WebHelper.GetFormInt("payCreditCount");//支付积分
                //验证支付积分
                if (payCreditCount > 0)
                {
                    if (payCreditCount > WorkContext.PartUserInfo.PayCredits)
                        return AjaxResult("noenoughpaycredit", "你使用的" + Credits.PayCreditName + "数超过你所拥有的" + WorkContext.PartUserInfo.PayCredits + "数");
                    if (payCreditCount > Credits.OrderMaxUsePayCredits * storeList.Count)
                        return AjaxResult("maxusepaycredit", "此笔订单最多使用" + Credits.OrderMaxUsePayCredits + "个" + Credits.PayCreditName);
                }

                #region 验证优惠劵

                string[] couponIdList = Request.Form.GetValues("couponId");//客户已经激活的优惠劵
                string[] couponSNList = Request.Form.GetValues("couponSN");//客户还未激活的优惠劵
                List<CouponInfo> couponList = new List<CouponInfo>();
                if (couponIdList.Length > 0 || couponSNList.Length > 0)
                {
                    foreach (string couponId in couponIdList)
                    {
                        int tempCouponId = TypeHelper.StringToInt(couponId);
                        if (tempCouponId > 0)
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponId(TypeHelper.StringToInt(couponId));
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }
                    foreach (string couponSN in couponSNList)
                    {
                        if (!string.IsNullOrWhiteSpace(couponSN))
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵" + couponSN + "不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }

                    int result = 0;
                    bool isMut = couponList.Count > 1;
                    foreach (CouponInfo couponInfo in couponList)
                    {
                        int limitId = Coupons.VerifyCoupon(out result, couponInfo, isMut, WorkContext.PartUserInfo, storeIdList, orderProductList);
                        if (result == 0)
                            continue;

                        switch (result)
                        {
                            case 1:
                                return AjaxResult("nocoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型不存在");
                            case 2:
                                return AjaxResult("closecoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型已关闭");
                            case 3:
                                return AjaxResult("unstartcoupon", "编号为" + couponInfo.CouponSN + "优惠劵还未到使用时间");
                            case 4:
                                return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                            case 5:
                                return AjaxResult("userranklowercoupon", "你的用户等级太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                            case 6:
                                return AjaxResult("nomutcoupon", "编号为" + couponInfo.CouponSN + "优惠劵不能叠加使用");
                            case 7:
                                return AjaxResult("orderamountlowercoupon", "订单金额太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                            case 8:
                                return AjaxResult("wrongstorecoupon", "此优惠劵只能在购买指定店铺的商品时使用");
                            case 9:
                                return AjaxResult("limitstoreclasscoupon", "此优惠劵只能在购买指定店铺分类的商品时使用");
                            case 10:
                                return AjaxResult("limitproductcoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能在购买指定商品时使用");
                        }
                    }
                }

                #endregion

                List<SinglePromotionInfo> singlePromotionList = new List<SinglePromotionInfo>();//购物车中的单品促销活动
                //验证购物车
                ActionResult verifyCartResult = VerifyCart(orderProductList, ref singlePromotionList);
                if (verifyCartResult != null)
                    return verifyCartResult;

                //最佳配送时间
                DateTime bestTime = TypeHelper.StringToDateTime(WebHelper.GetFormString("bestTime"), new DateTime(1970, 1, 1));

                string oidList = "";
                foreach (StoreInfo storeInfo in storeList)
                {
                    OrderInfo orderInfo = Orders.CreateOrder(WorkContext.PartUserInfo, storeInfo, orderProductList.FindAll(x => x.StoreId == storeInfo.StoreId), singlePromotionList.FindAll(x => x.StoreId == storeInfo.StoreId), fullShipAddressInfo, payPluginInfo, ref payCreditCount, couponList.FindAll(x => x.StoreId == storeInfo.StoreId), bestTime, buyerRemark, WorkContext.IP);
                    if (orderInfo != null)
                    {
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = WorkContext.Uid,
                            RealName = "本人",
                            ActionType = (int)OrderActionType.Submit,
                            ActionTime = DateTime.Now,
                            ActionDes = "您提交了订单，请等待系统确认"
                        });
                        if (orderInfo.OrderState == (int)OrderState.WaitPaying)
                        {
                            oidList += orderInfo.Oid + ",";
                        }
                        else
                        {
                            //创建订单处理
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = 0,
                                RealName = "系统",
                                ActionType = (int)OrderActionType.Pay,
                                ActionTime = DateTime.Now,
                                ActionDes = "由于订单的支付金额为0元，所以无需支付"
                            });
                        }
                    }
                    else
                    {
                        return AjaxResult("error", "提交失败，请联系管理员");
                    }
                }
                Orders.SetCartProductCountCookie(0);
                return AjaxResult("success", Url.Action("SubmitSuccess", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
            }
        }

        /// <summary>
        /// 提交成功
        /// </summary>
        public ActionResult SubmitSuccess()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.WaitPaying)
                    orderList.Add(orderInfo);
                else
                    return Redirect("/");
                allSurplusMoney += orderInfo.SurplusMoney;
            }

            if (orderList.Count < 1)
                return Redirect("/");

            SubmitSuccessModel model = new SubmitSuccessModel();
            model.OidList = oidList;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(orderList[0].PaySystemName);
            model.AllSurplusMoney = allSurplusMoney;
            return View(model);
        }

        /// <summary>
        /// 支付展示
        /// </summary>
        public ActionResult PayShow()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.WaitPaying)
                    orderList.Add(orderInfo);
                else
                    return Redirect("/");
                allSurplusMoney += orderInfo.SurplusMoney;
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                return Redirect("/");

            PayShowModel model = new PayShowModel();
            model.OidList = oidList;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(orderList[0].PaySystemName);
            model.ShowView = "/Plugins/" + model.PayPlugin.Folder + "/Views/Show.cshtml";
            model.AllSurplusMoney = allSurplusMoney;
            return View(model);
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        public ActionResult PaySuccess()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.WaitPaying)
                    orderList.Add(orderInfo);
                else
                    return Redirect("/");
                allSurplusMoney += orderInfo.SurplusMoney;
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                return Redirect("/");

            return View(orderList);
        }

        /// <summary>
        /// 验证购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <returns></returns>
        private ActionResult VerifyCart(List<OrderProductInfo> orderProductList, ref List<SinglePromotionInfo> singlePromotionList)
        {
            //商品id列表
            string pidList = Orders.GetPidList(orderProductList);

            //验证商品信息
            List<PartProductInfo> partProductList = Products.GetPartProductList(pidList);
            if (partProductList.Count != StringHelper.SplitString(pidList).Length)
                return AjaxResult("outsaleproduct", "购物车中商品已经下架，请重新下单");
            PartProductInfo errorProductInfo = Orders.VerifyProductInfo(orderProductList, partProductList);
            if (errorProductInfo != null)
                return AjaxResult("changeproduct", "商品" + errorProductInfo.Name + "信息有变化，请重新下单");

            //验证商品库存
            List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList);
            OrderProductInfo stockoutProductInfo = Orders.VerifyProductStock(orderProductList, productStockList);
            if (stockoutProductInfo != null)
                return AjaxResult("stockoutproduct", "商品" + stockoutProductInfo.Name + "库存不足");

            //验证促销活动
            int result = 0;
            List<int> verifiedSuitList = new List<int>();//已经验证的套装
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)//普通商品
                {
                    //验证买送促销活动
                    Orders.VerifyBuySendPromotion(out result, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        switch (result)
                        {
                            case 1:
                                return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止");
                            case 2:
                                return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请重新下单");
                            case 3:
                                return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动");
                            case 4:
                                return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请重新下单");
                        }
                    }

                    //验证单品促销活动
                    SinglePromotionInfo singlePromotionInfo = Orders.VerifySinglePromotion(out result, ref singlePromotionList, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        switch (result)
                        {
                            case 1:
                                return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止");
                            case 2:
                                return AjaxResult("replacesingel", "商品" + orderProductInfo.Name + "的单品促销活动已经替换，请重新下单");
                            case 3:
                                return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动");
                            case 4:
                                return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个");
                            case 5:
                                return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个");
                            case 6:
                                return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足");
                            case 7:
                                return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "次");
                            case 8:
                                return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请重新下单");
                        }
                    }

                    //验证满赠促销活动
                    Orders.VerifyFullSendPromotion(out result, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        switch (result)
                        {
                            case 1:
                                return AjaxResult("stopfullsend", "商品" + orderProductInfo.Name + "的满赠促销活动已经停止");
                            case 2:
                                return AjaxResult("replacefullsend", "商品" + orderProductInfo.Name + "的满赠促销活动已经替换，请重新下单");
                            case 3:
                                return AjaxResult("userranklowerfullsend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的满赠促销活动");
                            case 4:
                                return AjaxResult("changefullsend", "商品" + orderProductInfo.Name + "的满赠促销活动已经改变，请重新下单");
                        }
                    }

                    //验证满减促销活动
                    Orders.VerifyFullCutPromotion(out result, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        switch (result)
                        {
                            case 1:
                                return AjaxResult("stopfullcut", "商品" + orderProductInfo.Name + "的满减促销活动已经停止");
                            case 2:
                                return AjaxResult("replacefullcut", "商品" + orderProductInfo.Name + "的满减促销活动已经替换，请重新下单");
                            case 3:
                                return AjaxResult("userranklowerfullcut", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的满减促销活动");
                            case 4:
                                return AjaxResult("changefullcut", "商品" + orderProductInfo.Name + "的满减促销活动已经改变，请重新下单");
                        }
                    }

                    //验证赠品促销活动
                    GiftPromotionInfo giftPromotionInfo = Orders.VerifyGiftPromotion(out result, orderProductList, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        switch (result)
                        {
                            case 1:
                                return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止");
                            case 2:
                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请重新下单");
                            case 3:
                                return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动");
                            case 4:
                                return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个");
                            case 5:
                                return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止");
                            case 6:
                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请重新下单");
                            case 7:
                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请重新下单");
                        }
                    }
                }
                else if (orderProductInfo.Type == 2)//套装
                {
                    if (verifiedSuitList.Contains(orderProductInfo.ExtCode7))
                        continue;
                    else
                        verifiedSuitList.Add(orderProductInfo.ExtCode7);

                    SuitPromotionInfo suitPromotionInfo = Orders.VerifySuitPromotion(out result, ref singlePromotionList, orderProductList, orderProductInfo, WorkContext.PartUserInfo);
                    if (result != 0)
                    {
                        if (result == -1)
                            return AjaxResult("stopsuit", "套装" + orderProductInfo.ExtStr3 + "已经停止");
                        else if (result == -2)
                            return AjaxResult("userranklowersuit", "你的用户等级太低，无法购买套装" + orderProductInfo.ExtStr3 + "");
                        else if (result == -3)
                            return AjaxResult("ordermuchsuit", "套装" + orderProductInfo.ExtStr3 + "要求每单最多购买" + suitPromotionInfo.QuotaUpper + "个");
                        else if (result == -4)
                            return AjaxResult("usermuchsuit", "套装" + orderProductInfo.ExtStr3 + "要求每人最多购买1个");
                        else
                            return AjaxResult("changesuit", "套装" + orderProductInfo.ExtStr3 + "已经改变，请重新下单");
                    }
                }
            }


            //验证满赠赠品
            List<OrderProductInfo> fullSendMinorOrderProductList = Orders.GetFullSendMinorOrderProductList(orderProductList);
            foreach (OrderProductInfo minorOrderProductInfo in fullSendMinorOrderProductList)
            {
                if (!Promotions.IsExistFullSendProduct(minorOrderProductInfo.ExtCode1, minorOrderProductInfo.Pid, 1))
                    return AjaxResult("changefullsend", "商品" + minorOrderProductInfo.Name + "已经不是赠品，请重新下单");
            }
            return null;
        }

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            if (WorkContext.Uid < 1)
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = AjaxResult("nologin", "请先登录");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("Login", "Account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
    }
}
