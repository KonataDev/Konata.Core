﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Konata.Core.Attributes;

// ReSharper disable FunctionNeverReturns
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Konata.Core.Components.Model
{
    [Component("ScheduleComponent", "Konata Schedule Task Component")]
    internal class ScheduleComponent : InternalComponent
    {
        private class Schedule
        {
            public const int Infinity = Int32.MaxValue;

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
        private readonly ConcurrentDictionary<string, Schedule> _taskDict;
        private readonly ManualResetEvent _taskNotify;

        public ScheduleComponent()
        {
            _taskDict = new();
            _taskNotify = new(false);
            _taskThread = new(SchedulerThread);

            // Start task thread
            _taskThread.Start();
        }

        /// <summary>
        /// Scheduler thread
        /// </summary>
        private void SchedulerThread()
        {
            bool needUpdate;
            int minInterval;
            DateTime startTime;
            List<Schedule> todoList;
            List<Schedule> taskTable;
            ScheduleSorter taskSorter;
            {
                todoList = new();
                taskTable = new();
                taskSorter = new();
                needUpdate = false;

                // Scheduler steps
                while (true)
                {
                    Update();
                    WaitOne();
                    DoTheTask();
                }
            }

            // Select the task
            void Update()
            {
                if (needUpdate)
                {
                    // Try get the new tasks from outside
                    foreach (var (key, value) in _taskDict)
                    {
                        if (taskTable.Find(i =>
                            i.GetHashCode() == value.GetHashCode()) == null)
                        {
                            // Set the value
                            value.RemainTimes = value.Times;
                            value.RemainInterval = value.Interval;

                            // Join the queue
                            taskTable.Add(value);
                            LogI(TAG, $"Join the task => {key}");
                        }
                    }

                    // Mark as no need
                    needUpdate = false;
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
                {
                    // Set sleep time
                    var sleepTime = minInterval == 0
                        ? Int32.MaxValue
                        : minInterval;

                    // Reset event and wait
                    _taskNotify.Reset();
                    _taskNotify.WaitOne(sleepTime);
                }
                var passedTime = (int) ((DateTime.Now - startTime).TotalSeconds * 1000);

                // If the thread woke up ahead of the minInterval
                // So that means a new task has joined
                needUpdate = passedTime < minInterval || minInterval == 0;

                // Calculate the remain
                todoList.Clear();
                for (int i = taskTable.Count - 1; i >= 0; --i)
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
                        LogI(TAG, $"Destroy the task => '{taskTable[i].Name}'");

                        _taskDict.TryRemove(taskTable[i].Name, out _);
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
        /// Cancel the task
        /// </summary>
        /// <param name="name"><b>[In]</b> Task identity name</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Cancel(string name)
        {
            // Remove the task
            if (_taskDict.TryGetValue(name, out var task))
            {
                // Cancel the task
                task.RemainTimes = -1;
                task.RemainInterval = Schedule.Infinity;

                // Wakeup the scheduler thread
                _taskNotify.Set();
            }
        }

        /// <summary>
        /// Executes the task with specific interval
        /// </summary>
        /// <param name="name"><b>[In]</b> Task identity name</param>
        /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
        /// <param name="times"><b>[In]</b> Execute times</param>
        /// <param name="action"><b>[In]</b> Callback action</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        private void Interval(string name, int interval, int times, Action action)
        {
            var task = new Schedule
                (name, action, interval, times);

            // Check duplicate
            if (_taskDict.ContainsKey(name))
            {
                LogW(TAG, $"Conflict schedule found. '{name}', override.");
                _taskDict[name] = task;
            }

            // Add new task
            _taskDict.TryAdd(name, task);

            // Wakeup the scheduler thread
            _taskNotify.Set();
        }

        /// <summary>
        /// Executes the task with specific interval
        /// </summary>
        /// <param name="name"><b>[In]</b> Task identity name</param>
        /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
        /// <param name="action"><b>[In]</b> Callback action</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Interval(string name, int interval, Action action)
            => Interval(name, interval, Schedule.Infinity, action);

        /// <summary>
        /// Executes once the task
        /// </summary>
        /// <param name="name"><b>[In]</b> Task identity name</param>
        /// <param name="delay"><b>[In]</b> Delay time in milliseconds</param>
        /// <param name="action"><b>[In]</b> Callback action</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void RunOnce(string name, int delay, Action action)
            => Interval(name, delay, 1, action);

        /// <summary>
        /// Executes once the task
        /// </summary>
        /// <param name="name"><b>[In]</b> Task identity name</param>
        /// <param name="date"><b>[In]</b> Execute date</param>
        /// <param name="action"><b>[In]</b> Callback action</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void RunOnce(string name, DateTime date, Action action)
            => RunOnce(name, (int) ((date - DateTime.Now).TotalSeconds * 1000), action);

        /// <summary>
        /// Execute a task immediately
        /// </summary>
        /// <param name="name"></param>
        public void Trigger(string name)
        {
            // Check the task
            if (!_taskDict.TryGetValue(name, out var task))
            {
                LogW(TAG, $"Schedule '{name}' not exist.");
                return;
            }

            // Set interval to 0
            task.RemainInterval = 0;

            // Wakeup the scheduler thread
            _taskNotify.Set();
        }
    }
}
