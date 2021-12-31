using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspectApp {
	class MyComparer : IEqualityComparer<Tuple<MethodBase, List<object>>> {
		public bool Equals(Tuple<MethodBase, List<object>> x, Tuple<MethodBase, List<object>> y) {
			return x.Item1.Equals(y.Item1) && x.Item2.SequenceEqual(y.Item2);
		}

		public int GetHashCode(Tuple<MethodBase, List<object>> obj) {
			var hashcode = new HashCode();
			hashcode.Add(obj.Item1);
			foreach (object o in obj.Item2) {
				hashcode.Add(o);
			}
			return hashcode.ToHashCode();
		}
	}

	static class AspectCache {
		public static Dictionary<Tuple<MethodBase, List<object>>, object> Cache { get; } =
			new Dictionary<Tuple<MethodBase, List<object>>, object>(new MyComparer());
	}

	[PSerializable]
	public class PureCacheAttribute : MethodInterceptionAspect {
		public override void OnInvoke(MethodInterceptionArgs args) {
			var list = args.Arguments.ToList();
			var tuple = new Tuple<MethodBase, List<object>>(args.Method, list);
			if (AspectCache.Cache.TryGetValue(tuple, out var result)) { 
				args.ReturnValue = result;
				return;
			}
			base.OnInvoke(args);
			AspectCache.Cache[tuple] = args.ReturnValue;
		}
	}
}
