using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ShiftChanger
{
    class Program
    {
        static void Main(string[] args)
        {   

            string Url = "http://kovrovku.ru/API/WeightApi/1"; 

            var client = new HttpClient();
           
            var c = client.GetAsync(Url).Result.StatusCode;
            Console.WriteLine(c);
        }
    }
}
