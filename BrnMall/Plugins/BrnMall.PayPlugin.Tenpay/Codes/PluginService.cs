using System;
using System.Web;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.PayPlugin.Tenpay
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
            get { return "AdminTenpay"; }
        }
        /// <summary>
        /// 插件配置动作方法
        /// </summary>
        public string ConfigAction
        {
            get { return "Config"; }
        }

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

            string sp_billno = oidList.ToString();
            string product_name = "";
            string remarkexplain = "";
            decimal money = allSurplusMoney;

            HttpContext Context = HttpContext.Current;

            //创建RequestHandler实例
            RequestHandler reqHandler = new RequestHandler(Context);
            //初始化
            reqHandler.init();
            //设置密钥
            reqHandler.setKey(TenpayUtil.tenpay_key);
            reqHandler.setGateUrl("https://gw.tenpay.com/gateway/pay.htm");




            //-----------------------------
            //设置支付参数
            //-----------------------------
            reqHandler.setParameter("partner", TenpayUtil.bargainor_id);		        //商户号
            reqHandler.setParameter("out_trade_no", sp_billno);		//商家订单号
            reqHandler.setParameter("total_fee", (money * 100).ToString().Replace(".00", ""));			        //商品金额,以分为单位
            reqHandler.setParameter("return_url", TenpayUtil.tenpay_return);		    //交易完成后跳转的URL
            reqHandler.setParameter("notify_url", TenpayUtil.tenpay_notify);		    //接收财付通通知的URL
            reqHandler.setParameter("body", "商品描述");	                    //商品描述
            reqHandler.setParameter("bank_type", "DEFAULT");		    //银行类型(中介担保时此参数无效)
            reqHandler.setParameter("spbill_create_ip", Context.Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
            reqHandler.setParameter("fee_type", "1");                    //币种，1人民币
            reqHandler.setParameter("subject", "商品名称");              //商品名称(中介交易时必填)


            //系统可选参数
            reqHandler.setParameter("sign_type", "MD5");
            reqHandler.setParameter("service_version", "1.0");
            reqHandler.setParameter("input_charset", "UTF-8");
            reqHandler.setParameter("sign_key_index", "1");

            //业务可选参数

            reqHandler.setParameter("attach", "");                      //附加数据，原样返回
            reqHandler.setParameter("product_fee", "0");                 //商品费用，必须保证transport_fee + product_fee=total_fee
            reqHandler.setParameter("transport_fee", "0");               //物流费用，必须保证transport_fee + product_fee=total_fee
            reqHandler.setParameter("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));            //订单生成时间，格式为yyyymmddhhmmss
            reqHandler.setParameter("time_expire", "");                 //订单失效时间，格式为yyyymmddhhmmss
            reqHandler.setParameter("buyer_id", "");                    //买方财付通账号
            reqHandler.setParameter("goods_tag", "");                   //商品标记
            reqHandler.setParameter("trade_mode", "1");                 //交易模式，1即时到账(默认)，2中介担保，3后台选择（买家进支付中心列表选择）
            reqHandler.setParameter("transport_desc", "");              //物流说明
            reqHandler.setParameter("trans_type", "1");                  //交易类型，1实物交易，2虚拟交易
            reqHandler.setParameter("agentid", "");                     //平台ID
            reqHandler.setParameter("agent_type", "");                  //代理模式，0无代理(默认)，1表示卡易售模式，2表示网店模式
            reqHandler.setParameter("seller_id", "");                   //卖家商户号，为空则等同于partner



            //获取请求带参数的url
            return reqHandler.getRequestURL();
        }
    }
}
