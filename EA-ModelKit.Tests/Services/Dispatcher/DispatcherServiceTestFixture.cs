// -------------------------------------------------------------------------------------------------
//   <copyright file="DispatcherServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Dispatcher
{
    using EA;

    using EAModelKit.Services.Dispatcher;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Selection;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DispatcherServiceTestFixture
    {
        private DispatcherService dispatcher;
        private Mock<ISelectionService> selectionService;
        private Mock<ILoggerService> loggerService;
        private Mock<Repository> repository;
        
        [SetUp]
        public void Setup()
        {
            this.selectionService = new Mock<ISelectionService>();
            this.loggerService = new Mock<ILoggerService>();
            this.repository = new Mock<Repository>();
            
            this.dispatcher = new DispatcherService(this.loggerService.Object, this.selectionService.Object);
        }

        [Test]
        public void VerifyConnect()
        {
            this.dispatcher.Connect(this.repository.Object);

            Assert.Multiple(() =>
            {
                this.loggerService.Verify(x => x.InitializeService(this.repository.Object), Times.Once);
                this.loggerService.Verify(x => x.Log(LogLevel.Information, "EA Model-Kit plugin successfully connected to EA"), Times.Once);
            });
        }

        [Test]
        public void VerifyDisconnect()
        {
            this.dispatcher.Disconnect();
            this.loggerService.Verify(x => x.Log(LogLevel.Information, "EA Model-Kit plugin successfully disconnected to EA"), Times.Once);
        }

        [Test]
        public void VerifyOnGenericExport()
        {
            var selectedElements = new List<Element>
            {
                CreateNewElement("Requirement"),
                CreateNewElement("Requirement"),
                CreateNewElement("Requirement"),
                CreateNewElement("Requirement"),
                CreateNewElement("Block"),
                CreateNewElement("Block"),
                CreateNewElement("Block")
            };
            
            this.selectionService.Setup(x => x.QuerySelectedElements(this.repository.Object)).Returns(selectedElements);
            this.dispatcher.OnGenericExport(this.repository.Object);

            Assert.Multiple(() =>
            {
                this.loggerService.Verify(x => x.Log(LogLevel.Debug, "Found {0} Elements With Stereotype {1}", 4, "Requirement"), Times.Once);
                this.loggerService.Verify(x => x.Log(LogLevel.Debug, "Found {0} Elements With Stereotype {1}", 3, "Block"), Times.Once);
            });
        }

        private static Element CreateNewElement(string stereotypeName)
        {
            var element = new Mock<Element>();
            element.Setup(x => x.Stereotype).Returns(stereotypeName);
            return element.Object;
        }
    }
}
