﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Automation_Example_App.Tests
{
    public class MathOperations
    {
        /// <summary>
        /// Adds two given numbers together and verifies them against a passed in expected result
        /// </summary>
        /// <param name="elements">The key value pair dictionary of webpage elements</param>
        /// <param name="firstElement">The first number you want to added</param>
        /// <param name="secondElement">The second number you want added</param>
        /// <param name="expected">The expected sum of the two numbers</param>
        /// <returns>True or False</returns>
        [TestMethod]
        public static Boolean Addition(Dictionary<string, IWebElement> elements, string firstElement, string secondElement, int expected)
        {
            // Test 1: Add each number to itself and verify the output is correct.
            elements[firstElement].Click();
            elements["plus"].Click();
            elements[secondElement].Click();
            elements["equals"].Click();
            int test = Int32.Parse(elements["result"].Text.Substring(0, expected.ToString().Length));

            if (test != expected) { Console.WriteLine($"{firstElement} / {secondElement}: expected {expected} - actual {test}"); }

            return test == expected;
        }

        /// <summary>
        /// Multiplies two given numbers together and verifies them against a passed in expected result
        /// </summary>
        /// <param name="elements">The key value pair dictionary of webpage elements</param>
        /// <param name="firstElement">The first number you want to multiplied</param>
        /// <param name="secondElement">The second number you want multiplied</param>
        /// <param name="expected">The expected product of the two numbers</param>
        /// <returns>True or False</returns>
        [TestMethod]
        public static Boolean Multiplication(Dictionary<string, IWebElement> elements, string firstElement, string secondElement, int expected)
        {
            // Test 2: Multiple each number by itself and verify the output is correct.
            elements[firstElement].Click();
            elements["times"].Click();
            elements[secondElement].Click();
            elements["equals"].Click();
            int test = Int32.Parse(elements["result"].Text.Substring(0, expected.ToString().Length));

            if (test != expected) { Console.WriteLine($"{firstElement} / {secondElement}: expected {expected} - actual {test}"); }

            return test == expected;
        }

        /// <summary>
        /// Divides two given numbers together and verifies them against a passed in expected result
        /// </summary>
        /// <param name="elements">The key value pair dictionary of webpage elements</param>
        /// <param name="firstElement">The divisor number of the equation</param>
        /// <param name="secondElement">The number you want divided into the first</param>
        /// <param name="secondElement">The number you want divided into the first</param>
        /// <param name="expected">The expected quotient of the two numbers</param>
        /// <returns>True or False</returns>
        [TestMethod]
        public static Boolean Division(Dictionary<string, IWebElement> elements, string firstElement, string secondElement, double expected)
        {
            double test;
            // Test 3: Divide each number by itself and verify the output is correct.
            elements[firstElement].Click();
            Thread.Sleep(100);
            elements["divide"].Click();
            Thread.Sleep(100);
            elements[secondElement].Click();
            Thread.Sleep(100);
            elements["equals"].Click();

            // If expected result is -1 then we are dividing 0 by 0 which will return error on the calculator page
            if (expected == -1 && elements["result"].Text.ToLower() == "error") return true;

            test = Double.Parse(elements["result"].Text.Substring(0, expected.ToString().Length));

            if (test != expected) { Console.WriteLine($"{firstElement} / {secondElement}: expected {expected} - actual {test}"); }

            return test == expected;
        }

        /// <summary>
        /// Multiplies two given numbers together and verifies them against a passed in expected result
        /// </summary>
        /// <param name="elements">The key value pair dictionary of webpage elements</param>
        /// <param name="firstElement">The number you want to subtract from</param>
        /// <param name="secondElement">The number you want subtracted from the first</param>
        /// <param name="expected">The expected difference of the two numbers</param>
        /// <returns>True or False</returns>
        [TestMethod]
        public static Boolean Subtraction(Dictionary<string, IWebElement> elements, string firstElement, string secondElement, int expected)
        {
            // Test 4: Subtract each number by itself and verify the output is correct.
            elements[firstElement].Click();
            elements["minus"].Click();
            elements[secondElement].Click();
            elements["equals"].Click();
            int test = Int32.Parse(elements["result"].Text.Substring(0, expected.ToString().Length));

            if (test != expected) { Console.WriteLine($"{firstElement} / {secondElement}: expected {expected} - actual {test}"); }

            return test == expected;
        }
    }
}