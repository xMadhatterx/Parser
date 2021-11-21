using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Reader.Concrete;
using System.Text.RegularExpressions;
namespace ContractReaderV2.Handlers
{
    public class SectionHandler
    {
        
        private bool _inActiveSection = false;
        private bool _atleastOne = false;
        public List<Contract> GetSections(List<string> lines, int lineAmount,bool advancedFiltering)
        {
            var _lastSectionId = string.Empty;
           var contractLines = new List<Contract>();
            var lineCount = 0;
            var contract = new Contract();
            bool sameSection = false;

            while (true)
            {
                //If we are on the last line then wrap it up.
                if (lineCount == lineAmount)
                {
                    if (!string.IsNullOrEmpty(contract.Data))
                    {
                        contractLines.Add(contract);
                    }
                    return contractLines;
                }

                //If we are 1 past the last line then wrap it up
                if (lineCount > lineAmount) return contractLines;

                var lineData = lines[lineCount];
                if (!string.IsNullOrEmpty(lineData))
                {
                    //Remove \f from doc
                    if (lineData.StartsWith("\f"))
                    {
                        lineData = lineData.Replace("\f", "");
                    }

                    //Remove \t from doc
                    if (lineData.StartsWith("\t"))
                    {
                        lineData = lineData.Replace("\t", "");
                    }

                    //Second check to replace any mid line tabs with spaces.
                    if (lineData.Contains("\t"))
                    {
                        lineData = lineData.Replace("\t", " ");
                    }

                    //Get section is we have one on this line
                    var section = GetNewDocumentSection(lineData);

                    //Work out section magic
                    sameSection = false;
                    if (!string.IsNullOrWhiteSpace(section.Trim()) && section != "consolidate")
                    {
                        //Section Found
                        //-------------
                        //Check if we have a previous section id
                        if (!string.IsNullOrEmpty(_lastSectionId))
                        {
                            //If we do compare the current section to the last section
                            if (section == _lastSectionId)
                            {
                                //boom - same section
                                sameSection = true;
                            }
                            else
                            {
                                try
                                {
                                    var workingSection = section;
                                    var workingLastSection = _lastSectionId;
                                    if (workingSection.Length == 1) workingSection = workingSection + ".0";
                                    if (workingLastSection.Length == 1) workingLastSection = workingLastSection + ".0";
                                    var newSection = new Version(workingSection);
                                    var lastSection = new Version(workingLastSection);

                                    var result = newSection.CompareTo(lastSection);
                                    if (result > 0)
                                        _lastSectionId = section.Trim();
                                    else if (result < 0)
                                        sameSection = true;
                                    else
                                        sameSection = true;
                                }
                                catch
                                {
                                    var newSection = section.Replace(".", "");
                                    var newLastSection = _lastSectionId.Replace(".", "");
                                    Int32.TryParse(newSection, out int newSectionDecimal);
                                    Int32.TryParse(newLastSection, out int lastSectionDecimal);


                                        if (newSectionDecimal < lastSectionDecimal)
                                        {
                                            sameSection = true;
                                        }
                                        else
                                        {
                                            //New Section, let's set our sectionid
                                            _lastSectionId = section.Trim();
                                        }
                            
                                }

                            }
                        }
                        else
                        {
                            //New Section, let's set our _lastSectionId
                            _lastSectionId = section.Trim();
                        }
                    }
                    else
                    {
                        //Section Not Found
                        //-----------------
                        //Check if we are consolidating
                        if (section == "consolidate")
                        {
                            _atleastOne = true;
                            contractLines.Add(contract);
                            contract = new Contract();
                        }
                        else
                        {
                            //Check if we have a previous section id
                            if (!string.IsNullOrEmpty(_lastSectionId) && _inActiveSection)
                            {
                                //We have a previous section, let's use that
                                sameSection = true;
                            }
                            else
                            {
                                _lastSectionId = section.Trim();
                            }
                        }
                    }

                    //Remove Section from current line
                    if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(lineData) && lineData.Contains(section))
                    {
                        lineData = lineData.Remove(0, section.Length + 1);
                    }

                    //Lets check and see if we are in a new section or a continuation section
                    if (sameSection)
                    {
                        //Same section as previous
                        //------------------------
                        if (!string.IsNullOrEmpty(lineData))
                            contract.Data += lineData;

                    }
                    else
                    {
                        //New section
                        //------------
                        //If we have data sitting in contract we need to add it to the linedata and create a new instance.
                        if (!string.IsNullOrEmpty(contract.Data))
                        {
                            contractLines.Add(contract);
                            contract = new Contract();
                        }
                        //Add line data to contract
                        if (!string.IsNullOrEmpty(_lastSectionId) && section != "consolidate" && _inActiveSection)
                        {
                            contract.DocumentSection = _lastSectionId;
                            contract.Data = lineData;
                        }
                    }
                }
                lineCount++;
            }
        }

        private string GetNewDocumentSection(string line)
        {
            var section = string.Empty;
            bool dotPresent = false;

            var match = false;
            var leadingLetter = false;
            var sectionChecker = new Regex(@"(?m)^\d+(?:\.\d+)*[ \t]+\S.*$");
            var sectionCheckerTrailing = new Regex(@"(?m)^\d+(?:\.\d+)*\S[ \t]+\S.*$");
            var sectionCheckerLeading = new Regex(@"(?m)^\S.\d+(?:\.\d+)*[ \t]+\S.*$");

            if (sectionChecker.IsMatch(line))
            {
                match = true;
            }
            else if (sectionCheckerTrailing.IsMatch(line))
            {
                match = true;
            }
            else if (sectionCheckerLeading.IsMatch(line))
            {
                match = true;
                leadingLetter = true;
            }

            if (!match) return section;
            if (!match && !_inActiveSection) return section;

            foreach (var t in line)
            {
                if (leadingLetter)
                {
                    section += t.ToString();
                    leadingLetter = false;
                }
                else
                {
                    if (t.ToString() == " ")
                    {
                        if (_inActiveSection)
                        {
                            if (!string.IsNullOrEmpty(section) && section.ToString() != " ")
                            {
                                //We hit the next section after a section was filled in
                                //_inActiveSection = false;
                                return section;
                                //return "consolidate";
                            }
                            else
                            {
                                _inActiveSection = false;
                                return "consolidate";
                            }
                        }
                        _inActiveSection = true;
                        if (dotPresent)
                        {
                            return section;
                        }
                        else
                        {
                            return section;
                        }
                    }
                    if (t.ToString() == ".")
                        dotPresent = true;
                    var isNumber = int.TryParse(t.ToString(), out _);
                    if (isNumber || t.ToString() == ".")
                    {
                        section += t.ToString();
                    }
                }
            }

            if (dotPresent)
            {
                return section;
            }
            else
            {
                return "";
            }
        }
    }
}
