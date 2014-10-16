﻿using System;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall非关系型数据库策略之用户接口
    /// </summary>
    public interface IUserNOSQLStrategy
    {
        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        PartUserInfo GetPartUserById(int uid);

        /// <summary>
        /// 创建部分用户
        /// </summary>
        /// <param name="partUserInfo">部分用户信息</param>
        void CreatePartUser(PartUserInfo partUserInfo);

        /// <summary>
        /// 获得用户细节
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        UserDetailInfo GetUserDetailById(int uid);

        /// <summary>
        /// 创建用户细节
        /// </summary>
        /// <param name="userDetailInfo">用户细节</param>
        void CreateUserDetail(UserDetailInfo userDetailInfo);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        void UpdateUser(UserInfo userInfo);

        /// <summary>
        /// 更新部分用户
        /// </summary>
        /// <param name="partUserInfo">部分用户信息</param>
        void UpdatePartUser(PartUserInfo partUserInfo);

        /// <summary>
        /// 更新用户细节
        /// </summary>
        /// <param name="userDetailInfo">用户细节</param>
        void UpdateUserDetail(UserDetailInfo userDetailInfo);

        /// <summary>
        /// 更新用户最后访问
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="visitTime">访问时间</param>
        /// <param name="ip">ip</param>
        /// <param name="regionId">区域id</param>
        void UpdateUserLastVisit(int uid, DateTime visitTime, string ip, int regionId);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="uidList">用户id</param>
        void DeleteUserById(string uidList);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userName">用户名</param>
        /// <param name="nickName">昵称</param>
        /// <param name="avatar">头像</param>
        /// <param name="gender">性别</param>
        /// <param name="realName">真实名称</param>
        /// <param name="bday">出生日期</param>
        /// <param name="idCard">The id card.</param>
        /// <param name="regionId">区域id</param>
        /// <param name="address">所在地</param>
        /// <param name="bio">简介</param>
        void UpdateUser(int uid, string userName, string nickName, string avatar, int gender, string realName, DateTime bday, string idCard, int regionId, string address, string bio);

        /// <summary>
        /// 更新用户邮箱
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="email">邮箱</param>
        void UpdateUserEmailByUid(int uid, string email);

        /// <summary>
        /// 更新用户手机
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="mobile">手机</param>
        void UpdateUserMobileByUid(int uid, string mobile);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="password">密码</param>
        void UpdateUserPasswordByUid(int uid, string password);

        /// <summary>
        /// 更新用户解禁时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="liftBanTime">解禁时间</param>
        void UpdateUserLiftBanTimeByUid(int uid, DateTime liftBanTime);

        /// <summary>
        /// 更新用户等级
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userRid">用户等级id</param>
        void UpdateUserRankByUid(int uid, int userRid);
    }
}
