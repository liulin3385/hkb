using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.PayPlugin.Alipay
{
    /// <summary>
    /// 插件服务类
    /// </summary>
    public class PluginService : IPayPlugin
    {
        /// <summary>
        /// 插件配置控制器
        /// </summary>
        public string ConfigController
        {
            get { return "AdminAlipay"; }
        }
        /// <summary>
        /// 插件配置动作方法
        /// </summary>
        public string ConfigAction
        {
            get { return "Config"; }
        }

        /// <summary>
        /// 获得支付手续费
        /// </summary>
        /// <param name="productAmount">商品合计</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="partUserInfo">购买用户</param>
        /// <returns></returns>
        public decimal GetPayFee(decimal productAmount, DateTime buyTime, PartUserInfo partUserInfo)
        {
            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            if (productAmount >= pluginSetInfo.FreeMoney)
                return 0M;
            else
                return pluginSetInfo.PayFee;
        }

        /// <summary>
        /// 获得付款请求的url
        /// </summary>
        /// <param name="orderList">订单列表</param>
        /// <returns></returns>
        public static string GetRequestUrl(List<OrderInfo> orderList)
        {
            string oidList = "";
            decimal allSurplusMoney = 0M;
            foreach (OrderInfo orderInfo in orderList)
            {
                oidList += orderInfo.Oid + ",";
                allSurplusMoney += orderInfo.SurplusMoney;
            }
            oidList = oidList.TrimEnd(',');

            //支付类型，必填，不能修改
            string paymentType = "1";

            //服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数
            string notifyUrl = string.Format("http://{0}/Alipay/Notify", BMAConfig.MallConfig.SiteUrl);
            //页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/
            string returnUrl = string.Format("http://{0}/Alipay/Return", BMAConfig.MallConfig.SiteUrl);

            //收款支付宝帐户
            string sellerEmail = AlipayConfig.Seller;
            //合作者身份ID
            string partner = AlipayConfig.Partner;
            //交易安全检验码
            string key = AlipayConfig.Key;

            //商户订单号
            string outTradeNo = oidList;
            //订单名称
            string subject = "";
            //付款金额
            string totalFee = allSurplusMoney.ToString();
            //订单描述
            string body = "";

            //防钓鱼时间戳,若要使用请调用类文件submit中的query_timestamp函数
            string antiPhishingKey = "";
            //客户端的IP地址,非局域网的外网IP地址，如：221.0.0.1
            string exterInvokeIP = "";

            //把请求参数打包成数组
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("partner", partner);
            parms.Add("_input_charset", key);
            parms.Add("service", "create_direct_pay_by_user");
            parms.Add("payment_type", paymentType);
            parms.Add("notify_url", notifyUrl);
            parms.Add("return_url", returnUrl);
            parms.Add("seller_email", sellerEmail);
            parms.Add("out_trade_no", outTradeNo);
            parms.Add("subject", subject);
            parms.Add("total_fee", totalFee);
            parms.Add("body", body);
            parms.Add("anti_phishing_key", antiPhishingKey);
            parms.Add("exter_invoke_ip", exterInvokeIP);

            return AlipaySubmit.BuildRequestUrl(parms, AlipayConfig.Gateway, AlipayConfig.InputCharset, AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code);
        }
    }
}
