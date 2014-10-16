using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 浏览历史数据访问类
    /// </summary>
    public class BrowseHistories
    {
        /// <summary>
        /// 获得用户浏览商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetUserBrowseProductList(int pageSize, int pageNumber, int uid)
        {
            List<PartProductInfo> partProductList = new List<PartProductInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetUserBrowseProductList(pageSize, pageNumber, uid);
            while (reader.Read())
            {
                PartProductInfo partProductInfo = new PartProductInfo();

                partProductInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
                partProductInfo.PSN = reader["psn"].ToString();
                partProductInfo.CateId = TypeHelper.ObjectToInt(reader["cateid"]);
                partProductInfo.BrandId = TypeHelper.ObjectToInt(reader["brandid"]);
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

                partProductList.Add(partProductInfo);
            }

            reader.Close();
            return partProductList;
        }

        /// <summary>
        /// 获得用户浏览商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static int GetUserBrowseProductCount(int uid)
        {
            return BrnMall.Core.BMAData.RDBS.GetUserBrowseProductCount(uid);
        }

        /// <summary>
        /// 更新浏览历史
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pid">商品id</param>
        /// <param name="updateTime">更新时间</param>
        public static void UpdateBrowseHistory(int uid, int pid, DateTime updateTime)
        {
            BrnMall.Core.BMAData.RDBS.UpdateBrowseHistory(uid, pid, updateTime);
        }

        /// <summary>
        /// 清空过期浏览历史
        /// </summary>
        public static void ClearExpiredBrowseHistory()
        {
            BrnMall.Core.BMAData.RDBS.ClearExpiredBrowseHistory();
        }
    }
}
