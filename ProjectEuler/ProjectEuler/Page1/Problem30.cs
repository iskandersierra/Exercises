using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem30
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem30",
        "Digit fifth powers",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        Surprisingly there are only three numbers that can be written as the sum of fourth powers of their digits:
        1634 = 1^4 + 6^4 + 3^4 + 4^4
        8208 = 8^4 + 2^4 + 0^4 + 8^4
        9474 = 9^4 + 4^4 + 7^4 + 4^4
        As 1 = 1^4 is not a sum it is not included.
        The sum of these numbers is 1634 + 8208 + 9474 = 19316.
        Find the sum of all the numbers that can be written as the sum of fifth powers of their digits.
        """,
        new Uri("https://projecteuler.net/problem=30")
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
            "Numbers that can be written as the sum of fourth powers of their digits",
            "4",
            expectedOutput: new Output(27));

        yield return new ProblemInputStringSource(
            "question",
            "Numbers that can be written as the sum of fifth powers of their digits",
            "5",
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
            console.MarkupLineInterpolated($"{options.Indent}> Find the sum of all the numbers that can be written as the sum of [green]{Number}th[/] powers of their digits.");
        }
    }

    [GeneratedRegex(@"^\s*((?<num>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [num=4]. E.g. 4, 5"),
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
                return new Input(4);

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
                    .DefaultValue(4)
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
            console.MarkupLineInterpolated($"{options.Indent}> The sum is [green]{Sum}[/].");
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

            var pows = Enumerable.Range(0, 10).Select(e => (int) Math.Pow(e, input.Number)).ToArray();
            var minNum = 10;
            var maxNum = pows[9] * input.Number;
            var sum = 0;

            for (var i = minNum; i <= maxNum; i++)
            {
                var num = i;
                var total = 0;

                while (num > 0)
                {
                    var q = Math.DivRem(num, 10, out var r);
                    total += pows[r];
                    num = q;
                }

                if (total == i)
                    sum += i;
            }

            return new Output(sum);
        }
    }
}
