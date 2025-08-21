using OfficeOpenXml;

namespace TaskA;

public static class Exporter
{
    public static void ExportToExcel(ProcessingResult result, string excelFilePath)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization"); //This will also set the Company property to the organization name provided in the argument.

        using var package = new ExcelPackage();

        var validSheet = package.Workbook.Worksheets.Add("Valid Entities");
        var invalidSheet = package.Workbook.Worksheets.Add("Invalid Entities");
        var summarySheet = package.Workbook.Worksheets.Add("Processing Summary");

        ExportEntitiesToSheet(validSheet, result.ValidEntities, includeErrors: false);

        ExportEntitiesToSheet(invalidSheet, result.InvalidEntities, includeErrors: true);

        CreateSummarySheet(summarySheet, result);

        package.SaveAs(new FileInfo(excelFilePath));
    }

    private static void ExportEntitiesToSheet(ExcelWorksheet sheet, List<Entity> entities, bool includeErrors)
    {
        var headers = new[]
        {
            "entity_id", "entity_first_name", "entity_middle_name", "entity_last_name",
            "entity_dob", "is_master", "address", "entity_gender"
        };

        if (includeErrors)
        {
            headers = headers.Concat(new[] { "validation_errors" }).ToArray();
        }
        
        // write header
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cells[1, i + 1].Value = headers[i];
            sheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // process data
        for (int row = 0; row < entities.Count; row++)
        {
            var entity = entities[row];
            var excelRow = row + 2;

            sheet.Cells[excelRow, 1].Value = entity.EntityId;
            sheet.Cells[excelRow, 2].Value = entity.EntityFirstName;
            sheet.Cells[excelRow, 3].Value = entity.EntityMiddleName;
            sheet.Cells[excelRow, 4].Value = entity.EntityLastName;
            sheet.Cells[excelRow, 5].Value = entity.EntityDob;
            sheet.Cells[excelRow, 6].Value = entity.IsMaster;
            sheet.Cells[excelRow, 7].Value = entity.Address;
            sheet.Cells[excelRow, 8].Value = entity.EntityGender;

            if (includeErrors)
            {
                sheet.Cells[excelRow, 9].Value = string.Join("; ", entity.ValidationErrors);
            }
        }
        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
    }

    private static void CreateSummarySheet(ExcelWorksheet sheet, ProcessingResult result)
    {
        sheet.Cells["A1"].Value = "Processing Summary";
        sheet.Cells["A1"].Style.Font.Bold = true;
        sheet.Cells["A1"].Style.Font.Size = 16;

        int row = 3;
        sheet.Cells[row++, 1].Value = "Total Records Processed:";
        sheet.Cells[row - 1, 2].Value = result.TotalProcessed;

        sheet.Cells[row++, 1].Value = "Valid Records:";
        sheet.Cells[row - 1, 2].Value = result.ValidEntities.Count;

        sheet.Cells[row++, 1].Value = "Invalid Records:";
        sheet.Cells[row - 1, 2].Value = result.InvalidEntities.Count;

        sheet.Cells[row++, 1].Value = "Success Rate:";
        var successRate = result.TotalProcessed > 0
            ? (double)result.ValidEntities.Count / result.TotalProcessed * 100
            : 0;
        sheet.Cells[row - 1, 2].Value = $"{successRate:F1}%";

        row += 2;
        if (result.ProcessingErrors.Any())
        {
            sheet.Cells[row++, 1].Value = "Processing Errors:";
            foreach (var error in result.ProcessingErrors)
            {
                sheet.Cells[row++, 1].Value = error;
            }
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
    }
}