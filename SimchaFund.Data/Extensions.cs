using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public static class Extensions
    {
        public static T GetOrNull<T>(this DbDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default;
            }

            return (T)value;
        }
    }
}
