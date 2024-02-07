using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem5
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem5",
        "Smallest Multiple",
        GetInputParsers,
        GetSolvers,
        """
        2520 is the smallest number that can be divided by each of the numbers from 1 to 10 without any remainder.
        What is the smallest positive number that is evenly divisible by all of the numbers from 1 to 20?
        """,
        new Uri("https://projecteuler.net/problem=5")
    ));

    public static IProblem Instance => instance.Value;

    private static IEnumerable<IProblemInputParser> GetInputParsers()
    {
        yield return new PromptInputParser();
        yield return new StringInputParser();
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new NaiveSolver();
    }

    public record Input(int MaxValue) : IProblemInput;

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=10]. E.g. 10, 20"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not { Success: true } match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (match.Groups["num"] is not { Success: true, Value: { } numberValue })
                return new Input(10);

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
                    .DefaultValue(10)
                    .ValidationErrorMessage("Must be a valid integer.")
                    .Validate(max => max switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(int LargestPrime) : IProblemOutput;

    public class NaiveSolver() :
        ProblemSolver(
            "naive",
            "Naive solver",
            "Solves the problem using a naive algorithm."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var minNumber = 2;
            var maxNumber = input.MaxValue;

            var factors = Enumerable
                .Range(minNumber, maxNumber - minNumber + 1)
                .SelectMany(x => Primes
                    .GetPrimeFactors(x)
                    .GroupBy(f => f)
                    .Select(g => (Prime: g.Key, Power: g.Count())))
                .GroupBy(x => x.Prime)
                .Select(g => (Prime: g.Key, Power: g.Select(e => e.Power).Max()))
                .ToArray();

            var result = factors
                .Aggregate(1L, (acc, x) => acc * (long)Math.Pow(x.Prime, x.Power));

            return new Output((int)result);
        }
    }
}