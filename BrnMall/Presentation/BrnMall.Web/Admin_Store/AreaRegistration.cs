using System;

namespace BrnMall.Web.StoreAdmin
{
    public class AreaRegistration : System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StoreAdmin";
            }
        }

        public override void RegisterArea(System.Web.Mvc.AreaRegistrationContext context)
        {
            //此路由不能删除
            context.MapRoute("StoreAdmin_default",
                              "StoreAdmin/{controller}/{action}",
                              new { controller = "Home", action = "Index", area = "StoreAdmin" },
                              new[] { "BrnMall.Web.StoreAdmin.Controllers" });

        }
    }
}
