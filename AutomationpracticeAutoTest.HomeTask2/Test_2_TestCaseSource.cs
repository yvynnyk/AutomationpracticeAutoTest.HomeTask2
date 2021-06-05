using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.IO;

namespace AutomationpracticeAutoTest.HomeTask2.Test2
{
    [TestFixture]
    public class Test_2_TestCaseSource
    {
        IWebDriver driver;
        readonly string testURL = "http://automationpractice.com/index.php";
        readonly string errorMessageText = "There is 1 error";
        readonly string errorDescriptionText = "Invalid email address.";
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
            driver = new ChromeDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            driver.Manage().Window.Maximize();
        }

        [TestCaseSource("UserCredentials")]
        public void VerifyInvalidLogin(string userName, string userPassword)
        {
            driver.Navigate().GoToUrl(testURL);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            IWebElement buttonSignin = driver.FindElement(By.ClassName("login"));
            //other options:
            buttonSignin = driver.FindElement(By.XPath("//nav//div//a[@rel='nofollow']"));
            buttonSignin = driver.FindElement(By.CssSelector("a[title='Log in to your customer account']"));
            
            buttonSignin.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("login_form")));
            //other options:
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h3[text()='Already registered?']//parent::form")));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form[id=login_form]")));
  
            IWebElement emailAddress = driver.FindElement(By.Id("email"));
            //other options:
            emailAddress = driver.FindElement(By.XPath("//label[@for='email']//following-sibling::input"));
            emailAddress = driver.FindElement(By.CssSelector("#email"));

            emailAddress.SendKeys(userName);

            IWebElement password = driver.FindElement(By.Id("passwd"));
            //other options:
            password = driver.FindElement(By.XPath("//label[@for='passwd']//following-sibling::span//input"));
            password = driver.FindElement(By.CssSelector("input[type='password']"));

            password.SendKeys(userPassword);

            IWebElement buttonSigninSubmit = driver.FindElement(By.Name("SubmitLogin"));
            buttonSigninSubmit.Click();

            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(5);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(200);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Message = "Element is not found";

            fluentWait.Until(drv => drv.FindElement(By.CssSelector("div[id=center_column] > div.alert.alert-danger")));

            IWebElement errorMessage = driver.FindElement(By.CssSelector("div.alert.alert-danger > p"));
            IWebElement errorDescription = driver.FindElement(By.CssSelector("div.alert.alert-danger > ol > li"));

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
