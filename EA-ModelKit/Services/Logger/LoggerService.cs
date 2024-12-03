// -------------------------------------------------------------------------------------------------
// <copyright file="LoggerService.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Services.Logger
{
    using System;

    using EA;

    using EAModelKit.Extensions;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="ILoggerService" /> provides logging to file and to Entreprise Architect output system capabilities
    /// </summary>
    internal class LoggerService : ILoggerService
    {
        /// <summary>
        /// The name of the Output Tab
        /// </summary>
        internal const string TabName = "EA ModelKit";

        /// <summary>
        /// Asserts if the plugin is running as debug levle
        /// </summary>
        private readonly bool isDebug;

        /// <summary>
        /// Gets the injected <see cref="ILogger" />
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the <see cref="Repository" /> that is used to log to Entreprise Architect
        /// </summary>
        private Repository repository;

        /// <summary>
        /// Initializes a new instance of <see cref="ILogger" />
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" /></param>
        public LoggerService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(nameof(LoggerService));
#if DEBUG
            this.isDebug = true;
#endif
        }

        /// <summary>
        /// Initialize this service
        /// </summary>
        /// <param name="initialRepository">The <see cref="Repository" /></param>
        public void InitializeService(Repository initialRepository)
        {
            this.repository = initialRepository;
        }

        /// <summary>
        /// Logs a message for a certain <see cref="LogLevel" />
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel" /></param>
        /// <param name="message">The message to log</param>
        /// <param name="arguments">A collection of arguments for the message formatting</param>
        public void Log(LogLevel logLevel, string message, params object[] arguments)
        {
            this.logger.Log(logLevel, message, arguments);

            if (logLevel != LogLevel.Debug || this.isDebug)
            {
                this.WriteToEnterpriseArchitect(message, arguments);
            }
        }

        /// <summary>
        /// Logs an <see cref="Exception" />
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="arguments">A collection of arguments for the message formatting</param>
        public void LogException(Exception exception, string message, params object[] arguments)
        {
            this.logger.Log(LogLevel.Critical, exception, message, arguments);
            this.WriteToEnterpriseArchitect(message, arguments);
        }

        /// <summary>
        /// Writes a message into the Enterprise Architect output
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="arguments">A collection of arguments for the message formatting</param>
        private void WriteToEnterpriseArchitect(string message, params object[] arguments)
        {
            if (string.IsNullOrEmpty(message) || this.repository == null)
            {
                return;
            }

            if (this.repository.IsProjectOpen() && this.repository.IsTabOpen(TabName) == 0)
            {
                this.repository.CreateOutputTab(TabName);
            }

            this.repository.EnsureOutputVisible(TabName);
            this.repository.WriteOutput(TabName, string.Format(message, arguments), 0);
        }
    }
}
