// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExportSetupViewModel.cs" company="Starion Group S.A.">
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

namespace EAModelKit.ViewModels.Exporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EA;

    using EAModelKit.Model.Slims;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="GenericExportSetupViewModel" /> provides setup functionalities for values that have to be exported for Element of the same kind
    /// </summary>
    internal class GenericExportSetupViewModel : ReactiveObject, IGenericExportSetupViewModel
    {
        /// <summary>
        /// Backing field for <see cref="SelectedConnectorsForExport" />
        /// </summary>
        private IEnumerable<string> selectedConnectorsForExport;

        /// <summary>
        /// Backing field for <see cref="SelectedTaggedValuesForExport" />
        /// </summary>
        private IEnumerable<string> selectedTaggedValuesForExport;

        /// <summary>
        /// Backing field for <see cref="ShouldBeExported" />
        /// </summary>
        private bool shouldBeExported = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExportSetupViewModel" /> class.
        /// </summary>
        /// <param name="elements">The read-only list of <see cref="SlimElement" /></param>
        public GenericExportSetupViewModel(IReadOnlyList<SlimElement> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (elements.Count == 0)
            {
                throw new ArgumentException("The collection of elements cannot be empty", nameof(elements));
            }

            this.ElementKind = elements[0].ElementKind;
            this.ElementType = elements[0].ElementType;

            this.AvailableTaggedValuesForExport = elements
                .SelectMany(x => x.TaggedValues.Keys)
                .Distinct()
                .OrderBy(x => x);

            this.AvailableConnectorsForExport = elements
                .SelectMany(x => x.Connectors.Keys)
                .Distinct()
                .OrderBy(x => x);

            this.SelectedTaggedValuesForExport = [..this.AvailableTaggedValuesForExport];
            this.SelectedConnectorsForExport = [..this.AvailableConnectorsForExport];
            this.ExportableElements = elements;
        }

        /// <summary>
        /// Gets the <see cref="Element" /> Type
        /// </summary>
        public string ElementType { get; }

        /// <summary>
        /// Gets the collection of available Connectors kind that could be exported
        /// </summary>
        public IEnumerable<string> AvailableConnectorsForExport { get; }

        /// <summary>
        /// Gets or sets the collection of selected Connectors kind that have to be exported
        /// </summary>
        public IEnumerable<string> SelectedConnectorsForExport
        {
            get => this.selectedConnectorsForExport;
            set => this.RaiseAndSetIfChanged(ref this.selectedConnectorsForExport, value);
        }

        /// <summary>
        /// Asserts that any Connector are available for export
        /// </summary>
        public bool HaveAnyConnectors => this.AvailableConnectorsForExport.Any();

        /// <summary>
        /// Gets the collection of exportable <see cref="SlimElement" /> tied to this setup
        /// </summary>
        public IReadOnlyList<SlimElement> ExportableElements { get; }

        /// <summary>
        /// Asserts that any TaggedValue are available for export
        /// </summary>
        public bool HaveAnyTaggedValues => this.AvailableTaggedValuesForExport.Any();

        /// <summary>
        /// Gets or sets the collection of selected TaggedValues name that are selected for export
        /// </summary>
        public IEnumerable<string> SelectedTaggedValuesForExport
        {
            get => this.selectedTaggedValuesForExport;
            set => this.RaiseAndSetIfChanged(ref this.selectedTaggedValuesForExport, value);
        }

        /// <summary>
        /// Gets the associated Element Kind
        /// </summary>
        public string ElementKind { get; }

        /// <summary>
        /// Asserts that all <see cref="Element" /> for the current stereotype should be exported or not
        /// </summary>
        public bool ShouldBeExported
        {
            get => this.shouldBeExported;
            set => this.RaiseAndSetIfChanged(ref this.shouldBeExported, value);
        }

        /// <summary>
        /// Gets the collection of available TaggedValues name that could be exported
        /// </summary>
        public IEnumerable<string> AvailableTaggedValuesForExport { get; }
    }
}
