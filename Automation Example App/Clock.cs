namespace Automation_Example_App
{
    public class Clock
    {
        public string _clockZone { get; set; }
        public string _location { get; set; }
        public double _offset { get; set; }

        public Clock(string ClockZone, string Location, double Offset = 0)
        {
            _clockZone = ClockZone;
            _location = Location;
            _offset = Offset;
        }
    }
}
