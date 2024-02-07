namespace Exercises;

public static class Primes
{
    public static long MaximumCommonFactor(long a, long b)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(a);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(b);

        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static bool AreRelativePrimes(long a, long b)
    {
        return MaximumCommonFactor(a, b) == 1;
    }

    public static bool IsPrime(long number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);

        if (number < 2) return false;
        if (number < 4) return true;
        if (number % 2 == 0) return false;
        var maxToCheck = (long)Math.Sqrt(number);
        for (var i = 3; i <= maxToCheck; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    public static bool IsPrimeFor(long number, IReadOnlyList<int> previousPrimes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);
        ArgumentNullException.ThrowIfNull(previousPrimes);

        for (int i = 0; i < previousPrimes.Count; i++)
        {
            var prime = previousPrimes[i];
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
    }

    public static bool IsPrimeFor(long number, IReadOnlyList<long> previousPrimes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);
        ArgumentNullException.ThrowIfNull(previousPrimes);

        for (int i = 0; i < previousPrimes.Count; i++)
        {
            var prime = previousPrimes[i];
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
    }

    public static bool IsPrimeFor(long number, IEnumerable<int> previousPrimes)
    {
        foreach (var prime in previousPrimes)
        {
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
    }

    public static bool IsPrimeFor(long number, IEnumerable<long> previousPrimes)
    {
        foreach (var prime in previousPrimes)
        {
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
    }

    public static IEnumerable<int> GetPrimesInt()
    {
        List<int> primes = [ 2, 3 ];

        foreach (var prime in primes)
        {
            yield return prime;
        }

        for (var t = 6; t <= int.MaxValue - 1; t += 6)
        {
            var candidate = t - 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
                yield return candidate;
            }

            candidate = t + 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
                yield return candidate;
            }
        }
    }
    
    public static IEnumerable<long> GetPrimes()
    {
        List<long> primes = [ 2, 3 ];

        foreach (var prime in primes)
        {
            yield return prime;
        }

        for (var t = 6L; t <= long.MaxValue - 1; t += 6)
        {
            var candidate = t - 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
                yield return candidate;
            }

            candidate = t + 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
                yield return candidate;
            }
        }
    }

    public static List<int> GetPrimesBelow(int below, bool inclusive = true)
    {
        var last = inclusive ? below : below - 1;

        switch (last)
        {
            case < 2:
                return [];

            case 2:
                return [2];

            case 3:
                return [2, 3];
        }

        var estimatedCapacity = (int)Math.Ceiling(last / Math.Log(last)) + 10;

        return FillPrimesInt(estimatedCapacity, primes => primes[^1] < last);
    }
    
    public static List<long> GetPrimesBelow(long below, bool inclusive = true)
    {
        var last = inclusive ? below : below - 1;

        switch (last)
        {
            case < 2:
                return [];

            case 2:
                return [2];

            case 3:
                return [2, 3];
        }

        var estimatedCapacity = (int)Math.Ceiling(last / Math.Log(last)) + 10;

        return FillPrimes(estimatedCapacity, primes => primes[^1] < last);
    }

    public static List<int> GetFirstPrimesInt(int amount)
    {
        switch (amount)
        {
            case < 1:
                return [];

            case 1:
                return [2];

            case 2:
                return [2, 3];
        }

        return FillPrimesInt(amount, primes => primes.Count < amount);
    }
    
    public static List<long> GetFirstPrimes(int amount)
    {
        switch (amount)
        {
            case < 1:
                return [];

            case 1:
                return [2];

            case 2:
                return [2, 3];
        }

        return FillPrimes(amount, primes => primes.Count < amount);
    }

    private static List<int> FillPrimesInt(int capacity, Func<List<int>, bool> shouldContinue)
    {
        var primes = new List<int>(capacity: capacity) { 2, 3 };

        for (var t = 1; shouldContinue(primes); t++)
        {
            // try with 6k-1 and 6k+1 for primes, using the primes before it
            var candidate = 6 * t - 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
            }

            if (!shouldContinue(primes)) break;

            candidate += 2;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
            }
        }

        return primes;
    }

    private static List<long> FillPrimes(int capacity, Func<List<long>, bool> shouldContinue)
    {
        var primes = new List<long>(capacity: capacity) { 2, 3 };

        for (var t = 1; shouldContinue(primes); t++)
        {
            // try with 6k-1 and 6k+1 for primes, using the primes before it
            var candidate = 6 * t - 1;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
            }

            if (!shouldContinue(primes)) break;

            candidate += 2;
            if (IsPrimeFor(candidate, primes))
            {
                primes.Add(candidate);
            }
        }

        return primes;
    }


    public static IEnumerable<int> GetPrimeFactors(long number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);

        var remaining = number;

        foreach (var prime in GetPrimesInt())
        {
            while (Math.DivRem(remaining, prime, out var remainder) is var quotient && remainder == 0)
            {
                remaining = quotient;
                yield return prime;
            }

            if (remaining == 1) break;
        }
    }
}
