using System.Collections;
using Exercises;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console.Rendering;
using System;

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

    private static ISegmentSet<Segment<long>> MapSourceUsing(ISegmentSet<Segment<long>> sources, InputMap map)
    {
        var desc = Segments.Int64;
        var result = desc.CreateSet();

        foreach (var source in sources)
        {
            var leftovers = desc.CreateSet(source);
            foreach (var line in map.Lines)
            {
                if (leftovers.IsEmpty) break;
                var offset = line.Destination - line.Source;
                var lineSegment = desc.Create(line.Source, line.Source + line.Length);
                var intersection = leftovers.Intersection(lineSegment);
                leftovers.Subtract(lineSegment);
                foreach (var intersectionSegment in intersection)
                {
                    var mappedSegment = desc.Create(
                        intersectionSegment.Start + offset,
                        intersectionSegment.End + offset);

                    result.Add(mappedSegment);
                }
            }

            if (!leftovers.IsEmpty)
            {
                result.AddRange(leftovers);
            }
        }

        return result;
    }

    private static ISegmentSet<Segment<long>> MapSourceUsing(
        ISegmentSet<Segment<long>> sources,
        InputMap map,
        IAnsiConsole console)
    {
        var desc = Segments.Int64;
        var result = desc.CreateSet();

        foreach (var source in sources)
        {
            console.WriteLine($"  - Mapping source {source} ...");
            var leftovers = desc.CreateSet(source);
            foreach (var line in map.Lines)
            {
                var offset = line.Destination - line.Source;
                var lineSegment = desc.Create(line.Source, line.Source + line.Length);
                console.WriteLine($"    - Leftovers {leftovers} with map {line} using segment {lineSegment} and offset {offset}");
                if (leftovers.IsEmpty) break;
                var intersection = leftovers.Intersection(lineSegment);
                console.WriteLine($"      - Leftover \u2229 Map = {intersection}");
                leftovers.Subtract(lineSegment);
                console.WriteLine($"      - Leftover \u2216 Map = {leftovers}");
                foreach (var intersectionSegment in intersection)
                {
                    var mappedSegment = desc.Create(
                        intersectionSegment.Start + offset,
                        intersectionSegment.End + offset);

                    console.WriteLine($"      - Mapped segment: {intersectionSegment} ==> {mappedSegment}");

                    result.Add(mappedSegment);
                }
            }

            if (!leftovers.IsEmpty)
            {
                console.WriteLine($"    - Adding remaining leftovers: {leftovers}");
                result.AddRange(leftovers);
            }
            console.WriteLine($"    - Partial results: {result}");
        }

        return result;
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

            var result = 0L;

            var desc = Segments.Int64;
            ISegmentSet<Segment<long>> sources = desc
                .CreateSet(input.Seeds.Chunk(2).Select(c => desc.Create(c[0], c[0] + c[1])));
            console.WriteLine($"seeds: {sources}");

            foreach (var map in input.Maps)
            {
                var destinations = MapSourceUsing(sources, map, console);
                console.WriteLine($"{map.To}: {destinations}");
                sources = destinations;
            }

            result = sources.First().Start;

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

            var desc = Segments.Int64;
            ISegmentSet<Segment<long>> sources = desc
                .CreateSet(input.Seeds.Chunk(2).Select(c => desc.Create(c[0], c[0] + c[1])));

            foreach (var map in input.Maps)
            {
                var destinations = MapSourceUsing(sources, map);
                sources = destinations;
            }

            result = sources.First().Start;

            return new Output(result);
        }
    }
}
