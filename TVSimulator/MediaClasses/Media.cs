using System;
using System.Collections.Generic;
using System.Text;

namespace MediaClasses
{
    class Media
    {
        private string path;
        private string name;
        private string runtime;
        private string gnere;

        public Media(string path,string name, string runtime = "")
        {
            this.path = path;
            this.name = name;
            this.runtime = runtime;
        }

        protected string Runtime { get => runtime; set => runtime = value; }
        protected string Gnere { get => gnere; set => gnere = value; }
        protected string Path { get => path; set => path = value; }
        protected string Name { get => name; set => name = value; }
    }
}
