// -------------------------------------------------------------------------------------------------
// <copyright file="IBaseDialogViewModel.cs" company="Starion Group S.A.">
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
    using System;

    /// <summary>
    /// The <see cref="IBaseDialogViewModel" /> provides base behavior and properties definition for any viewmodel linked to a dialog
    /// </summary>
    internal interface IBaseDialogViewModel: IDisposable, ICloseableWindowViewModel
    {
        /// <summary>
        /// Asserts that the current viewModel is busy with a task
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// Asserts that the associated view should be the topmost or not
        /// </summary>
        bool IsTopMost { get; set; }
    }
}
