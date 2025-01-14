﻿// -------------------------------------------------------------------------------------------------
// <copyright file="IDispatcherService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Dispatcher
{
    using EA;

    /// <summary>
    /// The <see cref="IDispatcherService" /> provides EA events abstraction layer and available actions entry point
    /// </summary>
    internal interface IDispatcherService
    {
        /// <summary>
        /// Handles the connection to EA
        /// </summary>
        /// <param name="repository">The EA <see cref="Repository" /></param>
        void Connect(Repository repository);

        /// <summary>
        /// Handles the disconnection to EA
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Handles the Generic Export action
        /// </summary>
        /// <param name="repository">The EA <see cref="Repository" /></param>
        void OnGenericExport(Repository repository);

        /// <summary>
        /// Handles the file opening event
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnFileOpen(Repository repository);

        /// <summary>
        /// Handles the creation of a new file
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnFileNew(Repository repository);

        /// <summary>
        /// Handle the post-creation of a <see cref="Package" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPostNewPackage(Repository repository);

        /// <summary>
        /// Handle the post-creation of an <see cref="Element" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPostNewElement(Repository repository);

        /// <summary>
        /// Handle the post-creation of a <see cref="Connector" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPostNewConnector(Repository repository);

        /// <summary>
        /// Handle the post-creation of an <see cref="Attribute" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPostNewAttribute(Repository repository);

        /// <summary>
        /// Handle the pre-deletion of an <see cref="Element" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPreDeleteElement(Repository repository);

        /// <summary>
        /// Handle the pre-deletion of an <see cref="Attribute" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPreDeleteAttribute(Repository repository);

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Connector" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPreDeleteConnector(Repository repository);

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Package" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPreDeletePackage(Repository repository);

        /// <summary>
        /// Handle the post-creation of a <see cref="Diagram" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository"/></param>
        void OnPostNewDiagram(Repository repository);

        /// <summary>
        /// Handle the pre-deletion of a <see cref="Diagram" />
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnPreDeleteDiagram(Repository repository);

        /// <summary>
        /// Handle the modification of the current item
        /// </summary>
        /// <param name="repository">The <see cref="Repository" /></param>
        void OnNotifyContextItemModified(Repository repository);
    }
}
