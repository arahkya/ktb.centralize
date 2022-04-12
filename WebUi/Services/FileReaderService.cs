using System;
using System.Collections.ObjectModel;
using WebUi.ViewModels;

namespace WebUi.Services
{
    public class FileReaderService : IObservable<IEnumerable<DisputeViewModel>>
    {
        private readonly List<IObserver<IEnumerable<DisputeViewModel>>> observers = new();
        public readonly ObservableCollection<DisputeViewModel> Disputes = new();

        public async Task ReadFileAsync(Stream fileStream)
        {
            using var memoryStream = new MemoryStream();
            var buffer = new byte[16 * 1024];
            var index = 0;

            while ((index = await fileStream.ReadAsync(buffer)) > 0)
            {
                memoryStream.Write(buffer, 0, index);
            }

            index = 0;
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(memoryStream);
            var line = string.Empty;
            do
            {
                line = await streamReader.ReadLineAsync();
                if (line == null)
                {
                    continue;
                }

                if (index > 0)
                {
                    var columns = line.Split(',');
                    var createdOn = DateTime.MinValue;
                    var terminal = columns[1];
                    var branch = columns[2];
                    var disputeType = columns[3];

                    if (!DateTime.TryParse(columns[0], out createdOn))
                    {
                        throw new FormatException($"Transaction Date format invalid ({columns[0]})");
                    }

                    Disputes.Add(new DisputeViewModel
                    {
                        Branch = branch,
                        TerminalID = terminal,
                        DisputeType = disputeType,
                        CreatedOn = createdOn
                    });
                }

                index++;
            }
            while (line != null);

            observers.ForEach(p => p.OnNext(Disputes));
        }

        public IDisposable Subscribe(IObserver<IEnumerable<DisputeViewModel>> observer)
        {
            observers.Add(observer);

            return new ObserverDispose(observers, observer);
        }

        private class ObserverDispose : IDisposable
        {
            private readonly List<IObserver<IEnumerable<DisputeViewModel>>> observers;
            private readonly IObserver<IEnumerable<DisputeViewModel>> observer;

            public ObserverDispose(List<IObserver<IEnumerable<DisputeViewModel>>> observers, IObserver<IEnumerable<DisputeViewModel>> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                observers.Remove(observer);
            }
        }
    }
}
