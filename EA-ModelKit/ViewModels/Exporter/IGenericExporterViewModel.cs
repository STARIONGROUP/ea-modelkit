// -------------------------------------------------------------------------------------------------
// <copyright file="IGenericExporterViewModel.cs" company="Starion Group S.A.">
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
    using System.Reactive;

    using DynamicData;

    using EA;

    using EAModelKit.Utilities.Dialogs;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="IGenericExporterViewModel" /> is a ViewModel that provides user interaction to export data
    /// </summary>
    internal interface IGenericExporterViewModel : IBaseDialogViewModel
    {
        /// <summary>
        /// Gets the <see cref="SourceList{T}" /> of <see cref="IGenericExportSetupViewModel" />
        /// </summary>
        SourceList<IGenericExportSetupViewModel> ExportSetups { get; }

        /// <summary>
        /// Gets the path to the file that should be use for export
        /// </summary>
        string SelectedFilePath { get; }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand{T1,T2}" /> that allows the selection of the output file
        /// </summary>
        ReactiveCommand<Unit, Unit> OutputFileCommand { get; }

        /// <summary>
        /// Asserts if the <see cref="ExportCommand" /> can be executed or not
        /// </summary>
        bool CanProceed { get; set; }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand{TParam,TResult}" /> that allows the export of <see cref="Element" /> data
        /// </summary>
        ReactiveCommand<Unit, Unit> ExportCommand { get; }

        /// <summary>
        /// Initialies properties of the ViewModel
        /// </summary>
        /// <param name="elements">A collection of <see cref="Element" /> that have been selected for export</param>
        void InitializeViewModel(IReadOnlyList<Element> elements);
    }
}
