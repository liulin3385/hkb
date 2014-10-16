using System;

using BrnMall.Core;

namespace BrnMall.ConfigStrategy.File
{
    /// <summary>
    /// 基于文件的配置策略
    /// </summary>
    public class ConfigStrategy : IConfigStrategy
    {
        #region 私有字段

        private readonly string _rdbsconfigfilepath = "/App_Data/RDBS.config";//关系数据库配置信息文件路径
        private readonly string _mallconfigfilepath = "/App_Data/Mall.config";//商城基本配置信息文件路径
        private readonly string _emailconfigfilepath = "/App_Data/Email.config";//邮件配置信息文件路径
        private readonly string _smsconfigfilepath = "/App_Data/SMS.config";//短信配置信息文件路径
        private readonly string _creditconfigfilepath = "/App_Data/Credit.config";//积分配置信息文件路径
        private readonly string _eventconfigfilepath = "/App_Data/Event.config";//事件配置信息文件路径
        private readonly string _nosqlconfigfilepath = "/App_Data/NOSQL.config";//非关系型数据库配置信息文件路径

        #endregion

        #region 帮助方法

        /// <summary>
        /// 从文件中加载配置信息
        /// </summary>
        /// <param name="configInfoType">配置信息类型</param>
        /// <param name="configInfoFile">配置信息文件路径</param>
        /// <returns>配置信息</returns>
        private IConfigInfo LoadConfigInfo(Type configInfoType, string configInfoFile)
        {
            return (IConfigInfo)IOHelper.DeserializeFromXML(configInfoType, configInfoFile);
        }

        /// <summary>
        /// 将配置信息保存到文件中
        /// </summary>
        /// <param name="configInfo">配置信息</param>
        /// <param name="configInfoFile">保存路径</param>
        /// <returns>是否保存成功</returns>
        private bool SaveConfigInfo(IConfigInfo configInfo, string configInfoFile)
        {
            return IOHelper.SerializeToXml(configInfo, configInfoFile);
        }

        #endregion

        /// <summary>
        /// 保存关系数据库配置
        /// </summary>
        /// <param name="configInfo">关系数据库配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveRDBSConfig(RDBSConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_rdbsconfigfilepath));
        }

        /// <summary>
        /// 获得关系数据库配置
        /// </summary>
        public RDBSConfigInfo GetRDBSConfig()
        {
            return (RDBSConfigInfo)LoadConfigInfo(typeof(RDBSConfigInfo), IOHelper.GetMapPath(_rdbsconfigfilepath));
        }

        /// <summary>
        /// 保存商城基本配置
        /// </summary>
        /// <param name="configInfo">商城基本配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveMallConfig(MallConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_mallconfigfilepath));
        }

        /// <summary>
        /// 获得商城基本配置
        /// </summary>
        public MallConfigInfo GetMallConfig()
        {
            return (MallConfigInfo)LoadConfigInfo(typeof(MallConfigInfo), IOHelper.GetMapPath(_mallconfigfilepath));
        }

        /// <summary>
        /// 保存邮件配置
        /// </summary>
        /// <param name="configInfo">邮件配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveEmailConfig(EmailConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_emailconfigfilepath));
        }

        /// <summary>
        /// 获得邮件配置
        /// </summary>
        public EmailConfigInfo GetEmailConfig()
        {
            return (EmailConfigInfo)LoadConfigInfo(typeof(EmailConfigInfo), IOHelper.GetMapPath(_emailconfigfilepath));
        }

        /// <summary>
        /// 保存短信配置
        /// </summary>
        /// <param name="configInfo">短信配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveSMSConfig(SMSConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_smsconfigfilepath));
        }

        /// <summary>
        /// 获得短信配置
        /// </summary>
        public SMSConfigInfo GetSMSConfig()
        {
            return (SMSConfigInfo)LoadConfigInfo(typeof(SMSConfigInfo), IOHelper.GetMapPath(_smsconfigfilepath));
        }

        /// <summary>
        /// 保存积分配置
        /// </summary>
        /// <param name="configInfo">积分配置信息</param>
        /// <returns>是否保存结果</returns>
        public bool SaveCreditConfig(CreditConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_creditconfigfilepath));
        }

        /// <summary>
        /// 获得积分配置
        /// </summary>
        /// <returns></returns>
        public CreditConfigInfo GetCreditConfig()
        {
            return (CreditConfigInfo)LoadConfigInfo(typeof(CreditConfigInfo), IOHelper.GetMapPath(_creditconfigfilepath));
        }

        /// <summary>
        /// 保存事件配置
        /// </summary>
        /// <param name="configInfo">事件配置信息</param>
        /// <returns>是否保存成功</returns>
        public bool SaveEventConfig(EventConfigInfo configInfo)
        {
            return SaveConfigInfo(configInfo, IOHelper.GetMapPath(_eventconfigfilepath));
        }

        /// <summary>
        /// 获得事件配置
        /// </summary>
        /// <returns></returns>
        public EventConfigInfo GetEventConfig()
        {
            return (EventConfigInfo)LoadConfigInfo(typeof(EventConfigInfo), IOHelper.GetMapPath(_eventconfigfilepath));
        }

        /// <summary>
        /// 获得非关系型数据库配置
        /// </summary>
        public NOSQLConfigInfo GetNOSQLConfig()
        {
            return (NOSQLConfigInfo)LoadConfigInfo(typeof(NOSQLConfigInfo), IOHelper.GetMapPath(_nosqlconfigfilepath));
        }
    }
}
