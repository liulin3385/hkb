using System;
using System.IO;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall会话状态管理类
    /// </summary>
    public class BMASession
    {
        private static ISessionStrategy _sessionstrategy = null;//会话状态策略

        static BMASession()
        {
            Load();
        }

        /// <summary>
        /// 会话状态策略实例
        /// </summary>
        public static ISessionStrategy Instance
        {
            get { return _sessionstrategy; }
        }

        /// <summary>
        /// 加载会话状态策略
        /// </summary>
        private static void Load()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.SessionStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _sessionstrategy = (ISessionStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.SessionStrategy.{0}.SessionStrategy, BrnMall.SessionStrategy.{0}", fileNameList[0].Substring(fileNameList[0].IndexOf("SessionStrategy.") + 16).Replace(".dll", "")),
                                                                                           false,
                                                                                           true));
            }
            catch
            {
                throw new BMAException("创建\"会话状态策略对象\"失败，可能存在的原因：未将\"会话状态策略程序集\"添加到bin目录中；将多个\"会话状态策略程序集\"添加到bin目录中；\"会话状态策略程序集\"文件名不符合\"BrnMall.SessionStrategy.{策略名称}.dll\"格式");
            }
        }
    }
}
