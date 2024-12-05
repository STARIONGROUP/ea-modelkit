// -------------------------------------------------------------------------------------------------
// <copyright file="IExcelWriter.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Writer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EAModelKit.Model.Export;

    /// <summary>
    /// The <see cref="IExcelWriter" /> provides writting features to Excel format
    /// </summary>
    internal interface IExcelWriter
    {
        /// <summary>
        /// Writes the content of the dictionary into an Excel file, at the given <paramref name="filePath" /> location
        /// </summary>
        /// <param name="exportableObjectsContent">The Dictionary that contains all information that should be exported.</param>
        /// <param name="filePath">The export file path</param>
        /// <returns>A <see cref="Task" /></returns>
        Task Write(IReadOnlyDictionary<string, IReadOnlyList<ExportableObject>> exportableObjectsContent, string filePath);
    }
}
