using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class ShopBuss
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

        //public object Do_Submit(BaseApi baseApi)
        //{

        //}
    }

    public class ScanCodeParam
    {
        public string code;
    }

    public class SubmitParam
    {
        public int userId;
        public double total;
        public string ticketCode;
        public string ticketImg;
        
    }
}
