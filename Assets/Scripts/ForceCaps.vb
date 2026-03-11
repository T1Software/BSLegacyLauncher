Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports UnityEngine.UI

Public Class ForceCaps
    Inherits MonoBehaviour

    Public Input As InputField

    Private Sub Update()
        Dim text As String = Input.text
        If text <> Input.text.ToUpper() Then
            Input.text = Input.text.ToUpper()
        End If
    End Sub
End Class
