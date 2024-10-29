using System.Text.Json.Serialization;
using System.Text.Json;

internal class AnimalConverter : JsonConverter<Animal>
{
    public override bool CanConvert(Type typeToConvert) => typeof(Animal).IsAssignableFrom(typeToConvert);

    public override Animal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonElement.ParseValue(ref reader);
        var animalValue = jsonObject.Deserialize<AnimalWithDiscriminator>(options);
        if (animalValue == null) throw new JsonException();
        return new Animal(animalValue);
    }

    public override void Write(
        Utf8JsonWriter writer, Animal animal, JsonSerializerOptions options)
    {
        var jsonNode = JsonSerializer.SerializeToNode(animal.Value as AnimalWithDiscriminator, options);
        if(jsonNode == null) throw new JsonException();
        jsonNode["animalType"] = animal.AnimalType switch
        {
            AnimalType.Dog => "Dog",
            AnimalType.Cat => "Cat",
            _ => throw new JsonException()
        };
        jsonNode.WriteTo(writer, options);
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AnimalType
{
    Dog,
    Cat
}

[JsonConverter(typeof(AnimalConverter))]
public record Animal
{
    public Animal(Dog dog)
    {
        AnimalType = AnimalType.Dog;
        Value = dog;
    }

    public Animal(Cat cat)
    {
        AnimalType = AnimalType.Cat;
        Value = cat;
    }

    internal Animal(AnimalWithDiscriminator value)
    {
        switch (value)
        {
            case Dog dog:
                AnimalType = AnimalType.Dog;
                Value = dog;
                break;
            case Cat cat:
                AnimalType = AnimalType.Cat;
                Value = cat;
                break;
            default:
                throw new Exception("Unexpected animal type");
        }
    }

    [JsonPropertyName("animalType")] public AnimalType AnimalType { get; }

    [JsonIgnore] public AnimalValue Value { get; }
    public bool IsDog => AnimalType == AnimalType.Dog;
    public bool IsCat => AnimalType == AnimalType.Cat;
    public T As<T>() where T : AnimalValue => (T)Value;
    public Dog AsDog() => (Dog)Value;
    public Cat AsCat() => (Cat)Value;

    public T Match<T>(Func<Dog, T> onDog, Func<Cat, T> onCat)
    {
        switch (AnimalType)
        {
            case AnimalType.Dog:
                return onDog((Dog)Value);
            case AnimalType.Cat:
                return onCat((Cat)Value);
            default:
                throw new Exception("Unexpected animal type");
        }
    }

    public void Visit(Action<Dog> onDog, Action<Cat> onCat)
    {
        switch (AnimalType)
        {
            case AnimalType.Dog:
                onDog((Dog)Value);
                break;
            case AnimalType.Cat:
                onCat((Cat)Value);
                break;
            default:
                throw new Exception("Unexpected animal type");
        }
    }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "animalType",
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(Dog), typeDiscriminator: "Dog")]
[JsonDerivedType(typeof(Cat), typeDiscriminator: "Cat")]
internal interface AnimalWithDiscriminator;
public interface AnimalValue;

public record Dog : AnimalValue, AnimalWithDiscriminator
{
    [JsonPropertyName("likesToWoof")] public bool LikesToWoof { get; set; }
}

public record Cat : AnimalValue, AnimalWithDiscriminator
{
    [JsonPropertyName("likesToMeow")] public bool LikesToMeow { get; set; }
}