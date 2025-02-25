﻿// -------------------------------------------------------------------------------------------------
// <copyright file="VersionService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Version
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The <see cref="VersionService" /> provides version information about the EA Plugin
    /// </summary>
    internal class VersionService : IVersionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionService" /> class.
        /// </summary>
        public VersionService()
        {
            this.Version = typeof(VersionService).Assembly.GetName().Version;
        }

        /// <summary>
        /// Gets the <see cref="Version" /> of the EA Plugin
        /// </summary>
        public Version Version { get; }
    }
}
