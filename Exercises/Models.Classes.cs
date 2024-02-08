using Spectre.Console;
using System;
using System.Reflection;

namespace Exercises;

public abstract class Describable(
    string title,
    string? summary = null,
    Uri? mainLink = null) :
    IDescribable,
    IHasPrintSummary
{
    public string Title { get; } = title;
    public string? Summary { get; } = summary;
    public Uri? MainLink { get; } = mainLink;

    public virtual void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null)
    {
        options ??= PrintSummaryOptions.Default;

        PrintTitleComponent(console, options);
        PrintSummaryComponent(console, options);
        PrintLinkComponent(console, options);
    }

    protected virtual void PrintTitleComponent(IAnsiConsole console, PrintSummaryOptions options)
    {
        console.Write(options.Indent);
        console.Write(new Markup(Title, new Style(Color.Black, Color.Aqua)));
        console.WriteLine();
    }

    protected virtual void PrintSummaryComponent(IAnsiConsole console, PrintSummaryOptions options)
    {
        if (options.Summary && !string.IsNullOrWhiteSpace(Summary))
        {
            console.Write(options.Indent);
            console.Write(new Markup(Summary, new Style(Color.Grey)));
            console.WriteLine();
        }
    }

    protected virtual void PrintLinkComponent(IAnsiConsole console, PrintSummaryOptions options)
    {
        if (options.Link && MainLink != null)
        {
            console.Write(options.Indent);
            console.Write(new Markup(MainLink.ToString(), new Style(Color.Aqua, decoration: Decoration.Underline)));
            console.WriteLine();
        }
    }
}

public abstract class DescribableWithKeyword(
    string keyword,
    string title,
    string? summary = null,
    Uri? mainLink = null) :
    Describable(title, summary, mainLink),
    IHasKeyword
{
    public string Keyword { get; } = keyword;

    protected override void PrintTitleComponent(IAnsiConsole console, PrintSummaryOptions options)
    {
        console.Write(options.Indent);
        console.Write(new Markup(Keyword, new Style(Color.Green)));
        console.Write(": ");
        console.Write(new Markup(Title, new Style(Color.Black, Color.Aqua)));
        console.WriteLine();
    }
}


public class ProblemSource(
    string keyword,
    string title,
    Func<IEnumerable<IProblemCategory>> getCategories,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemSource
{
    private readonly Lazy<IEnumerable<IProblemCategory>> categories = new(getCategories);

    public IEnumerable<IProblemCategory> GetCategories() => categories.Value;
}

public class ProblemCategory(
    string keyword,
    string title,
    Func<IEnumerable<IProblem>> getProblems,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemCategory
{
    private readonly Lazy<IEnumerable<IProblem>> problems = new(getProblems);

    public IEnumerable<IProblem> GetProblems() => problems.Value;
}

public class Problem(
    string keyword,
    string title,
    Func<IEnumerable<IProblemInputParser>> getInputParsers,
    Func<IEnumerable<IProblemInputSource>> getInputSources,
    Func<IEnumerable<IProblemSolver>> getSolvers,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblem
{
    private readonly Lazy<IEnumerable<IProblemInputParser>> inputParsers = new(getInputParsers);
    private readonly Lazy<IEnumerable<IProblemInputSource>> inputSources = new(getInputSources);
    private readonly Lazy<IEnumerable<IProblemSolver>> solvers = new(getSolvers);

    public IProblemInputParser? DefaultInputParser => inputParsers.Value.FirstOrDefault();
    public IProblemInputSource? DefaultInputSource => inputSources.Value.FirstOrDefault();
    public IProblemSolver? DefaultSolver => solvers.Value.FirstOrDefault();

    public IEnumerable<IProblemInputParser> GetInputParsers() => inputParsers.Value;
    public IEnumerable<IProblemInputSource> GetInputSources() => inputSources.Value;
    public IEnumerable<IProblemSolver> GetSolvers() => solvers.Value;
}

public abstract class ProblemInputParser(
    string keyword,
    string title,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemInputParser
{
}

public abstract class ProblemSolver(
    string keyword,
    string title,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemSolver
{
}

public class ProblemInputStringSource(
    string keyword,
    string title,
    string input,
    IProblemOutput? expectedOutput = null,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemInputSource
{
    public string GetInput() => input;

    public IProblemOutput? GetExpectedOutput() => expectedOutput;
}

public class ProblemInputEmbeddedSource(
    string keyword,
    string title,
    string resourceName,
    IProblemOutput? expectedOutput = null,
    Assembly? assembly = null,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblemInputSource
{
    private readonly Assembly assembly = assembly ?? Assembly.GetCallingAssembly();

    public string GetInput()
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public IProblemOutput? GetExpectedOutput() => expectedOutput;
}
