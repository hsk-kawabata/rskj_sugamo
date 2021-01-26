Module M_FUSION
    Public GCom As MenteCommon.clsCommon
    'ログインユーザ用
    Public gstrUSER_ID As String

    'ディレクトリ変数
    Public gstrEXE_OPENDIR As String
    Public gstrGAKKOU_EXE_OPENDIR As String '学校自振EXE指定場所

    Public GOwnerForm As Form

    '
    ' 機能　 ： 画面右上ラベル位置設定
    '
    ' 引数　 ： ARG1 - ログイン名 ：    (Label)
    ' 　　　 　 ARG2 - システム日付 ：  (Label)
    ' 　　　 　 ARG3 - ユーザＩＤ       (Label)
    ' 　　　 　 ARG4 - システム日付     (Label)
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Public Sub SetMonitorTopArea(ByVal Label2 As Label, ByVal Label3 As Label, _
                        ByVal lblUser As Label, ByVal lblDate As Label)
        Try
            Label2.Location = New Point(580, 8)
            Label3.Location = New Point(580, 28)
            lblUser.Location = New Point(665, 8)
            lblDate.Location = New Point(665, 28)
            lblUser.Text = gstrUSER_ID
            lblDate.Text = String.Format("{0:yyyy年MM月dd日}", Date.Now)
        Catch ex As Exception
            lblUser.Text = "SYSTEM ERROR"
            lblDate.Text = "SYSTEM ERROR"
        End Try
    End Sub

End Module
