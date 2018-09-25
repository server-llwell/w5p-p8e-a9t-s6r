using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class UsersDao
    {
        public ShopUser GetShopUser(string openID)
        {
            ShopUser shopUser = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.SELECT_SHOP_USER_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if(dt != null && dt.Rows.Count == 1)
            {
                shopUser = new ShopUser
                {
                    shopUserImg = dt.Rows[0]["SHOP_USER_IMG"].ToString(),
                    shopUserName = dt.Rows[0]["SHOP_USER_NAME"].ToString(),
                };
            }

            return shopUser;
        }

        public Shop GetShopByCode(string shopCode)
        {
            Shop shop = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.SELECT_BIND_SHOP_BY_CODE, shopCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                shop = new Shop
                {
                    shopId = dt.Rows[0]["SHOP_ID"].ToString(),
                };
            }

            return shop;
        }

        public bool BindShop(BindShopParam bindShopParam, string openID, string shopId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.INSERT_BIND_SHOP,
                bindShopParam.nickName,
                bindShopParam.avatarUrl,
                openID,
                shopId);
            string sqlInsert = builder.ToString();
            builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.UPDATE_BIND_SHOP_CODE,
                bindShopParam.shopCode);
            string sqlUpdate = builder.ToString();
            ArrayList list = new ArrayList();
            list.Add(sqlInsert);
            list.Add(sqlUpdate);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public User GetUser(string openID)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.SELECT_USER_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                user = new User
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString(),
                    userId = dt.Rows[0]["USER_ID"].ToString(),
                    openid = dt.Rows[0]["OPENID"].ToString(),
                    userImg = dt.Rows[0]["USER_IMG"].ToString(),
                    phone = dt.Rows[0]["USER_PHONE"].ToString(),
                    userType = dt.Rows[0]["USER_TYPE"].ToString(),
                };
            }

            return user;
        }

        public User GetUserByPhone(string phone)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.SELECT_USER_BY_PHONE, phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                user = new User
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString(),
                    userId = dt.Rows[0]["USER_ID"].ToString(),
                    openid = dt.Rows[0]["OPENID"].ToString(),
                    userImg = dt.Rows[0]["USER_IMG"].ToString(),
                    phone = dt.Rows[0]["USER_PHONE"].ToString(),
                };
            }

            return user;
        }

        public User GetAgent(string code)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.SELECT_AGENT_BY_CODE, code);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                user = new User
                {
                   userId = dt.Rows[0]["USER_ID"].ToString(),
                   userName = dt.Rows[0]["USER_NAME"].ToString(),
                };
            }

            return user;
        }

        public bool UserReg(UserRegParam userRegParam, string openID, string agentId)
        {
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(openID));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }
            
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.INSERT_USER_REG,
                openID,
                userRegParam.nickName,
                userRegParam.avatarUrl,
                scanCode,
                agentId,
                userRegParam.phone,
                userRegParam.userType);
            string sqlInsert = builder.ToString();
            builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.UPDATE_USER_REG_AGENT_CODE,
                userRegParam.agentCode);
            string sqlUpdate = builder.ToString();
            ArrayList list = new ArrayList();
            list.Add(sqlInsert);
            list.Add(sqlUpdate);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public bool UpdateUserPhone(string openID, string phone)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UsersSqls.UPDATE_USER_PHONE,
                openID,
                phone);
            string sqlUpdate = builder.ToString();

            return DatabaseOperationWeb.ExecuteDML(sqlUpdate);
        }
    }

    public class UsersSqls
    {
        public const string SELECT_SHOP_USER_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP_USER "
            + "WHERE SHOP_USER_OPENID = '{0}'";
        public const string SELECT_BIND_SHOP_BY_CODE = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP A, T_BUSS_SHOP_CODE B "
            + "WHERE A.SHOP_ID = B.SHOP_ID "
            + "AND B.CODE = '{0}' "
            + "AND B.STATE > 0";
        public const string INSERT_BIND_SHOP = ""
            + "INSERT INTO T_BASE_SHOP_USER"
            + "(SHOP_USER_NAME,SHOP_USER_IMG,SHOP_USER_OPENID,SHOP_ID) "
            + "VALUES('{0}','{1}','{2}','{3}')";
        public const string UPDATE_BIND_SHOP_CODE = ""
            + "UPDATE T_BUSS_SHOP_CODE "
            + "SET STATE = STATE - 1 "
            + "WHERE CODE = '{0}'";
        public const string SELECT_USER_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE OPENID = '{0}'";
        public const string SELECT_USER_BY_PHONE = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE USER_PHONE = '{0}'";
        public const string SELECT_AGENT_BY_CODE = ""
            + "SELECT * "
            + "FROM T_BASE_USER A, T_BUSS_AGENT_CODE B "
            + "WHERE B.AGENT_ID = A.USER_ID "
            + "AND AGENT_CODE = '{0}' "
            + "AND AGENT_STATE > 0";
        public const string INSERT_USER_REG = ""
            + "INSERT INTO T_BASE_USER"
            + "(OPENID,USER_NAME,USER_IMG,SCAN_CODE,USER_AGENT,USER_PHONE,USER_TYPE) "
            + "VALUES('{0}','{1}','{2}','{3}',{4},'{5}',{6})";
        public const string UPDATE_USER_REG_AGENT_CODE = ""
            + "UPDATE T_BUSS_AGENT_CODE "
            + "SET AGENT_STATE = AGENT_STATE - 1 "
            + "WHERE AGENT_CODE = '{0}'";
        public const string UPDATE_USER_PHONE = ""
            + "UPDATE T_BASE_USER "
            + "SET USER_PHONE = {1} "
            + "WHERE OPENID = '{0}'";
    }


}
