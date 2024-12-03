// -------------------------------------------------------------------------------------------------
//  <copyright file="ISelectionService.cs" company="Starion Group S.A.">
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
//  </copyright>
// ------------------------------------------------------------------------------------------------

namespace EAModelKit.Services.Selection
{
    using System.Collections.Generic;

    using EA;

    /// <summary>
    /// The <see cref="ISelectionService" /> provides information about Element that are currently selected or contained by a selected package, supporting nesting.
    /// </summary>
    internal interface ISelectionService
    {
        /// <summary>
        /// Queries all <see cref="Element" /> that are part of the current selection.
        /// </summary>
        /// <param name="repository">The <see cref="Repository"/></param>
        /// <returns>A read-only collection of selected <see cref="Element" />s</returns>
        public IReadOnlyCollection<Element> QuerySelectedElements(Repository repository);
    }
}
