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


        public WeightApiController() 
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

        //Считывание показаний в десятичной системе 
        [HttpPut]
        public HttpResponseMessage Put([FromBody] string[] s)
        {
            if (s != null)
            {
                try
                {
                    SensorManager.UpdateDecimal(s);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    response.Content = new StringContent(e.Message);
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }


        [HttpPost]
        public HttpResponseMessage Post([FromBody] string[] s)
        {
            if (s != null)
            {
                try
                {
                    SensorManager.UpdateHex(s);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    response.Content = new StringContent("Cant update selected sensor");
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}
    
