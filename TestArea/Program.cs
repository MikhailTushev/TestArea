using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestArea.Workers;

namespace TestArea;

class Program
{
    static async Task Main(string[] args)
    {
        var manager = new WorkerManager<SportLevelEventStatusRequestDto>();
        int i = 1;
        while (true)
        {
            var b = CreateObjects(i);
            await manager.TryPushMessage(b.TranslationId, b);
            await Task.Delay(100);
            i++;
        }
    }

    static SportLevelEventStatusRequestDto CreateObjects(int i)
    {
        return new SportLevelEventStatusRequestDto(
            1,
            Random.Shared.Next(1, 20),
            DateTimeOffset.Now,
            i,
            Random.Shared.Next(0, 10),
            Random.Shared.Next(0, 10),
            Random.Shared.Next(0, 10),
            Random.Shared.Next(0, 10),
            Random.Shared.Next(0, 10));
    }

    private class FileReader : IEnumerable<string>
    {
        private readonly IEnumerator<string> _lineEnumerator;

        public FileReader(string filePath)
        {
            _lineEnumerator = File.ReadLines(filePath).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _lineEnumerator;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return (IEnumerator<string>)GetEnumerator();
        }
    }
}