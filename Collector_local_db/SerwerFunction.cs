using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Newtonsoft.Json;

namespace Collector_local_db
{




    public static class SerwerFunction
    {

        public static int Uid = -1;
        public static string login;
        public static string password;


        public static async Task<object> Getfromserver<TClass>(string pathname, string type, object input)
        {
            var fullpathname = "http://msk.mini.pw.edu.pl/Collector/api/" + pathname;
            var request = (HttpWebRequest)WebRequest.Create(fullpathname);
            request.ContentType = "text/json";
            request.Method = type;
            
            if (type == "POST" || type =="PUT" || type=="DELETE")
            {
                try
            {
              
                    using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                    {
                        var json = JsonConvert.SerializeObject(input);

                        streamWriter.Write(json);
                    }


                    var response = (HttpWebResponse) await request.GetResponseAsync();
                    string resultJson;
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        resultJson = streamReader.ReadToEnd();
                    }
                    var restult = JsonConvert.DeserializeObject<TClass>(resultJson);

                return restult;
            }

                catch(Exception e)
                {
                    ErrorDialog(e.Message);
                
                }
            }
            else
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var json = await httpClient.GetStringAsync(fullpathname);

                        return JsonConvert.DeserializeObject<TClass>(json);
                    }
                }
                catch (Exception e)
                {
                    ErrorDialog(e.Message);
                 
                }
            }
            return null;
        }

        private static void ErrorDialog(string message)
        {
            string problem;
            if (message ==
                "An error occurred while sending the request. The text associated with this error code could not be found.\r\n\r\nThe certificate authority is invalid or incorrect\r\n")
                problem = "You dont have access to ssl certificate";
            else
                problem = (message == "Input string was not in a correct format.")
                    ? "There is something wrong with numbers"
                    : message;
            


            var msgbox = new MessageDialog(problem);

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 0 });

            var res = msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {

            }
            return;

        }


    }
}
