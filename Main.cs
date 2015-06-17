using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace SetAffinity
{
    public static class Args
    {
	private static List<string> subprogramArguments = new List<string>();
	private static int affinity = 0;
	public static int Affinity { get { return affinity; } }
	public static IEnumerable<string> SubprogramArguments { get { return subprogramArguments; } }
	public static int IndexOf<T>(this IEnumerable<T> args, T value) 
	{
	    int index = 0;
	    foreach (var v in args) {
		if (v.Equals(value)) {
		    return index;
		} else {
		    index++;
		}
	    }
	    return -1;
	}
	public static bool Parse(IEnumerable<string> args)
	{
	    if (args.Contains("--help") || args.Contains("-help")) {
		return false;
	    } else if (!args.Contains("--")) {
		Console.WriteLine("Couldn't find the -- to separate the invocation from the current program!");
		return false;
	    } else {
		int index = args.IndexOf("--");
		subprogramArguments.Clear();
		// divide up the number of arguments between sub and this program
		subprogramArguments.AddRange(args.Skip(args.IndexOf("--") + 1));
		return ParseProgramArguments(args.Take(args.IndexOf("--")));
	    }
	}
	private static bool ParseProgramArguments(IEnumerable<string> args)
	{
	    if (args.Count() == 0) {
		Console.WriteLine("No affinity mask provided!");
		return false;
	    } else if (args.Count() > 1) {
		Console.WriteLine("Too many arguments provided! Affinity mask only");
		return false;
	    } else {
		if (!int.TryParse(args.First(), out affinity)) {
		    Console.WriteLine("Couldn't parse affinity mask!");
		    return false;
		} else {
		    return true;
		}
	    }
	}
	public static class Program
	{
	    public static void Main(string[] args)
	    {
		if (!Args.Parse(args)) {
		    return;
		}
		Console.WriteLine("Affinity mask: {0}", Args.Affinity);
		Console.WriteLine("--");
		foreach (var v in Args.SubprogramArguments) {
		    Console.WriteLine(v);
		}
	    }
	}
    }
}
