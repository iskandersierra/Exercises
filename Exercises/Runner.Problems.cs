using System.Diagnostics;
using Spectre.Console;

namespace Exercises;

public class ProblemRunner(
    IProblemSourceProvider provider,
    IAnsiConsole console,
    ProblemRunner.Config config) :
    IRunner
{
    public int Run()
    {
        var problem = provider.GetProblem(console, config.Source, config.Category, config.Problem);

        if (problem is null) return 1;

        ModelsExtensions.PrintItem(problem, console);
        console.WriteLine();

        var input = problem.GetProblemInput(console, config.Parser, config.Input);

        if (input is null) return 1;

        var solvers = problem.GetProblemSolvers(console, config.Solvers);

        if (solvers is []) return 1;

        foreach (var solver in solvers)
        {
            ModelsExtensions.PrintItem(solver, console);
            RunSolver(solver, input);
            console.WriteLine();
        }

        return 0;
    }

    private void RunSolver(
        IProblemSolver solver,
        IProblemInput input)
    {
        switch (solver)
        {
            case IProblemOutputSolver outputSolver:
            {
                // warm up
                if (config.WarmUp > 0)
                {
                    for (var i = 0; i < config.WarmUp; i++)
                        outputSolver.Solve(input);
                }

                // measure
                var watch = Stopwatch.StartNew();
                var output = outputSolver.Solve(input);
                watch.Stop();
                // print
                console.MarkupLine($"[bold]Output:[/]");
                output.PrintTo(console);
                console.MarkupLine($"[bold]Elapsed time:[/] {watch.Elapsed.ToPreciseString()}");
                break;
            }

            default:
            {
                console.MarkupLine($"[red]Unsupported solver: {solver.GetType().FullName}[/]");
                break;
            }
        }
    }

    public class Config
    {
        public string? Source { get; set; }
        public string? Category { get; set; }
        public string? Problem { get; set; }
        public string? Parser { get; set; }
        public string? Input { get; set; }
        public string[] Solvers { get; set; } = Array.Empty<string>();
        public int WarmUp { get; set; }
    }
}
