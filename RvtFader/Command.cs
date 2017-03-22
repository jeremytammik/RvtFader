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
    /// <summary>
    /// Select only floor elements.
    /// </summary>
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

    /// <summary>
    /// Retrieve or set up the attenuation 
    /// display style for the given view.
    /// </summary>
    static void SetAvfDisplayStyle( View view )
    {
      Document doc = view.Document;

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

          view.AnalysisDisplayStyleId = analysisDisplayStyle.Id;

          t.Commit();
        }
      }
    }

    static int _schemaId = -1;
    static SpatialFieldManager _sfm = null;
    static int _sfp_index = -1;

    /// <summary>
    /// Set up the AVF spatial field manager 
    /// for the given view.
    /// </summary>
    static void SetUpAvfSfm( 
      View view, 
      Reference faceReference )
    {
      if( view.AnalysisDisplayStyleId
        == ElementId.InvalidElementId )
      {
        SetAvfDisplayStyle( view );
      }

      _sfm = SpatialFieldManager
        .GetSpatialFieldManager( view );

      if( null == _sfm )
      {
        _sfm = SpatialFieldManager
          .CreateSpatialFieldManager( view, 1 );
      }
      else if( 0 < _sfp_index )
      {
        _sfm.RemoveSpatialFieldPrimitive(
          _sfp_index );
      }

      _sfp_index = _sfm.AddSpatialFieldPrimitive(
        faceReference );

      //_sfm = SpatialFieldManager.GetSpatialFieldManager( 
      //  view );

      //if( null == _sfm )
      //{
      //  _sfm = SpatialFieldManager
      //    .CreateSpatialFieldManager( view, 1 );
      //}

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
        AnalysisResultSchema schema
          = new AnalysisResultSchema( "Attenuation",
            "RvtFader signal attenuation" );

        List<string> unitNames = new List<string>( new string[1] {"dB"} );
        List<double> unitFactors = new List<double>( new double[1] { 1.0 } );
        schema.SetUnits( unitNames, unitFactors );

        _schemaId = _sfm.RegisterResult( schema );
      }
    }

    /// <summary>
    /// Calculate the signal attenuation between the 
    /// given source and target points.
    /// </summary>
    static double AttenuationAt( XYZ psource, XYZ ptarget )
    {
      return ptarget.DistanceTo( psource );
    }

    /// <summary>
    /// Calculate and paint the attenuation
    /// values on the given face.
    /// </summary>
    public static void PaintFace(
      Face face,
      XYZ psource )

      //Document doc, 
      //Reference r )
    {
      //XYZ psource = r.GlobalPoint;
      //UV uvsource = r.UVPoint;
      //Element floor = doc.GetElement( r.ElementId );
      //Face face = floor.GetGeometryObjectFromReference(
      //  r ) as Face;

      IList<UV> uvPts = new List<UV>();
      IList<ValueAtPoint> uvValues = new List<ValueAtPoint>();

      BoundingBoxUV bb = face.GetBoundingBox();

      double umin = bb.Min.U;
      double umax = bb.Max.U;
      double ustep = 0.2 * ( umax - umin );

      double vmin = bb.Min.V;
      double vmax = bb.Max.V;
      double vstep = 0.2 * ( vmax - vmin );

      List<double> vals = new List<double>( 1 );
      vals.Add( 0 );

      for( double u = umin; u <= umax; u += ustep )
      {
        for( double v = vmin; v <= vmax; v += vstep )
        {
          UV uv = new UV( u, v );

          if( face.IsInside( uv ) )
          {
            uvPts.Add( uv );

            XYZ ptarget = face.Evaluate( uv );
            vals[0] = AttenuationAt( psource, ptarget );
            uvValues.Add( new ValueAtPoint( vals ) );
          }
        }
      }

      FieldDomainPointsByUV fpts
        = new FieldDomainPointsByUV( uvPts );

      FieldValues fvals = new FieldValues( uvValues );

      //int idx = _sfm.AddSpatialFieldPrimitive( r );

      _sfm.UpdateSpatialFieldPrimitive(
        _sfp_index, fpts, fvals, _schemaId );
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

      // Set up AVF display style.

      View view = uidoc.ActiveView;

      SetUpAvfSfm( view, r );

      // Display attenuation.

      Element floor = doc.GetElement( r.ElementId );
      Face face = floor.GetGeometryObjectFromReference(
        r ) as Face;

      PaintFace( face, r.GlobalPoint );
      
      return Result.Succeeded;
    }
  }
}
