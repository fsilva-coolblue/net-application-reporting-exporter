using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OfficeOpenXml;

namespace ApplicationReportingDataDownloader
{
    public class EventExcelExporter
    {
        public bool Export(Dictionary<string, List<ReportingEventDataEntity>> eventsPerName, string fileName)
        {
            try
            {
                using(var package = new ExcelPackage())
                {
                    foreach(var kvp in eventsPerName)
                    {
                        var worksheet = package.Workbook.Worksheets.Add(kvp.Key);

                        var allAvailableProperties =
                            kvp.Value.SelectMany(evt => evt.Properties.Keys.ToList()).Distinct().ToList();

                        var column = 1;
                        var row = 1;

                        worksheet.Cells[row, column].Value = "EventName";
                        worksheet.Cells[row, column++].Style.Font.Bold = true;

                        worksheet.Cells[row, column].Value = "UserName";
                        worksheet.Cells[row, column++].Style.Font.Bold = true;

                        worksheet.Cells[row, column].Value = "EventTime";
                        worksheet.Cells[row, column++].Style.Font.Bold = true;

                        foreach(var property in allAvailableProperties)
                        {
                            worksheet.Cells[row, column].Value = property;
                            worksheet.Cells[row, column++].Style.Font.Bold = true;
                        }

                        foreach(var evt in kvp.Value)
                        {
                            row++;
                            column = 1;

                            worksheet.Cells[row, column++].Value = evt.Name;
                            worksheet.Cells[row, column++].Value = evt.UserName;
                            worksheet.Cells[row, column++].Value = evt.EventTime;

                            foreach(var property in allAvailableProperties)
                                worksheet.Cells[row, column++].Value = evt.Properties[property];
                        }
                    }


                    package.SaveAs(new FileInfo(fileName));
                }
            }
            catch(Exception e)
            {
                File.AppendAllText("errors.txt", e.Message);
                return false;
            }

            return true;
        }
    }
}