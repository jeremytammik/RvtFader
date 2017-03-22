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
    View3D _view3d;
    ElementFilter _wallFilter;

    public AttenuationCalculator( Document doc )
    {
      // Find a 3D view to use for the 
      // ReferenceIntersector constructor.

      FilteredElementCollector collector
        = new FilteredElementCollector( doc );

      Func<View3D, bool> isNotTemplate = v3
        => !( v3.IsTemplate );

      _view3d = collector
        .OfClass( typeof( View3D ) )
        .Cast<View3D>()
        .First<View3D>( isNotTemplate );

      _wallFilter = new LogicalAndFilter(
        new ElementCategoryFilter( BuiltInCategory.OST_Walls ),
        new ElementIsElementTypeFilter( false ) );
    }

    /// <summary>
    /// Calculate the signal attenuation between the 
    /// given source and target points using ray tracing.
    /// Walls and distance through air cause losses.
    /// </summary>
    public double Attenuation( XYZ psource, XYZ ptarget )
    {
      return ptarget.DistanceTo( psource );

      ReferenceIntersector intersector
        = new ReferenceIntersector( _wallFilter,
          FindReferenceTarget.Face, _view3d );

      intersector.FindReferencesInRevitLinks = true;

      IList<ReferenceWithContext> referencesWithContext
        = intersector.Find( psource, ptarget - psource );
    }
  }
}
