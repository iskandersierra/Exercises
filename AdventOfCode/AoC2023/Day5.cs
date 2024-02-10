using System.Collections;
using Exercises;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console.Rendering;

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

    public record InputMapLine(long Destination, long Source, long Length);
    public record InputMap(string From, string To, InputMapLine[] Lines);
    public record Input(long[] Seeds, InputMap[] Maps) : IProblemInput, IHasPrintSummary
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
            var seeds = match.Groups["seed"].Captures.Select(c => long.Parse(c.Value)).ToArray();
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
                    var dest = long.Parse(match.Groups["dest"].Value);
                    var src = long.Parse(match.Groups["src"].Value);
                    var len = long.Parse(match.Groups["len"].Value);
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

    private static long MapSourceUsing(long source, InputMap map)
    {
        foreach (var line in map.Lines)
        {
            if (source >= line.Source && source < line.Source + line.Length)
            {
                return line.Destination + (source - line.Source);
            }
        }

        return source;
    }

    private static Dictionary<string, long[]> CreateMapping(Input input)
    {
        var dict = new Dictionary<string, long[]>();
        dict["seeds"] = input.Seeds;

        var currentSource = input.Seeds;
        foreach (var map in input.Maps)
        {
            var currentDestination = new long[currentSource.Length];
            for (var i = 0; i < currentSource.Length; i++)
            {
                var source = currentSource[i];
                var destination = MapSourceUsing(source, map);
                currentDestination[i] = destination;
            }
            dict[map.To] = currentDestination;
            currentSource = currentDestination;
        }
        return dict;
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var result = 0L;

            var mappings = CreateMapping(input);

            var table = new Table();
            var highlightStyle = new Style(Color.Green, decoration: Decoration.Bold);
            table.AddColumn(new TableColumn(new Markup("type", highlightStyle)));
            for (int i = 0; i < input.Seeds.Length; i++)
                table.AddColumn($"value {i}");

            List<IRenderable> row =
            [
                new Markup("seeds", highlightStyle),
                ..input.Seeds.Select(seed => new Markup($"{seed}"))
            ];
            table.AddRow(row);

            foreach (var map in input.Maps)
            {
                row = 
                [
                    new Markup(map.To, highlightStyle),
                    .. mappings[map.To].Select(dest => new Markup($"{dest}"))
                ];
                table.AddRow(row);
            }

            console.Write(table);

            result = mappings["location"].Min();

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

            var mappings = CreateMapping(input);
            result = mappings["location"].Min();

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
            var seeds = new List<LongSegment>();
            for (int i = 0; i < input.Seeds.Length; i+=2)
            {
                seeds.Add(new LongSegment(input.Seeds[i], input.Seeds[i+1]));
            }

            foreach (var seed in seeds)
            {
                console.WriteLine($"Seed: {seed}");
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

            return new Output(result);
        }
    }
}

public readonly record struct LongSegment(long Start, long End)
{
    public static readonly LongSegment Empty = new(0, 0);

    /// <summary>
    /// Exclusive end of the segment
    /// </summary>
    public long Length { get; } = End - Start;

    public bool IsEmpty { get; } = Start >= End;

    public bool Contains(long value) => value >= Start && value < End;

    public bool Contains(LongSegment segment) => segment.Start >= Start && segment.End <= End;

    public LongSegment Intersect(LongSegment segment)
    {
        if (IsEmpty || segment.IsEmpty) return Empty;
        var start = Math.Max(Start, segment.Start);
        var end = Math.Min(End, segment.End);
        if (start < end) return new LongSegment(start, end);
        return Empty;
    }

    public LongSegment Merge(LongSegment segment)
    {
        if (IsEmpty) return segment;
        if (segment.IsEmpty) return this;
        var start = Math.Min(Start, segment.Start);
        var end = Math.Max(End, segment.End);
        return new LongSegment(start, end);
    }

    public override string ToString()
    {
        return IsEmpty ? "\u2205" : $"[{Start}, {End})";
    }

    public static readonly IComparer<LongSegment> StartComparer = new LongSegmentStartComparer();

    private class LongSegmentStartComparer : IComparer<LongSegment>
    {
        public int Compare(LongSegment x, LongSegment y)
        {
            if (x.IsEmpty)
            {
                if (y.IsEmpty) return 0;
                return -1;
            }
            if (y.IsEmpty) return 1;
            return x.Start.CompareTo(y.Start);
        }
    }
}

public class LongSegmentSet : IEnumerable<LongSegment>
{
    private List<LongSegment> segments = new();

    public IEnumerator<LongSegment> GetEnumerator()
    {
        return segments.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(LongSegment item)
    {
        if (item.IsEmpty) return;
        // TODO: Optimize
        var newSegments = new List<LongSegment>();
        var merged = item;
        var mergedAdded = false;
        foreach (var segment in segments)
        {
            if (segment.End < merged.Start)
            {
                newSegments.Add(segment);
            }
            else if (segment.Start > merged.End)
            {
                if (!mergedAdded)
                {
                    newSegments.Add(merged);
                    mergedAdded = true;
                }
                newSegments.Add(segment);
            }
            else
            {
                merged = merged.Merge(segment);
            }
        }

        if (!mergedAdded)
        {
            newSegments.Add(merged);
        }

        segments = newSegments;
    }

    public void Clear()
    {
        segments.Clear();
    }

    public bool Contains(LongSegment item)
    {
        return segments.Contains(item);
    }

    public void CopyTo(LongSegment[] array, int arrayIndex)
    {
        segments.CopyTo(array, arrayIndex);
    }

    public int Count => segments.Count;
}
