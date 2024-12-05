// -------------------------------------------------------------------------------------------------
// <copyright file="DispatcherServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Dispatcher
{
    using Autofac;

    using EA;

    using EAModelKit.Services.Cache;
    using EAModelKit.Services.Dispatcher;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Selection;
    using EAModelKit.Services.ViewBuilder;
    using EAModelKit.ViewModels.Exporter;
    using EAModelKit.Views.Export;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using App = EAModelKit.App;

    [TestFixture]
    public class DispatcherServiceTestFixture
    {
        private DispatcherService dispatcher;
        private Mock<ISelectionService> selectionService;
        private Mock<ILoggerService> loggerService;
        private Mock<Repository> repository;
        private Mock<IViewBuilderService> viewBuilderService;
        private Mock<ICacheService> cacheService;
        
        [SetUp]
        public void Setup()
        {
            this.selectionService = new Mock<ISelectionService>();
            this.loggerService = new Mock<ILoggerService>();
            this.repository = new Mock<Repository>();
            this.viewBuilderService = new Mock<IViewBuilderService>();
            this.cacheService = new Mock<ICacheService>();
            
            this.dispatcher = new DispatcherService(this.loggerService.Object, this.selectionService.Object, this.viewBuilderService.Object, this.cacheService.Object);
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
            this.selectionService.Setup(x => x.QuerySelectedElements(this.repository.Object)).Returns([]);
            this.dispatcher.OnGenericExport(this.repository.Object);

            this.loggerService.Verify(x => x.Log(LogLevel.Warning, It.IsAny<string>()), Times.Once);
            
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

            var exporterViewModel = new Mock<IGenericExporterViewModel>();
            var container = new ContainerBuilder();
            container.RegisterInstance(exporterViewModel.Object);
            App.BuildContainer(container);
            this.selectionService.Setup(x => x.QuerySelectedElements(this.repository.Object)).Returns(selectedElements);
            this.dispatcher.OnGenericExport(this.repository.Object);
            
            Assert.Multiple(() =>
            {
                exporterViewModel.Verify(x => x.InitializeViewModel(selectedElements), Times.Once);
                this.viewBuilderService.Verify(x => x.ShowDxDialog<GenericExport, IGenericExporterViewModel>(exporterViewModel.Object), Times.Once);
            });
        }

        [Test]
        public void VerifyResetServices()
        {
            this.dispatcher.OnFileOpen(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Once);
            
            this.dispatcher.OnFileNew(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(2));

            this.dispatcher.OnPostNewPackage(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(3));
            
            this.dispatcher.OnPostNewElement(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(4));
            
            this.dispatcher.OnPostNewConnector(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(5));
            
            this.dispatcher.OnPostNewAttribute(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(6));
            
            this.dispatcher.OnPreDeleteElement(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(7));
            
            this.dispatcher.OnPreDeleteAttribute(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(8));
            
            this.dispatcher.OnPreDeleteConnector(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(9));
            
            this.dispatcher.OnPreDeletePackage(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(10));
            
            this.dispatcher.OnPostNewDiagram(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(11));
            
            this.dispatcher.OnPreDeleteDiagram(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(12));
            
            this.dispatcher.OnNotifyContextItemModified(this.repository.Object);
            this.cacheService.Verify(x => x.Initialize(this.repository.Object), Times.Exactly(13));
        }

        private static Element CreateNewElement(string stereotypeName)
        {
            var element = new Mock<Element>();
            element.Setup(x => x.Stereotype).Returns(stereotypeName);
            return element.Object;
        }
    }
}
