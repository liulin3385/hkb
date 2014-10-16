using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// PV统计操作管理类
    /// </summary>
    public class PVStats
    {
        private static object _locker = new object();//锁对象

        private static int _lastupdatepvstatstime = 0;//最后一次更新pv统计的时间

        private static int _user = 0;//用户访问数
        private static int _member = 0;//会员访问数
        private static int _guest = 0;//游客访问数
        private static int _year = 0;//今年访问数
        private static int _month = 0;//本月访问数
        private static int _day = 0;//今天访问数
        private static int _hour = 0;//当前小时访问数
        private static int _week = 0;//本周访问数
        private static volatile ConcurrentDictionary<string, int> _browser = new ConcurrentDictionary<string, int>();//浏览器统计
        private static volatile ConcurrentDictionary<string, int> _os = new ConcurrentDictionary<string, int>();//操作系统统计
        private static volatile ConcurrentDictionary<int, int> _location = new ConcurrentDictionary<int, int>();//位置统计

        /// <summary>
        /// 更新PV统计
        /// </summary>
        /// <param name="state">UpdatePVStatState类型对象</param>
        public static void UpdatePVStat(object state)
        {
            lock (_locker)
            {
                UpdatePVStatState updatePVStatState = (UpdatePVStatState)state;
                MallConfigInfo shopConfig = BMAConfig.MallConfig;

                ++_user;
                ++_year;
                ++_month;
                ++_day;
                ++_hour;
                ++_week;
                if (updatePVStatState.IsMember)
                    ++_member;
                else
                    ++_guest;

                //更新浏览器统计
                if (shopConfig.IsStatBrowser == 1)
                {
                    if (_browser.ContainsKey(updatePVStatState.Browser))
                        _browser[updatePVStatState.Browser]++;
                    else
                        _browser.TryAdd(updatePVStatState.Browser, 1);
                }

                //更新操作系统统计
                if (shopConfig.IsStatOS == 1)
                {
                    if (_os.ContainsKey(updatePVStatState.OS))
                        _os[updatePVStatState.OS]++;
                    else
                        _os.TryAdd(updatePVStatState.OS, 1);
                }

                //更新位置统计
                if (shopConfig.IsStatRegion == 1)
                {
                    if (_location.ContainsKey(updatePVStatState.RegionId))
                        _location[updatePVStatState.RegionId]++;
                    else
                        _location.TryAdd(updatePVStatState.RegionId, 1);
                }

                if (_lastupdatepvstatstime < (Environment.TickCount - BMAConfig.MallConfig.UpdatePVStatTimespan * 1000 * 60))
                {
                    UpdateWeekPVStat(_week);
                    UpdateHourPVStat(_hour);
                    UpdateDayPVStat(_day);
                    UpdateMonthPVStat(_month);
                    UpdateYearPVStat(_year);
                    UpdateGuestPVStat(_guest);
                    UpdateMemberPVStat(_member);
                    UpdateUserPVStat(_user);

                    foreach (KeyValuePair<string, int> item in _browser)
                        UpdateBrowserPVStat(item.Key, item.Value);
                    foreach (KeyValuePair<string, int> item in _os)
                        UpdateOSPVStat(item.Key, item.Value);
                    foreach (KeyValuePair<int, int> item in _location)
                        UpdateRegionPVStat(item.Key.ToString(), item.Value);

                    ResetPVStats();

                    _lastupdatepvstatstime = Environment.TickCount;
                }
            }
        }

        /// <summary>
        /// 获得省级区域统计
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProvinceRegionStat()
        {
            return BrnMall.Data.PVStats.GetProvinceRegionStat();
        }

        /// <summary>
        /// 获得市级区域统计
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <returns></returns>
        public static DataTable GetCityRegionStat(int provinceId)
        {
            return BrnMall.Data.PVStats.GetCityRegionStat(provinceId);
        }

        /// <summary>
        /// 获得区/县级区域统计
        /// </summary>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static DataTable GetCountyRegionStat(int cityId)
        {
            return BrnMall.Data.PVStats.GetCountyRegionStat(cityId);
        }

        /// <summary>
        /// 获得PV统计列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static List<PVStatInfo> GetPVStatList(string condition)
        {
            return BrnMall.Data.PVStats.GetPVStatList(condition);
        }

        /// <summary>
        /// 获得PV统计
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static PVStatInfo GetPVStatByCategoryAndValue(string category, string value)
        {
            return BrnMall.Data.PVStats.GetPVStatByCategoryAndValue(category, value);
        }

        /// <summary>
        /// 获得小时的PV统计列表
        /// </summary>
        /// <param name="startHour">开始小时</param>
        /// <param name="endHour">结束小时</param>
        /// <returns></returns>
        public static List<PVStatInfo> GetHourPVStatList(string startHour, string endHour)
        {
            return GetPVStatList(string.Format(" [category]='hour' AND [value]>='{0}' AND [value]<='{1}'", startHour, endHour));
        }

        /// <summary>
        /// 获得今天小时的PV统计列表
        /// </summary>
        /// <returns></returns>
        public static List<PVStatInfo> GetTodayHourPVStatList()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return GetHourPVStatList(date + "00", date + "23");
        }

        /// <summary>
        /// 获得浏览器统计
        /// </summary>
        /// <returns></returns>
        public static List<PVStatInfo> GetBrowserStat()
        {
            return GetPVStatList(" [category]='browser'");
        }

        /// <summary>
        /// 获得操作系统统计
        /// </summary>
        /// <returns></returns>
        public static List<PVStatInfo> GetOSStat()
        {
            return GetPVStatList(" [category]='os'");
        }

        #region 辅助方法

        /// <summary>
        /// 重置pv统计
        /// </summary>
        private static void ResetPVStats()
        {
            _user = _member = _guest = _year = _month = _day = _hour = _week = 0;
            _browser = new ConcurrentDictionary<string, int>();
            _os = new ConcurrentDictionary<string, int>();
            _location = new ConcurrentDictionary<int, int>();
        }

        /// <summary>
        /// 更新位置PV统计
        /// </summary>
        /// <param name="location">位置</param>
        /// <param name="count">数量</param>
        private static void UpdateRegionPVStat(string location, int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("region", location, count);
        }

        /// <summary>
        /// 更新浏览器pv统计
        /// </summary>
        /// <param name="brower">浏览器</param>
        /// <param name="count">数量</param>
        private static void UpdateBrowserPVStat(string brower, int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("browser", brower, count);
        }

        /// <summary>
        /// 更新操作系统pv统计
        /// </summary>
        /// <param name="os">操作系统</param>
        /// <param name="count">数量</param>
        private static void UpdateOSPVStat(string os, int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("os", os, count);
        }

        /// <summary>
        /// 更新本周pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateWeekPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("week", DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.Month.ToString("00") + ((int)DateTime.Now.DayOfWeek).ToString(), count);
        }

        /// <summary>
        /// 更新当前小时pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateHourPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("hour", DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.Hour.ToString("00"), count);
        }

        /// <summary>
        /// 更新今天pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateDayPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("day", DateTime.Now.ToString("yyyy-MM-dd"), count);
        }

        /// <summary>
        /// 更新本月pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateMonthPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("month", DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00"), count);
        }

        /// <summary>
        /// 更新今年pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateYearPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("year", DateTime.Now.Year.ToString(), count);
        }

        /// <summary>
        /// 更新游客pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateGuestPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("total", "guest", count);
        }

        /// <summary>
        /// 更新会员pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateMemberPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("total", "member", count);
        }

        /// <summary>
        /// 更新用户pv统计
        /// </summary>
        /// <param name="count">数量</param>
        private static void UpdateUserPVStat(int count)
        {
            BrnMall.Data.PVStats.UpdatePVStat("total", "user", count);
        }

        #endregion
    }
}
