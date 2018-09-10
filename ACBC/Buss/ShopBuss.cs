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
            return ApiType.UploadApi;
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
            Shop shop = shopDao.GetShop(submitParam.shopId, baseApi.lang);
            if(shop == null)
            {
                throw new ApiException(CodeMessage.InvalidShopId, "InvalidShopId");
            }

            //if (!shopDao.InputRecord(submitParam, shop.shopRate, shop.userRate, submitParam.total * ))
            //{
            //    throw new ApiException(CodeMessage.BindShopError, "BindShopError");
            //}
            return "";
        }
    }

    public class ScanCodeParam
    {
        public string code;
    }

    public class SubmitParam
    {
        public int userId;
        public int shopId;
        public double total;
        public string ticketCode;
        public string ticketImg;
        public int inputState;
    }
}
