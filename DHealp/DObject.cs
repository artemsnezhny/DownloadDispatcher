using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHelp
{
    [Serializable]
    public enum DTypeObject
    { 
        File,
        Directory
    }

    [Serializable]
    public class DObject : INotifyPropertyChanged
    {
        public static readonly TimeSpan WARNING_TIME = new TimeSpan(24, 0, 0);

        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        private DateTime deleteDate;
        public DateTime DeleteDate
        {
            get { return deleteDate; }
            set
            {
                deleteDate = value;
                RaisePropertyChanged("DeleteDate");
            }
        }
        public DTypeObject DType { get; set; }
        private bool soonRemoved;
        public bool SoonRemoved {
            get { return soonRemoved; }
            set 
            {
                soonRemoved = value;
                RaisePropertyChanged("SoonRemoved");
            } 
        }
        
        public DObject(string name, DateTime addDate, DateTime deleteDate, DTypeObject dType)
        {
            this.Name = name;
            this.AddDate = addDate;
            this.DeleteDate = deleteDate;
            this.DType = dType;
            this.SoonRemoved = false;
        }

        public DObject(DirectoryObject dObject, DateTime addDate, DateTime deleteDate)
        {
            this.Name = dObject.Name;
            this.DType = dObject.DType;
            this.AddDate = addDate;
            this.DeleteDate = deleteDate;
            this.SoonRemoved = false;
        }

        public DObject()
        { }

        public virtual void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == "DeleteDate")
                this.SoonRemoved = (this.DeleteDate.Subtract(DateTime.Now) <= WARNING_TIME);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class DirectoryObject
    {
        public string Name { get; set; }
        public DTypeObject DType { get; set; }

        public DirectoryObject(string name, DTypeObject dType)
        {
            this.Name = name;
            this.DType = dType;
        }
    }
}
