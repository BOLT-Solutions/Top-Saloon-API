using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using TopSaloon;
using System.Linq;

namespace TopSaloon.Repository.Common
{


    public class GoogleSheetsHelper
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "TopSaloon";

        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId = "171QH0qSv_75dXz8GwNyY_pisAZIRMqNNzz65LN1zhbU";// Live Sheet
       // private readonly string _spreadsheetId = "1CvjhZViJy-fJSTbEA3Yh9zJRA7WdgnuLz1fFrnIF1XE";// Test Sheet


        //static readonly string sheet = "TestSheet";
        GoogleCredential credential;

       
       
    public GoogleSheetsHelper() //Initialize google sheets API.
        {
            using (var stream = new FileStream("topsaloon-1605782329463-e0653a0a3535.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, "client_secret_389780309451 - 4g6qd451dhjduncf882vlqqvsnokgpmo.apps.googleusercontent.com.json"); 
                   credential = GoogleCredential.FromFile("topsaloon-1605782329463-e0653a0a3535.json").CreateScoped(Scopes);
                
            }
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
               
            });
        }

        public void CreateEntry(OrderToRecord GoogleSheetRecord)
        {

            var range = "!A1:F1"; 

            var valueRange = new ValueRange();
            var oblist = new List<object>();
            valueRange.Range = range;
            valueRange.Values = new List<IList<object>> { oblist };
            float totalServiceDone = 0; 
           
            int RowsCount = ReadEntries(); // Fetch Rows Count Before Creation

            for (int i = 0; i < GoogleSheetRecord.Services.Count; i++)
            {
                if (i == 0)
                {
                    oblist.Add(GoogleSheetRecord.CustomerNameAR);
                    oblist.Add(GoogleSheetRecord.CustomerNameEN);
                    oblist.Add(GoogleSheetRecord.BarberNameAR);
                    oblist.Add(GoogleSheetRecord.BarberNameEN);
                    oblist.Add(GoogleSheetRecord.OrderStartTime.Value.ToString("MM/dd/yyyy hh:mm tt"));
                    oblist.Add(GoogleSheetRecord.OrderEndTime.Value.ToString("MM/dd/yyyy hh:mm tt"));
                    oblist.Add(GoogleSheetRecord.OrderTotalAmount);
                    oblist.Add(GoogleSheetRecord.DiscountPrice);
                    oblist.Add(GoogleSheetRecord.DiscountRate);
                    // shoud not be repeated 

                    oblist.Add(GoogleSheetRecord.Services[i].ServiceNameAR);
                    oblist.Add(GoogleSheetRecord.Services[i].ServiceNameEN);
                    oblist.Add(GoogleSheetRecord.Services[i].ServiceTime);
                    oblist.Add(GoogleSheetRecord.Services[i].ServicePrice);


                    if (GoogleSheetRecord.Services[i].ServiceStatus == true)
                    {
                        oblist.Add("Done");
                        totalServiceDone += (float)GoogleSheetRecord.Services[i].ServicePrice; 
                    }
                    else if (GoogleSheetRecord.Services[i].ServiceStatus == false)
                    {
                        oblist.Add("Canceled");
                    }

                }
               

                if (i>0 && GoogleSheetRecord.Services[i].ServiceNameAR != GoogleSheetRecord.Services[i-1].ServiceNameAR)
                {


                    oblist.Remove(GoogleSheetRecord.Services[i-1].ServiceNameAR);
                    oblist.Remove(GoogleSheetRecord.Services[i-1].ServiceNameEN);
                    oblist.Remove(GoogleSheetRecord.Services[i-1].ServiceTime);
                    oblist.Remove(GoogleSheetRecord.Services[i-1].ServicePrice);
                    if (GoogleSheetRecord.Services[i-1].ServiceStatus == true)
                    {
                        oblist.Remove("Done");
                    }
                    else if (GoogleSheetRecord.Services[i-1].ServiceStatus == false)
                    {
                        oblist.Remove("Canceled");
                    }

                    

                    oblist.Add(GoogleSheetRecord.Services[i].ServiceNameAR);
                    oblist.Add(GoogleSheetRecord.Services[i].ServiceNameEN);
                    oblist.Add(GoogleSheetRecord.Services[i].ServiceTime);
                    oblist.Add(GoogleSheetRecord.Services[i].ServicePrice);

                   
                    if (GoogleSheetRecord.Services[i].ServiceStatus == true)
                    {
                        oblist.Add("Done");
                        totalServiceDone += (float)GoogleSheetRecord.Services[i].ServicePrice;

                    }
                    else if (GoogleSheetRecord.Services[i].ServiceStatus == false)
                    {
                        oblist.Add("Canceled");
                    }
                   
                }
            
                var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendReponse = appendRequest.Execute();
               // oblist.Clear();

            }
         
                
            var userEnteredFormat =  new CellFormat();
            string DayName = DateTime.Now.DayOfWeek.ToString();
            if (DayName.Equals("Monday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red =(float) 0.074,
                        Green = (float)0.38 ,
                        Blue = (float)0.09 ,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if(DayName.Equals("Tuesday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red = (float)0.48,
                        Green = (float)0.082,
                        Blue = (float)0.070,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if (DayName.Equals("Wednesday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red = (float)0.075,
                        Green = (float)0.39,
                        Blue = (float)0.48,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if (DayName.Equals("Thursday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red = (float)0.48,
                        Green = (float)0.070,
                        Blue = (float)0.09,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if (DayName.Equals("Friday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red = (float)0.07,
                        Green = (float)0.48,
                        Blue = (float)0.39,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if (DayName.Equals("Saturday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Blue = 0,
                        Red = 1,
                        Green = 1,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            else if (DayName.Equals("Sunday"))
            {
                userEnteredFormat = new CellFormat()
                {

                    BackgroundColor = new Color()
                    {
                        Red = (float)0.40,
                        Green = (float)0.48,
                        Blue = (float)0.070,
                        Alpha = 1
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                };
            }
            //define cell color

            BatchUpdateSpreadsheetRequest bussr = new BatchUpdateSpreadsheetRequest();

           
           
            //create the update request for cells from the first row
            var updateCellsRequest = new Request()
            {

                RepeatCell = new RepeatCellRequest()
                {
                    //sheetId =  587278492, 
                    Range = new GridRange()
                    {
                        SheetId = 0,
                        StartRowIndex = RowsCount,
                        EndRowIndex = (GoogleSheetRecord.Services.Count + RowsCount),
                        StartColumnIndex = 0,
                        EndColumnIndex = 14
                    },
                    Cell = new CellData()
                    {
                        UserEnteredFormat = userEnteredFormat
                    },
                    Fields = "UserEnteredFormat(BackgroundColor,TextFormat)"
                }
            };
            bussr.Requests = new List<Request>();
            bussr.Requests.Add(updateCellsRequest);
            var bur = _sheetsService.Spreadsheets.BatchUpdate(bussr, _spreadsheetId);
            bur.Execute();
        }

        public int ReadEntries()
        {
            var range = "!A:F1";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);
            var rowCount = 0;
            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
               
                rowCount = values.Count;
                return rowCount;
            }
            else
            {
                return 0;
               
            }
        }
    }
}




























//Google Archive
//void UpdateEntry()
//{
//    var range = $"{sheet}!A:F";
//    var valueRange = new ValueRange();

//    var oblist = new List<object>() { "updated" };
//    valueRange.Values = new List<IList<object>> { oblist };

//    var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
//    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
//    var appendReponse = updateRequest.Execute();
//}

//public void ReadEntries()
//{
//    var range = $"{sheet}!A:F";
//    SpreadsheetsResource.ValuesResource.GetRequest request =
//            _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

//    var response = request.Execute();
//    IList<IList<object>> values = response.Values;
//    if (values != null && values.Count > 0)
//    {
//        foreach (var row in values)
//        {
//            // Print columns A to F, which correspond to indices 0 and 4.
//            Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}", row[0], row[1], row[2], row[3], row[4], row[5]);
//        }
//    }
//    else
//    {
//        Console.WriteLine("No data found.");
//    }
//}

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

