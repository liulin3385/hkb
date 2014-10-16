using System;

namespace BrnMall.Web.MallAdmin
{
    public class AreaRegistration : System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MallAdmin";
            }
        }

        public override void RegisterArea(System.Web.Mvc.AreaRegistrationContext context)
        {
            //此路由不能删除
            context.MapRoute("MallAdmin_default",
                              "MallAdmin/{controller}/{action}",
                              new { controller = "Home", action = "Index", area = "MallAdmin" },
                              new[] { "BrnMall.Web.MallAdmin.Controllers" });

        }
    }
}
