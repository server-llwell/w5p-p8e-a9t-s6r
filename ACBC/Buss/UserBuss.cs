using ACBC.Common;
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

        public object Do_CheckSignature(object param)
        {
            CheckSignatureParam checkSignatureParam = JsonConvert.DeserializeObject<CheckSignatureParam>(param.ToString());
            if (checkSignatureParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var checkSuccess = EncryptHelper.CheckSignature(checkSignatureParam.token, checkSignatureParam.rawData, checkSignatureParam.signature);
            if (checkSuccess)
            {
                return new { check = checkSuccess };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, "校验失败");
            }
        }

        public object Do_DecodeEncryptedData(object param)
        {
            DecodeEncryptedDataParam decodeEncryptedDataParam = JsonConvert.DeserializeObject<DecodeEncryptedDataParam>(param.ToString());
            if (decodeEncryptedDataParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            DecodeEntityBase decodedEntity = null;
            switch (decodeEncryptedDataParam.type.ToUpper())
            {
                case "USERINFO"://wx.getUserInfo()
                    decodedEntity = EncryptHelper.DecodeUserInfoBySessionId(
                        decodeEncryptedDataParam.token,
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

    }

    

    public class CheckSignatureParam
    {
        public string token;
        public string rawData;
        public string signature;
    }

    public class DecodeEncryptedDataParam
    {
        public string token;
        public string type;
        public string encryptedData;
        public string iv;
    }
}
