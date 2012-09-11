using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ParallelLoop.Practice
{
    public class PracticeAddNumber
    {
        static Int32 num = 10;
        static Double[] number1;
        static Double[] number2;
        static Double[] numbers;

        private static void ProduceNumber()
        {
            number1 = new Double[num];

            Parallel.ForEach(Partitioner.Create(0, num),
                new ParallelOptions(),
                () => { return new Random(500); },
                (range, loopState, random) =>
                {
                    for (Int32 i = range.Item1; i < range.Item2; i++)
                    {
                        number1[i] = random.NextDouble() * 1000000000000000;
                    }
                    return random;
                },
                _ => { });

            number2 = new Double[num];
            Parallel.ForEach(Partitioner.Create(0, num),
                new ParallelOptions(),
                () => { return new Random(500); },
                (range, loopState, random) =>
                {
                    for (Int32 i = range.Item1; i < range.Item2; i++)
                    {
                        number2[i] = random.NextDouble() * 1000000000000000;
                    }
                    return random;
                },
                _ => { });
        }

        public static void AddNumberSequence()
        {
            ProduceNumber();
            numbers = new Double[num];
            for (int i = 0; i < num; i++)
            {
                numbers[i] = number1[i] + number2[i];
                Console.WriteLine(number1[i] + " + " + number2[i] + " = " + numbers[i]);
            }
        }

        public static void AddNumberParallel()
        {
            ProduceNumber();
            numbers = new Double[num];

            Parallel.For(0, num, (i) =>
            {
                numbers[i] = number1[i] + number2[i];
                Console.WriteLine(number1[i] + " + " + number2[i] + " = " + numbers[i]);
            });
        }
    }
}
