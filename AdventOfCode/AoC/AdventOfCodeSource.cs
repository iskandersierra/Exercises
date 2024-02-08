using AoC2023;
using Exercises;

namespace AoC;

public static class AdventOfCodeSource
{
    private static readonly Lazy<IProblemSource> instance = new(() => new ProblemSource(
        "aoc",
        "Advent of Code",
        GetCategories,
        """
        Advent of Code is an Advent calendar of small programming puzzles for a 
        variety of skill sets and skill levels that can be solved in any programming 
        language you like. People use them as interview prep, company training, 
        university coursework, practice problems, a speed contest, or to challenge 
        each other.
        """,
        new Uri("https://adventofcode.com")
    ));

    public static IProblemSource Instance => instance.Value;

    private static IEnumerable<IProblemCategory> GetCategories()
    {
        yield return Year2023Category.Instance;
    }
}
