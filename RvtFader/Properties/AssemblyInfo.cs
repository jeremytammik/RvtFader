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
// 2017-03-22 2017.0.0.1 PaintFace works; todo: overwrite AVF when called repeatedly and implement ray trace
// 2017-03-22 2017.0.0.2 avf works fine now with repeated calls
// 2017-03-22 2017.0.0.3 refactored SetUpAvfSfm and PaintFace
// 2017-03-22 2017.0.0.4 started implementing AttenuationCalculator and debugging ray tracing
// 2017-03-23 2017.0.0.5 added graphical debugging to draw ModelLine representing ray --> rvtfader_graphical_debug_model_line.png
// 2017-03-23 2017.0.0.6 removed graphical debugging and simplified wall filter; ray tacing works, walls are detected
// 2017-03-23 2017.0.0.7 implemented GetWallCount and attenuation calculation --> rvtfader_attenuation_with_doors.png
// 2017-03-23 2017.0.0.8 documented implementation and further reading
// 2017-03-23 2017.0.0.9 cleanup
// 2017-03-23 2017.0.0.10 created custom icon
// 2017-03-24 2017.0.0.11 published
// 2017-04-04 2017.0.0.12 added test files from https://github.com/jeremytammik/forgefader
// 2017-04-04 2017.0.0.13 added images and updated readme
// 2017-10-15 2018.0.0.0 flat migration to Revit 2018
//
[assembly: AssemblyVersion( "2018.0.0.0" )]
[assembly: AssemblyFileVersion( "2018.0.0.0" )]
