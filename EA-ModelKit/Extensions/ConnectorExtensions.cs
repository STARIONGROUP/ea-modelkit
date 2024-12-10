// -------------------------------------------------------------------------------------------------
// <copyright file="ConnectorExtensions.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Extensions
{
    using EA;

    /// <summary>
    /// Extensions methods for the <see cref="Connector" /> class
    /// </summary>
    internal static class ConnectorExtensions
    {
        /// <summary>
        /// Query the <see cref="Connector" />'s kind
        /// </summary>
        /// <param name="connector">An <see cref="Connector" /></param>
        /// <returns>The Stereotype if present, the type either</returns>
        public static string QueryConnectorKind(this Connector connector)
        {
            return string.IsNullOrEmpty(connector.Stereotype) ? connector.Type : connector.Stereotype;
        }
    }
}
