using System.Collections;

namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    internal class TranslationEntry
    {
        private string mBase;
        private ArrayList mTransEntries;
        private string mTranslatedAA;

        internal TranslationEntry(string NucleotideBase, ArrayList TranslationEntries)
        {
            mBase = NucleotideBase;
            mTransEntries = TranslationEntries;
        }

        internal TranslationEntry(string NucleotideBase, string TranslatedAA) : base()
        {
            mBase = NucleotideBase;
            mTranslatedAA = TranslatedAA;
        }

        internal string BaseLetter => mBase;

        internal ArrayList TranslationEntries => mTransEntries;

        internal string TranslatedAA => mTranslatedAA;
    }
}