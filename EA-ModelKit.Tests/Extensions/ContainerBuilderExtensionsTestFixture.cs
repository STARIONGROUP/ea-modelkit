// -------------------------------------------------------------------------------------------------
//   <copyright file="ContainerBuilderExtensionsTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Extensions
{
    using Autofac;

    using EAModelKit.Extensions;

    using NUnit.Framework;

    [TestFixture]
    public class ContainerBuilderExtensionsTestFixture
    {
        private ContainerBuilder containerBuilder;

        [SetUp]
        public void SetUp()
        {
            this.containerBuilder = new ContainerBuilder();
        }

        [Test]
        public void VerifyContainerBuilderExtensions()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => this.containerBuilder.RegisterViewModels(), Throws.Nothing);                
                Assert.That(() => this.containerBuilder.RegisterServices(), Throws.Nothing);                
                Assert.That(() => this.containerBuilder.Build(), Throws.Nothing);                
            });
        }
    }
}
