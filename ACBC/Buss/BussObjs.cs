using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    #region Sys

    public class SessionUser
    {
        public string openid;
        public string checkCode;
        public string userType;
    }

    public class SmsCodeRes
    {
        public int error_code;
        public string reason;
    }

    #endregion

    #region Params

    public class LoginParam
    {
        public string code;
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

    public class UserRegParam
    {
        public string checkCode;
        public string agentCode;
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

    public class CheckCodeParam
    {
        public string phone;
    }

    public class ScanCodeParam
    {
        public string code;
    }

    public class SubmitParam
    {
        public string userId;
        public string shopId;
        public double total;
        public string ticketCode;
        public string ticketImg;
        public int inputState;
    }

    public class GetRecordParam
    {
        public string shopId;
    }

    public class GetShopInfoParam
    {
        public string shopId;
    }

    #endregion

    #region DaoObjs

    public class Shop
    {
        public string shopId;
        public string shopName;
        public double shopRate;
        public double userRate;
        public double shopAgentRate;
        public double userAgentRate;
        public double platformRate;
        public string shopCode;
    }

    public class User
    {
        public string userName;
        public string userId;
        public string openid;
        public string userImg;
    }

    public class ShopRecord
    {
        public string recordId;
        public string recordTime;
        public double total;
        public string userName;
        public string shopRate;
        public double shopMoney;
        public string recordCode;
        public string recordCodeImg;
        public string payState;
        public string inputState;
    }

    public class ShopUser
    {
        public string shopUserName;
        public string shopUserImg;
    }

    public class Agent
    {
        public string agentId;
        public string agentName;
    }

    public class ShopShow
    {
        public string shopId;
        public string shopName;
        public string shopAddr;
        public string shopPhone;
        public string shopListImg;
        public string userRate;
    }

    public class ShopInfo
    {
        public string shopName;
        public string shopDesc;
        public string shopInfoImg;
        public string shopAddr;
        public string shopPhone;
        public string userRate;
        public List<ShopBrands> brandsList;
    }

    public class ShopBrands
    {
        public string shopBrandsId;
        public string brandsName;
        public string brandsImg;
        public string goodsNum;
        public string recommend;
    }

    #endregion
}
