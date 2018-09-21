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
        public string checkPhone;
        public string checkCode;
        public string userType;
    }

    public class SmsCodeRes
    {
        public int error_code;
        public string reason;
    }

    public class WsPayState
    {
        public string userId;
        public string scanCode;
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
        public string phone;
        public string userType;
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

    public class UpdateBankcardParam
    {
        public string bankcardCode;
        public string bankName;
        public string subName;
        public string bankcardUserName;
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
        public string phone;
        public string userType;
        public string scanCode;
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

    public class RecordStateSum
    {
        public int process;
        public int pay;
        public int paid;

        public double payMoney;

        public string userName;
        public string userImg;

        public string phone;
    }

    public class RecordStateList
    {
        public int process;
        public int pay;
        public int paid;

        public double processMoney;
        public double payMoney;
        public double paidMoney;

        public double processTotal;
        public double payTotal;
        public double paidTotal;

        public List<RecordState> processList = new List<RecordState>();
        public List<RecordState> payList = new List<RecordState>();
        public List<RecordState> paidList = new List<RecordState>();
    }

    public class RecordState
    {
        public string recordId;
        public string recordTime;
        public double total;
        public string rate;
        public double money;
        public string payType;
        public string applyTime;
        public string payTime;
        public string payAddr;
        public string payState;
        public string payStateEx;
    }

    public class HomeImg
    {
        public string homeImgId;
        public string img;
        public string urlCode;
    }

    public class Bankcard
    {
        public string bankcardId;
        public string bankcardCode;
        public string bankName;
        public string subName;
        public string bankcardUserName;
    }

    public class UserApply
    {
        public string money;
        public string applyAddr;
        public string applyTime;
    }
    #endregion
}
