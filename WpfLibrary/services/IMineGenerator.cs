
namespace WpfLibrary.services
{
    public interface IMineGenerator
    {
        void PlaceMines(models.Board board, int safeRow, int safeCol);
    }
}

