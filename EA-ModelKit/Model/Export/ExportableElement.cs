// -------------------------------------------------------------------------------------------------
// <copyright file="ExportableElement.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Net;

    using EA;

    using EAModelKit.Model.Slims;

    /// <summary>
    /// Data class that provide data that have to be exported from an <see cref="Element"/>
    /// </summary>
    internal class ExportableElement: ExportableObject
    {
        /// <summary>
        /// Defines the base headers for the <see cref="ExportableElement"/>
        /// </summary>
        private readonly string[] baseHeaders = ["Name", "Alias", "Notes"];
        
        /// <summary>
        /// Initializes a new instance of <see cref="ExportableElement"/>
        /// </summary>
        /// <param name="element">The <see cref="SlimElement"/> that should be exported</param>
        /// <param name="taggedValuesToExport">All name of TaggeValue that have to be exported</param>
        public ExportableElement(SlimElement element, IReadOnlyList<string> taggedValuesToExport)
        {
            this.KindName = element.ElementKind;
            this.Headers = [..this.baseHeaders, ..taggedValuesToExport];

            var values = new List<string> {element.Name, element.Alias,WebUtility.HtmlDecode(element.Notes)};

            for(var baseHeaderIndex = 0;baseHeaderIndex < this.baseHeaders.Length;baseHeaderIndex++)
            {
                this.ExportableValues[this.baseHeaders[baseHeaderIndex]] = values[baseHeaderIndex];
            }

            foreach (var taggedValueToExport in taggedValuesToExport)
            {
                this.ExportableValues[taggedValueToExport] = element.TaggedValues.TryGetValue(taggedValueToExport, out var existingValue)
                    ? string.Join("\n", existingValue.Select(x => x.Value))
                    : string.Empty; 
            }
        }
    }
}
