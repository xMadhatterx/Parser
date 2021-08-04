using System;
using System.Collections.Generic;
using System.Text;
using ContractReaderV2.Concrete;
using Microsoft.Office.Interop.Word;
namespace TestDocReader.Logic
{
   public class FileExportHandler
    {
        private List<Word> _keywordsV2;
        public string LinesToDoc(List<ContractReaderV2.Concrete.Contract> lines)
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
                    var lineType = string.IsNullOrWhiteSpace(line.DataType.ToString()) ? "Empty" : line.DataType.ToString();
                    var lineContent = string.IsNullOrWhiteSpace(line.Data) ? "Empty" : line.Data;
                    htmlString.Append("<tr>");
                    htmlString.Append($"<td style='width:10%'>{section}</td>");
                    //htmlString.Append($"<td style='width:10%;font-weight:bold'>{lineType}</td>");
                    foreach (var keyword in _keywordsV2)
                    {
                        if (!keyword.Split)
                        {
                            lineContent = lineContent.ToLower().Replace(keyword.Keyword.ToLower(), $"<span style = 'background-color: #FFFF00'>{keyword.Keyword.ToLower()}</span>");
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

        public void CreateWordDoc(List<ContractReaderV2.Concrete.Contract> lines,string filepath)
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
            firstTable.PreferredWidth = document.PageSetup.PageWidth - (document.PageSetup.LeftMargin + document.PageSetup.RightMargin);
            firstTable.Borders.Enable = 1;
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
                            cell.Range.Text = "Document Section";
                        }
                        if (cell.ColumnIndex == 2)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 70;
                            cell.Range.Text = "Contents";
                        }
                        if (cell.ColumnIndex == 3)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 5;
                            cell.Range.Text = "Y/N";
                        }
                        if (cell.ColumnIndex == 4)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 15;
                            cell.Range.Text = "If yes, provide discriminator/past performance";
                        }

                    }
                    else
                    {


                        if (cell.ColumnIndex == 1)
                        {
                            //if(cell.RowIndex>2)
                            //{
                            //    if (lines[row.Index].DocumentSection < lines[row.Index - 1].DocumentSection)
                            //    {

                            //    }
                            //}
                            cell.Range.Text = string.IsNullOrWhiteSpace(lines[row.Index -2].DocumentSection) ? "Empty" : lines[row.Index-2].DocumentSection;
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 10;

                        }
                        if (cell.ColumnIndex == 2)
                        {
                            cell.Range.Text = lines[row.Index-2].Data;
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 70;
                        }
                        if (cell.ColumnIndex == 3)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 5;
                        }
                        if (cell.ColumnIndex == 4)
                        {
                            cell.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                            cell.PreferredWidth = 15;
                        }
                    }
                }
            }

            object filename = filepath;
            document.SaveAs2(ref filename);
            document.Close(ref missing, ref missing, ref missing);
            document = null;
            winword.Quit(ref missing, ref missing, ref missing);
            winword = null;

        }
    }
}
