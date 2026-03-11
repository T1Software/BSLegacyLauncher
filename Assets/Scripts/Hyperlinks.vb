Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports Newtonsoft.Json
Imports UnityEngine

Public Class Person
    Public Property Name As String
    Public Property URL As String
End Class

Public Class Links
    Public Property People As List(Of Person)
End Class

Public Class Hyperlinks
    Inherits MonoBehaviour

    Private Const List As String = "https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/media/other/links.json"

    Private Shared Person As New List(Of Person)()

    Public Sub Start()
        Dim http = New HttpClient()
        Dim f = http.GetStringAsync(List).GetAwaiter().GetResult()
        Person = JsonConvert.DeserializeObject(Of List(Of Person))(f)
        http.Dispose()
    End Sub

    Private Shared Function GetUrl(input As String) As String
        Dim p = Person.FirstOrDefault(Function(x) x.Name.Equals(input))
        If p IsNot Nothing Then
            Return p.URL
        Else
            Return "https://riskivr.com/"
        End If
    End Function

    Public Sub Link_Riski()
        Application.OpenURL(GetUrl("RiskiVR"))
    End Sub

    Public Sub Link_DDAkebono()
        Application.OpenURL(GetUrl("DDAkebono"))
    End Sub

    Public Sub Link_ComputerElite()
        Application.OpenURL(GetUrl("ComputerElite"))
    End Sub
End Class
