using System;
using System.IO;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall数据管理类
    /// </summary>
    public class BMAData
    {
        private static IRDBSStrategy _rdbs = null;//关系型数据库

        private static object _locker = new object();//锁对象
        private static bool _enablednosql = false;//是否启用非关系型数据库
        private static IUserNOSQLStrategy _usernosql = null;//用户非关系型数据库
        private static IProductNOSQLStrategy _productnosql = null;//商品非关系型数据库
        private static IOrderNOSQLStrategy _ordernosql = null;//订单非关系型数据库

        static BMAData()
        {
            _enablednosql = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.NOSQLStrategy.*.dll", SearchOption.TopDirectoryOnly).Length > 0;
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.RDBSStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _rdbs = (IRDBSStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.RDBSStrategy.{0}.RDBSStrategy, BrnMall.RDBSStrategy.{0}", fileNameList[0].Substring(fileNameList[0].LastIndexOf("RDBSStrategy.") + 13).Replace(".dll", "")),
                                                                                            false,
                                                                                            true));
            }
            catch
            {
                throw new BMAException("创建\"关系数据库策略对象\"失败，可能存在的原因：未将\"关系数据库策略程序集\"添加到bin目录中；将多个\"关系数据库策略程序集\"添加到bin目录中；\"关系数据库策略程序集\"文件名不符合\"BrnMall.RDBSStrategy.{策略名称}.dll\"格式");
            }
        }

        /// <summary>
        /// 关系型数据库
        /// </summary>
        public static IRDBSStrategy RDBS
        {
            get { return _rdbs; }
        }

        /// <summary>
        /// 用户非关系型数据库
        /// </summary>
        public static IUserNOSQLStrategy UserNOSQL
        {
            get
            {
                if (_enablednosql && BMAConfig.NOSQLConfig.UserNOSQL.Enabled == 1)
                {
                    if (_usernosql == null)
                    {
                        lock (_locker)
                        {
                            if (_usernosql == null)
                            {
                                try
                                {
                                    string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.NOSQLStrategy.*.dll", SearchOption.TopDirectoryOnly);
                                    _usernosql = (IUserNOSQLStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.NOSQLStrategy.{0}.UserNOSQLStrategy, BrnMall.NOSQLStrategy.{0}", fileNameList[0].Substring(fileNameList[0].LastIndexOf("NOSQLStrategy.") + 14).Replace(".dll", "")),
                                                                                                                          false,
                                                                                                                          true),
                                                                                              BMAConfig.NOSQLConfig.UserNOSQL.ParamList);
                                }
                                catch
                                {
                                    throw new BMAException("创建\"用户非关系数据库策略对象\"失败，可能存在的原因：未将\"用户非关系数据库策略程序集\"添加到bin目录中；将多个\"用户非关系数据库策略程序集\"添加到bin目录中；\"用户非关系数据库策略程序集\"文件名不符合\"BrnMall.NOSQLStrategy.{策略名称}.dll\"格式");
                                }
                            }
                        }
                    }
                }
                return _usernosql;
            }
        }

        /// <summary>
        /// 商品非关系型数据库
        /// </summary>
        public static IProductNOSQLStrategy ProductNOSQL
        {
            get
            {
                if (_enablednosql && BMAConfig.NOSQLConfig.ProductNOSQL.Enabled == 1)
                {
                    if (_productnosql == null)
                    {
                        lock (_locker)
                        {
                            if (_productnosql == null)
                            {
                                try
                                {
                                    string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.NOSQLStrategy.*.dll", SearchOption.TopDirectoryOnly);
                                    _productnosql = (IProductNOSQLStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.NOSQLStrategy.{0}.ProductNOSQLStrategy, BrnMall.NOSQLStrategy.{0}", fileNameList[0].Substring(fileNameList[0].LastIndexOf("NOSQLStrategy.") + 14).Replace(".dll", "")),
                                                                                                                                false,
                                                                                                                                true),
                                                                                                     BMAConfig.NOSQLConfig.ProductNOSQL.ParamList);
                                }
                                catch
                                {
                                    throw new BMAException("创建\"商品非关系数据库策略对象\"失败，可能存在的原因：未将\"商品非关系数据库策略程序集\"添加到bin目录中；将多个\"商品非关系数据库策略程序集\"添加到bin目录中；\"商品非关系数据库策略程序集\"文件名不符合\"BrnMall.NOSQLStrategy.{策略名称}.dll\"格式");
                                }
                            }
                        }
                    }
                }
                return _productnosql;
            }
        }

        /// <summary>
        /// 订单非关系型数据库
        /// </summary>
        public static IOrderNOSQLStrategy OrderNOSQL
        {
            get
            {
                if (_enablednosql && BMAConfig.NOSQLConfig.OrderNOSQL.Enabled == 1)
                {
                    if (_ordernosql == null)
                    {
                        lock (_locker)
                        {
                            if (_ordernosql == null)
                            {
                                try
                                {
                                    string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.NOSQLStrategy.*.dll", SearchOption.TopDirectoryOnly);
                                    _ordernosql = (IOrderNOSQLStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.NOSQLStrategy.{0}.OrderNOSQLStrategy, BrnMall.NOSQLStrategy.{0}", fileNameList[0].Substring(fileNameList[0].LastIndexOf("NOSQLStrategy.") + 14).Replace(".dll", "")),
                                                                                                                            false,
                                                                                                                            true),
                                                                                                BMAConfig.NOSQLConfig.UserNOSQL.ParamList);
                                }
                                catch
                                {
                                    throw new BMAException("创建\"订单非关系数据库策略对象\"失败，可能存在的原因：未将\"订单非关系数据库策略程序集\"添加到bin目录中；将多个\"订单非关系数据库策略程序集\"添加到bin目录中；\"订单非关系数据库策略程序集\"文件名不符合\"BrnMall.NOSQLStrategy.{策略名称}.dll\"格式");
                                }
                            }
                        }
                    }
                }
                return _ordernosql;
            }
        }
    }
}
