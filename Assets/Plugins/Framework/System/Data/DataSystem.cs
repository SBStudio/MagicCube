using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public interface IDataInfo : IParser<Dictionary<string, object>>
	{
		object key { get; }
	}

	public sealed class DataSystem<T> where T : IDataInfo, new()
	{
		private static Dictionary<object, T> s_DataDict = new Dictionary<object, T>();

		public static void Init(string database, string table)
		{
			SqliteUtil sqliteUtil = new SqliteUtil(database);
			string tableName = table;

			SqliteDataReader reader = sqliteUtil.GetAll(tableName);
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
				
				s_DataDict[dataInfo.key] = dataInfo;
			}

			sqliteUtil.Dispose();
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
	}
}