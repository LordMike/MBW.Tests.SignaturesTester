using MBW.Tests.SignaturesTester;
using System;
using System.Linq;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class SignaturesBuilderTests
{
    [Fact]
    public void BasicTest()
    {
        var builder = SignaturesBuilder.Types<TestType>()
            .MustHave("instance Int32 Add(Int32, Int32)");

        Assert.Equal(
            new[] { "instance Int32 Add(Int32, Int32)" },
            builder.GetDesiredSignatures().Select(s => s.signature));
    }

    [Fact]
    public void TSelfTest()
    {
        var builder = SignaturesBuilder.Types<TestType>()
            .MustHave("instance TSelf Add(Int32, Int32)");

        Assert.Equal(
            new[] { "instance TSelf Add(Int32, Int32)" },
            builder.GetDesiredSignatures().Select(s => s.signature));
    }

    [Fact]
    public void SingleTypeArgument()
    {
        var builder = SignaturesBuilder.Types<TestType>()
            .MustHave("instance Int32 Add(T1, T1)", _ => typeof(long));

        Assert.Equal(
            new[] { "instance Int32 Add(Int64, Int64)" },
            builder.GetDesiredSignatures().Select(s => s.signature));
    }

    [Fact]
    public void MultipleTypeArguments()
    {
        var builder = SignaturesBuilder.Types<TestType>()
            .MustHave("instance Int32 Add(T1, T2)", _ => typeof(int), _ => typeof(long));

        Assert.Equal(
            new[] { "instance Int32 Add(Int32, Int64)" },
            builder.GetDesiredSignatures().Select(s => s.signature));
    }

    [Fact]
    public void MultipleTypeSetsArguments()
    {
        var builder = SignaturesBuilder.Types<TestType>()
            .MustHave("instance Int32 Add(T1, T2)", _ => new[] { typeof(int) , typeof(string)}, _ => new[] { typeof(long), typeof(bool) });

        Assert.Equal(
            new[] {
                "instance Int32 Add(Int32, String)",
                "instance Int32 Add(Int64, Boolean)"
            },
            builder.GetDesiredSignatures().Select(s => s.signature));
    }

    class TestType
    {
        public int Add(int a, int b) => throw new NotImplementedException();
    }
}