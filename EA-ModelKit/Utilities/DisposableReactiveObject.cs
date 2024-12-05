// -------------------------------------------------------------------------------------------------
// <copyright file="DisposableReactiveObject.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Utilities
{
    using System;
    using System.Collections.Generic;

    using ReactiveUI;

    /// <summary>
    /// Utility class that implements the <see cref="IDisposable" /> interface and that is a <see cref="ReactiveObject" />
    /// </summary>
    public abstract class DisposableReactiveObject : ReactiveObject, IDisposable
    {
        /// <summary>
        /// A collection of <see cref="IDisposable" />
        /// </summary>
        protected readonly List<IDisposable> Disposables = [];

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Value asserting if this component should dispose or not</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.Disposables.ForEach(x => x.Dispose());
            this.Disposables.Clear();
        }
    }
}
