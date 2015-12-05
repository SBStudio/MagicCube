using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public interface IData : IParser<Dictionary<string, object>>
	{
		Dictionary<string, object> dataDict { get; }
	}

	public sealed class DataSystem<T> where T : IData, new()
	{
		public static string[] fields { get; private set; }
		public static string keyField { get; private set; }
		public static string table { get; private set; }

		private static Dictionary<object, T> s_DataDict = new Dictionary<object, T>();
		private static SqliteUtil s_SqliteUtil;

		public static void Init(string database, string table, string[] fields, Type[] types)
		{
			DataSystem<T>.table = table;
			DataSystem<T>.fields = fields;
			DataSystem<T>.keyField = fields[0];

			s_SqliteUtil = new SqliteUtil(database);
			s_SqliteUtil.Create(table, fields, types);

			SqliteDataReader reader = s_SqliteUtil.GetAll(table);
			while (reader.Read())
			{
				T dataInfo = new T();
				dataInfo.Parse(Parse(reader));
				
				s_DataDict[dataInfo.dataDict[keyField]] = dataInfo;
			}
		}

		public static T Get(object key)
		{
			if (!s_DataDict.ContainsKey(key))
			{
				return default(T);
			}

			return s_DataDict[key];
		}

		public static T[] GetAll()
		{
			T[] datas = new T[s_DataDict.Count];
			s_DataDict.Values.CopyTo(datas, 0);

			return datas;
		}

		public static void Set(T value)
		{
			object key = value.dataDict[keyField];
			object[] datas = new object[value.dataDict.Count];
			value.dataDict.Values.CopyTo(datas, 0);

			if (!s_DataDict.ContainsKey(key))
			{
				s_SqliteUtil.Add(table, fields, datas);
			}
			else
			{
				s_SqliteUtil.Set(table, keyField, key, fields, datas);
			}

			s_DataDict[key] = value;
		}

		private static Dictionary<string, object> Parse(SqliteDataReader reader)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();

			for (int i = reader.FieldCount; --i >= 0;)
			{
				dict[reader.GetName(i)] = reader.GetValue(i);
			}

			return dict;
		}
	}
}