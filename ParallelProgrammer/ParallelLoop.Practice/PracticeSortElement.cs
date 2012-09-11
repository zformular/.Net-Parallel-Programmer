using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ValueHelper;
using ValueHelper.Infrastructure;

namespace ParallelLoop.Practice
{
    public class PracticeSortElement
    {
        static Int32 num = 10;
        static Double[] numbers;

        private static void ProduceNumber()
        {
            numbers = new Double[num];

            Parallel.ForEach(Partitioner.Create(0, num),
                new ParallelOptions(),
                () => { return new Random(500); },
                (range, loopState, random) =>
                {
                    for (Int32 i = range.Item1; i < range.Item2; i++)
                    {
                        numbers[i] = random.NextDouble() * 1000000000000000;
                    }
                    return random;
                },
                _ => { });
        }

        public static void SortNumberSequence()
        {
            ProduceNumber();

            for (int i = 0; i < num; i++)
            {
                for (int j = i; j < num; j++)
                {
                    if (numbers[j] < numbers[i])
                    {
                        var tempNum = numbers[i];
                        numbers[i] = numbers[j];
                        numbers[j] = tempNum;
                    }
                }
            }

            for (int i = 0; i < num; i++)
            {
                Console.WriteLine(numbers[i]);
            }
        }

        public static void SortNumberParallel()
        {
            ProduceNumber();

            Parallel.For(0, num, (i) =>
            {
                Parallel.For(i, num, (j) =>
                {
                    if (numbers[j] < numbers[i])
                    {
                        var tempNum = numbers[i];
                        numbers[i] = numbers[j];
                        numbers[j] = tempNum;
                    }
                });
            });

            for (int i = 0; i < num; i++)
            {
                Console.WriteLine(numbers[i]);
            }

            Console.WriteLine("暂时实现不了, 或者真的不行?");
        }
    }
}
