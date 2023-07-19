// ********************************************************************************************************
// Product Name: DotSpatial.Projection
// Description:  The basic module for MapWindow version 6.0
// ********************************************************************************************************
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 8/18/2009 9:31:11 AM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//        Name         |    Date    |        Comment
// --------------------|------------|------------------------------------------------------------
// Ted Dunsford        |   5/3/2010 |  Updated project to DotSpatial.Projection and license to LGPL
// ********************************************************************************************************

using System.Collections.Generic;
using System.IO;
using System;

namespace DotSpatial.Projections
{
    public interface IGridShift
    {
        bool ThrowGridShiftMissingExceptions { get; set; }
        void InitializeExternalGrids(IEnumerable<(string FileName, Func<Stream> StreamFactory)> grids);
        void InitializeExternalGrids(string gridsFolder, bool recursive);
        void InitializeExternalGrids();
        void Apply(string[] names, bool inverse, double[] xy, int startIndex, long numPoints);
    }
}