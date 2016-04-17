using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.scripts.util
{
    public class ThreadManager
    {
        public static ThreadManager Instance = new ThreadManager();

        private const int BackgroundThreadCount = 6;

        private List<Thread> WorkerThreads { get; set; }
        private AutoResetEvent WorkReadyEvent { get; set; }

        private Queue<Action> BackgroundWork { get; set; }

        private ThreadManagerHost Host { get; set; }

        private bool RunThreads { get; set; }

        private ThreadManager()
        {
            WorkReadyEvent = new AutoResetEvent(false);
            BackgroundWork = new Queue<Action>();
            WorkerThreads = new List<Thread>();
            RunThreads = true;

            Host = new GameObject("ThreadManagerHost").AddComponent<ThreadManagerHost>();

            for (int i=0; i<BackgroundThreadCount; ++i)
            {
                var workerThread = new Thread(ConsumeWork);
                WorkerThreads.Add(workerThread);
                workerThread.Start();
            }
        }

        private void ConsumeWork()
        {
            Action currentJob = null;
            while(RunThreads)
            {
                try
                {

                    lock (BackgroundWork)
                    {
                        if (BackgroundWork.Count > 0)
                        {
                            currentJob = BackgroundWork.Dequeue();
                        }
                    }

                    if (currentJob == null)
                    {
                        //nothing to process, let's wait a bit
                        WorkReadyEvent.WaitOne();
                    }
                    else
                    {
                        try
                        {
                            currentJob();
                        }
                        catch (Exception e)
                        {
                            ExecuteInMainThread(() => { Debug.LogError("Error in background thread: " + e); });
                        }
                        currentJob = null;
                    }
                }
                catch (Exception e)
                {
                    ExecuteInMainThread(() => { Debug.LogError("Error running background job thread: " + e); });
                }
            }
        }

        public void AbortThreads()
        {
            RunThreads = false;
        }

        public void ExecuteInBackground(Action job)
        {

#if THREADING_USES_THREADPOOL
            ThreadPool.QueueUserWorkItem((context) => {
                try
                {
                    job();
                } catch (Exception e)
                {
                    ExecuteInMainThread(() => { Debug.LogError("Error in background thread: " + e); });
                }
            });
#else
            lock(BackgroundWork)
            {
                BackgroundWork.Enqueue(job);
            }
            WorkReadyEvent.Set();
#endif
        }

        public int GetBackgroundJobsCount()
        {
            return BackgroundWork.Count;
        }

        public void ExecuteInMainThread(Action job)
        {
            if (Host.IsMainThread())
            {
                job();
            } else
            {
                Host.QueueJob(job);
            }
        }

        private class ThreadManagerHost : MonoBehaviour
        {
            private Queue<Action> JobQueue { get; set; }

            private Thread MainThread { get; set; }

            private float MaxJobRunLength = 1.0f / 60f;

            void Start()
            {
                MainThread = Thread.CurrentThread;

                JobQueue = new Queue<Action>();

                StartCoroutine(Co_ExecuteMainThreadLoop());
            }

            private IEnumerator Co_ExecuteMainThreadLoop()
            {
                double elapsedTime = 0;
                while (true)
                {
                    var before = DateTime.Now;

                    Action job = null;
                    lock(JobQueue)
                    {
                        if (JobQueue.Count > 0)
                        {
                            job = JobQueue.Dequeue();
                        }
                    }
                    if (job != null)
                    {
                        try
                        {
                            job();
                        } catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                        var after = DateTime.Now;
                        elapsedTime += (after - before).TotalSeconds;
                        if (elapsedTime >= MaxJobRunLength)
                        {
                            elapsedTime = 0;
                            yield return null;
                        }
                    }
                    else
                    {
                        elapsedTime = 0;
                        yield return null;
                    }
                }
            }

            public bool IsMainThread()
            {
                return MainThread == Thread.CurrentThread;
            }

            public void QueueJob(Action job)
            {
                lock(JobQueue)
                {
                    JobQueue.Enqueue(job);
                }
            }
            
            void OnGUI()
            {
                GUI.Label(new Rect(0, 0, 100, 100), string.Format("{0}, {1}", ThreadManager.Instance.GetBackgroundJobsCount(), JobQueue.Count()));
            }

            void OnDisable()
            {
                ThreadManager.Instance.AbortThreads();
            }
            
        }

    }

}
