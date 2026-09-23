using System;

public static class IdGenerator
{
    public static string New()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    public static string New(string prefix)
    {
        string sanitized = Sanitize(prefix);

        return sanitized.Length > 0 ? sanitized + "_" + New() : New();
    }

    private static string Sanitize(string value)
    {
        char[] buffer = new char[value.Length];
        int length = 0;

        foreach (char character in value)
        {
            if (char.IsLetterOrDigit(character))
                buffer[length++] = character;
            else if (length > 0 && buffer[length - 1] != '_')
                buffer[length++] = '_';
        }

        return new string(buffer, 0, length).Trim('_');
    }
}
