using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Common
{
    /// <summary>
    /// API类型分组
    /// </summary>
    public enum ApiType
    {
        OpenApi,//完全开放API
        UsersApi,//条件开放的用户API
        UserApi,//代购用户API
        ShopApi,//店铺用户API
        StaffApi,//平台员工API
        UploadApi,//上传组件API
    }

    public enum CheckType
    {
        Open,
        Token,
        Sign,
    }

    public enum InputType
    {
        Header,
        Body,
    }

    public abstract class BaseApi
    {
        public string appId;
        public string lang;
        public string code;
        public string method;
        public string token;
        public string sign;
        public string nonceStr;
        public object param;

        public abstract CheckType GetCheckType();
        public abstract ApiType GetApiType();
        public abstract InputType GetInputType();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{2}\rmethod:{0}\rparam:{1}", method, param, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string rets = builder.ToString();
            return rets;
        }
    }

    /// <summary>
    /// Upload类API
    /// </summary>
    public class UploadApi : BaseApi
    {
        public override CheckType GetCheckType()
        {
            return CheckType.Open;
        }

        public override InputType GetInputType()
        {
            return InputType.Header;
        }

        public override ApiType GetApiType()
        {
            return ApiType.UploadApi;
        }

    }

    /// <summary>
    /// 完全开放
    /// </summary>
    public class OpenApi : BaseApi
    {
        public override CheckType GetCheckType()
        {
            return CheckType.Open;
        }

        public override InputType GetInputType()
        {
            return InputType.Body;
        }

        public override ApiType GetApiType()
        {
            return ApiType.OpenApi;
        }

    }

    /// <summary>
    /// 条件开放，所有用户接口
    /// </summary>
    public class UsersApi : BaseApi
    {

        public override CheckType GetCheckType()
        {
            return CheckType.Open;
        }

        public override InputType GetInputType()
        {
            return InputType.Body;
        }

        public override ApiType GetApiType()
        {
            return ApiType.UsersApi;
        }

    }

    /// <summary>
    /// 合作店铺接口
    /// </summary>
    public class ShopApi : BaseApi
    {

        public override CheckType GetCheckType()
        {
            return CheckType.Token;
        }

        public override InputType GetInputType()
        {
            return InputType.Body;
        }

        public override ApiType GetApiType()
        {
            return ApiType.ShopApi;
        }

    }

    /// <summary>
    /// 代购用户接口
    /// </summary>
    public class UserApi : BaseApi
    {

        public override CheckType GetCheckType()
        {
            return CheckType.Token;
        }

        public override InputType GetInputType()
        {
            return InputType.Body;
        }

        public override ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

    }

    /// <summary>
    /// 平台员工接口
    /// </summary>
    public class StaffApi : BaseApi
    {

        public override CheckType GetCheckType()
        {
            return CheckType.Token;
        }

        public override InputType GetInputType()
        {
            return InputType.Body;
        }

        public override ApiType GetApiType()
        {
            return ApiType.StaffApi;
        }

    }
    
}
