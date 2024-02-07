using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem1
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem1",
        "Multiples of 3 and 5",
        GetInputParsers,
        GetSolvers,
        """
        If we list all the natural numbers below 10 that are multiples of 3 or 5,
        we get 3, 5, 6, and 9. 
        The sum of these multiples is 23. Find the sum of all the multiples of 
        3 or 5 below 1000.
        """,
        new Uri("https://projecteuler.net/problem=1")
    ));

    public static IProblem Instance => instance.Value;

    private static IEnumerable<IProblemInputParser> GetInputParsers()
    {
        yield return new PromptInputParser();
        yield return new StringInputParser();
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Solver();
        yield return new NaiveSolver();
    }

    public record Input(int Below, int Prime1, int Prime2) : IProblemInput;

    [GeneratedRegex(@"^\s*((?<below>\d+)(\s+(?<prime1>\d+)\s+(?<prime2>\d+))?\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [below=1000][prime1=3][prime2=5]. E.g. 1000, 10000 5 11"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not {Success: true} match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (match.Groups["below"] is not { Success: true, Value: { } belowValue })
                return new Input(1000, 3, 5);

            if (!int.TryParse(belowValue, out var below))
            {
                console.MarkupLine("[red]Below value is not a valid integer.[/]");
                return null;
            }

            if (below < 1)
            {
                console.MarkupLine("[red]Below must be a positive integer.[/]");
                return null;
            }

            if (match.Groups["prime1"] is not { Success: true, Value: { } prime1Value })
                return new Input(below, 3, 5);

            if (!int.TryParse(prime1Value, out var prime1))
            {
                console.MarkupLine("[red]Prime1 value is not a valid integer.[/]");
                return null;
            }

            if (match.Groups["prime2"] is not { Success: true, Value: { } prime2Value })
            {
                if (!Primes.AreRelativePrimes(prime1, 5))
                {
                    console.MarkupLine("[red]{0} and 5 are not relative primes.[/]", prime1);
                    return null;
                }

                return new Input(below, prime1, 5);
            }

            if (!int.TryParse(prime2Value, out var prime2))
            {
                console.MarkupLine("[red]Prime2 value is not a valid integer.[/]");
                return null;
            }

            if (!Primes.AreRelativePrimes(prime1, prime2))
            {
                console.MarkupLine("[red]{0} and {1} are not relative primes.[/]", prime1, prime2);
                return null;
            }

            return new Input(below, prime1, prime2);
        }
    }

    public class PromptInputParser() :
        ProblemInputParser(
            "prompt",
            "Prompt input",
            "Prompts the user for the below, prime1, and prime2 values."),
        IProblemPromptInputParser
    {
        public IProblemInput? Prompt(IAnsiConsole console)
        {
            var below = AnsiConsole.Prompt(
                new TextPrompt<int>("Add multiples [green]below[/]:")
                    .DefaultValue(1000)
                    .ValidationErrorMessage("Must be an integer.")
                    .Validate(below1 => below1 switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            var prime1 = AnsiConsole.Prompt(
                new TextPrompt<int>("First [green]prime[/]:")
                    .DefaultValue(3)
                    .ValidationErrorMessage("Must be an integer.")
                    .Validate(prime1 => prime1 switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            var prime2 = AnsiConsole.Prompt(
                new TextPrompt<int>("Second [green]prime[/]:")
                    .DefaultValue(5)
                    .ValidationErrorMessage("Must be an integer.")
                    .Validate(prime2 => prime2 switch
                    {
                        < 1 =>
                            ValidationResult.Error("Must be a positive integer."),
                        _ when !Primes.AreRelativePrimes(prime1, prime2) =>
                            ValidationResult.Error("Must be relative primes."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(below, prime1, prime2);
        }
    }

    public record Output(long Amount) : IProblemOutput;

    public class Solver() :
        ProblemSolver(
            "solver",
            "Solver",
            "Calculates the amount of multiples of prime1, prime2 and both to compute the result."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            long count1 = (input.Below - 1) / input.Prime1;
            long count2 = (input.Below - 1) / input.Prime2;
            long countBoth = (input.Below - 1) / (input.Prime1 * input.Prime2);

            var amount1 = input.Prime1 * count1 * (count1 + 1) / 2;
            var amount2 = input.Prime2 * count2 * (count2 + 1) / 2;
            var amountBoth = input.Prime1 * input.Prime2 * countBoth * (countBoth + 1) / 2;

            var amount = (amount1 - amountBoth) + amount2;
            return new Output(amount);
        }
    }

    public class NaiveSolver() :
        ProblemSolver(
            "naive",
            "Naive solver",
            "Counts how many numbers from 1 to below are multiples of prime1 or prime2."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            long amount = 0;
            for (var i = 1; i < input.Below; i++)
            {
                if (i % input.Prime1 == 0 || i % input.Prime2 == 0)
                {
                    amount += i;
                }
            }

            return new Output(amount);
        }
    }
}
