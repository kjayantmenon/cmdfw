using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;

namespace Gateway
{
    using Microsoft.Azure.Devices.Client;
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the Gateway...");
            
            string deviceId="FOG01-18062016";
            string iotHubConnString = "HostName=jiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xq3K8e6B44BsQcdif/RJ7eJFL5YaF4CanJo96sYrilU="; ;
            
            Device gateway =new Device(deviceId);

            Task.Run(async () =>
            {

                try
                {
                    gateway = await RegistryManager.CreateFromConnectionString(iotHubConnString).AddDeviceAsync(gateway);
                }
                catch (DeviceAlreadyExistsException)
                {
                    gateway = await RegistryManager.CreateFromConnectionString(iotHubConnString).GetDeviceAsync(deviceId);
                }
            }

            ).Wait();

           
    
            
            RegisterToReceiveCommand(gateway);

            SendGatewayHearbeatAsync(gateway);

            Console.ReadKey();
        }

        static string iotHubUri = "jiothub.azure-devices.net";
        private static async Task SendGatewayHearbeatAsync(Device gateway)
        {
            
            DeviceClient gatewayClient = DeviceClient.Create(iotHubUri,
                new DeviceAuthenticationWithRegistrySymmetricKey(gateway.Id,
                    gateway.Authentication.SymmetricKey.PrimaryKey));

            while (true)
            {

                var telemetryDataPoint = new
                {
                    deviceId = gateway.Id,
                    heartbeatTime = DateTime.Now
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await gatewayClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(5000).Wait();
            }

        }



        private static async Task RegisterToReceiveCommand(Device gateway)
        {
            
            Console.WriteLine("Registering to receive ");
            DeviceClient gatewayClient = DeviceClient.Create(iotHubUri,
                 new DeviceAuthenticationWithRegistrySymmetricKey(gateway.Id,
                     gateway.Authentication.SymmetricKey.PrimaryKey)); ;
            while (true)
            {
                Message receivedCommand = await gatewayClient.ReceiveAsync();
                if (receivedCommand == null) continue;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Received Command: {0}", Encoding.ASCII.GetString(receivedCommand.GetBytes()));
                Console.ResetColor();

                await gatewayClient.CompleteAsync(receivedCommand);
            }
            Console.WriteLine("Registration complete.....");
        }
    }
}
