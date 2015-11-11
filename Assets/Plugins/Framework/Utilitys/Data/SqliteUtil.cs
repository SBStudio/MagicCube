using UnityEngine;
using Mono.Data.Sqlite;

namespace Framework
{
	public sealed class SqliteUtil
	{
		public enum DataType
		{
			NULL,
			INTEGER,
			REAL,
			TEXT,
			BLOB,
		}

		private const string COMMAND_SPACE = " ";
		private const string COMMAND_SPLIT = ", ";
		private const string COMMAND_EQUAL = " = ";
		private const string COMMAND_OR = " or ";
		private const string COMMAND_AND = " and ";
		private const string COMMAND_TEXT = "'%0'";
		private const string COMMAND_OPEN = "data source=%0";
		private const string COMMAND_CREATE = "create table if not exists %0 (%1)";
		private const string COMMAND_ADD = "insert into %0 values (%1)";
		private const string COMMAND_ADD_INTO = "insert into %0 (%1) values (%2)";
		private const string COMMAND_GET = "select %0 from %1 where %2";
		private const string COMMAND_GET_ALL = "select * from %0";
		private const string COMMAND_SET = "update %0 SET %1 where %2";
		private const string COMMAND_DELETE = "delete from %0 where %1";
		private const string COMMAND_CLEAR = "delete from %0";
		
		private SqliteConnection m_SqliteConnection;
		private SqliteCommand m_SqliteCommand;
		
		public SqliteUtil(string name)
		{
			string command = ObjectExt.Replace(COMMAND_OPEN, name);
			m_SqliteConnection = new SqliteConnection(command);
			m_SqliteConnection.Open();
		}
		
		public void Close()
		{
			m_SqliteCommand.Dispose();

			m_SqliteConnection.Close();
			m_SqliteConnection.Dispose();
		}
		
		public SqliteDataReader Execute(string command)
		{
			m_SqliteCommand = m_SqliteConnection.CreateCommand();
			m_SqliteCommand.CommandText = command;
			SqliteDataReader sqliteReader = m_SqliteCommand.ExecuteReader();

			return sqliteReader;
		}
		
		public SqliteDataReader Create(string name, string[] columns, DataType[] types)
		{
			string key = columns[0] + COMMAND_SPACE + types[0].ToString();
			for (int i = 1; i < columns.Length; ++i)
			{
				key += COMMAND_SPLIT + columns[i] + COMMAND_SPACE + types[i].ToString();
			}
			
			string command = ObjectExt.Replace(COMMAND_CREATE, name, key);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string name, string[] values)
		{
			string value = ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				value += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_ADD, name, value);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string name, string[] keys, string[] values)
		{
			string key = keys[0];
			for (int i = 1; i < keys.Length; ++i)
			{
				key += COMMAND_SPLIT + keys[i];
			}

			string value = ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				value += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_ADD_INTO, name, key, value);
			
			return Execute(command);
		}

		public SqliteDataReader Get(string name, string[] selections, string[] keys, string[] conditions, string[] values)
		{
			string select = selections[0];
			for (int i = 1; i < selections.Length; ++i)
			{
				select += COMMAND_SPLIT + selections[i];
			}

			string condition = keys[0] + conditions[0] + ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < keys.Length; ++i)
			{
				condition += COMMAND_AND + keys[i] + conditions[i] + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_GET, name, select, condition);

			return Execute(command);
		}
		
		public SqliteDataReader GetAll(string name)
		{
			string command = ObjectExt.Replace(COMMAND_GET_ALL, name);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string name, string getKey, string getValue, string setKey, string setValue)
		{
			string get = getKey + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getValue);
			string set = setKey + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValue);
			string command = ObjectExt.Replace(COMMAND_SET, name, set, get);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string name, string getKey, string getValue, string[] setKeys, string[] setValues)
		{
			string get = getKey + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getValue);
			string set = setKeys[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValues[0]);
			for (int i = 1; i < setValues.Length; ++i)
			{
				set += COMMAND_SPLIT + setKeys[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValues[i]);
			}

			string command = ObjectExt.Replace(COMMAND_SET, name, set, get);
			
			return Execute(command);
		}
	
		public SqliteDataReader Delete(string name, string[] keys, string[] values)
		{
			string get = keys[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < keys.Length; ++i)
			{
				get += COMMAND_OR + keys[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_DELETE, name, get);

			return Execute(command);
		}

		public SqliteDataReader Clear(string name)
		{
			string command = ObjectExt.Replace(COMMAND_CLEAR, name);
			
			return Execute(command);
		}
	}
}