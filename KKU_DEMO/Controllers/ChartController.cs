using KKU_DEMO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KKU_DEMO.DAL;

namespace KKU_DEMO.Controllers
{
    public class ChartController : Controller
    {
       
        public ContentResult JSON()
        {
            KKUContext db = new KKUContext();
            List<DataPoint> dataPoints = new List<DataPoint>();
            var sensor = db.Sensor.Find(1);
            dataPoints.Add(new DataPoint( sensor.TotalWeight, DateTime.UtcNow
               .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
               .TotalMilliseconds));
            

            JsonSerializerSettings _jsonSetting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return Content(JsonConvert.SerializeObject(dataPoints, _jsonSetting), "application/json");
        }

    }
}