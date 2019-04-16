using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace AsyncProcStart
{
    public class TaskManager : ISynchronizeInvoke
    {
        public List<Process> Executing { get; private set; }
        public int Pcount => _pcount;
        private int _pcount = 0;
        public void DoTask()
        {
            for (var i = 0; i < 5; i++)
            {
                var p = new Process();
                p.StartInfo.FileName = "notepad.exe";
                p.SynchronizingObject = this;
                p.Exited += p_Exited;
                p.EnableRaisingEvents = true;
                p.Start();

                _pcount++;
            }
        }
        
        private void p_Exited(object sender, EventArgs e)
        {
            var p = (Process) sender;
            Console.WriteLine($"{p.Id} has exited.");

            _pcount--;
        }
        
        #region ISynchronizeInvoke implementation
        private readonly object _sync;

        public TaskManager() {
            _sync = new object();
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args) {
            var result = new TaskAsyncResult();


            ThreadPool.QueueUserWorkItem(delegate {
                result.AsyncWaitHandle = new ManualResetEvent(false);
                try {
                    result.AsyncState = Invoke(method, args);
                } catch (Exception exception) {
                    result.Exception = exception;
                }
                result.IsCompleted = true;
            });


            return result;
        }

        public object EndInvoke(IAsyncResult result) {
            if (!result.IsCompleted) {
                result.AsyncWaitHandle.WaitOne();
            }


            return result.AsyncState;
        }

        public object Invoke(Delegate method, object[] args) {
            lock (_sync) {
                return method.DynamicInvoke(args);
            }
        }

        public bool InvokeRequired => true;
        #endregion
    }
    class TaskAsyncResult : IAsyncResult {
        object _state;

        public bool IsCompleted { get; set; }

        public WaitHandle AsyncWaitHandle { get; internal set; }

        public object AsyncState {
            get {
                if (Exception != null) {
                    throw Exception;
                }
                return _state;
            }
            internal set {
                _state = value;
            }
        }

        public bool CompletedSynchronously => IsCompleted;

        internal Exception Exception;
    }
}