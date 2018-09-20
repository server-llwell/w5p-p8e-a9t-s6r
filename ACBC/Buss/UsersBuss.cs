using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.Text;

namespace ACBC.Buss
{
    public class UsersBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UsersApi;
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
            string openID = Utils.GetOpenID(baseApi.token);
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
            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            SessionUser sessionUser = new SessionUser();
            sessionUser.openid = sessionBag.OpenId;
            sessionUser.userType = "SHOP";
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);

            SessionContainer.Update(sessionBag.Key, sessionBag);
            return "";
        }

        public object Do_CheckCode(BaseApi baseApi)
        {
            CheckCodeParam checkCodeParam = JsonConvert.DeserializeObject<CheckCodeParam>(baseApi.param.ToString());
            if (checkCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            
            string code = new Random().Next(999999).ToString().PadLeft(6, '0');
            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            SessionUser sessionUser = JsonConvert.DeserializeObject<SessionUser>(sessionBag.Name);
            if (sessionUser == null)
            {
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }
            sessionUser.checkCode = code;
            sessionUser.checkPhone = checkCodeParam.phone;
            sessionUser.userType = "USER";
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
            SessionContainer.Update(sessionBag.Key, sessionBag);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(Global.SMS_CODE_URL, checkCodeParam.phone, code);
            string url = builder.ToString();
            string res = Utils.GetHttp(url);

            SmsCodeRes smsCodeRes = JsonConvert.DeserializeObject<SmsCodeRes>(res);
            if (smsCodeRes == null || smsCodeRes.error_code != 0)
            {
                throw new ApiException(CodeMessage.SmsCodeError, (smsCodeRes == null ? "SmsCodeError" : smsCodeRes.reason));
            }

            return "";
        }

        public object Do_UserReg(BaseApi baseApi)
        {
            UserRegParam userRegParam = JsonConvert.DeserializeObject<UserRegParam>(baseApi.param.ToString());
            if (userRegParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            if (sessionBag == null || sessionBag.Name == null)
            {
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }
            SessionUser sessionUser = JsonConvert.DeserializeObject<SessionUser>(sessionBag.Name);
            if (sessionUser == null || 
                sessionUser.checkCode == null || 
                sessionUser.checkCode != userRegParam.checkCode || 
                sessionUser.checkPhone != userRegParam.phone)
            {
                throw new ApiException(CodeMessage.SmsCodeError, "SmsCodeError");
            }

            UsersDao usersDao = new UsersDao();
            string openID = Utils.GetOpenID(baseApi.token);
            var user = usersDao.GetUser(openID);
            if (user != null)
            {
                throw new ApiException(CodeMessage.UserExist, "UserExist");
            }
            user = usersDao.GetUserByPhone(userRegParam.phone);
            if (user != null)
            {
                throw new ApiException(CodeMessage.PhoneExist, "PhoneExist");
            }
            var agent = usersDao.GetAgent(userRegParam.agentCode, "1");
            if (agent == null)
            {
                throw new ApiException(CodeMessage.InvalidAgentCode, "InvalidAgentCode");
            }
            if (!usersDao.UserReg(userRegParam, openID, agent.agentId))
            {
                throw new ApiException(CodeMessage.BindShopError, "BindShopError");
            }
            sessionUser.openid = sessionBag.OpenId;
            sessionUser.userType = "USER";
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);

            SessionContainer.Update(sessionBag.Key, sessionBag);
            return "";
        }
    }


}
