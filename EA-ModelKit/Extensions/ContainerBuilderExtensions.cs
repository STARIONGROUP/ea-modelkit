// -------------------------------------------------------------------------------------------------
// <copyright file="ContainerBuilderExtensions.cs" company="Starion Group S.A.">
// 
//     Copyright (C) 2024-2024 Starion Group S.A.
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
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace EAModelKit.Extensions
{
    using Autofac;

    using AutofacSerilogIntegration;

    using EAModelKit.Services.Dispatcher;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Selection;
    using EAModelKit.Services.Version;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension class for <see cref="ContainerBuilder" />
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Register all required services for the application
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder" /></param>
        public static void RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterLogger();

            builder.RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();

            builder.RegisterType<LoggerService>().As<ILoggerService>().SingleInstance();
            builder.RegisterType<VersionService>().As<IVersionService>().SingleInstance();
            builder.RegisterType<SelectionService>().As<ISelectionService>().SingleInstance();
            builder.RegisterType<DispatcherService>().As<IDispatcherService>().SingleInstance();
        }

        /// <summary>
        /// Register all required ViewModel for the application
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder" /></param>
        public static void RegisterViewModels(this ContainerBuilder builder)
        {
        }
    }
}
