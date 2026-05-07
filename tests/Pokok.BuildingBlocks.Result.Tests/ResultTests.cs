using Xunit;
using R = Pokok.BuildingBlocks.Result.Result;

namespace Pokok.BuildingBlocks.Result.Tests
{
    public class ResultTests
    {
        private static readonly Error TestError = new("Test.Error", "A test error.");

        [Fact]
        public void Success_ShouldCreateSuccessfulResult()
        {
            var result = R.Success();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void Failure_ShouldCreateFailedResult()
        {
            var result = R.Failure(TestError);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void Success_WithError_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() =>
                R.Failure(Error.None));
        }

        [Fact]
        public void GenericSuccess_ShouldCreateResultWithValue()
        {
            var result = R.Success(42);

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void GenericFailure_ShouldCreateFailedResult()
        {
            var result = R.Failure<int>(TestError);

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void Match_OnSuccess_ShouldExecuteOnSuccess()
        {
            var executed = false;
            var result = R.Success();

            result.Match(
                onSuccess: () => executed = true,
                onFailure: _ => { });

            Assert.True(executed);
        }

        [Fact]
        public void Match_OnFailure_ShouldExecuteOnFailure()
        {
            Error? captured = null;
            var result = R.Failure(TestError);

            result.Match(
                onSuccess: () => { },
                onFailure: e => captured = e);

            Assert.Equal(TestError, captured);
        }

        [Fact]
        public void Bind_OnSuccess_ShouldChain()
        {
            var result = R.Success()
                .Bind(() => R.Success());

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Bind_OnFailure_ShouldPropagateError()
        {
            var result = R.Failure(TestError)
                .Bind(() => R.Success());

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void BindGeneric_OnSuccess_ShouldChain()
        {
            var result = R.Success()
                .Bind(() => R.Success(42));

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void BindGeneric_OnFailure_ShouldPropagateError()
        {
            var result = R.Failure(TestError)
                .Bind(() => R.Success(42));

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void OnSuccess_ShouldExecuteOnlyWhenSuccessful()
        {
            var count = 0;

            R.Success().OnSuccess(() => count++);
            R.Failure(TestError).OnSuccess(() => count++);

            Assert.Equal(1, count);
        }

        [Fact]
        public void OnFailure_ShouldExecuteOnlyWhenFailed()
        {
            var count = 0;

            R.Success().OnFailure(_ => count++);
            R.Failure(TestError).OnFailure(_ => count++);

            Assert.Equal(1, count);
        }
    }
}
