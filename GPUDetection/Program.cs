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
    class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (true)    //无限循环等待输入
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "getgpu":
                        Console.WriteLine(GetGPUData());
                        break;
                    default:
                        Console.WriteLine("Input error.Please check!");
                        break;
                }
            }
        }

        

        private static string GetGPUData()      //取得所有GPU型号和每个GPU sensor的type、name、value
        {
           
            int x, y;
            int hardwareCount;
            int sensorcount;
            Computer computerHardware = new Computer();
            computerHardware.GPUEnabled = true;
            computerHardware.Open();
            hardwareCount = computerHardware.Hardware.Count();
            List<GPU> GPUData = new List<GPU>();
            for (x = 0; x < hardwareCount; x++)
            {
                GPU _gpu = new GPU
                {
                    Name = computerHardware.Hardware[x].Name,
                    HardwareType = computerHardware.Hardware[x].HardwareType.ToString()//判断是A卡还是N卡条件              
                };
                sensorcount = computerHardware.Hardware[x].Sensors.Count(); ;
                 
                    for (y = 0; y < sensorcount; y++)
                    {
                    Sensor _sensor = new Sensor
                    {
                        Name = computerHardware.Hardware[x].Sensors[y].Name.ToString(),
                        Value = computerHardware.Hardware[x].Sensors[y].Value.ToString(),
                        Type = computerHardware.Hardware[x].Sensors[y].SensorType.ToString(),
                        Max = computerHardware.Hardware[x].Sensors[y].Max ?? 0,
                        Min = computerHardware.Hardware[x].Sensors[y].Min ?? 0,
                        Index = computerHardware.Hardware[x].Sensors[y].Index
                    };
                    _gpu.Sensors.Add(_sensor);
                    }
                GPUData.Add(_gpu);
                computerHardware.Close();
            }
           return JsonConvert.SerializeObject(GPUData);

        }

    }

}
