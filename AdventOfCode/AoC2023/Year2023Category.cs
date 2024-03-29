﻿using Exercises;

namespace AoC2023;

public static class Year2023Category
{
    private static readonly Lazy<IProblemCategory> instance = new(() => new ProblemCategory(
        "2023",
        "Advent of Code 2023",
        GetProblems,
        "Advent of Code 2023",
        new Uri("https://adventofcode.com/2023")
    ));

    public static IProblemCategory Instance => instance.Value;

    private static IEnumerable<IProblem> GetProblems()
    {
        yield return Day1.Instance;
        yield return Day2.Instance;
        yield return Day3.Instance;
        yield return Day4.Instance;
        yield return Day5.Instance;
        yield return Day6.Instance;
        yield return Day7.Instance;
    }
}