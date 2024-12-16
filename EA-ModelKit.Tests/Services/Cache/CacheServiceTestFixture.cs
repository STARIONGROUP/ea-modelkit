// -------------------------------------------------------------------------------------------------
// <copyright file="CacheServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Cache
{
    using EA;

    using EAModelKit.Services.Cache;
    using EAModelKit.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    using File = File;

    [TestFixture]
    public class CacheServiceTestFixture
    {
        private CacheService cacheService;
        private Mock<Repository> repository;

        [SetUp]
        public void Setup()
        {
            this.cacheService = new CacheService();
            this.repository = new Mock<Repository>();
            this.cacheService.Initialize(this.repository.Object);

            this.repository.Setup(x => x.SQLQuery(It.Is<string>(i => i.Contains("t_objectproperties"))))
                .Returns(QueryResourceContent("TaggedValues.xml"));

            this.repository.Setup(x => x.SQLQuery(It.Is<string>(i => i.Contains(" Start_Object_ID=5"))))
                .Returns(QueryResourceContent("Connectors.xml"));
            
            this.repository.Setup(x => x.SQLQuery(It.Is<string>(i => i.Contains(" Start_Object_ID=15"))))
                .Returns(QueryResourceContent("Connectors.xml"));

            this.repository.Setup(x => x.SQLQuery(It.Is<string>(i => i.Contains(" Start_Object_ID=6"))))
                .Returns(QueryResourceContent("EmptyConnectors.xml"));

            var connector = new Mock<Connector>();
            connector.Setup(x => x.ConnectorID).Returns(20);
            connector.Setup(x => x.ClientID).Returns(5);
            connector.Setup(x => x.SupplierID).Returns(15);
            connector.Setup(x => x.Type).Returns("Association");
            this.repository.Setup(x => x.GetConnectorByID(20)).Returns(connector.Object);

            var elements = new List<Element>
            {
                CreateNewElement("Requirement", "", "REQ-01", 5),
                CreateNewElement("Block", "Function", "FUNC-01", 15)
            };

            this.repository.Setup(x => x.GetElementSet("5,15", 0)).Returns(new TestCollection(elements));
        }

        [Test]
        public void VerifyGetTaggedValues()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.cacheService.GetTaggedValues(10), Has.Count.EqualTo(1));
                Assert.That(this.cacheService.GetTaggedValues(20), Has.Count.EqualTo(2));
                Assert.That(this.cacheService.GetTaggedValues(26), Has.Count.EqualTo(1));
                Assert.That(this.cacheService.GetTaggedValues(27), Has.Count.EqualTo(0));
                Assert.That(this.cacheService.GetTaggedValues([10, 20, 26]), Has.Count.EqualTo(4));
            });
        }
        
        [Test]
        public void VerifyGetConnectors()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.cacheService.GetAssociatedConnectors(5),Has.Count.EqualTo(1));
                Assert.That(this.cacheService.GetAssociatedConnectors(15),Has.Count.EqualTo(1));
                Assert.That(this.cacheService.GetAssociatedConnectors(6),Has.Count.EqualTo(0));
            });
        }

        private static Element CreateNewElement(string type, string stereotype, string name, int elementId)
        {
            var element = new Mock<Element>();
            element.Setup(x => x.ElementID).Returns(elementId);
            element.Setup(x => x.Type).Returns(type);
            element.Setup(x => x.Stereotype).Returns(stereotype);
            element.Setup(x => x.Name).Returns(name);
            return element.Object;
        }

        private static string QueryResourceContent(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CacheService", fileName);
            return File.ReadAllText(path);
        }
    }
}
