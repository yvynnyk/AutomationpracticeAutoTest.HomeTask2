using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;

namespace AutomationpracticeAutoTest.HomeTask2.Test2
{
    [TestFixture]
    public class Test_2_TestCaseSource
    {
        IWebDriver driver;
        string testURL = "http://automationpractice.com/index.php";
        string errorMessageText = "There is 1 error";
        string errorDescriptionText = "Invalid email address.";
        public static IEnumerable<TestCaseData> UserCredentials
        {
            get
            {
                yield return new TestCaseData("JohnDoe", "passw0rd");
                yield return new TestCaseData("LiliaJY", "isNotMe");
                yield return new TestCaseData("GoingTo", "BeAuto!");
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [TestCaseSource("UserCredentials")]
        public void VerifyInvalidLogin(string userName, string userPassword)
        {
            driver.Navigate().GoToUrl(testURL);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            IWebElement buttonSignin = driver.FindElement(By.ClassName("login"));
            buttonSignin.Click();

            IWebElement emailAddress = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            emailAddress.SendKeys(userName);

            IWebElement password = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("passwd")));
            password.SendKeys(userPassword);

            IWebElement buttonSigninSubmit = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SubmitLogin")));
            buttonSigninSubmit.Click();

            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(5);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(200);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Message = "Element is not found";

            IWebElement errorMessage = fluentWait.Until(drv => drv.FindElement(By.CssSelector("div.alert.alert-danger > p")));
            IWebElement errorDescription = fluentWait.Until(drv => drv.FindElement(By.CssSelector("div.alert.alert-danger > ol > li")));

            Assert.Multiple(() =>
            {
                Assert.That(errorMessage.Text, Is.EqualTo(errorMessageText), $"Message '{errorMessageText}' is not displayed");
                Assert.That(errorDescription.Text, Is.EqualTo(errorDescriptionText), $"Message '{errorDescriptionText}' is not displayed");
            });
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
