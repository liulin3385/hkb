using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.PayPlugin.ChinaBank;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 前台网银在线控制器类
    /// </summary>
    public class ChinaBankController : Controller
    {
        /// <summary>
        /// 返回调用
        /// </summary>
        public ActionResult Return()
        {
            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();

            // 如果您还没有设置MD5密钥请登陆我们为您提供商户后台，地址：https://merchant3.chinabank.com.cn/
            // 登陆后在上面的导航栏里可能找到“B2C”，在二级导航栏里有“MD5密钥设置”
            // 建议您设置一个16位以上的密钥或更高，密钥最多64位，但设置16位已经足够了
            string key = pluginSetInfo.Key;

            string v_oid = WebHelper.GetRequestString("v_oid");
            string v_pstatus = WebHelper.GetRequestString("v_pstatus");
            string v_pstring = WebHelper.GetRequestString("v_pstring");
            string v_pmode = WebHelper.GetRequestString("v_pmode");
            string v_md5str = WebHelper.GetRequestString("v_md5str");
            decimal v_amount = TypeHelper.StringToDecimal(WebHelper.GetRequestString("v_amount"));
            string v_moneytype = WebHelper.GetRequestString("v_moneytype");
            string remark1 = WebHelper.GetRequestString("remark1");
            string remark2 = WebHelper.GetRequestString("remark2");

            string str = v_oid + v_pstatus + v_amount + v_moneytype + key;

            str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToUpper();

            if (str == v_md5str)
            {
                if (v_pstatus.Equals("20"))
                {
                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(v_oid))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    //支付成功
                    if (orderList.Count > 0 && allSurplusMoney <= v_amount)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying)
                            {
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, "", DateTime.Now);
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = DateTime.Now,
                                    ActionDes = "你使用网银在线支付订单成功，支付银行为:" + v_pmode
                                });
                            }
                        }
                    }

                    return RedirectToAction("PaySuccess", "Order", new RouteValueDictionary { { "oidList", v_oid } });
                }
                else
                {
                    return Content("支付失败");
                }
            }
            else
            {
                return Content("校验失败，数据可疑");
            }
        }

        /// <summary>
        /// 通知调用
        /// </summary>
        public ActionResult Notify()
        {
            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();

            // 如果您还没有设置MD5密钥请登陆我们为您提供商户后台，地址：https://merchant3.chinabank.com.cn/
            // 登陆后在上面的导航栏里可能找到“B2C”，在二级导航栏里有“MD5密钥设置”
            // 建议您设置一个16位以上的密钥或更高，密钥最多64位，但设置16位已经足够了
            string key = pluginSetInfo.Key;

            string v_oid = WebHelper.GetRequestString("v_oid");
            string v_pstatus = WebHelper.GetRequestString("v_pstatus");
            string v_pstring = WebHelper.GetRequestString("v_pstring");
            string v_pmode = WebHelper.GetRequestString("v_pmode");
            string v_md5str = WebHelper.GetRequestString("v_md5str");
            decimal v_amount = TypeHelper.StringToDecimal(WebHelper.GetRequestString("v_amount"));
            string v_moneytype = WebHelper.GetRequestString("v_moneytype");
            string remark1 = WebHelper.GetRequestString("remark1");
            string remark2 = WebHelper.GetRequestString("remark2");

            string str = v_oid + v_pstatus + v_amount + v_moneytype + key;

            str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToUpper();

            if (str == v_md5str)
            {
                if (v_pstatus.Equals("20"))
                {
                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(v_oid))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    //支付成功
                    if (orderList.Count > 0 && allSurplusMoney <= v_amount)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying)
                            {
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, "", DateTime.Now);
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = DateTime.Now,
                                    ActionDes = "你使用网银在线支付订单成功，支付银行为:" + v_pmode
                                });
                            }
                        }
                    }

                }
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }
    }
}
