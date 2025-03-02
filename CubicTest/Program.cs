using Cubic.V1.Crypto;
using Cubic.V1.Formatter;
using Cubic.V1.Parser;
using Cubic.V1.Protocol.Auth;
using Cubic.V1.Protocol.World;
using Google.Protobuf;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CubicTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            Ed25519Wallet wallet = Ed25519Wallet.GenerateKeyPairFromPassphrase("hello world", "my password");

            JsonProtocolParser parser = new JsonProtocolParser();
            JsonProtocolFormatter formatter = new JsonProtocolFormatter();

            ClientWebSocket client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:3030/ws"), default);

            string json = formatter.WriteProtocol(new HelloAuthRequest());
            ArraySegment<byte> send = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            await client.SendAsync(send, WebSocketMessageType.Text, true, default);

            while (true)
            {
                ArraySegment<byte> recv = new ArraySegment<byte>(new byte[2048]);
                WebSocketReceiveResult res = await client.ReceiveAsync(recv, default);

                string response = Encoding.Default.GetString(recv.Array, 0, res.Count);

                Console.WriteLine(response);
                IMessage message = parser.ReadProtocol(response);

                if (message is HelloAuthResponse har)
                {
                    byte[] signature = wallet.Sign(har.Key.ToByteArray());
                    json = formatter.WriteProtocol(new ConnectRequest()
                    {
                        Signature = ByteString.CopyFrom(signature),
                        PublicKey = ByteString.CopyFrom(wallet.PublicKey)
                    });

                    send = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                    await client.SendAsync(send, WebSocketMessageType.Text, true, default);
                }
                else if (message is ConnectResponse)
                {
                    json = formatter.WriteProtocol(new HelloWorldRequest());
                    send = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                    await client.SendAsync(send, WebSocketMessageType.Text, true, default);
                }
            }
        }
    }
}
