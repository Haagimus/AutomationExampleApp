using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Automation_Example_App.Tests
{
    [TestClass]
    public class UnitTests
    {
        private readonly ClockOperations _cOps = new ClockOperations();
        [TestMethod]
        public void WholeHourTest()
        {
            Assert.AreEqual(30, _cOps.ClockAngleCalc(1, 0));
            Assert.AreEqual(60, _cOps.ClockAngleCalc(2, 0));
            Assert.AreEqual(90, _cOps.ClockAngleCalc(3, 0));
            Assert.AreEqual(120, _cOps.ClockAngleCalc(4, 0));
            Assert.AreEqual(150, _cOps.ClockAngleCalc(5, 0));
            Assert.AreEqual(180, _cOps.ClockAngleCalc(6, 0));
            Assert.AreEqual(150, _cOps.ClockAngleCalc(7, 0));
            Assert.AreEqual(120, _cOps.ClockAngleCalc(8, 0));
            Assert.AreEqual(90, _cOps.ClockAngleCalc(9, 0));
            Assert.AreEqual(60, _cOps.ClockAngleCalc(10, 0));
            Assert.AreEqual(30, _cOps.ClockAngleCalc(11, 0));
            Assert.AreEqual(0, _cOps.ClockAngleCalc(12, 0));
        }

        [TestMethod]
        public void QuarterHourTest()
        {
            Assert.AreEqual(52.5, _cOps.ClockAngleCalc(1, 15));
            Assert.AreEqual(22.5, _cOps.ClockAngleCalc(2, 15));
            Assert.AreEqual(7.5, _cOps.ClockAngleCalc(3, 15));
            Assert.AreEqual(37.5, _cOps.ClockAngleCalc(4, 15));
            Assert.AreEqual(67.5, _cOps.ClockAngleCalc(5, 15));
            Assert.AreEqual(97.5, _cOps.ClockAngleCalc(6, 15));
            Assert.AreEqual(127.5, _cOps.ClockAngleCalc(7, 15));
            Assert.AreEqual(157.5, _cOps.ClockAngleCalc(8, 15));
            Assert.AreEqual(172.5, _cOps.ClockAngleCalc(9, 15));
            Assert.AreEqual(142.5, _cOps.ClockAngleCalc(10, 15));
            Assert.AreEqual(112.5, _cOps.ClockAngleCalc(11, 15));
            Assert.AreEqual(82.5, _cOps.ClockAngleCalc(12, 15));
        }

        [TestMethod]
        public void HalfHourTest()
        {
            Assert.AreEqual(135, _cOps.ClockAngleCalc(1, 30));
            Assert.AreEqual(105, _cOps.ClockAngleCalc(2, 30));
            Assert.AreEqual(75, _cOps.ClockAngleCalc(3, 30));
            Assert.AreEqual(45, _cOps.ClockAngleCalc(4, 30));
            Assert.AreEqual(15, _cOps.ClockAngleCalc(5, 30));
            Assert.AreEqual(15, _cOps.ClockAngleCalc(6, 30));
            Assert.AreEqual(45, _cOps.ClockAngleCalc(7, 30));
            Assert.AreEqual(75, _cOps.ClockAngleCalc(8, 30));
            Assert.AreEqual(105, _cOps.ClockAngleCalc(9, 30));
            Assert.AreEqual(135, _cOps.ClockAngleCalc(10, 30));
            Assert.AreEqual(165, _cOps.ClockAngleCalc(11, 30));
            Assert.AreEqual(165, _cOps.ClockAngleCalc(12, 30));
        }

        [TestMethod]
        public void ThreeQuarterHourTest()
        {
            Assert.AreEqual(217.5, _cOps.ClockAngleCalc(1, 45));
            Assert.AreEqual(187.5, _cOps.ClockAngleCalc(2, 45));
            Assert.AreEqual(157.5, _cOps.ClockAngleCalc(3, 45));
            Assert.AreEqual(127.5, _cOps.ClockAngleCalc(4, 45));
            Assert.AreEqual(97.5, _cOps.ClockAngleCalc(5, 45));
            Assert.AreEqual(67.5, _cOps.ClockAngleCalc(6, 45));
            Assert.AreEqual(37.5, _cOps.ClockAngleCalc(7, 45));
            Assert.AreEqual(7.5, _cOps.ClockAngleCalc(8, 45));
            Assert.AreEqual(22.5, _cOps.ClockAngleCalc(9, 45));
            Assert.AreEqual(52.5, _cOps.ClockAngleCalc(10, 45));
            Assert.AreEqual(82.5, _cOps.ClockAngleCalc(11, 45));
            Assert.AreEqual(112.5, _cOps.ClockAngleCalc(12, 45));
        }
    }
}
