using System.ComponentModel;
using Exercises;
using ProjectEuler;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<SolveCommand>();

app.Configure(config =>
{
    config.AddCommand<PrintCommand>("print");
});

return app.Run(args);

public class SolveCommand : Command<SolveCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[source]")]
        public string? Source { get; set; }

        [CommandArgument(1, "[category]")]
        public string? Category { get; set; }

        [CommandArgument(2, "[problem]")]
        public string? Problem { get; set; }

        [CommandOption("-p|--parser")]
        public string? Parser { get; set; }

        [CommandOption("-i|--input")]
        public string? Input { get; set; }

        [CommandOption("-s|--solver")]
        public string[]? Solvers { get; set; }

        [CommandOption("-w|--warm-up")]
        [DefaultValue(0)]
        public int WarmUp { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var config = new ProblemRunner.Config
        {
            Source = settings.Source,
            Category = settings.Category,
            Problem = settings.Problem,
            Parser = settings.Parser,
            Input = settings.Input,
            Solvers = settings.Solvers ?? [],
            WarmUp = settings.WarmUp,
        };

        var runner = new ProblemRunner(
            ProblemSourceProvider.Instance,
            AnsiConsole.Console,
            config);

        return runner.Run();
    }
}

public class PrintCommand : Command<PrintCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-s|--summary")]
        public bool Summary { get; set; }

        [CommandOption("-l|--link")]
        public bool Link { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var options = new PrintSummaryOptions(
            Summary: settings.Summary,
            Link: settings.Link);

        var runner = new PrintProblemsRunner(
            ProblemSourceProvider.Instance,
            AnsiConsole.Console,
            options);

        return runner.Run();
    }
}

public class ProblemSourceProvider : IProblemSourceProvider
{
    private static readonly Lazy<IProblemSourceProvider> instance = new(() => new ProblemSourceProvider());

    public static IProblemSourceProvider Instance => instance.Value;

    public IEnumerable<IProblemSource> GetSources()
    {
        yield return ProjectEulerSource.Instance;
    }
}
