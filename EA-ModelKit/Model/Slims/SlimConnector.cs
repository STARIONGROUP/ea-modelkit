// -------------------------------------------------------------------------------------------------
// <copyright file="SlimConnector.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;

    using EA;

    using EAModelKit.Extensions;

    /// <summary>
    /// Slim class for <see cref="Connector" />
    /// </summary>
    internal class SlimConnector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SlimConnector" />
        /// </summary>
        /// <param name="connector">The associated <see cref="Connector" /></param>
        /// <param name="source">The source <see cref="Element" /> of the <see cref="Connector" /></param>
        /// <param name="target">The target <see cref="Element" /> of the <see cref="Connector" /></param>
        public SlimConnector(Connector connector, Element source, Element target)
        {
            this.ConnectorType = connector.Type;
            this.ConnectorStereotype = connector.Stereotype;
            this.SourceId = connector.ClientID;
            this.TargetId = connector.SupplierID;
            this.SourceKind = source.QueryElementKind();
            this.TargetKind = target.QueryElementKind();
            this.SourceName = source.Name;
            this.TargetName = target.Name;
        }

        /// <summary>
        /// Gets the name of the target <see cref="Element" />
        /// </summary>
        private string TargetName { get; }

        /// <summary>
        /// Gets the name of the source <see cref="Element" />
        /// </summary>
        private string SourceName { get; set; }

        /// <summary>
        /// Gets the source <see cref="Element" />'s kind
        /// </summary>
        private string SourceKind { get; }

        /// <summary>
        /// Gets the target <see cref="Element" />'s kind
        /// </summary>
        private string TargetKind { get; }

        /// <summary>
        /// Gets the id of the target <see cref="Element" />
        /// </summary>
        private int TargetId { get; }

        /// <summary>
        /// Gets the id of the source <see cref="Element" />
        /// </summary>
        private int SourceId { get; }

        /// <summary>
        /// Gets the applied stereotype to the <see cref="Connector" />
        /// </summary>
        private string ConnectorStereotype { get; }

        /// <summary>
        /// Gets the type of the <see cref="Connector" />
        /// </summary>
        private string ConnectorType { get; }

        /// <summary>
        /// Gets the <see cref="Element" />'s name for the opposite <see cref="Element" />
        /// </summary>
        /// <param name="currentElementId">The id of an <see cref="Element" />, either the source or the target</param>
        /// <returns>The <see cref="Element" />'s name of the opposite <see cref="Element" /></returns>
        /// <exception cref="ArgumentException">If the provided <see cref="Element" /> id neither the source or target</exception>
        public string GetOppositeElementName(int currentElementId)
        {
            return this.IsSource(currentElementId) ? this.TargetName : this.SourceName;
        }

        /// <summary>
        /// Computes the full name of a connector. The full name is computed as following :
        /// - Type
        /// - Stereotype
        /// - Is tied element the source or target
        /// - Opposite Element Kind
        /// </summary>
        /// <param name="currentElementId">The id of the <see cref="Element" /></param>
        /// <returns>The computed Connector full name</returns>
        public string ComputeConnectorKindFullName(int currentElementId)
        {
            this.VerifyValidElementId(currentElementId);

            var nameParts = new List<string> { this.ConnectorType };

            if (!string.IsNullOrEmpty(this.ConnectorStereotype))
            {
                nameParts.Add(this.ConnectorStereotype);
            }

            nameParts.Add(this.IsSource(currentElementId) ? "With Target" : "With Source");
            nameParts.Add(this.GetOppositeElementKind(currentElementId));
            return string.Join(" ", nameParts);
        }

        /// <summary>
        /// Gets the <see cref="Element" />'s kind for the opposite <see cref="Element" />
        /// </summary>
        /// <param name="currentElementId">The id of an <see cref="Element" />, either the source or the target</param>
        /// <returns>The <see cref="Element" />'s kind of the opposite <see cref="Element" /></returns>
        /// <exception cref="ArgumentException">If the provided <see cref="Element" /> id neither the source or target</exception>
        private string GetOppositeElementKind(int currentElementId)
        {
            return this.IsSource(currentElementId) ? this.TargetKind : this.SourceKind;
        }

        /// <summary>
        /// Asserts that the provided <see cref="Element" /> id is the source of the <see cref="Connector" />
        /// </summary>
        /// <param name="currentElementId">The <see cref="Element" /> id</param>
        /// <returns>The asserts</returns>
        /// <exception cref="ArgumentException">If the provided <see cref="Element" /> id neither the source or target</exception>
        private bool IsSource(int currentElementId)
        {
            this.VerifyValidElementId(currentElementId);
            return this.SourceId == currentElementId;
        }

        /// <summary>
        /// Verifies that the provided <see cref="Element" /> id is valid, either the id of the source or of the target
        /// </summary>
        /// <param name="elementId">The <see cref="Element" /> id to validate</param>
        /// <exception cref="ArgumentException">If the provided <see cref="Element" /> id neither the source or target</exception>
        private void VerifyValidElementId(int elementId)
        {
            if (elementId != this.SourceId && elementId != this.TargetId)
            {
                throw new ArgumentException($"The provided Element ID ({elementId}) is neither related to the source ({this.SourceId}) or the target ({this.TargetId}).");
            }
        }
    }
}
