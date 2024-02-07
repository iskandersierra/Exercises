namespace Exercises;

public static class Strings
{
    public static bool IsPalindrome(ReadOnlySpan<char> chars)
    {
        var midIndex = chars.Length / 2;
        for (int i = 0; i < midIndex; i++)
        {
            if (chars[i] != chars[chars.Length - 1 - i])
                return false;
        }
        return true;
    }
}
