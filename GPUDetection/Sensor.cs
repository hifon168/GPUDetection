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

    public class Sensor    //sensor中所有属性
    {  

        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
        public int Index { get; set; }


    }
}
