// -------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="Starion Group S.A.">
// 
//     Copyright (C) 2024 Starion Group S.A.
// 
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// 
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace EAModelKit
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Autofac;

    using EA;

    using EAModelKit.Extensions;
    using EAModelKit.Services.Dispatcher;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Version;

    using Microsoft.Extensions.Logging;

    using Serilog;
    using Serilog.Events;

    using File = System.IO.File;

    /// <summary>
    /// Entry point of the EA ModelKit Plugin
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class App
    {
        /// <summary>
        /// The name of the Ribbon Category
        /// </summary>
        private const string RibbonCategoryName = "Publish";

        /// <summary>
        /// The name of the menu header, inisde the ribbon
        /// </summary>
        private const string MenuHeaderName = "-&EA ModelKit";

        /// <summary>
        /// The name of the Generic Export Entry
        /// </summary>
        private const string GenericExportEntry = "&Generic Export";

        /// <summary>
        /// Gets the <see cref="IDispatcherService" />
        /// </summary>
        private IDispatcherService dispatcher;

        /// <summary>
        /// Gets the <see cref="ILoggerService" />
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// Gets the <see cref="IVersionService" />
        /// </summary>
        private IVersionService versionService;

        /// <summary>
        /// Stores the location of the assembly, used to resolve other dependencies
        /// </summary>
        private static string assemblyLocation = Path.GetDirectoryName(typeof(App).Assembly.Location)!;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            Directory.SetCurrentDirectory(assemblyLocation);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel
                .Debug()
                .WriteTo
                .File(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Starion Group", "EA-ModelKit", "logs", "ea-modelkit.log"))
                .CreateLogger();

            try
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterServices();
                containerBuilder.RegisterViewModels();
                BuildContainer(containerBuilder);

                this.ResolveRequiredServices();

                this.LogAppStart();
            }
            catch (Exception ex)
            {
                Log.Logger.Write(LogEventLevel.Fatal, ex, "App failed to start");
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IContainer" />
        /// </summary>
        public static IContainer Container { get; private set; }

        /// <summary>
        /// Builds the <see cref="Container" />
        /// </summary>
        /// <param name="containerBuilder">An optional <see cref="Container" /></param>
        public static void BuildContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder ??= new ContainerBuilder();
            Container = containerBuilder.Build();
        }

        /// <summary>
        /// Called before EA starts to check Add-In Exists, necessary for the Add-In to work
        /// </summary>
        /// <param name="repository">The <see cref="EA.Repository" /></param>
        public void EA_Connect(Repository repository)
        {
            this.dispatcher.Connect(repository);
        }

        /// <summary>
        /// EA calls this operation on Exit
        /// </summary>
        public void EA_Disconnect()
        {
            this.dispatcher.Disconnect();
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        /// Used by EA to identify the the Ribbon in which the Add-In should place its menu icon
        /// </summary>
        /// <param name="_">The <see cref="Repository" /></param>
        /// <returns>The Category where the AddIn is placed</returns>
        public string EA_GetRibbonCategory(Repository _)
        {
            return RibbonCategoryName;
        }

        /// <summary>
        /// Called when user Clicks Add-Ins Menu item from within EA.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="location">
        /// A string representing the part of the user interface that brought up the menu. This can be
        /// TreeView, MainMenu or Diagram.
        /// </param>
        /// <param name="menuName">
        /// The name of the parent menu for which sub-items are to be defined. In the case of the top-level
        /// menu this is an empty string.
        /// </param>
        /// <returns>The definition of the menu option</returns>
        public object EA_GetMenuItems(Repository repository, string location, string menuName)
        {
            return location switch
            {
                "MainMenu" => menuName switch
                {
                    "" => MenuHeaderName,
                    MenuHeaderName => new[] { GenericExportEntry },
                    _ => null
                },
                _ => null
            };
        }

        /// <summary>
        /// EA_MenuClick events are received by an Add-In in response to user selection of a menu option.
        /// The event is raised when the user clicks on a particular menu option. When a user clicks on one of your non-parent menu
        /// options, your Add-In receives a <c>MenuClick</c> event.
        /// Notice that your code can directly access Enterprise Architect data and UI elements using CurrentRepository methods.
        /// </summary>
        /// <param name="repository">
        /// An EA.CurrentRepository object representing the currently open Enterprise Architect model. Poll its
        /// members to retrieve model data and user interface status information.
        /// </param>
        /// <param name="location">Not used</param>
        /// <param name="menuName">
        /// The name of the parent menu for which sub-items are to be defined.
        /// In the case of the top-level menu this is an empty string.
        /// Not used.
        /// </param>
        /// <param name="itemName">The name of the option actually clicked.</param>
        public void EA_MenuClick(Repository repository, string location, string menuName, string itemName)
        {
            try
            {
                switch (itemName)
                {
                    case GenericExportEntry:
                        this.dispatcher.OnGenericExport(repository);
                        break;
                    default:
                        this.logger.Log(LogLevel.Error, "Unsupported item action {0}", itemName);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex, "An exception occured while proceeding to the {0} action!", itemName);
                throw;
            }
        }

        /// <summary>
        /// Called once Menu has been opened to see what menu items should active.
        /// </summary>
        /// <param name="repository">the repository</param>
        /// <param name="location">the location of the menu</param>
        /// <param name="menuName">the name of the menu</param>
        /// <param name="itemName">the name of the menu item</param>
        /// <param name="isEnabled">boolean indicating whether the menu item is enabled</param>
        /// <param name="isChecked">boolean indicating whether the menu is checked</param>
        public void EA_GetMenuState(Repository repository, string location, string menuName, string itemName, ref bool isEnabled, ref bool isChecked)
        {
            isEnabled = repository.IsProjectOpen();
        }

        /// <summary>
        /// The event occurs when the model being viewed by the Enterprise Architect user changes, for whatever reason (through
        /// user interaction or Add-In activity).
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void EA_FileOpen(Repository repository)
        {
            this.dispatcher.OnFileOpen(repository);
        }

        /// <summary>
        /// The event occurs when the model being viewed by the Enterprise Architect user changes, for whatever reason (through
        /// user interaction or Add-In activity).
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void EA_FileNew(Repository repository)
        {
            this.dispatcher.OnFileNew(repository);
        }

        /// <summary>
        /// This event occurs when a user drags a new Package from the Toolbox or Resources window onto a diagram,
        /// or by selecting the New Package icon from the Project Browser.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True if the Package has been updated during this notification. Return False otherwise.</returns>
        public bool EA_OnPostNewPackage(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPostNewPackage(repository);
            return false;
        }

        /// <summary>
        /// This event occurs after a user has dragged a new element from the Toolbox or 'Resources' tab of the Browser window onto a diagram.
        /// The notification is provided immediately after the element is added to the model.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True if the element has been updated during this notification. Return False otherwise.</returns>
        public bool EA_OnPostNewElement(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPostNewElement(repository);
            return false;
        }

        /// <summary>
        /// This event occurs after a user has dragged a new connector from the Toolbox or 'Resources' tab of the Browser window onto a diagram.
        /// The notification is provided immediately after the connector is added to the model.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True if the connector has been updated during this notification. Return False otherwise.</returns>
        public bool EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPostNewConnector(repository);
            return false;
        }

        /// <summary>
        /// This event occurs when a user creates a new attribute on an element by either drag-and-dropping from the Browser window, using the 'Attributes' tab of the Features window, or using the in-place editor on the diagram.
        /// The notification is provided immediately after the attribute is created.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True if the attribute has been updated during this notification. Return False otherwise.</returns>
        public bool EA_OnPostNewAttribute(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPostNewAttribute(repository);
            return false;
        }

        /// <summary>
        /// EA_OnPostNewDiagram notifies Add-Ins that a new diagram has been created. It enables Add-Ins to modify the diagram upon creation.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True if the attribute has been updated during this notification. Return False otherwise.</returns>
        public bool EA_OnPostNewDiagram(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPostNewDiagram(repository);
            return false;
        }

        /// <summary>
        /// This event occurs when a user deletes an element from the Browser window or on a diagram.
        /// The notification is provided immediately before the element is deleted, so that the Add-In can disable deletion of the element.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True to enable deletion of the element from the model. Return False to disable deletion of the element.</returns>
        public bool EA_OnPreDeleteElement(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPreDeleteElement(repository);
            return true;
        }

        /// <summary>
        /// This event occurs when a user attempts to permanently delete an attribute from the Browser window.
        /// The notification is provided immediately before the attribute is deleted, so that the Add-In can disable deletion of the attribute.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True to enable deletion of the attribute from the model. Return False to disable deletion of the attribute.</returns>
        public bool EA_OnPreDeleteAttribute(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPreDeleteAttribute(repository);
            return true;
        }

        /// <summary>
        /// This event occurs when a user attempts to permanently delete a connector on a diagram.
        /// The notification is provided immediately before the connector is deleted, so that the Add-In can disable deletion of the connector.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True to enable deletion of the connector from the model. Return False to disable deletion of the connector.</returns>
        public bool EA_OnPreDeleteConnector(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPreDeleteConnector(repository);
            return true;
        }

        /// <summary>
        /// This event occurs when a user attempts to permanently delete a Package from the Browser window.
        /// The notification is provided immediately before the Package is deleted, so that the Add-In can disable deletion of the Package.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True to enable deletion of the Package from the model. Return False to disable deletion of the Package.</returns>
        public bool EA_OnPreDeletePackage(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPreDeletePackage(repository);
            return true;
        }

        /// <summary>
        /// This event occurs when a user attempts to permanently delete a diagram from the Browser window.
        /// The notification is provided immediately before the diagram is deleted, so that the Add-In can disable deletion of the diagram.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="info">The <see cref="EventProperties" /></param>
        /// <returns>Return True to enable deletion of the Package from the model. Return False to disable deletion of the Package.</returns>
        public bool EA_OnPreDeleteDiagram(Repository repository, EventProperties info)
        {
            this.dispatcher.OnPreDeleteDiagram(repository);
            return true;
        }

        /// <summary>
        /// This event occurs when a user has modified the context item. Add-Ins that require knowledge of when an item has been
        /// modified can subscribe to this broadcast function.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <param name="guid">The guid of the Item</param>
        /// <param name="objectType">The <see cref="ObjectType" /> of the item</param>
        public void EA_OnNotifyContextItemModified(Repository repository, string guid, ObjectType objectType)
        {
            this.dispatcher.OnNotifyContextItemModified(repository);
        }
        
        /// <summary>
        /// Resolves all required services for the current class
        /// </summary>
        private void ResolveRequiredServices()
        {
            var scope = Container.BeginLifetimeScope();
            var loggerFactory = scope.Resolve<ILoggerFactory>();
            loggerFactory.AddSerilog();
            this.logger = scope.Resolve<ILoggerService>();
            this.versionService = scope.Resolve<IVersionService>();
            this.dispatcher = scope.Resolve<IDispatcherService>();
        }

        /// <summary>
        /// Occures when <see cref="AppDomain.AssemblyResolve" /> event is called
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="args">The event args</param>
        /// <returns>The assembly</returns>
        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (string.IsNullOrEmpty(assemblyLocation))
            {
                return null;
            }

            var assemblyPath = Path.Combine(assemblyLocation, new AssemblyName(args.Name).Name + ".dll");
            return !File.Exists(assemblyPath) ? null : Assembly.LoadFile(assemblyPath);
        }

        /// <summary>
        /// Add a header to the log file
        /// </summary>
        private void LogAppStart()
        {
            this.logger.Log(LogLevel.Debug, "-----------------------------------------------------------------------------------------");
            this.logger.Log(LogLevel.Debug, "Starting EA ModelKit {0}", this.versionService.Version);
            this.logger.Log(LogLevel.Debug, "-----------------------------------------------------------------------------------------");
        }
    }
}
