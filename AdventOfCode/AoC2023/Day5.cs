using Exercises;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public static partial class Day5
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day5",
        "If You Give A Seed A Fertilizer",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/5")
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
            "AoC2023.Day5.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day5.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record InputMapLine(int Destination, int Source, int Length);
    public record InputMap(string From, string To, InputMapLine[] Lines);
    public record Input(int[] Seeds, InputMap[] Maps) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> There are [green]{Seeds.Length}[/] seeds and [green]{Maps.Length}[/] maps");
        }
    }

    [GeneratedRegex(@"^seeds:(\s*(?<seed>\d+))+$")]
    private static partial Regex GetSeedsRegex();

    [GeneratedRegex(@"^(?<source>\w+)-to-(?<destination>\w+)\s+map:$")]
    private static partial Regex GetMapHeaderRegex();

    [GeneratedRegex(@"^(?<dest>\d+)\s+(?<src>\d+)\s+(?<len>\d+)$")]
    private static partial Regex GetMapLineRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: <length> <digits>"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            using var reader = new StringReader(input);

            var line = reader.ReadLine();
            var match = GetSeedsRegex().Match(line);
            var seeds = match.Groups["seed"].Captures.Select(c => int.Parse(c.Value)).ToArray();
            reader.ReadLine(); // skip empty line

            var maps = new List<InputMap>();
            while (true)
            {
                line = reader.ReadLine();
                if (line is null) break;

                match = GetMapHeaderRegex().Match(line);
                var sourceName = match.Groups["source"].Value;
                var destinationName = match.Groups["destination"].Value;

                var mapLines = new List<InputMapLine>();
                while (true)
                {
                    line = reader.ReadLine();
                    if (line is null || line.Length == 0) break;

                    match = GetMapLineRegex().Match(line);
                    var dest = int.Parse(match.Groups["dest"].Value);
                    var src = int.Parse(match.Groups["src"].Value);
                    var len = int.Parse(match.Groups["len"].Value);
                    mapLines.Add(new InputMapLine(dest, src, len));
                }

                maps.Add(new InputMap(sourceName, destinationName, mapLines.ToArray()));
            }

            return new Input(seeds.ToArray(), maps.ToArray());
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

            var result = 0;
            
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

            var result = 0;
            
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

            var result = 0;

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

            var result = 0;

            return new Output(result);
        }
    }
}
