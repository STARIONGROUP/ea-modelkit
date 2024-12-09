// -----------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverterTestFixture.cs" company="Starion Nederland B.V.">
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
    using System.Windows;

    using EAModelKit.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class BooleanToVisibilityConverterTestFixture
    {
        private BooleanToVisibilityConverter converter;

        [SetUp]
        public void Setup()
        {
            this.converter = new BooleanToVisibilityConverter();
        }

        [Test]
        public void VerifyConvert()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.converter.Convert(null, typeof(Visibility), "", CultureInfo.InvariantCulture), Is.EqualTo(Visibility.Collapsed));
                Assert.That(this.converter.Convert(true, typeof(Visibility), "", CultureInfo.InvariantCulture), Is.EqualTo(Visibility.Visible));
                Assert.That(this.converter.Convert(false, typeof(Visibility), "", CultureInfo.InvariantCulture), Is.EqualTo(Visibility.Collapsed));
                Assert.That(this.converter.Convert(false, typeof(Visibility), "Invert", CultureInfo.InvariantCulture), Is.EqualTo(Visibility.Visible));
                Assert.That(this.converter.Convert(true, typeof(Visibility), "Invert", CultureInfo.InvariantCulture), Is.EqualTo(Visibility.Collapsed));
                Assert.That(() => this.converter.ConvertBack(Visibility.Collapsed, typeof(bool), "", CultureInfo.InvariantCulture), Throws.Exception);
            });
        }
    }
}
