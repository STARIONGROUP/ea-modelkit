// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExporterViewModelTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.ViewModels.Exporter
{
    using DevExpress.Mvvm.Native;

    using EA;

    using EAModelKit.Behaviors;
    using EAModelKit.Model.Export;
    using EAModelKit.Model.Slims;
    using EAModelKit.Services.Cache;
    using EAModelKit.Services.Exporter;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.ViewBuilder;
    using EAModelKit.ViewModels.Exporter;

    using Moq;

    using NUnit.Framework;

    using Task = System.Threading.Tasks.Task;

    [TestFixture]
    public class GenericExporterViewModelTestFixture
    {
        private GenericExporterViewModel exporterViewModel;
        private Mock<ILoggerService> loggerService;
        private Mock<ICacheService> cacheService;
        private Mock<IViewBuilderService> builderService;
        private Mock<IGenericExporterService> exporterService;
        private List<Element> elements;
        private Mock<ICloseWindowBehavior> closeWindowBehavior;
        
        [SetUp]
        public void Setup()
        {
            this.loggerService = new Mock<ILoggerService>();
            this.cacheService = new Mock<ICacheService>();
            this.builderService = new Mock<IViewBuilderService>();
            this.exporterService = new Mock<IGenericExporterService>();
            
            this.exporterViewModel = new GenericExporterViewModel(this.loggerService.Object, this.cacheService.Object, this.builderService.Object, this.exporterService.Object);
            this.closeWindowBehavior = new Mock<ICloseWindowBehavior>();
            this.exporterViewModel.CloseWindowBehavior = this.closeWindowBehavior.Object;
            
            this.elements = 
            [
                CreateNewElement(4, "Requirement", ""),
                CreateNewElement(8, "Block", "Function"),
                CreateNewElement(16, "Block", "Product"),
                CreateNewElement(32, "Block", "Function"),
            ];
            
            var taggedValues = new List<SlimTaggedValue>
            {
                new ()
                {
                    ContainerId = 4,
                    Name = "ABC"
                },
                new ()
                {
                    ContainerId = 4,
                    Name = "DEF"
                },
                new ()
                {
                    ContainerId = 8,
                    Name = "123"
                },
                new ()
                {
                    ContainerId = 8,
                    Name = "456"
                },
                new ()
                {
                    ContainerId = 32,
                    Name = "123"
                }
            };

            var connectors = new List<SlimConnector>
            {
                CreateNewSlimConnector("Association", "implements", this.elements[0], this.elements[1]),
                CreateNewSlimConnector("Association", "", this.elements[2], this.elements[3]),
            };
            
            this.cacheService.Setup(x => x.GetTaggedValues(It.IsAny<int[]>()))
                .Returns(taggedValues);

            this.cacheService.Setup(x => x.GetAssociatedConnectors(this.elements[0].ElementID)).Returns([connectors[0]]);
            this.cacheService.Setup(x => x.GetAssociatedConnectors(this.elements[1].ElementID)).Returns([connectors[0]]);
            this.cacheService.Setup(x => x.GetAssociatedConnectors(this.elements[2].ElementID)).Returns([connectors[1]]);
            this.cacheService.Setup(x => x.GetAssociatedConnectors(this.elements[3].ElementID)).Returns([connectors[1]]);
        }

        [Test]
        public void VerifyViewModelProperties()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.exporterViewModel.CanProceed, Is.False);
                Assert.That(this.exporterViewModel.SelectedFilePath, Is.Null);
                Assert.That(this.exporterViewModel.ExportSetups.Items, Is.Empty);
                Assert.That(this.exporterViewModel.OutputFileCommand, Is.Null);
                Assert.That(this.exporterViewModel.ExportCommand, Is.Null);
            });
        }

        [Test]
        public void VerifyInitializeViewModel()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => this.exporterViewModel.InitializeViewModel(null), Throws.ArgumentNullException);
                Assert.That(() => this.exporterViewModel.InitializeViewModel([]), Throws.ArgumentException);
            });
            
            this.exporterViewModel.InitializeViewModel(this.elements);

            Assert.Multiple(() =>
            {
                this.cacheService.Verify(x => x.GetTaggedValues(It.IsAny<int[]>()), Times.Once);
                Assert.That(this.exporterViewModel.ExportSetups.Items, Is.Not.Empty);
                Assert.That(this.exporterViewModel.ExportSetups, Has.Count.EqualTo(3));
                Assert.That(this.exporterViewModel.OutputFileCommand, Is.Not.Null);
                Assert.That(this.exporterViewModel.ExportCommand, Is.Not.Null);
                Assert.That(this.exporterViewModel.ExportSetups.Items.All(x => x.ShouldBeExported), Is.True);
                
                Assert.That(this.exporterViewModel.ExportSetups.Items.Single(x => x.ElementKind == "Requirement").AvailableTaggedValuesForExport
                    .Count(), Is.EqualTo(2));
                
                Assert.That(this.exporterViewModel.ExportSetups.Items.Single(x => x.ElementKind == "Function").AvailableTaggedValuesForExport
                    .Count(), Is.EqualTo(2));
                
                Assert.That(this.exporterViewModel.ExportSetups.Items.Single(x => x.ElementKind == "Product").HaveAnyTaggedValues,
                    Is.False);
                
                Assert.That(this.exporterViewModel.ExportSetups.Items.All(x => x.AvailableTaggedValuesForExport.Count()
                == x.SelectedTaggedValuesForExport.Count()), Is.True);
            });
        }

        [Test]
        public void VerifyOutputFileCommand()
        {
            this.exporterViewModel.InitializeViewModel(this.elements);
            
            this.builderService.Setup(x => x.GetSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),0))
                .Returns(string.Empty);
            
            this.exporterViewModel.OutputFileCommand.Execute().Subscribe();
            Assert.That(this.exporterViewModel.SelectedFilePath, Is.Empty);
            
            const string selectedPath = @"a\path\to\file.xlsx";
            
            this.builderService.Setup(x => x.GetSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),0))
                .Returns(selectedPath);
            
            this.exporterViewModel.OutputFileCommand.Execute().Subscribe();
            Assert.That(this.exporterViewModel.SelectedFilePath, Is.EqualTo(selectedPath));
        }

        [Test]
        public void VerifyCanProceedComputation()
        {
            this.exporterViewModel.InitializeViewModel(this.elements);

            Assert.That(this.exporterViewModel.CanProceed, Is.False);
            
            this.builderService.Setup(x => x.GetSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),0))
                .Returns("abc");
            
            this.exporterViewModel.OutputFileCommand.Execute().Subscribe();
            Assert.That(this.exporterViewModel.CanProceed, Is.True);
            this.exporterViewModel.ExportSetups.Items.ForEach(x => x.ShouldBeExported = false);
            
            Assert.That(this.exporterViewModel.CanProceed, Is.False);
            this.exporterViewModel.ExportSetups.Items[0].ShouldBeExported = true;
            Assert.That(this.exporterViewModel.CanProceed, Is.True);
        }

        [Test]
        public void VerifyExportCommand()
        {
            this.exporterViewModel.InitializeViewModel(this.elements);
            
            this.builderService.Setup(x => x.GetSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),0))
                .Returns("abc");

            this.exporterService.Setup(x => x.ExportElementsAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<GenericExportConfiguration>>()))
                .Returns(Task.CompletedTask);
            
            this.exporterViewModel.OutputFileCommand.Execute().Subscribe();
            this.exporterViewModel.ExportCommand.Execute().Subscribe();

            this.closeWindowBehavior.Verify(x => x.Close(), Times.Once);
            
            this.exporterService.Setup(x => x.ExportElementsAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<GenericExportConfiguration>>()))
                .ThrowsAsync(new InvalidOperationException());
            
            this.exporterViewModel.ExportCommand.Execute().Subscribe();

            Assert.Multiple(() =>
            {
                this.closeWindowBehavior.Verify(x => x.Close(), Times.Once);
                this.loggerService.Verify(x => x.LogException(It.IsAny<InvalidOperationException>(), It.IsAny<string>()), Times.Once);
            });
        }

        private static Element CreateNewElement(int id, string typeName, string stereotype)
        {
            var element = new Mock<Element>();
            element.Setup(x => x.ElementID).Returns(id);
            element.Setup(x => x.Type).Returns(typeName);
            element.Setup(x => x.Stereotype).Returns(stereotype);
            
            return element.Object;
        }

        private static SlimConnector CreateNewSlimConnector(string connectorType, string connectorStereotype, Element source, Element target)
        {
            var connector = new Mock<Connector>();
            connector.Setup(x => x.Stereotype).Returns(connectorStereotype);
            connector.Setup(x => x.Type).Returns(connectorType);
            connector.Setup(x => x.ClientID).Returns(source.ElementID);
            connector.Setup(x => x.SupplierID).Returns(target.ElementID);
            return new SlimConnector(connector.Object, source, target);
        }
    }
}
