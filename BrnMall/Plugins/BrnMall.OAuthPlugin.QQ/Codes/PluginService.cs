using System;
using System.Web.Routing;

using BrnMall.Core;

namespace BrnMall.OAuthPlugin.QQ
{
    /// <summary>
    /// 插件服务类
    /// </summary>
    public class PluginService : IOAuthPlugin
    {
        /// <summary>
        /// 插件配置控制器
        /// </summary>
        public string ConfigController
        {
            get { return "AdminQQOAuth"; }
        }
        /// <summary>
        /// 插件配置动作方法
        /// </summary>
        public string ConfigAction
        {
            get { return "Config"; }
        }

        /// <summary>
        /// 登陆控制器
        /// </summary>
        public string LoginController
        {
            get { return "QQOAuth"; }
        }
        /// <summary>
        /// 登陆动作方法
        /// </summary>
        public string LoginAction
        {
            get { return "Login"; }
        }
        /// <summary>
        /// 登陆路由数据
        /// </summary>
        public RouteValueDictionary LoginRouteValues
        {
            get { return new RouteValueDictionary() { { "area", null } }; }
        }
    }
}
