// -----------------------------------------------------------------------------------
// <copyright file="FileImportExportCollectionConverterTestFixture.cs" company="Starion Nederland B.V.">
//    Copyright (c) 2024 Starion Nederland B.V.
// 
//    Authors: Alex Vorobiev, Antoine Théate, Sam Gerené, Anh-Toan Bui Long
// 
//    This file is part of ESA SysML plugin for Enterprise Architect
//    European Space Agency Community License – v2.4 Permissive (Type 3)
//    See LICENSE file for details
// 
// </copyright>
// -----------------------------------------------------------------------------------

namespace EAModelKit.Tests.Converters
{
    using System.Globalization;

    using EAModelKit.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class StringCollectionConverterTestFixture
    {
        private StringCollectionConverter converter;

        [SetUp]
        public void Setup()
        {
            this.converter = new StringCollectionConverter();
        }

        [Test]
        public void VerifyConvert()
        {
            Assert.That(this.converter.Convert(null, typeof(IEnumerable<string>), null, CultureInfo.InvariantCulture), Is.EquivalentTo(new List<string>()));
        }

        [Test]
        public void VerifyConvertBack()
        {
            Assert.That(this.converter.ConvertBack(null, typeof(IEnumerable<string>), null, CultureInfo.InvariantCulture), Is.Empty);
        }
    }
}
