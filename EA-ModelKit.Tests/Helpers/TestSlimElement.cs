// -------------------------------------------------------------------------------------------------
// <copyright file="TestSlimElement.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Helpers
{
    using EA;

    using EAModelKit.Model.Slims;

    using Moq;

    /// <summary>
    /// <see cref="TestSlimElement"/> is a <see cref="SlimElement"/> that is used for test purpose
    /// </summary>
    internal class TestSlimElement: SlimElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TestSlimElement" />
        /// </summary>
        /// <param name="element">The associated <see cref="Element"/></param>
        /// <param name="taggedValues">The associated collection of <see cref="SlimTaggedValue"/></param>
        /// <param name="connectors">The associated collection of <see cref="SlimConnector"/></param>
        public TestSlimElement(Element element, IReadOnlyCollection<SlimTaggedValue> taggedValues, IReadOnlyCollection<SlimConnector> connectors) : base(element, taggedValues, connectors)
        {
        }

        public TestSlimElement(string kind, string name, string alias, string notes, IReadOnlyList<SlimTaggedValue> taggedValues, 
            IReadOnlyCollection<SlimConnector> connectors):
            this(CreateElement(kind, name, alias, notes), taggedValues, connectors)
        {
        }

        private static Element CreateElement(string kind, string name, string alias, string notes)
        {
            var element = new Mock<Element>();
            element.Setup(x => x.Name).Returns(name);
            element.Setup(x => x.Alias).Returns(alias);
            element.Setup(x => x.Notes).Returns(notes);
            element.Setup(x => x.Stereotype).Returns(kind);
            return element.Object;
        }
    }
}
