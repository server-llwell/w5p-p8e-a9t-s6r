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

    public class ExchangeRes
    {
        public string reason;
        public ExchangeResult result;
        public int error_code;
    }
    public class ExchangeResult
    {
        public string update;
        public List<string[]> list;
    }

    public enum ScanCodeType
    {
        Shop,
        User,
        Null,
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

    public class StaffRegParam
    {
        public string staffCode;
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

    public class UpdateBankcardParam
    {
        public string bankcardCode;
        public string bankName;
        public string subName;
        public string bankcardUserName;
    }

    public class ApplyPayParam
    {
        public string payType;
    }

    public class UpdatePhoneParam
    {
        public string phone;
        public string checkCode;
    }

    public class GetPayRecordParam
    {
        public string shopId;
    }

    public class GetPaidRecordParam
    {
        public string guid;
    }

    public class ScanTheCodeParam
    {
        public string scanCode;
    }

    public class ShopPayParam
    {
        public string shopId;
        public string shopUserId;
        public string staffId;
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

    public class Staff
    {
        public string staffName;
        public string staffId;
        public string openid;
        public string staffImg;
        public string staffCode;
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

        public double processRmbMoney;
        public double payRmbMoney;
        public double paidRmbMoney;

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
        public string shopName;
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

    public class ConfigItem
    {
        public string configCode;
        public string configValue;
        public string configDesc;
    }

    public class PayApply
    {
        public string money;
        public string applyAddr;
        public string payTime;
    }

    public class RecordShopPaySum
    {
        public string shopId;
        public string shopName;
        public int num;
        public double sumTotal;
        public double sumShopMoney;
        public string shopRate;
        public string gatherTime;
        public string guid;
    }

    public class RecordShopPayItem
    {
        public string recordTime;
        public double total;
        public string shopRate;
        public double shopMoney;
    }

    public class ScanCodeResult
    {
        public string resultType;
        public string resultKey;
        public string resultTitle;
        public double resultMoney;
        public string resultUser;
    }

    #endregion
}
