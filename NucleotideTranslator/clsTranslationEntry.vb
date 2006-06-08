Friend Class clsTranslationEntry


    Protected m_Base As String
    Protected m_TransEntries As ArrayList
    Protected m_TranslatedAA As String

    Friend Sub New(ByVal NucleotideBase As String, ByVal TranslationEntries As ArrayList)
        Me.m_Base = NucleotideBase
        Me.m_TransEntries = TranslationEntries
    End Sub

    Friend Sub New(ByVal NucleotideBase As String, ByVal TranslatedAA As String)
        MyBase.New()
        Me.m_Base = NucleotideBase
        Me.m_TranslatedAA = TranslatedAA
    End Sub

    Friend ReadOnly Property BaseLetter() As String
        Get
            Return Me.m_Base
        End Get
    End Property

    Friend ReadOnly Property TranslationEntries() As ArrayList
        Get
            Return Me.m_TransEntries
        End Get
    End Property

    Friend ReadOnly Property TranslatedAA() As String
        Get
            Return Me.m_TranslatedAA
        End Get
    End Property

End Class