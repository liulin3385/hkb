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
    /// 商城后台品牌控制器类
    /// </summary>
    public class BrandController : BaseMallAdminController
    {
        /// <summary>
        /// 品牌列表
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult List(string brandName, string sortColumn, string sortDirection, int pageSize = 15, int pageNumber = 1)
        {
            string condition = AdminBrands.AdminGetBrandListCondition(brandName);
            string sort = AdminBrands.AdminGetBrandListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminBrands.AdminGetBrandCount(condition));

            BrandListModel model = new BrandListModel()
            {
                BrandList = AdminBrands.AdminGetBrandList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                BrandName = brandName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&brandName={5}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn,
                                                          sortDirection,
                                                          brandName));
            return View(model);
        }

        /// <summary>
        /// 品牌选择列表
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ContentResult SelectList(string brandName, int pageNumber = 1, int pageSize = 24)
        {
            string condition = AdminBrands.AdminGetBrandListCondition(brandName);
            if (condition.Length == 0)
            {
                condition = " [isshow]=1 ";
            }
            else
            {
                condition = string.Format(" {0} AND [isshow]=1 ", condition);
            }

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminBrands.AdminGetBrandCount(condition));

            DataTable brandSelectList = AdminBrands.AdminGetBrandSelectList(pageModel.PageSize, pageModel.PageNumber, condition);

            StringBuilder result = new StringBuilder("({");
            result.AppendFormat("\"count\":\"{0}\",\"page\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (DataRow row in brandSelectList.Rows)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", row["brandid"], row["name"].ToString().Trim(), "}");

            if (brandSelectList.Rows.Count > 0)
                result.Remove(result.Length - 1, 1);

            result.Append("]})");
            return Content(result.ToString());
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            BrandModel model = new BrandModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        [HttpPost]
        public ActionResult Add(BrandModel model)
        {
            if (model.BrandName != null && AdminBrands.AdminGetBrandIdByName(model.BrandName) > 0)
                ModelState.AddModelError("BrandName", "名称已经存在");

            if (ModelState.IsValid)
            {
                BrandInfo brandInfo = new BrandInfo()
                {
                    IsShow = 1,
                    DisplayOrder = model.DisplayOrder,
                    Name = model.BrandName.Trim(),
                    Logo = model.Logo
                };

                AdminBrands.CreateBrand(brandInfo);
                AddMallAdminLog("添加品牌", "添加品牌,品牌为:" + model.BrandName);
                return PromptView("品牌添加成功！");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑品牌
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int brandId = -1)
        {
            BrandInfo brandInfo = AdminBrands.AdminGetBrandById(brandId);
            if (brandInfo == null)
                return PromptView("品牌不存在！");

            BrandModel model = new BrandModel();
            model.DisplayOrder = brandInfo.DisplayOrder;
            model.BrandName = brandInfo.Name;
            model.Logo = brandInfo.Logo;
            Load();

            return View(model);
        }

        /// <summary>
        /// 编辑品牌
        /// </summary>
        [HttpPost]
        public ActionResult Edit(BrandModel model, int brandId = -1)
        {
            BrandInfo brandInfo = AdminBrands.AdminGetBrandById(brandId);
            if (brandInfo == null)
                return PromptView("品牌不存在！");

            if (model.BrandName != null)
            {
                int brandId2 = AdminBrands.AdminGetBrandIdByName(model.BrandName.Trim());
                if (brandId2 > 0 && brandId2 != brandId)
                    ModelState.AddModelError("BrandName", "名称已经存在");
            }

            if (ModelState.IsValid)
            {
                brandInfo.DisplayOrder = model.DisplayOrder;
                brandInfo.Name = model.BrandName.Trim();
                brandInfo.Logo = model.Logo;

                AdminBrands.UpdateBrand(brandInfo);
                AddMallAdminLog("修改品牌", "修改品牌,品牌ID为:" + brandId);
                return PromptView("品牌修改成功！");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        public ActionResult Del(int brandId = -1)
        {
            AdminBrands.DeleteBrandById(brandId);
            AddMallAdminLog("删除品牌", "删除品牌,品牌ID为:" + brandId);
            return PromptView("品牌删除成功！");
        }

        /// <summary>
        /// 隐藏品牌
        /// </summary>
        public ActionResult Hide(int brandId = -1)
        {
            AdminBrands.HideBrandById(brandId);
            AddMallAdminLog("隐藏品牌", "隐藏品牌,品牌ID为:" + brandId);
            return PromptView("品牌隐藏成功！");
        }

        /// <summary>
        /// 显示品牌
        /// </summary>
        public ActionResult Show(int brandId = -1)
        {
            AdminBrands.ShowBrandById(brandId);
            AddMallAdminLog("显示品牌", "显示品牌,品牌ID为:" + brandId);
            return PromptView("品牌显示成功！");
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.BrandThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["AllowImgType"] = allowImgType;
            ViewData["MaxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }
    }
}
