// -------------------------------------------------------------------------------------------------
//   <copyright file="App.cs" company="Starion Group S.A.">
// 
//     Copyright 2024 Starion Group S.A.
// 
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
//     Unless required by applicable law or agreed to in writing, softwareUseCases
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// 
//   </copyright>
//   ------------------------------------------------------------------------------------------------

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
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);

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
        public static void BuildContainer(ContainerBuilder containerBuilder = null)
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
            switch (location)
            {
                case "MainMenu":
                    switch (menuName)
                    {
                        case "":
                            return MenuHeaderName;
                        case MenuHeaderName:
                            return new[]
                            {
                                GenericExportEntry
                            };
                    }

                    break;
            }

            return null;
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
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
            {
                return null;
            }

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
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
