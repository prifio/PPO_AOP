using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace AspectApp {
	class Program {
		static void Main(string[] args) {
			ProfileStatistic.Init();
			FibSolver.Fib(33);
			FibSolver.Fib2(33);
			JustWaiter.Fun1();
			ProfileStatistic.PrintStatistic();
			Console.ReadLine();
		}
	}
}
