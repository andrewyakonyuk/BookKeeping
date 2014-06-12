using BookKeeping.Persistent.Storage;
using BookKeeping.UI;
using BookKeeping.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace BookKeeping.App.ViewModels
{
    public class EventHistoryViewModel : WorkspaceViewModel
    {
        public EventHistoryViewModel()
        {
            DisplayName = T("EventHistory");

            Events = new ObservableCollection<string>(GetEvents());

            ReloadCmd = new DelegateCommand(_ =>
            {
                Events.Clear();
                foreach (var item in GetEvents())
                {
                    Events.Add(item);
                }
            });
        }

        protected IEnumerable<string> GetEvents()
        {
            return Context.Current.EventStore.LoadEventStream(0, int.MaxValue).Events.Select(t => t.ToString()).Reverse();
        }

        public ICommand ReloadCmd { get; private set; }

        public ObservableCollection<string> Events { get; private set; }
    }
}
