using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspectApp {
	static class FibSolver {
		[Profile]
		public static long Fib(int i) {
			if (i == 0 || i == 1) {
				return 1;
			}
			return Fib(i - 1) + Fib(i - 2);
		}

		[Profile, PureCache]
		public static long Fib2(int i) {
			if (i == 0 || i == 1) {
				return 1;
			}
			return Fib2(i - 1) + Fib2(i - 2);
		}
	}

	//profiled by AssemblyInfo and slice
	static class JustWaiter {
		public static void Fun1() {
			System.Threading.Thread.Sleep(1000);
			Fun2();
			Fun2();
			System.Threading.Thread.Sleep(2000);
		}

		public static void Fun2() {
			System.Threading.Thread.Sleep(1000);
		}
	}
}
