﻿using System;
using System.Data;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 后台用户操作管理类
    /// </summary>
    public class AdminUsers : Users
    {
        /// <summary>
        /// 后台获得用户列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetUserList(int pageSize, int pageNumber, string condition, string sort)
        {
            return BrnMall.Data.Users.AdminGetUserList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得用户列表条件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="mobile">手机</param>
        /// <param name="userRid">用户等级</param>
        /// <param name="mallAGid">商城管理员组</param>
        /// <returns></returns>
        public static string AdminGetUserListCondition(string userName, string email, string mobile, int userRid, int mallAGid)
        {
            return BrnMall.Data.Users.AdminGetUserListCondition(userName, email, mobile, userRid, mallAGid);
        }

        /// <summary>
        /// 后台获得用户列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetUserListSort(string sortColumn, string sortDirection)
        {
            return BrnMall.Data.Users.AdminGetUserListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得用户列表数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetUserCount(string condition)
        {
            return BrnMall.Data.Users.AdminGetUserCount(condition);
        }

        /// <summary>
        /// 设置店铺管理员
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="storeId">店铺id</param>
        public static void SetStoreAdminer(int uid, int storeId)
        {
            BrnMall.Data.Users.SetStoreAdminer(uid, storeId);
        }
    }
}
