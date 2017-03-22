using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "RvtFader" )]
[assembly: AssemblyDescription( "C# .NET Revit add-in to calculate and display signal attenuation using AVF" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Autodesk Inc." )]
[assembly: AssemblyProduct( "RvtFader Revit C# .NET Add-In" )]
[assembly: AssemblyCopyright( "Copyright 2017 (C) Jeremy Tammik, Autodesk Inc." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "321044f7-b0b2-4b1c-af18-e71a19252be0" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
//
// History:
//
// 2017-03-22 2017.0.0.0 settings and avf complete; todo: attenuation raytracing
// 2017-03-23 2017.0.0.1 PaintFace works; todo: overwrite AVF when called repeatedly and implement ray trace
// 2017-03-23 2017.0.0.2 avf works fine now with repeated calls
// 2017-03-23 2017.0.0.3 refactored SetUpAvfSfm and PaintFace
//
[assembly: AssemblyVersion( "2017.0.0.3" )]
[assembly: AssemblyFileVersion( "2017.0.0.3" )]
