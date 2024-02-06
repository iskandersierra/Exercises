using Exercises;

namespace ProjectEuler.Page1.Problem50;

/// <summary>
/// Consecutive Prime Sum
/// https://projecteuler.net/problem=50
/// The prime 41, can be written as the sum of six consecutive primes:
///     41=2+3+5+7+11+13
/// This is the longest sum of consecutive primes that adds to a prime below one-hundred.
/// The longest sum of consecutive primes below one-thousand that adds to a prime, contains
/// 21 terms, and is equal to 953.
/// Which prime, below one-million, can be written as the sum of the most consecutive primes?
/// </summary>
public record Input(int Below);

public record Output(int Prime, int Start, int Length);

public class Solver
{
    public Output Solve(Input input)
    {
        var primes = Primes.GetPrimesBelow(input.Below, inclusive: false);

        var longestStart = 0;
        var longestLength = 1;
        var correspondingPrime = primes[longestStart];

        var primeSet = new HashSet<int>(primes);

        for (int start = 0; start < primes.Count - longestLength; start++)
        {
            for (int length = longestLength + 1; start + length < primes.Count; length++)
            {
                var sum = 0;
                for (int i = 0; i < length; i++)
                {
                    sum += primes[start + i];
                }
                if (sum >= input.Below) break;
                if (!primeSet.Contains(sum)) continue;
                longestStart = start;
                longestLength = length;
                correspondingPrime = sum;
            }
        }

        return new Output(correspondingPrime, longestStart, longestLength);
    }
}
