using System;

namespace BrnMall.Core
{
    /// <summary>
    /// 非关系型数据库配置信息类
    /// </summary>
    [Serializable]
    public class NOSQLConfigInfo : IConfigInfo
    {
        private NOSQLInfo _usernosql = null;//用户非关系型数据库
        private NOSQLInfo _productnosql = null;//商品非关系型数据库
        private NOSQLInfo _ordernosql = null;//订单非关系型数据库

        /// <summary>
        /// 用户非关系型数据库
        /// </summary>
        public NOSQLInfo UserNOSQL
        {
            get { return _usernosql; }
            set { _usernosql = value; }
        }

        /// <summary>
        /// 商品非关系型数据库
        /// </summary>
        public NOSQLInfo ProductNOSQL
        {
            get { return _productnosql; }
            set { _productnosql = value; }
        }

        /// <summary>
        /// 订单非关系型数据库
        /// </summary>
        public NOSQLInfo OrderNOSQL
        {
            get { return _ordernosql; }
            set { _ordernosql = value; }
        }

    }
}
