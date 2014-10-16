using System;
using System.IO;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall配置管理类
    /// </summary>
    public class BMAConfig
    {
        private static object _locker = new object();//锁对象

        private static IConfigStrategy _configstrategy = null;//配置策略对象

        private static RDBSConfigInfo _rdbsconfiginfo = null;//关系数据库配置信息
        private static MallConfigInfo _mallconfiginfo = null;//商城基本配置信息
        private static EmailConfigInfo _emailconfiginfo = null;//邮件配置信息
        private static SMSConfigInfo _smsconfiginfo = null;//短信配置信息
        private static CreditConfigInfo _creditconfiginfo = null;//积分配置信息
        private static EventConfigInfo _eventconfiginfo = null;//事件配置信息
        private static NOSQLConfigInfo _nosqlconfiginfo = null;//非关系数据库配置信息

        static BMAConfig()
        {
            Load();
            _rdbsconfiginfo = _configstrategy.GetRDBSConfig();
            _mallconfiginfo = _configstrategy.GetMallConfig();
        }

        /// <summary>
        /// 加载配置策略
        /// </summary>
        private static void Load()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.ConfigStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _configstrategy = (IConfigStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.ConfigStrategy.{0}.ConfigStrategy, BrnMall.ConfigStrategy.{0}", fileNameList[0].Substring(fileNameList[0].LastIndexOf("ConfigStrategy.") + 15).Replace(".dll", "")),
                                                                                         false,
                                                                                         true));
            }
            catch
            {
                throw new BMAException("创建\"配置策略对象\"失败，可能存在的原因：未将\"配置策略程序集\"添加到bin目录中；将多个\"配置策略程序集\"添加到bin目录中；\"配置策略程序集\"文件名不符合\"BrnMall.ConfigStrategy.{策略名称}.dll\"格式");
            }
        }

        /// <summary>
        /// 关系数据库配置信息
        /// </summary>
        public static RDBSConfigInfo RDBSConfig
        {
            get { return _rdbsconfiginfo; }
        }

        /// <summary>
        /// 商城基本配置信息
        /// </summary>
        public static MallConfigInfo MallConfig
        {
            get { return _mallconfiginfo; }
        }

        /// <summary>
        /// 邮件配置信息
        /// </summary>
        public static EmailConfigInfo EmailConfig
        {
            get
            {
                if (_emailconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_emailconfiginfo == null)
                        {
                            _emailconfiginfo = _configstrategy.GetEmailConfig();
                        }
                    }
                }

                return _emailconfiginfo;
            }
        }

        /// <summary>
        /// 短息配置信息
        /// </summary>
        public static SMSConfigInfo SMSConfig
        {
            get
            {
                if (_smsconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_smsconfiginfo == null)
                        {
                            _smsconfiginfo = _configstrategy.GetSMSConfig();
                        }
                    }
                }
                return _smsconfiginfo;
            }
        }

        /// <summary>
        /// 积分配置信息
        /// </summary>
        public static CreditConfigInfo CreditConfig
        {
            get
            {
                if (_creditconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_creditconfiginfo == null)
                        {
                            _creditconfiginfo = _configstrategy.GetCreditConfig();
                        }
                    }
                }
                return _creditconfiginfo;
            }
        }

        /// <summary>
        /// 事件配置信息
        /// </summary>
        public static EventConfigInfo EventConfig
        {
            get
            {
                if (_eventconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_eventconfiginfo == null)
                        {
                            _eventconfiginfo = _configstrategy.GetEventConfig();
                        }
                    }
                }
                return _eventconfiginfo;
            }
        }

        /// <summary>
        /// 非关系型数据库配置信息
        /// </summary>
        public static NOSQLConfigInfo NOSQLConfig
        {
            get
            {
                if (_nosqlconfiginfo == null)
                {
                    lock (_locker)
                    {
                        if (_nosqlconfiginfo == null)
                        {
                            _nosqlconfiginfo = _configstrategy.GetNOSQLConfig();
                        }
                    }
                }
                return _nosqlconfiginfo;
            }
        }



        /// <summary>
        /// 保存商城配置信息
        /// </summary>
        public static void SaveMallConfig(MallConfigInfo mallConfigInfo)
        {
            lock (_locker)
            {
                if (_configstrategy.SaveMallConfig(mallConfigInfo))
                    _mallconfiginfo = mallConfigInfo;
            }
        }

        /// <summary>
        /// 保存邮件配置信息
        /// </summary>
        public static void SaveEmailConfig(EmailConfigInfo emailConfigInfo)
        {
            lock (_locker)
            {
                if (_configstrategy.SaveEmailConfig(emailConfigInfo))
                    _emailconfiginfo = null;
            }
        }

        /// <summary>
        /// 保存短信配置信息
        /// </summary>
        public static void SaveSMSConfig(SMSConfigInfo smsConfigInfo)
        {
            lock (_locker)
            {
                if (_configstrategy.SaveSMSConfig(smsConfigInfo))
                    _smsconfiginfo = null;
            }
        }

        /// <summary>
        /// 保存积分配置信息
        /// </summary>
        public static void SaveCreditConfig(CreditConfigInfo creditConfigInfo)
        {
            lock (_locker)
            {
                if (_configstrategy.SaveCreditConfig(creditConfigInfo))
                    _creditconfiginfo = null;
            }
        }

        /// <summary>
        /// 保存事件配置信息
        /// </summary>
        public static void SaveEventConfig(EventConfigInfo eventConfigInfo)
        {
            lock (_locker)
            {
                if (_configstrategy.SaveEventConfig(eventConfigInfo))
                    _eventconfiginfo = null;
            }
        }
    }
}
