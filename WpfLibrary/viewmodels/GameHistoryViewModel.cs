using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public class GameHistoryViewModel : BaseViewModel
    {
        private readonly IGameHistoryService _historyService;

        public ObservableCollection<GameHistoryEntry> Entries { get; } = new();

        private int _totalShown = 20;
        public int TotalShown
        {
            get => _totalShown;
            set { SetProperty(ref _totalShown, value); Refresh(); }
        }

        public int WinsCount => Entries.Count(e => e.IsWon);
        public int LossesCount => Entries.Count(e => !e.IsWon);

        public ICommand ClearCommand { get; }
        public ICommand RefreshCommand { get; }
        public GameHistoryViewModel(IGameHistoryService historyService)
        {
            _historyService = historyService;
            ClearCommand = new RelayCommand(ClearHistory);
            RefreshCommand = new RelayCommand(Refresh);
            Refresh();
        }
        public void Refresh()
        {
            Entries.Clear();
            foreach (var entry in _historyService.GetRecent(TotalShown))
                Entries.Add(entry);

            OnPropertyChanged(nameof(WinsCount));
            OnPropertyChanged(nameof(LossesCount));
        }

        private void ClearHistory()
        {
            _historyService.Clear();
            Refresh();
        }
    }
}