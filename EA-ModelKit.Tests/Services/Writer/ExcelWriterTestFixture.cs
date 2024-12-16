// -------------------------------------------------------------------------------------------------
// <copyright file="ExcelWriterTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Writer
{
    using EAModelKit.Model.Export;
    using EAModelKit.Services.Writer;
    using EAModelKit.Tests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ExcelWriterTestFixture
    {
        private ExcelWriter excelWriter;
        private string exportFilePath;

        [SetUp]
        public void Setup()
        {
            this.excelWriter = new ExcelWriter();
            this.exportFilePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))!, "ExportTestToExcel.xlsx");
        }
        
        [TearDown]
        public void Teardown()
        {
            if (File.Exists(this.exportFilePath))
            {
                File.Delete(this.exportFilePath);
            }
        }

        [Test]
        public void VerifyWrite()
        {
            const string kind = "TestElement";
            var element = new ExportableElement(new TestSlimElement(kind, "name", "alias", "a note", [], []), [], []);
            
            var content = new Dictionary<string, IReadOnlyList<ExportableObject>>
            {
                {kind, [element]}
            };
            
            Assert.That(() => this.excelWriter.WriteAsync(content, this.exportFilePath), Throws.Nothing);
        }
    }
}
