using MediaClasses;
using System.IO;
using System.Threading.Tasks;

namespace TVSimulator
{
    interface IFileImporter
    {

        void getAllMediaFromDirectory(string path, bool isIncludeSubfolders);

        Task<bool> videoHandler(FileInfo fileInfo, string type, string filePath);

        void musicHandler(string path, string fileName);

        string extractVideoName(string fullName, string compareArg);

        Task<Media> extendVideoInfo(string videoName, string path, string type);

    }
}
