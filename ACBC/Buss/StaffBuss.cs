using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class StaffBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.StaffApi;
        }

        public object Do_GetGatherList(BaseApi baseApi)
        {
            StaffDao staffDao = new StaffDao();
            List<RecordShopPaySum> payList = staffDao.GetRecordShopPayList();
            List<RecordShopPaySum> paidList = staffDao.GetGatherList(20);

            int payCount = 0;
            double paySumTotal = 0;
            double paySumMoney = 0;

            int paidCount = 0;
            double paidSumTotal = 0;
            double paidSumMoney = 0;

            foreach (RecordShopPaySum recordShopPaySum in payList)
            {
                payCount += recordShopPaySum.num;
                paySumTotal += recordShopPaySum.sumTotal;
                paySumMoney += recordShopPaySum.sumShopMoney;
            }

            foreach (RecordShopPaySum recordShopPaySum in paidList)
            {
                paidCount += recordShopPaySum.num;
                paidSumTotal += recordShopPaySum.sumTotal;
                paidSumMoney += recordShopPaySum.sumShopMoney;
            }

            return new {
                pay = new { payCount, paySumTotal, paySumMoney, payList },
                paid = new { paidCount, paidSumTotal, paidSumMoney, paidList }
            };
        }

        public object Do_GetPayRecordList(BaseApi baseApi)
        {
            GetPayRecordParam getPayRecordParam = JsonConvert.DeserializeObject<GetPayRecordParam>(baseApi.param.ToString());
            if (getPayRecordParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StaffDao staffDao = new StaffDao();
            List<RecordShopPayItem> payList = staffDao.GetRecordShopPayItemListByShopId(getPayRecordParam.shopId);

            int payCount = 0;
            double paySumTotal = 0;
            double paySumMoney = 0;

            foreach (RecordShopPayItem recordShopPayItem in payList)
            {
                payCount ++;
                paySumTotal += recordShopPayItem.total;
                paySumMoney += recordShopPayItem.shopMoney;
            }

            return new
            {
                pay = new { payCount, paySumTotal, paySumMoney, payList }
            };
        }

        public object Do_GetPaidRecordList(BaseApi baseApi)
        {
            GetPaidRecordParam getPaidRecordParam = JsonConvert.DeserializeObject<GetPaidRecordParam>(baseApi.param.ToString());
            if (getPaidRecordParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StaffDao staffDao = new StaffDao();
            List<RecordShopPayItem> paidList = staffDao.GetRecordShopPayItemListByGuid(getPaidRecordParam.guid);

            int paidCount = 0;
            double paidSumTotal = 0;
            double paidSumMoney = 0;

            foreach (RecordShopPayItem recordShopPayItem in paidList)
            {
                paidCount++;
                paidSumTotal += recordShopPayItem.total;
                paidSumMoney += recordShopPayItem.shopMoney;
            }

            return new
            {
                paid = new { paidCount, paidSumTotal, paidSumMoney, paidList }
            };
        }

        public object Do_ScanTheCode(BaseApi baseApi)
        {
            ScanTheCodeParam scanTheCodeParam = JsonConvert.DeserializeObject<ScanTheCodeParam>(baseApi.param.ToString());
            if (scanTheCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            StaffDao staffDao = new StaffDao();
            ScanCodeType scanCodeType = staffDao.GetScanCodeType(scanTheCodeParam.scanCode);
            switch(scanCodeType)
            {
                case ScanCodeType.User:
                    return GetUserPayInfo(scanTheCodeParam.scanCode);
                case ScanCodeType.Shop:
                    return GetShopPayInfo(scanTheCodeParam.scanCode);
                case ScanCodeType.Null:
                    throw new ApiException(CodeMessage.InvalidScanCode, "InvalidScanCode");
                default:
                    throw new ApiException(CodeMessage.InvalidScanCode, "InvalidScanCode");
            }
        }

        private object GetShopPayInfo(string scanCode)
        {
            StaffDao staffDao = new StaffDao();
            ScanCodeResult scanCodeResult = staffDao.GetShopPayInfo(scanCode);
            if(scanCodeResult == null)
            {
                throw new ApiException(CodeMessage.ScanCodeNoData, "ScanCodeNoData");
            }
            return scanCodeResult;
        }

        private object GetUserPayInfo(string scanCode)
        {
            StaffDao staffDao = new StaffDao();
            ScanCodeResult scanCodeResult = staffDao.GetUserPayInfo(scanCode);
            if (scanCodeResult == null)
            {
                throw new ApiException(CodeMessage.ScanCodeNoData, "ScanCodeNoData");
            }
            return scanCodeResult;
        }

        public object Do_ShopPay(BaseApi baseApi)
        {
            ShopPayParam shopPayParam = JsonConvert.DeserializeObject<ShopPayParam>(baseApi.param.ToString());
            if (shopPayParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StaffDao staffDao = new StaffDao();
            Staff staff = staffDao.GetStaffByOpenID(Utils.GetOpenID(baseApi.token));
            if(staff == null)
            {
                throw new ApiException(CodeMessage.StaffNotExist, "StaffNotExist");
            }
            if (!staffDao.ShopPay(shopPayParam.shopId, shopPayParam.shopUserId, staff.staffId))
            {
                throw new ApiException(CodeMessage.ShopPayError, "ShopPayError");
            }

            return "";
        }

        public object Do_GetPayList(BaseApi baseApi)
        {
            GetApplyListParam getApplyListParam = JsonConvert.DeserializeObject<GetApplyListParam>(baseApi.param.ToString());
            if (getApplyListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            StaffDao staffDao = new StaffDao();
            List<PayItem> applyList;
            int applyCount = 0;
            double applyMoney = 0;
            double applyTotal = 0;
            List<PayItem> payList;
            int payCount = 0;
            double payMoney = 0;
            double payTotal = 0;
            switch (getApplyListParam.payType)
            {
                case "0":
                    applyList = staffDao.GetPayList(getApplyListParam.userId, "0", getApplyListParam.payType);
                    payList = staffDao.GetPayList(getApplyListParam.userId, "1", getApplyListParam.payType);

                    foreach (PayItem payItem in applyList)
                    {
                        applyCount += payItem.count;
                        applyMoney += payItem.money;
                        applyTotal += payItem.total;
                    }

                    foreach (PayItem payItem in payList)
                    {
                        payCount += payItem.count;
                        payMoney += payItem.money;
                        payTotal += payItem.total;
                    }

                    return new
                    {
                        apply = new { applyCount, applyMoney, applyTotal, applyList },
                        pay = new { payCount, payMoney, payTotal, payList }
                    };
                case "1":
                    applyList = staffDao.GetPayList(getApplyListParam.userId, "0", getApplyListParam.payType);
                    payList = staffDao.GetPayList(getApplyListParam.userId, "1", getApplyListParam.payType);

                    foreach (PayItem payItem in applyList)
                    {
                        applyCount += payItem.count;
                        applyMoney += payItem.money;
                        applyTotal += payItem.total;
                    }

                    foreach (PayItem payItem in payList)
                    {
                        payCount += payItem.count;
                        payMoney += payItem.money;
                        payTotal += payItem.total;
                    }

                    return new
                    {
                        apply = new { applyCount, applyMoney, applyTotal, applyList },
                        pay = new { payCount, payMoney, payTotal, payList }
                    };
                default:
                    throw new ApiException(CodeMessage.InvalidPayType, "InvalidPayType");
            }
            
        }

        public object Do_GetUserPayRecordList(BaseApi baseApi)
        {
            GetUserPayRecordListParam getUserPayRecordListParam = JsonConvert.DeserializeObject<GetUserPayRecordListParam>(baseApi.param.ToString());
            if (getUserPayRecordListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StaffDao staffDao = new StaffDao();
            List<RecordUserPayItem> list = staffDao.GetPayRecordList(
                getUserPayRecordListParam.guid, 
                getUserPayRecordListParam.payType);

            if(list == null || list.Count == 0)
            {
                throw new ApiException(CodeMessage.InvalidGuidOrPayType, "InvalidGuidOrPayType");
            }

            switch(getUserPayRecordListParam.payType)
            {
                case "0":
                    return staffDao.GetPayInfo(getUserPayRecordListParam.guid, list);
                case "1":
                    return staffDao.GetBankcardInfo(getUserPayRecordListParam.guid, list);
                default:
                    throw new ApiException(CodeMessage.InvalidPayType, "InvalidPayType");
            }
        }
    }
}
