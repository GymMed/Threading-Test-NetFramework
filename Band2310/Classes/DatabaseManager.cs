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

    public class DatabaseManager
    {
        private string connectionString;
        private OleDbConnection connection;

        public DatabaseManager(string mdbFilePath)
        {
            connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={mdbFilePath};Jet OLEDB:Engine Type=5";
            connection = new OleDbConnection(connectionString);
        }

        public bool InsertData(int threadID, DateTime time, string data)
        {
            try
            {
                string query = "INSERT INTO [Work] (ThreadID, [Time], Data) VALUES (@ThreadID, @Time, @Data)";
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    if (cmd == null)
                    {
                        throw new Exception("Nepavyko sukurti OleDbCommand obijekto");
                    }

                    cmd.Parameters.Add("@ThreadID", OleDbType.Integer).Value = threadID;
                    cmd.Parameters.Add("@Time", OleDbType.Date).Value = time;
                    cmd.Parameters.Add("@Data", OleDbType.VarChar).Value = data;

                    if (!OpenConnection())
                    {
                        Console.WriteLine("Nepavyko atverti duomenu bazes.");
                        return false;
                    }

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
            finally
            {
                CloseConnection();
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
                return false;
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
