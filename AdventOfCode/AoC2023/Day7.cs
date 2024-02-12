using Exercises;
using Spectre.Console;
using System;
using System.Text.RegularExpressions;

namespace AoC2023;

public static partial class Day7
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day7",
        "Camel Cards",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/7")
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
            "AoC2023.Day7.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day7.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    private static readonly Dictionary<char, byte> CardValuesStep1 =
        "23456789TJQKA"
            .Select((c, i) => (c, (byte)i))
            .ToDictionary(t => t.c, t => t.Item2);

    private static readonly Dictionary<char, byte> CardValuesStep2 =
        "J23456789TQKA"
            .Select((c, i) => (c, (byte)i))
            .ToDictionary(t => t.c, t => t.Item2);

    public enum HandType
    {
        HighCard = 0,
        OnePair = 1,
        TwoPairs = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6,
    }

    private static HandType ComputeHandTypeStep1(byte[] hand)
    {
        var counts = hand
            .GroupBy(c => c)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .ToArray();

        return counts switch
        {
            [5] => HandType.FiveOfAKind,
            [4, 1] => HandType.FourOfAKind,
            [3, 2] => HandType.FullHouse,
            [3, 1, 1] => HandType.ThreeOfAKind,
            [2, 2, 1] => HandType.TwoPairs,
            [2, 1, 1, 1] => HandType.OnePair,
            _ => HandType.HighCard
        };
    }

    private static HandType ComputeHandTypeStep2(byte[] hand)
    {
        var js = hand.Count(c => c == 0);

        var counts = hand
            .Where(c => c != 0)
            .GroupBy(c => c)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .ToArray();

        return (js, counts) switch
        {
            (0, [5]) => HandType.FiveOfAKind,
            (0, [4, 1]) => HandType.FourOfAKind,
            (0, [3, 2]) => HandType.FullHouse,
            (0, [3, 1, 1]) => HandType.ThreeOfAKind,
            (0, [2, 2, 1]) => HandType.TwoPairs,
            (0, [2, 1, 1, 1]) => HandType.OnePair,
            (0, _) => HandType.HighCard,

            (1, [4]) => HandType.FiveOfAKind,
            (1, [3, 1]) => HandType.FourOfAKind,
            (1, [2, 2]) => HandType.FullHouse,
            (1, [2, 1, 1]) => HandType.ThreeOfAKind,
            (1, _) => HandType.OnePair,

            (2, [3]) => HandType.FiveOfAKind,
            (2, [2, 1]) => HandType.FourOfAKind,
            (2, _) => HandType.ThreeOfAKind,

            (3, [2]) => HandType.FiveOfAKind,
            (3, _) => HandType.FourOfAKind,

            (4, _) => HandType.FiveOfAKind,

            (5, _) => HandType.FiveOfAKind,
        };
    }

    public record InputHand(string Hand, int Bid)
    {
        public override string ToString() =>
            $"{Hand} {Bid}";
    }

    public record Input(InputHand[] Hands) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> There are [green]{Hands.Length}[/] hands");
        }
    }

    [GeneratedRegex(@"^(?<hand>\w{5})\s+(?<bid>\d+)$")]
    private static partial Regex GetHandRegex();

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
            var hands = new List<InputHand>();
            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = GetHandRegex().Match(line);
                var hand = match.Groups["hand"].Value;
                var bid = int.Parse(match.Groups["bid"].Value);
                hands.Add(new InputHand(hand, bid));
            }

            return new Input(hands.ToArray());
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

    public record HandData(InputHand Hand, byte[] Values, HandType Type)
    {
        public override string ToString() =>
            $"{Hand,-10} {Type}";
    }

    private static HandData GetHandDataStep1(InputHand hand)
    {
        var values = hand.Hand.Select(c => CardValuesStep1[c]).ToArray();
        var type = ComputeHandTypeStep1(values);
        return new HandData(hand, values, type);
    }

    private static HandData GetHandDataStep2(InputHand hand)
    {
        var values = hand.Hand.Select(c => CardValuesStep2[c]).ToArray();
        var type = ComputeHandTypeStep2(values);
        return new HandData(hand, values, type);
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var result = 0L;

            var hands = input.Hands
                .Select(GetHandDataStep1)
                .OrderByDescending(e => e, HandComparer.Instance)
                .ToList();
            console.WriteLine("Sorted hands:");
            foreach (var hand in hands)
            {
                console.WriteLine($"- {hand}");
            }
            result = hands.Select((h, i) => h.Hand.Bid * (hands.Count - (long)i)).Sum();

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

            var result = 0L;

            var hands = input.Hands
                .Select(GetHandDataStep1)
                .OrderByDescending(e => e, HandComparer.Instance)
                .ToList();
            result = hands.Select((h, i) => h.Hand.Bid * (hands.Count - (long)i)).Sum();

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

            var result = 0L;

            var hands = input.Hands
                .Select(GetHandDataStep2)
                .OrderByDescending(e => e, HandComparer.Instance)
                .ToList();
            console.WriteLine("Sorted hands:");
            foreach (var hand in hands)
            {
                console.WriteLine($"- {hand}");
            }
            result = hands.Select((h, i) => h.Hand.Bid * (hands.Count - (long)i)).Sum();

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

            var result = 0L;

            var hands = input.Hands
                .Select(GetHandDataStep2)
                .OrderByDescending(e => e, HandComparer.Instance)
                .ToList();
            result = hands.Select((h, i) => h.Hand.Bid * (hands.Count - (long)i)).Sum();

            return new Output(result);
        }
    }

    public class HandComparer : IComparer<HandData>
    {
        public static HandComparer Instance { get; } = new();

        public int Compare(HandData? x, HandData? y)
        {
            if (x is null && y is null)
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return 1;

            var typeComparison = x.Type.CompareTo(y.Type);
            if (typeComparison != 0)
                return typeComparison;

            return x.Values
                .Select((t, i) => t.CompareTo(y.Values[i]))
                .FirstOrDefault(comparison => comparison != 0);
        }
    }
}
