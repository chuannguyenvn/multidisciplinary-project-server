namespace Server.WateringRules;

public abstract class RuleUnit
{
    public abstract bool Evaluate(MetricValues metricValues);
}

public abstract class Operand
{
    public abstract float Evaluate(MetricValues metricValues);
}

public class FloatUnit : Operand
{
    public float Value { get; private set; }

    public FloatUnit(float value)
    {
        Value = value;
    }

    public override float Evaluate(MetricValues metricValues)
    {
        return Value;
    }
}

public class MetricUnit : Operand
{
    private readonly MetricType _metricType;

    public MetricUnit(MetricType metricType)
    {
        _metricType = metricType;
    }

    public override float Evaluate(MetricValues metricValues)
    {
        return _metricType switch
        {
            MetricType.Light => metricValues.LightValue,
            MetricType.Temperature => metricValues.TemperatureValue,
            MetricType.Moisture => metricValues.MoistureValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public class LogicUnit : RuleUnit
{
    private readonly Logic _logic;
    private readonly RuleUnit _lhs;
    private readonly RuleUnit _rhs;

    public LogicUnit(Logic logic, RuleUnit lhs)
    {
        _logic = logic;
        _lhs = lhs;
    }

    public LogicUnit(Logic logic, RuleUnit lhs, RuleUnit rhs)
    {
        _logic = logic;
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool Evaluate(MetricValues metricValues)
    {
        return _logic switch
        {
            Logic.And => _lhs.Evaluate(metricValues) && _rhs.Evaluate(metricValues),
            Logic.Or => _lhs.Evaluate(metricValues) || _rhs.Evaluate(metricValues),
            Logic.Not => !_lhs.Evaluate(metricValues),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public class ComparisionUnit : RuleUnit
{
    private readonly Comparision _comparision;
    private readonly Operand _lhs;
    private readonly Operand _rhs;

    public ComparisionUnit(Comparision comparision, Operand lhs, Operand rhs)
    {
        _comparision = comparision;
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool Evaluate(MetricValues metricValues)
    {
        return _comparision switch
        {
            Comparision.LessThan => _lhs.Evaluate(metricValues) < _rhs.Evaluate(metricValues),
            Comparision.LessThanOrEqual => _lhs.Evaluate(metricValues) <= _rhs.Evaluate(metricValues),
            Comparision.GreaterThan => _lhs.Evaluate(metricValues) > _rhs.Evaluate(metricValues),
            Comparision.GreaterThanOrEqual => _lhs.Evaluate(metricValues) >= _rhs.Evaluate(metricValues),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public class BracedUnit : RuleUnit
{
    private readonly LogicUnit _logicUnit;

    public BracedUnit(LogicUnit logicUnit)
    {
        _logicUnit = logicUnit;
    }

    public override bool Evaluate(MetricValues metricValues)
    {
        return _logicUnit.Evaluate(metricValues);
    }
}

public class WateringRule
{
    private List<RuleUnit> RuleUnits;

    public WateringRule(List<RuleUnit> ruleUnits)
    {
        RuleUnits = ruleUnits;
    }

    public bool Evaluate(MetricValues metricValues)
    {
        foreach (var ruleUnit in RuleUnits)
        {
            if (ruleUnit.Evaluate(metricValues)) return true;
        }

        return false;
    }
}