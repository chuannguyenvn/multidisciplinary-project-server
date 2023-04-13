using Antlr4.Runtime;
using multidisciplinary_project_server;
using Server.WateringRules;

namespace Server.Services;

public class WateringRuleService : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            string text = "T < 3;";
            AntlrInputStream inputStream = new AntlrInputStream(text);
            WateringRuleLexer wateringRuleLexer = new WateringRuleLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(wateringRuleLexer);
            WateringRuleParser wateringRuleParser = new WateringRuleParser(commonTokenStream);
            WateringRuleParser.ProgramContext programContext = wateringRuleParser.program();
            WateringRuleVisitor wateringRuleVisitor = new WateringRuleVisitor();
            WateringRule wateringRule = (WateringRule)wateringRuleVisitor.Visit(programContext);

            MetricValues metricValues = new MetricValues(5, 123, 3);
            Console.WriteLine("Is it time to water: " + wateringRule.Evaluate(metricValues));
        }
    }
}