using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class ShopDao
    {
        public User GetUser(string code)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.SELECT_USER_BY_CODE, code);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                user = new User
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString(),
                    userId = dt.Rows[0]["USER_ID"].ToString(),
                };
            }

            return user;
        }

        public Shop GetShopByOpenID(string openID, string lang)
        {
            Shop shop = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.SELECT_SHOP_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                shop = new Shop
                {
                    shopName = dt.Rows[0]["SHOP_NAME_" + lang.ToUpper()].ToString(),
                    shopId = dt.Rows[0]["SHOP_ID"].ToString(),
                };
            }

            return shop;
        }

        public Shop GetShop(int shopId, string lang)
        {
            Shop shop = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.SELECT_SHOP_BY_SHOP_ID, shopId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                shop = new Shop
                {
                    shopName = dt.Rows[0]["SHOP_NAME_" + lang.ToUpper()].ToString(),
                    shopId = dt.Rows[0]["SHOP_ID"].ToString(),
                    shopRate = (double)dt.Rows[0]["SHOP_RATE"],
                    userRate = (double)dt.Rows[0]["USER_RATE"],
                };
            }

            return shop;
        }

        public bool InputRecord(SubmitParam submitParam, double shopRate, double userRate, double money, double absMoney)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.INSERT_RECORD,
                submitParam.userId,
                submitParam.total,
                shopRate,
                userRate,
                money,
                absMoney,
                submitParam.ticketCode,
                submitParam.ticketImg,
                submitParam.inputState,
                0);
            string sqlInsert = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }
    }

    public class ShopSqls
    {
        public const string SELECT_SHOP_BY_SHOP_ID = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP "
            + "WHERE SHOP_ID = '{0}'";
        public const string SELECT_SHOP_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP_USER A, T_BASE_SHOP B "
            + "WHERE A.SHOP_ID = B.SHOP_ID "
            + "AND A.SHOP_USER_OPENID = '{0}'";
        public const string SELECT_USER_BY_CODE = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE SCAN_CODE = '{0}'";
        public const string SELECT_SHOP_RATE_BY_USER_ID = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP_USER "
            + "WHERE SCAN_CODE = '{0}'";
        public const string INSERT_RECORD = ""
            + "INSERT INTO T_BUSS_RECORD"
            + "(RECORD_TIME,USER_ID,TOTAL,SHOP_RATE,USER_RATE,MONEY,ABS_MONEY,RECORD_CODE,RECORD_IMG,PAY_STATE,INPUT_STATE) "
            + "VALUES(NOW(),{0},{1},{2},{3},{4},{5},'{6}','{7}','{8}',{9},{10})";

    }

    public class Shop
    {
        public string shopId;
        public string shopName;
        public double shopRate;
        public double userRate;
    }

    public class User
    {
        public string userName;
        public string userId;
    }
}
