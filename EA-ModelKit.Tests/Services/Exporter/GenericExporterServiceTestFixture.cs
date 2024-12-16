// -------------------------------------------------------------------------------------------------
// <copyright file="GenericExporterServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Exporter
{
    using EA;

    using EAModelKit.Model.Export;
    using EAModelKit.Model.Slims;
    using EAModelKit.Services.Exporter;
    using EAModelKit.Services.Logger;
    using EAModelKit.Services.Writer;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using Task = Task;

    [TestFixture]
    public class GenericExporterServiceTestFixture
    {
        private GenericExporterService exporterService;
        private Mock<ILoggerService> loggerService;
        private Mock<IExcelWriter> excelWriter;

        [SetUp]
        public void Setup()
        {
            this.loggerService = new Mock<ILoggerService>();
            this.excelWriter = new Mock<IExcelWriter>();
            this.exporterService = new GenericExporterService(this.loggerService.Object, this.excelWriter.Object);
        }

        [Test]
        public async Task VerifyExportElements()
        {
            var slimTaggedValue = new SlimTaggedValue
            {
                ContainerId = 1,
                Name = "abc",
                Value = "15"
            };
            
            var element = new Mock<Element>();
            element.Setup(x => x.Name).Returns("abc");
            element.Setup(x => x.Stereotype).Returns("Function");
            element.Setup(x => x.ElementID).Returns(slimTaggedValue.ContainerId);

            var targetElement = new Mock<Element>();
            targetElement.Setup(x => x.Name).Returns("def");
            element.Setup(x => x.Stereotype).Returns("");
            element.Setup(x => x.Type).Returns("Requirement");
            element.Setup(x => x.ElementID).Returns(45);
            
            var connector = new Mock<Connector>();
            connector.Setup(x => x.Stereotype).Returns("");
            connector.Setup(x => x.Type).Returns("Dependency");
            connector.Setup(x => x.ClientID).Returns(element.Object.ElementID);
            connector.Setup(x => x.SupplierID).Returns(targetElement.Object.ElementID);
            
            var slimConnector = new SlimConnector(connector.Object, element.Object, targetElement.Object);
            var slimElement = new SlimElement(element.Object, [slimTaggedValue], [slimConnector]);
            
            var genericConfigurations = new List<GenericExportConfiguration>
            {
                new([slimElement], [slimTaggedValue.Name], [slimConnector.ComputeConnectorKindFullName(slimElement.ElementId)])
            };

            this.excelWriter.Setup(x => x.WriteAsync(It.IsAny<Dictionary<string, IReadOnlyList<ExportableObject>>>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await this.exporterService.ExportElementsAsync("abcpath", genericConfigurations);

            this.loggerService.Verify(x => x.Log(LogLevel.Information, It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(2));

            this.excelWriter.Setup(x => x.WriteAsync(It.IsAny<Dictionary<string, IReadOnlyList<ExportableObject>>>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException());

            await Assert.ThatAsync(() => this.exporterService.ExportElementsAsync("abcpath", genericConfigurations), Throws.InvalidOperationException);
        }
    }
}
