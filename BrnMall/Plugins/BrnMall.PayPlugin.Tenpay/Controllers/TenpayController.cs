using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.PayPlugin.Tenpay;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 前台财付通控制器类
    /// </summary>
    public class TenpayController : Controller
    {
        /// <summary>
        /// 返回调用
        /// </summary>
        public ActionResult Return()
        {
            System.Web.HttpContext Context = System.Web.HttpContext.Current;

            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(Context);
            resHandler.setKey(TenpayUtil.tenpay_key);

            //判断签名
            if (resHandler.isTenpaySign())
            {
                //支付结果
                string trade_state = resHandler.getParameter("trade_state");
                //交易模式，1即时到账，2中介担保
                string trade_mode = resHandler.getParameter("trade_mode");

                if ("1".Equals(trade_mode) && "0".Equals(trade_state))
                {
                    string out_trade_no = resHandler.getParameter("out_trade_no");//商户订单号
                    string tradeSN = resHandler.getParameter("transaction_id");//财付通订单号
                    decimal tradeMoney = TypeHelper.StringToDecimal(resHandler.getParameter("total_fee")) / 100;//金额,以分为单位
                    DateTime tradeTime = DateTime.Now;//交易时间

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
                                    ActionDes = "你使用财付通支付订单成功，财付通交易号为:" + tradeSN
                                });
                            }
                        }
                    }

                    return RedirectToAction("PaySuccess", "Order", new RouteValueDictionary { { "oidList", out_trade_no } });
                }
                else
                {
                    return Content("财付通支付失败");
                }
            }
            else
            {
                return Content("认证签名失败");
            }
        }

        /// <summary>
        /// 通知调用
        /// </summary>
        public ActionResult Notify()
        {
            System.Web.HttpContext Context = System.Web.HttpContext.Current;

            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(Context);
            resHandler.setKey(TenpayUtil.tenpay_key);

            //判断签名
            if (resHandler.isTenpaySign())
            {
                ///通知id
                string notify_id = resHandler.getParameter("notify_id");
                //通过通知ID查询，确保通知来至财付通
                //创建查询请求
                RequestHandler queryReq = new RequestHandler(Context);
                queryReq.init();
                queryReq.setKey(TenpayUtil.tenpay_key);
                queryReq.setGateUrl("https://gw.tenpay.com/gateway/simpleverifynotifyid.xml");
                queryReq.setParameter("partner", TenpayUtil.bargainor_id);
                queryReq.setParameter("notify_id", notify_id);

                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                httpClient.setTimeOut(5);
                //设置请求内容
                httpClient.setReqContent(queryReq.getRequestURL());
                //后台调用
                if (httpClient.call())
                {
                    //设置结果参数
                    ClientResponseHandler queryRes = new ClientResponseHandler();
                    queryRes.setContent(httpClient.getResContent());
                    queryRes.setKey(TenpayUtil.tenpay_key);
                    //判断签名及结果
                    //只有签名正确,retcode为0，trade_state为0才是支付成功
                    if (queryRes.isTenpaySign())
                    {
                        //支付结果
                        string trade_state = resHandler.getParameter("trade_state");
                        //交易模式，1即时到帐 2中介担保
                        string trade_mode = resHandler.getParameter("trade_mode");
                        #region
                        //判断签名及结果
                        if ("0".Equals(queryRes.getParameter("retcode")))
                        {
                            if ("1".Equals(trade_mode) && "0".Equals(trade_state))
                            {
                                string out_trade_no = queryRes.getParameter("out_trade_no");//商户订单号
                                string tradeSN = queryRes.getParameter("transaction_id");//财付通订单号
                                decimal tradeMoney = TypeHelper.StringToDecimal(queryRes.getParameter("total_fee"));//金额,以分为单位
                                DateTime tradeTime = DateTime.Now;//交易时间

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
                                                ActionDes = "你使用财付通支付订单成功，财付通交易号为:" + tradeSN
                                            });
                                        }
                                    }
                                }

                                return new EmptyResult();
                            }
                            else
                            {
                                return Content("交易失败");
                            }
                        }
                        else
                        {
                            return Content("查询验证签名失败或id验证失败");
                        }
                        #endregion
                    }
                    else
                    {
                        return Content("通知ID查询签名验证失败");
                    }
                }
                else
                {
                    return Content("后台调用通信失败");
                }
            }
            else
            {
                return Content("签名验证失败");
            }
        }
    }
}
