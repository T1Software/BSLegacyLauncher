Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class FocusMute
    Inherits MonoBehaviour

    Private Sub Update()
        If Application.isFocused Then
            GetComponent(Of AudioSource)().mute = (Not Settings.vars.ambient)
        Else
            GetComponent(Of AudioSource)().mute = True
        End If
    End Sub
End Class
