using System.Windows.Input;

namespace BookKeeping.UI.ViewModels
{
    public class ToolbarCommandViewModel : CommandViewModel
    {
        public ToolbarCommandViewModel(string title, ICommand command)
            : base(title, command)
        {
        }
    }
}