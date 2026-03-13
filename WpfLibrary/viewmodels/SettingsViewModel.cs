using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.Services;
namespace WpfLibrary.viewmodels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly JsonSettingsService _settingsService;
        private GameSettings _settings;

        public DifficultyLevel SelectedDifficulty
        {
            get => _settings.SelectedDifficulty;
            set
            {
                _settings.SelectedDifficulty = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCustomSelected));
            }
        }

        public bool IsCustomSelected => SelectedDifficulty == DifficultyLevel.Custom;

        public int CustomRows
        {
            get => _settings.CustomRows;
            set { _settings.CustomRows = value; OnPropertyChanged(); }
        }

        public int CustomColumns
        {
            get => _settings.CustomColumns;
            set { _settings.CustomColumns = value; OnPropertyChanged(); }
        }

        public int CustomMineCount
        {
            get => _settings.CustomMineCount;
            set { _settings.CustomMineCount = value; OnPropertyChanged(); }
        }

        public string Theme
        {
            get => _settings.Theme;
            set { _settings.Theme = value; OnPropertyChanged(); }
        }

        public int CellSize
        {
            get => _settings.CellSize;
            set { _settings.CellSize = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel(JsonSettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = settingsService.Load();
            SaveCommand = new RelayCommand(Save);
        }

        public GameSettings GetCurrentSettings() => _settings;

        private void Save() => _settingsService.Save(_settings);
    }
}
