using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public interface IDataInfo<T> : IParser<Dictionary<string, object>>
	{
		string database { get; }
		string table { get; }
		Dictionary<string, Type> types { get; }
		T key { get; }
	}

	public sealed class DataSystem<K, V> where V : IDataInfo<K>, new()
	{
		public static readonly DataSystem<K, V> instance = new DataSystem<K, V>();

		private Dictionary<K, V> m_DataDict = new Dictionary<K, V>();

		public DataSystem()
		{
			V dataInfo = new V();

			SqliteUtil sqliteUtil = new SqliteUtil(dataInfo.database);
			string tableName = dataInfo.table;
			string[] types = new string[dataInfo.types.Count];
			dataInfo.types.Keys.CopyTo(types, 0);
			Type[] dataTypes = new Type[dataInfo.types.Count];
			dataInfo.types.Values.CopyTo(dataTypes, 0);

			sqliteUtil.Create(tableName, types, dataTypes);

			SqliteDataReader reader = sqliteUtil.GetAll(tableName);
			while (reader.Read())
			{
				Dictionary<string, object> data = new Dictionary<string, object>();
				for (int i = reader.FieldCount; --i >= 0;)
				{
					string type = reader.GetName(i);
					int index = reader.GetOrdinal(type);

					Type dataType = reader.GetFieldType(index);
					if (typeof(int) == dataType)
					{
						data[type] = reader.GetInt32(index);
					}
					else if (typeof(float) == dataType)
					{
						data[type] = reader.GetFloat(index);
					}
					else
					{
						data[type] = reader.GetString(index);
					}
				}

				dataInfo = new V();
				dataInfo.Parse(data);
				
				m_DataDict[dataInfo.key] = dataInfo;
			}
		}

		public V Get(K key)
		{
			if (!m_DataDict.ContainsKey(key))
			{
				return default(V);
			}

			return m_DataDict[key];
		}
	}
}