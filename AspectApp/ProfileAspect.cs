using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AspectApp {
	class StraceElement {
		public long StartTime { get; set; }
		public long Delta { get; set; }

		public StraceElement(long startTime) {
			StartTime = startTime;
			Delta = 0;
		}
	}

	static class ProfileStatistic {
		public static Dictionary<MethodBase, List<long>> Stat { get; } = new Dictionary<MethodBase, List<long>>();
		public static List<StraceElement> ProfileStrace { get; } = new List<StraceElement>();
		public static Stopwatch Timer { get; } = new Stopwatch();

		public static void Init() {
			ProfileStrace.Add(new StraceElement(0));
			Timer.Start();
		}

		public static void AddCall() {
			ProfileStrace.Add(new StraceElement(Timer.ElapsedMilliseconds));
		}

		public static void EndCall(MethodBase method) {
			var endTime = Timer.ElapsedMilliseconds;
			var index = ProfileStrace.Count - 1;
			var startTime = ProfileStrace[index].StartTime;
			var delta = ProfileStrace[index].Delta;
			ProfileStrace.RemoveAt(index);
			if (!Stat.ContainsKey(method)) {
				Stat.Add(method, new List<long>());
			}
			Stat[method].Add(endTime - startTime - delta);
			ProfileStrace[index - 1].Delta += endTime - startTime;
		}

		public static void PrintStatistic() {
			var list = new List<Tuple<long, MethodBase>>();
			foreach (var pair in Stat) {
				pair.Value.Sort();
				var sum = pair.Value.Sum();
				list.Add(new Tuple<long, MethodBase>(sum, pair.Key));
			}
			list.Sort((a, b) => b.Item1.CompareTo(a.Item1));
			Console.WriteLine("Name            Total     Calls     Avg       50%       Max");
			foreach (var sumAndMethod in list) {
				var sum = sumAndMethod.Item1;
				var name = sumAndMethod.Item2.Name;
				var curStat = Stat[sumAndMethod.Item2];
				var calls = curStat.Count;
				Console.WriteLine("{0,-16}{1,-10}{2,-10}{3,-10:0.0}{4,-10}{5}",
					name, sum, calls, 1.0 * sum / calls,
					curStat[calls / 2], curStat[calls - 1]);
			}
		}
	}

	[PSerializable]
	public class ProfileAttribute : OnMethodBoundaryAspect {
		public override void OnEntry(MethodExecutionArgs args) {
			ProfileStatistic.AddCall();
		}

		public override void OnExit(MethodExecutionArgs args) {
			ProfileStatistic.EndCall(args.Method);
		}
	}
}
