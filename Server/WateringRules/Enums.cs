namespace Server.WateringRules;

public enum MetricType
{
    Light,
    Temperature,
    Moisture,
}

public enum Comparision
{
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}

public enum Logic
{
    And,
    Or,
    Not,
}

public class MetricValues
{
    public float LightValue { get; private set; }
    public float TemperatureValue { get; private set; }
    public float MoistureValue { get; private set; }

    public MetricValues(float lightValue, float temperatureValue, float moistureValue)
    {
        LightValue = lightValue;
        TemperatureValue = temperatureValue;
        MoistureValue = moistureValue;
    }
}