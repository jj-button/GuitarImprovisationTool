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
    class NoteDatabaseManager
    {
        public List<string> getData()
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                var holder = connection.Query<string>(("SELECT Note FROM GuitarNotes;"), new DynamicParameters());
                return holder.ToList();
            }
        }

        private static string LoadConnectionString()
        {
            return ("Data Source=" + System.IO.Path.GetFullPath("./NoteDatabase.db"));
        }
    }
}
