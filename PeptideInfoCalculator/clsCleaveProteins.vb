Friend Class clsCleaveProteins

    Private m_CutSites As String
    Private m_NumMC As Integer
    Private m_MinSize As Integer
    Private m_MaxSize As Integer

    Friend Sub New( _
        ByVal CleavageResidues As String, _
        Optional ByVal AllowedMissedCleavages As Integer = 4, _
        Optional ByVal MinPeptideSize As Integer = 6, _
        Optional ByVal MaxPeptideSize As Integer = 50)

        Me.m_CutSites = CleavageResidues
        Me.m_NumMC = AllowedMissedCleavages
        Me.m_MinSize = MinPeptideSize
        Me.m_MaxSize = MaxPeptideSize



    End Sub

    Friend ReadOnly Property CleavageResidues() As String
        Get
            Return Me.m_CutSites
        End Get
    End Property

    Private Function CleaveProtein(ByVal ProteinSequence As String) As Hashtable
        'Dim p As New clsPeptideFragments
        Dim currAA As String
        Dim AAPos As Integer
        Dim seqLen As Integer = Len(ProteinSequence)
        Dim cleavagePoints As New ArrayList
        Dim peptides As New ArrayList

        Dim res As Char
        Dim seqArray As Char() = ProteinSequence.ToCharArray

        cleavagePoints.Add(0)

        For Each res In seqArray
            AAPos += 1
            currAA = res.ToString
            If InStr(1, Me.m_CutSites, currAA) > 0 Then
                cleavagePoints.Add(AAPos)

            End If
        Next

        cleavagePoints.Add(seqLen)

        'Cruise cleavagepoints for starts and stops
        Dim startPoint As Integer
        Dim endPoint As Integer

        Dim startCount As Integer
        Dim endCount As Integer

        For startCount = 1 To cleavagePoints.Count
            startPoint = CInt(cleavagePoints.Item(startCount))

            For endCount = startCount + 1 To cleavagePoints.Count
                peptides.Add(New clsPeptideFragments(startPoint, endPoint))

            Next

        Next

    End Function

End Class

Friend Class clsPeptideFragments
    Private m_Sequence As String
    Private m_StartRes As String
    Private m_EndRes As String
    Private m_Length As Integer
    Private m_StartLoc As Integer
    Private m_StopLoc As Integer

    Friend Sub New( _
        ByVal StartLocation As Integer, _
        ByVal StopLocation As Integer)

        Me.m_StartLoc = StartLocation
        Me.m_StopLoc = StopLocation

    End Sub

    Friend Property CleanSequence() As String
        Get
            Return Me.m_Sequence
        End Get
        Set(ByVal Value As String)
            Me.m_Sequence = Value
        End Set
    End Property

    Friend Property StartRes() As String
        Get
            Return Me.m_StartRes
        End Get
        Set(ByVal Value As String)
            Me.m_StartRes = Value
        End Set
    End Property

    Friend Property EndRes() As String
        Get
            Return Me.m_EndRes
        End Get
        Set(ByVal Value As String)
            Me.m_EndRes = Value
        End Set
    End Property

    Friend Property Length() As Integer
        Get
            Return Me.m_Length
        End Get
        Set(ByVal Value As Integer)
            Me.m_Length = Value
        End Set
    End Property

    Friend Property StartLoc() As Integer
        Get
            Return Me.m_StartLoc
        End Get
        Set(ByVal Value As Integer)
            Me.m_StartLoc = Value
        End Set
    End Property

    Friend Property StopLoc() As Integer
        Get
            Return Me.m_StopLoc
        End Get
        Set(ByVal Value As Integer)
            Me.m_StopLoc = Value
        End Set
    End Property
End Class