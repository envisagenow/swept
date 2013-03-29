//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    public class BuildReporter
    {
        public XElement GenerateBuildDeltaXml( BreakageDelta delta )
        {
            XElement xml = new XElement( "SweptBuildDeltas" );

            foreach (string fail in delta.Failures)
            {
                XElement xmlFailure = new XElement( "SweptBuildFailure", fail );
                xml.Add( xmlFailure );
            }

            foreach (string fix in delta.Fixes)
            {
                XElement xmlFix = new XElement( "SweptBuildFix", fix );
                xml.Add( xmlFix );
            }

            return xml;
        }

        //public string ReportOn( RuleTasks ruleTasks, List<string> failures )
        //{
        //    if (_args.Check)
        //        return ReportCheckResult( failures );
        //    else
        //        return ReportDetailsXml( ruleTasks );
        //}

        public string ReportFailures( List<string> failures )
        {
            if (failures.Count == 0)
                return string.Empty;
            else
                return "Error:  " + string.Join( "\r\nError:  ", failures.ToArray() ) + "\r\n";
        }

        public string ReportCheckResult( List<string> failures )
        {
            if (!failures.Any())
                return "Swept check passed!\r\n";
            else
                return "Swept check failed!\r\n";
        }

        public string ReportDetailsXml( RuleTasks ruleTasks )
        {
            XDocument report_doc = new XDocument();
            XElement report_root = new XElement( "SweptBuildReport" );

            //  TODO: Get failure details into this XML report
            // Goal code:
            //  var failures_element = GetFailureReportElement( failures );
            //  report_root.Add( failures_element );

            int totalTasks = 0;
            foreach (Rule rule in ruleTasks.Keys.OrderBy( c => c.ID ))
            {
                int totalTasksPerRule = 0;

                var rule_element = new XElement( "Rule",
                    new XAttribute( "ID", rule.ID ),
                    new XAttribute( "Description", rule.Description )
                );

                var fileMatches = ruleTasks[rule];
                foreach (SourceFile file in fileMatches.Keys.OrderBy( file => file.Name ))
                {
                    var match = fileMatches[file];
                    var tasks = Task.FromMatch( match, rule, file );

                    if (tasks.Count == 0)
                        continue;

                    totalTasksPerRule += tasks.Count;

                    var file_tasks = new XElement( "SourceFile",
                        new XAttribute( "Name", file.Name ),
                        new XAttribute( "TaskCount", tasks.Count )
                    );
                    rule_element.Add( file_tasks );
                }

                rule_element.Add( new XAttribute( "TotalTasks", totalTasksPerRule ) );
                totalTasks += totalTasksPerRule;
                report_root.Add( rule_element );
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
                var failuresText = string.Join( "\n", failures.ToArray() );

                string failureMessage = "Swept failed due to build breaking rule {0}:\n{1}\n";
                return string.Format( failureMessage, "failure".Plurs( failures.Count ), failuresText );
            }
        }

    }
}
