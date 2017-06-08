using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Repositories;

namespace KKU_DEMO.Managers
{
    public class SensorManager
    {
        private IRepository<Sensor> SensorRepo;
        private IncidentManager IncidentManager;
        private ShiftManager ShiftManager;
        private FileManager FileManager;

        public SensorManager()
        {
            SensorRepo = new SensorRepository();
            IncidentManager = new IncidentManager();
            ShiftManager = new ShiftManager(this);
            FileManager = new FileManager();
        }

        public List<Sensor> GetAll()
        {
            return SensorRepo.GetList().ToList();
        }

        public Sensor GetById(int id)
        {
            return SensorRepo.Get(id);
        }

        public Sensor GetByToken(string token)
        {
            return SensorRepo.GetList().FirstOrDefault(s => s.Token == token);
        }

        public Sensor GetByFactoryId(int? id, string name)
        {
            return SensorRepo.GetList().FirstOrDefault(s => s.FactoryId == id && s.Name == name);
        }

        public void Create(Sensor sensor)
        {
            sensor.StateEnum = StateEnum.STOP;
            SensorRepo.Create(sensor);
            SensorRepo.Save();
        }

        /// <summary>
        /// Апдейт показаний датчика, если новые пришли в десятичной системе
        /// </summary>
        /// <param name="s">JSON[1:Token;2:Текущие показания;3:Всего]</param>
        public void UpdateDecimal(string[] s)
        {
            //Количество тиков до объявления инцидента
            int maxOffCount = 15;
            //Текущее состояние датчика
            StateEnum curState;

            var token = s[0];

            Incident opIncident = null;
            var sensor = GetByToken(token);

            Shift curShift = ShiftManager.GetByFactoryId(sensor.FactoryId, StateEnum.INPROCESS.ToString()).First();
               
            if (sensor != null && curShift != null)
            {
                opIncident = IncidentManager.GetIncident(curShift.Id, sensor.Id);

                if (s[1].Contains("5TOP") || s[1] == "")
                {
                    curState = StateEnum.STOP;
                    if (curShift != null)
                    {
                        sensor.NoLoadCount++;
                        sensor.DownTime += 3;
                    }
                }
                else if (Math.Abs(sensor.TotalWeight - s[2].ToDouble()) <= 0.01)
                {
                    curState = StateEnum.NOLOAD;

                    if (curShift != null)
                    {
                        sensor.NoLoadCount++;
                        sensor.DownTime += 3;
                    }
                }
                else
                {
                    curState = StateEnum.OK;
                    sensor.NoLoadCount = 0;
                }

                if (sensor.NoLoadCount > maxOffCount  && opIncident == null)
                {
                    IncidentManager.AddIncident(curShift.Id, sensor.Id);
                }
                if (sensor.NoLoadCount!=0 && sensor.NoLoadCount % maxOffCount == 0  && opIncident != null)
                {
                    IncidentManager.Notify(IncidentManager.GetIncident(curShift.Id, sensor.Id));
                }

                sensor.StateEnum = curState;
                sensor.CurWeight = s[1];
                sensor.TotalWeight = s[2].ToDouble();
                sensor.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);


                
                try
                {
                    FileManager.WrightData("\\sensor_" + sensor.Id + "_Log.txt",sensor.ToString());
                }
                catch (Exception)
                {
                    throw;
                }

                SensorRepo.Update(sensor);
                SensorRepo.Save();
                
            }
            else
            {
                throw new Exception("Ошибка поиска смены или датчика");
            }
        }

        public void UpdateHex(string[] s)
        {
            var HexData = HexToString(s[1]);
            string [] output = new string[3];
            output[0] = s[0];
            output[1] = HexData[0];
            output[2] = HexData[1];
            UpdateDecimal(output);

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
            var totalMatch = totalRegex.Match(c[1]);

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
            byte[] bytes = new byte[NumberChars/2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i/2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
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