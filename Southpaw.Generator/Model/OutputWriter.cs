using System.Text;

namespace Southpaw.Generator
{
    public class OutputWriter
    {
        private int _currentIndentation = 0;
        private bool _isNewLine = true;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public OutputWriter Indent()
        {
            _currentIndentation++;
            return this;
        }

        public OutputWriter Unindent()
        {
            _currentIndentation--;
            return this;
        }

        public OutputWriter Write(string s)
        {
            if (_isNewLine)
            {
                _stringBuilder.AppendFormat("{0," + (_currentIndentation * 4) + "}", "");
                _isNewLine = false;
            }
            _stringBuilder.Append(s);
            return this;
        }

        
        public OutputWriter EndLine()
        {
            _isNewLine = true;
            _stringBuilder.AppendLine();
            return this;
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}