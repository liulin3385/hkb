using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台分类控制器类
    /// </summary>
    public class CategoryController : BaseMallAdminController
    {
        /// <summary>
        /// 分类列表
        /// </summary>
        public ActionResult CategoryList()
        {
            CategoryListModel model = new CategoryListModel();
            model.CategoryList = AdminCategories.AdminGetCategoryTree();
            MallUtils.SetAdminRefererCookie(Url.Action("CategoryList"));
            return View(model);
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        [HttpGet]
        public ViewResult AddCategory()
        {
            CategoryModel model = new CategoryModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        [HttpPost]
        public ActionResult AddCategory(CategoryModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.CategroyName) && AdminCategories.GetCateIdByName(model.CategroyName.Trim()) > 0)
                ModelState.AddModelError("CateName", "名称已经存在");

            if (ModelState.IsValid)
            {
                CategoryInfo categoryInfo = new CategoryInfo()
                {
                    DisplayOrder = model.DisplayOrder,
                    Name = model.CategroyName.Trim(),
                    ParentId = model.ParentCateId,
                    PriceRange = CommonHelper.StringArrayToString(CommonHelper.RemoveArrayItem(StringHelper.SplitString(CommonHelper.TBBRTrim(model.PriceRange).Replace("，", ","))))
                };

                AdminCategories.CreateCategory(categoryInfo);
                AddMallAdminLog("添加分类", "添加分类,分类为:" + model.CategroyName);
                return PromptView("分类添加成功！");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        [HttpGet]
        public ActionResult EditCategory(int cateId = -1)
        {
            CategoryInfo categortInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categortInfo == null)
                return PromptView("此分类不存在！");

            CategoryModel model = new CategoryModel();
            model.CategroyName = categortInfo.Name;
            model.ParentCateId = categortInfo.ParentId;
            model.DisplayOrder = categortInfo.DisplayOrder;
            model.PriceRange = categortInfo.PriceRange;

            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        [HttpPost]
        public ActionResult EditCategory(CategoryModel model, int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("此分类不存在！");

            if (!string.IsNullOrWhiteSpace(model.CategroyName))
            {
                int cateId2 = AdminCategories.AdminGetCateIdByName(model.CategroyName.Trim());
                if (cateId2 > 0 && cateId2 != cateId)
                    ModelState.AddModelError("CateName", "名称已经存在");
            }

            if (ModelState.IsValid)
            {
                int oldParentId = categoryInfo.ParentId;

                categoryInfo.ParentId = model.ParentCateId;
                categoryInfo.Name = model.CategroyName.Trim();
                categoryInfo.DisplayOrder = model.DisplayOrder;
                categoryInfo.PriceRange = CommonHelper.StringArrayToString(CommonHelper.RemoveArrayItem(StringHelper.SplitString(CommonHelper.TBBRTrim(model.PriceRange).Replace("，", ","))));

                AdminCategories.UpdateCategory(categoryInfo, oldParentId);
                AddMallAdminLog("修改分类", "修改分类,分类ID为:" + cateId);
                return PromptView("商品修改成功！");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        public ActionResult DelCategory(int cateId = -1)
        {
            int result = AdminCategories.DeleteCategoryById(cateId);
            if (result == 0)
                return PromptView("删除失败！请先转移或删除此分类下的分类！");
            AddMallAdminLog("删除分类", "删除分类,分类ID为:" + cateId);
            return PromptView("分类删除成功！");
        }

        /// <summary>
        /// 隐藏分类
        /// </summary>
        public ActionResult HideCategory(int cateId = -1)
        {
            int result = AdminCategories.HideCategoryById(cateId);
            if (result == 0)
                return PromptView("隐藏失败！请先转移或删除此分类下的分类！");
            AddMallAdminLog("隐藏分类", "隐藏分类,分类ID为:" + cateId);
            return PromptView("分类隐藏成功！");
        }

        /// <summary>
        /// 显示分类
        /// </summary>
        public ActionResult ShowCategory(int cateId = -1)
        {
            int result = AdminCategories.ShowCategoryById(cateId);
            if (result == 0)
                return PromptView("显示失败！请先转移或删除此分类下的分类！");
            AddMallAdminLog("显示分类", "显示分类,分类ID为:" + cateId);
            return PromptView("分类显示成功！");
        }

        private void Load()
        {
            ViewData["categoryList"] = AdminCategories.AdminGetCategoryTree();
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }




        /// <summary>
        /// 属性分组列表
        /// </summary>
        public ActionResult AttributeGroupList(int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            AttributeGroupListModel model = new AttributeGroupListModel()
            {
                AttributeGroupList = AdminCategories.GetAttributeGroupListByCateId(cateId),
                CateId = cateId,
                CategoryName = categoryInfo.Name
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?cateId={1}", Url.Action("AttributeGroupList"), cateId));
            return View(model);
        }

        /// <summary>
        /// 添加属性分组
        /// </summary>
        [HttpGet]
        public ActionResult AddAttributeGroup(int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            AttributeGroupModel model = new AttributeGroupModel();
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加属性分组
        /// </summary>
        [HttpPost]
        public ActionResult AddAttributeGroup(AttributeGroupModel model, int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            if (!string.IsNullOrWhiteSpace(model.AttributeGroupName) && AdminCategories.GetAttributeGroupIdByCateIdAndName(cateId, model.AttributeGroupName.Trim()) > 0)
                ModelState.AddModelError("AttributeGroupName", "名称已经存在");

            if (ModelState.IsValid)
            {
                AttributeGroupInfo attributeGroupInfo = new AttributeGroupInfo()
                {
                    Name = model.AttributeGroupName.Trim(),
                    CateId = categoryInfo.CateId,
                    DisplayOrder = model.DisplayOrder
                };

                AdminCategories.CreateAttributeGroup(attributeGroupInfo);
                AddMallAdminLog("添加属性分组", "添加属性分组,属性分组为:" + model.AttributeGroupName);
                return PromptView("属性分组添加成功！");
            }
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑属性分组
        /// </summary>
        [HttpGet]
        public ActionResult EditAttributeGroup(int attrGroupId = -1)
        {
            AttributeGroupInfo attributeGroupInfo = AdminCategories.GetAttributeGroupById(attrGroupId);
            if (attributeGroupInfo == null)
                return PromptView("属性分组不存在！");

            AttributeGroupModel model = new AttributeGroupModel();
            model.AttributeGroupName = attributeGroupInfo.Name;
            model.DisplayOrder = attributeGroupInfo.DisplayOrder;
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(attributeGroupInfo.CateId);
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑属性分组
        /// </summary>
        [HttpPost]
        public ActionResult EditAttributeGroup(AttributeGroupModel model, int attrGroupId = -1)
        {
            AttributeGroupInfo attributeGroupInfo = AdminCategories.GetAttributeGroupById(attrGroupId);
            if (attributeGroupInfo == null)
                return PromptView("属性分组不存在！");

            if (!string.IsNullOrWhiteSpace(model.AttributeGroupName))
            {
                int attrGroupId2 = AdminCategories.GetAttributeGroupIdByCateIdAndName(attributeGroupInfo.CateId, model.AttributeGroupName.Trim());
                if (attrGroupId2 > 0 && attrGroupId2 != attrGroupId)
                    ModelState.AddModelError("AttributeGroupName", "名称已经存在");
            }

            if (ModelState.IsValid)
            {
                attributeGroupInfo.Name = model.AttributeGroupName.Trim();
                attributeGroupInfo.DisplayOrder = model.DisplayOrder;

                AdminCategories.UpdateAttributeGroup(attributeGroupInfo);
                AddMallAdminLog("修改属性分组", "修改属性分组,属性分组ID为:" + attrGroupId);
                return PromptView("属性分组修改成功！");
            }

            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(attributeGroupInfo.CateId);
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除属性分组
        /// </summary>
        public ActionResult DelAttributeGroup(int attrGroupId = -1)
        {
            AdminCategories.DeleteAttributeGroupById(attrGroupId);
            AddMallAdminLog("删除属性分组", "删除属性分组,属性分组ID为:" + attrGroupId);
            return PromptView("属性分组删除成功！");
        }







        /// <summary>
        /// 属性列表
        /// </summary>
        public ActionResult AttributeList(string sortColumn, string sortDirection, int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            string sort = AdminBrands.AdminGetBrandListSort(sortColumn, sortDirection);

            AttributeListModel model = new AttributeListModel()
            {
                AttributeList = AdminCategories.AdminGetAttributeList(cateId, sort),
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CateId = cateId,
                CategoryName = categoryInfo.Name
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?cateId={1}", Url.Action("AttributeList"), cateId));
            return View(model);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        [HttpGet]
        public ActionResult AddAttribute(int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            AttributeModel model = new AttributeModel();
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["AttributeGroupList"] = GetAttributeGroupSelectList(cateId);
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        [HttpPost]
        public ActionResult AddAttribute(AttributeModel model, int cateId = -1)
        {
            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("分类不存在！");

            if (!string.IsNullOrWhiteSpace(model.AttributName) && AdminCategories.GetAttrIdByCateIdAndName(cateId, model.AttributName.Trim()) > 0)
                ModelState.AddModelError("AttributName", "名称已经存在");

            AttributeGroupInfo attributeGroupInfo = AdminCategories.GetAttributeGroupById(model.AttrGroupId);
            if (attributeGroupInfo == null || attributeGroupInfo.CateId != cateId)
                ModelState.AddModelError("AttrGroupId", "属性组不存在");

            if (ModelState.IsValid)
            {
                AttributeInfo attributeInfo = new AttributeInfo();
                attributeInfo.Name = model.AttributName.Trim();
                attributeInfo.CateId = cateId;
                attributeInfo.AttrGroupId = model.AttrGroupId;
                attributeInfo.ShowType = model.ShowType;
                attributeInfo.IsFilter = model.IsFilter;
                attributeInfo.DisplayOrder = model.DisplayOrder;

                AdminCategories.CreateAttribute(attributeInfo, attributeGroupInfo);
                AddMallAdminLog("添加分类属性", "添加分类属性,属性为:" + model.AttributName);
                return PromptView("分类属性添加成功！");
            }
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["AttributeGroupList"] = GetAttributeGroupSelectList(cateId);
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        [HttpGet]
        public ActionResult EditAttribute(int attrId = -1)
        {
            AttributeInfo attributeInfo = AdminCategories.GetAttributeById(attrId);
            if (attributeInfo == null)
                return PromptView("属性不存在！");

            AttributeModel model = new AttributeModel();
            model.AttributName = attributeInfo.Name;
            model.AttrGroupId = attributeInfo.AttrGroupId;
            model.ShowType = attributeInfo.ShowType;
            model.IsFilter = attributeInfo.IsFilter;
            model.DisplayOrder = attributeInfo.DisplayOrder;

            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(attributeInfo.CateId);
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["AttributeGroupList"] = GetAttributeGroupSelectList(categoryInfo.CateId);
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        [HttpPost]
        public ActionResult EditAttribute(AttributeModel model, int attrId = -1)
        {
            AttributeInfo attributeInfo = AdminCategories.GetAttributeById(attrId);
            if (attributeInfo == null)
                return PromptView("属性不存在！");

            if (!string.IsNullOrWhiteSpace(model.AttributName))
            {
                int attrId2 = AdminCategories.GetAttrIdByCateIdAndName(attributeInfo.CateId, model.AttributName.Trim());
                if (attrId2 > 0 && attrId2 != attrId)
                    ModelState.AddModelError("AttributName", "名称已经存在");
            }

            AttributeGroupInfo attributeGroupInfo = AdminCategories.GetAttributeGroupById(model.AttrGroupId);
            if (attributeGroupInfo == null || attributeGroupInfo.CateId != attributeInfo.CateId)
                ModelState.AddModelError("AttrGroupId", "属性组不存在");

            if (ModelState.IsValid)
            {
                attributeInfo.Name = model.AttributName;
                attributeInfo.AttrGroupId = model.AttrGroupId;
                attributeInfo.IsFilter = model.IsFilter;
                attributeInfo.ShowType = model.ShowType;
                attributeInfo.DisplayOrder = model.DisplayOrder;

                AdminCategories.UpdateAttribute(attributeInfo);
                AddMallAdminLog("编辑分类属性", "编辑分类属性,分类属性ID为:" + attrId);
                return PromptView("分类属性修改成功！");
            }

            CategoryInfo categoryInfo = AdminCategories.AdminGetCategoryById(attributeInfo.CateId);
            ViewData["CateId"] = categoryInfo.CateId;
            ViewData["CategoryName"] = categoryInfo.Name;
            ViewData["AttributeGroupList"] = GetAttributeGroupSelectList(categoryInfo.CateId);
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        public ActionResult DelAttribute(int attrId)
        {
            AdminCategories.DeleteAttributeById(attrId);
            AddMallAdminLog("删除商品属性", "删除商品属性,属性ID为:" + attrId);
            return PromptView("商品属性删除成功！");
        }

        /// <summary>
        /// 获得分类的属性及其值JSON列表
        /// </summary>
        /// <param name="cateId">分类id</param>
        /// <returns></returns>
        public ContentResult AANDVJSONList(int cateId = -1)
        {
            return Content(AdminCategories.GetCategoryAAndVListJSONCache(cateId));
        }

        private List<SelectListItem> GetAttributeGroupSelectList(int cateId)
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            List<AttributeGroupInfo> attributeGroupList = AdminCategories.GetAttributeGroupListByCateId(cateId);
            itemList.Add(new SelectListItem() { Text = "请选择分类", Value = "0" });
            foreach (AttributeGroupInfo attributeGroupInfo in attributeGroupList)
            {
                itemList.Add(new SelectListItem() { Text = attributeGroupInfo.Name, Value = attributeGroupInfo.AttrGroupId.ToString() });
            }
            return itemList;
        }







        /// <summary>
        /// 属性值列表
        /// </summary>
        public ActionResult AttributeValueList(int attrId = -1)
        {
            AttributeInfo attributeInfo = AdminCategories.GetAttributeById(attrId);
            if (attributeInfo == null)
                return PromptView("属性不存在！");

            AttributeValueListModel model = new AttributeValueListModel()
            {
                AttributeValueList = AdminCategories.GetAttributeSelectValueListByAttrId(attrId),
                AttrId = attributeInfo.AttrId,
                AttributeName = attributeInfo.Name
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?attrId={1}", Url.Action("AttributeValueList"), attrId));
            return View(model);
        }

        /// <summary>
        /// 添加属性值
        /// </summary>
        [HttpGet]
        public ActionResult AddAttributeValue(int attrId = -1)
        {
            AttributeInfo attributeInfo = AdminCategories.GetAttributeById(attrId);
            if (attributeInfo == null)
                return PromptView("属性不存在！");

            AttributeValueModel model = new AttributeValueModel();
            ViewData["AttrId"] = attributeInfo.AttrId;
            ViewData["AttributeName"] = attributeInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加属性值
        /// </summary>
        [HttpPost]
        public ActionResult AddAttributeValue(AttributeValueModel model, int attrId = -1)
        {
            AttributeInfo attributeInfo = AdminCategories.GetAttributeById(attrId);
            if (attributeInfo == null)
                ModelState.AddModelError("AttributName", "属性不存在");

            if (!string.IsNullOrWhiteSpace(model.AttrValue) && AdminCategories.GetAttributeValueIdByAttrIdAndValue(attrId, model.AttrValue.Trim()) > 0)
                ModelState.AddModelError("AttributName", "值已经存在");


            if (ModelState.IsValid)
            {
                AttributeGroupInfo attributeGroupInfo = AdminCategories.GetAttributeGroupById(attributeInfo.AttrGroupId);
                AttributeValueInfo attributeValueInfo = new AttributeValueInfo();

                attributeValueInfo.AttrId = attributeInfo.AttrId;
                attributeValueInfo.AttrName = attributeInfo.Name;
                attributeValueInfo.AttrDisplayOrder = attributeInfo.DisplayOrder;
                attributeValueInfo.AttrShowType = attributeInfo.ShowType;

                attributeValueInfo.AttrGroupId = attributeGroupInfo.AttrGroupId;
                attributeValueInfo.AttrGroupName = attributeGroupInfo.Name;
                attributeValueInfo.AttrGroupDisplayOrder = attributeGroupInfo.DisplayOrder;

                attributeValueInfo.AttrValue = model.AttrValue.Trim();
                attributeValueInfo.IsInput = 0;
                attributeValueInfo.AttrValueDisplayOrder = model.DisplayOrder;

                AdminCategories.CreateAttributeValue(attributeValueInfo);
                AddMallAdminLog("添加属性值", "添加属性值,属性值为:" + model.AttrValue);
                return PromptView("属性值添加成功！");
            }
            ViewData["AttrId"] = attributeInfo.AttrId;
            ViewData["AttributeName"] = attributeInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        [HttpGet]
        public ActionResult EditAttributeValue(int attrValueId = -1)
        {
            AttributeValueInfo attributeValueInfo = AdminCategories.GetAttributeValueById(attrValueId);
            if (attributeValueInfo == null)
                return PromptView("属性值不存在！");
            if (attributeValueInfo.IsInput == 1)
                return PromptView("输入型属性值不能修改！");

            AttributeValueModel model = new AttributeValueModel();
            model.AttrValue = attributeValueInfo.AttrValue;
            model.DisplayOrder = attributeValueInfo.AttrValueDisplayOrder;

            AttributeInfo attributeInfo = Categories.GetAttributeById(attributeValueInfo.AttrId);
            ViewData["AttrId"] = attributeInfo.AttrId;
            ViewData["AttributeName"] = attributeInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        [HttpPost]
        public ActionResult EditAttributeValue(AttributeValueModel model, int attrValueId = 0)
        {
            AttributeValueInfo attributeValueInfo = AdminCategories.GetAttributeValueById(attrValueId);
            if (attributeValueInfo == null)
                return PromptView("属性值不存在！");
            if (attributeValueInfo.IsInput == 1)
                return PromptView("输入型属性值不能修改！");

            if (!string.IsNullOrWhiteSpace(model.AttrValue))
            {
                int attrValueId2 = AdminCategories.GetAttributeValueIdByAttrIdAndValue(attributeValueInfo.AttrId, model.AttrValue.Trim());
                if (attrValueId2 > 0 && attrValueId2 != attrValueId)
                    ModelState.AddModelError("AttrValue", "值已经存在");
            }

            if (ModelState.IsValid)
            {
                attributeValueInfo.AttrValue = model.AttrValue;
                attributeValueInfo.AttrValueDisplayOrder = model.DisplayOrder;
                AdminCategories.UpdateAttributeValue(attributeValueInfo);
                AddMallAdminLog("修改属性值", "修改属性值,属性值ID为:" + attrValueId);
                return PromptView("属性值修改成功！");
            }

            AttributeInfo attributeInfo = Categories.GetAttributeById(attributeValueInfo.AttrId);
            ViewData["AttrId"] = attributeInfo.AttrId;
            ViewData["AttributeName"] = attributeInfo.Name;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 删除属性值
        /// </summary>
        public ActionResult DelAttributeValue(int attrValueId = -1)
        {
            AdminCategories.DeleteAttributeValueById(attrValueId);
            AddMallAdminLog("删除属性值", "删除属性值,属性值ID为:" + attrValueId);
            return PromptView("属性值删除成功！");
        }
    }
}

