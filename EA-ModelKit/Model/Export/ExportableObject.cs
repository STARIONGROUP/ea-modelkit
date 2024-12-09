// -------------------------------------------------------------------------------------------------
// <copyright file="ExportableObject.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Based common class for any object that are exportable
    /// </summary>
    internal abstract class ExportableObject
    {
        /// <summary>
        /// Gets all exportable values, associated to the header
        /// </summary>
        protected readonly Dictionary<string, string> ExportableValues = [];

        /// <summary>
        /// Gets the value at an header position
        /// </summary>
        /// <param name="header">The header position</param>
        /// <returns>The current value</returns>
        public string this[string header] => this.TryGetValue(header);

        /// <summary>
        /// Gets the headers to be used to export the <see cref="ExportableObject" />
        /// </summary>
        public IReadOnlyList<string> Headers { get; protected set; }

        /// <summary>
        /// Gets the name of the current export kind
        /// </summary>
        public string KindName { get; protected set; }

        /// <summary>
        /// Tries to get the value for the provided <paramref name="header" />
        /// </summary>
        /// <param name="header">The name of the header</param>
        /// <returns>The associated value</returns>
        private string TryGetValue(string header)
        {
            if (!this.Headers.Contains(header))
            {
                throw new ArgumentOutOfRangeException(nameof(header), $"The provided header value [{header}] is not a valid header");
            }

            return this.ExportableValues.TryGetValue(header, out var value) ? value : string.Empty;
        }
    }
}
