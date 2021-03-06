﻿using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web.StoreAdmin.Models
{
    /// <summary>
    /// 订单列表模型类
    /// </summary>
    public class OrderListModel
    {
        public PageModel PageModel { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public DataTable OrderList { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
    }

    /// <summary>
    /// 订单信息模型类
    /// </summary>
    public class OrderInfoModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 区域信息
        /// </summary>
        public RegionInfo RegionInfo { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public UserRankInfo UserRankInfo { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 订单处理列表
        /// </summary>
        public List<OrderActionInfo> OrderActionList { get; set; }
    }

    /// <summary>
    /// 更新订单配送费用模型类
    /// </summary>
    public class UpdateOrderShipFeeModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "配送费用不能小于0")]
        [Required(ErrorMessage = "配送费用不能为空！")]
        public decimal ShipFee { get; set; }
    }

    /// <summary>
    /// 更新订单折扣模型类
    /// </summary>
    public class UpdateOrderDiscountModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "订单折扣不能小于0")]
        [Required(ErrorMessage = "订单折扣不能为空！")]
        public decimal Discount { get; set; }
    }

    /// <summary>
    /// 订单发货模型类
    /// </summary>
    public class SendOrderProductModel
    {
        public SendOrderProductModel()
        {
            ShipCoId = -1;
            ShipCoName = "选择配送公司";
        }

        [Required(ErrorMessage = "发货单号不能为空！")]
        [StringLength(30, ErrorMessage = "发货单号长度不能超过30")]
        public string ShipSN { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择配送公司")]
        [Required(ErrorMessage = "请选择配送公司！")]
        public int ShipCoId { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string ShipCoName { get; set; }
    }

    /// <summary>
    /// 打印订单模型类
    /// </summary>
    public class PrintOrderModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 区域信息
        /// </summary>
        public RegionInfo RegionInfo { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
    }

    /// <summary>
    /// 订单退款列表模型类
    /// </summary>
    public class OrderRefundListModel
    {
        public List<OrderRefundInfo> OrderRefundList { get; set; }
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
    }
}
