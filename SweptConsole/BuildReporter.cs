﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    public class BuildReporter
    {
        public string ReportOn( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> filesPerChange )
        {
            XDocument report_doc = new XDocument();
            XElement report_root = new XElement( "SweptBuildReport" );

            //TODO Goal code:
            //  var failures_element = GetFailureReportElement( failures );
            //  report_root.Add( failures_element );

            int totalTasks = 0;
            foreach (Change change in filesPerChange.Keys.OrderBy( c => c.ID ))
            {
                int totalTasksPerChange = 0;

                var change_element = new XElement( "Change",
                    new XAttribute( "ID", change.ID ),
                    new XAttribute("Description",change.Description)
                );

                var fileMatches = filesPerChange[change];
                foreach (SourceFile file in fileMatches.Keys.OrderBy( file => file.Name ))
                {
                    var match = fileMatches[file];
                    var tasks = Task.FromMatch( match, change, file );

                    if (tasks.Count == 0)
                        continue;

                    totalTasksPerChange += tasks.Count;

                    var file_tasks = new XElement( "SourceFile",
                        new XAttribute( "Name", file.Name ),
                        new XAttribute( "TaskCount", tasks.Count )
                    );
                    change_element.Add( file_tasks );
                }

                change_element.Add( new XAttribute( "TotalTasks", totalTasksPerChange ) );
                totalTasks += totalTasksPerChange;
                report_root.Add( change_element );
            }

            report_root.Add( new XAttribute( "TotalTasks", totalTasks ) );
            report_doc.Add( report_root );
            return report_doc.ToString();
        }

        public string ReportBuildFailures( List<string> failures )
        {
            if (failures.Count == 0)
                return string.Empty;
            else
            {
                string failureMessage = "Swept failed due to build breaking rule failure{0}:\n{1}";
                string failuresText = "";
                foreach (string fail in failures)
                {
                    failuresText += fail + "\n";
                }

                var plurality = failures.Count > 1 ? "s" : "";

                return string.Format( failureMessage, plurality, failuresText );
            }
        }

        public XElement GenerateBuildFailureXML( List<string> failures )
        {
            XElement xml = new XElement( "SweptBuildFailures" );

            foreach (string fail in failures)
            {
                XElement xmlFailure = new XElement( "SweptBuildFailure", fail );
                xml.Add( xmlFailure );
            }

            return xml;
        }

    }
}
