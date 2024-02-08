using Exercises;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace ProjectEuler.Page1;

public static partial class Problem8
{
    private static readonly Lazy<IProblem> instance = new(() => new Problem(
        "problem8",
        "Largest product in a series",
        GetInputParsers,
        GetInputSources,
        GetSolvers,
        """
        The four adjacent digits in the 1000-digit number that have the greatest
        product are 9 × 9 × 8 × 9 = 5832.
        7316717... (see problem page for full number)
        Find the thirteen adjacent digits in the 1000-digit number that have the
        greatest product. What is the value of this product?
        """,
        new Uri("https://projecteuler.net/problem=8")
    ));

    public static IProblem Instance => instance.Value;

    private static IEnumerable<IProblemInputParser> GetInputParsers()
    {
        yield return new StringInputParser();
        yield return new PromptInputParser();
    }

    private static IEnumerable<IProblemInputSource> GetInputSources()
    {
        yield return new ProblemInputEmbeddedSource(
            "sample",
            "4 adjacent digits",
            "ProjectEuler.Page1.Problem8.sample.txt",
            expectedOutput: new Output(5832, [9,9,8,9]));

        yield return new ProblemInputEmbeddedSource(
            "question",
            "13 adjacent digits",
            "ProjectEuler.Page1.Problem8.question.txt",
            expectedOutput: new Output(23514624000, [1,2,3,4]));
    }

    private static IEnumerable<IProblemSolver> GetSolvers()
    {
        yield return new Solver();
    }

    public record Input(int Length, string Digits) : IProblemInput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            console.MarkupLineInterpolated($"{options.Indent}> Find the [green]{Length}[/] adjacent digits in the 1000-digit number that have the greatest product.");
        }
    }

    [GeneratedRegex(@"^(?<length>\d+)\s+(?<digits>[\d\s]+)$")]
    private static partial Regex GetInputRegex();

    [GeneratedRegex(@"\s")]
    private static partial Regex GetDigitsSpacesRegex();

    public class StringInputParser() :
        ProblemInputParser(
            "string",
            "String input",
            "Parses a string in the format: <length> <digits>"),
        IProblemStringInputParser
    {
        public IProblemInput? Parse(IAnsiConsole console, string input)
        {
            if (GetInputRegex().Match(input) is not {Success: true} match)
            {
                console.MarkupLine("[red]Input is in an invalid format.[/]");
                return null;
            }

            if (!int.TryParse(match.Groups["length"].Value, out var length))
            {
                console.MarkupLine("[red]Must be a valid integer.[/]");
                return null;
            }

            if (length is < 1 or > 16)
            {
                console.MarkupLine("[red]Must be a positive integer less than or equal to 16.[/]");
                return null;
            }

            var digits = GetDigitsSpacesRegex().Replace(match.Groups["digits"].Value, string.Empty);

            return new Input(length, digits);
        }
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex GetDigitsRegex();

    public class PromptInputParser() :
        ProblemInputParser(
            "prompt",
            "Prompt input",
            "Prompts the user for the number."),
        IProblemPromptInputParser
    {
        public IProblemInput? Prompt(IAnsiConsole console)
        {
            var length = console.Prompt(
                new TextPrompt<int>("Length:")
                    .DefaultValue(4)
                    .ValidationErrorMessage("Must be a valid integer.")
                    .Validate(max => max switch
                    {
                        < 1 or > 16 => ValidationResult.Error("Must be a positive integer less than or equal to 16."),
                        _ => ValidationResult.Success()
                    }));

            var digits = console.Prompt(
                new TextPrompt<string>("Digits:")
                    .Validate(d => GetDigitsRegex().Match(d) is { Success: true } match
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Must be a valid string of digits.")));

            return new Input(length, digits);
        }
    }

    public record Output(long Product, byte[] Digits) : IProblemOutput, IHasPrintSummary
    {
        public void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
        {
            options ??= new PrintSummaryOptions();
            var digits = string.Join('\u2a2f', Digits.Select(d => d.ToString()));
            console.MarkupLineInterpolated($"{options.Indent}> The greatest product is [green]{Product}[/] = {digits}.");
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

            var digits = new byte[input.Digits.Length];
            for (var i = 0; i < input.Digits.Length; i++)
            {
                digits[i] = (byte)(input.Digits[i] - '0');
            }

            long greatestProduct = 0;
            int bestPos = 0;

            for (var i = 0; i < digits.Length - input.Length; i++)
            {
                long product = 1;
                for (var j = 0; j < input.Length; j++)
                {
                    product *= digits[i + j];
                }

                if (product > greatestProduct)
                {
                    greatestProduct = product;
                    bestPos = i;
                }
            }

            var bestDigits = digits.Skip(bestPos).Take(input.Length).ToArray();

            return new Output(greatestProduct, bestDigits);
        }
    }
}
