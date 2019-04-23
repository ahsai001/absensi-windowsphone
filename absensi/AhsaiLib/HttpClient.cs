using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace absensi.AhsaiLib
{
    class HttpClient
    {
        private string data = "";
        private string url = "";
        public delegate void EventHandlerAfterConnection(string response);
        EventHandlerAfterConnection handler;
        public void GetDataFrom(string url, string data, EventHandlerAfterConnection handler)
        {
            this.data = data;
            Debug.WriteLine(data);
            this.url = url;
            this.handler = handler;
            System.Uri myUri = new System.Uri(url);
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), myRequest);

        }
        private void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);

            // Create the post data
            string postData = ""+this.data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Start the web request
            myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
        }
        private void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
            if (response != null)
            {
                using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    string result = httpWebStreamReader.ReadToEnd();
                    //For debug: show results
                    //Debug.WriteLine(result);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.handler(result);
                        this.data = "";
                        this.url = "";
                        this.handler = null;
                    });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.handler("");
                    this.data = "";
                    this.url = "";
                    this.handler = null;
                });
            }
        }
    }
   
}
