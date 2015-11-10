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
		private const string COMMAND_AND = " AND ";
		private const string COMMAND_TEXT = "'%0'";
		private const string COMMAND_OPEN = "data source=%0";
		private const string COMMAND_FIND_ALL = "SELECT * FROM %0";
		private const string COMMAND_ADD = "INSERT INTO %0 VALUES (%1)";
		private const string COMMAND_ADD_INTO = "INSERT INTO %0 (%1) VALUES (%2)";
		private const string COMMAND_GET = "SELECT %0 FROM %1 WHERE %2";
		private const string COMMAND_SET = "UPDATE %0 SET %1 WHERE %2";
		private const string COMMAND_DELETE = "DELETE FROM %0 WHERE %1";
		private const string COMMAND_CLEAR = "DELETE FROM %0";
		private const string COMMAND_CREATE = "CREATE TABLE %0 (%1)";
		
		public SqliteConnection sqliteConnection { get; private set; }
		public SqliteCommand sqliteCommand { get; private set; }
		
		public SqliteUtil(string name)
		{
			sqliteConnection = new SqliteConnection(ObjectExt.Replace(COMMAND_OPEN, name));
			sqliteConnection.Open();
		}
		
		public void Close()
		{
			sqliteCommand.Dispose();

			sqliteConnection.Close();
			sqliteConnection.Dispose();
		}
		
		public SqliteDataReader Execute(string command)
		{
			sqliteCommand = sqliteConnection.CreateCommand();
			sqliteCommand.CommandText = command;
			SqliteDataReader sqliteReader = sqliteCommand.ExecuteReader();

			return sqliteReader;
		}
		
		public SqliteDataReader FindAll(string name)
		{
			string command = ObjectExt.Replace(COMMAND_FIND_ALL, name);
			
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

		public SqliteDataReader Create(string name, string[] keys, DataType[] types)
		{
			string key = keys[0] + COMMAND_SPACE + types[0].ToString();
			for (int i = 1; i < keys.Length; ++i)
			{
				key += COMMAND_SPLIT + keys[i] + COMMAND_SPACE + types[i].ToString();
			}

			string command = ObjectExt.Replace(COMMAND_CREATE, name, key);

			return Execute(command);
		}
	}
}