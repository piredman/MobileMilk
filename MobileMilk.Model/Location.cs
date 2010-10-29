using System.Collections.Generic;

namespace MobileMilk.Model
{
    public class Location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Zoom { get; set; }
        public string Address { get; set; }
        public bool Viewable { get; set; }
    }
}