using System.IO;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IUploadService
    {
        Task UploadStream(Stream stream, string filename, MemoryStream data);

        Task UploadToFavorite(string filename, MemoryStream data);
    }
}