using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class ShopBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.ShopApi;
        }

        public object Do_ScanCode(BaseApi baseApi)
        {
            ScanCodeParam scanCodeParam = JsonConvert.DeserializeObject<ScanCodeParam>(baseApi.param.ToString());
            if (scanCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            ShopDao shopDao = new ShopDao();
            User user =  shopDao.GetUser(scanCodeParam.code);
            if(user == null)
            {
                throw new ApiException(CodeMessage.InvalidScanCode, "InvalidScanCode");
            }
            return user;
        }

        public object Do_GetShop(BaseApi baseApi)
        {
            ShopDao shopDao = new ShopDao();
            Shop shop = shopDao.GetShopByOpenID(Global.GetOpenID(baseApi.token), baseApi.lang);

            if (shop == null)
            {
                throw new ApiException(CodeMessage.InvalidShopUser, "InvalidShopUser");
            }
            return shop;
        }

        public object Do_Submit(BaseApi baseApi)
        {
            SubmitParam submitParam = JsonConvert.DeserializeObject<SubmitParam>(baseApi.param.ToString());
            if (submitParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            ShopDao shopDao = new ShopDao();
            Shop shop = shopDao.GetShop(submitParam.shopId);
            if(shop == null)
            {
                throw new ApiException(CodeMessage.InvalidShopId, "InvalidShopId");
            }

            //string fileUrl = OssManager.UploadFileToOSS(submitParam.ticketImg, Global.OssDir, submitParam.ticketImg);
            //if(fileUrl == "")
            //{
            //    throw new ApiException(CodeMessage.UploadOSSError, "UploadOSSError");
            //}

            //submitParam.ticketImg = fileUrl;
            if(submitParam.inputState == 1)
            {
                submitParam.total = -submitParam.total;
            }
            if (!shopDao.InputRecord(
                submitParam, 
                shop.shopRate, 
                shop.userRate, 
                Math.Round(submitParam.total * shop.shopRate, 2),
                Math.Abs(Math.Round(submitParam.total * shop.shopRate, 2)),
                Math.Round(submitParam.total * shop.userRate, 2),
                Math.Abs(Math.Round(submitParam.total * shop.userRate, 2))
                ))
            {
                throw new ApiException(CodeMessage.BindShopError, "BindShopError");
            }
            return "";
        }

        public object Do_GetRecord(BaseApi baseApi)
        {
            GetRecordParam getRecordParam = JsonConvert.DeserializeObject<GetRecordParam>(baseApi.param.ToString());
            if (getRecordParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            ShopDao shopDao = new ShopDao();

            List<Record> listUnPay = shopDao.GetRecordByShopIdAndPayState(getRecordParam.shopId, "0");
            List<Record> listPay = shopDao.GetRecordByShopIdAndPayState(getRecordParam.shopId, "1");
            List<Record> listAll = new List<Record>();
            listAll.AddRange(listUnPay);
            listAll.AddRange(listPay);

            double sumTotalUnPay = 0;
            double sumReturnTotalUnPay = 0;
            double sumShopMoneyUnPay = 0;

            double sumTotalPay = 0;
            double sumReturnTotalPay = 0;
            double sumShopMoneyPay = 0;

            double sumTotalAll = 0;
            double sumReturnTotalAll = 0;
            double sumShopMoneyAll = 0;

            foreach (Record record in listUnPay)
            {
                sumTotalUnPay += record.total;
                sumShopMoneyUnPay += record.shopMoney;
                if(record.inputState == "1")
                {
                    sumReturnTotalUnPay += record.total;
                }
            }

            foreach (Record record in listPay)
            {
                sumTotalPay += record.total;
                sumShopMoneyPay += record.shopMoney;
                if (record.inputState == "1")
                {
                    sumReturnTotalPay += record.total;
                }
            }

            foreach (Record record in listAll)
            {
                sumTotalAll += record.total;
                if (record.payState == "0")
                {
                    sumShopMoneyAll += record.shopMoney;
                }
                if (record.inputState == "1")
                {
                    sumReturnTotalAll += record.total;
                }
            }

            return new {
                all = new { listAll, sumTotalAll, sumReturnTotalAll, sumShopMoneyAll },
                unPay = new { listUnPay, sumTotalUnPay, sumReturnTotalUnPay, sumShopMoneyUnPay },
                pay = new { listPay, sumTotalPay, sumReturnTotalPay, sumShopMoneyPay },
            };
        }
    }

    public class ScanCodeParam
    {
        public string code;
    }

    public class SubmitParam
    {
        public string userId;
        public string shopId;
        public double total;
        public string ticketCode;
        public string ticketImg;
        public int inputState;
    }

    public class GetRecordParam
    {
        public string shopId;
    }

}
