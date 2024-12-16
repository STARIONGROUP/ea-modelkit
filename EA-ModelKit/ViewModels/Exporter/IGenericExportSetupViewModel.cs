// -------------------------------------------------------------------------------------------------
// <copyright file="IGenericExportSetupViewModel.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.ComponentModel;

    using EA;

    using EAModelKit.Model.Slims;

    /// <summary>
    /// The <see cref="IGenericExportSetupViewModel" /> provides setup functionalities for values that have to be exported for Element of the same kind
    /// </summary>
    internal interface IGenericExportSetupViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the collection of selected TaggedValues name that are selected for export
        /// </summary>
        IEnumerable<string> SelectedTaggedValuesForExport { get; set; }

        /// <summary>
        /// Gets the associated Element Kind
        /// </summary>
        string ElementKind { get; }

        /// <summary>
        /// Asserts that all <see cref="Element" /> for the current stereotype should be exported or not
        /// </summary>
        bool ShouldBeExported { get; set; }

        /// <summary>
        /// Gets the collection of available TaggedValues name that could be exported
        /// </summary>
        IEnumerable<string> AvailableTaggedValuesForExport { get; }

        /// <summary>
        /// Asserts that any TaggedValue are available for export
        /// </summary>
        bool HaveAnyTaggedValues { get; }

        /// <summary>
        /// Asserts that any Connector are available for export
        /// </summary>
        bool HaveAnyConnectors { get; }

        /// <summary>
        /// Gets the collection of exportable <see cref="SlimElement" /> tied to this setup
        /// </summary>
        IReadOnlyList<SlimElement> ExportableElements { get; }

        /// <summary>
        /// Gets the collection of available Connectors kind that could be exported
        /// </summary>
        IEnumerable<string> AvailableConnectorsForExport { get; }

        /// <summary>
        /// Gets or sets the collection of selected Connectors kind that have to be exported
        /// </summary>
        IEnumerable<string> SelectedConnectorsForExport { get; set; }

        /// <summary>
        /// Gets the <see cref="Element" /> Type
        /// </summary>
        string ElementType { get; }
    }
}
