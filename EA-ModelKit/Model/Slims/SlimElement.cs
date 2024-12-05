// -------------------------------------------------------------------------------------------------
// <copyright file="SlimElement.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Model.Slims
{
    using System.Collections.Generic;
    using System.Linq;

    using EA;

    /// <summary>
    /// Slim class for <see cref="Element"/>
    /// </summary>
    internal class SlimElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SlimElement" />
        /// </summary>
        /// <param name="element">The associated <see cref="Element"/></param>
        /// <param name="taggedValues">The associated collection of <see cref="SlimTaggedValue"/></param>
        public SlimElement(Element element, IReadOnlyList<SlimTaggedValue> taggedValues)
        {
            this.Name = element.Name;
            this.Notes = element.Notes;
            this.Alias = element.Alias;
            this.ElementKind = string.IsNullOrEmpty(element.Stereotype) ? element.Type : element.Stereotype;
            
            this.TaggedValues = taggedValues.GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, IReadOnlyList<SlimTaggedValue> (x) => x.ToList());
        }

        /// <summary>
        /// Gets the kind of Element that is represented
        /// </summary>
        public string ElementKind { get; }

        /// <summary>
        /// Gets the <see cref="Element"/>'s name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="Element"/>'s alias
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the <see cref="Element"/>'s Notes
        /// </summary>
        public string Notes { get; }
        
        /// <summary>
        /// Gets the associated dictionary of <see cref="SlimTaggedValue"/>, grouped by TaggedValueName
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<SlimTaggedValue>> TaggedValues { get; }
    }
}
