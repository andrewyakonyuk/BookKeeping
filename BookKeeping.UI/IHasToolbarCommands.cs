using System.Collections.ObjectModel;
using BookKeeping.UI.ViewModels;

namespace BookKeeping.UI
{
    public interface IHasToolbarCommands
    {
        ObservableCollection<ToolbarCommandViewModel> Toolbar { get; }
    }
}
