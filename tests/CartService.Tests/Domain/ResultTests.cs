using CartService.Application.Common;

namespace CartService.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void GenericResult_Ok_SetsIsSuccessAndValue()
    {
        var result = Result<string>.Ok("hello");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
        result.Error.Should().BeNull();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void GenericResult_Fail_DefaultStatusCode_Is400()
    {
        var result = Result<string>.Fail("something went wrong");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("something went wrong");
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GenericResult_Fail_WithCustomStatusCode()
    {
        var result = Result<string>.Fail("not found", 404);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public void Result_Ok_SetsIsSuccess()
    {
        var result = Result.Ok();

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void Result_Fail_DefaultStatusCode_Is400()
    {
        var result = Result.Fail("error");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("error");
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public void Result_Fail_WithCustomStatusCode()
    {
        var result = Result.Fail("unauthorized", 401);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Error.Should().Be("unauthorized");
    }
}
