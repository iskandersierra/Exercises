using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem50
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem50",
        "Consecutive Prime Sum",
        GetInputParsers,
        GetSolvers,
        """
        The prime 41, can be written as the sum of six consecutive primes:
            41=2+3+5+7+11+13
        This is the longest sum of consecutive primes that adds to a prime below one-hundred.
        The longest sum of consecutive primes below one-thousand that adds to a prime, contains
        21 terms, and is equal to 953.
        Which prime, below one-million, can be written as the sum of the most consecutive primes?
        """,
        new Uri("https://projecteuler.net/problem=50")
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
    }
    public record Input(int Below) : IProblemInput;

    [GeneratedRegex(@"^\s*((?<below>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [below=100]. E.g. 100, 1000000"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not { Success: true } match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (match.Groups["below"] is not { Success: true, Value: { } belowValue })
                return new Input(4000000);

            if (!int.TryParse(belowValue, out var below))
            {
                console.MarkupLine("[red]Max value is not a valid integer.[/]");
                return null;
            }

            if (below < 1)
            {
                console.MarkupLine("[red]Must be a positive integer.[/]");
                return null;
            }

            return new Input(below);
        }
    }

    public class PromptInputParser() :
        ProblemInputParser(
            "prompt",
            "Prompt input",
            "Prompts the user for the below value."),
        IProblemPromptInputParser
    {
        public IProblemInput? Prompt(IAnsiConsole console)
        {
            var maxValue = console.Prompt(
                new TextPrompt<int>("Below:")
                    .DefaultValue(4000000)
                    .ValidationErrorMessage("Must be an integer.")
                    .Validate(max => max switch
                    {
                        < 1 => ValidationResult.Error("Must be a positive integer."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(int Prime, int Start, int Length) : IProblemOutput;

    public class Solver() :
        ProblemSolver(
            "solver",
            "Solver",
            "From the list of primes between 2 and the input, makes a n^2 loop to find the longest sum of consecutive primes that adds to a prime below the input."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var primes = Primes.GetPrimesBelow(input.Below, inclusive: false);

            var longestStart = 0;
            var longestLength = 1;
            var correspondingPrime = primes[longestStart];

            var primeSet = new HashSet<int>(primes);

            for (int start = 0; start < primes.Count - longestLength; start++)
            {
                for (int length = longestLength + 1; start + length < primes.Count; length++)
                {
                    var sum = 0;
                    for (int i = 0; i < length; i++)
                    {
                        sum += primes[start + i];
                    }
                    if (sum >= input.Below) break;
                    if (!primeSet.Contains(sum)) continue;
                    longestStart = start;
                    longestLength = length;
                    correspondingPrime = sum;
                }
            }

            return new Output(correspondingPrime, longestStart, longestLength);
        }
    }
}
