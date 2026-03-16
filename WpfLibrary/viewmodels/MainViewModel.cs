using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public enum AppView { Game, Leaderboard, Statistics, Settings }
    public class MainViewModel : BaseViewModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IStatisticsService _statisticsService;
        private AppView _currentView;

        public GameViewModel GameViewModel { get; }
        public LeaderboardViewModel LeaderboardViewModel { get; }
        public StatisticsViewModel StatisticsViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }

        public AppView CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public bool IsGameView => CurrentView == AppView.Game;
        public bool IsLeaderboardView => CurrentView == AppView.Leaderboard;
        public bool IsStatisticsView => CurrentView == AppView.Statistics;
        public bool IsSettingsView => CurrentView == AppView.Settings;

        public ICommand ShowGameCommand { get; }
        public ICommand ShowLeaderboardCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand StartNewGameCommand { get; }

        private BaseViewModel _currentPage;
        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set
            {
                SetProperty(ref _currentPage, value);
                NotifyViewChanged();
            }
        }

        public MainViewModel(
            GameViewModel gameViewModel,
            LeaderboardViewModel leaderboardViewModel,
            StatisticsViewModel statisticsViewModel,
            SettingsViewModel settingsViewModel,
            IRecordRepository recordRepository,
            IStatisticsService statisticsService)
        {
            GameViewModel = gameViewModel;
            LeaderboardViewModel = leaderboardViewModel;
            StatisticsViewModel = statisticsViewModel;
            SettingsViewModel = settingsViewModel;
            _recordRepository = recordRepository;
            _statisticsService = statisticsService;

            ShowGameCommand = new RelayCommand(NavigateToGame);
            ShowLeaderboardCommand = new RelayCommand(NavigateToLeaderboard);
            ShowStatisticsCommand = new RelayCommand(NavigateToStatistics);
            ShowSettingsCommand = new RelayCommand(NavigateToSettings);
            StartNewGameCommand = new RelayCommand(StartNewGame);

            GameViewModel.GameWon += OnGameWon;
            GameViewModel.GameLost += OnGameLost;

            SettingsViewModel.SettingsSaved += StartNewGame;

            var settings = SettingsViewModel.GetCurrentSettings();
            GameViewModel.LoadGame(settings.GetDifficulty(), settings.CellSize);
        }

        public void NavigateToGamePublic() => NavigateToGame();

        private void NavigateToGame()
        {
            CurrentView = AppView.Game;
            CurrentPage = GameViewModel;
        }

        private void NavigateToLeaderboard()
        {
            CurrentView = AppView.Leaderboard;
            LeaderboardViewModel.Refresh();
            CurrentPage = LeaderboardViewModel;
        }

        private void NavigateToStatistics()
        {
            CurrentView = AppView.Statistics;
            StatisticsViewModel.Refresh();
            CurrentPage = StatisticsViewModel;
        }
        private void NavigateToSettings()
        {
            CurrentView = AppView.Settings;
            CurrentPage = SettingsViewModel;
        }

        private void StartNewGame()
        {
            var settings = SettingsViewModel.GetCurrentSettings();
            GameViewModel.LoadGame(settings.GetDifficulty(), settings.CellSize);
            NavigateToGame();
        }

        private void OnGameWon(int timeSeconds)
        {
            var playerName = string.IsNullOrWhiteSpace(GameViewModel.PlayerName)
                ? "Anonymous"
                : GameViewModel.PlayerName;

            var difficulty = SettingsViewModel.GetCurrentSettings().SelectedDifficulty;

            _recordRepository.Save(new GameRecord(playerName, timeSeconds, difficulty));
            _statisticsService.RecordWin(difficulty, timeSeconds);
            StatisticsViewModel.Refresh();
        }
        private void OnGameLost()
        {
            var difficulty = SettingsViewModel.GetCurrentSettings().SelectedDifficulty;
            _statisticsService.RecordLoss(difficulty);
            StatisticsViewModel.Refresh();
        }

        private void NotifyViewChanged()
        {
            OnPropertyChanged(nameof(IsGameView));
            OnPropertyChanged(nameof(IsLeaderboardView));
            OnPropertyChanged(nameof(IsStatisticsView));
            OnPropertyChanged(nameof(IsSettingsView));
        }
    }
}