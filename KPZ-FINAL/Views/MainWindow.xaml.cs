using System.Windows;
using System.Windows.Threading;
using WpfLibrary.services;
using WpfLibrary.Services;
using WpfLibrary.viewmodels;

namespace KPZ_FINAL.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = BuildMainViewModel();
            DataContext = vm;
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => vm.NavigateToGamePublic());
        }

        private static MainViewModel BuildMainViewModel()
        {
            var recordRepository = new JsonRecordRepository();
            var settingsService = new JsonSettingsService();
            var statisticsService = new JsonStatisticsService();
            var achievementService = new AchievementService();
            var historyService = new JsonGameHistoryService();
            var taTimerService = new TimerService();
            var mineGenerator = new RandomMineGenerator();
            var taService = new TimeAttackService(mineGenerator);
            var gameService = new GameService(mineGenerator);
            var timerService = new TimerService();

            var gameVm = new GameViewModel(gameService, timerService);
            var leaderboardVm = new LeaderboardViewModel(recordRepository);
            var historyVm = new GameHistoryViewModel(historyService);
            var timeAttackVm = new TimeAttackViewModel(taService, taTimerService);
            var statisticsVm = new StatisticsViewModel(statisticsService);
            var achievementVm = new AchievementViewModel(achievementService);
            var settingsVm = new SettingsViewModel(settingsService);

            return new MainViewModel(
                gameVm, leaderboardVm, historyVm, timeAttackVm, statisticsVm, achievementVm, settingsVm,
                recordRepository, statisticsService, achievementService, historyService);
        }
    }
}