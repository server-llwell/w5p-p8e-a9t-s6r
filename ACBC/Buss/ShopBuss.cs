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
            Shop shop = shopDao.GetShopByOpenID(Utils.GetOpenID(baseApi.token), baseApi.lang);

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
            double shopMoney = Math.Round(submitParam.total * shop.shopRate, 2);
            double shopAgentMoney = Math.Round(submitParam.total * shop.shopAgentRate, 2);
            double userAgentMoney = Math.Round(submitParam.total * shop.userAgentRate, 2);
            double platformMoney = Math.Round(submitParam.total * shop.platformRate, 2);
            double userMoney = shopMoney - shopAgentMoney - userAgentMoney - platformMoney;
            if (!shopDao.InputRecord(
                submitParam, 
                shop.shopRate, 
                shop.userRate,
                shopMoney,
                Math.Abs(shopMoney),
                userMoney,
                Math.Abs(userMoney),
                shop.shopAgentRate,
                shop.userAgentRate,
                shopAgentMoney,
                userAgentMoney,
                Math.Abs(shopAgentMoney),
                Math.Abs(userAgentMoney),
                shop.platformRate,
                platformMoney,
                Math.Abs(platformMoney),
                Utils.GetOpenID(baseApi.token)
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

            List<ShopRecord> listUnPay = shopDao.GetRecordByShopIdAndPayState(getRecordParam.shopId, "0");
            List<ShopRecord> listPay = shopDao.GetRecordByShopIdAndPayState(getRecordParam.shopId, "1");
            List<ShopRecord> listAll = new List<ShopRecord>();
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

            foreach (ShopRecord shopRecord in listUnPay)
            {
                sumTotalUnPay += shopRecord.total;
                sumShopMoneyUnPay += shopRecord.shopMoney;
                if(shopRecord.inputState == "1")
                {
                    sumReturnTotalUnPay += shopRecord.total;
                }
            }

            foreach (ShopRecord shopRecord in listPay)
            {
                sumTotalPay += shopRecord.total;
                sumShopMoneyPay += shopRecord.shopMoney;
                if (shopRecord.inputState == "1")
                {
                    sumReturnTotalPay += shopRecord.total;
                }
            }

            foreach (ShopRecord shopRecord in listAll)
            {
                sumTotalAll += shopRecord.total;
                if (shopRecord.payState == "0")
                {
                    sumShopMoneyAll += shopRecord.shopMoney;
                }
                if (shopRecord.inputState == "1")
                {
                    sumReturnTotalAll += shopRecord.total;
                }
            }

            sumTotalUnPay = Math.Round(sumTotalUnPay,2);
            sumReturnTotalUnPay = Math.Round(sumReturnTotalUnPay, 2);
            sumShopMoneyUnPay = Math.Round(sumShopMoneyUnPay, 2);

            sumTotalPay = Math.Round(sumTotalPay, 2);
            sumReturnTotalPay = Math.Round(sumReturnTotalPay, 2);
            sumShopMoneyPay = Math.Round(sumShopMoneyPay, 2);

            sumTotalAll = Math.Round(sumTotalAll, 2);
            sumReturnTotalAll = Math.Round(sumReturnTotalAll, 2);
            sumShopMoneyAll = Math.Round(sumShopMoneyAll, 2);

            return new {
                all = new { listAll, sumTotalAll, sumReturnTotalAll, sumShopMoneyAll },
                unPay = new { listUnPay, sumTotalUnPay, sumReturnTotalUnPay, sumShopMoneyUnPay },
                pay = new { listPay, sumTotalPay, sumReturnTotalPay, sumShopMoneyPay },
            };
        }
    }


}
