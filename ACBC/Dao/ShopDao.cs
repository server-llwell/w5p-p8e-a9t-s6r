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

        public Shop GetShop(string shopId)
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
                    shopRate = (double)dt.Rows[0]["SHOP_RATE"],
                    userRate = (double)dt.Rows[0]["USER_RATE"],
                };
            }

            return shop;
        }

        public bool InputRecord(
            SubmitParam submitParam, 
            double shopRate, 
            double userRate, 
            double shopMoney, 
            double absShopMoney, 
            double userMoney, 
            double absUserMoney)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.INSERT_RECORD,
                submitParam.userId,
                submitParam.total,
                shopRate,
                userRate,
                shopMoney,
                absShopMoney,
                userMoney,
                absUserMoney,
                submitParam.ticketCode,
                submitParam.ticketImg,
                submitParam.inputState,
                0,
                submitParam.shopId);
            string sqlInsert = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }

        public List<Record> GetRecordByShopIdAndPayState(string shopId, string payState)
        {
            List<Record> list = new List<Record>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.SELECT_SHOP_RECORD_BY_SHOP_ID_AND_PAY_STATE, shopId, payState);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    Record record = new Record
                    {
                        recordId = dr["RECORD_ID"].ToString(),
                        recordTime = dr["RECORD_TIME"].ToString(),
                        userName = dr["USER_NAME"].ToString(),
                        total = (double)dr["TOTAL"],
                        shopMoney = (double)dr["SHOP_MONEY"],
                        shopRate = ((double)dr["SHOP_RATE"]*100) + "%",
                        recordCode = dr["RECORD_CODE"].ToString(),
                        recordCodeImg = dr["RECORD_CODE_IMG"].ToString(),
                        payState = dr["PAY_STATE"].ToString(),
                        inputState = dr["INPUT_STATE"].ToString()
                    };
                    list.Add(record);
                }
            }
            return list;
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
            + "(RECORD_TIME,USER_ID,TOTAL,SHOP_RATE,USER_RATE,SHOP_MONEY,ABS_SHOP_MONEY,USER_MONEY,ABS_USER_MONEY,RECORD_CODE,RECORD_CODE_IMG,INPUT_STATE,PAY_STATE,SHOP_ID) "
            + "VALUES(NOW(),{0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}',{10},{11},{12})";
        public const string SELECT_SHOP_RECORD_BY_SHOP_ID_AND_PAY_STATE = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD A, T_BASE_USER B "
            + "WHERE A.USER_ID = B.USER_ID "
            + "AND SHOP_ID = '{0}' "
            + "AND PAY_STATE = {1} "
            + "ORDER BY RECORD_TIME DESC";
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

    public class Record
    {
        public string recordId;
        public string recordTime;
        public double total;
        public string userName;
        public string shopRate;
        public double shopMoney;
        public string recordCode;
        public string recordCodeImg;
        public string payState;
        public string inputState;
    }
}
