using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

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
		if (subprogramArguments.Count() == 0) {
		    Console.WriteLine("No program specified on command line!");
		    return false;
		} else {
		    return ParseProgramArguments(args.Take(args.IndexOf("--")));
		}
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
		try {
		    affinity = int.Parse(args.First(), NumberStyles.HexNumber, null);
		    return true;
		} catch(FormatException) {
		    try {
			affinity = int.Parse(args.First());
			return true;
		    } catch (Exception) {
			Console.WriteLine("Couldn't parse affinity mask! {0}", args.First());
			return false;
		    }
		}

	    }
	}
	public static string Join(IEnumerable<string> contents, string delimiter)
	{
	    StringBuilder sb = new StringBuilder();
	    foreach (var v in contents) {
		sb.Append(v);
		sb.Append(delimiter);
	    }
	    return sb.ToString();
	}
	public static string Join(IEnumerable<string> contents)
	{
	    return Join(contents, " ");
	}
	public static ProcessStartInfo GetProcess()
	{
	    if (subprogramArguments.Count() == 0) {
		Console.WriteLine("No subprogram specified!");
		return null;
	    } else {
		return new ProcessStartInfo(subprogramArguments.First(),
			Join((from x in subprogramArguments.Skip(1)
				select string.Format("\"{0}\"", x))));
	    }
	}
    }
    public static class Program
    {
	public static void Usage()
	{
	    Console.WriteLine("Usage: set-affinity.exe <affinity-mask> -- <program> <program-args>");
	}
	public static void Main(string[] args)
	{
	    if (!Args.Parse(args)) {
		Usage();
		return;
	    }
	    ProcessStartInfo psi = Args.GetProcess();
	    if (psi == null) {
		Console.WriteLine("No program to execute specified!");
		return;
	    } else {
		Process p = Process.Start(psi);
		if (p == null) {
		    Console.WriteLine("Couldn't start {0}", psi.FileName);
		    return;
		} else {
		    Console.WriteLine("Setting affinity for process {0} to {1}", psi.FileName, Args.Affinity);
		    p.ProcessorAffinity = (IntPtr)Args.Affinity;
		    Console.WriteLine("Bye!");
		}
	    }
	}
    }
}
