using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem6
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem6",
        "Sum square difference",
        GetInputParsers,
        GetSolvers,
        """
        The sum of the squares of the first ten natural numbers is,
            1^2 + 2^2 + ... + 10^2 = 385
        The square of the sum of the first ten natural numbers is,
            (1 + 2 + ... + 10)^2 = 55^2 = 3025
        Hence the difference between the sum of the squares of the first ten
        natural numbers and the square of the sum is 3025 − 385 = 2640.
        Find the difference between the sum of the squares of the first one
        hundred natural numbers and the square of the sum.
        """,
        new Uri("https://projecteuler.net/problem=6")
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

    public record Input(int Number) : IProblemInput;

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

    public record Output(long Result) : IProblemOutput;

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

            var sum = Enumerable.Range(1, input.Number).Sum();
            var sumSq = sum * sum;
            var squareSums = Enumerable.Range(1, input.Number).Select(x => x * x).Sum();
            var result = sumSq - squareSums;
            return new Output(result);
        }
    }
}
