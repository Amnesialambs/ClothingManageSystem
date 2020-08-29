using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace ClothingManageSystem.utils
{
    class SQLLiteDBUtil
    {
        // 数据库文件夹
        static string DbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database");

        //与指定的数据库(实际上就是一个文件)建立连接
        private static SQLiteConnection CreateDatabaseConnection(string dbName = null)
        {
            if (!string.IsNullOrEmpty(DbPath) && !Directory.Exists(DbPath))
                Directory.CreateDirectory(DbPath);
            dbName = dbName == null ? "database.db" : dbName;
            var dbFilePath = Path.Combine(DbPath, dbName);
            if (!File.Exists(dbFilePath))
            {
                File.Create(dbFilePath);
            }
            return new SQLiteConnection("DataSource = " + dbFilePath);
        }


        // 使用全局静态变量保存连接
        private static SQLiteConnection connection = CreateDatabaseConnection();

        // 判断连接是否处于打开状态
        private static void Open(SQLiteConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public static void ExecuteNonQuery(string sql)
        {
            // 确保连接打开
            Open(connection);

            using (var tr = connection.BeginTransaction())
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                    tr.Commit();
                }
                catch (System.Data.SQLite.SQLiteException E)
                {
                    tr.Rollback();
                    Console.WriteLine(E.Message);
                    MessageBox.Show(E.Message);
                }
                finally
                {

                    Close(connection);
                }
            }
        }
        public static void BatchExecuteNonQuery(ArrayList sqlList)
        {
            // 确保连接打开
            Open(connection);

            using (var tr = connection.BeginTransaction())
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        for (int i = 0; i < sqlList.Count; i++)
                        {
                            string sql = sqlList[i].ToString().Trim();
                            if (sql.Length > 1)
                            {
                                command.CommandText = sql;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    tr.Commit();
                }
                catch (System.Data.SQLite.SQLiteException E)
                {
                    tr.Rollback();
                    Console.WriteLine(E.Message);
                    MessageBox.Show(E.Message);
                }
                finally
                {

                    Close(connection);
                }
            }
        }
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dataTable = new DataTable();
            // 确保连接打开
            Open(connection);

            using (var tr = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {

                    command.CommandText = sql;

                    // 执行查询会返回一个SQLiteDataReader对象
                    SQLiteDataReader reader = command.ExecuteReader();

                    //reader.Read()方法会从读出一行匹配的数据到reader中。注意：是一行数据。
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();
                        int count = reader.FieldCount;
                        for (int i = 0; i < count; i++)
                        {
                            if (dataTable.Columns.Count < count)
                            {
                                dataTable.Columns.Add(new DataColumn(reader.GetName(i)));
                            }
                            dataRow[reader.GetName(i)] = reader[i];
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                tr.Commit();
            }
            return dataTable;
        }

        // 因为SQLite是文件型数据库，可以直接删除文件。但只要数据库连接没有被回收，就无法删除文件。
        public static void DeleteDatabase(string dbName)
        {
            var path = Path.Combine(DbPath, dbName);
            connection.Close();

            // 置空，手动GC，并等待GC完成后执行文件删除。
            connection = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete(path);
        }

        public static void Close(SQLiteConnection connection)
        {
            connection.Close();
        }

    }
}
