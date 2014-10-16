using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.Models;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 商城目录控制器类
    /// </summary>
    public class CatalogController : BaseWebController
    {
        /// <summary>
        /// 商品
        /// </summary>
        public ActionResult Product()
        {
            //商品id
            int pid = GetRouteInt("pid");
            if (pid == 0)
                pid = WebHelper.GetQueryInt("pid");

            //判断商品是否存在
            ProductInfo productInfo = Products.GetProductById(pid);
            if (productInfo == null)
                return PromptView("/", "你访问的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的商品不存在");

            //商品存在时
            ProductModel model = new ProductModel();
            //商品id
            model.Pid = pid;
            //商品信息
            model.ProductInfo = productInfo;
            //商品分类
            model.CategoryInfo = Categories.GetCategoryById(productInfo.CateId);
            //店铺信息
            model.StoreInfo = storeInfo;
            //店铺区域
            model.StoreRegion = Regions.GetRegionById(storeInfo.RegionId);
            //商品品牌
            model.BrandInfo = Brands.GetBrandById(productInfo.BrandId);
            //商品图片列表
            model.ProductImageList = Products.GetProductImageList(pid);
            //扩展商品属性列表
            model.ExtProductAttributeList = Products.GetExtProductAttributeList(pid);
            //商品SKU列表
            model.ProductSKUList = Products.GetProductSKUListBySKUGid(productInfo.SKUGid);
            //商品库存数量
            model.StockNumber = Products.GetProductStockNumberByPid(pid);


            //单品促销
            model.SinglePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(pid, DateTime.Now);
            //买送促销活动列表
            model.BuySendPromotionList = Promotions.GetBuySendPromotionList(productInfo.StoreId, pid, DateTime.Now);
            //赠品列表
            model.GiftList = Promotions.GetGiftList(pid, DateTime.Now);
            //套装商品列表
            model.SuitProductList = Promotions.GetSuitProductList(productInfo.StoreId, pid, DateTime.Now);
            //满赠促销活动
            model.FullSendPromotionInfo = Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(productInfo.StoreId, pid, DateTime.Now);
            //满减促销活动
            model.FullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(productInfo.StoreId, pid, DateTime.Now);

            //广告语
            model.Slogan = model.SinglePromotionInfo == null ? "" : model.SinglePromotionInfo.Slogan;
            //商品促销信息
            model.PromotionMsg = Promotions.GeneratePromotionMsg(model.SinglePromotionInfo, model.BuySendPromotionList, model.FullSendPromotionInfo, model.FullCutPromotionInfo);
            //商品折扣价格
            model.DiscountPrice = Promotions.ComputeDiscountPrice(model.ProductInfo.ShopPrice, model.SinglePromotionInfo);

            //关联商品列表
            model.RelateProductList = Products.GetRelateProductList(pid);

            //用户浏览历史
            model.UserBrowseHistory = BrowseHistories.GetUserBrowseHistory(WorkContext.Uid, pid);

            //更新浏览历史
            if (WorkContext.Uid > 0)
                Asyn.UpdateBrowseHistory(WorkContext.Uid, pid);
            //更新商品统计
            Asyn.UpdateProductStat(pid, WorkContext.RegionId);

            return View(model);
        }

        /// <summary>
        /// 套装
        /// </summary>
        public ActionResult Suit()
        {
            //套装id
            int pmId = GetRouteInt("pmId");
            if (pmId == 0)
                pmId = WebHelper.GetQueryInt("pmId");

            //判断套装是否存在或过期
            SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(pmId, DateTime.Now);
            if (suitPromotionInfo == null)
                return PromptView("/", "你访问的套装不存在或过期");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(suitPromotionInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的套装不存在");

            //套装商品列表
            DataTable suitProductList = Promotions.GetSuitProductList(pmId);

            SuitModel model = new SuitModel();
            model.SuitPromotionInfo = suitPromotionInfo;
            model.SuitProductList = suitProductList;
            model.StoreInfo = storeInfo;
            model.StoreRegion = Regions.GetRegionById(storeInfo.RegionId);

            foreach (DataRow row in suitProductList.Rows)
            {
                model.SuitDiscount += TypeHelper.ObjectToInt(row["discount"]);
                model.ProductAmount += TypeHelper.ObjectToDecimal(row["shopprice"]);
            }

            return View(model);
        }

        /// <summary>
        /// 分类
        /// </summary>
        public ActionResult Category()
        {
            //分类id
            int cateId = GetRouteInt("cateId");
            if (cateId == 0)
                cateId = WebHelper.GetQueryInt("cateId");
            //品牌id
            int brandId = GetRouteInt("brandId");
            if (brandId == 0)
                brandId = WebHelper.GetQueryInt("brandId");
            //筛选价格
            int filterPrice = GetRouteInt("filterPrice");
            if (filterPrice == 0)
                filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            string filterAttr = GetRouteString("filterAttr");
            if (filterAttr.Length == 0)
                filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = GetRouteInt("onlyStock");
            if (onlyStock == 0)
                onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");

            //分类信息
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("/", "此分类不存在");

            //分类关联品牌列表
            List<BrandInfo> brandList = Categories.GetCategoryBrandList(cateId);
            //属性值id列表
            string[] attrValueIdList = StringHelper.SplitString(filterAttr, "-");
            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //普通sql条件
            StringBuilder commonCondition = new StringBuilder();
            //添加分类
            commonCondition.AppendFormat(" [p].[cateid]={0}", cateId);
            //添加品牌
            if (brandId > 0)
                commonCondition.AppendFormat(" AND [p].[brandid]={0}", brandId);
            //添加价格
            if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            {
                string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
                if (priceRange.Length == 1)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
                }
                else if (priceRange.Length == 2)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
                }
            }
            //只显示上架商品
            commonCondition.Append(" AND [p].[state]=0");


            //属性sql条件
            StringBuilder attrCondition = new StringBuilder();
            //添加属性
            foreach (string attrValueId in attrValueIdList)
            {
                if (TypeHelper.StringToInt(attrValueId) > 0)
                    attrCondition.AppendFormat(" AND [pa].[attrvalueid]={0}", attrValueId);
            }


            //sql排序
            StringBuilder sort = new StringBuilder(" [p].[displayorder] DESC,");
            //排序列
            switch (sortColumn)
            {
                case 0:
                    sort.Append("[p].[salecount]");
                    break;
                case 1:
                    sort.Append("[p].[shopprice]");
                    break;
                case 2:
                    sort.Append("[p].[reviewcount]");
                    break;
                case 3:
                    sort.Append("[p].[addtime]");
                    break;
                case 4:
                    sort.Append("[p].[visitcount]");
                    break;
                default:
                    sort.Append("[p].[salecount]");
                    break;
            }
            //排序方向
            switch (sortDirection)
            {
                case 0:
                    sort.Append(" DESC");
                    break;
                case 1:
                    sort.Append(" ASC");
                    break;
                default:
                    sort.Append(" DESC");
                    break;
            }

            string commonC = commonCondition.ToString();
            string attrC = attrCondition.ToString();
            string sortC = sort.ToString();

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetCategoryProductCount(commonC, attrC, onlyStock != 0 ? 1 : 0));
            //视图对象
            CategoryModel model = new CategoryModel()
            {
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CategoryInfo = categoryInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Products.GetCategoryProductList(pageModel.PageSize, pageModel.PageNumber, commonC, attrC, onlyStock != 0 ? 1 : 0, sortC)
            };

            return View(model);
        }

        /// <summary>
        /// 品牌
        /// </summary>
        public ActionResult Brand()
        {
            //品牌id
            int brandId = GetRouteInt("brandId");
            if (brandId == 0)
                brandId = WebHelper.GetQueryInt("brandId");
            //分类id
            int cateId = GetRouteInt("cateId");
            if (cateId == 0)
                cateId = WebHelper.GetQueryInt("cateId");
            //筛选价格
            int filterPrice = GetRouteInt("filterPrice");
            if (filterPrice == 0)
                filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            string filterAttr = GetRouteString("filterAttr");
            if (filterAttr.Length == 0)
                filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = GetRouteInt("onlyStock");
            if (onlyStock == 0)
                onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");

            //品牌信息
            BrandInfo brandInfo = Brands.GetBrandById(brandId);
            if (brandInfo == null)
                return PromptView("/", "此品牌不存在");
            //分类信息
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("/", "此分类不存在");

            //获取品牌相关的分类
            List<CategoryInfo> categoryList = Brands.GetBrandCategoryListByBrandId(brandId);
            //属性值id列表
            string[] attrValueIdList = StringHelper.SplitString(filterAttr, "-");
            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //普通sql条件
            StringBuilder commonCondition = new StringBuilder();
            //添加分类
            commonCondition.AppendFormat(" [p].[cateid]={0}", cateId);
            //添加品牌
            commonCondition.AppendFormat(" AND [p].[brandid]={0}", brandId);
            //添加价格
            if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            {
                string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
                if (priceRange.Length == 1)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
                }
                else if (priceRange.Length == 2)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
                }
            }
            //只显示上架商品
            commonCondition.Append(" AND [p].[state]=0");


            //属性sql条件
            StringBuilder attrCondition = new StringBuilder();
            //添加属性
            foreach (string attrValueId in attrValueIdList)
            {
                if (TypeHelper.StringToInt(attrValueId) > 0)
                    attrCondition.AppendFormat(" AND [pa].[attrvalueid]={0}", attrValueId);
            }


            //sql排序
            StringBuilder sort = new StringBuilder(" [p].[displayorder] DESC,");
            //排序列
            switch (sortColumn)
            {
                case 0:
                    sort.Append("[p].[salecount]");
                    break;
                case 1:
                    sort.Append("[p].[shopprice]");
                    break;
                case 2:
                    sort.Append("[p].[reviewcount]");
                    break;
                case 3:
                    sort.Append("[p].[addtime]");
                    break;
                case 4:
                    sort.Append("[p].[visitcount]");
                    break;
                default:
                    sort.Append("[p].[salecount]");
                    break;
            }
            //排序方向
            switch (sortDirection)
            {
                case 0:
                    sort.Append(" DESC");
                    break;
                case 1:
                    sort.Append(" ASC");
                    break;
                default:
                    sort.Append(" DESC");
                    break;
            }

            string commonC = commonCondition.ToString();
            string attrC = attrCondition.ToString();
            string sortC = sort.ToString();

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetCategoryProductCount(commonC, attrC, onlyStock != 0 ? 1 : 0));
            //视图对象
            BrandModel model = new BrandModel()
            {
                BrandId = brandId,
                CateId = cateId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                BrandInfo = brandInfo,
                CategoryInfo = categoryInfo,
                CategoryList = categoryList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Products.GetCategoryProductList(pageModel.PageSize, pageModel.PageNumber, commonC, attrC, onlyStock != 0 ? 1 : 0, sortC)
            };

            return View(model);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public ActionResult Search()
        {
            //搜索词
            string keyword = WebHelper.GetQueryString("keyword");
            if (keyword.Length == 0)
                return PromptView(WorkContext.UrlReferrer, "请输入搜索词");
            if (!SecureHelper.IsSafeSqlString(keyword))
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //异步保存搜索历史
            Asyn.UpdateSearchHistory(WorkContext.Uid, keyword);

            //判断搜索词是否为分类名称，如果是则重定向到分类页面
            int cateId = Categories.GetCateIdByName(keyword);
            if (cateId > 0)
            {
                return Redirect(Url.Action("Category", new RouteValueDictionary { { "cateId", cateId } }));
            }
            else
            {
                cateId = WebHelper.GetQueryInt("cateId");
            }

            //分类列表
            List<CategoryInfo> categoryList = null;
            //分类信息
            CategoryInfo categoryInfo = null;
            //品牌列表
            List<BrandInfo> brandList = null;

            //品牌id
            int brandId = Brands.GetBrandIdByName(keyword);
            if (brandId > 0)//当搜索词为品牌名称时
            {
                //获取品牌相关的分类
                categoryList = Brands.GetBrandCategoryListByBrandId(brandId);

                //由于搜索结果的展示是以分类为基础的，所以当分类不存在时直接将搜索结果设为“搜索的商品不存在”
                if (categoryList.Count == 0)
                    return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

                if (cateId > 0)
                {
                    categoryInfo = Categories.GetCategoryById(cateId);
                }
                else
                {
                    //当没有进行分类的筛选时，将分类列表中的首项设为当前选中的分类
                    categoryInfo = categoryList[0];
                    cateId = categoryInfo.CateId;
                }
                brandList = new List<BrandInfo>();
                brandList.Add(Brands.GetBrandById(brandId));
            }
            else//当搜索词为商品关键词时
            {
                //获取商品关键词相关的分类
                categoryList = Products.GetCategoryListByKeyword(keyword);

                //由于搜索结果的展示是以分类为基础的，所以当分类不存在时直接将搜索结果设为“搜索的商品不存在”
                if (categoryList.Count == 0)
                    return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

                if (cateId > 0)
                {
                    categoryInfo = Categories.GetCategoryById(cateId);
                }
                else
                {
                    categoryInfo = categoryList[0];
                    cateId = categoryInfo.CateId;
                }
                //根据商品关键词获取分类相关的品牌
                brandList = Products.GetCategoryBrandListByKeyword(cateId, keyword);
                if (brandList.Count == 0)
                    return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");
                brandId = WebHelper.GetQueryInt("brandId");
            }
            //最后再检查一遍分类是否存在
            if (categoryInfo == null)
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //筛选价格
            int filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            string filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = WebHelper.GetQueryInt("page");

            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //属性值id列表
            string[] attrValueIdList = StringHelper.SplitString(filterAttr, "-");
            //当筛选属性和分类的筛选属性数目不对应时，重置筛选属性
            if (cateAAndVList.Count > 0 && attrValueIdList.Length != cateAAndVList.Count)
            {
                int count = cateAAndVList.Count;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < count; i++)
                    sb.Append("0-");
                filterAttr = sb.Remove(sb.Length - 1, 1).ToString();
                attrValueIdList = StringHelper.SplitString(filterAttr, "-");
            }
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //普通sql条件
            StringBuilder commonCondition = new StringBuilder();
            //添加分类
            commonCondition.AppendFormat(" [p].[cateid]={0}", cateId);
            //添加品牌
            if (brandId > 0)
                commonCondition.AppendFormat(" AND [p].[brandid]={0}", brandId);
            //添加价格
            if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            {
                string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
                if (priceRange.Length == 1)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
                }
                else if (priceRange.Length == 2)
                {
                    commonCondition.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
                }
            }
            //只显示上架商品
            commonCondition.Append(" AND [p].[state]=0");


            //属性sql条件
            StringBuilder attrCondition = new StringBuilder();
            //添加属性
            foreach (string attrValueId in attrValueIdList)
            {
                if (TypeHelper.StringToInt(attrValueId) > 0)
                    attrCondition.AppendFormat(" AND [pa].[attrvalueid]={0}", attrValueId);
            }

            //sql排序
            StringBuilder sort = new StringBuilder(" [p].[displayorder] DESC,");
            //排序列
            switch (sortColumn)
            {
                case 0:
                    sort.Append("[pk].[relevancy]");
                    break;
                case 1:
                    sort.Append("[p].[salecount]");
                    break;
                case 2:
                    sort.Append("[p].[shopprice]");
                    break;
                case 3:
                    sort.Append("[p].[reviewcount]");
                    break;
                case 4:
                    sort.Append("[p].[addtime]");
                    break;
                case 5:
                    sort.Append("[p].[visitcount]");
                    break;
                default:
                    sort.Append("[pk].[relevancy]");
                    break;
            }
            //排序方向
            switch (sortDirection)
            {
                case 0:
                    sort.Append(" DESC");
                    break;
                case 1:
                    sort.Append(" ASC");
                    break;
                default:
                    sort.Append(" DESC");
                    break;
            }

            string commonC = commonCondition.ToString();
            string attrC = attrCondition.ToString();
            string sortC = sort.ToString();

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetSearchMallProductCount(keyword, commonC, attrC, onlyStock != 0 ? 1 : 0));
            //视图对象
            MallSearchModel model = new MallSearchModel()
            {
                Word = keyword,
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CategoryList = categoryList,
                CategoryInfo = categoryInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Products.SearchMallProducts(pageModel.PageSize, pageModel.PageNumber, keyword, commonC, attrC, onlyStock != 0 ? 1 : 0, sortC)
            };
            WorkContext.SearchWord = keyword;

            return View(model);
        }

        /// <summary>
        /// 活动专题
        /// </summary>
        public ActionResult Topic()
        {
            //专题id
            int topicId = GetRouteInt("topicId");
            if (topicId == 0)
                topicId = WebHelper.GetQueryInt("topicId");

            TopicInfo topicInfo = Topics.GetTopicById(topicId);
            if (topicInfo == null)
                return PromptView("此活动专题不存在");

            return View(topicInfo);
        }

        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult ProductReviewList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int page = WebHelper.GetQueryInt("page");

            //判断商品是否存在
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView("/", "你访问的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的商品不存在");

            PageModel pageModel = new PageModel(10, page, ProductReviews.GetProductReviewCount(pid));
            ProductReviewListModel model = new ProductReviewListModel()
            {
                ProductInfo = productInfo,
                StoreInfo = storeInfo,
                StoreRegion = Regions.GetRegionById(storeInfo.RegionId),
                PageModel = pageModel,
                ProductReviewList = ProductReviews.GetProductReviewList(pid, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult AjaxProductReviewList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int page = WebHelper.GetQueryInt("page");

            PageModel pageModel = new PageModel(10, page, ProductReviews.GetProductReviewCount(pid));
            AjaxProductReviewListModel model = new AjaxProductReviewListModel()
            {
                Pid = pid,
                PageModel = pageModel,
                ProductReviewList = ProductReviews.GetProductReviewList(pid, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public ActionResult ProductConsultList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int consultTypeId = WebHelper.GetQueryInt("consultTypeId");
            string consultMessage = WebHelper.GetQueryString("consultMessage");
            int page = WebHelper.GetQueryInt("page");

            //判断商品是否存在
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView("/", "你访问的商品不存在");

            if (!SecureHelper.IsSafeSqlString(consultMessage))
                return PromptView(WorkContext.UrlReferrer, "您搜索的内容不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的商品不存在");

            PageModel pageModel = new PageModel(10, page, ProductConsults.GetProductConsultCount(pid, consultTypeId, consultMessage));
            ProductConsultListModel model = new ProductConsultListModel()
            {
                ProductInfo = productInfo,
                StoreInfo = storeInfo,
                StoreRegion = Regions.GetRegionById(storeInfo.RegionId),
                ProductConsultTypeList = ProductConsults.GetProductConsultTypeList(),
                PageModel = pageModel,
                ProductConsultList = ProductConsults.GetProductConsultList(pageModel.PageSize, pageModel.PageNumber, pid, consultTypeId, consultMessage),
                IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages)
            };

            return View(model);
        }

        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public ActionResult AjaxProductConsultList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int consultTypeId = WebHelper.GetQueryInt("consultTypeId");
            string consultMessage = WebHelper.GetQueryString("consultMessage");
            int page = WebHelper.GetQueryInt("page");

            if (!SecureHelper.IsSafeSqlString(consultMessage))
                return View(new AjaxProductConsultListModel());

            PageModel pageModel = new PageModel(10, page, ProductConsults.GetProductConsultCount(pid, consultTypeId, consultMessage));
            AjaxProductConsultListModel model = new AjaxProductConsultListModel()
            {
                Pid = pid,
                ProductConsultTypeList = ProductConsults.GetProductConsultTypeList(),
                PageModel = pageModel,
                ProductConsultList = ProductConsults.GetProductConsultList(pageModel.PageSize, pageModel.PageNumber, pid, consultTypeId, consultMessage),
                IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages)
            };

            return View(model);
        }

        /// <summary>
        /// 咨询商品
        /// </summary>
        public ActionResult ConsultProduct()
        {
            //不允许游客访问
            if (WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            //验证验证码
            if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
            {
                string verifyCode = WebHelper.GetFormString("verifyCode");//验证码
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    return AjaxResult("emptyverifycode", "验证码不能为空"); ;
                }
                else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                {
                    return AjaxResult("wrongverifycode", "验证码错误"); ;
                }
            }

            int pid = WebHelper.GetFormInt("pid");
            int consultTypeId = WebHelper.GetFormInt("consultTypeId");
            string consultMessage = WebHelper.GetFormString("consultMessage");

            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");

            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("noproduct", "请选择商品");

            if (consultTypeId > 0)
            {
                ProductConsultTypeInfo productConsultTypeInfo = ProductConsults.GetProductConsultTypeById(consultTypeId);
                if (productConsultTypeInfo == null)
                    return AjaxResult("noproductconsulttype", "请选择咨询类型"); ;
            }
            else
            {
                consultTypeId = 0;
            }

            if (string.IsNullOrWhiteSpace(consultMessage))
                return AjaxResult("noconsultmessage", "请填写咨询内容"); ;
            if (consultMessage.Length > 100)
                return AjaxResult("muchconsultmessage", "咨询内容内容太长"); ;
            if (!SecureHelper.IsSafeSqlString(consultMessage))
                return AjaxResult("dangerconsultmessage", "咨询内中包含非法字符"); ;

            ProductConsults.ConsultProduct(pid, consultTypeId, WorkContext.Uid, partProductInfo.StoreId, DateTime.Now, consultMessage, WorkContext.NickName, partProductInfo.Name, partProductInfo.ShowImg, WorkContext.IP);
            return AjaxResult("success", "咨询成功,我们会尽快回复"); ;
        }
    }
}
