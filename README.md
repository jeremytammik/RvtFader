# RvtFader

C# .NET Revit API add-in implementing an external application to calculate and display signal attenuation using 
the [analysis visualisation framework](http://thebuildingcoder.typepad.com/blog/avf) AVF
and `ReferenceIntersector` ray tracing.

You might also be interested in comparing this add-in with 
the [ForgeFader app](https://github.com/jeremytammik/forgefader) implementing
similar functionality using 
the [Autodesk Forge](https://developer.autodesk.com) platform.

![RvtFader icon](RvtFader/iCommand.png "RvtFader icon")


## Task and Functionality

This application works in a Revit model with a floor plan containing walls.

It calculates the signal attenuation caused by distance and obstacles.

In the first iteration, the only obstacles taken into account are walls.

Two signal attenuation values in decibels are defined in the application settings:

- Attenuation per metre in air
- Attenuation by a wall

Given a source point, calculate the attenuation in a widening circle around it and display that as a heat map.

We use the Revit API `ReferenceIntersector` ray tracing functionality to detect walls and
the [analysis visualisation framework AVF](http://thebuildingcoder.typepad.com/blog/avf) to display the heat map.


## Implementation

To achieve this task, RvtFader implements the following:

- Implement an external application with custom ribbon tab, panel, split button, main and settings commands

![RvtFader ribbon tab](img/rvtfader_ribbon_tab.png "RvtFader ribbon tab")

- Manage settings to be edited and stored (signal loss in dB).
- Enable user to pick a source point on a floor.
- Determine the floor boundaries.
- Shoot rays from the picked point to an array of other target points covering the floor.
- Determine the obstacles encountered by the ray, specifically wall elements.
- Display a 'heat map', i.e. colour gradient, representing the signal loss caused by the distance and number of walls between the source and the target points.

Summary of the steps towards achieving this:

- Skeleton add-in using the [Visual Studio Revit Add-In Wizards](http://thebuildingcoder.typepad.com/blog/about-the-author.html#5.20).
- External command for the settings user interface displaying a Windows form and storing data in JSON as developed for
the [HoloLens escape path waypoint JSON exporter](http://thebuildingcoder.typepad.com/blog/2016/09/hololens-escape-path-waypoint-json-exporter.html):
    - Display modal Windows form.
    - Implement form validation using `ErrorProvider` class, `Validating` and `Validated` events.
    - Store add-in option settings in JSON using the `JavaScriptSerializer` class.
- AVF heat map, initially simply based on distance from the selected source point:</br>
![RvtFader displaying distance using AVF](img/rvtfader_avf.png "RvtFader displaying distance using AVF")
- Graphical debugging displaying model lines representing the `ReferenceIntersector` rays traced using `ReferenceIntersector`, conditionally compiled based on the pragma definition `DEBUG_GRAPHICAL`:</br>
![Graphical debugging displaying model lines](img/rvtfader_graphical_debug_model_line.png "Graphical debugging displaying model lines")
- `AttenuationCalculator` taking walls and door openings into account:</br>
![Attenuation calculation results](img/rvtfader_attenuation_with_doors.png "Attenuation calculation results")

For more details on the implementation steps, please refer to
the [list of releases](releases) and [commits](commits).

Two sample models are provided in the [test](test) subdirectory; the same are also provided and used with
the [ForgeFader app](https://github.com/jeremytammik/forgefader) app:

![Fader test model](img/rvtfader_result.png "Fader test model")


## Further Reading

- **The Analysis Visualisation Framework AVF**:
    - An introduction to AVF programming basics is provided by Matt Mason's Autodesk University
class [CP5229 Seeing Data and More &ndash; The Analysis Visualization Framework](http://aucache.autodesk.com/au2011/sessions/5229/class_handouts/v1_CP5229-SeeingDataAndMore-TheAVFinRevitAPI.pdf)
([^](doc/cp5229_matt_mason_avf.pdf))
    - [Discussion of AVF by The Building Coder](http://thebuildingcoder.typepad.com/blog/avf)
- **`ReferenceIntersector` ray tracing**:
    - The `ReferenceIntersector` was previously named [`FindReferencesByDirection`](http://thebuildingcoder.typepad.com/blog/2010/01/findreferencesbydirection.html)
    - [Dimension walls using `FindReferencesByDirection`](http://thebuildingcoder.typepad.com/blog/2011/02/dimension-walls-using-findreferencesbydirection.html)
    - [Intersect Solid Filter, AVF vs DirectShape Debugging](http://thebuildingcoder.typepad.com/blog/2015/07/intersect-solid-filter-avf-and-directshape-for-debugging.html)
    - [Using `ReferenceIntersector` in linked files](http://thebuildingcoder.typepad.com/blog/2015/07/using-referenceintersector-in-linked-files.html)
- **Signal attenuation**:
    - [Attenuation](https://en.wikipedia.org/wiki/Attenuation)
    - [Modeling Signal Attenuation in IEEE 802.11 Wireless LANs - Vol. 1](http://www-cs-students.stanford.edu/~dbfaria/files/faria-TR-KP06-0118.pdf)
    - [The Basics of Signal Attenuation](http://www.dataloggerinc.com/content/resources/white_papers/332/the_basics_of_signal_attenuation/)
    - [RF Basics - Part 1](http://community.arubanetworks.com/aruba/attachments/aruba/tkb@tkb/121/1/RF-Basics_Part1.pdf) says "the free-space loss for 2.4 GHz at 100 meters from the transmitter is about 80 dB".


## Author

Jeremy Tammik,
[The Building Coder](http://thebuildingcoder.typepad.com),
[ADN](http://www.autodesk.com/adn)
[Open](http://www.autodesk.com/adnopen),
[Autodesk Inc.](http://www.autodesk.com)


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.
