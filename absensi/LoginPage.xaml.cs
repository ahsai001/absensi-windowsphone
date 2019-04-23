using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using absensi.AhsaiLib;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
namespace absensi
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            loginButton.IsEnabled = false;
            loadingIndicator.Visibility = Visibility.Visible;
            string user = "ahmad.saifullah";
            if (!usernameInput.Text.Equals(""))
            {
                user = usernameInput.Text;
            }
            string pass = "doxgetron";
            if (!passwordInput.Password.Equals(""))
            {
                if (usernameInput.Text.Equals(""))
                {
                    pass = pass + passwordInput.Password;
                }
                else
                {
                    pass = passwordInput.Password;
                }
            }
            //client.GetDataFrom("http://202.61.124.20:8270/tes/absen/api/login.php", "username=" + usernameInput.Text + "&password=" + passwordInput.Password, new AhsaiLib.HttpClient.EventHandlerAfterConnection(ConnectionHandler));
            client.GetDataFrom("http://202.61.124.20:8270/tes/absen/api/login.php", "username=" + user + "&password=" + pass, new AhsaiLib.HttpClient.EventHandlerAfterConnection(ConnectionHandler));
        
        }
        public void ConnectionHandler(string data)
        {
            //outputBox.Text = data;
            string json = @data;

            JsonTextReader reader = new JsonTextReader(new StringReader(json));

            bool isCookies = false;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Debug.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    if(isCookies)
                        App.cookies = reader.Value.ToString();
                    if (reader.Value.Equals("cookies"))
                        isCookies = true;
                    else
                        isCookies = false;
                }
                else
                {
                    Debug.WriteLine("Token: {0}", reader.TokenType);
                }
            }

            loadingIndicator.Visibility = Visibility.Collapsed;
            loginButton.IsEnabled = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
        }
    }
}