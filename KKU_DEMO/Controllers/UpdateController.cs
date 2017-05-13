using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;
using Newtonsoft.Json;

namespace KKU_DEMO.Controllers
{
    public class UpdateController : Controller
    {
        [HttpPut]
        public ActionResult Index()
        {
            KKUContext db = new KKUContext();

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            string[] input = null;
            try
            {
                // assuming JSON.net/Newtonsoft library from http://json.codeplex.com/
                input = JsonConvert.DeserializeObject<string[]>(json);
            }

            catch (Exception ex)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (input != null)
            {

                var token = input[0];
                var sensor = db.Sensor
                    .FirstOrDefault(u => u.Token == token);
                if (sensor != null)
                {
                    sensor.CurWeight = input[1];
                    sensor.TotalWeight = Single.Parse(input[2]);

                    if (sensor.CurWeight == "5TOP" || sensor.CurWeight == "")
                    {
                        sensor.StateEnum = StateEnum.STOP;
                        sensor.DownTime += 3;
                    }
                    else
                    {
                        sensor.StateEnum = StateEnum.OK;
                    }


                }


                db.SaveChanges();


            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}