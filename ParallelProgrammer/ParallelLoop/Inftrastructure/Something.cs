using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ParallelLoop.Inftrastructure
{
    public class Something
    {
        public String Name { get; set; }

        public String ID { get; set; }

        public void DoSomething() { Console.WriteLine("Do some thing!"); }
    }

    public class SomethingList
    {
        public List<Something> somethings { get; set; }
    }
}
