using System;
using System.Data;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall关系数据库策略之订单分部接口
    /// </summary>
    public partial interface IRDBSStrategy
    {
        #region 用户配送地址

        /// <summary>
        /// 创建用户配送地址
        /// </summary>
        int CreateShipAddress(ShipAddressInfo shipAddressInfo);

        /// <summary>
        /// 更新用户配送地址
        /// </summary>
        void UpdateShipAddress(ShipAddressInfo shipAddressInfo);

        /// <summary>
        /// 获得完整用户配送地址列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        IDataReader GetFullShipAddressList(int uid);

        /// <summary>
        /// 获得用户配送地址数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        int GetShipAddressCount(int uid);

        /// <summary>
        /// 获得默认完整用户配送地址
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        IDataReader GetDefaultFullShipAddress(int uid);

        /// <summary>
        /// 获得完整用户配送地址
        /// </summary>
        /// <param name="saId">配送地址id</param>
        /// <returns></returns>
        IDataReader GetFullShipAddressBySAId(int saId);

        /// <summary>
        /// 获得用户配送地址
        /// </summary>
        /// <param name="saId">配送地址id</param>
        /// <returns></returns>
        IDataReader GetShipAddressBySAId(int saId);

        /// <summary>
        /// 删除用户配送地址
        /// </summary>
        /// <param name="saId">配送地址id</param>
        /// <param name="uid">用户id</param>
        bool DeleteShipAddress(int saId, int uid);

        /// <summary>
        /// 更新用户配送地址的默认状态
        /// </summary>
        /// <param name="saId">配送地址id</param>
        /// <param name="uid">用户id</param>
        /// <param name="isDefault">状态</param>
        /// <returns></returns>
        bool UpdateShipAddressIsDefault(int saId, int uid, int isDefault);

        #endregion

        #region 订单商品

        /// <summary>
        /// 获得订单商品
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <returns></returns>
        IDataReader GetOrderProductByRecordId(int recordId);

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        int GetCartProductCount(int uid);

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        int GetCartProductCount(string sid);

        /// <summary>
        /// 添加订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        int AddOrderProduct(OrderProductInfo orderProductInfo);

        /// <summary>
        /// 更新订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        void UpdateOrderProduct(OrderProductInfo orderProductInfo);

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        IDataReader GetCartProductList(int uid);

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        IDataReader GetCartProductList(string sid);

        /// <summary>
        /// 更新购物车的用户id
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        void UpdateCartUidBySid(int uid, string sid);

        /// <summary>
        /// 删除订单商品
        /// </summary>
        /// <param name="recordIdList">记录id列表</param>
        void DeleteOrderProductByRecordId(string recordIdList);

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        int ClearCart(int uid);

        /// <summary>
        /// 清空购物车的商品
        /// </summary>
        /// <param name="sid">sid</param>
        /// <returns></returns>
        int ClearCart(string sid);

        /// <summary>
        /// 更新订单商品的数量
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <param name="realCount">真实数量</param>
        /// <param name="buyCount">购买数量</param>
        void UpdateOrderProductCount(int recordId, int realCount, int buyCount);

        /// <summary>
        /// 更新订单商品的满减促销活动
        /// </summary>
        /// <param name="recordIdList">记录id列表</param>
        /// <param name="limitMoney">限制金额</param>
        /// <param name="cutMoney">优惠金额</param>
        void UpdateFullCutPromotionOfOrderProduct(string recordIdList, int limitMoney, int cutMoney);

        /// <summary>
        /// 清空过期购物车
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        void ClearExpiredCart(DateTime expireTime);

        #endregion

        #region 订单

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        /// <returns>订单id</returns>
        int CreateOrder(OrderInfo orderInfo);

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns>订单信息</returns>
        IDataReader GetOrderByOid(int oid);

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="osn">订单编号</param>
        /// <returns>订单信息</returns>
        IDataReader GetOrderByOSN(string osn);

        /// <summary>
        /// 获得订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        int GetOrderStateByOid(int oid);

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        int GetOrderCountByCondition(int storeId, int orderState, string startTime, string endTime);

        /// <summary>
        /// 获得订单列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        DataTable GetOrderList(int pageSize, int pageNumber, string condition, string sort);

        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <param name="uid">用户id</param>
        /// <param name="consignee">收货人</param>
        /// <param name="orderState">订单状态</param>
        /// <returns></returns>
        string GetOrderListCondition(int storeId, string osn, int uid, string consignee, int orderState);

        /// <summary>
        /// 获得列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        string GetOrderListSort(string sortColumn, string sortDirection);

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int GetOrderCount(string condition);

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        IDataReader GetOrderProductList(int oid);

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oidList">订单id列表</param>
        /// <returns></returns>
        IDataReader GetOrderProductList(string oidList);

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="discount">折扣</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        void UpdateOrderDiscount(int oid, decimal discount, decimal orderAmount, decimal surplusMoney);

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">配送费用</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        void UpdateOrderShipFee(int oid, decimal shipFee, decimal orderAmount, decimal surplusMoney);

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        void UpdateOrderState(int oid, OrderState orderState);

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="shipSN">配送单号</param>
        /// <param name="shipCoId">配送公司id</param>
        /// <param name="shipCoName">配送公司名称</param>
        /// <param name="shipTime">配送时间</param>
        void SendOrderProduct(int oid, OrderState orderState, string shipSN, int shipCoId, string shipCoName, DateTime shipTime);

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="paySN">支付单号</param>
        /// <param name="payTime">支付时间</param>
        void PayOrder(int oid, OrderState orderState, string paySN, DateTime payTime);

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        DataTable GetUserOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime, int orderState);

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        int GetUserOrderCount(int uid, string startAddTime, string endAddTime, int orderState);

        /// <summary>
        /// 获得销售商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        DataTable GetSaleProductList(int pageSize, int pageNumber, string startTime, string endTime);

        /// <summary>
        /// 获得销售商品数量
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        int GetSaleProductCount(string startTime, string endTime);

        /// <summary>
        /// 获得销售趋势
        /// </summary>
        /// <param name="trendType">趋势类型(0代表订单数，1代表订单合计)</param>
        /// <param name="timeType">时间类型(0代表小时，1代表天，2代表月，3代表年)</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        DataTable GetSaleTrend(int trendType, int timeType, string startTime, string endTime);

        #endregion

        #region 订单处理

        /// <summary>
        /// 创建订单处理
        /// </summary>
        /// <param name="orderActionInfo">订单处理信息</param>
        void CreateOrderAction(OrderActionInfo orderActionInfo);

        /// <summary>
        /// 获得订单处理列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        IDataReader GetOrderActionList(int oid);

        /// <summary>
        /// 获得订单id列表
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderActionType">订单操作类型</param>
        /// <returns></returns>
        DataTable GetOrderIdList(DateTime startTime, DateTime endTime, int orderActionType);

        #endregion

        #region 订单退款

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="orderRefundInfo">订单退款信息</param>
        void ApplyRefund(OrderRefundInfo orderRefundInfo);

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        void RefundOrder(int refundId, string refundSN, string refundSystemName, string refundFriendName, DateTime refundTime);

        /// <summary>
        /// 获得订单退款列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        IDataReader GetOrderRefundList(int pageSize, int pageNumber, string condition);

        /// <summary>
        /// 获得订单退款列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <returns></returns>
        string GetOrderRefundListCondition(int storeId, string osn);

        /// <summary>
        /// 获得订单退款数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int GetOrderRefundCount(string condition);

        #endregion
    }
}
