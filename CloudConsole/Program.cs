using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;

namespace CloudConsole
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=jiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xq3K8e6B44BsQcdif/RJ7eJFL5YaF4CanJo96sYrilU=";

        static void Main(string[] args)
        {

            Console.WriteLine("Send a command to the gateway from the platform\n");

            
            
            StartIoTReceiver();
           

            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            
        }

        private static async Task StartIoTReceiver()
        {
            string iotHubEndpoint = "messages/events";

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubEndpoint);
            var gateway2cpartition = eventHubClient.GetRuntimeInformation().PartitionIds;

            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            var gateway2cpartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (var partition in gateway2cpartitions)
            {
                tasks.Add(ReceiveMessagesFromGatewayAsync(eventHubClient, partition, cts.Token));
            }

            Task.WaitAll(tasks.ToArray());
        }

        

        private static async Task ReceiveMessagesFromGatewayAsync(EventHubClient eventHubClient, string partition, CancellationToken token)
        {
            var eventhubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (true)
            {
                if (token.IsCancellationRequested) break;
                EventData eventData = await eventhubReceiver.ReceiveAsync();
                if (eventData == null) continue;
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received at partition{0} : {1}", partition, data);
            }

        }
        

       
    }
}
