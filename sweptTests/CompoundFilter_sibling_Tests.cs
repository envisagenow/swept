using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class CompoundFilter_sibling_Tests
    {
/*
whole file set:  (foo.cs, bar.cs, baz.cs)
when /a/
    gives whole file set to match
    match /a/
        takes SourceFiles (foo.cs, bar.cs, baz.cs)
        returns List<IssueSet> (foo.cs:3;5, bar.cs:10;22)
    sets progress to result from match /a/
and  /b/  (foo.cs, bar.cs)
    gives progress to match
    match /b/
        takes SourceFiles (foo.cs, bar.cs)
        returns List<IssueSet> (bar.cs:10;77;99)
    sets progress to intersection of progess and result of match /b/ (bar.cs:10)
*/

        [Test]
        public void Test()
        {
            
        }
    }
}
