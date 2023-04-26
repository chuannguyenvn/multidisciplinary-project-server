using System.Security.Claims;
using System.Text;
using Antlr4.Runtime;
using multidisciplinary_project_server;
using Server.Models;
using Server.WateringRules;

namespace Server.Services;

public class HelperService
{
    private readonly Settings _settings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public string AnnounceTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitAnnounceFeedName;
    public string SensorTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitSensorFeedName;
    public string LightTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitLightFeedName;
    public string TemperatureTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitTemperatureFeedName;
    public string MoistureTopicPath => _settings.AdafruitUsername + "/feeds/" + _settings.AdafruitFeedName + "." + _settings.AdafruitMoistureFeedName;

    public HelperService(Settings settings, IServiceScopeFactory serviceScopeFactory)
    {
        _settings = settings;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public string DecodeMqttPayload(byte[] encodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(Convert.ToBase64String(encodedData));
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string ConstructAddNewPlantRequestMessage(int plantId)
    {
        return plantId + ";N";
    }

    public string ConstructAddNewPlantResponseMessage(int plantId)
    {
        return plantId + ";ND";
    }

    public string ConstructRemovePlantRequestMessage(int plantId)
    {
        return plantId + ";R";
    }

    public string ConstructRemovePlantResponseMessage(int plantId)
    {
        return plantId + ";RD";
    }

    public string ConstructWaterPlantRequestMessage(int plantId)
    {
        return plantId + ";W";
    }

    public string ConstructWaterPlantResponseMessage(int plantId)
    {
        return plantId + ";WD";
    }

    public (bool success, WateringRule wateringRule) TryParserWateringRuleString(string ruleString)
    {
        WateringRule wateringRule;
        try
        {
            AntlrInputStream inputStream = new AntlrInputStream(ruleString);
            WateringRuleLexer wateringRuleLexer = new WateringRuleLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(wateringRuleLexer);
            WateringRuleParser wateringRuleParser = new WateringRuleParser(commonTokenStream);
            WateringRuleParser.ProgramContext programContext = wateringRuleParser.program();
            WateringRuleVisitor wateringRuleVisitor = new WateringRuleVisitor();
            wateringRule = (WateringRule)wateringRuleVisitor.Visit(programContext);
        }
        catch (Exception e)
        {
            return (false, null);
        }

        return (true, wateringRule);
    }

    public bool DoesPlantIdExist(int plantId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        return dbContext.PlantInformations.Any(info => info.Id == plantId);
    }

    public bool DoesUserOwnThisPlant(ClaimsPrincipal user, int plantId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        int userId = int.Parse(user.FindFirst("id").Value);
        return dbContext.PlantInformations.Any(info => info.Id == plantId && info.Owner.Id == userId);
    }

    public bool DoesPlantHaveAnyDataLog(int plantId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        return dbContext.PlantDataLogs.Any(log => log.LoggedPlant.Id == plantId);
    }
}