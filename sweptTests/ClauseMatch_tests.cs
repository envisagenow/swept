//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ClauseMatch_tests
    {
        private readonly LineMatch lines_00 = new LineMatch( new int[] { } );
        private readonly LineMatch lines_12 = new LineMatch( new[] { 1, 2 } );
        private readonly LineMatch lines_23 = new LineMatch( new[] { 2, 3 } );

        private readonly FileMatch file_T = new FileMatch( true );
        private readonly FileMatch file_F = new FileMatch( false );

        [Test]
        public void LineMatch_to_LineMatch()
        {
            var u12 = lines_12.Union( lines_23 ) as LineMatch;
            var i12 = lines_12.Intersection( lines_23 ) as LineMatch;
            var d12 = lines_12.Subtraction( lines_23 ) as LineMatch;

            //  LineMatches operating on each other have standard set operation semantics
            Assert.That( u12.Lines.Count, Is.EqualTo( 3 ) );
            Assert.That( i12.Lines.Count, Is.EqualTo( 1 ) );
            Assert.That( d12.Lines.Count, Is.EqualTo( 1 ) );

            Assert.That( i12.Lines[0], Is.EqualTo( 2 ) );
            Assert.That( d12.Lines[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void Count_works()
        {
            Assert.That( lines_00.Count, Is.EqualTo( 0 ) );
            Assert.That( lines_12.Count, Is.EqualTo( 2 ) );

            Assert.That( file_F.Count, Is.EqualTo( 0 ) );
            Assert.That( file_T.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void LineMatch_to_FileMatch()
        {
            var ulT = lines_12.Union( file_T ) as LineMatch;
            var ilT = lines_12.Intersection( file_T ) as LineMatch;
            var dlT = lines_12.Subtraction( file_T ) as FileMatch;
            var dT1 = file_T.Subtraction( lines_12 ) as FileMatch;

            var ulF = lines_12.Union( file_F ) as LineMatch;
            var ilF = lines_12.Intersection( file_F ) as FileMatch;
            var dlF = lines_12.Subtraction( file_F ) as LineMatch;
            var dF1 = file_F.Subtraction( lines_12 ) as FileMatch;

            var u0T = lines_00.Union( file_T ) as FileMatch;
            var i0T = lines_00.Intersection( file_T ) as FileMatch;
            var d0T = lines_00.Subtraction( file_T ) as FileMatch;
            var dT0 = file_T.Subtraction( lines_00 ) as FileMatch;

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
            var match = lines_00.Union( file_T );

            Assert.That( match is FileMatch );
            Assert.That( match.DoesMatch );
        }

        [Test]
        public void FileMatch_to_FileMatch()
        {
            var uTT = file_T.Union( file_T ) as FileMatch;
            var uTF = file_T.Union( file_F ) as FileMatch;
            var uFF = file_F.Union( file_F ) as FileMatch;

            var iTT = file_T.Intersection( file_T ) as FileMatch;
            var iTF = file_T.Intersection( file_F ) as FileMatch;
            var iFF = file_F.Intersection( file_F ) as FileMatch;

            var dTT = file_T.Subtraction( file_T ) as FileMatch;
            var dTF = file_T.Subtraction( file_F ) as FileMatch;
            var dFT = file_F.Subtraction( file_T ) as FileMatch;
            var dFF = file_F.Subtraction( file_F ) as FileMatch;

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
