using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Web.Models
{
    /// <summary>
    /// 购物车添加成功模型类
    /// </summary>
    public class CartAddSuccessModel
    {
        /// <summary>
        /// 项id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 项类型(0代表商品，1代表套装)
        /// </summary>
        public int ItemType { get; set; }
    }

    /// <summary>
    /// 购物车模型类
    /// </summary>
    public class CartModel
    {
        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 商品合计
        /// </summary>
        public decimal ProductAmount { get; set; }
        /// <summary>
        /// 满减折扣
        /// </summary>
        public decimal FullCutDiscount { get; set; }
        /// <summary>
        /// 订单合计
        /// </summary>
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> OrderProductList { get; set; }
    }
}