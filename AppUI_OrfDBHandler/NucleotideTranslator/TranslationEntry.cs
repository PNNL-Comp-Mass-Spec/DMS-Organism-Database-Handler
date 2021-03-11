using System.Collections;

namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    internal class TranslationEntry
    {
        private string mBase;
        private ArrayList mTransEntries;
        private string mTranslatedAA;

        internal TranslationEntry(string nucleotideBase, ArrayList translationEntries)
        {
            mBase = nucleotideBase;
            mTransEntries = translationEntries;
        }

        internal TranslationEntry(string nucleotideBase, string translatedAA) : base()
        {
            mBase = nucleotideBase;
            mTranslatedAA = translatedAA;
        }

        internal string BaseLetter => mBase;

        internal ArrayList TranslationEntries => mTransEntries;

        internal string TranslatedAA => mTranslatedAA;
    }
}