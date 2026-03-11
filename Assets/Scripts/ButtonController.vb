Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class ButtonController
    Inherits MonoBehaviour

    Public target As Single = 0
    Public speed As Single = 1

    Public Sub [set](movement As Integer, yearCount As Integer)
        Dim buttonWidth As Single = 1.43F
        target = buttonWidth * movement - (yearCount * buttonWidth / 2) + 0.7125F
    End Sub

    Private Sub Update()
        If Mathf.Abs(target - transform.position.x) > speed * Time.deltaTime Then
            transform.position = New Vector3(transform.position.x + speed * Time.deltaTime * Mathf.Sign(target - transform.position.x), transform.position.y)
        Else
            transform.position = New Vector3(target, transform.position.y)
        End If
    End Sub
End Class
