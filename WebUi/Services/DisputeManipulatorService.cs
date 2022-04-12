using System.Collections.ObjectModel;
using WebUi.ViewModels;

namespace WebUi.Services
{
    public class DisputeManipulatorService : IObserver<IEnumerable<DisputeViewModel>>, IObservable<IEnumerable<DisputeGroupByBranchViewModel>>
    {
        private readonly List<IObserver<IEnumerable<DisputeGroupByBranchViewModel>>> observers = new();

        public ObservableCollection<DisputeGroupByBranchViewModel> Groups { get; set; } = new();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IEnumerable<DisputeViewModel> value)
        {
            foreach(var group in value.GroupBy(p => Convert.ToInt16(p.Branch)).Select(p => new DisputeGroupByBranchViewModel
            {
                Branch = p.Key,
                ATMErrorCount = p.Count(p => p.DisputeType == "ATM") + p.Count(p => p.DisputeType == "RCM" && (p.TerminalID.ToLower().StartsWith("k3") || p.TerminalID.ToLower().StartsWith("k4"))),
                RCMErrorCount = p.Count(p => p.DisputeType == "RCM" && (!p.TerminalID.ToLower().StartsWith("k3") || !p.TerminalID.ToLower().StartsWith("k4"))),
            }).OrderBy(p => p.Branch))
            {
                Groups.Add(group);
            }

            observers.ForEach(p => p.OnNext(Groups));
        }

        public IDisposable Subscribe(IObserver<IEnumerable<DisputeGroupByBranchViewModel>> observer)
        {
            observers.Add(observer);

            return new Disposer(observers, observer);
        }

        private class Disposer : IDisposable
        {
            private readonly List<IObserver<IEnumerable<DisputeGroupByBranchViewModel>>> observers;
            private readonly IObserver<IEnumerable<DisputeGroupByBranchViewModel>> observer;

            public Disposer(List<IObserver<IEnumerable<DisputeGroupByBranchViewModel>>> observers, IObserver<IEnumerable<DisputeGroupByBranchViewModel>> observer)
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
