//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

/// <summary>This excludes code from NCover coverage stats</summary>
[CoverageExclude]
public class CoverageExcludeAttribute : Attribute
{
    private readonly string _reason;

    public CoverageExcludeAttribute() : base() { }

    public CoverageExcludeAttribute(string reason)
        : base()
    {
        _reason = reason;
    }

    public override string ToString()
    {
        return (_reason == null)? 
            "Coverage Excluded."
            : String.Format("Coverage Excluded: {0}", _reason);
    }
}
