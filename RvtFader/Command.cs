#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Analysis;
#endregion

namespace RvtFader
{
  [Transaction( TransactionMode.Manual )]
  public class Command : IExternalCommand
  {
    class FloorFilter : ISelectionFilter
    {
      public bool AllowElement( Element e )
      {
        return e is Floor;
      }

      public bool AllowReference( Reference r, XYZ p )
      {
        return true;
      }
    }

    const string _displayStyleName = "Attenuation";

    static ElementId GetAvfDisplayStyleId( Document doc )
    {
      ElementId id = AnalysisDisplayStyle.FindByName( 
        doc, _displayStyleName );

      if( ElementId.InvalidElementId == id )
      {
        using( Transaction t = new Transaction( doc ) )
        {
          t.Start( "Create AVF Display Style" );

          AnalysisDisplayColoredSurfaceSettings
            coloredSurfaceSettings = new
              AnalysisDisplayColoredSurfaceSettings();

          coloredSurfaceSettings.ShowGridLines = true;

          AnalysisDisplayColorSettings colorSettings
            = new AnalysisDisplayColorSettings();

          AnalysisDisplayLegendSettings legendSettings
            = new AnalysisDisplayLegendSettings();

          legendSettings.ShowLegend = true;

          AnalysisDisplayStyle analysisDisplayStyle
            = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
              doc, _displayStyleName, coloredSurfaceSettings,
              colorSettings, legendSettings );

          id = analysisDisplayStyle.Id;

          //view.AnalysisDisplayStyleId = analysisDisplayStyle.Id;

          t.Commit();
        }
      }
      return id;
    }

    static int _schemaId = -1;
    static SpatialFieldManager _sfm = null;

    static void SetUpAvfSfm( View view )
    {
      if( view.AnalysisDisplayStyleId
        == ElementId.InvalidElementId )
      {
        view.AnalysisDisplayStyleId
          = GetAvfDisplayStyleId( view.Document );
      }

      _sfm = SpatialFieldManager.GetSpatialFieldManager( 
        view );

      if( null == _sfm )
      {
        _sfm = SpatialFieldManager
          .CreateSpatialFieldManager( view, 1 );
      }

      if( -1 != _schemaId )
      {
        IList<int> results = _sfm.GetRegisteredResults();
        if( !results.Contains( _schemaId ) )
        {
          _schemaId = -1;
        }
      }

      if( -1 == _schemaId )
      {
        AnalysisResultSchema resultSchema
          = new AnalysisResultSchema( "Attenuation",
            "RvtFader signal attenuation" );

        _schemaId = _sfm.RegisterResult( resultSchema );
      }
    }

    public static void PaintFace( Face face, double value )
    {
      Transform trf = Transform.Identity;

      int idx = _sfm.AddSpatialFieldPrimitive( face, trf );
      IList<UV> uvPts = new List<UV>();
      List<double> doubleList = new List<double>();
      IList<ValueAtPoint> valList = new List<ValueAtPoint>();
      BoundingBoxUV bb = face.GetBoundingBox();
      uvPts.Add( bb.Min );
      doubleList.Add( value );
      valList.Add( new ValueAtPoint( doubleList ) );

      FieldDomainPointsByUV pnts
        = new FieldDomainPointsByUV( uvPts );

      FieldValues vals = new FieldValues( valList );
      _sfm.UpdateSpatialFieldPrimitive( idx, pnts,
        vals, _schemaId );
    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Application app = uiapp.Application;
      Document doc = uidoc.Document;
      Selection sel = uidoc.Selection;

      // Pick signal source point on floor slab.

      Reference r;

      try
      {
        r = sel.PickObject( 
          ObjectType.Face, 
          new FloorFilter(), 
          "Please pick signal source point." );
      }
      catch( Autodesk.Revit.Exceptions
        .OperationCanceledException )
      {
        return Result.Cancelled;
      }

      // Retrieve face to paint.

      Element floor;
      Face face;
      XYZ psource;
      UV uvsource;

      floor = doc.GetElement( r.ElementId );
      psource = r.GlobalPoint;
      uvsource = r.UVPoint;

      face = floor.GetGeometryObjectFromReference(
        r ) as Face;

      // Set up AVF display style.

      View view = uidoc.ActiveView;

      SetUpAvfSfm( view );

      // Display attenuation.

      PaintFace( face, 123 );
      
      return Result.Succeeded;
    }
  }
}
