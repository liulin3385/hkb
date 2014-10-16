using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 促销活动数据访问类
    /// </summary>
    public class Promotions
    {
        #region 私有方法

        /// <summary>
        /// 从IDataReader创建SinglePromotionInfo
        /// </summary>
        private static SinglePromotionInfo BuildSinglePromotionFromReader(IDataReader reader)
        {
            SinglePromotionInfo singlePromotionInfo = new SinglePromotionInfo();
            singlePromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            singlePromotionInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
            singlePromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            singlePromotionInfo.StartTime1 = TypeHelper.ObjectToDateTime(reader["starttime1"]);
            singlePromotionInfo.EndTime1 = TypeHelper.ObjectToDateTime(reader["endtime1"]);
            singlePromotionInfo.StartTime2 = TypeHelper.ObjectToDateTime(reader["starttime2"]);
            singlePromotionInfo.EndTime2 = TypeHelper.ObjectToDateTime(reader["endtime2"]);
            singlePromotionInfo.StartTime3 = TypeHelper.ObjectToDateTime(reader["starttime3"]);
            singlePromotionInfo.EndTime3 = TypeHelper.ObjectToDateTime(reader["endtime3"]);
            singlePromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            singlePromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            singlePromotionInfo.Name = reader["name"].ToString();
            singlePromotionInfo.Slogan = reader["slogan"].ToString();
            singlePromotionInfo.DiscountType = TypeHelper.ObjectToInt(reader["discounttype"]);
            singlePromotionInfo.DiscountValue = TypeHelper.ObjectToInt(reader["discountvalue"]);
            singlePromotionInfo.CouponTypeId = TypeHelper.ObjectToInt(reader["coupontypeid"]);
            singlePromotionInfo.PayCredits = TypeHelper.ObjectToInt(reader["paycredits"]);
            singlePromotionInfo.IsStock = TypeHelper.ObjectToInt(reader["isstock"]);
            singlePromotionInfo.Stock = TypeHelper.ObjectToInt(reader["stock"]);
            singlePromotionInfo.QuotaLower = TypeHelper.ObjectToInt(reader["quotalower"]);
            singlePromotionInfo.QuotaUpper = TypeHelper.ObjectToInt(reader["quotaupper"]);
            singlePromotionInfo.AllowBuyCount = TypeHelper.ObjectToInt(reader["allowbuycount"]);
            return singlePromotionInfo;
        }

        /// <summary>
        /// 从IDataReader创建BuySendPromotionInfo
        /// </summary>
        private static BuySendPromotionInfo BuildBuySendPromotionFromReader(IDataReader reader)
        {
            BuySendPromotionInfo buySendPromotionInfo = new BuySendPromotionInfo();
            buySendPromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            buySendPromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            buySendPromotionInfo.StartTime = TypeHelper.ObjectToDateTime(reader["starttime"]);
            buySendPromotionInfo.EndTime = TypeHelper.ObjectToDateTime(reader["endtime"]);
            buySendPromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            buySendPromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            buySendPromotionInfo.Name = reader["name"].ToString();
            buySendPromotionInfo.Type = TypeHelper.ObjectToInt(reader["type"]);
            buySendPromotionInfo.BuyCount = TypeHelper.ObjectToInt(reader["buycount"]);
            buySendPromotionInfo.SendCount = TypeHelper.ObjectToInt(reader["sendcount"]);
            return buySendPromotionInfo;
        }

        /// <summary>
        /// 从IDataReader创建GiftPromotionInfo
        /// </summary>
        private static GiftPromotionInfo BuildGiftPromotionFromReader(IDataReader reader)
        {
            GiftPromotionInfo giftPromotionInfo = new GiftPromotionInfo();
            giftPromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            giftPromotionInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
            giftPromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            giftPromotionInfo.StartTime1 = TypeHelper.ObjectToDateTime(reader["starttime1"]);
            giftPromotionInfo.EndTime1 = TypeHelper.ObjectToDateTime(reader["endtime1"]);
            giftPromotionInfo.StartTime2 = TypeHelper.ObjectToDateTime(reader["starttime2"]);
            giftPromotionInfo.EndTime2 = TypeHelper.ObjectToDateTime(reader["endtime2"]);
            giftPromotionInfo.StartTime3 = TypeHelper.ObjectToDateTime(reader["starttime3"]);
            giftPromotionInfo.EndTime3 = TypeHelper.ObjectToDateTime(reader["endtime3"]);
            giftPromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            giftPromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            giftPromotionInfo.Name = reader["name"].ToString();
            giftPromotionInfo.QuotaUpper = TypeHelper.ObjectToInt(reader["quotaupper"]);
            return giftPromotionInfo;
        }

        /// <summary>
        /// 从IDataReader创建SuitPromotionInfo
        /// </summary>
        private static SuitPromotionInfo BuildSuitPromotionFromReader(IDataReader reader)
        {
            SuitPromotionInfo suitPromotionInfo = new SuitPromotionInfo();
            suitPromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            suitPromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            suitPromotionInfo.StartTime1 = TypeHelper.ObjectToDateTime(reader["starttime1"]);
            suitPromotionInfo.EndTime1 = TypeHelper.ObjectToDateTime(reader["endtime1"]);
            suitPromotionInfo.StartTime2 = TypeHelper.ObjectToDateTime(reader["starttime2"]);
            suitPromotionInfo.EndTime2 = TypeHelper.ObjectToDateTime(reader["endtime2"]);
            suitPromotionInfo.StartTime3 = TypeHelper.ObjectToDateTime(reader["starttime3"]);
            suitPromotionInfo.EndTime3 = TypeHelper.ObjectToDateTime(reader["endtime3"]);
            suitPromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            suitPromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            suitPromotionInfo.Name = reader["name"].ToString();
            suitPromotionInfo.QuotaUpper = TypeHelper.ObjectToInt(reader["quotaupper"]);
            suitPromotionInfo.OnlyOnce = TypeHelper.ObjectToInt(reader["onlyonce"]);
            return suitPromotionInfo;
        }

        /// <summary>
        /// 从IDataReader创建FullSendPromotionInfo
        /// </summary>
        private static FullSendPromotionInfo BuildFullSendPromotionFromReader(IDataReader reader)
        {
            FullSendPromotionInfo fullSendPromotionInfo = new FullSendPromotionInfo();
            fullSendPromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            fullSendPromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            fullSendPromotionInfo.StartTime = TypeHelper.ObjectToDateTime(reader["starttime"]);
            fullSendPromotionInfo.EndTime = TypeHelper.ObjectToDateTime(reader["endtime"]);
            fullSendPromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            fullSendPromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            fullSendPromotionInfo.Name = reader["name"].ToString();
            fullSendPromotionInfo.LimitMoney = TypeHelper.ObjectToInt(reader["limitmoney"]);
            fullSendPromotionInfo.AddMoney = TypeHelper.ObjectToInt(reader["addmoney"]);
            return fullSendPromotionInfo;
        }

        /// <summary>
        /// 从IDataReader创建FullCutPromotionInfo
        /// </summary>
        private static FullCutPromotionInfo BuildFullCutPromotionFromReader(IDataReader reader)
        {
            FullCutPromotionInfo fullCutPromotionInfo = new FullCutPromotionInfo();
            fullCutPromotionInfo.PmId = TypeHelper.ObjectToInt(reader["pmid"]);
            fullCutPromotionInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            fullCutPromotionInfo.Type = TypeHelper.ObjectToInt(reader["type"]);
            fullCutPromotionInfo.StartTime = TypeHelper.ObjectToDateTime(reader["starttime"]);
            fullCutPromotionInfo.EndTime = TypeHelper.ObjectToDateTime(reader["endtime"]);
            fullCutPromotionInfo.UserRankLower = TypeHelper.ObjectToInt(reader["userranklower"]);
            fullCutPromotionInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            fullCutPromotionInfo.Name = reader["name"].ToString();
            fullCutPromotionInfo.LimitMoney1 = TypeHelper.ObjectToInt(reader["limitmoney1"]);
            fullCutPromotionInfo.CutMoney1 = TypeHelper.ObjectToInt(reader["cutmoney1"]);
            fullCutPromotionInfo.LimitMoney2 = TypeHelper.ObjectToInt(reader["limitmoney2"]);
            fullCutPromotionInfo.CutMoney2 = TypeHelper.ObjectToInt(reader["cutmoney2"]);
            fullCutPromotionInfo.LimitMoney3 = TypeHelper.ObjectToInt(reader["limitmoney3"]);
            fullCutPromotionInfo.CutMoney3 = TypeHelper.ObjectToInt(reader["cutmoney3"]);
            return fullCutPromotionInfo;
        }

        #endregion

        /// <summary>
        /// 后台获得单品促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static SinglePromotionInfo AdminGetSinglePromotionById(int pmId)
        {
            SinglePromotionInfo singlePromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetSinglePromotionById(pmId);
            if (reader.Read())
            {
                singlePromotionInfo = BuildSinglePromotionFromReader(reader);
            }

            reader.Close();
            return singlePromotionInfo;
        }

        /// <summary>
        /// 创建单品促销活动
        /// </summary>
        public static void CreateSinglePromotion(SinglePromotionInfo singlePromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateSinglePromotion(singlePromotionInfo);
        }

        /// <summary>
        /// 更新单品促销活动
        /// </summary>
        public static void UpdateSinglePromotion(SinglePromotionInfo singlePromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateSinglePromotion(singlePromotionInfo);
        }

        /// <summary>
        /// 删除单品促销活动
        /// </summary>
        /// <param name="pmIdList">促销活动id</param>
        public static void DeleteSinglePromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteSinglePromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得单品促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetSinglePromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSinglePromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得单品促销活动列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetSinglePromotionListCondition(int storeId, int pid, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSinglePromotionListCondition(storeId, pid, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得单品促销活动排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetSinglePromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSinglePromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得单品促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetSinglePromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSinglePromotionCount(condition);
        }

        /// <summary>
        /// 后台判断商品在指定时间段内是否已经存在单品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int AdminIsExistSinglePromotion(int pid, DateTime startTime, DateTime endTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminIsExistSinglePromotion(pid, startTime, endTime);
        }

        /// <summary>
        /// 获得单品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SinglePromotionInfo GetSinglePromotionByPidAndTime(int pid, DateTime nowTime)
        {
            SinglePromotionInfo singlePromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetSinglePromotionByPidAndTime(pid, nowTime);
            if (reader.Read())
            {
                singlePromotionInfo = BuildSinglePromotionFromReader(reader);
            }
            reader.Close();

            return singlePromotionInfo;
        }

        /// <summary>
        /// 获得单品促销活动商品的购买数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static int GetSinglePromotionProductBuyCount(int uid, int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.GetSinglePromotionProductBuyCount(uid, pmId);
        }

        /// <summary>
        /// 更新单品促销活动的库存
        /// </summary>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        public static void UpdateSinglePromotionStock(List<SinglePromotionInfo> singlePromotionList)
        {
            BrnMall.Core.BMAData.RDBS.UpdateSinglePromotionStock(singlePromotionList);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">单品促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListBySinglePmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListBySinglePmId(pmIdList);
        }






        /// <summary>
        /// 后台获得买送促销活动
        /// </summary>
        /// <param name="pmId">获得id</param>
        /// <returns></returns>
        public static BuySendPromotionInfo AdminGetBuySendPromotionById(int pmId)
        {
            BuySendPromotionInfo buySendPromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetBuySendPromotionById(pmId);
            if (reader.Read())
            {
                buySendPromotionInfo = BuildBuySendPromotionFromReader(reader);
            }

            reader.Close();
            return buySendPromotionInfo;
        }

        /// <summary>
        /// 创建买送促销活动
        /// </summary>
        public static void CreateBuySendPromotion(BuySendPromotionInfo buySendPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateBuySendPromotion(buySendPromotionInfo);
        }

        /// <summary>
        /// 更新买送促销活动
        /// </summary>
        public static void UpdateBuySendPromotion(BuySendPromotionInfo buySendPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateBuySendPromotion(buySendPromotionInfo);
        }

        /// <summary>
        /// 删除买送促销活动
        /// </summary>
        /// <param name="pmIdList">活动id</param>
        public static void DeleteBuySendPromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteBuySendPromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得买送促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetBuySendPromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendPromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得买送促销活动列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetBuySendPromotionListCondition(int storeId, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendPromotionListCondition(storeId, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得买送促销活动排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetBuySendPromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendPromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得买送促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetBuySendPromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendPromotionCount(condition);
        }

        /// <summary>
        /// 获得买送促销活动列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<BuySendPromotionInfo> GetBuySendPromotionList(int storeId, int pid, DateTime nowTime)
        {
            List<BuySendPromotionInfo> buySendPromotionList = new List<BuySendPromotionInfo>();

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetBuySendPromotionList(storeId, pid, nowTime);
            while (reader.Read())
            {
                BuySendPromotionInfo buySendPromotionInfo = BuildBuySendPromotionFromReader(reader);
                buySendPromotionList.Add(buySendPromotionInfo);
            }
            reader.Close();

            return buySendPromotionList;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">买送促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByBuySendPmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByBuySendPmId(pmIdList);
        }





        /// <summary>
        /// 添加买送商品
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        public static void AddBuySendProduct(int pmId, int pid)
        {
            BrnMall.Core.BMAData.RDBS.AddBuySendProduct(pmId, pid);
        }

        /// <summary>
        /// 删除买送商品
        /// </summary>
        /// <param name="recordIdList">记录id列表</param>
        /// <returns></returns>
        public static bool DeleteBuySendProductByRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.DeleteBuySendProductByRecordId(recordIdList);
        }

        /// <summary>
        /// 买送商品是否已经存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistBuySendProduct(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistBuySendProduct(pmId, pid);
        }

        /// <summary>
        /// 后台获得买送商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static DataTable AdminGetBuySendProductList(int pageSize, int pageNumber, int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendProductList(pageSize, pageNumber, pmId, pid);
        }

        /// <summary>
        /// 后台获得买送商品数量
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static int AdminGetBuySendProductCount(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBuySendProductCount(pmId, pid);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="recordIdList">买送商品记录id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByBuySendProductRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByBuySendProductRecordId(recordIdList);
        }







        /// <summary>
        /// 后台获得赠品促销活动
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public static GiftPromotionInfo AdminGetGiftPromotionById(int pmId)
        {
            GiftPromotionInfo giftPromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetGiftPromotionById(pmId);
            if (reader.Read())
            {
                giftPromotionInfo = BuildGiftPromotionFromReader(reader);
            }

            reader.Close();
            return giftPromotionInfo;
        }

        /// <summary>
        /// 创建赠品促销活动
        /// </summary>
        public static void CreateGiftPromotion(GiftPromotionInfo giftPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateGiftPromotion(giftPromotionInfo);
        }

        /// <summary>
        /// 更新赠品促销活动
        /// </summary>
        public static void UpdateGiftPromotion(GiftPromotionInfo giftPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateGiftPromotion(giftPromotionInfo);
        }

        /// <summary>
        /// 删除赠品促销活动
        /// </summary>
        /// <param name="pmIdList">活动id</param>
        public static void DeleteGiftPromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteGiftPromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得赠品促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetGiftPromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetGiftPromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得赠品促销活动列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetGiftPromotionListCondition(int storeId, int pid, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetGiftPromotionListCondition(storeId, pid, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得赠品促销活动列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetGiftPromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetGiftPromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得赠品促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetGiftPromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetGiftPromotionCount(condition);
        }

        /// <summary>
        /// 后台判断商品在指定时间段内是否已经存在赠品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int AdminIsExistGiftPromotion(int pid, DateTime startTime, DateTime endTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminIsExistGiftPromotion(pid, startTime, endTime);
        }

        /// <summary>
        /// 获得赠品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static GiftPromotionInfo GetGiftPromotionByPidAndTime(int pid, DateTime nowTime)
        {
            GiftPromotionInfo giftPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetGiftPromotionByPidAndTime(pid, nowTime);
            if (reader.Read())
            {
                giftPromotionInfo = BuildGiftPromotionFromReader(reader);
            }
            reader.Close();

            return giftPromotionInfo;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">赠品促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByGiftPmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByGiftPmId(pmIdList);
        }




        /// <summary>
        /// 添加赠品
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="giftId">赠品id</param>
        /// <param name="number">数量</param>
        public static void AddGift(int pmId, int giftId, int number)
        {
            BrnMall.Core.BMAData.RDBS.AddGift(pmId, giftId, number);
        }

        /// <summary>
        /// 删除赠品
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="giftId">赠品id</param>
        /// <returns></returns>
        public static bool DeleteGiftByPmIdAndGiftId(int pmId, int giftId)
        {
            return BrnMall.Core.BMAData.RDBS.DeleteGiftByPmIdAndGiftId(pmId, giftId);
        }

        /// <summary>
        /// 赠品是否已经存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="giftId">赠品id</param>
        public static bool IsExistGift(int pmId, int giftId)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistGift(pmId, giftId);
        }

        /// <summary>
        /// 更新赠品的数量
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="giftId">赠品id</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public static bool UpdateGiftNumber(int pmId, int giftId, int number)
        {
            return BrnMall.Core.BMAData.RDBS.UpdateGiftNumber(pmId, giftId, number);
        }

        /// <summary>
        /// 后台获得赠品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static DataTable AdminGetGiftListByPmId(int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetGiftListByPmId(pmId);
        }

        /// <summary>
        /// 获得赠品列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static DataTable GetGiftList(int pid, DateTime nowTime)
        {
            return BrnMall.Core.BMAData.RDBS.GetGiftList(pid, nowTime);
        }







        /// <summary>
        /// 后台获得套装促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static SuitPromotionInfo AdminGetSuitPromotionById(int pmId)
        {
            SuitPromotionInfo suitPromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetSuitPromotionById(pmId);
            if (reader.Read())
            {
                suitPromotionInfo = BuildSuitPromotionFromReader(reader);
            }

            reader.Close();
            return suitPromotionInfo;
        }

        /// <summary>
        /// 创建套装促销活动
        /// </summary>
        public static int CreateSuitPromotion(SuitPromotionInfo suitPromotionInfo)
        {
            return BrnMall.Core.BMAData.RDBS.CreateSuitPromotion(suitPromotionInfo);
        }

        /// <summary>
        /// 更新套装促销活动
        /// </summary>
        public static void UpdateSuitPromotion(SuitPromotionInfo suitPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateSuitPromotion(suitPromotionInfo);
        }

        /// <summary>
        /// 删除套装促销活动
        /// </summary>
        /// <param name="pmIdList">活动id</param>
        public static void DeleteSuitPromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteSuitPromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得套装促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetSuitPromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSuitPromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得套装促销活动列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetSuitPromotionListCondition(int storeId, int pid, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSuitPromotionListCondition(storeId, pid, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得套装促销活动列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetSuitPromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSuitPromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得套装促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetSuitPromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetSuitPromotionCount(condition);
        }

        /// <summary>
        /// 获得套装促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SuitPromotionInfo GetSuitPromotionByPmIdAndTime(int pmId, DateTime nowTime)
        {
            SuitPromotionInfo suitPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetSuitPromotionByPmIdAndTime(pmId, nowTime);
            if (reader.Read())
            {
                suitPromotionInfo = BuildSuitPromotionFromReader(reader);
            }
            reader.Close();

            return suitPromotionInfo;
        }

        /// <summary>
        /// 判断用户是否参加过指定套装促销活动
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static bool IsJoinSuitPromotion(int uid, int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.IsJoinSuitPromotion(uid, pmId);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">套装促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListBySuitPmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListBySuitPmId(pmIdList);
        }





        /// <summary>
        /// 添加套装商品
        /// </summary>
        public static void AddSuitProduct(int pmId, int pid, int discount, int number)
        {
            BrnMall.Core.BMAData.RDBS.AddSuitProduct(pmId, pid, discount, number);
        }

        /// <summary>
        /// 删除套装商品
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        /// <returns></returns>
        public static bool DeleteSuitProductByPmIdAndPid(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.DeleteSuitProductByPmIdAndPid(pmId, pid);
        }

        /// <summary>
        /// 套装商品是否存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistSuitProduct(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistSuitProduct(pmId, pid);
        }

        /// <summary>
        /// 修改套装商品数量
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public static bool UpdateSuitProductNumber(int pmId, int pid, int number)
        {
            return BrnMall.Core.BMAData.RDBS.UpdateSuitProductNumber(pmId, pid, number);
        }

        /// <summary>
        /// 修改套装商品折扣值
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        /// <param name="discount">折扣值</param>
        /// <returns></returns>
        public static bool UpdateSuitProductDiscountValue(int pmId, int pid, int discount)
        {
            return BrnMall.Core.BMAData.RDBS.UpdateSuitProductDiscountValue(pmId, pid, discount);
        }

        /// <summary>
        /// 获得套装商品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static DataTable GetSuitProductList(int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.GetSuitProductList(pmId);
        }

        /// <summary>
        /// 获得套装商品列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static DataTable GetSuitProductList(int storeId, int pid, DateTime nowTime)
        {
            return BrnMall.Core.BMAData.RDBS.GetSuitProductList(storeId, pid, nowTime);
        }






        /// <summary>
        /// 后台获得满赠促销活动
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public static FullSendPromotionInfo AdminGetFullSendPromotionById(int pmId)
        {
            FullSendPromotionInfo fullSendPromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetFullSendPromotionById(pmId);
            if (reader.Read())
            {
                fullSendPromotionInfo = BuildFullSendPromotionFromReader(reader);
            }

            reader.Close();
            return fullSendPromotionInfo;
        }

        /// <summary>
        /// 创建满赠促销活动
        /// </summary>
        public static void CreateFullSendPromotion(FullSendPromotionInfo fullSendPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateFullSendPromotion(fullSendPromotionInfo);
        }

        /// <summary>
        /// 更新满赠促销活动
        /// </summary>
        public static void UpdateFullSendPromotion(FullSendPromotionInfo fullSendPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateFullSendPromotion(fullSendPromotionInfo);
        }

        /// <summary>
        /// 删除满赠促销活动
        /// </summary>
        /// <param name="pmIdList">活动id</param>
        public static void DeleteFullSendPromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteFullSendPromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得满赠促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetFullSendPromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendPromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得满赠促销活动列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetFullSendPromotionListCondition(int storeId, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendPromotionListCondition(storeId, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得满赠促销活动排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetFullSendPromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendPromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得满赠促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetFullSendPromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendPromotionCount(condition);
        }

        /// <summary>
        /// 获得满赠促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullSendPromotionInfo GetFullSendPromotionByStoreIdAndPidAndTime(int storeId, int pid, DateTime nowTime)
        {
            FullSendPromotionInfo fullSendPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetFullSendPromotionByStoreIdAndPidAndTime(storeId, pid, nowTime);
            if (reader.Read())
            {
                fullSendPromotionInfo = BuildFullSendPromotionFromReader(reader);
            }
            reader.Close();

            return fullSendPromotionInfo;
        }

        /// <summary>
        /// 获得满赠促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullSendPromotionInfo GetFullSendPromotionByPmIdAndTime(int pmId, DateTime nowTime)
        {
            FullSendPromotionInfo fullSendPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetFullSendPromotionByPmIdAndTime(pmId, nowTime);
            if (reader.Read())
            {
                fullSendPromotionInfo = BuildFullSendPromotionFromReader(reader);
            }
            reader.Close();

            return fullSendPromotionInfo;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">满赠促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByFullSendPmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByFullSendPmId(pmIdList);
        }





        /// <summary>
        /// 添加满赠商品
        /// </summary>
        public static void AddFullSendProduct(int pmId, int pid, int type)
        {
            BrnMall.Core.BMAData.RDBS.AddFullSendProduct(pmId, pid, type);
        }

        /// <summary>
        /// 删除满赠商品
        /// </summary>
        /// <param name="recordIdList">记录id</param>
        /// <returns></returns>
        public static bool DeleteFullSendProductByRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.DeleteFullSendProductByRecordId(recordIdList);
        }

        /// <summary>
        /// 满赠商品是否存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistFullSendProduct(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistFullSendProduct(pmId, pid);
        }

        /// <summary>
        /// 后台获得满赠商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pmId">活动id</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static DataTable AdminGetFullSendProductList(int pageSize, int pageNumber, int pmId, int type)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendProductList(pageSize, pageNumber, pmId, type);
        }

        /// <summary>
        /// 后台获得满赠商品数量
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static int AdminGetFullSendProductCount(int pmId, int type)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullSendProductCount(pmId, type);
        }

        /// <summary>
        /// 判断满赠商品是否存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsExistFullSendProduct(int pmId, int pid, int type)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistFullSendProduct(pmId, pid, type);
        }

        /// <summary>
        /// 获得满赠赠品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetFullSendPresentList(int pmId)
        {
            List<PartProductInfo> fullSendPresentList = new List<PartProductInfo>();

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetFullSendPresentList(pmId);
            while (reader.Read())
            {
                PartProductInfo partProductInfo = new PartProductInfo();

                partProductInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
                partProductInfo.PSN = reader["psn"].ToString();
                partProductInfo.CateId = TypeHelper.ObjectToInt(reader["cateid"]);
                partProductInfo.BrandId = TypeHelper.ObjectToInt(reader["brandid"]);
                partProductInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
                partProductInfo.StoreCid = TypeHelper.ObjectToInt(reader["storecid"]);
                partProductInfo.StoreSTid = TypeHelper.ObjectToInt(reader["storestid"]);
                partProductInfo.SKUGid = TypeHelper.ObjectToInt(reader["skugid"]);
                partProductInfo.Name = reader["name"].ToString();
                partProductInfo.ShopPrice = TypeHelper.ObjectToDecimal(reader["shopprice"]);
                partProductInfo.MarketPrice = TypeHelper.ObjectToDecimal(reader["marketprice"]);
                partProductInfo.CostPrice = TypeHelper.ObjectToDecimal(reader["costprice"]);
                partProductInfo.State = TypeHelper.ObjectToInt(reader["state"]);
                partProductInfo.IsBest = TypeHelper.ObjectToInt(reader["isbest"]);
                partProductInfo.IsHot = TypeHelper.ObjectToInt(reader["ishot"]);
                partProductInfo.IsNew = TypeHelper.ObjectToInt(reader["isnew"]);
                partProductInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);
                partProductInfo.Weight = TypeHelper.ObjectToInt(reader["weight"]);
                partProductInfo.ShowImg = reader["showimg"].ToString();
                partProductInfo.SaleCount = TypeHelper.ObjectToInt(reader["salecount"]);
                partProductInfo.VisitCount = TypeHelper.ObjectToInt(reader["visitcount"]);
                partProductInfo.ReviewCount = TypeHelper.ObjectToInt(reader["reviewcount"]);
                partProductInfo.Star1 = TypeHelper.ObjectToInt(reader["star1"]);
                partProductInfo.Star2 = TypeHelper.ObjectToInt(reader["star2"]);
                partProductInfo.Star3 = TypeHelper.ObjectToInt(reader["star3"]);
                partProductInfo.Star4 = TypeHelper.ObjectToInt(reader["star4"]);
                partProductInfo.Star5 = TypeHelper.ObjectToInt(reader["star5"]);
                partProductInfo.AddTime = TypeHelper.ObjectToDateTime(reader["addtime"]);

                fullSendPresentList.Add(partProductInfo);
            }
            reader.Close();

            return fullSendPresentList;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="recordIdList">满赠商品记录id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByFullSendProductRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByFullSendProductRecordId(recordIdList);
        }





        /// <summary>
        /// 后台获得满减促销活动
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public static FullCutPromotionInfo AdminGetFullCutPromotionById(int pmId)
        {
            FullCutPromotionInfo fullCutPromotionInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetFullCutPromotionById(pmId);
            if (reader.Read())
            {
                fullCutPromotionInfo = BuildFullCutPromotionFromReader(reader);
            }

            reader.Close();
            return fullCutPromotionInfo;
        }

        /// <summary>
        /// 创建满减促销活动
        /// </summary>
        public static void CreateFullCutPromotion(FullCutPromotionInfo fullCutPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateFullCutPromotion(fullCutPromotionInfo);
        }

        /// <summary>
        /// 更新满减促销活动
        /// </summary>
        public static void UpdateFullCutPromotion(FullCutPromotionInfo fullCutPromotionInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateFullCutPromotion(fullCutPromotionInfo);
        }

        /// <summary>
        /// 删除满减促销活动
        /// </summary>
        /// <param name="pmIdList">活动id</param>
        public static void DeleteFullCutPromotionById(string pmIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteFullCutPromotionById(pmIdList);
        }

        /// <summary>
        /// 后台获得满减促销活动列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetFullCutPromotionList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutPromotionList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得满减促销活动列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="promotionName">活动名称</param>
        /// <param name="promotionTime">活动时间</param>
        /// <returns></returns>
        public static string AdminGetFullCutPromotionListCondition(int storeId, string promotionName, string promotionTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutPromotionListCondition(storeId, promotionName, promotionTime);
        }

        /// <summary>
        /// 后台获得满减促销活动列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetFullCutPromotionListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutPromotionListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得满减促销活动数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetFullCutPromotionCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutPromotionCount(condition);
        }

        /// <summary>
        /// 获得满减促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullCutPromotionInfo GetFullCutPromotionByStoreIdAndPidAndTime(int storeId, int pid, DateTime nowTime)
        {
            FullCutPromotionInfo fullCutPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetFullCutPromotionByStoreIdAndPidAndTime(storeId, pid, nowTime);
            if (reader.Read())
            {
                fullCutPromotionInfo = BuildFullCutPromotionFromReader(reader);
            }
            reader.Close();

            return fullCutPromotionInfo;
        }

        /// <summary>
        /// 获得满减促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullCutPromotionInfo GetFullCutPromotionByPmIdAndTime(int pmId, DateTime nowTime)
        {
            FullCutPromotionInfo fullCutPromotionInfo = null;

            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetFullCutPromotionByPmIdAndTime(pmId, nowTime);
            if (reader.Read())
            {
                fullCutPromotionInfo = BuildFullCutPromotionFromReader(reader);
            }
            reader.Close();

            return fullCutPromotionInfo;
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="pmIdList">满减促销活动id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByFullCutPmId(string pmIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByFullCutPmId(pmIdList);
        }





        /// <summary>
        /// 添加满减商品
        /// </summary>
        public static void AddFullCutProduct(int pmId, int pid)
        {
            BrnMall.Core.BMAData.RDBS.AddFullCutProduct(pmId, pid);
        }

        /// <summary>
        /// 删除满减商品
        /// </summary>
        /// <param name="recordIdList">记录id</param>
        /// <returns></returns>
        public static bool DeleteFullCutProductByRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.DeleteFullCutProductByRecordId(recordIdList);
        }

        /// <summary>
        /// 满减商品是否已经存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistFullCutProduct(int pmId, int pid)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistFullCutProduct(pmId, pid);
        }

        /// <summary>
        /// 后台获得满减商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public static DataTable AdminGetFullCutProductList(int pageSize, int pageNumber, int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutProductList(pageSize, pageNumber, pmId);
        }

        /// <summary>
        /// 后台获得满减商品数量
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public static int AdminGetFullCutProductCount(int pmId)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetFullCutProductCount(pmId);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <param name="recordIdList">满减商品记录id列表</param>
        /// <returns></returns>
        public static DataTable GetStoreIdListByFullCutProductRecordId(string recordIdList)
        {
            return BrnMall.Core.BMAData.RDBS.GetStoreIdListByFullCutProductRecordId(recordIdList);
        }
    }
}
