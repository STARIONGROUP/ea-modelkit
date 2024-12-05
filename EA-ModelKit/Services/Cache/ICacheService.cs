// -------------------------------------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Cache
{
    using System;
    using System.Collections.Generic;

    using EA;

    using EAModelKit.Model.Slims;

    /// <summary>
    /// The <see cref="ICacheService" /> provides caching functionalities within the plugin
    /// </summary>
    internal interface ICacheService
    {
        /// <summary>
        /// Get all <see cref="SlimTaggedValue" /> contained by an <see cref="Element" />
        /// </summary>
        /// <param name="elementId">The ID of the <see cref="Element" /> container</param>
        /// <returns>All contained <see cref="SlimTaggedValue" /></returns>
        IReadOnlyList<SlimTaggedValue> GetTaggedValues(int elementId);

        /// <summary>
        /// Initializes this service properties
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void Initialize(Repository repository);

        /// <summary>
        /// Get all <see cref="SlimTaggedValue" /> contained by many <see cref="Element" />
        /// </summary>
        /// <param name="elementIds">An array of <see cref="Element" /> id</param>
        /// <returns>A read-only collection of <see cref="SlimTaggedValue" /></returns>
        IReadOnlyList<SlimTaggedValue> GetTaggedValues(int[] elementIds);
    }
}
