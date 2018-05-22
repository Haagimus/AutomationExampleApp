using OpenQA.Selenium;
using System;
using System.Threading;

namespace Automation_Example_App.Tests
{
    class ClockOperations
    { 
        /// <summary>
        /// Calculates the angle between hands on a clock, this is assuming no movement between minute and hour except
        /// for the tick that occurs every 60 seconds.
        /// </summary>
        /// <param name="hour">The hour of the clock</param>
        /// <param name="minute">The minute of the clock</param>
        /// <returns>The angle between the two hands</returns>
        public static double ClockAngleCalc(int hour, int minute)
        {
            double angle;
            double hourHand;
            double minuteHand;
            
            if (hour == 12 || hour == 0) hourHand = 0;
            if (minute == 60 || minute == 0) minuteHand = 0;
            
            hourHand = ((180.0 / Math.PI) * 0.5) * (60 * hour + minute);
            minuteHand = ((180.0 / Math.PI) * 6.0) * minute;
            
            angle = hourHand - minuteHand;
            angle = Math.PI * angle / 180.0;
            angle = Math.Min(360 - angle, angle);
            
            return Math.Abs(angle); 

        }

        /// <summary>
        /// Use this function to parse out substrings within a longer string
        /// </summary>
        /// <param name="strSource">The source string you want to search inside</param>
        /// <param name="strStart">The string immediately before the substring</param>
        /// <param name="strEnd">The string immediately after the substring</param>
        /// <returns>The desired substring</returns>
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// This is used to check if two times are equal or different.
        /// </summary>
        /// <param name="webClock">This is the in page clock that is checked against</param>
        /// <param name="clock">This is the clock object that will provide offset from UTC to check with</param>
        /// <returns>True or False</returns>
        public static bool CheckClockTime(IWebElement webClock, Clock clock)
        {
            var hrMin = webClock.FindElement(By.ClassName("c-city__hrMin"));
            var ampm = webClock.FindElement(By.ClassName("c-city__ampm"));
            return (hrMin.GetAttribute("innerHTML") == String.Format("{0:h:mm}", DateTime.UtcNow.AddHours(clock._offset))
            && ampm.GetAttribute("innerHTML") == String.Format("{0:tt}", DateTime.UtcNow.AddHours(clock._offset)).ToLower());
        }

        /// <summary>
        /// Checks translation on a web clock hand and verifies it is changing.
        /// </summary>
        /// <param name="webClock">This is the in page clock that is checked</param>
        /// <param name="hand">This is the class name of the hand you want checked</param>
        /// <returns>True or False</returns>
        public static bool CheckHandMovement(IWebElement webClock, string hand)
        {
            string firstAngle, lastAngle;
            // Capture the translation of the minute hand
            firstAngle = webClock.FindElement(By.ClassName(hand)).GetCssValue("transform");

            // Wait five seconds then capture the translation of the minute hand again
            Thread.Sleep(5000);
            lastAngle = webClock.FindElement(By.ClassName(hand)).GetCssValue("transform");

            return firstAngle != lastAngle;
        }
    }
}
