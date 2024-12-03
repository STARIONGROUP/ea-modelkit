// -------------------------------------------------------------------------------------------------
// <copyright file="PackageWrapper.cs" company="Starion Group S.A.">
// 
//     Copyright (C) 2024-2024 Starion Group S.A.
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
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace EAModelKit.Model.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EA;

    /// <summary>
    /// Wrapper class for a <see cref="IDualPackage" />
    /// </summary>
    public class PackageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageWrapper" /> class.
        /// </summary>
        public PackageWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageWrapper" /> class.
        /// </summary>
        /// <param name="package">The associated <see cref="IDualPackage" /></param>
        public PackageWrapper(IDualPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.PackageName = package.Name;
            this.ContainerId = package.ParentID;
            this.PackageId = package.PackageID;
            this.PackageGuid = package.PackageGUID;
            this.RelatedElementId = package.Element?.ElementID;
        }

        /// <summary>
        /// Gets the ID of the associated <see cref="Element" />, if applicable. If null, means that the <see cref="EA.Package" />
        /// is the root package.
        /// </summary>
        public int? RelatedElementId { get; set; }

        /// <summary>
        /// Gets the GUID of the <see cref="EA.Package" />
        /// </summary>
        public string PackageGuid { get; set; }

        /// <summary>
        /// Gets the ID of the <see cref="EA.Package" />
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// Gets the ID of the container
        /// </summary>
        public int ContainerId { get; set; }

        /// <summary>
        /// Gets the name of the <see cref="PackageWrapper" />
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// Queries nested <see cref="PackageWrapper" /> id that are contained into a <see cref="PackageWrapper" />
        /// </summary>
        /// <param name="packages">A collection of all available <see cref="PackageWrapper" /></param>
        /// <param name="containerId">The id of the container <see cref="PackageWrapper" /></param>
        /// <returns>A collection of all nested ids</returns>
        public static IReadOnlyCollection<int> QueryContainedPackagesId(IReadOnlyCollection<PackageWrapper> packages, int containerId)
        {
            var nestedPackageIds = new List<int>();

            foreach (var nestedPackageId in packages.Where(x => x.ContainerId == containerId).Select(x => x.PackageId))
            {
                nestedPackageIds.Add(nestedPackageId);
                nestedPackageIds.AddRange(QueryContainedPackagesId(packages, nestedPackageId));
            }

            return nestedPackageIds;
        }
    }
}
