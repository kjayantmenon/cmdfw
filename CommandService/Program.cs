using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace CommandService
{
    
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=jiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xq3K8e6B44BsQcdif/RJ7eJFL5YaF4CanJo96sYrilU=";
        static void Main(string[] args)
        {

         

            string iotHubUri = "jiothub.azure-devices.net";


            //SendingMessageToCloud(device);
            //SendDeviceToCloudMessagesAsync(device);
            //ReceiveC2dAsync();
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ReceiveAckAsync();
            CreateMenu();
            Console.ReadKey();
            RecvMessage();
            QueueCommandToSend();
            TrackCommand();

        }

        private static async Task CreateMenu()
        {
            while (true)
            {
                Console.WriteLine("Press a key to send a command.");
                Console.WriteLine("Press 1 - for a gateway command.");
                Console.WriteLine("Press 2 - for a device command.");
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.D0) break;
                switch (key)
                {
                    case ConsoleKey.D1:
                        SendGatewayCommand().Wait();
                        break;
                    case ConsoleKey.D2:
                        SendDeviceCommand().Wait();
                        break;
                }
            }

        }

        private async static Task SendGatewayCommand()
        {
            string gateway = "FOG01-18062016";
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Configure Gateway"));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(gateway, commandMessage);
        }

        private async static Task SendDeviceCommand()
        {
            string gateway = "FOG01-18062016";
            string deviceId = "MA012016";
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Run a bumptest on device " + deviceId));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(gateway, commandMessage);
        }

        private async static void ReceiveAckAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving Ack from gateway");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Ack feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
        private static void QueueCommandToSend()
        {
            //Assign a command Id
            //Serialize the command
            //Create a Message with the Command Id to correlate back 
            //Add the command to a dictionary and mark the status as sending
            //Send the command
            //Update the status of the command as CommandSent on receiving the delivery ack 

            throw new NotImplementedException();
        }


        private static void TrackCommand()
        {
            
        }

        private static void RecvMessage()
        {
            throw new NotImplementedException();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            string deviceKey = "MA012016";
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message. another message here..."));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(deviceKey, commandMessage);
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
