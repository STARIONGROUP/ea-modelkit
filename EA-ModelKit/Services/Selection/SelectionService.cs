// -------------------------------------------------------------------------------------------------
//  <copyright file="SelectionService.cs" company="Starion Group S.A.">
// 
//     Copyright 2024 Starion Group S.A.
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
//  </copyright>
// ------------------------------------------------------------------------------------------------

namespace EAModelKit.Services.Selection
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    using EA;

    using EAModelKit.Model.Wrappers;

    /// <summary>
    /// The <see cref="SelectionService" /> provides information about Element that are currently selected or contained by a selected package, supporting nesting.
    /// </summary>
    internal class SelectionService : ISelectionService
    {
        /// <summary>
        /// Queries all <see cref="Element" /> that are part of the current selection.
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        /// <returns>A read-only collection of selected <see cref="Element" />s</returns>
        public IReadOnlyCollection<Element> QuerySelectedElements(Repository repository)
        {
            var selectedPackagesId = QueryCurrentlySelectedPackagesId(repository);

            if (selectedPackagesId.Count == 0)
            {
                return [..repository.CurrentSelection.ElementSet.OfType<Element>()];
            }

            var existingPackages = QueriesAllExistingPackages(repository);
            var allSelectedPackages = new List<int>(selectedPackagesId);

            foreach (var selectedPackageId in selectedPackagesId)
            {
                allSelectedPackages.AddRange(PackageWrapper.QueryContainedPackagesId(existingPackages, selectedPackageId));
            }

            var sqlQuery = $"SELECT Object_ID from t_object WHERE Package_ID in ({string.Join(",", allSelectedPackages)}) AND Object_Type != 'Package'";
            var sqlResponse = repository.SQLQuery(sqlQuery);

            var xElement = XElement.Parse(sqlResponse);
            var xRows = xElement.Descendants("Row");

            var selectedElementsId = xRows.Select(r => int.Parse(r.Elements().First(MatchElementByName("Object_ID")).Value,
                CultureInfo.InvariantCulture)).Distinct().ToList();

            return selectedElementsId.Count == 0 ? [] : repository.GetElementSet(string.Join(",", selectedElementsId), 0).OfType<Element>().ToList();
        }

        /// <summary>
        /// Queries <see cref="Package" />s id that are part of the selection, without any nesting
        /// </summary>
        /// <param name="repository">The <see cref="IDualRepository" /></param>
        /// <returns>The <see cref="HashSet{T}" /> of all selected package</returns>
        private static HashSet<int> QueryCurrentlySelectedPackagesId(IDualRepository repository)
        {
            var packagesId = new HashSet<int>();

            for (short selectionListIndex = 0; selectionListIndex < repository.CurrentSelection.List.Count; selectionListIndex++)
            {
                var item = (EAContext)repository.CurrentSelection.List.GetAt(selectionListIndex);

                if (item.BaseType == nameof(Package))
                {
                    packagesId.Add(item.ElementID);
                }
            }

            return packagesId;
        }

        /// <summary>
        /// Function that verifies that an <see cref="XElement" /> matches a name
        /// </summary>
        /// <param name="matchingName">The name that have to match</param>
        /// <returns>A <see cref="Func{TResult}" /></returns>
        private static Func<XElement, bool> MatchElementByName(string matchingName)
        {
            return x => string.Equals(x.Name.LocalName, matchingName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Queries all <see cref="PackageWrapper" /> that exists inside the current EA project
        /// </summary>
        /// <param name="repository">The <see cref="IDualRepository" /></param>
        /// <returns>A read-only collection of <see cref="PackageWrapper" />, for all Package that exists inside the current EA project</returns>
        private static IReadOnlyCollection<PackageWrapper> QueriesAllExistingPackages(IDualRepository repository)
        {
            const string sqlQuery = "SELECT package.Package_Id as PACKAGE_ID, package.Parent_Id as PARENT_ID from t_package package";
            var sqlResponse = repository.SQLQuery(sqlQuery);
            var xElement = XElement.Parse(sqlResponse);
            var xRows = xElement.Descendants("Row");

            return
            [
                ..xRows.Select(r => new PackageWrapper
                {
                    PackageId = int.Parse(r.Elements().First(MatchElementByName("PACKAGE_ID")).Value, CultureInfo.InvariantCulture),
                    ContainerId = int.Parse(r.Elements().First(MatchElementByName("PARENT_ID")).Value, CultureInfo.InvariantCulture)
                })
            ];
        }
    }
}
