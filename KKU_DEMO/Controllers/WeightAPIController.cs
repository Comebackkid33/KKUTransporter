using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using KKU_DEMO.DAL;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using Newtonsoft.Json;


namespace KKU_DEMO.Controllers
{
    public class WeightApiController : ApiController
    {
        private IncidentManager IncidentManager;
        private SensorManager SensorManager;
        private ShiftManager ShiftManager;
       

        public WeightApiController() : base()
        {
            IncidentManager = new IncidentManager();
            SensorManager = new SensorManager();
            ShiftManager = new ShiftManager();
        }
        //Переключение смен
        public HttpResponseMessage Get(int id)
        {
            try
            {
                this.ShiftManager.ChangeShift(id);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Cant find free shifts in requested factory");
                return response;
            }
               
            
            
        }

        //Считывание показаний 
        [HttpPut]
        public HttpResponseMessage Put([FromBody] string[] s)
        {
            KKUContext db = new KKUContext();
            if (s != null)
            {
                int maxOffCount = 15;
                StateEnum curState ;
                var token = s[0];
                Incident opIncident = null;
                var sensor = db.Sensor.FirstOrDefault(u => u.Token == token);

                Shift curShift = db.Shift.FirstOrDefault(sh => sh.FactoryId == sensor.FactoryId && sh.State== StateEnum.INPROCESS.ToString());

                if (curShift != null)
                { 
                    opIncident = db.Incident.FirstOrDefault(i => i.ShiftId == curShift.Id && i.State  == StateEnum.OPENED.ToString() && i.SensorId == sensor.Id );
                }
                if (sensor != null)
                {

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
                        {   sensor.NoLoadCount++;
                            sensor.DownTime += 3;
                        }
                    }
                    else
                    {
                        curState = StateEnum.OK;
                        sensor.NoLoadCount=0;
                    }

                    if (sensor.NoLoadCount > maxOffCount && curShift!=null && opIncident== null)
                    {
                        IncidentManager.AddIncident(curShift.Id,sensor.Id);
                       
                    }
                    if (sensor.NoLoadCount% maxOffCount ==0  && curShift != null && opIncident != null)
                    {
                       
                        IncidentManager.Notify(IncidentManager.GetIncident(curShift.Id, sensor.Id));

                    }

                    sensor.StateEnum = curState;
                    sensor.CurWeight = s[1];
                    sensor.TotalWeight = s[2].ToDouble();
                    sensor.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);


                    string path = HttpContext.Current.Server.MapPath("~") + "\\log\\sensor_" + sensor.Id + "_Log.txt";
                    //try
                    //{
                    //    StreamWriter sw = new StreamWriter(path, true);
                    //    sw.WriteLine(sensor.ToString());
                    //    sw.Close();
                    //}
                    //catch (Exception)
                    //{
                        
                    //    throw;
                    //}
                  
                   
                    db.SaveChanges();
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return new HttpResponseMessage(HttpStatusCode.Conflict);
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] string[] s)
        {
            KKUContext db = new KKUContext();
            if (s != null)
            {
                var curData = SensorManager.HexToString(s[1]);

                int maxOffCount = 15;
                StateEnum curState;
                Incident opIncident=null;
                var token = s[0];
                var sensor = db.Sensor
                    .FirstOrDefault(u => u.Token == token);
                Shift curShift = db.Shift.FirstOrDefault(sh => sh.FactoryId == sensor.FactoryId && sh.State == StateEnum.INPROCESS.ToString());

                if (curShift != null)
                {
                    opIncident = db.Incident.FirstOrDefault(i => i.ShiftId == curShift.Id && i.State == StateEnum.OPENED.ToString() && i.SensorId == sensor.Id);
                }
                if (sensor != null)
                {

                    if (curData[0].Contains("5TOP") || curData[1] == "")
                    {
                        curState = StateEnum.STOP;
                        if (curShift != null)
                        {
                            sensor.NoLoadCount++;
                            sensor.DownTime += 3;
                        }

                    }
                    else if (Math.Abs(sensor.TotalWeight - curData[1].ToDouble()) <= 0.01)
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

                    if (sensor.NoLoadCount > maxOffCount && curShift != null && opIncident == null)
                    {
                        IncidentManager.AddIncident(curShift.Id, sensor.Id);

                    }
                    if (sensor.NoLoadCount % maxOffCount == 0 && curShift != null && opIncident != null)
                    {

                        IncidentManager.Notify(IncidentManager.GetIncident(curShift.Id, sensor.Id));

                    }

                    sensor.StateEnum = curState;
                    sensor.CurWeight = curData[0];
                    sensor.TotalWeight = curData[1].ToDouble();
                    sensor.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);


                    string path = HttpContext.Current.Server.MapPath("~") + "\\log\\sensor_" + sensor.Id + "_Log.txt";
                    //try
                    //{
                    //    StreamWriter sw = new StreamWriter(path, true);
                    //    sw.WriteLine(sensor.ToString());
                    //    sw.Close();
                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}


                    db.SaveChanges();
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return new HttpResponseMessage(HttpStatusCode.Conflict);
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
    
