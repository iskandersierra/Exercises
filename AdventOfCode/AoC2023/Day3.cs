using Exercises;
using Spectre.Console;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AoC2023;

public static partial class Day3
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day3",
        "Gear Ratios",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/3")
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
            "AoC2023.Day3.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day3.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record Input(char[,] Schematic) : IProblemInput, IHasPrintSummary
    {
        public int Width { get; } = Schematic.GetLength(0);
        public int Height { get; } = Schematic.GetLength(1);
        public int Size { get; } = Schematic.GetLength(0) * Schematic.GetLength(1);

        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Solve for a schematic of [yellow]{Width}x{Height}[/] ({Size} cells).");
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
            var cells = new List<string>();

            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                cells.Add(line);
            }

            var width = cells.Max(s => s.Length);
            var height = cells.Count;
            var schematic = new char[width, height];
            for (var y = 0; y < height; y++)
            {
                var line = cells[y];
                for (var x = 0; x < width; x++)
                {
                    var ch = x < line.Length ? line[x] : '.';
                    schematic[x, y] = ch;
                }
            }

            return new Input(schematic);
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

    record PartShape(int Column, int Row, int Length, string Text, int Value)
    {
        public static PartShape Create(Input input, int column, int row, int length)
        {
            var text = ReadPartString(input, column, row, length);
            var value = int.Parse(text);
            return new PartShape(column, row, length, text, value);
        }

        public virtual bool Equals(PartShape? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Column == other.Column && Row == other.Row;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsDigit(Input input, int column, int row)
    {
        return input.Schematic[column, row] is >= '0' and <= '9';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsSymbol(Input input, int column, int row)
    {
        return input.Schematic[column, row] is not '.' and not (>= '0' and <= '9');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsGearSymbol(Input input, int column, int row)
    {
        return input.Schematic[column, row] is '*';
    }

    static PartShape? FindPartAt(Input input, int column, int row)
    {
        if (!IsDigit(input, column, row)) return null;
        // find left and right bounds
        var left = column;
        while (left > 0 && IsDigit(input, left - 1, row)) left--;
        var right = column;
        while (right < input.Width - 1 && IsDigit(input, right + 1, row)) right++;
        var length = right - left + 1;
        return PartShape.Create(input, left, row, length);
    }

    static ISet<PartShape> GetGearParts(Input input, int column, int row)
    {
        // Look around the gear to check for parts and accumulate them in the resulting set
        var parts = new HashSet<PartShape>();
        for (var i = -1; i < 2; i++)
        {
            var checkCol = column + i;
            if (checkCol < 0 || checkCol >= input.Width) continue;
            for (var j = -1; j < 2; j++)
            {
                var checkRow = row + j;
                if (checkRow < 0 || checkRow >= input.Height || (i, j) == (0, 0)) continue;
                var part = FindPartAt(input, checkCol, checkRow);
                if (part is not null)
                {
                    parts.Add(part);
                }
            }
        }
        return parts;
    }

    static string ReadPartString(Input input, int column, int row, int length)
    {
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            sb.Append(input.Schematic[column + i, row]);
        }
        return sb.ToString();
    }

    static bool IsActualPart(Input input, PartShape part)
    {
        // Look around the part to check for symbols. If any symbol ys found then it is an actual part
        var width = input.Schematic.GetLength(0);
        var height = input.Schematic.GetLength(1);
        if (part.Row > 0)
        {
            for (var i = -1; i < part.Length + 1; i++)
            {
                var checkCol = part.Column + i;
                if (checkCol >= 0 && checkCol < width)
                {
                    if (IsSymbol(input, checkCol, part.Row - 1)) return true;
                }
            }
        }

        if (part.Row < height - 1)
        {
            for (var i = -1; i < part.Length + 1; i++)
            {
                var checkCol = part.Column + i;
                if (checkCol >= 0 && checkCol < width)
                {
                    if (IsSymbol(input, checkCol, part.Row + 1)) return true;
                }
            }
        }

        if (part.Column > 0 &&
            IsSymbol(input, part.Column - 1, part.Row)) return true;

        if (part.Column + part.Length < width &&
            IsSymbol(input, part.Column + part.Length, part.Row)) return true;

        return false;
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;

            var result = 0;
            for (int row = 0; row < input.Height; row++)
            {
                for (int col = 0; col < input.Width; col++)
                {
                    if (FindPartAt(input, col, row) is { } part && part.Column == col)
                    {
                        if (IsActualPart(input, part))
                        {
                            console.MarkupLineInterpolated($"[green]{part}[/]");
                            result += part.Value;
                            col += part.Length - 1;
                        }
                        else
                        {
                            console.MarkupLineInterpolated($"[red]{part}[/]");
                        }
                    }
                }
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
            for (int row = 0; row < input.Height; row++)
            {
                for (int col = 0; col < input.Width; col++)
                {
                    if (FindPartAt(input, col, row) is { } part &&
                        part.Column == col &&
                        IsActualPart(input, part))
                    {
                        result += part.Value;
                        col += part.Length - 1;
                    }

                }
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
            for (int row = 0; row < input.Height; row++)
            {
                for (int col = 0; col < input.Width; col++)
                {
                    if (!IsGearSymbol(input, col, row)) continue;
                    var gearParts = GetGearParts(input, col, row);
                    if (gearParts.Count == 2)
                    {
                        var ratio = gearParts.Select(p => p.Value).Aggregate(1, (a, b) => a * b);
                        result += ratio;
                        console.MarkupLineInterpolated($"[green]Gear at ({col},{row})[/] with ratio [yellow]{ratio}[/]");
                    }
                    else if (gearParts.Count < 2)
                    {
                        console.MarkupLineInterpolated($"[red]Gear at ({col},{row})[/] has less than 2 parts");
                    }
                    else
                    {
                        console.MarkupLineInterpolated($"[red]Gear at ({col},{row})[/] has more than 2 parts");
                    }
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
            for (int row = 0; row < input.Height; row++)
            {
                for (int col = 0; col < input.Width; col++)
                {
                    if (!IsGearSymbol(input, col, row)) continue;
                    var gearParts = GetGearParts(input, col, row);
                    if (gearParts.Count == 2)
                    {
                        var ratio = gearParts.Select(p => p.Value).Aggregate(1, (a, b) => a * b);
                        result += ratio;
                    }
                }
            }

            return new Output(result);
        }
    }
}
