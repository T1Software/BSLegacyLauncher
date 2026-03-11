Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports UnityEngine
Imports UnityEngine.UI

Public Class HideInputs
    Inherits MonoBehaviour

    Public mainInputField As InputField

    Public Sub ToggleInputType()
        If Me.mainInputField IsNot Nothing Then
            If Me.mainInputField.contentType = InputField.ContentType.Password Then
                Me.mainInputField.contentType = InputField.ContentType.Standard
            Else
                Me.mainInputField.contentType = InputField.ContentType.Password
            End If

            Me.mainInputField.ForceLabelUpdate()
        End If
    End Sub
End Class
