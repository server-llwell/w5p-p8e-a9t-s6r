using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;

namespace ACBC.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public object Do_CheckSignature(BaseApi baseApi)
        {
            CheckSignatureParam checkSignatureParam = JsonConvert.DeserializeObject<CheckSignatureParam>(baseApi.param.ToString());
            if (checkSignatureParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var checkSuccess = EncryptHelper.CheckSignature(baseApi.token, checkSignatureParam.rawData, checkSignatureParam.signature);
            if (checkSuccess)
            {
                return new { check = checkSuccess };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, "校验失败");
            }
        }

        public object Do_DecodeEncryptedData(BaseApi baseApi)
        {
            DecodeEncryptedDataParam decodeEncryptedDataParam = JsonConvert.DeserializeObject<DecodeEncryptedDataParam>(baseApi.param.ToString());
            if (decodeEncryptedDataParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            DecodeEntityBase decodedEntity = null;
            switch (decodeEncryptedDataParam.type.ToUpper())
            {
                case "USERINFO"://wx.getUserInfo()
                    decodedEntity = EncryptHelper.DecodeUserInfoBySessionId(
                        baseApi.token,
                        decodeEncryptedDataParam.encryptedData, decodeEncryptedDataParam.iv);
                    break;
                default:
                    break;
            }
            //检验水印
            var checkWartmark = false;
            if (decodedEntity != null)
            {
                checkWartmark = decodedEntity.CheckWatermark(Global.APPID);
            }

            if (checkWartmark)
            {
                return new { check = checkWartmark };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, "校验失败");
            }
        }

        public object Do_BindShop(BaseApi baseApi)
        {
            BindShopParam bindShopParam = JsonConvert.DeserializeObject<BindShopParam>(baseApi.param.ToString());
            if (bindShopParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            UsersDao usersDao = new UsersDao();
            string openID = Global.GetOpenID(baseApi.token);
            var shopUser = usersDao.GetShopUser(openID);
            if(shopUser != null)
            {
                throw new ApiException(CodeMessage.BindShopUserExist, "BindShopUserExist");
            }
            var shop = usersDao.GetShopByCode(bindShopParam.shopCode);
            if(shop == null)
            {
                throw new ApiException(CodeMessage.BindShopInvalidCode, "BindShopInvalidCode");
            }
            if(!usersDao.BindShop(bindShopParam, openID, shop.shopId))
            {
                throw new ApiException(CodeMessage.BindShopError, "BindShopError");
            }
            return "";
        }
    }

    public class BindShopParam
    {
        public string shopCode;
        public string avatarUrl;
        public string city;
        public string country;
        public string gender;
        public string language;
        public string nickName;
        public string province;
    }

    public class CheckSignatureParam
    {
        public string rawData;
        public string signature;
    }

    public class DecodeEncryptedDataParam
    {
        public string type;
        public string encryptedData;
        public string iv;
    }
}
