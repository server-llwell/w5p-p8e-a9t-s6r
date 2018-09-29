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
    public class StaffDao
    {
        public List<RecordShopPaySum> GetRecordShopPayList()
        {
            List<RecordShopPaySum> list = new List<RecordShopPaySum>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_SHOP_BY_PAY_STATE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    RecordShopPaySum recordShopPaySum = new RecordShopPaySum
                    {
                        shopId = dr["SHOP_ID"].ToString(),
                        shopName = dr["SHOP_NAME"].ToString(),
                        num = Convert.ToInt32(dr["NUM"]),
                        sumTotal = Convert.ToDouble(dr["SUM_TOTAL"]),
                        sumShopMoney = Convert.ToDouble(dr["SUM_SHOP_MONEY"]),
                        shopRate = (Convert.ToDouble(dr["SHOP_RATE"]) * 100).ToString() + "%",
                    };
                    list.Add(recordShopPaySum);
                }
                
            }

            return list;
        }

        public List<RecordShopPaySum> GetGatherList(int limit)
        {
            List<RecordShopPaySum> list = new List<RecordShopPaySum>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_GATHER, limit);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RecordShopPaySum recordShopPaySum = new RecordShopPaySum
                    {
                        shopId = dr["SHOP_ID"].ToString(),
                        shopName = dr["SHOP_NAME_ZH"].ToString(),
                        num = Convert.ToInt32(dr["RECORD_COUNT"]),
                        sumTotal = Convert.ToDouble(dr["TOTAL"]),
                        sumShopMoney = Convert.ToDouble(dr["SHOP_MONEY"]),
                        shopRate = dr["SHOP_RATE"].ToString(),
                        gatherTime = dr["GATHER_TIME"].ToString(),
                        guid = dr["GUID"].ToString(),
                    };
                    list.Add(recordShopPaySum);
                }

            }

            return list;
        }

        public List<RecordShopPayItem> GetRecordShopPayItemListByShopId(string shopId)
        {
            List<RecordShopPayItem> list = new List<RecordShopPayItem>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_SHOP_LIST_BY_SHOP_ID, shopId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RecordShopPayItem recordShopPayItem = new RecordShopPayItem
                    {
                        recordTime = dr["RECORD_TIME"].ToString(),
                        total = Convert.ToDouble(dr["TOTAL"]),
                        shopMoney = Convert.ToDouble(dr["SHOP_MONEY"]),
                        shopRate = (Convert.ToDouble(dr["SHOP_RATE"]) * 100).ToString() + "%",
                    };
                    list.Add(recordShopPayItem);
                }

            }

            return list;
        }

        public List<RecordShopPayItem> GetRecordShopPayItemListByGuid(string guid)
        {
            List<RecordShopPayItem> list = new List<RecordShopPayItem>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_GATHER_LIST_BY_GUID, guid);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RecordShopPayItem recordShopPayItem = new RecordShopPayItem
                    {
                        recordTime = dr["RECORD_TIME"].ToString(),
                        total = Convert.ToDouble(dr["TOTAL"]),
                        shopMoney = Convert.ToDouble(dr["SHOP_MONEY"]),
                        shopRate = (Convert.ToDouble(dr["SHOP_RATE"]) * 100).ToString() + "%",
                    };
                    list.Add(recordShopPayItem);
                }

            }

            return list;
        }

        public ScanCodeType GetScanCodeType(string scanCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_SHOP_USER_CODE, scanCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return ScanCodeType.Shop;
            }
            
            builder.Clear();
            builder.AppendFormat(StaffSqls.SELECT_USER_CODE, scanCode);
            sql = builder.ToString();
            dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return ScanCodeType.User;
            }

            return ScanCodeType.Null;
        }

        public ScanCodeResult GetShopPayInfo(string scanCode)
        {
            ScanCodeResult scanCodeResult = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_SHOP_BY_SHOP_CODE, scanCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                scanCodeResult = new ScanCodeResult
                {
                    resultType = "SHOP",
                    resultKey = dt.Rows[0]["SHOP_ID"].ToString(),
                    resultTitle = dt.Rows[0]["SHOP_NAME"].ToString(),
                    resultMoney = Convert.ToDouble(dt.Rows[0]["SUM_SHOP_MONEY"]),
                    resultUser = dt.Rows[0]["SHOP_USER_ID"].ToString(),
                };

            }

            return scanCodeResult;
        }

        public ScanCodeResult GetUserPayInfo(string scanCode)
        {
            ScanCodeResult scanCodeResult = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_USER_BY_SCAN_CODE, scanCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                scanCodeResult = new ScanCodeResult
                {
                    resultType = "USER",
                    resultKey = dt.Rows[0]["GUID"].ToString(),
                    resultTitle = dt.Rows[0]["USER_NAME"].ToString() + " " + dt.Rows[0]["USER_PHONE"].ToString(),
                    resultMoney = Convert.ToDouble(dt.Rows[0]["MONEY"]),
                    resultUser = dt.Rows[0]["USER_ID"].ToString(),
                };

            }

            return scanCodeResult;
        }

        public Staff GetStaffByOpenID(string openId)
        {
            Staff staff = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_STAFF_BY_OPENID, openId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                staff = new Staff
                {
                    staffId = dt.Rows[0]["STAFF_ID"].ToString(),
                };
            }

            return staff;
        }

        public bool ShopPay(string shopId, string shopUserId, string staffId)
        {
            string guid = Guid.NewGuid().ToString();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.UPDATE_RECORD_SHOP_PAY_BY_SHOP_ID, shopId, guid);
            string updateSql = builder.ToString();
            if(!DatabaseOperationWeb.ExecuteDML(updateSql))
            {
                return false;
            }

            List<RecordShopPayItem> paidList = GetRecordShopPayItemListByGuid(guid);

            if(paidList.Count == 0)
            {
                return false;
            }

            int paidCount = 0;
            double paidSumTotal = 0;
            double paidSumMoney = 0;
            string shopRate = "";
            foreach (RecordShopPayItem recordShopPayItem in paidList)
            {
                paidCount++;
                paidSumTotal += recordShopPayItem.total;
                paidSumMoney += recordShopPayItem.shopMoney;
                shopRate = recordShopPayItem.shopRate;
            }

            builder.Clear();
            builder.AppendFormat(
                StaffSqls.INSERT_GATHER, 
                shopId,
                shopUserId,
                staffId,
                paidSumMoney,
                shopRate,
                paidSumTotal,
                guid,
                paidCount);
            string insertSql = builder.ToString();
            if (!DatabaseOperationWeb.ExecuteDML(insertSql))
            {
                return false;
            }
            builder.Clear();
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(shopUserId + new Random().Next()));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }
            builder.AppendFormat(StaffSqls.UPDATE_SHOP_QRCODE, scanCode, shopUserId);
            string sql = builder.ToString();
            DatabaseOperationWeb.ExecuteDML(sql);
            return true;
        }

        public List<PayItem> GetPayList(string payState, string payType)
        {
            List<PayItem> list = new List<PayItem>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_APPLY_LIST_BY_USER_ID, payState, payType);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PayItem payItem = new PayItem
                    {
                        payName = dr["USER_NAME"].ToString() + " " + dr["USER_PHONE"].ToString(),
                        count = Convert.ToInt32(dr["COUNT"]),
                        money = Convert.ToDouble(dr["MONEY"]),
                        total = Convert.ToDouble(dr["TOTAL"]),
                        applyTime = dr["APPLY_TIME"].ToString(),
                        payTime = dr["PAY_TIME"].ToString(),
                        addr = dr["ADDR"].ToString(),
                        guid = dr["GUID"].ToString(),
                        payImg = dr["PAY_IMG"].ToString(),
                    };
                    list.Add(payItem);
                }

            }

            return list;
        }

        public List<RecordUserPayItem> GetPayRecordList(string guid, string payType)
        {
            switch(payType)
            {
                case "0":
                    return getPay(guid);
                case "1":
                    return getBunkcardPay(guid);
                default:
                    return null;
            }
        }

        private List<RecordUserPayItem> getPay(string guid)
        {
            List<RecordUserPayItem> list = new List<RecordUserPayItem>();
            StringBuilder builder = new StringBuilder();
            string sql;
            builder.AppendFormat(StaffSqls.SELECT_RECORD_USER_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtUser = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            builder.Clear();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_SHOP_AGENT_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtShopAgent = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            builder.Clear();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_USER_AGENT_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtUserAgent = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            int count = 0;
            double money = 0;
            double total = 0;
            foreach (DataRow dr in dtUser.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["USER_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["USER_MONEY"]),
                };
                count ++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }
            foreach (DataRow dr in dtShopAgent.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                };
                count++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }
            foreach (DataRow dr in dtUserAgent.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                };
                count++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }

            return list;
        }

        private List<RecordUserPayItem> getBunkcardPay(string guid)
        {
            List<RecordUserPayItem> list = new List<RecordUserPayItem>();
            StringBuilder builder = new StringBuilder();
            string sql;
            builder.AppendFormat(StaffSqls.SELECT_RECORD_USER_RMB_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtUser = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            builder.Clear();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_SHOP_AGENT_RMB_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtShopAgent = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            builder.Clear();
            builder.AppendFormat(StaffSqls.SELECT_RECORD_USER_AGENT_RMB_SUM_BY_GUID, guid);
            sql = builder.ToString();
            DataTable dtUserAgent = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            int count = 0;
            double money = 0;
            double total = 0;
            foreach (DataRow dr in dtUser.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["USER_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["USER_RMB_MONEY"]),
                };
                count++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }
            foreach (DataRow dr in dtShopAgent.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["SHOP_AGENT_RMB_MONEY"]),
                };
                count++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }
            foreach (DataRow dr in dtUserAgent.Rows)
            {
                RecordUserPayItem recordUserPayItem = new RecordUserPayItem
                {
                    recordTime = dr["RECORD_TIME"].ToString(),
                    total = Convert.ToInt32(dr["TOTAL"]),
                    rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100).ToString() + "%",
                    money = Convert.ToDouble(dr["USER_AGENT_RMB_MONEY"]),
                };
                count++;
                money += recordUserPayItem.money;
                total += recordUserPayItem.total;
                list.Add(recordUserPayItem);
            }

            return list;
        }

        public PayInfo GetPayInfo(string guid, List<RecordUserPayItem> list)
        {
            PayInfo payInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_BUSS_PAY_BY_GUID, guid);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                payInfo = new PayInfo
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString() + " " + dt.Rows[0]["USER_PHONE"].ToString(),
                    addr = dt.Rows[0]["ADDR"].ToString(),
                    applyTime = dt.Rows[0]["APPLY_TIME"].ToString(),
                    payTime = dt.Rows[0]["PAY_TIME"].ToString(),
                    total = Convert.ToDouble(dt.Rows[0]["TOTAL"]),
                    money = Convert.ToDouble(dt.Rows[0]["MONEY"]),
                    count = Convert.ToInt32(dt.Rows[0]["COUNT"]),
                    list = list,
                };
            }

            return payInfo;
        }

        public BankcardInfo GetBankcardInfo(string guid, List<RecordUserPayItem> list)
        {
            BankcardInfo bankcardInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StaffSqls.SELECT_BUSS_PAY_BANKCARD_BY_GUID, guid);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                bankcardInfo = new BankcardInfo
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString() + " " + dt.Rows[0]["USER_PHONE"].ToString(),
                    applyTime = dt.Rows[0]["APPLY_TIME"].ToString(),
                    payTime = dt.Rows[0]["PAY_TIME"].ToString(),
                    total = Convert.ToDouble(dt.Rows[0]["TOTAL"]),
                    money = Convert.ToDouble(dt.Rows[0]["MONEY"]),
                    count = Convert.ToInt32(dt.Rows[0]["COUNT"]),
                    bankcardCode = dt.Rows[0]["BANKCARD_CODE"].ToString(),
                    bankName = dt.Rows[0]["BANK_NAME"].ToString(),
                    bankcardUserName = dt.Rows[0]["BANKCARD_USER_NAME"].ToString(),
                    subName = dt.Rows[0]["SUB_NAME"].ToString(),
                    payImg = dt.Rows[0]["PAY_IMG"].ToString(),
                    list = list,
                };
            }

            return bankcardInfo;
        }

        public bool UserPay(string guid, string staffId, string userId)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            string sql;
            builder.AppendFormat(StaffSqls.UPDATE_RECORD_USER_PAY_STATE_BY_GUID, guid);
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(StaffSqls.UPDATE_RECORD_SHOP_AGENT_PAY_STATE_BY_GUID, guid);
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(StaffSqls.UPDATE_RECORD_USER_AGENT_PAY_STATE_BY_GUID, guid);
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(StaffSqls.UPDATE_BUSS_PAY_STATE_BY_GUID, guid, staffId);
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(userId + new Random().Next()));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }
            builder.AppendFormat(StaffSqls.UPDATE_USER_QRCODE, scanCode, userId);
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }
    }

    public class StaffSqls
    {
        public const string SELECT_SHOP_USER_CODE = ""
            + "SELECT SHOP_CODE AS SCAN_CODE "
            + "FROM T_BASE_SHOP_USER "
            + "WHERE SHOP_CODE = '{0}'";
        public const string SELECT_USER_CODE = ""
           + "SELECT SCAN_CODE "
           + "FROM T_BASE_USER "
           + "WHERE SCAN_CODE = '{0}'";
        public const string SELECT_STAFF_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_STAFF "
            + "WHERE OPENID = '{0}'";
        public const string SELECT_RECORD_SHOP_BY_PAY_STATE = ""
            + "SELECT A.SHOP_ID,A.SHOP_NAME_ZH AS SHOP_NAME, "
            + "A.SHOP_RATE, COUNT(*) AS NUM, "
            + "SUM(TOTAL) AS SUM_TOTAL, "
            + "SUM(SHOP_MONEY) AS SUM_SHOP_MONEY "
            + "FROM T_BUSS_RECORD T, T_BASE_SHOP A "
            + "WHERE PAY_STATE = 0 "
            + "AND T.SHOP_ID = A.SHOP_ID "
            + "GROUP BY A.SHOP_ID,A.SHOP_NAME_ZH, A.SHOP_RATE ";
        public const string SELECT_GATHER = ""
            + "SELECT * "
            + "FROM T_BUSS_GATHER T,T_BASE_SHOP A "
            + "WHERE T.SHOP_ID = A.SHOP_ID "
            + "ORDER BY GATHER_TIME DESC "
            + "LIMIT {0}";
        public const string SELECT_RECORD_SHOP_LIST_BY_SHOP_ID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE SHOP_ID = {0} "
            + "AND PAY_STATE = 0 ";
        public const string SELECT_GATHER_LIST_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE SHOP_GATHER_GUID = '{0}' ";
        public const string SELECT_RECORD_SHOP_BY_SHOP_CODE = ""
            + "SELECT A.SHOP_ID,A.SHOP_NAME_ZH AS SHOP_NAME,B.SHOP_USER_ID,SUM(SHOP_MONEY) AS SUM_SHOP_MONEY "
            + "FROM T_BUSS_RECORD T, T_BASE_SHOP A, T_BASE_SHOP_USER B "
            + "WHERE PAY_STATE = 0 "
            + "AND T.SHOP_ID = A.SHOP_ID "
            + "AND T.SHOP_ID = B.SHOP_ID "
            + "AND B.SHOP_CODE = '{0}' "
            + "GROUP BY A.SHOP_ID,A.SHOP_NAME_ZH,B.SHOP_USER_ID ";
        public const string SELECT_RECORD_USER_BY_SCAN_CODE = ""
            + "SELECT * "
            + "FROM T_BUSS_PAY T,T_BASE_USER A "
            + "WHERE A.USER_ID = T.USER_ID "
            + "AND A.SCAN_CODE = '{0}' "
            + "AND PAY_STATE = 0 "
            + "AND PAY_TYPE = 0 "
            + "ORDER BY APPLY_TIME "
            + "LIMIT 1";
        public const string UPDATE_RECORD_SHOP_PAY_BY_SHOP_ID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET SHOP_PAY_TIME = NOW(), "
            + "PAY_STATE = 1, "
            + "SHOP_GATHER_GUID = '{1}' "
            + "WHERE SHOP_ID = {0} "
            + "AND PAY_STATE = 0";
        public const string INSERT_GATHER = ""
            + "INSERT INTO T_BUSS_GATHER "
            + "(GATHER_TIME,"
            + "SHOP_ID,"
            + "SHOP_USER_ID,"
            + "STAFF_ID,"
            + "SHOP_MONEY,"
            + "SHOP_RATE,"
            + "TOTAL,"
            + "GUID,"
            + "RECORD_COUNT) "
            + "VALUES(NOW(),{0},{1},{2},{3},'{4}',{5},'{6}',{7}) ";
        public const string SELECT_APPLY_LIST_BY_USER_ID = ""
            + "SELECT * "
            + "FROM T_BUSS_PAY T, T_BASE_USER A "
            + "WHERE PAY_TYPE = {1} "
            + "AND PAY_STATE = {0} "
            + "AND T.USER_ID = A.USER_ID ";
        public const string SELECT_RECORD_USER_SUM_BY_GUID = ""
             + "SELECT * "
             + "FROM T_BUSS_RECORD "
             + "WHERE USER_PAY_GUID = '{0}' AND USER_PAY_TYPE = 0";
        public const string SELECT_RECORD_SHOP_AGENT_SUM_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE SHOP_AGENT_PAY_GUID = '{0}' AND SHOP_AGENT_PAY_TYPE = 0";
        public const string SELECT_RECORD_USER_AGENT_SUM_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE USER_AGENT_PAY_GUID = '{0}' AND USER_AGENT_PAY_TYPE = 0";
        public const string SELECT_RECORD_USER_RMB_SUM_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE USER_PAY_GUID = '{0}' AND USER_PAY_TYPE = 1";
        public const string SELECT_RECORD_SHOP_AGENT_RMB_SUM_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE SHOP_AGENT_PAY_GUID = '{0}' AND SHOP_AGENT_PAY_TYPE = 1";
        public const string SELECT_RECORD_USER_AGENT_RMB_SUM_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_RECORD "
            + "WHERE USER_AGENT_PAY_GUID = '{0}' AND USER_AGENT_PAY_TYPE = 1";
        public const string SELECT_BUSS_PAY_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_PAY A, T_BASE_USER B "
            + "WHERE GUID = '{0}' "
            + "AND A.USER_ID = B.USER_ID";
        public const string SELECT_BUSS_PAY_BANKCARD_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_PAY A,T_BASE_BANKCARD B,T_BASE_USER C "
            + "WHERE GUID = '{0}' "
            + "AND A.PAY_BANKCARD_ID = B.BANKCARD_ID "
            + "AND C.USER_ID = A.USER_ID";
        public const string UPDATE_RECORD_USER_PAY_STATE_BY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET USER_PAY_TIME = NOW(), "
            + "USER_PAY_STATE = 2 "
            + "WHERE USER_PAY_GUID = '{0}' "
            + "AND USER_PAY_STATE = 1 "
            + "AND USER_PAY_TYPE = 0";
        public const string UPDATE_RECORD_SHOP_AGENT_PAY_STATE_BY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET SHOP_AGENT_PAY_TIME = NOW(), "
            + "SHOP_AGENT_PAY_STATE = 2 "
            + "WHERE SHOP_AGENT_PAY_GUID = '{0}' "
            + "AND SHOP_AGENT_PAY_STATE = 1 "
            + "AND SHOP_AGENT_PAY_TYPE = 0";
        public const string UPDATE_RECORD_USER_AGENT_PAY_STATE_BY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET USER_AGENT_PAY_TIME = NOW(), "
            + "USER_AGENT_PAY_STATE = 2 "
            + "WHERE USER_AGENT_PAY_GUID = '{0}' "
            + "AND USER_AGENT_PAY_STATE = 1 "
            + "AND USER_AGENT_PAY_TYPE = 0";
        public const string UPDATE_BUSS_PAY_STATE_BY_GUID = ""
            + "UPDATE T_BUSS_PAY "
            + "SET PAY_TIME = NOW(), "
            + "PAY_STATE = 1, "
            + "STAFF_ID = {1} "
            + "WHERE PAY_STATE = 0 "
            + "AND GUID = '{0}'"
            + "AND PAY_TYPE = 0";
        public const string UPDATE_USER_QRCODE = ""
            + "UPDATE T_BASE_USER "
            + "SET SCAN_CODE = '{0}' "
            + "WHERE USER_ID = {1} ";
        public const string UPDATE_SHOP_QRCODE = ""
            + "UPDATE T_BASE_SHOP_USER "
            + "SET SHOP_CODE = '{0}' "
            + "WHERE SHOP_USER_ID = {1} ";
    }
}
