using Assets.Framework.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.Util
{
    class TimeTask
    {
        public int tid;
        public float destTime;
        public Action callback;
    }
    public class GameTimer:Singleton<GameTimer>
    {
        private int tid;
        private static readonly string lockTid = "lockTid";
        private static readonly string lockTime = "lockTime";
        private List<int> tidLst = new List<int>();
        private List<int> recTidLst = new List<int>();
        List<TimeTask> taskList = new List<TimeTask>();
        List<TimeTask> tempTaskList = new List<TimeTask>();
        Stack<TimeTask> timeTaskStack = new Stack<TimeTask>();

        private List<int> tmpDelTimeLst = new List<int>();
        bool isClear = false;

        public override void Init()
        {
            //base.Init();
            

        }
        public void Update()
        {
            if (isClear) return;
            CheckTimeTask();
            DelTimeTask();
            if (recTidLst.Count > 0)
            {
                lock (lockTid)
                {
                    RecycleTid();
                }
            }

        }
        private void CheckTimeTask()
        {
            lock (lockTime)
            {
                for (int tempIndex = 0; tempIndex < tempTaskList.Count; tempIndex++)
                {
                    taskList.Add(tempTaskList[tempIndex]);
                }
            }


            for (int taskIndex = 0; taskIndex < taskList.Count - tempTaskList.Count; taskIndex++)
            {
                TimeTask task = taskList[taskIndex];
                if (Time.time < task.destTime)
                {
                    continue;
                }
                else
                {
                    if (task.callback != null)
                    {
                        task.callback();
                        Debug.Log("1");
                        taskList.RemoveAt(taskIndex);
                        taskIndex--;
                        recTidLst.Add(task.tid);
                        timeTaskStack.Push(task);
                    }
                }
            }

            tempTaskList.Clear();
        }

        public int AddTimeTask(float delay, Action callback)
        {
            if (callback == null) return 0;
            TimeTask timeTask;
            if (timeTaskStack.Count > 0)
            {
                timeTask = timeTaskStack.Pop();
            }
            else
            {
                timeTask = new TimeTask();
            }

            timeTask.destTime = Time.time + delay;
            timeTask.callback = callback;
            tempTaskList.Add(timeTask);
            timeTask.tid = GetTid();
            return timeTask.tid;
        }

        private void DelTimeTask()
        {
            if (tmpDelTimeLst.Count > 0)
            {
                lock (lockTime)
                {
                    for (int i = 0; i < tmpDelTimeLst.Count; i++)
                    {
                        bool isDel = false;
                        int delTid = tmpDelTimeLst[i];
                        for (int j = 0; j < taskList.Count; j++)
                        {
                            TimeTask task = taskList[j];
                            if (task.tid == delTid)
                            {
                                isDel = true;
                                taskList.RemoveAt(j);
                                recTidLst.Add(delTid);
                                break;
                            }
                        }

                        if (isDel)
                            continue;

                        for (int j = 0; j < tempTaskList.Count; j++)
                        {
                            TimeTask task = tempTaskList[j];
                            if (task.tid == delTid)
                            {
                                tempTaskList.RemoveAt(j);
                                recTidLst.Add(delTid);
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void DeleteTimeTask(int tid)
        {
            lock (lockTime)
            {
                tmpDelTimeLst.Add(tid);
            }
        }

        public void ClearAllTask()
        {
            isClear = true;
            for (int i = 0; i < tempTaskList.Count; i++)
            {
                timeTaskStack.Push(tempTaskList[i]);
            }
            tempTaskList.Clear();
            for (int i = 0; i < taskList.Count; i++)
            {
                timeTaskStack.Push(taskList[i]);
            }
            taskList.Clear();

            tidLst.Clear();
            tmpDelTimeLst.Clear();

            isClear = false;
        }

        private int GetTid()
        {
            lock (lockTid)
            {
                tid += 1;

                //安全代码，以防万一
                while (true)
                {
                    if (tid == int.MaxValue)
                    {
                        tid = 0;
                    }

                    bool used = false;
                    for (int i = 0; i < tidLst.Count; i++)
                    {
                        if (tid == tidLst[i])
                        {
                            used = true;
                            break;
                        }
                    }
                    if (!used)
                    {
                        tidLst.Add(tid);
                        break;
                    }
                    else
                    {
                        tid += 1;
                    }
                }
            }

            return tid;
        }
        private void RecycleTid()
        {
            for (int i = 0; i < recTidLst.Count; i++)
            {
                int tid = recTidLst[i];

                for (int j = 0; j < tidLst.Count; j++)
                {
                    if (tidLst[j] == tid)
                    {
                        tidLst.RemoveAt(j);
                        break;
                    }
                }
            }
            recTidLst.Clear();
        }
    }
}
