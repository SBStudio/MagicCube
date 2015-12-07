using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public interface IData : IParser<Dictionary<string, object>>
	{
		Dictionary<string, object> dataDict { get; }
	}

	public sealed class DataSystem<T> : Singleton<DataSystem<T>> where T : IData, new()
	{
		public string[] fields { get; private set; }
		public string keyField { get; private set; }
		public string table { get; private set; }

		private Dictionary<object, T> m_DataDict = new Dictionary<object, T>();
		private SqliteUtil m_SqliteUtil;

		public void Init(string database, string table, Dictionary<string, Type> fieldsDict)
		{
			this.table = table;

			fields = new string[fieldsDict.Count];
			fieldsDict.Keys.CopyTo(fields, 0);
			Type[] types = new Type[fieldsDict.Count];
			fieldsDict.Values.CopyTo(types, 0);

			keyField = fields[0];

			m_SqliteUtil = new SqliteUtil(database);
			m_SqliteUtil.Create(table, fields, types);

			SqliteDataReader reader = m_SqliteUtil.GetAll(table);
			while (reader.Read())
			{
				T dataInfo = new T();
				dataInfo.Parse(Parse(reader));
				
				m_DataDict[dataInfo.dataDict[keyField]] = dataInfo;
			}
		}

		public T Get(object key)
		{
			if (!m_DataDict.ContainsKey(key))
			{
				return default(T);
			}

			return m_DataDict[key];
		}

		public T[] GetAll()
		{
			if (0 >= m_DataDict.Count)
			{
				return null;
			}

			T[] datas = new T[m_DataDict.Count];
			m_DataDict.Values.CopyTo(datas, 0);

			return datas;
		}

		public void Set(T value)
		{
			object key = value.dataDict[keyField];
			object[] datas = new object[value.dataDict.Count];
			value.dataDict.Values.CopyTo(datas, 0);

			if (!m_DataDict.ContainsKey(key))
			{
				m_SqliteUtil.Add(table, fields, datas);
			}
			else
			{
				m_SqliteUtil.Set(table, keyField, key, fields, datas);
			}

			m_DataDict[key] = value;
		}

		public void Delete(object key)
		{
			if (!m_DataDict.ContainsKey(key))
			{
				return;
			}

			m_SqliteUtil.Delete(table, new string[] { keyField }, new object[] { key });

			m_DataDict.Remove(key);
		}

		private Dictionary<string, object> Parse(SqliteDataReader reader)
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