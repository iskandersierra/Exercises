using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem20
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem20",
        "Factorial digit sum",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        n! means n × (n - 1) × ... × 3 × 2 × 1
        For example, 10! = 10 × 9 × ... × 3 × 2 × 1 = 3628800,
        and the sum of the digits in the number 10! is 3 + 6 + 2 + 8 + 8 + 0 + 0 = 27.
        Find the sum of the digits in the number 100!
        """,
        new Uri("https://projecteuler.net/problem=20")
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
            "Sum of the digits in the number 10!",
            "10",
            expectedOutput: new Output(27));

        yield return new ProblemInputStringSource(
            "question",
            "Sum of the digits in the number 100!",
            "100",
            expectedOutput: new Output(648));
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
            console.MarkupLineInterpolated($"{options.Indent}> Find the sum of the digits in the number [green]{Number}![/].");
        }
    }

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=10]. E.g. 10, 100"),
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
            console.MarkupLineInterpolated($"{options.Indent}> The sum of digits is [green]{Sum}[/].");
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

            var factorial = Numbers.FactorialBig(input.Number);

            var sum = factorial.ToString().Sum(c => c - '0');

            return new Output(sum);
        }
    }
}
