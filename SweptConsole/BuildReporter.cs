//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    public class BuildReporter
    {
        public BuildReporter()
        {
        }

        public string ReportOn( Dictionary<Change, List<IssueSet>> filesPerChange )
        {
            XDocument report_doc = new XDocument();
            XElement report_root = new XElement( "SweptBuildReport" );

            foreach (Change change in filesPerChange.Keys)
            {
                int totalTasksPerChange = 0;

                var change_element = new XElement( "Change",
                    new XAttribute( "ID", change.ID ),
                    new XAttribute("Description",change.Description)
                );

                foreach(IssueSet issueSet in filesPerChange[change])
                {
                    var source_file_element = new XElement("SourceFile",
                        new XAttribute("Name",issueSet.SourceFile.Name),
                        new XAttribute("TaskCount",issueSet.SourceFile.TaskCount));
                    totalTasksPerChange += issueSet.SourceFile.TaskCount;

                    change_element.Add( source_file_element );
                }

                change_element.Add( new XAttribute( "TotalTasks", totalTasksPerChange ) );

                report_root.Add( change_element );
            }

            report_doc.Add( report_root );
            return report_doc.ToString();
        }
    }
}
