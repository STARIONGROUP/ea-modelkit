// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExporterService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Exporter
{
    using System.Collections.Generic;
    using System.Linq;

    using EA;

    using EAModelKit.Model.Export;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Writer;

    using Microsoft.Extensions.Logging;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The <see cref="GenericExporterService" /> provides <see cref="Element" /> export feature
    /// </summary>
    internal class GenericExporterService : IGenericExporterService
    {
        /// <summary>
        /// Gets the injected <see cref="ILoggerService" />
        /// </summary>
        private readonly ILoggerService logger;

        /// <summary>
        /// Gets the injected <see cref="IExcelWriter" />
        /// </summary>
        private readonly IExcelWriter writer;

        /// <summary>
        /// Initializes a new instance of <see cref="GenericExporterService" />
        /// </summary>
        /// <param name="loggerService">The injected <see cref="ILoggerService" /></param>
        /// <param name="writer">The injected <see cref="IExcelWriter" /></param>
        public GenericExporterService(ILoggerService loggerService, IExcelWriter writer)
        {
            this.logger = loggerService;
            this.writer = writer;
        }

        /// <summary>
        /// Exports <see cref="Element" /> data based on <see cref="GenericExportConfiguration" /> to a specific file
        /// </summary>
        /// <param name="filePath">The path to the output file to be used</param>
        /// <param name="elementsConfigurations">The colleciton of <see cref="GenericExportConfiguration" /> that defines export configuration</param>
        public async Task ExportElementsAsync(string filePath, IReadOnlyList<GenericExportConfiguration> elementsConfigurations)
        {
            var exportableObjects = new Dictionary<string, IReadOnlyList<ExportableObject>>();

            foreach (var elementsConfiguration in elementsConfigurations)
            {
                exportableObjects[elementsConfiguration.ExportableElements[0].ElementKind] = elementsConfiguration.ExportableElements
                    .Select(x => new ExportableElement(x, elementsConfiguration.ExportableTaggedValues))
                    .ToList();
            }

            this.logger.Log(LogLevel.Information, "Starting to export {0} kind of Elements, {1} Elements in total to file {2}", exportableObjects.Keys.Count,
                exportableObjects.Values.Sum(x => x.Count), filePath);

            await this.writer.WriteAsync(exportableObjects, filePath);

            this.logger.Log(LogLevel.Information, "Export completed successfully");
        }
    }
}
