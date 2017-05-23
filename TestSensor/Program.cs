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
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;


namespace TestSensor
{
    internal class Program
    {
       


        private delegate void preventCrossThreading(string x);

        private preventCrossThreading accessControlFromCentralThread;

        private static void Main(string[] args)
        {
            var Url = "http://192.168.0.36/KKU_DEMO/API/WeightApi";

            Random r = new Random();
            int range = 1;
            bool stop = false;

             string Token;
             string[] lineReadIn = new string[3];

            Console.WriteLine("Введите ключ датчика:");
            Token = Console.ReadLine();

            if (Token.Length == 0)
            {
                Token = "XXXX-XXXX-XXXX-XXXX";
            }

            lineReadIn[0] = Token;
            lineReadIn[2] = "0";
            try
            {
                while (true)
                {
                    Console.WriteLine("Выберете тип моделирования состояния датчиков:");
                    Console.WriteLine("1 - OK");
                    Console.WriteLine("2 - NOLOAD");
                    Console.WriteLine("3 - STOP");

                    int pos = Int32.Parse(Console.ReadLine());


                    switch (pos)
                    {
                        case 1:
                        {
                            stop = false;
                            range = 1;
                            break;
                        }
                        case 2:
                        {
                            stop = false;
                            range = 100;
                            break;
                        }
                        case 3:
                        {
                            stop = true;
                            break;
                        }
                    }


                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            try
                            {
                                double rDouble = r.NextDouble()/range;

                                if (stop)
                                {
                                    lineReadIn[1] = "5TOP";
                                }
                                else
                                {
                                    lineReadIn[1] = rDouble.ToString();
                                    lineReadIn[2] = (lineReadIn[2].ToDouble() + rDouble).ToString();
                                }


                                Console.WriteLine(DateTime.Now);

                                Console.WriteLine("Текущие показания: --- " + lineReadIn[1] + " Всего: --- " +
                                                  lineReadIn[2]);
                                Console.WriteLine();
                                Console.WriteLine("-------------------------------------------------------");
                                Console.WriteLine();


                                SendReq(lineReadIn, Url);
                                lineReadIn[1] = "";

                                Thread.Sleep(3000);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                Thread.Sleep(3000);
                            }
                        }
                    } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
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

    public static class StringExtension
    {
        public static double ToDouble(this string s)
        {
            double result = 0;
            try
            {
                result = Double.Parse(s);
            }
            catch (Exception)
            {
                try
                {
                    s = s.Replace(".", ",");
                    result = Double.Parse(s);
                }
                catch (Exception)
                {
                    try
                    {
                        s = s.Replace(",", ".");
                        result = Double.Parse(s);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result;
        }
    }
}



