using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band2310.Classes
{
    using System;
    using System.Data;
    using System.Data.OleDb;

    public class DatabaseManager : IDisposable
    {
        private string connectionString;
        private OleDbConnection connection;

        private static DatabaseManager instance = null;
        private static readonly object lockObject = new object();

        private DatabaseManager()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string mdbFileName = "Threads-uvs.mdb";
            string mdbFilePath = Path.Combine(baseDirectory, mdbFileName);

            connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={mdbFilePath};Jet OLEDB:Engine Type=5";
            connection = new OleDbConnection(connectionString);
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        // dar karta patikrint, nes kai kurios gijos
                        // gali praeiti pirma patikrinima
                        if (instance == null)
                        {
                            instance = new DatabaseManager();
                        }
                    }
                }
                return instance;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        public bool InsertData(int threadID, DateTime time, string data)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO [Work] (ThreadID, [Time], Data) VALUES (@ThreadID, @Time, @Data)";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        if (cmd == null)
                        {
                            Console.WriteLine("Nepavyko sukurti OleDbCommand objekto");
                            throw new Exception("Nepavyko sukurti OleDbCommand obijekto");
                        }

                        cmd.Parameters.Add("@ThreadID", OleDbType.Integer).Value = threadID;
                        cmd.Parameters.Add("@Time", OleDbType.Date).Value = time;
                        cmd.Parameters.Add("@Data", OleDbType.VarChar).Value = data;

                        //if (!OpenConnection())
                        //{
                        //    Console.WriteLine("Nepavyko atverti duomenu bazes.");
                        //    return false;
                        //}
                        if (ThreadsManager.Instance.threadsState == ThreadsManager.ThreadsState.Stopped)
                            return false;

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine("NullReferenceException: " + ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
                //finally
                //{
                //    CloseConnection();
                //}
            }
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ivyko klaida atveriant rysi su duomenu baze: " + ex.Message);
                throw new Exception("Ivyko klaida atveriant rysi su duomenu baze: " + ex.Message);
                //return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ivyko klaida nutraukiant rysi su duomenu baze: " + ex.Message);
                return false;
            }
        }
    }
}
