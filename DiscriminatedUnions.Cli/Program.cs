using DiscriminatedUnions.Cli;

var animalService = new AnimalService();
var animals = await animalService.GetAnimalsAsync().ConfigureAwait(false);

foreach (var animal in animals)
{
    #region Use type switch to determine the type of the animal

    switch (animal.Value)
    {
        case Dog dog:
            Console.WriteLine("Dog");
            break;
        case Cat cat:
            Console.WriteLine("Cat");
            break;
        default:
            throw new InvalidOperationException("Unknown animal type");
    }

    #endregion

    #region Use enum type switch to determine the type of the animal

    switch (animal.AnimalType)
    {
        case AnimalType.Dog:
            Console.WriteLine("Dog");
            break;
        case AnimalType.Cat:
            Console.WriteLine("Cat");
            break;
        default:
            throw new InvalidOperationException("Unknown animal type");
    }

    #endregion

    #region Use Match method to determine the type of the animal

    var isCat = animal.Match(
        _ => false,
        _ => true
    );

    #endregion

    #region Use Visit method to determine the type of the animal

    animal.Visit(
        _ => Console.WriteLine("Dog"),
        _ => Console.WriteLine("Cat")
    );

    #endregion

    #region Use helper methods to check the type of the animal

    if (animal.IsDog)
    {
        Console.WriteLine("Dog");
    }
    else if (animal.IsCat)
    {
        Console.WriteLine("Cat");
    }
    else
    {
        throw new InvalidOperationException("Unknown animal type");
    }

    #endregion

    #region Cast the animal to the correct type

    if (animal.IsDog)
    {
        var dog = animal.AsDog();
        dog = (Dog)animal.Value;
        Console.WriteLine("Dog");
    }
    else if (animal.IsCat)
    {
        var cat = animal.AsCat();
        cat = (Cat)animal.Value;
        Console.WriteLine("Cat");
    }
    else
    {
        throw new InvalidOperationException("Unknown animal type");
    }

    #endregion
}

await animalService.AddAnimalAsync(new Animal(new Dog { LikesToWoof = true })).ConfigureAwait(false);
await animalService.AddAnimalAsync(new Dog { LikesToWoof = true }).ConfigureAwait(false);
await animalService.AddAnimalAsync(new Cat { LikesToMeow = true }).ConfigureAwait(false);
