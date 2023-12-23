using Microsoft.CodeAnalysis;

namespace ZeroUnit;

internal static class TestCallGenerator
{
    internal static string Generate(IMethodSymbol method, int index)
    {
        var sb = new CodeStringBuilder();
        Generate(sb, method, index);
        return sb.ToString();
    }

    internal static void Generate(CodeStringBuilder sb, IMethodSymbol method, int index)
    {
        var containingType = method.ContainingType;
        var containingTypeFullName = NameHelper.GetFullName(containingType);
        var isContaningTypeDisposable = containingType.AllInterfaces.Any(x => NameHelper.GetFullName(x) == "System.IDisposable");

        if (!method.IsStatic)
        {
            sb.StartLine();
            if (isContaningTypeDisposable)
            {
                sb.Append("using (");
            }
            sb.Append($"var fixture{index} = new {containingTypeFullName}()");
            if (isContaningTypeDisposable)
            {
                sb.EndLine(")");
            }
            else
            {
                sb.EndLine(";");
            }
        }

        if (isContaningTypeDisposable)
        {
            sb.AppendLine("{");
            sb.Indent();
        }

        sb.StartLine();

        if (!method.ReturnsVoid)
        {
            sb.Append("await ");
        }

        if (method.IsStatic)
        {
            sb.Append(containingTypeFullName);
        }
        else
        {
            sb.Append($"fixture{index}");
        }

        sb.Append($".{method.Name}()");

        if (!method.ReturnsVoid)
        {
            sb.Append(".ConfigureAwait(false)");
        }

        sb.EndLine(";");

        if (isContaningTypeDisposable)
        {
            sb.Unindent();
            sb.AppendLine("}");
        }
    }
}