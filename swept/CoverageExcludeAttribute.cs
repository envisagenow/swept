using System;

/// <summary>This will exclude the tagged code from ncover coverage stats</summary>
[CoverageExclude]
public class CoverageExcludeAttribute: Attribute
{
    public string Reason { get; private set; }
    public CoverageExcludeAttribute() { }
    public CoverageExcludeAttribute(string reason) : base() { Reason = reason; }
}
