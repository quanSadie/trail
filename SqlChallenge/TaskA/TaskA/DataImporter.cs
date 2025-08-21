using System.Text;

namespace TaskA;

public class DataImporter
{
    public ProcessingResult ProcessCsvFile(string csvFilePath)
    {
        var result = new ProcessingResult();

        if (!File.Exists(csvFilePath))
        {
            result.ProcessingErrors.Add($"csv file not found: {csvFilePath}");
            return result;
        }

        try
        {
            var lines = File.ReadAllLines(csvFilePath, Encoding.UTF8);

            if (lines.Length == 0)
            {
                result.ProcessingErrors.Add("csv file is empty");
                return result;
            }

            var dataLines = lines.Skip(HasHeader(lines) ? 1 : 0).ToArray(); // check if header row exists
            result.TotalProcessed = dataLines.Length;

            for (var i = 0; i < dataLines.Length; i++)
            {
                try
                {
                    var entity = ParseCsvLine(dataLines[i], i + (HasHeader(lines) ? 2 : 1));

                    if (entity.IsValid)
                    {
                        result.ValidEntities.Add(entity);
                    }
                    else
                    {
                        result.InvalidEntities.Add(entity);
                    }
                }
                catch (Exception ex)
                {
                    result.ProcessingErrors.Add($"error at line {i + 1}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            result.ProcessingErrors.Add($"Error reading csv file: {ex.Message}");
        }

        return result;
    }

    private Entity ParseCsvLine(string line, int lineNumber)
    {
        var entity = new Entity();
        var fields = ParseCsvFields(line);

        // Ensure we have enough fields
        while (fields.Count < 8)
        {
            fields.Add(string.Empty);
        }

        try
        {
            entity.EntityId = ParseEntityId(fields[0], entity.ValidationErrors);
            entity.EntityFirstName =
                Sanitizer.SanitizeName(fields[1], "First Name", entity.ValidationErrors, isRequired: true);
            entity.EntityMiddleName =
                Sanitizer.SanitizeName(fields[2], "Middle Name", entity.ValidationErrors, isRequired: false);
            entity.EntityLastName =
                Sanitizer.SanitizeName(fields[3], "Last Name", entity.ValidationErrors, isRequired: false);
            entity.EntityDob = Sanitizer.ParseDate(fields[4], entity.ValidationErrors);
            entity.IsMaster = Sanitizer.ParseBoolean(fields[5]);
            entity.Address = Sanitizer.SanitizeAddress(fields[6]);
            entity.EntityGender = Sanitizer.SanitizeGender(fields[7], entity.ValidationErrors);
        }
        catch (Exception ex)
        {
            entity.ValidationErrors.Add($"Line {lineNumber}: Parsing error - {ex.Message}");
        }

        return entity;
    }

    private static bool HasHeader(string[] lines)
    {
        if (lines.Length == 0) return false;

        var firstLine = lines[0].ToLower();
        return firstLine.Contains("entity_id") ||
               firstLine.Contains("first_name") ||
               firstLine.Contains("last_name") ||
               firstLine.Contains("name");
    }

    private static List<string> ParseCsvFields(string line)
    {
        var fields = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        foreach (var c in line)
        {
            switch (c)
            {
                case '"':
                    inQuotes = !inQuotes;
                    break;
                case ',' when !inQuotes:
                    fields.Add(field.ToString().Trim());
                    field.Clear();
                    break;
                default:
                    field.Append(c);
                    break;
            }
        }

        fields.Add(field.ToString().Trim());
        return fields;
    }

    private static int ParseEntityId(string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("Entity ID is required");
            return 0;
        }

        if (int.TryParse(value.Trim(), out var id))
        {
            if (id > 0) return id;
            errors.Add("Entity ID must be > 0");
            return 0;

        }

        errors.Add($"Invalid Entity ID format: '{value}'");
        return 0;
    }
}