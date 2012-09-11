using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelLoop.Inftrastructure;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace ParallelLoop
{
    class Program
    {
        static void Main(string[] args)
        {
            #region list对象

            SomethingList list = new SomethingList()
            {
                somethings = new List<Something>{
                    new Something{
                        ID = "1",
                        Name = "some1"
                    },
                    new Something{
                        ID="2",
                        Name ="some2"
                    },
                    new Something{
                        ID="3",
                        Name="some3"
                    },
                    new Something{
                        ID="4",
                        Name="some4"
                    },
                    new Something{
                        ID="5",
                        Name="some5"
                    },
                    new Something{
                        ID="6",
                        Name="some6"
                    },
                    new Something{
                        ID="7",
                        Name="some7"
                    },
                    new Something{
                        ID="8",
                        Name="some8"
                    },

                }
            };

            #endregion

            // 并行发生无顺序
            GetSomethingParallel(list);
            GetSomethingPlinq(list);
            GetSomethingBreak(list);
            GetSomethingStop(list);

            // 不知该如何调用:外部循环取消
            // DoGetSometingLoop(SomethingList list, CancellationTokenSource cts)

            GetSomethingSmallSet(list);

            GetRandomNumber();


            Console.ReadLine();
        }

        /// <summary>
        ///  并行ForEach
        /// </summary>
        /// <param name="list"></param>
        static void GetSomethingParallel(SomethingList list)
        {
            Parallel.ForEach(list.somethings, something =>
            {
                something.DoSomething();
                Console.WriteLine(something.Name);
            });
            Console.WriteLine("GetSomethingParallel");
            Console.WriteLine();
        }

        /// <summary>
        ///  并行PLINQ.ForAll
        /// </summary>
        /// <param name="list"></param>
        static void GetSomethingPlinq(SomethingList list)
        {
            list.somethings.AsParallel().ForAll(something =>
            {
                something.DoSomething();
                Console.WriteLine(something.Name);
            });
            Console.WriteLine("GetSomethingPlinq");
            Console.WriteLine();
        }

        /// <summary>
        ///  并行For中断
        /// </summary>
        /// <param name="list"></param>
        static void GetSomethingBreak(SomethingList list)
        {
            var count = list.somethings.Count;

            var loopResult = Parallel.For(0, count, (i, loopState) =>
            {
                if (list.somethings[i].ID == "5")
                {
                    loopState.Break();
                    Console.WriteLine("并行中断,条件为ID=5");
                    return;
                }
                list.somethings[i].DoSomething();
                Console.WriteLine(list.somethings[i].Name);

            });

            if (!loopResult.IsCompleted &&
                loopResult.LowestBreakIteration.HasValue)
            {
                Console.WriteLine("当前并行循环在{0}处中断",
                    loopResult.LowestBreakIteration.Value);
            }
            Console.WriteLine();
        }

        /// <summary>
        ///  并行For停止
        /// </summary>
        /// <param name="list"></param>
        static void GetSomethingStop(SomethingList list)
        {
            var count = list.somethings.Count;

            var loopResult = Parallel.For(0, count, (i, loopState) =>
            {
                if (list.somethings[i].ID == "6")
                {
                    loopState.Stop();
                    return;
                }
                list.somethings[i].DoSomething();
                Console.WriteLine(list.somethings[i].Name);
            });

            if (!loopResult.IsCompleted &&
                !loopResult.LowestBreakIteration.HasValue)
            {
                Console.WriteLine("并行循环停止");
            }
            Console.WriteLine();
        }

        /// <summary>
        ///  外部循环取消
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cts"></param>
        static void DoGetSometingLoop(SomethingList list, CancellationTokenSource cts)
        {
            var count = list.somethings.Count;

            CancellationToken token = cts.Token;

            var options = new ParallelOptions
            {
                CancellationToken = token
            };

            try
            {
                Parallel.For(0, count, options, (i) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("并行循环请求取消");
                    }

                    list.somethings[i].DoSomething();
                    Console.WriteLine(list.somethings[i].Name);
                });
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("捕捉到OperationCanceledException异常");
            }
            Console.WriteLine("外部循环取消");
            Console.WriteLine();
        }

        /// <summary>
        ///  小循环体的特殊处理
        /// </summary>
        /// <param name="list"></param>
        static void GetSomethingSmallSet(SomethingList list)
        {
            var count = list.somethings.Count;

            Double[] result = new Double[count];
            Parallel.ForEach(Partitioner.Create(0, count, count / 2),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        result[i] = (Double)(Int32.Parse(list.somethings[i].ID));
                        Console.WriteLine(i + " : " + result[i]);
                    }
                });
            Console.WriteLine();
        }

        /// <summary>
        ///  产生随机数
        /// </summary>
        static void GetRandomNumber()
        {
            Int32 numberOfSteps = 100;

            Double[] result = new Double[numberOfSteps];

            Parallel.ForEach(Partitioner.Create(0, numberOfSteps),
                new ParallelOptions(),
                () => { return new Random(MakeRandomSeed()); },
                (range, loopState, random) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        result[i] = random.NextDouble();
                        Console.WriteLine(result[i]);
                    }
                    return random;
                },
                _ => { });
        }

        static Int32 MakeRandomSeed()
        {
            return DateTime.Now.Millisecond;
        }
    }
}
