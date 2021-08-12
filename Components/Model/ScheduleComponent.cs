using System;
using System.Threading;
using System.Collections.Generic;
using Konata.Core.Attributes;

// ReSharper disable FunctionNeverReturns
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Konata.Core.Components.Model
{
    [Component("ScheduleComponent", "Konata Schedule Task Component")]
    internal class ScheduleComponent : InternalComponent
    {
        private class Schedule
        {
            public const int Infinity = -1;

            /// <summary>
            /// Task name
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Task action
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// Task interval
            /// </summary>
            public int Interval { get; }

            /// <summary>
            /// Execute how many times
            /// Keep zero for infinite
            /// </summary>
            public int Times { get; }

            /// <summary>
            /// Task remain interval
            /// </summary>
            internal int RemainInterval { get; set; }

            /// <summary>
            /// Task remain times
            /// </summary>
            internal int RemainTimes { get; set; }

            public Schedule(string name, Action action, int interval, int times)
            {
                Name = name;
                Action = action;
                Interval = interval;
                Times = times;
            }

            public override int GetHashCode()
                => Name.GetHashCode();
        }

        private class ScheduleSorter : IComparer<Schedule>
        {
            public int Compare(Schedule x, Schedule y)
            {
                return x!.RemainInterval - y!.RemainInterval;
            }
        }

        // private List<Schedule> _taskList;
        private const string TAG = "ScheduleComponent";
        private readonly Thread _taskThread;
        private readonly Dictionary<string, Schedule> _taskDict;
        private readonly CancellationTokenSource _taskNotify;

        public ScheduleComponent()
        {
            _taskDict = new();
            _taskNotify = new();
            _taskThread = new(SchedulerThread);

            // Start task thread
            _taskThread.Start();
        }

        /// <summary>
        /// Scheduler thread
        /// </summary>
        private void SchedulerThread()
        {
            int minInterval;
            DateTime startTime;
            List<Schedule> todoList;
            List<Schedule> taskTable;
            ScheduleSorter taskSorter;
            {
                todoList = new();
                taskTable = new();
                taskSorter = new();

                // Scheduler steps
                while (true)
                {
                    Select();
                    WaitOne();
                    DoTheTask();
                }
            }

            // Select the task
            void Select()
            {
                // Try get the new tasks from outside
                foreach (var (key, value) in _taskDict)
                {
                    if (taskTable.Find(i =>
                        i.GetHashCode() == value.GetHashCode()) != null)
                    {
                        // Set the remain
                        value.RemainTimes = value.Times;
                        value.RemainInterval = value.Interval;

                        // Join the queue
                        taskTable.Add(value);
                        LogV(TAG, $"Join the task => {key}");
                    }
                }

                // Sort the task
                taskTable.Sort(taskSorter);

                // Pickup minimal interval to wait
                minInterval = 0;
                if (taskTable.Count > 0)
                {
                    minInterval = taskTable[0].RemainInterval;
                }
            }

            // Wait the task and
            // calculate the remaining
            void WaitOne()
            {
                startTime = DateTime.Now;

                try
                {
                    var sleepTime = minInterval == 0 ? 1000 : minInterval;
                    _taskNotify.Token.WaitHandle.WaitOne(sleepTime, false);
                }

                // Don't bother me :)
                catch (Exception e)
                {
                    LogW(TAG, "Anyone else?");
                    LogE(TAG, e);

                    Select();
                    WaitOne();
                }

                todoList.Clear();
                var passedTime = (int) (DateTime.Now - startTime).TotalSeconds;

                // Calculate the remain
                for (int i = taskTable.Count; i >= 0; --i)
                {
                    // Reduce the interval
                    taskTable[i].RemainInterval -= passedTime;

                    // Task timeout
                    if (taskTable[i].RemainInterval <= 0)
                    {
                        // Reduce the counter
                        if (taskTable[i].Times != Schedule.Infinity)
                        {
                            taskTable[i].RemainTimes -= 1;
                        }
                        else
                        {
                            // Reset the interval
                            taskTable[i].RemainInterval = taskTable[i].Interval;
                        }

                        // Mark the tasks
                        // we are going to do
                        todoList.Add(taskTable[i]);
                    }

                    // Cleanup died tasks
                    if (taskTable[i].RemainTimes <= 0)
                    {
                        taskTable.RemoveAt(i);
                    }
                }
            }

            // Do the task
            void DoTheTask()
            {
                foreach (var i in todoList)
                {
                    ThreadPool.QueueUserWorkItem(_ => i.Action.Invoke());
                }
            }
        }

        /// <summary>
        /// Wakeup the scheduler thread
        /// </summary>
        private void Knock()
            => _taskNotify.Cancel(false);

        /// <summary>
        /// Executes the task with specific interval
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="times"></param>
        /// <param name="action"></param>
        private void Interval(string name, int interval, int times, Action action)
        {
            var task = new Schedule
                (name, action, interval, times);

            // Check duplicate
            if (_taskDict.ContainsKey(name))
            {
                LogW(TAG, $"Conlict schedule found. '{name}', overrided.");
                _taskDict[name] = task;
            }

            // Add new task
            _taskDict.Add(name, task);

            // Wakeup thread to queue the tasks
            Knock();
        }

        /// <summary>
        /// Executes the task with specific interval
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        public void Interval(string name, int interval, Action action)
            => Interval(name, interval, Schedule.Infinity, action);

        /// <summary>
        /// Executes the task once
        /// </summary>
        /// <param name="name"></param>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void RunOnce(string name, int delay, Action action)
            => Interval(name, delay, 1, action);

        /// <summary>
        /// Executes the task once
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="action"></param>
        public void RunOnce(string name, DateTime date, Action action)
            => RunOnce(name, (int) ((date - DateTime.Now).TotalSeconds), action);
    }
}
