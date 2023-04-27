using System.Security.Claims;
using System.Text;
using Antlr4.Runtime;
using Cronos;
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

    public string ConstructAddNewPlantRequestMessage(int plantOrder)
    {
        return plantOrder + ";N";
    }

    public string ConstructAddNewPlantResponseMessage(int plantOrder)
    {
        return plantOrder + ";ND";
    }

    public string ConstructRemovePlantRequestMessage(int plantOrder)
    {
        return plantOrder + ";R";
    }

    public string ConstructRemovePlantResponseMessage(int plantOrder)
    {
        return plantOrder + ";RD";
    }

    public string ConstructWaterPlantRequestMessage(int plantOrder)
    {
        return plantOrder + ";W";
    }

    public string ConstructWaterPlantResponseMessage(int plantOrder)
    {
        return plantOrder + ";WD";
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

    public (bool success, CronExpression cronExpression) TryParserCronString(string cronString)
    {
        CronExpression cronExpression;
        try
        {
            cronExpression = CronExpression.Parse(cronString);
        }
        catch (Exception e)
        {
            return (false, null);
        }

        return (true, cronExpression);
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

    public string GenerateRecognizerCode(int plantId)
    {
        Dictionary<int, char> translator = new()
        {
            {0, 'R'},
            {1, 'G'},
            {2, 'B'},
            {3, 'C'},
            {4, 'Y'},
            {5, 'M'},
        };

        string code = "";

        Random random = new Random(plantId);
        
        for (int i = 0; i < 16; i++)
        {
            code += translator[random.Next() % 6];
        }

        return code;
    }
}