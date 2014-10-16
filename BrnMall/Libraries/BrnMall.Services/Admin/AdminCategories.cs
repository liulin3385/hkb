using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 后台分类操作管理类
    /// </summary>
    public class AdminCategories : Categories
    {
        //商城后台分类选择列表缓存文件
        private const string _malladmincategoryselectlistcachefile = "/Admin_Mall/Cache/Category/SelectList.js";

        /// <summary>
        /// 后台获得分类列表
        /// </summary>
        /// <returns></returns>
        public static List<CategoryInfo> AdminGetCategoryList()
        {
            return BrnMall.Data.Categories.AdminGetCategoryList();
        }

        /// <summary>
        /// 后台获得分类列表
        /// </summary>
        /// <returns></returns>
        public static List<CategoryInfo> AdminGetCategoryTree()
        {
            List<CategoryInfo> categoryList = new List<CategoryInfo>();
            List<CategoryInfo> sourceCategoryList = AdminCategories.AdminGetCategoryList();
            AdminCategories.CreateCategoryTree(sourceCategoryList, categoryList, 0);
            return categoryList;
        }

        /// <summary>
        /// 后台通过分类名称获得分类id
        /// </summary>
        /// <param name="categoryName">分类名称</param>
        /// <returns></returns>
        public static int AdminGetCateIdByName(string categoryName)
        {
            foreach (CategoryInfo categoryInfo in AdminGetCategoryList())
            {
                if (categoryInfo.Name == categoryName)
                    return categoryInfo.CateId;
            }
            return 0;
        }

        /// <summary>
        /// 后台获得分类
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <returns></returns>
        public static CategoryInfo AdminGetCategoryById(int cateId)
        {
            foreach (CategoryInfo categoryInfo in AdminGetCategoryList())
            {
                if (categoryInfo.CateId == cateId)
                    return categoryInfo;
            }

            return null;
        }

        /// <summary>
        /// 更新分类
        /// </summary>
        public static void UpdateCategory(CategoryInfo categoryInfo, int oldParentId)
        {
            if (oldParentId != categoryInfo.ParentId)//父分类改变时
            {
                List<CategoryInfo> categoryList = AdminGetCategoryList();
                if (categoryInfo.ParentId > 0)//非顶层分类时
                {
                    CategoryInfo newParentCategoryInfo = categoryList.Find(x => x.CateId == categoryInfo.ParentId);//新的父分类
                    categoryInfo.Layer = newParentCategoryInfo.Layer + 1;
                    categoryInfo.Path = newParentCategoryInfo.Path + "," + categoryInfo.CateId;

                    if (categoryList.FindAll(x => x.ParentId == newParentCategoryInfo.CateId).Count == 0)
                    {
                        newParentCategoryInfo.HasChild = 1;
                        BrnMall.Data.Categories.UpdateCategory(newParentCategoryInfo);
                    }
                }
                else//顶层分类时
                {
                    categoryInfo.Layer = 1;
                    categoryInfo.Path = categoryInfo.CateId.ToString();
                }

                if (categoryList.FindAll(x => x.ParentId == oldParentId).Count == 1)
                {
                    CategoryInfo oldParentCategoryInfo = categoryList.Find(x => x.CateId == oldParentId);//旧的父分类
                    oldParentCategoryInfo.HasChild = 0;
                    BrnMall.Data.Categories.UpdateCategory(oldParentCategoryInfo);
                }
            }

            BrnMall.Data.Categories.UpdateCategory(categoryInfo);

            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_LIST);

            WriteMallAdminCategorySelectListCache(GetCategoryList());
        }

        /// <summary>
        /// 创建分类
        /// </summary>
        public static void CreateCategory(CategoryInfo categoryInfo)
        {
            if (categoryInfo.ParentId > 0)
            {
                List<CategoryInfo> categoryList = AdminGetCategoryList();
                CategoryInfo parentCategoryInfo = categoryList.Find(x => x.CateId == categoryInfo.ParentId);
                categoryInfo.Layer = parentCategoryInfo.Layer + 1;
                categoryInfo.HasChild = 0;
                categoryInfo.Path = "";
                int categoryId = BrnMall.Data.Categories.CreateCategory(categoryInfo);

                categoryInfo.CateId = categoryId;
                categoryInfo.Path = parentCategoryInfo.Path + "," + categoryId;
                BrnMall.Data.Categories.UpdateCategory(categoryInfo);

                if (parentCategoryInfo.HasChild == 0)
                {
                    parentCategoryInfo.HasChild = 1;
                    BrnMall.Data.Categories.UpdateCategory(parentCategoryInfo);
                }
            }
            else
            {
                categoryInfo.Layer = 1;
                categoryInfo.HasChild = 0;
                categoryInfo.Path = "";
                int categoryId = BrnMall.Data.Categories.CreateCategory(categoryInfo);
                categoryInfo.CateId = categoryId;
                categoryInfo.Path = categoryId.ToString();
                BrnMall.Data.Categories.UpdateCategory(categoryInfo);
            }


            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_LIST);

            WriteMallAdminCategorySelectListCache(GetCategoryList());
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <returns>0代表此分类下还有分类未删除,1代表删除成功</returns>
        public static int DeleteCategoryById(int cateId)
        {
            List<CategoryInfo> categoryList = AdminGetCategoryList();
            CategoryInfo categoryInfo = categoryList.Find(x => x.CateId == cateId);
            if (categoryInfo.HasChild == 1)
                return 0;

            BrnMall.Data.Categories.DeleteCategoryById(cateId);

            if (categoryList.FindAll(x => x.ParentId == categoryInfo.ParentId).Count == 1)
            {
                categoryInfo.HasChild = 0;
                BrnMall.Data.Categories.UpdateCategory(categoryInfo);
            }

            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_LIST);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_BRANDLIST + cateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + cateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + cateId);

            WriteMallAdminCategorySelectListCache(GetCategoryList());
            return 1;
        }

        /// <summary>
        /// 隐藏分类
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <returns>0代表此分类下还有分类,1代表隐藏成功</returns>
        public static int HideCategoryById(int cateId)
        {
            if (AdminGetCategoryById(cateId).HasChild == 1)
                return 0;

            BrnMall.Data.Categories.HideCategoryById(cateId);

            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_LIST);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_BRANDLIST + cateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + cateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + cateId);

            WriteMallAdminCategorySelectListCache(GetCategoryList());
            return 1;
        }

        /// <summary>
        /// 展示分类
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <returns>0代表此分类下还有分类,1代表显示成功</returns>
        public static int ShowCategoryById(int cateId)
        {
            if (AdminGetCategoryById(cateId).HasChild == 1)
                return 0;

            BrnMall.Data.Categories.ShowCategoryById(cateId);

            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_LIST);

            WriteMallAdminCategorySelectListCache(GetCategoryList());
            return 1;
        }

        /// <summary>
        /// 将分类选择列表写入缓存文件
        /// </summary>
        private static void WriteMallAdminCategorySelectListCache(List<CategoryInfo> categoryList)
        {
            StringBuilder sb = new StringBuilder("<div id=\"categoryTree\"><table width=\"100%\"><thead><tr><th align=\"left\">分类名称</th><th width=\"100\" align=\"left\">管理操作</th></tr></thead><tbody>");

            foreach (CategoryInfo categoryInfo in categoryList)
            {
                bool isHasChild = categoryInfo.HasChild == 1;
                sb.AppendFormat("<tr layer=\"{0}\" {1}><th>{2}<span {3} onclick=\"categoryTree(this,{0})\"></span>{4}</th><td>", categoryInfo.Layer, categoryInfo.Layer != 1 ? "style=\"display:none\"" : "", CommonHelper.GetHtmlSpan(categoryInfo.Layer - 1), isHasChild ? "class='open'" : "", categoryInfo.Name);
                if (!isHasChild)
                    sb.AppendFormat("<a href=\"javascript:SetSelectedCategory({0},'{1}')\" class=\"editOperate\">[选择]</a>", categoryInfo.CateId, categoryInfo.Name);
                sb.AppendFormat("</td></tr>");
            }

            sb.AppendFormat("</tbody></table></div>");

            try
            {
                string filePath = IOHelper.GetMapPath(_malladmincategoryselectlistcachefile);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Byte[] info = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    fs.Write(info, 0, info.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            { }
        }





        /// <summary>
        /// 更新属性分组
        /// </summary>
        /// <param name="newAttributeGroupInfo">新属性分组</param>
        /// <param name="oldAttributeGroupInfo">原属性分组</param>
        public static void UpdateAttributeGroup(AttributeGroupInfo attributeGroupInfo)
        {
            BrnMall.Data.Categories.UpdateAttributeGroup(attributeGroupInfo);
        }

        /// <summary>
        /// 创建属性分组
        /// </summary>
        public static void CreateAttributeGroup(AttributeGroupInfo attributeGroupInfo)
        {
            BrnMall.Data.Categories.CreateAttributeGroup(attributeGroupInfo);
        }

        /// <summary>
        /// 删除属性分组
        /// </summary>
        /// <param name="attrGroupId">属性分组id</param>
        /// <returns>0代表此分类下还有展示属性未删除,1代表删除成功</returns>
        public static void DeleteAttributeGroupById(int attrGroupId)
        {
            BrnMall.Data.Categories.DeleteAttributeGroupById(attrGroupId);
        }






        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="newAttributeInfo">新属性</param>
        /// <param name="oldAttributeInfo">原属性</param>
        public static void UpdateAttribute(AttributeInfo attributeInfo)
        {
            BrnMall.Data.Categories.UpdateAttribute(attributeInfo);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }

        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="attributeInfo">属性信息</param>
        /// <param name="attributeGroupInfo">属性组信息</param>
        public static void CreateAttribute(AttributeInfo attributeInfo, AttributeGroupInfo attributeGroupInfo)
        {
            BrnMall.Data.Categories.CreateAttribute(attributeInfo, attributeGroupInfo.AttrGroupId, attributeGroupInfo.Name, attributeGroupInfo.DisplayOrder);
            //BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="attrId">属性id</param>
        public static void DeleteAttributeById(int attrId)
        {
            AttributeInfo attributeInfo = GetAttributeById(attrId);
            BrnMall.Data.Categories.DeleteAttributeById(attrId);
            if (attributeInfo.IsFilter == 1)
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }

        /// <summary>
        /// 后台获得属性列表
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetAttributeList(int cateId, string sort)
        {
            return BrnMall.Data.Categories.AdminGetAttributeList(cateId, sort);
        }

        /// <summary>
        /// 后台获得属性列表排序
        /// </summary>
        /// <param name="sortColumn">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetAttributeListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Data.Categories.AdminGetAttributeListSort(sortColumn, sortDirection);
        }







        /// <summary>
        /// 创建属性值
        /// </summary>
        public static void CreateAttributeValue(AttributeValueInfo attributeValueInfo)
        {
            BrnMall.Data.Categories.CreateAttributeValue(attributeValueInfo);
            AttributeInfo attributeInfo = GetAttributeById(attributeValueInfo.AttrId);
            if (attributeInfo.IsFilter == 1)
            {
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            }
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }

        /// <summary>
        /// 删除属性值
        /// </summary>
        /// <param name="attrValueId">属性值id</param>
        public static void DeleteAttributeValueById(int attrValueId)
        {
            if (attrValueId < 1) return;

            AttributeValueInfo attributeValueInfo = GetAttributeValueById(attrValueId);
            AttributeInfo attributeInfo = GetAttributeById(attributeValueInfo.AttrId);
            BrnMall.Data.Categories.DeleteAttributeValueById(attrValueId);
            if (attributeInfo.IsFilter == 1)
            {
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            }
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_ATTRIBUTEVALUE_INFO + attrValueId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }

        /// <summary>
        /// 更新属性值
        /// </summary>
        public static void UpdateAttributeValue(AttributeValueInfo attributeValueInfo)
        {
            BrnMall.Data.Categories.UpdateAttributeValue(attributeValueInfo);
            AttributeInfo attributeInfo = GetAttributeById(attributeValueInfo.AttrId);
            if (attributeInfo.IsFilter == 1)
            {
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_FILTERAANDVLIST + attributeInfo.CateId);
            }
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_ATTRIBUTEVALUE_INFO + attributeValueInfo.AttrValueId);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_CATEGORY_AANDVLISTJSONCACHE + attributeInfo.CateId);
        }
    }
}
