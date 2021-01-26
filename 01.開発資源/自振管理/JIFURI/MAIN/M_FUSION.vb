Imports System
Imports System.Text
Imports System.Windows.Forms
Imports clsFUSION.clsMain
Imports CASTCommon
Imports MenteCommon

Module M_FUSION

    Public lngEXITCODE As Long
    Public clsFUSION As New clsFUSION.clsMain

    'ログインユーザ用
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    'KFJMAIN0701G.lstSCHLISTのインデックス値
    Public gintMAIN0701G_LIST_INDEX As Integer
    'KFJMAIN0101G.lstSCHLISTのインデックス値
    Public gintMAIN0101G_LIST_INDEX As Integer
    'KFJMAIN1201G.lstSCHLISTのインデックス値
    Public gintMAIN1201G_LIST_INDEX As Integer
    'KFJENTR0101G.lstKOKYAKULISTのインデックス値
    Public gintENTR0101G_LIST_INDEX As Integer

    Public gstrDB_CONNECT As String = CASTCommon.DB.CONNECT

    Public gstrFURI_DATE As String

    '--------------------------------------------------------------------------
    ' 以下、Astar 追加部 (2007年度岡崎信金仕様)
    '--------------------------------------------------------------------------

    Public GCom As MenteCommon.clsCommon

    Public GOwnerForm As Form

    Public MainForm As KFJMENU010

    ' メイン
    Sub Main()
        MainForm = New KFJMENU010

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
