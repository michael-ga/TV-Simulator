using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperClasses
{
    public class MyTaskProgressReport
    {
        //current progress
        public int CurrentProgressAmount { get; set; }
        //total progress
        public int TotalProgressAmount { get; set; }
        //some message to pass to the UI of current progress
        public string CurrentProgressMessage { get; set; }
    }
    // represent A tuple of path and bool indicates inclusion of subfolders
    public class costumPath
    {
        string path;
        bool isIncludeSubFolders;
        public costumPath(string p, bool v)
        {
            Path = p;
            IsIncludeSubFolders = v;
        }
        public bool isExist(string _path)
        {
            return path.Equals(path);
        }
        public string Path { get => path; set => path = value; }
        public bool IsIncludeSubFolders { get => isIncludeSubFolders; set => isIncludeSubFolders = value; }
    }
}
