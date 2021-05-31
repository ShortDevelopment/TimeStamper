Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Public Class Form1

#Region "Desgin"
    Public ReadOnly Property RedColor As Color = Color.FromArgb(229, 57, 53)
    Public ReadOnly Property GreenColor As Color = Color.FromArgb(67, 160, 71)
    Public ReadOnly Property DiasbledColor As Color = Color.FromArgb(200, 200, 200)

    Public Sub EnableStart()
        Dim StopButton = Button2
        StopButton.Enabled = False
        StopButton.ForeColor = DiasbledColor

        Dim StartButton = Button1
        StartButton.Enabled = True
        StartButton.ForeColor = GreenColor
    End Sub

    Public Sub EnableStop()
        Dim StartButton = Button1
        StartButton.Enabled = False
        StartButton.ForeColor = DiasbledColor

        Dim StopButton = Button2
        StopButton.Enabled = True
        StopButton.ForeColor = RedColor
    End Sub
#End Region

#Region "Save"
    'Dim SavePath As String = Path.Combine(Application.StartupPath, "work_time.json")
    Public ReadOnly Property SavePath As String
        Get
            Dim monday = DateTime.Today.AddDays(-CInt(If(DateTime.Today.DayOfWeek = DayOfWeek.Sunday, 7, DateTime.Today.DayOfWeek)) + CInt(DayOfWeek.Monday))
            Dim dir = Path.Combine(Application.StartupPath, monday.Year.ToString())
            If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            dir = Path.Combine(dir, monday.ToString("MM MMMM"))
            If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            Return Path.Combine(dir, $"Woche {monday.ToString("dd")}. {monday.ToString("MMMM")}.json")
        End Get
    End Property

    Private Sub Save() Handles MyBase.FormClosing
        File.WriteAllText(SavePath, JsonConvert.SerializeObject(Data, JsonSettings))
    End Sub
#End Region

    Dim Data As New List(Of WorkDay)
    Dim JsonSettings As New JsonSerializerSettings()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        JsonSettings.Converters.Add(New StringEnumConverter())

        If File.Exists(SavePath) Then
            Data = JsonConvert.DeserializeObject(Of List(Of WorkDay))(File.ReadAllText(SavePath), JsonSettings)
        End If

        If Not Data.Count = 0 AndAlso Data.Last().TimeStamps.Last().Type = WorkDay.TimeStamp.TimeStampType.Start Then EnableStop()
    End Sub



#Region "Button Events"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim CurrentDay As WorkDay = Data.Where(Function(x) x.CurrentDate = Date.Today)(0)
        If CurrentDay Is Nothing Then
            CurrentDay = New WorkDay With {
                .CurrentDate = Date.Today
            }
            Data.Add(CurrentDay)
        End If
        CurrentDay.TimeStamps.Add(New WorkDay.TimeStamp(WorkDay.TimeStamp.TimeStampType.Start, New TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
        Save()

        EnableStop()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim CurrentDay As WorkDay = Data.Where(Function(x) x.CurrentDate = Date.Today)(0)
        If CurrentDay Is Nothing Then
            CurrentDay = New WorkDay With {
                .CurrentDate = Date.Today
            }
            Data.Add(CurrentDay)
        End If
        CurrentDay.TimeStamps.Add(New WorkDay.TimeStamp(WorkDay.TimeStamp.TimeStampType.Stop, New TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
        Save()

        EnableStart()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim opf As New OpenFileDialog()
        opf.Filter = "JSON|*.json"
        If opf.ShowDialog() = DialogResult.OK Then
            Try
                Dim Data = JsonConvert.DeserializeObject(Of List(Of WorkDay))(File.ReadAllText(opf.FileName), JsonSettings)
                Dim TimeGes As TimeSpan
                For Each day In Data
                    For i As Integer = 0 To day.TimeStamps.Count - 1
                        Dim timestamp = day.TimeStamps(i)
                        If timestamp.Type = WorkDay.TimeStamp.TimeStampType.Start Then
                            If i + 1 = day.TimeStamps.Count Then
                                TimeGes += New TimeSpan(24, 0, 0) - timestamp.Time
                            Else
                                Dim StopTimeStamp = day.TimeStamps(i + 1)
                                If StopTimeStamp.Type = WorkDay.TimeStamp.TimeStampType.Start Then Throw New Exception($"Fehler in der Reihenfolge von Start und Stopp! (Tag: {day.CurrentDate.ToString()}, Index: {i}")
                                TimeGes += StopTimeStamp.Time - timestamp.Time
                            End If
                        ElseIf i = 0 Then
                            TimeGes += timestamp.Time
                        End If
                    Next
                Next
                Label1.Text = TimeGes.ToString()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical)
            End Try
        End If
    End Sub
#End Region

End Class
