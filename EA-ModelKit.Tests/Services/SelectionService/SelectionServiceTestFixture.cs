// -------------------------------------------------------------------------------------------------
//   <copyright file="SelectionServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.SelectionService
{
    using System.Xml.Linq;

    using EA;

    using EAModelKit.Services.Selection;
    using EAModelKit.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    using File = System.IO.File;

    [TestFixture]
    public class SelectionServiceTestFixture
    {
        private SelectionService selectionService;
        private Mock<Repository> repository;
        private Mock<EASelection> selection;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<Repository>();
            this.selectionService = new SelectionService();
            this.selection = new Mock<EASelection>();
            this.repository.Setup(x => x.CurrentSelection).Returns(this.selection.Object);
        }

        [Test]
        public void VerifyQuerySelectedElementsNoPackage()
        {
            this.selection.Setup(x => x.List).Returns(new TestCollection());
            this.selection.Setup(x => x.ElementSet).Returns(new TestCollection());

            Assert.That(this.selectionService.QuerySelectedElements(this.repository.Object), Is.Empty);

            // Contains 4 Elements and 3 Diagrams 
            var selectedElements = new List<object>
            {
                new Mock<Element>().Object,
                new Mock<Element>().Object,
                new Mock<Element>().Object,
                new Mock<Element>().Object,
                new Mock<Diagram>().Object,
                new Mock<Diagram>().Object,
                new Mock<Diagram>().Object
            };
            
            this.selection.Setup(x => x.ElementSet).Returns(new TestCollection(selectedElements));
            var selectedElementsResponse = this.selectionService.QuerySelectedElements(this.repository.Object);
            
            Assert.Multiple(() =>
            {
                Assert.That(selectedElementsResponse, Is.Not.Empty);
                Assert.That(selectedElementsResponse, Has.Count.EqualTo(4));
                Assert.That(selectedElementsResponse, Is.All.AssignableTo(typeof(Element)));
            });
        }

        [Test]
        public void VerifyQuerySelectedElements()
        {
            var rootPackage = new Mock<EAContext>();
            rootPackage.Setup(x => x.BaseType).Returns(nameof(Package));
            rootPackage.Setup(x => x.ElementID).Returns(1);

            this.selection.Setup(x => x.List).Returns(new TestCollection([rootPackage.Object]));
            var existingPackagesContent = QueryResourceContent("AllExistingPackages.xml");
            this.repository.Setup(x => x.SQLQuery(It.Is<string>(s => s.Contains(" from t_package ")))).Returns(existingPackagesContent);

            var emptyElementsContent = QueryResourceContent("EmptyElements.xml");
            this.repository.Setup(x => x.SQLQuery(It.Is<string>(s => s.Contains(" from t_object ")))).Returns(emptyElementsContent);
            
            Assert.That(this.selectionService.QuerySelectedElements(this.repository.Object), Is.Empty);
            
            // Contains 9 ids
            var existingElementsContent = QueryResourceContent("AllSelectedElements.xml");
            this.repository.Setup(x => x.SQLQuery(It.Is<string>(s => s.Contains(" from t_object ")))).Returns(existingElementsContent);

            var existingElements = new List<Element>();
            var xElement = XElement.Parse(existingElementsContent);
            
            for (var existingElementsIndex = 0; existingElementsIndex < xElement.Descendants("Row").Count(); existingElementsIndex++)
            {
                existingElements.Add(new Mock<Element>().Object);
            }
            
            this.repository.Setup(x => x.GetElementSet(It.IsAny<string>(),0)).Returns(new TestCollection(existingElements));
    
            var selectedElementsResponse = this.selectionService.QuerySelectedElements(this.repository.Object);
            
            Assert.Multiple(() =>
            {
                Assert.That(selectedElementsResponse, Is.Not.Empty);
                Assert.That(selectedElementsResponse, Has.Count.EqualTo(9));
                Assert.That(selectedElementsResponse, Is.All.AssignableTo(typeof(Element)));
            });
        }

        private static string QueryResourceContent(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SelectionService",fileName);
            return File.ReadAllText(path);
        }
    }
}
