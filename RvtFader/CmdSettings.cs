#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System.Windows.Forms;
#endregion

namespace RvtFader
{
  [Transaction( TransactionMode.ReadOnly )]
  public class CmdSettings : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      IWin32Window revit_window
        = new JtWindowHandle(
          ComponentManager.ApplicationWindow );

      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Document thisDoc = uidoc.Document;
      FormSettings form = new FormSettings();
      form.ShowDialog( revit_window );
      App.Instance.SetTopButtonCurrent();
      return Result.Succeeded;
    }
  }
}
