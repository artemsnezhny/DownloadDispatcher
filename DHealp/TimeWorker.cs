using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace DHelp
{
    public class TimeWorker
    {
        public string Directory { get; private set; }
        private ObservableCollection<DObject> dObjects;

        readonly TimeSpan TIME_LEFT = new TimeSpan(1, 0, 0, 0, 0);
        public const string OBJECTS_FILE = "objects.xml";

        public ObservableCollection<DObject> DObjects
        {
            get { return dObjects; }
        }

        public TimeWorker(string directory)
        {
            this.Directory = directory;
            dObjects = new ObservableCollection<DObject>();
        }

        public TimeWorker(string directory, IEnumerable<DObject> dObjects)
        {
            this.Directory = directory;
            this.dObjects = new ObservableCollection<DObject>(dObjects);
        }

        public void AddObject(DObject dObject)
        {
            dObjects.Add(dObject);
        }

        public void AddObject(DirectoryObject dirObject)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                dObjects.Add(new DObject(dirObject, DateTime.Now, DateTime.Now.AddTicks(TIME_LEFT.Ticks)))
                ));
        }

        public void AddObjects(IEnumerable<DirectoryObject> dirObjects)
        {
            foreach (var dObject in dirObjects)
                AddObject(dObject);
        }

        public void RemoveObject(DObject dObject)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                this.dObjects.Remove(dObject)
                ));
        }

        public void AddObjects(IEnumerable<DObject> dObjects)
        {
            foreach (var dObject in dObjects)
                this.dObjects.Add(dObject);
        }

        public void SaveObjects()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(DObject[]));
            using (FileStream fs = new FileStream(OBJECTS_FILE, FileMode.Create))
            {
                formatter.Serialize(fs, this.DObjects.ToArray<DObject>());
            }
        }

        public IEnumerable<DObject> LoadObjects()
        {
            DObject[] dObjects;
            XmlSerializer formatter = new XmlSerializer(typeof(DObject[]));
            using (FileStream fs = new FileStream(OBJECTS_FILE, FileMode.Open))
            {
                dObjects = (DObject[])formatter.Deserialize(fs);
            }
            return dObjects;
        }

        public void Work()
        {
            while (true)
            {
                //Sync
                var dirObjects = DirectoryWorker.GetDObjects(Directory);

                var dirLookup = dirObjects.ToLookup(dir => dir.Name);
                var diff = dObjects.Where(dObj => (!dirLookup.Contains(dObj.Name)));

                for (int i = 0; i < diff.Count(); i++)
                {
                    RemoveObject(diff.ElementAt(i));
                }

                for (int i = 0; i < dirObjects.Count(); i++)
                {
                    if (!this.dObjects.Any(dObject => dObject.Name == dirObjects.ElementAt(i).Name))
                    {
                        AddObject(dirObjects.ElementAt(i));
                    }
                }

                List<DObject> objectsForDelete = new List<DObject>();
                foreach (var obj in this.dObjects)
                {
                    if (obj.DeleteDate < DateTime.Now)
                    {
                        objectsForDelete.Add(obj);
                    }
                    else
                    {
                        obj.SoonRemoved = (obj.DeleteDate.Subtract(DateTime.Now) <= DObject.WARNING_TIME);
                    }
                }

                for (int i = 0; i < objectsForDelete.Count(); i++)
                {
                    RemoveObject(objectsForDelete[i]);
                    DirectoryWorker.DeleteDObject(Directory, objectsForDelete[i]);
                }

                //Wait...
                SaveObjects();
                Thread.Sleep(new TimeSpan(0, 15, 0));
            }
        }
    }
}
