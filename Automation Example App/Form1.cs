﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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

namespace Automation_Example_App
{
    public partial class Form1 : Form
    {
        List<IWebElement> calcPageElements = new List<IWebElement>();
        Dictionary<string, IWebElement> kvpElements = new Dictionary<string, IWebElement>();
        List<Clock> clocks = new List<Clock>();
        List<IWebElement> clockDisplays = new List<IWebElement>();
        IWebDriver calcDriver, clockDriver;
        List<bool> results = new List<bool>();
        List<string> calcElements = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "–", "×", "/", "=", "C", "0" };
        List<string> calcNames = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "plus", "minus", "times", "divide", "equals", "clear", "result" };
        readonly SynchronizationContext synchronizationContext;
        int testCount = 10;

        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        // Most of the methods are run asyncronously so the UI thread does not get hung up and the main form can be moved around and does not appear frozen

        // Let's open the calculator webpage and run the tests
        private async void BtnOpenCalculator_Click(object sender, EventArgs e)
        {
            LblStatus.Text = "Opening Calculator webpage.";
            await Task.Run(() =>
            {
                calcDriver = WebpageHelpers.OpenWebpage("http://www.calculator.net");
            });
            LblStatus.Text = "";

            StartCalculatorTests();
        }

        // Let's open the calculator webpage and run the tests
        private async void BtnOpenClocks_Click(object sender, EventArgs e)
        {
            LblStatus.Text = "Opening Clock webpage.";
            await Task.Run(() =>
            {
                clockDriver = WebpageHelpers.OpenWebpage("https://www.timeanddate.com/worldclock/personal.html");
            });
            LblStatus.Text = "";

            StartClockTests();
        }

        private async void StartCalculatorTests()
        {
            LblStatus.Text = "Finding calculator elements.";
            await Task.Run(() =>
            {
                // Find all the buttons with the class 'scinm' which are the number buttons
                for (var i = 0; i < 10; i++)
                {
                    IWebElement btn = WebpageHelpers.GetElementByClass(calcDriver, "scinm", calcElements[i]);
                    calcPageElements.Add(btn);
                }

                // Find all the buttons with the class 'sciop' which are the operation buttons
                for (var i = 10; i < 14; i++)
                {
                    IWebElement operation = WebpageHelpers.GetElementByClass(calcDriver, "sciop", calcElements[i]);
                    calcPageElements.Add(operation);
                }

                // Find the two buttons with the class 'scieq' which are the equal and clear buttons
                for (var i = 14; i < 16; i++)
                {
                    IWebElement btn = WebpageHelpers.GetElementByClass(calcDriver, "scieq", calcElements[i]);
                    calcPageElements.Add(btn);
                }

                // Find the output window
                IWebElement output = WebpageHelpers.GetElementByID(calcDriver, "sciOutPut", calcElements[16]);
                calcPageElements.Add(output);

                // This is not really necessary but adding key value pairs to the list of controls makes them easier to reference later on
                for (int i = 0; i < calcElements.Count; i++)
                {
                    kvpElements.Add(calcNames[i], calcPageElements[i]);
                }
            });

            ExecuteAdditionTests();
        }

        private async void StartClockTests()
        {
            LblStatus.Text = "Finding clocks on clock page.";
            await Task.Run(() =>
            {
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
                        clocks.Add(new Clock(ClockOperations.getBetween(clockZone, "title=\\\"", "\\\">").ToString(),
                            ClockOperations.getBetween(location, "name\":\"", "\",\"co").ToString()));

                        clockZone = clockZone.Substring(clockZone.IndexOf(clocks[i]._clockZone + "\\\">"));
                        location = location.Substring(location.IndexOf(clocks[i]._location + "\",\"co"));
                    }
                }
            });

            // Now that we have all the controls let's run the tests
            VerifyClocks();
        }

        /// <summary>
        /// Execute addition tests.
        /// </summary>
        private async void ExecuteAdditionTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the add function for each number
            for (int i = 0; i < testCount; i++)
            {
                LblStatus.Text = "Executing Addition Tests.";

                LblAdditionTest.Text = $"Addition Test: Checking {i} + {i}";
                // Run the function and add the result to the results list
                await Task.Run(() => results.Add(MathOperations.Addition(kvpElements, i.ToString(), i.ToString(), i + i)));
            }

            // Verify each result is true and return passed or failed
            LblAdditionTest.Text = results.TrueForAll(b => b) ? "Addition Test: Passed" : "Addition Test: Failed";
            ExecuteMultiplyTests();
        }

        /// <summary>
        /// Execute multiplication tests.
        /// </summary>
        private async void ExecuteMultiplyTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the multiply function for each number
            for (int i = 0; i < testCount; i++)
            {
                LblStatus.Text = "Executing Multiplication Tests.";

                LblMultiplicationTest.Text = $"Multiplication Test: Checking {i} * {i}";
                // Run the function and add the result to the results list
                await Task.Run(() => results.Add(MathOperations.Multiplication(kvpElements, i.ToString(), i.ToString(), i * i)));
            }

            // Verify each result is true and return passed or failed
            LblMultiplicationTest.Text = results.TrueForAll(b => b) ? "Multiplication Test: Passed" : "Multiplication Test: Failed";
            ExecuteDivideTests();
        }

        /// <summary>
        /// Execute division tests.
        /// </summary>
        private async void ExecuteDivideTests()
        {
            results.Clear();

            // We know 0/0 will return an error so we pass an expected result of -1 to handle it in the function
            results.Add(MathOperations.Division(kvpElements, "0", "0", -1));

            // Loop 0 through 10 and run the divide function for each number
            for (int i = 1; i < testCount; i++)
            {
                LblStatus.Text = "Executing Division Tests.";

                LblDivisionTest.Text = $"Division Test: Checking {i} / {i}";
                // Run the function and add the result to the results list
                await Task.Run(() => results.Add(MathOperations.Division(kvpElements, i.ToString(), i.ToString(), 1)));
            }

            // Verify each result is true and return passed or failed
            LblDivisionTest.Text = results.TrueForAll(b => b) ? "Division Test: Passed" : "Division Test: Failed";
            ExecuteSubtractTests();
        }

        /// <summary>
        /// Execute subtraction tests.
        /// </summary>
        private async void ExecuteSubtractTests()
        {
            results.Clear();

            // Loop 0 through 10 and run the subtract function for each number
            for (int i = 0; i < testCount; i++)
            {
                LblStatus.Text = "Executing Subtraction Tests.";

                LblSubtractionTest.Text = $"Subtraction Test: Checking {i} - {i}";
                // Run the function and add the result to the results list
                await Task.Run(() => results.Add(MathOperations.Subtraction(kvpElements, i.ToString(), i.ToString(), 0)));
            }

            // Verify each result is true and return passed or failed
            LblSubtractionTest.Text = results.TrueForAll(b => b) ? "Subtraction Test: Passed" : "Subtraction Test: Failed";
            RandomMathTest();
        }

        /// <summary>
        /// Execute random math operation tests.
        /// </summary>
        private async void RandomMathTest()
        {
            Random r = new Random();
            results.Clear();

            LblStatus.Text = "Executing Subtraction Tests.";
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
                        LblRandomTest.Text = $"Random Test: Checking {r1} + {r2}";
                        // Run the function and add the result to the results list
                        await Task.Run(() => results.Add(MathOperations.Addition(kvpElements, r1.ToString(), r2.ToString(), r1 + r2)));
                        break;
                    case 2:
                        LblRandomTest.Text = $"Random Test: Checking {r1} * {r2}";
                        // Run the function and add the result to the results list
                        await Task.Run(() => results.Add(MathOperations.Multiplication(kvpElements, r1.ToString(), r2.ToString(), r1 * r2)));
                        break;
                    case 3:
                        // We need to account for the possible outcomes of division where it may not just be a simple positive or negative number
                        if (r1 == 0 && r2 == 0)
                        {
                            LblRandomTest.Text = $"Random Test: Checking {r1} / {r2}";
                            // This is when 0 is divided by 0, we know it will throw an error so we send -1 and handle it in the function
                            await Task.Run(() => results.Add(MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), -1)));
                        }
                        else if (r1 == 0 && r2 > 0)
                        {
                            LblRandomTest.Text = $"Random Test: Checking {r1} / {r2}";
                            // This is when we are dividing 0 by any number which will return a 0
                            await Task.Run(() => results.Add(MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), 0)));
                        }
                        else if (r1 > 0 && r2 == 0)
                        {
                            LblRandomTest.Text = $"Random Test: Checking {r1} / {r2 + 1}";
                            // This is when we are dividing any number by 0, we just add 1 to 0 so we can perform the function
                            await Task.Run(() => results.Add(MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), r1 / (r2 + 1))));
                        }
                        else if (r2 > r1)
                        {
                            LblRandomTest.Text = $"Random Test: Checking {r1} / {r2}";
                            // This is when a decimal will be returned, the calculator will only display out to three decimal places so we format
                            // the string .net calculates because it has far more accuracy
                            await Task.Run(() => results.Add(MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), Double.Parse(string.Format("{0:N3}", (double)r1 / r2)))));
                        }
                        else
                        {
                            LblRandomTest.Text = $"Random Test: Checking {r1} / {r2}";
                            // This is when it's a straight forward division problem where the divisor is smaller than the number to be divided
                            await Task.Run(() => results.Add(MathOperations.Division(kvpElements, r1.ToString(), r2.ToString(), r1 / r2)));
                        }
                        break;
                    case 4:
                        LblRandomTest.Text = $"Random Test: Checking {r1} - {r2}";
                        // Run the function and add the result to the results list
                        await Task.Run(() => results.Add(MathOperations.Subtraction(kvpElements, r1.ToString(), r2.ToString(), r1 - r2)));
                        break;
                }
            }
            // Verify each result is true and return passed or failed
            LblRandomTest.Text = results.TrueForAll(b => b) ? "Random Test: Passed" : "Random Test: Failed";
            TakeScreenshot(1);
        }

        /// <summary>
        /// Verify displayed clocks show the correct times.
        /// </summary>
        private async void VerifyClocks()
        {
            results.Clear();
            LblStatus.Text = "Verifying displayed times are correct.";

            // First load the timezone text file and match the locale to the correct offset then add that to the correcpsonding clock object
            using (StreamReader sr = new StreamReader(File.OpenRead(@".\Resources\TimeZones.txt")))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    foreach (Clock clock in clocks)
                    {
                        // If the timezone in the stream matches that of the current clock then we found the correct timezone and
                        // need to add the corresponding offset to the clock object
                        if (line.ToLower().Contains(clock._clockZone.ToLower()))
                        {
                            int start = line.LastIndexOf('\t') + 2;
                            int len = (line.Length - 2) - line.LastIndexOf('\t');
                            clock._offset = line.Substring(line.LastIndexOf('\t') + 1, 1) ==
                                "−" ? -1 * double.Parse(line.Substring(start, len)) : double.Parse(line.Substring(start, len));
                        }

                    }
                }

                foreach (var webClock in clockDisplays)
                {
                    foreach (var clock in clocks)
                    {
                        if (clock._location == webClock.Text.Substring(0, webClock.Text.IndexOf('\r')))
                        {
                            LblTimeTest.Text = $"Time Test: Checking {clock._location}";
                            await Task.Run(() => results.Add(ClockOperations.CheckClockTime(webClock, clock)));
                        }
                    }
                }
            }


            // Verify each result is true and return passed or failed
            LblTimeTest.Text = results.TrueForAll(b => b) ? "Time Test: Passed" : "Time Test: Failed";
            VerifyMinuteHands();
        }

        /// <summary>
        /// Verify each clocks minute hand moves every second.
        /// </summary>
        private async void VerifyMinuteHands()
        {
            results.Clear();

            LblStatus.Text = "Verifying minute hands are updating.";
            foreach (var item in clockDisplays)
            {
                LblMinHandTest.Text = $"Minute Hand Test: {item.Text.Substring(0, item.Text.IndexOf('\r'))}";

                // If the minute hand translation is diffirent the hand is updating
                await Task.Run(() => results.Add(ClockOperations.CheckHandMovement(item, "min")));
            }

            // Verify each result is true and return passed or failed
            LblMinHandTest.Text = results.TrueForAll(b => b) ? "Minute Hand Test: Passed" : "Minute Hand Test: Failed";
            VerifyHourHands();
        }

        /// <summary>
        /// Verify each clocks hour hand moves every minute.
        /// </summary>
        private async void VerifyHourHands()
        {
            results.Clear();

            LblStatus.Text = "Verifying hour hands are updating.";
            foreach (var item in clockDisplays)
            {
                LblHourHandTest.Text = $"Hour Hand Test: {item.Text.Substring(0, item.Text.IndexOf('\r'))}";

                // If the hour hand translation is diffirent the hand is updating
                await Task.Run(() => results.Add(ClockOperations.CheckHandMovement(item, "hour")));
            }

            // Verify each result is true and return passed or failed
            LblHourHandTest.Text = results.TrueForAll(b => b) ? "Hour Hand Test: Passed" : "Hour Hand Test: Failed";
            VerifyClockAngles();
        }

        /// <summary>
        /// Calculate the angle between the hour hand and minute hand of each clock.
        /// </summary>
        private void VerifyClockAngles()
        {
            List<double> angles = new List<double>();

            LblStatus.Text = "Calculating angles based on hands.";
            foreach (var webClock in clockDisplays)
            {
                var hrMin = webClock.FindElement(By.ClassName("c-city__hrMin"));
                try
                {
                    angles.Add(ClockOperations.ClockAngleCalc(Int32.Parse(hrMin.Text.Substring(0, hrMin.Text.IndexOf(":"))), 2));
                }
                catch (Exception)
                {
                    angles.Add(999.999);
                }
            }

            // Verify each angle is actually a number and not an error and return pass or fail
            LblHandAngleTest.Text = angles.TrueForAll(b => b != 999.999 ) ? "Hand Angle Test: Passed" : "Hnd Angle Test: Failed";
            TakeScreenshot(2);
        }

        /// <summary>
        /// Take a screenshot of the page.
        /// </summary>
        private void TakeScreenshot(int testNum)
        {
            var driver = testNum == 1 ? calcDriver : clockDriver;
            // Find the window that we are running the calculator in
            var proc = Process.GetProcesses().FirstOrDefault(x => x.MainWindowTitle == driver.Title + " - Internet Explorer");
            var rect = new User32.Rect();
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            // Create a new bitmap with the matching dimensions of our window
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            // Copy the internet explorer pixels onto the bitmap we just created
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            if (testNum == 1)
            {
                // Save the bitmap as a png file to the users desktop
                bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Calculator Test.png", ImageFormat.Png);
                LblScreenshotTest1.Text = $"Screenshot Test:\r\n{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Calculator Test.png";
                WebpageHelpers.CloseWebpage(calcDriver);
                LblStatus.Text = "Calculator Testing Complete";
            }
            else
            {
                // Save the bitmap as a png file to the users desktop
                bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Clock Test.png", ImageFormat.Png);
                LblScreenshotTest2.Text = $"Screenshot Test:\r\n{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Clock Test.png";
                WebpageHelpers.CloseWebpage(clockDriver);
                LblStatus.Text = "Clock Testing Complete";
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (clockDriver != null) clockDriver.Dispose();
            if (calcDriver != null) calcDriver.Dispose();
        }
    }
}