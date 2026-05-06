using System.Data;
using System.Data.SQLite;

namespace NagaisoraFramework
{
	public class SQLite
	{
		public SQLiteConnection m_dbConnection;

		public string DBFilePath;
		public int DBVersion;

		public void ConntentDB()
		{
			//没有数据库则自动创建
			m_dbConnection = new($"Data Source={DBFilePath};Version={DBVersion};");
			m_dbConnection.Open();
		}


		public DataTable DelectTable(string DataTable)
		{
			string SQLCommand = $"Select * from {DataTable}";
			SQLiteDataAdapter command = new(SQLCommand, m_dbConnection);

			DataTable DTable = new();
			command.Fill(DTable);

			return DTable;
		}

		public DataTable ListALL(string DataTable)
		{
			string SQLCommand = $"Select * from {DataTable}";
			SQLiteDataAdapter command = new(SQLCommand, m_dbConnection);

			DataTable DTable = new();
			command.Fill(DTable);

			return DTable;
		}

		/// <summary> 
		/// 添加数据记录
		/// </summary>
		/// <param name="DataTable">要执行的数据表</param>
		/// <param name="Command">后端SQL命令 格式：(--,--,--) Values (--,'--','--')</param>
		/// <returns>null</returns>
		public void InsertInto(string DataTable, string Command)
		{
			string SQLCommand = $"insert into {DataTable} {Command}";
			SQLiteCommand command = new(SQLCommand, m_dbConnection);

			command.ExecuteNonQuery();
		}

		public DataTable SelectFrom(string DataTable, string Command)
		{
			string SQLCommand = $"Select * from {DataTable} {Command}";
			SQLiteDataAdapter command = new(SQLCommand, m_dbConnection);

			DataTable DTable = new();
			command.Fill(DTable);

			return DTable;
		}

		public void InsertFrom(string DataTable, string Command)
		{
			string SQLCommand = $"insert from {DataTable} {Command}";
			SQLiteCommand command = new(SQLCommand, m_dbConnection);

			command.ExecuteNonQuery();
		}

		public void Update(string DataTable, string Command)
		{
			string SQLCommand = $"Update {DataTable} {Command}";
			SQLiteCommand command = new(SQLCommand, m_dbConnection);

			command.ExecuteNonQuery();
		}
	}
}