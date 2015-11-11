using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Framework
{
	public sealed class DataSystem : Singleton<DataSystem>
	{
		private Dictionary<string, SqliteUtil> m_SqliteDict = new Dictionary<string, SqliteUtil>();

		public static void Add<T>(string name) where T : IDatabase, new()
		{
			if (instance.m_SqliteDict.ContainsKey(name))
			{
				return;
			}

			SqliteUtil sqliteUtil = new SqliteUtil(name);
			instance.m_SqliteDict[name] = sqliteUtil;
			
			T database = new T();
			string[] columns = new string[database.columns.Length];
			SqliteUtil.DataType[] types = new SqliteUtil.DataType[database.columns.Length];
			for (int i = database.columns.Length; --i >= 0;)
			{
				string column = database.columns[i];
				columns[i] = column;
				types[i] = SqliteUtil.DataType.TEXT;
			}
			
			sqliteUtil.Create(typeof(T).Name, columns, types);
		}

		public static void Get<T>(string name) where T : IDatabase, new()
		{

		}
	}
}