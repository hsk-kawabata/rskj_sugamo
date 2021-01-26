Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFWMAIN010
    Private MainLOG As New CASTCommon.BatchLOG("KFWMAIN010", "WEB伝送データ落込処理画面")
    Private Const msgTitle As String = "WEB伝送データ落込処理画面(KFWMAIN010)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private WEB_REV_Folder As String
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private FileNames() As String '取得ファイル名

#Region " ロード"
    Private Sub KFWMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'コンボボックスを表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Get_FileName()

            'コントロール制御
            btnAction.Enabled = True
            btnEnd.Enabled = True
            cmbFileName.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 実行"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :実行ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            If FileNames Is Nothing OrElse FileNames.Length = 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '登録前確認メッセージ
            If MessageBox.Show(String.Format(MSG0015I, "登録"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            'パラメータ設定：ファイル名
            MainDB = New CASTCommon.MyOracle
            Dim jobid As String = "W010"
            Dim para As String = Path.GetFileName(FileNames(Me.cmbFileName.SelectedIndex))
            Dim iRet As Integer = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If MainLOG.InsertJOBMAST(jobid, gstrUSER_ID, para, MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "WEB伝送データ落込処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Get_FileName()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region " 終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'WEB_REVフォルダ
        WEB_REV_Folder = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV")
        If WEB_REV_Folder.ToUpper = "ERR" OrElse WEB_REV_Folder = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "WEB_REVフォルダ", "WEB_DEN", "WEB_REV"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        fn_INI_Read = True

    End Function

    Private Function Get_FileName() As Boolean
        '============================================================================
        'NAME           :Get_FileName
        'Parameter      :
        'Description    :WEB伝送データをコンボボックスに格納する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :
        'Update         :
        '============================================================================
        Try

            cmbFileName.Items.Clear()

            '対象のWEB伝送ファイル
            Dim FileList() As String = Directory.GetFiles(WEB_REV_Folder)
            If FileList.Length = 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return True
            End If
            ReDim FileNames(0)
            'コンボボックスにアイテムを追加
            For No As Integer = 0 To FileList.Length - 1

                cmbFileName.Items.Add(Path.GetFileName(FileList(No)))
                ReDim Preserve FileNames(No)
                FileNames(No) = FileList(No)
            Next
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "WEB伝送データ取得", "失敗", ex.ToString)
            Return False

        End Try
    End Function

End Class