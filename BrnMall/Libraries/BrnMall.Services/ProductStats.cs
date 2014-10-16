using System;
using System.Data;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 商品统计操作管理类
    /// </summary>
    public class ProductStats
    {
        /// <summary>
        /// 更新商品统计
        /// </summary>
        /// <param name="state">UpdateProductStatState类型对象</param>
        public static void UpdateProductStat(object state)
        {
            UpdateProductStatState updateProductStatState = (UpdateProductStatState)state;

            string year = updateProductStatState.Time.Year.ToString();
            string month = updateProductStatState.Time.Year.ToString() + updateProductStatState.Time.Month.ToString("00");
            string day = updateProductStatState.Time.ToString("yyyy-MM-dd");
            string hour = updateProductStatState.Time.ToString("yyyy-MM-dd") + updateProductStatState.Time.Hour.ToString("00");
            string week = updateProductStatState.Time.ToString("yyyy-MM-dd") + updateProductStatState.Time.Month.ToString("00") + ((int)updateProductStatState.Time.DayOfWeek).ToString();

            string condition = string.Format(@"([pid]={0} AND [category]='total') 
                                                OR ([pid]={0} AND [category]='year' AND [value]='{1}')
                                                OR ([pid]={0} AND [category]='month' AND [value]='{2}') 
                                                OR ([pid]={0} AND [category]='day' AND [value]='{3}') 
                                                OR ([pid]={0} AND [category]='hour' AND [value]='{4}') 
                                                OR ([pid]={0} AND [category]='week' AND [value]='{5}') 
                                                OR ([pid]={0} AND [category]='region' AND [value]='{6}')",
                                                updateProductStatState.Pid, year, month, day, hour, week, updateProductStatState.RegionId);

            int affectRow = BrnMall.Data.ProductStats.UpdateProductStat(condition);
            if (affectRow < 7)
            {
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "total", "");
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "year", year);
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "month", month);
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "day", day);
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "hour", hour);
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "week", week);
                BrnMall.Data.ProductStats.AddProductStat(updateProductStatState.Pid, "region", updateProductStatState.RegionId.ToString());
            }
        }

        /// <summary>
        /// 获得商品总访问量列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProductTotalVisitCountList()
        {
            return BrnMall.Data.ProductStats.GetProductTotalVisitCountList();
        }
    }
}
