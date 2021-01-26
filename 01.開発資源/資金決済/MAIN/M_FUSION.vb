Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports CASTCommon
Imports MenteCommon

Module M_FUSION

    'ログインユーザ用
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    '--------------------------------------------------------------------------
    ' 以下、Astar 追加部 (2007年度岡崎信金仕様)
    '--------------------------------------------------------------------------

    Public GCom As MenteCommon.clsCommon

    Public GOwnerForm As Form

    Public MainForm As KFKMENU010

    ' メイン
    Sub Main()
        MainForm = New KFKMENU010

        MainForm.Show()

        Application.Run(MainForm)
    End Sub

    '
    ' 機能　 ： メインメニューを表示する
    '
    ' 備考　 ：
    '
    Public Sub ShowMain()
        MainForm.Visible = True
        MainForm.Activate()
    End Sub

    '
    ' 機能　 ： メインメニューを隠す
    '
    ' 備考　 ：
    '
    Public Sub HideMain()
        MainForm.Visible = False
    End Sub

End Module
