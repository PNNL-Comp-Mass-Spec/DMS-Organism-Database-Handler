﻿using System.Collections;

namespace NucleotideTranslator
{
    internal class TranslationEntry
    {
        protected string m_Base;
        protected ArrayList m_TransEntries;
        protected string m_TranslatedAA;

        internal TranslationEntry(string NucleotideBase, ArrayList TranslationEntries)
        {
            m_Base = NucleotideBase;
            m_TransEntries = TranslationEntries;
        }

        internal TranslationEntry(string NucleotideBase, string TranslatedAA) : base()
        {
            m_Base = NucleotideBase;
            m_TranslatedAA = TranslatedAA;
        }

        internal string BaseLetter => m_Base;

        internal ArrayList TranslationEntries => m_TransEntries;

        internal string TranslatedAA => m_TranslatedAA;
    }
}