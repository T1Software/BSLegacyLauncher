Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports UnityEngine
Imports UnityEngine.UI

Namespace Assets.Scripts
    Class SteamCodePopup
        Inherits MonoBehaviour

        Public Description As TextMesh
        Public codeField As InputField
        Public EnterButton As Button

        <HideInInspector>
        Public callback As Action(Of String)

        Public Sub Start()
            codeField.onEndEdit.AddListener(Sub(value)
                If Input.GetKey(KeyCode.Return) Then EnterPressed()
            End Sub)
        End Sub

        Public Sub EnterPressed()
            callback.Invoke(codeField.text)
            Destroy(gameObject)
        End Sub
    End Class
End Namespace
