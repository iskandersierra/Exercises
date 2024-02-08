using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem10
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem10",
        "Summation of primes",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        The sum of the primes below 10 is 2 + 3 + 5 + 7 = 17.
        Find the sum of all the primes below two million.
        """,
        new Uri("https://projecteuler.net/problem=10")
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
            "Sum of the primes below 10",
            "10",
            expectedOutput: new Output(17));

        yield return new ProblemInputStringSource(
            "question",
            "Sum of the primes below two million",
            "2000000",
            expectedOutput: new Output(142913828922L));
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
            console.MarkupLineInterpolated($"{options.Indent}> Find the sum of all the primes below [green]{Number}[/].");
        }
    }

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=10]. E.g. 10, 1000000"),
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

    public record Output(long Sum) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The sum of primes is [green]{Sum}[/].");
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

            var sum = Numbers.GetPrimes().TakeWhile(p => p < input.Number).Sum();

            return new Output(sum);
        }
    }
}
