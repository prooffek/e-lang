using E_Lang.Seeder.Interfaces;

namespace E_Lang.Seeder.Seeders;

public class CollectionSeeder : SeederBase
{
    public CollectionSeeder(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    
    public override async Task Seed()
    {
        await SeedCollections();
    }

    private async Task SeedCollections()
    {
        var userId = _userService.GetUserId();

        await Builder
            .AddCollection(out var _, userId)
                .SetName("English")
                    .AddSubcollection(out var _)
                        .SetName("Level A")
                        .AddSubcollection(out var _)
                            .SetName("A0")
                            .Build()
                        .AddSubcollection(out var _)
                            .SetName("A1")
                            .Build()
                        .AddSubcollection(out var _)
                            .SetName("A2")
                            .Build()
                        .Build()
                    .AddSubcollection(out var _)
                        .SetName("Level B")
                        .AddSubcollection(out var _)
                            .SetName("B1")
                            .Build()
                        .AddSubcollection(out var _)
                            .SetName("B2")
                            .Build()
                        .Build()
                    .AddSubcollection(out var _)
                        .SetName("Level C")
                        .AddSubcollection(out var _)
                            .SetName("C1")
                            .Build()
                        .AddSubcollection(out var _)
                            .SetName("C2")
                            .Build()
                        .Build()
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Polish")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("German")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("French")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Chinese")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Italian")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Japanese")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Korean")
                .Build()
            .AddCollection(out var _, userId)
                .SetName("Nahuatl")
                .Build()
            .SaveAsync();
    }
}