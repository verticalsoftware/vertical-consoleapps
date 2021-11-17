using System;
using System.Collections.Generic;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Pipeline;

public class RequestItemsTests
{
    [Fact]
    public void SetRejectsNullValue()
    {
        Should.Throw<ArgumentNullException>(() => new RequestItems().Set<string>(null!));
    }

    [Fact]
    public void ObjectIsRetrievableByType()
    {
        var items = new RequestItems();
        items.Set(new List<string>());
        items.Get<List<string>>().ShouldNotBeNull();
        items.GetRequired<List<string>>().ShouldNotBeNull();
    }

    [Fact]
    public void RetrieveItemNotAddedThrows()
    {
        Should.Throw<InvalidOperationException>(() => new RequestItems().GetRequired<List<string>>());
    }
}