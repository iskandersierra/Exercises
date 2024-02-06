namespace Exercises;

public static class Primes
{
    public static int MaximumCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static bool AreRelativePrimes(int a, int b)
    {
        return MaximumCommonFactor(a, b) == 1;
    }

    public static bool IsPrime(int number)
    {
        if (number < 2) return false;
        if (number < 4) return true;
        if (number % 2 == 0) return false;
        var maxToCheck = (int)Math.Sqrt(number);
        for (var i = 3; i <= maxToCheck; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    public static bool IsPrimeFor(int number, IReadOnlyList<int> previousPrimes)
    {
        for (int i = 0; i < previousPrimes.Count; i++)
        {
            var prime = previousPrimes[i];
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
    }

    public static bool IsPrimeFor(int number, IEnumerable<int> previousPrimes)
    {
        foreach (var prime in previousPrimes)
        {
            var div = Math.DivRem(number, prime, out var remainder);
            if (remainder == 0) return false;
            if (div < prime) return true;
        }
        return true;
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

        return FillPrimes(estimatedCapacity, primes => primes[^1] < last);
    }

    public static List<int> GetFirstPrimes(int amount)
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

    private static List<int> FillPrimes(int capacity, Func<List<int>, bool> shouldContinue)
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

    //public static IEnumerable<int> GetPrimes(int below)
    //{
    //    var primes = new List<int> { 2 };
    //    for (var i = 3; i < below; i += 2)
    //    {
    //        if (IsPrime(i))
    //        {
    //            primes.Add(i);
    //        }
    //    }
    //    return primes;
    //}
}
