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
            ScanCodeResult scanCodeResult = new ScanCodeResult();
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
            return true;
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
            + "AND B.SHOP_CODE = {0} "
            + "GROUP BY A.SHOP_ID,A.SHOP_NAME_ZH,B.SHOP_USER_ID ";
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
    }
}
