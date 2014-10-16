using System;
using System.Web.Mvc;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.Models;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 分类控制器类
    /// </summary>
    public class CategoryController : BaseWebController
    {
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            CategoryListModel model = new CategoryListModel();
            model.CategoryList = Categories.GetCategoryList();
            return View(model);
        }
    }
}
