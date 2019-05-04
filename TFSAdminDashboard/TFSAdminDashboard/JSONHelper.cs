using System;
using System.IO;
using System.Net;

namespace TFSAdminDashboard
{
    public class JSONHelper
    {
        public string GetTFSJsonData(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                            TFSAdminDashboard.Properties.Settings.Default.Password,
                                                            TFSAdminDashboard.Properties.Settings.Default.Domain
                                                            );
                request.Method = "GET";
                request.ContentType = "application/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.Write(response.StatusCode);
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}