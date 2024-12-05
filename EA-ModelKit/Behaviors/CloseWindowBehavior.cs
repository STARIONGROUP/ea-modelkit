// -------------------------------------------------------------------------------------------------
// <copyright file="CloseWindowBehavior.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Behaviors
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using DevExpress.Mvvm.UI.Interactivity;

    using EAModelKit.Utilities.Dialogs;

    /// <summary>
    /// Attachable behavior for a view that can be close from its view model
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class CloseWindowBehavior: Behavior<Window>, ICloseWindowBehavior 
    {
        /// <summary>
        /// Closes the <see cref="Behavior{T}.AssociatedObject" /> view
        /// </summary>
        public void Close()
        {
            this.AssociatedObject.Close();
        }

        /// <summary>
        /// Occurs when this behavior attaches to a view
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.DataContextChanged += this.DataContextChanged;
        }

        /// <summary>
        /// Occurs when this behavior detaches from its associated view
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.DataContextChanged -= this.DataContextChanged;
        }

        /// <summary>
        /// Occurs when the data context of <see cref="Behavior{T}.AssociatedObject" /> changes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /></param>
        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((ICloseableWindowViewModel)this.AssociatedObject.DataContext).CloseWindowBehavior = this;
        }
    }
}
