﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //商品路由
            routes.MapRoute("Product",
                            "{pid}.html",
                            new { controller = "Catalog", action = "Product" },
                            new[] { "BrnMall.Web.Controllers" });
            //分类路由
            routes.MapRoute("Category",
                            "list/{filterAttr}-{cateId}-{brandId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "Catalog", action = "Category" },
                            new[] { "BrnMall.Web.Controllers" });
            //分类路由
            routes.MapRoute("ShortCategory",
                            "list/{cateId}.html",
                            new { controller = "Catalog", action = "Category" },
                            new[] { "BrnMall.Web.Controllers" });
            //商城搜索路由
            routes.MapRoute("MallSearch",
                            "search",
                            new { controller = "Catalog", action = "Search" },
                            new[] { "BrnMall.Web.Controllers" });
            //品牌路由
            routes.MapRoute("Brand",
                            "brand/{filterAttr}-{brandId}-{cateId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "Catalog", action = "Brand" },
                            new[] { "BrnMall.Web.Controllers" });
            //品牌路由
            routes.MapRoute("ShortBrand",
                            "brand/{brandId}.html",
                            new { controller = "Catalog", action = "Brand" },
                            new[] { "BrnMall.Web.Controllers" });
            //店铺路由
            routes.MapRoute("Store",
                            "store/{storeId}.html",
                            new { controller = "Store", action = "Index" },
                            new[] { "BrnMall.Web.Controllers" });
            //店铺分类路由
            routes.MapRoute("StoreClass",
                            "storeClass/{storeId}-{storeCid}-{startPrice}-{endPrice}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "Store", action = "Class" },
                            new[] { "BrnMall.Web.Controllers" });
            //店铺分类路由
            routes.MapRoute("ShortStoreClass",
                            "storeClass/{storeId}-{storeCid}.html",
                            new { controller = "Store", action = "Class" },
                            new[] { "BrnMall.Web.Controllers" });
            //店铺搜索路由
            routes.MapRoute("StoreSearch",
                            "searchStore",
                            new { controller = "Store", action = "Search" },
                            new[] { "BrnMall.Web.Controllers" });
            //默认路由(此路由不能删除)
            routes.MapRoute("Default",
                            "{controller}/{action}",
                            new { controller = "Home", action = "Index" },
                            new[] { "BrnMall.Web.Controllers" });
        }

        protected void Application_Start()
        {
            //将默认视图引擎替换为ThemeRazorViewEngine引擎
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemeRazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            //启动事件机制
            BMAEvent.Start();
            //服务器宕机启动后重置在线用户表
            if (Environment.TickCount > 0 && Environment.TickCount < 900000)
                OnlineUsers.ResetOnlineUserTable();
        }
    }
}