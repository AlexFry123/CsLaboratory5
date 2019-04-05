using CsLaboratory5.Managers;
using CsLaboratory5.Models;
using CsLaboratory5.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace CsLaboratory5.ViewModels
{
    internal class ProcessesViewModel : INotifyPropertyChanged, ILoaderOwner
    {
        public CustomProcess SelectedProcess { get; set; }

        private ObservableCollection<CustomProcess> _processes;
        private ObservableCollection<CustomModule> _modules;
        private ObservableCollection<CustomThread> _threads;

        private ICommand _threadsCommand;
        private ICommand _modulesCommand;
        private ICommand _killCommand;
        private ICommand _openFolderCommand;

        private Thread _refrProcList;
        private Thread _refrMetaData;

        private Visibility _loaderVisibility = Visibility.Visible;
        private bool _isControlEnabled = false;

        private CancellationToken _listRefreshToken = Cancellation.RefreshListToken;
        private CancellationToken _metaDataToken = Cancellation.RefreshMetaDataToken;

        public Visibility LoaderVisibility
        {
            get { return _loaderVisibility; }
            set
            {
                _loaderVisibility = value;
                OnPropertyChanged();
            }
        }
        public bool IsControlEnabled
        {
            get { return _isControlEnabled; }
            set
            {
                _isControlEnabled = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CustomProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CustomModule> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CustomThread> Threads
        {
            get { return _threads; }
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public ICommand ThreadsCommand
        {
            get
            {
                return _threadsCommand ?? (_threadsCommand =
                           new RelayCommand<object>(ThreadsShow));
            }
        }

        public ICommand ModulesCommand
        {
            get
            {
                return _modulesCommand ?? (_modulesCommand =
                           new RelayCommand<object>(ModulesShow));
            }
        }

        public ICommand KillCommand
        {
            get
            {
                return _killCommand ?? (_killCommand =
                           new RelayCommand<object>(KillProcess));
            }
        }

        public ICommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand =
                           new RelayCommand<object>(OpenFolder));
            }
        }

        private void OpenFolder(object obj)
        {
            if (SelectedProcess.FilePath.Equals("Access denied"))
            {
                MessageBox.Show("Access denied");
                return;
            }
            string folderPath = SelectedProcess.FilePath;
            Process.Start("explorer.exe", folderPath.Remove(folderPath.LastIndexOf('\\')));
        }

        private void KillProcess(object obj)
        {
            if (SelectedProcess.FilePath.Equals("Access denied"))
            {
                MessageBox.Show("Access denied");
                return;
            }
            Process.GetProcessById(SelectedProcess.Id).Kill();
            Processes.Remove(SelectedProcess);
        }

        private void ThreadsShow(object obj)
        {
            var tmpThreads = new List<CustomThread>();
            foreach(ProcessThread prThread in Process.GetProcessById(SelectedProcess.Id).Threads)
            {
                DateTime tmpTime;
                try
                {
                    tmpTime = prThread.StartTime;
                }catch(Exception)
                {
                    tmpTime = DateTime.Now;
                }
                tmpThreads.Add(new CustomThread(prThread.Id, prThread.ThreadState.ToString(), tmpTime));
            }
            Threads = new ObservableCollection<CustomThread>(tmpThreads);
        }

        private void ModulesShow(object obj)
        {
            if (SelectedProcess.FilePath.Equals("Access denied"))
            {
                MessageBox.Show("Access denied");
                return;
            }
            var tmpModules = new List<CustomModule>();
            foreach (ProcessModule mod in Process.GetProcessById(SelectedProcess.Id).Modules)
            {
                tmpModules.Add(new CustomModule(mod.ModuleName, mod.FileName));
            }
            Modules = new ObservableCollection<CustomModule>(tmpModules);
        }

        internal ProcessesViewModel() 
        {
            _processes = new ObservableCollection<CustomProcess>();
            LoaderManager.Instance.Initialize(this);
            _refrProcList = new Thread(Refresh);
            _refrMetaData = new Thread(RefreshMetaData);
            _refrProcList.Start();
            _refrMetaData.Start();
        }

        private void RefreshList(List<Process> next)
        {
            var removeList = new List<CustomProcess>();
            foreach (var csProc in Processes)
            {
                if (next.All(proc => proc.Id != csProc.Id))
                {
                    removeList.Add(csProc);
                }
            }
            var addList = new List<CustomProcess>();
            foreach (var process in next)
            {
                if (!Processes.Any(proc => proc.Id == process.Id))
                {
                    addList.Add(new CustomProcess(process));
                }
            }
            foreach (var csProc in addList)
            {
                App.Current.Dispatcher.Invoke(() => Processes.Add(csProc));
            }
            foreach (var csProc in removeList)
            {
                App.Current.Dispatcher.Invoke(() => Processes.Remove(csProc));
            }
        }

        private void Refresh()
        {
            while (!_listRefreshToken.IsCancellationRequested)
            {
                RefreshList(Process.GetProcesses().ToList());
                LoaderManager.Instance.HideLoader();
                Thread.Sleep(4000);
            }
        }

        private void RefreshMetaData()
        {
            while(!_metaDataToken.IsCancellationRequested)
            {
                try
                {
                    foreach (CustomProcess process in Processes)
                    {
                        process.RefreshMetaData();
                    }
                }
                catch (Exception) { }
                Thread.Sleep(1500);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
