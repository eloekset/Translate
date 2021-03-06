﻿using Moq;
using Translate.CrossCutting;
using Translate.Domain.Contracts;
using Xunit;
using Should;
using Translate.Domain.Entities;

namespace Translate.Business.Tests
{
    public class TranslationServiceTest : TestsFor<TranslationService>
    {
        private IExceptionHandler _exceptionHandler;

        public TranslationServiceTest()
        {
        }


        public override void OverrideMocks()
        {
            var mockedLogger = GetMockFor<ILogger>().Object;
            _exceptionHandler = new ExceptionHandler(mockedLogger);
            Inject(_exceptionHandler);
        }


        [Fact]
        public void SupportedLanguages_WhenCalled_AquiresLanguages()
        {
            // Act
            var result = Instance.SupportedLanguages;

            // Assert
            GetMockFor<ITranslationClient>().Verify(o => o.GetLanguageNames(), Times.Once());                
        }


        [Fact]
        public void SupportedLanguages_WhenCalledTwice_AquiresLanguagesOnlyOnce()
        {
            // Act
            var result = Instance.SupportedLanguages;
            var result2 = Instance.SupportedLanguages;

            // Assert
            GetMockFor<ITranslationClient>().Verify(o => o.GetLanguageNames(), Times.Once());
        }


        [Fact]
        public void TranslateSingle_FromIsEmpty_ResultIsEmptyString()
        {
            // Act
            var result = Instance.TranslateSingle(null, "en", "yes we have no bananas");

            // Assert
            result.ShouldBeEmpty();
        }


        [Fact]
        public void TranslateSingle_ToIsEmpty_ResultIsEmptyString()
        {
            // Act
            var result = Instance.TranslateSingle("en", null, "yes we have no bananas");

            // Assert
            result.ShouldBeEmpty();
        }


        [Fact]
        public void TranslateSingle_TextIsEmpty_ResultIsEmptyString()
        {
            // Act
            var result = Instance.TranslateSingle("en", "no", null);

            // Assert
            result.ShouldBeEmpty();
        }


        [Fact]
        public void TranslateSingle_FromIsInvalidLanguage_ResultIsEmptyString()
        {
            // Arrange
            SupportEnglishAndNorwegian();
            var invalidCode = "Invalid";

            // Act
            var result = Instance.TranslateSingle(invalidCode, "en", "yes we have no bananas");

            // Assert
            result.ShouldBeEmpty();
        }


        [Fact]
        public void TranslateSingle_FromIsInvalidLanguage_LoggerIsInvoked()
        {
            // Arrange
            SupportEnglishAndNorwegian();
            var invalidCode = "Invalid";

            // Act
            var result = Instance.TranslateSingle(invalidCode, "en", "yes we have no bananas");

            // Assert
            GetMockFor<ILogger>().Verify(l => l.LogError(It.IsAny<string>()), Times.Once());
        }


        [Fact]
        public void TranslateSingle_ToIsInvalidLanguage_LoggerIsInvoked()
        {
            // Arrange
            SupportEnglishAndNorwegian();
            var invalidCode = "Invalid";

            // Act
            var result = Instance.TranslateSingle("en", invalidCode, "yes we have no bananas");

            // Assert
            GetMockFor<ILogger>().Verify(l => l.LogError(It.IsAny<string>()), Times.Once());
        }


        [Fact]
        public void TranslateSingle_ToIsInvalidLanguage_ResultIsEmptyString()
        {
            // Arrange
            SupportEnglishAndNorwegian();
            var invalidCode = "Invalid";

            // Act
            var result = Instance.TranslateSingle("en", invalidCode, "yes we have no bananas");

            // Assert
            result.ShouldBeEmpty();
        }


        [Fact]
        public void TranslateSingle_AllParamatersValid_ResultReturnedFromTranslationClient()
        {
            // Arrange
            SupportEnglishAndNorwegian();
            var from = "en";
            var to = "no";
            var text = "yes we have no bananas";
            var translated = "ja vi mangler bananer";
            GetMockFor<ITranslationClient>().Setup(c => c.TranslateSingle(from, to, text)).Returns(translated);

            // Act
            var result = Instance.TranslateSingle(from, to, text);

            // Assert
            result.ShouldEqual(translated);
        }


        private void SupportEnglishAndNorwegian()
        {
            var supportedLanguages = new Language[] { new Language { Code = "no"}, new Language { Code = "en"} };

            GetMockFor<ITranslationClient>().Setup(o => o.GetLanguageNames()).Returns(supportedLanguages);
        }


    }
}
