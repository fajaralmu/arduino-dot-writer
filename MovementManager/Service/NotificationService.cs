using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using MovementManager.Model;

namespace MovementManager.Service
{

    public interface INotificationService : IDisposable
    {
        void Start();
        void NotifyProgress(int completed, int total, MovementProperty movementProperty);
        void NotifyComplete();
    }
    public class NotificationService : INotificationService
    {
        const int INDEX_PROGRESS = 0, INDEX_COMPLETE = 1;
        private readonly MemoryMappedFile _memoryMappedFile;
        private readonly string _memoryMapName;
        private readonly long _capacity;
        private Process _worker;

        public string WorkerApplication { get; set; } = ".\\WorkerService\\MovementManagerWorker.exe";

        public NotificationService(string memoryMapName, long capacity)
        {
            _memoryMapName      = memoryMapName;
            _capacity           = capacity;
            _memoryMappedFile   = GetMemoryMappedFile();

        }

        private MemoryMappedFile GetMemoryMappedFile()
        {
            return MemoryMappedFile.CreateOrOpen(_memoryMapName, _capacity);
        }

        public void Start()
        {
            if (_worker != null)
            {
                return;
            }
            _worker = CreateWorker( Environment.CurrentDirectory );
            _worker.Start();
            Debug.WriteLine("Started worker: " + _worker.ProcessName);
        }

        public void NotifyProgress(int completed, int total, MovementProperty movementProperty)
        {
            try
            {
                using (var accessor = _memoryMappedFile.CreateViewAccessor(0, _capacity))
                {
                    accessor.Write(INDEX_COMPLETE, 0);
                    accessor.WriteArray<int>(INDEX_PROGRESS, new int[] { completed, total }, 0, 2);
                    // accessor.Write(2, total);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to notify progress: " + e.Message);
            }

        }

        public void NotifyComplete()
        {
            try
            {
                using (var accessor = _memoryMappedFile.CreateViewAccessor(0, _capacity))
                {
                    accessor.Write(INDEX_COMPLETE, 1);
                    // accessor.Write(2, total);
                }
                Debug.WriteLine("Notify complete");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to notify progress: " + e.Message);
            }
        }

        // TODO fix method
        private void TerminateWorker()
        {
            _worker.Kill();
            _worker.WaitForExit();
            _worker.Dispose();
        }

        private Process CreateWorker(string workingDirectory, string prefix = "/k")
        {
            string arguments    = $"{WorkerApplication} mapName=" + _memoryMapName;
            Process proc        = new Process
            {
                StartInfo       = new ProcessStartInfo
                {
                    FileName            = "cmd.exe",
                    Arguments           = prefix + arguments,
                    WindowStyle         = ProcessWindowStyle.Normal,
                    WorkingDirectory    = @workingDirectory,
                    UseShellExecute     = true
                }
            };

            return proc;
        }

        public void Dispose()
        {
            _memoryMappedFile?.Dispose();
            _worker?.Dispose();
        }
    }
}