Imports UnityEngine
Imports Yggdrasil.Logging

Namespace Assets.Scripts
    Public Class UnityConsoleLogTarget
        Inherits LoggerTarget

        Public Overrides Function GetFormat(level As LogLevel) As String
            Select Case level
                Case LogLevel.Info
                    Return ChrW(&H1B) & "^c13;[{0}][{1}][{2}]" & ChrW(&H1B) & "^r; - {3}" ' White
                Case LogLevel.Warning
                    Return ChrW(&H1B) & "^c14;[{0}][{1}][{2}]" & ChrW(&H1B) & "^r; - {3}" ' Yellow
                Case LogLevel.Error
                    Return ChrW(&H1B) & "^c12;[{0}][{1}][{2}]" & ChrW(&H1B) & "^r; - {3}" ' Red
                Case LogLevel.Debug
                    Return ChrW(&H1B) & "^c8;[{0}][{1}][{2}]" & ChrW(&H1B) & "^r; - {3}" ' Dark Gray
                Case LogLevel.Status
                    Return ChrW(&H1B) & "^c10;[{0}][{1}][{2}]" & ChrW(&H1B) & "^r; - {3}" ' Green
            End Select

            Return "[{0}] - {1}"
        End Function

        Public Overrides Sub Write(level As LogLevel, message As String, messageRaw As String, messageClean As String)
            If level = LogLevel.Info OrElse level = LogLevel.Debug Then
                Debug.Log(messageClean)
            End If
            If level = LogLevel.Error Then
                Debug.LogError(messageClean)
            End If
            If level = LogLevel.Warning Then
                Debug.LogWarning(messageClean)
            End If
        End Sub
    End Class
End Namespace
