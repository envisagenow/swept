using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{

    public class BuildHistory
    {
        public List<BuildRun> BuildRuns;

        public BuildHistory()
        {
            BuildRuns = new List<BuildRun>();
        }
    }

    public class BuildRun
    {
        public int BuildNumber;
        public DateTime BuildDate;
        public Dictionary<String, int> ChangeViolations;

        public BuildRun()
        {
            ChangeViolations = new Dictionary<string, int>();
        }
    }

}
