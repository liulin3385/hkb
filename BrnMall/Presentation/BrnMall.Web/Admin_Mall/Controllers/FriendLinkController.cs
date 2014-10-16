﻿using System;
using System.Web;
using System.Web.Mvc;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台友情链接控制器类
    /// </summary>
    public class FriendLinkController : BaseMallAdminController
    {
        /// <summary>
        /// 友情链接列表
        /// </summary>
        public ActionResult List()
        {
            FriendLinkListModel model = new FriendLinkListModel();
            model.FriendLinkList = AdminFriendLinks.GetFriendLinkList();

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.BrandThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            MallUtils.SetAdminRefererCookie(Url.Action("list"));
            return View(model);
        }

        /// <summary>
        /// 添加友情链接
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            FriendLinkModel model = new FriendLinkModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加友情链接
        /// </summary>
        [HttpPost]
        public ActionResult Add(FriendLinkModel model)
        {
            if (ModelState.IsValid)
            {
                FriendLinkInfo friendLinkInfo = new FriendLinkInfo()
                {
                    Name = model.FriendLinkName.Trim(),
                    Title = model.FriendLinkTitle == null ? "" : model.FriendLinkTitle.Trim(),
                    Logo = model.FriendLinkLogo == null ? "" : model.FriendLinkLogo.Trim(),
                    Url = model.FriendLinkUrl.Trim(),
                    Target = model.Target,
                    DisplayOrder = model.DisplayOrder
                };

                AdminFriendLinks.CreateFriendLink(friendLinkInfo);
                AddMallAdminLog("添加友情链接", "添加友情链接,友情链接为:" + model.FriendLinkName);
                return PromptView("友情链接添加成功！");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑友情链接
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            FriendLinkInfo friendLinkInfo = AdminFriendLinks.GetFriendLinkById(id);
            if (friendLinkInfo == null)
                return PromptView("友情链接不存在！");

            FriendLinkModel model = new FriendLinkModel();
            model.FriendLinkName = friendLinkInfo.Name;
            model.FriendLinkTitle = friendLinkInfo.Title;
            model.FriendLinkLogo = friendLinkInfo.Logo;
            model.FriendLinkUrl = friendLinkInfo.Url;
            model.Target = friendLinkInfo.Target;
            model.DisplayOrder = friendLinkInfo.DisplayOrder;
            Load();

            return View(model);
        }

        /// <summary>
        /// 编辑友情链接
        /// </summary>
        [HttpPost]
        public ActionResult Edit(FriendLinkModel model, int id = -1)
        {
            FriendLinkInfo friendLinkInfo = AdminFriendLinks.GetFriendLinkById(id);
            if (friendLinkInfo == null)
                return PromptView("友情链接不存在！");

            if (ModelState.IsValid)
            {
                friendLinkInfo.Name = model.FriendLinkName.Trim();
                friendLinkInfo.Title = model.FriendLinkTitle == null ? "" : model.FriendLinkTitle.Trim();
                friendLinkInfo.Logo = model.FriendLinkLogo == null ? "" : model.FriendLinkLogo.Trim();
                friendLinkInfo.Url = model.FriendLinkUrl.Trim();
                friendLinkInfo.Target = model.Target;
                friendLinkInfo.DisplayOrder = model.DisplayOrder;

                AdminFriendLinks.UpdateFriendLink(friendLinkInfo);
                AddMallAdminLog("修改友情链接", "修改友情链接,友情链接ID为:" + id);
                return PromptView("友情链接修改成功！");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除友情链接
        /// </summary>
        public ActionResult Del(int[] idList)
        {
            AdminFriendLinks.DeleteFriendLinkById(idList);
            AddMallAdminLog("删除友情链接", "删除友情链接,友情链接ID为:" + CommonHelper.IntArrayToString(idList));
            return PromptView("友情链接删除成功！");
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.FriendLinkThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["AllowImgType"] = allowImgType;
            ViewData["MaxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }
    }
}
