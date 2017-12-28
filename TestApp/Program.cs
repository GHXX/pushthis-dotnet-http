using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using Pushthis;
using Newtonsoft.Json;
using System.IO;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new PushthisExample().Run().GetAwaiter().GetResult();
        }
    }

    class PushthisExample
    {
        public async Task Run()
        {
            if (!File.Exists("keys.txt"))
            {
                File.WriteAllText("keys.txt", "YOURPUBLICKEYHERE\nYOURSECRETKEYHERE");
                return;
            }
            var lines = File.ReadAllLines("keys.txt");
            string publicKey = lines[0];
            string secretKey = lines[1];
            string authUrl = "https://na.pushthis.io/auth"; //send
            string apiUrl = "https://na.pushthis.io/api";   //send

            string accessUrl = "https://na-connect.pushthis.io"; //receive

            string channelName = "pushthisChat";

            var authClient = new Pushthis.AuthClient(publicKey, secretKey, authUrl);
            var receiveClient = new Pushthis.ApiClient(publicKey, secretKey, accessUrl, "C1-r");   //receive
            var sendClient = new Pushthis.ApiClient(publicKey, secretKey, apiUrl, "C2-s");      //send


            receiveClient.sock.On(Socket.EVENT_MESSAGE, (data) =>
            {
                Console.WriteLine("[Client 1] Received message: " + data?.ToString() ?? "null");
            });

            receiveClient.sock.On(Socket.EVENT_ERROR, (data) => {Console.WriteLine("[Client 1] ERROR: " + data?.ToString()??"null"); });
            receiveClient.sock.On(Socket.EVENT_CONNECT, (data) => {Console.WriteLine("[Client 1] CONNECTED: " + data?.ToString() ?? "null"); });
            receiveClient.sock.On(Socket.EVENT_RECONNECTING, (data) => {Console.WriteLine("[Client 1] RECONNECTING: " + data?.ToString() ?? "null"); });
            receiveClient.sock.On(Socket.EVENT_DISCONNECT, (data) => {Console.WriteLine("[Client 1] DISCONNECT: " + data?.ToString() ?? "null"); });
            receiveClient.sock.On(Socket.EVENT_CONNECT_TIMEOUT, (data) => {Console.WriteLine("[Client 1] CONN TIMEOUT: " + data?.ToString() ?? "null"); });

            receiveClient.Connect(true);
            sendClient.Connect(true);
            Console.WriteLine("Connected.");
            await Task.Delay(1000);
            receiveClient.sock.Emit("join", new string[][] { new[] { "channel", channelName }, new[] { "presence", "false" }, new[] { "authorize", "false" } });
            Console.WriteLine("Rec joined.");

            sendClient.sock.Emit("join", new string[][] { new[] { "channel", channelName }, new[] { "presence", "false" }, new[] { "authorize", "false" } });
            Console.WriteLine("Sender joined.");



            for (int i = 0; i < 100; i++)
            {
                await sendClient.SendRequestAsync(new ApiRequest(publicKey, secretKey).AddPayload(channelName, "message",
                    new Dictionary<string, string>
                    {
                    { "message", $"hello there! [{i}]" } }
                    ));
                Console.WriteLine("Sender sent msg.");
                await Task.Delay(1000);
            }

            await Task.Delay(-1);
        }
    }
}
