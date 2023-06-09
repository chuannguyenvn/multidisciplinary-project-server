﻿using Communications.Requests;
using Server.WateringRules;

namespace Server.Services;

public class WateringRuleTestingService
{
    private readonly HelperService _helperService;
    private WateringRule _currentWateringRule;

    public WateringRuleTestingService(HelperService helperService)
    {
        _helperService = helperService;
    }

    public (bool success, string result) ChangeRule(ChangeTestingRuleRequest request)
    {
        (bool success, WateringRule wateringRule) = _helperService.TryParserWateringRuleString(request.NewRule);
        if (success)
        {
            _currentWateringRule = wateringRule;
            return (true, "Successful.");
        }
        else return (false, "Failed.");
    }

    public (bool success, string result) TestAgainstMetrics(TestAgainstMetricsRequest request)
    {
        if (_currentWateringRule == null) return (false, "No rule registered.");
        var metricValues = new MetricValues(request.Light, request.Temperature, request.Moisture);

        bool evaluatedResult;
        try
        {
            evaluatedResult = _currentWateringRule.Evaluate(metricValues);
        }
        catch (Exception e)
        {
            return (false, "Failed: " + e);
        }

        if (evaluatedResult)
            return (true, "Must water now!");
        else
            return (true, "No need to water now.");
    }
}