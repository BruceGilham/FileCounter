using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace FileCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            Data = new ObservableCollection<Results>();
            this.DataContext = this;
        }

        public string Dir { get; set; }
        public ObservableCollection<Results> Data { get; set; }

        public Boolean IsRunning { get; set; }

        private void BtnBrowseClick(object sender, RoutedEventArgs e)
        {
            var fld = new FolderBrowserDialog();
            if (fld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Dir = fld.SelectedPath;
            }
        }

        private void BtnRunClick(object sender, RoutedEventArgs e)
        {
            //Start at the Dir and then add up all the different file extenstions\
            Data.Clear();
            this.IsRunning = true;
            var task = new TaskFactory().StartNew(() =>
            {
                GetFileDetails(Dir);
            }).ContinueWith(task1 =>
            {
                this.IsRunning = false;
            });
        }

        private void GetFileDetails(string dir)
        {
            try
            {
                var dirInfo = new DirectoryInfo(dir);
                foreach (var file in dirInfo.EnumerateFiles())
                {
                    var ext = System.IO.Path.GetExtension(file.FullName);
                    var rec = Data.FirstOrDefault(f => f.Ext == ext);
                    if (rec == null)
                    {
                        rec = new Results { Ext = ext };
                        Application.Current.Dispatcher.Invoke(() => { Data.Add(rec); });
                    }
                    rec.Count++;

                }
                foreach (var directoryInfo in dirInfo.EnumerateDirectories())
                {
                    GetFileDetails(directoryInfo.FullName);
                }
            }
            catch (Exception)
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Results : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Ext { get; set; }
        public int Count { get; set; }
    }
}
