using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly IStatisticsService _statisticsService;
        private GameStatistics _stats;

        public int TotalGames => _stats.TotalGames;
        public int TotalWins => _stats.TotalWins;
        public int TotalLosses => _stats.TotalLosses;
        public double WinRate => _stats.WinRate;
        public string AverageTime => _stats.AverageTime;
        public DifficultyStats UltraEasyStats => _stats.UltraEasy;
        public DifficultyStats EasyStats => _stats.Easy;
        public DifficultyStats MediumStats => _stats.Medium;
        public DifficultyStats HardStats => _stats.Hard;

        private DifficultyLevel _selectedDifficulty = DifficultyLevel.Easy;
        public DifficultyLevel SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                SetProperty(ref _selectedDifficulty, value);
                OnPropertyChanged(nameof(SelectedStats));
            }
        }

        public DifficultyStats SelectedStats => _stats.GetStats(SelectedDifficulty);

        public ICommand ResetCommand { get; }

        public StatisticsViewModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
            _stats = _statisticsService.Load();
            ResetCommand = new RelayCommand(ResetStatistics);
        }
        public void Refresh()
        {
            _stats = _statisticsService.Load();
            NotifyAllProperties();
        }

        private void ResetStatistics()
        {
            _statisticsService.Reset();
            _stats = _statisticsService.Load();
            NotifyAllProperties();
        }

        private void NotifyAllProperties()
        {
            OnPropertyChanged(nameof(TotalGames));
            OnPropertyChanged(nameof(TotalWins));
            OnPropertyChanged(nameof(TotalLosses));
            OnPropertyChanged(nameof(WinRate));
            OnPropertyChanged(nameof(AverageTime));
            OnPropertyChanged(nameof(UltraEasyStats));
            OnPropertyChanged(nameof(EasyStats));
            OnPropertyChanged(nameof(MediumStats));
            OnPropertyChanged(nameof(HardStats));
            OnPropertyChanged(nameof(SelectedStats));
        }
    }
}