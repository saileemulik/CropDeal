using Microsoft.EntityFrameworkCore;
using CropDeal.AppDB;

namespace CropDeal.UnitTests.Helpers;

// This makes a fake database for testing
public static class TestDbContextFactory
{
    // Creates a new fake database each time
    public static CropDealDBContext CreateFakeDatabase()
    {
        var options = new DbContextOptionsBuilder<CropDealDBContext>()
            .UseInMemoryDatabase("FakeDB_" + Guid.NewGuid())
            .Options;
        
        return new CropDealDBContext(options);
    }
}