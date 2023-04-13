using multidisciplinary_project_server;

namespace Server.WateringRules;

public class WateringRuleVisitor : WateringRuleBaseVisitor<object>
{
    public override object VisitProgram(WateringRuleParser.ProgramContext context)
    {
        List<RuleUnit> ruleUnits = new();
        foreach (var ruleContext in context.rule())
        {
            ruleUnits.Add((RuleUnit)Visit(ruleContext));
        }

        return new WateringRule(ruleUnits);
    }

    public override object VisitRule(WateringRuleParser.RuleContext context)
    {
        return Visit(context.expr());
    }

    public override object VisitExpr(WateringRuleParser.ExprContext context)
    {
        return Visit(context.logic_expr());
    }

    public override object VisitLogic_expr(WateringRuleParser.Logic_exprContext context)
    {
        if (context.AND() != null) return new LogicUnit(Logic.And, (RuleUnit)Visit(context.logic_expr(0)), (RuleUnit)Visit(context.logic_expr(1)));
        if (context.OR() != null) return new LogicUnit(Logic.Or, (RuleUnit)Visit(context.logic_expr(0)), (RuleUnit)Visit(context.logic_expr(1)));
        if (context.NOT() != null) return new LogicUnit(Logic.Not, (RuleUnit)Visit(context.logic_expr(0)));
        if (context.comp_expr() != null) return Visit(context.comp_expr());
        return Visit(context.braced_expr());
    }

    public override object VisitComp_expr(WateringRuleParser.Comp_exprContext context)
    {
        Comparision comparision;
        if (context.LT() != null) comparision = Comparision.LessThan;
        else if (context.LE() != null) comparision = Comparision.LessThanOrEqual;
        else if (context.GT() != null) comparision = Comparision.GreaterThan;
        else comparision = Comparision.GreaterThanOrEqual;

        return new ComparisionUnit(comparision, (Operand)Visit(context.operand(0)), (Operand)Visit(context.operand(1)));
    }

    public override object VisitBraced_expr(WateringRuleParser.Braced_exprContext context)
    {
        return Visit(context.expr());
    }

    public override object VisitOperand(WateringRuleParser.OperandContext context)
    {
        if (context.metric() != null) return Visit(context.metric());
        return new FloatUnit(float.Parse(context.NUM().GetText()));
    }

    public override object VisitMetric(WateringRuleParser.MetricContext context)
    {
        if (context.L() != null)
            return new MetricUnit(MetricType.Light);
        else if (context.T() != null)
            return new MetricUnit(MetricType.Temperature);
        else
            return new MetricUnit(MetricType.Moisture);
    }
    
}