using System;
using System.Collections.Generic;
using System.Text;

namespace TestDocReader.Logic
{
   public class FileExportHandler
    {
        public void LinesToDoc(List<ContractReaderV2.Concrete.Contract> lines,string outputDocumentPath)
        {
            try
            {
                var htmlString = new StringBuilder();
                htmlString.Append("<html><head>");
                //htmlString.Append("<style>table{border-collapse:collapse;}th{border-right-color:black;border-right-style:solid;border-bottom-width:1.5pt;}td{border-right-color:black;border-right-style:solid;border-bottom-width:1.5pt;}tr{border-bottom-color:black;border-bottom-style:solid;border-bottom-width:1.5pt;" +
                //    "height:40px;background-color:white;color:black;}th{ text-align:left; background-color:White; color:black;font-weight:bold;" +
                //    " height: 40px; padding-left:5px}td{ padding-left:5px; }</style> ");
                htmlString.Append("<style>table{border-collapse:collapse;}th{border-color:black;border-style:solid;border-width:1.5pt;}td{border-color:black;border-style:solid;border-width:1.5pt;}tr{" +
                                "height:40px;background-color:white;color:black;}th{ text-align:center; background-color:White; color:black;font-weight:bold;" +
                                " height: 40px; padding-left:5px}td{ padding-left:5px; }</style> ");
                htmlString.Append("<body>");
                htmlString.Append("<table style='width:100%'><tr><th>Document Section</th><th>Line Type</th><th>Line</th><th>Y/N</th><th>If yes, provide descriminator/past performance</th></tr>");
                foreach (var line in lines)
                {
                    var section = string.IsNullOrWhiteSpace(line.DocumentSection) ? "Empty" : line.DocumentSection;
                    var lineType = string.IsNullOrWhiteSpace(line.DataType.ToString()) ? "Empty" : line.DataType.ToString();
                    var lineContent = string.IsNullOrWhiteSpace(line.Data) ? "Empty" : line.Data;
                    htmlString.Append("<tr>");
                    htmlString.Append($"<td style='width:10%'>{section}</td>");
                    htmlString.Append($"<td style='width:10%;font-weight:bold'>{lineType}</td>");
                    htmlString.Append($"<td style='width:60%'>{lineContent}</td>");
                    htmlString.Append($"<td style='width:5%'></td>");//Y/N column
                    htmlString.Append($"<td style='width:15%'></td>");//if yes column
                    htmlString.Append("</tr>");

                }
                htmlString.Append("</table></body></head></html>");
                if (System.IO.File.Exists(outputDocumentPath))
                {
                    System.IO.File.Delete(outputDocumentPath);
                }
                System.IO.File.AppendAllText(outputDocumentPath, htmlString.ToString());
               
            }
            catch (Exception ex)
            {

                throw new Exception($"Error => Error while exporting information {Environment.NewLine} {ex.Message}");
            }

        }
    }
}
