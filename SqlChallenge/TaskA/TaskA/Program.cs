using TaskA;

Console.WriteLine("csv Data Import and Sanitization Tool");
Console.WriteLine("=====================================\n");

try
{
    string csvFilePath = GetCsvFilePath(args);
    string excelFilePath = GenerateExcelFilePath(csvFilePath);

    Console.WriteLine($"Processing CSV file: {csvFilePath}");
    Console.WriteLine($"Output Excel file: {excelFilePath}\n");

    var processor = new DataImporter();

    var result = processor.ProcessCsvFile(csvFilePath);

    DisplayProcessingResults(result);

    Exporter.ExportToExcel(result, excelFilePath);

    Console.WriteLine($"\nProcessing complete! Excel file saved: {excelFilePath}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Environment.ExitCode = 1;
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
return;

static string GetCsvFilePath(string[] args)
{
    if (args.Length > 0 && File.Exists(args[0]))
    {
        return args[0];
    }

    Console.Write("Enter CSV file path: ");
    string? filePath = Console.ReadLine();
            
    if (string.IsNullOrWhiteSpace(filePath))
    {
        throw new ArgumentException("csv file path is required");
    }

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"csv file not found: {filePath}");
    }

    return filePath;
}

static string GenerateExcelFilePath(string csvFilePath)
{
    string directory = Path.GetDirectoryName(csvFilePath) ?? "";
    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(csvFilePath);
    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

    return Path.Combine(directory, $"{fileNameWithoutExtension}_processed_{timestamp}.xlsx");
}

static void DisplayProcessingResults(ProcessingResult result)
{
    Console.WriteLine("Processing Results:");
    Console.WriteLine($"- Total records processed: {result.TotalProcessed}");
    Console.WriteLine($"- Valid records: {result.ValidEntities.Count}");
    Console.WriteLine($"- Invalid records: {result.InvalidEntities.Count}");

    if (result.TotalProcessed > 0)
    {
        double successRate = (double)result.ValidEntities.Count / result.TotalProcessed * 100;
        Console.WriteLine($"- Success rate: {successRate:F1}%");
    }

    if (result.ProcessingErrors.Any())
    {
        Console.WriteLine("\nProcessing Errors:");
        foreach (var error in result.ProcessingErrors)
        {
            Console.WriteLine($"- {error}");
        }
    }

    if (result.InvalidEntities.Count == 0) return;
    Console.WriteLine("\nValidation Issues Found:");
    var errorSummary = result.InvalidEntities
        .SelectMany(e => e.ValidationErrors)
        .GroupBy(e => e)
        .OrderByDescending(g => g.Count())
        .Take(5);

    foreach (var errorGroup in errorSummary)
    {
        Console.WriteLine($"- {errorGroup.Key} ({errorGroup.Count()} occurrences)");
    }
}