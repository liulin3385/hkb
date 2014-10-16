using System;
using System.Data;

namespace BrnMall.Data
{
    /// <summary>
    /// 商品统计数据访问类
    /// </summary>
    public class ProductStats
    {
        /// <summary>
        /// 添加商品统计
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="category">分类</param>
        /// <param name="value">值</param>
        public static void AddProductStat(int pid, string category, string value)
        {
            BrnMall.Core.BMAData.RDBS.AddProductStat(pid, category, value);
        }

        /// <summary>
        /// 更新商品统计
        /// </summary>
        /// <param name="condition">条件</param>
        public static int UpdateProductStat(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.UpdateProductStat(condition);
        }

        /// <summary>
        /// 获得商品总访问量列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProductTotalVisitCountList()
        {
            return BrnMall.Core.BMAData.RDBS.GetProductTotalVisitCountList();
        }
    }
}
