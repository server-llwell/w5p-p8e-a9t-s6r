using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class OpenBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OpenApi;
        }

        public object Do_Login(BaseApi baseApi)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(baseApi.param.ToString());
            if (loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                AccessTokenContainer.Register(Global.APPID, Global.APPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                
                UsersDao usersDao = new UsersDao();
                var shopUser = usersDao.GetShopUser(jsonResult.openid);
                SessionUser sessionUser = new SessionUser();
                if (shopUser != null)
                {
                    sessionUser.userType = "SHOP";
                    sessionUser.openid = sessionBag.OpenId;
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = true, shopUserName = shopUser.shopUserName, shopUserImg = shopUser.shopUserImg };
                }
                else
                {
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = false };
                }
                
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }

        public object Do_UserLogin(BaseApi baseApi)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(baseApi.param.ToString());
            if (loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.USERAPPID, Global.USERAPPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                AccessTokenContainer.Register(Global.USERAPPID, Global.USERAPPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                sessionBag.ExpireTime = DateTime.Now.AddSeconds(7200);
                SessionContainer.Update(sessionBag.Key, sessionBag);
                UsersDao usersDao = new UsersDao();
                var user = usersDao.GetUser(jsonResult.openid);
                SessionUser sessionUser = new SessionUser();
                if (user != null)
                {
                    switch (user.userType)
                    {
                        case "0":
                            sessionUser.userType = "USER";
                            break;
                        case "1":
                            sessionUser.userType = "AGENT";
                            break;
                        default:
                            sessionUser.userType = "USER";
                            break;
                    }

                    sessionUser.openid = sessionBag.OpenId;
                    
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);

                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new {
                        token = sessionBag.Key,
                        isReg = true,
                        userId = user.userId,
                        userName = user.userName,
                        userImg = user.userImg,
                        userType = user.userType,
                    };
                }
                else
                {
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = false };
                }

            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }

        public object Do_StaffLogin(BaseApi baseApi)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(baseApi.param.ToString());
            if (loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.STAFFAPPID, Global.STAFFAPPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                AccessTokenContainer.Register(Global.STAFFAPPID, Global.STAFFAPPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);

                UsersDao usersDao = new UsersDao();
                Staff staff = usersDao.GetStaff(jsonResult.openid);
                SessionUser sessionUser = new SessionUser();
                if (staff != null)
                {
                    sessionUser.openid = sessionBag.OpenId;
                    sessionUser.userType = "STAFF";
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = true, staffName = staff.staffName, staffImg = staff.staffImg };
                }
                else
                {
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = false };
                }

            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }
    }

    
}
