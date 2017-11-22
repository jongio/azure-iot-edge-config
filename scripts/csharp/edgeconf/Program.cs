using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace edgeconf
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = string.Empty; 
            var deviceId = string.Empty; 
            var filePath = string.Empty;

            try
            {
                connectionString = args[0];
                deviceId = args[1];
                filePath = args[2];
            }
            catch
            {
                Console.WriteLine("Usage: edgeconf iothubowner-connection-string device-id config-file-path");
            }
            // 1. Read Json Config File Into Memory
            Console.WriteLine("Reading Config File...");
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText(filePath));

            // 2. Build Configuration Context
            Console.WriteLine("Building Config Objects...");
            var cc = new ConfigurationContent() { ModuleContent = new Dictionary<string, TwinContent>() };
            foreach (var module in config.moduleContent)
            {
                var twinContent = new TwinContent();
                twinContent.TargetContent = new TwinCollection(module.Value["properties.desired"].ToString());
                cc.ModuleContent[module.Name.ToString()] = twinContent;
            }

            // 3. Apply Config via Registry Manager
            Console.WriteLine("Applying Configuration...");
            var rm = RegistryManager.CreateFromConnectionString(connectionString);
            Task.Run(async () => { await rm.ApplyConfigurationContentOnDeviceAsync(deviceId, cc); }).GetAwaiter().GetResult();

            Console.WriteLine("Complete");
        }
    }
}
