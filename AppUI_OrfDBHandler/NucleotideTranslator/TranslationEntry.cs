using System.Collections;

namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    internal class TranslationEntry
    {
        internal TranslationEntry(string nucleotideBase, ArrayList translationEntries)
        {
            BaseLetter = nucleotideBase;
            TranslationEntries = translationEntries;
        }

        internal TranslationEntry(string nucleotideBase, string translatedAA) : base()
        {
            BaseLetter = nucleotideBase;
            TranslatedAA = translatedAA;
        }

        internal string BaseLetter { get; }

        internal ArrayList TranslationEntries { get; }

        internal string TranslatedAA { get; }
    }
}