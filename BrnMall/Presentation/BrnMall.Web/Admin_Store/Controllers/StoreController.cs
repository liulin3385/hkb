using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.StoreAdmin.Models;

namespace BrnMall.Web.StoreAdmin.Controllers
{
    /// <summary>
    /// 店铺后台店铺控制器类
    /// </summary>
    public class StoreController : BaseStoreAdminController
    {
        /// <summary>
        /// 编辑店铺
        /// </summary>
        [HttpGet]
        public ActionResult EditStore()
        {
            StoreModel model = new StoreModel();

            model.StoreName = WorkContext.StoreInfo.Name;
            model.RegionId = WorkContext.StoreInfo.RegionId;
            model.StoreIid = WorkContext.StoreInfo.StoreIid;
            model.Logo = WorkContext.StoreInfo.Logo;
            model.Mobile = WorkContext.StoreInfo.Mobile;
            model.Phone = WorkContext.StoreInfo.Phone;
            model.QQ = WorkContext.StoreInfo.QQ;
            model.WW = WorkContext.StoreInfo.WW;
            model.Theme = WorkContext.StoreInfo.Theme;
            model.Banner = WorkContext.StoreInfo.Banner;
            model.Announcement = WorkContext.StoreInfo.Announcement;
            model.Description = WorkContext.StoreInfo.Description;

            LoadStore(model.RegionId);
            return View(model);
        }

        /// <summary>
        /// 编辑店铺
        /// </summary>
        [HttpPost]
        public ActionResult EditStore(StoreModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.StoreName))
            {
                int storeId = AdminStores.GetStoreIdByName(model.StoreName.Trim());
                if (storeId > 0 && storeId != WorkContext.StoreId)
                    ModelState.AddModelError("StoreName", "店铺名已经存在");
            }

            if (ModelState.IsValid)
            {
                WorkContext.StoreInfo.Name = model.StoreName.Trim();
                WorkContext.StoreInfo.RegionId = model.RegionId;
                WorkContext.StoreInfo.StoreIid = model.StoreIid;
                WorkContext.StoreInfo.Logo = model.Logo == null ? "" : model.Logo.Trim();
                WorkContext.StoreInfo.Mobile = model.Mobile == null ? "" : model.Mobile.Trim();
                WorkContext.StoreInfo.Phone = model.Phone == null ? "" : model.Phone.Trim();
                WorkContext.StoreInfo.QQ = model.QQ == null ? "" : model.QQ.Trim();
                WorkContext.StoreInfo.WW = model.WW == null ? "" : model.WW.Trim();
                WorkContext.StoreInfo.Theme = model.Theme;
                WorkContext.StoreInfo.Banner = model.Banner == null ? "" : model.Banner.Trim();
                WorkContext.StoreInfo.Announcement = model.Announcement == null ? "" : model.Announcement.Trim();
                WorkContext.StoreInfo.Description = model.Description == null ? "" : model.Description;

                AdminStores.UpdateStore(WorkContext.StoreInfo);
                AddStoreAdminLog("修改店铺", "修改店铺");
                return PromptView("店铺修改成功！");
            }

            LoadStore(model.RegionId);
            return View(model);
        }

        private void LoadStore(int regionId)
        {
            List<SelectListItem> storeIndustryList = new List<SelectListItem>();
            storeIndustryList.Add(new SelectListItem() { Text = "选择店铺行业", Value = "-1" });
            foreach (StoreIndustryInfo storeIndustryInfo in AdminStoreIndustries.GetStoreIndustryList())
            {
                storeIndustryList.Add(new SelectListItem() { Text = storeIndustryInfo.Title, Value = storeIndustryInfo.StoreIid.ToString() });
            }
            ViewData["storeIndustryList"] = storeIndustryList;

            List<SelectListItem> themeList = new List<SelectListItem>();
            DirectoryInfo dir = new DirectoryInfo(IOHelper.GetMapPath("/Themes"));
            foreach (DirectoryInfo themeDir in dir.GetDirectories())
            {
                themeList.Add(new SelectListItem() { Text = themeDir.Name, Value = themeDir.Name });
            }
            ViewData["themeList"] = themeList;

            RegionInfo regionInfo = Regions.GetRegionById(regionId);
            if (regionInfo != null)
            {
                ViewData["provinceId"] = regionInfo.ProvinceId;
                ViewData["cityId"] = regionInfo.CityId;
                ViewData["countyId"] = regionInfo.RegionId;
            }
            else
            {
                ViewData["provinceId"] = -1;
                ViewData["cityId"] = -1;
                ViewData["countyId"] = -1;
            }

            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.StoreLogoThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["AllowImgType"] = allowImgType;
            ViewData["MaxImgSize"] = BMAConfig.MallConfig.UploadImgSize;

            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }





        /// <summary>
        /// 店铺分类列表
        /// </summary>
        public ActionResult StoreClassList()
        {
            StoreClassListModel model = new StoreClassListModel()
            {
                StoreClassList = AdminStores.GetStoreClassList(WorkContext.StoreId)
            };
            MallUtils.SetAdminRefererCookie(Url.Action("StoreClassList"));
            return View(model);
        }

        /// <summary>
        /// 添加店铺分类
        /// </summary>
        [HttpGet]
        public ViewResult AddStoreClass()
        {
            StoreClassModel model = new StoreClassModel();
            LoadStoreClass(WorkContext.StoreId);
            return View(model);
        }

        /// <summary>
        /// 添加店铺分类
        /// </summary>
        [HttpPost]
        public ActionResult AddStoreClass(StoreClassModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.StoreClassName) && AdminStores.GetStoreCidByStoreIdAndName(WorkContext.StoreId, model.StoreClassName.Trim()) > 0)
                ModelState.AddModelError("StoreClassName", "名称已经存在");

            if (ModelState.IsValid)
            {
                StoreClassInfo storeClassInfo = new StoreClassInfo()
                {
                    StoreId = WorkContext.StoreId,
                    DisplayOrder = model.DisplayOrder,
                    Name = model.StoreClassName.Trim(),
                    ParentId = model.ParentId
                };

                AdminStores.CreateStoreClass(storeClassInfo);
                AddStoreAdminLog("添加店铺分类", "添加店铺分类,店铺分类为:" + model.StoreClassName);
                return PromptView("店铺分类添加成功！");
            }

            LoadStoreClass(WorkContext.StoreId);
            return View(model);
        }

        /// <summary>
        /// 编辑店铺分类
        /// </summary>
        [HttpGet]
        public ActionResult EditStoreClass(int storeCid = -1)
        {
            StoreClassInfo storeClassInfo = AdminStores.GetStoreClassByStoreIdAndStoreCid(WorkContext.StoreId, storeCid);
            if (storeClassInfo == null)
                return PromptView("此店铺分类不存在！");

            StoreClassModel model = new StoreClassModel();
            model.StoreClassName = storeClassInfo.Name;
            model.ParentId = storeClassInfo.ParentId;
            model.DisplayOrder = storeClassInfo.DisplayOrder;

            LoadStoreClass(WorkContext.StoreId);
            return View(model);
        }

        /// <summary>
        /// 编辑店铺分类
        /// </summary>
        [HttpPost]
        public ActionResult EditStoreClass(StoreClassModel model, int storeCid = -1)
        {
            StoreClassInfo storeClassInfo = AdminStores.GetStoreClassByStoreIdAndStoreCid(WorkContext.StoreId, storeCid);
            if (storeClassInfo == null)
                return PromptView("此店铺分类不存在！");

            if (!string.IsNullOrWhiteSpace(model.StoreClassName))
            {
                int storeCid2 = AdminStores.GetStoreCidByStoreIdAndName(WorkContext.StoreId, model.StoreClassName.Trim());
                if (storeCid2 > 0 && storeCid2 != storeCid)
                    ModelState.AddModelError("StoreClassName", "名称已经存在");
            }

            if (ModelState.IsValid)
            {
                int oldParentId = storeClassInfo.ParentId;

                storeClassInfo.ParentId = model.ParentId;
                storeClassInfo.Name = model.StoreClassName.Trim();
                storeClassInfo.DisplayOrder = model.DisplayOrder;

                AdminStores.UpdateStoreClass(storeClassInfo, oldParentId);
                AddStoreAdminLog("修改店铺分类", "修改店铺分类,店铺分类ID为:" + storeCid);
                return PromptView("商品修改成功！");
            }

            LoadStoreClass(WorkContext.StoreId);
            return View(model);
        }

        /// <summary>
        /// 删除店铺分类
        /// </summary>
        public ActionResult DelStoreClass(int storeCid = -1)
        {
            int result = AdminStores.DeleteStoreClassByStoreIdAndStoreCid(WorkContext.StoreId, storeCid);
            if (result == -1)
                return PromptView("此店铺分类不存在");
            else if (result == 0)
                return PromptView("删除失败！请先转移或删除此店铺分类下的店铺分类！");
            else if (result == -2)
                return PromptView("删除失败！请先转移或删除此店铺分类下的商品！");
            AddStoreAdminLog("删除店铺分类", "删除店铺分类,店铺分类ID为:" + storeCid);
            return PromptView("店铺分类删除成功！");
        }

        private void LoadStoreClass(int storeId)
        {
            ViewData["storeClassList"] = AdminStores.GetStoreClassList(storeId);
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }





        /// <summary>
        /// 店铺配送模板列表
        /// </summary>
        public ActionResult StoreShipTemplateList()
        {
            StoreShipTemplateListModel model = new StoreShipTemplateListModel()
            {
                StoreShipTemplateList = AdminStores.GetStoreShipTemplateList(WorkContext.StoreId)
            };

            MallUtils.SetAdminRefererCookie(Url.Action("StoreShipTemplateList"));
            return View(model);
        }

        /// <summary>
        /// 添加店铺配送模板
        /// </summary>
        [HttpGet]
        public ActionResult AddStoreShipTemplate()
        {
            AddStoreShipTemplateModel model = new AddStoreShipTemplateModel();
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加店铺配送模板
        /// </summary>
        [HttpPost]
        public ActionResult AddStoreShipTemplate(AddStoreShipTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                StoreShipTemplateInfo storeShipTemplateInfo = new StoreShipTemplateInfo()
                {
                    StoreId = WorkContext.StoreId,
                    Title = model.TemplateTitle.Trim(),
                    Free = model.Free,
                    Type = model.Type
                };

                int storeSTid = AdminStores.CreateStoreShipTemplate(storeShipTemplateInfo);
                if (storeSTid > 0)
                {
                    StoreShipFeeInfo storeShipFeeInfo = new StoreShipFeeInfo()
                    {
                        StoreSTid = storeSTid,
                        RegionId = -1,
                        StartValue = model.StartValue,
                        StartFee = model.StartFee,
                        AddValue = model.AddValue,
                        AddFee = model.AddFee
                    };
                    AdminStores.CreateStoreShipFee(storeShipFeeInfo);
                    AddStoreAdminLog("添加店铺配送模板", "添加店铺配送模板,店铺配送模板为:" + model.TemplateTitle);
                    return PromptView("店铺配送模板添加成功！");
                }
                else
                {
                    return PromptView("店铺配送模板添加失败！");
                }
            }
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑店铺配送模板
        /// </summary>
        [HttpGet]
        public ActionResult EditStoreShipTemplate(int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            EditStoreShipTemplateModel model = new EditStoreShipTemplateModel();
            model.TemplateTitle = storeShipTemplateInfo.Title;
            model.Free = storeShipTemplateInfo.Free;
            model.Type = storeShipTemplateInfo.Type;

            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑店铺配送模板
        /// </summary>
        [HttpPost]
        public ActionResult EditStoreShipTemplate(EditStoreShipTemplateModel model, int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            if (ModelState.IsValid)
            {
                storeShipTemplateInfo.Title = model.TemplateTitle.Trim();
                storeShipTemplateInfo.Free = model.Free;
                storeShipTemplateInfo.Type = model.Type;

                AdminStores.UpdateStoreShipTemplate(storeShipTemplateInfo);
                AddStoreAdminLog("修改店铺配送模板", "修改店铺配送模板,店铺配送模板ID为:" + storeSTid);
                return PromptView("店铺配送模板修改成功！");
            }

            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除店铺配送模板
        /// </summary>
        public ActionResult DelStoreShipTemplate(int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            int result = AdminStores.DeleteStoreShipTemplateById(storeSTid);
            if (result == -1)
                return PromptView("请先移除此模板下的商品！");
            AddStoreAdminLog("删除店铺配送模板", "删除店铺配送模板,店铺配送模板ID为:" + storeSTid);
            return PromptView("店铺配送模板删除成功！");
        }





        /// <summary>
        /// 店铺配送费用列表
        /// </summary>
        public ActionResult StoreShipFeeList(int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            StoreShipFeeListModel model = new StoreShipFeeListModel()
            {
                StoreSTid = storeSTid,
                StoreShipFeeList = AdminStores.AdminGetStoreShipFeeList(storeSTid)
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?storeSTid={1}", Url.Action("StoreShipFeeList"), storeSTid));
            return View(model);
        }

        /// <summary>
        /// 添加店铺配送费用
        /// </summary>
        [HttpGet]
        public ActionResult AddStoreShipFee(int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            StoreShipFeeModel model = new StoreShipFeeModel();
            LoadStoreShipFee(0);
            return View(model);
        }

        /// <summary>
        /// 添加店铺配送费用
        /// </summary>
        [HttpPost]
        public ActionResult AddStoreShipFee(StoreShipFeeModel model, int storeSTid = -1)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            if (ModelState.IsValid)
            {
                StoreShipFeeInfo storeShipFeeInfo = new StoreShipFeeInfo()
                {
                    StoreSTid = storeSTid,
                    RegionId = model.RegionId,
                    StartValue = model.StartValue,
                    StartFee = model.StartFee,
                    AddValue = model.AddValue,
                    AddFee = model.AddFee
                };
                AdminStores.CreateStoreShipFee(storeShipFeeInfo);
                AddStoreAdminLog("添加店铺配送费用", "添加店铺配送费用");
                return PromptView("店铺配送费用添加成功！");
            }
            LoadStoreShipFee(model.RegionId);
            return View(model);
        }

        /// <summary>
        /// 编辑店铺配送费用
        /// </summary>
        [HttpGet]
        public ActionResult EditStoreShipFee(int recordId = -1)
        {
            StoreShipFeeInfo storeShipFeeInfo = AdminStores.GetStoreShipFeeById(recordId);
            if (storeShipFeeInfo == null)
                return PromptView("店铺配送费用不存在！");
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeShipFeeInfo.StoreSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            StoreShipFeeModel model = new StoreShipFeeModel();
            model.RegionId = storeShipFeeInfo.RegionId;
            model.StartValue = storeShipFeeInfo.StartValue;
            model.StartFee = storeShipFeeInfo.StartFee;
            model.AddValue = storeShipFeeInfo.AddValue;
            model.AddFee = storeShipFeeInfo.AddFee;

            LoadStoreShipFee(model.RegionId);
            return View(model);
        }

        /// <summary>
        /// 编辑店铺配送费用
        /// </summary>
        [HttpPost]
        public ActionResult EditStoreShipFee(StoreShipFeeModel model, int recordId = -1)
        {
            StoreShipFeeInfo storeShipFeeInfo = AdminStores.GetStoreShipFeeById(recordId);
            if (storeShipFeeInfo == null)
                return PromptView("店铺配送费用不存在！");
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeShipFeeInfo.StoreSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");

            if (ModelState.IsValid)
            {
                storeShipFeeInfo.RegionId = model.RegionId;
                storeShipFeeInfo.StartValue = model.StartValue;
                storeShipFeeInfo.StartFee = model.StartFee;
                storeShipFeeInfo.AddValue = model.AddValue;
                storeShipFeeInfo.AddFee = model.AddFee;

                AdminStores.UpdateStoreShipFee(storeShipFeeInfo);
                AddStoreAdminLog("修改店铺配送费用", "修改店铺配送费用,店铺配送费用ID为:" + recordId);
                return PromptView("店铺配送费用修改成功！");
            }

            LoadStoreShipFee(model.RegionId);
            return View(model);
        }

        /// <summary>
        /// 删除店铺配送费用
        /// </summary>
        public ActionResult DelStoreShipFee(int recordId = -1)
        {
            StoreShipFeeInfo storeShipFeeInfo = AdminStores.GetStoreShipFeeById(recordId);
            if (storeShipFeeInfo == null)
                return PromptView("店铺配送费用不存在！");
            StoreShipTemplateInfo storeShipTemplateInfo = AdminStores.GetStoreShipTemplateById(storeShipFeeInfo.StoreSTid);
            if (storeShipTemplateInfo == null)
                return PromptView("店铺配送模板不存在！");
            if (storeShipTemplateInfo.StoreId != WorkContext.StoreId)
                return PromptView("不能操作其它店铺的配送模板！");
            if (storeShipFeeInfo.RegionId == -1)
                return PromptView("默认店铺配送费用不能删除！");

            AdminStores.DeleteStoreShipFeeById(recordId);
            AddStoreAdminLog("删除店铺配送费用", "删除店铺配送费用,店铺配送费用ID为:" + recordId);
            return PromptView("店铺配送费用删除成功！");
        }

        private void LoadStoreShipFee(int regionId)
        {
            RegionInfo regionInfo = Regions.GetRegionById(regionId);
            if (regionInfo != null)
            {
                if (regionInfo.Layer == 1)
                {
                    ViewData["provinceId"] = regionInfo.ProvinceId;
                    ViewData["cityId"] = 0;
                }
                else
                {
                    RegionInfo parentRegionInfo = Regions.GetRegionById(regionInfo.ParentId);
                    ViewData["provinceId"] = parentRegionInfo.ProvinceId;
                    ViewData["cityId"] = regionInfo.RegionId;
                }
            }
            else
            {
                ViewData["provinceId"] = 0;
                ViewData["cityId"] = 0;
            }

            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }
    }
}
