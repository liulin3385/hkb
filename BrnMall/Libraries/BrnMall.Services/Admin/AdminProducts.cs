﻿using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 后台商品操作管理类
    /// </summary>
    public class AdminProducts : Products
    {
        private static object _locker = new object();//锁对象
        private static int _skugid;//sku组id;

        static AdminProducts()
        {
            _skugid = GetMaxSKUGid();
            if (_skugid < 0)
                throw new BMAException("无法获得有效的最大sku组id值");
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <param name="stockNumber">库存数量</param>
        /// <param name="stockLimit">库存警戒线</param>
        /// <param name="productAttributeList">商品属性列表</param>
        /// <returns></returns>
        public static int AddProduct(ProductInfo productInfo, int stockNumber, int stockLimit, List<ProductAttributeInfo> productAttributeList)
        {
            //创建商品
            int pid = CreateProduct(productInfo);
            if (pid > 0)
            {
                //创建商品库存
                CreateProductStock(pid, stockNumber, stockLimit);
                if (productAttributeList != null)
                {
                    //创建商品属性
                    foreach (ProductAttributeInfo productAttributeInfo in productAttributeList)
                    {
                        productAttributeInfo.Pid = pid;
                        CreateProductAttribute(productAttributeInfo);
                    }
                }
            }
            return pid;
        }

        /// <summary>
        /// 添加SKU
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <param name="productSKUItemList">商品SKU项列表</param>
        public static void AddSKU(ProductInfo productInfo, List<ProductSKUItemInfo> productSKUItemList)
        {
            lock (_locker)
            {
                List<ProductSKUItemInfo> attrList = new List<ProductSKUItemInfo>();
                List<ProductSKUItemInfo> attrValueList = new List<ProductSKUItemInfo>();

                foreach (ProductSKUItemInfo productSKUItemInfo in productSKUItemList)
                {
                    if (attrValueList.Find(x => x.AttrValueId == productSKUItemInfo.AttrValueId) == null)
                    {
                        attrValueList.Add(productSKUItemInfo);
                    }
                }
                foreach (ProductSKUItemInfo productSKUItemInfo in attrValueList)
                {
                    if (attrList.Find(x => x.AttrId == productSKUItemInfo.AttrId) == null)
                    {
                        attrList.Add(productSKUItemInfo);
                    }
                }

                //sku数量
                int skuCount = 1;
                for (var i = 0; i < attrList.Count; i++)
                {
                    skuCount = skuCount * attrValueList.FindAll(x => x.AttrId == attrList[i].AttrId).Count;
                }


                //sku项数量
                int skuItemCount = productSKUItemList.Count / skuCount;

                //设置商品的sku组id
                ++_skugid;
                productInfo.SKUGid = _skugid;

                //商品原始名称
                string oName = productInfo.Name;
                //循环创建商品，商品属性，商品sku项s
                for (int i = 0; i < skuCount; i++)
                {
                    //格式化商品名称
                    StringBuilder pName = new StringBuilder(oName);
                    for (int j = skuItemCount * i; j < skuItemCount * (i + 1); j++)
                    {
                        AttributeValueInfo attributeValueInfo = AdminCategories.GetAttributeValueById(productSKUItemList[j].AttrValueId);
                        if (attributeValueInfo.IsInput == 0)
                            pName.AppendFormat(" {0}", attributeValueInfo.AttrValue);
                        else
                            pName.AppendFormat(" {0}", productSKUItemList[j].InputValue);
                    }
                    productInfo.Name = pName.ToString();
                    int pid = AddProduct(productInfo, 0, 0, null);//创建商品及其属性
                    if (pid > 0)
                    {
                        //创建商品sku项
                        for (int j = skuItemCount * i; j < skuItemCount * (i + 1); j++)
                        {
                            productSKUItemList[j].Pid = pid;
                            productSKUItemList[j].SKUGid = _skugid;
                            CreateProductSKUItem(productSKUItemList[j]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <returns>商品id</returns>
        public static int CreateProduct(ProductInfo productInfo)
        {
            return BrnMall.Data.Products.CreateProduct(productInfo);
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <param name="stockNumber">库存数量</param>
        /// <param name="stockLimit">库存警戒线</param>
        public static void UpdateProduct(ProductInfo productInfo, int stockNumber, int stockLimit)
        {
            UpdateProduct(productInfo);
            UpdateProductStock(productInfo.Pid, stockNumber, stockLimit);
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        public static void UpdateProduct(ProductInfo productInfo)
        {
            BrnMall.Data.Products.UpdateProduct(productInfo);
        }

        /// <summary>
        /// 后台获得商品列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <param name="productName">商品名称</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public static string AdminGetProductListCondition(int storeId, int storeCid, string productName, int cateId, int brandId, int state)
        {
            return BrnMall.Data.Products.AdminGetProductListCondition(storeId, storeCid, productName, cateId, brandId, state);
        }

        /// <summary>
        /// 后台获得商品列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetProductListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Data.Products.AdminGetProductListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetProductList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Data.Products.AdminGetProductList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得商品数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetProductCount(string condition)
        {
            return BrnMall.Data.Products.AdminGetProductCount(condition);
        }

        /// <summary>
        /// 后台获得商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static ProductInfo AdminGetProductById(int pid)
        {
            if (pid < 1) return null;
            return BrnMall.Data.Products.AdminGetProductById(pid);
        }

        /// <summary>
        /// 后台获得部分商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static PartProductInfo AdminGetPartProductById(int pid)
        {
            if (pid < 1) return null;
            return BrnMall.Data.Products.AdminGetPartProductById(pid);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="pidList">商品id</param>
        public static void DeleteProductById(int[] pidList)
        {
            if (pidList != null && pidList.Length > 0)
                BrnMall.Data.Products.DeleteProductById(CommonHelper.IntArrayToString(pidList));
        }

        /// <summary>
        /// 修改商品状态
        /// </summary>
        /// <param name="pidList">商品id</param>
        /// <param name="state">商品状态</param>
        public static bool UpdateProductState(int[] pidList, ProductState state)
        {
            if (pidList != null && pidList.Length > 0)
                return BrnMall.Data.Products.UpdateProductState(CommonHelper.IntArrayToString(pidList), state);
            return false;
        }

        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="pidList">商品id</param>
        public static bool OnSaleProductById(int[] pidList)
        {
            return UpdateProductState(pidList, ProductState.OnSale);
        }

        /// <summary>
        /// 下架商品
        /// </summary>
        /// <param name="pidList">商品id</param>
        public static bool OutSaleProductById(int[] pidList)
        {
            return UpdateProductState(pidList, ProductState.OutSale);
        }

        /// <summary>
        /// 回收商品
        /// </summary>
        /// <param name="pidList">商品id</param>
        public static bool RecycleProductById(int[] pidList)
        {
            return UpdateProductState(pidList, ProductState.RecycleBin);
        }

        /// <summary>
        /// 恢复商品
        /// </summary>
        /// <param name="pidList">商品id</param>
        public static bool RestoreProductById(int[] pidList)
        {
            return UpdateProductState(pidList, ProductState.OutSale);
        }

        /// <summary>
        /// 修改商品排序
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="displayOrder">商品排序</param>
        public static bool UpdateProductDisplayOrder(int pid, int displayOrder)
        {
            if (pid < 1) return false;
            return BrnMall.Data.Products.UpdateProductDisplayOrder(pid, displayOrder);
        }

        /// <summary>
        /// 改变商品新品状态
        /// </summary>
        /// <param name="pidList">商品id</param>
        /// <param name="isNew">是否新品</param>
        public static bool ChangeProductIsNew(int[] pidList, int isNew)
        {
            if (pidList != null && pidList.Length > 0)
                return BrnMall.Data.Products.ChangeProductIsNew(CommonHelper.IntArrayToString(pidList), isNew);
            return false;
        }

        /// <summary>
        /// 改变商品热销状态
        /// </summary>
        /// <param name="pidList">商品id</param>
        /// <param name="isHot">是否热销</param>
        public static bool ChangeProductIsHot(int[] pidList, int isHot)
        {
            if (pidList != null && pidList.Length > 0)
                return BrnMall.Data.Products.ChangeProductIsHot(CommonHelper.IntArrayToString(pidList), isHot);
            return false;
        }

        /// <summary>
        /// 改变商品精品状态
        /// </summary>
        /// <param name="pidList">商品id</param>
        /// <param name="isBest">是否精品</param>
        public static bool ChangeProductIsBest(int[] pidList, int isBest)
        {
            if (pidList != null && pidList.Length > 0)
                return BrnMall.Data.Products.ChangeProductIsBest(CommonHelper.IntArrayToString(pidList), isBest);
            return false;
        }

        /// <summary>
        /// 修改商品商城价格
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="shopPrice">商城价格</param>
        public static bool UpdateProductShopPrice(int pid, decimal shopPrice)
        {
            if (pid < 1 || shopPrice < 0M) return false;
            return BrnMall.Data.Products.UpdateProductShopPrice(pid, shopPrice);

        }

        /// <summary>
        /// 更新商品图片
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="showImg">商品图片</param>
        public static void UpdateProductShowImage(int pid, string showImg)
        {
            BrnMall.Data.Products.UpdateProductShowImage(pid, showImg);
        }

        /// <summary>
        /// 后台通过商品名称获得商品id
        /// </summary>
        /// <param name="name">商品名称</param>
        /// <returns></returns>
        public static int AdminGetProductIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return -1;
            return BrnMall.Data.Products.AdminGetProductIdByName(name);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByPid(int[] pidList)
        {
            if (pidList != null && pidList.Length > 0)
                return BrnMall.Data.Products.GetStoreIdListByPid(CommonHelper.IntArrayToString(pidList));
            return new DataTable();
        }

        /// <summary>
        /// 判断商品是否为指定店铺的商品
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static bool IsStoreProductByPid(int storeId, int[] pidList)
        {
            DataTable storeIdList = GetStoreIdListByPid(pidList);
            if (storeIdList.Rows.Count != 1 || storeId != TypeHelper.ObjectToInt(storeIdList.Rows[0][0]))
                return false;
            return true;
        }

        /// <summary>
        /// 后台获得店铺商品数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static int AdminGetStoreProductCount(int storeId)
        {
            return BrnMall.Data.Products.AdminGetStoreProductCount(storeId);
        }

        /// <summary>
        /// 后台获得店铺分类商品数量
        /// </summary>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static int AdminGetStoreClassProductCount(int storeCid)
        {
            return BrnMall.Data.Products.AdminGetStoreClassProductCount(storeCid);
        }

        /// <summary>
        /// 后台获得店铺配送模板商品数量
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static int AdminGetStoreShipTemplateProductCount(int storeSTid)
        {
            return BrnMall.Data.Products.AdminGetStoreShipTemplateProductCount(storeSTid);
        }




        /// <summary>
        /// 创建商品属性
        /// </summary>
        public static bool CreateProductAttribute(ProductAttributeInfo productAttributeInfo)
        {
            return BrnMall.Data.Products.CreateProductAttribute(productAttributeInfo);
        }

        /// <summary>
        /// 更新商品属性
        /// </summary>
        public static bool UpdateProductAttribute(ProductAttributeInfo productAttributeInfo)
        {
            if (productAttributeInfo.Pid < 1 || productAttributeInfo.AttrId < 1 || productAttributeInfo.AttrValueId < 0 || (productAttributeInfo.AttrValueId == 0 && string.IsNullOrWhiteSpace(productAttributeInfo.InputValue)))
                return false;
            return BrnMall.Data.Products.UpdateProductAttribute(productAttributeInfo);
        }

        /// <summary>
        /// 删除商品属性
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        public static bool DeleteProductAttributeByPidAndAttrId(int pid, int attrId)
        {
            if (pid > 0 && attrId > 0)
                return BrnMall.Data.Products.DeleteProductAttributeByPidAndAttrId(pid, attrId);
            return false;
        }






        /// <summary>
        /// 创建商品sku项
        /// </summary>
        /// <param name="productSKUItemInfo">商品sku项信息</param>
        public static void CreateProductSKUItem(ProductSKUItemInfo productSKUItemInfo)
        {
            BrnMall.Data.Products.CreateProductSKUItem(productSKUItemInfo);
        }

        /// <summary>
        /// 获得sku组id的最大值
        /// </summary>
        /// <returns></returns>
        public static int GetMaxSKUGid()
        {
            return BrnMall.Data.Products.GetMaxSKUGid();
        }






        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="pImgId">商品id</param>
        /// <returns></returns>
        public static bool DeleteProductImageById(int pImgId)
        {
            ProductImageInfo productImageInfo = GetProductImageById(pImgId);
            bool result = BrnMall.Data.Products.DeleteProductImageById(productImageInfo.Pid, pImgId);
            if (result && productImageInfo.IsMain == 1)
            {
                UpdateProductShowImage(productImageInfo.Pid, "");
            }
            return result;
        }

        /// <summary>
        /// 设置图片为商品主图
        /// </summary>
        /// <param name="pImgId">商品图片id</param>
        /// <returns></returns>
        public static bool SetProductMainImage(int pImgId)
        {
            bool result = false;
            ProductImageInfo productImageInfo = GetProductImageById(pImgId);
            if (productImageInfo != null && productImageInfo.IsMain != 1)
            {
                result = BrnMall.Data.Products.SetProductMainImage(productImageInfo.Pid, pImgId);
                if (result)
                    UpdateProductShowImage(productImageInfo.Pid, productImageInfo.ShowImg);
            }
            return result;
        }

        /// <summary>
        /// 改变商品图片排序
        /// </summary>
        /// <param name="pImgId">商品图片id</param>
        /// <param name="showImg">图片排序</param>
        /// <returns></returns>
        public static bool ChangeProductImageDisplayOrder(int pImgId, int displayOrder)
        {
            ProductImageInfo productImageInfo = GetProductImageById(pImgId);
            if (productImageInfo != null)
                return BrnMall.Data.Products.ChangeProductImageDisplayOrder(productImageInfo.Pid, pImgId, displayOrder);
            return false;
        }

        /// <summary>
        /// 创建商品图片
        /// </summary>
        public static bool CreateProductImage(ProductImageInfo productImageInfo)
        {
            return BrnMall.Data.Products.CreateProductImage(productImageInfo);
        }

        /// <summary>
        /// 获得商品图片
        /// </summary>
        /// <param name="pImgId">图片id</param>
        /// <returns></returns>
        public static ProductImageInfo GetProductImageById(int pImgId)
        {
            if (pImgId < 1) return null;
            return BrnMall.Data.Products.GetProductImageById(pImgId);
        }





        /// <summary>
        /// 创建商品库存
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="number">商品数量</param>
        /// <param name="limit">商品库存警戒线</param>
        /// <returns></returns>
        public static bool CreateProductStock(int pid, int number, int limit)
        {
            return BrnMall.Data.Products.CreateProductStock(pid, number, limit);
        }

        /// <summary>
        /// 更新商品库存
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="number">商品数量</param>
        /// <param name="limit">商品库存警戒线</param>
        public static void UpdateProductStock(int pid, int number, int limit)
        {
            BrnMall.Data.Products.UpdateProductStock(pid, number, limit);
        }

        /// <summary>
        /// 更新商品库存数量
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="number">库存数量</param>
        /// <returns></returns>
        public static bool UpdateProductStockNumber(int pid, int number)
        {
            return BrnMall.Data.Products.UpdateProductStockNumber(pid, number);
        }





        /// <summary>
        /// 创建商品关键词
        /// </summary>
        /// <param name="productKeywordInfo">商品关键词信息</param>
        public static void CreateProductKeyword(ProductKeywordInfo productKeywordInfo)
        {
            BrnMall.Data.Products.CreateProductKeyword(productKeywordInfo);
        }

        /// <summary>
        /// 更新商品关键词
        /// </summary>
        /// <param name="productKeywordInfo">商品关键词信息</param>
        public static void UpdateProductKeyword(ProductKeywordInfo productKeywordInfo)
        {
            BrnMall.Data.Products.UpdateProductKeyword(productKeywordInfo);
        }

        /// <summary>
        /// 删除商品关键词
        /// </summary>
        /// <param name="keywordIdList">关键词id列表</param>
        public static bool DeleteProductKeyword(int[] keywordIdList)
        {
            if (keywordIdList != null && keywordIdList.Length > 0)
                return BrnMall.Data.Products.DeleteProductKeyword(CommonHelper.IntArrayToString(keywordIdList));
            return false;
        }

        /// <summary>
        /// 获得商品关键词列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static List<ProductKeywordInfo> GetProductKeywordList(int pid)
        {
            return BrnMall.Data.Products.GetProductKeywordList(pid);
        }

        /// <summary>
        /// 是否存在商品关键词
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public static bool IsExistProductKeyWord(int pid, string keyword)
        {
            return BrnMall.Data.Products.IsExistProductKeyword(pid, keyword);
        }

        /// <summary>
        /// 更新商品关键词的相关性
        /// </summary>
        /// <param name="keywordId">关键词id</param>
        /// <param name="relevancy">相关性</param>
        /// <returns></returns>
        public static bool UpdateProductKeywordRelevancy(int keywordId, int relevancy)
        {
            return BrnMall.Data.Products.UpdateProductKeywordRelevancy(keywordId, relevancy);
        }

        /// <summary>
        /// 获得商品id
        /// </summary>
        /// <param name="keywordId">关键词id</param>
        /// <returns></returns>
        public static int GetPidByKeywordId(int keywordId)
        {
            return BrnMall.Data.Products.GetPidByKeywordId(keywordId);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="keywordIdList">关键词id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByKeywordId(int[] keywordIdList)
        {
            if (keywordIdList != null && keywordIdList.Length > 0)
                return BrnMall.Data.Products.GetStoreIdListByKeywordId(CommonHelper.IntArrayToString(keywordIdList));
            return new DataTable();
        }

        /// <summary>
        /// 判断商品是否为指定店铺的商品
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="keywordIdList">关键词id列表</param>
        /// <returns></returns>
        public static bool IsStoreProductByKeywordId(int storeId, int[] keywordIdList)
        {
            DataTable storeIdList = GetStoreIdListByKeywordId(keywordIdList);
            if (storeIdList.Rows.Count != 1 || storeId != TypeHelper.ObjectToInt(storeIdList.Rows[0][0]))
                return false;
            return true;
        }





        /// <summary>
        /// 添加关联商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="relatePid">关联商品id</param>
        public static void AddRelateProduct(int pid, int relatePid)
        {
            BrnMall.Data.Products.AddRelateProduct(pid, relatePid);
        }

        /// <summary>
        /// 删除关联商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="relatePid">关联商品id</param>
        /// <returns></returns>
        public static bool DeleteRelateProductByPidAndRelatePid(int pid, int relatePid)
        {
            return BrnMall.Data.Products.DeleteRelateProductByPidAndRelatePid(pid, relatePid);
        }

        /// <summary>
        /// 关联商品是否已经存在
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="relatePid">关联商品id</param>
        public static bool IsExistRelateProduct(int pid, int relatePid)
        {
            return BrnMall.Data.Products.IsExistRelateProduct(pid, relatePid);
        }

        /// <summary>
        /// 后台获得关联商品列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static DataTable AdminGetRelateProductList(int pid)
        {
            return BrnMall.Data.Products.AdminGetRelateProductList(pid);
        }
    }
}
