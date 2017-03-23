#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using System.Linq;
#endregion

namespace RvtFader
{
  public class AttenuationCalculator
  {
    Document _doc;
    Settings _settings;
    View3D _view3d;
    ElementFilter _wallFilter;

#if DEBUG_GRAPHICAL
    /// <summary>
    /// Draw model lines for graphical geometrical debugging.
    /// </summary>
    SketchPlane _sketch;
#endif // DEBUG_GRAPHICAL

    public AttenuationCalculator(
      Document doc,
      Settings settings )
    {
      _doc = doc;
      _settings = settings;

      // Find a 3D view to use for the 
      // ReferenceIntersector constructor.

      _view3d
        = new FilteredElementCollector( doc )
          .OfClass( typeof( View3D ) )
          .Cast<View3D>()
          .First<View3D>( v => !v.IsTemplate );

      _wallFilter = new ElementClassFilter(
        typeof( Wall ) );
    }

    /// <summary>
    /// Return the number of walls encountered 
    /// between the two given points.
    /// </summary>
    int GetWallCount( XYZ psource, XYZ ptarget )
    {
      double d = ptarget.DistanceTo( psource );

      ReferenceIntersector intersector
        = new ReferenceIntersector( _wallFilter,
          FindReferenceTarget.Face, _view3d );

      intersector.FindReferencesInRevitLinks = true;

      IList<ReferenceWithContext> referencesWithContext
        = intersector.Find( psource, ptarget - psource );

      List<ElementId> wallIds = new List<ElementId>();

      foreach( ReferenceWithContext rc in
        referencesWithContext )
      {
        if( rc.Proximity <= d )
        {
          Reference r = rc.GetReference();
          Element e = _doc.GetElement( r.ElementId );
          Debug.Assert( e is Wall, "expected only walls" );
          Debug.Print( string.Format( "wall {0} at {1}",
            e.Id, d ) );

          if( !wallIds.Contains( e.Id ) )
          {
            wallIds.Add( e.Id );
          }
        }
      }
      return wallIds.Count;
    }

    /// <summary>
    /// Calculate the signal attenuation between the 
    /// given source and target points using ray tracing.
    /// Walls and distance through air cause losses.
    /// </summary>
    public double Attenuation( XYZ psource, XYZ ptarget )
    {
      Debug.Print( string.Format( "{0} -> {1}",
        Util.PointString( psource ),
        Util.PointString( ptarget ) ) );

#if DEBUG_GRAPHICAL
      if( null == _sketch || 0.0001
        < _sketch.GetPlane().Origin.Z - psource.Z )
      {
        Plane plane = Plane.CreateByNormalAndOrigin(
          XYZ.BasisZ, psource );

        _sketch = SketchPlane.Create( _doc, plane );

      }
      Line line = Line.CreateBound( psource, ptarget );

      _sketch.Document.Create.NewModelCurve( line, _sketch );
#endif // DEBUG_GRAPHICAL

      double d = ptarget.DistanceTo( psource );

      double a = Util.FootToMetre( d )
        * _settings.AttenuationAirPerMetreInDb;

      int wallCount = GetWallCount( psource, ptarget );

      a += wallCount * _settings.AttenuationWallInDb;

      return a;
    }
  }
}
