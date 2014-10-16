using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 商品操作管理类
    /// </summary>
    public class Products
    {
        /// <summary>
        /// 获得商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static ProductInfo GetProductById(int pid)
        {
            if (pid < 1) return null;
            return BrnMall.Data.Products.GetProductById(pid);
        }

        /// <summary>
        /// 获得部分商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static PartProductInfo GetPartProductById(int pid)
        {
            if (pid < 1) return null;
            return BrnMall.Data.Products.GetPartProductById(pid);
        }

        /// <summary>
        /// 获得部分商品列表
        /// </summary>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetPartProductList(string pidList)
        {
            if (string.IsNullOrEmpty(pidList))
                return new List<PartProductInfo>();
            return BrnMall.Data.Products.GetPartProductList(pidList);
        }

        /// <summary>
        /// 获得商品的影子访问数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static int GetProductShadowVisitCountById(int pid)
        {
            return BrnMall.Data.Products.GetProductShadowVisitCountById(pid);
        }

        /// <summary>
        /// 更新商品的影子访问数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="visitCount">访问数量</param>
        public static void UpdateProductShadowVisitCount(int pid, int visitCount)
        {
            BrnMall.Data.Products.UpdateProductShadowVisitCount(pid, visitCount);
        }

        /// <summary>
        /// 增加商品的影子销售数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="saleCount">销售数量</param>
        public static void AddProductShadowSaleCount(int pid, int saleCount)
        {
            BrnMall.Data.Products.AddProductShadowSaleCount(pid, saleCount);
        }

        /// <summary>
        /// 增加商品的影子评价数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="starType">星星类型</param>
        public static void AddProductShadowReviewCount(int pid, int starType)
        {
            BrnMall.Data.Products.AddProductShadowReviewCount(pid, starType);
        }

        /// <summary>
        /// 获得分类商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="attrCondition">属性条件</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static List<StoreProductInfo> GetCategoryProductList(int pageSize, int pageNumber, string commonCondition, string attrCondition, int onlyStock, string sort)
        {
            return BrnMall.Data.Products.GetCategoryProductList(pageSize, pageNumber, commonCondition, attrCondition, onlyStock, sort);
        }

        /// <summary>
        /// 获得分类商品数量
        /// </summary>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="attrCondition">属性条件</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <returns></returns>
        public static int GetCategoryProductCount(string commonCondition, string attrCondition, int onlyStock)
        {
            return BrnMall.Data.Products.GetCategoryProductCount(commonCondition, attrCondition, onlyStock);
        }

        /// <summary>
        /// 搜索商城商品
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="keyword">关键词</param>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="attrCondition">属性条件</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static List<StoreProductInfo> SearchMallProducts(int pageSize, int pageNumber, string keyword, string commonCondition, string attrCondition, int onlyStock, string sort)
        {
            return BrnMall.Data.Products.SearchMallProducts(pageSize, pageNumber, keyword, commonCondition, attrCondition, onlyStock, sort);
        }

        /// <summary>
        /// 获得搜索商城商品数量
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="attrCondition">属性条件</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <returns></returns>
        public static int GetSearchMallProductCount(string keyword, string commonCondition, string attrCondition, int onlyStock)
        {
            return BrnMall.Data.Products.GetSearchMallProductCount(keyword, commonCondition, attrCondition, onlyStock);
        }

        /// <summary>
        /// 获得店铺分类商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetStoreClassProductList(int pageSize, int pageNumber, string commonCondition, string sort)
        {
            return BrnMall.Data.Products.GetStoreClassProductList(pageSize, pageNumber, commonCondition, sort);
        }

        /// <summary>
        /// 获得店铺分类商品数量
        /// </summary>
        /// <param name="commonCondition">普通条件</param>
        /// <returns></returns>
        public static int GetStoreClassProductCount(string commonCondition)
        {
            return BrnMall.Data.Products.GetStoreClassProductCount(commonCondition);
        }

        /// <summary>
        /// 搜索店铺商品
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="keyword">关键词</param>
        /// <param name="commonCondition">普通条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static List<PartProductInfo> SearchStoreProducts(int pageSize, int pageNumber, string keyword, string commonCondition, string sort)
        {
            return BrnMall.Data.Products.SearchStoreProducts(pageSize, pageNumber, keyword, commonCondition, sort);
        }

        /// <summary>
        /// 获得搜索店铺商品数量
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="commonCondition">普通条件</param>
        /// <returns></returns>
        public static int GetSearchStoreProductCount(string keyword, string commonCondition)
        {
            return BrnMall.Data.Products.GetSearchStoreProductCount(keyword, commonCondition);
        }

        /// <summary>
        /// 获得店铺特征商品列表
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <param name="trait">特征(0代表精品,1代表热销,2代表新品)</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetStoreTraitProductList(int count, int storeId, int storeCid, int trait)
        {
            List<PartProductInfo> storeTraitProductList = BrnMall.Core.BMACache.Get(string.Format(CacheKeys.MALL_PRODUCT_STORETRAITLIST, count, storeId, storeCid, trait)) as List<PartProductInfo>;
            if (storeTraitProductList == null)
            {
                storeTraitProductList = BrnMall.Data.Products.GetStoreTraitProductList(count, storeId, storeCid, trait);
                BrnMall.Core.BMACache.Insert(string.Format(CacheKeys.MALL_PRODUCT_STORETRAITLIST, count, storeId, storeCid, trait), storeTraitProductList);
            }

            return storeTraitProductList;
        }

        /// <summary>
        /// 获得店铺销量商品列表
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetStoreSaleProductList(int count, int storeId, int storeCid)
        {
            List<PartProductInfo> storeSaleProductList = BrnMall.Core.BMACache.Get(string.Format(CacheKeys.MALL_PRODUCT_STORESALELIST, count, storeId, storeCid)) as List<PartProductInfo>;
            if (storeSaleProductList == null)
            {
                storeSaleProductList = BrnMall.Data.Products.GetStoreSaleProductList(count, storeId, storeCid);
                BrnMall.Core.BMACache.Insert(string.Format(CacheKeys.MALL_PRODUCT_STORESALELIST, count, storeId, storeCid), storeSaleProductList);
            }

            return storeSaleProductList;
        }

        /// <summary>
        /// 获得商品汇总列表
        /// </summary>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static DataTable GetProductSummaryList(string pidList)
        {
            return BrnMall.Data.Products.GetProductSummaryList(pidList);
        }




        /// <summary>
        /// 获得商品属性
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        public static ProductAttributeInfo GetProductAttributeByPidAndAttrId(int pid, int attrId)
        {
            return BrnMall.Data.Products.GetProductAttributeByPidAndAttrId(pid, attrId);
        }

        /// <summary>
        /// 获得商品属性列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static List<ProductAttributeInfo> GetProductAttributeList(int pid)
        {
            return BrnMall.Data.Products.GetProductAttributeList(pid);
        }

        /// <summary>
        /// 获得扩展商品属性列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static List<ExtProductAttributeInfo> GetExtProductAttributeList(int pid)
        {
            return BrnMall.Data.Products.GetExtProductAttributeList(pid);
        }




        /// <summary>
        /// 获得商品的sku项列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static DataTable GetProductSKUItemList(int pid)
        {
            return BrnMall.Data.Products.GetProductSKUItemList(pid);
        }

        /// <summary>
        /// 获得商品的sku列表
        /// </summary>
        /// <param name="skuGid">sku组id</param>
        /// <returns></returns>
        public static List<ExtProductSKUItemInfo> GetProductSKUListBySKUGid(int skuGid)
        {
            if (skuGid > 0)
                return BrnMall.Data.Products.GetProductSKUListBySKUGid(skuGid);
            return new List<ExtProductSKUItemInfo>();
        }





        /// <summary>
        /// 获得商品图片列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static List<ProductImageInfo> GetProductImageList(int pid)
        {
            if (pid > 0)
                return BrnMall.Data.Products.GetProductImageList(pid);

            return new List<ProductImageInfo>();
        }




        /// <summary>
        /// 获得商品库存
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static ProductStockInfo GetProductStockByPid(int pid)
        {
            return BrnMall.Data.Products.GetProductStockByPid(pid);
        }

        /// <summary>
        /// 获得商品库存数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static int GetProductStockNumberByPid(int pid)
        {
            return BrnMall.Data.Products.GetProductStockNumberByPid(pid);
        }

        /// <summary>
        /// 增加商品库存数量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void IncreaseProductStockNumber(List<OrderProductInfo> orderProductList)
        {
            BrnMall.Data.Products.IncreaseProductStockNumber(orderProductList);
        }

        /// <summary>
        /// 减少商品库存数量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void DecreaseProductStockNumber(List<OrderProductInfo> orderProductList)
        {
            BrnMall.Data.Products.DecreaseProductStockNumber(orderProductList);
        }

        /// <summary>
        /// 获得商品库存列表
        /// </summary>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static List<ProductStockInfo> GetProductStockList(string pidList)
        {
            if (!string.IsNullOrEmpty(pidList))
                return BrnMall.Data.Products.GetProductStockList(pidList);

            return new List<ProductStockInfo>();
        }



        /// <summary>
        /// 获得分类列表
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public static List<CategoryInfo> GetCategoryListByKeyword(string keyword)
        {
            return BrnMall.Data.Products.GetCategoryListByKeyword(keyword);
        }

        /// <summary>
        /// 获得分类品牌列表
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public static List<BrandInfo> GetCategoryBrandListByKeyword(int cateId, string keyword)
        {
            return BrnMall.Data.Products.GetCategoryBrandListByKeyword(cateId, keyword);
        }




        /// <summary>
        /// 获得关联商品列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetRelateProductList(int pid)
        {
            return BrnMall.Data.Products.GetRelateProductList(pid);
        }
    }
}
