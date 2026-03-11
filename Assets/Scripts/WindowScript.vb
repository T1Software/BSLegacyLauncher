Imports UnityEngine
Imports UnityEngine.EventSystems

Public Class WindowScript
    Inherits MonoBehaviour

    Public defaultWindowSize As Vector2Int
    Public borderSize As Vector2Int

    Private _deltaValue As Vector2 = Vector2.zero
    Private _maximized As Boolean

    Private Sub Awake()
        If Not Application.isEditor Then
            Screen.SetResolution(1280, 800, False, 120)

            If Not BorderlessWindow.framed Then
                Return
            End If

            BorderlessWindow.SetFramelessWindow()
            BorderlessWindow.MoveWindowPos(Vector2Int.zero, defaultWindowSize.x, defaultWindowSize.y)
            ' BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y)
        End If
    End Sub
End Class
