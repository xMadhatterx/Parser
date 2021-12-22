using System;
using System.Collections.Generic;
using System.Text;
using SimTrixx.Reader.Concrete;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
namespace TestDocReader.Logic
{
   public class FileExportHandler
    {
        private List<Word> _keywordsV2;
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


                    //row.Cells[1].Range.Shading.BackgroundPatternColor = WdColor.wdColorLightBlue;
                    //row.Cells[2].Range.Shading.BackgroundPatternColor = WdColor.wdColorLightBlue;
                    //row.Cells[3].Range.Shading.BackgroundPatternColor = WdColor.wdColorLightBlue;
                    //row.Cells[4].Range.Shading.BackgroundPatternColor = WdColor.wdColorLightBlue;
                }
                else
                {
                    row.Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
                rowCount++;
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
                int rowcount = 2;
                foreach (var line in lines)
                {
                    rowcount += 1;
                    if (rowcount > 2)
                    {
                        workSheet.Cells[rowcount, 1] = line.DocumentSection;
                        workSheet.Cells[rowcount, 2] = line.Data;
                    }
                }
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, 2]];
                cellRange.EntireColumn.AutoFit();
                cellRange.EntireColumn.WrapText = true;
                Microsoft.Office.Interop.Excel.Borders border = cellRange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                //Style Sheet Header
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 2]];
                cellRange.Interior.Color = System.Drawing.Color.Yellow;
                cellRange.Font.Bold = true;

                //Style Column Headers
                cellRange = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[2,2]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                cellRange.Interior.Color = System.Drawing.Color.DarkGray;
                cellRange.Font.Color = System.Drawing.Color.White;
                cellRange.Font.Bold = true;
                //Center left column
                cellRange = workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[rowcount, 1]];
                cellRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

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
    }
}
