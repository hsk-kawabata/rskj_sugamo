Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' 入庫情報一覧画面
''' </summary>
''' <remarks>2017/11/27 標準版 added for 照合対応(MASTPTN2用)</remarks>
Public Class KFJMAIN122

#Region "クラス定数"
    Private Const msgTitle As String = "入庫情報一覧画面(KFJMAIN122)"
    Private CAST As New CASTCommon.Events

#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN122", "入庫情報一覧画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    '送付状実績ＴＢＬキー項目保持
    Private Structure RecordInformation
        Dim ListView As ListView
        Dim psFSYORI_KBN As String        '振替処理区分
        Dim psTORIS_CODE As String        '取引先主
        Dim psTORIF_CODE As String        '取引先副
        Dim psENTRY_DATE As String        '送付状入力日
        Dim psBAITAI_KANRI_CODE As String '媒体管理コード　搬入ケース番号
        Dim psITAKU_CODE As String        '委託者コード
        Dim psSTATION_ENTRY_NO As String        '端末番号
        Dim psSYUBETU_CODE As String      '種別コード
        Dim psFURI_DATE As String         '振替日
        Dim psBAITAI_CODE As String       '媒体コード
        Dim psENTRY_NO As Integer         '通番
        Dim psSYORI_KEN As Long           '件数
        Dim psSYORI_KIN As Long           '金額
        Dim psDELETE_FLG As Integer       '削除ＦＬＧ
        Dim psFORCE_FLG As Integer        '強制登録ＦＬＧ
        Dim psUPLOAD_DATE As String       'アップロード日付
        Dim psUPLOAD_FLG As Integer       'アップロードＦＬＧ
        Dim psCHECK_FLG As Integer        '照合FLG
        Dim psENTRY_OP As String          '担当者ＩＤ(送付状入力）
        Dim psUPDATE_OP As String         '担当者ＩＤ（送付状変更）
        Dim psDELETE_OP As String         '担当者ＩＤ（送付状削除）
        Dim psNewFuriDate As String       '変更後振替日
        Dim psITAKUNNAME As String        '委託者名

        Dim psIN_DATE As String           '媒体入庫日
        Dim psSTATION_IN_NO As String     '入庫端末番号
        Dim pnIN_COUNTER As Integer       '受付通番
    End Structure
    Private RecInf As RecordInformation

    'ListView Control
    Public WriteOnly Property SetListView() As ListView
        Set(ByVal Value As ListView)
            RecInf.ListView = Value
        End Set
    End Property

    Public Property SELECT_DATE() As String
        Get
            Return strSELECT_DATE
        End Get
        Set(ByVal value As String)
            strSELECT_DATE = value
        End Set
    End Property
    Private strSELECT_DATE As String

    '初期稼動画面
    Private MyOwnerForm As Form
    'ソートオーダーフラグ
    Private SortOrderFlag As Boolean = True

    'クリックした列の番号
    Private ClickedColumn As Integer
    Private Const ThisModuleName As String = "KFJUKT0220G.vb"

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFJMAIN122_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            'ログの書込に必要な情報の取得
            '--------------------------------------------------
            Me.LW.UserID = GCom.GetUserID
            Me.LW.ToriCode = "000000000000"
            Me.LW.FuriDate = "00000000"

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)開始", "成功", "")

            '--------------------------------------------------
            'システム日付とユーザ名を表示
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label2, Me.Label3, Me.lblUser, Me.lblDate)

            'フォームの初期化
            Call FormInitializa()     '画面初期化
            Call FormDataLoad()       '初期画面データロード

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 変更ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnHenkou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHenkou.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(変更)開始", "成功", "")

            '--------------------------------------------------
            '選択チェック
            '--------------------------------------------------
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '削除されていたらメッセージ表示
            If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "削" Then
                MessageBox.Show(MSG0385W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '--------------------------------------------------
            '送付状入力画面に遷移
            '--------------------------------------------------
            Dim KFJMAIN121 As New KFJMAIN121
            With KFJMAIN121
                .SetListView = Me.ListView1
                .SetpsFSYORI_KBN = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 0))
                .SetpsTORIS_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 1))
                .SetpsTORIF_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 2))
                .SetpsENTRY_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 18))
                .SetpsBAITAI_KANRI_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 5))
                .SetpsITAKU_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 6))
                .SetpsSTATION_ENTRY_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 3))
                .SetpsSYUBETU_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 8))
                .SetpsFURI_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9))
                .SetpsBAITAI_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 13))
                .SetpsENTRY_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 4))
                .SetpsSYORI_KEN = GCom.NzLong(GCom.SelectedItem(Me.ListView1, 10))
                .SetpsSYORI_KIN = GCom.NzLong(GCom.SelectedItem(Me.ListView1, 11))
                If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "削" Then
                    .SetpsDELETE_FLG = 1
                Else
                    .SetpsDELETE_FLG = 0
                End If
                .SetpsFORCE_FLG = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 14))
                .SetpsUPLOAD_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 15))
                .SetpsUPLOAD_FLG = GCom.NzInt(GCom.SelectedItem(Me.ListView1, 16))
                .SetpsCHECK_FLG = GCom.NzInt(GCom.SelectedItem(Me.ListView1, 17))
                .SetpsENTRY_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 19))
                .SetpsUPDATE_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 20))
                .SetpsDELETE_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 21))
                .SetpsIN_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 24))
                .SetpsSTATION_IN_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 25))
                .SetpsIN_COUNTER = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 26))
                .SetpnHENKOU_SW = 1

            End With

            CASTCommon.ShowFORM(GCom.GetUserID, CType(KFJMAIN121, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(変更)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(変更)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 削除ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(削除)開始", "成功", "")

            '--------------------------------------------------
            '選択チェック
            '--------------------------------------------------
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '--------------------------------------------------
            'メッセージ設定
            '--------------------------------------------------
            Dim MSG As String
            Dim bDeleteFlg As Boolean = False
            If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "削" Then
                MSG = String.Format(MSG0015I, "削除取消")
                bDeleteFlg = False
            Else
                MSG = String.Format(MSG0015I, "削除")
                bDeleteFlg = True
            End If
            If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '--------------------------------------------------
            '削除／削除取消処理
            '--------------------------------------------------
            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            Dim RecordCount As Integer = 0
            Dim RecordCountIn As Integer = 0

            SQL.Length = 0
            SQL.Append("SELECT COUNT(*) COUNT_A")
            SQL.Append(" FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE BAITAI_KANRI_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 5)).Trim & "'")
            SQL.Append(" AND FURI_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9)).Trim & "'")
            SQL.Append(" AND DELETE_FLG_ME = '0'")
            SQL.Append(" AND UPLOAD_FLG_ME = '0'")

            If OraReader.DataReader(SQL) = True Then
                RecordCount = OraReader.GetInt("COUNT_A")
            Else
                RecordCount = 0
            End If

            OraReader.Close()

            SQL.Length = 0
            SQL.Append("UPDATE MEDIA_ENTRY_TBL SET")
            If bDeleteFlg = True Then
                SQL.Append(" DELETE_FLG_ME = '1'")
            Else
                SQL.Append(" DELETE_FLG_ME = '0'")
            End If
            SQL.Append(",DELETE_OP_ME = '" & GCom.GetUserID & "'")
            SQL.Append(",UPDATE_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 23)).Trim & "'")
            SQL.Append(" WHERE ITAKU_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 6)).Trim & "'")
            SQL.Append(" AND SYUBETU_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 8)).Trim & "'")
            SQL.Append(" AND FURI_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9)).Trim & "'")
            SQL.Append(" AND STATION_ENTRY_NO_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 3)).Trim & "'")
            SQL.Append(" AND BAITAI_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 13)).Trim & "'")
            SQL.Append(" AND ENTRY_NO_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 4)).Trim & "'")

            Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
            If iRet < 0 Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "送付状実績マスタ更新", "失敗", Me.MainDB.Message)
                Me.MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            RecordCountIn = 0
            RecordCount = 0

            Me.MainDB.Commit()

            '2018/05/25 タスク）西野 ADD 標準版修正：広島信金対応（削除/削除取消時の画面反映）------------------------ START
            Dim TEMP As String = IIf(bDeleteFlg, "削", "")
            Call GCom.SelectedItem(Me.ListView1, 12, TEMP)
            '2018/05/25 タスク）西野 ADD 標準版修正：広島信金対応（削除/削除取消時の画面反映）------------------------ END

            If bDeleteFlg = True Then
                MSG = String.Format(MSG0016I, "削除")
            Else
                MSG = String.Format(MSG0016I, "削除取消")
            End If
            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(削除)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' リストビューカラムクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
        With CType(sender, ListView)

            If ClickedColumn = e.Column Then
                ' 同じ列をクリックした場合は，逆順にする 
                SortOrderFlag = Not SortOrderFlag
            End If

            ' 列番号設定
            ClickedColumn = e.Column

            ' 列水平方向配置
            Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

            ' ソート
            .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

            ' ソート実行
            .Sort()
        End With
    End Sub

    ''' <summary>
    ''' リストビューダブルクリックイベント（検証用ＣＳＶ出力）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(ListView1)
    End Sub

#End Region


#Region "プライベートメソッド"

    ''' <summary>
    ''' フォーム上に該当データ表示
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FormDataLoad()
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Me.MainDB = New CASTCommon.MyOracle

        Try
            Me.ListView1.Items.Clear()

            SQL.Append("SELECT E.FSYORI_KBN_ME,")
            SQL.Append(" E.TORIS_CODE_ME,")
            SQL.Append(" E.TORIF_CODE_ME,")
            SQL.Append(" E.ENTRY_DATE_ME,")
            SQL.Append(" E.FURI_DATE_ME,")
            SQL.Append(" E.ITAKU_KANRI_CODE_ME,")
            SQL.Append(" E.ITAKU_CODE_ME,")
            SQL.Append(" E.STATION_ENTRY_NO_ME,")
            SQL.Append(" E.SYUBETU_CODE_ME,")
            SQL.Append(" E.BAITAI_CODE_ME,")
            SQL.Append(" E.ENTRY_NO_ME,")
            SQL.Append(" E.SYORI_KEN_ME,")
            SQL.Append(" E.SYORI_KIN_ME,")
            SQL.Append(" E.DELETE_FLG_ME,")
            SQL.Append(" E.FORCE_FLG_ME,")
            SQL.Append(" E.UPLOAD_DATE_ME,")
            SQL.Append(" E.UPDATE_OP_ME,")
            SQL.Append(" E.DELETE_OP_ME,")
            SQL.Append(" E.CREATE_DATE_ME,")
            SQL.Append(" E.UPDATE_DATE_ME,")
            SQL.Append(" E.IN_DATE_ME,")
            SQL.Append(" E.STATION_IN_NO_ME,")
            SQL.Append(" E.IN_COUNTER_ME,")
            SQL.Append(" NVL(E.UPLOAD_FLG_ME, 0) UPLOAD_FLG_ME,")
            SQL.Append(" E.CHECK_FLG_ME,")
            SQL.Append(" E.ENTRY_OP_ME ,")
            SQL.Append(" G.ITAKU_NNAME_T")
            SQL.Append(" FROM MEDIA_ENTRY_TBL E,")
            SQL.Append(" (SELECT FSYORI_KBN_T,")
            SQL.Append(" TORIS_CODE_T,")
            SQL.Append(" TORIF_CODE_T,")
            SQL.Append(" ITAKU_NNAME_T")
            SQL.Append(" FROM TORIMAST) G")
            SQL.Append(" WHERE E.FSYORI_KBN_ME = G.FSYORI_KBN_T")
            SQL.Append(" AND E.TORIS_CODE_ME = G.TORIS_CODE_T")
            SQL.Append(" AND E.TORIF_CODE_ME = G.TORIF_CODE_T")
            SQL.Append(" AND E.ENTRY_DATE_ME = '" & Me.SELECT_DATE & "'")
            SQL.Append(" ORDER BY E.STATION_ENTRY_NO_ME ASC")
            SQL.Append(" , E.ENTRY_NO_ME ASC")

            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    Dim LineData(26) As String                 '送付状実績ＴＢＬワーク

                    LineData(7) = GCom.GetLimitString(GCom.NzStr(OraReader.GetItem("ITAKU_NNAME_T")).Trim, 20) '委託者名

                    LineData(0) = GCom.NzStr(OraReader.GetItem("FSYORI_KBN_ME"))
                    LineData(1) = GCom.NzStr(OraReader.GetItem("TORIS_CODE_ME"))
                    LineData(2) = GCom.NzStr(OraReader.GetItem("TORIF_CODE_ME"))
                    LineData(4) = GCom.NzStr(OraReader.GetItem("ENTRY_NO_ME"))              '通番
                    LineData(5) = GCom.NzStr(OraReader.GetItem("ITAKU_KANRI_CODE_ME"))      '搬送ケースコード
                    LineData(6) = GCom.NzStr(OraReader.GetItem("ITAKU_CODE_ME"))            '委託者コード
                    LineData(8) = GCom.NzStr(OraReader.GetItem("SYUBETU_CODE_ME"))          '種別
                    LineData(9) = GCom.NzStr(OraReader.GetItem("FURI_DATE_ME"))             '振替日

                    LineData(10) = String.Format("{0:#,##0}", GCom.NzLong(OraReader.GetItem("SYORI_KEN_ME"))) '件数
                    LineData(11) = String.Format("{0:#,##0}", GCom.NzLong(OraReader.GetItem("SYORI_KIN_ME"))) '金額

                    If GCom.NzDec(OraReader.GetItem("DELETE_FLG_ME")) = 1 Then              '削除
                        LineData(12) = "削"
                    Else
                        LineData(12) = ""
                    End If
                    LineData(3) = GCom.NzStr(OraReader.GetItem("STATION_ENTRY_NO_ME"))      '端末番号
                    LineData(13) = GCom.NzStr(OraReader.GetItem("BAITAI_CODE_ME"))          '媒体コード

                    LineData(14) = GCom.NzStr(OraReader.GetItem("FORCE_FLG_ME"))            '強制登録
                    LineData(15) = GCom.NzStr(OraReader.GetItem("UPLOAD_DATE_ME"))          'アップロード
                    LineData(16) = GCom.NzStr(OraReader.GetItem("UPLOAD_FLG_ME"))           'アップロード
                    LineData(17) = GCom.NzStr(OraReader.GetItem("CHECK_FLG_ME"))            '照合
                    LineData(18) = GCom.NzStr(OraReader.GetItem("ENTRY_DATE_ME"))           '送付状入力日
                    LineData(19) = GCom.NzStr(OraReader.GetItem("ENTRY_OP_ME"))             'ID(入力)
                    LineData(20) = GCom.NzStr(OraReader.GetItem("UPDATE_OP_ME"))            'ID(変更）
                    LineData(21) = GCom.NzStr(OraReader.GetItem("DELETE_OP_ME"))            'ID(削除）
                    LineData(22) = GCom.NzStr(OraReader.GetItem("CREATE_DATE_ME"))          '作成日
                    LineData(23) = GCom.NzStr(OraReader.GetItem("UPDATE_DATE_ME"))          '更新日

                    LineData(24) = GCom.NzStr(OraReader.GetItem("IN_DATE_ME"))              '入庫日
                    LineData(25) = GCom.NzStr(OraReader.GetItem("STATION_IN_NO_ME"))        '入庫端末番号
                    LineData(26) = GCom.NzStr(OraReader.GetItem("IN_COUNTER_ME"))           '受付通番

                    Dim LstItem As New ListViewItem(LineData)
                    ListView1.Items.AddRange(New ListViewItem() {LstItem})


                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "該当データ取得", "失敗", ex.Message)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Sub

    ''' <summary>
    ''' フォーム上の値を初期化
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FormInitializa()

        With ListView1
            .Clear()
            .Columns.Add("振替処理区分", 0, HorizontalAlignment.Center)
            .Columns.Add("取引先主", 0, HorizontalAlignment.Center)
            .Columns.Add("取引先副", 0, HorizontalAlignment.Center)
            .Columns.Add("端末番号", 20, HorizontalAlignment.Center)
            .Columns.Add("通番", 45, HorizontalAlignment.Right)
            .Columns.Add("搬送ケース番号", 130, HorizontalAlignment.Center)
            .Columns.Add("委託者CD", 100, HorizontalAlignment.Center)
            .Columns.Add("委託者名", 120, HorizontalAlignment.Left)
            .Columns.Add("種別", 50, HorizontalAlignment.Center)
            .Columns.Add("振替日", 80, HorizontalAlignment.Center)
            .Columns.Add("件数", 70, HorizontalAlignment.Right)
            .Columns.Add("金額", 120, HorizontalAlignment.Right)
            .Columns.Add("削", 40, HorizontalAlignment.Center)
            .Columns.Add("媒体コード", 0, HorizontalAlignment.Center)
            .Columns.Add("強制登録", 0, HorizontalAlignment.Center)
            .Columns.Add("ｱｯﾌﾟﾛｰﾄﾞ日", 0, HorizontalAlignment.Center)
            .Columns.Add("ｱｯﾌﾟﾛｰﾄﾞ", 0, HorizontalAlignment.Center)
            .Columns.Add("照合FLG", 0, HorizontalAlignment.Center)
            .Columns.Add("送付状入力日", 0, HorizontalAlignment.Center)
            .Columns.Add("ＩＤ（入力）", 0, HorizontalAlignment.Center)
            .Columns.Add("ＩＤ（変更）", 0, HorizontalAlignment.Center)
            .Columns.Add("ＩＤ（削除）", 0, HorizontalAlignment.Center)
            .Columns.Add("作成日", 0, HorizontalAlignment.Center)
            .Columns.Add("更新日", 0, HorizontalAlignment.Center)
            .Columns.Add("入庫日", 0, HorizontalAlignment.Center)
            .Columns.Add("端末入庫", 0, HorizontalAlignment.Center)
            .Columns.Add("受付通番", 0, HorizontalAlignment.Center)
        End With
    End Sub

#End Region

End Class
