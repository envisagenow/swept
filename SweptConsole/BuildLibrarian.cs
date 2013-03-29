//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace swept
{
    public class BuildLibrarian
    {
        private readonly IStorageAdapter _storage;
        private readonly Arguments _args;

        private RunHistoryEntry _latestPassingRun = null;

        public BuildLibrarian( Arguments args, IStorageAdapter storage )
        {
            _args = args;
            _storage = storage;
        }


        public RunHistory ReadRunHistory()
        {
            XDocument doc;
            try
            {
                doc = _storage.LoadRunHistory( _args.History );
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument();
            }
            return ReadRunHistory( doc );
        }

        public RunHistory ReadRunHistory( XDocument historyXml )
        {
            RunHistory runHistory = new RunHistory();

            foreach (var runXml in historyXml.Descendants( "Run" ))
            {
                var run = new RunHistoryEntry
                {
                    Number = int.Parse( runXml.Attribute( "Number" ).Value ),
                    Date = DateTime.Parse( runXml.Attribute( "DateTime" ).Value ),
                    Passed = Boolean.Parse( runXml.Attribute( "Passed" ).Value )
                };

                foreach (var ruleXml in runXml.Descendants( "Rule" ))
                {
                    string ruleID = ruleXml.Attribute( "ID" ).Value;

                    int ruleViolations = int.Parse( ruleXml.Attribute( "Violations" ).Value );
                    int rulePrior = int.Parse( ruleXml.Attribute( "Prior" ).Value );
                    bool ruleBreaking = bool.Parse( ruleXml.Attribute( "Breaking" ).Value );
                    RuleFailOn ruleFailOn = (RuleFailOn)Enum.Parse( typeof( RuleFailOn ), ruleXml.Attribute( "FailOn" ).Value );
                    run.RuleResults[ruleID] = new HistoricRuleResult
                    {
                        ID = ruleID,
                        Violations = ruleViolations,
                        Prior = rulePrior,
                        FailOn = ruleFailOn,
                        Breaking = ruleBreaking
                    };
                }

                runHistory.AddEntry( run );

                if (run.Passed)
                    _latestPassingRun = run;
            }

            return runHistory;
        }

        public void WriteRunHistory( RunHistory runHistory )
        {
            var report = new XDocument();

            XElement report_root = new XElement( "RunHistory" );
            report.Add( report_root );

            foreach (var run in runHistory.Runs)
            {
                var runElement = new XElement( "Run",
                    new XAttribute( "Number", run.Number ),
                    new XAttribute( "DateTime", run.Date.ToString() ),
                    new XAttribute( "Passed", run.Passed.ToString() )
                );

                foreach (HistoricRuleResult result in run.RuleResults.Values.OrderBy( r => r.ID ))
                {
                    var ruleElement = new XElement( "Rule",
                        new XAttribute( "ID", result.ID ),
                        new XAttribute( "Violations", result.Violations ),
                        new XAttribute( "Prior", result.Prior ),
                        new XAttribute( "FailOn", result.FailOn ),
                        new XAttribute( "Breaking", result.Breaking )
                    );

                    runElement.Add( ruleElement );
                }

                report_root.Add( runElement );
            }

            _storage.SaveRunHistory( report, _args.History );
        }

        //  #######################################################################################
        //  #############################   Everything under here better leave town afore sunset.


        // ########################  Reporting
    }
}
