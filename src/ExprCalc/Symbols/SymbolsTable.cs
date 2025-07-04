using System.Collections.Generic;

namespace ExprCalc.Symbols
{
    public class SymbolsTable
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public static PrimitiveType Number = new PrimitiveType("<number>");
        public static PrimitiveType Boolean = new PrimitiveType("<boolean>");
        public static PrimitiveType String = new PrimitiveType("<string>");
        public static PrimitiveType Time = new PrimitiveType("<time>");
        public static PrimitiveType Undefined = new PrimitiveType("<undefined>");

        public SymbolsTable(List<Symbol> builtInSymbols)
        {
            DefineBuiltInTypes();

            foreach (var symbol in builtInSymbols)
                Define(symbol);
        }

        private void DefineBuiltInTypes()
        {
            Define(Number);
            Define(Boolean);
            Define(String);
            Define(Time);
            Define(Undefined);
        }

        public Symbol Define(Symbol symbol)
        {
            if (!_symbols.ContainsKey(symbol.Name))
            {
                _symbols.Add(symbol.Name, symbol);
                return symbol;
            }

            return _symbols[symbol.Name];
        }

        public Symbol Resolve(string name) => _symbols[name];
    }
}
