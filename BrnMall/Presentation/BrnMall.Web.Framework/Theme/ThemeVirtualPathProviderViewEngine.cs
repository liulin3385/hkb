using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using BrnMall.Core;

namespace BrnMall.Web.Framework
{
    /// <summary>
    /// 主题路径提供者视图引擎
    /// </summary>
    public abstract class ThemeVirtualPathProviderViewEngine : VirtualPathProviderViewEngine
    {
        /// <summary>
        /// 移动视图文件的修饰符
        /// </summary>
        private readonly string _mobileviewmodifier = "Mobile";

        protected ThemeVirtualPathProviderViewEngine()
        {
            //将视图文件扩展名限制为cshtml
            FileExtensions = new[] { "cshtml" };
        }

        #region 重写方法

        /// <summary>
        /// 使用指定的控制器上下文和母版视图名称来查找指定的视图
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="viewName">视图的名称</param>
        /// <param name="masterName">母版视图的名称</param>
        /// <param name="useCache">若为 true，则使用缓存的视图</param>
        /// <returns>页视图</returns>
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            //判断是否为移动访问
            bool mobile = WebHelper.IsMobile();

            //如果为移动访问，则构建一个新的视图名称
            string overrideViewName = mobile ? string.Format("{0}.{1}", viewName, _mobileviewmodifier) : viewName;
            //构建一个视图引擎结果
            ViewEngineResult result = FindThemeView(controllerContext, overrideViewName, masterName, useCache, mobile);

            //如果为移动访问且没有对应视图文件时采用原视图名称解析
            if (mobile && (result == null || result.View == null))
                result = FindThemeView(controllerContext, viewName, masterName, useCache, false);
            return result;

        }

        /// <summary>
        /// 寻找分部视图的方法
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="partialViewName">分部视图的名称</param>
        /// <param name="useCache">若为 true，则使用缓存的分部视图</param>
        /// <returns>分部视图</returns>
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            //判断是否为移动访问
            bool mobile = WebHelper.IsMobile();

            //如果为移动访问，则构建一个新的分部视图名称
            string overrideViewName = mobile ? string.Format("{0}.{1}", partialViewName, _mobileviewmodifier) : partialViewName;
            //构建一个分部视图引擎结果
            ViewEngineResult result = FindThemePartialView(controllerContext, overrideViewName, useCache, mobile);

            //如果为移动访问且没有对应分部视图文件时采用原分部视图名称解析
            if (mobile && (result == null || result.View == null))
                result = FindThemePartialView(controllerContext, partialViewName, useCache, false);
            return result;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取文件的路径
        /// </summary>
        private string GetPath(ControllerContext controllerContext, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, bool mobile, out string[] searchedLocations)
        {
            searchedLocations = null;

            //视图位置列表
            string[] locations = null;

            //获取区域
            string area = GetRouteDataTokenValue("area", controllerContext.RouteData.DataTokens).ToLower();
            //获取主题
            string theme = GetRouteDataTokenValue("theme", controllerContext.RouteData.DataTokens);

            if (string.IsNullOrWhiteSpace(area))//商城前台视图位置的处理
            {
                if (theme == "")//商城页面
                {
                    locations = new string[2] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml" };
                }
                else//店铺页面
                {
                    locations = new string[1] { "~/Themes/{2}/Views/{0}.cshtml" };
                }
            }
            else//店铺后台和商城后台视图位置的处理
            {
                //不能通过移动访问后台
                if (mobile)
                {
                    searchedLocations = new string[0];
                    return string.Empty;
                }
                if (area == "storeadmin")//访问店铺后台管理区域
                {
                    locations = new string[2] { "~/Admin_Store/Views/{1}/{0}.cshtml", "~/Admin_Store/Views/Shared/{0}.cshtml" };
                }
                else if (area == "malladmin")//访问商城后台管理区域
                {
                    locations = new string[2] { "~/Admin_Mall/Views/{1}/{0}.cshtml", "~/Admin_Mall/Views/Shared/{0}.cshtml" };
                }
            }

            //是否为特殊路径的标识
            bool flag2 = IsSpecificName(name);

            //从缓存中获取视图位置
            string cacheKey = CreateCacheKey(cacheKeyPrefix, name, flag2 ? string.Empty : controllerName, area, theme);//视图位置的缓存键
            if (useCache)
            {
                //从缓存中得到视图位置
                var cachedPath = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
                if (cachedPath != null)
                {
                    return cachedPath;
                }
            }

            //如果视图位置不在缓存中，则构建视图位置并存储到缓存中
            if (!flag2)//不是特殊路径时的操作
            {
                return GetPathFromGeneralName(controllerContext, locations, name, controllerName, theme, cacheKey, ref searchedLocations);
            }
            else//特殊路径时的操作
            {
                return GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations);
            }
        }

        /// <summary>
        /// 特殊名称时构建视图路径
        /// </summary>
        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            //将路径添加到视图位置缓存
            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, name);
            //扩展名不为“.cshtml”或者文件不存在时
            if (!IsSupportedExtension(name) || !FileExists(controllerContext, name))
            {
                searchedLocations = new string[] { name };
                return string.Empty;
            }
            return name;
        }

        /// <summary>
        /// 普通名称时构建视图路径
        /// </summary>
        private string GetPathFromGeneralName(ControllerContext controllerContext, string[] viewLocationFormats, string name, string controllerName, string theme, string cacheKey, ref string[] searchedLocations)
        {
            int count = viewLocationFormats.Length;
            searchedLocations = new string[count];

            //循环视图位置
            for (int i = 0; i < count; i++)
            {
                string path = string.Format(viewLocationFormats[i], name, controllerName, theme);
                if (FileExists(controllerContext, path))
                {
                    searchedLocations = null;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, path);
                    return path;
                }
                //将路径添加到搜索位置列表中
                searchedLocations[i] = path;
            }

            return string.Empty;
        }

        /// <summary>
        /// 创建视图位置的缓存键
        /// </summary>
        private string CreateCacheKey(string prefix, string name, string controllerName, string area, string theme)
        {
            return string.Format(":ViewCacheKey:{0}:{1}:{2}:{3}:{4}:{5}", new object[] { base.GetType().AssemblyQualifiedName, prefix, name, controllerName, area, theme });
        }

        /// <summary>
        /// 构建视图引擎结果
        /// </summary>
        private ViewEngineResult FindThemeView(ControllerContext controllerContext, string viewName, string masterName, bool useCache, bool mobile)
        {
            //视图文件路径搜索列表
            string[] strArray1 = null;
            //布局文件路径搜索列表
            string[] strArray2 = null;

            //获取控制器名称
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");

            //获取视图文件路径
            string viewPath = GetPath(controllerContext, "ViewLocationFormats", viewName, controllerName, "View", useCache, mobile, out strArray1);

            //当视图文件存在时
            if (!string.IsNullOrWhiteSpace(viewPath))
            {
                if (string.IsNullOrWhiteSpace(masterName))
                {
                    return new ViewEngineResult(CreateView(controllerContext, viewPath, string.Empty), this);
                }
                else
                {
                    //获取布局文件的路径
                    string masterPath = GetPath(controllerContext, "MasterLocationFormats", masterName, controllerName, "Master", useCache, mobile, out strArray2);
                    if (!string.IsNullOrWhiteSpace(masterPath))
                    {
                        return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
                    }
                }
            }

            //当视图文件或布局文件不存在时将搜索路径返回
            if (strArray2 == null)
            {
                return new ViewEngineResult(strArray1);
            }
            else
            {
                return new ViewEngineResult(strArray1.Union<string>(strArray2));
            }
        }

        /// <summary>
        /// 构建分部视图引擎结果
        /// </summary>
        private ViewEngineResult FindThemePartialView(ControllerContext controllerContext, string partialViewName, bool useCache, bool mobile)
        {
            //分部视图文件路径搜索列表
            string[] strArray;

            //获取控制器名称
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");

            //获取分部视图文件路径
            string partialViewPath = GetPath(controllerContext, "PartialViewLocationFormats", partialViewName, controllerName, "Partial", useCache, mobile, out strArray);

            //当分部视图文件存在时
            if (!string.IsNullOrWhiteSpace(partialViewPath))
            {
                return new ViewEngineResult(CreatePartialView(controllerContext, partialViewPath), this);
            }
            //分部视图文件不存在时返回搜索路径
            return new ViewEngineResult(strArray);
        }

        /// <summary>
        /// 判读视图名称是否以“~”或“/”开头
        /// </summary>
        private bool IsSpecificName(string name)
        {
            char ch = name[0];
            if (ch != '~')
            {
                return (ch == '/');
            }
            return true;
        }

        /// <summary>
        /// 判读文件扩展名是否为“.cshtml”
        /// </summary>
        private bool IsSupportedExtension(string path)
        {
            return path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取路由集合中指定键的值
        /// </summary>
        private string GetRouteDataTokenValue(string key, RouteValueDictionary dict)
        {
            object value = dict[key];
            if (value == null)
                return string.Empty;
            return value.ToString();
        }

        #endregion
    }
}
