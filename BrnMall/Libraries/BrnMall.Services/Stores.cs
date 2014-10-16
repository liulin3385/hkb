﻿using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 店铺操作管理类
    /// </summary>
    public class Stores
    {
        /// <summary>
        /// 获得店铺
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreInfo GetStoreById(int storeId)
        {
            if (storeId < 1) return null;
            return BrnMall.Data.Stores.GetStoreById(storeId);
        }

        /// <summary>
        /// 根据店铺名称得到店铺id
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <returns></returns>
        public static int GetStoreIdByName(string storeName)
        {
            return BrnMall.Data.Stores.GetStoreIdByName(storeName);
        }





        /// <summary>
        /// 获得店长
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreKeeperInfo GetStoreKeeperById(int storeId)
        {
            return BrnMall.Data.Stores.GetStoreKeeperById(storeId);
        }





        /// <summary>
        /// 获得店铺分类列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static List<StoreClassInfo> GetStoreClassList(int storeId)
        {
            List<StoreClassInfo> storeClassList = BrnMall.Core.BMACache.Get(CacheKeys.MALL_STORE_CLASSLIST + storeId) as List<StoreClassInfo>;
            if (storeClassList == null)
            {
                storeClassList = new List<StoreClassInfo>();
                List<StoreClassInfo> sourceStoreClassList = BrnMall.Data.Stores.GetStoreClassList(storeId);
                CreateStoreClassTree(sourceStoreClassList, storeClassList, 0);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_STORE_CLASSLIST + storeId, storeClassList);
            }
            return storeClassList;
        }

        /// <summary>
        /// 递归创建店铺分类列表树
        /// </summary>
        private static void CreateStoreClassTree(List<StoreClassInfo> sourceStoreClassList, List<StoreClassInfo> resultStoreClassList, int parentId)
        {
            foreach (StoreClassInfo storeClassInfo in sourceStoreClassList)
            {
                if (storeClassInfo.ParentId == parentId)
                {
                    resultStoreClassList.Add(storeClassInfo);
                    CreateStoreClassTree(sourceStoreClassList, resultStoreClassList, storeClassInfo.StoreCid);
                }
            }
        }

        /// <summary>
        /// 获得店铺分类id
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="name">店铺分类名称</param>
        /// <returns></returns>
        public static int GetStoreCidByStoreIdAndName(int storeId, string name)
        {
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.Name == name)
                    return storeClassInfo.StoreCid;
            }
            return 0;
        }

        /// <summary>
        /// 获得店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="name">店铺分类名称</param>
        /// <returns></returns>
        public static StoreClassInfo GetStoreClassByStoreIdAndName(int storeId, string name)
        {
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.Name == name)
                    return storeClassInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static StoreClassInfo GetStoreClassByStoreIdAndStoreCid(int storeId, int storeCid)
        {
            if (storeCid < 1) return null;
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.StoreCid == storeCid)
                    return storeClassInfo;
            }

            return null;
        }

        /// <summary>
        /// 判断是否有子店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static bool IsHasChildStoreClass(int storeId, int storeCid)
        {
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.ParentId == storeCid)
                    return true;
            }
            return false;
        }





        /// <summary>
        /// 获得店铺配送模板
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static StoreShipTemplateInfo GetStoreShipTemplateById(int storeSTid)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = BrnMall.Core.BMACache.Get(CacheKeys.MALL_STORE_SHIPTEMPLATEINFO + storeSTid) as StoreShipTemplateInfo;
            if (storeShipTemplateInfo == null)
            {
                storeShipTemplateInfo = BrnMall.Data.Stores.GetStoreShipTemplateById(storeSTid);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_STORE_SHIPTEMPLATEINFO + storeSTid, storeShipTemplateInfo);
            }

            return storeShipTemplateInfo;
        }





        /// <summary>
        /// 获得店铺配送费用
        /// </summary>
        /// <param name="storeSTId">店铺模板id</param>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static StoreShipFeeInfo GetStoreShipFeeByStoreSTidAndRegion(int storeSTId, int provinceId, int cityId)
        {
            return BrnMall.Data.Stores.GetStoreShipFeeByStoreSTidAndRegion(storeSTId, provinceId, cityId);
        }
    }
}
