﻿// -------------------------------------------------------------------------------------------------
// <copyright file="RepositoryExtensions.cs" company="Starion Group S.A.">
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
    /// Extension class for <see cref="Repository" />
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Asserts that a project is opened
        /// </summary>
        /// <param name="repository">The <see cref="IDualRepository" /></param>
        /// <returns>True if a project is opened</returns>
        public static bool IsProjectOpen(this IDualRepository repository)
        {
            try
            {
                _ = repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
