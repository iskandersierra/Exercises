using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace AoC2023;

public static partial class Day1
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day1",
        "Trebuchet?!",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/1")
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
            "AoC2023.Day1.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "sample2",
            "Second sample",
            "AoC2023.Day1.sample2.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day1.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record Input(string[] Lines) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Solve for [green]{Lines.Length}[/] lines of input.");
        }
    }

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
            var lines = new List<string>();

            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                lines.Add(line);
            }

            return new Input(lines.ToArray());
        }
    }

    public record Output(long Result) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The result is [green]{Result}[/].");
        }
    }

    private static readonly IReadOnlyCollection<(string Pattern, int Value)> step1Patterns =
    [
        ("1", 1),
        ("2", 2),
        ("3", 3),
        ("4", 4),
        ("5", 5),
        ("6", 6),
        ("7", 7),
        ("8", 8),
        ("9", 9),
    ];

    private static readonly IReadOnlyCollection<(string Pattern, int Value)> step2Patterns =
    [
        .. step1Patterns,
        ("one", 1),
        ("two", 2),
        ("three", 3),
        ("four", 4),
        ("five", 5),
        ("six", 6),
        ("seven", 7),
        ("eight", 8),
        ("nine", 9),
    ];

    private static int FindPatternValue(
        ReadOnlySpan<char> input,
        IReadOnlyCollection<(string Pattern, int Value)> patterns)
    {

        for (int i = 0; i < patterns.Count; i++)
        {
            var (pattern, value) = patterns.ElementAt(i);
            if (pattern.Length > input.Length) continue;
            if (input.StartsWith(pattern)) return value;
        }
        return -1;
    }

    private static int FindFirstPatternValue(
        string input,
        IReadOnlyCollection<(string Pattern, int Value)> patterns)
    {
        for (int start = 0; start < input.Length; start++)
        {
            var value = FindPatternValue(input.AsSpan(start), patterns);
            if (value != -1) return value;
        }
        return -1;
    }

    private static int FindLastPatternValue(
        string input,
        IReadOnlyCollection<(string Pattern, int Value)> patterns)
    {
        for (int start = input.Length - 1; start >= 0; start--)
        {
            var value = FindPatternValue(input.AsSpan(start), patterns);
            if (value != -1) return value;
        }
        return -1;
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var sum = 0L;

            foreach (var line in input.Lines)
            {
                var first = FindFirstPatternValue(line, step1Patterns);
                var last = FindLastPatternValue(line, step1Patterns);
                console.MarkupLineInterpolated($"{line} -> {first}{last}");
                if (first == -1 || last == -1)
                {
                    throw new InvalidOperationException("Invalid input");
                }
                var value = first * 10 + last;
                sum += value;
            }

            console.MarkupLineInterpolated($"Result: [green]{sum}[/]");
        }
    }

    public class Step1Solver() :
        ProblemSolver("step1", "Solver for step 1"),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var sum = 0L;

            foreach (var line in input.Lines)
            {
                var first = FindFirstPatternValue(line, step1Patterns);
                var last = FindLastPatternValue(line, step1Patterns);
                if (first == -1 || last == -1)
                {
                    throw new InvalidOperationException($"{line} -> {first} {last}");
                }
                var value = first * 10 + last;
                sum += value;
            }

            return new Output(sum);
        }
    }

    public class Step2ShowSolver() :
        ProblemSolver("step2show", "Solver for step 2"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var sum = 0L;

            foreach (var line in input.Lines)
            {
                var first = FindFirstPatternValue(line, step2Patterns);
                var last = FindLastPatternValue(line, step2Patterns);
                console.MarkupLineInterpolated($"{line} -> {first}{last}");
                if (first == -1 || last == -1)
                {
                    throw new InvalidOperationException("Invalid input");
                }
                var value = first * 10 + last;
                sum += value;
            }

            console.MarkupLineInterpolated($"Result: [green]{sum}[/]");
        }
    }

    public class Step2Solver() :
        ProblemSolver("step2", "Solver for step 2"),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var sum = 0L;

            foreach (var line in input.Lines)
            {
                var first = FindFirstPatternValue(line, step2Patterns);
                var last = FindLastPatternValue(line, step2Patterns);
                if (first == -1 || last == -1)
                {
                    throw new InvalidOperationException($"{line} -> {first} {last}");
                }
                var value = first * 10 + last;
                sum += value;
            }

            return new Output(sum);
        }
    }
}
