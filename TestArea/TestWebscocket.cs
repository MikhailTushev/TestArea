using System;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestArea;

public class TestWebscocket
{
    public static async Task Start()
    {
        var can = new CancellationTokenSource();

        var cl = new ClientWebSocket();
        await cl.ConnectAsync(new Uri("ws://localhost:5050/ws"), CancellationToken.None);

        var timerInterval = TimeSpan.FromSeconds(10);
        using var asyncTimer =
            new AsyncTimer(async (client, cancellationToken) => { await HelloWorld(client, cancellationToken); },
                timerInterval, cl, can.Token);

        asyncTimer.Start();

        while (cl.State is WebSocketState.Open)
        {
            Console.WriteLine("I AM WORKING!!!");
            await Task.Delay(1000, can.Token);
        }

        asyncTimer.Stop();

        await cl.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", can.Token);

        Console.ReadLine();
    }

    static async Task HelloWorld(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        byte[] buf = new byte[1056];
        var result = await webSocket.ReceiveAsync(buf, CancellationToken.None);
        Console.WriteLine(Encoding.ASCII.GetString(buf, 0, result.Count));
    }
}