using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.Models;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 店铺控制器类
    /// </summary>
    public class StoreController : BaseWebController
    {
        /// <summary>
        /// 店铺首页
        /// </summary>
        public ActionResult Index()
        {
            //店铺id
            int storeId = GetRouteInt("storeId");
            if (storeId == 0)
                storeId = WebHelper.GetQueryInt("storeId");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的店铺不存在");

            RouteData.DataTokens.Add("theme", storeInfo.Theme);
            return View(storeInfo);
        }

        /// <summary>
        /// 店铺分类
        /// </summary>
        public ActionResult Class()
        {
            //店铺id
            int storeId = GetRouteInt("storeId");
            if (storeId == 0)
                storeId = WebHelper.GetQueryInt("storeId");
            //店铺分类id
            int storeCid = GetRouteInt("storeCid");
            if (storeCid == 0)
                storeCid = WebHelper.GetQueryInt("storeCid");
            //开始价格
            int startPrice = GetRouteInt("startPrice");
            if (startPrice == 0)
                startPrice = WebHelper.GetQueryInt("startPrice");
            //结束价格
            int endPrice = GetRouteInt("endPrice");
            if (endPrice == 0)
                endPrice = WebHelper.GetQueryInt("endPrice");
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


            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的店铺不存在");

            //店铺分类信息
            StoreClassInfo storeClassInfo = Stores.GetStoreClassByStoreIdAndStoreCid(storeId, storeCid);
            if (storeClassInfo == null)
                return PromptView("/", "此店铺分类不存在");


            //普通sql条件
            StringBuilder commonCondition = new StringBuilder();
            //添加店铺分类
            commonCondition.AppendFormat(" [storecid]={0}", storeCid);
            //添加价格
            if (startPrice >= 0 && startPrice < endPrice)
            {
                commonCondition.AppendFormat(" AND [shopprice]>={0} AND [shopprice]<{1}", startPrice, endPrice);
            }
            //只显示上架商品
            commonCondition.Append(" AND [state]=0");

            //sql排序
            StringBuilder sort = new StringBuilder(" [displayorder] DESC,");
            //排序列
            switch (sortColumn)
            {
                case 0:
                    sort.Append("[salecount]");
                    break;
                case 1:
                    sort.Append("[shopprice]");
                    break;
                case 2:
                    sort.Append("[reviewcount]");
                    break;
                case 3:
                    sort.Append("[addtime]");
                    break;
                case 4:
                    sort.Append("[visitcount]");
                    break;
                default:
                    sort.Append("[salecount]");
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
            string sortC = sort.ToString();

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetStoreClassProductCount(commonC));
            //视图对象
            StoreClassModel model = new StoreClassModel()
            {
                StoreId = storeId,
                StoreCid = storeCid,
                StartPrice = startPrice,
                EndPrice = endPrice,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Products.GetStoreClassProductList(pageModel.PageSize, pageModel.PageNumber, commonC, sortC),
                StoreInfo = storeInfo,
                StoreClassInfo = storeClassInfo
            };

            RouteData.DataTokens.Add("theme", storeInfo.Theme);
            return View(model);
        }

        /// <summary>
        /// 店铺搜索
        /// </summary>
        public ActionResult Search()
        {
            //店铺id
            int storeId = WebHelper.GetQueryInt("storeId");
            //搜索词
            string keyword = WebHelper.GetQueryString("keyword");
            if (keyword.Length == 0)
                return PromptView(WorkContext.UrlReferrer, "请输入搜索词");
            if (!SecureHelper.IsSafeSqlString(keyword))
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的店铺不存在");

            //判断搜索词是否为店铺分类名称，如果是则重定向到店铺分类页面
            int storeCid = Stores.GetStoreCidByStoreIdAndName(storeId, keyword);
            if (storeCid > 0)
            {
                return Redirect(Url.Action("Class", new RouteValueDictionary { { "storeId", storeId }, { "storeCid", storeCid } }));
            }

            //店铺分类id
            storeCid = WebHelper.GetQueryInt("storeCid");
            //开始价格
            int startPrice = WebHelper.GetQueryInt("startPrice");
            //结束价格
            int endPrice = WebHelper.GetQueryInt("endPrice");
            //排序列
            int sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = WebHelper.GetQueryInt("page");

            //店铺分类信息
            StoreClassInfo storeClassInfo = Stores.GetStoreClassByStoreIdAndStoreCid(storeId, storeCid);
            if (storeCid > 0 && storeClassInfo == null)
                return PromptView("/", "此店铺分类不存在");

            //普通sql条件
            StringBuilder commonCondition = new StringBuilder();
            //添加店铺分类
            if (storeCid > 0)
            {
                commonCondition.AppendFormat(" AND [p].[storecid]={0}", storeCid);
            }
            //添加价格
            if (startPrice >= 0 && startPrice < endPrice)
            {
                commonCondition.AppendFormat(" AND [p].[shopprice]>={0} AND [p].[shopprice]<{1}", startPrice, endPrice);
            }
            //只显示上架商品
            commonCondition.Append(" AND [p].[state]=0");

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

            string commonC = commonCondition.Remove(0, 4).ToString();
            string sortC = sort.ToString();

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetSearchStoreProductCount(keyword, commonC));
            //视图对象
            StoreSearchModel model = new StoreSearchModel()
            {
                StoreId = storeId,
                Word = keyword,
                StoreCid = storeCid,
                StartPrice = startPrice,
                EndPrice = endPrice,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Products.SearchStoreProducts(pageModel.PageSize, pageModel.PageNumber, keyword, commonC, sortC),
                StoreInfo = storeInfo,
                StoreClassInfo = storeClassInfo
            };

            //异步保存搜索历史
            Asyn.UpdateSearchHistory(WorkContext.Uid, keyword);

            RouteData.DataTokens.Add("theme", storeInfo.Theme);
            return View(model);
        }

        /// <summary>
        /// 店铺详情
        /// </summary>
        public ActionResult Details()
        {
            //店铺id
            int storeId = GetRouteInt("storeId");
            if (storeId == 0)
                storeId = WebHelper.GetQueryInt("storeId");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "你访问的店铺不存在");

            RouteData.DataTokens.Add("theme", storeInfo.Theme);
            return View(storeInfo);
        }
    }
}
