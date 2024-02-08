using Exercises;
using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem4
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem4",
        "Largest Palindrome Product",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        A palindromic number reads the same both ways. The largest palindrome made
        from the product of two 2-digit numbers is 9009 = 91 * 99.
        Find the largest palindrome made from the product of two 3-digit numbers.
        """,
        new Uri("https://projecteuler.net/problem=4")
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
            "Largest palindrome made from the product of two 2-digit numbers",
            "2",
            expectedOutput: new Output(9009, 91, 99));

        yield return new ProblemInputStringSource(
            "question",
            "Largest palindrome made from the product of two 3-digit numbers",
            "3",
            expectedOutput: new Output(906609, 913, 993));
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Solver();
    }

    public record Input(int Digits) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Find the largest palindrome made from the product of two [green]{Digits}-digit[/] numbers.");
        }
    }

    [GeneratedRegex(@"^\s*((?<digits>\d+)\s*)?$")]
    private static partial Regex GetInputRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: [digits=2]. E.g. 2, 3"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not { Success: true } match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (match.Groups["digits"] is not { Success: true, Value: { } digitsValue })
                return new Input(2);

            if (!int.TryParse(digitsValue, out var digits))
            {
                console.MarkupLine("[red]Must be a valid integer.[/]");
                return null;
            }

            if (digits < 1 || digits > 9)
            {
                console.MarkupLine("[red]Must be a positive integer between 1 and 9.[/]");
                return null;
            }

            return new Input(digits);
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
                new TextPrompt<int>("Digits (1-9):")
                    .DefaultValue(1234)
                    .ValidationErrorMessage("Must be a valid integer.")
                    .Validate(max => max switch
                    {
                        < 1 or > 9 => ValidationResult.Error("Must be a positive integer between 1 and 9."),
                        _ => ValidationResult.Success()
                    }));

            return new Input(maxValue);
        }
    }

    public record Output(long LargestPalindrome, int Number1, int Number2) : IProblemOutput, IHasPrintSummary
    {
        public virtual bool Equals(Output? other)
        {
            if (other is null) return false;
            if (this == other) return true;

            if (LargestPalindrome != other.LargestPalindrome) return false;

            if (Number1 == other.Number1 && Number2 == other.Number2) return true;
            if (Number1 == other.Number2 && Number2 == other.Number1) return true;

            return false;
        }

        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> The largest palindrome is [green]{LargestPalindrome}[/] which is the product of [green]{Number1}[/] and [green]{Number2}[/].");
        }
    }

    public class Solver() :
        ProblemSolver(
            "solver",
            "Solver",
            "Solves the problem using a naive algorithm."),
        IProblemOutputSolver
    {
        public IProblemOutput Solve(IProblemInput problemInput)
        {
            var input = (Input)problemInput;

            var maxNumber = (int)Math.Pow(10, input.Digits) - 1;
            var minNumber = (int)Math.Pow(10, input.Digits - 1);
            var largest = 0L;
            int largestNumber1 = 0, largestNumber2 = 0;

            for (int i = minNumber; i <= maxNumber; i++)
            {
                for (int j = i; j <= maxNumber; j++)
                {
                    var product = (long)i * j;

                    if (product <= largest)
                        continue;

                    var text = product.ToString(CultureInfo.InvariantCulture);

                    if (Strings.IsPalindrome(text))
                    {
                        largest = product;
                        largestNumber1 = i;
                        largestNumber2 = j;
                    }
                }
            }

            return new Output(largest, largestNumber1, largestNumber2);
        }
    }
}