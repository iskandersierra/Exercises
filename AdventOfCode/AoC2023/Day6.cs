using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace AoC2023;

public static partial class Day6
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day6",
        "Wait For It",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/6")
    ));

    public static IProblem Instance => instance.Value;

    private static IEnumerable<IProblemInputParser> GetInputParsers()
    {
        yield return new StringInputParser();
    }

    private static IEnumerable<IProblemInputSource> GetInputSources()
    {
        yield return new ProblemInputEmbeddedSource(
            "sample1",
            "First sample",
            "AoC2023.Day6.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day6.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record InputRace(int Time, int Distance);

    public record Input(InputRace[] Races) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> There are [green]{Races.Length}[/] races");
        }
    }

    [GeneratedRegex(@"^\w+:(\s*(?<num>\d+))*$")]
    private static partial Regex GetLineRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: <time> <distance>"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            using var reader = new StringReader(input);
            var line = reader.ReadLine()!;
            var match = GetLineRegex().Match(line);
            var times = match.Groups["num"].Captures.Select(c => int.Parse(c.Value)).ToArray();
            line = reader.ReadLine()!;
            match = GetLineRegex().Match(line);
            var distances = match.Groups["num"].Captures.Select(c => int.Parse(c.Value)).ToArray();
            var races = times.Zip(distances, (t, d) => new InputRace(t, d)).ToArray();
            return new Input(races);
        }
    }

    public record Output(long Result) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The result is [green]{Result}[/]");
        }
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var result = 1L;

            var epsilon = 0.000001;
            foreach (var race in input.Races)
            {
                console.WriteLine($"Race for {race.Time} ms to beat {race.Distance} mm");

                var t = (double)race.Time;
                var d = (double)race.Distance;
                var term = t * t - 4 * d;
                console.WriteLine($"  - Term: {term}");
                var diff = Math.Sqrt(term) * 0.5;
                console.WriteLine($"  - Diff: {diff}");
                var x0 = t * 0.5 - diff;
                var x1 = t * 0.5 + diff;
                console.WriteLine($"  - x0: {x0}");
                console.WriteLine($"  - x1: {x1}");
                var value = (int)(Math.Floor(x1 - epsilon) - Math.Ceiling(x0 + epsilon) + 1);
                console.WriteLine($"  - R: {value}");
                result *= value;
            }

            console.MarkupLineInterpolated($"[yellow]Result:[/] {result}");
        }
    }

    public class Step1Solver() :
        ProblemSolver("step1", "Solver for step 1"),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var result = 1L;

            var epsilon = 0.000001;
            foreach (var race in input.Races)
            {
                var t = (double)race.Time;
                var d = (double)race.Distance;
                var term = t * t - 4 * d;
                var diff = Math.Sqrt(term) * 0.5;
                var x0 = t * 0.5 - diff;
                var x1 = t * 0.5 + diff;
                var value = (int)(Math.Floor(x1 - epsilon) - Math.Ceiling(x0 + epsilon) + 1);
                result *= value;
            }

            return new Output(result);
        }
    }

    public class Step2ShowSolver() :
        ProblemSolver("step2show", "Solver for step 2"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var result = 1L;

            var epsilon = 0.000001;
            var t = double.Parse(input.Races.Select(r => r.Time).Aggregate("", (acc, v) => acc + v));
            var d = double.Parse(input.Races.Select(r => r.Distance).Aggregate("", (acc, v) => acc + v));
            console.WriteLine($"Race for {t} ms to beat {d} mm");

            var term = t * t - 4 * d;
            console.WriteLine($"  - Term: {term}");
            var diff = Math.Sqrt(term) * 0.5;
            console.WriteLine($"  - Diff: {diff}");
            var x0 = t * 0.5 - diff;
            var x1 = t * 0.5 + diff;
            console.WriteLine($"  - x0: {x0}");
            console.WriteLine($"  - x1: {x1}");
            var value = (int)(Math.Floor(x1 - epsilon) - Math.Ceiling(x0 + epsilon) + 1);
            console.WriteLine($"  - R: {value}");
            result *= value;

            console.MarkupLineInterpolated($"[yellow]Result:[/] {result}");
        }
    }

    public class Step2Solver() :
        ProblemSolver("step2", "Solver for step 2"),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var result = 1L;

            var epsilon = 0.000001;
            var t = double.Parse(input.Races.Select(r => r.Time).Aggregate("", (acc, v) => acc + v));
            var d = double.Parse(input.Races.Select(r => r.Distance).Aggregate("", (acc, v) => acc + v));

            var term = t * t - 4 * d;
            var diff = Math.Sqrt(term) * 0.5;
            var x0 = t * 0.5 - diff;
            var x1 = t * 0.5 + diff;
            var value = (int)(Math.Floor(x1 - epsilon) - Math.Ceiling(x0 + epsilon) + 1);
            result *= value;

            return new Output(result);
        }
    }
}
