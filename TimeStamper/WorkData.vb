Imports System.Runtime.Serialization
Imports Newtonsoft.Json

Public Class WorkDay
    <JsonProperty("date")>
    Public Property CurrentDate As Date
    <JsonProperty("time_stamps")>
    Public Property TimeStamps As New List(Of TimeStamp)
    Public Class TimeStamp
        Public Sub New(Type As TimeStampType, Time As TimeSpan)
            Me.Type = Type
            Me.Time = Time
        End Sub
        Public Sub New()

        End Sub
        Public Enum TimeStampType
            Start
            [Stop]
        End Enum
        <JsonProperty("type")>
        Public Property Type As TimeStampType
        <JsonProperty("time")>
        Public Property Time As TimeSpan
    End Class
End Class