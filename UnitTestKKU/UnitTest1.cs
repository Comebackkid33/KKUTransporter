using System;
using KKU_DEMO.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;


namespace UnitTestKKU
{
    [TestClass]
    public class UnitTest1
    {

        private AccountController controller;
        private ViewResult result;
        private string returnUrl { get; set; }
    
        [TestInitialize]
        public void SetupContext()
        {
            returnUrl = "Test";
            controller = new AccountController();
            result = controller.Login(returnUrl) as ViewResult;
          
        }

        [TestMethod]
        public void LoginViewResultNotNull()
        {
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginViewEqualIndexCshtml()
        {          
            Assert.AreEqual("Login", result.ViewName);
        }

        [TestMethod]
        public void LoginStringInViewbag()
        {
           
            Assert.AreEqual(returnUrl, result.ViewBag.returnUrl);
        }
    }
}
