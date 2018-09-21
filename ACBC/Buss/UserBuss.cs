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
            List<ShopShow> listShop = userDao.GetShopShow();
            List<HomeImg> listHome = userDao.GetHomeImg();

            return new { listHome, listShop };
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

            return new
            {
                process = new {
                    recordStateList.processList,
                    recordStateList.process,
                    recordStateList.processMoney,
                    recordStateList.processTotal,
                },
                pay = new {
                    recordStateList.payList,
                    recordStateList.pay,
                    recordStateList.payMoney,
                    recordStateList.payTotal,
                },
                paid = new {
                    recordStateList.paidList,
                    recordStateList.paid,
                    recordStateList.paidMoney,
                    recordStateList.paidTotal,
                },
            };
        }

        public object Do_GetBankCard(BaseApi baseApi)
        {
            UserDao userDao = new UserDao();
            Bankcard bankcard = userDao.GetBankcardIfNullInsert(Utils.GetOpenID(baseApi.token));
            return bankcard;
        }

        public object Do_UpdateBankCard(BaseApi baseApi)
        {
            UpdateBankcardParam updateBankcardParam = JsonConvert.DeserializeObject<UpdateBankcardParam>(baseApi.param.ToString());
            if (updateBankcardParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            UserDao userDao = new UserDao();
            if (!userDao.UpdateBankcardByOpenId(updateBankcardParam, Utils.GetOpenID(baseApi.token)))
            {
                throw new ApiException(CodeMessage.UpdateBankcardError, "UpdateBankcardError");
            }
            return "";
        }
    }
}
