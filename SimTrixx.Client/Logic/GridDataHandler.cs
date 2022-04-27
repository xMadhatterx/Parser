using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using SimTrixx.Reader.Concrete;

namespace SimTrixx.Client.Logic
{
   public class GridDataHandler
    {
        private BindingList<Word> _keywordsV2;
        public string BuildHtmlString(List<SimTrixx.Reader.Concrete.Contract> lines)
        {
            try
            {
                _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;
                var htmlString = new StringBuilder();
                htmlString.Append("<html><head>");
                htmlString.Append("<style>table{border-collapse:collapse;}"+
                                    "th{border-color:black;border-style:solid;border-width:1.5pt;text-align:center; background-color:rgb(73, 106, 129); color:white;font-weight:bold;height: 40px; padding-left:5px}" +
                                    "td{border-color:black;border-style:solid;border-width:1.5pt;padding-left:5px;font-size:12px;font-family:monospace }" +
                                    "tr{height:40px;background-color:white;color:black;}</style>");
                htmlString.Append("<body>");
                htmlString.Append("<table style='width:100%'><tr><th>Section</th><th>Section Data</th><th>Y/N</th><th>If yes, provide discriminator/past performance</th></tr>");
                foreach (var line in lines)
                {
                    var section = string.IsNullOrWhiteSpace(line.DocumentSection) ? "Empty" : line.DocumentSection;
                    var lineContent = string.IsNullOrWhiteSpace(line.Data) ? "Empty" : Microsoft.Security.Application.Encoder.HtmlEncode(line.Data);
                    htmlString.Append("<tr>");
                    htmlString.Append($"<td style='width:10%;text-align:center'>{section}</td>");
                    foreach (var keyword in _keywordsV2)
                    {

                        if (!keyword.Split)
                        {
                            if (!string.IsNullOrEmpty(keyword.Replacement) && keyword.Replacement != "Change Me")
                            {
                                lineContent = Regex.Replace(lineContent, keyword.Keyword, $"<span style = 'background-color: #FFFF00'>{keyword.Replacement}</span>", RegexOptions.IgnoreCase);
                            }
                            else
                            {
                                lineContent = Regex.Replace(lineContent, keyword.Keyword, $"<span style = 'background-color: #FFFF00'>{keyword.Keyword.ToLower()}</span>", RegexOptions.IgnoreCase);
                            }
                        }
                        else
                        {
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
    }
}
