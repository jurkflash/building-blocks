using Xunit;
using R = Pokok.BuildingBlocks.Result.Result;

namespace Pokok.BuildingBlocks.Result.Tests
{
    public class ResultExtensionsTests
    {
        private static readonly Error TestError = new("Test.Error", "A test error.");
        private static readonly Error OtherError = new("Other.Error", "Another error.");

        [Fact]
        public async Task MapAsync_OnSuccess_ShouldTransformValue()
        {
            var task = Task.FromResult(Result<int>.Success(5) as Result<int>);

            var result = await task.Map(v => v * 3);

            Assert.True(result.IsSuccess);
            Assert.Equal(15, result.Value);
        }

        [Fact]
        public async Task MapAsync_OnFailure_ShouldPropagateError()
        {
            var task = Task.FromResult(Result<int>.Failure(TestError) as Result<int>);

            var result = await task.Map(v => v * 3);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task BindAsync_OnSuccess_ShouldChain()
        {
            var task = Task.FromResult(Result<int>.Success(5) as Result<int>);

            var result = await task.Bind(v => Result<string>.Success($"Value: {v}"));

            Assert.True(result.IsSuccess);
            Assert.Equal("Value: 5", result.Value);
        }

        [Fact]
        public async Task BindAsync_OnFailure_ShouldPropagateError()
        {
            var task = Task.FromResult(Result<int>.Failure(TestError) as Result<int>);

            var result = await task.Bind(v => Result<string>.Success($"Value: {v}"));

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task MatchAsync_OnSuccess_ShouldReturnMappedValue()
        {
            var task = Task.FromResult(Result<int>.Success(5) as Result<int>);

            var output = await task.Match(v => v * 2, _ => -1);

            Assert.Equal(10, output);
        }

        [Fact]
        public async Task MatchAsync_OnFailure_ShouldReturnErrorValue()
        {
            var task = Task.FromResult(Result<int>.Failure(TestError) as Result<int>);

            var output = await task.Match(v => v * 2, _ => -1);

            Assert.Equal(-1, output);
        }

        [Fact]
        public async Task MatchResultAsync_OnSuccess_ShouldExecuteOnSuccess()
        {
            var executed = false;
            var task = Task.FromResult(R.Success());

            await task.Match(
                onSuccess: () => executed = true,
                onFailure: _ => { });

            Assert.True(executed);
        }

        [Fact]
        public void Combine_AllSuccess_ShouldReturnSuccess()
        {
            var result = ResultExtensions.Combine(
                R.Success(),
                R.Success(),
                R.Success());

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Combine_AnyFailure_ShouldReturnFirstFailure()
        {
            var result = ResultExtensions.Combine(
                R.Success(),
                R.Failure(TestError),
                R.Failure(OtherError));

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void CombineAsValidation_AllSuccess_ShouldReturnSuccess()
        {
            var result = ResultExtensions.CombineAsValidation(
                R.Success(),
                R.Success());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CombineAsValidation_MultipleFailures_ShouldCollectAllErrors()
        {
            var result = ResultExtensions.CombineAsValidation(
                R.Failure(TestError),
                R.Success(),
                R.Failure(OtherError));

            Assert.True(result.IsFailure);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains(TestError, result.Errors);
            Assert.Contains(OtherError, result.Errors);
        }

        [Fact]
        public void ToResult_NonNull_ShouldReturnSuccess()
        {
            string? value = "hello";
            var result = value.ToResult(TestError);

            Assert.True(result.IsSuccess);
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public void ToResult_Null_ShouldReturnFailure()
        {
            string? value = null;
            var result = value.ToResult(TestError);

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }

        [Fact]
        public void ToResult_ValueType_NonNull_ShouldReturnSuccess()
        {
            int? value = 42;
            var result = value.ToResult(TestError);

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void ToResult_ValueType_Null_ShouldReturnFailure()
        {
            int? value = null;
            var result = value.ToResult(TestError);

            Assert.True(result.IsFailure);
            Assert.Equal(TestError, result.Error);
        }
    }
}
