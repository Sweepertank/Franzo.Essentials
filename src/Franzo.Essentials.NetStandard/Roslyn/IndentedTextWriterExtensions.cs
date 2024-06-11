using System.CodeDom.Compiler;

namespace Franzo.Essentials.Roslyn;

public static class IndentedTextWriterExtensions
{
    public static void WriteBracedSectionStart(this IndentedTextWriter self)
    {
        self.Write("{");
        self.WriteLine();
        self.Indent++;
    }

    public static void WriteBracedSectionEnd(this IndentedTextWriter self)
    {
        self.Indent--;
        self.Write("}");
        self.WriteLine();
    }
}
