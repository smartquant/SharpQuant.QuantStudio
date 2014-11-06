SharpQuant.QuantStudio
==========================

Extendable simple application framework built upon [Dockpanel.Suite] and [Autofac].

- Application would look for an Autofac startup module defined in the app.config to setup entire application
- An example config is loaded if no startup module is found
- The example config contains the property window, an example tree launcher menu and the console.
- Settings will be serialized when application is closed. 
- Splash screen and menus parameterizable through autofac modules.


[Dockpanel.Suite]:http://dockpanelsuite.com/
[Autofac]:http://autofac.org/