using System;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// banner操作管理类
    /// </summary>
    public class Banners
    {
        /// <summary>
        /// 获得首页banner列表
        /// </summary>
        /// <returns></returns>
        public static BannerInfo[] GetHomeBannerList()
        {
            BannerInfo[] bannerList = BrnMall.Core.BMACache.Get(CacheKeys.MALL_BANNER_HOMELIST) as BannerInfo[];
            if (bannerList == null)
            {
                bannerList = BrnMall.Data.Banners.GetHomeBannerList(DateTime.Now);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_BANNER_HOMELIST, bannerList);
            }
            return bannerList;
        }
    }
}
