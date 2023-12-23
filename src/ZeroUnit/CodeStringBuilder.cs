using System.Text;

namespace ZeroUnit;

internal class CodeStringBuilder
{
    private const int SpaceCount = 4;
    private readonly StringBuilder _builder = new();
    private int _indent;

    public CodeStringBuilder StartLine(string value)
    {
        AppendIndentation();
        _builder.Append(value);
        return this;
    }

    public CodeStringBuilder StartLine()
    {
        AppendIndentation();
        return this;
    }

    public CodeStringBuilder EndLine(string value)
    {
        return AppendUnindentedLine(value);
    }

    public CodeStringBuilder AppendLine(string value)
    {
        AppendIndentation();
        _builder.AppendLine(value);
        return this;
    }

    public CodeStringBuilder AppendUnindentedLine(string value)
    {
        _builder.AppendLine(value);
        return this;
    }

    public CodeStringBuilder AppendLine()
    {
        _builder.AppendLine();
        return this;
    }

    public CodeStringBuilder Append(string value)
    {
        _builder.Append(value);
        return this;
    }

    public CodeStringBuilder Indent()
    {
        _indent++;
        return this;
    }

    public CodeStringBuilder Unindent()
    {
        if (_indent == 0)
            throw new InvalidOperationException("No indentation");

        _indent--;
        return this;
    }

    private void AppendIndentation()
    {
        _builder.Append(' ', SpaceCount * _indent);
    }

    public override string ToString() => _builder.ToString();
}