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

        public event Action? SettingsSaved;
        public DifficultyLevel SelectedDifficulty
        {
            get => _settings.SelectedDifficulty;
            set
            {
                _settings.SelectedDifficulty = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCustomSelected));
                OnPropertyChanged(nameof(ValidationError));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public bool IsCustomSelected => SelectedDifficulty == DifficultyLevel.Custom;

        public int CustomRows
        {
            get => _settings.CustomRows;
            set
            {
                _settings.CustomRows = Math.Max(2, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationError));
                OnPropertyChanged(nameof(CanSave));
            }
        }
        public int CustomColumns
        {
            get => _settings.CustomColumns;
            set
            {
                _settings.CustomColumns = Math.Max(2, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationError));
                OnPropertyChanged(nameof(CanSave));
            }
        }
        public int CustomMineCount
        {
            get => _settings.CustomMineCount;
            set
            {
                _settings.CustomMineCount = Math.Max(1, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationError));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string Theme
        {
            get => _settings.Theme;
            set { _settings.Theme = value; OnPropertyChanged(); }
        }

        public int CellSize
        {
            get => _settings.CellSize;
            set { _settings.CellSize = Math.Clamp(value, 24, 48); OnPropertyChanged(); }
        }

        public string? ValidationError
        {
            get
            {
                if (!IsCustomSelected) return null;
                if (CustomRows < 2 || CustomColumns < 2)
                    return "Мінімальний розмір поля: 2×2";
                int maxMines = CustomRows * CustomColumns - 1;
                if (CustomMineCount >= CustomRows * CustomColumns)
                    return $"Кількість мін ({CustomMineCount}) не може бути ≥ кількості клітинок ({CustomRows * CustomColumns}). Максимум: {maxMines}";
                return null;
            }
        }

        public bool CanSave => ValidationError == null;
        public ICommand SaveCommand { get; }

        public SettingsViewModel(JsonSettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = settingsService.Load();
            SaveCommand = new RelayCommand(Save, () => CanSave);
        }

        public GameSettings GetCurrentSettings() => _settings;

        private void Save()
        {
            _settingsService.Save(_settings);
            SettingsSaved?.Invoke();
        }
    }
}