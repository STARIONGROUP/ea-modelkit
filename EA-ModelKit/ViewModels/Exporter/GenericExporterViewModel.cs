// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExporterViewModel.cs" company="Starion Group S.A.">
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
    using System.Reactive;
    using System.Windows.Input;

    using DynamicData;
    using DynamicData.Binding;

    using EA;

    using EAModelKit.Model.Export;
    using EAModelKit.Model.Slims;
    using EAModelKit.Services.Cache;
    using EAModelKit.Services.Exporter;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.ViewBuilder;
    using EAModelKit.Utilities.Dialogs;

    using ReactiveUI;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The <see cref="GenericExporterViewModel" /> is a ViewModel that provides user interaction to export data
    /// </summary>
    internal class GenericExporterViewModel : BaseDialogViewModel, IGenericExporterViewModel
    {
        /// <summary>
        /// The injected <see cref="ICacheService" />
        /// </summary>
        private readonly ICacheService cacheService;

        /// <summary>
        /// The injected <see cref="IViewBuilderService" />
        /// </summary>
        private readonly IViewBuilderService viewBuilderService;

        /// <summary>
        /// Backing field for <see cref="CanProceed" />
        /// </summary>
        private bool canProceed;

        /// <summary>
        /// Backing field for <see cref="SelectedFilePath" />
        /// </summary>
        private string selectedFilePath;

        /// <summary>
        /// Gets the injected <see cref="IGenericExporterService"/>
        /// </summary>
        private readonly IGenericExporterService exporterService;

        /// <summary>
        /// Initialize a new instance of <see cref="GenericExporterViewModel" />
        /// </summary>
        /// <param name="loggerService">The <see cref="ILoggerService" /></param>
        /// <param name="cacheService">The injected <see cref="ICacheService" /></param>
        /// <param name="viewBuilderService">The injected <see cref="IViewBuilderService" /></param>
        /// <param name="exporterService">The injected <see cref="IGenericExporterService"/></param>
        public GenericExporterViewModel(ILoggerService loggerService, ICacheService cacheService, IViewBuilderService viewBuilderService,
            IGenericExporterService exporterService) : base(loggerService)
        {
            this.cacheService = cacheService;
            this.viewBuilderService = viewBuilderService;
            this.exporterService = exporterService;
        }

        /// <summary>
        /// Gets the path to the file that should be use for export
        /// </summary>
        public string SelectedFilePath
        {
            get => this.selectedFilePath;
            private set => this.RaiseAndSetIfChanged(ref this.selectedFilePath, value);
        }

        /// <summary>
        /// Asserts if the <see cref="ExportCommand" /> can be executed or not
        /// </summary>
        public bool CanProceed
        {
            get => this.canProceed;
            set => this.RaiseAndSetIfChanged(ref this.canProceed, value);
        }

        /// <summary>
        /// Gets the <see cref="SourceList{T}" /> of <see cref="IGenericExportSetupViewModel" />
        /// </summary>
        public SourceList<IGenericExportSetupViewModel> ExportSetups { get; } = new();

        /// <summary>
        /// Gets the <see cref="ReactiveCommand{TParam,TResult}" /> that allows the selection of the output file
        /// </summary>
        public ReactiveCommand<Unit, Unit> OutputFileCommand { get; private set; }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand{TParam,TResult}" /> that allows the export of <see cref="Element" /> data
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExportCommand { get; private set; }

        /// <summary>
        /// Initialies properties of the ViewModel
        /// </summary>
        /// <param name="elements">A collection of <see cref="Element" /> that have been selected for export</param>
        public void InitializeViewModel(IReadOnlyList<Element> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (elements.Count == 0)
            {
                throw new ArgumentException("The collection of Element to export can not be empty", nameof(elements));
            }

            var taggedValues = this.cacheService.GetTaggedValues([..elements.Select(x => x.ElementID)])
                .GroupBy(x => x.ContainerId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var slimElements = elements.Select(x => new SlimElement(x, taggedValues.TryGetValue(x.ElementID, out var existingTaggedValues)
                ? existingTaggedValues
                : []));

            this.ExportSetups.AddRange(slimElements.GroupBy(x => x.ElementKind)
                .Select(e => new GenericExportSetupViewModel(e.ToList())));

            this.InitializeObservablesAndCommands();
        }

        /// <summary>
        /// Initializes <see cref="IObservable{T}" /> and <see cref="ICommand" /> for this view model
        /// </summary>
        private void InitializeObservablesAndCommands()
        {
            this.OutputFileCommand = ReactiveCommand.Create(this.OnOutputFileSelect);

            this.Disposables.Add(this.ExportSetups.Connect().WhenPropertyChanged(x => x.ShouldBeExported)
                .Subscribe(_ => this.ComputeCanProceed()));

            this.Disposables.Add(this.WhenPropertyChanged(x => x.SelectedFilePath).Subscribe(_ => this.ComputeCanProceed()));
            this.ExportCommand = ReactiveCommand.CreateFromTask(this.OnExport, this.WhenAnyValue(x => x.CanProceed));
        }

        /// <summary>
        /// Proceed with the export functionalitis following the user-defined setup
        /// </summary>
        private async Task OnExport()
        {
            this.IsBusy = true;

            try
            {
                var exportConfiguration = this.ExportSetups.Items.Where(x => x.ShouldBeExported)
                    .Select(x => new GenericExportConfiguration(x.ExportableElements, x.SelectedTaggedValuesForExport.ToList()));
                
                await this.exporterService.ExportElementsAsync(this.selectedFilePath, [..exportConfiguration]);
                this.CloseWindowBehavior.Close();
            }
            catch (Exception ex)
            {
                this.LoggerService.LogException(ex, "An error occured while exporting data.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Computes the value of <see cref="CanProceed" />
        /// </summary>
        private void ComputeCanProceed()
        {
            if (string.IsNullOrEmpty(this.SelectedFilePath))
            {
                this.CanProceed = false;
                return;
            }

            this.CanProceed = this.ExportSetups.Items.Any(x => x.ShouldBeExported);
        }

        /// <summary>
        /// Handles the behavior of the output file selection
        /// </summary>
        private void OnOutputFileSelect()
        {
            this.IsTopMost = false;
            this.SelectedFilePath = this.viewBuilderService.GetSaveFileDialog("Export", ".xlsx", "Excel File|*.xlsx", 0);
            this.IsTopMost = true;
        }
    }
}
