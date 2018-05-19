using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.Collections.Generic;

namespace Automation_Example_App
{
    public class WebpageHelpers
    {
        private static IWebDriver driver;

        public static IWebDriver Driver { get => driver; set => driver = value; }

        // Open the calculator webpage and return the driver object so it can be referenced in the main form
        public static IWebDriver OpenWebpage(string hyperlink)
        {
            Driver = new InternetExplorerDriver();
            Driver.Navigate().GoToUrl(hyperlink);
            return Driver;
        }

        // Close the driver console and webpage
        public static void CloseWebpage(IWebDriver driver)
        {
            driver.Dispose();
        }

        /// <summary>
        /// Search for a web element in a given web page accessed via a driver
        /// </summary>
        /// <param name="driver">The web driver page that will be searched</param>
        /// <param name="searchClass">The class type that will be enumerated through</param>
        /// <param name="searchText">The text you want to search for in the enumerated classes</param>
        /// <returns>The web element that matches the search criteria or returns nothing.</returns>
        public static IWebElement GetElementByClass(IWebDriver driver, string searchClass, string searchText)
        {
            var results = driver.FindElements(By.ClassName(searchClass));

            foreach (var r in results)
            {
                if (r.Text == searchText)
                {
                    return r;
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
        public static IWebElement GetElementByID(IWebDriver driver, string searchId, string searchText)
        {
            var results = driver.FindElements(By.Id(searchId));

            foreach (var r in results)
            {
                if (r.Text == searchText)
                {
                    return r;
                }
            }

            return null;
        }

        public static List<IWebElement> GetClockElelements(IWebDriver driver)
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