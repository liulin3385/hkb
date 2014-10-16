using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web.StoreAdmin.Controllers
{
    /// <summary>
    /// 店铺后台工具控制器类
    /// </summary>
    public class ToolController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            string ip = WebHelper.GetIP();
            //当用户ip不在允许的后台访问ip列表时
            if (!string.IsNullOrEmpty(BMAConfig.MallConfig.AdminAllowAccessIP) && !ValidateHelper.InIPList(ip, BMAConfig.MallConfig.AdminAllowAccessIP))
            {
                filterContext.Result = HttpNotFound();
                return;
            }
            //当用户IP被禁止时
            if (BannedIPs.CheckIP(ip))
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            PartUserInfo partUserInfo = null;
            //获得用户id
            int uid = MallUtils.GetUidCookie();
            if (uid < 1)
                uid = WebHelper.GetRequestInt("uid");

            if (uid < 1)//当用户为游客时
            {
                //创建游客
                partUserInfo = Users.CreatePartGuest();
            }
            else//当用户为会员时
            {
                //获得保存在cookie中的密码
                string encryptPwd = MallUtils.GetCookiePassword();
                if (string.IsNullOrWhiteSpace(encryptPwd))
                    encryptPwd = WebHelper.GetRequestString("password");
                //防止用户密码被篡改为危险字符
                if (encryptPwd.Length == 0 || !SecureHelper.IsBase64String(encryptPwd))
                {
                    //创建游客
                    partUserInfo = Users.CreatePartGuest();
                    MallUtils.SetUidCookie(-1);
                    MallUtils.SetCookiePassword("");
                }
                else
                {
                    partUserInfo = Users.GetPartUserByUidAndPwd(uid, MallUtils.DecryptCookiePassword(encryptPwd));
                    if (partUserInfo == null)
                    {
                        partUserInfo = Users.CreatePartGuest();
                        MallUtils.SetUidCookie(-1);
                        MallUtils.SetCookiePassword("");
                    }
                }
            }

            //当用户等级是禁止访问等级时
            if (partUserInfo.UserRid == 1)
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            //如果当前用户没有登录
            if (partUserInfo.Uid < 1)
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            //店铺不存在或店铺关闭
            StoreInfo storeInfo = AdminStores.GetStoreById(partUserInfo.StoreId);
            if (storeInfo == null || storeInfo.State == (int)StoreState.Close)
            {
                filterContext.Result = HttpNotFound();
                return;
            }
        }

        /// <summary>
        /// 上传商品图片
        /// </summary>
        /// <param name="productImage">商品图片</param>
        /// <returns></returns>
        public ActionResult UploadProductImage(HttpPostedFileBase productImage)
        {
            string result = MallUtils.SaveUplaodProductImage(productImage);
            return Content(result);
        }

        /// <summary>
        /// 上传商品编辑器中图片
        /// </summary>
        /// <returns>-1代表图片为空，-2代表图片类型错误，-3代表图片太大</returns>
        public ActionResult UploadProductEditorImage()
        {
            HttpPostedFileBase image = ControllerContext.RequestContext.HttpContext.Request.Files[0];
            string result = MallUtils.SaveProductEditorImage(image);

            string state = "SUCCESS";
            string url = "";
            if (result == "-1")
            {
                state = "上传图片不能为空";
            }
            else if (result == "-2")
            {
                state = "不允许的图片类型";
            }
            else if (result == "-3")
            {
                state = "图片大小超出网站限制";
            }
            else
            {
                url = result;
            }

            //获取图片描述
            string title = ControllerContext.RequestContext.HttpContext.Request.Form["pictitle"];

            string oriName = "";
            //获取原始文件名
            if (ControllerContext.RequestContext.HttpContext.Request.Form["fileName"] != null)
            {
                oriName = ControllerContext.RequestContext.HttpContext.Request.Form["fileName"].Split(',')[1];
            }
            return Content(string.Format("{4}'url':'Upload/Product/Editor/Thumb/{0}','title':'{1}','original':'{2}','state':'{3}'{5}", url, title, oriName, state, "{", "}"));
        }

        /// <summary>
        /// 上传商品编辑器中文件
        /// </summary>
        /// <returns>-1代表文件为空，-2代表文件类型错误，-3代表文件太大</returns>
        public ActionResult UplaodProductEditorFile()
        {
            HttpPostedFileBase file = ControllerContext.RequestContext.HttpContext.Request.Files[0];
            string result = MallUtils.SaveProductEditorFile(file);

            string state = "SUCCESS";
            string url = "";
            if (result == "-1")
            {
                state = "上传文件不能为空";
            }
            else if (result == "-2")
            {
                state = "不允许的文件类型";
            }
            else if (result == "-3")
            {
                state = "文件大小超出网站限制";
            }
            else
            {
                url = result;
            }

            //获取图片描述
            string title = ControllerContext.RequestContext.HttpContext.Request.Form["pictitle"];
            //获取原始文件名
            string oriName = ControllerContext.RequestContext.HttpContext.Request.Form["fileName"];

            return Content(string.Format("{4}'url':'Upload/Product/Editor/File/{0}','title':'{1}','original':'{2}','state':'{3}'{5}", url, title, oriName, state, "{", "}"));
        }

        /// <summary>
        /// 上传店铺logo
        /// </summary>
        /// <param name="storeLogo">店铺logo</param>
        /// <returns></returns>
        public ActionResult UploadStoreLogo(HttpPostedFileBase storeLogo)
        {
            string result = MallUtils.SaveUploadStoreLogo(storeLogo);
            return Content(result);
        }

        /// <summary>
        /// 上传店铺banner
        /// </summary>
        /// <param name="storeBanner">店铺banner</param>
        /// <returns></returns>
        public ActionResult UploadStoreBanner(HttpPostedFileBase storeBanner)
        {
            string result = MallUtils.SaveUploadStoreBanner(storeBanner);
            return Content(result);
        }

        /// <summary>
        /// 省列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProvinceList()
        {
            List<RegionInfo> regionList = Regions.GetProvinceList();

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

        /// <summary>
        /// 市列表
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <returns></returns>
        public ActionResult CityList(int provinceId = -1)
        {
            List<RegionInfo> regionList = Regions.GetCityList(provinceId);

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

        /// <summary>
        /// 县或区列表
        /// </summary>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public ActionResult CountyList(int cityId = -1)
        {
            List<RegionInfo> regionList = Regions.GetCountyList(cityId);

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

    }
}
