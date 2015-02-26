using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GoDutch.Repository
{
    public static class SqlExtensions
    {
        public static string GetStringValue(this SqlDataReader reader, string fieldName, string defaultValue = "")
        {
            return reader[fieldName] as string ?? defaultValue;
        }

        public static string GetStringValue(this SqlDataReader reader, int ordinal, string defaultValue = "")
        {
            return reader[ordinal] as string ?? defaultValue;
        }

        public static bool GetBoolValue(this SqlDataReader reader, string fieldName, bool defaultValue = false)
        {
            return reader[fieldName] as bool? ?? defaultValue;
        }

        public static bool GetBoolValue(this SqlDataReader reader, int ordinal, bool defaultValue = false)
        {
            return reader[ordinal] as bool? ?? defaultValue;
        }

        public static int GetIntValue(this SqlDataReader reader, string fieldName, int defaultValue = 0)
        {
            return reader[fieldName] as int? ?? defaultValue;
        }

        public static int GetIntValue(this SqlDataReader reader, int ordinal, int defaultValue = 0)
        {
            return reader[ordinal] as int? ?? defaultValue;
        }

        public static Guid GetGuidValue(this SqlDataReader reader, string fieldName, Guid defaultValue = new Guid())
        {
            return reader[fieldName] as Guid? ?? defaultValue;
        }

        public static Guid GetGuidValue(this SqlDataReader reader, int ordinal, Guid defaultValue = new Guid())
        {
            return reader[ordinal] as Guid? ?? defaultValue;
        }

        public static T Get<T>(this SqlDataReader reader, string fieldName, T defaultValue = default(T))
        {
            Object obj = reader[fieldName];
            if (obj == DBNull.Value) return defaultValue;
            return (T)obj;
        }

        public static T Get<T>(this SqlDataReader reader, int ordinal, T defaultValue = default(T))
        {
            Object obj = reader[ordinal];
            if (obj == DBNull.Value) return defaultValue;
            return (T)obj;
        }

        public static object DbNullIfEmpty(this Guid guid)
        {
            return guid == Guid.Empty ? (object)DBNull.Value : (Guid?)guid;
        }

        public static string TrimToDbNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? DBNull.Value.ToString() : text.Trim();
        }

        public static string ToStringWithParameterValues(this SqlCommand cmd)
        {
            return cmd.Parameters
                        .Cast<SqlParameter>()
                        .Aggregate(cmd.CommandText, (current, p) => current.Replace(p.ParameterName, p.Value.ToString()));

        }
    }
}