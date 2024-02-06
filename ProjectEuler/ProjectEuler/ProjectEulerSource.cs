using Exercises;
using ProjectEuler.Page1;

namespace ProjectEuler;

public static class ProjectEulerSource
{
    private static readonly Lazy<IProblemSource> instance = new(() => new ProblemSource(
        "euler",
        "Project Euler",
        GetCategories,
        """
        Project Euler exists to encourage, challenge, and develop the skills and enjoyment
        of anyone with an interest in the fascinating world of mathematics.
        """,
        new Uri("https://projecteuler.net")
    ));

    public static IProblemSource Instance => instance.Value;

    private static IEnumerable<IProblemCategory> GetCategories()
    {
        yield return Page1Category.Instance;
    }
}
