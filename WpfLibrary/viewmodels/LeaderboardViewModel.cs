using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;
namespace WpfLibrary.viewmodels
{
    public class LeaderboardViewModel : BaseViewModel
    {
        private readonly IRecordRepository _recordRepository;
        private DifficultyLevel _selectedDifficulty;

        public ObservableCollection<GameRecord> Records { get; } = new();

        public DifficultyLevel SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                SetProperty(ref _selectedDifficulty, value);
                LoadRecords();
            }
        }

        public ICommand ClearCommand { get; }

        public LeaderboardViewModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
            ClearCommand = new RelayCommand(ClearRecords);
            SelectedDifficulty = DifficultyLevel.Easy;
        }

        public void Refresh() => LoadRecords();

        private void LoadRecords()
        {
            Records.Clear();
            foreach (var record in _recordRepository.GetByDifficulty(SelectedDifficulty))
                Records.Add(record);
        }

        private void ClearRecords()
        {
            _recordRepository.Clear();
            LoadRecords();
        }
    }
}
