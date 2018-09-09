using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class ShopDao
    {
        public User GetUser(string code)
        {
            User user = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShopSqls.SELECT_USER_BY_CODE, code);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                user = new User
                {
                    userName = dt.Rows[0]["USER_NAME"].ToString(),
                    userId = (int)dt.Rows[0]["USER_ID"],
                };
            }

            return user;
        }
    }

    public class ShopSqls
    {
        public const string SELECT_USER_BY_CODE = ""
            + "SELECT * "
            + "FROM T_BASE_USER "
            + "WHERE SCAN_CODE = '{0}'";
    }

    public class User
    {
        public string userName;
        public int userId;
    }
}
