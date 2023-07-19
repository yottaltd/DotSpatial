// ********************************************************************************************************
// Product Name: DotSpatial.Projection
// Description:  The basic module for MapWindow version 6.0
// ********************************************************************************************************
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 7/24/2009 9:10:23 AM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//        Name         |    Date    |        Comment
// --------------------|------------|------------------------------------------------------------
// Ted Dunsford        |   5/3/2010 |  Updated project to DotSpatial.Projection and license to LGPL
// ********************************************************************************************************

namespace DotSpatial.Projections
{
    public interface IReproject
    {
        double[] ReprojectAffine(double[] affine, double numRows, double numCols, ProjectionInfo source, ProjectionInfo dest);
        void ReprojectPoints(double[] xy, double[] z, ProjectionInfo source, ProjectionInfo dest, int startIndex, int numPoints);
        void ReprojectPoints(double[] xy, double[] z, ProjectionInfo source, double srcZtoMeter, ProjectionInfo dest, double dstZtoMeter, IDatumTransform idt, int startIndex, int numPoints);
    }
}