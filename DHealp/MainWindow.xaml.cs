using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading;

namespace DHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread workThread;
        const string DIR_PARAM_NAME = "Directory";
        TimeWorker timeWorker;

        public MainWindow()
        {
            InitializeComponent();
            string path = SettingsFile.Read(DIR_PARAM_NAME);
            if (path == null)
            {
                path = DirectoryWorker.GetDirectory();
                if (path == null)
                    this.Close();
                else
                    SettingsFile.Write(DIR_PARAM_NAME, path);
            }

            timeWorker = new TimeWorker(path);
            workThread = new Thread(Start);
            workThread.Start();
        }
        public void Start()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                 DList.ItemsSource = timeWorker.DObjects), null);
            if (!File.Exists(TimeWorker.OBJECTS_FILE))
            {
                timeWorker.AddObjects(DirectoryWorker.GetDObjects(timeWorker.Directory));
                timeWorker.SaveObjects();
            }
            else
            {
                timeWorker.AddObjects(timeWorker.LoadObjects());
            }
            
            timeWorker.Work();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            workThread.Abort();
        }
    }
}
