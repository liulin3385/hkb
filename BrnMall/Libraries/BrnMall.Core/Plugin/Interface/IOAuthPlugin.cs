using System;
using System.Web.Routing;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall开放授权插件接口
    /// </summary>
    public interface IOAuthPlugin : IPlugin
    {
        /// <summary>
        /// 登陆控制器
        /// </summary>
        string LoginController { get; }

        /// <summary>
        /// 登陆动作方法
        /// </summary>
        string LoginAction { get; }

        /// <summary>
        /// 登陆路由数据
        /// </summary>
        RouteValueDictionary LoginRouteValues { get; }
    }
}
