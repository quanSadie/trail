using System.Globalization;
using System.Text.RegularExpressions;

namespace TaskA;

public static class Sanitizer
{
    private static readonly HashSet<string> _validGenders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Male", "Female", "M", "F", "Other", "Non-binary", "Prefer not to say", ""
    };

    private static readonly string[] _dateFormats = {
        "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "MM-dd-yyyy",
        "yyyy/MM/dd", "dd.MM.yyyy", "MM.dd.yyyy", "yyyy.MM.dd"
    };
    
    internal static string SanitizeName(string value, string fieldName, List<string> errors, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (isRequired)
            {
                errors.Add($"{fieldName} is required");
            }

            return string.Empty;
        }

        var cleaned = value.Trim();

        cleaned = Regex.Replace(cleaned, @"[^\w\s\-']", "");

        if (cleaned.Length > 128)
        {
            cleaned = cleaned.Substring(0, 128);
            errors.Add($"{fieldName} was truncated to 128 characters");
        }

        if (!string.IsNullOrWhiteSpace(cleaned) || !isRequired) return cleaned;
        errors.Add($"{fieldName} contains no valid characters");
        return string.Empty;

    }

    internal static DateTime? ParseDate(string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        foreach (var format in _dateFormats)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime date))
            {
                if (date < new DateTime(1900, 1, 1) || date > DateTime.Now)
                {
                    errors.Add($"Date of birth is outside valid range: {value}");
                    return null;
                }

                return date;
            }
        }

        if (DateTime.TryParse(value, out DateTime parsedDate))
        {
            if (parsedDate < new DateTime(1900, 1, 1) || parsedDate > DateTime.Now)
            {
                errors.Add($"Date of birth is outside valid range: {value}");
                return null;
            }

            return parsedDate;
        }

        errors.Add($"Invalid date format: '{value}'");
        return null;
    }

    internal static bool ParseBoolean(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim().ToLower();

        return value == "true" || value == "1" || value == "yes" ||
               value == "y" || value == "master" || value == "primary";
    }

    internal static string? SanitizeAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var cleaned = value.Trim();

        if (cleaned.Length > 512)
        {
            cleaned = cleaned.Substring(0, 512);
        }

        return cleaned;
    }

    internal static string? SanitizeGender(string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var cleaned = value.Trim();

        // Normalize common variations
        var normalized = cleaned.ToLower();
        switch (normalized)
        {
            case "m":
            case "male":
                return "Male";
            case "f":
            case "female":
                return "Female";
            case "other":
            case "non-binary":
            case "nonbinary":
                return "Other";
            case "prefer not to say":
            case "unknown":
                return "Prefer not to say";
            default:
                if (cleaned.Length > 16)
                {
                    errors.Add($"Gender field too long, truncated: '{cleaned}'");
                    return cleaned.Substring(0, 16);
                }

                return cleaned;
        }
    }
}