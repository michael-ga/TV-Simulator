using MediaClasses;

namespace TVSimulator
{
    interface IFileImporter
    {
        void LoadLocalFilesFromDirectory(string path,bool includeSubfolders);
        void removeFileFromDB(string name);
        Media getFileFromDB(string name);
    }
}
