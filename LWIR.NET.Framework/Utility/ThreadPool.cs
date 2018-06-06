using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using LWIR.NET.Framework.Enum;
using System.ComponentModel.Composition;
using LWIR.NET.Framework.Interface;

namespace LWIR.NET.Framework.Utility
{
    public class ThreadPool
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private static readonly object lockobj = new object();
        private static readonly Lazy<ThreadPool> lazy = new Lazy<ThreadPool>(() => new ThreadPool());
        private Queue<Action> actionQueue = new Queue<Action>();
        private Queue<Action> actionStack = new Queue<Action>();
        private bool isRunning = false;
        private ManualResetEvent taskSignal = new ManualResetEvent(false);

        private bool specifyCpuEnabled = false;
        /// <summary>
        /// Specify cpu automatically, and make an even distribution, modification bring it into effect before threadpool running
        /// and failed during running
        /// </summary>
        public bool SpecifyCpuEnabled
        {
            get { return specifyCpuEnabled; }
            set
            {
                if (!isRunning)
                {
                    specifyCpuEnabled = value;
                }
            }
        }

        /// <summary>
        /// The count of last messages
        /// </summary>
        public Action<long> LastMsgCountEvent;

        private ThreadPool()
        {
        }

        public static ThreadPool Instance
        {
            get { return lazy.Value; }
        }

        /// <summary>
        /// Start the threadpool with max threads count
        /// </summary>
        /// <param name="maxCapacity">Default max capacity of thread is ((cpu count-1) * 2) in threadpool. 
        /// default maxCapacity if current is large than maxCapacity.</param>
        public void Start(uint maxCapacity)
        {
            if (maxCapacity == 0)
                throw new Exception("ThreadPool start failed because of capacity is 0.");

            int cpuCount = Environment.ProcessorCount;
            int sysMaxCapacity = cpuCount < 2 ? 1 : (cpuCount - 1) << 1; // couple thread in single core: (cpuCount-1)*2
            maxCapacity = maxCapacity > sysMaxCapacity ? (uint)sysMaxCapacity : maxCapacity;

            if (isRunning)
                return;

            for (int num = 0; num < maxCapacity; num++)
            {
                var param = new ParameterizedThreadStart(TaskInThread);

                // run only one thread in single core
                // multi-cores: first core is reserved for system
                new Thread(param).Start(cpuCount == 1 ? 0 : (num >> 1) + 1);
            }

            isRunning = true;
            taskSignal.Set();
        }

        /// <summary>
        /// Stop the threadpool
        /// </summary>
        public void Stop()
        {
            lock (lockobj)
            {
                actionQueue.Clear();
            }

            isRunning = false;
            taskSignal.Set();
        }

        /// Execute task in thread on special cpu
        /// </summary>
        /// <param name="cpuCoreIndex">from 0 if only single core, otherwise from 1</param>
        private void TaskInThread(object cpuCoreIndex)
        {
            int curCpuIndex = (int)cpuCoreIndex;

            #region Specify cpu to run current thread
            if (SpecifyCpuEnabled)
            {
                uint nativeId = GetCurrentThreadId();
                ProcessThread pt = Process.GetCurrentProcess().Threads.Cast<ProcessThread>().FirstOrDefault(x => x.Id == nativeId);

                if (pt != null)
                {
                    pt.ResetIdealProcessor();

                    //The IdealProcessor value is zero-based. In other words, to set the thread affinity for the first processor,
                    //set the property to zero.The system schedules threads on their preferred processors whenever possible.
                    //cpu core index from 0 set to IdealProcessor.
                    pt.IdealProcessor = curCpuIndex;

                    //Each processor is represented as a bit. Bit 0 is processor one, bit 1 is processor two, and so forth. 
                    //If you set a bit to the value 1, the corresponding processor is selected for thread assignment. When 
                    //you set the ProcessorAffinity value to zero, the operating system's scheduling algorithms set the 
                    //thread's affinity. When the ProcessorAffinity value is set to any nonzero value, the value is interpreted
                    //as a bitmask that specifies those processors eligible for selection.
                    pt.ProcessorAffinity = new IntPtr(1 << curCpuIndex);
                }

                LogService.Log.WriteLog(LogType.Info, this.GetType(), string.Format("Thread in cpu[{0}] start.", curCpuIndex));
                Thread.BeginThreadAffinity();
            }
            #endregion

            #region thread main part
            while (taskSignal.WaitOne())
            {
                if (!isRunning)
                {
                    break;
                }

                Action action = null;

                #region Get the task
                lock (lockobj)
                {
                    if (actionStack.Count > 0)
                    {
                        action = actionStack.Dequeue();
                    }
                    else
                    {
                        if (actionQueue.Count > 0)
                        {
                            action = actionQueue.Dequeue();
                        }
                        else
                        {
                            // Notify the count of msg queue
                            if (LastMsgCountEvent != null)
                            {
                                LastMsgCountEvent(0);
                            }

                            taskSignal.Reset();
                            continue;
                        }

                        // Notify the count of msg queue
                        if (LastMsgCountEvent != null)
                        {
                            LastMsgCountEvent(actionStack.Count + actionQueue.Count);
                        }
                    }
                }
                #endregion

                action();

                //Thread.Sleep(500);
            }
            #endregion

            //Make a end flag if specify cpu
            if (SpecifyCpuEnabled)
            {
                Thread.EndThreadAffinity();
                LogService.Log.WriteLog(LogType.Info, this.GetType(), string.Format("Thread in cpu[{0}] terminal.", curCpuIndex));
            }

            // Set false if thread stop in order to restart, protect crash to restart successfully.
            isRunning = false;
        }

        /// <summary>
        /// Add task to the threadpool
        /// </summary>
        /// <param name="task">a task added</param>
        /// <param name="isHighPriority">High priority flag</param>
        public void Add(Action task, bool isHighPriority = false)
        {
            lock (lockobj)
            {
                if (isHighPriority)
                {
                    actionStack.Enqueue(task);
                }
                else
                {
                    actionQueue.Enqueue(task);
                }

                // Notify the count of msg queue
                if (LastMsgCountEvent != null)
                {
                    LastMsgCountEvent(actionStack.Count + actionQueue.Count);
                }

                taskSignal.Set();
            }
        }

        /// <summary>
        /// Clear all the msg
        /// </summary>
        public void Clear()
        {
            lock (lockobj)
            {
                actionStack.Clear();
                actionQueue.Clear();

                // Notify the count of msg queue
                if (LastMsgCountEvent != null)
                {
                    LastMsgCountEvent(actionStack.Count + actionQueue.Count);
                }

                taskSignal.Set();
            }
        }
    }
}
