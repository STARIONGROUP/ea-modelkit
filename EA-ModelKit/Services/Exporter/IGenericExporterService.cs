// -------------------------------------------------------------------------------------------------
// <copyright file="IGenericExporterService.cs" company="Starion Group S.A.">
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

    using EA;

    using EAModelKit.Model.Export;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The <see cref="IGenericExporterService" /> provides <see cref="Element" /> export feature
    /// </summary>
    internal interface IGenericExporterService
    {
        /// <summary>
        /// Exports <see cref="Element"/> data based on <see cref="GenericExportConfiguration"/> to a specific file 
        /// </summary>
        /// <param name="filePath">The path to the output file to be used</param>
        /// <param name="elementsConfigurations">The colleciton of <see cref="GenericExportConfiguration"/> that defines export configuration</param>
        Task ExportElements(string filePath, IReadOnlyList<GenericExportConfiguration> elementsConfigurations);
    }
}
