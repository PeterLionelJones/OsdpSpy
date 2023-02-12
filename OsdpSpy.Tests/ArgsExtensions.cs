using NUnit.Framework;


namespace OsdpSpy.Tests;

public class ArgsExtensions
{
    [Test]
    [TestCase(new string[0], null)]
    [TestCase(new string[] { "listen" }, null)]
    [TestCase(new string[] { "listen", "-s" }, null)]
    [TestCase(new string[] { "listen", "--seq" }, null)]
    [TestCase(new string[] { "listen", "-s", "https://seq.com" }, "https://seq.com")]
    [TestCase(new string[] { "listen", "--seq", "https://seq.com" }, "https://seq.com")]
    [TestCase(new string[] { "listen", "-e", "https://elasticsearch.com" }, null)]
    [TestCase(new string[] { "listen", "--elasticsearch", "https://elasticsearch.com" }, null)]
    [TestCase(new string[] { "import", "-s", "https://seq.com" }, null)]
    [TestCase(new string[] { "import", "--seq", "https://seq.com" }, null)]
    [TestCase(new string[] { "LISTEN", "-s", "https://seq.com" }, "https://seq.com")]
    [TestCase(new string[] { "LISTEN", "--seq", "https://seq.com" }, "https://seq.com")]
    [TestCase(new string[] { "IMPORT", "-s", "https://seq.com" }, null)]
    [TestCase(new string[] { "IMPORT", "--seq", "https://seq.com" }, null)]
    public void SeqUrl_ExtractsUrl_ReturnsExpected(string[] input, string expectedOutput)
    {
        var output = input.SeqUrl();
        Assert.That(output == expectedOutput);
    }
    
    [Test]
    [TestCase(new string[0], null)]
    [TestCase(new string[] { "listen" }, null)]
    [TestCase(new string[] { "listen", "-e" }, null)]
    [TestCase(new string[] { "listen", "--elasticsearch" }, null)]
    [TestCase(new string[] { "listen", "-e", "https://elasticsearch.com" }, "https://elasticsearch.com")]
    [TestCase(new string[] { "listen", "--elasticsearch", "https://elasticsearch.com" }, "https://elasticsearch.com")]
    [TestCase(new string[] { "listen", "-s", "https://seq.com" }, null)]
    [TestCase(new string[] { "listen", "--seq", "https://seq.com" }, null)]
    [TestCase(new string[] { "import", "-e", "https://elasticsearch.com" }, null)]
    [TestCase(new string[] { "import", "--elasticsearch", "https://elasticsearch.com" }, null)]
    [TestCase(new string[] { "LISTEN", "--elasticsearch" }, null)]
    [TestCase(new string[] { "LISTEN", "-e", "https://elasticsearch.com" }, "https://elasticsearch.com")]
    [TestCase(new string[] { "LISTEN", "--elasticsearch", "https://elasticsearch.com" }, "https://elasticsearch.com")]
    [TestCase(new string[] { "IMPORT", "-e", "https://elasticsearch.com" }, null)]
    [TestCase(new string[] { "IMPORT", "--elasticsearch", "https://elasticsearch.com" }, null)]
    public void ElasticsearchUrl_ExtractsUrl_ReturnsExpected(string[] input, string expectedOutput)
    {
        var output = input.ElasticsearchUrl();
        Assert.That(output == expectedOutput);
    }
}