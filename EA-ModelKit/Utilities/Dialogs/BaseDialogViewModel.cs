// -------------------------------------------------------------------------------------------------
// <copyright file="BaseDialogViewModel.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Utilities.Dialogs
{
    using System.Reactive;

    using EAModelKit.Behaviors;
    using EAModelKit.Services.Logger;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="BaseDialogViewModel" /> provides base behavior and properties definition for any viewmodel linked to a dialog
    /// </summary>
    internal abstract class BaseDialogViewModel: DisposableReactiveObject, IBaseDialogViewModel
    {
        /// <summary>
        /// Backing field for <see cref="IsBusy" />
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Backing field for <see cref="IsTopMost" />
        /// </summary>
        private bool isTopMost = true;

        /// <summary>
        /// Gets the injected <see cref="ILoggerService" />
        /// </summary>
        protected ILoggerService LoggerService { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="BaseDialogViewModel" />
        /// </summary>
        /// <param name="loggerService">The <see cref="ILoggerService" /></param>
        protected BaseDialogViewModel(ILoggerService loggerService)
        {
            this.LoggerService = loggerService;
            this.CancelCommand = ReactiveCommand.Create(() => this.CloseWindowBehavior?.Close());
        }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand{T1, T2}" /> that cancel the operation
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        /// <summary>
        /// Gets or sets the <see cref="ICloseWindowBehavior" />
        /// </summary>
        public ICloseWindowBehavior CloseWindowBehavior { get; set; }

        /// <summary>
        /// Asserts that the current viewModel is busy with a task
        /// </summary>
        public bool IsBusy
        {
            get => this.isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        /// <summary>
        /// Asserts that the view should be the topmost or not
        /// </summary>
        public bool IsTopMost
        {
            get => this.isTopMost;
            set => this.RaiseAndSetIfChanged(ref this.isTopMost, value);
        }
    }
}
