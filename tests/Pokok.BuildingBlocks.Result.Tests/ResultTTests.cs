using Xunit;

namespace Pokok.BuildingBlocks.Result.Tests
{
    public class ResultTTests
    {
        private static readonly Error TestError = new("Test.Error", "A test error.");

        [Fact]
        public void Success_ShouldCreateResultWithValue()
        {
            var result = Result<string>.Success("hello");

            Assert.True(result.IsSuccess);
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public void Failure_ShouldPreventValueAccess()
        {
            var result = Result<string>.Failure(TestError);

            Assert.True(result.IsFailure);
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void ImplicitConversion_FromValue_ShouldCreateSuccess()
        {
            Result<int> result = 42;

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void ImplicitConversion_FromError_ShouldCreateFailure()
        {
            Result<int> result = TestError;

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void Match_OnSuccess_ShouldReturnMappedValue()
        {
            var result = Result<int>.Success(5);

            var output = result.Match(
                onSuccess: v => v * 2,
                onFailure: _ => -1);

            Assert.Equal(10, output);
        }

        [Fact]
        public void Match_OnFailure_ShouldReturnErrorValue()
        {
            var result = Result<int>.Failure(TestError);

            var output = result.Match(
                onSuccess: v => v * 2,
                onFailure: _ => -1);

            Assert.Equal(-1, output);
        }

        [Fact]
        public void Map_OnSuccess_ShouldTransformValue()
        {
            var result = Result<int>.Success(3)
                .Map(v => v.ToString());

            Assert.True(result.IsSuccess);
            Assert.Equal("3", result.Value);
        }

        [Fact]
        public void Map_OnFailure_ShouldPropagateError()
        {
            var result = Result<int>.Failure(TestError)
                .Map(v => v.ToString());

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void Bind_OnSuccess_ShouldChainResults()
        {
            var result = Result<int>.Success(5)
                .Bind(v => Result<string>.Success($"Value: {v}"));

            Assert.True(result.IsSuccess);
            Assert.Equal("Value: 5", result.Value);
        }

        [Fact]
        public void Bind_OnFailure_ShouldPropagateError()
        {
            var result = Result<int>.Failure(TestError)
                .Bind(v => Result<string>.Success($"Value: {v}"));

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void OnSuccess_ShouldExecuteOnlyWhenSuccessful()
        {
            var captured = 0;

            Result<int>.Success(7).OnSuccess(v => captured = v);
            Result<int>.Failure(TestError).OnSuccess(v => captured = -1);

            Assert.Equal(7, captured);
        }

        [Fact]
        public void OnFailure_ShouldExecuteOnlyWhenFailed()
        {
            Error? captured = null;

            Result<int>.Success(7).OnFailure(e => captured = e);
            Assert.Null(captured);

            Result<int>.Failure(TestError).OnFailure(e => captured = e);
            Assert.Equal(TestError, captured);
        }

        [Fact]
        public void ChainedOperations_ShouldWorkEndToEnd()
        {
            var result = Result<int>.Success(10)
                .Map(v => v * 2)
                .Bind(v => v > 15
                    ? Result<string>.Success($"OK: {v}")
                    : Result<string>.Failure(Error.Validation("TooSmall", "Value too small")));

            Assert.True(result.IsSuccess);
            Assert.Equal("OK: 20", result.Value);
        }

        [Fact]
        public void ChainedOperations_ShouldShortCircuitOnFailure()
        {
            var result = Result<int>.Failure(TestError)
                .Map(v => v * 2)
                .Bind(v => Result<string>.Success($"OK: {v}"));

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }
    }
}
