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
		private const string COMMAND_GET = "select %1 from %0 where %2";
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

		private string GetFieldType(Type type)
		{
			if (typeof(int) == type)
			{
				return "INT";
			}
			else if (typeof(long) == type)
			{
				return "INTEGER";
			}
			else if (typeof(bool) == type)
			{
				return "BOOLEAN";
			}
			else if (typeof(float) == type)
			{
				return "REAL";
			}
			else if (typeof(double) == type)
			{
				return "DOUBLE";
			}
			else if (typeof(char) == type)
			{
				return "CHAR";
			}

			return "TEXT";
		}
		
		public void Dispose()
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
		
		public SqliteDataReader Create(string table, string[] fields, Type[] types)
		{
			string command = fields[0] + COMMAND_SPACE + GetFieldType(types[0]);
			for (int i = 1; i < fields.Length; ++i)
			{
				command += COMMAND_SPLIT + fields[i] + COMMAND_SPACE + GetFieldType(types[i]);
			}
			
			command = ObjectExt.Replace(COMMAND_CREATE, table, command);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string table, object[] datas)
		{
			string command = ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < datas.Length; ++i)
			{
				command += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
			}

			command = ObjectExt.Replace(COMMAND_ADD, table, command);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string table, string[] fields, object[] datas)
		{
			string field = fields[0];
			for (int i = 1; i < fields.Length; ++i)
			{
				field += COMMAND_SPLIT + fields[i];
			}

			string data = ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < datas.Length; ++i)
			{
				data += COMMAND_SPLIT + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
			}

			string command = ObjectExt.Replace(COMMAND_ADD_INTO, table, field, data);
			
			return Execute(command);
		}

		public SqliteDataReader Get(string table, string[] getFields, string[] fields, string[] conditions, object[] datas)
		{
			string field = getFields[0];
			for (int i = 1; i < getFields.Length; ++i)
			{
				field += COMMAND_SPLIT + getFields[i];
			}

			string command = fields[0] + conditions[0] + ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < fields.Length; ++i)
			{
				command += COMMAND_AND + fields[i] + conditions[i] + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
			}

			command = ObjectExt.Replace(COMMAND_GET, table, field, command);

			return Execute(command);
		}
		
		public SqliteDataReader GetAll(string table)
		{
			string command = ObjectExt.Replace(COMMAND_GET_ALL, table);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string table, string getData, object getValue, string setField, object setData)
		{
			string get = getData + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getValue);
			string set = setField + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setData);
			string command = ObjectExt.Replace(COMMAND_SET, table, set, get);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string table, string getField, object getData, string[] setFields, object[] setDatas)
		{
			string get = getField + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, getData);
			string set = setFields[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setDatas[0]);
			for (int i = 1; i < setDatas.Length; ++i)
			{
				set += COMMAND_SPLIT + setFields[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, setDatas[i]);
			}

			string command = ObjectExt.Replace(COMMAND_SET, table, set, get);
			
			return Execute(command);
		}
	
		public SqliteDataReader Delete(string table, string[] fields, object[] datas)
		{
			string type = fields[0] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, datas[0]);
			for (int i = 1; i < fields.Length; ++i)
			{
				type += COMMAND_OR + fields[i] + COMMAND_EQUAL + ObjectExt.Replace(COMMAND_TEXT, datas[i]);
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