using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }

    public class ShopUser
    {
        public string shopUserName;
        public string shopUserImg;
    }

    
}
