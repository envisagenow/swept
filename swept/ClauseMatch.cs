using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public abstract class ClauseMatch
    {
        public abstract ClauseMatch Union( ClauseMatch other );
        public abstract ClauseMatch Union( bool does );

        public abstract ClauseMatch Intersection( ClauseMatch other );
        public abstract ClauseMatch Intersection( bool does );

        public abstract ClauseMatch Subtraction( ClauseMatch other );
        public abstract ClauseMatch Subtraction( bool does );
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
            //  Is this type-grind irreducible?  Is there a better place for it?
            //  So far as I see, yes, and no.
            if (other is LineMatch)
                return Union( ((LineMatch)other).Lines );
            else
                return Union( ((FileMatch)other).Does );
        }

        public override ClauseMatch Union( IEnumerable<int> lines )
        {
            return new LineMatch( lines.Union( Lines ) );
        }

        public override ClauseMatch Union( bool does )
        {
            if (does && !Lines.Any())
                return new FileMatch( does );
            else
                return new LineMatch( Lines );
        }

        public override ClauseMatch Intersection( ClauseMatch other )
        {
            if (other is LineMatch)
                return Intersection( ((LineMatch)other).Lines );
            else
                return Intersection( ((FileMatch)other).Does );
        }
        public ClauseMatch Intersection( IEnumerable<int> lines )
        {
            return new LineMatch( lines.Intersect( Lines ) );
        }
        public override ClauseMatch Intersection( bool does )
        {
            return new LineMatch( does ? Lines : new List<int>() );
        }

        public override ClauseMatch Subtraction( ClauseMatch other )
        {
            if (other is LineMatch)
                return Subtraction( ((LineMatch)other).Lines );
            else
                return Subtraction( ((FileMatch)other).Does );
        }

        private ClauseMatch Subtraction( IEnumerable<int> lines )
        {
            return new LineMatch( Lines.Where( num => !lines.Contains( num ) ) );
        }
        public override ClauseMatch Subtraction( bool does )
        {
            return new LineMatch( does ? new List<int>() : Lines );
        }
    }

    public class FileMatch : ClauseMatch
    {
        public FileMatch( bool does )
        {
            Does = does;
        }

        public bool Does { get; private set; }

        public override ClauseMatch Union( ClauseMatch other )
        {
            return other.Union( Does );
        }
        public override ClauseMatch Union( bool does )
        {
            return new FileMatch( Does || does );
        }

        public override ClauseMatch Intersection( ClauseMatch other )
        {
            return other.Intersection( Does );
        }
        public override ClauseMatch Intersection( bool does )
        {
            return new FileMatch( Does && does );
        }

        public override ClauseMatch Subtraction( ClauseMatch other )
        {
            bool does = other is LineMatch ? ((LineMatch)other).Lines.Any() : ((FileMatch)other).Does;
            return Subtraction( does );
        }
        public override ClauseMatch Subtraction( bool does )
        {
            return new FileMatch( Does && !does );
        }
    }
}
