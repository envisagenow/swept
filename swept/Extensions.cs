//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{
    public static class PluralExtensions
    {
        public static string Plurs( this string root, int count )
        {
            if (count == 1)
                return root;

            // to avoid constructing strings in the singular case.
            return string.Concat( root, "s" );
        }

        public static string Plures( this string root, int count )
        {
            if (count == 1)
                return root;

            // to avoid constructing strings in the singular case.
            return string.Concat( root, "es" );
        }

        public static string Plur( this string root, int count, string pluralForm )
        {
            if (count == 1)
                return root;

            return pluralForm;
        }

        public static string PlurFormat( this string root, int count, string pluralForm, params object[] args )
        {
            if (count == 1)
                return string.Format( root, count, args );

            return string.Format( pluralForm, count, args );
        }
    }
}
