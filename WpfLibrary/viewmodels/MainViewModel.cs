using System.Windows.Input;
using WpfLibrary.Commands;

namespace WpfLibrary.viewmodels
{
    public enum AppView { Game, Leaderboard, Settings }

    public class MainViewModel : BaseViewModel
    {
        private AppView _currentView;

        public GameViewModel GameViewModel { get; }
        public LeaderboardViewModel LeaderboardViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }

        public AppView CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public bool IsGameView => CurrentView == AppView.Game;
        public bool IsLeaderboardView => CurrentView == AppView.Leaderboard;
        public bool IsSettingsView => CurrentView == AppView.Settings;

        public ICommand ShowGameCommand { get; }
        public ICommand ShowLeaderboardCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand StartNewGameCommand { get; }

        public MainViewModel(GameViewModel gameViewModel, LeaderboardViewModel leaderboardViewModel,
            SettingsViewModel settingsViewModel)
        {
            GameViewModel = gameViewModel;
            LeaderboardViewModel = leaderboardViewModel;
            SettingsViewModel = settingsViewModel;

            ShowGameCommand = new RelayCommand(NavigateToGame);
            ShowLeaderboardCommand = new RelayCommand(NavigateToLeaderboard);
            ShowSettingsCommand = new RelayCommand(NavigateToSettings);
            StartNewGameCommand = new RelayCommand(StartNewGame);

            StartNewGame();
        }

        private void NavigateToGame()
        {
            CurrentView = AppView.Game;
            NotifyViewChanged();
        }

        private void NavigateToLeaderboard()
        {
            CurrentView = AppView.Leaderboard;
            LeaderboardViewModel.Refresh();
            NotifyViewChanged();
        }

        private void NavigateToSettings()
        {
            CurrentView = AppView.Settings;
            NotifyViewChanged();
        }

        private void StartNewGame()
        {
            var difficulty = SettingsViewModel.GetCurrentSettings().GetDifficulty();
            GameViewModel.LoadGame(difficulty);
            NavigateToGame();
        }

        private void NotifyViewChanged()
        {
            OnPropertyChanged(nameof(IsGameView));
            OnPropertyChanged(nameof(IsLeaderboardView));
            OnPropertyChanged(nameof(IsSettingsView));
        }
    }
}