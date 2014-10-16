using System;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 后台导航栏操作管理类
    /// </summary>
    public class AdminNavs : Navs
    {
        /// <summary>
        /// 创建导航栏
        /// </summary>
        public static void CreateNav(NavInfo navInfo)
        {
            BrnMall.Data.Navs.CreateNav(navInfo);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_LIST);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_MAINLIST);
        }

        /// <summary>
        /// 删除导航栏
        /// </summary>
        /// <param name="id">导航栏id</param>
        public static int DeleteNavById(int id)
        {
            if (GetSubNavList(id).Count > 0)
                return 0;

            BrnMall.Data.Navs.DeleteNavById(id);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_LIST);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_MAINLIST);
            return 1;
        }

        /// <summary>
        /// 更新导航栏
        /// </summary>
        public static void UpdateNav(NavInfo navInfo)
        {
            BrnMall.Data.Navs.UpdateNav(navInfo);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_LIST);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_NAV_MAINLIST);
        }
    }
}
