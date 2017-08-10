using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace KKU_COM
{
    internal class Program
    {
        private const string Token = "XXXX-XXXX-XXXX-XXXX";


        private delegate void preventCrossThreading(string x);

        private preventCrossThreading accessControlFromCentralThread;

        private static void Main(string[] args)
        {
            var Url = "http://kovrovku.ru/API/WeightApi";

            SerialPort Com1 = new SerialPort("Com1", 19200, Parity.None, 8, StopBits.One);
            string[] lineReadIn = new string[2];
            lineReadIn[0] = Token;
            string hex = "FF01C647FFFF";
           
            byte[] vals = StringToByteArray(hex);
            byte[] resVals = new byte[26];
            


            try
            {
                Com1.Open(); //открываем порт
                while (true)
                {
                    try
                    {
                        //считывем показания в байтах
                        if (Com1.BytesToRead > 0)
                        {
                            Com1.Read(resVals, 0, resVals.Length);
                        }

                        List<byte> res = new List<byte>();
                       
                    
                        for (var i = 0; i < resVals.Length; i++)
                        {
                            
                             res.Add( resVals[i]); 
                            
                        }

                        string c = ByteArrayToString(res.ToArray());

                        lineReadIn[1] = c;
                       

                        Console.WriteLine( DateTime.Now);
                        Console.WriteLine("Получено: --- " + c );
                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine();

                  

                        Task<string> result = SendReq(lineReadIn, Url);
                        var finalResult = result.Result;
                        Console.WriteLine(finalResult);
                        Console.WriteLine("-------------------------------------------------------");

                        lineReadIn[1] = "";
                        Thread.Sleep(3000);
                        Com1.Write(vals, 0, vals.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Thread.Sleep(3000);
                        Com1.Write(vals, 0, vals.Length);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars/2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static async Task<string> SendReq(string[] body, string Url)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            var client = new HttpClient();
            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(body, Formatting.None, jsSettings);
            Console.WriteLine(Url);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Url, content);
            var contents = await response.Content.ReadAsStringAsync();



        
            return response.ToString();
        }
    }
}

   

