using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data.OleDb;
using System.Data.Common;
using System.Collections.ObjectModel;

namespace CanConsteel.Models
{
    class DataAccess
    {
        public static int totalByte=1;
        public static int currentByte;
        
        

        public static bool CheckDatabase()
        {
            string connStr = Properties.Settings.Default.Connection;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                conn.Close();
                return true;
            }
        }

        public static void Backup(string Path)
        {
            string connectionString = Properties.Settings.Default.Connection + ";charset=utf8;convertzerodatetime=true;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlBackup mySqlBackup = new MySqlBackup(cmd);
                mySqlBackup.ExportInfo.IntervalForProgressReport = 50;
                mySqlBackup.ExportInfo.GetTotalRowsMode = GetTotalRowsMethod.InformationSchema;
                mySqlBackup.ExportProgressChanged += MySqlBackup_ExportProgressChanged;
                mySqlBackup.ExportCompleted += MySqlBackup_ExportCompleted;
                
                
                conn.Open();
                try
                {
                    mySqlBackup.ExportToFile(Path + "\\CanConsteel_" + DateTime.Now.ToString("dd-MM-yyyy--H-mm-ss") + ".sql");
                }
                catch
                {
                    throw;
                }
                conn.Close();
            }

        }

        private static void MySqlBackup_ExportCompleted(object sender, ExportCompleteArgs e)
        {
            if (!e.HasError)
                currentByte = totalByte;
        }

        private static void MySqlBackup_ExportProgressChanged(object sender, ExportProgressArgs e)
        {
            totalByte = (int)e.TotalRowsInAllTables;
            currentByte = (int)e.CurrentRowIndexInAllTables;
            
        }

        public static void Restore(string file)
        {
            string connectionString = Properties.Settings.Default.Connection + ";charset=utf8;convertzerodatetime=true;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlBackup mySqlBackup = new MySqlBackup(cmd);
                mySqlBackup.ImportInfo.IntervalForProgressReport = 10;
                mySqlBackup.ImportProgressChanged += MySqlBackup_ImportProgressChanged;
                conn.Open();
                try
                {
                    mySqlBackup.ImportFromFile(file);
                }
                catch
                {
                    throw;
                }
                conn.Close();
            }
        }

        private static void MySqlBackup_ImportProgressChanged(object sender, ImportProgressArgs e)
        {
            totalByte = (int)e.TotalBytes;
            currentByte = (int)e.CurrentBytes;
        }

        public static void InsertAlarm(Alarm alarm)
        {
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }
                try
                {
                    conn.Open();
                    string strquery = "INSERT INTO alarms (code, occurTime, Description, State) VALUES (@code, @Time, @description, @State)";
                    MySqlCommand query = new MySqlCommand(strquery, conn);
                    query.Parameters.AddWithValue("@code", alarm.code);
                    query.Parameters.AddWithValue("@Time", alarm.OccurTime);
                    query.Parameters.AddWithValue("@description", alarm.Description);
                    query.Parameters.AddWithValue("@State", alarm.State);
                    query.ExecuteNonQuery();
                    conn.Close();
                }
                catch
                {

                }
            }
        }

        public static long CheckExistAlarm(int code)
        {
            long returnValue = 0;
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }
                try
                {
                    string strquery = "SELECT * FROM alarms WHERE code = @code AND ISNULL(resetTime)";
                    MySqlCommand query = new MySqlCommand(strquery, conn);
                    conn.Open();
                    query.Parameters.AddWithValue("@code", code);
                    MySqlDataReader reader = query.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];
                        returnValue = (long)row["Id"];

                    }
                    conn.Close();
                }
                catch { }
            }
            return returnValue;
        }

        public static void AckAlarm(long Id)
        {
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }
                conn.Open();
                string strquery = "UPDATE alarms SET ackTime = @Time, State=1 WHERE Id = @Id";

                MySqlCommand query = new MySqlCommand(strquery, conn);
                query.Parameters.AddWithValue("@Time", DateTime.Now);
                query.Parameters.AddWithValue("@Id", Id);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void ResetAlarm(long Id)
        {
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }
                conn.Open();
                string strquery = "UPDATE alarms SET resetTime = @Time, State=0 WHERE Id = @Id";

                MySqlCommand query = new MySqlCommand(strquery, conn);
                query.Parameters.AddWithValue("@Time", DateTime.Now);
                query.Parameters.AddWithValue("@Id", Id);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void DeleteHistory()
        {
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }

                string strquery = "DELETE FROM alarms WHERE `resetTime` IS NOT NULL";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                conn.Open();
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static ObservableCollection<Alarm> GetAlarms()
        {
            ObservableCollection<Alarm> returnValue = new ObservableCollection<Alarm>();
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }
                string strquery = "SELECT * FROM alarms ORDER BY Id DESC";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                conn.Open();
                MySqlDataReader reader = query.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Alarm newAlarm = new Alarm();
                        newAlarm.Id = (long)row["Id"];
                        newAlarm.code = (int)row["code"];
                        newAlarm.OccurTime = (DateTime)row["occurTime"];
                        if (!(row["ackTime"] is DBNull))
                            newAlarm.AckTime = (DateTime)row["ackTime"];
                        if (!(row["resetTime"] is DBNull))
                            newAlarm.ResetTime = (DateTime)row["resetTime"];
                        newAlarm.Description = (string)row["description"];
                        returnValue.Add(newAlarm);
                    }
                }
                conn.Close();
            }
            return returnValue;
        }

        public static ObservableCollection<Billet> GetBillets()
        {
            ObservableCollection<Billet> returnValue = new ObservableCollection<Billet>();
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }
                string strquery = "SELECT * FROM billets ORDER BY Id DESC";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                conn.Open();
                MySqlDataReader reader = query.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Billet newBillet = new Billet();
                        newBillet.Id = (long)row["Id"];
                        newBillet.InsertTime = (DateTime)row["Time"];
                        newBillet.Lenght = (double)row["Lenght"];
                        if (!(row["Weinght"] is DBNull))
                            newBillet.Weinght = (double)row["Weinght"];
                        newBillet.Batch = (int)row["Batch"];
                        if (!(row["Grade"] is DBNull))
                            newBillet.Grade = row["Grade"].ToString();
                        else
                            newBillet.Grade = "";
                        newBillet.ReqWeinght = (double)row["ReqWeinght"];
                        returnValue.Add(newBillet);
                    }
                }
                conn.Close();
            }
            return returnValue;
        }

        public static long InsertBatch(Batch batch)
        {
            long returnValue = 0;
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }
                conn.Open();
                string strquery = "INSERT INTO batchs (BatchId, Weight, StartTime, StopTime) VALUES (@BatchId, @Weight, @StartTime, @StopTime)";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                query.Parameters.AddWithValue("@BatchId", batch.BatchId);
                query.Parameters.AddWithValue("@Weight", batch.Weinght);
                query.Parameters.AddWithValue("@StartTime", batch.StartTime);
                query.Parameters.AddWithValue("@StopTime", batch.StopTime);
                query.ExecuteNonQuery();
                returnValue = query.LastInsertedId;
                conn.Close();
            }
            return returnValue;
        }

        public static void UpdateBatchWeinght(long id, double weinght)
        {
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }
                conn.Open();
                string strquery = "UPDATE batchs SET Weight = @Weinght, StopTime = @StopTime WHERE BatchId = @Id";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                query.Parameters.AddWithValue("@Weinght", weinght);
                query.Parameters.AddWithValue("@StopTime", DateTime.Now);
                query.Parameters.AddWithValue("@Id", id);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static ObservableCollection<Batch> GetBatchs(DateTime from, DateTime to, ref double totalWeinght)
        {
            ObservableCollection<Batch> returnValue = new ObservableCollection<Batch>();
            DateTime startTime = from.Date.AddHours(7);
            DateTime stopTime = to.Date.AddHours(7);
            totalWeinght = 0;
            using (MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.Connection))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null");
                }
                string strquery = "SELECT * FROM batchs Where `StopTime`>= @start AND `StopTime`<@stop ORDER BY `StopTime` DESC";
                MySqlCommand query = new MySqlCommand(strquery, conn);
                conn.Open();
                query.Parameters.AddWithValue("@start", startTime);
                query.Parameters.AddWithValue("@stop", stopTime);
                MySqlDataReader reader = query.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Batch newBatch = new Batch();
                        newBatch.Id = (long)row["Id"];
                        newBatch.BatchId = (long)row["BatchId"];
                        newBatch.StartTime = (DateTime)row["StartTime"];
                        if(!(row["StopTime"] is DBNull))
                            newBatch.StopTime = (DateTime)row["StopTime"];
                        newBatch.Weinght = (double)row["Weight"];
                        totalWeinght += (double)row["Weight"];
                        returnValue.Add(newBatch);
                    }
                }
                conn.Close();
            }
            return returnValue;
        }
    }
}
