using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class UserDao
    {
        public List<ShopShow> GetShopShow()
        {
            List<ShopShow> list = new List<ShopShow>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_SHOP_SHOW);
            string sql = builder.ToString();

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    ShopShow shopShow = new ShopShow
                    {
                        shopId = dr["SHOP_ID"].ToString(),
                        shopName = dr["SHOP_NAME_ZH"].ToString(),
                        shopAddr = dr["SHOP_ADDR"].ToString(),
                        shopPhone = dr["SHOP_PHONE"].ToString(),
                        shopListImg = dr["SHOP_LIST_IMG"].ToString(),
                        userRate = ((double)dr["USER_RATE"] * 100) + "%",
                    };
                    list.Add(shopShow);
                }
                
            }

            return list;
        }

        public ShopInfo GetShopInfo(string shopId)
        {
            ShopInfo shopInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.SELECT_SHOP_INFO, shopId);
            string sql = builder.ToString();

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                shopInfo = new ShopInfo
                {
                    shopName = dt.Rows[0]["SHOP_NAME_ZH"].ToString(),
                    shopDesc = dt.Rows[0]["SHOP_DESC"].ToString(),
                    shopInfoImg = dt.Rows[0]["SHOP_INFO_IMG"].ToString(),
                    shopAddr = dt.Rows[0]["SHOP_ADDR"].ToString(),
                    shopPhone = dt.Rows[0]["SHOP_PHONE"].ToString(),
                    userRate = ((double)dt.Rows[0]["USER_RATE"] * 100) + "%",
                };

                builder.Clear();
                builder.AppendFormat(UserSqls.SELECT_SHOP_BRANDS, shopId);
                sql = builder.ToString();
                dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
                if (dt != null)
                {
                    List<ShopBrands> list = new List<ShopBrands>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ShopBrands shopBrands = new ShopBrands
                        {
                            shopBrandsId = dr["SHOP_BRANDS_ID"].ToString(),
                            brandsName = dr["BRANDS_NAME"].ToString(),
                            brandsImg = dr["BRANDS_IMG"].ToString(),
                            goodsNum = dr["GOODS_NUM"].ToString(),
                            recommend = dr["RECOMMEND"].ToString(),
                        };
                        list.Add(shopBrands);
                    }
                    shopInfo.brandsList = list;
                }
            }

            return shopInfo;
        }

        public string GetUserScanCode(string openId)
        {
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(openId + new Random().Next()));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(UserSqls.UPDATE_USER_QRCODE_BY_OPENID, scanCode, openId);
            string sqlUpdate = builder.ToString();

            if(DatabaseOperationWeb.ExecuteDML(sqlUpdate))
            {
                return scanCode;
            }
            else
            {
                return "";
            }
        }
    }

    public class UserSqls
    {
        public const string SELECT_SHOP_SHOW = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP "
            + "ORDER BY PAY_SORT";
        public const string SELECT_SHOP_INFO = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP "
            + "WHERE SHOP_ID = {0}";
        public const string SELECT_SHOP_BRANDS = ""
            + "SELECT * "
            + "FROM T_BASE_SHOP_BRANDS "
            + "WHERE SHOP_ID = {0} "
            + "ORDER BY PAY_SORT";
        public const string UPDATE_USER_QRCODE_BY_OPENID = ""
            + "UPDATE T_BASE_USER "
            + "SET SCAN_CODE = '{0}' "
            + "WHERE OPENID = '{1}'";
    }
}
