using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus; //Namespace for ServiceBusQueue
using System.Text; //Namespace for Encoding SB Message


namespace SharingSessionDotNetCore
{
    class Program
    {
        const string storageAccountName = "";
        const string storageAccountKey = "";

        ///

        const string serviceBusConnectionString = "";
        const string serviceBusQueueName = "";

        static IQueueClient sbQueueClient;


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CloudQueue myqueue = CreateStorageQueueAsync(storageAccountName, storageAccountKey).Result;

            //SendStorageQueueMessageAsync(myqueue, "Hello From the session!").Wait();

            sbQueueClient = new QueueClient(serviceBusConnectionString, serviceBusQueueName);

            SendServiceBusMessageAsync("Hello from the session!").Wait();



        }


        private static async Task<CloudQueue> CreateStorageQueueAsync(string stgAccountName, string stgAccountKey)
        {

            StorageCredentials storageCredentials = new StorageCredentials(stgAccountName, stgAccountKey);

            CloudStorageAccount myStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);

            CloudQueueClient queueClient = myStorageAccount.CreateCloudQueueClient();

            Console.WriteLine("\n Creating queue: ...");
            CloudQueue queue = queueClient.GetQueueReference("sessionqueue");

            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine("Failed to create a queue, error was:" + ex.ToString());
                Console.ReadLine();
                throw;
            }

            return queue;

 
        }

        private static async Task SendStorageQueueMessageAsync(CloudQueue inputQueue, string message)
        {
            Console.WriteLine("\n Adding Message:...");

            try
            {
                await inputQueue.AddMessageAsync(new CloudQueueMessage(message));
            }
            catch(StorageException ex)
            {
                Console.WriteLine("Failed to add message: Error:" + ex.ToString());
                Console.ReadLine();
                throw;
            }

            Console.WriteLine("\n Message was added Successfully!");
        }

        private static async Task SendServiceBusMessageAsync(string sbMessage)
        {
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(sbMessage));

                Console.WriteLine("\n Sending Service Bus Message:...");

                await sbQueueClient.SendAsync(message);

              
            }
            catch(ServiceBusException ex)
            {
                Console.WriteLine("Failed to send message: Error: " + ex.ToString());
            }
            


        }


            
    }
}
