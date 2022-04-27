using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SimTrixx.Reader.Concrete;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace SimTrixx.Client.Logic
{
   public class FileExportHandler
    {
        private BindingList<Word> _keywordsV2;

        public FileExportHandler()
        {
            _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;
        }
        public string LinesToDoc(List<SimTrixx.Reader.Concrete.Contract> lines)
        {
            try
            {
                _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;
                var htmlString = new StringBuilder();
                htmlString.Append("<html><head>");
                //htmlString.Append("<style>table{border-collapse:collapse;}th{border-right-color:black;border-right-style:solid;border-bottom-width:1.5pt;}td{border-right-color:black;border-right-style:solid;border-bottom-width:1.5pt;}tr{border-bottom-color:black;border-bottom-style:solid;border-bottom-width:1.5pt;" +
                //    "height:40px;background-color:white;color:black;}th{ text-align:left; background-color:White; color:black;font-weight:bold;" +
                //    " height: 40px; padding-left:5px}td{ padding-left:5px; }</style> ");
                htmlString.Append("<style>table{border-collapse:collapse;}th{border-color:black;border-style:solid;border-width:1.5pt;}td{border-color:black;border-style:solid;border-width:1.5pt;}tr{" +
                                "height:40px;background-color:white;color:black;}th{ text-align:center; background-color:White; color:black;font-weight:bold;" +
                                " height: 40px; padding-left:5px}td{ padding-left:5px; }</style> ");
                htmlString.Append("<body>");
                htmlString.Append("<table style='width:100%'><tr><th>Document Section</th><th>Line</th><th>Y/N</th><th>If yes, provide discriminator/past performance</th></tr>");
                foreach (var line in lines)
                {
                    var section = string.IsNullOrWhiteSpace(line.DocumentSection) ? "Empty" : line.DocumentSection;
                    //var lineType = string.IsNullOrWhiteSpace(line.DataType.ToString()) ? "Empty" : line.DataType.ToString();
                    var lineContent = string.IsNullOrWhiteSpace(line.Data) ? "Empty" : Microsoft.Security.Application.Encoder.HtmlEncode(line.Data);
                    htmlString.Append("<tr>");
                    htmlString.Append($"<td style='width:10%'>{section}</td>");
                    //htmlString.Append($"<td style='width:10%;font-weight:bold'>{lineType}</td>");
                    foreach (var keyword in _keywordsV2)
                    {
                        if (!keyword.Split)
                        {
                            //lineContent = lineContent.ToLower().Replace(keyword.Keyword.ToLower(), $"<span style = 'background-color: #FFFF00'>{keyword.Keyword.ToLower()}</span>");
                            lineContent = Regex.Replace(lineContent, keyword.Keyword, $"<span style = 'background-color: #FFFF00'>{keyword.Keyword.ToLower()}</span>", RegexOptions.IgnoreCase);
                        }
                    }
                    htmlString.Append($"<td style='width:70%'>{lineContent}</td>");
                    htmlString.Append($"<td style='width:5%'></td>");//Y/N column
                    htmlString.Append($"<td style='width:15%'></td>");//if yes column
                    htmlString.Append("</tr>");

                }
                htmlString.Append("</table></body></head></html>");
                return htmlString.ToString();
                //if (System.IO.File.Exists(outputDocumentPath))
                //{
                //    System.IO.File.Delete(outputDocumentPath);
                //}
                //System.IO.File.AppendAllText(outputDocumentPath, htmlString.ToString());
               
            }
            catch (Exception ex)
            {

                throw new Exception($"Error => Error while exporting information {Environment.NewLine} {ex.Message}");
            }

        }

        public void CreateWordDoc(List<SimTrixx.Reader.Concrete.Contract> lines,string filepath)
        {
            _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;

            var newLines = new Contract();
            //Replace Keywords
            foreach (var keyword in _keywordsV2)
            {
                if (!string.IsNullOrWhiteSpace(keyword.Replacement) && keyword.Replacement.ToLower() != "change me")
                {
                    foreach(var line in lines)
                    {
                        line.Data = Regex.Replace(line.Data, keyword.Keyword, keyword.Replacement, RegexOptions.IgnoreCase);
                    }
                }
            }

            var winword = new Microsoft.Office.Interop.Word.Application();
            winword.ShowAnimation = false;
            
            //Set status for word application is to be visible or not.  
            winword.Visible = false;

            //Create a missing variable for missing value  
            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            document.Content.SetRange(0, 0);
            
            Table firstTable = document.Tables.Add(document.Range(0,0),lines.Count +1, 4, ref missing, ref missing);
            firstTable.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
            firstTable.PreferredWidth = 100;
            //firstTable.Columns[1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            //firstTable.Columns[1].PreferredWidth = 10;

            //firstTable.Columns[2].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            //firstTable.Columns[2].PreferredWidth = 70;

            //firstTable.Columns[3].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            //firstTable.Columns[3].PreferredWidth = 5;

            //firstTable.Columns[4].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
            //firstTable.Columns[4].PreferredWidth = 15;


            //firstTable.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
            document.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
            firstTable.PreferredWidth = document.PageSetup.PageWidth - (document.PageSetup.LeftMargin + document.PageSetup.RightMargin);
            firstTable.Borders.Enable = 1;
            var rowCount = 1;
            firstTable.Rows[1].HeadingFormat = -1;
            foreach (Row row in firstTable.Rows)
            {
                
                foreach (Cell cell in row.Cells)
                {
                    cell.WordWrap = true;
                    //cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
                    if (cell.RowIndex == 1)
                    {

                        cell.Range.Font.Bold = 1;

                        if (cell.ColumnIndex == 1)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 10;
                            cell.Range.Text = "Section";
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            
                        }
                        if (cell.ColumnIndex == 2)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 70;
                            cell.Range.Text = "Contents";
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                        if (cell.ColumnIndex == 3)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 5;
                            cell.Range.Text = "Y/N";
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                        if (cell.ColumnIndex == 4)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 15;
                            cell.Range.Text = "If yes, provide discriminator/past performance";
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }

                    }
                    else
                    {


                        if (cell.ColumnIndex == 1)
                        {
                            cell.Range.Text = string.IsNullOrWhiteSpace(lines[row.Index -2].DocumentSection) ? "Empty" : lines[row.Index-2].DocumentSection;
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        }
                        if (cell.ColumnIndex == 2)
                        {
                            cell.Range.Text = lines[row.Index-2].Data;
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 70;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                        if (cell.ColumnIndex == 3)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 5;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                        if (cell.ColumnIndex == 4)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 15;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                 
                    }
                }
               
                if(rowCount == 1)
                {
                    row.Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    row.Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    row.Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    row.Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;


                    row.Cells[1].Range.Shading.BackgroundPatternColor = WdColor.wdColorDarkBlue;
                    row.Cells[2].Range.Shading.BackgroundPatternColor = WdColor.wdColorDarkBlue;
                    row.Cells[3].Range.Shading.BackgroundPatternColor = WdColor.wdColorDarkBlue;
                    row.Cells[4].Range.Shading.BackgroundPatternColor = WdColor.wdColorDarkBlue;
                }
                else
                {
                    row.Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    if(rowCount == 2)
                    {
                        //row.Cells[2].Merge(row.Cells[4]);
                        //row.Cells[2].Merge(row.Cells[4]);
                    }
                }
                rowCount++;
            }

           
            //Keyword Highlighting
            foreach (var keyword in _keywordsV2)
            {
                var rng = document.Range();
                if (!string.IsNullOrWhiteSpace(keyword.Replacement) && keyword.Replacement.ToLower() != "change me")
                {
                    rng.Find.Text = keyword.Replacement;
                }
                else
                {
                    rng.Find.Text = keyword.Keyword;
                }
                rng.Find.MatchCase = false;

                while (rng.Find.Execute(Forward: true))
                {
                    rng.Font.Bold = 1;
                    rng.HighlightColorIndex = WdColorIndex.wdYellow;
                    
                }
            }

            object filename = filepath;
            document.SaveAs2(ref filename);
            document.Close(ref missing, ref missing, ref missing);
            document = null;
            winword.Quit(ref missing, ref missing, ref missing);
            winword = null;

        }

        public void CreateExcelDoc(List<SimTrixx.Reader.Concrete.Contract> lines, string filepath)
        {
   
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workBook;
            Microsoft.Office.Interop.Excel.Worksheet workSheet;
            Microsoft.Office.Interop.Excel.Range cellRange;
            excel = new Microsoft.Office.Interop.Excel.Application();

            var numberDropDown = new List<string>();
            numberDropDown.Add("1");
            numberDropDown.Add("2");
            numberDropDown.Add("3");
            numberDropDown.Add("4");
            numberDropDown.Add("5");
            var flatList = string.Join(",", numberDropDown.ToArray());

            try
            {
                
                excel.Visible = false;
                excel.DisplayAlerts = false;
                workBook = excel.Workbooks.Add(Type.Missing);


                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;
                workSheet.Name = "SimTrixx Matrix";

                workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 2]].Merge();
                workSheet.Cells[1, 1] = $"SimTrixx Simple Matrix - {System.IO.Path.GetFileName(filepath)}";
                workSheet.Cells.Font.Size = 15;
                workSheet.Cells[2, 1] = "Section";
                workSheet.Cells[2, 2] = "Data";
                workSheet.Cells[2, 3] = "Y / N";
                workSheet.Cells[2, 4] = "Capabilities Ranking";
                workSheet.Cells[2, 5] = "Provide discriminator/past performance";
                int rowcount = 2;
                foreach (var line in lines)
                {
                    rowcount += 1;
                    if (rowcount > 2)
                    {
                        workSheet.Cells[rowcount, 1] = line.DocumentSection;

                        //Check for keyword replacements
                        foreach (var keyword in _keywordsV2)
                        {

                            if (!string.IsNullOrWhiteSpace(keyword.Replacement) && keyword.Replacement.ToLower() != "change me")
                            {
                                line.Data = Regex.Replace(line.Data, keyword.Keyword, keyword.Replacement, RegexOptions.IgnoreCase);
                            }
   
                        }
                        workSheet.Cells[rowcount, 2] = line.Data;

                        //Add Color Dropdown Menu
                        var cell = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowcount, 4];
                        cell.Validation.Delete();
                        cell.Validation.Add(
                           XlDVType.xlValidateList,
                           XlDVAlertStyle.xlValidAlertInformation,
                           XlFormatConditionOperator.xlBetween,
                           flatList,
                           Type.Missing);

                        cell.Validation.IgnoreBlank = true;
                        cell.Validation.InCellDropdown = true;
                    }
                }

                //Global Style
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, 5]];
                cellRange.EntireColumn.AutoFit();
                cellRange.EntireColumn.WrapText = true;
                Microsoft.Office.Interop.Excel.Borders border = cellRange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                //Style Sheet Header
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 5]];
                cellRange.Interior.Color = System.Drawing.Color.Yellow;
                cellRange.Font.Bold = true;
           

                //Style Column Headers
                cellRange = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[2,5]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                cellRange.Font.Color = System.Drawing.Color.White;
                cellRange.Font.Bold = true;
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //Center left column
                cellRange = workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[rowcount, 1]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //Center Color Column
                cellRange = workSheet.Range[workSheet.Cells[3, 4], workSheet.Cells[rowcount, 4]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //*************************************************************************************

                FormatCondition format = (FormatCondition)(workSheet.get_Range($"D3:D{rowcount}",
                    Type.Missing).FormatConditions.Add(XlFormatConditionType.xlExpression,
                                                       XlFormatConditionOperator.xlEqual,
                                                       "=D3=1",
                                                       Type.Missing, Type.Missing, Type.Missing,
                                                       Type.Missing, Type.Missing));

                    format.Font.Bold = true;
                    format.Interior.Color = 0x000000FF; //Red
                    FormatCondition format2 = (FormatCondition)(workSheet.get_Range($"D3:D{rowcount}",
                    Type.Missing).FormatConditions.Add(XlFormatConditionType.xlExpression,
                                                       XlFormatConditionOperator.xlEqual,
                                                       "=$D3=2",
                                                       Type.Missing, Type.Missing, Type.Missing,
                                                       Type.Missing, Type.Missing));

                    format2.Font.Bold = true;
                    format2.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);

                    FormatCondition format3 = (FormatCondition)(workSheet.get_Range($"D3:D{rowcount}",
                    Type.Missing).FormatConditions.Add(XlFormatConditionType.xlExpression,
                                                       XlFormatConditionOperator.xlEqual,
                                                       "=$D3=3",
                                                       Type.Missing, Type.Missing, Type.Missing,
                                                       Type.Missing, Type.Missing));

                    format3.Font.Bold = true;
                    format3.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);

                    FormatCondition format4 = (FormatCondition)(workSheet.get_Range($"D3:D{rowcount}",
                    Type.Missing).FormatConditions.Add(XlFormatConditionType.xlExpression,
                                                       XlFormatConditionOperator.xlEqual,
                                                       "=$D3=4",
                                                       Type.Missing, Type.Missing, Type.Missing,
                                                       Type.Missing, Type.Missing));

                    format4.Font.Bold = true;
                    format4.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);

                    FormatCondition format5 = (FormatCondition)(workSheet.get_Range($"D3:D{rowcount}",
                    Type.Missing).FormatConditions.Add(XlFormatConditionType.xlExpression,
                                                       XlFormatConditionOperator.xlEqual,
                                                       "=$D3=5",
                                                       Type.Missing, Type.Missing, Type.Missing,
                                                       Type.Missing, Type.Missing));

                    format5.Font.Bold = true;
                    format5.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Orange);


                //*************************************************************************************

                //foreach (var keyword in _keywordsV2)
                //{
                //    var rng = (Microsoft.Office.Interop.Excel.Range)workSheet.UsedRange;
                //    if (!string.IsNullOrWhiteSpace(keyword.Replacement) && keyword.Replacement.ToLower() != "change me")
                //    {
                //        bool success = (bool)rng.Replace(keyword.Keyword, keyword.Replacement, XlLookAt.xlWhole, XlSearchOrder.xlByRows, false);
                //    }
                //}

                workBook.SaveAs(filepath);
                workBook.Close();
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                excel.Quit();
                workSheet = null;
                workBook = null;
                cellRange = null;
                excel = null;
            }

        }

        public void CreateExcelAbbrviationDoc(List<SimTrixx.Reader.Concrete.AbbrvContainer> abbreviationList,string filepath)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workBook;
            Microsoft.Office.Interop.Excel.Worksheet workSheet;
            Microsoft.Office.Interop.Excel.Range cellRange;
            excel = new Microsoft.Office.Interop.Excel.Application();

            //var numberDropDown = new List<string>();
            //numberDropDown.Add("1");
            //numberDropDown.Add("2");
            //numberDropDown.Add("3");
            //numberDropDown.Add("4");
            //numberDropDown.Add("5");
            //var flatList = string.Join(",", numberDropDown.ToArray());

            try
            {

                excel.Visible = false;
                excel.DisplayAlerts = false;
                workBook = excel.Workbooks.Add(Type.Missing);


                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;
                workSheet.Name = "SimTrixx - Acronym Export";

                workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 2]].Merge();
                workSheet.Cells[1, 1] = $"SimTrixx Acronym Export - {System.IO.Path.GetFileName(filepath)}";
                workSheet.Cells.Font.Size = 10;
                workSheet.Cells[2, 1] = "Acronym";
                workSheet.Cells[2, 2] = "Definition";
                workSheet.Cells[2, 3] = "Location";
                workSheet.Cells[2, 4] = "Count";
                int rowcount = 2;
                foreach (var abbrv in abbreviationList)
                {
                    foreach (var loc in abbrv.Location)
                    {
                        rowcount += 1;
                        if (rowcount > 2)
                        {
                            workSheet.Cells[rowcount, 1] = abbrv.Abbrv;
                            workSheet.Cells[rowcount, 2] = abbrv.Definition;
                            workSheet.Cells[rowcount, 3] = loc;
                            workSheet.Cells[rowcount, 4] = abbrv.Count;

                            var abbrvIndex = loc.IndexOf(abbrv.Abbrv);
                           
                            workSheet.Cells[rowcount, 3].Font.FontStyle = "Regular";
                            workSheet.Cells[rowcount, 3].Characters(abbrvIndex+1, abbrv.Abbrv.Length).Font.FontStyle = "Bold";

                        }
                    }
                }

                //Global Style
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, 4]];
                cellRange.EntireColumn.AutoFit();
                cellRange.EntireColumn.WrapText = true;
                Microsoft.Office.Interop.Excel.Borders border = cellRange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                //Style Sheet Header
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 4]];
                cellRange.Interior.Color = System.Drawing.Color.Yellow;
                cellRange.Font.Bold = true;


                //Style Column Headers
                cellRange = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[2, 4]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                cellRange.Font.Color = System.Drawing.Color.White;
                cellRange.Font.Bold = true;
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //Center Acronym column
                cellRange = workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[rowcount, 1]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //vCenter Definition column
                cellRange = workSheet.Range[workSheet.Cells[3, 2], workSheet.Cells[rowcount, 2]];
                //cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //Center Color Column
                cellRange = workSheet.Range[workSheet.Cells[3, 4], workSheet.Cells[rowcount, 4]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                workBook.SaveAs(filepath);
                workBook.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                excel.Quit();
                workSheet = null;
                workBook = null;
                cellRange = null;
                excel = null;
            }
        }
    }
}
