using System;

namespace BrnMall.Data
{
    /// <summary>
    /// 开放授权数据访问类
    /// </summary>
    public class OAuths
    {
        /// <summary>
        /// 创建开放授权用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="openId">开放id</param>
        /// <param name="server">服务商</param>
        public static bool CreateOAuthUser(int uid, string openId, string server)
        {
            return BrnMall.Core.BMAData.RDBS.CreateOAuthUser(uid, openId, server);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="openId">开放id</param>
        /// <param name="server">服务商</param>
        public static int GetUidByOpenIdAndServer(string openId, string server)
        {
            return BrnMall.Core.BMAData.RDBS.GetUidByOpenIdAndServer(openId, server);
        }
    }
}
