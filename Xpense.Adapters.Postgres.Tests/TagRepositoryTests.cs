using System.Collections.Immutable;
using Dapper;
using FluentAssertions;
using Xpense.Adapters.Postgres.Models;
using Xpense.Adapters.Postgres.Persistence;
using Xpense.Adapters.Postgres.Repositories;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests;

public class TagRepositoryTests(ITestOutputHelper outputHelper) : IntegrationTestSuite(outputHelper)
{
    private ITagRepository _repository = null!;

    protected override void Construct()
    {
        _repository = new TagRepository(Connection);
    }

    protected override async Task TruncateTable()
    {
        await Connection.ExecuteAsync("truncate table xpense.tag cascade;");
    }

    [Fact]
    public async Task CreateRange_WhenInvokedWithMultipleLabels_ShouldInsertMultipleTags()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act & Assert
        var createdTags = await _repository.CreateRange(tags);

        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();
        createdTags.Data.Count.Should().Be(3);

        createdTags.Data[0].Label.Should().Be("Tag1");
        createdTags.Data[0].BgColorHex.Should().Be("aaaaaa");
        createdTags.Data[0].FgColorHex.Should().Be("000000");

        createdTags.Data[1].Label.Should().Be("Tag2");
        createdTags.Data[1].BgColorHex.Should().Be("bbbbbb");
        createdTags.Data[1].FgColorHex.Should().Be("333333");

        createdTags.Data[2].Label.Should().Be("Tag3");
        createdTags.Data[2].BgColorHex.Should().Be("cccccc");
        createdTags.Data[2].FgColorHex.Should().Be("888888");
    }

    [Fact]
    public async Task CreateRange_WhenInvokedWithPartiallyExistingLabels_ShouldInsertDifferentTagsOnly()
    {
        // Arrange
        var tagsBatch1 = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        var tagsBatch2 = new List<Tag>
        {
            new() { Label = "Tag2", BgColorHex = "331144", FgColorHex = "331144" }, // exists already
            new() { Label = "Tag3", BgColorHex = "556677", FgColorHex = "556677" }, // exists already
            new() { Label = "Tag4", BgColorHex = "cccccc", FgColorHex = "888888" },
            new() { Label = "Tag5", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act & Assert
        _ = await _repository.CreateRange(tagsBatch1);
        var createdTags = await _repository.CreateRange(tagsBatch2);

        true.Should().Be(true);

        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();
        createdTags.Data.Count.Should().Be(4);

        createdTags.Data[0].Label.Should().Be("Tag2");
        createdTags.Data[0].BgColorHex.Should().Be("331144");
        createdTags.Data[0].FgColorHex.Should().Be("331144");
        createdTags.Data[0].LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(200));

        createdTags.Data[1].Label.Should().Be("Tag3");
        createdTags.Data[1].BgColorHex.Should().Be("556677");
        createdTags.Data[1].FgColorHex.Should().Be("556677");
        createdTags.Data[1].LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(200));

        createdTags.Data[2].Label.Should().Be("Tag4");
        createdTags.Data[2].BgColorHex.Should().Be("cccccc");
        createdTags.Data[2].FgColorHex.Should().Be("888888");
        createdTags.Data[2].LastModified.Should().BeNull();

        createdTags.Data[3].Label.Should().Be("Tag5");
        createdTags.Data[3].BgColorHex.Should().Be("cccccc");
        createdTags.Data[3].FgColorHex.Should().Be("888888");
        createdTags.Data[3].LastModified.Should().BeNull();
    }

    [Fact]
    public async Task CreateRange_WhenInvokedWithEmptyTagList_ShouldReturnSuccessfullOperation()
    {
        // Arrange
        var tagsBatch1 = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        var tagsBatch2 = new List<Tag>
        {
            new() { Label = "Tag2", BgColorHex = "331144", FgColorHex = "331144" }, // exists already
            new() { Label = "Tag3", BgColorHex = "556677", FgColorHex = "556677" }, // exists already
            new() { Label = "Tag4", BgColorHex = "cccccc", FgColorHex = "888888" },
            new() { Label = "Tag5", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act & Assert
        _ = await _repository.CreateRange(tagsBatch1);
        var createdTags = await _repository.CreateRange(tagsBatch2);

        true.Should().Be(true);

        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();
        createdTags.Data.Count.Should().Be(4);

        createdTags.Data[0].Label.Should().Be("Tag2");
        createdTags.Data[0].BgColorHex.Should().Be("331144");
        createdTags.Data[0].FgColorHex.Should().Be("331144");
        createdTags.Data[0].LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(200));

        createdTags.Data[1].Label.Should().Be("Tag3");
        createdTags.Data[1].BgColorHex.Should().Be("556677");
        createdTags.Data[1].FgColorHex.Should().Be("556677");
        createdTags.Data[1].LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(200));

        createdTags.Data[2].Label.Should().Be("Tag4");
        createdTags.Data[2].BgColorHex.Should().Be("cccccc");
        createdTags.Data[2].FgColorHex.Should().Be("888888");
        createdTags.Data[2].LastModified.Should().BeNull();

        createdTags.Data[3].Label.Should().Be("Tag5");
        createdTags.Data[3].BgColorHex.Should().Be("cccccc");
        createdTags.Data[3].FgColorHex.Should().Be("888888");
        createdTags.Data[3].LastModified.Should().BeNull();
    }

    [Fact]
    public async Task GetByLabel_WhenInvokedForExistingTag_ShouldReturnTag()
    {
        // Arrange
        var tagsBatch = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act && Assert
        var createdTags = await _repository.CreateRange(tagsBatch);
        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();

        var tag = await _repository.GetByLabel("Tag2");
        tag.Status.Should().Be(StorageResultStatus.Success);
        tag.Data.Should().NotBeNull();
        tag.Data.Label.Should().Be("Tag2");
        tag.Data.BgColorHex.Should().Be("bbbbbb");
        tag.Data.FgColorHex.Should().Be("333333");
        tag.Data.LastModified.Should().BeNull();
    }

    [Fact]
    public async Task GetByLabel_WhenInvokedForNonExistingTag_ShouldReturnNotFound()
    {
        // Arrange
        var tagsBatch = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act && Assert
        var createdTags = await _repository.CreateRange(tagsBatch);
        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();
        createdTags.Data.Count.Should().Be(3);

        var tag = await _repository.GetByLabel("Tag5");
        tag.Status.Should().Be(StorageResultStatus.NotFound);
        tag.Data.Should().BeNull();
    }

    [Fact]
    public async Task Exists_WhenInvokedOnArbitraryListOfTagLabels_ShouldMarkExistingTagsCorrectly()
    {
        // Arrange
        var tagsBatch = new List<Tag>
        {
            new() { Label = "Tag1", BgColorHex = "aaaaaa", FgColorHex = "000000" },
            new() { Label = "Tag2", BgColorHex = "bbbbbb", FgColorHex = "333333" },
            new() { Label = "Tag3", BgColorHex = "cccccc", FgColorHex = "888888" },
        }.ToImmutableList();

        // Act && Assert
        var createdTags = await _repository.CreateRange(tagsBatch);
        createdTags.Status.Should().Be(StorageResultStatus.Success);
        createdTags.Data.Should().NotBeNull();
        createdTags.Data.Count.Should().Be(3);

        var result = await _repository.Exists(["Tag1", "Tag2", "Tag3", "Tag4", "Tag5"]);
        result.Status.Should().Be(StorageResultStatus.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<ImmutableDictionary<string, bool>>();
        result.Data.Count.Should().Be(5);

        result.Data["Tag1"].Should().BeTrue();
        result.Data["Tag2"].Should().BeTrue();
        result.Data["Tag3"].Should().BeTrue();
        result.Data["Tag4"].Should().BeFalse();
        result.Data["Tag5"].Should().BeFalse();
    }
}