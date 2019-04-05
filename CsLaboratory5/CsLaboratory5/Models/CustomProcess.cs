using CsLaboratory5.Tools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CsLaboratory5.Models
{
    public class CustomProcess : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Id { get; set; }
        private bool _isActive;
        private double _usageCPU;
        private double _usageMemPercentage;
        private double _memoryUsage;
        private int _threadsQuantity;
        public string UserName { get; set; }
        public string FilePath { get; set; }
        public DateTime StartDate { get; set; }

        private readonly PerformanceCounter _cpuCount;
        private readonly PerformanceCounter _ramCount;
        private readonly PerformanceCounter _ramInMBCount;
        private readonly Int64 _memoryCapacity = MemoryTranslator.GetTotalMemoryInMiB() * 10240;
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformationW(IntPtr hServer, int SessionId, int WTSInfoClass, out IntPtr ppBuffer, out IntPtr pBytesReturned);

        public double UsageCPU
        {
            get { return _usageCPU; }
            set
            {
                _usageCPU = value;
                OnPropertyChanged();
            }
        }

        public double UsageMemPercentage
        {
            get { return _usageMemPercentage; }
            set
            {
                _usageMemPercentage = value;
                OnPropertyChanged();
            }
        }

        public double MemoryUsage
        {
            get { return _memoryUsage; }
            set
            {
                _memoryUsage = value;
                OnPropertyChanged();
            }
        }

        public int ThreadsQuantity
        {
            get { return _threadsQuantity; }
            set
            {
                _threadsQuantity = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public CustomProcess(Process process)
        {
            Name = process.ProcessName;
            Id = process.Id;
            IsActive = process.Responding;
            try
            {
                ThreadsQuantity = process.Threads.Count;
            }
            catch (Exception) { }
            UserName = GetUserName(process);
            try
            {
                FilePath = process.MainModule.FileName;
            }
            catch (Exception)
            {
                FilePath = "Access denied";
            }
            try
            {
                StartDate = process.StartTime;
            }
            catch (Exception) { }
            _cpuCount = new PerformanceCounter("Process", "% Processor Time", Name, true);
            _ramCount = new PerformanceCounter("Process", "Working Set", Name, true);
            _ramInMBCount = new PerformanceCounter("Process", "Working Set", Name, true);
        }

        private string GetUserName(Process process)
        {
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
            int WTS_UserName = 5;
            string resName = "SYSTEM";
            if (process.ProcessName != "Idle")
            {
                if (WTSQuerySessionInformationW(WTS_CURRENT_SERVER_HANDLE, process.SessionId, WTS_UserName, out IntPtr AnswerBytes, out IntPtr AnswerCount))
                {
                    resName = Marshal.PtrToStringUni(AnswerBytes);
                }
            }
            return resName;
        }

        internal void RefreshMetaData()
        {
            try
            {
                UsageCPU = _cpuCount.NextValue() / Environment.ProcessorCount;
            }
            catch (InvalidOperationException) { }
            try
            {
                UsageMemPercentage = _ramCount.NextValue() / _memoryCapacity;
            }
            catch (InvalidOperationException) { }
            try
            {
                MemoryUsage = Convert.ToInt32(_ramInMBCount.NextValue()) / (int)(1024*1024);
            }
            catch (InvalidOperationException) { }
            try
            {
                ThreadsQuantity = Process.GetProcessById(Id).Threads.Count;
            }
            catch (Exception) { }
            try { 
            IsActive = Process.GetProcessById(Id).Responding;
            }
            catch (Exception) { }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
