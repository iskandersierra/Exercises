using Spectre.Console;

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
    public virtual void PrintSummary(IAnsiConsole console, string indent = "")
    {
        console.Write(indent);
        console.Write(new Markup(Title, new Style(Color.Black, Color.Aqua)));
        console.WriteLine();

        if (!string.IsNullOrWhiteSpace(Summary))
        {
            console.Write(indent);
            console.Write(new Markup(Summary, new Style(Color.Grey)));
            console.WriteLine();
        }

        if (MainLink != null)
        {
            console.Write(indent);
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

    public override void PrintSummary(IAnsiConsole console, string indent = "")
    {
        console.Write(indent);
        console.Write(new Markup(Keyword, new Style(Color.Green)));
        console.Write(": ");
        console.Write(new Markup(Title, new Style(Color.Black, Color.Aqua)));
        console.WriteLine();

        if (!string.IsNullOrWhiteSpace(Summary))
        {
            console.Write(indent);
            console.Write(new Markup(Summary, new Style(Color.Grey)));
            console.WriteLine();
        }

        if (MainLink != null)
        {
            console.Write(indent);
            console.Write(new Markup(MainLink.ToString(), new Style(Color.Aqua, decoration: Decoration.Underline)));
            console.WriteLine();
        }
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
    Func<IEnumerable<IProblemSolver>> getSolvers,
    string? summary = null,
    Uri? mainLink = null) :
    DescribableWithKeyword(keyword, title, summary, mainLink),
    IProblem
{
    private readonly Lazy<IEnumerable<IProblemInputParser>> inputParsers = new(getInputParsers);
    private readonly Lazy<IEnumerable<IProblemSolver>> solvers = new(getSolvers);

    public IEnumerable<IProblemInputParser> GetInputParsers() => inputParsers.Value;
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
