// -------------------------------------------------------------------------------------------------
// <copyright file="ViewBuilderService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.ViewBuilder
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using Autofac;

    using DevExpress.Xpf.Core;

    using Microsoft.Win32;

    /// <summary>
    /// The <see cref="ViewBuilderService" /> provides build feature for views.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ViewBuilderService : IViewBuilderService
    {
        /// <summary>
        /// Brings a <see cref="ThemedWindow" /> to the user sight as a modal with it's associated view model of the provided type
        /// <typeparamref name="TView" />
        /// </summary>
        /// <typeparam name="TView">Any <see cref="ThemedWindow" /></typeparam>
        /// <typeparam name="TViewModel">The View Model to associate with the view</typeparam>
        /// <param name="viewModel">The View Model instance</param>
        /// <returns>A value indicating the dialog result</returns>
        public bool? ShowDxDialog<TView, TViewModel>(TViewModel viewModel = default) where TView : ThemedWindow, new() where TViewModel : class
        {
            return BuildView<TView, TViewModel>(viewModel).ShowDialog();
        }

        /// <summary>
        /// Gets the location of the file to be saved
        /// </summary>
        /// <param name="defaultFilename">the default filename</param>
        /// <param name="extension">the extension of the file to create</param>
        /// <param name="filter">the filter for the dialog</param>
        /// <param name="filterIndex">the index of the filter currently selected</param>
        /// <returns>the path of the file to create or null if the operation was cancelled.</returns>
        public string GetSaveFileDialog(string defaultFilename, string extension, string filter, int filterIndex)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFilename,
                DefaultExt = extension,
                Filter = filter
            };

            if (!string.IsNullOrEmpty(saveFileDialog.Filter))
            {
                saveFileDialog.FilterIndex = filterIndex;
            }

            var showDialog = saveFileDialog.ShowDialog();
            var result = showDialog != null && (bool)showDialog;

            return result ? saveFileDialog.FileName : null;
        }

        /// <summary>
        /// Builds up the view instance with it's associated view model of the provided type <typeparamref name="TViewModel" />
        /// </summary>
        /// <typeparam name="TView">The view <see cref="System.Type" /> to show</typeparam>
        /// <typeparam name="TViewModel">The View Model <see cref="System.Type" /> to associate with the view</typeparam>
        /// <param name="viewModel">The View Model instance</param>
        private static TView BuildView<TView, TViewModel>(TViewModel viewModel) where TView : Window, new() where TViewModel : class
        {
            viewModel ??= App.Container.Resolve<TViewModel>();
            return new TView { DataContext = viewModel };
        }
    }
}
