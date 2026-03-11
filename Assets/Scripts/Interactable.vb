Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports UnityEngine.UI

Public Class Interactable
    Inherits MonoBehaviour

    Public ButtonArray() As Button

    Public Sub InteractableTrue()
        For Each button As Button In ButtonArray
            button.interactable = True
        Next
    End Sub

    Public Sub InteractableFalse()
        For Each button As Button In ButtonArray
            button.interactable = False
        Next
    End Sub
End Class
