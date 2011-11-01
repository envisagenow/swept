//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public abstract class ClauseMatch
    {
        public Change Change { get; set; }

        public abstract ClauseMatch Union( ClauseMatch other );
        public abstract ClauseMatch Union( IEnumerable<int> lines );

        public abstract ClauseMatch Intersection( ClauseMatch other );
        public abstract ClauseMatch Intersection( IEnumerable<int> lines );

        public abstract ClauseMatch Subtraction( ClauseMatch other );
        public abstract ClauseMatch Subtraction( IEnumerable<int> lines );

        public abstract bool DoesMatch { get; }
        public abstract int Count { get; }
    }

    public class LineMatch : ClauseMatch
    {
        public LineMatch( IEnumerable<int> lines )
        {
            Lines = new List<int>( lines );
        }

        public List<int> Lines { get; private set; }

        public override ClauseMatch Union( ClauseMatch other )
        {
            return other.Union( Lines );
        }

        public override ClauseMatch Union( IEnumerable<int> lines )
        {
            return new LineMatch( lines.Union( Lines ) );
        }

        public override ClauseMatch Intersection( ClauseMatch other )
        {
            return other.Intersection( Lines );
        }

        public override ClauseMatch Intersection( IEnumerable<int> lines )
        {
            return new LineMatch( lines.Intersect( Lines ) );
        }

        public override ClauseMatch Subtraction( ClauseMatch other )
        {
            if (other is LineMatch)
                return Subtraction( ((LineMatch)other).Lines );
            else if (other.DoesMatch)
                return new FileMatch( false );
            else
                return new LineMatch( Lines );
        }

        public override ClauseMatch Subtraction( IEnumerable<int> lines )
        {
            return new LineMatch( Lines.Where( num => !lines.Contains( num ) ) );
        }

        public override bool DoesMatch
        {
            get { return Lines.Any(); }
        }

        public override int Count
        {
            get { return Lines.Count; }
        }
    }

    public class FileMatch : ClauseMatch
    {
        public FileMatch( bool does )
        {
            this.does = does;
        }

        private bool does;
        public override bool DoesMatch
        {
            get { return does; }
        }

        public override int Count
        {
            get { return does ? 1 : 0; }
        }

        public override ClauseMatch Union( ClauseMatch other )
        {
            //  Is this type-grind soluble?
            if (other is LineMatch)
                return Union( ((LineMatch)other).Lines );
            else
                return new FileMatch( DoesMatch || other.DoesMatch );
        }

        public override ClauseMatch Union( IEnumerable<int> lines )
        {
            if (lines.Any())
                return new LineMatch( lines );
            else
                return new FileMatch( DoesMatch );
        }

        public override ClauseMatch Intersection( ClauseMatch other )
        {
            if (other is LineMatch)
                return Intersection( ((LineMatch)other).Lines );
            else
                return new FileMatch( DoesMatch && other.DoesMatch );
        }

        public override ClauseMatch Intersection( IEnumerable<int> lines )
        {
            if (!DoesMatch || ! lines.Any())
                return new FileMatch( false );
            else
                return new LineMatch( lines );
        }

        public override ClauseMatch Subtraction( ClauseMatch other )
        {
            return new FileMatch( DoesMatch && !other.DoesMatch );
        }

        public override ClauseMatch Subtraction( IEnumerable<int> lines )
        {
            return new FileMatch( DoesMatch && !lines.Any() );
        }
    }
}
