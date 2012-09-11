using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelLoop.Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            PracticeSortElement.SortNumberSequence();
            Console.WriteLine();
            // 暂时实现不了
            PracticeSortElement.SortNumberParallel();
            Console.WriteLine();

            PracticeAddNumber.AddNumberSequence();
            Console.WriteLine();
            PracticeAddNumber.AddNumberParallel();
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
