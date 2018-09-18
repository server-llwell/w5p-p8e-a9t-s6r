﻿using ACBC.Common;
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
                if(shopUser != null)
                {
                    SessionUser sessionUser = new SessionUser();
                    sessionUser.openid = sessionBag.OpenId;
                    sessionUser.userType = "SHOP";
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);

                    return new { token = sessionBag.Key, isReg = true, shopUserName = shopUser.shopUserName, shopUserImg = shopUser.shopUserImg };
                }
                else
                {
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

            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                AccessTokenContainer.Register(Global.APPID, Global.APPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                sessionBag.ExpireTime = DateTime.Now.AddSeconds(7200);
                SessionContainer.Update(sessionBag.Key, sessionBag);
                UsersDao usersDao = new UsersDao();
                var user = usersDao.GetUser(jsonResult.openid);
                SessionUser sessionUser = new SessionUser();
                if (user != null)
                {
                    sessionUser.openid = sessionBag.OpenId;
                    sessionUser.userType = "USER";
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);

                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new {
                        token = sessionBag.Key,
                        isReg = true,
                        userId = user.userId,
                        userName = user.userName,
                        userImg = user.userImg,
                    };
                }
                else
                {
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
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
