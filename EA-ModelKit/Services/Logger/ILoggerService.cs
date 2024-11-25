// -------------------------------------------------------------------------------------------------
//   <copyright file="ILoggerService.cs" company="Starion Group S.A.">
// 
//     Copyright 2024 Starion Group S.A.
// 
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
//     Unless required by applicable law or agreed to in writing, softwareUseCases
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// 
//   </copyright>
//   ------------------------------------------------------------------------------------------------

namespace EAModelKit.Services.Logger
{
    using System;

    using EA;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="ILoggerService" /> provides logging to file and to Entreprise Architect output system capabilities
    /// </summary>
    internal interface ILoggerService
    {
        /// <summary>
        /// Initialize this service
        /// </summary>
        /// <param name="initialRepository">The <see cref="Repository" /></param>
        void InitializeService(Repository initialRepository);

        /// <summary>
        /// Logs a message for a certain <see cref="LogLevel" />
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel" /></param>
        /// <param name="message">The message to log</param>
        /// <param name="arguments">A collection of arguments for the message formatting</param>
        void Log(LogLevel logLevel, string message, params object[] arguments);

        /// <summary>
        /// Logs an <see cref="Exception" />
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="arguments">A collection of arguments for the message formatting</param>
        void LogException(Exception exception, string message, params object[] arguments);
    }
}
