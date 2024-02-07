using Spectre.Console;

namespace Exercises;

public interface IHasKeyword
{
    string Keyword { get; }
}

public record PrintSummaryOptions(
    bool Summary = false,
    bool Link = false,
    string Indent = "")
{
    public static readonly PrintSummaryOptions Default = new();
}

public interface IHasPrintSummary
{
    void PrintSummary(IAnsiConsole console, PrintSummaryOptions? options = null);
}

public interface IDescribable
{
    string Title { get; }
    string? Summary { get; }
    Uri? MainLink { get; }
}

public interface IProblemSourceProvider
{
    IEnumerable<IProblemSource> GetSources();
}

public interface IProblemSource : IDescribable, IHasKeyword
{
    IEnumerable<IProblemCategory> GetCategories();
}

public interface IProblemCategory : IDescribable, IHasKeyword
{
    IEnumerable<IProblem> GetProblems();
}

public interface IProblem : IDescribable, IHasKeyword
{
    IEnumerable<IProblemInputParser> GetInputParsers();
    IEnumerable<IProblemSolver> GetSolvers();
}

public interface IProblemInput
{
}

public interface IProblemInputParser : IDescribable, IHasKeyword
{
}

public interface IProblemStringInputParser : IProblemInputParser
{
    IProblemInput? Parse(IAnsiConsole console, string input);
}

public interface IProblemPromptInputParser : IProblemInputParser
{
    IProblemInput? Prompt(IAnsiConsole console);
}

public interface IProblemSolver : IDescribable, IHasKeyword
{
}

public interface IProblemOutput
{
}

public interface IProblemOutputSolver : IProblemSolver
{
    IProblemOutput Solve(IProblemInput input);
}