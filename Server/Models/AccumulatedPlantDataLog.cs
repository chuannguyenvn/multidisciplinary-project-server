namespace Server.Models;

public class AccumulatedPlantDataLog
{
    public int PlantId { get; set; }

    public float AccumulatedLightValue { get; set; }
    public int LightValueCount { get; set; }
    public float AveragedLightValue => LightValueCount != 0 ? AccumulatedLightValue / LightValueCount : 0;

    public float AccumulatedTemperatureValue { get; set; }
    public int TemperatureValueCount { get; set; }
    public float AveragedTemperatureValue => TemperatureValueCount != 0 ? AccumulatedTemperatureValue / TemperatureValueCount : 0;

    public float AccumulatedMoistureValue { get; set; }
    public int MoistureValueCount { get; set; }
    public float AveragedMoistureValue => MoistureValueCount != 0 ? AccumulatedMoistureValue / MoistureValueCount : 0;
}