using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 订单操作管理类
    /// </summary>
    public class Orders
    {
        private static object _locker = new object();//锁对象

        private static DateTime _thisdate;//今天日期
        private static int _todayordercount;//今天订单数
        private static string _osnformat;//订单编号格式

        static Orders()
        {
            _thisdate = DateTime.Now.Date;
            _osnformat = BMAConfig.MallConfig.OSNFormat;
            _todayordercount = GetOrderCountByCondition(0, 0, _thisdate.ToString(), "");
        }

        /// <summary>
        /// 获得订单商品
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <returns></returns>
        public static OrderProductInfo GetOrderProductByRecordId(int recordId)
        {
            return BrnMall.Data.Orders.GetOrderProductByRecordId(recordId);
        }



        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static int GetCartProductCount(int uid)
        {
            if (uid < 1)
                return 0;
            return BrnMall.Data.Orders.GetCartProductCount(uid);
        }

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static int GetCartProductCount(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid))
                return 0;
            return BrnMall.Data.Orders.GetCartProductCount(sid);
        }

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static int GetCartProductCount(int uid, string sid)
        {
            if (uid > 0)
                return GetCartProductCount(uid);
            else
                return GetCartProductCount(sid);
        }




        /// <summary>
        /// 添加订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        public static int AddOrderProduct(OrderProductInfo orderProductInfo)
        {
            return BrnMall.Data.Orders.AddOrderProduct(orderProductInfo);
        }

        /// <summary>
        /// 更新订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        public static void UpdateOrderProduct(OrderProductInfo orderProductInfo)
        {
            BrnMall.Data.Orders.UpdateOrderProduct(orderProductInfo);
        }





        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(int uid)
        {
            return BrnMall.Data.Orders.GetCartProductList(uid);
        }

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(string sid)
        {
            return BrnMall.Data.Orders.GetCartProductList(sid);
        }

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(int uid, string sid)
        {
            if (uid > 0)
                return GetCartProductList(uid);
            else
                return GetCartProductList(sid);
        }

        /// <summary>
        /// 整理店铺订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> TidyStoreOrderProductList(List<OrderProductInfo> orderProductList)
        {
            int count = orderProductList.Count;

            //当没有商品或只有一个商品时
            if (count < 2)
                return orderProductList;

            //声明一个整理的订单商品列表
            List<OrderProductInfo> tidiedOrderProductList = new List<OrderProductInfo>(count);

            for (int i = 0; i < count; i++)
            {
                OrderProductInfo orderProductInfo = orderProductList[i];
                if (orderProductInfo == null)
                    continue;

                if (orderProductInfo.Type == 0)//当商品是普通订单商品时
                {
                    //获取商品的满赠商品
                    if (orderProductInfo.ExtCode7 > 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            OrderProductInfo item = orderProductList[j];
                            if (item != null && item.Type == 3 && item.ExtCode1 == orderProductInfo.ExtCode7)
                            {
                                tidiedOrderProductList.Add(item);
                                orderProductList[j] = null;
                            }
                        }
                    }

                    tidiedOrderProductList.Add(orderProductInfo);
                    orderProductList[i] = null;
                    //获取商品的赠品
                    for (int j = 0; j < count; j++)
                    {
                        OrderProductInfo item = orderProductList[j];
                        if (item != null && item.Type == 1 && item.ExtCode4 == 0 && item.ExtCode2 == orderProductInfo.Pid)
                        {
                            tidiedOrderProductList.Add(item);
                            orderProductList[j] = null;
                        }
                    }
                    //获取同一满赠商品
                    if (orderProductInfo.ExtCode7 > 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            OrderProductInfo item = orderProductList[j];
                            if (item != null && item.ExtCode7 == orderProductInfo.ExtCode7)
                            {
                                tidiedOrderProductList.Add(item);
                                orderProductList[j] = null;
                                for (int k = 0; k < count; k++)
                                {
                                    OrderProductInfo item2 = orderProductList[k];
                                    if (item2 != null && item2.Type == 1 && item2.ExtCode2 == item.Pid)
                                    {
                                        tidiedOrderProductList.Add(item2);
                                        orderProductList[k] = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (orderProductInfo.Type == 2)//当商品是套装商品时
                {
                    for (int j = 0; j < count; j++)
                    {
                        OrderProductInfo item = orderProductList[j];
                        //获取同一套装商品
                        if (item != null && item.Type == 2 && item.ExtCode7 == orderProductInfo.ExtCode7)
                        {
                            tidiedOrderProductList.Add(item);
                            orderProductList[j] = null;
                            //获取商品的赠品
                            for (int k = 0; k < count; k++)
                            {
                                OrderProductInfo item2 = orderProductList[k];
                                if (item2 != null && item2.Type == 1 && item2.ExtCode4 > 0 && item2.ExtCode2 == item.Pid)
                                {
                                    tidiedOrderProductList.Add(item2);
                                    orderProductList[k] = null;
                                }
                            }
                        }
                    }
                }
            }

            return tidiedOrderProductList;
        }

        /// <summary>
        /// 整理商城订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> TidyMallOrderProductList(List<OrderProductInfo> orderProductList)
        {
            //声明一个整理的订单商品列表
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> tidiedOrderProductList = new List<KeyValuePair<StoreInfo, List<OrderProductInfo>>>();

            int count = orderProductList.Count;

            if (count == 1)
            {
                StoreInfo storeInfo = Stores.GetStoreById(orderProductList[0].StoreId);
                tidiedOrderProductList.Add(new KeyValuePair<StoreInfo, List<OrderProductInfo>>(storeInfo, orderProductList));
                return tidiedOrderProductList;
            }

            List<int> storeIdList = new List<int>(count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeIdList.Add(orderProductInfo.StoreId);
            }
            foreach (int storeId in storeIdList.Distinct<int>())
            {
                StoreInfo storeInfo = Stores.GetStoreById(storeId);
                List<OrderProductInfo> tidyStoreOrderProductList = TidyStoreOrderProductList(orderProductList.FindAll(x => x.StoreId == storeId));
                tidiedOrderProductList.Add(new KeyValuePair<StoreInfo, List<OrderProductInfo>>(storeInfo, tidyStoreOrderProductList));
            }

            return tidiedOrderProductList;
        }



        /// <summary>
        /// 更新购物车的用户id
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static void UpdateCartUidBySid(int uid, string sid)
        {
            BrnMall.Data.Orders.UpdateCartUidBySid(uid, sid);
        }

        /// <summary>
        /// 删除订单商品
        /// </summary>
        /// <param name="recordIdList">记录id列表</param>
        public static void DeleteOrderProductByRecordId(string recordIdList)
        {
            if (!string.IsNullOrEmpty(recordIdList))
                BrnMall.Data.Orders.DeleteOrderProductByRecordId(recordIdList);
        }

        /// <summary>
        /// 获得购物车商品数量
        /// </summary>
        /// <returns></returns>
        public static int GetCartProductCountCookie()
        {
            return TypeHelper.StringToInt(WebHelper.GetCookie("cart", "pcount"));
        }

        /// <summary>
        /// 设置购物车商品数量cookie
        /// </summary>
        /// <param name="count">购物车商品数量</param>
        /// <returns></returns>
        public static void SetCartProductCountCookie(int count)
        {
            if (count < 0) count = 0;
            WebHelper.SetCookie("cart", "pcount", count.ToString(), BMAConfig.MallConfig.SCExpire * 24 * 60);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">sid</param>
        /// <returns></returns>
        public static int ClearCart(int uid, string sid)
        {
            if (uid > 0)
                return BrnMall.Data.Orders.ClearCart(uid);
            else
                return BrnMall.Data.Orders.ClearCart(sid);
        }

        /// <summary>
        /// 更新订单商品的数量
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <param name="realCount">真实数量</param>
        /// <param name="buyCount">购买数量</param>
        public static void UpdateOrderProductCount(int recordId, int realCount, int buyCount)
        {
            BrnMall.Data.Orders.UpdateOrderProductCount(recordId, realCount, buyCount);
        }

        /// <summary>
        /// 更新订单商品的满减促销活动
        /// </summary>
        /// <param name="recordIdList">记录id列表</param>
        /// <param name="limitMoney">限制金额</param>
        /// <param name="cutMoney">优惠金额</param>
        public static void UpdateFullCutPromotionOfOrderProduct(string recordIdList, int limitMoney, int cutMoney)
        {
            BrnMall.Data.Orders.UpdateFullCutPromotionOfOrderProduct(recordIdList, limitMoney, cutMoney);
        }

        /// <summary>
        /// 清空过期购物车
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        public static void ClearExpiredCart(DateTime expireTime)
        {
            BrnMall.Data.Orders.ClearExpiredCart(expireTime);
        }





        /// <summary>
        /// 获得记录id列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static string GetRecordIdList(List<OrderProductInfo> orderProductList)
        {
            if (orderProductList.Count == 0)
                return string.Empty;

            if (orderProductList.Count == 1)
                return orderProductList[0].RecordId.ToString();

            StringBuilder recordIdList = new StringBuilder();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                recordIdList.AppendFormat("{0},", orderProductInfo.RecordId);
            return recordIdList.Remove(recordIdList.Length - 1, 1).ToString();
        }

        /// <summary>
        /// 获得商品id列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static string GetPidList(List<OrderProductInfo> orderProductList)
        {
            if (orderProductList.Count == 0)
                return string.Empty;

            if (orderProductList.Count == 1)
                return orderProductList[0].Pid.ToString();

            StringBuilder pidList = new StringBuilder();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                pidList.AppendFormat("{0},", orderProductInfo.Pid);
            return CommonHelper.GetUniqueString(pidList.Remove(pidList.Length - 1, 1).ToString());
        }

        /// <summary>
        /// 汇总订单商品数量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumOrderProductCount(List<OrderProductInfo> orderProductList)
        {
            int count = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                count = count + orderProductInfo.RealCount;
            return count;
        }

        /// <summary>
        /// 汇总订单商品重量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumOrderProductWeight(List<OrderProductInfo> orderProductList)
        {
            int totalWeight = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                totalWeight = totalWeight + orderProductInfo.RealCount * orderProductInfo.Weight;
            return totalWeight;
        }

        /// <summary>
        /// 获得商品总计
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                productAmount = productAmount + orderProductInfo.BuyCount * orderProductInfo.DiscountPrice;
            return productAmount;
        }

        /// <summary>
        /// 汇总满减
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumFullCut(List<OrderProductInfo> orderProductList)
        {
            //当商品列表为空时返回0
            if (orderProductList.Count < 1)
                return 0;

            //满减商品列表
            List<OrderProductInfo> fullCutOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.ExtCode10 > 0)
                    fullCutOrderProductList.Add(orderProductInfo);
            }
            //当满减商品列表为空时返回0
            if (fullCutOrderProductList.Count < 1)
                return 0;

            //满减
            int fullCut = 0;
            //按照满减促销活动id分组
            var dic = fullCutOrderProductList.ToLookup(i => i.ExtCode10);
            foreach (var item in dic)
            {
                decimal temp = 0M;
                foreach (var item1 in item)
                    temp += (item1.RealCount * item1.DiscountPrice);
                OrderProductInfo first = item.First();
                //当商品合计满足满减促销活动条件时
                if (temp >= first.ExtCode11)
                    fullCut += first.ExtCode12;

                //重置临时值为0
                temp = 0M;
            }
            return fullCut;
        }

        /// <summary>
        /// 汇总支付积分
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumPayCredits(List<OrderProductInfo> orderProductList)
        {
            int payCredits = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                payCredits = payCredits + orderProductInfo.RealCount * orderProductInfo.PayCredits;
            return payCredits;
        }

        /// <summary>
        /// 汇总商品折扣
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductDiscount(List<OrderProductInfo> orderProductList)
        {
            decimal discount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                discount += (orderProductInfo.RealCount * orderProductInfo.ShopPrice - orderProductInfo.BuyCount * orderProductInfo.ShopPrice);
            return discount;
        }

        /// <summary>
        /// 汇总优惠劵
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static Dictionary<int, string> SumCouponType(List<OrderProductInfo> orderProductList)
        {
            Dictionary<int, string> couponTypeList = new Dictionary<int, string>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.CouponTypeId > 0 && !couponTypeList.ContainsKey(orderProductInfo.CouponTypeId))
                    couponTypeList.Add(orderProductInfo.CouponTypeId, orderProductInfo.ExtStr3);
            }
            return couponTypeList;
        }






        /// <summary>
        /// 从订单商品列表中获得普通商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetCommonOrderProductByPid(int pid, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo info in orderProductList)
            {
                if (info.Type == 0 && info.Pid == pid)
                    return info;
            }
            return null;
        }

        /// <summary>
        /// 获得普通订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCommonOrderProductList(List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> commonOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                    commonOrderProductList.Add(orderProductInfo);
            }
            return commonOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetOrderProductByPid(int pid, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Pid == pid)
                    return orderProductInfo;
            }
            return null;
        }

        /// <summary>
        /// 从订单商品列表中获得指定商品的礼品列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetGiftOrderProductList(int pid, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> giftOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 1 && orderProductInfo.ExtCode2 == pid)
                    giftOrderProductList.Add(orderProductInfo);
            }
            return giftOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定套装商品列表
        /// </summary>
        /// <param name="pmId">套装促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="isContainGift">是否包含赠品</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetSuitOrderProductList(int pmId, List<OrderProductInfo> orderProductList, bool isContainGift)
        {
            List<OrderProductInfo> suitOrderProductList = new List<OrderProductInfo>();
            if (isContainGift)
            {
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if ((orderProductInfo.Type == 2 && orderProductInfo.ExtCode7 == pmId) || (orderProductInfo.Type == 1 && orderProductInfo.ExtCode4 == pmId))
                        suitOrderProductList.Add(orderProductInfo);
                }
            }
            else
            {
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if (orderProductInfo.Type == 2 && orderProductInfo.ExtCode7 == pmId)
                        suitOrderProductList.Add(orderProductInfo);
                }
            }
            return suitOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定满减商品列表
        /// </summary>
        /// <param name="pmId">满减促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullCutOrderProductList(int pmId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> fullCutOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0 && orderProductInfo.ExtCode10 == pmId)
                    fullCutOrderProductList.Add(orderProductInfo);
            }
            return fullCutOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定的满赠主商品列表
        /// </summary>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullSendMainOrderProductList(int pmId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0 && orderProductInfo.ExtCode7 == pmId)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 从订单商品列表中获得指定的满赠次商品
        /// </summary>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetFullSendMinorOrderProduct(int pmId, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 3 && orderProductInfo.ExtCode1 == pmId)
                    return orderProductInfo;
            }
            return null;
        }

        /// <summary>
        /// 从订单商品列表中获的满赠次商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullSendMinorOrderProductList(List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 3)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<int> GetStoreIdList(List<OrderProductInfo> orderProductList)
        {
            List<int> storeIdList = new List<int>(orderProductList.Count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeIdList.Add(orderProductInfo.StoreId);
            }
            return storeIdList.Distinct<int>().ToList<int>();
        }

        /// <summary>
        /// 从订单商品列表中获得指定订单的商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(int oid, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Oid == oid)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 按照店铺对订单商品列表进行分组
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> GetStoreGroupOrderProductList(List<OrderProductInfo> orderProductList)
        {
            List<KeyValuePair<StoreInfo, List<OrderProductInfo>>> storeGroupOrderProductList = new List<KeyValuePair<StoreInfo, List<OrderProductInfo>>>();

            //对订单商品进行店铺分组
            List<int> storeIdList = new List<int>(orderProductList.Count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeIdList.Add(orderProductInfo.StoreId);
            }
            foreach (int storeId in storeIdList.Distinct<int>())
            {
                StoreInfo storeInfo = Stores.GetStoreById(storeId);
                List<OrderProductInfo> storeOrderProductList = new List<OrderProductInfo>();
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if (orderProductInfo.StoreId == storeId)
                        storeOrderProductList.Add(orderProductInfo);
                }
                storeGroupOrderProductList.Add(new KeyValuePair<StoreInfo, List<OrderProductInfo>>(storeInfo, storeOrderProductList));
            }

            return storeGroupOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得同一配送方式的商品列表
        /// </summary>
        /// <param name="storeSTId">店铺配送模板id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetSameShipOrderProductList(int storeSTId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.StoreSTid == storeSTId)
                    list.Add(orderProductInfo);
            }
            return list;
        }







        /// <summary>
        /// 设置订单商品的买送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="buySendPromotionInfo">买送优惠活动</param>
        public static void SetBuySendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, int buyCount, BuySendPromotionInfo buySendPromotionInfo)
        {
            orderProductInfo.RealCount = buyCount + buySendPromotionInfo.SendCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.ExtCode4 = buySendPromotionInfo.PmId;
            orderProductInfo.ExtCode5 = buySendPromotionInfo.BuyCount;
            orderProductInfo.ExtCode6 = buySendPromotionInfo.SendCount;
            orderProductInfo.ExtStr2 = buySendPromotionInfo.Name;
        }

        /// <summary>
        /// 更新订单商品的买送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="buyTime">购买时间</param>
        /// <returns></returns>
        public static bool UpdateBuySendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, int buyCount, DateTime buyTime)
        {
            //获得商品的买送促销活动
            BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(buyCount, orderProductInfo.StoreId, orderProductInfo.Pid, buyTime);

            if (buySendPromotionInfo == null && orderProductInfo.ExtCode4 > 0)//当商品存在买送促销活动但添加后不存在买送促销活动时
            {
                orderProductInfo.RealCount = buyCount;
                orderProductInfo.BuyCount = buyCount;
                orderProductInfo.ExtCode4 = 0;
                orderProductInfo.ExtCode5 = 0;
                orderProductInfo.ExtCode6 = 0;
                orderProductInfo.ExtStr2 = "";
                return true;
            }
            else if (buySendPromotionInfo != null && orderProductInfo.ExtCode4 <= 0)//当商品不存在买送促销活动但添加后存在买送促销活动时
            {
                SetBuySendPromotionOfOrderProduct(orderProductInfo, buyCount, buySendPromotionInfo);
                return true;
            }
            else if (buySendPromotionInfo != null && orderProductInfo.ExtCode4 > 0)//当商品存在买送促销活动但添加后仍然满足买送促销活动时
            {
                SetBuySendPromotionOfOrderProduct(orderProductInfo, buyCount, buySendPromotionInfo);
                return IsChangeBuySendPromotion(orderProductInfo, buySendPromotionInfo);
            }
            else
            {
                orderProductInfo.RealCount = buyCount;
                orderProductInfo.BuyCount = buyCount;
            }
            return false;
        }

        /// <summary>
        /// 判断买送促销活动是否改变
        /// </summary>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="buySendPromotionInfo">买送促销活动</param>
        /// <returns></returns>
        public static bool IsChangeBuySendPromotion(OrderProductInfo orderProductInfo, BuySendPromotionInfo buySendPromotionInfo)
        {
            if (orderProductInfo.ExtCode4 != buySendPromotionInfo.PmId
                || orderProductInfo.ExtCode5 != buySendPromotionInfo.BuyCount
                || orderProductInfo.ExtCode6 != buySendPromotionInfo.SendCount
                || orderProductInfo.ExtStr2 != buySendPromotionInfo.Name)
                return true;
            return false;
        }

        /// <summary>
        /// 设置订单商品的单品促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="singlePromotionInfo">单品促销活动</param>
        public static void SetSinglePromotionOfOrderProduct(OrderProductInfo orderProductInfo, SinglePromotionInfo singlePromotionInfo)
        {
            orderProductInfo.ExtCode1 = singlePromotionInfo.PmId;
            orderProductInfo.ExtCode3 = singlePromotionInfo.DiscountValue;
            orderProductInfo.ExtStr1 = singlePromotionInfo.Name;
            switch (singlePromotionInfo.DiscountType)
            {
                case 0://折扣
                    {
                        orderProductInfo.ExtCode2 = 0;
                        decimal temp = Math.Ceiling((orderProductInfo.ShopPrice * singlePromotionInfo.DiscountValue) / 10);
                        orderProductInfo.DiscountPrice = temp < 0 ? orderProductInfo.ShopPrice : temp;
                        break;
                    }
                case 1://直降
                    {
                        orderProductInfo.ExtCode2 = 1;
                        decimal temp = orderProductInfo.ShopPrice - singlePromotionInfo.DiscountValue;
                        orderProductInfo.DiscountPrice = temp < 0 ? orderProductInfo.ShopPrice : temp;
                        break;
                    }
                case 2://折后价
                    {
                        orderProductInfo.ExtCode2 = 2;
                        orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    }
            }
            //设置赠送积分
            if (singlePromotionInfo.PayCredits > 0)
                orderProductInfo.PayCredits = singlePromotionInfo.PayCredits;

            //设置赠送优惠劵
            if (singlePromotionInfo.CouponTypeId > 0)
            {
                CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(singlePromotionInfo.CouponTypeId);
                if (couponTypeInfo != null)
                {
                    orderProductInfo.CouponTypeId = couponTypeInfo.CouponTypeId;
                    orderProductInfo.ExtStr3 = couponTypeInfo.Name;
                }
            }
        }

        /// <summary>
        /// 设置订单商品的满赠促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="fullSendPromotionInfo">满赠促销活动</param>
        public static void SetFullSendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, FullSendPromotionInfo fullSendPromotionInfo)
        {
            orderProductInfo.ExtCode7 = fullSendPromotionInfo.PmId;
            orderProductInfo.ExtCode8 = fullSendPromotionInfo.LimitMoney;
            orderProductInfo.ExtCode9 = fullSendPromotionInfo.AddMoney;
            orderProductInfo.ExtStr3 = fullSendPromotionInfo.Name;
        }

        /// <summary>
        /// 设置订单商品的满减促销活动
        /// </summary>
        /// <param name="fullCutOrderProductList">满减订单商品列表</param>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="fullCutPromotionInfo">满减促销活动</param>
        public static void SetFullCutPromotionOfOrderProduct(List<OrderProductInfo> fullCutOrderProductList, OrderProductInfo orderProductInfo, FullCutPromotionInfo fullCutPromotionInfo)
        {
            decimal productAmount = SumOrderProductAmount(fullCutOrderProductList);
            if (fullCutPromotionInfo.LimitMoney3 > 0 && fullCutPromotionInfo.LimitMoney3 <= productAmount)
            {
                orderProductInfo.ExtCode11 = fullCutPromotionInfo.LimitMoney3;
                orderProductInfo.ExtCode12 = fullCutPromotionInfo.CutMoney3;
            }
            else if (fullCutPromotionInfo.LimitMoney2 > 0 && fullCutPromotionInfo.LimitMoney2 <= productAmount)
            {
                orderProductInfo.ExtCode11 = fullCutPromotionInfo.LimitMoney2;
                orderProductInfo.ExtCode12 = fullCutPromotionInfo.CutMoney2;
            }
            else
            {
                orderProductInfo.ExtCode11 = fullCutPromotionInfo.LimitMoney1;
                orderProductInfo.ExtCode12 = fullCutPromotionInfo.CutMoney1;
            }
            orderProductInfo.ExtCode10 = fullCutPromotionInfo.PmId;
            orderProductInfo.ExtStr4 = fullCutPromotionInfo.Name;
        }

        /// <summary>
        /// 设置赠品订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="type">类型(0代表普通商品的赠品，其它代表套装商品的赠品并且值为套装促销活动的id)</param>
        /// <param name="pid">主商品id</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="number">赠送数量</param>
        /// <param name="pmId">赠品促销活动id</param>
        /// <param name="pmName">赠品促销活动名称</param>
        public static void SetGiftOrderProduct(OrderProductInfo orderProductInfo, int type, int pid, int buyCount, int number, int pmId, string pmName)
        {
            orderProductInfo.DiscountPrice = 0M;
            orderProductInfo.RealCount = buyCount * number;
            orderProductInfo.BuyCount = 0;
            orderProductInfo.Type = 1;
            orderProductInfo.ExtCode1 = pmId;
            orderProductInfo.ExtCode2 = pid;
            orderProductInfo.ExtCode3 = number;
            orderProductInfo.ExtCode4 = type;
            orderProductInfo.ExtStr1 = pmName;
        }

        /// <summary>
        /// 更新赠品订单商品
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="orderProductInfo">订单商品</param>
        public static void UpdateGiftOrderProduct(List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo)
        {
            //获得赠品订单商品列表
            List<OrderProductInfo> giftOrderProductList = GetGiftOrderProductList(orderProductInfo.Pid, orderProductList);
            foreach (OrderProductInfo item in giftOrderProductList)
            {
                item.RealCount = orderProductInfo.RealCount * item.ExtCode3;
                UpdateOrderProductCount(item.RecordId, item.RealCount, item.BuyCount);
            }
        }

        /// <summary>
        /// 设置套装订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="discount">折扣值</param>
        /// <param name="number">数量</param>
        /// <param name="suitPromotionInfo">套装促销活动</param>
        public static void SetSuitOrderProduct(OrderProductInfo orderProductInfo, int discount, int number, SuitPromotionInfo suitPromotionInfo)
        {
            orderProductInfo.Type = 2;
            orderProductInfo.ExtCode7 = suitPromotionInfo.PmId;
            orderProductInfo.ExtCode8 = discount;
            orderProductInfo.ExtCode9 = number;
            orderProductInfo.ExtStr3 = suitPromotionInfo.Name;
            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice - discount;
        }

        /// <summary>
        /// 设置满赠订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="limitMoney">限制金额</param>
        /// <param name="addMoney">补充金额</param>
        public static void SetFullSendOrderProduct(OrderProductInfo orderProductInfo, int pmId, int limitMoney, int addMoney)
        {
            orderProductInfo.RealCount = 1;
            orderProductInfo.BuyCount = 1;
            orderProductInfo.DiscountPrice = addMoney;
            orderProductInfo.Type = 3;
            orderProductInfo.ExtCode1 = pmId;
            orderProductInfo.ExtCode2 = limitMoney;
            orderProductInfo.ExtCode3 = addMoney;
        }




        /// <summary>
        /// 删除购物车中的商品
        /// </summary>
        /// <param name="orderProductList">购物车中商品列表</param>
        /// <param name="orderProductInfo">删除商品</param>
        public static void DeleteCartProduct(ref List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo)
        {
            //需要删除的商品列表
            List<OrderProductInfo> delOrderProductList = new List<OrderProductInfo>();

            //将主商品添加到需要删除的商品列表中
            delOrderProductList.Add(orderProductInfo);

            //赠品商品列表
            List<OrderProductInfo> giftOrderProductList = GetGiftOrderProductList(orderProductInfo.Pid, orderProductList);
            //将赠品添加到需要删除的商品列表中
            if (giftOrderProductList.Count > 0)
                delOrderProductList.AddRange(giftOrderProductList);

            //当商品存在满赠促销活动时
            if (orderProductInfo.ExtCode7 > 0)
            {
                //获得满赠促销活动的次商品
                OrderProductInfo fullSendMinorOrderProductInfo = GetFullSendMinorOrderProduct(orderProductInfo.ExtCode7, orderProductList);
                if (fullSendMinorOrderProductInfo != null)
                {
                    //获得满赠促销活动的主商品列表
                    List<OrderProductInfo> fullSendMainOrderProductList = GetFullSendMainOrderProductList(orderProductInfo.ExtCode7, orderProductList);
                    //从满赠促销活动的主商品列表移除当前商品
                    fullSendMainOrderProductList.Remove(orderProductInfo);
                    //判断剩余的主商品是否满足当前满赠促销活动，如果不满足则将次商品添加到需要删除的商品列表中
                    if (SumOrderProductCount(fullSendMainOrderProductList) < orderProductInfo.ExtCode8)
                        delOrderProductList.Add(fullSendMinorOrderProductInfo);
                }
            }

            //当商品存在满减促销活动时
            if (orderProductInfo.ExtCode10 > 0)
            {
                //获得满减促销活动的商品列表
                List<OrderProductInfo> fullCutOrderProductList = GetFullCutOrderProductList(orderProductInfo.ExtCode10, orderProductList);
                if (fullCutOrderProductList.Count > 1)
                {
                    //从满减促销活动的商品列表移除当前商品
                    fullCutOrderProductList.Remove(orderProductInfo);
                    //判断剩余的商品是否满足当前满减促销活动,如果不满足则更新商品的满减促销活动
                    decimal productAmount = SumOrderProductAmount(fullCutOrderProductList);
                    if (productAmount < orderProductInfo.ExtCode11)
                    {
                        FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByPmIdAndTime(orderProductInfo.ExtCode10, DateTime.Now);
                        if (fullCutPromotionInfo == null)
                        {
                            UpdateFullCutPromotionOfOrderProduct(GetRecordIdList(fullCutOrderProductList), 0, 0);
                        }
                        else
                        {
                            if (fullCutPromotionInfo.LimitMoney1 != orderProductInfo.ExtCode10 && fullCutPromotionInfo.CutMoney1 != orderProductInfo.ExtCode11)
                            {
                                int limitMoney;
                                int cutMoeny;
                                if (fullCutPromotionInfo.LimitMoney3 > 0 && fullCutPromotionInfo.LimitMoney3 <= productAmount)
                                {
                                    limitMoney = fullCutPromotionInfo.LimitMoney3;
                                    cutMoeny = fullCutPromotionInfo.CutMoney3;
                                }
                                else if (fullCutPromotionInfo.LimitMoney2 > 0 && fullCutPromotionInfo.LimitMoney2 <= productAmount)
                                {
                                    limitMoney = fullCutPromotionInfo.LimitMoney2;
                                    cutMoeny = fullCutPromotionInfo.CutMoney2;
                                }
                                else
                                {
                                    limitMoney = fullCutPromotionInfo.LimitMoney1;
                                    cutMoeny = fullCutPromotionInfo.CutMoney1;
                                }

                                UpdateFullCutPromotionOfOrderProduct(GetRecordIdList(fullCutOrderProductList), limitMoney, cutMoeny);

                                foreach (OrderProductInfo item in fullCutOrderProductList)
                                {
                                    item.ExtCode10 = limitMoney;
                                    item.ExtCode11 = cutMoeny;
                                }
                            }
                        }
                    }
                }
            }

            //删除商品
            DeleteOrderProductByRecordId(GetRecordIdList(delOrderProductList));
            foreach (OrderProductInfo item in delOrderProductList)
                orderProductList.Remove(item);
        }

        /// <summary>
        /// 添加已经存在的商品到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="sid">sid</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddExistProductToCart(ref List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo, int buyCount, string sid, int uid, DateTime buyTime)
        {
            //更新买送促销活动
            bool isUpdateByBuySendPromotion = UpdateBuySendPromotionOfOrderProduct(orderProductInfo, orderProductInfo.BuyCount + buyCount, buyTime);

            //获得满减促销活动
            FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(orderProductInfo.StoreId, orderProductInfo.Pid, buyTime);
            if (fullCutPromotionInfo != null)
            {
                if (fullCutPromotionInfo.PmId == orderProductInfo.ExtCode10)//当满减活动相同时
                {
                    List<OrderProductInfo> fullCutOrderProductList = GetFullCutOrderProductList(fullCutPromotionInfo.PmId, orderProductList);
                    int temp1 = orderProductInfo.ExtCode11;
                    int temp2 = orderProductInfo.ExtCode12;
                    SetFullCutPromotionOfOrderProduct(fullCutOrderProductList, orderProductInfo, fullCutPromotionInfo);

                    if (temp1 != orderProductInfo.ExtCode11 || temp2 != orderProductInfo.ExtCode12)//当满减等级改变时
                    {
                        isUpdateByBuySendPromotion = true;
                        //同步各满减商品的属性
                        if (fullCutOrderProductList.Count > 1)
                        {
                            string recordIdList = "";
                            for (int i = 0; i < fullCutOrderProductList.Count - 1; i++)
                            {
                                OrderProductInfo fullCutOrderProductInfo = fullCutOrderProductList[i];
                                if (fullCutOrderProductInfo.RecordId == fullCutOrderProductInfo.RecordId)
                                    continue;
                                fullCutOrderProductInfo.ExtCode11 = orderProductInfo.ExtCode11;
                                fullCutOrderProductInfo.ExtCode12 = orderProductInfo.ExtCode12;
                                recordIdList = recordIdList + fullCutOrderProductInfo.RecordId + ",";
                            }
                            UpdateFullCutPromotionOfOrderProduct(recordIdList.Remove(recordIdList.Length - 1, 1), orderProductInfo.ExtCode11, orderProductInfo.ExtCode11);
                        }
                    }
                }
                else//当满减活动不相同时
                {
                    orderProductList.Clear();
                    ClearCart(uid, sid);
                    return;
                }
            }
            else//当满减活动不存在时
            {
                if (orderProductInfo.ExtCode10 > 0)
                {
                    orderProductList.Clear();
                    ClearCart(uid, sid);
                    return;
                }
            }

            //当商品信息改变时更新商品信息
            if (isUpdateByBuySendPromotion)
                UpdateOrderProduct(orderProductInfo);
            else
                UpdateOrderProductCount(orderProductInfo.RecordId, orderProductInfo.RealCount, orderProductInfo.BuyCount);

            //更新订单商品的赠品促销活动
            UpdateGiftOrderProduct(orderProductList, orderProductInfo);
        }

        /// <summary>
        /// 添加新商品到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="ip">ip地址</param>
        public static void AddNewProductToCart(ref List<OrderProductInfo> orderProductList, int buyCount, PartProductInfo partProductInfo, string sid, int uid, DateTime buyTime, string ip)
        {
            //需要添加的商品列表
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();

            //初始化订单商品
            OrderProductInfo mainOrderProductInfo = BuildOrderProduct(partProductInfo);
            InitOrderProduct(mainOrderProductInfo, buyCount, sid, uid, buyTime, ip);

            //获得买送促销活动
            BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(buyCount, partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            //当买送促销活动存在时设置订单商品信息
            if (buySendPromotionInfo != null)
                SetBuySendPromotionOfOrderProduct(mainOrderProductInfo, buyCount, buySendPromotionInfo);

            //获得单品促销活动
            SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(partProductInfo.Pid, buyTime);
            //当单品促销活动存在时则设置订单商品信息
            if (singlePromotionInfo != null)
                SetSinglePromotionOfOrderProduct(mainOrderProductInfo, singlePromotionInfo);

            //获得满赠促销活动
            FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            if (fullSendPromotionInfo != null)
                SetFullSendPromotionOfOrderProduct(mainOrderProductInfo, fullSendPromotionInfo);

            //获得满减促销活动
            FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            if (fullCutPromotionInfo != null)
            {
                List<OrderProductInfo> fullCutOrderProductList = GetFullCutOrderProductList(fullCutPromotionInfo.PmId, orderProductList);
                fullCutOrderProductList.Add(mainOrderProductInfo);
                SetFullCutPromotionOfOrderProduct(fullCutOrderProductList, mainOrderProductInfo, fullCutPromotionInfo);
                //同步各个满减商品的属性
                if (fullCutOrderProductList.Count > 1 && (fullCutOrderProductList[0].ExtCode11 != mainOrderProductInfo.ExtCode11 || fullCutOrderProductList[0].ExtCode12 != mainOrderProductInfo.ExtCode12))
                {
                    string recordIdList = "";
                    for (int i = 0; i < fullCutOrderProductList.Count - 2; i++)
                    {
                        OrderProductInfo fullCutOrderProductInfo = fullCutOrderProductList[i];
                        fullCutOrderProductInfo.ExtCode11 = mainOrderProductInfo.ExtCode11;
                        fullCutOrderProductInfo.ExtCode12 = mainOrderProductInfo.ExtCode12;
                        recordIdList = recordIdList + fullCutOrderProductInfo.RecordId + ",";
                    }
                    UpdateFullCutPromotionOfOrderProduct(recordIdList.Remove(recordIdList.Length - 1, 1), mainOrderProductInfo.ExtCode11, mainOrderProductInfo.ExtCode12);
                }
            }

            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            if (1 == 1)
            {
                //获得赠品列表
                DataTable giftList = Promotions.GetGiftList(partProductInfo.Pid, buyTime);
                foreach (DataRow row in giftList.Rows)
                {
                    OrderProductInfo giftOrderProduct = BuildOrderProduct(row);
                    InitOrderProduct(giftOrderProduct, 0, sid, uid, buyTime, ip);
                    SetGiftOrderProduct(giftOrderProduct, 0, mainOrderProductInfo.Pid, mainOrderProductInfo.RealCount, TypeHelper.ObjectToInt(row["number"]), TypeHelper.ObjectToInt(row["pmid"]), row["pmname"].ToString());
                    //将赠品添加到"需要添加的商品列表"中
                    addOrderProductList.Add(giftOrderProduct);
                }
            }

            //将需要添加的商品持久化
            foreach (OrderProductInfo addOrderProductInfo in addOrderProductList)
            {
                int recordId = AddOrderProduct(addOrderProductInfo);
                addOrderProductInfo.RecordId = recordId;
            }

            orderProductList.AddRange(addOrderProductList);
        }

        /// <summary>
        /// 将商品添加到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        public static void AddProductToCart(ref List<OrderProductInfo> orderProductList, int buyCount, PartProductInfo partProductInfo, string sid, int uid, DateTime buyTime, string ip)
        {
            if (orderProductList.Count == 0)
            {
                AddNewProductToCart(ref orderProductList, buyCount, partProductInfo, sid, uid, buyTime, ip);
            }
            else
            {
                OrderProductInfo orderProductInfo = GetCommonOrderProductByPid(partProductInfo.Pid, orderProductList);
                if (orderProductInfo == null)//此商品作为普通商品不存在于购物车中时
                    AddNewProductToCart(ref orderProductList, buyCount, partProductInfo, sid, uid, buyTime, ip);
                else//此商品作为普通商品存在于购物车中时
                    AddExistProductToCart(ref orderProductList, orderProductInfo, buyCount, sid, uid, buyTime);
            }
        }





        /// <summary>
        /// 删除购物车中的套装
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="pmId">套装促销活动id</param>
        public static void DeleteCartSuit(ref List<OrderProductInfo> orderProductList, int pmId)
        {
            StringBuilder recordIdList = new StringBuilder();
            for (int i = 0; i < orderProductList.Count; )
            {
                OrderProductInfo orderProductInfo = orderProductList[i];
                if ((orderProductInfo.Type == 2 && orderProductInfo.ExtCode7 == pmId) || (orderProductInfo.Type == 1 && orderProductInfo.ExtCode4 == pmId))
                {
                    recordIdList.AppendFormat("{0},", orderProductInfo.RecordId);
                    orderProductList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            if (recordIdList.Length > 0)
            {
                recordIdList.Remove(recordIdList.Length - 1, 1);
                DeleteOrderProductByRecordId(recordIdList.ToString());
            }
        }

        /// <summary>
        /// 添加已经存在的套装到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="suitOrderProductList">套装商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddExistSuitToCart(ref List<OrderProductInfo> orderProductList, List<OrderProductInfo> suitOrderProductList, int buyCount, DateTime buyTime)
        {
            //更新非赠品数量
            foreach (OrderProductInfo orderProductInfo in suitOrderProductList)
            {
                if (orderProductInfo.Type == 2)
                {
                    int newCount = orderProductInfo.RealCount + buyCount * orderProductInfo.ExtCode9;
                    orderProductInfo.RealCount = orderProductInfo.BuyCount = newCount;
                    UpdateOrderProductCount(orderProductInfo.RecordId, newCount, newCount);
                }
            }

            //更新赠品数量
            foreach (OrderProductInfo orderProductInfo in suitOrderProductList)
            {
                if (orderProductInfo.Type == 1)
                {
                    OrderProductInfo temp = null;
                    foreach (OrderProductInfo item in suitOrderProductList)
                    {
                        if (item.Type == 2 && item.Pid == orderProductInfo.ExtCode2)
                        {
                            temp = item;
                            break;
                        }
                    }
                    int newCount = temp.RealCount * orderProductInfo.ExtCode3;
                    orderProductInfo.RealCount = newCount;
                    UpdateOrderProductCount(orderProductInfo.RecordId, newCount, newCount);
                }
            }
        }

        /// <summary>
        /// 添加新套装到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="suitProductList">套装商品列表</param>
        /// <param name="suitPromotionInfo">套装促销活动信息</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="ip">ip地址</param>
        public static void AddNewSuitToCart(ref List<OrderProductInfo> orderProductList, DataTable suitProductList, SuitPromotionInfo suitPromotionInfo, int buyCount, string sid, int uid, DateTime buyTime, string ip)
        {
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();

            foreach (DataRow row in suitProductList.Rows)
            {
                OrderProductInfo suitOrderProductInfo = BuildOrderProduct(row);
                InitOrderProduct(suitOrderProductInfo, buyCount * TypeHelper.ObjectToInt(row["number"]), sid, uid, buyTime, ip);
                SetSuitOrderProduct(suitOrderProductInfo, TypeHelper.ObjectToInt(row["discount"]), TypeHelper.ObjectToInt(row["number"]), suitPromotionInfo);

                addOrderProductList.Add(suitOrderProductInfo);

                //获得赠品列表
                DataTable giftList = Promotions.GetGiftList(suitOrderProductInfo.Pid, buyTime);
                foreach (DataRow gift in giftList.Rows)
                {
                    OrderProductInfo giftOrderProductInfo = BuildOrderProduct(gift);
                    InitOrderProduct(giftOrderProductInfo, 0, sid, uid, buyTime, ip);
                    SetGiftOrderProduct(giftOrderProductInfo, suitPromotionInfo.PmId, suitOrderProductInfo.Pid, suitOrderProductInfo.RealCount, TypeHelper.ObjectToInt(gift["number"]), TypeHelper.ObjectToInt(gift["pmid"]), gift["pmname"].ToString());
                    //将赠品添加到"需要添加的商品列表"中
                    addOrderProductList.Add(giftOrderProductInfo);
                }
            }

            //将需要添加的商品持久化
            foreach (OrderProductInfo addOrderProductInfo in addOrderProductList)
            {
                int recordId = AddOrderProduct(addOrderProductInfo);
                addOrderProductInfo.RecordId = recordId;
            }

            orderProductList.AddRange(addOrderProductList);
        }

        /// <summary>
        /// 添加套装到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="suitProductList">套装商品列表</param>
        /// <param name="suitPromotionInfo">套装促销活动</param>
        /// <param name="buyCount">购买数量</param>
        /// <returns></returns>
        public static void AddSuitToCart(ref List<OrderProductInfo> orderProductList, DataTable suitProductList, SuitPromotionInfo suitPromotionInfo, int buyCount, string sid, int uid, DateTime buyTime, string ip)
        {
            //套装商品列表
            List<OrderProductInfo> suitOrderProductList = Orders.GetSuitOrderProductList(suitPromotionInfo.PmId, orderProductList, true);
            if (suitOrderProductList.Count < 1)
                AddNewSuitToCart(ref orderProductList, suitProductList, suitPromotionInfo, buyCount, sid, uid, buyTime, ip);
            else
                AddExistSuitToCart(ref orderProductList, suitOrderProductList, buyCount, buyTime);
        }





        /// <summary>
        /// 删除购物车中的满赠
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="pmId">满赠促销活动id</param>
        public static void DeleteCartFullSend(ref List<OrderProductInfo> orderProductList, int pmId)
        {
            //赠送商品
            OrderProductInfo fullSendMinorOrderProductInfo = Orders.GetFullSendMinorOrderProduct(pmId, orderProductList);
            if (fullSendMinorOrderProductInfo != null)
            {
                orderProductList.Remove(fullSendMinorOrderProductInfo);
                DeleteOrderProductByRecordId(fullSendMinorOrderProductInfo.RecordId.ToString());
            }
        }

        /// <summary>
        /// 删除购物车中的满赠
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="fullSendMinorOrderProductInfo">赠送商品</param>
        public static void DeleteCartFullSend(ref List<OrderProductInfo> orderProductList, OrderProductInfo fullSendMinorOrderProductInfo)
        {
            orderProductList.Remove(fullSendMinorOrderProductInfo);
            DeleteOrderProductByRecordId(fullSendMinorOrderProductInfo.RecordId.ToString());
        }

        /// <summary>
        /// 添加满赠到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="limitMoney">限制金额</param>
        /// <param name="addMoney">补充金额</param>
        public static void AddFullSendToCart(ref List<OrderProductInfo> orderProductList, PartProductInfo partProductInfo, int pmId, int limitMoney, int addMoney, string sid, int uid, DateTime buyTime, string ip)
        {
            OrderProductInfo orderProductInfo = BuildOrderProduct(partProductInfo);
            InitOrderProduct(orderProductInfo, 1, sid, uid, buyTime, ip);
            SetFullSendOrderProduct(orderProductInfo, pmId, limitMoney, addMoney);
            int recordId = AddOrderProduct(orderProductInfo);
            orderProductInfo.RecordId = recordId;
            orderProductList.Add(orderProductInfo);
        }




        /// <summary>
        /// 验证商品信息
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="partProductList">商品列表</param>
        /// <returns></returns>
        public static PartProductInfo VerifyProductInfo(List<OrderProductInfo> orderProductList, List<PartProductInfo> partProductList)
        {
            foreach (PartProductInfo partProductInfo in partProductList)
            {
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if (orderProductInfo.Pid == partProductInfo.Pid)
                    {
                        if (orderProductInfo.Name != partProductInfo.Name
                            || orderProductInfo.StoreCid != partProductInfo.StoreCid
                            || orderProductInfo.StoreSTid != partProductInfo.StoreSTid
                            || orderProductInfo.ShopPrice != partProductInfo.ShopPrice
                            || orderProductInfo.MarketPrice != partProductInfo.MarketPrice
                            || orderProductInfo.CostPrice != partProductInfo.CostPrice
                            || orderProductInfo.Weight != partProductInfo.Weight
                            || orderProductInfo.PSN != partProductInfo.PSN)
                        {
                            return partProductInfo;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 验证商品库存
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="productStockList">商品库存列表</param>
        /// <returns></returns>
        public static OrderProductInfo VerifyProductStock(List<OrderProductInfo> orderProductList, List<ProductStockInfo> productStockList)
        {
            int totalCount = 0;
            OrderProductInfo orderProductInfo = null;
            foreach (ProductStockInfo productStockInfo in productStockList)
            {
                foreach (OrderProductInfo item in orderProductList)
                {
                    if (item.Pid == productStockInfo.Pid)
                    {
                        orderProductInfo = item;
                        totalCount += orderProductInfo.RealCount;
                    }
                }
                if (totalCount > productStockInfo.Number)
                    break;

                totalCount = 0;
                orderProductInfo = null;
            }
            return orderProductInfo;
        }

        /// <summary>
        /// 验证买送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static void VerifyBuySendPromotion(out int result, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            if (orderProductInfo.ExtCode4 > 0)
            {
                //买送促销活动
                BuySendPromotionInfo promotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                if (promotionInfo == null)
                {
                    result = 1;
                }
                else if (promotionInfo.PmId != orderProductInfo.ExtCode4)
                {
                    result = 2;
                }
                else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
                {
                    result = 3;
                }
                else if ((promotionInfo.BuyCount != orderProductInfo.ExtCode5) || (promotionInfo.SendCount != orderProductInfo.ExtCode6) || (promotionInfo.Name != orderProductInfo.ExtStr2))
                {
                    result = 4;
                }
            }
        }

        /// <summary>
        /// 验证单品促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static SinglePromotionInfo VerifySinglePromotion(out int result, ref List<SinglePromotionInfo> singlePromotionList, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            if (orderProductInfo.ExtCode1 > 0)
            {
                //单品促销活动
                SinglePromotionInfo promotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                if (promotionInfo == null)
                {
                    result = 1;
                }
                else if (promotionInfo.PmId != orderProductInfo.ExtCode1)
                {
                    result = 2;
                }
                else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
                {
                    result = 3;
                }
                else if (promotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < promotionInfo.QuotaLower)
                {
                    result = 4;
                }
                else if (promotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > promotionInfo.QuotaUpper)
                {
                    result = 5;
                }
                else if (promotionInfo.IsStock == 1 && promotionInfo.Stock < orderProductInfo.RealCount)
                {
                    result = 6;
                }
                else if (promotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(partUserInfo.Uid, promotionInfo.PmId) > promotionInfo.AllowBuyCount)
                {
                    result = 7;
                }
                else if (promotionInfo.Name != orderProductInfo.ExtStr1
                         || promotionInfo.PayCredits != orderProductInfo.PayCredits
                         || promotionInfo.DiscountType != orderProductInfo.ExtCode2
                         || promotionInfo.DiscountValue != orderProductInfo.ExtCode3
                         || promotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                {
                    result = 8;
                }
                else if (promotionInfo.CouponTypeId > 0)
                {
                    CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(promotionInfo.CouponTypeId);
                    if (couponTypeInfo == null)
                        result = 8;
                }

                if (promotionInfo.IsStock == 1 || promotionInfo.PayCredits > 0 || promotionInfo.CouponTypeId > 0)
                {
                    //限购
                    if (promotionInfo.IsStock == 1)
                    {
                        promotionInfo.Stock = promotionInfo.Stock - orderProductInfo.RealCount;
                        singlePromotionList.Add(promotionInfo);
                    }
                }

                return promotionInfo;
            }
            return null;
        }

        /// <summary>
        /// 验证满送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static void VerifyFullSendPromotion(out int result, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            if (orderProductInfo.ExtCode7 > 0)
            {
                FullSendPromotionInfo promotionInfo = Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                if (promotionInfo == null)
                {
                    result = 1;
                }
                else if (promotionInfo.PmId != orderProductInfo.ExtCode7)
                {
                    result = 2;
                }
                else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
                {
                    result = 3;
                }
                else if ((promotionInfo.LimitMoney != orderProductInfo.ExtCode8) || (promotionInfo.AddMoney != orderProductInfo.ExtCode9))
                {
                    result = 4;
                }
            }
        }

        /// <summary>
        /// 验证满减促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static void VerifyFullCutPromotion(out int result, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            if (orderProductInfo.ExtCode10 > 0)
            {
                //满减促销活动
                FullCutPromotionInfo promotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                if (promotionInfo == null)
                {
                    result = 1;
                }
                else if (promotionInfo.PmId != orderProductInfo.ExtCode10)
                {
                    result = 2;
                }
                else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
                {
                    result = 3;
                }
                else if (!((promotionInfo.LimitMoney1 == orderProductInfo.ExtCode11 && promotionInfo.CutMoney1 == orderProductInfo.ExtCode12)
                        || (promotionInfo.LimitMoney2 == orderProductInfo.ExtCode11 && promotionInfo.CutMoney2 == orderProductInfo.ExtCode12)
                        || (promotionInfo.LimitMoney3 == orderProductInfo.ExtCode11 && promotionInfo.CutMoney3 == orderProductInfo.ExtCode12)))
                {
                    result = 4;
                }
            }
        }

        /// <summary>
        /// 验证赠品促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static GiftPromotionInfo VerifyGiftPromotion(out int result, List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            //赠品促销活动
            GiftPromotionInfo promotionInfo = null;

            List<OrderProductInfo> oldGiftOrderProductList = GetGiftOrderProductList(orderProductInfo.Pid, orderProductList);
            if (oldGiftOrderProductList.Count > 0)
            {
                promotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                if (promotionInfo == null)
                {
                    result = 1;
                }
                else if (promotionInfo.PmId != oldGiftOrderProductList[0].ExtCode1)
                {
                    result = 2;
                }
                else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
                {
                    result = 3;
                }
                else if (promotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > promotionInfo.QuotaUpper)
                {
                    result = 4;
                }
                else
                {
                    //赠品列表
                    DataTable giftList = Promotions.GetGiftList(orderProductInfo.Pid, DateTime.Now);
                    if (giftList.Rows.Count < 1)//如果赠品不存在
                    {
                        result = 5;
                    }
                    else if (giftList.Rows.Count != oldGiftOrderProductList.Count)//如果赠品列表已经改变
                    {
                        result = 6;
                    }
                    else
                    {
                        List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(giftList.Rows.Count);
                        foreach (DataRow row in giftList.Rows)
                        {
                            OrderProductInfo giftOrderProduct = BuildOrderProduct(row);
                            SetGiftOrderProduct(giftOrderProduct, 0, orderProductInfo.Pid, orderProductInfo.RealCount, TypeHelper.ObjectToInt(row["number"]), TypeHelper.ObjectToInt(row["pmid"]), row["pmname"].ToString());
                            newGiftOrderProductList.Add(giftOrderProduct);
                        }
                        foreach (OrderProductInfo oldGift in oldGiftOrderProductList)
                        {
                            OrderProductInfo newGift = GetOrderProductByPid(oldGift.Pid, newGiftOrderProductList);
                            if (newGift == null || newGift.ExtCode3 != oldGift.ExtCode3)
                            {
                                result = 7;
                                break;
                            }
                        }
                    }
                }
            }
            return promotionInfo;
        }

        /// <summary>
        /// 验证套装促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="partUserInfo">用户信息</param>
        /// <returns></returns>
        public static SuitPromotionInfo VerifySuitPromotion(out int result, ref List<SinglePromotionInfo> stockSinglePromotionList, List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo, PartUserInfo partUserInfo)
        {
            result = 0;
            //套装促销活动
            SuitPromotionInfo promotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(orderProductInfo.ExtCode7, DateTime.Now);
            //套装促销活动不存在时
            if (promotionInfo == null)
            {
                result = -1;
            }
            else if (promotionInfo.UserRankLower > partUserInfo.UserRid)
            {
                result = -2;
            }
            else if (promotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount / orderProductInfo.ExtCode9 > promotionInfo.QuotaUpper)
            {
                result = -3;
            }
            else if (promotionInfo.OnlyOnce == 1 && Promotions.IsJoinSuitPromotion(partUserInfo.Uid, promotionInfo.PmId))
            {
                result = -4;
            }

            List<OrderProductInfo> newSuitProductList = new List<OrderProductInfo>();
            foreach (DataRow row in Promotions.GetSuitProductList(promotionInfo.PmId).Rows)
            {
                OrderProductInfo suitOrderProductInfo = BuildOrderProduct(row);
                SetSuitOrderProduct(suitOrderProductInfo, TypeHelper.ObjectToInt(row["discount"]), TypeHelper.ObjectToInt(row["number"]), promotionInfo);
                newSuitProductList.Add(suitOrderProductInfo);
            }

            if (newSuitProductList.Count != GetSuitOrderProductList(orderProductInfo.ExtCode7, orderProductList, false).Count)
            {
                result = -5;
            }
            else
            {
                List<OrderProductInfo> oldSuitProductList = GetSuitOrderProductList(orderProductInfo.ExtCode7, orderProductList, true);
                foreach (OrderProductInfo oldSuitProductInfo in oldSuitProductList)
                {
                    if (oldSuitProductInfo.Type == 2)
                    {
                        OrderProductInfo newSuitProductInfo = GetOrderProductByPid(oldSuitProductInfo.Pid, newSuitProductList);
                        if (newSuitProductInfo == null || (newSuitProductInfo.ExtCode8 != oldSuitProductInfo.ExtCode8) || (newSuitProductInfo.ExtCode9 != oldSuitProductInfo.ExtCode9))
                        {
                            result = -6;
                            break;
                        }
                        else
                        {
                            VerifyGiftPromotion(out result, orderProductList, orderProductInfo, partUserInfo);
                            if (result > 0)
                                break;
                        }
                    }
                }
            }
            return promotionInfo;
        }





        /// <summary>
        /// 重置订单编号格式
        /// </summary>
        public static void ResetOSNFormat()
        {
            lock (_locker)
            {
                _osnformat = BMAConfig.MallConfig.OSNFormat;
            }
        }

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="uid">用户id</param>
        /// <param name="shipRegionId">配送区域id</param>
        /// <param name="addTime">下单时间</param>
        /// <returns>订单编号</returns>
        private static string GenerateOSN(int storeId, int uid, int shipRegionId, DateTime addTime)
        {
            StringBuilder osn = new StringBuilder(_osnformat);
            osn.Replace("{storeid}", storeId.ToString());
            osn.Replace("{uid}", uid.ToString());
            osn.Replace("{srid}", shipRegionId.ToString());
            osn.Replace("{tdoc}", _todayordercount.ToString());
            osn.Replace("{addtime}", addTime.ToString("yyyyMMddHHmm"));
            return osn.ToString();
        }

        /// <summary>
        /// 获得配送费用
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int GetShipFee(int provinceId, int cityId, List<OrderProductInfo> orderProductList)
        {
            List<int> storeSTidList = new List<int>(orderProductList.Count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeSTidList.Add(orderProductInfo.StoreSTid);
            }
            storeSTidList = storeSTidList.Distinct<int>().ToList<int>();

            int shipFee = 0;
            foreach (int storeSTId in storeSTidList)
            {
                StoreShipTemplateInfo storeShipTemplateInfo = Stores.GetStoreShipTemplateById(storeSTId);
                if (storeShipTemplateInfo.Free == 0)
                {
                    StoreShipFeeInfo storeShipFeeInfo = Stores.GetStoreShipFeeByStoreSTidAndRegion(storeSTId, provinceId, cityId);
                    List<OrderProductInfo> list = GetSameShipOrderProductList(storeSTId, orderProductList);
                    if (storeShipTemplateInfo.Type == 0)
                    {
                        int totalCount = SumOrderProductCount(orderProductList);
                        if (totalCount <= storeShipFeeInfo.StartValue)
                        {
                            shipFee += storeShipFeeInfo.StartFee;
                        }
                        else
                        {
                            int temp = 0;
                            if ((totalCount - storeShipFeeInfo.StartValue) % storeShipFeeInfo.AddValue == 0)
                                temp = (totalCount - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue;
                            else
                                temp = (totalCount - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue + 1;
                            shipFee += storeShipFeeInfo.StartFee + temp * storeShipFeeInfo.AddFee;
                        }
                    }
                    else
                    {
                        int totalWeight = SumOrderProductWeight(orderProductList);
                        if (totalWeight <= storeShipFeeInfo.StartValue)
                        {
                            shipFee += storeShipFeeInfo.StartFee;
                        }
                        else
                        {
                            int temp = 0;
                            if ((totalWeight - storeShipFeeInfo.StartValue) % storeShipFeeInfo.AddValue == 0)
                                temp = (totalWeight - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue;
                            else
                                temp = (totalWeight - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue + 1;
                            shipFee += storeShipFeeInfo.StartFee + temp * storeShipFeeInfo.AddFee;
                        }
                    }
                }
            }

            return shipFee;
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOid(int oid)
        {
            if (oid > 0)
                return BrnMall.Data.Orders.GetOrderByOid(oid);
            else
                return null;
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="osn">订单编号</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOSN(string osn)
        {
            if (!string.IsNullOrWhiteSpace(osn))
                return BrnMall.Data.Orders.GetOrderByOSN(osn);
            return null;
        }

        /// <summary>
        /// 获得订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static int GetOrderStateByOid(int oid)
        {
            return BrnMall.Data.Orders.GetOrderStateByOid(oid);
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetOrderCountByCondition(int storeId, int orderState, string startTime, string endTime)
        {
            return BrnMall.Data.Orders.GetOrderCountByCondition(storeId, orderState, startTime, endTime);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(int oid)
        {
            return BrnMall.Data.Orders.GetOrderProductList(oid);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oidList">订单id列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(string oidList)
        {
            if (!string.IsNullOrEmpty(oidList))
                return BrnMall.Data.Orders.GetOrderProductList(oidList);
            return new List<OrderProductInfo>();
        }

        #region 订单操作

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺id</param>
        /// <param name="orderProductList">商品列表</param>
        /// <param name="singlePromotionList">单品促销活动</param>
        /// <param name="fullShipAddressInfo">配送地址</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="payCreditCount">支付积分数</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="bestTime">最佳配送时间</param>
        /// <param name="buyerRemark">买家备注</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder(PartUserInfo partUserInfo, StoreInfo storeInfo, List<OrderProductInfo> orderProductList, List<SinglePromotionInfo> singlePromotionList, FullShipAddressInfo fullShipAddressInfo, PluginInfo payPluginInfo, ref int payCreditCount, List<CouponInfo> couponList, DateTime bestTime, string buyerRemark, string ip)
        {
            lock (_locker)
            {
                DateTime nowTime = DateTime.Now;
                //如果日期改变，则重置新日期和今天的订单数
                if (_thisdate != nowTime.Date)
                {
                    _thisdate = nowTime.Date;
                    _todayordercount = 0;
                }

                //今天的订单数量增加1
                _todayordercount++;
                //生成订单号
                string osn = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, fullShipAddressInfo.RegionId, nowTime);

                IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

                OrderInfo orderInfo = new OrderInfo();

                orderInfo.OSN = osn;
                orderInfo.Uid = partUserInfo.Uid;

                orderInfo.Weight = SumOrderProductWeight(orderProductList);
                orderInfo.ProductAmount = SumOrderProductAmount(orderProductList);
                orderInfo.FullCut = SumFullCut(orderProductList);
                decimal amount = orderInfo.ProductAmount - orderInfo.FullCut;
                orderInfo.ShipFee = GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, orderProductList);
                orderInfo.PayFee = payPlugin.GetPayFee(amount, nowTime, partUserInfo);
                orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee;

                decimal payCreditMoney = Credits.PayCreditsToMoney(payCreditCount);
                if (orderInfo.OrderAmount >= payCreditMoney)
                {
                    orderInfo.PayCreditCount = payCreditCount;
                    orderInfo.PayCreditMoney = payCreditMoney;
                    payCreditCount = 0;
                }
                else
                {
                    int orderPayCredits = Credits.MoneyToPayCredits(orderInfo.OrderAmount);
                    orderInfo.PayCreditCount = orderPayCredits;
                    orderInfo.PayCreditMoney = orderInfo.OrderAmount;
                    payCreditCount = payCreditCount - orderPayCredits;
                }

                orderInfo.CouponMoney = Coupons.SumCouponMoney(couponList);
                orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney;

                orderInfo.OrderState = orderInfo.SurplusMoney > 0M ? (int)OrderState.WaitPaying : (int)OrderState.Confirming;

                orderInfo.ParentId = 0;
                orderInfo.AddTime = nowTime;
                orderInfo.StoreId = storeInfo.StoreId;
                orderInfo.StoreName = storeInfo.Name;
                orderInfo.PaySystemName = payPluginInfo.SystemName;
                orderInfo.PayFriendName = payPluginInfo.FriendlyName;

                orderInfo.RegionId = fullShipAddressInfo.RegionId;
                orderInfo.Consignee = fullShipAddressInfo.Consignee;
                orderInfo.Mobile = fullShipAddressInfo.Mobile;
                orderInfo.Phone = fullShipAddressInfo.Phone;
                orderInfo.Email = fullShipAddressInfo.Email;
                orderInfo.ZipCode = fullShipAddressInfo.ZipCode;
                orderInfo.Address = fullShipAddressInfo.Address;
                orderInfo.BestTime = bestTime;

                orderInfo.BuyerRemark = buyerRemark;
                orderInfo.IP = ip;

                int oid = 0;
                try
                {
                    //添加订单
                    oid = BrnMall.Data.Orders.CreateOrder(orderInfo);
                }
                catch (Exception ex)
                {
                    _todayordercount--;
                    throw ex;
                }

                if (oid > 0)
                {
                    orderInfo.Oid = oid;

                    //减少商品库存数量
                    Products.DecreaseProductStockNumber(orderProductList);
                    //更新限购库存
                    if (singlePromotionList.Count > 0)
                        Promotions.UpdateSinglePromotionStock(singlePromotionList);
                    //使用支付积分
                    Credits.PayOrder(ref partUserInfo, orderInfo, payCreditCount, nowTime);
                    //使用优惠劵
                    foreach (CouponInfo couponInfo in couponList)
                    {
                        if (couponInfo.Uid > 0)
                            Coupons.UseCoupon(couponInfo.CouponId, oid, nowTime, ip);
                        else
                            Coupons.ActivateAndUseCoupon(couponInfo.CouponId, partUserInfo.Uid, oid, nowTime, ip);
                    }

                    return orderInfo;
                }
                else
                {
                    //当订单添加失败时，将订单数减一
                    _todayordercount--;
                    return null;
                }
            }
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="discount">折扣</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        public static void UpdateOrderDiscount(int oid, decimal discount, decimal orderAmount, decimal surplusMoney)
        {
            BrnMall.Data.Orders.UpdateOrderDiscount(oid, discount, orderAmount, surplusMoney);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">配送费用</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        public static void UpdateOrderShipFee(int oid, decimal shipFee, decimal orderAmount, decimal surplusMoney)
        {
            BrnMall.Data.Orders.UpdateOrderShipFee(oid, shipFee, orderAmount, surplusMoney);
        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="paySN">支付单号</param>
        /// <param name="payTime">支付时间</param>
        public static void PayOrder(int oid, OrderState orderState, string paySN, DateTime payTime)
        {
            BrnMall.Data.Orders.PayOrder(oid, orderState, paySN, payTime);
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void ConfirmOrder(OrderInfo orderInfo)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
        }

        /// <summary>
        /// 备货
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void PreProduct(OrderInfo orderInfo)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.PreProducting);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="shipSN">配送单号</param>
        /// <param name="shipCoId">配送公司id</param>
        /// <param name="shipCoName">配送公司名称</param>
        /// <param name="shipTime">配送时间</param>
        public static void SendOrder(int oid, OrderState orderState, string shipSN, int shipCoId, string shipCoName, DateTime shipTime)
        {
            BrnMall.Data.Orders.SendOrderProduct(oid, orderState, shipSN, shipCoId, shipCoName, shipTime);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="completeTime">完成时间</param>
        /// <param name="ip">ip</param>
        public static void CompleteOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, DateTime completeTime, string ip)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Completed);//将订单状态设为完成状态

            //订单商品列表
            List<OrderProductInfo> orderProductList = GetOrderProductList(orderInfo.Oid);

            //发放完成订单积分
            Credits.SendCompleteOrderCredits(ref partUserInfo, orderInfo, orderProductList, completeTime);

            //发放单品促销活动支付积分和优惠劵
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0 && orderProductInfo.ExtCode1 > 0)
                {
                    if (orderProductInfo.PayCredits > 0)
                        Credits.SendSinglePromotionCredits(ref partUserInfo, orderInfo, orderProductInfo.PayCredits, orderProductInfo.ExtStr1, completeTime);
                    if (orderProductInfo.CouponTypeId > 0)
                        Coupons.SendSinglePromotionCoupon(partUserInfo, orderProductInfo.CouponTypeId, orderInfo, ip);
                }
            }
        }

        /// <summary>
        /// 退货
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="returnTime">退货时间</param>
        public static void ReturnOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime returnTime)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Returned);//将订单状态设为退货状态

            if (orderInfo.OrderState == (int)OrderState.Sended)//用户收货时退货
            {
                if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                    Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

                if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                    Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, returnTime);

                if (orderInfo.PaySN.Length > 0)//退回用户支付的金钱(此操作只是将退款记录保存到表'orderrefunds'中，实际退款还需要再次操作)
                    OrderRefunds.ApplyRefund(new OrderRefundInfo
                    {
                        Oid = orderInfo.Oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = returnTime,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        PaySN = orderInfo.PaySN,
                        PaySystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName
                    });

            }
            else if (orderInfo.OrderState == (int)OrderState.Completed)//订单完成后退货
            {
                if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                    Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

                if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                    Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, returnTime);

                //应退金钱
                decimal returnMoney = orderInfo.SurplusMoney;

                //订单发放的积分
                DataTable sendCredits = Credits.GetUserOrderSendCredits(orderInfo.Oid);
                int payCreditAmount = TypeHelper.ObjectToInt(sendCredits.Rows[0]["paycreditamount"]);
                int rankCreditAmount = TypeHelper.ObjectToInt(sendCredits.Rows[0]["rankcreditamount"]);
                //判断用户当前积分是否足够退回，如果不足够就将差额核算成金钱并在应退金钱中减去
                if (partUserInfo.PayCredits < payCreditAmount)
                {
                    returnMoney = returnMoney - Credits.PayCreditsToMoney(payCreditAmount - partUserInfo.PayCredits);
                    payCreditAmount = partUserInfo.PayCredits;
                }
                //收回订单发放的积分
                Credits.ReturnUserOrderSendCredits(ref partUserInfo, orderInfo, payCreditAmount, rankCreditAmount, operatorId, returnTime);

                StringBuilder couponIdList = new StringBuilder();
                //订单发放的优惠劵列表
                List<CouponInfo> couponList = Coupons.GetUserOrderSendCouponList(orderInfo.Oid);
                //判断优惠劵是否已经被使用，如果已经使用就在应退金钱中减去优惠劵金额
                foreach (CouponInfo couponInfo in couponList)
                {
                    if (couponInfo.Oid > 0)
                        returnMoney = returnMoney - couponInfo.Money;
                    else
                        couponIdList.AppendFormat("{0},", couponInfo.CouponId);
                }
                //收回订单发放的优惠劵
                if (couponIdList.Length > 0)
                {
                    Coupons.DeleteCouponById(couponIdList.Remove(couponIdList.Length - 1, 1).ToString());
                }

                if (returnMoney > 0)//退回用户支付的金钱(此操作只是将退款记录保存到表'orderrefunds'中，实际退款还需要再次操作)
                    OrderRefunds.ApplyRefund(new OrderRefundInfo
                    {
                        Oid = orderInfo.Oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = returnTime,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = returnMoney,
                        PaySN = orderInfo.PaySN,
                        PaySystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName
                    });
            }

            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
        }

        /// <summary>
        /// 锁定订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void LockOrder(OrderInfo orderInfo)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Locked);
            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="cancelTime">取消时间</param>
        public static void CancelOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime cancelTime)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Cancelled);//将订单状态设为取消状态

            if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

            if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, cancelTime);

            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
        }

        #endregion

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public static void UpdateOrderState(int oid, OrderState orderState)
        {
            BrnMall.Data.Orders.UpdateOrderState(oid, orderState);
        }

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
        public static DataTable GetUserOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime, int orderState)
        {
            return BrnMall.Data.Orders.GetUserOrderList(uid, pageSize, pageNumber, startAddTime, endAddTime, orderState);
        }

        /// <summary>
        /// 获得用户订单数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserOrderCount(int uid, string startAddTime, string endAddTime, int orderState)
        {
            return BrnMall.Data.Orders.GetUserOrderCount(uid, startAddTime, endAddTime, orderState);
        }

        /// <summary>
        /// 是否评价了所有订单商品
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static bool IsReviewAllOrderProduct(List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.IsReview == 0)
                    return false;
            }
            return true;
        }




        /// <summary>
        /// 创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProduct(PartProductInfo partProuctInfo)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.Pid = partProuctInfo.Pid;
            orderProductInfo.PSN = partProuctInfo.PSN;
            orderProductInfo.CateId = partProuctInfo.CateId;
            orderProductInfo.BrandId = partProuctInfo.BrandId;
            orderProductInfo.StoreId = partProuctInfo.StoreId;
            orderProductInfo.StoreCid = partProuctInfo.StoreCid;
            orderProductInfo.StoreSTid = partProuctInfo.StoreSTid;
            orderProductInfo.Name = partProuctInfo.Name;
            orderProductInfo.DiscountPrice = partProuctInfo.ShopPrice;
            orderProductInfo.ShopPrice = partProuctInfo.ShopPrice;
            orderProductInfo.MarketPrice = partProuctInfo.MarketPrice;
            orderProductInfo.CostPrice = partProuctInfo.CostPrice;
            orderProductInfo.Weight = partProuctInfo.Weight;
            orderProductInfo.ShowImg = partProuctInfo.ShowImg;
            return orderProductInfo;
        }

        /// <summary>
        /// 创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProduct(DataRow row)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.Pid = TypeHelper.ObjectToInt(row["pid"]);
            orderProductInfo.PSN = row["psn"].ToString();
            orderProductInfo.CateId = TypeHelper.ObjectToInt(row["cateid"]);
            orderProductInfo.BrandId = TypeHelper.ObjectToInt(row["brandid"]);
            orderProductInfo.StoreId = TypeHelper.ObjectToInt(row["storeid"]);
            orderProductInfo.StoreCid = TypeHelper.ObjectToInt(row["storecid"]);
            orderProductInfo.StoreSTid = TypeHelper.ObjectToInt(row["storestid"]);
            orderProductInfo.Name = row["name"].ToString();
            orderProductInfo.DiscountPrice = TypeHelper.ObjectToDecimal(row["shopprice"]);
            orderProductInfo.ShopPrice = TypeHelper.ObjectToDecimal(row["shopprice"]);
            orderProductInfo.MarketPrice = TypeHelper.ObjectToDecimal(row["marketprice"]);
            orderProductInfo.CostPrice = TypeHelper.ObjectToDecimal(row["costprice"]);
            orderProductInfo.Weight = TypeHelper.ObjectToInt(row["weight"]);
            orderProductInfo.ShowImg = row["showimg"].ToString();
            return orderProductInfo;
        }

        /// <summary>
        /// 初始化订单商品
        /// </summary>
        private static void InitOrderProduct(OrderProductInfo orderProductInfo, int buyCount, string sid, int uid, DateTime buyTime, string ip)
        {
            if (uid > 0)
                orderProductInfo.Sid = "";
            else
                orderProductInfo.Sid = sid;
            orderProductInfo.Uid = uid;
            orderProductInfo.RealCount = buyCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.AddTime = buyTime;
            orderProductInfo.IP = ip;
        }
    }
}
