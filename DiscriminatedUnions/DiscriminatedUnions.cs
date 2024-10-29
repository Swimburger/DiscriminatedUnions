using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(TwoDimensionalPoint), typeDiscriminator: "2d")]
[JsonDerivedType(typeof(ThreeDimensionalPoint), typeDiscriminator: "3d")]
[JsonDerivedType(typeof(FourDimensionalPoint), typeDiscriminator: "4d")]
[JsonDerivedType(typeof(TwoDimensionalPointWithName), typeDiscriminator: "2dNamed")]
public interface IBasePoint
{
    public string Type { get; }
}

public static class IBasePointExtensions
{
    public static T Match<T>(this IBasePoint basePoint,
        Func<TwoDimensionalPoint, T> onTwoDimensionalPoint,
        Func<TwoDimensionalPointWithName, T> onTwoDimensionalPointWithName,
        Func<ThreeDimensionalPoint, T> onThreeDimensionalPoint,
        Func<FourDimensionalPoint, T> onFourDimensionalPoint,
        Func<T> onFallback
    )
    {
        switch (basePoint)
        {
            case TwoDimensionalPoint twoDimensionalPoint:
                return onTwoDimensionalPoint(twoDimensionalPoint);
            case TwoDimensionalPointWithName twoDimensionalPointWithName:
                return onTwoDimensionalPointWithName(twoDimensionalPointWithName);
            case ThreeDimensionalPoint threeDimensionalPoint:
                return onThreeDimensionalPoint(threeDimensionalPoint);
            case FourDimensionalPoint fourDimensionalPoint:
                return onFourDimensionalPoint(fourDimensionalPoint);
            default:
                return onFallback();
        }
    }
    public static void Visit(this IBasePoint basePoint,
        Action<TwoDimensionalPoint> onTwoDimensionalPoint,
        Action<TwoDimensionalPointWithName> onTwoDimensionalPointWithName,
        Action<ThreeDimensionalPoint> onThreeDimensionalPoint,
        Action<FourDimensionalPoint> onFourDimensionalPoint,
        Action onFallback
    )
    {
        switch (basePoint)
        {
            case TwoDimensionalPoint twoDimensionalPoint:
                onTwoDimensionalPoint(twoDimensionalPoint);
                break;
            case TwoDimensionalPointWithName twoDimensionalPointWithName:
                onTwoDimensionalPointWithName(twoDimensionalPointWithName);
                break;
            case ThreeDimensionalPoint threeDimensionalPoint:
                onThreeDimensionalPoint(threeDimensionalPoint);
                break;
            case FourDimensionalPoint fourDimensionalPoint:
                onFourDimensionalPoint(fourDimensionalPoint);
                break;
            default:
                onFallback();
                break;
        }
    }
}

public interface ITwoDimensionalPoint
{
    public int X { get; set; }
    public int Y { get; set; }
}

public record TwoDimensionalPoint : ITwoDimensionalPoint, IBasePoint
{
    [JsonPropertyName("type")]
    public string Type => "2d";
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
}

public interface IThreeDimensionalPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}

public record ThreeDimensionalPoint : IThreeDimensionalPoint, IBasePoint
{
    [JsonPropertyName("type")]
    public string Type => "3d";
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
    [JsonPropertyName("z")]
    public int Z { get; set; }
}

public interface IFourDimensionalPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int W { get; set; }
}

public record FourDimensionalPoint : IFourDimensionalPoint, IBasePoint
{
    [JsonPropertyName("type")]
    public string Type => "4d";
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
    [JsonPropertyName("z")]
    public int Z { get; set; }
    [JsonPropertyName("w")]
    public int W { get; set; }
}

public interface INamed
{
    public string Name { get; set; }
}

public record TwoDimensionalPointWithName : ITwoDimensionalPoint, INamed, IBasePoint
{
    [JsonPropertyName("type")]
    public string Type => "2dNamed";
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
}