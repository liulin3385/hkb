using System;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall支付插件接口
    /// </summary>
    public interface IPayPlugin : IPlugin
    {
        /// <summary>
        /// 获得支付手续费
        /// </summary>
        /// <param name="productAmount">商品合计</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="partUserInfo">购买用户</param>
        /// <returns></returns>
        decimal GetPayFee(decimal productAmount, DateTime buyTime, PartUserInfo partUserInfo);
    }
}
