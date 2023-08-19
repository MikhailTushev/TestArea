using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TestArea;

public class AsyncTimer : IDisposable
{
    private readonly Func<ClientWebSocket, CancellationToken, Task> _callback;
    private readonly TimeSpan _interval;
    private Timer _timer;
    private bool _isRunning;
    private readonly ClientWebSocket _webSocketClient;
    private readonly CancellationToken _cancellationToken;

    public AsyncTimer(Func<ClientWebSocket, CancellationToken, Task> callback, TimeSpan interval,
        ClientWebSocket webSocketClient, CancellationToken cancellationToken)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        _interval = interval;
        _webSocketClient = webSocketClient;
        _isRunning = false;
        _cancellationToken = cancellationToken;
    }

    public void Start()
    {
        if (_isRunning)
        {
            return;
        }

        _timer = new Timer(async _ => await TimerCallback(), null, _interval, _interval);
        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _isRunning = false;
    }

    private async Task TimerCallback()
    {
        if (_webSocketClient.State is not WebSocketState.Open || _cancellationToken.IsCancellationRequested)
        {
            Stop();
        }

        try
        {
            await _callback(_webSocketClient, _cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in timer callback: {ex}");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _isRunning = false;
    }
}