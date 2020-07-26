﻿
using System;
using System.Collections.Concurrent;
using System.Threading;
using prompto.value;

namespace prompto.runtime
{
    public abstract class Scheduler
    {

        static Int64 JOB_COUNTER = 0L;
        static ConcurrentDictionary<Int64, Timer> JOB_TIMERS = new ConcurrentDictionary<Int64, Timer>();

        public static Int64 schedule(ClosureValue method, DateTimeOffset executeAt, PeriodValue repeatEvery, String jobName)
        {
            long delayInMillis = (executeAt.Ticks - DateTime.Now.Ticks) / 10000; // 1 tick = 100 nanosecond
            if (delayInMillis < 0)
                delayInMillis = 0;
            long periodInMillis = repeatEvery == null ? -1L : repeatEvery.TotalMilliseconds();
            Int64 jobId = Interlocked.Increment(ref JOB_COUNTER);
            TimerCallback callback = state => HandleCallback(method, repeatEvery == null ? jobId : (Int64?)null);
            JOB_TIMERS[jobId] = new Timer(callback, null, delayInMillis, periodInMillis);
            return jobId;
        }

        public static void HandleCallback(ClosureValue method, Int64? jobIdToCancel)
        {
            try
            {
                method.interpret(ApplicationContext.Get());
            }
            finally
            {
                // gracefully dispose 1-time job timers (repeated jobs must call cancel)
                if (jobIdToCancel != null)
                {
                    Timer timer;
                    JOB_TIMERS.TryRemove((Int64)jobIdToCancel, out timer);
                }
            }
        }

        public static void cancel(Int64 jobId)
        {
            Timer timer;
            if (JOB_TIMERS.TryRemove((Int64)jobId, out timer))
            {
                timer.Change(-1, -1);

            }
            else
                Console.WriteLine("Timer not found: " + jobId); // TODO raise Prompto error
        }
    }
}
