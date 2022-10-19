using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace NEA
{
    class KeyDatabaseManager
    {
        public List<int> getData(string currentKey, string guitarString)
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                var holder = connection.Query<int>(("SELECT " + guitarString + " FROM " + currentKey + ";"), new DynamicParameters());
                return holder.ToList();
            }
        }

        private static string LoadConnectionString()
        {
            return ("Data Source=" + System.IO.Path.GetFullPath("./KeyDatabase.db"));
        }
    }
}
