using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class UserDao
    {
        public List<ShopShow> GetShopShow()
        {
            List<ShopShow> list = new List<ShopShow>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_SHOP_SHOW);
            string sql = builder.ToString();

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    ShopShow shopShow = new ShopShow
                    {
                        shopId = dr["SHOP_ID"].ToString(),
                        shopName = dr["SHOP_NAME_ZH"].ToString(),
                        shopAddr = dr["SHOP_ADDR"].ToString(),
                        shopPhone = dr["SHOP_PHONE"].ToString(),
                        shopListImg = dr["SHOP_LIST_IMG"].ToString(),
                        userRate = ((double)dr["USER_RATE"] * 100) + "%",
                    };
                    list.Add(shopShow);
                }
                
            }

            return list;
        }

        public ShopInfo GetShopInfo(string shopId)
        {
            ShopInfo shopInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_SHOP_INFO, shopId);
            string sql = builder.ToString();

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                shopInfo = new ShopInfo
                {
                    shopName = dt.Rows[0]["SHOP_NAME_ZH"].ToString(),
                    shopDesc = dt.Rows[0]["SHOP_DESC"].ToString(),
                    shopInfoImg = dt.Rows[0]["SHOP_INFO_IMG"].ToString(),
                    shopAddr = dt.Rows[0]["SHOP_ADDR"].ToString(),
                    shopPhone = dt.Rows[0]["SHOP_PHONE"].ToString(),
                    userRate = ((double)dt.Rows[0]["USER_RATE"] * 100) + "%",
                };

                builder.Clear();
                builder.AppendFormat(UserSqls.SELECT_SHOP_BRANDS, shopId);
                sql = builder.ToString();
                dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
                if (dt != null)
                {
                    List<ShopBrands> list = new List<ShopBrands>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ShopBrands shopBrands = new ShopBrands
                        {
                            shopBrandsId = dr["SHOP_BRANDS_ID"].ToString(),
                            brandsName = dr["BRANDS_NAME"].ToString(),
                            brandsImg = dr["BRANDS_IMG"].ToString(),
                            goodsNum = dr["GOODS_NUM"].ToString(),
                            recommend = dr["RECOMMEND"].ToString(),
                        };
                        list.Add(shopBrands);
                    }
                    shopInfo.brandsList = list;
                }
            }

            return shopInfo;
        }

        public string GetUserScanCode(string openId)
        {
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(openId + new Random().Next()));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.UPDATE_USER_QRCODE_BY_OPENID, scanCode, openId);
            string sqlUpdate = builder.ToString();

            if(DatabaseOperationWeb.ExecuteDML(sqlUpdate))
            {
                return scanCode;
            }
            else
            {
                return "";
            }
        }

        public User GetUserByOpenID(string openId)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_USER_BY_OPENID, openId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                user = new User
                {
                    userId = dt.Rows[0]["USER_ID"].ToString(),
                    userType = dt.Rows[0]["USER_TYPE"].ToString(),
                };
            }

            return user;
        }

        public RecordStateSum GetStateSum(string userId, string userType)
        {
            RecordStateSum recordStateSum = new RecordStateSum();
            StringBuilder builder = new StringBuilder();
            string sql;
            DataTable dt;
            double sumMoney = 0;
            switch (userType)
            {
                case "0":
                    builder.AppendFormat(
                        UserSqls.SELECT_USER_RECORD_BY_USER_ID,
                        userId
                    );
                    sql = builder.ToString();
                    dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

                    DataRow[] drsProcess = dt.Select("PAY_STATE = 0 AND USER_PAY_STATE = 0");
                    DataRow[] drsPayApply = dt.Select("PAY_STATE = 1 AND USER_PAY_STATE = 1");
                    DataRow[] drsPaid = dt.Select("PAY_STATE = 1 AND USER_PAY_STATE = 2");
                    DataRow[] drsPayNotApply = dt.Select("PAY_STATE = 1 AND USER_PAY_STATE = 0");

                    foreach (DataRow dr in drsPayNotApply)
                    {
                        sumMoney += Convert.ToDouble(dr["USER_MONEY"]);
                    }

                    recordStateSum.process = drsProcess.Length;
                    recordStateSum.pay = drsPayApply.Length + drsPayNotApply.Length;
                    recordStateSum.paid = drsPaid.Length;
                    recordStateSum.payMoney = sumMoney;
                    break;
                case "1":
                    builder.AppendFormat(
                        UserSqls.SELECT_USER_RECORD_BY_AGENT_USER_ID,
                        userId
                    );
                    sql = builder.ToString();
                    dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

                    DataRow[] drsShopAgentProcess = dt.Select("PAY_STATE = 0 AND SHOP_AGENT_PAY_STATE = 0");
                    DataRow[] drsShopAgentPayApply = dt.Select("PAY_STATE = 1 AND SHOP_AGENT_PAY_STATE = 1");
                    DataRow[] drsShopAgentPaid = dt.Select("PAY_STATE = 1 AND SHOP_AGENT_PAY_STATE = 2");
                    DataRow[] drsShopAgentPayNotApply = dt.Select("PAY_STATE = 1 AND SHOP_AGENT_PAY_STATE = 0");

                    DataRow[] drsUserAgentProcess = dt.Select("PAY_STATE = 0 AND USER_AGENT_PAY_STATE = 0");
                    DataRow[] drsUserAgentPayApply = dt.Select("PAY_STATE = 1 AND USER_AGENT_PAY_STATE = 1");
                    DataRow[] drsUserAgentPaid = dt.Select("PAY_STATE = 1 AND USER_AGENT_PAY_STATE = 2");
                    DataRow[] drsUserAgentPayNotApply = dt.Select("PAY_STATE = 1 AND USER_AGENT_PAY_STATE = 0");

                    foreach (DataRow dr in drsShopAgentPayNotApply)
                    {
                        sumMoney += Convert.ToDouble(dr["SHOP_AGENT_MONEY"]);
                    }

                    foreach (DataRow dr in drsUserAgentPayNotApply)
                    {
                        sumMoney += Convert.ToDouble(dr["USER_AGENT_MONEY"]);
                    }

                    recordStateSum.process = drsShopAgentProcess.Length + drsUserAgentProcess.Length;
                    recordStateSum.pay = drsShopAgentPayApply.Length + drsShopAgentPayNotApply.Length + drsUserAgentPayApply.Length + drsUserAgentPayNotApply.Length;
                    recordStateSum.paid = drsShopAgentPaid.Length + drsUserAgentPaid.Length;
                    recordStateSum.payMoney = sumMoney;
                    break;
                
            }

            return recordStateSum;
        }

        public RecordStateList GetStateList(string userId, string userType)
        {
            RecordStateList recordStateList = new RecordStateList();
            StringBuilder builder = new StringBuilder();
            string sql;
            DataTable dt;
            double sumProcessMoney = 0;
            double sumPayMoney = 0;
            double sumPaidMoney = 0;
            double sumProcessTotal = 0;
            double sumPayTotal = 0;
            double sumPaidTotal = 0;
            switch (userType)
            {
                case "0":
                    builder.AppendFormat(
                        UserSqls.SELECT_USER_RECORD_BY_USER_ID,
                        userId
                    );
                    sql = builder.ToString();
                    dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

                    DataRow[] drsProcess = dt.Select("PAY_STATE = 0 AND USER_PAY_STATE = 0");
                    DataRow[] drsPay = dt.Select("PAY_STATE = 1 AND USER_PAY_STATE IN (0,1)");
                    DataRow[] drsPaid = dt.Select("PAY_STATE = 1 AND USER_PAY_STATE = 2");

                    foreach (DataRow dr in drsProcess)
                    {
                        sumProcessMoney += Convert.ToDouble(dr["USER_MONEY"]);
                        sumProcessTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                        };
                        recordStateList.processList.Add(recordState);
                    }
                    foreach (DataRow dr in drsPay)
                    {
                        sumPayMoney += Convert.ToDouble(dr["USER_MONEY"]);
                        sumPayTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                        };
                        recordStateList.payList.Add(recordState);
                    }
                    foreach (DataRow dr in drsPaid)
                    {
                        sumPaidMoney += Convert.ToDouble(dr["USER_MONEY"]);
                        sumPaidTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                        };
                        recordStateList.paidList.Add(recordState);
                    }

                    recordStateList.process = drsProcess.Length;
                    recordStateList.pay = drsPay.Length;
                    recordStateList.paid = drsPaid.Length;

                    recordStateList.processMoney = sumProcessMoney;
                    recordStateList.payMoney = sumPayMoney;
                    recordStateList.paidMoney = sumPaidMoney;

                    recordStateList.processTotal = sumProcessTotal;
                    recordStateList.payTotal = sumPayTotal;
                    recordStateList.paidTotal = sumPaidTotal;
                    break;
                case "1":
                    builder.AppendFormat(
                        UserSqls.SELECT_USER_RECORD_BY_AGENT_USER_ID,
                        userId
                    );
                    sql = builder.ToString();
                    dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

                    DataRow[] drsShopAgentProcess = dt.Select("PAY_STATE = 0 AND SHOP_AGENT_PAY_STATE = 0");
                    DataRow[] drsShopAgentPay = dt.Select("PAY_STATE = 1 AND SHOP_AGENT_PAY_STATE IN (0,1)");
                    DataRow[] drsShopAgentPaid = dt.Select("PAY_STATE = 1 AND SHOP_AGENT_PAY_STATE = 2");

                    DataRow[] drsUserAgentProcess = dt.Select("PAY_STATE = 0 AND USER_AGENT_PAY_STATE = 0");
                    DataRow[] drsUserAgentPay = dt.Select("PAY_STATE = 1 AND USER_AGENT_PAY_STATE IN (0,1)");
                    DataRow[] drsUserAgentPaid = dt.Select("PAY_STATE = 1 AND USER_AGENT_PAY_STATE = 2");

                    foreach (DataRow dr in drsShopAgentProcess)
                    {
                        sumProcessMoney += Convert.ToDouble(dr["SHOP_AGENT_MONEY"]);
                        sumProcessTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.processList.Add(recordState);
                    }
                    foreach (DataRow dr in drsUserAgentProcess)
                    {
                        sumProcessMoney += Convert.ToDouble(dr["USER_AGENT_MONEY"]);
                        sumProcessTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.processList.Add(recordState);
                    }

                    foreach (DataRow dr in drsShopAgentPay)
                    {
                        sumPayMoney += Convert.ToDouble(dr["SHOP_AGENT_MONEY"]);
                        sumPayTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.payList.Add(recordState);
                    }
                    foreach (DataRow dr in drsUserAgentPay)
                    {
                        sumPayMoney += Convert.ToDouble(dr["USER_AGENT_MONEY"]);
                        sumPayTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.payList.Add(recordState);
                    }

                    foreach (DataRow dr in drsShopAgentPaid)
                    {
                        sumPaidMoney += Convert.ToDouble(dr["SHOP_AGENT_MONEY"]);
                        sumPaidTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.paidList.Add(recordState);
                    }
                    foreach (DataRow dr in drsUserAgentPaid)
                    {
                        sumPaidMoney += Convert.ToDouble(dr["USER_AGENT_MONEY"]);
                        sumPaidTotal += Convert.ToDouble(dr["TOTAL"]);
                        RecordState recordState = new RecordState
                        {
                            recordId = dr["RECORD_ID"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                        };
                        recordStateList.paidList.Add(recordState);
                    }

                    recordStateList.process = drsShopAgentProcess.Length + drsUserAgentProcess.Length;
                    recordStateList.pay = drsShopAgentPay.Length + drsUserAgentPay.Length;
                    recordStateList.paid = drsShopAgentPaid.Length + drsUserAgentPaid.Length;

                    recordStateList.processMoney = sumProcessMoney;
                    recordStateList.payMoney = sumPayMoney;
                    recordStateList.paidMoney = sumPaidMoney;

                    recordStateList.processTotal = sumProcessTotal;
                    recordStateList.payTotal = sumPayTotal;
                    recordStateList.paidTotal = sumPaidTotal;

                    break;

            }

            return recordStateList;
        }
    }

    public class UserSqls
    {
        public const string SELECT_SHOP_SHOW = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP "
            + "ORDER BY PAY_SORT";
        public const string SELECT_SHOP_INFO = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP "
            + "WHERE SHOP_ID = {0}";
        public const string SELECT_SHOP_BRANDS = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP_BRANDS "
            + "WHERE SHOP_ID = {0} "
            + "ORDER BY PAY_SORT";
        public const string UPDATE_USER_QRCODE_BY_OPENID = ""
            + "UPDATE T_BASE_USER "
            + "SET SCAN_CODE = '{0}' "
            + "WHERE OPENID = '{1}'";
        public const string SELECT_USER_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE OPENID = '{0}'";
        public const string SELECT_USER_RECORD_BY_USER_ID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD RECORD,"
            + "T_BASE_USER USER,"
            + "T_BASE_SHOP SHOP "
            + "WHERE SHOP.SHOP_ID = RECORD.SHOP_ID "
            + "AND RECORD.USER_ID = USER.USER_ID "
            + "AND USER.USER_ID = {0} "
            + "ORDER BY RECORD_TIME DESC";
        public const string SELECT_USER_RECORD_BY_AGENT_USER_ID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD RECORD,"
            + "T_BASE_USER USER,"
            + "T_BASE_SHOP SHOP "
            + "WHERE SHOP.SHOP_ID = RECORD.SHOP_ID "
            + "AND RECORD.USER_ID = USER.USER_ID "
            + "AND (USER.USER_AGENT = {0} OR SHOP.SHOP_AGENT = {0})"
            + "ORDER BY RECORD_TIME DESC";
    }
}
