Public Class MatchInfo

    Private m_BaseReference As String
    Private m_MatchingReference As String
    'Private m_

    Property BaseReference() As String
        Get
            Return Me.m_BaseReference
        End Get
        Set(ByVal Value As String)
            Me.m_BaseReference = Value
        End Set
    End Property

    Property MatchingReference() As String
        Get
            Return Me.m_MatchingReference
        End Get
        Set(ByVal Value As String)
            Me.m_MatchingReference = Value
        End Set
    End Property

End Class
