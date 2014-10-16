using System;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web.Models
{
    /// <summary>
    /// 品牌列表模型类
    /// </summary>
    public class BrandListModel
    {
        public PageModel PageModel { get; set; }
        public string BrandName { get; set; }
        public List<BrandInfo> BrandList { get; set; }
    }
}