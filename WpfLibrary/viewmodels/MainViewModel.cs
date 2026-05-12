using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public enum AppView { Game, TimeAttack, Leaderboard, History, Statistics, Achievements, Settings }
    public class MainViewModel : BaseViewModel
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IStatisticsService _statisticsService;
        private readonly IAchievementService _achievementService;
        private readonly IGameHistoryService _historyService;

        private int _winStreak;

        public TimeAttackViewModel TimeAttackViewModel { get; }
        private AppView _currentView;

        public GameViewModel GameViewModel { get; }
        public LeaderboardViewModel LeaderboardViewModel { get; }
        public GameHistoryViewModel HistoryViewModel { get; }
        public StatisticsViewModel StatisticsViewModel { get; }
        public AchievementViewModel AchievementViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }

        public AppView CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public bool IsGameView => CurrentView == AppView.Game;
        public bool IsTimeAttackView => CurrentView == AppView.TimeAttack;
        public bool IsLeaderboardView => CurrentView == AppView.Leaderboard;
        public bool IsHistoryView => CurrentView == AppView.History;
        public bool IsStatisticsView => CurrentView == AppView.Statistics;
        public bool IsAchievementsView => CurrentView == AppView.Achievements;
        public bool IsSettingsView => CurrentView == AppView.Settings;

        public ICommand ShowGameCommand { get; }
        public ICommand ShowTimeAttackCommand { get; }
        public ICommand ShowLeaderboardCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand ShowAchievementsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand StartNewGameCommand { get; }

        private BaseViewModel _currentPage;
        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set { SetProperty(ref _currentPage, value); NotifyViewChanged(); }
        }

        public MainViewModel(
            GameViewModel gameViewModel,
            LeaderboardViewModel leaderboardViewModel,
            GameHistoryViewModel historyViewModel,
            TimeAttackViewModel timeAttackViewModel,
            StatisticsViewModel statisticsViewModel,
            AchievementViewModel achievementViewModel,
            SettingsViewModel settingsViewModel,
            IRecordRepository recordRepository,
            IStatisticsService statisticsService,
            IAchievementService achievementService,
            IGameHistoryService historyService)
        {
            GameViewModel = gameViewModel;
            LeaderboardViewModel = leaderboardViewModel;
            HistoryViewModel = historyViewModel;
            TimeAttackViewModel = timeAttackViewModel;
            StatisticsViewModel = statisticsViewModel;
            AchievementViewModel = achievementViewModel;
            SettingsViewModel = settingsViewModel;
            _recordRepository = recordRepository;
            _statisticsService = statisticsService;
            _achievementService = achievementService;
            _historyService = historyService;

            ShowGameCommand = new RelayCommand(NavigateToGame);
            ShowTimeAttackCommand = new RelayCommand(NavigateToTimeAttack);
            ShowLeaderboardCommand = new RelayCommand(NavigateToLeaderboard);
            ShowHistoryCommand = new RelayCommand(NavigateToHistory);
            ShowStatisticsCommand = new RelayCommand(NavigateToStatistics);
            ShowAchievementsCommand = new RelayCommand(NavigateToAchievements);
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

        private void NavigateToTimeAttack()
        {
            CurrentView = AppView.TimeAttack;
            CurrentPage = TimeAttackViewModel;
        }

        private void NavigateToLeaderboard()
        {
            CurrentView = AppView.Leaderboard;
            LeaderboardViewModel.Refresh();
            CurrentPage = LeaderboardViewModel;
        }

        private void NavigateToHistory()
        {
            CurrentView = AppView.History;
            HistoryViewModel.Refresh();
            CurrentPage = HistoryViewModel;
        }

        private void NavigateToStatistics()
        {
            CurrentView = AppView.Statistics;
            StatisticsViewModel.Refresh();
            CurrentPage = StatisticsViewModel;
        }

        private void NavigateToAchievements()
        {
            CurrentView = AppView.Achievements;
            AchievementViewModel.Refresh();
            CurrentPage = AchievementViewModel;
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
            _winStreak++;
            var difficulty = SettingsViewModel.GetCurrentSettings().SelectedDifficulty;
            var playerName = string.IsNullOrWhiteSpace(GameViewModel.PlayerName)
                ? "Anonymous" : GameViewModel.PlayerName;

            _recordRepository.Save(new GameRecord(playerName, timeSeconds, difficulty));
            _statisticsService.RecordWin(difficulty, timeSeconds);
            _historyService.Record(new GameHistoryEntry(difficulty, true, timeSeconds));

            AchievementViewModel.OnWin(new GameResult(difficulty, timeSeconds, GameViewModel.FlagsUsed, _winStreak));
            StatisticsViewModel.Refresh();
            HistoryViewModel.Refresh();
        }
        private void OnGameLost()
        {
            _winStreak = 0;
            var difficulty = SettingsViewModel.GetCurrentSettings().SelectedDifficulty;

            _statisticsService.RecordLoss(difficulty);
            _historyService.Record(new GameHistoryEntry(difficulty, false, GameViewModel.ElapsedSeconds));

            StatisticsViewModel.Refresh();
            HistoryViewModel.Refresh();
        }

        private void NotifyViewChanged()
        {
            OnPropertyChanged(nameof(IsGameView));
            OnPropertyChanged(nameof(IsTimeAttackView));
            OnPropertyChanged(nameof(IsLeaderboardView));
            OnPropertyChanged(nameof(IsHistoryView));
            OnPropertyChanged(nameof(IsStatisticsView));
            OnPropertyChanged(nameof(IsAchievementsView));
            OnPropertyChanged(nameof(IsSettingsView));
        }
    }
}