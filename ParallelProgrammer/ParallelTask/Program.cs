using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelTask
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelTask();
            Console.WriteLine();

            ParallelInvoke();
            Console.WriteLine();

            ParallelTaskCancel();
            Console.WriteLine();

            WaitFirstTask();
            Console.WriteLine();

            SpeculativeInvoke(DoSomething1, DoSomething2, DoSomething3);
            Console.WriteLine();

            Console.ReadLine();
        }

        /// <summary>
        ///  Task执行并行任务
        /// </summary>
        static void ParallelTask()
        {
            Task doSome1 = Task.Factory.StartNew(() => DoSomething1());
            Task doSome2 = Task.Factory.StartNew(() => DoSomething2());

            Task.WaitAll(doSome1, doSome2);
            Console.WriteLine("ParallelTask finish.");
        }

        /// <summary>
        ///  Invoke执行并行任务
        /// </summary>
        static void ParallelInvoke()
        {
            Parallel.Invoke(
                () => DoSomething1(),
                () => DoSomething2());

            Console.WriteLine("ParallelInvoke finish.");
        }

        /// <summary>
        ///  并行任务取消并捕捉异常
        /// </summary>
        static void ParallelTaskCancel()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task myTask = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        Console.WriteLine("LoopIndex: " + i);
                    }
                }, token);

            cts.Cancel();

            try
            {
                myTask.Wait();
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("ParallelTaskCancel-IsCanceled: " + myTask.IsCanceled);

                ae.Flatten().Handle(e =>
                {
                    if (e is Exception)
                        return true;
                    else
                        return false;
                });
            }
        }

        /// <summary>
        ///  逐个输出任务完成的通知
        /// </summary>
        static void WaitFirstTask()
        {
            var taskIndex = -1;

            Task[] tasks = new Task[]{
                Task.Factory.StartNew(DoSomething1),
                Task.Factory.StartNew(DoSomething2),
                Task.Factory.StartNew(DoSomething3)
            };

            Task[] allTasks = tasks;
            // 逐个输出任务完成的通知
            while (tasks.Length > 0)
            {
                taskIndex = Task.WaitAny(tasks);
                Console.WriteLine("Finished task {0}.", taskIndex + 1);
                tasks = tasks.Where((t) => t != tasks[taskIndex]).ToArray();
            }

            try
            {
                Task.WaitAll(allTasks);
            }
            catch (AggregateException ae)
            {

            }
        }

        /// <summary>
        ///  并行运行多个方法,等待最快完成后停止
        /// </summary>
        /// <param name="actions"></param>
        static void SpeculativeInvoke(params Action<CancellationToken>[] actions)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var tasks =
                (from a in actions
                 select Task.Factory.StartNew(() => a(token), token)).ToArray();

            // 等待最快完成的任务
            Task.WaitAny(tasks);

            // 取消所有其他任务
            cts.Cancel();
            Console.WriteLine("取消任务");

            // 等待取消请求并观测异常
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ae)
            {
                // 筛选掉由于取消请求引起的异常
                ae.Flatten().Handle(e => e is OperationCanceledException);
            }
            finally
            {
                if (cts != null) cts.Dispose();
            }
        }


        static void DoSomething1(CancellationToken cancel)
        {
            for (int i = 0; i < 10; i++)
            {
                cancel.ThrowIfCancellationRequested();
                Console.WriteLine("Do something1.");
            }
        }

        static void DoSomething2(CancellationToken cancel)
        {
            for (int i = 0; i < 100000000; i++)
            {
                cancel.ThrowIfCancellationRequested();

                Console.WriteLine("Do something2.");
            }
        }

        static void DoSomething3(CancellationToken cancel)
        {
            for (int i = 0; i < 100000000; i++)
            {
                cancel.ThrowIfCancellationRequested();

                Console.WriteLine("Do something3.");
            }
        }

        static void DoSomething1() { Console.WriteLine("Do something1."); }

        static void DoSomething2() { Console.WriteLine("Do something2."); }

        static void DoSomething3() { Console.WriteLine("Do something3."); }
    }
}
