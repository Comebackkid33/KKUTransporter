using System;
using System.Collections.Generic;
using System.Web.Mvc;
using KKU_DEMO.Controllers;
using KKU_DEMO.Managers;
using KKU_DEMO.Models;
using KKU_DEMO.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestKKU
{
    [TestClass]
    public class UnitTestShiftGet
    {
        private ShiftController controller;
        private ViewResult result;
        Mock<IManager<Shift>> mock = new Mock<IManager<Shift>>();

        [TestInitialize]
        public void SetupContext()
        {
            mock.Setup(a => a.GetAll()).Returns(new List <Shift>());
            controller = new ShiftController();
            result = controller.GetShifts(new ShiftSearchModel()) as ViewResult;

        }
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsNotNull(result.Model);
        }
    }
}
