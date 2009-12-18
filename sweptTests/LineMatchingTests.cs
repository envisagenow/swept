using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class LineMatchingTests
    {
        private List<int> _matchList;
        private List<int> _lineIndices;

        [Test]
        public void can_return_list_of_matched_lines()
        {
            string multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";

            generateLineIndices( multiLineFile );

            Assert.That( _lineIndices.Count, Is.EqualTo( 5 ) );
            Assert.That( _lineIndices[0], Is.EqualTo( 1 ) );
            Assert.That( _lineIndices[1], Is.EqualTo( 10 ) );
        }

        private void generateLineIndices( string multiLineFile )
        {
            //list of newline indexes
            Regex lineCatcher = new Regex( "\n", RegexOptions.Multiline );
            MatchCollection lineMatches = lineCatcher.Matches( multiLineFile );

            _lineIndices = new List<int>();
            foreach (Match match in lineMatches)
            {
                _lineIndices.Add( match.Index );
            }
        }

        [Test]
        public void can_return_list_of_matched_line_numbers()
        {
            const int number_of_Bs = 2;
            string multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";

            generateLineIndices( multiLineFile );
            identifyMatchLineNumbers( multiLineFile, new Regex( "b" ) );

            Assert.That( _matchList.Count, Is.EqualTo( number_of_Bs ) );
            Assert.That( _matchList[0], Is.EqualTo( 3 ) );
            Assert.That( _matchList[1], Is.EqualTo( 4 ) );
        }

        private void identifyMatchLineNumbers( string multiLineFile, Regex rx )
        {
            MatchCollection matches = rx.Matches( multiLineFile );
            _matchList = new List<int>();

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, _lineIndices );
                _matchList.Add( line );
            }
        }

        [Test]
        public void match_first_character_is_line_1()
        {
            Assert.That( lineNumberOfMatch( 1, new List<int> { 2, 17 } ), Is.EqualTo( 1 ) );
        }

        [Test]
        public void match_after_index_of_first_newline_is_line_2()
        {
            Assert.That( lineNumberOfMatch( 4, new List<int> { 2, 5 } ), Is.EqualTo( 2 ) );
        }

        [Test]
        public void match_first_newline_is_line_1()
        {
            Assert.That( lineNumberOfMatch( 2, new List<int> { 2, 5 } ), Is.EqualTo( 1 ) );
        }

        [Test]
        public void match_after_last_newline_is_last_line()
        {
            Assert.That( lineNumberOfMatch( 40, new List<int> { 2, 5 } ), Is.EqualTo( 3 ) );
        }

        private int lineNumberOfMatch( int matchIndex, List<int> lineIndices )
        {
            int currentLineNumber = 1;
            foreach (int lineIndex in lineIndices)
            {
                if (matchIndex > lineIndex)
                {
                    currentLineNumber++;
                }
                else
                {
                    break;
                }
            }

            return currentLineNumber;
        }
    }
}
