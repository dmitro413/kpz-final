using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface IStatisticsService
    {
        GameStatistics Load();

        void Save(GameStatistics statistics);

        void RecordWin(DifficultyLevel difficulty, int timeSeconds);

        void RecordLoss(DifficultyLevel difficulty);
        void Reset();
    }
}