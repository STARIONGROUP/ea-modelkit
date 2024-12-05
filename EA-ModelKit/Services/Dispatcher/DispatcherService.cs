// -------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Dispatcher
{
    using Autofac;

    using EA;

    using EAModelKit.Services.Cache;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Selection;
    using EAModelKit.Services.ViewBuilder;
    using EAModelKit.ViewModels.Exporter;
    using EAModelKit.Views.Export;

    using Microsoft.Extensions.Logging;

    using App = App;

    /// <summary>
    /// The <see cref="DispatcherService" /> provides EA events abstraction layer and available actions entry point
    /// </summary>
    internal class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Gets the injected <see cref="ICacheService" />
        /// </summary>
        private readonly ICacheService cacheService;

        /// <summary>
        /// Gets the injected <see cref="ILoggerService" />
        /// </summary>
        private readonly ILoggerService logger;

        /// <summary>
        /// Gets the injected <see cref="ISelectionService" />
        /// </summary>
        private readonly ISelectionService selectionService;

        /// <summary>
        /// Gets the injected <see cref="IViewBuilderService" />
        /// </summary>
        private readonly IViewBuilderService viewBuilderService;

        /// <summary>Initializes a new instance of the <see cref="DispatcherService" /> class.</summary>
        /// <param name="logger">The injected <see cref="ILoggerService" /></param>
        /// <param name="selectionService">The injected <see cref="ISelectionService" /></param>
        /// <param name="viewBuilderService">The injected <see cref="IViewBuilderService" /></param>
        /// <param name="cacheService">The injected <see cref="ICacheService" /></param>
        public DispatcherService(ILoggerService logger, ISelectionService selectionService, IViewBuilderService viewBuilderService, ICacheService cacheService)
        {
            this.logger = logger;
            this.selectionService = selectionService;
            this.viewBuilderService = viewBuilderService;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// Handles the connection to EA
        /// </summary>
        /// <param name="repository">The EA <see cref="Repository" /></param>
        public void Connect(Repository repository)
        {
            this.logger.InitializeService(repository);
            this.logger.Log(LogLevel.Information, "EA Model-Kit plugin successfully connected to EA");
        }

        /// <summary>
        /// Handles the disconnection to EA
        /// </summary>
        public void Disconnect()
        {
            this.logger.Log(LogLevel.Information, "EA Model-Kit plugin successfully disconnected to EA");
        }

        /// <summary>
        /// Handles the Generic Export action
        /// </summary>
        /// <param name="repository">The EA <see cref="Repository" /></param>
        public void OnGenericExport(Repository repository)
        {
            var selectedElements = this.selectionService.QuerySelectedElements(repository);

            if (selectedElements.Count == 0)
            {
                this.logger.Log(LogLevel.Warning, "Cannot proceed with export feature since no Element is part of the selection");
                return;
            }

            var exporterViewModel = App.Container.Resolve<IGenericExporterViewModel>();
            exporterViewModel.InitializeViewModel(selectedElements);
            this.viewBuilderService.ShowDxDialog<GenericExport, IGenericExporterViewModel>(exporterViewModel);
        }

        /// <summary>
        /// Handles the file opening event
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnFileOpen(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handles the creation of a new file
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnFileNew(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the post-creation of a <see cref="Package" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPostNewPackage(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the post-creation of an <see cref="Element" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPostNewElement(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the post-creation of a <see cref="Connector" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPostNewConnector(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the post-creation of an <see cref="Attribute" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPostNewAttribute(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the pre-deletion of an <see cref="Element" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPreDeleteElement(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the pre-deletion of an <see cref="Attribute" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPreDeleteAttribute(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Connector" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPreDeleteConnector(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Package" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPreDeletePackage(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the post-creation of a <see cref="Diagram" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository"/></param>
        public void OnPostNewDiagram(Repository repository)
        {
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Diagram" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnPreDeleteDiagram(Repository repository)
        {            
            this.ResetServices(repository);
        }

        /// <summary>
        /// Handle the modification of the current item
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void OnNotifyContextItemModified(Repository repository)
        {
            this.ResetServices(repository);
        }
        
        /// <summary>
        /// Reset some services that requires to after a model update
        /// </summary>
        private void ResetServices(Repository repository)
        {
            this.cacheService.Initialize(repository);
        }
    }
}
