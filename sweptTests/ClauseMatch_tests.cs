using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ClauseMatch_tests
    {
        private LineMatch lm_00 = new LineMatch( new int[] { } );
        private LineMatch lm_12 = new LineMatch( new[] { 1, 2 } );
        private LineMatch lm_23 = new LineMatch( new[] { 2, 3 } );

        private FileMatch fm_T = new FileMatch( true );
        private FileMatch fm_F = new FileMatch( false );

        [Test]
        public void LineMatch_to_LineMatch()
        {
            var u12 = lm_12.Union( lm_23 ) as LineMatch;
            var i12 = lm_12.Intersection( lm_23 ) as LineMatch;
            var d12 = lm_12.Subtraction( lm_23 ) as LineMatch;

            //  LineMatches operating on each other have standard set operation semantics
            Assert.That( u12.Lines.Count, Is.EqualTo( 3 ) );
            Assert.That( i12.Lines.Count, Is.EqualTo( 1 ) );
            Assert.That( d12.Lines.Count, Is.EqualTo( 1 ) );

            Assert.That( i12.Lines[0], Is.EqualTo( 2 ) );
            Assert.That( d12.Lines[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void LineMatch_to_FileMatch()
        {
            var ulT = lm_12.Union( fm_T ) as LineMatch;
            var ilT = lm_12.Intersection( fm_T ) as LineMatch;
            var dlT = lm_12.Subtraction( fm_T ) as FileMatch;
            var dT1 = fm_T.Subtraction( lm_12 ) as FileMatch;

            var ulF = lm_12.Union( fm_F ) as LineMatch;
            var ilF = lm_12.Intersection( fm_F ) as FileMatch;
            var dlF = lm_12.Subtraction( fm_F ) as LineMatch;
            var dF1 = fm_F.Subtraction( lm_12 ) as FileMatch;

            var u0T = lm_00.Union( fm_T ) as FileMatch;
            var i0T = lm_00.Intersection( fm_T ) as FileMatch;
            var d0T = lm_00.Subtraction( fm_T ) as FileMatch;
            var dT0 = fm_T.Subtraction( lm_00 ) as FileMatch;

            Assert.That( ulT.Lines.Count, Is.EqualTo( 2 ) );
            Assert.That( ilT.Lines.Count, Is.EqualTo( 2 ) );
            Assert.That( dlT.DoesMatch, Is.False );
            Assert.That( dT1.DoesMatch, Is.False );

            Assert.That( ulF.Lines.Count, Is.EqualTo( 2 ) );
            Assert.That( ilF.DoesMatch, Is.False );
            Assert.That( dlF.Lines.Count, Is.EqualTo( 2 ) );
            Assert.That( dF1.DoesMatch, Is.False );

            Assert.That( u0T.DoesMatch );
            Assert.That( i0T.DoesMatch, Is.False );
            Assert.That( d0T.DoesMatch, Is.False );
            Assert.That( dT0.DoesMatch );
        }

        [Test]
        public void Empty_list_Union_File_True_makes_File_True()
        {
            var match = lm_00.Union( fm_T );

            Assert.That( match is FileMatch );
            Assert.That( match.DoesMatch );
        }

        [Test]
        public void FileMatch_to_FileMatch()
        {
            var uTT = fm_T.Union( fm_T ) as FileMatch;
            var uTF = fm_T.Union( fm_F ) as FileMatch;
            var uFF = fm_F.Union( fm_F ) as FileMatch;

            var iTT = fm_T.Intersection( fm_T ) as FileMatch;
            var iTF = fm_T.Intersection( fm_F ) as FileMatch;
            var iFF = fm_F.Intersection( fm_F ) as FileMatch;

            var dTT = fm_T.Subtraction( fm_T ) as FileMatch;
            var dTF = fm_T.Subtraction( fm_F ) as FileMatch;
            var dFT = fm_F.Subtraction( fm_T ) as FileMatch;
            var dFF = fm_F.Subtraction( fm_F ) as FileMatch;

            Assert.That( uTT.DoesMatch );
            Assert.That( uTF.DoesMatch );
            Assert.That( uFF.DoesMatch, Is.False );

            Assert.That( iTT.DoesMatch );
            Assert.That( iTF.DoesMatch, Is.False );
            Assert.That( iFF.DoesMatch, Is.False );

            Assert.That( dTT.DoesMatch, Is.False );
            Assert.That( dTF.DoesMatch );
            Assert.That( dFT.DoesMatch, Is.False );
            Assert.That( dFF.DoesMatch, Is.False );
        }
    }
}
