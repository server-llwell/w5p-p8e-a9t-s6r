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
    public class UserDao
    {
        public List<HomeImg> GetHomeImg()
        {
            List<HomeImg> list = new List<HomeImg>();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_HOME_IMG);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HomeImg homeImg = new HomeImg
                    {
                        homeImgId = dr["HOME_IMG_ID"].ToString(),
                        img = dr["IMG"].ToString(),
                        urlCode = dr["URL_CODE"].ToString(),
                    };
                    list.Add(homeImg);
                }   
            }

            return list;
        }

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
            User user;
            double sumMoney = 0;
            builder.AppendFormat(UserSqls.SELECT_USER_BY_USER_ID, userId);
            sql = builder.ToString();
            builder.Clear();
            dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                user = new User
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString(),
                    userImg = dt.Rows[0]["USER_IMG"].ToString(),
                    phone = dt.Rows[0]["USER_PHONE"].ToString(),
                };
                recordStateSum.userName = user.userName;
                recordStateSum.userImg = user.userImg;
                recordStateSum.phone = user.phone;
            }
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_MONEY"]),
                            payType = dr["USER_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_PAY_TIME"].ToString(),
                            payAddr = dr["USER_PAY_ADDR"].ToString(),
                            payState = dr["USER_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["SHOP_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["SHOP_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["SHOP_AGENT_PAY_TYPE"].ToString() == "0" ? 
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_AGENT_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["SHOP_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["SHOP_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["SHOP_AGENT_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_AGENT_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["SHOP_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["SHOP_AGENT_MONEY"]),
                            payType = dr["SHOP_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["SHOP_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["SHOP_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["SHOP_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["SHOP_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["SHOP_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["SHOP_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["SHOP_AGENT_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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
                            shopName = dr["SHOP_NAME_ZH"].ToString(),
                            recordTime = dr["RECORD_TIME"].ToString(),
                            total = Convert.ToDouble(dr["TOTAL"]),
                            rate = (Convert.ToDouble(dr["USER_AGENT_RATE"]) * 100) + "%",
                            money = Convert.ToDouble(dr["USER_AGENT_MONEY"]),
                            payType = dr["USER_AGENT_PAY_TYPE"].ToString(),
                            applyTime = dr["USER_AGENT_PAY_APPLY_TIME"].ToString(),
                            payTime = dr["USER_AGENT_PAY_TIME"].ToString(),
                            payAddr = dr["USER_AGENT_PAY_ADDR"].ToString(),
                            payState = dr["USER_AGENT_PAY_STATE"].ToString(),
                            payStateEx = dr["USER_AGENT_PAY_STATE"].ToString() == "0" ?
                                        "0" : (dr["USER_AGENT_PAY_STATE"].ToString() == "1" ?
                                        (dr["USER_AGENT_PAY_TYPE"].ToString() == "0" ?
                                        "1" : "2") : ""),
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

        public Bankcard GetBankcard(string openId)
        {
            Bankcard bankcard = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_BANKCARD_BY_OPENID, openId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt != null && dt.Rows.Count == 1)
            {
                bankcard = new Bankcard
                {
                    bankcardId = dt.Rows[0]["BANKCARD_ID"].ToString(),
                    bankcardCode = dt.Rows[0]["BANKCARD_CODE"].ToString(),
                    bankName = dt.Rows[0]["BANK_NAME"].ToString(),
                    subName = dt.Rows[0]["SUB_NAME"].ToString(),
                    bankcardUserName = dt.Rows[0]["BANKCARD_USER_NAME"].ToString(),
                };
            }

            return bankcard;
        }

        public Bankcard GetBankcardIfNullInsert(string openId)
        {
            Bankcard bankcard = GetBankcard(openId);

            if (bankcard == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(UserSqls.INSERT_BANKCARD_IF_NULL, openId);
                string sqlInsert = builder.ToString();
                builder.Clear();
                builder.AppendFormat(UserSqls.UPDATE_USER_BANKCARD_ID_BY_OPENID, openId);
                string sqlUpdate = builder.ToString();
                ArrayList list = new ArrayList();
                list.Add(sqlInsert);
                list.Add(sqlUpdate);
                DatabaseOperationWeb.ExecuteDML(list);
                return GetBankcardIfNullInsert(openId);
            }

            return bankcard;
        }

        public bool UpdateBankcardByOpenId(UpdateBankcardParam updateBankcardParam, string openId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                UserSqls.UPDATE_BANKCARD_BY_OPENID, 
                openId, 
                updateBankcardParam.bankcardCode, 
                updateBankcardParam.bankName, 
                updateBankcardParam.subName, 
                updateBankcardParam.bankcardUserName);
            string sqlUpdate = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sqlUpdate);
        }

        public Dictionary<string, ConfigItem> GetConfig()
        {
            Dictionary<string, ConfigItem> keyValues = new Dictionary<string, ConfigItem>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                        UserSqls.SELECT_CONFIG_ALL
                    );
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    ConfigItem item = new ConfigItem
                    {
                        configCode = dr["CONFIG_CODE"].ToString(),
                        configValue = dr["CONFIG_VALUE"].ToString(),
                        configDesc = dr["CONFIG_DESC"].ToString(),
                    };
                    keyValues.Add(item.configCode, item);
                }
            }

            return keyValues;
        }
        
        public bool UpdateUserApply(
            string userId, 
            string userType, 
            string payType, 
            string applyAddr, 
            string applyTime,
            string bankcardId,
            string guid)
        {
            StringBuilder builder = new StringBuilder();
            string sql;
            ArrayList list = new ArrayList();
            switch (userType)
            {
                case "0":
                    builder.AppendFormat(
                        UserSqls.UPDATE_RECORD_USER_PAY_GUID,
                        userId,
                        applyTime,
                        applyAddr,
                        guid,
                        payType
                    );
                    sql = builder.ToString();
                    list.Add(sql);
                    builder.Clear();
                    if (payType == "0")
                    {
                        builder.AppendFormat(
                            UserSqls.INSERT_BUSS_PAY_GUID,
                            guid,
                            applyTime,
                            userId,
                            applyAddr
                        );
                    }
                    else
                    {
                        builder.AppendFormat(
                            UserSqls.INSERT_BUSS_BANKCARD_PAY_GUID,
                            guid,
                            applyTime,
                            userId,
                            applyAddr,
                            bankcardId
                         );
                    }
                    sql = builder.ToString();
                    list.Add(sql);
                    return DatabaseOperationWeb.ExecuteDML(list);
                case "1":
                    builder.AppendFormat(
                        UserSqls.UPDATE_RECORD_SHOP_AGENT_PAY_GUID,
                        userId,
                        applyTime,
                        applyAddr,
                        guid,
                        payType
                    );
                    sql = builder.ToString();
                    list.Add(sql);
                    builder.Clear();
                    builder.AppendFormat(
                        UserSqls.UPDATE_RECORD_USER_AGENT_PAY_GUID,
                        userId,
                        applyTime,
                        applyAddr,
                        guid,
                        payType
                    );
                    sql = builder.ToString();
                    list.Add(sql);
                    builder.Clear();
                    if (payType == "0")
                    {
                        builder.AppendFormat(
                            UserSqls.INSERT_BUSS_PAY_GUID,
                            guid,
                            applyTime,
                            userId,
                            applyAddr
                        );
                    }
                    else
                    {
                        builder.AppendFormat(
                            UserSqls.INSERT_BUSS_BANKCARD_PAY_GUID,
                            guid,
                            applyTime,
                            userId,
                            applyAddr,
                            bankcardId
                         );
                    }
                    sql = builder.ToString();
                    list.Add(sql);
                    return DatabaseOperationWeb.ExecuteDML(list);
                default:
                    return false;
            }
        }

        public PayApply GetPayApply(string guid)
        {
            PayApply payApply = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_PAY_APPLY_BY_GUID, guid);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt != null && dt.Rows.Count == 1)
            {
                payApply = new PayApply
                {
                    money = dt.Rows[0]["MONEY"].ToString(),
                    applyAddr = dt.Rows[0]["ADDR"].ToString(),
                    payTime = dt.Rows[0]["PAY_TIME"].ToString(),
                };
            }

            return payApply;
        }
    }

    public class UserSqls
    {
        public const string SELECT_CONFIG_ALL = ""
            + "SELECT * "
            + "FROM T_BASE_CONFIG ";
        public const string SELECT_BANKCARD_BY_OPENID = ""
            + "SELECT * "
            + "FROM T_BASE_USER A,T_BASE_BANKCARD B "
            + "WHERE A.USER_BANKCARD_ID = B.BANKCARD_ID "
            + "AND A.OPENID = '{0}'";
        public const string INSERT_BANKCARD_IF_NULL = ""
            + "INSERT INTO T_BASE_BANKCARD"
            + "(OPENID,BANKCARD_CODE,BANK_NAME,SUB_NAME,BANKCARD_USER_NAME) "
            + "VALUES('{0}','','','','') ";
        public const string UPDATE_BANKCARD_BY_OPENID = ""
            + "UPDATE T_BASE_BANKCARD "
            + "SET BANKCARD_CODE = '{1}',"
            + "BANK_NAME = '{2}',"
            + "SUB_NAME = '{3}',"
            + "BANKCARD_USER_NAME = '{4}' "
            + "WHERE OPENID = '{0}' ";
        public const string UPDATE_USER_BANKCARD_ID_BY_OPENID = ""
            + "UPDATE T_BASE_USER "
            + "SET USER_BANKCARD_ID = "
            + "(SELECT BANKCARD_ID " 
            + "FROM T_BASE_BANKCARD " 
            + "WHERE OPENID = '{0}') "
            + "WHERE OPENID = '{0}'";
        public const string SELECT_HOME_IMG = ""
            + "SELECT * "
            + "FROM T_BASE_HOME_IMG "
            + "ORDER BY SORT";
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
        public const string SELECT_USER_BY_USER_ID = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE USER_ID = '{0}'";
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
        public const string UPDATE_RECORD_USER_PAY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET USER_PAY_STATE = 1, "
            + "USER_PAY_TYPE = {4}, "
            + "USER_PAY_APPLY_TIME = NOW(), "
            + "USER_PAY_TIME = STR_TO_DATE('{1}', '%Y-%m-%d %H'), "
            + "USER_PAY_ADDR = '{2}', "
            + "USER_PAY_GUID = '{3}' "
            + "WHERE PAY_STATE = 1 "
            + "AND USER_PAY_STATE = 0 "
            + "AND USER_ID = {0}"; 
        public const string UPDATE_RECORD_SHOP_AGENT_PAY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET SHOP_AGENT_PAY_STATE = 1, "
            + "SHOP_AGENT_PAY_TYPE = {4}, "
            + "SHOP_AGENT_PAY_APPLY_TIME = NOW(), "
            + "SHOP_AGENT_PAY_TIME = STR_TO_DATE('{1}', '%Y-%m-%d %H'), "
            + "SHOP_AGENT_PAY_ADDR = '{2}', "
            + "SHOP_AGENT_PAY_GUID = '{3}' "
            + "WHERE PAY_STATE = 1 "
            + "AND SHOP_AGENT_PAY_STATE = 0 "
            + "AND SHOP_ID IN(SELECT SHOP_ID FROM T_BASE_SHOP WHERE SHOP_AGENT = {0}) ";
        public const string UPDATE_RECORD_USER_AGENT_PAY_GUID = ""
            + "UPDATE T_BUSS_RECORD "
            + "SET USER_AGENT_PAY_STATE = 1, "
            + "USER_AGENT_PAY_TYPE = {4}, "
            + "USER_AGENT_PAY_APPLY_TIME = NOW(), "
            + "USER_AGENT_PAY_TIME = STR_TO_DATE('{1}', '%Y-%m-%d %H'), "
            + "USER_AGENT_PAY_ADDR = '{2}', "
            + "USER_AGENT_PAY_GUID = '{3}' "
            + "WHERE PAY_STATE = 1 "
            + "AND USER_AGENT_PAY_STATE = 0 "
            + "AND USER_ID IN(SELECT USER_ID FROM T_BASE_USER WHERE USER_AGENT = {0}) ";
        public const string INSERT_BUSS_BANKCARD_PAY_GUID = ""
            + "INSERT INTO T_BUSS_PAY "
            + "(PAY_TIME,APPLY_TIME,USER_ID,ADDR,PAY_TYPE,PAY_BANKCARD_ID,MONEY,PAY_STATE,GUID) "
            + "VALUES("
            + "STR_TO_DATE('{1}', '%Y-%m-%d %H'),"
            + "NOW(),"
            + "{2},"
            + "'{3}',"
            + "1,"
            + "{4},"
            + "(IFNULL((" + SELECT_RECORD_USER_SUM_BY_GUID
            + "),0)+IFNULL((" + SELECT_RECORD_SHOP_AGENT_SUM_BY_GUID
            + "),0)+IFNULL((" + SELECT_RECORD_USER_AGENT_SUM_BY_GUID + "),0)),"
            + "0,"
            + "'{0}')";
        public const string INSERT_BUSS_PAY_GUID = ""
            + "INSERT INTO T_BUSS_PAY "
            + "(PAY_TIME,APPLY_TIME,USER_ID,ADDR,PAY_TYPE,MONEY,PAY_STATE,GUID) "
            + "VALUES("
            + "STR_TO_DATE('{1}', '%Y-%m-%d %H'),"
            + "NOW(),"
            + "{2},"
            + "'{3}',"
            + "0,"
            + "(IFNULL((" + SELECT_RECORD_USER_SUM_BY_GUID
            + "),0)+IFNULL((" + SELECT_RECORD_SHOP_AGENT_SUM_BY_GUID
            + "),0)+IFNULL((" + SELECT_RECORD_USER_AGENT_SUM_BY_GUID + "),0)),"
            + "0,"
            + "'{0}')";
        public const string SELECT_RECORD_USER_SUM_BY_GUID = ""
            + "SELECT SUM(USER_MONEY) "
            + "FROM T_BUSS_RECORD "
            + "WHERE USER_PAY_GUID = '{0}'";
        public const string SELECT_RECORD_SHOP_AGENT_SUM_BY_GUID = ""
            + "SELECT SUM(SHOP_AGENT_MONEY) "
            + "FROM T_BUSS_RECORD "
            + "WHERE SHOP_AGENT_PAY_GUID = '{0}'";
        public const string SELECT_RECORD_USER_AGENT_SUM_BY_GUID = ""
            + "SELECT SUM(USER_AGENT_MONEY) "
            + "FROM T_BUSS_RECORD "
            + "WHERE USER_AGENT_PAY_GUID = '{0}'";
        public const string SELECT_PAY_APPLY_BY_GUID = ""
            + "SELECT * "
            + "FROM T_BUSS_PAY "
            + "WHERE GUID = '{0}'";
    }
}
