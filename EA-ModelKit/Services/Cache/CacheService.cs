// -------------------------------------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    using EA;

    using EAModelKit.Model.Slims;
    using EAModelKit.Utilities;

    /// <summary>
    /// The <see cref="CacheService" /> provides caching functionalities within the plugin
    /// </summary>
    internal class CacheService : ICacheService
    {
        /// <summary>
        /// The <see cref="Repository" /> that should be use to perform queries
        /// </summary>
        private Repository currentRepository;

        /// <summary>
        /// Flag that asserts if the service should reset before processing the next query
        /// </summary>
        private bool resetOnNextQuery = true;

        /// <summary>
        /// Cache dictionary for all casted <see cref="TaggedValue" /> that an <see cref="Element" /> have
        /// </summary>
        private Dictionary<int, IReadOnlyList<SlimTaggedValue>> taggedValuesPerElement = [];

        /// <summary>
        /// Get all <see cref="SlimTaggedValue" /> contained by an <see cref="Element" />
        /// </summary>
        /// <param name="elementId">The ID of the <see cref="Element" /> container</param>
        /// <returns>All contained <see cref="SlimTaggedValue" /></returns>
        public IReadOnlyList<SlimTaggedValue> GetTaggedValues(int elementId)
        {
            this.VerifyNeedReset();
            return this.taggedValuesPerElement.TryGetValue(elementId, out var taggedValues) ? taggedValues : [];
        }

        /// <summary>
        /// Initializes this service properties
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        public void Initialize(Repository repository)
        {
            this.resetOnNextQuery = true;
            this.currentRepository = repository;
        }

        /// <summary>
        /// Get all <see cref="SlimTaggedValue" /> contained by many <see cref="Element" />
        /// </summary>
        /// <param name="elementIds">An array of <see cref="Element" /> id</param>
        /// <returns>A read-only collection of <see cref="SlimTaggedValue" /></returns>
        public IReadOnlyList<SlimTaggedValue> GetTaggedValues(int[] elementIds)
        {
            this.VerifyNeedReset();

            var taggedValues = elementIds
                .AsParallel()
                .SelectMany(id => this.taggedValuesPerElement.TryGetValue(id, out var cachedTaggedValues) 
                    ? cachedTaggedValues 
                    : Enumerable.Empty<SlimTaggedValue>());

            return taggedValues.ToList();
        }

        /// <summary>
        /// Verifies if cache should be reset or not
        /// </summary>
        private void VerifyNeedReset()
        {
            if (!this.resetOnNextQuery)
            {
                return;
            }

            this.CacheSlimTaggedValues();
            this.resetOnNextQuery = false;
        }

        /// <summary>
        /// Caches all existing <see cref="TaggedValue" /> into <see cref="SlimTaggedValue" />
        /// </summary>
        private void CacheSlimTaggedValues()
        {
            const string sqlQuery = "SELECT Object_ID, Property, Value FROM t_objectproperties";

            var queryResult = this.currentRepository.SQLQuery(sqlQuery);
            var xElement = XElement.Parse(queryResult);
            var xRows = xElement.Descendants("Row");

            this.taggedValuesPerElement = xRows.Select(r => new SlimTaggedValue
                {
                    ContainerId = int.Parse(r.Elements().First(XElementHelper.MatchElementByName("Object_ID")).Value, CultureInfo.InvariantCulture),
                    Name = r.Elements().First(XElementHelper.MatchElementByName("Property")).Value,
                    Value = r.Elements().First(XElementHelper.MatchElementByName("Value")).Value
                }).GroupBy(s => s.ContainerId)
                .ToDictionary(x => x.Key, IReadOnlyList<SlimTaggedValue> (x) => x.ToList());
        }
    }
}
