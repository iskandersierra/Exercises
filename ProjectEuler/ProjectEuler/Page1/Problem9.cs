using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem9
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem9",
        "Special Pythagorean triplet",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        A Pythagorean triplet is a set of three natural numbers, a < b < c, for which,
        a^2 + b^2 = c^2
        For example, 3^2 + 4^2 = 9 + 16 = 25 = 5^2.
        There exists exactly one Pythagorean triplet for which a + b + c = 1000.
        Find the product abc.
        """,
        new Uri("https://projecteuler.net/problem=9")
    ));

    public static IProblem Instance => instance.Value;

    private static IEnumerable<IProblemInputParser> GetInputParsers()
    {
        yield return new StringInputParser();
        yield return new PromptInputParser();
    }

    private static IEnumerable<IProblemInputSource> GetInputSources()
    {
        yield return new ProblemInputStringSource(
            "sample",
            "Pythagorean triplet for which a + b + c = 12",
            "12",
            expectedOutput: new Output(60, 3, 4, 5));

        yield return new ProblemInputStringSource(
            "question",
            "Pythagorean triplet for which a + b + c = 1000",
            "1000",
            expectedOutput: new Output(31875000, 200, 375, 425));
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Solver();
    }

    public record Input(int Number) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Find the Pythagorean triplet for which [green]a + b + c = {Number}[/].");
        }
    }

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=6]. E.g. 6, 10001"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not {Success: true} match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (match.Groups["num"] is not { Success: true, Value: { } numberValue })
                return new Input(12);

            if (!int.TryParse(numberValue, out var number))
            {
                console.MarkupLine("[red]Must be a valid integer.[/]");
                return null;
            }

            if (number < 12)
            {
                console.MarkupLine("[red]Must be at least 12.[/]");
                return null;
            }

            return new Input(number);
        }
    }

    public class PromptInputParser() :
        ProblemInputParser(
            "prompt",
            "Prompt input",
            "Prompts the user for the number."),
        IProblemPromptInputParser
    {
        public IProblemInput? Prompt(IAnsiConsole console)
        {
            var maxValue = console.Prompt(
                new TextPrompt<int>("Number:")
                    .DefaultValue(12)
                    .ValidationErrorMessage("Must be a valid integer.")
                    .Validate(max => max switch
                    {
                        < 12 => ValidationResult.Error("Must be at least 12."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(int Product, int A, int B, int C) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The Pythagorean triplet is [green]{A}, {B}, {C}[/] with a product of [green]{Product}[/].");
        }
    }

    public class Solver() :
        ProblemSolver(
            "solver",
            "Solver",
            "Solves the problem."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            for (var a = 1; a < input.Number; a++)
            {
                for (var b = a + 1; b < input.Number; b++)
                {
                    var c = input.Number - a - b;
                    if (a * a + b * b == c * c)
                    {
                        return new Output(a * b * c, a, b, c);
                    }
                }
            }

            return new Output(0, 0, 0, 0);
        }
    }
}
