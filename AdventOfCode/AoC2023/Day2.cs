using System.Text;
using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Exercises.ProblemRunner;
using System;

namespace AoC2023;

public static partial class Day2
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "day2",
        "Cube Conundrum",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        mainLink: new Uri("https://adventofcode.com/2023/day/2")
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
            "AoC2023.Day2.sample1.txt");

        yield return new ProblemInputEmbeddedSource(
            "question",
            "Main question",
            "AoC2023.Day2.question.txt");
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Step1Solver();
        yield return new Step1ShowSolver();
        yield return new Step2Solver();
        yield return new Step2ShowSolver();
    }

    public record InputCubesShow(int Red, int Green, int Blue)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Red > 0) sb.Append($" {Red}R");
            if (Green > 0) sb.Append($" {Green}G");
            if (Blue > 0) sb.Append($" {Blue}B");

            return sb.ToString();
        }
    }

    public record InputGame(int Number, InputCubesShow[] Shows)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"Game {Number,3}:");
            foreach (var show in Shows)
            {
                sb.Append($"{show};");
            }
        
            return sb.ToString();
        }
    }

    public record Input(InputGame[] Games) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Solve for [green]{Games.Length}[/] games.");
        }
    }

    [GeneratedRegex(@"^Game (?<game>\d+):(?<data>.+)$")]
    private static partial Regex GetGameRegex();

    [GeneratedRegex(@"(?<num>\d+)\s(?<color>\w+)")]
    private static partial Regex GetCubeCountRegex();

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
            var games = new List<InputGame>();

            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var game = ParseGame(line);
                games.Add(game);
            }

            return new Input(games.ToArray());

            static InputGame ParseGame(string line)
            {
                if (GetGameRegex().Match(line) is not { Success: true } match)
                    throw new InvalidOperationException($"Invalid input: {line}");

                var gameNumber = int.Parse(match.Groups["game"].Value);
                var data = match.Groups["data"].Value;
                var shows = data.Split(';').Select(ParseShow).ToArray();

                return new InputGame(gameNumber, shows);
            }

            static InputCubesShow ParseShow(string line)
            {
                return line.Split(',')
                    .Select(s => GetCubeCountRegex().Match(s))
                    .Aggregate(
                        new InputCubesShow(0, 0, 0),
                        (show, match) =>
                        {
                            var num = int.Parse(match.Groups["num"].Value);
                            switch (match.Groups["color"].Value)
                            {
                                case "red":
                                    return show with { Red = num };
                                case "green":
                                    return show with { Green = num };
                                case "blue":
                                    return show with { Blue = num };
                                default:
                                    throw new InvalidOperationException($"Invalid input: {line}");
                            }
                        });
            }
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

    static bool GameFitsIn(InputCubesShow show, InputCubesShow config)
    {
        return show.Red <= config.Red && show.Green <= config.Green && show.Blue <= config.Blue;
    }

    static int GamePower(InputGame game)
    {
        var minRed = game.Shows.Max(show => show.Red);
        var minGreen = game.Shows.Max(show => show.Green);
        var minBlue = game.Shows.Max(show => show.Blue);
        return minRed * minGreen * minBlue;
    }

    public class Step1ShowSolver() :
        ProblemSolver("step1show", "Solver for step 1"),
        IProblemConsoleSolver
    {
        public void Solve(IProblemInput problemInput, IAnsiConsole console)
        {
            var input = (Input)problemInput;
            var config = new InputCubesShow(12, 13, 14);

            var result = 0;
            foreach (var game in input.Games)
            {
                var fits = game.Shows.All(show => GameFitsIn(show, config));
                if (fits)
                {
                    console.MarkupLineInterpolated($"[green]{game}[/]");
                    result += game.Number;
                }
                else
                {
                    console.MarkupLineInterpolated($"[grey]{game}[/]");
                }
            }

            console.MarkupLineInterpolated($"[green]Result:[/] {result}");
        }
    }

    public class Step1Solver() :
        ProblemSolver("step1", "Solver for step 1"),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;
            var config = new InputCubesShow(12, 13, 14);

            var result = input.Games
                .Where(game => game.Shows
                    .All(show => GameFitsIn(show, config)))
                .Sum(game => game.Number);

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
            foreach (var game in input.Games)
            {
                var power = GamePower(game);
                console.MarkupLineInterpolated($"{game} has power [green]{power}[/]");
                result += power;
            }

            console.MarkupLineInterpolated($"[green]Result:[/] {result}");
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
            foreach (var game in input.Games)
            {
                var power = GamePower(game);
                result += power;
            }

            return new Output(result);
        }
    }
}
