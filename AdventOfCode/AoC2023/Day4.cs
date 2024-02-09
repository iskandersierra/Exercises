using Exercises;
using Spectre.Console;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public static partial class Day4
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day4",
        "Scratchcards",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/4")
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
            "AoC2023.Day4.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day4.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record InputCard(int Id, int[] Winning, int[] Actual)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Card {Id,3}:");
            for (var i = 0; i < Winning.Length; i++)
            {
                sb.Append($" {Winning[i],2}");
            }
            sb.Append(" |");
            for (var i = 0; i < Actual.Length; i++)
            {
                sb.Append($" {Actual[i],2}");
            }
            return sb.ToString();
        }
    }

    public record Input(InputCard[] Cards) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> There are [green]{Cards.Length}[/] cards");
        }
    }

    [GeneratedRegex(@"^Card\s+(?<id>\d+):(\s+(?<win>\d+))*\s*\|(\s+(?<actual>\d+))*$")]
    private static partial Regex GetCardRegex();

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
            var cards = new List<InputCard>();

            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = GetCardRegex().Match(line);
                if (!match.Success)
                {
                    throw new InvalidOperationException($"Invalid line: {line}");
                }
                var id = int.Parse(match.Groups["id"].Value);
                var win = match.Groups["win"].Captures.Select(c => int.Parse(c.Value)).ToArray();
                var actual = match.Groups["actual"].Captures.Select(c => int.Parse(c.Value)).ToArray();
                cards.Add(new InputCard(id, win, actual));
            }

            return new Input(cards.ToArray());
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
            foreach (var card in input.Cards)
            {
                var win = new HashSet<int>(card.Winning);
                var actual = new HashSet<int>(card.Actual);
                var count = win.Intersect(actual).Count();
                var score = count switch
                {
                    <= 0 => 0,
                    1 => 1,
                    _ => (int)Math.Pow(2, count - 1),
                };
                result += score;

                console.MarkupLineInterpolated($"{card} has score [green]{score}[/]");
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

            var result = 0;
            foreach (var card in input.Cards)
            {
                var win = new HashSet<int>(card.Winning);
                var actual = new HashSet<int>(card.Actual);
                var count = win.Intersect(actual).Count();
                var score = count switch
                {
                    <= 0 => 0,
                    1 => 1,
                    _ => (int)Math.Pow(2, count - 1),
                };
                result += score;
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

            var result = 0;

            var counters = new int[input.Cards.Length];
            Array.Fill(counters, 1);

            for (int i = 0; i < counters.Length; i++)
            {
                var card = input.Cards[i];
                console.MarkupLineInterpolated($"Found {counters[i]} instances of card {card.Id}");
                result += counters[i];
                var win = new HashSet<int>(card.Winning);
                var actual = new HashSet<int>(card.Actual);
                var count = win.Intersect(actual).Count();
                for (int j = 1; j <= count; j++)
                {
                    counters[i + j] += counters[i];
                }
            }

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

            var counters = new int[input.Cards.Length];
            Array.Fill(counters, 1);

            for (int i = 0; i < counters.Length; i++)
            {
                result += counters[i];
                var win = new HashSet<int>(input.Cards[i].Winning);
                var actual = new HashSet<int>(input.Cards[i].Actual);
                var count = win.Intersect(actual).Count();
                for (int j = 1; j <= count; j++)
                {
                    counters[i + j] += counters[i];
                }
            }
            
            return new Output(result);
        }
    }
}
