using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Common
{
    /// <summary>
    /// 返回信息对照
    /// </summary>
    public enum CodeMessage
    {
        OK = 0,
        PostNull = -1,

        AppIDError = 201,
        SignError = 202,

        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        PaymentError = 3000,
        PaymentTotalPriceZero=3001,
        PaymentMsgError = 3002,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,
        InterfaceRole = 4003,//接口权限不足
        InterfaceValueError = 4004,//接口的参数不对
        InterfaceDBError=4005,//接口数据库操作失败

        BindShopUserExist = 10001,
        BindShopInvalidCode = 10002,
        BindShopError = 10003,

        InvalidShopUser = 10101,
        InvalidShopId = 10102,

        InvalidScanCode = 10201,
        UploadOSSError = 10202,

        SmsCodeError = 10301,
        UserExist = 10302,
        InvalidAgentCode = 10303,
        PhoneExist = 10304,
        UpdatePhoneError = 10305,
        RegUserError = 10306,

        InvalidShopInfo = 10401,
        UserNotExist = 10402,
        UpdateBankcardError = 10403,
        NeedBankcardFirst = 10404,
        ApplyRecordError = 10405,

        StaffExist = 10501,
        RegStaffError = 10502,
        InvalidStaffCode = 10503,

        ScanCodeNoData = 10601,
        ShopPayError = 10602,
        StaffNotExist = 10603,
        InvalidPayType = 10604,
        InvalidGuidOrPayType = 10605,
    }
}
