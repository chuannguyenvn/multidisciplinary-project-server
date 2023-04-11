namespace Server.Models;

public class AccumulatedPlantDataLog
{
    public int PlantId { get; set; }

    public float AccumulatedLightValue { get; set; }
    public int LightValueCount { get; set; }
    public float AveragedLightValue => AccumulatedLightValue / LightValueCount;

    public float AccumulatedTemperatureValue { get; set; }
    public int TemperatureValueCount { get; set; }
    public float AveragedTemperatureValue => AccumulatedTemperatureValue / TemperatureValueCount;

    public float AccumulatedMoistureValue { get; set; }
    public int MoistureValueCount { get; set; }
    public float AveragedMoistureValue => AccumulatedMoistureValue / MoistureValueCount;
}