using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.ViewModels
{
    public class DataWeather
    {
        public Main Main { get; set; }
        public Wind Wind { get; set; }
        public Sys Sys { get; set; }
        public String Name { get; set; }
        public List<Weather> Weather { get; set; }
    }
    public class Main
    {
        public float Temp { get; set; }
        public int Pressure { get; set; }
    }
    public class Wind
    {
        public float Speed { get; set; }
    }
    public class Sys
    {
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
    public class Geo
    {   
        public string City { get; set; }
    }
    public class Weather
    {
        public string Main { get; set; }
        public string Icon { get; set; }
    }

}
