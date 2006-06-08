Imports System.Text.RegularExpressions

Public Interface IImportAnnotations

    'Structure GOInfoCategories
    'End Structure

End Interface

Public MustInherit Class clsImportAnnotationsBase

    Public Sub New()

    End Sub




End Class

Public Class GeneOntologyEntry

    Private m_ID As String
    Private m_Name As String
    Private m_Namespace As String
    Private m_Definition As String
    Private m_Comment As String
    Private m_IsObsolete As Boolean
    Private m_ExactSynonym As ArrayList
    Private m_IsA As ArrayList
    Private m_XRefAnalog As ArrayList
    Private m_Relationship As ArrayList
    Private m_SubSet As ArrayList


    Property ID() As String
        Get
            Return Me.m_ID
        End Get
        Set(ByVal Value As String)
            Me.m_ID = Me.CleanUpLine(Value)
        End Set
    End Property
    Property Name() As String
        Get
            Return Me.m_Name
        End Get
        Set(ByVal Value As String)
            Me.m_Name = Me.CleanUpLine(Value)
        End Set
    End Property
    Property [NameSpace]() As String
        Get
            Return Me.m_Namespace
        End Get
        Set(ByVal Value As String)
            Me.m_Namespace = Me.CleanUpLine(Value)
        End Set
    End Property
    Property Definition() As String
        Get
            Return Me.m_Definition
        End Get
        Set(ByVal Value As String)
            Me.m_Definition = Me.CleanUpLine(Value)
        End Set
    End Property
    Property Comment() As String
        Get
            Return Me.m_Comment
        End Get
        Set(ByVal Value As String)
            Me.m_Comment = Me.CleanUpLine(Value)
        End Set
    End Property
    Property IsObsolete() As Boolean
        Get
            Return Me.m_IsObsolete
        End Get
        Set(ByVal Value As Boolean)
            Me.m_IsObsolete = Value
        End Set
    End Property
    ReadOnly Property ExactSynonym_List() As ArrayList
        Get
            Return Me.m_ExactSynonym
        End Get
    End Property
    ReadOnly Property IsA_List() As ArrayList
        Get
            Return Me.m_IsA
        End Get
    End Property
    ReadOnly Property XRefAnalog_List() As ArrayList
        Get
            Return Me.m_XRefAnalog
        End Get
    End Property
    ReadOnly Property Relationship() As ArrayList
        Get
            Return Me.m_Relationship
        End Get
    End Property
    ReadOnly Property SubSet() As ArrayList
        Get
            Return Me.m_SubSet
        End Get
    End Property


    Sub Add_ExactSynonym_Entry(ByVal synonym As String)
        Me.m_ExactSynonym.Add(Me.CleanUpLine(synonym))
    End Sub
    Sub Add_IsA_Entry(ByVal IsAReference As String)
        Me.m_IsA.Add(IsAReference)
    End Sub
    Sub Add_XRefAnalog_Entry(ByVal XRef As String)
        Me.m_XRefAnalog.Add(XRef)
    End Sub
    Sub Add_RelationShip_Entry(ByVal Relationship As String)
        Me.m_Relationship.Add(Relationship)
    End Sub
    Sub Add_Subset_Entry(ByVal Subset As String)
        Me.m_SubSet.Add(Subset)
    End Sub

    Private Function CleanUpLine(ByVal entryLine As String) As String
        Dim tmpEntryLine As String = entryLine.Replace("\"c, Nothing)
        Return tmpEntryLine
    End Function

End Class


Public Class GeneOntologyListOBO


#Region " Regular Expressions "
    Private r_entryHeader As Regex
    Private r_IDLine As Regex
    Private r_NameLine As Regex
    Private r_NameSpaceLine As Regex
    Private r_DefinitionLine As Regex
    Private r_CommentLine As Regex
    Private r_IsObsoleteLine As Regex
    Private r_ExactSynonymLine As Regex
    Private r_IsALine As Regex
    Private r_XRefAnalogLine As Regex
    Private r_RelationshipLine As Regex
    Private r_SubsetLine As Regex
#End Region




    'Send it the text block from a single entry
    Sub New(ByVal GOEntryText As System.Collections.Specialized.StringCollection)

    End Sub

    Sub New()

    End Sub

    Protected Sub ProcessEntry(ByVal EntryCollection As System.Collections.Specialized.StringCollection)

    End Sub

    Private Sub SetupRegexes()
        Me.r_entryHeader = New Regex( _
            "^\[Term\]$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_IDLine = New Regex( _
            "^(?<tag>id):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_NameLine = New Regex( _
            "^(?<tag>name):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_NameSpaceLine = New Regex( _
            "^(?<tag>namespace):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_DefinitionLine = New Regex( _
            "^(?<tag>def):\s+\""+(?<value>.*)\""\s*\[*(?<xref>.*)\]*\s*", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_CommentLine = New Regex( _
            "^(?<tag>comment):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_IsObsoleteLine = New Regex( _
            "^(?<tag>is_obsolete):\s+(?<value>true|false)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_ExactSynonymLine = New Regex( _
            "^(?<tag>exact_synonym):\s+\""+(?<value>.*)\""\s*\[(?<xref>.*" _
            + ")\]S*$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_IsALine = New Regex( _
            "^(?<tag>is_a):\s+(?<value>\S+)\s*\!.*$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_RelationshipLine = New Regex( _
            "^(?<tag>relationship):\s+part_of\s+(?<value>\S+)\s*\!.*$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_XRefAnalogLine = New Regex( _
            "^(?<tag>xref_analog):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
        Me.r_SubsetLine = New Regex( _
            "^(?<tag>subset):\s+(?<value>.+)$", _
            RegexOptions.IgnoreCase _
            Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace _
            Or RegexOptions.Compiled _
            )
    End Sub



End Class
