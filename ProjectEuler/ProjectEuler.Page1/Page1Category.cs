using Exercises;

namespace ProjectEuler.Page1;

public static class Page1Category
{
    private static readonly Lazy<IProblemCategory> instance = new(() => new ProblemCategory(
        "page1",
        "Problems 1-50",
        GetProblems,
        "The first 50 problems from Project Euler.",
        new Uri("https://projecteuler.net/archives")
    ));

    public static IProblemCategory Instance => instance.Value;
    
    private static IEnumerable<IProblem> GetProblems()
    {
        yield return Problem1.Instance;
        yield return Problem2.Instance;
    }
}
