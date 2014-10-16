using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Web.Models
{
    /// <summary>
    /// 确认订单模型类
    /// </summary>
    public class ConfirmOrderModel
    {
        /// <summary>
        /// 默认完整用户配送地址
        /// </summary>
        public FullShipAddressInfo DefaultFullShipAddressInfo { get; set; }
        /// <summary>
        /// 默认支付插件
        /// </summary>
        public PluginInfo DefaultPayPluginInfo { get; set; }

        /// <summary>
        /// 店铺订单列表
        /// </summary>
        public List<StoreOrder> StoreOrderList { get; set; }

        /// <summary>
        /// 支付积分名称
        /// </summary>
        public string PayCreditName { get; set; }
        /// <summary>
        /// 用户支付积分
        /// </summary>
        public int UserPayCredits { get; set; }
        /// <summary>
        /// 最大使用支付积分
        /// </summary>
        public int MaxUsePayCredits { get; set; }

        /// <summary>
        /// 支付费用
        /// </summary>
        public decimal PayFee { get; set; }
        /// <summary>
        /// 全部订单合计
        /// </summary>
        public decimal AllOrderAmount { get; set; }

        /// <summary>
        /// 是否显示验证码
        /// </summary>
        public bool IsVerifyCode { get; set; }
    }

    /// <summary>
    /// 店铺订单
    /// </summary>
    public class StoreOrder
    {
        /// <summary>
        /// 店铺信息
        /// </summary>
        public StoreInfo StoreInfo { get; set; }
        /// <summary>
        /// 购物车商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 商品合计
        /// </summary>
        public decimal ProductAmount { get; set; }
        /// <summary>
        /// 满减
        /// </summary>
        public decimal FullCut { get; set; }
        /// <summary>
        /// 配送费用
        /// </summary>
        public int ShipFee { get; set; }
        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 商品总重量
        /// </summary>
        public int TotalWeight { get; set; }
    }

    /// <summary>
    /// 提交成功模型类
    /// </summary>
    public class SubmitSuccessModel
    {
        public string OidList { get; set; }
        public List<OrderInfo> OrderList { get; set; }
        public PluginInfo PayPlugin { get; set; }
        public decimal AllSurplusMoney { get; set; }
    }

    /// <summary>
    /// 支付展示模型类
    /// </summary>
    public class PayShowModel
    {
        public string OidList { get; set; }
        public List<OrderInfo> OrderList { get; set; }
        public PluginInfo PayPlugin { get; set; }
        public string ShowView { get; set; }
        public decimal AllSurplusMoney { get; set; }
    }
}