# Application lifecycle and functional
modularization

## Purpose and scope

This document describes in detail the lifecycle of the application
as well as the means of modularization, including information about
creating new application modules.

The contents of this document relate only to SpectralSpring functional modules.

## Technologies

### Spring.NET

> Spring.NET provides
> comprehensive
> infrastructural support for developing enterprise .NET applications.
> &nbsp;
Spring.NET offers a lightweight DI/IoC container built around the
concept of factories. Along with the IoC container, Spring.NET also
contains a configuration container, along with a suite of services that
can be applied to objects configured by the IoC container.

More information:

&nbsp; &nbsp; [http://www.springframework.net/](http://www.springframework.net/)

&nbsp;&nbsp;&nbsp; [http://msdn.microsoft.com/en-us/magazine/cc163739.aspx](http://msdn.microsoft.com/en-us/magazine/cc163739.aspx)

### Microsoft P&amp;P Prism

> Prism provides guidance designed to help you more easily design
> and build rich, flexible, and easy-to-maintain Windows Presentation
> Foundation (WPF) desktop applications, Silverlight Rich Internet
> Applications (RIAs), and Windows Phone 7 applications. Using design
> patterns that embody important architectural design principles, such as
> separation of concerns and loose coupling, Prism helps you to design
> and build applications using loosely coupled components that can evolve
> independently but that can be easily and seamlessly integrated into the
> overall application. These types of applications are known as composite
> applications.
Prism offers the capability to create modularizable composite user
interfaces by defining user interface regions that can accomodate any
UserControl.

This capability, along with the Spring.NET IoC integration creates the
posibility of fully dynamic modules that can extend the user interface,
allowing with the application's capabilities.

More information: [http://compositewpf.codeplex.com/](http://compositewpf.codeplex.com/)

### Microsoft P&amp;P Common Service Locator

> The Common Service Locator library contains a
> shared interface for service location which application and framework
> developers can reference. 
> 
> The library provides an abstraction over IoC containers and service
> locators.

The Common Service Locator is used to glue together Prism and
Spring.Net, allowing Prism to acces objects defined by the Spring.Net
container.

More information: [http://commonservicelocator.codeplex.com/](http://commonservicelocator.codeplex.com/)

## 

## Application lifecycle

### Application initialization steps

1. Application starts up, configured with App.xaml as startup object.

2. App.xaml creates a custom Bootstrapper that extends *SpringBootstrapper* and defines a
method that can create a module catalog and can retrieve the main shell
of the application from the Spring.NET context.

3. App.xaml call's the Bootstrapper's *Run*
method that creates the Spring.NET container and configures the Prism
objects (see the *AddMandatoryObjects*
and *AddDefaultObjects *methods
of SpringBootstrapper.

4. The Spring.NET container creates dependent objects, such as Data
Access Objects and Business Objects, as well as other services. The
SQLite embedded database is also created in this step.

5. SpringBootstrapper iterates through the modules present in the
module catalog (*SpringModuleCatalog*
basically takes all container objects that extends *Module*) **and creates instances.

6. Each module's lifecycle takes over control and create its own
Spring.NET subcontext, registers one or more menus with the *MainMenuManager* and also declares a
*MainView *that will be added
to the main application region.

7. After each module is initialized, the application menu is refrheshed
and the main window can be considered initialized.

### Application teardown steps

*TODO - no teardown yet*

## Components of a functional module

### The module class

Each functional module must have at least one class that extends *Module*.

This class must override the *ModuleName*
property, as well as the *Initialize*
method.

The *ModuleName* property must
be unique, as it must uniquely identify the module inside the module
catalog.

The *Initialize* method <span
 style="font-weight: bold;">can* configure a Spring.NET
subcontext for the module. This is not compulsory, as there may be some
modules that do not need a subcontext.

The *Initialize* method <span
 style="font-weight: bold;">can* specify a *MainView* that will be added to
Prism's *RegionManager*****.

The *Initialize* method <span
 style="font-weight: bold;">can* specify one or more menu items
for the application menu. If the module defines a *MainView*, a *SwitchViewMenuItem *can be used, to
automatically activate the modules' MainView.

Sample Module class:

<pre style="background-color: rgb(204, 204, 204);">using System;
using System.Collections.Generic;

using SpectralSpring.ModuleSupport;
using SpectralSpring.ModuleSupport.Ui;

namespace SpectralSpring.Sample.Module1
{
    public class SampleModuleOne : Module
    {

        public SampleModuleOne() {}

        public override void Initialize()
        {
            InitializeModuleContext("assembly://SpectralSpring.Sample.Module1/SpectralSpring.Sample.Module1.Config/SampleModuleOne.config");

            MainView = (ModuleView) ModuleContext.GetObject(ModuleName + "View");

            SwitchViewMenuItem menuItem = new SwitchViewMenuItem(ModuleName , MainView);
            MainMenuManager.AddMenuItem("general", 1000, menuItem);
        }

        public override string ModuleName
        {
            get { return "SampleModuleOne"; }
        }
    }
}
</pre>

### The module context

Each functional module can define a Spring.NET subcontext that can
contain the modules' objects.

The modules' subcontext will be merged in the applications' main
context, so that it has access to all objects defined in the main
context.

The subcontext can contain any type of object.

Sample module subcontext:

<pre style="background-color: rgb(204, 204, 204);">&lt;?xml version="1.0" encoding="utf-8" ?&gt;
&lt;objects xmlns="http://www.springframework.net"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://www.springframework.net http://www.springframework.net/schema/objects/spring-objects.xsd"&gt;

  &lt;description&gt;Sample Module One configuration file.&lt;/description&gt;

  &lt;object id="messagesResourcesConfigurer" type="SpectralSpring.Utils.MessageResourcesConfigurer"&gt;
    &lt;constructor-arg name="bundles"&gt;
      &lt;list element-type="string"&gt;
        &lt;value&gt;/SpectralSpring.Sample.Module1;component/Strings/Messages&lt;/value&gt;
      &lt;/list&gt;
    &lt;/constructor-arg&gt;
  &lt;/object&gt;

  &lt;object id="SampleModuleOneView"
          type="SpectralSpring.Sample.Module1.Ui.ModuleOneView"/&gt;

&lt;/objects&gt;

</pre>

### The module view

Each module can define multiple views, and one *MainView*

The view can be any control that extends *ModuleView*, be it in XAML or custom
c# WPF.


### Module resources

Each module can contain resources such as string and image bundles.

