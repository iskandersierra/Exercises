using Spectre.Console;

namespace Exercises;

public class PrintProblemsRunner(
    IProblemSourceProvider provider,
    IAnsiConsole console) :
    IRunner
{
    public int Run()
    {
        foreach (var source in provider.GetSources())
        {
            ModelsExtensions.PrintItem(source, console);

            foreach (var category in source.GetCategories())
            {
                ModelsExtensions.PrintItem(category, console, "  ");

                foreach (var problem in category.GetProblems())
                {
                    ModelsExtensions.PrintItem(problem, console, "    ");
                }
            }
        }

        return 0;
    }
}