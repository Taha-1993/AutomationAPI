using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;

namespace Service.Helpers
{
    public static class Utilities
    {
        public static dynamic ToExpando(this IDataRecord record)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < record.FieldCount; i++)
            {
                expandoObject.Add(record.GetName(i), Convert.IsDBNull(record[i]) ? null : record[i]);
            }

            return expandoObject;
        }

        public static IEnumerable<dynamic> Get(string connectionString, string storedProcedure, List<SqlParameter> sqlparams = default(List<SqlParameter>))
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var sqlCommand = new SqlCommand(storedProcedure, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    if (sqlparams != default(List<SqlParameter>))
                    {
                        sqlCommand.Parameters.AddRange(sqlparams.ToArray());
                    }

                    sqlCommand.Connection.Open();

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        sqlCommand.Parameters.Clear();
                        while (reader.Read())
                        {
                            yield return reader.ToExpando();
                        }
                    }
                }
            }
        }

        public static int UpsertHelper(string connectionString, string storedProcedure, List<SqlParameter> sqlParams = default(List<SqlParameter>))
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                SqlTransaction sqlTransaction = default(SqlTransaction);
                int numberOfRowsAffected = default(int);

                try
                {
                    sqlConnection.Open();
                    sqlTransaction = sqlConnection.BeginTransaction();

                    using (SqlCommand sqlCommand = new SqlCommand(storedProcedure, sqlConnection, sqlTransaction))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        if (sqlParams != default(List<SqlParameter>))
                        {
                            sqlCommand.Parameters.AddRange(sqlParams.ToArray());
                        }

                        numberOfRowsAffected = sqlCommand.ExecuteScalar().ToInt();
                    }

                    sqlTransaction.Commit();
                    return numberOfRowsAffected;
                }
                catch (Exception e)
                {
                    try
                    {
                        if (sqlTransaction != default(SqlTransaction)) sqlTransaction.Rollback();
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        public static string ToNullableString(this object input)
        {
            string result = default(string);
            try
            {
                if (input != DBNull.Value)
                {
                    result = input.ToString();
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static double ToDouble(this string input)
        {
            double result = default(double);
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    result = Convert.ToDouble(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }
        public static double ToDouble(this object input)
        {
            double result = default(double);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToDouble(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static decimal ToDecimal(this object input)
        {
            decimal result = default(decimal);
            try
            {
                if (input != default(object))
                {
                    result = (input != null) ? Convert.ToDecimal(input) : result;
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static DateTime? ToNullableDateTime(this object input)
        {
            DateTime? result = default(DateTime?);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToDateTime(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static double? ToNullableDouble(this object input)
        {
            double? result = default(double?);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToDouble(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static decimal? ToNullableDecimal(this object input, int precision = -1)
        {
            decimal? result = default(decimal?);
            try
            {
                if (input != default(object))
                {
                    if (precision != -1)
                    {
                        result = decimal.Round(Convert.ToDecimal(input), precision, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        result = Convert.ToDecimal(input);
                    }
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static short? ToNullableInt16(this object input)
        {
            short? result = default(short?);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToInt16(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static int? ToNullableInt32(this object input)
        {
            int? result = default(int?);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToInt32(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static int ToInt(this object input)
        {
            int result = default(int);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToInt32(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static bool ToBoolean(this object input)
        {
            bool result = default(bool);
            try
            {
                if (input != default(object))
                {
                    result = Convert.ToBoolean(input);
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public static object ToDBNull(this bool? flag)
        {
            if (flag == default(bool?))
                return DBNull.Value;
            return flag;
        }


        public static object ToDBNull(this string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return DBNull.Value;
            return str;
        }

        public static object ToDBNull(this short? integer)
        {
            if (integer == default(short?))
                return DBNull.Value;
            return integer;
        }

        public static object ToDBNull(this int? integer)
        {
            if (integer == default(int?))
                return DBNull.Value;
            return integer;
        }

        public static object ToDBNull(this decimal? amount)
        {
            if (amount == default(decimal?))
                return DBNull.Value;
            return amount;
        }
        public static object ToDBNull(this double? amount)
        {
            if (amount == default(double?))
                return DBNull.Value;
            return amount;
        }
        public static object ToDBNull(this DateTime? dateTime)
        {
            if (dateTime == default(DateTime?))
                return DBNull.Value;
            return dateTime;
        }

        public static object ToDBNull(this DateTime dateTime)
        {
            if (dateTime == default(DateTime))
                return DBNull.Value;
            return dateTime;
        }
    }
}
