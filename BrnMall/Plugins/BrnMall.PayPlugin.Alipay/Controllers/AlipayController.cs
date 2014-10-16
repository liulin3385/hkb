using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;

using BrnMall.PayPlugin.Alipay;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 前台支付宝控制器类
    /// </summary>
    public class AlipayController : Controller
    {
        /// <summary>
        /// 返回调用
        /// </summary>
        public ActionResult Return()
        {
            SortedDictionary<string, string> paras = AlipayCore.GetRequestGet();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.QueryString["notify_id"], Request.QueryString["sign"], AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.VeryfyUrl, AlipayConfig.Partner);

                if (verifyResult && (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    string out_trade_no = Request.QueryString["out_trade_no"];//商户订单号
                    string tradeSN = Request.QueryString["trade_no"];//支付宝交易号
                    decimal tradeMoney = TypeHelper.StringToDecimal(Request.QueryString["total_fee"]);//交易金额
                    DateTime tradeTime = TypeHelper.StringToDateTime(Request.QueryString["notify_time"]);//交易时间

                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(out_trade_no))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    if (orderList.Count > 0 && allSurplusMoney <= tradeMoney)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying)
                            {
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, DateTime.Now);
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = tradeTime,
                                    ActionDes = "你使用支付宝支付订单成功，支付宝交易号为:" + tradeSN
                                });
                            }
                        }
                    }

                    return RedirectToAction("PaySuccess", "Order", new RouteValueDictionary { { "oidList", out_trade_no } });
                }
                else//验证失败
                {
                    return new EmptyResult();
                }
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// 通知调用
        /// </summary>
        public ActionResult Notify()
        {
            SortedDictionary<string, string> paras = AlipayCore.GetRequestPost();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.QueryString["notify_id"], Request.QueryString["sign"], AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.VeryfyUrl, AlipayConfig.Partner);

                if (verifyResult && (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    string out_trade_no = Request.QueryString["out_trade_no"];//商户订单号
                    string tradeSN = Request.QueryString["trade_no"];//支付宝交易号
                    decimal tradeMoney = TypeHelper.StringToDecimal(Request.QueryString["total_fee"]);//交易金额
                    DateTime tradeTime = TypeHelper.StringToDateTime(Request.QueryString["gmt_payment"]);//交易时间

                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(out_trade_no))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    if (orderList.Count > 0 && allSurplusMoney <= tradeMoney)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying)
                            {
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, DateTime.Now);
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = tradeTime,
                                    ActionDes = "你使用支付宝支付订单成功，支付宝交易号为:" + tradeSN
                                });
                            }
                        }
                    }

                    return new EmptyResult();
                }
                else//验证失败
                {
                    return new EmptyResult();
                }
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
