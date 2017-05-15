using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;

namespace KKU_DEMO.Managers
{
    public class SensorManager
    {
        private KKUContext db;

        public SensorManager()
        {
            db = new KKUContext();
        }

        public List<Sensor> GetAll()
        {
            return db.Sensor.ToList();
        }

        public Sensor GetById(int id)
        {
            return db.Sensor.Find(id);
        }
        public Sensor GetByFactoryId(int id,string name)
        {
            return  db.Sensor.FirstOrDefault(s => s.FactoryId == id && s.Name == name);
        }

        public void Create (Sensor sensor)
        {
            sensor.StateEnum = StateEnum.STOP;
            db.Entry(sensor).State = EntityState.Added;
            db.SaveChanges();
        }



        public string[] HexToString(string s)
        {
            var HexArrey = ToByteArray(s);
            List<byte> res1 = new List<byte>();
            List<byte> res2 = new List<byte>();
            bool flag = false;

            for (var i = 0; i < HexArrey.Length; i++)
            {

                if (HexArrey[i] == 0x000d)
                {
                    flag = true;
                    i++;
                }
                if (flag == false)
                {
                    res1.Add(HexArrey[i]);
                }
                else
                {
                    res2.Add(HexArrey[i]);
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

            string[] output = new string[2];
            output[0] = curMatch.Value;
            output[1] = totalMatch.Value;

            if (output[1].EndsWith("."))
                output[1] += "0";

            return output;
        }

        public static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}