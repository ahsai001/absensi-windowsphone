using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using absensi.AhsaiLib;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace absensi
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            //DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void refreshDailyList_Click_1(object sender, RoutedEventArgs e)
        {
            
            refreshDailyList.IsEnabled = false;
            loadingDailyIndicator.Visibility = Visibility.Visible;
            HttpClient client = new HttpClient();
            client.GetDataFrom("http://202.61.124.20:8270/tes/absen/api/absen.php", "cookies=" + App.cookies, new AhsaiLib.HttpClient.EventHandlerAfterConnection(ConnectionHandler));
        }
        public void ConnectionHandler(string data)
        {
            //outputBox.Text = data;
            string json = @data;
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            
            List<string> fieldList = new List<string>();
            int i = 0;
            bool isRun = true;
            while (reader.Read() && isRun)
            {
                if (reader.Path.Equals("data.field["+i.ToString()+"]"))
                {
                    fieldList.Add(reader.Value.ToString());
                    i++;
                }
                else if (reader.Path.Equals("data.item"))
                {
                   isRun = false;
                }             
       
            }
            List<string> machineList = new List<string>();
            List<string> employeeList = new List<string>();
            List<string> dateList = new List<string>();
            List<string> statusList = new List<string>();
            List<string> lateList = new List<string>();

            int fieldNum = fieldList.Count;
            int j = 0;
            int k = 0;
            isRun = true;
            while (reader.Read() && isRun)
            {
                if (reader.Path.Equals("data.item[" + j.ToString() + "]["+k.ToString()+"]"))
                {
                    if (k == 0)
                    {
                        machineList.Add(reader.Value.ToString());
                        k = 1;
                    }
                    else if (k == 1)
                    {
                        employeeList.Add(reader.Value.ToString());
                        k = 2;
                    }
                    else if (k == 2)
                    {
                        dateList.Add(reader.Value.ToString());
                        k = 3;
                    }
                    else if (k == 3)
                    {
                        statusList.Add(reader.Value.ToString());
                        k = 4;
                    }
                    else if (k == 4)
                    {
                        lateList.Add(reader.Value.ToString());
                        j++;
                        k = 0;
                    }
                    
                }
                else if (reader.Path.Equals("data"))
                {
                    isRun = false;
                }
                
            }
            App.ViewModel.ClearAllItems();
            for (int x = 0; x < machineList.Count; x++)
            {
                App.ViewModel.AddItem(machineList.ElementAt(x), employeeList.ElementAt(x), dateList.ElementAt(x), statusList.ElementAt(x), lateList.ElementAt(x));
            }
            harianList.ItemsSource = App.ViewModel.Items;
            refreshDailyList.IsEnabled = true;
            loadingDailyIndicator.Visibility = Visibility.Collapsed;
        }

        private void loadMonthLyList_Click_1(object sender, RoutedEventArgs e)
        {
            loadMonthLyList.IsEnabled = false;
            loadingMonthlyIndicator.Visibility = Visibility.Visible;
            DateTime? date = datePicker.Value as DateTime?;
            string month = date.Value.Month.ToString();
            if(month.Length == 1)
            {
                month = "0"+month;
            }

            HttpClient client = new HttpClient();
            client.GetDataFrom("http://202.61.124.20:8270/tes/absen/api/list.php", "cookies=" + App.cookies + "&tahun=" + date.Value.Year.ToString() + "&bulan=" + month, new AhsaiLib.HttpClient.EventHandlerAfterConnection(LoadMonthlyData));
        }


        public void LoadMonthlyData(string data)
        {
            //outputBox.Text = data;
            string json = @data;
            JsonTextReader reader = new JsonTextReader(new StringReader(json.Replace("  "," ")));

            string summary3x = "";
            string summary2x = "";
            string summary1x = "";
            string summarynoabsent = "";
            string summaryin = "";
            bool isRun = true;
            while (reader.Read() && isRun)
            {
                if (reader.Path.Equals("data.summary.IN (YES 3X)") && reader.TokenType.ToString().Equals("Integer") && reader.ValueType.ToString().Equals("System.Int64"))
                {
                    summary3x = reader.Value.ToString();
                }
                else if (reader.Path.Equals("data.summary.IN (YES 2X)") && reader.TokenType.ToString().Equals("Integer") && reader.ValueType.ToString().Equals("System.Int64"))
                {
                    summary2x = reader.Value.ToString();
                }
                else if (reader.Path.Equals("data.summary.IN (YES 1X)") && reader.TokenType.ToString().Equals("Integer") && reader.ValueType.ToString().Equals("System.Int64"))
                {
                    summary1x = reader.Value.ToString();
                }
                else if (reader.Path.Equals("data.summary.NO ABSENT") && reader.TokenType.ToString().Equals("Integer") && reader.ValueType.ToString().Equals("System.Int64"))
                {
                    summarynoabsent = reader.Value.ToString();
                }
                else if (reader.Path.Equals("data.summary.IN") && reader.TokenType.ToString().Equals("Integer") && reader.ValueType.ToString().Equals("System.Int64"))
                {
                    summaryin = reader.Value.ToString();
                }
                else if (reader.Path.Equals("data.field"))
                {
                    isRun = false;
                }
            }

            int i = 0;
            isRun = true;
            List<string> fieldList = new List<string>();
            while (reader.Read() && isRun)
            {
                if (reader.Path.Equals("data.field[" + i.ToString() + "]"))
                {
                    fieldList.Add(reader.Value.ToString());
                    i++;
                }
                else if (reader.Path.Equals("data.item"))
                {
                    isRun = false;
                }

            }
            List<string> machineList = new List<string>();
            List<string> dateList = new List<string>();
            List<string> lateList = new List<string>();

            int j = 0;
            int k = 0;
            isRun = true;
            while (reader.Read() && isRun)
            {
                if (reader.Path.Equals("data.item[" + j.ToString() + "][" + k.ToString() + "]"))
                {
                    if (k == 0)
                    {
                        machineList.Add(reader.Value.ToString());
                        k = 1;
                    }
                    else if (k == 1)
                    {
                        dateList.Add(reader.Value.ToString());
                        k = 2;
                    }
                    else if (k == 2)
                    {
                        lateList.Add(reader.Value.ToString());
                        j++;
                        k = 0;
                    }
                }
                else if (reader.Path.Equals("data"))
                {
                    isRun = false;
                }

            }
            App.MonthViewModel.ClearAllItems();
            App.MonthViewModel.AddItem("YES 3X : " + summary3x + " hari", "YES 2X : " + summary2x + " hari", "YES 1X : " + summary1x + " hari", "NO ABSENT : " + summarynoabsent + " hari", "IN : " + summaryin + " hari");
            for (int x = 0; x < machineList.Count; x++)
            {
                App.MonthViewModel.AddItem(machineList.ElementAt(x), dateList.ElementAt(x), lateList.ElementAt(x), "", "");
            }
            bulananList.ItemsSource = App.MonthViewModel.Items;
            loadMonthLyList.IsEnabled = true;
            loadingMonthlyIndicator.Visibility = Visibility.Collapsed;
        }
    }
}