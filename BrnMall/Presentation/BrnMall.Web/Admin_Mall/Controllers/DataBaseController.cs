using System;
using System.Web.Mvc;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台数据库控制器类
    /// </summary>
    public class DataBaseController : BaseMallAdminController
    {
        /// <summary>
        /// 数据库管理
        /// </summary>
        public ActionResult Manage()
        {
            return View();
        }

        /// <summary>
        /// 运行SQL语句
        /// </summary>
        public ActionResult RunSql(string sql = "")
        {
            if (string.IsNullOrWhiteSpace(sql))
                return PromptView(Url.Action("Manage"), "SQL语句不能为空！");

            string message = DataBases.RunSql(sql);
            AddMallAdminLog("运行SQL语句", "运行SQL语句,SQL语句为:" + sql);
            if (string.IsNullOrWhiteSpace(message))
                return PromptView(Url.Action("Manage"), "SQL语句运行成功！");
            else
                return PromptView(Url.Action("Manage"), "SQL语句运行失败！错误信息为：" + message, false);
        }
    }
}
