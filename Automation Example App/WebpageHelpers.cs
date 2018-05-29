using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.Collections.Generic;
using System.IO;

namespace Automation_Example_App
{
    public class WebpageHelpers
    {
        // Open the calculator webpage and return the driver object so it can be referenced in the main form
        public IWebDriver OpenWebpage(string hyperlink)
        {
            if(!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\debug\\IEDriverServer.exe"))
            {
                File.Copy(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Resources\\IEDriverServer.exe"),
                    Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\debug\\IEDriverServer.exe");
            }

            IWebDriver Driver = new InternetExplorerDriver();
            Driver.Navigate().GoToUrl(hyperlink);
            return Driver;
        }

        public void CloseDriver(IWebDriver driver)
        {
            driver.Quit();
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
            foreach (var item in driver.FindElements(By.ClassName(searchClass)))
            {
                if (item.Text == searchText)
                {
                    return item;
                }
            }

            return null;
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
            foreach (var r in driver.FindElements(By.Id(searchId)))
            {
                if (r.Text == searchText)
                {
                    return r;
                }
            }

            return null;
        }

        public List<IWebElement> GetClockElelements(IWebDriver driver)
        {
            var results = driver.FindElements(By.ClassName("tad-sortable-item"));
            List<IWebElement> clocks = new List<IWebElement>();

            foreach (var r in results)
            {
                if (r.Text != "")
                {
                    clocks.Add(r);
                }
            }

            return clocks;
        }
    }
}