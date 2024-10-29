using System.Text.Json;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace DiscriminatedUnions.Cli;

public class AnimalService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<List<Animal>> GetAnimalsAsync()
    {
        var json = await File.ReadAllTextAsync("Files/animals.json").ConfigureAwait(false);
        var animals = JsonSerializer.Deserialize<List<Animal>>(json, JsonOptions)
                      ?? throw new Exception("Failed to deserialize animals");
        return animals;
    }

    public Task AddAnimalAsync(Animal animal)
    {
        Console.WriteLine("Added animal:");
        Console.WriteLine(JsonSerializer.Serialize(animal, JsonOptions));
        return Task.CompletedTask;
    }
}