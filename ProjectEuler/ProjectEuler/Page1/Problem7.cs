using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem7
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem7",
        "10001st prime",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        By listing the first six prime numbers: 2, 3, 5, 7, 11, and 13, we can see that the 6th 
        prime is 13.
        What is the 10 001st prime number?
        """,
        new Uri("https://projecteuler.net/problem=7")
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
            "Prime number 6th",
            "6",
            expectedOutput: new Output(13));

        yield return new ProblemInputStringSource(
            "question",
            "Prime number 10001st",
            "10001",
            expectedOutput: new Output(104743));
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
            console.MarkupLineInterpolated($"{options.Indent}> Find the [green]{Number}[/]st prime number.");
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
                return new Input(6);

            if (!int.TryParse(numberValue, out var number))
            {
                console.MarkupLine("[red]Must be a valid integer.[/]");
                return null;
            }

            if (number < 1)
            {
                console.MarkupLine("[red]Must be a positive integer.[/]");
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
                    .DefaultValue(6)
                    .ValidationErrorMessage("Must be a valid integer.")
                    .Validate(max => max switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(long Result) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The prime is [green]{Result}[/].");
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

            var nthPrime = Numbers.GetPrimes().Skip(input.Number - 1).First();
            return new Output(nthPrime);
        }
    }
}
