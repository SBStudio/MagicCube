using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public interface IData : IParser<Dictionary<string, object>>
	{
		Dictionary<string, object> dataDict { get; }
		string type { get; }
		object data { get; }
	}

	public sealed class DataSystem<T> where T : IData, new()
	{
		private static Dictionary<object, T> s_DataDict = new Dictionary<object, T>();
		private static SqliteUtil s_SqliteUtil;
		public static string table { get; private set; }

		public static void Init(string database, string table, params string[] fields)
		{
			DataSystem<T>.table = table;
			s_SqliteUtil = new SqliteUtil(database);

			Type[] types = new Type[fields.Length];
			for (int i = fields.Length; --i >= 0;)
			{
				types[i] = fields[i].GetType();
			}
			s_SqliteUtil.Create(table, fields, types);

			SqliteDataReader reader = s_SqliteUtil.GetAll(table);
			while (reader.Read())
			{
				Dictionary<string, object> data = new Dictionary<string, object>();
				for (int i = reader.FieldCount; --i >= 0;)
				{
					string type = reader.GetName(i);
					data[type] = reader.GetValue(i);
				}

				T dataInfo = new T();
				dataInfo.Parse(data);
				
				s_DataDict[dataInfo.data] = dataInfo;
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
			if (!s_DataDict.ContainsKey(value.data))
			{
				string[] fields;
				object[] datas;
				GetDatas(value.dataDict, out fields, out datas);

				s_SqliteUtil.Add(table, fields, datas);
			}

			s_DataDict[value.data] = value;
		}

		private static void GetDatas(Dictionary<string, object> dataDict, out string[] fields, out object[] datas)
		{
			string[] ts = new string[dataDict.Count];
			object[] ds = new object[dataDict.Count];

			int i = 0;
			foreach (KeyValuePair<string, object> dataInfo in dataDict)
			{
				ts[i] = dataInfo.Key;
				ds[i] = dataInfo.Value;
				++i;
			}

			fields = ts;
			datas = ds;
		}
	}
}