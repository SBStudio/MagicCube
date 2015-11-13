using UnityEngine;
using System;
using Mono.Data.Sqlite;

namespace Framework
{
	public sealed class SqliteUtil
	{
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

		private string GetDataType(Type type)
		{
			if (typeof(int) == type)
			{
				return "INTEGER";
			}
			else if (typeof(float) == type)
			{
				return "REAL";
			}

			return "TEXT";
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
		
		public SqliteDataReader Create(string table, string[] types, Type[] dataTypes)
		{
			string type = types[0] + COMMAND_SPACE + GetDataType(dataTypes[0]);
			for (int i = 1; i < types.Length; ++i)
			{
				type += COMMAND_SPLIT + types[i] + COMMAND_SPACE + GetDataType(dataTypes[i]);
			}
			
			string command = ObjectExt.Replace(COMMAND_CREATE, table, type);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string table, string[] datas)
		{
			string data = ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < datas.Length; ++i)
			{
				data += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
			}

			string command = ObjectExt.Replace(COMMAND_ADD, table, data);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string table, string[] types, string[] datas)
		{
			string type = types[0];
			for (int i = 1; i < types.Length; ++i)
			{
				type += COMMAND_SPLIT + types[i];
			}

			string data = ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < datas.Length; ++i)
			{
				data += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
			}

			string command = ObjectExt.Replace(COMMAND_ADD_INTO, table, type, data);
			
			return Execute(command);
		}

		public SqliteDataReader Get(string table, string[] getTypes, string[] types, string[] conditions, string[] values)
		{
			string getType = getTypes[0];
			for (int i = 1; i < getTypes.Length; ++i)
			{
				getType += COMMAND_SPLIT + getTypes[i];
			}

			string condition = types[0] + conditions[0] + ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < types.Length; ++i)
			{
				condition += COMMAND_AND + types[i] + conditions[i] + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_GET, getType, table, condition);

			return Execute(command);
		}
		
		public SqliteDataReader GetAll(string table)
		{
			string command = ObjectExt.Replace(COMMAND_GET_ALL, table);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string table, string getType, string getValue, string setType, string setValue)
		{
			string get = getType + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getValue);
			string set = setType + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValue);
			string command = ObjectExt.Replace(COMMAND_SET, table, set, get);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string table, string getType, string getValue, string[] setTypes, string[] setValues)
		{
			string get = getType + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getValue);
			string set = setTypes[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValues[0]);
			for (int i = 1; i < setValues.Length; ++i)
			{
				set += COMMAND_SPLIT + setTypes[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setValues[i]);
			}

			string command = ObjectExt.Replace(COMMAND_SET, table, set, get);
			
			return Execute(command);
		}
	
		public SqliteDataReader Delete(string table, string[] types, string[] values)
		{
			string type = types[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, values[0]);
			for (int i = 1; i < types.Length; ++i)
			{
				type += COMMAND_OR + types[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, values[i]);
			}

			string command = ObjectExt.Replace(COMMAND_DELETE, table, type);

			return Execute(command);
		}

		public SqliteDataReader Clear(string table)
		{
			string command = ObjectExt.Replace(COMMAND_CLEAR, table);
			
			return Execute(command);
		}
	}
}