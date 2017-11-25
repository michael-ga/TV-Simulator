using LiteDB;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVSimulator
{
    class Database
    {
        LiteDatabase db;
        public Database ()
        {
            db = new LiteDatabase(@"C:\\TVSimulatorDB\MyData.db");
        }


        public void removeMediaCollection(string collName)
        {
            var media1 = db.GetCollection<Media>(collName);
            media1.Delete(Query.All(Query.Descending));
        }
    }
}
