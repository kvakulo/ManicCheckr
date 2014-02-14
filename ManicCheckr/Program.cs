using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManicCheckr
{
    class Program
    {
        private const int TimelimitMinutes = 10;
        private const int CheckIntervalMinutes = 10;

        private static void Main()
        {
            MainAsync().Wait();
        }

        private async static Task MainAsync()
        {
            var sdfFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Finkit\ManicTime\ManicTime.sdf");

            while (true)
            {
                var connectionString = string.Format(@"Data Source={0};Persist Security Info=False;", sdfFile);
                using (var connection = new SqlCeConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCeCommand("SELECT EndLocalTime FROM Activity ORDER BY ActivityId DESC", connection);
                    var lastActivityTime = DateTime.Parse(command.ExecuteScalar().ToString());
                    var diff = DateTime.Now - lastActivityTime;
                    Console.WriteLine("Last activity was " + diff.TotalMinutes + " minutes ago");
                    if (diff.TotalMinutes > TimelimitMinutes)
                    {
                        MessageBox.Show(string.Format("It has been {0} minutes since the last activity in ManicTime.\r\n" +
                            "Please check if ManicTime is running correctly.", Math.Round(diff.TotalMinutes)), "ManicTime Alert", MessageBoxButtons.OK);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(CheckIntervalMinutes));
            }
        }
    }
}
