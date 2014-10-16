using System;

using BrnMall.Core;

namespace BrnMall.PayPlugin.ChinaBank
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
            get { return "AdminChinaBank"; }
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
    }
}
