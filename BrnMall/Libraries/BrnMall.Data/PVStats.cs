﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// PV统计数据访问类
    /// </summary>
    public class PVStats
    {
        #region 私有方法

        /// <summary>
        /// 从IDataReader创建PVStatInfo
        /// </summary>
        private static PVStatInfo BuildPVStatFromReader(IDataReader reader)
        {
            PVStatInfo pvStatInfo = new PVStatInfo();

            pvStatInfo.Category = reader["category"].ToString();
            pvStatInfo.Value = reader["value"].ToString();
            pvStatInfo.Count = TypeHelper.ObjectToInt(reader["count"]);

            return pvStatInfo;
        }

        #endregion

        /// <summary>
        /// 更新pv统计
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="value">值</param>
        /// <param name="count">数量</param>
        public static void UpdatePVStat(string category, string value, int count)
        {
            BrnMall.Core.BMAData.RDBS.UpdatePVStat(category, value, count);
        }

        /// <summary>
        /// 获得省级区域统计
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProvinceRegionStat()
        {
            return BrnMall.Core.BMAData.RDBS.GetProvinceRegionStat();
        }

        /// <summary>
        /// 获得市级区域统计
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <returns></returns>
        public static DataTable GetCityRegionStat(int provinceId)
        {
            return BrnMall.Core.BMAData.RDBS.GetCityRegionStat(provinceId);
        }

        /// <summary>
        /// 获得区/县级区域统计
        /// </summary>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static DataTable GetCountyRegionStat(int cityId)
        {
            return BrnMall.Core.BMAData.RDBS.GetCountyRegionStat(cityId);
        }

        /// <summary>
        /// 获得PV统计列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static List<PVStatInfo> GetPVStatList(string condition)
        {
            List<PVStatInfo> pvStatList = new List<PVStatInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetPVStatList(condition);
            while (reader.Read())
            {
                PVStatInfo pvStatInfo = BuildPVStatFromReader(reader);
                pvStatList.Add(pvStatInfo);
            }

            reader.Close();
            return pvStatList;
        }

        /// <summary>
        /// 获得PV统计
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static PVStatInfo GetPVStatByCategoryAndValue(string category, string value)
        {
            PVStatInfo pvStatInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetPVStatByCategoryAndValue(category, value);
            if (reader.Read())
            {
                pvStatInfo = BuildPVStatFromReader(reader);
            }

            reader.Close();
            return pvStatInfo;
        }
    }
}
