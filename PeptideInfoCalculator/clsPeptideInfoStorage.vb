Friend Class clsPeptideInfoStorage
    Private m_StartLocation As Integer
    Private m_StopLocation As Integer
    Private m_Length As Integer
    Private m_ORF_ID As Integer
    Private m_MonoMass As Single
    Private m_AvgMass As Single
    Private m_Sequence As String
    Private m_StartRes As String
    Private m_EndRes As String

    Friend Property StartLocation() As Integer
        Get
            Return Me.m_StartLocation
        End Get
        Set(ByVal Value As Integer)
            Me.m_StartLocation = Value
        End Set
    End Property

    Friend Property StopLocation() As Integer
        Get
            Return Me.m_StopLocation
        End Get
        Set(ByVal Value As Integer)
            Me.m_StopLocation = Value
        End Set
    End Property

    Friend Property Length() As Integer
        Get
            Return Me.m_Length
        End Get
        Set(ByVal Value As Integer)

        End Set
    End Property

End Class