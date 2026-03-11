using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface IRecordRepository
    {
        IReadOnlyList<GameRecord> GetAll();
        IReadOnlyList<GameRecord> GetByDifficulty(DifficultyLevel difficulty);
        void Save(GameRecord record);
        void Clear();
    }
}
