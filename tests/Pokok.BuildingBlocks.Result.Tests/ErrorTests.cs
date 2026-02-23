using Xunit;

namespace Pokok.BuildingBlocks.Result.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void None_ShouldHaveEmptyCodeAndDescription()
        {
            var none = Error.None;

            Assert.Equal(string.Empty, none.Code);
            Assert.Equal(string.Empty, none.Description);
        }

        [Fact]
        public void NotFound_ShouldCreateError()
        {
            var error = Error.NotFound("User.NotFound", "User was not found.");

            Assert.Equal("User.NotFound", error.Code);
            Assert.Equal("User was not found.", error.Description);
        }

        [Fact]
        public void Validation_ShouldCreateError()
        {
            var error = Error.Validation("Field.Invalid", "Field is invalid.");

            Assert.Equal("Field.Invalid", error.Code);
            Assert.Equal("Field is invalid.", error.Description);
        }

        [Fact]
        public void Conflict_ShouldCreateError()
        {
            var error = Error.Conflict("Email.Taken", "Email already in use.");

            Assert.Equal("Email.Taken", error.Code);
            Assert.Equal("Email already in use.", error.Description);
        }

        [Fact]
        public void Failure_ShouldCreateError()
        {
            var error = Error.Failure("General.Error", "Something went wrong.");

            Assert.Equal("General.Error", error.Code);
            Assert.Equal("Something went wrong.", error.Description);
        }

        [Fact]
        public void ToString_ShouldReturnCodeAndDescription()
        {
            var error = new Error("Code", "Desc");

            Assert.Equal("Code: Desc", error.ToString());
        }

        [Fact]
        public void Equality_ShouldWorkByValue()
        {
            var a = new Error("Code", "Desc");
            var b = new Error("Code", "Desc");
            var c = new Error("Other", "Desc");

            Assert.Equal(a, b);
            Assert.NotEqual(a, c);
        }
    }
}
