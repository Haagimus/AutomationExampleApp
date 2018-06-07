using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;

namespace Automation_Example_App
{
    public class WebpageHelpers
    {
        // Open the calculator webpage and return the driver object so it can be referenced in the main form
        public InternetExplorerDriver OpenWebpage(string hyperlink)
        {
            if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\debug\\IEDriverServer.exe"))
            {
                File.Copy(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Resources\\IEDriverServer.exe"),
                    Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\debug\\IEDriverServer.exe");
            }

            InternetExplorerOptions options = new InternetExplorerOptions
            {
                IgnoreZoomLevel = true
            };
            InternetExplorerDriver Driver = new InternetExplorerDriver(options);

            try
            {
                Driver.Manage().Window.Size = new System.Drawing.Size(800, 600);
                Driver.Navigate().GoToUrl(hyperlink);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Driver.Quit();
                Driver = null;
            }

            return Driver;
        }

        /// <summary>
        /// Search for a web element in a given web page accessed via a driver
        /// </summary>
        /// <param name="driver">The web driver page that will be searched</param>
        /// <param name="searchClass">The class type that will be enumerated through</param>
        /// <param name="searchText">The text you want to search for in the enumerated classes</param>
        /// <returns>The web element that matches the search criteria or returns nothing.</returns>
        public IWebElement GetElementByClass(IWebDriver driver, string searchClass, string searchText)
        {
            IWebElement result = null;

            foreach (var item in driver.FindElements(By.ClassName(searchClass)))
            {
                if (item.Text == searchText)
                {
                    result = item;
                }
            }

            return result;
        }

        /// <summary>
        /// Search for a web element in a given web page accessed via a driver
        /// </summary>
        /// <param name="driver">The web driver page that will be searched</param>
        /// <param name="searchId">The id name that will be enumerated through</param>
        /// <param name="searchText">The text you want to search for in the enumerated ids</param>
        /// <returns>The web element that matches the search criteria or returns nothing.</returns>
        public IWebElement GetElementByID(IWebDriver driver, string searchId, string searchText)
        {
            IWebElement result = null;

            foreach (var item in driver.FindElements(By.Id(searchId)))
            {
                if (item.Text == searchText)
                {
                    result = item;
                }
            }

            return result;
        }
    }
}