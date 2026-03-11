Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports UnityEngine.UI

Public Class BackButton
    Inherits MonoBehaviour

    ' Start is called before the first frame update
    Private Sub Start()
        GetComponent(Of Button)().onClick.AddListener(Sub()
            VersionButtonController.PublicVersions.SetActive(False)
            VersionButtonController.PublicPlane.SetActive(False)
            UninstallCheck.ShowLaunchOptions()
        End Sub)
    End Sub

    ' Update is called once per frame
    Private Sub Update()

    End Sub
End Class
