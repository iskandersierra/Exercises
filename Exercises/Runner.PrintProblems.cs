using Spectre.Console;

namespace Exercises;

public class PrintProblemsRunner(
    IProblemSourceProvider provider,
    IAnsiConsole console,
    PrintSummaryOptions options) :
    IRunner
{
    public int Run()
    {
        foreach (var source in provider.GetSources())
        {
            ModelsExtensions.PrintItem(source, console, options);

            foreach (var category in source.GetCategories())
            {
                ModelsExtensions.PrintItem(category, console, options with { Indent = "  " });

                foreach (var problem in category.GetProblems())
                {
                    ModelsExtensions.PrintItem(problem, console, options with { Indent = "    " });
                }
            }
        }

        return 0;
    }
}