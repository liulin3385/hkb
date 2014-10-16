using System;

namespace BrnMall.Core
{
    /// <summary>
    /// 非关系型数据库信息类
    /// </summary>
    public class NOSQLInfo
    {
        private int _enabled;//是否启用
        private string _name;//名称
        private string _paramlist;//参数列表

        /// <summary>
        /// 是否启用
        /// </summary>
        public int Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 参数列表
        /// </summary>
        public string ParamList
        {
            get { return _paramlist; }
            set { _paramlist = value; }
        }
    }
}
