﻿using Newtonsoft.Json;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.WxOpen.Containers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACBC.Common
{
    public class Utils
    {
        /// <summary>
        /// 获取系统已登录用户OPENID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetOpenID(string token)
        {
            SessionBag sessionBag = SessionContainer.GetSession(token);
            if (sessionBag == null)
            {
                return null;
            }
            return sessionBag.OpenId;
        }

        public static bool SetCache(string key, object value)
        {
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            var expiry = new TimeSpan(0, 0, 10);
            string valueStr = JsonConvert.SerializeObject(value);
            return db.StringSet(key, valueStr, expiry);
        }

        public static dynamic GetCache<T>(string key)
        {
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            if(db.StringGet(key).HasValue)
            {
                return JsonConvert.DeserializeObject<T>(db.StringGet(key));
            }
            return null;
        }

        public static bool DeleteCache(string key)
        {
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            if (db.StringGet(key).HasValue)
            {
                return db.KeyDelete(key);
            }
            return true;
        }

        public static string PostHttp(string url, string body, string contentType)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        public static string GetHttp(string url)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();

            return responseContent;
        }
    }
}
