#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RvtFader
{
  [Transaction( TransactionMode.Manual )]
  public class Command : IExternalCommand
  {
    #region Floor selection filter
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
    #endregion // Floor selection filter

    #region Set up analysis display style
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

          coloredSurfaceSettings.ShowGridLines = false;

          AnalysisDisplayColorSettings colorSettings
            = new AnalysisDisplayColorSettings();

          colorSettings.MinColor = new Color( 0, 255, 0 );
          colorSettings.MaxColor = new Color( 255, 0, 0 );

          AnalysisDisplayLegendSettings legendSettings
            = new AnalysisDisplayLegendSettings();

          legendSettings.ShowLegend = true;

          AnalysisDisplayStyle analysisDisplayStyle
            = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
              doc, _displayStyleName, coloredSurfaceSettings,
              colorSettings, legendSettings );

          view.AnalysisDisplayStyleId 
            = analysisDisplayStyle.Id;

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
    #endregion // Set up analysis display style

    static XYZ _z_offset = new XYZ( 0, 0, 5 );

  /// <summary>
  /// Calculate and paint the attenuation
  /// values on the given face.
  /// </summary>
  public static void PaintFace(
      Face face,
      XYZ psource,
      AttenuationCalculator calc )
    {
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

      XYZ psource2 = psource + _z_offset;

      for( double u = umin; u <= umax; u += ustep )
      {
        for( double v = vmin; v <= vmax; v += vstep )
        {
          UV uv = new UV( u, v );

          if( face.IsInside( uv ) )
          {
            uvPts.Add( uv );

            XYZ ptarget = face.Evaluate( uv ) 
              + _z_offset;

            vals[0] = calc.Attenuation( 
              psource2, ptarget );

            uvValues.Add( new ValueAtPoint( vals ) );
          }
        }
      }

      FieldDomainPointsByUV fpts
        = new FieldDomainPointsByUV( uvPts );

      FieldValues fvals = new FieldValues( uvValues );

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

        Debug.Print( Util.PointString( r.GlobalPoint ) );
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


      Settings settings = Settings.Load();

      AttenuationCalculator calc 
        = new AttenuationCalculator( doc, settings );

#if DEBUG_GRAPHICAL
      using( Transaction tx = new Transaction( doc ) )
      {
        tx.Start( "Draw Debug Model Lines" );
#endif // DEBUG_GRAPHICAL

      PaintFace( face, r.GlobalPoint, calc );

#if DEBUG_GRAPHICAL
        tx.Commit();
      }
#endif // DEBUG_GRAPHICAL

      return Result.Succeeded;
    }
  }
}
