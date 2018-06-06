using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using Automation_Example_App.Tests;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing;
using Automation_Example_App.Resources;
using System.Text.RegularExpressions;
using OpenQA.Selenium.IE;

namespace Automation_Example_App
{
    public partial class Form1 : Form
    {
        private readonly List<IWebElement> calcPageElements = new List<IWebElement>();
        private readonly Dictionary<string, IWebElement> kvpElements = new Dictionary<string, IWebElement>();
        private readonly List<Clock> clocks = new List<Clock>();
        private readonly List<IWebElement> clockDisplays = new List<IWebElement>();
        private InternetExplorerDriver calcDriver, clockDriver;
        private readonly List<bool> results = new List<bool>();
        private readonly List<string> calcElements = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "–", "×", "/", "=", "C", "0" };
        private readonly List<string> calcNames = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "plus", "minus", "times", "divide", "equals", "clear", "result" };
        private readonly SynchronizationContext synchronizationContext;
        private readonly int testCount = 2;
        private delegate void UpdateUIDelegate(Control formObject, string text);

        public static WebpageHelpers _WebHelper = new WebpageHelpers();
        public static MathOperations _MathOperations = new MathOperations();
        public static ClockOperations _ClockOperations = new ClockOperations();

        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void UpdateUI(Control formObject, string text)
        {
            if (!formObject.InvokeRequired)
            {
                formObject.Text = text;
                formObject.Refresh();
            }
            else
            {
                UpdateUIDelegate updateUI = new UpdateUIDelegate(UpdateUI);
                BeginInvoke(updateUI, new object[] { formObject, text });
            }
        }

        private void UpdateUI(Control formObject, bool enabled)
        {
            if (!formObject.InvokeRequired)
            {
                formObject.Enabled = enabled;
                formObject.Refresh();
            }
            else
            {
                UpdateUIDelegate updateUI = new UpdateUIDelegate(UpdateUI);
                BeginInvoke(updateUI, new object[] { formObject, enabled });
            }
        }

        private async void BtnOpenCalculator_Click(object sender, EventArgs e)
        {
            UpdateUI(LblStatus, "Opening Calculator Webpage.");
            UpdateUI(btnOpenCalculator, false);
            UpdateUI(BtnOpenClocks, false);
            await Task.Run(() => calcDriver = _WebHelper.OpenWebpage("http://www.calculator.net")).ConfigureAwait(false);
            UpdateUI(LblStatus, "");

            StartCalculatorTests();
        }

        private async void BtnOpenClocks_Click(object sender, EventArgs e)
        {
            UpdateUI(LblStatus, "Opening Clock webpage.");
            UpdateUI(btnOpenCalculator, false);
            UpdateUI(BtnOpenClocks, false);
            await Task.Run(() => clockDriver = _WebHelper.OpenWebpage("https://www.timeanddate.com/worldclock/personal.html")).ConfigureAwait(false);
            UpdateUI(LblStatus, "");

            StartClockTests();
        }

        private void StartCalculatorTests()
        {
            kvpElements.Clear();
            UpdateUI(LblStatus, "Finding number buttons.");
            // Find all the buttons with the class 'scinm' which are the number buttons
            for (var i = 0; i < 10; i++)
            {
                IWebElement btn = _WebHelper.GetElementByClass(calcDriver, "scinm", calcElements[i]);
                calcPageElements.Add(btn);
            }

            UpdateUI(LblStatus, "Finding operations buttons.");
            // Find all the buttons with the class 'sciop' which are the operation buttons
            for (var i = 10; i < 14; i++)
            {
                IWebElement operation = _WebHelper.GetElementByClass(calcDriver, "sciop", calcElements[i]);
                calcPageElements.Add(operation);
            }

            UpdateUI(LblStatus, "Finding equal and clear buttons.");
            // Find the two buttons with the class 'scieq' which are the equal and clear buttons
            for (var i = 14; i < 16; i++)
            {
                IWebElement btn = _WebHelper.GetElementByClass(calcDriver, "scieq", calcElements[i]);
                calcPageElements.Add(btn);
            }

            UpdateUI(LblStatus, "Finding output window.");
            // Find the output window
            IWebElement output = _WebHelper.GetElementByID(calcDriver, "sciOutPut", calcElements[16]);
            calcPageElements.Add(output);

            // This is not really necessary but adding key value pairs to the list of controls makes them easier to reference later on
            for (int i = 0; i < calcElements.Count; i++)
            {
                kvpElements.Add(calcNames[i], calcPageElements[i]);
            }

            //ExecuteAdditionTests();
            //ExecuteMultiplyTests();
            //ExecuteDivideTests();
            //ExecuteSubtractTests();
            //ExecuteRandomTests();
            TakeScreenshot(1);
            calcDriver.Dispose();
            UpdateUI(LblStatus, "Calculator tests complete. Driver closed.");

            UpdateUI(btnOpenCalculator, true);
            UpdateUI(BtnOpenClocks, true);
        }

        private void StartClockTests()
        {
            UpdateUI(LblStatus, "Finding clocks on clock page.");
            // Add the html nodes that contain the each clock and the associated display items
            foreach (var item in clockDriver.FindElements(By.ClassName("c-city__content")))
            {
                clockDisplays.Add(item);
            }

            // We need to find the script that contains all the clock information so we will search for it here
            var clock = clockDriver.FindElements(By.TagName("script"));
            var countClocks = 0;
            foreach (var item in clock)
            {
                var clockZone = item.GetAttribute("innerHTML");
                var location = clockZone;

                // If the items innerHTML length is 0 there is no content and it is not the script were looking for
                // If it has content and that content contains the string /time\/zones\ it is the script section we want
                if (clockZone.Length <= 0 && !clockZone.Contains("/time\\/zones\\")) continue;

                // Using this regex we count how many zones we find and add that to our count, this will give us the total
                // number of clocks on the page so we know how many of the following items to look for
                countClocks = Regex.Matches(clockZone, "zones").Count;
                for (var i = 0; i < countClocks; i++)
                {
                    // We now need to create the new clock objects so we can reference them later
                    // We are going to add the timezone and reference locations for each created item
                    clocks.Add(new Clock(_ClockOperations.getBetween(clockZone, "title=\\\"", "\\\">"),
                        _ClockOperations.getBetween(location, "name\":\"", "\",\"co")));

                    clockZone = clockZone.Substring(clockZone.IndexOf(clocks[i]._clockZone + "\\\">"));
                    location = location.Substring(location.IndexOf(clocks[i]._location + "\",\"co"));
                }
            }

            // Now that we have all the controls let's run the tests
            VerifyClocks();
            VerifyMinuteHands();
            VerifyHourHands();
            VerifyClockAngles();
            TakeScreenshot(2);
            _WebHelper.CloseDriver(calcDriver);
            UpdateUI(LblStatus, "Clock tests complete. Driver closed.");

            UpdateUI(btnOpenCalculator, true);
            UpdateUI(BtnOpenClocks, true);
        }

        /// <summary>
        /// Execute addition tests.
        /// </summary>
        private void ExecuteAdditionTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the add function for each number
            for (int i = 0; i < testCount; i++)
            {
                UpdateUI(LblStatus, "Executing Addition Tests.");

                UpdateUI(LblAdditionTest, $"Addition Test: Checking {i} + {i}");
                // Run the function and add the result to the results list
                results.Add(_MathOperations.Addition(kvpElements, i.ToString(), i.ToString(), i + i));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblAdditionTest, results.TrueForAll(b => b) ? "Addition Test: Passed" : "Addition Test: Failed");
        }

        /// <summary>
        /// Execute multiplication tests.
        /// </summary>
        private void ExecuteMultiplyTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the multiply function for each number
            for (int i = 0; i < testCount; i++)
            {
                UpdateUI(LblStatus, "Executing Multiplication Tests.");

                UpdateUI(LblMultiplicationTest, $"Multiplication Test: Checking {i} * {i}");
                // Run the function and add the result to the results list
                results.Add(_MathOperations.Multiplication(kvpElements, i.ToString(), i.ToString(), i * i));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblMultiplicationTest, results.TrueForAll(b => b) ? "Multiplication Test: Passed" : "Multiplication Test: Failed");
        }

        /// <summary>
        /// Execute division tests.
        /// </summary>
        private void ExecuteDivideTests()
        {
            results.Clear();

            // We know 0/0 will return an error so we pass an expected result of -1 to handle it in the function
            results.Add(_MathOperations.Division(kvpElements, "0", "0", -1));

            // Loop 0 through 10 and run the divide function for each number
            for (int i = 1; i < testCount; i++)
            {
                UpdateUI(LblStatus, "Executing Division Tests.");

                UpdateUI(LblDivisionTest, $"Division Test: Checking {i} / {i}");
                // Run the function and add the result to the results list
                results.Add(_MathOperations.Division(kvpElements, i.ToString(), i.ToString(), 1));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblDivisionTest, results.TrueForAll(b => b) ? "Division Test: Passed" : "Division Test: Failed");
        }

        /// <summary>
        /// Execute subtraction tests.
        /// </summary>
        private void ExecuteSubtractTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the subtract function for each number
            for (int i = 0; i < testCount; i++)
            {
                UpdateUI(LblStatus, "Executing Subtraction Tests.");

                UpdateUI(LblSubtractionTest, $"Subtraction Test: Checking {i} - {i}");
                // Run the function and add the result to the results list
                results.Add(_MathOperations.Subtraction(kvpElements, i.ToString(), i.ToString(), 0));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblSubtractionTest, results.TrueForAll(b => b) ? "Subtraction Test: Passed" : "Subtraction Test: Failed");
        }

        /// <summary>
        /// Execute random math operation tests.
        /// </summary>
        private void ExecuteRandomTests()
        {
            Random r = new Random();
            results.Clear();

            UpdateUI(LblStatus, "Executing Subtraction Tests.");
            // Now we will run ten random math tests and verify the results
            for (int i = 0; i < testCount; i++)
            {
                // r1 and r2 are the numbers we will send, r3 is used to selection the operation
                var r1 = r.Next(0, 10);
                var r2 = r.Next(0, 10);
                var r3 = r.Next(1, 5);

                switch (r3)
                {
                    case 1:
                        UpdateUI(LblRandomTest, $"Random Test: Checking {r1} + {r2}");
                        // Run the function and add the result to the results list
                        results.Add(_MathOperations.Addition(kvpElements, r1.ToString(), r2.ToString(), r1 + r2));
                        break;
                    case 2:
                        UpdateUI(LblRandomTest, $"Random Test: Checking {r1} * {r2}");
                        // Run the function and add the result to the results list
                        results.Add(_MathOperations.Multiplication(kvpElements, r1.ToString(), r2.ToString(), r1 * r2));
                        break;
                    case 3:
                        // We need to account for the possible outcomes of division where it may not just be a simple positive or negative number
                        if (r1 == 0 && r2 == 0)
                        {
                            UpdateUI(LblRandomTest, $"Random Test: Checking {r1} / {r2}");
                            // This is when 0 is divided by 0, we know it will throw an error so we send -1 and handle it in the function
                            results.Add(_MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), -1));
                        }
                        else if (r1 == 0 && r2 > 0)
                        {
                            UpdateUI(LblRandomTest, $"Random Test: Checking {r1} / {r2}");
                            // This is when we are dividing 0 by any number which will return a 0
                            results.Add(_MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), 0));
                        }
                        else if (r1 > 0 && r2 == 0)
                        {
                            UpdateUI(LblRandomTest, $"Random Test: Checking {r1} / {r2 + 1}");
                            // This is when we are dividing any number by 0, we just add 1 to 0 so we can perform the function
                            results.Add(_MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), r1 / (r2 + 1)));
                        }
                        else if (r2 > r1)
                        {
                            UpdateUI(LblRandomTest, $"Random Test: Checking {r1} / {r2}");
                            // This is when a decimal will be returned, the calculator will only display out to three decimal places so we format
                            // the string .net calculates because it has far more accuracy
                            results.Add(_MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), Math.Round(Double.Parse(string.Format("{0:N3}", (double)r1 / r2)))));
                        }
                        else
                        {
                            UpdateUI(LblRandomTest, $"Random Test: Checking {r1} / {r2}");
                            // This is when it's a straight forward division problem where the divisor is smaller than the number to be divided
                            results.Add(_MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), r1 / r2));
                        }
                        break;
                    case 4:
                        UpdateUI(LblRandomTest, $"Random Test: Checking {r1} - {r2}");
                        // Run the function and add the result to the results list
                        results.Add(_MathOperations.Subtraction(kvpElements, r1.ToString(), r2.ToString(), r1 - r2));
                        break;
                }
            }
            // Verify each result is true and return passed or failed
            UpdateUI(LblRandomTest, results.TrueForAll(b => b) ? "Random Test: Passed" : "Random Test: Failed");
        }

        /// <summary>
        /// Verify displayed clocks show the correct times.
        /// </summary>
        private void VerifyClocks()
        {
            results.Clear();
            UpdateUI(LblStatus, "Verifying displayed times are correct.");

            // First load the timezone text file and match the locale to the correct offset then add that to the correcpsonding clock object
            using (StreamReader sr = new StreamReader(File.OpenRead(@".\Resources\TimeZones.txt")))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    foreach (var clock in clocks)
                    {
                        // If the timezone in the stream matches that of the current clock then we found the correct timezone and
                        // need to add the corresponding offset to the clock object
                        if (line.IndexOf(clock._clockZone, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int start = line.LastIndexOf('\t') + 2;
                            int len = (line.Length - 2) - line.LastIndexOf('\t');
                            clock._offset = line.Substring(line.LastIndexOf('\t') + 1, 1)
                                == "−" ? -1 * double.Parse(line.Substring(start, len)) : double.Parse(line.Substring(start, len));
                        }
                    }
                }

                foreach (var webClock in clockDisplays)
                {
                    foreach (var clock in clocks)
                    {
                        if (clock._location == webClock.Text.Substring(0, webClock.Text.IndexOf('\r')))
                        {
                            UpdateUI(LblTimeTest, $"Time Test: Checking {clock._location}");
                            results.Add(_ClockOperations.CheckClockTime(webClock, clock));
                        }
                    }
                }
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblTimeTest, results.TrueForAll(b => b) ? "Time Test: Passed" : "Time Test: Failed");
        }

        /// <summary>
        /// Verify each clocks minute hand moves every second.
        /// </summary>
        private void VerifyMinuteHands()
        {
            results.Clear();

            UpdateUI(LblStatus, "Verifying minute hands are updating.");
            foreach (var item in clockDisplays)
            {
                UpdateUI(LblMinHandTest, $"Minute Hand Test: {item.Text.Substring(0, item.Text.IndexOf('\r'))}");

                // If the minute hand translation is diffirent the hand is updating
                results.Add(_ClockOperations.CheckHandMovement(item, "min"));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblMinHandTest, results.TrueForAll(b => b) ? "Minute Hand Test: Passed" : "Minute Hand Test: Failed");
        }

        /// <summary>
        /// Verify each clocks hour hand moves every minute.
        /// </summary>
        private void VerifyHourHands()
        {
            results.Clear();

            UpdateUI(LblStatus, "Verifying hour hands are updating.");
            foreach (var item in clockDisplays)
            {
                UpdateUI(LblHourHandTest, $"Hour Hand Test: {item.Text.Substring(0, item.Text.IndexOf('\r'))}");

                // If the hour hand translation is diffirent the hand is updating
                results.Add(_ClockOperations.CheckHandMovement(item, "hour"));
            }

            // Verify each result is true and return passed or failed
            UpdateUI(LblHourHandTest, results.TrueForAll(b => b) ? "Hour Hand Test: Passed" : "Hour Hand Test: Failed");
        }

        /// <summary>
        /// Calculate the angle between the hour hand and minute hand of each clock.
        /// </summary>
        private void VerifyClockAngles()
        {
            List<double> angles = new List<double>();

            UpdateUI(LblStatus, "Calculating angles based on hands.");
            foreach (var webClock in clockDisplays)
            {
                var hrMin = webClock.FindElement(By.ClassName("c-city__hrMin"));

                angles.Add(_ClockOperations.ClockAngleCalc(Int32.Parse(hrMin.Text.Substring(0, hrMin.Text.IndexOf(":"))), 2));
            }

            // Verify each angle is actually a number and not an error and return pass or fail
            UpdateUI(LblHandAngleTest, angles.TrueForAll(b => b != 999.999) ? "Hand Angle Test: Passed" : "Hnd Angle Test: Failed");
        }

        /// <summary>
        /// Take a screenshot of the page.
        /// </summary>
        /// <param name="testNum">1 is for calculator 2 is for clocks</param>
        private void TakeScreenshot(int testNum)
        {
            var driver = testNum == 1 ? calcDriver : clockDriver;
            // Find the window that we are running the calculator in

            var savePath = testNum == 1 ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Calculator Test.png" : Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Clock Test.png";

            Screenshot _ss = driver.GetScreenshot();
            _ss.SaveAsFile(savePath, ScreenshotImageFormat.Png);

            if (testNum == 1)
            {
                // Save the bitmap as a png file to the users desktop
                UpdateUI(LblStatus, "Calculator Testing Complete");
            }
            else
            {
                UpdateUI(LblStatus, "Clock Testing Complete");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clockDriver?.Dispose();
            calcDriver?.Dispose();
        }
    }
}
