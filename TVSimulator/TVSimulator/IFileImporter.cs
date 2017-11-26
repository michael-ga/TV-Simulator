using MediaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVSimulator
{
    interface IFileImporter
    {
        void LoadLocalFilesFromDirectory(string path);
        void removeFileFromDB(string name);
        Media getFileFromDB(string name);
    }
}
