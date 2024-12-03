// -------------------------------------------------------------------------------------------------
// <copyright file="LoggerServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Services.Logger
{
    using EA;

    using EAModelKit.Services.Logger;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class LoggerServiceTestFixture
    {
        private LoggerService loggerService;
        private Mock<Repository> repository;
        private Mock<ILogger> logger;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<Repository>();
            this.logger = new Mock<ILogger>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(nameof(LoggerService))).Returns(this.logger.Object);
            this.loggerService = new LoggerService(loggerFactory.Object);
        }

        [Test]
        public void VerifyLog()
        {
            this.loggerService.Log(LogLevel.Debug, "something to log");
            this.repository.Verify(x => x.IsTabOpen(LoggerService.TabName), Times.Never);

            this.loggerService.InitializeService(this.repository.Object);

            this.loggerService.Log(LogLevel.Trace, "something to log");

            Assert.Multiple(() =>
            {
                this.repository.Verify(x => x.IsTabOpen(LoggerService.TabName), Times.Once);
                this.repository.Verify(x => x.CreateOutputTab(LoggerService.TabName), Times.Once);
                this.repository.Verify(x => x.EnsureOutputVisible(LoggerService.TabName), Times.Once);
                this.repository.Verify(x => x.WriteOutput(LoggerService.TabName, It.IsAny<string>(), 0), Times.Once);
            });

            this.repository.Setup(x => x.IsTabOpen(LoggerService.TabName)).Returns(1);

            this.loggerService.LogException(new InvalidOperationException(), "Invalid operation error message");

            Assert.Multiple(() =>
            {
                this.repository.Verify(x => x.IsTabOpen(LoggerService.TabName), Times.Exactly(2));
                this.repository.Verify(x => x.CreateOutputTab(LoggerService.TabName), Times.Once);
                this.repository.Verify(x => x.EnsureOutputVisible(LoggerService.TabName), Times.Exactly(2));
                this.repository.Verify(x => x.WriteOutput(LoggerService.TabName, It.IsAny<string>(), 0), Times.Exactly(2));
            });
        }
    }
}
