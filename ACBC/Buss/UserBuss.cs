using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public object Do_GetShopShow(BaseApi baseApi)
        {
            UserDao userDao = new UserDao();
            List<ShopShow> list = userDao.GetShopShow();
            return list;
        }

        public object Do_GetShopInfo(BaseApi baseApi)
        {
            GetShopInfoParam getShopInfoParam = JsonConvert.DeserializeObject<GetShopInfoParam>(baseApi.param.ToString());
            if (getShopInfoParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            UserDao userDao = new UserDao();
            ShopInfo shopInfo = userDao.GetShopInfo(getShopInfoParam.shopId);
            if (shopInfo == null)
            {
                throw new ApiException(CodeMessage.InvalidShopInfo, "InvalidShopInfo");
            }
            return shopInfo;
        }

        public object Do_GetScanCode(BaseApi baseApi)
        {
            UserDao userDao = new UserDao();
            string scanCode = userDao.GetUserScanCode(Utils.GetOpenID(baseApi.token));
            return scanCode;
        }

        public object Do_GetMainInfo(BaseApi baseApi)
        {
            UserDao userDao = new UserDao();
            string openId = Utils.GetOpenID(baseApi.token);
            var user = userDao.GetUserByOpenID(openId);
            if (user == null)
            {
                throw new ApiException(CodeMessage.UserNotExist, "UserNotExist");
            }

            RecordStateSum recordStateSum = userDao.GetStateSum(user.userId, user.userType);
            
            return recordStateSum;
        }

        public object Do_GetMainList(BaseApi baseApi)
        {
            UserDao userDao = new UserDao();
            string openId = Utils.GetOpenID(baseApi.token);
            var user = userDao.GetUserByOpenID(openId);
            if (user == null)
            {
                throw new ApiException(CodeMessage.UserNotExist, "UserNotExist");
            }

            RecordStateList recordStateList = userDao.GetStateList(user.userId, user.userType);

            return recordStateList;
        }
    }
}
