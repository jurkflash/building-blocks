using Xunit;

namespace Pokok.BuildingBlocks.Result.Tests
{
    public class ValidationResultTests
    {
        private static readonly Error Error1 = new("V.1", "First error");
        private static readonly Error Error2 = new("V.2", "Second error");
        private static readonly Error Error3 = new("V.3", "Third error");

        [Fact]
        public void Success_ShouldHaveNoErrors()
        {
            var result = ValidationResult.Success();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void WithErrors_ShouldCollectAllErrors()
        {
            var result = ValidationResult.WithErrors(Error1, Error2, Error3);

            Assert.True(result.IsFailure);
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains(Error1, result.Errors);
            Assert.Contains(Error2, result.Errors);
            Assert.Contains(Error3, result.Errors);
        }

        [Fact]
        public void WithErrors_FirstError_ShouldBeAccessibleViaErrorProperty()
        {
            var result = ValidationResult.WithErrors(Error1, Error2);

            Assert.Equal(Error1, result.Error);
        }

        [Fact]
        public void WithErrors_NoErrors_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() =>
                ValidationResult.WithErrors());
        }

        [Fact]
        public void GenericSuccess_ShouldHaveValueAndNoErrors()
        {
            var result = ValidationResult<int>.Success(42);

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void GenericWithErrors_ShouldCollectAllErrors()
        {
            var result = ValidationResult<int>.WithErrors(Error1, Error2);

            Assert.True(result.IsFailure);
            Assert.Equal(2, result.Errors.Count);
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void GenericWithErrors_NoErrors_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() =>
                ValidationResult<int>.WithErrors());
        }
    }
}
