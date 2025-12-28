namespace Bourse.Interfaces
{
    public interface IFileSaver
    {
        Task<string?> SaveToDownloadsAsync(string filename, byte[] data);

        bool Download(string source);
    }

}
