// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExportConfiguration.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Model.Export
{
    using System.Collections.Generic;

    using EA;

    using EAModelKit.Model.Slims;

    /// <summary>
    /// The <see cref="GenericExportConfiguration" /> provides configuration details that should be used to export
    /// <see cref="Element" />
    /// </summary>
    internal class GenericExportConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExportConfiguration" /> class.
        /// </summary>
        /// <param name="exportableElements">The collection of <see cref="SlimElement" /> that should be exported</param>
        /// <param name="exportableTaggedValues">The collection of TaggedValue names that should be exported</param>
        public GenericExportConfiguration(IReadOnlyList<SlimElement> exportableElements, IReadOnlyList<string> exportableTaggedValues)
        {
            this.ExportableElements = exportableElements;
            this.ExportableTaggedValues = exportableTaggedValues;
        }

        /// <summary>
        /// Gets the read-only collection of <see cref="Element" /> that have to be exported
        /// </summary>
        public IReadOnlyList<SlimElement> ExportableElements { get; }

        /// <summary>
        /// Gets the read-only collection of name of TaggedValues that should be exported
        /// </summary>
        public IReadOnlyList<string> ExportableTaggedValues { get; }
    }
}
