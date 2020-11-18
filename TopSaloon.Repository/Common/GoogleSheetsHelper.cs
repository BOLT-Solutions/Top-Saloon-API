using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using TopSaloon;

namespace TopSaloon.Repository.Common
{


    public class GoogleSheetsHelper
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "GoogleSheetsHelper";

        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId = "171QH0qSv_75dXz8GwNyY_pisAZIRMqNNzz65LN1zhbU";
        static readonly string sheet = "Top-Saloon";

        public GoogleSheetsHelper(string credentialFileName, string spreadsheetId)
        {
            var credential = GoogleCredential.FromStream(new FileStream(credentialFileName, FileMode.Open)).CreateScoped(Scopes);

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            _spreadsheetId = spreadsheetId;
        }

        public void CreateEntry(OrderToRecord GoogleSheetRecord)
        {
            var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();
            var oblist = new List<object>();

            valueRange.Values = new List<IList<object>> { oblist };
            valueRange.Range = range;

            for (int i = 0; i < GoogleSheetRecord.Services.Count; i++)
            {
                oblist.Clear();
                if(i==0)
                {
                    oblist.Add(GoogleSheetRecord.CustomerNameAR);
                    oblist.Add(GoogleSheetRecord.BarberNameAR);
                }
                else
                {
                    oblist.Add(" ");
                    oblist.Add(" ");
                }
                
                oblist.Add(GoogleSheetRecord.Services[i].ServiceNameAR);
                oblist.Add(GoogleSheetRecord.Services[i].ServicePrice);
                oblist.Add(GoogleSheetRecord.Services[i].ServiceTime);
               var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
               var appendReponse = appendRequest.Execute();
            }
    

        }
        void UpdateEntry()
        {
            var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();

            var oblist = new List<object>() { "updated" };
            valueRange.Values = new List<IList<object>> { oblist };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
        }

        public void ReadEntries()
        {
            var range = $"{sheet}!A:F";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    // Print columns A to F, which correspond to indices 0 and 4.
                    Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}", row[0], row[1], row[2], row[3], row[4], row[5]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        //    public List<ExpandoObject> GetDataFromSheet(GoogleSheetParameters googleSheetParameters)
        //    {
        //        googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
        //        var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

        //        SpreadsheetsResource.ValuesResource.GetRequest request =
        //            _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

        //        var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
        //        var columnNames = new List<string>();
        //        var returnValues = new List<ExpandoObject>();

        //        if (!googleSheetParameters.FirstRowIsHeaders)
        //        {
        //            for (var i = 0; i <= numberOfColumns; i++)
        //            {
        //                columnNames.Add($"Column{i}");
        //            }
        //        }

        //        var response = request.Execute();

        //        int rowCounter = 0;
        //        IList<IList<Object>> values = response.Values;
        //        if (values != null && values.Count > 0)
        //        {
        //            foreach (var row in values)
        //            {
        //                if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
        //                {
        //                    for (var i = 0; i <= numberOfColumns; i++)
        //                    {
        //                        columnNames.Add(row[i].ToString());
        //                    }
        //                    rowCounter++;
        //                    continue;
        //                }

        //                var expando = new ExpandoObject();
        //                var expandoDict = expando as IDictionary<String, object>;
        //                var columnCounter = 0;
        //                foreach (var columnName in columnNames)
        //                {
        //                    expandoDict.Add(columnName, row[columnCounter].ToString());
        //                    columnCounter++;
        //                }
        //                returnValues.Add(expando);
        //                rowCounter++;
        //            }
        //        }

        //        return returnValues;
        //    }

        //    public void AddCells(GoogleSheetParameters googleSheetParameters, List<GoogleSheetRow> rows)
        //    {
        //        var requests = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

        //        var sheetId = GetSheetId(_sheetsService, _spreadsheetId, googleSheetParameters.SheetName);

        //        GridCoordinate gc = new GridCoordinate
        //        {
        //            ColumnIndex = googleSheetParameters.RangeColumnStart - 1,
        //            RowIndex = googleSheetParameters.RangeRowStart - 1,
        //            SheetId = sheetId
        //        };

        //        var request = new Request { UpdateCells = new UpdateCellsRequest { Start = gc, Fields = "*" } };

        //        var listRowData = new List<RowData>();

        //        foreach (var row in rows)
        //        {
        //            var rowData = new RowData();
        //            var listCellData = new List<CellData>();
        //            foreach (var cell in row.Cells)
        //            {
        //                var cellData = new CellData();
        //                var extendedValue = new ExtendedValue { StringValue = cell.CellValue };

        //                cellData.UserEnteredValue = extendedValue;
        //                var cellFormat = new CellFormat { TextFormat = new TextFormat() };

        //                if (cell.IsBold)
        //                {
        //                    cellFormat.TextFormat.Bold = true;
        //                }

        //                cellFormat.BackgroundColor = new Color { Blue = (float)cell.BackgroundColor.B / 255, Red = (float)cell.BackgroundColor.R / 255, Green = (float)cell.BackgroundColor.G / 255 };

        //                cellData.UserEnteredFormat = cellFormat;
        //                listCellData.Add(cellData);
        //            }
        //            rowData.Values = listCellData;
        //            listRowData.Add(rowData);
        //        }
        //        request.UpdateCells.Rows = listRowData;

        //        // It's a batch request so you can create more than one request and send them all in one batch. Just use reqs.Requests.Add() to add additional requests for the same spreadsheet
        //        requests.Requests.Add(request);

        //        _sheetsService.Spreadsheets.BatchUpdate(requests, _spreadsheetId).Execute();
        //    }

        //    private string GetColumnName(int index)
        //    {
        //        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //        var value = "";

        //        if (index >= letters.Length)
        //            value += letters[index / letters.Length - 1];

        //        value += letters[index % letters.Length];
        //        return value;
        //    }

        //    private GoogleSheetParameters MakeGoogleSheetDataRangeColumnsZeroBased(GoogleSheetParameters googleSheetParameters)
        //    {
        //        googleSheetParameters.RangeColumnStart = googleSheetParameters.RangeColumnStart - 1;
        //        googleSheetParameters.RangeColumnEnd = googleSheetParameters.RangeColumnEnd - 1;
        //        return googleSheetParameters;
        //    }

        //    private int GetSheetId(SheetsService service, string spreadSheetId, string spreadSheetName)
        //    {
        //        var spreadsheet = service.Spreadsheets.Get(spreadSheetId).Execute();
        //        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == spreadSheetName);
        //        int sheetId = (int)sheet.Properties.SheetId;

        //        return sheetId;
        //    }
        //}

        //public class GoogleSheetCell
        //{
        //    public string CellValue { get; set; }
        //    public bool IsBold { get; set; }
        //    public System.Drawing.Color BackgroundColor { get; set; } = System.Drawing.Color.White;
        //}

        //public class GoogleSheetParameters
        //{
        //    public int RangeColumnStart { get; set; }
        //    public int RangeRowStart { get; set; }
        //    public int RangeColumnEnd { get; set; }
        //    public int RangeRowEnd { get; set; }
        //    public string SheetName { get; set; }
        //    public bool FirstRowIsHeaders { get; set; }
        //}

        //public class GoogleSheetRow
        //{
        //    public GoogleSheetRow() => Cells = new List<GoogleSheetCell>();

        //    public List<GoogleSheetCell> Cells { get; set; }
        //}
    }
}
