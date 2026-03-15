using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface ISettingsService
    {
        GameSettings Load();
        void Save(GameSettings settings);
    }
}