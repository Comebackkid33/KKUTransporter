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
            string[] lineReadIn = new string[3];
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

                        bool flag = false;
                        List<byte> res1 = new List<byte>();
                        List<byte> res2 = new List<byte>();

                        for (var i = 0; i < resVals.Length; i++)
                        {
                           
                            if (resVals[i] == 0x000d)
                            {
                                flag = true;
                                i++;
                            }
                            if (flag == false)
                            {
                                res1.Add( resVals[i]); 
                            }
                            else
                            {
                                res2.Add(resVals[i]);
                            }
                        }

                        string[] c = new string[2];
                        // конвертим в строку
                        c[0] = System.Text.Encoding.ASCII.GetString(res1.ToArray());
                        c[1] = System.Text.Encoding.ASCII.GetString(res2.ToArray());


                        string curPattern = @"5TOP|-?\d+\.\d+|0";
                        string totalPattern = @"\d+\.\d*|0";

                        Regex curRegex = new Regex(curPattern);
                        Regex totalRegex = new Regex(totalPattern);

                        var curMatch = curRegex.Match(c[0]);
                        var totalMatch = curRegex.Match(c[1]);


                        lineReadIn[1] = curMatch.Value;
                        lineReadIn[2] = totalMatch.Value;

                        if (lineReadIn[2].EndsWith("."))
                            lineReadIn[2] += "0";

                        Console.WriteLine( DateTime.Now);
                        Console.WriteLine("Получено: --- " + c[0] +" | "+ c[1]);
                        Console.WriteLine("Текущие показания: --- "+lineReadIn[1]+ " Всего: --- " +lineReadIn[2]);
                        Console.WriteLine();
                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine();


                        SendReq(lineReadIn, Url);
                        lineReadIn[1] = "";
                        lineReadIn[2] = "";
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


        public static void SendReq(string[] body, string Url)
        {
            var client = new HttpClient();
            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(body, Formatting.None, jsSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.PutAsync(Url, content);


        }
    }
}

   

