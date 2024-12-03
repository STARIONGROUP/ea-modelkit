// -------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Starion Group S.A.">
// 
//     Copyright (C) 2024-2024 Starion Group S.A.
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
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace EAModelKit.Services.Dispatcher
{
    using System.Linq;

    using EA;

    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Selection;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="DispatcherService" /> provides EA events abstraction layer and available actions entry point
    /// </summary>
    internal class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Gets the injected <see cref="ILoggerService" />
        /// </summary>
        private readonly ILoggerService logger;

        /// <summary>
        /// Gets the injected <see cref="ISelectionService" />
        /// </summary>
        private readonly ISelectionService selectionService;

        /// <summary>Initializes a new instance of the <see cref="DispatcherService" /> class.</summary>
        /// <param name="logger">The injected <see cref="ILoggerService" /></param>
        /// <param name="selectionService">The injected <see cref="ISelectionService" /></param>
        public DispatcherService(ILoggerService logger, ISelectionService selectionService)
        {
            this.logger = logger;
            this.selectionService = selectionService;
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
            var elementsPerStereotype = selectedElements.GroupBy(x => x.Stereotype).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var elements in elementsPerStereotype)
            {
                this.logger.Log(LogLevel.Debug, "Found {0} Elements With Stereotype {1}", elements.Value.Count, elements.Key);
            }
        }
    }
}
