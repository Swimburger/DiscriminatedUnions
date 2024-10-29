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
        Assert.Multiple(() =>
        {
            Assert.That(
                Serialize(new TwoDimensionalPoint
                {
                    X = 1,
                    Y = 2
                }),
                Is.EqualTo("""
                           {
                             "type": "2d",
                             "x": 1,
                             "y": 2
                           }
                           """)
            );
            Assert.That(
                Serialize(new ThreeDimensionalPoint
                {
                    X = 1,
                    Y = 2,
                    Z = 3
                }),
                Is.EqualTo("""
                           {
                             "type": "3d",
                             "x": 1,
                             "y": 2,
                             "z": 3
                           }
                           """)
            );
            Assert.That(
                Serialize(new FourDimensionalPoint()
                {
                    X = 1,
                    Y = 2,
                    Z = 3,
                    W = 4
                }),
                Is.EqualTo("""
                           {
                             "type": "4d",
                             "x": 1,
                             "y": 2,
                             "z": 3,
                             "w": 4
                           }
                           """)
            );
            Assert.That(
                Serialize(new TwoDimensionalPointWithName
                {
                    Name = "root",
                    X = 1,
                    Y = 2,
                }),
                Is.EqualTo("""
                           {
                             "type": "2dNamed",
                             "name": "root",
                             "x": 1,
                             "y": 2
                           }
                           """)
            );
        });
    }

    [Test]
    public void Should_Deserialize_To_Discriminated_Unions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                Deserialize<IBasePoint>("""
                            {
                              "type": "2d",
                              "x": 1,
                              "y": 2
                            }
                            """),
                Is.EqualTo(new TwoDimensionalPoint
                {
                    X = 1,
                    Y = 2
                })
            );
            Assert.That(
                Deserialize<IBasePoint>("""
                                        {
                                          "type": "3d",
                                          "x": 1,
                                          "y": 2,
                                          "z": 3
                                        }
                                        """),
                Is.EqualTo(new ThreeDimensionalPoint
                {
                    X = 1,
                    Y = 2,
                    Z = 3
                })
            );
            Assert.That(
                Deserialize<IBasePoint>("""
                                        {
                                          "type": "4d",
                                          "x": 1,
                                          "y": 2,
                                          "z": 3,
                                          "w": 4
                                        }
                                        """),
                Is.EqualTo(new FourDimensionalPoint
                {
                    X = 1,
                    Y = 2,
                    Z = 3,
                    W = 4,
                })
            );
            Assert.That(
                Deserialize<IBasePoint>("""
                                        {
                                          "type": "2dNamed",
                                          "name": "root",
                                          "x": 1,
                                          "y": 2
                                        }
                                        """),
                Is.EqualTo(new TwoDimensionalPointWithName
                {
                    Name = "root",
                    X = 1,
                    Y = 2,
                })
            );
        });
    }
    
    [Test]
    public void Should_Switch_Over_Discriminated_Unions_Type()
    {
        var point = Deserialize<IBasePoint>("""
                                {
                                  "type": "3d",
                                  "x": 1,
                                  "y": 2,
                                  "z": 3
                                }
                                """);
        
        Assert.That(point is ThreeDimensionalPoint, Is.True, "Point is not ThreeDimensionalPoint");
        var namedPoint = Deserialize<IBasePoint>("""
                                            {
                                              "type": "2dNamed",
                                              "name": "root",
                                              "x": 1,
                                              "y": 2
                                            }
                                            """);
        Assert.Multiple(() =>
        {
            Assert.That(namedPoint is TwoDimensionalPointWithName, Is.True, "Point is not TwoDimensionalPointWithName");
            Assert.That(namedPoint is ITwoDimensionalPoint, Is.True, "Point is not ITwoDimensionalPoint");
            Assert.That(namedPoint is INamed, Is.True, "Point is not INamed");
        });
    }
    
    [Test]
    public void Should_Match_Discriminated_Unions()
    {
        var point = Deserialize<IBasePoint>("""
                                            {
                                              "type": "3d",
                                              "x": 1,
                                              "y": 2,
                                              "z": 3
                                            }
                                            """);

        var result = point.Match(
            _ => false,
            _ => false,
            _ => true,
            _ => false,
            () => false
        );
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void Should_Visit_Discriminated_Unions()
    {
        var point = Deserialize<IBasePoint>("""
                                            {
                                              "type": "3d",
                                              "x": 1,
                                              "y": 2,
                                              "z": 3
                                            }
                                            """);

        point.Visit(
            _ => Assert.Fail(),
            _ => Assert.Fail(),
            _ => Assert.Pass(),
            _ => Assert.Fail(),
            Assert.Fail
        );
        Assert.Fail();
    }
}
