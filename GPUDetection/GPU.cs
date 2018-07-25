using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections;
using System.Collections.Specialized;
using OpenHardwareMonitor.Hardware;
using Newtonsoft;
using Newtonsoft.Json;
namespace GPUDetection
{ 
    public class GPU
    {

        public string Name { get; set; }
        public string HardwareType { get; set; }
        public List<Sensor> Sensors = new List<Sensor>();
    }

}
