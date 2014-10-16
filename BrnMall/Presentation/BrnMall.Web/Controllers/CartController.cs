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
    /// 购物车控制器类
    /// </summary>
    public class CartController : BaseWebController
    {
        /// <summary>
        /// 购物车
        /// </summary>
        public ActionResult Index()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("List") } }));

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            return View(model);
        }

        /// <summary>
        /// 购物车快照
        /// </summary>
        public ActionResult Snap()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            if (orderProductList.Count < 1)
                return AjaxResult("empty", "购物车为空");

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            return View(model);
        }

        /// <summary>
        /// 添加商品到购物车
        /// </summary>
        public ActionResult AddProduct()
        {
            int pid = WebHelper.GetQueryInt("pid");//商品id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            int scSubmitType = WorkContext.MallConfig.SCSubmitType;//购物车的提交方式

            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                    case 1:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                    case 2:
                        return AjaxResult("nologin", "请先登录");
                    default:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                }
            }

            //判断商品是否存在
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "商品不存在");
                    case 1:
                        return PromptView("/", "商品不存在");
                    case 2:
                        return AjaxResult("noproduct", "请选择商品");
                    default:
                        return PromptView("/", "商品不存在");
                }
            }

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "商品不存在");
                    case 1:
                        return PromptView("/", "商品不存在");
                    case 2:
                        return AjaxResult("noproduct", "请选择商品");
                    default:
                        return PromptView("/", "商品不存在");
                }
            }

            //购买数量不能小于1
            if (buyCount < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                    case 1:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                    case 2:
                        return AjaxResult("buycountmin", "请填写购买数量");
                    default:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                }
            }

            //商品库存
            int stockNumber = Products.GetProductStockNumberByPid(pid);
            if (stockNumber < buyCount)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                    case 1:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                    case 2:
                        return AjaxResult("stockout", "商品库存不足");
                    default:
                        return PromptView(Url.Action("Product", "Catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                }
            }

            //购物车中已经存在的商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            if (Orders.GetCommonOrderProductByPid(pid, orderProductList) == null)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                {
                    switch (scSubmitType)
                    {
                        case 2:
                            return AjaxResult("full", "购物车已满");
                        default:
                            return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 0 }, { "id", pid } });
                    }
                }
            }


            //将商品添加到购物车
            Orders.AddProductToCart(ref orderProductList, buyCount, partProductInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now, WorkContext.IP);
            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(Orders.SumOrderProductCount(orderProductList));

            switch (scSubmitType)
            {
                case 0:
                    return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 0 }, { "id", pid } });
                case 1:
                    return RedirectToAction("Index");
                case 2:
                    return AjaxResult("success", "添加成功");
                default:
                    return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 0 }, { "id", pid } });
            }
        }

        /// <summary>
        /// 修改购物车中商品数量
        /// </summary>
        public ActionResult ChangePruductCount()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pid = WebHelper.GetQueryInt("pid");//商品id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //对应商品
            OrderProductInfo orderProductInfo = Orders.GetCommonOrderProductByPid(pid, orderProductList);

            if (orderProductInfo != null)//当商品已经存在
            {
                if (buyCount < 1)//当购买数量小于1时，删除此商品
                {
                    Orders.DeleteCartProduct(ref orderProductList, orderProductInfo);
                }
                else
                {
                    if (buyCount != orderProductInfo.BuyCount)
                        Orders.AddExistProductToCart(ref orderProductList, orderProductInfo, buyCount - orderProductInfo.BuyCount, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
                }
            }

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            return View("AjaxIndex", model);
        }

        /// <summary>
        /// 删除购物车中商品
        /// </summary>
        public ActionResult DelPruduct()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pid = WebHelper.GetQueryInt("pid");//商品id
            int pos = WebHelper.GetQueryInt("pos");//位置

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //对应商品
            OrderProductInfo orderProductInfo = Orders.GetCommonOrderProductByPid(pid, orderProductList);

            if (orderProductInfo != null)
                Orders.DeleteCartProduct(ref orderProductList, orderProductInfo);

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            if (pos == 0)
                return View("AjaxIndex", model);
            else
                return View("Snsp", model);
        }

        /// <summary>
        /// 添加套装到购物车
        /// </summary>
        public ActionResult AddSuit()
        {
            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            int scSubmitType = WorkContext.MallConfig.SCSubmitType;//购物车的提交方式

            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                    case 1:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                    case 2:
                        return AjaxResult("nologin", "请先登录");
                    default:
                        return Redirect(Url.Action("login", "Account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                }
            }

            //购买数量不能小于1
            if (buyCount < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("Suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                    case 1:
                        return PromptView(Url.Action("Suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                    case 2:
                        return AjaxResult("buycountmin", "请填写购买数量");
                    default:
                        return PromptView(Url.Action("Suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                }
            }

            //获得套装促销活动
            SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(pmId, DateTime.Now);
            //套装促销活动不存在时
            if (suitPromotionInfo == null)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "套装不存在");
                    case 1:
                        return PromptView("/", "套装不存在");
                    case 2:
                        return AjaxResult("nosuit", "请选择套装");
                    default:
                        return PromptView("/", "套装不存在");
                }
            }

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(suitPromotionInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "套装不存在");
                    case 1:
                        return PromptView("/", "套装不存在");
                    case 2:
                        return AjaxResult("nosuit", "请选择套装");
                    default:
                        return PromptView("/", "套装不存在");
                }
            }

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            if (Orders.GetSuitOrderProductList(pmId, orderProductList, true).Count == 0)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                {
                    switch (scSubmitType)
                    {
                        case 2:
                            return AjaxResult("full", "购物车已满");
                        default:
                            return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 1 }, { "id", pmId } });
                    }
                }
            }

            //套装商品列表
            DataTable suitProductList = Promotions.GetSuitProductList(suitPromotionInfo.PmId);
            if (suitProductList.Rows.Count < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                    case 1:
                        return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                    case 2:
                        return AjaxResult("noproduct", "套装中没有商品");
                    default:
                        return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                }
            }

            //套装商品id列表
            StringBuilder pidList = new StringBuilder();
            foreach (DataRow row in suitProductList.Rows)
                pidList.AppendFormat("{0},", row["pid"]);
            pidList.Remove(pidList.Length - 1, 1);

            //套装商品库存列表
            List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList.ToString());
            foreach (ProductStockInfo item in productStockList)
            {
                if (item.Number < buyCount)
                {
                    switch (scSubmitType)
                    {
                        case 0:
                            return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                        case 1:
                            return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                        case 2:
                            return AjaxResult("stockout", item.Pid.ToString());
                        default:
                            return PromptView(Url.Action("suit", "Catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                    }
                }
            }

            Orders.AddSuitToCart(ref orderProductList, suitProductList, suitPromotionInfo, buyCount, WorkContext.Sid, WorkContext.Uid, DateTime.Now, WorkContext.IP);
            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(Orders.SumOrderProductCount(orderProductList));

            switch (scSubmitType)
            {
                case 0:
                    return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 1 }, { "id", pmId } });
                case 1:
                    return RedirectToAction("Index");
                case 2:
                    return AjaxResult("success", "添加成功");
                default:
                    return RedirectToAction("AddSuccess", new RouteValueDictionary { { "type", 1 }, { "id", pmId } });
            }
        }

        /// <summary>
        /// 修改购物车中套装数量
        /// </summary>
        public ActionResult ChangeSuitCount()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //套装商品列表
            List<OrderProductInfo> suitOrderProductList = Orders.GetSuitOrderProductList(pmId, orderProductList, true);

            if (suitOrderProductList.Count > 0)//当套装已经存在
            {
                if (buyCount < 1)//当购买数量小于1时，删除此套装
                {
                    Orders.DeleteCartSuit(ref orderProductList, pmId);
                }
                else
                {
                    int oldBuyCount = suitOrderProductList[0].RealCount / suitOrderProductList[0].ExtCode9;
                    if (buyCount != oldBuyCount)
                        Orders.AddExistSuitToCart(ref orderProductList, suitOrderProductList, buyCount - oldBuyCount, DateTime.Now);
                }
            }

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            return View("AjaxIndex", model);
        }

        /// <summary>
        /// 删除购物车中套装
        /// </summary>
        public ActionResult DelSuit()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int pos = WebHelper.GetQueryInt("pos");//位置

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            Orders.DeleteCartSuit(ref orderProductList, pmId);

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            if (pos == 0)
                return View("AjaxIndex", model);
            else
                return View("Snsp", model);
        }

        /// <summary>
        /// 获得满赠赠品
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            //获得满赠赠品列表
            List<PartProductInfo> fullSendPresentList = Promotions.GetFullSendPresentList(pmId);

            return Content(JSON.ToJSON(fullSendPresentList));
        }

        /// <summary>
        /// 添加满赠到购物车
        /// </summary>
        public ActionResult AddFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            int pid = WebHelper.GetQueryInt("pid");//商品id

            //添加的商品
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("noproduct", "请选择商品");

            //商品库存
            int stockNumber = Products.GetProductStockNumberByPid(pid);
            if (stockNumber < 1)
                return AjaxResult("stockout", "商品库存不足");

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);

            //满赠主商品列表
            List<OrderProductInfo> fullSendMainOrderProductList = Orders.GetFullSendMainOrderProductList(pmId, orderProductList);
            if (fullSendMainOrderProductList.Count < 1)
                return AjaxResult("nolimit", "不符合活动条件");
            decimal amount = Orders.SumOrderProductAmount(fullSendMainOrderProductList);
            if (fullSendMainOrderProductList[0].ExtCode8 > amount)
                return AjaxResult("nolimit", "不符合活动条件");

            if (!Promotions.IsExistFullSendProduct(pmId, pid, 1))
                return AjaxResult("nofullsendproduct", "此商品不是满赠商品");

            //赠送商品
            OrderProductInfo fullSendMinorOrderProductInfo = Orders.GetFullSendMinorOrderProduct(pmId, orderProductList);
            if (fullSendMinorOrderProductInfo != null)
            {
                if (fullSendMinorOrderProductInfo.Pid != pid)
                    Orders.DeleteCartFullSend(ref orderProductList, fullSendMinorOrderProductInfo);
                else
                    return AjaxResult("exist", "此商品已经添加");
            }

            //添加满赠商品
            Orders.AddFullSendToCart(ref orderProductList, partProductInfo, fullSendMainOrderProductList[0].ExtCode7, fullSendMainOrderProductList[0].ExtCode8, fullSendMainOrderProductList[0].ExtCode9, WorkContext.Sid, WorkContext.Uid, DateTime.Now, WorkContext.IP);

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            return View("AjaxIndex", model);
        }

        /// <summary>
        /// 删除购物车中满赠
        /// </summary>
        public ActionResult DelFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            int pos = WebHelper.GetQueryInt("pos");//位置

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Orders.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //删除满赠
            Orders.DeleteCartFullSend(ref orderProductList, pmId);

            //商品总数量
            int totalCount = Orders.SumOrderProductCount(orderProductList);
            //商品合计
            decimal productAmount = Orders.SumOrderProductAmount(orderProductList);
            //满减折扣
            decimal fullCutDiscount = Orders.SumFullCut(orderProductList);
            //订单合计
            decimal orderAmount = productAmount - fullCutDiscount;
            //整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = Orders.TidyMallOrderProductList(orderProductList);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCutDiscount = fullCutDiscount,
                OrderAmount = orderAmount,
                OrderProductList = tidiedOrderProductList
            };

            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(totalCount);

            if (pos == 0)
                return View("AjaxIndex", model);
            else
                return View("Snsp", model);
        }

        /// <summary>
        /// 购物车添加成功提示
        /// </summary>
        public ActionResult AddSuccess()
        {
            int id = WebHelper.GetQueryInt("id");//项id
            int type = WebHelper.GetQueryInt("type");//项类型

            CartAddSuccessModel model = new CartAddSuccessModel
            {
                ItemId = id,
                ItemType = type
            };
            return View(model);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        public ActionResult Clear()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            Orders.ClearCart(WorkContext.Uid, WorkContext.Sid);
            //将购物车中商品数量写入cookie
            Orders.SetCartProductCountCookie(0);

            return AjaxResult("success", "清空完成");
        }
    }
}
