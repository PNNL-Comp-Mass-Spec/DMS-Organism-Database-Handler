using System.Collections;

namespace PRISMSeq_Uploader.NucleotideTranslator
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