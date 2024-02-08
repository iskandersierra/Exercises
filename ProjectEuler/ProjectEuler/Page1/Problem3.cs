using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem3
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem3",
        "Largest Prime Factor",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        The prime factors of 13195 are 5, 7, 13 and 29.
        What is the largest prime factor of the number 600851475143?
        """,
        new Uri("https://projecteuler.net/problem=3")
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
            "Largest prime factor of 13195",
            "13195",
            expectedOutput: new Output(29));

        yield return new ProblemInputStringSource(
            "question",
            "Largest prime factor of 600851475143",
            "600851475143",
            expectedOutput: new Output(6857));
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Solver();
    }

    public record Input(long Number) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Find the largest prime factor of the number [green]{Number}[/].");
        }
    }

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=13195]. E.g. 1024, 13195"),
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
                return new Input(13195);

            if (!long.TryParse(numberValue, out var number))
            {
                console.MarkupLine("[red]Number is not a valid integer.[/]");
                return null;
            }

            if (number < 1)
            {
                console.MarkupLine("[red]Number must be a positive integer.[/]");
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
                new TextPrompt<long>("Number:")
                    .DefaultValue(13195)
                    .ValidationErrorMessage("Must be an integer.")
                    .Validate(max => max switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(int LargestPrime) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The largest prime factor is [green]{LargestPrime}[/].");
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

            var largestPrime = Numbers.GetPrimeFactors(input.Number).Last();

            return new Output(largestPrime);
        }
    }
}
