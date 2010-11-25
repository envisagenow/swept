//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

[CoverageExclude()]
public class CoverageExcludeAttribute : Attribute
{
    private string _reason;

    public CoverageExcludeAttribute()
        : base()
    {
    }

    public CoverageExcludeAttribute(string reason)
        : base()
    {
        _reason = reason;
    }

    public override string ToString()
    {
        if (_reason == null) return "Coverage Excluded.";
        return String.Format("Coverage Excluded: {0}", _reason);
    }
}
