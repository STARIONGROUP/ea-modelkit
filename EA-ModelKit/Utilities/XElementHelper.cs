// -------------------------------------------------------------------------------------------------
// <copyright file="XElementHelper.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Utilities
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Helper class for query on <see cref="XElement"/> 
    /// </summary>
    internal static class XElementHelper
    {
        /// <summary>
        /// Function that verifies that an <see cref="XElement" /> matches a name
        /// </summary>
        /// <param name="matchingName">The name that have to match</param>
        /// <returns>A <see cref="Func{TResult}" /></returns>
        public static Func<XElement, bool> MatchElementByName(string matchingName)
        {
            return x => string.Equals(x.Name.LocalName, matchingName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
