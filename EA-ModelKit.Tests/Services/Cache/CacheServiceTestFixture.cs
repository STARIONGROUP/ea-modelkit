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

    using Moq;

    using NUnit.Framework;
    
    using File = System.IO.File;

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
        }

        [Test]
        public void VerifyGetTaggedValues()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.cacheService.GetTaggedValues(10).Count, Is.EqualTo(1));
                Assert.That(this.cacheService.GetTaggedValues(20).Count, Is.EqualTo(2));
                Assert.That(this.cacheService.GetTaggedValues(26).Count, Is.EqualTo(1));
                Assert.That(this.cacheService.GetTaggedValues(27).Count, Is.EqualTo(0));
                Assert.That(this.cacheService.GetTaggedValues([10,20,26]).Count, Is.EqualTo(4));
            });
        }

        private static string QueryResourceContent(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CacheService", fileName);
            return File.ReadAllText(path);
        }
    }
}
