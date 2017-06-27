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
            Return m_ID
        End Get
        Set(Value As String)
            m_ID = CleanUpLine(Value)
        End Set
    End Property
    Property Name() As String
        Get
            Return m_Name
        End Get
        Set(Value As String)
            m_Name = CleanUpLine(Value)
        End Set
    End Property
    Property [NameSpace]() As String
        Get
            Return m_Namespace
        End Get
        Set(Value As String)
            m_Namespace = CleanUpLine(Value)
        End Set
    End Property
    Property Definition() As String
        Get
            Return m_Definition
        End Get
        Set(Value As String)
            m_Definition = CleanUpLine(Value)
        End Set
    End Property
    Property Comment() As String
        Get
            Return m_Comment
        End Get
        Set(Value As String)
            m_Comment = CleanUpLine(Value)
        End Set
    End Property
    Property IsObsolete() As Boolean
        Get
            Return m_IsObsolete
        End Get
        Set(Value As Boolean)
            m_IsObsolete = Value
        End Set
    End Property
    ReadOnly Property ExactSynonym_List() As ArrayList
        Get
            Return m_ExactSynonym
        End Get
    End Property
    ReadOnly Property IsA_List() As ArrayList
        Get
            Return m_IsA
        End Get
    End Property
    ReadOnly Property XRefAnalog_List() As ArrayList
        Get
            Return m_XRefAnalog
        End Get
    End Property
    ReadOnly Property Relationship() As ArrayList
        Get
            Return m_Relationship
        End Get
    End Property
    ReadOnly Property SubSet() As ArrayList
        Get
            Return m_SubSet
        End Get
    End Property


    Sub Add_ExactSynonym_Entry(synonym As String)
        m_ExactSynonym.Add(CleanUpLine(synonym))
    End Sub
    Sub Add_IsA_Entry(IsAReference As String)
        m_IsA.Add(IsAReference)
    End Sub
    Sub Add_XRefAnalog_Entry(XRef As String)
        m_XRefAnalog.Add(XRef)
    End Sub
    Sub Add_RelationShip_Entry(Relationship As String)
        m_Relationship.Add(Relationship)
    End Sub
    Sub Add_Subset_Entry(Subset As String)
        m_SubSet.Add(Subset)
    End Sub

    Private Function CleanUpLine(entryLine As String) As String
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
    Sub New(GOEntryText As System.Collections.Specialized.StringCollection)

    End Sub

    Sub New()

    End Sub

    Protected Sub ProcessEntry(EntryCollection As System.Collections.Specialized.StringCollection)

    End Sub

    Private Sub SetupRegexes()

        Dim reOptions = RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled

        r_entryHeader = New Regex(
            "^\[Term\]$",
            reOptions)
        r_IDLine = New Regex(
            "^(?<tag>id):\s+(?<value>.+)$",
            reOptions
            )
        r_NameLine = New Regex(
            "^(?<tag>name):\s+(?<value>.+)$",
            reOptions
            )
        r_NameSpaceLine = New Regex(
            "^(?<tag>namespace):\s+(?<value>.+)$",
            reOptions
            )
        r_DefinitionLine = New Regex(
            "^(?<tag>def):\s+\""+(?<value>.*)\""\s*\[*(?<xref>.*)\]*\s*",
            reOptions
            )
        r_CommentLine = New Regex(
            "^(?<tag>comment):\s+(?<value>.+)$",
           reOptions
            )
        r_IsObsoleteLine = New Regex(
            "^(?<tag>is_obsolete):\s+(?<value>true|false)$",
            reOptions
            )
        r_ExactSynonymLine = New Regex(
            "^(?<tag>exact_synonym):\s+\""+(?<value>.*)\""\s*\[(?<xref>.*)\]S*$",
            reOptions
            )
        r_IsALine = New Regex(
            "^(?<tag>is_a):\s+(?<value>\S+)\s*\!.*$",
            reOptions
            )
        r_RelationshipLine = New Regex(
            "^(?<tag>relationship):\s+part_of\s+(?<value>\S+)\s*\!.*$",
            reOptions
            )
        r_XRefAnalogLine = New Regex(
            "^(?<tag>xref_analog):\s+(?<value>.+)$",
            reOptions
            )
        r_SubsetLine = New Regex(
            "^(?<tag>subset):\s+(?<value>.+)$",
            reOptions
            )
    End Sub



End Class
