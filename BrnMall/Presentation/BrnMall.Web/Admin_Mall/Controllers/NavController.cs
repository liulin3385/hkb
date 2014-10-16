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
    /// 商城后台导航控制器类
    /// </summary>
    public class NavController : BaseMallAdminController
    {
        /// <summary>
        /// 导航列表
        /// </summary>
        public ActionResult List()
        {
            NavListModel model = new NavListModel();
            model.NavList = AdminNavs.GetNavList();
            MallUtils.SetAdminRefererCookie(Url.Action("list"));
            return View(model);
        }

        /// <summary>
        /// 添加导航
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            NavModel model = new NavModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加导航
        /// </summary>
        [HttpPost]
        public ActionResult Add(NavModel model)
        {
            NavInfo parentNavInfo = null;
            if (model.Pid != 0)
            {
                parentNavInfo = AdminNavs.GetNavById(model.Pid);
                if (parentNavInfo == null)
                {
                    ModelState.AddModelError("Pid", "父导航不存在");
                }
            }

            if (ModelState.IsValid)
            {
                NavInfo navInfo = new NavInfo()
                {
                    Pid = model.Pid,
                    Layer = model.Pid == 0 ? 1 : parentNavInfo.Layer + 1,
                    Name = model.NavName.Trim(),
                    Title = model.NavTitle == null ? "" : model.NavTitle.Trim(),
                    Url = model.NavUrl.Trim(),
                    Target = model.Target,
                    DisplayOrder = model.DisplayOrder
                };

                AdminNavs.CreateNav(navInfo);
                AddMallAdminLog("添加导航", "添加导航,导航为:" + model.NavName);
                return PromptView("导航添加成功！");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑导航
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            NavInfo navInfo = AdminNavs.GetNavById(id);
            if (navInfo == null)
                return PromptView("导航不存在！");

            NavModel model = new NavModel();
            model.Pid = navInfo.Pid;
            model.NavName = navInfo.Name;
            model.NavTitle = navInfo.Title;
            model.NavUrl = navInfo.Url;
            model.Target = navInfo.Target;
            model.DisplayOrder = navInfo.DisplayOrder;
            Load();
            ((List<SelectListItem>)ViewData["NavList"]).RemoveAll(x => x.Value == id.ToString());

            return View(model);
        }

        /// <summary>
        /// 编辑导航
        /// </summary>
        [HttpPost]
        public ActionResult Edit(NavModel model, int id = -1)
        {
            NavInfo navInfo = AdminNavs.GetNavById(id);
            if (navInfo == null)
                return PromptView("导航不存在！");

            NavInfo parentNavInfo = null;
            if (model.Pid != 0)
            {
                parentNavInfo = AdminNavs.GetNavById(model.Pid);
                if (parentNavInfo == null)
                    ModelState.AddModelError("Pid", "父导航不存在");
            }

            if (ModelState.IsValid)
            {
                navInfo.Pid = model.Pid;
                navInfo.Layer = model.Pid == 0 ? 1 : navInfo.Layer + 1;
                navInfo.Name = model.NavName.Trim();
                navInfo.Title = model.NavTitle == null ? "" : model.NavTitle.Trim();
                navInfo.Url = model.NavUrl.Trim();
                navInfo.Target = model.Target;
                navInfo.DisplayOrder = model.DisplayOrder;

                AdminNavs.UpdateNav(navInfo);
                AddMallAdminLog("修改导航", "修改导航,导航ID为:" + id);
                return PromptView("导航修改成功！");
            }

            Load();
            ((List<SelectListItem>)ViewData["NavList"]).RemoveAll(x => x.Value == id.ToString());
            return View(model);
        }

        /// <summary>
        /// 删除导航
        /// </summary>
        public ActionResult Del(int id = -1)
        {
            int result = AdminNavs.DeleteNavById(id);
            if (result == 0)
                return PromptView("删除失败，请先删除此导航下的子导航！");

            AddMallAdminLog("删除导航", "删除导航,导航ID为:" + id);
            return PromptView("导航删除成功！");
        }

        private void Load()
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            List<NavInfo> navList = AdminNavs.GetNavList();
            itemList.Add(new SelectListItem() { Text = "顶级导航", Value = "0" });
            foreach (NavInfo navInfo in navList)
            {
                itemList.Add(new SelectListItem() { Text = navInfo.Name, Value = navInfo.Id.ToString() });
            }
            ViewData["NavList"] = itemList;
            ViewData["referer"] = MallUtils.GetAdminRefererCookie();
        }
    }
}
