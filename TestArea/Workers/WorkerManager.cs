using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TestArea.Workers;

public class WorkerManager<T>
    where T : IMessage
{
    private ConcurrentDictionary<long, Worker<T>> _workers;

    public WorkerManager()
    {
        _workers = new ConcurrentDictionary<long, Worker<T>>();
    }

    public async Task<Worker<T>> TryCreateWorker(long id)
    {
        var worker = new Worker<T>(id);
        _workers.TryAdd(id, worker);

        await worker.Start();
        return worker;
    }

    public async Task TryPushMessage(long id, T collectionMessages)
    {
        if (!_workers.TryGetValue(id, out var worker))
        {
            worker = await TryCreateWorker(id);
        }

        worker.PushMessage(collectionMessages);
    }
}