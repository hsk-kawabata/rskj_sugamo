Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' 送付状入力画面
''' </summary>
''' <remarks></remarks>
Public Class KFSMAIN141

#Region "クラス定数"
    Private Const msgTitle As String = "送付状入力画面(KFSMAIN141)"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const JYOUKEN As String = " AND SYOUGOU_KBN_T = '1'"    '照合要の先のみ対象
#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN141", "送付状入力画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    Private Structure TORIMAST_INF
        Dim FSYORI_KBN As String
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim BAITAI_CODE As String
        Dim BAITAI_KANRI_CODE As String
        Dim SYUBETU_CODE As String
        Dim ITAKU_CODE As String
        Dim ITAKU_NNAME As String
    End Structure
    Private TR As TORIMAST_INF

    '送付状実績ＴＢＬキー項目保持
    Private Structure RecordInformation
        Dim ListView As ListView
        Dim psFSYORI_KBN As String        '振替処理区分
        Dim psTORIS_CODE As String        '取引先主
        Dim psTORIF_CODE As String        '取引先副
        Dim psENTRY_DATE As String        '送付状入力日
        Dim psBAITAI_KANRI_CODE As String '媒体管理コード　搬入ケース番号
        Dim psITAKU_CODE As String        '委託者コード
        Dim psSTATION_ENTRY_NO As String  '端末番号
        Dim psSYUBETU_CODE As String      '種別コード
        Dim psFURI_DATE As String         '振込日
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
        Dim psNewFuriDate As String       '変更後振込日
        Dim psITAKUNNAME As String        '委託者名
        Dim psIN_DATE As String           '媒体入庫日
        Dim psSTATION_IN_NO As String     '入庫端末番号
        Dim pnIN_COUNTER As Integer       '受付通番
        Dim pnHENKOU_SW As Integer        '変更有無
    End Structure
    Private RecInf As RecordInformation

    'ListView Control
    Public WriteOnly Property SetListView() As ListView
        Set(ByVal Value As ListView)
            RecInf.ListView = Value
        End Set
    End Property

    '振替処理区分
    Public WriteOnly Property SetpsFSYORI_KBN() As String
        Set(ByVal Value As String)
            RecInf.psFSYORI_KBN = Value
        End Set
    End Property

    '取引先主
    Public WriteOnly Property SetpsTORIS_CODE() As String
        Set(ByVal Value As String)
            RecInf.psTORIS_CODE = Value
        End Set
    End Property

    '取引先副
    Public WriteOnly Property SetpsTORIF_CODE() As String
        Set(ByVal Value As String)
            RecInf.psTORIF_CODE = Value
        End Set
    End Property

    '送付状入力日
    Public WriteOnly Property SetpsENTRY_DATE() As String
        Set(ByVal Value As String)
            RecInf.psENTRY_DATE = Value
        End Set
    End Property

    '媒体管理コード　搬入ケース番号
    Public WriteOnly Property SetpsBAITAI_KANRI_CODE() As String
        Set(ByVal Value As String)
            RecInf.psBAITAI_KANRI_CODE = Value
        End Set
    End Property

    '委託者コード
    Public WriteOnly Property SetpsITAKU_CODE() As String
        Set(ByVal Value As String)
            RecInf.psITAKU_CODE = Value
        End Set
    End Property

    '端末番号
    Public WriteOnly Property SetpsSTATION_ENTRY_NO() As String
        Set(ByVal Value As String)
            RecInf.psSTATION_ENTRY_NO = Value
        End Set
    End Property

    '種別コード
    Public WriteOnly Property SetpsSYUBETU_CODE() As String
        Set(ByVal Value As String)
            RecInf.psSYUBETU_CODE = Value
        End Set
    End Property

    '振込日
    Public WriteOnly Property SetpsFURI_DATE() As String
        Set(ByVal Value As String)
            RecInf.psFURI_DATE = Value
        End Set
    End Property

    '媒体コード
    Public WriteOnly Property SetpsBAITAI_CODE() As String
        Set(ByVal Value As String)
            RecInf.psBAITAI_CODE = Value
        End Set
    End Property

    '通番
    Public WriteOnly Property SetpsENTRY_NO() As Integer
        Set(ByVal Value As Integer)
            RecInf.psENTRY_NO = Value
        End Set
    End Property

    '件数
    Public WriteOnly Property SetpsSYORI_KEN() As Long
        Set(ByVal Value As Long)
            RecInf.psSYORI_KEN = Value
        End Set
    End Property

    '金額
    Public WriteOnly Property SetpsSYORI_KIN() As Long
        Set(ByVal Value As Long)
            RecInf.psSYORI_KIN = Value
        End Set
    End Property

    '削除ＦＬＧ
    Public WriteOnly Property SetpsDELETE_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psDELETE_FLG = Value
        End Set
    End Property

    '強制登録ＦＬＧ
    Public WriteOnly Property SetpsFORCE_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psFORCE_FLG = Value
        End Set
    End Property

    'アップロード日
    Public WriteOnly Property SetpsUPLOAD_DATE() As String
        Set(ByVal Value As String)
            RecInf.psUPLOAD_DATE = Value
        End Set
    End Property

    'アップロードＦＬＧ
    Public WriteOnly Property SetpsUPLOAD_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psUPLOAD_FLG = Value
        End Set
    End Property

    '照合ＦＬＧ
    Public WriteOnly Property SetpsCHECK_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psCHECK_FLG = Value
        End Set
    End Property

    '担当者ＩＤ(送付状新規入力）
    Public WriteOnly Property SetpsENTRY_OP() As String
        Set(ByVal Value As String)
            RecInf.psENTRY_OP = Value
        End Set
    End Property

    '担当者ＩＤ(送付状変更入力）
    Public WriteOnly Property SetpsUPDATE_OP() As String
        Set(ByVal Value As String)
            RecInf.psUPDATE_OP = Value
        End Set
    End Property

    '担当者ＩＤ(送付状削除）
    Public WriteOnly Property SetpsDELETE_OP() As String
        Set(ByVal Value As String)
            RecInf.psDELETE_OP = Value
        End Set
    End Property

    'NEW振込日
    Public WriteOnly Property SetpsNewFuriDate() As String
        Set(ByVal Value As String)
            RecInf.psNewFuriDate = Value
        End Set
    End Property

    '委託者名
    Public WriteOnly Property SetpsITAKUNNAME() As String
        Set(ByVal Value As String)
            RecInf.psITAKUNNAME = Value
        End Set
    End Property

    '媒体入庫日
    Public WriteOnly Property SetpsIN_DATE() As String
        Set(ByVal Value As String)
            RecInf.psIN_DATE = Value
        End Set
    End Property

    '入庫端末番号
    Public WriteOnly Property SetpsSTATION_IN_NO() As String
        Set(ByVal Value As String)
            RecInf.psSTATION_IN_NO = Value
        End Set
    End Property

    '受付通番
    Public WriteOnly Property SetpsIN_COUNTER() As Integer
        Set(ByVal Value As Integer)
            RecInf.pnIN_COUNTER = Value
        End Set
    End Property

    '変更有無
    Public WriteOnly Property SetpnHENKOU_SW() As Integer
        Set(ByVal Value As Integer)
            RecInf.pnHENKOU_SW = Value
        End Set
    End Property

    Public nHeadKoumokuCnt As Integer = 0
    Private nKyouseitouroku As Integer               '強制登録ＦＬＧ
    Private sSetDateTime As String                   '処理開始日時

    '通番個数
    Private TUUBAN_CNT As Integer = 4   '初期値４個

    '通番表示用コントロール
    Private TUUBAN_LBL() As Label
    Private TUUBAN_VAL() As Label

    '通番情報
    Private Structure TYP_INFO_TUUBAN
        Dim LABEL As String
        Dim BAITAI As String
    End Structure
    Private INFO_TUUBAN() As TYP_INFO_TUUBAN

    '通番情報（その他）の媒体指定(True:他の通番で指定された媒体以外、False:INIファイルで指定(""以外))
    Private BAITAI_OTHER_FLG As Boolean = False
    Private OTHER_BAITAI As String = "" 'その他用の媒体

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFSMAIN141_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            '--------------------------------------------------
            '通番コントロールの動的生成
            '--------------------------------------------------
            If Not SetTuubanInfo() Then Return

            'フォームの初期化
            Call FormInitializa()

            '処理開始日時
            sSetDateTime = String.Format("{0:yyyyMMddHHmmss}", System.DateTime.Now)

            '振込日の判定に使用
            '該当月前後の休日情報のだけを蓄積する。
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 登録ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnTouroku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTouroku.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(登録)開始", "成功", "")

            '--------------------------------------------------
            'テキストボックスの入力チェック
            '--------------------------------------------------
            If Me.fn_check_text() = False Then
                Return
            End If

            RecInf.psNewFuriDate = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            RecInf.psSYORI_KEN = Me.DeleteComma(Me.txtKensu.Text)
            RecInf.psSYORI_KIN = Me.DeleteComma(Me.txtKingaku.Text)

            '--------------------------------------------------
            '重複入力のチェック
            '--------------------------------------------------
            Me.MainDB = New CASTCommon.MyOracle
            If Not RecInf.pnHENKOU_SW = 1 Then
                If Me.Double_Input_Judge() = False Then
                    MessageBox.Show(MSG0382W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    Return
                End If
            End If

            '取引先マスタのチェック
            If Me.CheckTorimast() = False Then
                Return
            End If

            '0:新規登録  1:変更登録(KFJMAIN140より選択データ受け取り表示したもの）
            If RecInf.pnHENKOU_SW = 1 Then
                '変更登録
                If Me.Update_Section = False Then
                    Me.MainDB.Rollback()
                    Return
                Else
                    Me.MainDB.Commit()
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                '媒体管理は行わない。
                'あくまで送付状の入力処理のみ

                Select Case TR.BAITAI_CODE
                    Case "00"
                        '媒体コードが伝送の場合、強制メッセージなしでの登録

                        '１：強制登録　２：通常登録
                        nKyouseitouroku = 2
                        If Me.Entry_Insert_Section(RecInf.psNewFuriDate) = False Then
                            Me.MainDB.Rollback()
                            Return
                        Else
                            Me.MainDB.Commit()
                            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        'FD、MT、CMT、WEB伝送、DVD、USB、MO、FDRを許可する
                    Case "01", "05", "06", "10", "11", "12", "13", "16"

                        'DB登録() １：強制登録　２：通常登録
                        nKyouseitouroku = 1
                        If Me.Entry_Insert_Section(RecInf.psNewFuriDate) = False Then
                            Me.MainDB.Rollback()
                            Return
                        Else
                            Me.MainDB.Commit()
                            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        '上記以外はエラー
                    Case Else
                        MessageBox.Show(MSG0383W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                        Return

                End Select
            End If

            Call FormInitializa()
            Me.txtTorisCode.Focus()

            If RecInf.pnHENKOU_SW = 0 Then
            Else
                '親画面に戻る
                Me.Close()
                Me.Dispose()
            End If

        Catch ex As Exception
            Me.MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)

        Finally
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(登録)終了", "成功", "")

        End Try
    End Sub

    ''' <summary>
    ''' 取消ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnTorikesi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTorikesi.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(取消)開始", "成功", "")

            '新規画面へもどるか変更画面へもどるか  
            'RecInf.pnHENKOU_SW = 0 : 新規画面(RecInf.pnHENKOU_SW = 1) : 変更画面
            If RecInf.pnHENKOU_SW = 0 Then
                Call FormInitializa() '初期画面に戻る
                Me.txtTorisCode.Focus()
            Else
                '親画面に戻る
                RecInf.pnHENKOU_SW = 0
                Me.Close()
                Me.Dispose()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(取消)終了", "成功", "")
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
    ''' テキストボックスヴァリデイティングイベント（ゼロパディング）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスヴァリデイテッドイベント（取引先取得）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles txtTorisCode.Validated, txtTorifCode.Validated

        Me.MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            If (Me.txtTorisCode.Text.Trim & Me.txtTorifCode.Text.Trim).Length <> 12 Then
                Return
            End If

            Call ItakuNameRead(0)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "種別チェック", "失敗", ex.Message)

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
    ''' 取引先カナインデックスチェンジイベント（取引先取得）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '選択カナで始まる委託者名を取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                If GCom.SelectItakuName_View(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "3", JYOUKEN) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者名コンボボックス設定", "失敗", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 取引先検索インデックスチェンジイベント（取引先取得）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先コードを取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString.Trim = Nothing Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                Call FMT_NzNumberString_Validated(sender, e)
                Me.txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コード設定", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            '取引先主コードチェック
            If Me.txtTorisCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return False
            End If

            '取引先副コードチェック
            If Me.txtTorifCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorifCode.Focus()
                Return False
            End If

            '振込日（年）チェック
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '振込日（月）チェック
            If Me.txtFuriDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振込日（日）チェック
            If Me.txtFuriDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = Me.txtFuriDateY.Text & "/" & Me.txtFuriDateM.Text & "/" & Me.txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '営業日チェック
            Dim KyuCode As Integer = 0
            Dim ChangeDate As String = String.Empty
            Dim bRet As Boolean = GCom.CheckDateModule(WORK_DATE, ChangeDate, KyuCode)
            If Not WORK_DATE = ChangeDate Then
                MessageBox.Show(MSG0093W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '件数チェック
            If Me.txtKensu.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "依頼件数"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKensu.Focus()
                Return False
            End If

            '金額チェック
            If Me.txtKingaku.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "依頼金額"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKingaku.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "テキストボックス入力チェック", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 送付状新規登録
    ''' </summary>
    ''' <param name="sSonDateWk">振込日</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function Entry_Insert_Section(ByVal sSonDateWk As String) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraReader2 As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(送付状新規登録)開始", "成功", "")

            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            Dim nTubanMax As Integer
            Dim sIN_DATE_WK As String
            Dim sSTATION_IN_NO_WK As String
            Dim sIN_COUNTER_WK As Integer

            '通番算出(本日最終通番)
            SQL.Append("SELECT NVL(MAX(ENTRY_NO_ME), 0) + 1 AS NEW_NUMBER FROM MEDIA_ENTRY_TBL")
            '本日日付
            SQL.Append(" WHERE ENTRY_DATE_ME = " & SQ(String.Format("{0:yyyyMMdd}", GCom.GetSysDate)))
            SQL.Append(" AND STATION_ENTRY_NO_ME = " & SQ(GCom.GetStationNo))
            Dim TARGET_INDEX As Integer = -1
            For i As Integer = 0 To TUUBAN_CNT - 1
                If INFO_TUUBAN(i).LABEL.Trim <> "その他" OrElse BAITAI_OTHER_FLG = False Then
                    If INFO_TUUBAN(i).BAITAI.IndexOf(TR.BAITAI_CODE) >= 0 Then
                        TARGET_INDEX = i
                        Exit For
                    End If
                End If
            Next

            If TARGET_INDEX >= 0 Then
                '媒体が一致した場合
                SQL.Append(" AND BAITAI_CODE_ME IN (" & INFO_TUUBAN(TARGET_INDEX).BAITAI.Trim & ")")
            Else
                '一致しなかった場合
                If BAITAI_OTHER_FLG Then
                    'フラグがたっている場合（他の媒体コード以外を「その他」とする場合）
                    SQL.Append(" AND BAITAI_CODE_ME NOT IN (" & OTHER_BAITAI.Trim & ")")
                    '「その他」の場合、最終インデックスを指定
                    TARGET_INDEX = TUUBAN_CNT - 1
                Else
                    MessageBox.Show(String.Format(MSG0388W, TR.BAITAI_CODE), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "通番取得エラー", "失敗", "媒体コード不一致　媒体コード：" & TR.BAITAI_CODE)
                    Return False
                End If
            End If

            nTubanMax = 1
            If OraReader.DataReader(SQL) = True Then
                nTubanMax = OraReader.GetInt64("NEW_NUMBER")
            End If

            OraReader.Close()

            '通番リスト用に保持する。１:FD・６:CMT．
            nHeadKoumokuCnt = 1       '１回でも登録を行ったかどうか。

            '取得した通番を表示する
            TUUBAN_VAL(TARGET_INDEX).Text = String.Format("{0:0000}", nTubanMax)

            '取引先マスタ有無確認
            SQL.Length = 0
            SQL.Append("SELECT * FROM S_TORIMAST_VIEW")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TR.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TR.TORIF_CODE))

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    Me.LW.ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

                    '搬送ケース入庫ＴＢＬはなし
                    sIN_DATE_WK = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
                    sSTATION_IN_NO_WK = "0"
                    sIN_COUNTER_WK = 0

                    '送付状実績ＴＢＬ(---Insert)
                    SQL.Length = 0
                    SQL.Append("INSERT INTO MEDIA_ENTRY_TBL ")
                    SQL.Append("(FSYORI_KBN_ME")               '振替処理区分
                    SQL.Append(", TORIS_CODE_ME")              '取引先主
                    SQL.Append(", TORIF_CODE_ME")              '取引先副
                    SQL.Append(", ENTRY_DATE_ME")              '送付状入力日
                    SQL.Append(", ITAKU_KANRI_CODE_ME")       '媒体管理コード

                    SQL.Append(", IN_DATE_ME")                 '入庫日
                    SQL.Append(", STATION_IN_NO_ME")           '入庫端末番号
                    SQL.Append(", IN_COUNTER_ME")              '受付通番

                    SQL.Append(", ITAKU_CODE_ME")              '委託者コード
                    SQL.Append(", STATION_ENTRY_NO_ME")        '端末番号
                    SQL.Append(", SYUBETU_CODE_ME")            '種別
                    SQL.Append(", FURI_DATE_ME")               '振込日
                    SQL.Append(", BAITAI_CODE_ME")             '媒体コード
                    SQL.Append(", ENTRY_NO_ME")                '通番
                    SQL.Append(", SYORI_KEN_ME")               '件数
                    SQL.Append(", SYORI_KIN_ME")               '金額
                    SQL.Append(", CYCLE_NO_ME")                'サイクル№
                    SQL.Append(", DELETE_FLG_ME")              '削除ＦＬＧ
                    SQL.Append(", FORCE_FLG_ME")               '強制登録ＦＬＧ
                    SQL.Append(", UPLOAD_DATE_ME")             'アップロード日
                    SQL.Append(", UPLOAD_FLG_ME")              'アップロードＦＬＧ
                    SQL.Append(", CHECK_FLG_ME")               '照合ＦＬＧ
                    SQL.Append(", CHECK_KBN_ME")               '照合要否区分
                    SQL.Append(", ENTRY_OP_ME")                '担当者ＩＤ（送付状入力）
                    SQL.Append(", UPDATE_OP_ME")               '担当者ＩＤ（送付状入力変更）
                    SQL.Append(", DELETE_OP_ME")               '担当者ＩＤ（送付状入力削除）
                    SQL.Append(", CREATE_DATE_ME")             '作成日
                    SQL.Append(", UPDATE_DATE_ME)")            '更新日

                    SQL.Append(" Values  (")
                    SQL.Append("'" & GCom.NzDec(OraReader.GetString("FSYORI_KBN_T"), "") & "'")
                    SQL.Append(",'" & GCom.NzDec(OraReader.GetString("TORIS_CODE_T"), "") & "'")
                    SQL.Append(",'" & GCom.NzDec(OraReader.GetString("TORIF_CODE_T"), "") & "'")
                    SQL.Append(",'" & String.Format("{0:yyyyMMdd}", GCom.GetSysDate) & "'")

                    '媒体管理コードはなし
                    SQL.Append(",'0000000000'")

                    SQL.Append(",'" & sIN_DATE_WK & "'")
                    SQL.Append(",'" & sSTATION_IN_NO_WK & "'")
                    SQL.Append(",'" & sIN_COUNTER_WK & "'")

                    SQL.Append(",'" & OraReader.GetString("ITAKU_CODE_T") & "'")
                    SQL.Append(",'" & GCom.GetStationNo & "'")
                    SQL.Append(",'" & OraReader.GetString("SYUBETU_T") & "'")
                    SQL.Append(",'" & RecInf.psNewFuriDate & "'")                '振込日
                    SQL.Append(",'" & String.Format("{0:00}", CType(OraReader.GetString("BAITAI_CODE_T"), Integer)) & "'")
                    SQL.Append(",'" & nTubanMax & "'")
                    SQL.Append(",'" & GCom.NzDec(Me.DeleteComma(Me.txtKensu.Text), "") & "'")       '件数
                    SQL.Append(",'" & GCom.NzDec(Me.DeleteComma(Me.txtKingaku.Text), "") & "'")     '金額
                    SQL.Append(",'0'")
                    SQL.Append(",'0'")
                    SQL.Append(",'" & nKyouseitouroku & "'")                           '強制登録有無
                    SQL.Append(",'" & Mid(sSetDateTime, 1, 8) & "'")    'アップロード処理を使用しないため、作成日を設定することとする。
                    SQL.Append(",'1'")                                          'アップロード処理を使用しないため、フラグを立てておくこととする。
                    SQL.Append(",'0'")
                    SQL.Append(",'" & GCom.NzInt(OraReader.GetString("SYOUGOU_KBN_T")) & "'")
                    SQL.Append(",'" & GCom.GetUserID & "'")
                    SQL.Append(",'" & "" & "'")
                    SQL.Append(",'" & "" & "'")
                    SQL.Append(",'" & sSetDateTime & "'")
                    SQL.Append(",'" & sSetDateTime & "'")        'アップロード処理を使用しないため、作成日を設定することとする。
                    SQL.Append(")")

                    Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
                    If iRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "登録"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "送付状実績テーブル登録", "失敗", Me.MainDB.Message)
                        Return False
                    End If

                    OraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "送付状実績テーブル登録", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not OraReader2 Is Nothing Then
                OraReader2.Close()
                OraReader2 = Nothing
            End If

            Me.LW.ToriCode = "000000000000"
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(送付状新規登録)終了", "成功", "")

        End Try
    End Function

    ''' <summary>
    ''' 送付状更新
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function Update_Section() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder
        Dim sFuri_Date As String
        Dim nDec As Integer
        Dim stemp As String

        sFuri_Date = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(送付状更新)開始", "成功", "")

            SQL.Append("SELECT *")
            SQL.Append(" FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")
            SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")
            SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")
            SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")
            SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")
            SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    Me.LW.ToriCode = RecInf.psTORIS_CODE & RecInf.psTORIF_CODE

                    SQL.Length = 0
                    SQL.Append("UPDATE MEDIA_ENTRY_TBL SET ")
                    SQL.Append(" FURI_DATE_ME = " & "'" & sFuri_Date & "',")                                       '振込日
                    SQL.Append(" STATION_ENTRY_NO_ME = " & GCom.GetStationNo & ",")                                '端末番号
                    SQL.Append(" SYORI_KEN_ME = " & "'" & GCom.NzDec(Me.DeleteComma(txtKensu.Text), nDec) & "',")                  '件数
                    SQL.Append(" SYORI_KIN_ME = " & "'" & GCom.NzDec(Me.DeleteComma(txtKingaku.Text), nDec) & "',")                '金額
                    SQL.Append(" UPDATE_OP_ME = " & "'" & GCom.GetUserID & "',")                                   '担当者ＩＤ（送付入力）
                    SQL.Append(" UPDATE_DATE_ME = " & "'" & String.Format("{0:yyyyMMddHHmmss}", System.DateTime.Now) & "'") '更新日
                    SQL.Append(" Where FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")
                    SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")
                    SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")
                    SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")
                    SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")
                    SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")
                    SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")

                    Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
                    If iRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "更新"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "送付状実績テーブル更新", "失敗", Me.MainDB.Message)
                        Return False
                    End If

                    OraReader.NextRead()
                End While
            End If

            '親画面ListView変更処理()
            RecInf.psSYUBETU_CODE = String.Format("{0:00}", lblSyubetuCode.Text)
            Call GCom.SelectedItem(RecInf.ListView, 8, RecInf.psSYUBETU_CODE.ToString)       '種別
            stemp = RecInf.psNewFuriDate.Substring(0, 4)
            stemp &= RecInf.psNewFuriDate.Substring(4, 2)
            stemp &= RecInf.psNewFuriDate.Substring(6)
            Call GCom.SelectedItem(RecInf.ListView, 9, stemp)                                '振込日
            stemp = String.Format("{0:#,##0}", RecInf.psSYORI_KEN)
            Call GCom.SelectedItem(RecInf.ListView, 10, stemp)                               '依頼件数
            stemp = String.Format("{0:#,##0}", RecInf.psSYORI_KIN)
            Call GCom.SelectedItem(RecInf.ListView, 11, stemp)

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "送付状実績テーブル更新", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Function

    ''' <summary>
    ''' 画面の入力内容を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FormInitializa()
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            GroupBox1.Visible = True
            lblItakuName.Text = ""
            lblSyubetuName.Text = ""

            txtFuriDateY.Text = ""
            txtFuriDateD.Text = ""
            txtFuriDateM.Text = ""
            txtKensu.Text = ""
            txtKingaku.Text = ""
            nKyouseitouroku = 0

            Me.txtTorisCode.Text = String.Empty
            Me.txtTorifCode.Text = String.Empty
            Me.lblItakuCode.Text = String.Empty
            Me.lblSyubetuCode.Text = String.Empty
            If GCom.SelectItakuName_View("", cmbToriName, txtTorisCode, txtTorifCode, "3", JYOUKEN) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            If RecInf.pnHENKOU_SW = 0 Then
                Me.txtTorisCode.ReadOnly = False
                Me.txtTorifCode.ReadOnly = False
                Me.cmbKana.Enabled = True
                Me.cmbToriName.Enabled = True
            Else
                '変更の場合(KFJMAIN142よりデータ受け渡し）送付状入力画面に表示
                GroupBox1.Visible = False

                Me.txtTorisCode.ReadOnly = True
                Me.txtTorifCode.ReadOnly = True
                Me.cmbKana.Enabled = False
                Me.cmbToriName.Enabled = False

                SQL.Append("SELECT * ")
                SQL.Append(" FROM MEDIA_ENTRY_TBL")
                SQL.Append(" WHERE ")
                SQL.Append(" FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")         '処理区分（変更画面から）
                SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")     '取引先主CD(変更画面から）
                SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")     '取引先副CD(変更画面から）
                SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")     '送付状入力日(変更画面から）
                SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")    '端末番号（変更画面から）
                SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")   '媒体コード（変更画面から）
                SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")         '受付通番（変更画面から）

                '初期化の呼び出し元はすべて接続していないはずだが、念のため
                If Not Me.MainDB Is Nothing Then
                    Me.MainDB.Close()
                    Me.MainDB = Nothing
                End If

                Me.MainDB = New CASTCommon.MyOracle
                OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        Me.lblItakuCode.Text = GCom.NzStr(OraReader.GetString("ITAKU_CODE_ME"))
                        Me.lblSyubetuCode.Text = String.Format("{0:00}", GCom.NzStr(OraReader.GetString("SYUBETU_CODE_ME")))
                        lblSyubetuName.Text = Me.SetSyubetuName(Me.lblSyubetuCode.Text)
                        txtFuriDateY.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(0, 4)
                        txtFuriDateM.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(4, 2)
                        txtFuriDateD.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(6, 2)
                        txtKensu.Text = String.Format("{0:#,##0}", GCom.NzDec(OraReader.GetString("SYORI_KEN_ME"), 0))
                        txtKingaku.Text = String.Format("{0:#,##0}", GCom.NzDec(OraReader.GetString("SYORI_KIN_ME"), 0))
                        Me.txtTorisCode.Text = GCom.NzStr(OraReader.GetString("TORIS_CODE_ME"))
                        Me.txtTorifCode.Text = GCom.NzStr(OraReader.GetString("TORIF_CODE_ME"))
                        Call ItakuNameRead(1)

                        OraReader.NextRead()
                    End While
                End If

                OraReader.Close()
                OraReader = Nothing

                Me.MainDB.Close()
                Me.MainDB = Nothing

            End If

            '*** 初期表示の時に最終通番表示         
            For i As Integer = 0 To TUUBAN_CNT - 1
                TUUBAN_VAL(i).Text = "0000"
            Next

            '通番算出(本日最終通番)
            SQL.Length = 0
            For i As Integer = 0 To TUUBAN_CNT - 1
                If i > 0 Then
                    SQL.Append(" UNION")
                End If
                SQL.Append(" SELECT NVL(MAX(ENTRY_NO_ME),0) AS NEW_NUMBER, " & i.ToString & " AS BAITAI_CODE FROM MEDIA_ENTRY_TBL")
                SQL.Append(" WHERE ENTRY_DATE_ME = " & SQ(String.Format("{0:yyyyMMdd}", GCom.GetSysDate)))
                SQL.Append(" AND STATION_ENTRY_NO_ME = " & SQ(GCom.GetStationNo))
                If INFO_TUUBAN(i).LABEL = "その他" AndAlso BAITAI_OTHER_FLG Then
                    '「その他」の場合
                    SQL.Append(" AND BAITAI_CODE_ME NOT IN (" & OTHER_BAITAI & ")")
                Else
                    SQL.Append(" AND BAITAI_CODE_ME IN (" & INFO_TUUBAN(i).BAITAI.Trim & ")")
                End If
            Next

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    TUUBAN_VAL(OraReader.GetInt("BAITAI_CODE")).Text = String.Format("{0:0000}", OraReader.GetInt("NEW_NUMBER"))

                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "初期化", "失敗", ex.Message)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Sub

    ''' <summary>
    ''' 委託者情報を取得します。
    ''' </summary>
    ''' <param name="ErrorSw"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ItakuNameRead(ByVal ErrorSw As Integer) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            '取引先取得方法変更
            With TR
                .FSYORI_KBN = ""
                .TORIS_CODE = GCom.NzDec(Me.txtTorisCode.Text, "")
                .TORIF_CODE = GCom.NzDec(Me.txtTorifCode.Text, "")
                .BAITAI_CODE = ""
                .BAITAI_KANRI_CODE = ""
                .SYUBETU_CODE = ""
                .ITAKU_CODE = ""
                .ITAKU_NNAME = ""
            End With

            If (TR.TORIS_CODE.Trim & TR.TORIF_CODE.Trim).Length = 0 Then

                lblItakuName.Text = ""

                TR.BAITAI_CODE = ""

                Return False
            Else
                SQL.Append("SELECT FSYORI_KBN_T")
                SQL.Append(", TORIS_CODE_T")
                SQL.Append(", TORIF_CODE_T")
                SQL.Append(", BAITAI_CODE_T")
                SQL.Append(", '0000000000' AS BAITAI_KANRI_CODE_T")
                SQL.Append(", SYUBETU_T")
                SQL.Append(", ITAKU_CODE_T")
                SQL.Append(", ITAKU_NNAME_T")
                SQL.Append(" FROM S_TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TR.TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(TR.TORIF_CODE))

                OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        With TR
                            .FSYORI_KBN = String.Format("{0:0}", GCom.NzDec(OraReader.GetItem("FSYORI_KBN_T"), 0))
                            .BAITAI_CODE = String.Format("{0:00}", GCom.NzDec(OraReader.GetItem("BAITAI_CODE_T"), 0))
                            .BAITAI_KANRI_CODE = String.Format("{0:0000000000}", GCom.NzDec(OraReader.GetItem("BAITAI_KANRI_CODE_T"), 0))
                            .SYUBETU_CODE = String.Format("{0:00}", GCom.NzDec(OraReader.GetItem("SYUBETU_T"), 0))
                            .ITAKU_CODE = String.Format("{0:0000000000}", GCom.NzDec(OraReader.GetItem("ITAKU_CODE_T"), 0))
                            .ITAKU_NNAME = GCom.NzStr(OraReader.GetItem("ITAKU_NNAME_T")).Trim
                        End With

                        lblItakuName.Text = GCom.GetLimitString(TR.ITAKU_NNAME, 50)
                        Me.lblSyubetuCode.Text = TR.SYUBETU_CODE
                        Me.lblItakuCode.Text = TR.ITAKU_CODE

                        Me.lblSyubetuName.Text = SetSyubetuName(TR.SYUBETU_CODE)

                        OraReader.NextRead()
                    End While

                Else
                    lblItakuName.Text = ""
                    lblSyubetuName.Text = ""

                    TR.BAITAI_CODE = ""

                    Me.lblItakuCode.Text = String.Empty
                    Me.lblSyubetuCode.Text = String.Empty

                    If ErrorSw = 0 Then
                    Else
                        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return True

    End Function


    Private Function SetSyubetuName(ByVal strSYUBETU As String) As String
        Select Case strSYUBETU
            Case "91" : Return "口振"
            Case "21" : Return "総振"
            Case "11" : Return "給与"
            Case "12" : Return "賞与"
            Case "71" : Return "公務員給与"
            Case "72" : Return "公務員賞与"
            Case Else : Return ""
        End Select
    End Function

    ''' <summary>
    ''' 二重入力チェック
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>全項目同じデータが存在の場合、重複入力とする</remarks>
    Private Function Double_Input_Judge() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            SQL.Append("SELECT * FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE TORIS_CODE_ME = " & SQ(Me.txtTorisCode.Text))
            SQL.Append(" AND TORIF_CODE_ME = " & SQ(Me.txtTorifCode.Text))
            SQL.Append(" AND FURI_DATE_ME = " & SQ(RecInf.psNewFuriDate))
            SQL.Append(" AND SYORI_KEN_ME = " & GCom.NzDec(Me.txtKensu.Text, ""))
            SQL.Append(" AND SYORI_KIN_ME = " & GCom.NzDec(Me.txtKingaku.Text, ""))
            SQL.Append(" AND DELETE_FLG_ME <> '1'")
            SQL.Append(" AND FSYORI_KBN_ME = '3'")

            If OraReader.DataReader(SQL) = True Then
                '存在するのでNG
                Return False
            Else
                '存在しないのでOK
                Return True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "二重入力チェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' カンマ削除後の数値を返します。
    ''' </summary>
    ''' <param name="strTarget">変換対象数値文字列</param>
    ''' <returns>カンマ削除後数値</returns>
    ''' <remarks></remarks>
    Private Function DeleteComma(ByVal strTarget As String) As Long
        Dim strNumber As String = strTarget.Replace(",", "").Replace(" ", "")
        Return GCom.NzLong(strNumber)
    End Function

    ''' <summary>
    ''' 取引先マスタのチェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks>2014/11/27 saitou 豊川信金 added</remarks>
    Private Function CheckTorimast() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select * from S_TORIMAST_VIEW")
                .Append(" where TORIS_CODE_T = " & SQ(Me.txtTorisCode.Text))
                .Append(" and TORIF_CODE_T = " & SQ(Me.txtTorifCode.Text))
            End With

            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetString("SYOUGOU_KBN_T") <> "1" Then
                    MessageBox.Show(MSG0384W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    Return False
                End If
            Else
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "取引先マスタチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function


    ''' <summary>
    ''' 通番コントロールの動的生成
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SetTuubanInfo() As Boolean
        Try
            Dim WK As String = ""

            '通番個数取得
            WK = GetRSKJIni("FORM", "KFSMAIN141_MAXTUUBAN").ToUpper
            If WK = "" OrElse Not IsNumeric(WK) Then
                '初期値のまま
            Else
                TUUBAN_CNT = CInt(WK)
            End If

            ReDim TUUBAN_LBL(TUUBAN_CNT - 1)
            ReDim TUUBAN_VAL(TUUBAN_CNT - 1)
            ReDim INFO_TUUBAN(TUUBAN_CNT - 1)

            For i As Integer = 1 To TUUBAN_CNT
                WK = GetRSKJIni("FORM", "KFSMAIN141_TUUBAN" & i.ToString).ToUpper
                If WK = "" OrElse WK = "ERR" Then
                    MessageBox.Show(String.Format(MSG0001E, "媒体通番" & i.ToString, "FORM", "KFSMAIN141_TUUBAN" & i.ToString), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write("設定ファイル取得", "失敗", "項目名:媒体通番" & i.ToString & " 分類:FORM 項目:KFSMAIN141_TUUBAN" & i.ToString)
                    Return False
                Else
                    Dim strBAITAI() As String = WK.Split(":"c)
                    If strBAITAI.Length <> 2 Then
                        MessageBox.Show(String.Format(MSG0035E, "媒体通番" & i.ToString, "FORM", "KFSMAIN141_TUUBAN" & i.ToString), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write("設定ファイル取得", "失敗", "項目名:媒体通番" & i.ToString & " 分類:FORM 項目:KFSMAIN141_TUUBAN" & i.ToString)
                        Return False
                    Else
                        With INFO_TUUBAN(i - 1)
                            .LABEL = strBAITAI(0)
                            .BAITAI = strBAITAI(1)

                            '媒体名が「その他」以外の媒体コードを収集する
                            If Trim(.LABEL) <> "その他" Then
                                If i = 1 Then
                                    OTHER_BAITAI = .BAITAI.Trim
                                Else
                                    OTHER_BAITAI &= ("," & .BAITAI.Trim)
                                End If
                            Else
                                If .BAITAI.Trim = "" Then
                                    '媒体名「その他」の媒体コード= NOT IN (OTHER_BAITAI)
                                    BAITAI_OTHER_FLG = True
                                End If
                            End If
                        End With
                    End If
                End If
            Next

            For i As Integer = 0 To TUUBAN_CNT - 1
                'ラベルの設定
                TUUBAN_LBL(i) = New Label
                With TUUBAN_LBL(i)
                    .Text = INFO_TUUBAN(i).LABEL
                    .Font = New Font("ＭＳ ゴシック", 9)
                    .BorderStyle = BorderStyle.None
                    .TextAlign = ContentAlignment.MiddleRight
                    .Size = New System.Drawing.Size(New System.Drawing.Point(102, 20))
                    .Location = New System.Drawing.Point(50, (i * 24) + 28)
                End With

                '通番部分の設定
                TUUBAN_VAL(i) = New Label
                With TUUBAN_VAL(i)
                    .Text = "0000"
                    .Font = New Font("ＭＳ ゴシック", 9)
                    .BorderStyle = BorderStyle.Fixed3D
                    .TextAlign = ContentAlignment.MiddleCenter
                    .Size = New System.Drawing.Size(New System.Drawing.Point(90, 20))
                    .Location = New System.Drawing.Point(180, (i * 24) + 28)
                End With

                GroupBox1.Controls.Add(TUUBAN_LBL(i))
                GroupBox1.Controls.Add(TUUBAN_VAL(i))
            Next

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "通番コントロール生成", "失敗", ex.Message)
            Return False
        End Try
    End Function
#End Region

End Class
