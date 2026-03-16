using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public class AchievementViewModel : BaseViewModel
    {
        private readonly IAchievementService _achievementService;

        public ObservableCollection<Achievement> AllAchievements { get; } = new();
        public ObservableCollection<Achievement> NewAchievements { get; } = new();

        private int _unlockedCount;
        public int UnlockedCount
        {
            get => _unlockedCount;
            private set => SetProperty(ref _unlockedCount, value);
        }

        public int TotalCount => AllAchievements.Count;

        private bool _hasNewAchievements;
        public bool HasNewAchievements
        {
            get => _hasNewAchievements;
            private set => SetProperty(ref _hasNewAchievements, value);
        }

        public ICommand ResetCommand { get; }
        public ICommand DismissNewCommand { get; }

        public AchievementViewModel(IAchievementService achievementService)
        {
            _achievementService = achievementService;
            ResetCommand = new RelayCommand(ResetAchievements);
            DismissNewCommand = new RelayCommand(DismissNew);
            LoadAll();
        }
        public void OnWin(DifficultyLevel difficulty, int timeSeconds, int flagsUsed, int winStreak)
        {
            var newOnes = _achievementService.CheckOnWin(
                difficulty, timeSeconds, flagsUsed, winStreak);

            if (newOnes.Any())
            {
                NewAchievements.Clear();
                foreach (var a in newOnes)
                    NewAchievements.Add(a);

                HasNewAchievements = true;
            }

            LoadAll();
        }

        public void Refresh() => LoadAll();

        private void LoadAll()
        {
            AllAchievements.Clear();
            foreach (var a in _achievementService.GetAll())
                AllAchievements.Add(a);

            UnlockedCount = AllAchievements.Count(a => a.IsUnlocked);
            OnPropertyChanged(nameof(TotalCount));
        }

        private void ResetAchievements()
        {
            _achievementService.Reset();
            NewAchievements.Clear();
            HasNewAchievements = false;
            LoadAll();
        }

        private void DismissNew()
        {
            NewAchievements.Clear();
            HasNewAchievements = false;
        }
    }
}