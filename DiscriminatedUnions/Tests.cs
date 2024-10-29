using System.Text.Json;

namespace DiscriminatedUnions;

public class Tests
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    private string Serialize<T>(T input)
    {
        return JsonSerializer.Serialize(input, _jsonSerializerOptions);
    }

    private T? Deserialize<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input, _jsonSerializerOptions);
    }

    [Test]
    public void Should_Serialize_To_Discriminated_Unions()
    {
        Assert.That(
            Serialize(new Animal(new Cat
            {
                LikesToMeow = true
            })),
            Is.EqualTo("""
                       {
                         "animalType": "Cat",
                         "likesToMeow": true
                       }
                       """)
        );
        Assert.That(
            Serialize<AnimalWithDiscriminator>(new Cat
            {
                LikesToMeow = true
            }),
            Is.EqualTo("""
                       {
                         "animalType": "Cat",
                         "likesToMeow": true
                       }
                       """)
        );
        Assert.That(
            Serialize(new Cat
            {
                LikesToMeow = true
            }),
            Is.EqualTo("""
                       {
                         "likesToMeow": true
                       }
                       """)
        );
    }

    [Test]
    public void Should_Deserialize_To_Discriminated_Unions()
    {
        Assert.That(
            Deserialize<Cat>("""
                             {
                               "likesToMeow": true
                             }
                             """),
            Is.EqualTo(new Cat
            {
                LikesToMeow = true
            })
        );
        Assert.That(
            Deserialize<AnimalWithDiscriminator>("""
                                     {
                                       "animalType": "Cat",
                                       "likesToMeow": true
                                     }
                                     """),
            Is.EqualTo(new Cat
            {
                LikesToMeow = true
            })
        );

        var animal = Deserialize<Animal>("""
                                         {
                                           "animalType": "Cat",
                                           "likesToMeow": true
                                         }
                                         """);
        Assert.Multiple(() =>
        {
            Assert.That(animal, Is.Not.Null);
            Assert.That(animal!.AnimalType, Is.EqualTo(AnimalType.Cat));
            Assert.That(animal.Value, Is.InstanceOf<AnimalValue>());
            Assert.That(animal.Value, Is.TypeOf<Cat>());
            Assert.That(animal.IsCat, Is.True);
            Assert.That(animal.IsDog, Is.False);
            Assert.That(animal.AsCat(), Is.InstanceOf<AnimalValue>());
            Assert.That(animal.AsCat(), Is.TypeOf<Cat>());
            Assert.That(animal.AsCat().LikesToMeow, Is.True);
        });
    }

    [Test]
    public void Should_Switch_Over_Discriminated_Unions_Type()
    {
        var animal = Deserialize<Animal>("""
                                         {
                                           "animalType": "Cat",
                                           "likesToMeow": true
                                         }
                                         """);

        switch (animal.Value)
        {
            case Dog dog:
                Assert.Fail();
                break;
            case Cat cat:
                break;
            default:
                Assert.Fail();
                break;
        }

        switch (animal.AnimalType)
        {
            case AnimalType.Dog:
                Assert.Fail();
                break;
            case AnimalType.Cat:
                break;
            default:
                Assert.Fail();
                break;
        }
    }

    [Test]
    public void Should_Match_Discriminated_Unions()
    {
        var animal = Deserialize<Animal>("""
                                         {
                                           "animalType": "Cat",
                                           "likesToMeow": true
                                         }
                                         """);

        var result = animal.Match(
            _ => false,
            _ => true
        );
        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_Visit_Discriminated_Unions()
    {
        var animal = Deserialize<Animal>("""
                                         {
                                           "animalType": "Cat",
                                           "likesToMeow": true
                                         }
                                         """);

        animal.Visit(
            _ => Assert.Fail(),
            _ => Assert.Pass()
        );
    }
}