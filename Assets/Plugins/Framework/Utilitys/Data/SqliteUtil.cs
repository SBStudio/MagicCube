using UnityEngine;
using Mono.Data.Sqlite;

namespace Framework
{
	public sealed class SqliteUtil
	{
		private const string COMMAND_SPLIT = ", ";
		private const string COMMAND_EQUAL = " = ";
		private const string COMMAND_OR = " or ";
		private const string COMMAND_OPEN = "data source=%0";
		private const string COMMAND_FIND_ALL = "SELECT * FROM %0";
		private const string COMMAND_ADD = "INSERT INTO %0 VALUES (%1)";
		private const string COMMAND_SET = "UPDATE %0 SET %1 WHERE %2";
		private const string COMMAND_DELETE = "DELETE FROM %0 WHERE %1";
		
		public SqliteConnection sqliteConnection { get; private set; }
		public SqliteCommand sqliteCommand { get; private set; }
		
		public SqliteUtil(string name)
		{
			sqliteConnection = new SqliteConnection(name.Replace(COMMAND_OPEN, name));
			sqliteCommand = sqliteConnection.CreateCommand();
		}
		
		public void Close()
		{
			sqliteCommand.Dispose();

			sqliteConnection.Close();
			sqliteConnection.Dispose();
		}
		
		public SqliteDataReader Execute(string command)
		{
			sqliteCommand.CommandText = command;
			SqliteDataReader sqliteReader = sqliteCommand.ExecuteReader();

			return sqliteReader;
		}
		
		public SqliteDataReader FindAll(string name)
		{
			string command = name.Replace(COMMAND_FIND_ALL, name);
			
			return Execute(command);
		}

		public SqliteDataReader Add(string name, params string[] values)
		{
			string value = values[0];
			for (int i = 1; i < values.Length; ++i)
			{
				value += COMMAND_SPLIT + values[i];
			}

			string command = value.Replace(COMMAND_ADD, name, value);
			
			return Execute(command);
		}
		
		public SqliteDataReader Set(string name, string getKey, string getValue, string setKey, string setValue)
		{
			string get = getKey + COMMAND_EQUAL + getValue;
			string set = setKey + COMMAND_EQUAL + setValue;
			string command = set.Replace(COMMAND_SET, name, set, get);
			
			return Execute(command);
		}

		public SqliteDataReader Set(string name, string getKey, string getValue, string[] setKeys, string[] setValues)
		{
			string get = getKey + COMMAND_EQUAL + getValue;
			string set = setKeys[0] + COMMAND_EQUAL + setValues[0];
			for (int i = 1; i < setValues.Length; ++i)
			{
				set += COMMAND_SPLIT + setKeys[i] + COMMAND_EQUAL + setValues[i];
			}

			string command = set.Replace(COMMAND_SET, name, set, get);
			
			return Execute(command);
		}
	
		public SqliteDataReader Delete(string name, string[] keys, string[] values)
		{
			string get = keys[0] + COMMAND_EQUAL + values[0];
			for (int i = 1; i < values.Length; ++i)
			{
				get += COMMAND_OR + keys[i] + COMMAND_EQUAL + values[i];
			}

			string command = get.Replace(COMMAND_DELETE, name, get);

			return Execute(command);
		}
		//		
		//		public SqliteDataReader InsertIntoSpecific (string tableName, string[] cols, string[] values)
		//			
		//		{
		//			
		//			if (cols.Length != values.Length) {
		//				
		//				throw new SqliteException ("columns.Length != values.Length");
		//				
		//			}
		//			
		//			string query = "INSERT INTO " + tableName + "(" + cols[0];
		//			
		//			for (int i = 1; i < cols.Length; ++i) {
		//				
		//				query += ", " + cols[i];
		//				
		//			}
		//			
		//			query += ") VALUES (" + values[0];
		//			
		//			for (int i = 1; i < values.Length; ++i) {
		//				
		//				query += ", " + values[i];
		//				
		//			}
		//			
		//			query += ")";
		//			
		//			return ExecuteQuery (query);
		//			
		//		}
		//		
		//		public SqliteDataReader DeleteContents (string tableName)
		//			
		//		{
		//			
		//			string query = "DELETE FROM " + tableName;
		//			
		//			return ExecuteQuery (query);
		//			
		//		}
		//		
		//		public SqliteDataReader CreateTable (string name, string[] col, string[] colType)
		//			
		//		{
		//			
		//			if (col.Length != colType.Length) {
		//				
		//				throw new SqliteException ("columns.Length != colType.Length");
		//				
		//			}
		//			
		//			string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
		//			
		//			for (int i = 1; i < col.Length; ++i) {
		//				
		//				query += ", " + col[i] + " " + colType[i];
		//				
		//			}
		//			
		//			query += ")";
		//			
		//			return ExecuteQuery (query);
		//			
		//		}
		//		
		//		public SqliteDataReader SelectWhere (string tableName, string[] items, string[] col, string[] operation, string[] values)
		//			
		//		{
		//			
		//			if (col.Length != operation.Length ¦¦ operation.Length != values.Length) {
		//				
		//				throw new SqliteException ("col.Length != operation.Length != values.Length");
		//				
		//			}
		//			
		//			string query = "SELECT " + items[0];
		//			
		//			for (int i = 1; i < items.Length; ++i) {
		//				
		//				query += ", " + items[i];
		//				
		//			}
		//			
		//			query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
		//			
		//			for (int i = 1; i < col.Length; ++i) {
		//				
		//				query += " AND " + col[i] + operation[i] + "'" + values[0] + "' ";
		//				
		//			}
		//			
		//			return ExecuteQuery (query);
		//			
		//		}
	}
}