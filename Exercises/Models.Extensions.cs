using Spectre.Console;
using static Exercises.ProblemRunner;
using System;

namespace Exercises;

public static class ModelsExtensions
{
    private static TItem? PrompForSelectableItem<TItem>(
        this IReadOnlyCollection<TItem> allItems,
        IAnsiConsole console,
        string? preselectedKeyword,
        string singular,
        string plural,
        TItem? defaultItem = null)
        where TItem : class, IDescribable, IHasKeyword
    {
        if (allItems.Count == 0)
        {
            console.MarkupLineInterpolated($"[red]No {plural} available[/]");
            return null;
        }

        var item = (preselectedKeyword != null
            ? allItems.FirstOrDefault(e => e.Keyword == preselectedKeyword)
            : null) ?? defaultItem;

        while (item is null)
        {
            if (allItems.Count == 1)
            {
                return allItems.First();
            }

            var selection = console.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Select a [white]{singular}[/]?")
                    .PageSize(10)
                    .MoreChoicesText($"[grey](Move up and down to reveal more {plural})[/]")
                    .AddChoices(allItems.Select(e => $"{e.Keyword} - {e.Title}")));

            item = allItems.FirstOrDefault(e => selection.StartsWith($"{e.Keyword} - "));

            if (item is null)
            {
                console.MarkupLine($"[red]Invalid {singular}[/]");
                item = null;
            }
            else
            {
                break;
            }
        }

        return item;
    }

    private static TItem[] PrompForSelectableItems<TItem>(
        this IReadOnlyCollection<TItem> allItems,
        IAnsiConsole console,
        string[] preselectedKeywords,
        string plural)
        where TItem : class, IDescribable, IHasKeyword
    {
        if (allItems.Count == 0)
        {
            console.MarkupLineInterpolated($"[red]No {plural} available[/]");
            return [];
        }

        if (preselectedKeywords is ["*"])
        {
            return allItems.ToArray();
        }

        var resultItems = preselectedKeywords.Length > 0
            ? preselectedKeywords
                .Select(e => allItems.FirstOrDefault(i => i.Keyword == e))
                .Where(e => e is not null)
                .Select(e => e!)
                .ToArray()
            : [];

        while (resultItems!.Length == 0)
        {
            if (allItems.Count == 1)
            {
                return [allItems.First()];
            }

            var selection = console.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"Select [white]{plural}[/]?")
                    .PageSize(10)
                    .MoreChoicesText($"[grey](Move up and down to reveal more {plural})[/]")
                    .AddChoices(allItems.Select(e => $"{e.Keyword} - {e.Title}")));

            resultItems = selection
                .Select(e => allItems.FirstOrDefault(i => e.StartsWith($"{i.Keyword} - ")))
                .Where(e => e is not null)
                .Select(e => e!)
                .ToArray();

            if (resultItems.Length == 0)
            {
                console.MarkupLine($"[red]Invalid {plural}[/]");
                resultItems = null;
            }
            else
            {
                break;
            }
        }

        return resultItems;
    }

    public static IProblemSource? GetProblemSource(
        this IProblemSourceProvider provider,
        IAnsiConsole console,
        string? preselectedSource)
    {
        var sources = provider.GetSources().ToArray();

        return sources.PrompForSelectableItem(console, preselectedSource, "source", "sources");
    }

    public static IProblemCategory? GetProblemCategory(
        this IProblemSource source,
        IAnsiConsole console,
        string? preselectedCategory)
    {
        var categories = source.GetCategories().ToArray();

        return categories.PrompForSelectableItem(console, preselectedCategory, "category", "categories");
    }

    public static IProblemCategory? GetProblemCategory(
        this IProblemSourceProvider provider,
        IAnsiConsole console,
        string? preselectedSource,
        string? preselectedCategory)
    {
        return provider
            .GetProblemSource(console, preselectedSource)?
            .GetProblemCategory(console, preselectedCategory);
    }

    public static IProblem? GetProblem(
        this IProblemCategory category,
        IAnsiConsole console,
        string? preselectedProblem)
    {
        var problems = category.GetProblems().ToArray();

        return problems.PrompForSelectableItem(console, preselectedProblem, "problem", "problems");
    }

    public static IProblem? GetProblem(
        this IProblemSourceProvider provider,
        IAnsiConsole console,
        string? preselectedSource,
        string? preselectedCategory,
        string? preselectedProblem)
    {
        return provider
            .GetProblemSource(console, preselectedSource)?
            .GetProblemCategory(console, preselectedCategory)?
            .GetProblem(console, preselectedProblem);
    }

    public static IProblemInputParser? GetProblemParser(
        this IProblem problem,
        IAnsiConsole console,
        string? preselectedParser)
    {
        var parsers = problem.GetInputParsers().ToArray();

        return parsers.PrompForSelectableItem(console, preselectedParser, "parser", "parsers", problem.DefaultInputParser);
    }

    public static IProblemInput? GetProblemInput(
        this IProblemInputParser parser,
        IProblem problem,
        IAnsiConsole console,
        string? preselectedInput)
    {
        switch (parser)
        {
            case IProblemStringInputParser stringParser:
                return stringParser.GetProblemInput(problem, console, preselectedInput);

            case IProblemPromptInputParser promptParser:
                return promptParser.GetProblemInput(problem, console, preselectedInput);

            default:
                console.MarkupLineInterpolated($"[red]Invalid parser[/]: {parser.GetType().FullName}");
                return null;
        }
    }

    public static IProblemInput? GetProblemInput(
        this IProblem problem,
        IAnsiConsole console,
        string? preselectedParser,
        string? preselectedInput)
    {
        return problem
            .GetProblemParser(console, preselectedParser)?
            .GetProblemInput(problem, console, preselectedInput);
    }

    public static IProblemInput? GetProblemInput(
        this IProblemStringInputParser parser,
        IProblem problem,
        IAnsiConsole console,
        string? preselectedInput)
    {
        var inputSources = problem.GetInputSources().ToArray();

        var input = preselectedInput != null
            ? ParseInput(preselectedInput, false)
            : null;

        while (input is null)
        {
            if (inputSources.Length == 0)
            {
                var inputString = console.Prompt(
                    new TextPrompt<string>("Input:"));

                input = ParseInput(inputString, false);
            }
            else
            {
                IEnumerable<string> options = [
                    .. inputSources.Select(e => $"{e.Keyword} - {e.Title}"),
                    "Custom input",
                ];

                var selection = console.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an input source:")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more input sources)[/]")
                        .AddChoices(options));

                if (selection == "Custom input")
                {
                    var inputString = console.Prompt(new TextPrompt<string>("Input:"));

                    input = ParseInput(inputString, false);
                }
                else
                {
                    input = ParseInput(selection, true);
                }
            }
        }

        return input;

        IProblemInput? ParseInput(string text, bool withPrefix)
        {
            var inputSource = withPrefix
                ? inputSources.FirstOrDefault(e => text.StartsWith($"{e.Keyword} - "))
                : inputSources.FirstOrDefault(e => e.Keyword == text);
            if (inputSource != null) return parser.Parse(console, inputSource.GetInput());

            // text could be a text file path or a plain string
            try
            {
                var current = Directory.GetCurrentDirectory();
                var absolute = Path.GetFullPath(text);
                if (!absolute.StartsWith(current))
                {
                    throw new IOException("File is not in the current directory.");
                }
                var fileContent = File.ReadAllText(text);
                return parser.Parse(console, fileContent);
            }
            catch (IOException)
            {
                // not a file
            }

            return parser.Parse(console, text);
        }
    }

    public static IProblemInput? GetProblemInput(
        this IProblemPromptInputParser parser,
        IProblem problem,
        IAnsiConsole console,
        string? preselectedInput)
    {
        return parser.Prompt(console);
    }

    public static IProblemSolver[] GetProblemSolvers(
        this IProblem problem,
        IAnsiConsole console,
        string[] preselectedSolvers)
    {
        if (preselectedSolvers.Length == 0) preselectedSolvers = ["*"];

        return problem
            .GetSolvers()
            .ToArray()
            .PrompForSelectableItems(console, preselectedSolvers, "solvers");
    }

    public static void PrintItem(object item, IAnsiConsole console, PrintSummaryOptions? options = null)
    {
        options ??= new PrintSummaryOptions();

        switch (item)
        {
            case IHasPrintSummary printSummary:
                printSummary.PrintSummary(console, options);
                break;

            default:
                console.Write(options.Indent);
                console.WriteLine($"{item}");
                break;
        }
    }

    public static void PrintTo(this IProblemInput item, IAnsiConsole console) => PrintItem(item, console);
    public static void PrintTo(this IProblemOutput item, IAnsiConsole console) => PrintItem(item, console);
    public static void PrintTo(this IHasPrintSummary item, IAnsiConsole console) => PrintItem(item, console);

    public static string ToPreciseString(this TimeSpan span)
    {
        if (span == TimeSpan.Zero) return "0 ms";
        if (span.TotalNanoseconds < 1) return "<1 ns";
        if (span.TotalMicroseconds < 1) return $"{span.TotalNanoseconds:0.000} ns";
        if (span.TotalMilliseconds < 1) return $"{span.TotalMicroseconds:0.000} µs";
        if (span.TotalSeconds < 1) return $"{span.TotalMilliseconds:0.000} ms";
        if (span.TotalMinutes < 1) return $"{span.TotalSeconds:0.000} s";
        if (span.TotalHours < 1) return $"{span:mm:ss.fff}";
        return $"{span:hh\\:mm\\:ss}";
    }
}
