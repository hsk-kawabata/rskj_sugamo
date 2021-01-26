Imports CASTCommon.ModMessage
Imports System
Imports System.IO
Imports CASTCommon
Imports CAstReports
Imports System.Text
Imports System.Data.OracleClient

Public Class KFGMAST011

#Region " 変数"
    '学校マスタ(２)最大項目数(開始=Zero)

    'Private Const MaxColumns As Integer = 102   '(全103項目)
    Private Const MaxColumns As Integer = 208   '(全103 + 106項目)
    Private Const MaxColumns_Sub As Integer = 106

    '学校マスタ２レコード項目情報       'Index 0 : 日本語項目名
    '                                   'Index 1 : ORACLE項目名
    '                                   'Index 2 : 項目属性(NUMBER, CHAR)
    '                                   'Index 3 : 変更前の値
    '                                   'Index 4 : 変更後の値
    '                                   'Index 5 : 備考(必須項目)
    Private sTORI(MaxColumns, 6) As String

    '学校マスタ２レコード項目情報       'Control Object
    Private oTORI(MaxColumns) As Control

    '学校マスタ２レコード項目情報           'Index 0 : 精度(NUMBER=PRECISION, CHAR=LENGTH)
    Private nTORI(MaxColumns, 1) As Integer 'Index 1 : Zero埋め有無(0=しない, 1=する)

    Private memTORI(MaxColumns, 1) As String    'ComboBox 値参照用配列
    '                                           'Index 0 : 既存値
    '                                           'Index 1 : 変更値

    Private GakkouKName(1) As String
    Private GakkouNName(1) As String

    Private Const 参照モード As String = "参   照"
    Private Const 解除モード As String = "解   除"

    Private MyOwnerForm As Form

    Private Const ThisModuleName As String = "KFGMAST011.vb"
    Private Const ThisFormName As String = "学校マスタメンテナンス"

    Private Gakkou1_Update_Sw As Boolean

    '自金庫金融機関コード
    Private SELF_BANK_CODE As String
    Private SELF_BANK_NAME As String
    Private CENTER_CODE As String
    Private KESSAI As String
    Private strCSV_FILE_NAME As String
    Private ErrMsgFlg As Boolean = True 'エラーメッセージ表示判定
    Private SyoriKBN As Integer '進行中の処理を特定する 0:登録 1:更新 2:参照 3:削除 4:取消
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST011", "学校マスタメンテナンス画面")
    Private Const msgTitle As String = "学校マスタメンテナンス画面(KFGMAST011)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    '更新時に学校マスタメンテに出力する項目があるか判定
    Private updCnt As Integer
    Private PrintCnt As Integer
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    Private GAK1_SAKUSEI As New Hashtable    '学校マスタ1の作成日を保持

    '2013/11/27 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/27 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
    '2014/01/14 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
    Private Structure strcInshizeiLabelString
        Dim Inshizei1 As String
        Dim Inshizei2 As String
        Dim Inshizei3 As String

        Public Sub Init()
            Inshizei1 = "1万円未満"
            Inshizei2 = "1万円以上3万円未満"
            Inshizei3 = "3万円以上"
        End Sub
    End Structure
    Private InshizeiLabel As strcInshizeiLabelString
    '2014/01/14 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<
    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
    Private TUKEKAMOKU_USE_ETC As Boolean = False       '決済科目に「その他」が存在するか　True:存在する、False:存在しない
    Private TUKEKAMOKU_ETC_CODE As String = ConvertKamoku1TO2("9")   '「その他」のコード値
    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END
#End Region

#Region " ロード"

    Private Sub KFGMAST011_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            Me.CmdBack.DialogResult = DialogResult.None
            GCom.GetUserID = Me.lblUser.Text
            KESSAI = CASTCommon.GetFSKJIni("OPTION", "KESSAI")
            Call SetInformation()

            '自金庫コード取得
            SELF_BANK_CODE = STR_JIKINKO_CODE
            SELF_BANK_NAME = GCom.GetBKBRName(SELF_BANK_CODE, "", 30)
            CENTER_CODE = STR_CENTER_CODE

            sTORI(GetIndex("TKIN_NO_T"), 3) = SELF_BANK_CODE

            '2013/11/27 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            '税率取得
            Me.TAX = New CASTCommon.ClsTAX
            Me.TAX.GetZeiritsu()
            '2013/11/27 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            Me.TAX.GetInshizei()
            '2014/01/14 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            '帳票印刷用に印紙税の区分を変数に持つようにする
            Me.InshizeiLabel.Init()
            '2014/01/14 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

            If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                '印紙税が登録されてない
                '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                Me.Label8.Text = Me.InshizeiLabel.Inshizei1
                Me.Label22.Text = Me.InshizeiLabel.Inshizei2
                Me.TESUU2_L.Text = Me.InshizeiLabel.Inshizei3
                'Me.Label8.Text = "1万円未満"
                'Me.Label22.Text = "1万円以上3万円未満"
                'Me.TESUU2_L.Text = "3万円以上"
                '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            Else
                '金額によって表示形式を変える
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If Me.TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 1000) & "千"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1)
                End If

                If Me.TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 1000) & "千"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2)
                End If

                '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                Me.InshizeiLabel.Inshizei1 = strInshizei1 & "円未満"
                Me.InshizeiLabel.Inshizei2 = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                Me.InshizeiLabel.Inshizei3 = strInshizei2 & "円以上"
                Me.Label8.Text = Me.InshizeiLabel.Inshizei1
                Me.Label22.Text = Me.InshizeiLabel.Inshizei2
                Me.TESUU2_L.Text = Me.InshizeiLabel.Inshizei3
                'Me.Label8.Text = strInshizei1 & "円未満"
                'Me.Label22.Text = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                'Me.TESUU2_L.Text = strInshizei2 & "円以上"
                '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            End If
            '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

            Call FormInitializa()
            Dim COMMON_KINKOCD As Boolean = (GCom.NzInt(CASTCommon.GetFSKJIni("COMMON", "KIGYO_JIFURI")) = 1)
            With Me
                '企業自振連携時(FSKJ.INI："COMMON","KIGYO_JIFURI") = 1
                .KIGYO_CODE_L.Visible = COMMON_KINKOCD
                .KIGYO_CODE_T.Visible = COMMON_KINKOCD
                .FURI_CODE_L.Visible = COMMON_KINKOCD
                .FURI_CODE_T.Visible = COMMON_KINKOCD

                '入力禁止制御
                .CmdUpdate.Enabled = False
                .CmdDelete.Enabled = False


                .SSTab.SelectedIndex = 0
                Application.DoEvents()

                .Show()
                .GAKKOU_CODE_T.Focus()
            End With

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
            '決済科目に「その他」が存在するかチェックする
            Call ChkKessaiKamoku()
            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

            If KESSAI_Disused() = False Then
                Return
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub

#End Region

#Region " 登録"

    Private Sub CmdInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdInsert.Click
        Dim MSG As String
        Dim Ret As Integer
        Dim TransMode As Boolean = False
        GAK1_SAKUSEI = Nothing
        SyoriKBN = 0
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            MainDB = New MyOracle
            Me.SuspendLayout()

            If KESSAI_Disused() = False Then
                Return
            End If

            If Not GAKKOU_CODE_T.Text.Trim = "" Then
                LW.ToriCode = GAKKOU_CODE_T.Text.Trim & "00"
            End If

            '設定値の参照(取得)
            Call GetControlsValue()

            '参照値の検査(主に相関チェック)
            MSG = ""
            Ret = CheckMutualRelation(MSG)
            If Ret = -1 Then
                '変更登録の有無チェック関数を使って印刷用CSVを出力する。
                If Not CheckUpdateItem(0) Then

                    MSG = MSG0040I
                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Call SetFocusOnErrorControl(GetIndex("BAITAI_CODE_T"))
                    Return
                End If
            Else

                If ErrMsgFlg = True Then    '不要な場合は表示しない
                    If MSG = "" Then
                        MSG = String.Format(MSG0281W, sTORI(Ret, 0))
                    End If
                    Call MessageBox.Show(MSG, msgTitle, _
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                If Ret < 0 Then
                    Application.DoEvents()
                    Me.SSTab.SelectedIndex = 0
                Else
                    Application.DoEvents()
                    Call SetFocusOnErrorControl(Ret)
                End If

                Return
            End If

            MSG = "学校マスタを登録します。よろしいですか？" & Space(8)
            If DialogResult.OK = MessageBox.Show(MSG, msgTitle, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then
                '先行レコードのチェック
                Ret = CheckRecordModule()
                Select Case Ret
                    Case Is >= 1
                        Select Case Ret
                            Case 1
                                MSG = "既に学校マスタに登録されています。"
                            Case 2
                                MSG = "既に取引先マスタに登録されています。"
                            Case Else
                                MSG = "既に学校マスタに登録されています。"
                                MSG &= "「参照」→「更新」で登録して下さい。" & Space(8)
                        End Select

                        Call MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        SetFocusOnErrorControl(GetIndex("GAKKOU_CODE_T"))

                    Case Is = 0
                        '登録処理
                        TransMode = True
                        MainDB.BeginTrans()

                        '先ず学校マスタ２を作成
                        Dim FLG As Boolean = INSERT_GAKMAST2()
                        If FLG Then

                            '次に学校マスタ１を作成
                            FLG = INSERT_GAKMAST1()
                            If FLG Then

                                '最後に取引先マスタを作成(自振連携)
                                FLG = PFUNC_INSERT_TORIMAST()
                            End If
                        End If

                        If FLG Then
                            TransMode = False
                            MainDB.Commit()
                            MSG = "学校マスタ登録処理が完了しました。"


                            Call MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)

                            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit Sub
                            End If

                            '印刷処理を起動する
                            If fn_CreateCSV_INSERT() Then
                                Call fn_Print(1)
                            End If
                        Else
                            TransMode = False
                            MainDB.Rollback()

                            MSG = "学校マスタ登録処理に失敗しました。"

                            Call MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Case Else
                        MSG = "学校マスタの事前参照に失敗しました。" & Space(8)


                        Call MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub

    Private Function INSERT_GAKMAST2() As Boolean
        Dim SQL As String
        Dim Cnt As Integer


        sTORI(GetIndex("SAKUSEI_DATE_T"), 4) = String.Format("{0:yyyyMMddHHmmss}", Date.Now)
        sTORI(GetIndex("KOUSIN_DATE_T"), 4) = String.Format("{0:yyyyMMddHHmmss}", Date.Now)

        '再振種別と持越種別
        Dim DB_VALUE As Integer
        Dim SFURI_SYUBETU As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T)
        Dim SFURI_SYUBETU_SUB As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T_SUB)

        Call GET_SET_SFURI_SYUBETU(1, DB_VALUE, SFURI_SYUBETU, SFURI_SYUBETU_SUB)

        Try
            '=============================================
            ' 学校マスタ２ <GAKMAST2> 登録
            '=============================================
            SQL = "INSERT INTO GAKMAST2"

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1

                If TypeOf oTORI(Cnt) Is Label Then
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T", "SINKYU_NENDO_T"
                            Select Case Cnt
                                Case 0
                                    SQL &= " (" & sTORI(Cnt, 1)
                                Case Else
                                    SQL &= ", " & sTORI(Cnt, 1)
                            End Select
                    End Select
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "GAKKOU_NNAME_G", "GAKKOU_KNAME_G"
                            '学校名は更新の必要なし
                        Case "SAKUSEI_DATE_T", "KOUSIN_DATE_T"
                            '作成日、更新日は最後に回す。
                        Case "SFURI_SYUBETU_T"
                            '再振種別と持越種別は最後に回す。
                        Case Else
                            Select Case Cnt
                                Case 0
                                    SQL &= " (" & sTORI(Cnt, 1)
                                Case Else
                                    SQL &= ", " & sTORI(Cnt, 1)
                            End Select
                    End Select
                End If
            Next Cnt

            SQL &= ", SFURI_SYUBETU_T"
            SQL &= ", SAKUSEI_DATE_T"
            SQL &= ", KOUSIN_DATE_T"

            SQL &= ") VALUES"

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1
                If TypeOf oTORI(Cnt) Is Label Then
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T", "SINKYU_NENDO_T"
                            Select Case Cnt
                                Case 0
                                    SQL &= " (" & SetColumnData(Cnt)
                                Case Else
                                    SQL &= ", " & SetColumnData(Cnt)
                            End Select
                    End Select
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "GAKKOU_NNAME_G", "GAKKOU_KNAME_G"
                            '学校名は更新の必要なし
                        Case "SAKUSEI_DATE_T", "KOUSIN_DATE_T"
                            '作成日、更新日は最後に回す。
                        Case "SFURI_SYUBETU_T"
                            '再振種別と持越種別は最後に回す。
                        Case Else
                            Select Case Cnt
                                Case 0
                                    SQL &= " (" & SetColumnData(Cnt)
                                Case Else
                                    SQL &= ", " & SetColumnData(Cnt)
                            End Select
                    End Select
                End If

            Next Cnt

            SQL &= ", '" & DB_VALUE.ToString & "'"              '再振種別
            SQL &= ", TO_CHAR(SYSDATE, 'yyyymmdd')"     '作成日
            SQL &= ", TO_CHAR(SYSDATE, 'yyyymmdd')"     '更新日
            SQL &= ")"

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret = 1 Then
            Else
                Throw New Exception("学校マスタ2に登録できませんでした。(登録件数=" & Ret & ")")
            End If

            '=============================================
            ' 学校マスタ２(固有情報) <GAKMAST2_SUB> 登録
            '=============================================
            SQL = "INSERT INTO GAKMAST2_SUB ("
            SQL &= "   GAKKOU_CODE_TSUB"
            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                SQL &= " , " & sTORI(Cnt, 1)
            Next Cnt

            SQL &= " ) VALUES ( "
            SQL &= "  " & SetColumnData(0)
            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                SQL &= ", " & SetColumnData(Cnt)
            Next Cnt
            SQL &= " ) "

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Ret = 1 Then
                Return True
            Else
                Throw New Exception("学校マスタ2(固有情報)に登録できませんでした。(登録件数=" & Ret & ")")
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return False

    End Function

    '
    ' 機能　　 : 学校マスタ１への登録（新規更新）処理
    '
    ' 戻り値　 : 成功 = True
    ' 　　　　   失敗 = False
    '
    ' 引き数　 : ARG1 - SEL = 0 (登録)
    ' 　　　 　             = 1 (更新)
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function INSERT_GAKMAST1(Optional ByVal SEL As Short = 0) As Boolean
        Dim SQL As String
        Dim Ret As Integer
        Try
            Dim GAKCODE As String = sTORI(GetIndex("GAKKOU_CODE_T"), 4)

            '先ず削除
            SQL = "DELETE FROM GAKMAST1"
            SQL &= " WHERE GAKKOU_CODE_G = '" & GAKCODE & "'"

            Dim SQLCode As Integer = 0
            Ret = MainDB.ExecuteNonQuery(SQL)

            'SQL文作成（学校マスタ１）
            For BYTE_Counter As Integer = 1 To CInt(SIYOU_GAKUNEN_T.Text)

                If Not fn_INSERT_GAKMAST1(CByte(BYTE_Counter)) Then
                    Exit Try
                End If

            Next BYTE_Counter

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録処理)", "失敗", ex.ToString)
        End Try

        Return False
    End Function

    Private Function fn_INSERT_GAKMAST1(ByVal pByte_Page As Byte) As Boolean
        '********************************************
        '学校マスタ１（登録）
        '********************************************

        Dim ret As Boolean = False

        Try
            Dim SQL As String = ""
            SQL &= "INSERT INTO"
            SQL &= " KZFMAST.GAKMAST1"
            SQL &= "("
            SQL &= " GAKKOU_CODE_G"
            SQL &= ",GAKKOU_NNAME_G"
            SQL &= ",GAKKOU_KNAME_G"
            SQL &= ",GAKUNEN_CODE_G"
            SQL &= ",GAKUNEN_NAME_G"
            SQL &= ",CLASS_CODE101_G"
            SQL &= ",CLASS_NAME101_G"
            SQL &= ",CLASS_CODE102_G"
            SQL &= ",CLASS_NAME102_G"
            SQL &= ",CLASS_CODE103_G"
            SQL &= ",CLASS_NAME103_G"
            SQL &= ",CLASS_CODE104_G"
            SQL &= ",CLASS_NAME104_G"
            SQL &= ",CLASS_CODE105_G"
            SQL &= ",CLASS_NAME105_G"
            SQL &= ",CLASS_CODE106_G"
            SQL &= ",CLASS_NAME106_G"
            SQL &= ",CLASS_CODE107_G"
            SQL &= ",CLASS_NAME107_G"
            SQL &= ",CLASS_CODE108_G"
            SQL &= ",CLASS_NAME108_G"
            SQL &= ",CLASS_CODE109_G"
            SQL &= ",CLASS_NAME109_G"
            SQL &= ",CLASS_CODE110_G"
            SQL &= ",CLASS_NAME110_G"
            SQL &= ",CLASS_CODE111_G"
            SQL &= ",CLASS_NAME111_G"
            SQL &= ",CLASS_CODE112_G"
            SQL &= ",CLASS_NAME112_G"
            SQL &= ",CLASS_CODE113_G"
            SQL &= ",CLASS_NAME113_G"
            SQL &= ",CLASS_CODE114_G"
            SQL &= ",CLASS_NAME114_G"
            SQL &= ",CLASS_CODE115_G"
            SQL &= ",CLASS_NAME115_G"
            SQL &= ",CLASS_CODE116_G"
            SQL &= ",CLASS_NAME116_G"
            SQL &= ",CLASS_CODE117_G"
            SQL &= ",CLASS_NAME117_G"
            SQL &= ",CLASS_CODE118_G"
            SQL &= ",CLASS_NAME118_G"
            SQL &= ",CLASS_CODE119_G"
            SQL &= ",CLASS_NAME119_G"
            SQL &= ",CLASS_CODE120_G"
            SQL &= ",CLASS_NAME120_G"
            SQL &= ",SAKUSEI_DATE_G"
            SQL &= ",KOUSIN_DATE_G"
            SQL &= ")"
            SQL &= "VALUES "
            SQL &= "("
            SQL &= "'" & GAKKOU_CODE_T.Text & "'"
            '学校名
            SQL &= " ,'" & sTORI(GetIndex("GAKKOU_NNAME_G"), 4) & "'"
            SQL &= " ,'" & sTORI(GetIndex("GAKKOU_KNAME_G"), 4) & "'"
            '学年
            SQL &= "," & pByte_Page
            '学年名称
            Dim dgv As DataGridView = Nothing
            Select Case (pByte_Page)
                Case 1
                    SQL &= ",'" & txtGAKUNEN_NAME1.Text & "'"
                    dgv = DataGridView1
                Case 2
                    SQL &= ",'" & txtGAKUNEN_NAME2.Text & "'"
                    dgv = DataGridView2
                Case 3
                    SQL &= ",'" & txtGAKUNEN_NAME3.Text & "'"
                    dgv = DataGridView3
                Case 4
                    SQL &= ",'" & txtGAKUNEN_NAME4.Text & "'"
                    dgv = DataGridView4
                Case 5
                    SQL &= ",'" & txtGAKUNEN_NAME5.Text & "'"
                    dgv = DataGridView5
                Case 6
                    SQL &= ",'" & txtGAKUNEN_NAME6.Text & "'"
                    dgv = DataGridView6
                Case 7
                    SQL &= ",'" & txtGAKUNEN_NAME7.Text & "'"
                    dgv = DataGridView7
                Case 8
                    SQL &= ",'" & txtGAKUNEN_NAME8.Text & "'"
                    dgv = DataGridView8
                Case 9
                    SQL &= ",'" & txtGAKUNEN_NAME9.Text & "'"
                    dgv = DataGridView9
            End Select

            'クラス情報設定
            With dgv
                For ClassCnt As Integer = 0 To 19 Step 1
                    If Trim(CStr(.Rows(ClassCnt).Cells(0).Value)) = "" Then
                        SQL += ",NULL,''"
                    Else
                        SQL += "," & Trim(CStr(.Rows(ClassCnt).Cells(0).Value))
                        SQL += ",'" & Trim(CStr(.Rows(ClassCnt).Cells(1).Value)) & "'"
                    End If
                Next
            End With
            If Not GAK1_SAKUSEI Is Nothing AndAlso GAK1_SAKUSEI.ContainsKey(pByte_Page.ToString) Then
                SQL &= ",'" & GAK1_SAKUSEI.Item(pByte_Page.ToString).ToString & "'"
                SQL &= ",'" & Now.ToString("yyyyMMdd") & "'"
            Else
                SQL &= ",'" & Now.ToString("yyyyMMdd") & "'"
                SQL &= ",''"
            End If

            SQL &= ")"

            'SQL実行
            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録処理)", "失敗", ex.ToString)
        End Try

        Return ret

    End Function

    '----------------------------------------------------------------------------
    'Name       :PFUNC_INSERT_TORIMAST
    'Description:取引先マスタ作成
    'Parameta   :
    'Create     :2004/??/??
    'Update     :2007/12/26, 2008.01.31
    '           :おかしん版に移行(企業自振取引先ﾏｽﾀ生成時に企業側取引先ﾏｽﾀの項目追加の為）
    '　　　     :
    '----------------------------------------------------------------------------
    Private Function PFUNC_INSERT_TORIMAST() As Boolean

        Dim SQL As StringBuilder
        Dim SQLBasic As StringBuilder
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim DayCnt As Integer

        With GCom.GLog
            .Job2 = "TORIMAST作成"
            .Result = ""
            .Discription = ""
        End With

        Try
            '=============================================
            ' 取引先マスタ <TORIMAST> 登録
            '=============================================
            SQLBasic = New StringBuilder
            SQLBasic.Append("INSERT INTO TORIMAST")
            SQLBasic.Append(" (FSYORI_KBN_T")                       '1
            SQLBasic.Append(", TORIS_CODE_T")                       '2
            SQLBasic.Append(", TORIF_CODE_T")                       '3
            SQLBasic.Append(", BAITAI_CODE_T")                      '4
            SQLBasic.Append(", LABEL_KBN_T")                        '5
            SQLBasic.Append(", CODE_KBN_T")                         '6
            SQLBasic.Append(", ITAKU_KANRI_CODE_T")                 '7
            SQLBasic.Append(", FILE_NAME_T")                        '8
            SQLBasic.Append(", FMT_KBN_T")                          '9
            SQLBasic.Append(", MULTI_KBN_T")                        '10
            SQLBasic.Append(", NS_KBN_T")                           '11
            SQLBasic.Append(", SYUBETU_T")                          '12
            SQLBasic.Append(", ITAKU_CODE_T")                       '13
            SQLBasic.Append(", ITAKU_KNAME_T")                      '14
            SQLBasic.Append(", TKIN_NO_T")                          '15
            SQLBasic.Append(", TSIT_NO_T")                          '16
            SQLBasic.Append(", KAMOKU_T")                           '17
            SQLBasic.Append(", KOUZA_T")                            '18
            SQLBasic.Append(", MOTIKOMI_KBN_T")                     '19
            SQLBasic.Append(", SOUSIN_KBN_T")                       '20
            SQLBasic.Append(", TAKO_KBN_T")                         '21
            SQLBasic.Append(", JIFURICHK_KBN_T")                    '22
            SQLBasic.Append(", TEKIYOU_KBN_T")                      '23
            SQLBasic.Append(", KTEKIYOU_T")                         '24
            SQLBasic.Append(", NTEKIYOU_T")                         '25
            SQLBasic.Append(", FURI_CODE_T")                        '26
            SQLBasic.Append(", KIGYO_CODE_T")                       '27
            SQLBasic.Append(", ITAKU_NNAME_T")                      '28
            SQLBasic.Append(", YUUBIN_T")                           '29
            SQLBasic.Append(", DENWA_T")                            '30
            SQLBasic.Append(", FAX_T")                              '31
            SQLBasic.Append(", KOKYAKU_NO_T")                       '32
            SQLBasic.Append(", KANREN_KIGYO_CODE_T")                '33
            SQLBasic.Append(", ITAKU_NJYU_T")                       '34
            SQLBasic.Append(", YUUBIN_KNAME_T")                     '35
            SQLBasic.Append(", YUUBIN_NNAME_T")                     '36
            SQLBasic.Append(", FURI_KYU_CODE_T")                    '37
            SQLBasic.Append(", DATE1_T")                            '38
            SQLBasic.Append(", DATE2_T")                            '39
            SQLBasic.Append(", DATE3_T")                            '40
            SQLBasic.Append(", DATE4_T")                            '41
            SQLBasic.Append(", DATE5_T")                            '42
            SQLBasic.Append(", DATE6_T")                            '43
            SQLBasic.Append(", DATE7_T")                            '44
            SQLBasic.Append(", DATE8_T")                            '45
            SQLBasic.Append(", DATE9_T")                            '46
            SQLBasic.Append(", DATE10_T")                           '47
            SQLBasic.Append(", DATE11_T")                           '48
            SQLBasic.Append(", DATE12_T")                           '49
            SQLBasic.Append(", DATE13_T")                           '50
            SQLBasic.Append(", DATE14_T")                           '51
            SQLBasic.Append(", DATE15_T")                           '52
            SQLBasic.Append(", DATE16_T")                           '53
            SQLBasic.Append(", DATE17_T")                           '54
            SQLBasic.Append(", DATE18_T")                           '55
            SQLBasic.Append(", DATE19_T")                           '56
            SQLBasic.Append(", DATE20_T")                           '57
            SQLBasic.Append(", DATE21_T")                           '58
            SQLBasic.Append(", DATE22_T")                           '59
            SQLBasic.Append(", DATE23_T")                           '60
            SQLBasic.Append(", DATE24_T")                           '61
            SQLBasic.Append(", DATE25_T")                           '62
            SQLBasic.Append(", DATE26_T")                           '63
            SQLBasic.Append(", DATE27_T")                           '64
            SQLBasic.Append(", DATE28_T")                           '65
            SQLBasic.Append(", DATE29_T")                           '66
            SQLBasic.Append(", DATE30_T")                           '67
            SQLBasic.Append(", DATE31_T")                           '68
            SQLBasic.Append(", TUKI1_T")                            '69
            SQLBasic.Append(", TUKI2_T")                            '70
            SQLBasic.Append(", TUKI3_T")                            '71
            SQLBasic.Append(", TUKI4_T")                            '72
            SQLBasic.Append(", TUKI5_T")                            '73
            SQLBasic.Append(", TUKI6_T")                            '74
            SQLBasic.Append(", TUKI7_T")                            '75
            SQLBasic.Append(", TUKI8_T")                            '76
            SQLBasic.Append(", TUKI9_T")                            '77
            SQLBasic.Append(", TUKI10_T")                           '78
            SQLBasic.Append(", TUKI11_T")                           '79
            SQLBasic.Append(", TUKI12_T")                           '80
            SQLBasic.Append(", SFURI_FLG_T")                        '81
            SQLBasic.Append(", SFURI_FCODE_T")                      '82
            SQLBasic.Append(", SFURI_DAY_T")                        '83
            SQLBasic.Append(", SFURI_KIJITSU_T")                    '84
            SQLBasic.Append(", SFURI_KYU_CODE_T")                   '85
            SQLBasic.Append(", KEIYAKU_DATE_T")                     '86
            SQLBasic.Append(", KAISI_DATE_T")                       '87
            SQLBasic.Append(", SYURYOU_DATE_T")                     '88
            SQLBasic.Append(", MOTIKOMI_KIJITSU_T")                 '89
            SQLBasic.Append(", IRAISYO_YDATE_T")                    '90
            SQLBasic.Append(", IRAISYO_KIJITSU_T")                  '91
            SQLBasic.Append(", IRAISYO_KYU_CODE_T")                 '92
            SQLBasic.Append(", IRAISYO_KBN_T")                      '93
            SQLBasic.Append(", IRAISYO_SORT_T")                     '94
            SQLBasic.Append(", TEIGAKU_KBN_T")                      '95
            SQLBasic.Append(", UMEISAI_KBN_T")                      '96
            SQLBasic.Append(", FUNOU_MEISAI_KBN_T")                 '97
            SQLBasic.Append(", KEKKA_HENKYAKU_KBN_T")               '98
            SQLBasic.Append(", KEKKA_MEISAI_KBN_T")                 '99
            SQLBasic.Append(", PRTNUM_T")                           '100
            SQLBasic.Append(", FKEKKA_TBL_T")                       '101
            SQLBasic.Append(", KESSAI_KBN_T")                       '102
            SQLBasic.Append(", TORIMATOME_SIT_T")                   '103
            SQLBasic.Append(", HONBU_KOUZA_T")                      '104
            SQLBasic.Append(", KESSAI_DAY_T")                       '105
            SQLBasic.Append(", KESSAI_KIJITSU_T")                   '106
            SQLBasic.Append(", KESSAI_KYU_CODE_T")                  '107
            SQLBasic.Append(", TUKEKIN_NO_T")                       '108
            SQLBasic.Append(", TUKESIT_NO_T")                       '109
            SQLBasic.Append(", TUKEKAMOKU_T")                       '110
            SQLBasic.Append(", TUKEKOUZA_T")                        '111
            SQLBasic.Append(", TUKEMEIGI_KNAME_T")                  '112
            SQLBasic.Append(", BIKOU1_T")                           '113
            SQLBasic.Append(", BIKOU2_T")                           '114
            SQLBasic.Append(", TESUUTYO_SIT_T")                     '115
            SQLBasic.Append(", TESUUTYO_KAMOKU_T")                  '116
            SQLBasic.Append(", TESUUTYO_KOUZA_T")                   '117
            SQLBasic.Append(", TESUUTYO_KBN_T")                     '118
            SQLBasic.Append(", TESUUTYO_PATN_T")                    '119
            SQLBasic.Append(", TESUUMAT_NO_T")                      '120
            SQLBasic.Append(", TESUUTYO_DAY_T")                     '121
            SQLBasic.Append(", TESUUTYO_KIJITSU_T")                 '122
            SQLBasic.Append(", TESUU_KYU_CODE_T")                   '123
            SQLBasic.Append(", SEIKYU_KBN_T")                       '124
            SQLBasic.Append(", KIHTESUU_T")                         '125
            SQLBasic.Append(", SYOUHI_KBN_T")                       '126
            SQLBasic.Append(", SOURYO_T")                           '127
            SQLBasic.Append(", KOTEI_TESUU1_T")                     '128
            SQLBasic.Append(", KOTEI_TESUU2_T")                     '129
            SQLBasic.Append(", TESUUMAT_MONTH_T")                   '130
            SQLBasic.Append(", TESUUMAT_ENDDAY_T")                  '131
            SQLBasic.Append(", TESUUMAT_KIJYUN_T")                  '132
            SQLBasic.Append(", TESUUMAT_PATN_T")                    '133
            SQLBasic.Append(", TESUU_GRP_T")                        '134
            SQLBasic.Append(", TESUU_TABLE_ID_T")                   '135
            SQLBasic.Append(", TESUU_A1_T")                         '136
            SQLBasic.Append(", TESUU_A2_T")                         '137
            SQLBasic.Append(", TESUU_A3_T")                         '138
            SQLBasic.Append(", TESUU_B1_T")                         '139
            SQLBasic.Append(", TESUU_B2_T")                         '140
            SQLBasic.Append(", TESUU_B3_T")                         '141
            SQLBasic.Append(", TESUU_C1_T")                         '142
            SQLBasic.Append(", TESUU_C2_T")                         '143
            SQLBasic.Append(", TESUU_C3_T")                         '144
            SQLBasic.Append(", ENC_KBN_T")                          '145
            SQLBasic.Append(", ENC_OPT1_T")                         '146
            SQLBasic.Append(", ENC_KEY1_T")                         '147
            SQLBasic.Append(", ENC_KEY2_T")                         '148
            SQLBasic.Append(", KOUSIN_SIKIBETU_T")                  '149
            SQLBasic.Append(", SAKUSEI_DATE_T")                     '150
            SQLBasic.Append(", KOUSIN_DATE_T")                      '151
            '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQLBasic.Append(", YOBI1_T")                            '152(WEB伝送ユーザ名)
            '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQLBasic.Append(", YOBI2_T")                            '153
            SQLBasic.Append(", YOBI3_T")                            '154
            SQLBasic.Append(", YOBI4_T")                            '155
            SQLBasic.Append(", YOBI5_T")                            '156
            SQLBasic.Append(", YOBI6_T")                            '157
            SQLBasic.Append(", YOBI7_T")                            '158
            SQLBasic.Append(", YOBI8_T")                            '159
            SQLBasic.Append(", YOBI9_T")                            '160
            SQLBasic.Append(", YOBI10_T")                           '161
            SQLBasic.Append(") VALUES")

            For Cnt = 1 To 4 Step 1
                SQL = New StringBuilder
                SQL.Append(" ('1'")                                                 '1 FSYORI_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))          '2 TORIS_CODE_T
                SQL.Append(", " & SQ(Cnt.ToString.PadLeft(2, "0"c)))                '3 TORIF_CODE_T
                SQL.Append(", '07'")                                                '4 BAITAI_CODE_T
                SQL.Append(", '0'")                                                 '5 LABEL_KBN_T
                SQL.Append(", '0'")                                                 '6 CODE_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4)))           '7 ITAKU_KANRI_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("FILE_NAME_T"), 4)))           '8 FILE_NAME_T
                SQL.Append(", '00'")                                                '9 FMT_KBN_T
                SQL.Append(", '0'")                                                 '10 MULTI_KBN_T
                If Cnt = 3 Then
                    SQL.Append(", '1'") '入金                                       '11 NS_KBN_T
                    SQL.Append(", '21'")                                            '12 SYUBETU_T
                Else
                    SQL.Append(", '9'") '出金                                       '11 NS_KBN_T
                    SQL.Append(", '91'")                                            '12 SYUBETU_T
                End If

                '2010/09/08.Sakon　委託者コードの下１桁変更を行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                SQL.Append(", " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4))) '13 ITAKU_CODE_T
                'SQL.Append(", " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4).Substring(0, 9) & GCom.NzStr(Cnt01))) '13 ITAKU_CODE_T
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                SQL.Append(", " & SQ(sTORI(GetIndex("GAKKOU_KNAME_G"), 4)))         '14 ITAKU_KNAME_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TKIN_NO_T"), 4)))              '15 TKIN_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TSIT_NO_T"), 4)))              '16 TSIT_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KAMOKU_T"), 4)))               '17 KAMOKU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KOUZA_T"), 4)))                '18 KOUZA_T
                SQL.Append(", '0'")                                                 '19 MOTIKOMI_KBN_T
                SQL.Append(", '0'")                                                 '20 SOUSIN_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TAKO_KBN_T"), 4)))             '21 TAKO_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("JIFURICHK_KBN_T"), 4)))        '22 JIFURICHK_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TEKIYOU_KBN_T"), 4)))          '23 TEKIYOU_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KTEKIYOU_T"), 4)))             '24 KTEKIYOU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("NTEKIYOU_T"), 4)))             '25 NTEKIYOU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("FURI_CODE_T"), 4)))            '26 FURI_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KIGYO_CODE_T"), 4)))           '27 KIGYO_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("GAKKOU_NNAME_G"), 4)))         '28 ITAKU_NNAME_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YUUBIN_T"), 4)))               '29 YUUBIN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("DENWA_T"), 4)))                '30 DENWA_T
                SQL.Append(", " & SQ(sTORI(GetIndex("FAX_T"), 4)))                  '31 FAX_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KOKYAKU_NO_T"), 4)))           '32 KOKYAKU_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KANREN_KIGYO_CODE_T"), 4)))    '33 KANREN_KIGYO_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("ITAKU_NJYU_T"), 4)))         '34 ITAKU_NJYU_T
                SQL.Append(", ''")                                                  '35 YUUBIN_KNAME_T
                SQL.Append(", ''")                                                  '36 YUUBIN_NNAME_T

                Select Case Cnt                                                     '37 FURI_KYU_CODE_T
                    Case 3
                        SQL.Append(", '" & sTORI(GetIndex("NKYU_CODE_T"), 4) & "'")
                    Case Else
                        SQL.Append(", '" & sTORI(GetIndex("SKYU_CODE_T"), 4) & "'")
                End Select

                Select Case Cnt                                                     '38 DATE1_T ～ 68 DATE31_T
                    Case 1
                        '初振
                        Ret = GCom.NzInt(sTORI(GetIndex("FURI_DATE_T"), 4))
                        For DayCnt = 1 To 31 Step 1
                            If DayCnt = Ret Then
                                SQL.Append(", '1'")
                            Else
                                SQL.Append(", '0'")
                            End If
                        Next DayCnt
                    Case 2
                        '再振
                        Ret = GCom.NzInt(sTORI(GetIndex("SFURI_DATE_T"), 4))
                        For DayCnt = 1 To 31 Step 1
                            If DayCnt = Ret Then
                                SQL.Append(", '1'")
                            Else
                                SQL.Append(", '0'")
                            End If
                        Next DayCnt

                    Case 3, 4
                        For DayCnt = 1 To 31 Step 1
                            SQL.Append(", '0'")
                        Next DayCnt
                End Select

                Select Case Cnt
                    Case 1, 2
                        Select Case True
                            Case (Cnt = 2 AndAlso GCom.GetComboBox(Me.SFURI_SYUBETU_T) = 0)
                                '再振なし
                                For No As Integer = 1 To 12
                                    SQL.Append(", '0'")                             '69 TUKI1_T ～ 80 TUKI12_T
                                Next
                            Case Else
                                For No As Integer = 1 To 12
                                    SQL.Append(", '1'")                             '69 TUKI1_T ～ 80 TUKI12_T
                                Next
                        End Select
                    Case 3, 4
                        '-03(入金)と-04(臨時出金)の場合は月別フラグを使用しない。
                        For No As Integer = 1 To 12
                            SQL.Append(", '0'")                                     '69 TUKI1_T ～ 80 TUKI12_T
                        Next
                End Select
                SQL.Append(", '0'")                                                 '81 SFURI_FLG_T
                SQL.Append(", ''")                                                  '82 SFURI_FCODE_T
                SQL.Append(", 0")                                                   '83 SFURI_DAY_T
                SQL.Append(", '0'")                                                 '84 SFURI_KIJITSU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SFURI_KYU_CODE_T"), 4)))       '85 SFURI_KYU_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KEIYAKU_DATE_T"), 4)))         '86 KEIYAKU_DATE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KAISI_DATE_T"), 4)))           '87 KAISI_DATE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SYURYOU_DATE_T"), 4)))         '88 SYURYOU_DATE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("MOTIKOMI_KIJITSU_T"), 4)))     '89 MOTIKOMI_KIJITSU_T
                SQL.Append(", '0'")                                                 '90 IRAISYO_YDATE_T
                SQL.Append(", '0'")                                                 '91 IRAISYO_KIJITSU_T
                SQL.Append(", '0'")                                                 '92 IRAISYO_KYU_CODE_T
                SQL.Append(", '0'")                                                 '93 IRAISYO_KBN_T
                SQL.Append(", '0'")                                                 '94 IRAISYO_SORT_T
                SQL.Append(", '0'")                                                 '95 TEIGAKU_KBN_T
                SQL.Append(", '0'")                                                 '96 UMEISAI_KBN_T
                SQL.Append(", '0'")                                                 '97 FUNOU_MEISAI_KBN_T
                SQL.Append(", '0'")                                                 '98 KEKKA_HENKYAKU_KBN_T
                SQL.Append(", '0'")                                                 '99 KEKKA_MEISAI_KBN_T
                SQL.Append(", 0")                                                   '100 PRTNUM_T
                SQL.Append(", " & SQ(sTORI(GetIndex("FKEKKA_TBL_T"), 4)))           '101 FKEKKA_TBL_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KESSAI_KBN_T"), 4)))           '102 KESSAI_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TORIMATOME_SIT_T"), 4)))       '103 TORIMATOME_SIT_T
                SQL.Append(", " & SQ(sTORI(GetIndex("HONBU_KOUZA_T"), 4)))          '104 HONBU_KOUZA_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KESSAI_DAY_T"), 4)))           '105 KESSAI_DAY_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)))       '106 KESSAI_KIJITSU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KESSAI_KYU_CODE_T"), 4)))      '107 KESSAI_KYU_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TUKEKIN_NO_T"), 4)))           '108 TUKEKIN_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TUKESIT_NO_T"), 4)))           '109 TUKESIT_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TUKEKAMOKU_T"), 4)))           '110 TUKEKAMOKU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TUKEKOUZA_T"), 4)))            '111 TUKEKOUZA_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TUKEMEIGI_T"), 4)))            '112 TUKEMEIGI_KNAME_T
                SQL.Append(", " & SQ(sTORI(GetIndex("DENPYO_BIKOU1_T"), 4)))        '113 BIKOU1_T
                SQL.Append(", " & SQ(sTORI(GetIndex("DENPYO_BIKOU2_T"), 4)))        '114 BIKOU2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_SIT_T"), 4)))         '115 TESUUTYO_SIT_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_KAMOKU_T"), 4)))      '116 TESUUTYO_KAMOKU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_KOUZA_T"), 4)))       '117 TESUUTYO_KOUZA_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_KBN_T"), 4)))         '118 TESUUTYO_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_PATN_T"), 4)))        '119 TESUUTYO_PATN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUMAT_NO_T"), 4)))          '120 TESUUMAT_NO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_DAY_T"), 4)))         '121 TESUUTYO_DAY_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4)))     '122 TESUUTYO_KIJITSU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_KYU_CODE_T"), 4)))       '123 TESUU_KYU_CODE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SEIKYU_KBN_T"), 4)))           '124 SEIKYU_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KIHTESUU_T"), 4)))             '125 KIHTESUU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SYOUHI_KBN_T"), 4)))           '126 SYOUHI_KBN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SOURYO_T"), 4)))               '127 SOURYO_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KOTEI_TESUU1_T"), 4)))         '128 KOTEI_TESUU1_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KOTEI_TESUU2_T"), 4)))         '129 KOTEI_TESUU2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUMAT_MONTH_T"), 4)))       '130 TESUUMAT_MONTH_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUMAT_ENDDAY_T"), 4)))      '131 TESUUMAT_ENDDAY_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUMAT_KIJYUN_T"), 4)))      '132 TESUUMAT_KIJYUN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUUMAT_PATN_T"), 4)))        '133 TESUUMAT_PATN_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_GRP_T"), 4)))            '134 TESUU_GRP_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_TABLE_ID_T"), 4)))       '135 TESUU_TABLE_ID_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_A1_T"), 4)))             '136 TESUU_A1_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_A2_T"), 4)))             '137 TESUU_A2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_A3_T"), 4)))             '138 TESUU_A3_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_B1_T"), 4)))             '139 TESUU_B1_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_B2_T"), 4)))             '140 TESUU_B2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_B3_T"), 4)))             '141 TESUU_B3_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_C1_T"), 4)))             '142 TESUU_C1_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_C2_T"), 4)))             '143 TESUU_C2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("TESUU_C3_T"), 4)))             '144 TESUU_C3_T
                SQL.Append(", '0'")                                                 '145 ENC_KBN_T
                SQL.Append(", '0'")                                                 '146 ENC_OPT1_T
                SQL.Append(", ''")                                                  '147 ENC_KEY1_T
                SQL.Append(", ''")                                                  '148 ENC_KEY2_T
                SQL.Append(", '0'")                                                 '149 KOUSIN_SIKIBETU_T
                SQL.Append(", " & SQ(sTORI(GetIndex("SAKUSEI_DATE_T"), 4)))         '150 SAKUSEI_DATE_T
                SQL.Append(", " & SQ(sTORI(GetIndex("KOUSIN_DATE_T"), 4)))          '151 KOUSIN_DATE_T
                '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI1_T"), 4)))                '152 YOBI1_T(WEB伝送ユーザ名)
                '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI2_T"), 4)))                '153 YOBI2_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI3_T"), 4)))                '154 YOBI3_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI4_T"), 4)))                '155 YOBI4_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI5_T"), 4)))                '156 YOBI5_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI6_T"), 4)))                '157 YOBI6_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI7_T"), 4)))                '158 YOBI7_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI8_T"), 4)))                '159 YOBI8_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI9_T"), 4)))                '160 YOBI9_T
                SQL.Append(", " & SQ(sTORI(GetIndex("YOBI10_T"), 4)))               '161 YOBI10_T
                SQL.Append(")")

                Ret = MainDB.ExecuteNonQuery(SQLBasic.ToString & SQL.ToString)
                If Ret = 1 Then
                Else
                    Throw New Exception("取引先マスタに登録できませんでした。(登録件数=" & Ret & ")")
                End If
            Next Cnt

            '=============================================
            ' 取引先マスタ(固有情報) <TORIMAST_SUB> 登録
            '=============================================
            SQLBasic = New StringBuilder
            SQLBasic.Append("INSERT INTO TORIMAST_SUB ( ")
            SQLBasic.Append("   FSYORI_KBN_TSUB")
            SQLBasic.Append(" , TORIS_CODE_TSUB")
            SQLBasic.Append(" , TORIF_CODE_TSUB")
            SQLBasic.Append(" , AITE_CNT_CODE_T")
            SQLBasic.Append(" , TOHO_CNT_CODE_T")
            SQLBasic.Append(" , DENSO_FILE_ID_T")
            SQLBasic.Append(" , HANSOU_KBN_T")
            SQLBasic.Append(" , HANSOU_ROOT1_T")
            SQLBasic.Append(" , HANSOU_ROOT2_T")
            SQLBasic.Append(" , HANSOU_ROOT3_T")
            SQLBasic.Append(" , HENKYAKU_SIT_NO_T")
            SQLBasic.Append(" , SYOUGOU_KBN_T")
            SQLBasic.Append(" , KEIYAKU_NO_T")
            SQLBasic.Append(" , MAE_SYORI_T")
            SQLBasic.Append(" , ATO_SYORI_T")
            SQLBasic.Append(" , TOKKIJIKOU1_T")
            SQLBasic.Append(" , TOKKIJIKOU2_T")
            SQLBasic.Append(" , TOKKIJIKOU3_T")
            SQLBasic.Append(" , TOKKIJIKOU4_T")
            For i As Integer = 1 To 50 Step 1
                SQLBasic.Append(" , CUSTOM_NUM" & Format(i, "00") & "_T")
            Next
            For i As Integer = 1 To 50 Step 1
                SQLBasic.Append(" , CUSTOM_VCR" & Format(i, "00") & "_T")
            Next
            SQLBasic.Append(" ) VALUES ( ")

            For Cnt = 1 To 4 Step 1
                SQL = New StringBuilder
                SQL.Append("   '1'")                                                ' FSYORI_KBN_TSUB
                SQL.Append(" , " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))         ' TORIS_CODE_TSUB
                SQL.Append(" , " & SQ(Cnt.ToString.PadLeft(2, "0"c)))               ' TORIF_CODE_TSUB
                SQL.Append(" , ''")                                                 ' AITE_CNT_CODE_T
                SQL.Append(" , ''")                                                 ' TOHO_CNT_CODE_T
                SQL.Append(" , ''")                                                 ' DENSO_FILE_ID_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("HANSOU_KBN_T"), 4)))          ' HANSOU_KBN_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("HANSOU_ROOT1_T"), 4)))        ' HANSOU_ROOT1_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("HANSOU_ROOT2_T"), 4)))        ' HANSOU_ROOT2_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("HANSOU_ROOT3_T"), 4)))        ' HANSOU_ROOT3_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("HENKYAKU_SIT_NO_T"), 4)))     ' HENKYAKU_SIT_NO_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("SYOUGOU_KBN_T"), 4)))         ' SYOUGOU_KBN_T
                SQL.Append(" , " & SQ(sTORI(GetIndex("KEIYAKU_NO_T"), 4)))          ' KEIYAKU_NO_T
                SQL.Append(" , ''")                                                 ' MAE_SYORI_T
                SQL.Append(" , ''")                                                 ' ATO_SYORI_T
                SQL.Append(" , ''")                                                 ' TOKKIJIKOU1_T
                SQL.Append(" , ''")                                                 ' TOKKIJIKOU2_T
                SQL.Append(" , ''")                                                 ' TOKKIJIKOU3_T
                SQL.Append(" , ''")                                                 ' TOKKIJIKOU4_T
                For i As Integer = 1 To 50 Step 1                                   ' CUSTOM_NUM01_T ～ CUSTOM_NUM50_T
                    SQL.Append(" , ''")
                Next
                For i As Integer = 1 To 50 Step 1                                   ' CUSTOM_VCR01_T ～ CUSTOM_VCR50_T
                    SQL.Append(" , ''")
                Next
                SQL.Append(" ) ")

                Ret = MainDB.ExecuteNonQuery(SQLBasic.ToString & SQL.ToString)
                If Ret = 1 Then
                Else
                    Throw New Exception("取引先マスタ(固有情報)に登録できませんでした。(登録件数=" & Ret & ")")
                End If
            Next Cnt

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録処理)", "失敗", ex.ToString)

            Return False
        End Try

    End Function

#End Region

#Region " 更新"

    Private Sub CmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUpdate.Click
        Dim MSG As String
        SyoriKBN = 1
        Try
            PrintCnt = 0
            LW.ToriCode = Me.GAKKOU_CODE_T.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            MainDB = New MyOracle
            Me.SuspendLayout()

            If KESSAI_Disused() = False Then
                Return
            End If

            '設定値の参照(取得)
            Call GetControlsValue()

            '参照値の検査(主に相関チェック)
            MSG = ""
            Dim Ret As Integer = CheckMutualRelation(MSG)
            If Ret = -1 Then

                '変更登録の有無チェック
                If Not CheckUpdateItem(1) Then
                    MSG = MSG0040I
                    If MessageBox.Show(G_MSG0018I, msgTitle, _
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.No Then
                        Call SetFocusOnErrorControl(GetIndex("ITAKU_CODE_T"))
                        Return
                    End If
                End If
            Else
                If ErrMsgFlg = True Then '不要な場合は表示しない
                    If MSG = "" Then
                        MSG = String.Format(MSG0281W, sTORI(Ret, 0))
                    End If
                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                If Ret < 0 Then
                    Application.DoEvents()
                    Me.SSTab.SelectedIndex = 0
                Else
                    Application.DoEvents()
                    Call SetFocusOnErrorControl(Ret)
                End If
                Return
            End If

            '先行レコードのチェック
            Select Case CheckRecordModule()
                Case 1
                    '更新処理
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons. _
                       YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        Return
                    End If
                    MainDB.BeginTrans()
                    Dim Counter As Integer = 0
                    Dim FLG As Boolean = UpdateModule(Counter)
                    If FLG Then

                        '学校マスタ１を登録
                        FLG = INSERT_GAKMAST1(1)

                        If Not FLG Then
                            MSG = "学校マスタ１"
                        End If

                        If FLG AndAlso Counter > 0 Then

                            'G_SCHMASTを更新
                            FLG = PFUNC_UPDATE_G_SCHMAST()

                            If Not FLG Then
                                MSG = "学校スケジュールマスタ・スケジュールマスタ"
                            End If

                            If FLG Then

                                '取引先マスタ(自振)を更新
                                FLG = PFUNC_UPDATE_TORIMAST()
                                If Not FLG Then
                                    MSG = "取引先マスタ"
                                End If
                            End If
                        End If
                    Else
                        MSG = "学校マスタ２"
                    End If

                    If FLG Then
                        MainDB.Commit()
                        MSG = MSG0006I

                        Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

                        GAKKOU_CODE_T.Text = sTORI(GetIndex("GAKKOU_CODE_T"), 3)

                        '印刷処理を起動する(失敗しても以降の処理は実行する)    
                        If PrintCnt > 0 Then
                            Call fn_Print(2)
                        End If

                        '取り敢えずロック解除モードにする。
                        Call SetControlToReadOnly(True)
                        Me.GAKKOU_CODE_T.Focus()
                    Else
                        MSG = String.Format(MSG0027E, MSG, "更新")
                        Call MessageBox.Show(MSG, msgTitle, _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                    End If
                Case 0
                    MSG = G_MSG0002W

                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MSG = String.Format(MSG0002E, "検索")

                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub

    '
    ' 機能　　 : 取引先マスタへの登録（既存更新）処理
    '
    ' 戻り値　 : 成功 = True
    ' 　　　　   失敗 = False
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function UpdateModule(ByRef Counter As Integer) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim Cnt As Integer
        '再振種別と持越種別
        Dim DB_VALUE As Integer
        Dim SFURI_SYUBETU As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T)
        Dim SFURI_SYUBETU_SUB As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T_SUB)

        If updCnt = 0 Then
            Return True
        End If

        Call GET_SET_SFURI_SYUBETU(1, DB_VALUE, SFURI_SYUBETU, SFURI_SYUBETU_SUB)
        sTORI(GetIndex("SAKUSEI_DATE_T"), 4) = sTORI(GetIndex("SAKUSEI_DATE_T"), 3)
        sTORI(GetIndex("KOUSIN_DATE_T"), 4) = String.Format("{0:yyyyMMddHHmmss}", Date.Now)

        Try
            '=============================================
            ' 学校マスタ２ <GAKMAST2> 更新
            '=============================================
            SQL.Length = 0
            SQL.Append("UPDATE GAKMAST2")
            SQL.Append(" SET KOUSIN_DATE_T = TO_CHAR(SYSDATE, 'yyyymmdd')")

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1

                If Not TypeOf oTORI(Cnt) Is Label Then
                    If sTORI(Cnt, 1).ToUpper = "SFURI_SYUBETU_T" Then  '再振種別=何もしない
                        Counter += 1
                    ElseIf sTORI(Cnt, 1).ToUpper = "GAKKOU_CODE_T" Then    '学校コード=何もしない
                    ElseIf sTORI(Cnt, 1).ToUpper = "GAKKOU_NNAME_G" OrElse sTORI(Cnt, 1).ToUpper = "GAKKOU_KNAME_G" Then
                        '学校名は更新の必要なし
                    ElseIf sTORI(Cnt, 1).ToUpper = "SAKUSEI_DATE_T" Then   '作成日=何もしない
                    ElseIf sTORI(Cnt, 1).ToUpper = "KOUSIN_DATE_T" Then    '更新日=何もしない
                    Else
                        If Not sTORI(Cnt, 3) = sTORI(Cnt, 4) Then
                            '変更された項目のみ更新する。
                            SQL.Append(", " & sTORI(Cnt, 1))             '項目名
                            SQL.Append(" = " & SetColumnData(Cnt))       '項目値
                            Counter += 1
                        End If
                    End If
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"

                            SQL.Append(", " & sTORI(Cnt, 1))             '項目名
                            SQL.Append(" = " & SetColumnData(Cnt))      '項目値
                    End Select
                End If
            Next Cnt
            SQL.Append(", SFURI_SYUBETU_T = " & SQ(DB_VALUE))

            '更新条件
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SetColumnData(GetIndex("GAKKOU_CODE_T")))

            '--------------------------------------------
            ' 更新項目が１項目以上の時、更新処理
            '--------------------------------------------
            If Counter > 0 Then
                Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
                If Ret <> 1 Then '更新失敗
                    MessageBox.Show(String.Format(MSG0027E, "学校マスタ２", "更新"), msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            End If

            '=============================================
            ' 学校マスタ２(固有情報) <GAKMAST2_SUB> 更新
            '=============================================
            Dim SubMast_Counter As Integer = 0
            SQL.Length = 0
            SQL.Append("UPDATE GAKMAST2_SUB SET")
            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                If Not sTORI(Cnt, 3) = sTORI(Cnt, 4) Then
                    '変更された項目のみ更新する。
                    Select Case SubMast_Counter
                        Case 0
                            SQL.Append("  " & sTORI(Cnt, 1) & " = " & SetColumnData(Cnt))  '項目名/変更値
                        Case Else
                            SQL.Append(", " & sTORI(Cnt, 1) & " = " & SetColumnData(Cnt))  '項目名/変更値
                    End Select
                    SubMast_Counter += 1
                End If
            Next
            SQL.Append(" WHERE ")
            SQL.Append("     GAKKOU_CODE_TSUB = " & SetColumnData(GetIndex("GAKKOU_CODE_T")))

            '--------------------------------------------
            ' 更新項目が１項目以上の時、更新処理
            '--------------------------------------------
            If SubMast_Counter > 0 Then
                Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
                If Ret <> 1 Then '更新失敗
                    MessageBox.Show(String.Format(MSG0027E, "学校マスタ２(固有情報)", "更新"), msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            End If

            Counter = Counter + SubMast_Counter

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, _
                 MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新処理)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    '----------------------------------------------------------------------------
    'Name       :PFUNC_UPDATE_G_SCHMAST
    'Description:学校スケジュールマスタ更新
    'Parameta   :
    'Create     :
    'Update     :
    '　　　     :
    '----------------------------------------------------------------------------
    Private Function PFUNC_UPDATE_G_SCHMAST() As Boolean
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim SQL As New StringBuilder
        Dim SQLCode As Integer

        Try
            For Cnt = 0 To 3 Step 1
                SQL = New StringBuilder
                SQL.Append("UPDATE G_SCHMAST")
                SQL.Append(" SET BAITAI_CODE_S = " & SQ(sTORI(GetIndex("BAITAI_CODE_T"), 4)))
                '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------START
                SQL.Append(", ITAKU_CODE_S = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4)))
                'SQL.Append(", ITAKU_CODE_S = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4).Substring(1, 9) & Cnt))
                '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------END
                SQL.Append(", TKIN_NO_S =" & SQ(sTORI(GetIndex("TKIN_NO_T"), 4)))
                SQL.Append(", TSIT_NO_S =" & SQ(sTORI(GetIndex("TSIT_NO_T"), 4)))
                SQL.Append(", TESUU_KBN_S = " & SQ(sTORI(GetIndex("TESUUTYO_KBN_T"), 4)))
                SQL.Append(" WHERE GAKKOU_CODE_S =" & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))
                SQL.Append(" AND FURI_KBN_S = " & SQ(Cnt.ToString))
                Ret = MainDB.ExecuteNonQuery(SQL)

                SQL = New StringBuilder
                SQL.Append("UPDATE SCHMAST")
                SQL.Append(" SET BAITAI_CODE_S = '07'")
                '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------START
                SQL.Append(", ITAKU_CODE_S = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4)))
                'SQL.Append(", ITAKU_CODE_S = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4).Substring(1, 9) & Cnt))
                '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------END
                SQL.Append(", TKIN_NO_S =" & SQ(sTORI(GetIndex("TKIN_NO_T"), 4)))
                SQL.Append(", TSIT_NO_S =" & SQ(sTORI(GetIndex("TSIT_NO_T"), 4)))
                SQL.Append(", TESUU_KBN_S = " & SQ(sTORI(GetIndex("TESUUTYO_KBN_T"), 4)))
                SQL.Append(" WHERE TORIS_CODE_S = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))
                '2017/08/14 saitou 上田信金(RSV2標準) MODIFY 自振連携不具合修正 ------------------------------------- START
                'スケジュール更新間違い。
                SQL.Append(" AND TORIF_CODE_S = " & SQ("0" & (Cnt + 1).ToString))
                'SQL.Append(" AND TORIF_CODE_S = " & SQ("0" & Cnt.ToString))
                '2017/08/14 saitou 上田信金(RSV2標準) MODIFY -------------------------------------------------------- END
                Ret = MainDB.ExecuteNonQuery(SQL)
            Next Cnt

            Return (SQLCode = 0)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return False
    End Function

    '----------------------------------------------------------------------------
    'Name       :PFUNC_UPDATE_TORIMAST
    'Description:取引先マスタ更新
    'Parameta   :
    'Create     :
    'Update     :
    '           :
    '　　　     :
    '----------------------------------------------------------------------------
    Private Function PFUNC_UPDATE_TORIMAST() As Boolean

        Dim SQL As StringBuilder
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim DayCnt As Integer

        Try
            For Cnt = 1 To 4 Step 1
                SQL = New StringBuilder
                '=============================================
                ' 取引先マスタ <TORIMAST> 更新
                '=============================================
                SQL.Length = 0
                SQL.Append("UPDATE TORIMAST SET ")
                SQL.Append(" FSYORI_KBN_T = '1'")                                                   '1 FSYORI_KBN_T
                SQL.Append(",TORIS_CODE_T = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))            '2 TORIS_CODE_T
                SQL.Append(",TORIF_CODE_T = " & SQ(Cnt.ToString.PadLeft(2, "0"c)))                  '3 TORIF_CODE_T
                SQL.Append(",BAITAI_CODE_T ='07'")                                                  '4 BAITAI_CODE_T
                SQL.Append(",LABEL_KBN_T = '0'")                                                    '5 LABEL_KBN_T
                SQL.Append(",CODE_KBN_T = '0'")                                                     '6 CODE_KBN_T
                SQL.Append(",ITAKU_KANRI_CODE_T = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4)))       '7 ITAKU_KANRI_CODE_T
                SQL.Append(",FILE_NAME_T = " & SQ(sTORI(GetIndex("FILE_NAME_T"), 4)))               '8 FILE_NAME_T
                SQL.Append(",FMT_KBN_T = '00'")                                                     '9 FMT_KBN_T
                SQL.Append(",MULTI_KBN_T = '0'")                                                    '10 MULTI_KBN_T
                If Cnt = 3 Then
                    SQL.Append(",NS_KBN_T = '1'") '入金                                             '11 NS_KBN_T
                    SQL.Append(",SYUBETU_T = '21'")                                                 '12 SYUBETU_T
                Else
                    SQL.Append(",NS_KBN_T = '9'") '出金                                             '11 NS_KBN_T
                    SQL.Append(",SYUBETU_T = '91'")                                                 '12 SYUBETU_T
                End If

                '2010/09/08.Sakon　委託者コードの下１桁変更を行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                SQL.Append(",ITAKU_CODE_T = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4))) '13 ITAKU_CODE_T
                'SQL.Append(",ITAKU_CODE_T = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4).Substring(0, 9) & GCom.NzStr(Cnt01))) '13 ITAKU_CODE_T
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                SQL.Append(",ITAKU_KNAME_T = " & SQ(sTORI(GetIndex("GAKKOU_KNAME_G"), 4)))          '14 ITAKU_KNAME_T
                SQL.Append(",TKIN_NO_T = " & SQ(sTORI(GetIndex("TKIN_NO_T"), 4)))                   '15 TKIN_NO_T
                SQL.Append(",TSIT_NO_T = " & SQ(sTORI(GetIndex("TSIT_NO_T"), 4)))                   '16 TSIT_NO_T
                SQL.Append(",KAMOKU_T = " & SQ(sTORI(GetIndex("KAMOKU_T"), 4)))                     '17 KAMOKU_T
                SQL.Append(",KOUZA_T = " & SQ(sTORI(GetIndex("KOUZA_T"), 4)))                       '18 KOUZA_T
                SQL.Append(",MOTIKOMI_KBN_T = '0'")                                                 '19 MOTIKOMI_KBN_T
                SQL.Append(",SOUSIN_KBN_T = '0'")                                                   '20 SOUSIN_KBN_T
                SQL.Append(",TAKO_KBN_T = " & SQ(sTORI(GetIndex("TAKO_KBN_T"), 4)))                 '21 TAKO_KBN_T
                SQL.Append(",JIFURICHK_KBN_T = " & SQ(sTORI(GetIndex("JIFURICHK_KBN_T"), 4)))       '22 JIFURICHK_KBN_T
                SQL.Append(",TEKIYOU_KBN_T = " & SQ(sTORI(GetIndex("TEKIYOU_KBN_T"), 4)))           '23 TEKIYOU_KBN_T
                SQL.Append(",KTEKIYOU_T = " & SQ(sTORI(GetIndex("KTEKIYOU_T"), 4)))                 '24 KTEKIYOU_T
                SQL.Append(",NTEKIYOU_T = " & SQ(sTORI(GetIndex("NTEKIYOU_T"), 4)))                 '25 NTEKIYOU_T
                SQL.Append(",FURI_CODE_T = " & SQ(sTORI(GetIndex("FURI_CODE_T"), 4)))               '26 FURI_CODE_T
                SQL.Append(",KIGYO_CODE_T = " & SQ(sTORI(GetIndex("KIGYO_CODE_T"), 4)))             '27 KIGYO_CODE_T
                SQL.Append(",ITAKU_NNAME_T = " & SQ(sTORI(GetIndex("GAKKOU_NNAME_G"), 4)))          '28 ITAKU_NNAME_T
                SQL.Append(",YUUBIN_T = " & SQ(sTORI(GetIndex("YUUBIN_T"), 4)))                     '29 YUUBIN_T
                SQL.Append(",DENWA_T = " & SQ(sTORI(GetIndex("DENWA_T"), 4)))                       '30 DENWA_T
                SQL.Append(",FAX_T = " & SQ(sTORI(GetIndex("FAX_T"), 4)))                           '31 FAX_T
                SQL.Append(",KOKYAKU_NO_T = " & SQ(sTORI(GetIndex("KOKYAKU_NO_T"), 4)))             '32 KOKYAKU_NO_T
                SQL.Append(",KANREN_KIGYO_CODE_T = " & SQ(sTORI(GetIndex("KANREN_KIGYO_CODE_T"), 4))) '33 KANREN_KIGYO_CODE_T
                SQL.Append(",ITAKU_NJYU_T = " & SQ(sTORI(GetIndex("ITAKU_NJYU_T"), 4)))             '34 ITAKU_NJYU_T
                SQL.Append(",YUUBIN_KNAME_T = ''")                                                  '35 YUUBIN_KNAME_T
                SQL.Append(",YUUBIN_NNAME_T =''")                                                   '36 YUUBIN_NNAME_T

                Select Case Cnt                                                                     '37 FURI_KYU_CODE_T
                    Case 3
                        SQL.Append(",FURI_KYU_CODE_T =" & SQ(sTORI(GetIndex("NKYU_CODE_T"), 4)))
                    Case Else
                        SQL.Append(",FURI_KYU_CODE_T =" & SQ(sTORI(GetIndex("SKYU_CODE_T"), 4)))
                End Select

                Select Case Cnt                                                                     '38 DATE1_T ～ 68 DATE31_T
                    Case 1
                        '初振
                        Ret = GCom.NzInt(sTORI(GetIndex("FURI_DATE_T"), 4))
                        For DayCnt = 1 To 31 Step 1
                            If DayCnt = Ret Then
                                SQL.Append(",DATE" & DayCnt.ToString & "_T = '1'")
                            Else
                                SQL.Append(",DATE" & DayCnt.ToString & "_T = '0'")
                            End If
                        Next DayCnt
                    Case 2
                        '再振
                        Ret = GCom.NzInt(sTORI(GetIndex("SFURI_DATE_T"), 4))
                        For DayCnt = 1 To 31 Step 1
                            If DayCnt = Ret Then
                                SQL.Append(",DATE" & DayCnt.ToString & "_T = '1'")
                            Else
                                SQL.Append(",DATE" & DayCnt.ToString & "_T = '0'")
                            End If
                        Next DayCnt

                    Case 3, 4
                        For DayCnt = 1 To 31 Step 1
                            SQL.Append(",DATE" & DayCnt.ToString & "_T = '0'")
                        Next DayCnt
                End Select

                Select Case Cnt
                    Case 1, 2
                        Select Case True
                            Case (Cnt = 2 AndAlso GCom.GetComboBox(Me.SFURI_SYUBETU_T) = 0)
                                '再振なし
                                For No As Integer = 1 To 12
                                    SQL.Append(",TUKI" & No.ToString & "_T = '0'")                  '69 TUKI1_T ～ 80 TUKI12_T
                                Next
                            Case Else
                                For No As Integer = 1 To 12
                                    SQL.Append(",TUKI" & No.ToString & "_T = '1'")                  '69 TUKI1_T ～ 80 TUKI12_T
                                Next
                        End Select
                    Case 3, 4
                        '-03(入金)と-04(臨時出金)の場合は月別フラグを使用しない。
                        For No As Integer = 1 To 12
                            SQL.Append(",TUKI" & No.ToString & "_T = '0'")                          '69 TUKI1_T ～ 80 TUKI12_T
                        Next
                End Select
                SQL.Append(",SFURI_FLG_T = '0'")                                                    '81 SFURI_FLG_T
                SQL.Append(",SFURI_FCODE_T = ''")                                                   '82 SFURI_FCODE_T
                SQL.Append(",SFURI_DAY_T = 0")                                                      '83 SFURI_DAY_T
                SQL.Append(",SFURI_KIJITSU_T = '0'")                                                '84 SFURI_KIJITSU_T
                SQL.Append(",SFURI_KYU_CODE_T = " & SQ(sTORI(GetIndex("SFURI_KYU_CODE_T"), 4)))     '85 SFURI_KYU_CODE_T
                SQL.Append(",KEIYAKU_DATE_T = " & SQ(sTORI(GetIndex("KEIYAKU_DATE_T"), 4)))         '86 KEIYAKU_DATE_T
                SQL.Append(",KAISI_DATE_T = " & SQ(sTORI(GetIndex("KAISI_DATE_T"), 4)))             '87 KAISI_DATE_T
                SQL.Append(",SYURYOU_DATE_T = " & SQ(sTORI(GetIndex("SYURYOU_DATE_T"), 4)))         '88 SYURYOU_DATE_T
                SQL.Append(",MOTIKOMI_KIJITSU_T =" & SQ(sTORI(GetIndex("MOTIKOMI_KIJITSU_T"), 4)))  '89 MOTIKOMI_KIJITSU_T
                SQL.Append(",IRAISYO_YDATE_T = '0'")                                                '90 IRAISYO_YDATE_T
                SQL.Append(",IRAISYO_KIJITSU_T = '0'")                                              '91 IRAISYO_KIJITSU_T
                SQL.Append(",IRAISYO_KYU_CODE_T = '0'")                                             '92 IRAISYO_KYU_CODE_T
                SQL.Append(",IRAISYO_KBN_T = '0'")                                                  '93 IRAISYO_KBN_T
                SQL.Append(",IRAISYO_SORT_T = '0'")                                                 '94 IRAISYO_SORT_T
                SQL.Append(",TEIGAKU_KBN_T = '0'")                                                  '95 TEIGAKU_KBN_T
                SQL.Append(",UMEISAI_KBN_T = '0'")                                                  '96 UMEISAI_KBN_T
                SQL.Append(",FUNOU_MEISAI_KBN_T = '0'")                                             '97 FUNOU_MEISAI_KBN_T
                SQL.Append(",KEKKA_HENKYAKU_KBN_T = '0'")                                           '98 KEKKA_HENKYAKU_KBN_T
                SQL.Append(",KEKKA_MEISAI_KBN_T = '0'")                                             '99 KEKKA_MEISAI_KBN_T
                SQL.Append(",PRTNUM_T = 0")                                                         '100 PRTNUM_T
                SQL.Append(",FKEKKA_TBL_T = " & SQ(sTORI(GetIndex("FKEKKA_TBL_T"), 4)))             '101 FKEKKA_TBL_T
                SQL.Append(",KESSAI_KBN_T = " & SQ(sTORI(GetIndex("KESSAI_KBN_T"), 4)))             '102 KESSAI_KBN_T
                SQL.Append(",TORIMATOME_SIT_T = " & SQ(sTORI(GetIndex("TORIMATOME_SIT_T"), 4)))     '103 TORIMATOME_SIT_T
                SQL.Append(",HONBU_KOUZA_T = " & SQ(sTORI(GetIndex("HONBU_KOUZA_T"), 4)))           '104 HONBU_KOUZA_T
                SQL.Append(",KESSAI_DAY_T = " & SQ(sTORI(GetIndex("KESSAI_DAY_T"), 4)))             '105 KESSAI_DAY_T
                SQL.Append(",KESSAI_KIJITSU_T = " & SQ(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)))     '106 KESSAI_KIJITSU_T
                SQL.Append(",KESSAI_KYU_CODE_T = " & SQ(sTORI(GetIndex("KESSAI_KYU_CODE_T"), 4)))   '107 KESSAI_KYU_CODE_T
                SQL.Append(",TUKEKIN_NO_T = " & SQ(sTORI(GetIndex("TUKEKIN_NO_T"), 4)))             '108 TUKEKIN_NO_T
                SQL.Append(",TUKESIT_NO_T = " & SQ(sTORI(GetIndex("TUKESIT_NO_T"), 4)))             '109 TUKESIT_NO_T
                SQL.Append(",TUKEKAMOKU_T = " & SQ(sTORI(GetIndex("TUKEKAMOKU_T"), 4)))             '110 TUKEKAMOKU_T
                SQL.Append(",TUKEKOUZA_T = " & SQ(sTORI(GetIndex("TUKEKOUZA_T"), 4)))               '111 TUKEKOUZA_T
                SQL.Append(",TUKEMEIGI_KNAME_T = " & SQ(sTORI(GetIndex("TUKEMEIGI_T"), 4)))         '112 TUKEMEIGI_KNAME_T
                SQL.Append(",BIKOU1_T = " & SQ(sTORI(GetIndex("DENPYO_BIKOU1_T"), 4)))              '113 BIKOU1_T
                SQL.Append(",BIKOU2_T = " & SQ(sTORI(GetIndex("DENPYO_BIKOU2_T"), 4)))              '114 BIKOU2_T
                SQL.Append(",TESUUTYO_SIT_T = " & SQ(sTORI(GetIndex("TESUUTYO_SIT_T"), 4)))         '115 TESUUTYO_SIT_T
                SQL.Append(",TESUUTYO_KAMOKU_T = " & SQ(sTORI(GetIndex("TESUUTYO_KAMOKU_T"), 4)))   '116 TESUUTYO_KAMOKU_T
                SQL.Append(",TESUUTYO_KOUZA_T = " & SQ(sTORI(GetIndex("TESUUTYO_KOUZA_T"), 4)))     '117 TESUUTYO_KOUZA_T
                SQL.Append(",TESUUTYO_KBN_T = " & SQ(sTORI(GetIndex("TESUUTYO_KBN_T"), 4)))         '118 TESUUTYO_KBN_T
                SQL.Append(",TESUUTYO_PATN_T = " & SQ(sTORI(GetIndex("TESUUTYO_PATN_T"), 4)))       '119 TESUUTYO_PATN_T
                SQL.Append(",TESUUMAT_NO_T = " & SQ(sTORI(GetIndex("TESUUMAT_NO_T"), 4)))           '120 TESUUMAT_NO_T
                SQL.Append(",TESUUTYO_DAY_T = " & SQ(sTORI(GetIndex("TESUUTYO_DAY_T"), 4)))         '121 TESUUTYO_DAY_T
                SQL.Append(",TESUUTYO_KIJITSU_T = " & SQ(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4))) '122 TESUUTYO_KIJITSU_T
                SQL.Append(",TESUU_KYU_CODE_T = " & SQ(sTORI(GetIndex("TESUU_KYU_CODE_T"), 4)))     '123 TESUU_KYU_CODE_T
                SQL.Append(",SEIKYU_KBN_T = " & SQ(sTORI(GetIndex("SEIKYU_KBN_T"), 4)))             '124 SEIKYU_KBN_T
                SQL.Append(",KIHTESUU_T = " & SQ(sTORI(GetIndex("KIHTESUU_T"), 4)))                 '125 KIHTESUU_T
                SQL.Append(",SYOUHI_KBN_T = " & SQ(sTORI(GetIndex("SYOUHI_KBN_T"), 4)))             '126 SYOUHI_KBN_T
                SQL.Append(",SOURYO_T = " & SQ(sTORI(GetIndex("SOURYO_T"), 4)))                     '127 SOURYO_T
                SQL.Append(",KOTEI_TESUU1_T = " & SQ(sTORI(GetIndex("KOTEI_TESUU1_T"), 4)))         '128 KOTEI_TESUU1_T
                SQL.Append(",KOTEI_TESUU2_T = " & SQ(sTORI(GetIndex("KOTEI_TESUU2_T"), 4)))         '129 KOTEI_TESUU2_T
                SQL.Append(",TESUUMAT_MONTH_T = " & SQ(sTORI(GetIndex("TESUUMAT_MONTH_T"), 4)))     '130 TESUUMAT_MONTH_T
                SQL.Append(",TESUUMAT_ENDDAY_T = " & SQ(sTORI(GetIndex("TESUUMAT_ENDDAY_T"), 4)))   '131 TESUUMAT_ENDDAY_T
                SQL.Append(",TESUUMAT_KIJYUN_T = " & SQ(sTORI(GetIndex("TESUUMAT_KIJYUN_T"), 4)))   '132 TESUUMAT_KIJYUN_T
                SQL.Append(",TESUUMAT_PATN_T = " & SQ(sTORI(GetIndex("TESUUMAT_PATN_T"), 4)))       '133 TESUUMAT_PATN_T
                SQL.Append(",TESUU_GRP_T = " & SQ(sTORI(GetIndex("TESUU_GRP_T"), 4)))               '134 TESUU_GRP_T
                SQL.Append(",TESUU_TABLE_ID_T = " & SQ(sTORI(GetIndex("TESUU_TABLE_ID_T"), 4)))     '135 TESUU_TABLE_ID_T
                SQL.Append(",TESUU_A1_T = " & SQ(sTORI(GetIndex("TESUU_A1_T"), 4)))                 '136 TESUU_A1_T
                SQL.Append(",TESUU_A2_T = " & SQ(sTORI(GetIndex("TESUU_A2_T"), 4)))                 '137 TESUU_A2_T
                SQL.Append(",TESUU_A3_T = " & SQ(sTORI(GetIndex("TESUU_A3_T"), 4)))                 '138 TESUU_A3_T
                SQL.Append(",TESUU_B1_T = " & SQ(sTORI(GetIndex("TESUU_B1_T"), 4)))                 '139 TESUU_B1_T
                SQL.Append(",TESUU_B2_T = " & SQ(sTORI(GetIndex("TESUU_B2_T"), 4)))                 '140 TESUU_B2_T
                SQL.Append(",TESUU_B3_T = " & SQ(sTORI(GetIndex("TESUU_B3_T"), 4)))                 '141 TESUU_B3_T
                SQL.Append(",TESUU_C1_T = " & SQ(sTORI(GetIndex("TESUU_C1_T"), 4)))                 '142 TESUU_C1_T
                SQL.Append(",TESUU_C2_T = " & SQ(sTORI(GetIndex("TESUU_C2_T"), 4)))                 '143 TESUU_C2_T
                SQL.Append(",TESUU_C3_T = " & SQ(sTORI(GetIndex("TESUU_C3_T"), 4)))                 '144 TESUU_C3_T
                SQL.Append(",ENC_KBN_T = '0'")                                                      '145 ENC_KBN_T
                SQL.Append(",ENC_OPT1_T = '0'")                                                     '146 ENC_OPT1_T
                SQL.Append(",ENC_KEY1_T = ''")                                                      '147 ENC_KEY1_T
                SQL.Append(",ENC_KEY2_T = ''")                                                      '148 ENC_KEY2_T
                SQL.Append(",KOUSIN_SIKIBETU_T = '0'")                                              '149 KOUSIN_SIKIBETU_T
                SQL.Append(",SAKUSEI_DATE_T = " & SQ(sTORI(GetIndex("SAKUSEI_DATE_T"), 4)))         '150 SAKUSEI_DATE_T
                SQL.Append(",KOUSIN_DATE_T = " & SQ(sTORI(GetIndex("KOUSIN_DATE_T"), 4)))           '151 KOUSIN_DATE_T
                '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                SQL.Append(",YOBI1_T = " & SQ(sTORI(GetIndex("YOBI1_T"), 4)))                       '152 YOBI1_T(WEB伝送ユーザ名)
                '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
                SQL.Append(",YOBI2_T = " & SQ(sTORI(GetIndex("YOBI2_T"), 4)))                       '153 YOBI2_T
                SQL.Append(",YOBI3_T = " & SQ(sTORI(GetIndex("YOBI3_T"), 4)))                       '154 YOBI3_T
                SQL.Append(",YOBI4_T = " & SQ(sTORI(GetIndex("YOBI4_T"), 4)))                       '155 YOBI4_T
                SQL.Append(",YOBI5_T = " & SQ(sTORI(GetIndex("YOBI5_T"), 4)))                       '156 YOBI5_T
                SQL.Append(",YOBI6_T = " & SQ(sTORI(GetIndex("YOBI6_T"), 4)))                       '157 YOBI6_T
                SQL.Append(",YOBI7_T = " & SQ(sTORI(GetIndex("YOBI7_T"), 4)))                       '158 YOBI7_T
                SQL.Append(",YOBI8_T = " & SQ(sTORI(GetIndex("YOBI8_T"), 4)))                       '159 YOBI8_T
                SQL.Append(",YOBI9_T = " & SQ(sTORI(GetIndex("YOBI9_T"), 4)))                       '160 YOBI9_T
                SQL.Append(",YOBI10_T = " & SQ(sTORI(GetIndex("YOBI10_T"), 4)))                     '161 YOBI10_T

                SQL.Append(" WHERE FSYORI_KBN_T ='1'")
                SQL.Append(" AND TORIS_CODE_T = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Cnt.ToString("0#")))
                Ret = MainDB.ExecuteNonQuery(SQL)

                If Not Ret = 1 Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ 更新", "失敗", _
                                  "取引先コード:" & sTORI(GetIndex("GAKKOU_CODE_T"), 4) & "-" & Cnt.ToString("0#"))
                    Exit For
                End If

                '=============================================
                ' 取引先マスタ(固有情報) <TORIMAST_SUB> 更新
                '=============================================
                SQL.Length = 0
                SQL.Append("UPDATE TORIMAST_SUB SET ")
                SQL.Append("   HANSOU_KBN_T      = " & SQ(sTORI(GetIndex("HANSOU_KBN_T"), 4)))
                SQL.Append(" , HANSOU_ROOT1_T    = " & SQ(sTORI(GetIndex("HANSOU_ROOT1_T"), 4)))
                SQL.Append(" , HANSOU_ROOT2_T    = " & SQ(sTORI(GetIndex("HANSOU_ROOT2_T"), 4)))
                SQL.Append(" , HANSOU_ROOT3_T    = " & SQ(sTORI(GetIndex("HANSOU_ROOT3_T"), 4)))
                SQL.Append(" , HENKYAKU_SIT_NO_T = " & SQ(sTORI(GetIndex("HENKYAKU_SIT_NO_T"), 4)))
                SQL.Append(" , SYOUGOU_KBN_T     = " & SQ(sTORI(GetIndex("SYOUGOU_KBN_T"), 4)))
                SQL.Append(" , KEIYAKU_NO_T      = " & SQ(sTORI(GetIndex("KEIYAKU_NO_T"), 4)))
                SQL.Append(" WHERE ")
                SQL.Append("     FSYORI_KBN_TSUB ='1'")
                SQL.Append(" AND TORIS_CODE_TSUB = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 4)))
                SQL.Append(" AND TORIF_CODE_TSUB = " & SQ(Cnt.ToString("0#")))
                Ret = MainDB.ExecuteNonQuery(SQL)

                If Not Ret = 1 Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ(固有情報) 更新", "失敗", _
                                  "取引先コード:" & sTORI(GetIndex("GAKKOU_CODE_T"), 4) & "-" & Cnt.ToString("0#"))
                    Exit For
                End If
            Next Cnt

            Return (Ret = 1)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return False

    End Function

#End Region

#Region " 削除"
    Private Sub CmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdDelete.Click

        SyoriKBN = 3
        Dim MSG As String
        Dim SQL As New StringBuilder(128)
        Dim Print As Boolean = False
        Dim REC As OracleDataReader = Nothing
        Dim Code As String = "OK"

        Try
            MainDB = New MyOracle
            LW.ToriCode = Me.GAKKOU_CODE_T.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            MSG = MSG0007I
            If DialogResult.OK = MessageBox.Show(MSG, msgTitle, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then


                'スケジュールマスタに進行中のレコードが有るか否かを検索する。
                SQL.Append("SELECT COUNT(*) COUNTER FROM SCHMAST")
                SQL.Append(" WHERE TORIS_CODE_S = " & SQ(GAKKOU_CODE_T.Text.Trim))
                SQL.Append(" AND TORIF_CODE_S IN('01','02','03','04')")
                SQL.Append(" AND TOUROKU_FLG_S = '1'")                                   '登録中
                SQL.Append(" AND NVL(TYUUDAN_FLG_S, '0') = '0'")                         '中断なし
                SQL.Append(" AND NVL(HENKAN_FLG_S, '0') = '0'")                          '未返還
                If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then

                    If GCom.NzInt(REC.Item("COUNTER"), 0) > 0 Then
                        MSG = MSG0295W
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Else
                    MSG = String.Format(MSG0002E, "検索")

                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                '取引先マスタ削除情報印刷
                Print = fn_CreateCSV_Delete()

                MSG = "学校・取引先マスタ"

                '=============================================
                ' 学校マスタ１ <GAKMAST1> 削除
                '=============================================
                SQL = New StringBuilder(128)
                SQL.Length = 0
                SQL.Append("DELETE FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 3)))
                Dim Ret As Integer
                Ret = MainDB.ExecuteNonQuery(SQL)
                If Ret > 0 Then
                    '=============================================
                    ' 学校マスタ２ <GAKMAST2> 削除
                    '=============================================
                    SQL.Length = 0
                    SQL.Append("DELETE FROM GAKMAST2")
                    SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 3)))
                    Ret = MainDB.ExecuteNonQuery(SQL)
                    If Ret > 0 Then
                        '=============================================
                        ' 学校マスタ２(固有情報) <GAKMAST2_SUB> 削除
                        '=============================================
                        SQL.Length = 0
                        SQL.Append("DELETE FROM GAKMAST2_SUB")
                        SQL.Append(" WHERE GAKKOU_CODE_TSUB = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 3)))
                        Ret = MainDB.ExecuteNonQuery(SQL)
                    Else
                        MSG = "学校マスタ２"
                        Code = "NG"
                    End If
                Else
                    MSG = "学校マスタ１"
                    Code = "NG"
                End If

                If Ret = 1 AndAlso Code = "OK" Then
                    '=============================================
                    ' 取引先マスタ <TORIMAST> 削除
                    '=============================================
                    SQL.Length = 0
                    SQL.Append("DELETE FROM TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 3)))
                    '2017/08/14 saitou 上田信金(RSV2標準) MODIFY 自振連携不具合修正 ------------------------------------- START
                    '学校自振側で登録したレコードのみ対象とする。
                    SQL.Append(" AND TORIF_CODE_T IN ('01','02','03','04')")
                    '2017/08/14 saitou 上田信金(RSV2標準) MODIFY -------------------------------------------------------- END
                    Ret = MainDB.ExecuteNonQuery(SQL)
                    If Ret > 0 Then
                        '=============================================
                        ' 取引先マスタ(固有情報) <TORIMAST_SUB> 削除
                        '=============================================
                        SQL.Length = 0
                        SQL.Append("DELETE FROM TORIMAST_SUB")
                        SQL.Append(" WHERE TORIS_CODE_TSUB = " & SQ(sTORI(GetIndex("GAKKOU_CODE_T"), 3)))
                        '2017/08/14 saitou 上田信金(RSV2標準) MODIFY 自振連携不具合修正 ------------------------------------- START
                        '学校自振側で登録したレコードのみ対象とする。
                        SQL.Append(" AND TORIF_CODE_TSUB IN ('01','02','03','04')")
                        '2017/08/14 saitou 上田信金(RSV2標準) MODIFY -------------------------------------------------------- END
                        Ret = MainDB.ExecuteNonQuery(SQL)
                        If Ret > 0 Then
                        Else
                            MSG = "取引先マスタ(固有情報)"
                            Code = "NG"
                        End If
                    Else
                        MSG = "取引先マスタ"
                        Code = "NG"
                    End If
                Else
                    MSG = "学校マスタ２(固有情報)"
                    Code = "NG"
                End If

                If Ret > 1 AndAlso Code = "OK" Then
                    MainDB.Commit()
                    '学校コンボ設定（全学校）
                    If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                        MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If

                    MSG = MSG0008I
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    '---------------------------------------------------
                    '印刷実行
                    '---------------------------------------------------
                    If Print = True Then
                        Call fn_Print(3)
                    Else
                        MSG = String.Format(MSG0231W, "学校マスタメンテ(削除)")
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    MSG = String.Format(MSG0027E, MSG, "削除")
                    Call MessageBox.Show(MSG, msgTitle, _
                      MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainDB.Rollback()
                    Exit Sub
                End If

                Call FormInitializa()
                Call SetControlToReadOnly(True)
                SSTab.SelectedIndex = 0
                Me.GAKKOU_CODE_T.Focus()

            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub
#End Region

#Region " 参照"
    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click
        Try
            SyoriKBN = 2
            LW.ToriCode = Me.GAKKOU_CODE_T.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            Me.SuspendLayout()
            Dim CTL As TextBox
            Select Case CmdSelect.Text
                Case 参照モード
                    Select Case True
                        Case Not Me.GAKKOU_CODE_T.Text.Length = 10
                            CTL = Me.GAKKOU_CODE_T
                            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            GAKKOU_CODE_T.Focus()
                        Case Else
                            Call FormInitializa(1)
                            If CmdSelect_Action() = False Then
                                Return
                            End If
                    End Select
                Case 解除モード
                    Call SetControlToReadOnly(True)
                    SSTab.SelectedIndex = 0
                    Me.GAKKOU_CODE_T.Focus()
            End Select
            If KESSAI_Disused() = False Then
                Return
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub

    '
    ' 機　能 : 既存MAST参照共通処理
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : 学校マスタ参照結果の画面展開
    '    
    Public Function CmdSelect_Action() As Boolean
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim onDate As DateTime
        Dim onText(2) As Integer
        Dim Index As Integer
        Dim REC As OracleDataReader = Nothing
        Try
            CmdSelect_Action = False
            Me.SuspendLayout()
            Dim SQL As New StringBuilder(128)
            SQL.Append("SELECT *")
            SQL.Append(" FROM ")
            SQL.Append("     GAKMAST_VIEW")
            SQL.Append(" WHERE ")
            SQL.Append("     GAKKOU_CODE_T = " & SQ(Mid(LW.ToriCode, 1, 10)))

            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then

                For Idx As Integer = 0 To REC.FieldCount - 1 Step 1

                    '変更前の配列へ値を格納する
                    Index = GetIndex(REC.GetName(Idx))
                    sTORI(Index, 3) = REC(Idx).ToString.Trim

                    Select Case True
                        Case TypeOf oTORI(Index) Is TextBox
                            'テキストボックス

                            Select Case sTORI(Index, 1).ToUpper
                                Case "KAISI_DATE_T", "SYURYOU_DATE_T", "KEIYAKU_DATE_T"

                                    Dim OK_FLG As Boolean = False

                                    If sTORI(Index, 3).Length = 8 AndAlso _
                                        GCom.IsNumber(sTORI(Index, 3)) Then

                                        onText(0) = GCom.NzInt(sTORI(Index, 3).Substring(0, 4))
                                        onText(1) = GCom.NzInt(sTORI(Index, 3).Substring(4, 2))
                                        onText(2) = GCom.NzInt(sTORI(Index, 3).Substring(6))

                                        Ret = GCom.SET_DATE(onDate, onText)
                                        If Ret = -1 Then

                                            OK_FLG = True

                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .KAISI_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .KAISI_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .SYURYOU_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .SYURYOU_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .KEIYAKU_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .KEIYAKU_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                End Select
                                            End With
                                        Else
                                            If sTORI(Index, 1).ToUpper = "KEIYAKU_DATE_T" AndAlso sTORI(Index, 3) = "00000000" Then
                                                Me.KEIYAKU_DATE_T.Text = "0000"
                                                Me.KEIYAKU_DATE_T1.Text = "00"
                                                Me.KEIYAKU_DATE_T2.Text = "00"
                                                OK_FLG = True
                                            End If
                                        End If
                                    End If

                                    If Not OK_FLG Then

                                        If GCom.NzLong(sTORI(Index, 3)) = 99999999 Then
                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = "2000"
                                                        .KAISI_DATE_T1.Text = "01"
                                                        .KAISI_DATE_T2.Text = "01"
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = "2099"
                                                        .SYURYOU_DATE_T1.Text = "12"
                                                        .SYURYOU_DATE_T2.Text = "31"
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = "2000"
                                                        .KEIYAKU_DATE_T1.Text = "01"
                                                        .KEIYAKU_DATE_T2.Text = "01"
                                                End Select
                                            End With
                                        Else
                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = "2000"
                                                        .KAISI_DATE_T1.Text = "01"
                                                        .KAISI_DATE_T2.Text = "01"
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = "2099"
                                                        .SYURYOU_DATE_T1.Text = "12"
                                                        .SYURYOU_DATE_T2.Text = "31"
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = "2000"
                                                        .KEIYAKU_DATE_T1.Text = "01"
                                                        .KEIYAKU_DATE_T2.Text = "01"
                                                End Select
                                            End With
                                        End If
                                    End If
                                Case "SOURYO_T", "KOTEI_TESUU1_T", "KOTEI_TESUU2_T", _
                                        "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                        "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                        "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                    '金額設定項目の場合
                                    oTORI(Index).Text = _
                                    String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 3)))
                                Case "TKIN_NO_T", "TUKEKIN_NO_T"
                                    '金融機関
                                    oTORI(Index).Text = sTORI(Index, 3)
                                    GetLabel(sTORI(Index, 1)).Text = GCom.GetBKBRName(sTORI(Index, 3), "", 30)
                                Case "TSIT_NO_T", "TUKESIT_NO_T", "TESUUTYO_SIT_T", _
                                     "TORIMATOME_SIT_T"
                                    '支店
                                    oTORI(Index).Text = sTORI(Index, 3)
                                    Dim BKCode As String = ""
                                    Dim BRCode As String = sTORI(Index, 3)
                                    Select Case sTORI(Index, 1).ToUpper
                                        Case "TSIT_NO_T".ToUpper
                                            BKCode = Me.TKIN_NO_T.Text
                                        Case "TUKESIT_NO_T".ToUpper
                                            BKCode = Me.TUKEKIN_NO_T.Text
                                        Case Else
                                            If BRCode.Trim.Length > 0 Then
                                                BKCode = SELF_BANK_CODE
                                            End If
                                    End Select
                                    If BRCode <> "" Then
                                        GetLabel(sTORI(Index, 1)).Text = GCom.GetBKBRName(BKCode, BRCode, 30)
                                    End If
                                Case "ITAKU_CODE_T"
                                    '委託者コード
                                    oTORI(Index).Text = sTORI(Index, 3)
                                Case "KIGYO_CODE_T"
                                    '企業コード
                                    If CENTER_CODE = "0" Then
                                        '信組の場合はそのまま使用
                                        oTORI(Index).Text = sTORI(Index, 3)
                                    Else
                                        '5桁の内1桁目は固定のため、2文字目以降を設定する
                                        oTORI(Index).Text = Mid(sTORI(Index, 3), 2)
                                    End If
                                Case Else
                                    '金額項目以外の場合
                                    oTORI(Index).Text = sTORI(Index, 3)
                            End Select

                        Case TypeOf oTORI(Index) Is ComboBox
                            'コンボボックス
                            Dim CMB As ComboBox = CType(oTORI(Index), ComboBox)
                            '再振種別と持越種別は特別な処置を行う。
                            Select Case sTORI(Index, 1).ToUpper
                                Case "SFURI_SYUBETU_T"
                                    Select Case GCom.NzInt(sTORI(Index, 3))
                                        Case 0
                                            SFURI_SYUBETU_T.SelectedIndex = 0
                                            SFURI_SYUBETU_T_SUB.SelectedIndex = 0
                                        Case 1
                                            SFURI_SYUBETU_T.SelectedIndex = 1
                                            SFURI_SYUBETU_T_SUB.SelectedIndex = 0
                                        Case 2
                                            SFURI_SYUBETU_T.SelectedIndex = 1
                                            SFURI_SYUBETU_T_SUB.SelectedIndex = 1
                                        Case 3
                                            SFURI_SYUBETU_T.SelectedIndex = 0
                                            SFURI_SYUBETU_T_SUB.SelectedIndex = 1
                                    End Select
                                Case Else
                                    Dim IntTemp As Integer = -1

                                    If sTORI(Index, 3).Length > 0 Then
                                        IntTemp = GCom.NzInt(sTORI(Index, 3))
                                    End If

                                    If IntTemp >= 0 Then

                                        For Cnt = 0 To CMB.Items.Count - 1 Step 1

                                            CMB.SelectedIndex = Cnt

                                            If GCom.GetComboBox(CMB) = IntTemp Then
                                                Exit For
                                            End If
                                        Next Cnt

                                        If Cnt >= CMB.Items.Count AndAlso CMB.Items.Count > 0 Then

                                            CMB.SelectedIndex = -1
                                        End If

                                        memTORI(Index, 0) = CMB.Text
                                    End If
                            End Select


                        Case TypeOf oTORI(Index) Is CheckBox
                            'チェックボックス

                            Dim CHK As CheckBox = CType(oTORI(Index), CheckBox)
                            CHK.Checked = (sTORI(Index, 3) = "1")

                        Case TypeOf oTORI(Index) Is Label

                            Dim LBL As Label = CType(oTORI(Index), Label)

                            Select Case LBL.Name
                                Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                     "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                     "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                    If GCom.NzDec(sTORI(GetIndex("TESUU_TABLE_ID_T"), 4), "").Length > 0 Then
                                        Dim onDec As Decimal = GCom.NzDec(sTORI(Index, 3))
                                        LBL.Text = String.Format("{0:#,##0}", onDec)
                                    End If
                                Case "SINKYU_NENDO_T"
                                    LBL.Text = GCom.NzStr(sTORI(Index, 3))
                                    sTORI(Index, 4) = sTORI(Index, 3)
                                Case Else
                                    'ラベル(作成日／更新日)
                                    If sTORI(Index, 3).Length >= 8 AndAlso _
                                        GCom.IsNumber(sTORI(Index, 3)) Then

                                        onDate = GCom.SET_DATE(sTORI(Index, 3))

                                        If Not onDate = Nothing Then

                                            Dim Temp As String = "yyyy年MM月dd日"
                                            LBL.Text = String.Format("{0:" & Temp & "}", onDate)

                                            sTORI(Index, 4) = sTORI(Index, 3)
                                        End If
                                    End If
                            End Select
                    End Select
                Next Idx

                Call SetControlToReadOnly(False)
                Me.SSTab.SelectedIndex = 0
                Me.BAITAI_CODE_T.Focus()
            Else
                'メッセージ変更
                Dim MSG As String = G_MSG0002W
                Dim DRet As DialogResult = MessageBox.Show(MSG, _
                   msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.GAKKOU_CODE_T.Focus()
                Return False
            End If

            '項目設定（学校マスタ１）
            If Not fn_SELECT_GAKMAST1() Then
                Exit Function
            End If

            CmdSelect_Action = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("マスタ参照", "失敗", ex.ToString)
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Me.ResumeLayout()
        End Try
    End Function

    Private Function fn_SELECT_GAKMAST1() As Boolean
        '********************************************
        '学校マスタ１項目設定
        '********************************************
        Dim ret As Boolean = False
        Dim REC As MyOracleReader = Nothing
        Try
            GAK1_SAKUSEI = New Hashtable
            MainDB = New MyOracle
            REC = New MyOracleReader(MainDB)
            For NenCnt As Integer = 1 To CInt(SIYOU_GAKUNEN_T.Text)
                '存在チェック
                Dim SQL As String = ""
                SQL = "SELECT  *"
                SQL &= " FROM GAKMAST1"
                SQL &= " WHERE GAKKOU_CODE_G = " & SQ(GAKKOU_CODE_T.Text.Trim)
                SQL &= " AND GAKUNEN_CODE_G = " & NenCnt

                If REC.DataReader(SQL) Then
                    '学校名設定
                    sTORI(GetIndex("GAKKOU_NNAME_G"), 3) = GCom.NzStr(REC.GetString("GAKKOU_NNAME_G"))
                    sTORI(GetIndex("GAKKOU_KNAME_G"), 3) = GCom.NzStr(REC.GetString("GAKKOU_KNAME_G"))
                    GAKKOU_NNAME_G.Text = GCom.NzStr(REC.GetString("GAKKOU_NNAME_G"))
                    GAKKOU_KNAME_G.Text = GCom.NzStr(REC.GetString("GAKKOU_KNAME_G"))
                    'クラス情報設定
                    Dim dgv As DataGridView = Nothing
                    Select Case (NenCnt)
                        Case 1
                            txtGAKUNEN_NAME1.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView1
                        Case 2
                            txtGAKUNEN_NAME2.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView2
                        Case 3
                            txtGAKUNEN_NAME3.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView3
                        Case 4
                            txtGAKUNEN_NAME4.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView4
                        Case 5
                            txtGAKUNEN_NAME5.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView5
                        Case 6
                            txtGAKUNEN_NAME6.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView6
                        Case 7
                            txtGAKUNEN_NAME7.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView7
                        Case 8
                            txtGAKUNEN_NAME8.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView8
                        Case 9
                            txtGAKUNEN_NAME9.Text = Trim(GCom.NzStr(REC.GetString("GAKUNEN_NAME_G")))
                            dgv = DataGridView9
                    End Select

                    Dim Str_Item_Class As String = ""

                    For ClassCnt As Integer = 0 To 19
                        Str_Item_Class = CStr(ClassCnt + 1)
                        If Str_Item_Class.Length = 1 Then
                            Str_Item_Class = "0" & Str_Item_Class
                        End If

                        If IsDBNull(GCom.NzStr(REC.GetString("CLASS_CODE1" & Str_Item_Class & "_G"))) = False Then
                            dgv.Rows(ClassCnt).Cells(0).Value = _
                            Trim(GCom.NzStr(REC.GetString("CLASS_CODE1" & Str_Item_Class & "_G")))
                        Else
                            dgv.Rows(ClassCnt).Cells(0).Value = ""
                        End If

                        If IsDBNull(GCom.NzStr(REC.GetString("CLASS_NAME1" & Str_Item_Class & "_G"))) = False Then
                            dgv.Rows(ClassCnt).Cells(1).Value = _
                            Trim(GCom.NzStr(REC.GetString("CLASS_NAME1" & Str_Item_Class & "_G")))
                        Else
                            dgv.Rows(ClassCnt).Cells(1).Value = ""
                        End If
                    Next ClassCnt

                    If Not GAK1_SAKUSEI.ContainsKey(NenCnt.ToString) Then
                        GAK1_SAKUSEI.Add(NenCnt.ToString, GCom.NzStr(REC.GetString("SAKUSEI_DATE_G")))
                    End If

                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Try
                    End If
                Else
                    '未登録エラー
                    MessageBox.Show(G_MSG0003W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    GAKKOU_CODE_T.Focus()
                    Exit Try
                End If

            Next NenCnt

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("マスタ参照", "失敗", ex.ToString)
        Finally
            If Not REC Is Nothing Then REC.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            Me.ResumeLayout()
        End Try
        Return ret

    End Function

#End Region

#Region " 取消"
    Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCancel.Click
        Try
            SyoriKBN = 4
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            '画面を初期化
            Call FormInitializa()
            'ロック解除する
            Call SetControlToReadOnly(True)
            '取引先検索コンボボックスの一番上を表示する
            cmbKana.SelectedIndex = 0
            cmbGAKKOUNAME.SelectedIndex = -1

            '最初のタブ、最初の入力場所にフォーカス設定
            SSTab.SelectedIndex = 0
            Me.GAKKOU_CODE_T.Focus()
        Catch ex As Exception
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub
#End Region

#Region " 印刷"
    Private Sub CmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdPrint.Click
        SyoriKBN = 4
        Try
            MainDB = New MyOracle
            LW.ToriCode = Me.GAKKOU_CODE_T.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '印刷処理を起動する
            If fn_CreateCSV_INSERT() = False Then
                Return
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "学校マスタメンテ(登録)"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Call fn_Print(1)

            SSTab.SelectedIndex = 0
            Me.GAKKOU_CODE_T.Focus()
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub
#End Region

#Region " 終了"
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 関数"

    '
    ' 機能　　 : 格納位置の検索
    '
    ' 戻り値　 : 格納位置
    '
    ' 引き数　 : ARG1 - ORACLE項目名
    '
    ' 備考　　 : 自振取引先マスタ専用
    '
    Private Function GetIndex(ByVal avColumnName As String) As Integer
        Dim Index As Integer
        Try
            For Index = 0 To MaxColumns Step 1
                If avColumnName.ToUpper = sTORI(Index, 1).ToUpper Then

                    Return Index
                End If
            Next Index
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "配列格納位置検索(" & avColumnName & ")"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
        End Try
        Return -1
    End Function

    '
    ' 機　能 : テキストボックス名で対のラベルオブジェクトを返す
    '
    ' 戻り値 : Label Object
    '
    ' 引き数 : ARG1 - ラベル名
    '
    ' 備　考 : なし
    '    
    Private Function GetLabel(ByVal TextBoxName As String) As Label
        Select Case TextBoxName.ToUpper
            Case "TKIN_NO_T".ToUpper
                Return TKIN_NO_TL
            Case "TUKEKIN_NO_T".ToUpper
                Return TUKEKIN_NO_TL
            Case "TSIT_NO_T".ToUpper
                Return TSIT_NO_TL
            Case "TUKESIT_NO_T".ToUpper
                Return TUKESIT_NO_TL
            Case "TORIMATOME_SIT_T".ToUpper
                Return TORIMATOME_SIT_TL
            Case "TESUUTYO_SIT_T".ToUpper
                Return TESUUTYO_SIT_TL
            Case "HENKYAKU_SIT_NO_T".ToUpper
                Return HENKYAKU_SIT_NO_TL
        End Select
        Return Nothing
    End Function

    '
    ' 機　能 : テキストボックス名で対のラベルオブジェクトを返す
    '
    ' 戻り値 : Label Object
    '
    ' 引き数 : ARG1 - ラベル名
    '
    ' 備　考 : なし
    '
    Private Function GetCaptionName(ByVal onCaption As String) As String
        Try
            Dim Temp As String = ""
            For Cnt As Integer = 1 To onCaption.Length Step 1
                Dim Idx As Integer = Cnt - 1
                Temp &= onCaption.Substring(Idx, 1).Trim
            Next Cnt
            Return Temp
        Catch ex As Exception
            Return onCaption
        End Try
    End Function

    Private Sub SetInformation()
        Dim Index As Integer

        With GCom.GLog
            .Job2 = "プログラム内部基本項目情報設定"
            .Result = ""
            .Discription = ""
        End With
        Try
            '学校マスタ１(現在／変更後)を初期化する。
            For Index = 0 To 1 Step 1
                GakkouKName(Index) = ""
                GakkouNName(Index) = ""
            Next Index

            '学校マスタ２(現在／変更後)を含む全データを初期化する。
            For Index = 0 To MaxColumns Step 1
                Select Case Index
                    Case Is = 0
                        oTORI(Index) = GAKKOU_CODE_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "学校コード"
                        sTORI(Index, 1) = "GAKKOU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 1
                        oTORI(Index) = SIYOU_GAKUNEN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "使用学年"
                        sTORI(Index, 1) = "SIYOU_GAKUNEN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 2
                        oTORI(Index) = SAIKOU_GAKUNEN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "最高学年"
                        sTORI(Index, 1) = "SAIKOU_GAKUNEN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 3
                        oTORI(Index) = SINKYU_NENDO_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "進級年度"
                        sTORI(Index, 1) = "SINKYU_NENDO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 4
                        oTORI(Index) = GAKKOU_NNAME_G
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "学校漢字名"
                        sTORI(Index, 1) = "GAKKOU_NNAME_G"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 5
                        oTORI(Index) = GAKKOU_KNAME_G
                        nTORI(Index, 0) = 40
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "学校カナ名"
                        sTORI(Index, 1) = "GAKKOU_KNAME_G"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 6
                        oTORI(Index) = FURI_DATE_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "振替日"
                        sTORI(Index, 1) = "FURI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 7
                        oTORI(Index) = SFURI_DATE_T
                        nTORI(Index, 0) = 2
                        '2011/06/15 標準版修正 再振ありの場合、再振日をゼロ埋め--------------START
                        nTORI(Index, 1) = 1
                        'nTORI(Index, 1) = 0
                        '2011/06/15 標準版修正 再振ありの場合、再振日をゼロ埋め--------------END
                        sTORI(Index, 0) = "再振日"
                        sTORI(Index, 1) = "SFURI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 8
                        oTORI(Index) = BAITAI_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "媒体コード"
                        sTORI(Index, 1) = "BAITAI_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 9
                        oTORI(Index) = FILE_NAME_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ファイル名"
                        sTORI(Index, 1) = "FILE_NAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 10
                        oTORI(Index) = TAKO_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "他行区分"
                        sTORI(Index, 1) = "TAKO_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 11
                        oTORI(Index) = JIFURICHK_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "自振契約作成区分"
                        sTORI(Index, 1) = "JIFURICHK_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 12
                        oTORI(Index) = ITAKU_CODE_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "委託者コード"
                        sTORI(Index, 1) = "ITAKU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 13
                        oTORI(Index) = KOKYAKU_NO_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "顧客番号"
                        sTORI(Index, 1) = "KOKYAKU_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "0"
                    Case Is = 14
                        oTORI(Index) = TKIN_NO_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "取扱金融機関"
                        sTORI(Index, 1) = "TKIN_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 15
                        oTORI(Index) = TSIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "取扱支店"
                        sTORI(Index, 1) = "TSIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 16
                        oTORI(Index) = KAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "科目"
                        sTORI(Index, 1) = "KAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 17
                        oTORI(Index) = KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "口座番号"
                        sTORI(Index, 1) = "KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 18
                        oTORI(Index) = SFURI_SYUBETU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "再振種別"
                        sTORI(Index, 1) = "SFURI_SYUBETU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 19
                        oTORI(Index) = NKYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "入金休日コード"
                        sTORI(Index, 1) = "NKYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 20
                        oTORI(Index) = SKYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "出金休日コード"
                        sTORI(Index, 1) = "SKYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 21
                        oTORI(Index) = SFURI_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "再振休日コード"
                        sTORI(Index, 1) = "SFURI_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 22
                        oTORI(Index) = TEKIYOU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "摘要区分"
                        sTORI(Index, 1) = "TEKIYOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 23
                        oTORI(Index) = KTEKIYOU_T
                        nTORI(Index, 0) = 13
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "カナ摘要"
                        sTORI(Index, 1) = "KTEKIYOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 24
                        oTORI(Index) = NTEKIYOU_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "漢字摘要"
                        sTORI(Index, 1) = "NTEKIYOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 25
                        oTORI(Index) = KEIYAKU_NO_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "契約書番号"
                        sTORI(Index, 1) = "KEIYAKU_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 26
                        oTORI(Index) = KEIYAKU_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "契約日"
                        sTORI(Index, 1) = "KEIYAKU_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 27
                        oTORI(Index) = KAISI_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "開始年月日"
                        sTORI(Index, 1) = "KAISI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 28
                        oTORI(Index) = SYURYOU_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "終了年月日"
                        sTORI(Index, 1) = "SYURYOU_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 29
                        oTORI(Index) = MOTIKOMI_KIJITSU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "持込期日"
                        sTORI(Index, 1) = "MOTIKOMI_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 30
                        oTORI(Index) = YUUBIN_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "郵便番号"
                        sTORI(Index, 1) = "YUUBIN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 31
                        oTORI(Index) = DENWA_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "電話番号"
                        sTORI(Index, 1) = "DENWA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 32
                        oTORI(Index) = FAX_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ＦＡＸ番号"
                        sTORI(Index, 1) = "FAX_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 33
                        oTORI(Index) = KANREN_KIGYO_CODE_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "関連企業情報"
                        sTORI(Index, 1) = "KANREN_KIGYO_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 34
                        oTORI(Index) = FKEKKA_TBL_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "振替結果変換テーブルID"
                        sTORI(Index, 1) = "FKEKKA_TBL_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 35
                        oTORI(Index) = ITAKU_NJYU_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "委託者住所漢字"
                        sTORI(Index, 1) = "ITAKU_NJYU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 36
                        oTORI(Index) = MEISAI_FUNOU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "口座振替結果一覧表印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_FUNOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 37
                        oTORI(Index) = MEISAI_KEKKA_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "口座振替不能明細一覧表印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_KEKKA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 38
                        oTORI(Index) = MEISAI_HOUKOKU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "収納報告書印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_HOUKOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 39
                        oTORI(Index) = MEISAI_TENBETU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "口座振替店別集計表印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_TENBETU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 40
                        oTORI(Index) = MEISAI_MINOU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "口座振替未納のお知らせ印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_MINOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 41
                        oTORI(Index) = MEISAI_YOUKYU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "要求性預金入金伝票印刷フラグ"
                        sTORI(Index, 1) = "MEISAI_YOUKYU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 42
                        oTORI(Index) = MEISAI_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "振替予定明細表作成区分"
                        sTORI(Index, 1) = "MEISAI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 43
                        oTORI(Index) = MEISAI_OUT_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "帳票ソート順"
                        sTORI(Index, 1) = "MEISAI_OUT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 44
                        oTORI(Index) = KESSAI_KBN_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済区分"
                        sTORI(Index, 1) = "KESSAI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 45
                        oTORI(Index) = TORIMATOME_SIT_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "とりまとめ店"
                        sTORI(Index, 1) = "TORIMATOME_SIT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 46
                        oTORI(Index) = HONBU_KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "本部別段口座番号"
                        sTORI(Index, 1) = "HONBU_KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 47
                        oTORI(Index) = KESSAI_DAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済日日数／基準日"
                        sTORI(Index, 1) = "KESSAI_DAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 48
                        oTORI(Index) = KESSAI_KIJITSU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済日指定区分"
                        sTORI(Index, 1) = "KESSAI_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 49
                        oTORI(Index) = KESSAI_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済休日コード"
                        sTORI(Index, 1) = "KESSAI_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 50
                        oTORI(Index) = TUKEKIN_NO_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済金融機関"
                        sTORI(Index, 1) = "TUKEKIN_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 51
                        oTORI(Index) = TUKESIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済支店"
                        sTORI(Index, 1) = "TUKESIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 52
                        oTORI(Index) = TUKEKAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済科目"
                        sTORI(Index, 1) = "TUKEKAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 53
                        oTORI(Index) = TUKEKOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済口座番号"
                        sTORI(Index, 1) = "TUKEKOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 54
                        oTORI(Index) = TUKEMEIGI_T
                        nTORI(Index, 0) = 40
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済名義人"
                        sTORI(Index, 1) = "TUKEMEIGI_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 55
                        oTORI(Index) = DENPYO_BIKOU1_T
                        nTORI(Index, 0) = 20
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "伝票備考１"
                        sTORI(Index, 1) = "DENPYO_BIKOU1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 56
                        oTORI(Index) = DENPYO_BIKOU2_T
                        nTORI(Index, 0) = 20
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "伝票備考２"
                        sTORI(Index, 1) = "DENPYO_BIKOU2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 57
                        oTORI(Index) = TESUUTYO_SIT_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求支店コード"
                        sTORI(Index, 1) = "TESUUTYO_SIT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 58
                        oTORI(Index) = TESUUTYO_KAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "手数料徴求科目"
                        sTORI(Index, 1) = "TESUUTYO_KAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 59
                        oTORI(Index) = TESUUTYO_KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求口座番号"
                        sTORI(Index, 1) = "TESUUTYO_KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 60
                        oTORI(Index) = TESUUTYO_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求区分"
                        sTORI(Index, 1) = "TESUUTYO_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 61
                        oTORI(Index) = TESUUTYO_PATN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求方法"
                        sTORI(Index, 1) = "TESUUTYO_PATN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 62
                        oTORI(Index) = TESUUMAT_NO_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料集計周期"
                        sTORI(Index, 1) = "TESUUMAT_NO_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 63
                        oTORI(Index) = TESUUTYO_DAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日数／基準日"
                        sTORI(Index, 1) = "TESUUTYO_DAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 64
                        oTORI(Index) = TESUUTYO_KIJITSU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求期日区分"
                        sTORI(Index, 1) = "TESUUTYO_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 65
                        oTORI(Index) = TESUU_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求日休日コード"
                        sTORI(Index, 1) = "TESUU_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 66
                        oTORI(Index) = SEIKYU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料請求引落区分"
                        sTORI(Index, 1) = "SEIKYU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 67
                        oTORI(Index) = KIHTESUU_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "振替手数料単価"
                        sTORI(Index, 1) = "KIHTESUU_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 68
                        oTORI(Index) = SYOUHI_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "消費税区分"
                        sTORI(Index, 1) = "SYOUHI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 69
                        oTORI(Index) = SOURYO_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "送料"
                        sTORI(Index, 1) = "SOURYO_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 70
                        oTORI(Index) = KOTEI_TESUU1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "固定手数料１"
                        sTORI(Index, 1) = "KOTEI_TESUU1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 71
                        oTORI(Index) = KOTEI_TESUU2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "固定手数料２"
                        sTORI(Index, 1) = "KOTEI_TESUU2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 72
                        oTORI(Index) = TESUUMAT_MONTH_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計月"
                        sTORI(Index, 1) = "TESUUMAT_MONTH_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 73
                        oTORI(Index) = TESUUMAT_ENDDAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計終了日"
                        sTORI(Index, 1) = "TESUUMAT_ENDDAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 74
                        oTORI(Index) = TESUUMAT_KIJYUN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計基準"
                        sTORI(Index, 1) = "TESUUMAT_KIJYUN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 75
                        oTORI(Index) = TESUUMAT_PATN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計方法"
                        sTORI(Index, 1) = "TESUUMAT_PATN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 76
                        oTORI(Index) = TESUU_GRP_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計企業グループ"
                        sTORI(Index, 1) = "TESUU_GRP_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 77
                        oTORI(Index) = TESUU_TABLE_ID_T
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                        ' 振込手数料基準ＩＤの属性を変更（桁数:2、Zero埋め有、NUMBER型からCHAR型）
                        'nTORI(Index, 0) = 1
                        'nTORI(Index, 1) = 0
                        'sTORI(Index, 0) = "振込手数料基準ＩＤ"
                        'sTORI(Index, 1) = "TESUU_TABLE_ID_T"
                        'sTORI(Index, 2) = "NUMBER"
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "振込手数料基準ＩＤ"
                        sTORI(Index, 1) = "TESUU_TABLE_ID_T"
                        sTORI(Index, 2) = "CHAR"
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 78
                        oTORI(Index) = TESUU_A1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "自行1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 79
                        oTORI(Index) = TESUU_A2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "自行1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 80
                        oTORI(Index) = TESUU_A3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "自行3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 81
                        oTORI(Index) = TESUU_B1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "支店1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 82
                        oTORI(Index) = TESUU_B2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "支店1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 83
                        oTORI(Index) = TESUU_B3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "支店3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 84
                        oTORI(Index) = TESUU_C1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "他行1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 85
                        oTORI(Index) = TESUU_C2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "他行1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 86
                        oTORI(Index) = TESUU_C3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "他行3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 87
                        oTORI(Index) = FURI_CODE_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "振替コード"
                        sTORI(Index, 1) = "FURI_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 88
                        oTORI(Index) = KIGYO_CODE_T
                        nTORI(Index, 0) = 5
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "企業コード"
                        sTORI(Index, 1) = "KIGYO_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 89
                        oTORI(Index) = JIFURI_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "自振区分"
                        sTORI(Index, 1) = "JIFURI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 90
                        oTORI(Index) = KESSAI_SYUBETU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済種別"
                        sTORI(Index, 1) = "KESSAI_SYUBETU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 91
                        oTORI(Index) = SAKUSEI_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "作成日"
                        sTORI(Index, 1) = "SAKUSEI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 92
                        oTORI(Index) = KOUSIN_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "更新日"
                        sTORI(Index, 1) = "KOUSIN_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 93
                        '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                        oTORI(Index) = YOBI1_T
                        nTORI(Index, 0) = 20
                        'oTORI(Index) = Nothing
                        'nTORI(Index, 0) = 50
                        '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
                        nTORI(Index, 1) = 0
                        '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
                        sTORI(Index, 0) = "WEB伝送ユーザ名"
                        'sTORI(Index, 0) = "予備1"
                        '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
                        sTORI(Index, 1) = "YOBI1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 94
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備2"
                        sTORI(Index, 1) = "YOBI2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 95
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備3"
                        sTORI(Index, 1) = "YOBI3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 96
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備4"
                        sTORI(Index, 1) = "YOBI4_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 97
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備5"
                        sTORI(Index, 1) = "YOBI5_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 98
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備6"
                        sTORI(Index, 1) = "YOBI6_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 99
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備7"
                        sTORI(Index, 1) = "YOBI7_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 100
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備8"
                        sTORI(Index, 1) = "YOBI8_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 101
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備9"
                        sTORI(Index, 1) = "YOBI9_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 102
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備10"
                        sTORI(Index, 1) = "YOBI10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 103
                        oTORI(Index) = HANSOU_KBN_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "搬送方法"
                        sTORI(Index, 1) = "HANSOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 104
                        oTORI(Index) = HANSOU_ROOT1_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート１"
                        sTORI(Index, 1) = "HANSOU_ROOT1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 105
                        oTORI(Index) = HANSOU_ROOT2_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート２"
                        sTORI(Index, 1) = "HANSOU_ROOT2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 106
                        oTORI(Index) = HANSOU_ROOT3_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート３"
                        sTORI(Index, 1) = "HANSOU_ROOT3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 107
                        oTORI(Index) = HENKYAKU_SIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "返却支店"
                        sTORI(Index, 1) = "HENKYAKU_SIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 108
                        oTORI(Index) = SYOUGOU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "照合要否区分"
                        sTORI(Index, 1) = "SYOUGOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 109
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM01_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 110
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM02_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 111
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM03_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 112
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM04_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 113
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM05_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 114
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM06_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 115
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM07_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 116
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM08_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 117
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM09_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 118
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM10_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 119
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM11_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 120
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM12_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 121
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM13_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 122
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM14_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 123
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM15_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 124
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM16_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 125
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM17_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 126
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM18_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 127
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM19_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 128
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM20_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 129
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM21_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 130
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM22_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 131
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM23_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 132
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM24_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 133
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM25_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 134
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM26_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 135
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM27_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 136
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM28_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 137
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM29_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 138
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM30_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 139
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM31_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 140
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM32_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 141
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM33_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 142
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM34_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 143
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM35_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 144
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM36_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 145
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM37_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 146
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM38_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 147
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM39_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 148
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM40_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 149
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM41_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 150
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM42_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 151
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM43_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 152
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM44_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 153
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM45_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 154
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM46_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 155
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM47_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 156
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM48_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 157
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM49_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 158
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM50_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 159
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR01_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 160
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR02_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 161
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR03_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 162
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR04_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 163
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR05_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 164
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR06_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 165
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR07_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 166
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR08_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 167
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR09_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 168
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 169
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR11_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 170
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR12_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 171
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR13_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 172
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR14_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 173
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR15_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 174
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR16_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 175
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR17_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 176
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR18_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 177
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR19_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 178
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR20_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 179
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR21_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 180
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR22_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 181
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR23_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 182
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR24_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 183
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR25_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 184
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR26_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 185
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR27_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 186
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR28_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 187
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR29_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 188
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR30_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 189
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR31_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 190
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR32_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 191
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR33_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 192
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR34_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 193
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR35_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 194
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR36_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 195
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR37_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 196
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR38_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 197
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR39_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 198
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR40_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 199
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR41_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 200
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR42_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 201
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR43_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 202
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR44_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 203
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR45_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 204
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR46_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 205
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR47_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 206
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR48_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 207
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR49_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 208
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR50_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                End Select
            Next Index
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '
    ' 機　能 : フォーム上の値を初期化
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 親画面
    '
    ' 備　考 : 初期化関数
    '    
    Private Function FormInitializa(Optional ByVal SEL As Integer = 0) As Boolean
        Dim CTL As Control
        Dim Index As Integer
        Dim Ret As Integer
        Dim CmbName As String = ""
        Dim TxtFileName As String = ""
        Dim Msg As String = ""

        With Me
            .GAKKOU_KNAME_G.Clear()
            .GAKKOU_NNAME_G.Clear()
        End With

        'DataGridView初期化
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()
        DataGridView3.Rows.Clear()
        DataGridView4.Rows.Clear()
        DataGridView5.Rows.Clear()
        DataGridView6.Rows.Clear()
        DataGridView7.Rows.Clear()
        DataGridView8.Rows.Clear()
        DataGridView9.Rows.Clear()
        DataGridView1.Rows.Add(20)
        DataGridView2.Rows.Add(20)
        DataGridView3.Rows.Add(20)
        DataGridView4.Rows.Add(20)
        DataGridView5.Rows.Add(20)
        DataGridView6.Rows.Add(20)
        DataGridView7.Rows.Add(20)
        DataGridView8.Rows.Add(20)
        DataGridView9.Rows.Add(20)

        '学年名削除
        txtGAKUNEN_NAME1.Text = ""
        txtGAKUNEN_NAME2.Text = ""
        txtGAKUNEN_NAME3.Text = ""
        txtGAKUNEN_NAME4.Text = ""
        txtGAKUNEN_NAME5.Text = ""
        txtGAKUNEN_NAME6.Text = ""
        txtGAKUNEN_NAME7.Text = ""
        txtGAKUNEN_NAME8.Text = ""
        txtGAKUNEN_NAME9.Text = ""

        SSTab.SelectedIndex = 0
        TabClass.SelectedIndex = 0

        Try
            For Index = 0 To MaxColumns Step 1
                CTL = oTORI(Index)
                Select Case True
                    Case (TypeOf CTL Is TextBox)
                        With CType(CTL, TextBox)
                            If FormInitializeSub(SEL, .Name) Then
                                .Text = ""
                            End If
                            .BackColor = System.Drawing.SystemColors.Window
                            Select Case .Name.ToUpper
                                Case Is = "KOKYAKU_NO_T"
                                    .Text = ""
                                Case Is = "MOTIKOMI_KIJITSU_T"
                                    .Text = "03"
                                Case Is = "KESSAI_DAY_T", "TESUUTYO_DAY_T"
                                    .Text = "01"
                                Case Is = "SOURYO_T"
                                    .Text = "0"
                                Case Is = "KOTEI_TESUU1_T", "KOTEI_TESUU2_T"
                                    .Text = "0"
                                Case Is = "HONBU_KOUZA_T", "TUKEKOUZA_T", "TESUUTYO_KOUZA_T "
                                    .Text = ""
                                Case Is = "KIHTESUU_T"
                                    .Text = "0"
                                Case Is = "TESUUMAT_NO_T"
                                    .Text = "0"
                                Case Is = "KIGYO_CODE_T"
                                    .Text = ""
                                Case Is = "FURI_CODE_T"
                                    .Text = ""
                                Case Is = "ITAKU_KANRI_CODE_T"
                                    .Text = ""
                                Case Is = "TKIN_NO_T"
                                    .Text = SELF_BANK_CODE
                                Case Is = "TUKEKIN_NO_T"
                                    .Text = SELF_BANK_CODE
                                Case Is = "KEIYAKU_DATE_T"
                                    .Text = "2000"
                                    KEIYAKU_DATE_T1.Text = "01"
                                    KEIYAKU_DATE_T2.Text = "01"
                                Case Is = "KAISI_DATE_T"
                                    .Text = "2000"
                                    KAISI_DATE_T1.Text = "01"
                                    KAISI_DATE_T2.Text = "01"
                                Case Is = "SYURYOU_DATE_T"
                                    .Text = "2099"
                                    SYURYOU_DATE_T1.Text = "12"
                                    SYURYOU_DATE_T2.Text = "31"
                                Case Is = "KESSAI_DAY_T", "TESUUTYO_DAY_T"
                                    .Text = "01"
                                Case Is = "FKEKKA_TBL_T"
                                    .Text = "0"
                                Case Is = "TORIMATOME_SIT_T", "TUKESIT_NO_T", "TESUUTYO_SIT_NO_T"
                                Case Is = "HANSOU_ROOT1_T", "HANSOU_ROOT2_T", "HANSOU_ROOT3_T", "HENKYAKU_SIT_NO_T", "KEIYAKU_NO_T"
                                    .Text = ""
                            End Select
                        End With
                    Case (TypeOf CTL Is ComboBox)
                        Dim CMB As ComboBox = CType(CTL, ComboBox)
                        With CMB
                            .Items.Clear()
                            .Text = ""
                            Ret = 0
                            Select Case .Name.ToUpper
                                Case "KAMOKU_T"
                                    CmbName = "科目"
                                    TxtFileName = "GFJ_科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "GFJ_科目.TXT", 2)
                                Case "BAITAI_CODE_T"
                                    CmbName = "媒体コード"
                                    TxtFileName = "GFJ_媒体.TXT"
                                    Ret = GCom.SetComboBox(CMB, "GFJ_媒体.TXT", True)
                                Case "SFURI_SYUBETU_T"
                                    CmbName = "再振持越種別"
                                    TxtFileName = "KFGMAST010_再振持越種別.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_再振持越種別.TXT", True)
                                    Ret = GCom.SetComboBox(Me.SFURI_SYUBETU_T_SUB, "KFGMAST010_再振持越種別.TXT", True)
                                Case "JIFURICHK_KBN_T"
                                    CmbName = "自振契約作成区分"
                                    TxtFileName = "KFGMAST010_自振契約作成区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_自振契約作成区分.TXT", True)
                                Case "TAKO_KBN_T"
                                    CmbName = "他行区分"
                                    TxtFileName = "GFJ_他行区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "GFJ_他行区分.TXT", True)
                                Case "SKYU_CODE_T"
                                    CmbName = "振替休日シフト"
                                    TxtFileName = "KFGMAST010_振替休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_振替休日シフト.TXT", True)
                                Case "NKYU_CODE_T"
                                    CmbName = "振替休日シフト"
                                    TxtFileName = "KFGMAST010_振替休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_振替休日シフト.TXT", 1)
                                Case "MEISAI_KBN_T"
                                    CmbName = "振替予定明細表作成区分"
                                    TxtFileName = "KFGMAST010_振替予定明細表作成区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_振替予定明細表作成区分.TXT", True)
                                Case "MEISAI_OUT_T"
                                    CmbName = "帳票ソート順"
                                    TxtFileName = "KFGMAST010_帳票ソート順指定.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_帳票ソート順指定.TXT", True)
                                Case "KESSAI_KIJITSU_T"
                                    CmbName = "日付区分(決済)"
                                    TxtFileName = "KFGMAST010_日付区分(決済).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_日付区分(決済).TXT", True)
                                Case "KESSAI_KYU_CODE_T"
                                    CmbName = "決済休日シフト"
                                    TxtFileName = "KFGMAST010_決済休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_決済休日シフト.TXT", True)
                                Case "TESUUTYO_PATN_T"
                                    CmbName = "手数料徴求方法"
                                    TxtFileName = "KFGMAST010_手数料徴求方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_手数料徴求方法.TXT", True)
                                Case "TESUUTYO_KBN_T"
                                    CmbName = "手数料徴求区分"
                                    TxtFileName = "KFGMAST010_手数料徴求区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_手数料徴求区分.TXT", True)
                                Case "TESUUMAT_KIJYUN_T"
                                    CmbName = "集計基準"
                                    TxtFileName = "KFGMAST010_集計基準.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_集計基準.TXT", True)
                                Case "TESUUMAT_PATN_T"
                                    CmbName = "集計方法"
                                    TxtFileName = "KFGMAST010_集計方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_集計方法.TXT", True)
                                Case "TESUU_KYU_CODE_T"
                                    CmbName = "手数料休日シフト"
                                    TxtFileName = "KFGMAST010_手数料休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_手数料休日シフト.TXT", True)
                                Case "SYOUHI_KBN_T"
                                    CmbName = "消費税区分"
                                    TxtFileName = "KFGMAST010_消費税区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_消費税区分.TXT", True)
                                Case "SEIKYU_KBN_T"
                                    CmbName = "手数料請求区分"
                                    TxtFileName = "KFGMAST010_手数料請求区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_手数料請求区分.TXT", True)
                                Case "TESUU_TABLE_ID_T"
                                    CmbName = "振込手数料基準ID"
                                    '2013/11/27 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                                    Ret = GCom.SetComboBox_TESUU_TABLE_ID_T(CMB, TAX.ZEIRITSU_ID, -1)
                                    'TxtFileName = "KFGMAST010_振込手数料基準ID.TXT"
                                    'Ret = GCom.SetComboBox(CMB, "KFGMAST010_振込手数料基準ID.TXT", 0)
                                    '2013/11/27 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
                                Case "KESSAI_KBN_T"
                                    CmbName = "決済区分"
                                    TxtFileName = "GFJ_決済区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "GFJ_決済区分.TXT", 1)
                                Case "TUKEKAMOKU_T"
                                    CmbName = "決済科目"
                                    TxtFileName = "Common_決済科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_決済科目.TXT", True)
                                Case "TESUUTYO_KAMOKU_T  "
                                    CmbName = "手数料_科目"
                                    TxtFileName = "GFJ_手数料_科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "GFJ_手数料_科目.TXT", True)
                                Case "TESUUTYO_KIJITSU_T"
                                    CmbName = "日付区分(手数料徴求)"
                                    TxtFileName = "KFGMAST010_日付区分(手数料徴求).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_日付区分(手数料徴求).TXT", 0)
                                Case "JIFURI_KBN_T"
                                    CmbName = "自振区分"
                                    TxtFileName = "KFGMAST010_自振区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_自振区分.TXT", True)
                                Case "KESSAI_SYUBETU_T"
                                    CmbName = "決済種別"
                                    TxtFileName = "KFGMAST010_決済種別.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_決済種別.TXT", True)
                                Case "SFURI_KYU_CODE_T"
                                    CmbName = "再振休日シフト"
                                    TxtFileName = "KFGMAST010_再振休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_再振休日シフト.TXT", True)
                                Case "TEKIYOU_KBN_T"
                                    CmbName = "摘要区分"
                                    TxtFileName = "KFGMAST010_摘要区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST010_摘要区分.TXT", True)
                                Case "TESUUTYO_KAMOKU_T"
                                    CmbName = "手数料徴求科目"
                                    TxtFileName = "Common_手数料徴求科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_手数料徴求科目.TXT", True)
                                Case "HANSOU_KBN_T"
                                    CmbName = "搬送方法"
                                    TxtFileName = "KFGMAST011_搬送方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST011_搬送方法.TXT", True)
                                Case "SYOUGOU_KBN_T"
                                    CmbName = "照合要否区分"
                                    TxtFileName = "KFGMAST011_照合要否区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFGMAST011_照合要否区分.TXT", True)
                            End Select

                            Select Case Ret
                                Case 1  'ファイルなし
                                    Msg = String.Format(MSG0025E, CmbName, TxtFileName)
                                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Msg = CmbName & "設定ファイルなし。ファイル:" & TxtFileName
                                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(初期表示関数)", "失敗", Msg)
                                    Return False
                                Case 2  '異常
                                    Msg = String.Format(MSG0026E, CmbName)
                                    MessageBox.Show(Msg.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Msg = "コンボボックス設定失敗 コンボボックス名:" & CmbName
                                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(初期表示関数)", "失敗", Msg)
                                    Return False
                            End Select
                            CMB.BackColor = System.Drawing.SystemColors.Window
                        End With
                    Case (TypeOf CTL Is CheckBox)
                        With CType(CTL, CheckBox)
                            Select Case CTL.Name
                                Case "MEISAI_FUNOU_T", "MEISAI_KEKKA_T", "MEISAI_HOUKOKU_T", "MEISAI_TENBETU_T", "MEISAI_MINOU_T"
                                    .Checked = True
                                    .BackColor = System.Drawing.SystemColors.Control
                                Case "MEISAI_YOUKYU_T"
                                    .Checked = False
                                    .BackColor = System.Drawing.SystemColors.Control
                                Case Else
                                    .Checked = False
                                    .BackColor = System.Drawing.SystemColors.Control
                            End Select

                        End With
                    Case (TypeOf CTL Is Label)
                        With CType(CTL, Label)
                            .Text = ""
                            With CType(CTL, Label)
                                If Not GCom.NzInt(.Tag, 0) = 6 Then
                                    .Text = ""
                                    .BackColor = System.Drawing.Color.LemonChiffon
                                End If
                            End With
                        End With
                End Select

                '変更後の格納エリアを初期化する
                sTORI(Index, 4) = ""

            Next Index

            '配列外のコントロールを設定する
            Dim EachObj() As Control = {KAISI_DATE_T1, KAISI_DATE_T2, _
                                        SYURYOU_DATE_T1, SYURYOU_DATE_T2, _
                                        SAKUSEI_DATE_T, KOUSIN_DATE_T}
            For Each CTL In EachObj

                If TypeOf CTL Is TextBox Then
                    CTL.BackColor = System.Drawing.SystemColors.Window
                    If GCom.NzInt(CTL.Tag, 0) = 0 AndAlso FormInitializeSub(SEL, CTL.Name) Then
                        CTL.Text = ""
                    End If
                Else
                    CTL.Text = ""
                End If
            Next CTL

            'ラベル初期化
            With Me
                .SINKYU_NENDO_T.Text = ""
                .TSIT_NO_TL.Text = ""
                .TORIMATOME_SIT_TL.Text = ""
                .TUKEKIN_NO_TL.Text = ""
                .TUKESIT_NO_TL.Text = ""
                .TESUUTYO_SIT_TL.Text = ""
            End With

            Me.TKIN_NO_TL.Text = SELF_BANK_NAME
            Me.TKIN_NO_T.Text = SELF_BANK_CODE

            TUKEKIN_NO_TL.Text = SELF_BANK_NAME

            Call SetControlToReadOnly(True)
            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    '半角英数カナ評価(全角文字抹消)


    '
    ' 機　能 : 取引先主／副保全可否
    '
    ' 戻り値 : 通常とおり初期化 = True
    ' 　　　   初期化回避　　　 = False
    '
    ' 引き数 : ARG1 - 参照=1. 初期化=0
    ' 　　　   ARG2 - コントロール名
    '
    ' 備　考 : IF文支援関数(2008.04.18 By Astar)
    '    
    Private Function FormInitializeSub(ByVal SEL As Integer, ByVal CTL_NAME As String) As Boolean
        Try
            Select Case SEL
                Case 0
                Case 1
                    Select Case CTL_NAME.ToUpper
                        Case "TORIS_CODE_T", "TORIF_CODE_T"

                            Return False
                    End Select
            End Select
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return True
    End Function

    '
    ' 機　能 : 初期化時の有効無効設定
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 初期化時の場合に=True
    '
    ' 備　考 : なし
    '    
    Private Sub SetControlToReadOnly(ByVal onBoolean As Boolean)
        Try
            With Me
                .CmdInsert.Enabled = (onBoolean)
                .CmdUpdate.Enabled = (Not onBoolean)
                .CmdDelete.Enabled = (Not onBoolean)
                .CmdPrint.Enabled = (Not onBoolean)
                .GAKKOU_CODE_T.ReadOnly = (Not onBoolean)
                .CmdSelect.Text = IIf(onBoolean, 参照モード, 解除モード).ToString
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "参照)", "失敗", ex.ToString)
        End Try
    End Sub

    '
    ' 機　能 : 設定データの参照
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : 値参考用の関数
    '    
    Private Sub GetControlsValue()
        Dim Index As Integer

        Try
            For Index = 0 To MaxColumns Step 1

                Dim Temp As String = ""

                Select Case True
                    Case (TypeOf oTORI(Index) Is TextBox)
                        'テキストボックス

                        Select Case CType(oTORI(Index), TextBox).Name.ToUpper
                            Case "KAISI_DATE_T", "SYURYOU_DATE_T", "KEIYAKU_DATE_T"
                                '日付関係(複数の領域：年月日)
                                With Me
                                    Select Case CType(oTORI(Index), TextBox).Name.ToUpper
                                        Case "KAISI_DATE_T"
                                            Temp = .KAISI_DATE_T.Text.PadLeft(4, "0"c)
                                            Temp &= .KAISI_DATE_T1.Text.PadLeft(2, "0"c)
                                            Temp &= .KAISI_DATE_T2.Text.PadLeft(2, "0"c)

                                            '進級年度自動設定
                                            Dim onDate As Date
                                            '2011/05/25 標準版修正 更新時は進級年度を設定しない---------START
                                            If SyoriKBN = 0 Then
                                                Dim onText(2) As Integer
                                                onText(0) = GCom.NzInt(Me.KAISI_DATE_T.Text, 0)
                                                onText(1) = GCom.NzInt(Me.KAISI_DATE_T1.Text, 0)
                                                onText(2) = GCom.NzInt(Me.KAISI_DATE_T2.Text, 0)
                                                Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                                                If Ret = -1 Then
                                                    Select Case Month(onDate)
                                                        Case Is <= 3
                                                            Me.SINKYU_NENDO_T.Text = (Year(onDate) - 1).ToString
                                                        Case Else
                                                            Me.SINKYU_NENDO_T.Text = Year(onDate).ToString
                                                    End Select
                                                End If
                                            End If '2011/05/25 標準版修正 更新時は進級年度を設定しない---------END
                                        Case "SYURYOU_DATE_T"
                                            Temp = .SYURYOU_DATE_T.Text.PadLeft(4, "0"c)
                                            Temp &= .SYURYOU_DATE_T1.Text.PadLeft(2, "0"c)
                                            Temp &= .SYURYOU_DATE_T2.Text.PadLeft(2, "0"c)
                                        Case "KEIYAKU_DATE_T"
                                            Temp = .KEIYAKU_DATE_T.Text.PadLeft(4, "0"c)
                                            Temp &= .KEIYAKU_DATE_T1.Text.PadLeft(2, "0"c)
                                            Temp &= .KEIYAKU_DATE_T2.Text.PadLeft(2, "0"c)
                                    End Select

                                    '空白であると推定できる場合には "" とする
                                    If GCom.NzLong(Temp) = 0 Then

                                        Temp = "00000000"
                                    End If
                                End With
                            Case "KIHTESUU_T", "SOURYO_T", "KOTEI_TESUU1_T", "KOTEI_TESUU2_T", _
                                 "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                 "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                 "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                '金額設定項目の場合

                                Temp = CType(oTORI(Index), TextBox).Text.Trim.Replace(","c, "")
                            Case "KIGYO_CODE_T"
                                If CENTER_CODE = "0" Then
                                    Temp = CType(oTORI(Index), TextBox).Text.Trim
                                Else
                                    Temp = CENTER_CODE & CType(oTORI(Index), TextBox).Text.Trim
                                End If
                                '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                            Case "YOBI1_T"
                                '媒体が「WEB伝送」以外は初期値にする
                                Dim RetBaitai As Integer = GCom.GetComboBox(CType(oTORI(GetIndex("BAITAI_CODE_T")), ComboBox))
                                If RetBaitai = 3 Then
                                    Temp = CType(oTORI(Index), TextBox).Text.Trim
                                Else
                                    Temp = ""
                                End If
                                '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

                            Case Else
                                '日付以外の設定値(単独の領域)

                                Temp = CType(oTORI(Index), TextBox).Text.Trim
                        End Select

                    Case (TypeOf oTORI(Index) Is ComboBox)
                        'コンボボックス

                        Dim Ret As Integer

                        '再振種別と持越種別は特別な処置を行う。
                        Select Case sTORI(Index, 1).ToUpper
                            Case "SFURI_SYUBETU_T"

                                Dim DB_VALUE As Integer = 0
                                Dim SFURI_SYUBETU As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T)
                                Dim SFURI_SYUBETU_SUB As Integer = GCom.GetComboBox(Me.SFURI_SYUBETU_T_SUB)

                                Call GET_SET_SFURI_SYUBETU(1, DB_VALUE, SFURI_SYUBETU, SFURI_SYUBETU_SUB)

                                Temp = DB_VALUE.ToString
                                Select Case DB_VALUE
                                    Case 0
                                        memTORI(Index, 1) = "再振なし, 持越なし"
                                    Case 1
                                        memTORI(Index, 1) = "再振あり, 持越なし"
                                    Case 2
                                        memTORI(Index, 1) = "再振あり, 持越あり"
                                    Case 3
                                        memTORI(Index, 1) = "再振なし, 持越あり"
                                End Select
                                sTORI(Index, 4) = DB_VALUE.ToString
                            Case Else
                                Ret = GCom.GetComboBox(CType(oTORI(Index), ComboBox))

                                'コンボ蓄積Add無しや非選択時には -1 が返る(PG仕様)
                                If Ret >= 0 Then
                                    Temp = Ret.ToString
                                End If
                                memTORI(Index, 1) = CType(oTORI(Index), ComboBox).Text
                        End Select
                    Case (TypeOf oTORI(Index) Is CheckBox)
                        'チェックボックス

                        If CType(oTORI(Index), CheckBox).Checked Then
                            Temp = 1.ToString
                        Else
                            Temp = 0.ToString
                        End If
                    Case (TypeOf oTORI(Index) Is Label)
                        Select Case CType(oTORI(Index), Label).Name.ToUpper
                            Case "TKIN_NO_T"
                                Temp = GCom.NzDec(CType(oTORI(Index), Label).Text, "")
                            Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                 "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                 "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                '金額設定項目
                                Temp = CType(oTORI(Index), Label).Text.Trim.Replace(","c, "")
                        End Select
                End Select

                '参照結果を格納する
                If TypeOf oTORI(Index) Is Label Then

                    Select Case CType(oTORI(Index), Label).Name.ToUpper
                        Case "TKIN_NO_T", "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", "TESUU_C1_T", _
                             "TESUU_C2_T", "TESUU_C3_T"
                            sTORI(Index, 4) = Temp
                    End Select
                Else
                    Select Case nTORI(Index, 1)
                        Case 0
                            '通常データ
                            sTORI(Index, 4) = Temp
                        Case Else
                            '区分等で頭Zero埋めするデータ
                            If Temp.Trim.Length > 0 Then
                                Dim FormatString As String = New String("0"c, nTORI(Index, 0))
                                sTORI(Index, 4) = String.Format("{0:" & FormatString & "}", GCom.NzDec(Temp))
                            Else
                                sTORI(Index, 4) = Temp
                            End If
                    End Select
                End If
            Next Index
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '
    ' 機　能 : 項目データ変更有無チェック
    '
    ' 戻り値 : 変更あり = True
    ' 　　　   変更なし = False
    '
    ' 引き数 : ARG1 - 登録 = 0, 更新 = 1
    ' 　　　   
    '
    ' 備　考 : 変更がある場合には印刷用CSV作成を行う
    '    
    Private Function CheckUpdateItem(ByVal aSEL As Integer) As Boolean
        Dim Index As Integer

        '変更の有無を検証する(更新日は除外)
        For Index = 1 To MaxColumns Step 1
            If Not sTORI(Index, 3) = sTORI(Index, 4) AndAlso sTORI(Index, 1) <> "KOUSIN_DATE_T" AndAlso sTORI(Index, 1) <> "SAKUSEI_DATE_T" _
               AndAlso Not TypeOf oTORI(Index) Is Label AndAlso sTORI(Index, 1) <> "KOUSIN_SIKIBETU_T" Then
                If sTORI(Index, 1) <> "GAKKOU_NNAME_G" AndAlso sTORI(Index, 1) <> "GAKKOU_KNAME_G" Then '学校名は更新対象と別にする
                    Exit For
                End If

            End If
        Next

        '登録の場合は抜ける
        If aSEL = 0 Then
            Return True
        End If

        '更新項目が無ければ関数を抜ける
        If aSEL = 1 AndAlso Index > MaxColumns Then
            If sTORI(GetIndex("GAKKOU_NNAME_G"), 3) <> sTORI(GetIndex("GAKKOU_NNAME_G"), 4) OrElse _
                     sTORI(GetIndex("GAKKOU_KNAME_G"), 3) <> sTORI(GetIndex("GAKKOU_KNAME_G"), 4) Then
            Else
                Return False
            End If
        End If

        Dim Prn_KFGP007 As New KFGP007
        Try
            strCSV_FILE_NAME = Prn_KFGP007.CreateCsvFile
            updCnt = 0

            '印刷用のパラメータ設定
            Dim Today As String = Now.ToString("yyyyMMdd")
            Dim Time As String = Now.ToString("HHmmss")

            Dim Machine As String = Environment.MachineName


            For Index = 1 To MaxColumns Step 1
                If Not TypeOf oTORI(Index) Is Label Then

                    Select Case True

                        Case sTORI(Index, 1).ToUpper = "SAKUSEI_DATE_T"
                            '作成日=何もしない
                        Case sTORI(Index, 1).ToUpper = "KOUSIN_DATE_T"
                            '更新日=何もしない
                        Case Else
                            If aSEL = 0 OrElse Not sTORI(Index, 3) = sTORI(Index, 4) Then

                                Dim DQ As String = ControlChars.Quote

                                Dim SetData(1) As String
                                Select Case sTORI(Index, 1)
                                    Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                        "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                        "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T", "SOURYO_T", "KOTEI_TESUU1_T", _
                                         "KOTEI_TESUU2_T"
                                        SetData(0) = String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 3)))
                                        SetData(1) = String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 4)))
                                    Case Else
                                        If TypeOf oTORI(Index) Is ComboBox Then
                                            SetData(0) = GetComboBoxValue(aSEL, Index, 0)
                                            SetData(1) = GetComboBoxValue(aSEL, Index, 1)
                                        Else
                                            SetData(0) = sTORI(Index, 3)
                                            SetData(1) = sTORI(Index, 4)
                                        End If
                                End Select

                                If aSEL = 0 Then
                                    SetData(0) = "新規登録"
                                End If

                                If SetData(0) <> SetData(1) Then
                                    '更新帳票印刷用CSVファイル作成
                                    With Prn_KFGP007
                                        .OutputCsvData(Today, True)                                 '処理日
                                        .OutputCsvData(Time, True)                                  'タイムスタンプ
                                        .OutputCsvData(GCom.GetUserID, True)                        'ログイン名
                                        .OutputCsvData(Machine, True)                               '端末名
                                        .OutputCsvData(sTORI(GetIndex("GAKKOU_CODE_T"), 3), True)   '学校コード
                                        .OutputCsvData(sTORI(GetIndex("ITAKU_CODE_T"), 3), True)    '委託者コード
                                        .OutputCsvData(sTORI(GetIndex("GAKKOU_NNAME_G"), 3), True)  '学校名
                                        .OutputCsvData(sTORI(Index, 0), True)                       '日本語項目名
                                        .OutputCsvData(sTORI(Index, 1), True)                       'データベース項目名
                                        .OutputCsvData(SetData(0), True)                            '変更前
                                        .OutputCsvData(SetData(1), True)                            '変更後
                                        .OutputCsvData(sTORI(GetIndex("FURI_CODE_T"), 3), True)         '振替コード
                                        .OutputCsvData(sTORI(GetIndex("KIGYO_CODE_T"), 3), True, True)  '企業コード
                                    End With
                                    PrintCnt += 1

                                    '更新項目数を加算
                                    '2011/06/15 標準版修正 学校名を更新しても取引先マスタに反映--------------START
                                    'If sTORI(Index, 1) <> "GAKKOU_NNAME_G" AndAlso sTORI(Index, 1) <> "GAKKOU_KNAME_G" Then
                                    updCnt += 1
                                    'Else
                                    'End If
                                    '2011/06/15 標準版修正 学校名を更新しても取引先マスタに反映--------------END
                                End If
                            End If
                    End Select
                End If
            Next Index
            Prn_KFGP007.CloseCsv()
            Return True
        Catch ex As Exception
            MainLOG.Write("変更有無チェック", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally

        End Try
    End Function

    '
    ' 機　能 : エラーのあった場所にフォーカスを設定する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - Control格納配列のIndex
    '
    ' 備　考 : なし
    '    
    Private Sub SetFocusOnErrorControl(ByVal Index As Integer)
        Try
            For Each TXT As Control In Me.Controls
                If TypeOf TXT Is TextBox Then
                    If TXT.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                        oTORI(Index).Focus()
                        Return
                    End If
                End If
            Next TXT

            For Each TAB As TabPage In SSTab.TabPages
                For Each CTL As Control In TAB.Controls
                    If TypeOf CTL Is GroupBox Then
                        For Each OBJ As Control In CTL.Controls
                            If OBJ.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                                SSTab.SelectedIndex = TAB.TabIndex
                                oTORI(Index).Focus()
                                Return
                            End If
                        Next OBJ
                    ElseIf TypeOf CTL Is TextBox Then
                        If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                            SSTab.SelectedIndex = TAB.TabIndex
                            oTORI(Index).Focus()
                            Return
                        End If
                    ElseIf TypeOf CTL Is CheckBox Then
                        If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                            SSTab.SelectedIndex = TAB.TabIndex
                            oTORI(Index).Focus()
                            Return
                        End If
                    ElseIf TypeOf CTL Is ComboBox Then
                        If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                            SSTab.SelectedIndex = TAB.TabIndex
                            oTORI(Index).Focus()
                            Return
                        End If
                    End If
                Next CTL
            Next TAB
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("フォーカス移動", "失敗", ex.ToString)
        End Try
    End Sub

    '
    ' 機能　　 : 再振種別と持越種別(返す／判定する)
    '
    ' 戻り値　 : なし
    '
    ' 引き数　 : ARG1 - DB参照後画面表示モード = 0
    '                   画面設定値参照モード   = 1
    '            ARG2 - DB値
    '            ARG3 - 画面(再振種別)値
    '            ARG4 - 画面(持越種別)値
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Sub GET_SET_SFURI_SYUBETU(ByVal SEL As Integer, Optional ByRef DB_VALUE As Integer = 0, _
    Optional ByRef SFURI_SYUBETU As Integer = 0, Optional ByRef SFURI_SYUBETU_SUB As Integer = 0)
        Select Case SEL
            Case 0
                Select Case DB_VALUE
                    Case 0
                        SFURI_SYUBETU = 0
                        SFURI_SYUBETU_SUB = 0
                    Case 1
                        SFURI_SYUBETU = 1
                        SFURI_SYUBETU_SUB = 0
                    Case 2
                        SFURI_SYUBETU = 1
                        SFURI_SYUBETU_SUB = 1
                    Case 3
                        SFURI_SYUBETU = 0
                        SFURI_SYUBETU_SUB = 1
                End Select
            Case 1
                Select Case SFURI_SYUBETU
                    Case 0
                        Select Case SFURI_SYUBETU_SUB
                            Case 0
                                DB_VALUE = 0
                            Case Else
                                DB_VALUE = 3
                        End Select
                    Case Else
                        Select Case SFURI_SYUBETU_SUB
                            Case 0
                                DB_VALUE = 1
                            Case Else
                                DB_VALUE = 2
                        End Select
                End Select
        End Select
    End Sub

    '
    ' 機能　　 : 学校マスタの存在確認
    '
    ' 戻り値　 : 存在 = 1
    ' 　　　　   不在 = 0
    ' 　　　　   失敗 = -1
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function CheckRecordModule() As Integer
        Dim REC As OracleDataReader = Nothing
        Try
            '学校マスタ(1／2)をチェック(GAKMAST1は付属品と考える)
            Dim SQL As String = "SELECT COUNT(*) COUNTER FROM GAKMAST2"
            SQL &= " WHERE GAKKOU_CODE_T = " & SetColumnData(GetIndex("GAKKOU_CODE_T"))

            Dim FLG As Integer = 0
            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read Then

                If GCom.NzInt(REC.Item("COUNTER"), 0) > 0 Then
                    FLG = 1
                End If
            End If

            If FLG = 0 Then
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If

                SQL = "SELECT COUNT(*) COUNTER FROM TORIMAST"
                SQL &= " WHERE FSYORI_KBN_T = " & "1"
                SQL &= " AND TORIS_CODE_T = " & SetColumnData(GetIndex("GAKKOU_CODE_T"))

                If GCom.SetDynaset(SQL, REC) AndAlso REC.Read Then
                    If GCom.NzInt(REC.Item("COUNTER"), 0) > 0 Then
                        FLG = 2
                    End If
                End If

            End If
            Return FLG
        Catch ex As Exception
            Return -1
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Function

    '
    '
    ' 機能　　 : 取引先マスタORACLE設定項目値作成支援関数
    '
    ' 戻り値　 : 設定項目値
    '
    ' 引き数　 : ARG1 - Index
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function SetColumnData(ByVal Index As Integer) As String
        Try
            If IsNothing(sTORI(Index, 4)) OrElse sTORI(Index, 4).Trim.Length = 0 Then
                Return "NULL"
            Else
                Select Case sTORI(Index, 2).Trim.ToUpper
                    Case "NUMBER"
                        Return GCom.NzDec(sTORI(Index, 4)).ToString
                    Case Else
                        Dim Temp As String = " "
                        If sTORI(Index, 4).Trim.Length > 0 Then
                            Temp = sTORI(Index, 4).Trim
                        End If
                        Return "'" & Temp & "'"
                End Select
            End If
        Catch ex As Exception

            Return "NULL"
        End Try
    End Function

    '

    '
    ' 機　能 : 変更差分印刷用の支援関数
    '
    ' 戻り値 : 印刷項目値
    '
    ' 引き数 : ARG1 - 登録 = 0, 更新 = 1, 削除 = 2
    ' 　　　   ARG2 - Index
    ' 　　　   ARG3 - OLD = 0, NEW = 1
    '
    ' 備　考 : なし
    '
    Private Function GetComboBoxValue(ByVal aSEL As Integer, _
                        ByVal Index As Integer, ByVal NewOld As Integer) As String
        Try
            If aSEL = 2 Then
                Return memTORI(Index, NewOld)
            Else
                Return "( " & sTORI(Index, NewOld + 3) & " ) " & memTORI(Index, NewOld)
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機能　　 : 決済未使用設定
    '
    ' 戻り値　 :
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function KESSAI_Disused() As Boolean
        KESSAI_Disused = False
        Try
            If KESSAI = "0" Then
                '決済対象外設定
                GCom.SetComboBox(KESSAI_KBN_T, "GFJ_決済区分.TXT", 99)
                '取りまとめ店 = 取り扱い支店
                TORIMATOME_SIT_T.Text = TSIT_NO_T.Text
                '2009/12/09 決済金融機関・支店追加
                TUKEKIN_NO_T.Text = ""
                TUKEKIN_NO_L.Text = ""
                TUKEKIN_NO_TL.Text = ""
                TUKESIT_NO_T.Text = ""
                TUKESIT_NO_L.Text = ""
                '=================================
                GB31.Enabled = False
                GB32.Enabled = False
                GB33.Enabled = False
            End If
        Catch ex As Exception
            MainLOG.Write("取引先存在チェック", "失敗", ex.ToString)
            Return False
        End Try
        KESSAI_Disused = True
    End Function

    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
    ''' <summary>
    ''' 決済科目に「その他」が存在するかチェックする
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ChkKessaiKamoku()
        For Each kamoku As Object In TUKEKAMOKU_T.Items
            If kamoku.ToString = "その他" Then
                TUKEKAMOKU_USE_ETC = True
                Return
            End If
        Next
    End Sub
    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

#End Region

#Region " GRIDイベント"
    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex
        Select Case e.ColumnIndex
            Case 0
                CType(sender, DataGridView).ImeMode = ImeMode.Disable
            Case 1
                CType(sender, DataGridView).ImeMode = ImeMode.Hiragana
        End Select
    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 0
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
            Case 1
                '2010/11/22.Sakon　制御文字考慮追加
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx2.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx2.KeyPress
                'AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                'AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
        End Select

    End Sub

    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Dim str_Value As String
        Select Case colNo
            Case 0
                If Not CType(sender, DataGridView).Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                    str_Value = CType(sender, DataGridView).Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
                    Select Case e.ColumnIndex
                        Case 0
                            str_Value = Format(GCom.NzLong(str_Value), "#0")
                            CType(sender, DataGridView).Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Trim(str_Value)
                    End Select
                End If
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
            Case 1
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
        End Select
    End Sub
#End Region

#Region " イベント"
    '適用区分入力制限
    Private Sub TEKIYOU_KBN_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TEKIYOU_KBN_T.SelectedIndexChanged
        Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(GetIndex("TEKIYOU_KBN_T")), ComboBox))
        Select Case Ret
            Case 0 'カナ適用
                KTEKIYOU_T.Enabled = True
                NTEKIYOU_T.Enabled = False
                NTEKIYOU_T.Text = ""
            Case 1  '漢字適用
                KTEKIYOU_T.Enabled = False
                KTEKIYOU_T.Text = ""
                NTEKIYOU_T.Enabled = True
            Case Else '可変適用
                KTEKIYOU_T.Enabled = False
                KTEKIYOU_T.Text = ""
                NTEKIYOU_T.Enabled = False
                NTEKIYOU_T.Text = ""
        End Select
    End Sub
    '2017/04/24 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
    'WEB伝送ユーザ名入力制限
    Private Sub BAITAI_CODE_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BAITAI_CODE_T.SelectedIndexChanged
        Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(GetIndex("BAITAI_CODE_T")), ComboBox))
        If Ret = 3 Then
            '媒体=「WEB伝送」
            YOBI1_T.Enabled = True
        Else
            YOBI1_T.Enabled = False
        End If
    End Sub
    '2017/04/24 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
    'タブ間のフォーカス移動
    Private Sub TabIndexSetFocus_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) _
        Handles ITAKU_NJYU_T.KeyPress, MEISAI_OUT_T.KeyPress, TESUU_TABLE_ID_T.KeyPress, _
                SYOUGOU_KBN_T.KeyPress

        Try
            Select Case Microsoft.VisualBasic.Asc(e.KeyChar)
                Case Keys.Tab, Keys.Return
                    With Me.SSTab
                        Select Case CType(sender, Control).Name
                            Case "ITAKU_NJYU_T"
                                .SelectedIndex = 2
                                Me.FURI_DATE_T.Focus()
                            Case "MEISAI_OUT_T"
                                .SelectedIndex = 3
                                Me.KESSAI_KBN_T.Focus()
                            Case "TESUU_TABLE_ID_T"
                                .SelectedIndex = 4
                                Me.HANSOU_KBN_T.Focus()
                            Case "SYOUGOU_KBN_T"
                                .SelectedIndex = 0
                                Me.txtGAKUNEN_NAME1.Focus()
                        End Select
                    End With
            End Select
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    '数値だけを抜き出して金額用の表示編集を行う
    Private Sub Money_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles KOTEI_TESUU1_T.Validating, KOTEI_TESUU2_T.Validating, SOURYO_T.Validating
        Try
            With CType(sender, TextBox)
                .Text = String.Format("{0:#,##0}", GCom.NzDec(.Text, 1))
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金額表示変数", "失敗", ex.ToString)
        End Try
    End Sub
    '数値評価するがZERO埋めしない
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles FKEKKA_TBL_T.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("数値評価編集1", "失敗", ex.ToString)
        End Try
    End Sub
    '数値評価するがZERO埋めしないNo2
    Private Sub TextBox2_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles KIHTESUU_T.Validating
        Try
            CType(sender, TextBox).Text = GCom.NzDec(CType(sender, TextBox).Text, 0).ToString("0")
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("数値評価編集2", "失敗", ex.ToString)
        End Try
    End Sub
    '許可したい文字だけでフィルタをかける
    Private Sub AnyChar_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles DENWA_T.Validating, YUUBIN_T.Validating, FAX_T.Validating
        Try
            Dim OBJ As TextBox = CType(sender, TextBox)
            Select Case OBJ.Name.ToUpper
                Case "YUUBIN_T".ToUpper
                    CType(sender, TextBox).Text = GCom.NzAny("0123456789-", CType(sender, TextBox).Text)
                Case "DENWA_T".ToUpper, "FAX_T".ToUpper
                    CType(sender, TextBox).Text = GCom.NzAny("0123456789()-", CType(sender, TextBox).Text)
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("文字フィルタ", "失敗", ex.ToString)
        End Try

    End Sub
    '振込手数料基準ＩＤ
    Private Sub TESUU_TABLE_ID_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles TESUU_TABLE_ID_T.SelectedIndexChanged
        Dim FL As StreamReader = Nothing
        Try
            Dim Name() As String = {"TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                    "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                    "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"}

            Dim Index As Integer = GetIndex("TESUU_TABLE_ID_T")
            Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(Index), ComboBox))

            If Ret < 0 Then
                For Cnt As Integer = 0 To 8 Step 1
                    Index = GetIndex(Name(Cnt))
                    CType(oTORI(Index), Label).Text = ""
                    sTORI(Index, 4) = ""
                Next
            Else
                '2013/11/27 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                '振込手数料取得方法変更
                Dim SQL As New StringBuilder
                Dim oraReader As OracleDataReader = Nothing

                Try
                    With SQL
                        .Append("select * from TESUUMAST")
                        .Append(" where FSYORI_KBN_C = '1'")
                        .Append(" and SYUBETU_C = '91'")
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                        '.Append(" and TESUU_TABLE_ID_C = " & SQ(Ret.ToString))
                        .Append(" and TESUU_TABLE_ID_C = " & SQ(Format(Ret, "00")))
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
                        .Append(" and TAX_ID_C = " & SQ(TAX.ZEIRITSU_ID))
                    End With

                    If GCom.SetDynaset(SQL.ToString, oraReader) AndAlso oraReader.Read Then
                        For Cnt As Integer = 0 To 8 Step 1
                            Index = GetIndex(Name(Cnt))
                            Dim onData As Decimal

                            Select Case Cnt
                                Case 0 : onData = GCom.NzDec(oraReader.Item("TESUU_A1_C"), 0)
                                Case 1 : onData = GCom.NzDec(oraReader.Item("TESUU_A2_C"), 0)
                                Case 2 : onData = GCom.NzDec(oraReader.Item("TESUU_A3_C"), 0)
                                Case 3 : onData = GCom.NzDec(oraReader.Item("TESUU_B1_C"), 0)
                                Case 4 : onData = GCom.NzDec(oraReader.Item("TESUU_B2_C"), 0)
                                Case 5 : onData = GCom.NzDec(oraReader.Item("TESUU_B3_C"), 0)
                                Case 6 : onData = GCom.NzDec(oraReader.Item("TESUU_C1_C"), 0)
                                Case 7 : onData = GCom.NzDec(oraReader.Item("TESUU_C2_C"), 0)
                                Case 8 : onData = GCom.NzDec(oraReader.Item("TESUU_C3_C"), 0)
                            End Select

                            Dim Temp As String = onData.ToString("#,##0")
                            CType(oTORI(Index), Label).Text = Temp
                            sTORI(Index, 4) = Temp.Replace(","c, "")
                        Next

                    End If

                Catch ex As Exception
                    MainLOG.Write("振込手数料マスタ参照", "失敗", ex.ToString)
                Finally
                    If Not oraReader Is Nothing Then
                        oraReader.Close()
                    End If
                    oraReader = Nothing
                End Try

                'Dim FileName As String = ""
                'FileName &= GCom.SET_PATH(GCom.GetTXTFolder)
                'FileName &= "KFGMAST010_振込手数料基準ID.TXT"

                'FL = New StreamReader(FileName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                'Dim LineData As String = FL.ReadLine
                'Do While Not LineData Is Nothing

                '    '決められた項目数でないとダメ
                '    Dim Data() As String = LineData.Split(","c)
                '    If Data.Length = 11 Then

                '        '該当データが見つかれば
                '        If GCom.NzInt(Data(0), 0) = Ret Then

                '            For Cnt As Integer = 0 To 8 Step 1

                '                Index = GetIndex(Name(Cnt))
                '                Dim onData As Decimal = GCom.NzDec(Data(Cnt + 2), 0)
                '                Dim Temp As String = onData.ToString("#,##0")
                '                CType(oTORI(Index), Label).Text = Temp
                '                sTORI(Index, 4) = Temp.Replace(","c, "")
                '            Next
                '        End If
                '    End If
                '    LineData = FL.ReadLine
                'Loop
                'FL.Close()
                '2013/11/27 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Sub
    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbKana.SelectedIndexChanged
        Try
            '学校コードがReadOnlyの場合は処理終了
            If GAKKOU_CODE_T.ReadOnly Then
                Exit Sub
            End If

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    '取引先コード設定
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbGAKKOUNAME.SelectedIndexChanged
        Try
            '********************************************
            '学校一覧コンボ
            '********************************************
            If cmbGAKKOUNAME.SelectedIndex = -1 Then
                Exit Sub
            End If

            'COMBOBOX選択時学校名,学校コード設定
            GAKKOU_NNAME_G.Text = Trim(cmbGAKKOUNAME.Text)
            GAKKOU_CODE_T.Text = Trim(STR_GCOAD(cmbGAKKOUNAME.SelectedIndex))

            Dim SQL As String = ""
            SQL = " SELECT GAKKOU_KNAME_G FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G='" & GAKKOU_CODE_T.Text & "'"

            GAKKOU_KNAME_G.Text = Trim(GFUNC_GET_SELECTSQL_ITEM(SQL, "GAKKOU_KNAME_G"))

            '参照ボタンにFOCUS
            CmdSelect.Focus()
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles GAKKOU_CODE_T.Validating, ITAKU_CODE_T.Validating, _
                TKIN_NO_T.Validating, TSIT_NO_T.Validating, KOUZA_T.Validating, FURI_CODE_T.Validating, _
                KIGYO_CODE_T.Validating, KOKYAKU_NO_T.Validating, KANREN_KIGYO_CODE_T.Validating, KOKYAKU_NO_T.Validating, _
                KEIYAKU_DATE_T.Validating, KEIYAKU_DATE_T1.Validating, KEIYAKU_DATE_T2.Validating, _
                KAISI_DATE_T.Validating, KAISI_DATE_T1.Validating, KAISI_DATE_T2.Validating, SYURYOU_DATE_T.Validating, _
                SYURYOU_DATE_T1.Validating, SYURYOU_DATE_T2.Validating, KEIYAKU_NO_T.Validating, TESUUTYO_KOUZA_T.Validating, HONBU_KOUZA_T.Validating, TUKEKOUZA_T.Validating, _
                HENKYAKU_SIT_NO_T.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try

    End Sub
    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：金融機関の場合)
    Private Sub Bank_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles TKIN_NO_T.Validating, TUKEKIN_NO_T.Validating

        Try
            Dim OBJ As TextBox = CType(sender, TextBox)
            Dim Temp As String = GCom.NzDec(OBJ.Text, "")
            If Temp.Length = 0 Then
                OBJ.Text = ""
                GetLabel(OBJ.Name).Text = ""
            Else
                Call GCom.NzNumberString(OBJ, True)
                GetLabel(OBJ.Name).Text = GCom.GetBKBRName(OBJ.Text, "", 30)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金融機関ゼロパディングフォーマット", "失敗", ex.ToString)
        End Try

    End Sub
    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：支店の場合)
    Private Sub Branch_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles TSIT_NO_T.Validating, TUKESIT_NO_T.Validating, TESUUTYO_SIT_T.Validating, TORIMATOME_SIT_T.Validating, _
                HENKYAKU_SIT_NO_T.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Dim Temp As String = GCom.NzDec(OBJ.Text, "")
        If Temp.Length = 0 Then
            OBJ.Text = ""
            GetLabel(OBJ.Name).Text = ""
        Else
            Call GCom.NzNumberString(OBJ, True)
            Dim BKCode As String = ""
            Dim BRCode As String = OBJ.Text
            Select Case OBJ.Name.ToUpper
                Case "TSIT_NO_T".ToUpper
                    BKCode = GCom.NzDec(Me.TKIN_NO_T.Text, "")
                Case "TUKESIT_NO_T".ToUpper
                    BKCode = GCom.NzDec(Me.TUKEKIN_NO_T.Text, "")
                Case Else
                    If BRCode.Trim.Length > 0 Then
                        BKCode = SELF_BANK_CODE
                    End If
            End Select

            GetLabel(OBJ.Name).Text = GCom.GetBKBRName(BKCode, BRCode, 30)
        End If
    End Sub
    Private Sub NzCheckString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles GAKKOU_KNAME_G.Validating, KTEKIYOU_T.Validating, DENPYO_BIKOU1_T.Validating, DENPYO_BIKOU2_T.Validating, TUKEMEIGI_T.Validating

        Call GCom.NzCheckString(CType(sender, TextBox))
        With CType(sender, TextBox)
            Select Case .Name
                Case "GAKKOU_KNAME_G", "TUKEMEIGI_T", "KTEKIYOU_T", "DENPYO_BIKOU1_T", "DENPYO_BIKOU2_T"
                    Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
            End Select
        End With
    End Sub
    '全角混入領域バイト数調整用
    '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ START
    Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles GAKKOU_NNAME_G.Validating, ITAKU_NJYU_T.Validating, NTEKIYOU_T.Validating, FILE_NAME_T.Validating, _
                      HANSOU_ROOT1_T.Validating, HANSOU_ROOT2_T.Validating, HANSOU_ROOT3_T.Validating, YOBI1_T.Validating

        Dim Index As Integer
        With CType(sender, TextBox)
            Select Case .Name
                Case "GAKKOU_NNAME_G" : Index = GetIndex("GAKKOU_NNAME_G")
                Case "ITAKU_NJYU_T" : Index = GetIndex("ITAKU_NJYU_T")
                Case "NTEKIYOU_T" : Index = GetIndex("NTEKIYOU_T")
                Case "FILE_NAME_T" : Index = GetIndex("FILE_NAME_T")
                Case "HANSOU_ROOT1_T" : Index = GetIndex("HANSOU_ROOT1_T")
                Case "HANSOU_ROOT2_T" : Index = GetIndex("HANSOU_ROOT2_T")
                Case "HANSOU_ROOT3_T" : Index = GetIndex("HANSOU_ROOT3_T")
                Case "YOBI1_T" : Index = GetIndex("YOBI1_T")
            End Select
            .Text = GCom.GetLimitString(.Text, nTORI(Index, 0))
        End With
    End Sub
    'Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    '         Handles GAKKOU_NNAME_G.Validating, ITAKU_NJYU_T.Validating, NTEKIYOU_T.Validating, FILE_NAME_T.Validating, _
    '                 HANSOU_ROOT1_T.Validating, HANSOU_ROOT2_T.Validating, HANSOU_ROOT3_T.Validating

    '    Dim Index As Integer
    '    With CType(sender, TextBox)
    '        Select Case .Name
    '            Case "GAKKOU_NNAME_G" : Index = GetIndex("GAKKOU_NNAME_G")
    '            Case "ITAKU_NJYU_T" : Index = GetIndex("ITAKU_NJYU_T")
    '            Case "NTEKIYOU_T" : Index = GetIndex("NTEKIYOU_T")
    '            Case "FILE_NAME_T" : Index = GetIndex("FILE_NAME_T")
    '            Case "HANSOU_ROOT1_T" : Index = GetIndex("HANSOU_ROOT1_T")
    '            Case "HANSOU_ROOT2_T" : Index = GetIndex("HANSOU_ROOT2_T")
    '            Case "HANSOU_ROOT3_T" : Index = GetIndex("HANSOU_ROOT3_T")
    '        End Select
    '        .Text = GCom.GetLimitString(.Text, nTORI(Index, 0))
    '    End With
    'End Sub
    '2017/04/24 タスク）西野 CHG 標準版修正（学校WEB伝送対応）------------------------------------ END
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " 相関チェック"

    '
    ' 機　能 : 項目間の相関をチェックする
    '
    ' 戻り値 : チェック抵触コントロール
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : 相関チェック用の関数(戻り値がZERO以外はエラー)
    '    
    Private Function CheckMutualRelation(ByRef MSG As String) As Integer
        Dim Ret As Integer
        Dim Index As Integer
        ErrMsgFlg = True  'エラーメッセージ表示判定
        '必須項目入力有無チェック
        Index = CheckMutualRelation_001(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '文字属性チェック
        Index = CheckMutualRelation_002(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '開始／終了日付チェック
        Index = CheckMutualRelation_003(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '摘要区分チェック
        Index = CheckMutualRelation_004(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '金融機関情報チェック
        Index = CheckMutualRelation_005(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '再振関連項目チェック
        Index = CheckMutualRelation_007(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '委託者コード関連項目チェック
        Index = CheckMutualRelation_008(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '日項目範囲チェック
        Index = CheckMutualRelation_010(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '決済関連項目チェック
        Index = CheckMutualRelation_011(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '備考項目チェック
        Index = CheckMutualRelation_012(MSG)
        If Not Index = -1 Then
            Return Index
        End If

        '学年情報のチェック
        Try
            Dim UseNen As Integer = GCom.NzInt(Me.SIYOU_GAKUNEN_T.Text, 0)
            Dim MaxNen As Integer = GCom.NzInt(Me.SAIKOU_GAKUNEN_T.Text, 0)

            If UseNen < MaxNen Then
                MessageBox.Show(G_MSG0001W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                SIYOU_GAKUNEN_L.Focus()
                ErrMsgFlg = False
                Return -100
                Exit Function
            End If

            'クラス情報チェック
            For NenCnt As Integer = 1 To UseNen
                Dim dvg As DataGridView = Nothing
                Select Case (NenCnt)
                    Case 1
                        dvg = DataGridView1
                    Case 2
                        dvg = DataGridView2
                    Case 3
                        dvg = DataGridView3
                    Case 4
                        dvg = DataGridView4
                    Case 5
                        dvg = DataGridView5
                    Case 6
                        dvg = DataGridView6
                    Case 7
                        dvg = DataGridView7
                    Case 8
                        dvg = DataGridView8
                    Case 9
                        dvg = DataGridView9
                End Select

                If PFUNC_Check_Class(CByte(NenCnt), dvg, MSG) = False Then
                    SSTab.SelectedIndex = 0
                    TabClass.SelectedIndex = NenCnt - 1
                    ErrMsgFlg = False
                    Return -100
                End If

            Next NenCnt
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("学年・クラスチェック", "失敗", ex.ToString)
            ErrMsgFlg = False
            Return -100
        End Try

        Try
            '進級年度
            Index = GetIndex("SINKYU_NENDO_T")
            If SyoriKBN = 0 Then
                Dim onDate As Date
                Dim onText(2) As Integer
                onText(0) = GCom.NzInt(Me.KAISI_DATE_T.Text, 0)
                onText(1) = GCom.NzInt(Me.KAISI_DATE_T1.Text, 0)
                onText(2) = GCom.NzInt(Me.KAISI_DATE_T2.Text, 0)
                Ret = GCom.SET_DATE(onDate, onText)
                If Ret = -1 Then
                    Select Case Month(onDate)
                        Case Is <= 3
                            Me.SINKYU_NENDO_T.Text = (Year(onDate) - 1).ToString
                        Case Else
                            Me.SINKYU_NENDO_T.Text = Year(onDate).ToString
                    End Select
                    sTORI(Index, 3) = Me.SINKYU_NENDO_T.Text
                    sTORI(Index, 4) = sTORI(Index, 3)
                Else
                    Select Case CInt(lblDate.Text.Substring(5, 2))
                        Case Is <= 3
                            sTORI(Index, 3) = (CInt(lblDate.Text.Substring(0, 4)) - 1).ToString
                        Case Else
                            sTORI(Index, 3) = (lblDate.Text.Substring(0, 4))
                    End Select
                    sTORI(Index, 4) = sTORI(Index, 3)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("進級年度チェック", "失敗", ex.ToString)
            ErrMsgFlg = False
            Return -100
        End Try

        Return -1
    End Function

    Private Function PFUNC_Check_Class(ByVal pByte_Page As Byte, _
                                   ByVal dgv As DataGridView, ByRef MSG As String) As Boolean
        '********************************************
        'クラス情報チェック
        '********************************************
        Dim ret As Boolean = False

        Try
            Dim blnCLS_flg As Boolean = False '2007/02/12　クラス存在チェックフラグ
            Dim Text As TextBox = Nothing

            Select Case (pByte_Page)
                Case 1
                    Text = txtGAKUNEN_NAME1
                Case 2
                    Text = txtGAKUNEN_NAME2
                Case 3
                    Text = txtGAKUNEN_NAME3
                Case 4
                    Text = txtGAKUNEN_NAME4
                Case 5
                    Text = txtGAKUNEN_NAME5
                Case 6
                    Text = txtGAKUNEN_NAME6
                Case 7
                    Text = txtGAKUNEN_NAME7
                Case 8
                    Text = txtGAKUNEN_NAME8
                Case 9
                    Text = txtGAKUNEN_NAME9
            End Select

            '学年名称未入力チェック
            If Text.Text = "" Then
                MSG = String.Format(MSG0285W, pByte_Page & "学年名称")
                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Try
            Else
                If StrConv(Text.Text, VbStrConv.Wide).Trim.Length > 10 Then
                    MSG = pByte_Page & "学年名称は全角文字で10文字以内に設定してください"
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If
            End If

            'クラスコード・クラス名称チェック
            With dgv
                For ClassCnt As Integer = 0 To 19
                    Select Case (True)
                        Case Trim(CStr(.Rows(ClassCnt).Cells(0).Value)) = "" AndAlso Trim(CStr(.Rows(ClassCnt).Cells(1).Value)) <> ""
                            MSG = String.Format(MSG0285W, pByte_Page & "学年のクラスコード")
                            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        Case Trim(CStr(.Rows(ClassCnt).Cells(0).Value)) <> "" AndAlso Trim(CStr(.Rows(ClassCnt).Cells(1).Value)) = ""
                            MSG = String.Format(MSG0285W, pByte_Page & "学年のクラス名称")
                            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        Case Trim(CStr(.Rows(ClassCnt).Cells(0).Value)) <> "" AndAlso Trim(CStr(.Rows(ClassCnt).Cells(1).Value)) <> ""
                            If StrConv(Trim(CStr(.Rows(ClassCnt).Cells(1).Value)), VbStrConv.Wide).Length > 10 Then
                                MSG = pByte_Page & "学年のクラス名称は全角文字で10文字以内に設定してください"
                                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Try
                            Else
                                blnCLS_flg = True
                            End If
                    End Select
                Next

            End With

            'クラスが１件も入力されていない場合、メッセージを表示する
            If blnCLS_flg = False Then
                MSG = String.Format(MSG0285W, pByte_Page & "学年のクラス")
                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Try
            End If

            'クラスコード重複チェック
            With dgv
                For ClassCnt1 As Integer = 0 To 18
                    For ClassCnt2 As Integer = ClassCnt1 + 1 To 19
                        If Trim(CStr(.Rows(ClassCnt1).Cells(0).Value)) <> "" Then
                            If Trim(CStr(.Rows(ClassCnt1).Cells(0).Value)) = Trim(CStr(.Rows(ClassCnt2).Cells(0).Value)) Then
                                MSG = pByte_Page & "学年のクラスコードが重複しています"
                                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Try
                            End If
                        End If
                    Next
                Next
            End With

            ret = True

        Catch ex As Exception
            ret = False
        End Try

        Return ret

    End Function
    '
    ' 機　能 : 必須入力項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_001(ByRef MSG As String) As Integer


        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            For Index = 0 To MaxColumns Step 1

                If GCom.NzInt(sTORI(Index, 5)) = 1 Then

                    If sTORI(Index, 4).Trim.Length <= 0 Then

                        If sTORI(Index, 1) = "TESUUMAT_NO_T" Then
                            '手数料集計周期
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_MONTH_T" Then
                            '集計基準月
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_ENDDAY_T" Then
                            '集計終了日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_KIJYUN2_T" Then
                            '集計基準
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_PATN_T" Then
                            '集計方法
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "KESSAI_DAY_T" Then
                            '資金確保日数／基準日
                            Select Case GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4))
                                Case 3
                                Case Else
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUTYO_DAY_T" Then
                            '手数料徴求日数／基準日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4))
                                Case 3, 4
                                Case Else
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                            End Select
                        Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                        End If
                    End If

                    If sTORI(Index, 1) = "KIGYO_CODE_T" Then    '企業コード
                        If sTORI(Index, 4).Trim.Length <= 1 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                        End If
                    End If
                Else
                    If sTORI(Index, 4).Trim.Length <= 0 Then
                        If sTORI(Index, 1) = "TESUUMAT_NO_T" Then
                            '手数料集計周期
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_MONTH_T" Then
                            '集計基準月
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_ENDDAY_T" Then
                            '集計終了日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_KIJYUN2_T" Then
                            '集計基準
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_PATN_T" Then
                            '集計方法
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        End If
                    End If
                End If


                'テキストボックスの場合
                If TypeOf oTORI(Index) Is TextBox Then

                    If sTORI(Index, 4).Trim.Length > 0 Then

                        Select Case sTORI(Index, 1).ToUpper
                            Case "GAKKOU_KNAME_G", _
                                "KTEKIYOU_T", _
                                "TUKEMEIGI_T"
                                '半角英数カナ評価(全角文字抹消)

                                Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
                                Dim onByte() As Byte = JISEncoding.GetBytes(sTORI(Index, 4))

                                If onByte.Length > sTORI(Index, 4).Length Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0297W, sTORI(Index, 0)))
                                Else
                                    Select Case sTORI(Index, 1).ToUpper
                                        Case "GAKKOU_KNAME_G", _
                                            "TUKEMEIGI_T"
                                            '全銀規制文字

                                            If Not GCom.CheckZenginChar(CType(oTORI(Index), TextBox)) Then
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0298W, sTORI(Index, 0)))
                                            End If
                                    End Select
                                End If
                        End Select
                    Else
                        Select Case sTORI(Index, 1).ToUpper
                            Case "FILE_NAME_T"
                                'ファイル名かつ媒体がFD(01)の場合は必須チェックを行う
                                If GCom.NzInt(sTORI(GetIndex("BAITAI_CODE_T"), 4)) = 1 Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(MSG0153W)
                                End If
                            Case "HONBU_KOUZA_T", "KESSAI_DAY_T", "TUKEKIN_NO_T", "TUKESIT_NO_T", "TUKEKOUZA_T", "TUKEMEIGI_T", _
                                 "TESUUTYO_SIT_T", "TESUUTYO_KOUZA_T", "TESUUTYO_DAY_T"
                                If GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4)) <> 99 Then    '決済対象外以外必須
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                                End If
                        End Select

                    End If
                End If

            Next Index

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then
            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 文字属性チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_002(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            For Index = 1 To MaxColumns Step 1

                'テキストボックスの場合(念のため：不要の様な気がする)
                If TypeOf oTORI(Index) Is TextBox Then

                    If sTORI(Index, 4).Trim.Length > 0 Then

                        Select Case sTORI(Index, 1).ToUpper
                            Case "GAKKOU_KNAME_G", _
                                "KTEKIYOU_T", _
                                "TUKEMEIGI_T"
                                Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
                                Dim onByte() As Byte = JISEncoding.GetBytes(sTORI(Index, 4))

                                If onByte.Length > sTORI(Index, 4).Length Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0297W, sTORI(Index, 0)))
                                Else
                                    Select Case sTORI(Index, 1).ToUpper
                                        Case "GAKKOU_KNAME_G", _
                                            "TUKEMEIGI_T"
                                            '全銀規制文字

                                            If Not GCom.CheckZenginChar(CType(oTORI(Index), TextBox)) Then
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0298W, sTORI(Index, 0)))
                                            End If
                                    End Select
                                End If
                        End Select
                    End If
                End If
            Next Index

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 日付歴日チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_003(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Ret As Integer
            Dim onDate As Date
            Dim onText(2) As Integer

            '契約日
            Index = GetIndex("KAISI_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日範囲チェック(月)(MSG0022W)
                If onText(1) < 1 OrElse onText(1) > 12 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0022W)
                End If
            End If

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日範囲チェック(日)(MSG0025W)
                If onText(2) < 1 OrElse onText(2) > 31 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0025W)
                End If
            End If

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日日付整合性チェック(MSG0027W)
                Ret = GCom.SET_DATE(onDate, onText)
                If Not Ret = -1 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0027W)
                End If
            End If

            '開始年月日
            Dim StartDate As String = "00000000"
            Index = GetIndex("KAISI_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            '開始年月必須チェック(年)(MSG0168W)
            If KAISI_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0168W)
            End If
            '開始年月必須チェック(月)(MSG0170W)
            If KAISI_DATE_T1.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0170W)
            End If
            '開始年月必須チェック(日)(MSG0172W)
            If KAISI_DATE_T2.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0172W)
            End If
            '開始年月範囲チェック(月)(MSG0022W)
            If onText(1) < 1 OrElse onText(1) > 12 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0022W)
            End If

            '開始年月範囲チェック(日)(MSG0025W)
            If onText(2) < 1 OrElse onText(2) > 31 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0025W)
            End If

            '開始年月日付整合性チェック(MSG0027W)
            Ret = GCom.SET_DATE(onDate, onText)
            If Not Ret = -1 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0027W)
            End If

            StartDate = sTORI(Index, 4).Trim

            '終了年月日
            Dim FinalDate As String = "99999999"
            Index = GetIndex("SYURYOU_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            '終了年月必須チェック(年)(MSG0174W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0174W)
            End If
            '終了年月必須チェック(月)(MSG0176W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0176W)
            End If
            '終了年月必須チェック(日)(MSG0178W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0178W)
            End If
            '終了年月日範囲チェック(月)(MSG0022W)
            If onText(1) < 1 OrElse onText(1) > 12 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0022W)
            End If

            '終了年月日範囲チェック(日)(MSG0025W)
            If onText(2) < 1 OrElse onText(2) > 31 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0025W)
            End If

            '終了年月日日付整合性チェック(MSG0027W)
            Ret = GCom.SET_DATE(onDate, onText)
            If Not Ret = -1 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0027W)
            End If

            FinalDate = sTORI(Index, 4).Trim

            If GCom.NzDec(StartDate) >= GCom.NzDec(FinalDate) Then

                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0099W)
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 摘要区分項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_004(ByRef MSG As String) As Integer
        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            '摘要区分
            Select Case GCom.NzInt(sTORI(GetIndex("TEKIYOU_KBN_T"), 4))
                Case Is = 0
                    'カナ摘要(摘要区分は必須)

                    Index = GetIndex("KTEKIYOU_T")
                    '2011/06/15 標準版修正 摘要は必須項目から除外--------------START 
                    'If sTORI(Index, 4).Length = 0 Then
                    '    'カナ摘要の設定が抜けている
                    '    MsgIcon = MessageBoxIcon.Warning
                    '    'カナ摘要必須チェック(MSG0212W)
                    '    Throw New Exception(MSG0212W)
                    'End If
                    '2011/06/15 標準版修正 摘要は必須項目から除外--------------END 
                   
                Case Is = 1
                    '漢字摘要(摘要区分は必須)

                    Index = GetIndex("NTEKIYOU_T")
                    '2011/06/15 標準版修正 摘要は必須項目から除外--------------START 
                                       'If sTORI(Index, 4).Length = 0 Then
                    '    '漢字摘要の設定が抜けている
                    '    MsgIcon = MessageBoxIcon.Warning
                    '    '漢字摘要必須チェック(MSG0213W)
                    '    Throw New Exception(MSG0213W)
                    'End If
                    '2011/06/15 標準版修正 摘要は必須項目から除外--------------END 
                Case Else
                    '可変摘要(摘要区分は必須)

            End Select

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function
    ' 機　能 : 金融機関情報項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_005(ByRef MSG As String) As Integer
        Try
            Dim Ret As Integer
            Dim BANK As Integer
            Dim BRANCH As Integer

            'ヘダー金融機関情報
            BANK = GetIndex("TKIN_NO_T")
            BRANCH = GetIndex("TSIT_NO_T")

            If GCom.NzInt(sTORI(BANK, 4)) + GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                Ret = GCom.CheckBankBranch(sTORI(BANK, 4), sTORI(BRANCH, 4))
                Select Case Ret
                    Case 1
                        '銀行無し
                        Return BANK
                    Case 2
                        '支店無し
                        Return BRANCH
                End Select
            End If

            '決済金融機関情報
            BANK = GetIndex("TUKEKIN_NO_T")
            BRANCH = GetIndex("TUKESIT_NO_T")

            If GCom.NzInt(sTORI(BANK, 4)) + GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                Ret = GCom.CheckBankBranch(sTORI(BANK, 4), sTORI(BRANCH, 4))
                Select Case Ret
                    Case 1
                        '銀行無し
                        Return BANK
                    Case 2
                        '支店無し
                        Return BRANCH
                End Select
            End If

            '手数料徴求支店(稼働金融機関イニファイル)     
            Dim MyBank As String = SELF_BANK_CODE

            Dim FieldName() As String = {"TESUUTYO_SIT_T", "TORIMATOME_SIT_T"}

            For Cnt As Integer = 0 To 1 Step 1

                BRANCH = GetIndex(FieldName(Cnt))
                If GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                    Ret = GCom.CheckBankBranch(MyBank, sTORI(BRANCH, 4))
                    If Ret = 2 Then
                        '支店無し
                        Return BRANCH
                    End If
                End If
            Next Cnt

            Return -1
        Catch ex As Exception
            MSG = ex.Message
        End Try
    End Function

    '
    ' 機　能 : 再振関連項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_007(ByRef MSG As String) As Integer
        With GCom.GLog
            .Job2 = "再振関連チェック"
        End With

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Select Case GCom.NzInt(sTORI(GetIndex("SFURI_SYUBETU_T"), 4))
                Case 1, 2
                    '再振契約あり

                    '再振日
                    Index = GetIndex("SFURI_DATE_T")
                    '2011/06/15 標準版修正 再振ありの場合、再振日をゼロ埋め--------------START
                    Dim SFURI_DATE As String = GCom.NzDec(sTORI(Index, 4), "").PadLeft(2, "0"c)
                    'Dim SFURI_DATE As String = GCom.NzDec(sTORI(Index, 4), "")
                    '2011/06/15 標準版修正 再振ありの場合、再振日をゼロ埋め--------------END
                    If SFURI_DATE = "" Then
                        '再振日必須チェック
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "再振日"))
                    End If
                    If Me.SFURI_DATE_T.Text = Me.FURI_DATE_T.Text Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("振替日と再振日が同一日が指定されています。")
                    End If

                    '手数料徴求期日区分
                    Index = GetIndex("TESUUTYO_KIJITSU_T")
                    If sTORI(Index, 4).Length = 0 Then

                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("再振＝有り、の場合には「営業日数指定」を行って下さい。")
                    End If

                    '2008/04/25 mitsu 再振休日コード追加
                    Index = GetIndex("SFURI_KYU_CODE_T")
                    Dim SFURI_KYU_CODE As String = GCom.NzDec(sTORI(Index, 4), "")
                    If SFURI_KYU_CODE = "" Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("再振休日コードを指定して下さい。")
                    End If
                Case Else
                    '再振契約なし

                    '再振日
                    Index = GetIndex("SFURI_DATE_T")
                    Dim SFURI_DATE As String = GCom.NzDec(sTORI(Index, 4), "")
                    If Not SFURI_DATE = "" Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("再振日が指定されています。")
                    End If

            End Select

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message & Space(5)
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 委託者コード関連項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_008(ByRef MSG As String) As Integer
        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error

        Dim SQL As String
        Dim REC As OracleDataReader = Nothing
        Try
            Index = GetIndex("ITAKU_CODE_T")
            Dim ITAKU_CODE As String = sTORI(Index, 4)

            '2010/09/08.Sakon　委託者コードの下１桁チェックは行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'If Not ITAKU_CODE.EndsWith("0") Then
            '    MsgIcon = MessageBoxIcon.Warning
            '    Throw New Exception("委託者コードの下１桁はゼロである必要があります。")
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            Index = GetIndex("GAKKOU_CODE_T")
            Dim TORI_CODE As String = sTORI(Index, 4)
            Index = GetIndex("ITAKU_CODE_T")

            SQL = "SELECT COUNT(*) FROM GAKMAST2"
            SQL &= " WHERE ITAKU_CODE_T = '" & ITAKU_CODE & "'"
            SQL &= " AND NOT GAKKOU_CODE_T = '" & TORI_CODE & "'"

            '2010/09/08.Sakon　委託者コードは一意でなくてもよい +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If GCom.SetDynaset(SQL, REC) AndAlso _
                    REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
                '2012/10/03 saitou 標準修正 MODIFY -------------------------------------------------->>>>
                'メッセージ定数化＆メッセージアイコン設定＆メッセージタイトル設定
                If MessageBox.Show(MSG0036I.Replace("取引先マスタ", "学校マスタ"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception("登録処理がキャンセルされました。")
                End If
                'If MessageBox.Show("この委託者コードを有する学校が既に学校マスタに存在します。" & vbCrLf & _
                '                   "登録してもよいですか？", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
                '    MsgIcon = MessageBoxIcon.Warning
                '    Throw New Exception("登録処理がキャンセルされました。")
                'End If
                '2012/10/03 saitou 標準修正 MODIFY --------------------------------------------------<<<<

            End If
            'If GCom.SetDynaset(SQL, REC) AndAlso _
            '        REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
            '    MsgIcon = MessageBoxIcon.Warning
            '    Throw New Exception("この委託者コードを有する学校が既に学校マスタに存在します。")
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            '2010/09/08.Sakon　委託者コードの下１桁変更を行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            SQL = "SELECT COUNT(*) FROM TORIMAST"
            SQL &= " WHERE ITAKU_CODE_T = '" & ITAKU_CODE & "'"
            SQL &= " AND NOT TORIS_CODE_T = '" & TORI_CODE & "'"

            'SQL = "SELECT COUNT(*) FROM TORIMAST"
            'SQL &= " WHERE ITAKU_CODE_T IN"
            'SQL &= " ('" & ITAKU_CODE.Substring(0, 9) & "0" & "'"
            'SQL &= ", '" & ITAKU_CODE.Substring(0, 9) & "1" & "'"
            'SQL &= ", '" & ITAKU_CODE.Substring(0, 9) & "2" & "'"
            'SQL &= ", '" & ITAKU_CODE.Substring(0, 9) & "3" & "'"
            'SQL &= ")"
            'SQL &= " AND NOT TORIS_CODE_T = '" & TORI_CODE & "'"
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '2010/09/08.Sakon　委託者コードは一意でなくてもよい +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If GCom.SetDynaset(SQL, REC) AndAlso _
                    REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
                '2012/10/03 saitou 標準修正 MODIFY -------------------------------------------------->>>>
                'メッセージ定数化＆メッセージアイコン設定＆メッセージタイトル設定
                If MessageBox.Show(MSG0036I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception("登録処理がキャンセルされました。")
                End If
                'If MessageBox.Show("この委託者コードを有する取引先が既に取引先マスタに存在します。" & vbCrLf & _
                '                                  "登録してもよいですか？", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
                '    MsgIcon = MessageBoxIcon.Warning
                '    Throw New Exception("登録処理がキャンセルされました。")
                'End If
                '2012/10/03 saitou 標準修正 MODIFY --------------------------------------------------<<<<
            End If
            'If GCom.SetDynaset(SQL, REC) AndAlso _
            '        REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
            '    MsgIcon = MessageBoxIcon.Warning
            '    Throw New Exception("この委託者コードを有する取引先が既に取引先マスタに存在します。")
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            '2010/09/08.Sakon　委託者コードの下１桁変更を行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            SQL = "SELECT COUNT(*) FROM TORIMAST"
            ' 2016/02/08 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
            'SQL &= " WHERE BAITAI_KANRI_CODE_T = '" & ITAKU_CODE & "'"
            SQL &= " WHERE ITAKU_KANRI_CODE_T = '" & ITAKU_CODE & "'"
            ' 2016/02/08 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
            SQL &= " AND NOT TORIS_CODE_T = '" & TORI_CODE & "'"
            'SQL = "SELECT COUNT(*) FROM TORIMAST"
            'SQL &= " WHERE BAITAI_KANRI_CODE_T IN"
            'SQL &= " ('" & ITAKU_CODE.Substring(0, 9) & "0" & "'"
            'SQL &= ")"
            'SQL &= " AND NOT TORIS_CODE_T = '" & TORI_CODE & "'"
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '2010/09/08.Sakon　委託者コードは一意でなくてもよい +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If GCom.SetDynaset(SQL, REC) AndAlso _
                    REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
                '2012/10/03 saitou 標準修正 MODIFY -------------------------------------------------->>>>
                'メッセージ定数化＆メッセージアイコン設定＆メッセージタイトル設定
                If MessageBox.Show(MSG0036I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception("登録処理がキャンセルされました。")
                End If
                'If MessageBox.Show("この委託者コードを有する取引先が既に取引先マスタに存在します。" & vbCrLf & _
                '                                  "登録してもよいですか？", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
                '    MsgIcon = MessageBoxIcon.Warning
                '    Throw New Exception("登録処理がキャンセルされました。")
                'End If
                '2012/10/03 saitou 標準修正 MODIFY --------------------------------------------------<<<<
            End If
            'If GCom.SetDynaset(SQL, REC) AndAlso _
            '        REC.Read AndAlso GCom.NzDec(REC.Item(0), 0) > 0 Then
            '    MsgIcon = MessageBoxIcon.Warning
            '    Throw New Exception("この委託者コード(委託管理コード)を有する取引先が既に取引先マスタに存在します。")
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Index = GetIndex("TAKO_KBN_T")
            '0, 他行データ作成非対象
            '1, 他行データ作成対象
            If GCom.NzInt(sTORI(Index, 3), 0) > GCom.NzInt(sTORI(Index, 4), 0) Then

                SQL = "SELECT COUNT(*) COUNTER FROM TAKOUMAST"
                SQL &= " WHERE TORIS_CODE_V = '" & TORI_CODE & "'"

                If GCom.SetDynaset(SQL, REC) AndAlso _
                        REC.Read AndAlso GCom.NzDec(REC.Item("COUNTER"), 0) > 0 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception("他行マスタにレコードが存在します。変更できません。")
                End If
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Index
    End Function


    '
    ' 機　能 : 日項目範囲チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_010(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim KESSAI_KIJITSU_Index As Integer = GetIndex("KESSAI_KIJITSU_T")
            Dim kESSAI_KBN As Integer = GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4)) '決済区分
            Index = GetIndex("KESSAI_DAY_T")
            If kESSAI_KBN <> 99 Then    '決済対象外以外
                Select Case GCom.NzInt(sTORI(KESSAI_KIJITSU_Index, 4))
                    Case 0
                        '0:営業日数指定
                        '日数/基準日＜決済情報＞範囲チェック(MSG????W)
                        If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0301W)
                        End If
                    Case 1, 2
                        '1:基準日指定, 2:翌月基準日指定
                        Select Case GCom.NzInt(sTORI(Index, 4))
                            Case 1 To 31
                            Case Else  '日数/基準日＜決済情報＞範囲チェック(MSG0025W)
                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0025W)
                        End Select
                End Select
            End If

            '*** 手数料徴求区分が「一括徴求(1)」　　　　　　　　               ***
            '*** の場合,手数料徴求日数／基準日はチェック対象外                 ***

            '手数料徴求日数／基準日
            Index = GetIndex("TESUUTYO_DAY_T")
            If kESSAI_KBN <> 99 Then    '決済対象外以外
                '*** 都度徴求なら、IF先のチェック判定を行う            ***
                If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "0" Then
                    Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4))
                        Case 0
                            '0, 営業日数指定
                            '日数/基準日＜決済情報＞範囲チェック(MSG????W)
                            If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0301W)
                            ElseIf GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0306W)
                            End If
                        Case 1
                            '1, 基準日指定
                            Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                                Case "1"
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    Else
                                        If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                            Select Case GCom.NzInt(sTORI(Index, 4))
                                                Case 1 To 31
                                                Case Else  '日数/基準日＜決済情報＞範囲チェック
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(MSG0025W)
                                            End Select
                                        End If
                                    End If
                                Case Else
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    End If
                            End Select
                        Case 2
                            '2, 翌月基準日指定
                            Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                                Case "1"
                                    If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                        Select Case GCom.NzInt(sTORI(Index, 4))
                                            Case 1 To 31
                                            Case Else
                                                Return Index
                                        End Select
                                    End If
                                Case Else
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    Else
                                        If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                            Select Case GCom.NzInt(sTORI(Index, 4))
                                                Case 1 To 31
                                                Case Else  '日数/基準日＜決済情報＞範囲チェック
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(MSG0025W)
                                            End Select
                                        End If
                                    End If
                            End Select
                    End Select
                End If
            End If

            '手数料区分
            Select Case sTORI(GetIndex("TESUUTYO_KBN_T"), 4)
                Case Is = "0", "2", "3"
                    '都度徴求,特別免除,別途徴求
                    Index = GetIndex("TESUUMAT_NO_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then

                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                    Index = GetIndex("TESUUMAT_MONTH_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                    Index = GetIndex("TESUUMAT_ENDDAY_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                Case Is = "1"
                    '一括徴求
                    Index = GetIndex("TESUUMAT_MONTH_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "集計基準月"))
                    End If

                    Index = GetIndex("TESUUMAT_ENDDAY_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "集計終了日"))
                    End If

                    Index = GetIndex("TESUUMAT_NO_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "手数料集計周期"))
                    Else
                        If Not (12 Mod GCom.NzInt(sTORI(Index, 4))) = 0 Then

                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0308W)
                        End If
                    End If
                Case Else
                    '他
            End Select

            '手数料徴求期日区分
            Index = GetIndex("TESUUTYO_KIJITSU_T")

            '都度徴求なら、IF先の判定を行う
            If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "0" Then

                Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                    Case "1"
                        If GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0306W)
                        End If
                    Case Else
                        If Not sTORI(GetIndex("KESSAI_KIJITSU_T"), 4) = sTORI(Index, 4) Then

                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0309W)
                        End If
                End Select
            End If
            '月範囲
            If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                Index = GetIndex("TESUUMAT_MONTH_T")
                Select Case GCom.NzInt(sTORI(Index, 4))
                    Case 1 To 12
                    Case Else
                        Return Index
                End Select
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function
    ' 機　能 : 決済関連項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_011(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Select Case GCom.NzDec(sTORI(GetIndex("KESSAI_SYUBETU_T"), 4), "")
                Case "0" '学校単位
                Case "1" '費目・口座単位
                    Index = GetIndex("KESSAI_KBN_T")
                    If GCom.NzDec(sTORI(Index, 4), "") <> "01" Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("費目・口座単位を設定した場合は口座入金以外設定できません")
                    End If
                    Index = GetIndex("TESUUTYO_PATN_T")
                    If GCom.NzDec(sTORI(Index, 4), "") <> "1" Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception("費目・口座単位を設定した場合は直接入金以外設定できません")
                    End If
            End Select

            Select Case GCom.NzDec(sTORI(GetIndex("TESUUTYO_KBN_T"), 4), "")
                Case "0"
                    '都度徴求
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "00"
                            '預け金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "0"
                                    '差引入金
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "99"
                                            '決済科目が諸勘定
                                        Case Else
                                            '諸勘定以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "諸勘定"))
                                    End Select

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning

                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '直接等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "都度徴求", "預け金", "差引入金"))
                            End Select
                        Case "01"
                            '口座入金
                            Index = GetIndex("TUKEKAMOKU_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "02", "01", "04"
                                    '決済科目が普通,当座,別段
                                Case Else
                                    '上記以外はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0310W, "普通,当座,別段"))
                            End Select

                            Index = GetIndex("SEIKYU_KBN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "0", "1"
                                    '請求引落区分が請求分徴求、振替分徴求
                                Case Else
                                    'それ以外はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                            End Select

                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "0", "1"
                                    '差引入金、直接
                                Case Else
                                    'それ以外はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0281W, "手数料徴求方法"))
                            End Select
                        Case "02"
                            '為替振込
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "0"
                                    '差引入金
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "02", "01"
                                            '決済科目が普通、当座

                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                            '判定位置変更(SELECT文の下へ)
                                            'Index = GetIndex("SEIKYU_KBN_T")
                                            'Select Case GCom.NzDec(sTORI(Index, 4), "")
                                            '    Case "0", "1"
                                            '        '請求引落区分が請求分徴求、振替分徴求
                                            '    Case Else
                                            '        'それ以外はエラー
                                            '        MsgIcon = MessageBoxIcon.Warning
                                            '        Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                            'End Select
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                                        Case Else
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                            If TUKEKAMOKU_USE_ETC Then
                                                '決済科目に「その他」が存在する場合
                                                If GCom.NzDec(sTORI(Index, 4), "") <> TUKEKAMOKU_ETC_CODE Then
                                                    '上記以外はエラー
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(String.Format(MSG0310W, "普通,当座,その他"))
                                                End If
                                            Else
                                                '上記以外はエラー
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            End If
                                            ''上記以外はエラー
                                            'MsgIcon = MessageBoxIcon.Warning
                                            'Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                                    End Select

                                    '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    '判定位置変更
                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                    '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                                Case Else
                                    '直接等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "都度徴求", "為替振込", "差引入金"))
                            End Select
                        Case "03"
                            '為替付替
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "0"
                                    '差引入金

                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If TUKEKAMOKU_USE_ETC Then
                                        Index = GetIndex("TUKEKAMOKU_T")
                                        '決済科目=「xx:その他」は登録エラーとする
                                        If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0312W, "決済区分が「為替付替」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                        End If
                                    End If
                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '直接等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "都度徴求", "為替付替", "差引入金"))
                            End Select
                        Case "04" '別段出金のみ
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                            If TUKEKAMOKU_USE_ETC Then
                                Index = GetIndex("TUKEKAMOKU_T")
                                '決済科目=「xx:その他」は登録エラーとする
                                If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0312W, "決済区分が「別段出金のみ」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                End If
                            End If
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                        Case "05" '特別企業
                        Case "99" '対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "1"  '一括徴求
                    ' 基準日指定または翌月基準日指定のみ可
                    Index = GetIndex("TESUUTYO_KIJITSU_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "1", "2"
                            '手数料徴求期日区分が基準日指定、翌月基準日指定
                        Case Else
                            'それ以外はエラー
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                    End Select
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "00" '預け金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "99"
                                            '決済科目が諸勘定
                                        Case Else
                                            '諸勘定以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "諸勘定"))
                                    End Select

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning

                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select

                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "一括徴求", "預け金", "直接入金"))
                            End Select
                        Case "01"   '口座入金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "02", "01", "04"
                                            '決済科目が普通,当座,別段

                                            ' 基準日指定または翌月基準日指定のみ可
                                            Index = GetIndex("TESUUTYO_KIJITSU_T")
                                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                                Case "1", "2"
                                                    '手数料徴求期日区分が基準日指定、翌月基準日指定
                                                Case Else
                                                    'それ以外はエラー
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                                            End Select
                                        Case Else
                                            '上記以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "普通,当座,別段"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "一括徴求", "口座入金", "直接入金"))
                            End Select
                        Case "02" '為替振込
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "01", "02"
                                            '決済科目が当座,普通
                                        Case Else
                                            '諸勘定以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "当座,普通"))
                                    End Select

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "一括徴求", "為替振込", "直接入金"))
                            End Select
                        Case "03" '為替付替
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接

                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If TUKEKAMOKU_USE_ETC Then
                                        Index = GetIndex("TUKEKAMOKU_T")
                                        '決済科目=「xx:その他」は登録エラーとする
                                        If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0312W, "決済区分が「為替付替」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                        End If
                                    End If
                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "一括徴求", "為替付替", "直接入金"))
                            End Select
                        Case "04" '別段出金のみ
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                            If TUKEKAMOKU_USE_ETC Then
                                Index = GetIndex("TUKEKAMOKU_T")
                                '決済科目=「xx:その他」は登録エラーとする
                                If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0312W, "決済区分が「別段出金のみ」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                End If
                            End If
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                        Case "05"    '特別企業
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "2" '特別免除
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "00" '預け金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "99"
                                            '決済科目が諸勘定
                                        Case Else
                                            '諸勘定以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "諸勘定"))
                                    End Select
                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "特別免除", "預け金", "直接入金"))
                            End Select
                        Case "01"   '口座入金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "02", "01", "04"
                                            '決済科目が普通,当座,別段
                                            '' 基準日指定または翌月基準日指定のみ可
                                            'Index = GetIndex("TESUUTYO_KIJITSU_T")
                                            'Select Case GCom.NzDec(sTORI(Index, 4), "")
                                            '    Case "1", "2"
                                            '        '手数料徴求期日区分が基準日指定、翌月基準日指定
                                            '    Case Else
                                            '        'それ以外はエラー
                                            '        MsgIcon = MessageBoxIcon.Warning
                                            '        Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                                            'End Select
                                        Case Else
                                            '上記以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "普通,当座,別段"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "特別免除", "口座入金", "直接入金"))
                            End Select
                        Case "02" '為替振込
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "01", "02"
                                            '決済科目が当座,普通
                                        Case Else
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                            If TUKEKAMOKU_USE_ETC Then
                                                '決済科目に「その他」が存在する場合
                                                If GCom.NzDec(sTORI(Index, 4), "") <> TUKEKAMOKU_ETC_CODE Then
                                                    '上記以外はエラー
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(String.Format(MSG0310W, "普通,当座,その他"))
                                                End If
                                            Else
                                                '上記以外はエラー
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            End If
                                            ''上記以外はエラー
                                            'MsgIcon = MessageBoxIcon.Warning
                                            'Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                                    End Select

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "特別免除", "為替振込", "直接入金"))
                            End Select
                        Case "03" '為替付替
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接

                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If TUKEKAMOKU_USE_ETC Then
                                        Index = GetIndex("TUKEKAMOKU_T")
                                        '決済科目=「xx:その他」は登録エラーとする
                                        If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0312W, "決済区分が「為替付替」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                        End If
                                    End If
                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "特別免除", "為替付替", "直接入金"))
                            End Select
                        Case "04" '別段出金のみ
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                            If TUKEKAMOKU_USE_ETC Then
                                Index = GetIndex("TUKEKAMOKU_T")
                                '決済科目=「xx:その他」は登録エラーとする
                                If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0312W, "決済区分が「別段出金のみ」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                End If
                            End If
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                        Case "05"    '特別企業
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "3"    '別途徴求
                    '2010/01/20 追加 =======================
                    ' 基準日指定または翌月基準日指定のみ可
                    Index = GetIndex("TESUUTYO_KIJITSU_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "1", "2"
                            '手数料徴求期日区分が基準日指定、翌月基準日指定
                        Case Else
                            'それ以外はエラー
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                    End Select
                    '=======================================
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "00" '預け金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "99"
                                            '決済科目が諸勘定
                                        Case Else
                                            '諸勘定以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "諸勘定"))
                                    End Select
                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "別途徴求", "預け金", "直接入金"))
                            End Select
                        Case "01"   '口座入金
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "02", "01", "04"
                                            '決済科目が普通,当座,別段
                                            ' 基準日指定または翌月基準日指定のみ可
                                            Index = GetIndex("TESUUTYO_KIJITSU_T")
                                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                                Case "1", "2"
                                                    '手数料徴求期日区分が基準日指定、翌月基準日指定
                                                Case Else
                                                    'それ以外はエラー
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                                            End Select
                                        Case Else
                                            '上記以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0310W, "普通,当座,別段"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "別途徴求", "口座入金", "直接入金"))
                            End Select
                        Case "02" '為替振込
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接
                                    Index = GetIndex("TUKEKAMOKU_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "01", "02"
                                            '決済科目が当座,普通
                                        Case Else
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                            If TUKEKAMOKU_USE_ETC Then
                                                '決済科目に「その他」が存在する場合
                                                If GCom.NzDec(sTORI(Index, 4), "") <> TUKEKAMOKU_ETC_CODE Then
                                                    '上記以外はエラー
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(String.Format(MSG0310W, "普通,当座,その他"))
                                                End If
                                            Else
                                                '上記以外はエラー
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            End If
                                            ''上記以外はエラー
                                            'MsgIcon = MessageBoxIcon.Warning
                                            'Throw New Exception(String.Format(MSG0310W, "普通,当座"))
                                            '2017/05/18 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                                    End Select

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "別途徴求", "為替振込", "直接入金"))
                            End Select
                        Case "03" '為替付替
                            Index = GetIndex("TESUUTYO_PATN_T")
                            Select Case GCom.NzDec(sTORI(Index, 4), "")
                                Case "1"
                                    '直接

                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If TUKEKAMOKU_USE_ETC Then
                                        Index = GetIndex("TUKEKAMOKU_T")
                                        '決済科目=「xx:その他」は登録エラーとする
                                        If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0312W, "決済区分が「為替付替」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                        End If
                                    End If
                                    '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                    Index = GetIndex("SEIKYU_KBN_T")
                                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                                        Case "0", "1"
                                            '請求引落区分が請求分徴求、振替分徴求
                                        Case Else
                                            'それ以外はエラー
                                            MsgIcon = MessageBoxIcon.Warning
                                            Throw New Exception(String.Format(MSG0281W, "請求引落区分"))
                                    End Select
                                Case Else
                                    '差引入金等はエラー
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0350W, "別途徴求", "為替付替", "直接入金"))
                            End Select
                        Case "04" '別段出金のみ
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                            If TUKEKAMOKU_USE_ETC Then
                                Index = GetIndex("TUKEKAMOKU_T")
                                '決済科目=「xx:その他」は登録エラーとする
                                If GCom.NzDec(sTORI(Index, 4), "") = TUKEKAMOKU_ETC_CODE Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0312W, "決済区分が「別段出金のみ」の場合に、" & vbCrLf & "決済科目に「その他」"))
                                End If
                            End If
                            '2017/05/18 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                        Case "05"    '特別企業
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
            End Select

            '2017/05/12 タスク）西野 ADD 標準版修正（決済区分と振込手数料基準IDの相関チェック追加）------- START
            '決済区分<>為替振込かつ決済区分<>為替付替の場合で、
            '振込手数料が発生する振込手数料基準IDが選択された場合エラーとする()
            Index = GetIndex("KESSAI_KBN_T")
            Select Case GCom.NzDec(sTORI(Index, 4), "")
                Case "02", "03"
                Case Else
                    If GCom.NzInt(TESUU_A1_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_A2_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_A3_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_B1_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_B2_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_B3_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_C1_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_C2_T.Text) <> 0 OrElse _
                       GCom.NzInt(TESUU_C3_T.Text) <> 0 Then

                        Index = GetIndex("TESUU_TABLE_ID_T")
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0380W, GetComboBoxValue(2, GetIndex("KESSAI_KBN_T"), 1)))

                    End If
            End Select
            '2017/05/12 タスク）西野 ADD 標準版修正（決済区分と振込手数料基準IDの相関チェック追加）------- END

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 備考項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : @MM-DD@ , @TEN@ , @KEN@
    '    
    Private Function CheckMutualRelation_012(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Counter As Integer
            Dim Temp() As String = {"@MM-DD@", "@TEN@", "@KEN@"}

            '備考１
            Index = GetIndex("DENPYO_BIKOU1_T")
            Counter = 0
            For Cnt As Integer = 0 To 2 Step 1

                If sTORI(Index, 4).IndexOf(Temp(Cnt)) >= 0 Then
                    Counter += 1
                    If Counter > 1 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(MSG0313W)
                    End If
                End If
            Next Cnt

            '備考２
            Index = GetIndex("DENPYO_BIKOU2_T")
            Counter = 0
            For Cnt As Integer = 0 To 2 Step 1

                If sTORI(Index, 4).IndexOf(Temp(Cnt)) >= 0 Then
                    Counter += 1
                    If Counter > 1 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(MSG0313W)
                    End If
                End If
            Next Cnt

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

#End Region

#Region " 帳票印刷"
    Public Function fn_CreateCSV_INSERT() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV_INSERT
        'Parameter      :
        'Description    :KFGP035(学校マスタメンテ(登録))印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2016/09/01
        'Update         :
        '============================================================================
        Dim i As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            '------------------------------------------------
            'ＣＳＶファイル作成
            '------------------------------------------------
            Dim CreateCSV As New KFGP035
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")

            Dim Kbn As Integer
            If SyoriKBN = 0 Then
                Kbn = 1
            Else
                Kbn = 0
            End If

            Dim SQL As New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM ")
            SQL.Append("     GAKMAST_VIEW")
            SQL.Append("   , GAKMAST1")
            SQL.Append(" WHERE ")
            SQL.Append("     GAKKOU_CODE_T = " & SQ(GAKKOU_CODE_T.Text))
            SQL.Append(" AND GAKKOU_CODE_T = GAKKOU_CODE_G")
            SQL.Append(" ORDER BY ")
            SQL.Append("     GAKUNEN_CODE_G ")

            Dim KINKO As String
            Dim SITEN As String
            Dim FURI_DATE As String
            Dim TUKI As String
            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
                strCSV_FILE_NAME = CreateCSV.CreateCsvFile()
                Do
                    With CreateCSV
                        'GCOM.NzStrはNULL値対策
                        'コンボボックスの内容は、画面の更新後項目を取得する
                        FURI_DATE = ""
                        TUKI = ""
                        .OutputCsvData(Today, True)                                                     'システム日付
                        .OutputCsvData(NowTime, True)                                                   'タイムスタンプ
                        .OutputCsvData(GCom.GetUserID, True)                                            'ログイン名
                        .OutputCsvData(Environment.MachineName, True)                                   '端末名
                        .OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_CODE_G")), True)                     '学校コード
                        .OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_KNAME_G")), True)                    '学校名カナ
                        .OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_NNAME_G")), True)                    '学校名漢字
                        .OutputCsvData(GCom.NzStr(REC.Item("SIYOU_GAKUNEN_T")), True)                   '使用学年
                        .OutputCsvData(GCom.NzStr(REC.Item("SAIKOU_GAKUNEN_T")), True)                  '最高学年
                        .OutputCsvData(GCom.NzStr(REC.Item("SINKYU_NENDO_T")), True)                    '最終進級処理年
                        .OutputCsvData(GCom.NzStr(REC.Item("GAKUNEN_CODE_G")), True)                    '学年
                        .OutputCsvData(GCom.NzStr(REC.Item("GAKUNEN_NAME_G")), True)                    '学年名
                        For No As Integer = 1 To 20                                                     'クラス1～20
                            If GCom.NzStr(REC.Item("CLASS_NAME1" & No.ToString("00") & "_G")).Trim = "" Then
                                .OutputCsvData("")
                            Else
                                .OutputCsvData(GCom.NzStr(REC.Item("CLASS_CODE1" & No.ToString("00") & "_G")), True)
                            End If
                        Next
                        For No As Integer = 1 To 20                                                     'クラス名1～20
                            .OutputCsvData(GCom.NzStr(REC.Item("CLASS_NAME1" & No.ToString("00") & "_G")), True)
                        Next
                        .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_CODE_T")), True)                      '委託者コード
                        KINKO = GCom.NzStr(REC.Item("TKIN_NO_T"))
                        .OutputCsvData(KINKO, True)                                                     '取扱金融機関コード
                        .OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                           '取扱金融機関名
                        SITEN = GCom.NzStr(REC.Item("TSIT_NO_T"))
                        .OutputCsvData(SITEN, True)                                                     '取扱支店コード
                        .OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                        '取扱支店名
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("KAMOKU_T"), Kbn), True)            '科目
                        .OutputCsvData(GCom.NzStr(REC.Item("KOUZA_T")), True)                           '口座番号
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TAKO_KBN_T"), Kbn), True)          '他行区分
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("JIFURICHK_KBN_T"), Kbn), True)     '自振契約作成区分
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TEKIYOU_KBN_T"), Kbn), True)       '摘要区分
                        .OutputCsvData(GCom.NzStr(REC.Item("KTEKIYOU_T")), True)                        'カナ摘要
                        .OutputCsvData(GCom.NzStr(REC.Item("NTEKIYOU_T")), True)                        '漢字摘要
                        .OutputCsvData(GCom.NzStr(REC.Item("FURI_CODE_T")), True)                       '振替コード
                        .OutputCsvData(GCom.NzStr(REC.Item("KIGYO_CODE_T")), True)                      '企業コード
                        .OutputCsvData(GCom.NzStr(REC.Item("YUUBIN_T")), True)                          '郵便番号
                        .OutputCsvData(GCom.NzStr(REC.Item("DENWA_T")), True)                           '電話番号
                        .OutputCsvData(GCom.NzStr(REC.Item("FAX_T")), True)                             'FAX番号
                        .OutputCsvData(GCom.NzStr(REC.Item("KOKYAKU_NO_T")), True)                      '顧客番号
                        .OutputCsvData(GCom.NzStr(REC.Item("KANREN_KIGYO_CODE_T")), True)               '関連企業情報
                        .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_NJYU_T")), True)                      '委託者住所漢字
                        .OutputCsvData(GCom.NzStr(REC.Item("FURI_DATE_T")), True)                       '振替日
                        .OutputCsvData(GCom.NzStr(REC.Item("SFURI_DATE_T")), True)                      '再振日
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), Kbn), True)       '媒体コード
                        .OutputCsvData(GCom.NzStr(REC.Item("FILE_NAME_T")), True)                       'ファイル名
                        Select Case GCom.NzInt(REC.Item("SFURI_SYUBETU_T"))
                            Case 0
                                .OutputCsvData("なし", True)    '再振種別
                                .OutputCsvData("なし", True)    '持越種別
                            Case 1
                                .OutputCsvData("あり", True)    '再振種別
                                .OutputCsvData("なし", True)    '持越種別
                            Case 2
                                .OutputCsvData("あり", True)    '再振種別
                                .OutputCsvData("あり", True)    '持越種別
                            Case 3
                                .OutputCsvData("なし", True)    '再振種別
                                .OutputCsvData("あり", True)    '持越種別
                        End Select
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("NKYU_CODE_T"), Kbn), True)         '出金休日シフト
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("SKYU_CODE_T"), Kbn), True)         '入金休日シフト
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("SFURI_KYU_CODE_T"), Kbn), True)    '再振休日シフト
                        .OutputCsvData(GCom.NzStr(REC.Item("MOTIKOMI_KIJITSU_T")), True)                '持込期日
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("JIFURI_KBN_T"), Kbn), True)        '自振区分
                        .OutputCsvData(GCom.NzStr(REC.Item("FKEKKA_TBL_T")), True)                      '振替結果変換テーブルID
                        If GCom.NzStr(REC.Item("KEIYAKU_DATE_T")) = "00000000" Then                     '契約日
                            .OutputCsvData("", True)
                        Else
                            .OutputCsvData(GCom.NzStr(REC.Item("KEIYAKU_DATE_T")), True)
                        End If
                        .OutputCsvData(GCom.NzStr(REC.Item("KAISI_DATE_T")), True)                      '開始年月
                        .OutputCsvData(GCom.NzStr(REC.Item("SYURYOU_DATE_T")), True)                    '終了年月
                        If GCom.NzStr(REC.Item("MEISAI_FUNOU_T")) = "1" Then                            '口座振替結果一覧表
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        If GCom.NzStr(REC.Item("MEISAI_KEKKA_T")) = "1" Then                            '口座振替不能明細一覧表
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        If GCom.NzStr(REC.Item("MEISAI_HOUKOKU_T")) = "1" Then                          '収納報告書
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        If GCom.NzStr(REC.Item("MEISAI_TENBETU_T")) = "1" Then                          '口座振替店別集計表
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        If GCom.NzStr(REC.Item("MEISAI_MINOU_T")) = "1" Then                            '口座振替未納のお知らせ
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        If GCom.NzStr(REC.Item("MEISAI_YOUKYU_T")) = "1" Then                           '要求性入金伝票
                            .OutputCsvData("出力対象", True)
                        Else
                            .OutputCsvData("出力なし", True)
                        End If
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("MEISAI_KBN_T"), Kbn), True)        '振替予定明細表作成区分
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("MEISAI_OUT_T"), Kbn), True)        '帳票ソート順指定
                        .OutputCsvData(GCom.NzStr(REC.Item("SAKUSEI_DATE_T")), True)                    '作成日
                        .OutputCsvData(GCom.NzStr(REC.Item("KOUSIN_DATE_T")), True)                     '更新日
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KBN_T"), Kbn), True)        '決済区分
                        SITEN = GCom.NzStr(REC.Item("TORIMATOME_SIT_T"))
                        .OutputCsvData(SITEN, True)                                                     'とりまとめ店コード
                        .OutputCsvData(GCom.GetBKBRName(SELF_BANK_CODE, SITEN, 30), True)               'とりまとめ店名
                        .OutputCsvData(GCom.NzStr(REC.Item("HONBU_KOUZA_T")), True)                     '本部別段口座番号
                        .OutputCsvData(GCom.NzStr(REC.Item("KESSAI_DAY_T")), True)                      '日数/基準日(決済)
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KIJITSU_T"), Kbn), True)    '日付区分(決済)
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KYU_CODE_T"), Kbn), True)   '決済休日シフト
                        KINKO = GCom.NzStr(REC.Item("TUKEKIN_NO_T"))
                        .OutputCsvData(KINKO, True)                                                     '決済金融機関コード
                        .OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                           '決済金融機関名
                        SITEN = GCom.NzStr(REC.Item("TUKESIT_NO_T"))
                        .OutputCsvData(SITEN, True)                                                     '決済支店コード
                        .OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                        '決済支店名
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TUKEKAMOKU_T"), Kbn), True)        '決済科目
                        .OutputCsvData(GCom.NzStr(REC.Item("TUKEKOUZA_T")), True)                       '決済口座番号
                        .OutputCsvData(GCom.NzStr(REC.Item("TUKEMEIGI_T")), True)                       '決済名義人(カナ)
                        .OutputCsvData(GCom.NzStr(REC.Item("DENPYO_BIKOU1_T")), True)                   '伝票備考1
                        .OutputCsvData(GCom.NzStr(REC.Item("DENPYO_BIKOU2_T")), True)                   '伝票備考2
                        SITEN = GCom.NzStr(REC.Item("TESUUTYO_SIT_T"))
                        .OutputCsvData(SITEN, True)                                                     '手数料徴求支店コード
                        .OutputCsvData(GCom.GetBKBRName(SELF_BANK_CODE, SITEN, 30), True)               '手数料徴求支店名
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KAMOKU_T"), Kbn), True)   '手数料徴求科目
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUUTYO_KOUZA_T")), True)                  '手数料徴求口座番号
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KBN_T"), Kbn), True)      '手数料徴求区分
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_PATN_T"), Kbn), True)     '手数料徴求方法
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_NO_T")).Trim, True)                '手数料集計周期
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUUTYO_DAY_T")), True)                    '日数/基準日(手数料徴求)
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KIJITSU_T"), Kbn), True)  '日付区分(手数料徴求)
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUU_KYU_CODE_T"), Kbn), True)    '手数料徴求休日シフト
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("SEIKYU_KBN_T"), Kbn), True)        '手数料請求区分
                        .OutputCsvData(GCom.NzStr(REC.Item("KIHTESUU_T")), True)                        '振替手数料単価
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("SYOUHI_KBN_T"), Kbn), True)        '消費税区分
                        .OutputCsvData(GCom.NzStr(REC.Item("SOURYO_T")), True)                          '送料
                        .OutputCsvData(GCom.NzStr(REC.Item("KOTEI_TESUU1_T")), True)                    '固定手数料1
                        .OutputCsvData(GCom.NzStr(REC.Item("KOTEI_TESUU2_T")), True)                    '固定手数料2
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_MONTH_T")), True)                  '集計基準月
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_ENDDAY_T")), True)                 '集計終了日
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUMAT_KIJYUN_T"), Kbn), True)   '集計基準
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUMAT_PATN_T"), Kbn), True)     '集計方法
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_GRP_T")), True)                       '集計企業GRP
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUU_TABLE_ID_T"), Kbn), True)    '振込手数料基準ID
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A1_T")), True)                        '自店1万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A2_T")), True)                        '自店1万円以上3万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A3_T")), True)                        '自店3万円以上
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B1_T")), True)                        '本支店1万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B2_T")), True)                        '本支店1万円以上3万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B3_T")), True)                        '本支店3万円以上
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C1_T")), True)                        '他行1万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C2_T")), True)                        '他行1万円以上3万円未満
                        .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C3_T")), True)                        '他行3万円以上
                        .OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                '印紙税1
                        .OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                '印紙税2
                        .OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                '印紙税3
                        .OutputCsvData(GCom.NzStr(REC.Item("KEIYAKU_NO_T")), True)                      '契約書番号
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_SYUBETU_T"), Kbn), True)    '決済種別
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("HANSOU_KBN_T"), 1), True)          '搬送方法
                        .OutputCsvData(GCom.NzStr(REC.Item("HANSOU_ROOT1_T")), True)                    '搬送ルート１
                        .OutputCsvData(GCom.NzStr(REC.Item("HANSOU_ROOT2_T")), True)                    '搬送ルート２
                        .OutputCsvData(GCom.NzStr(REC.Item("HANSOU_ROOT3_T")), True)                    '搬送ルート３
                        SITEN = GCom.NzStr(REC.Item("HENKYAKU_SIT_NO_T"))
                        .OutputCsvData(SITEN, True)                                                     '返却支店コード
                        .OutputCsvData(GCom.GetBKBRName(SELF_BANK_CODE, SITEN, 30), True)               '返却支店名
                        .OutputCsvData(GetComboBoxValue(2, GetIndex("SYOUGOU_KBN_T"), 1), True)         '照合要否区分
                        .OutputCsvData("", True, True)
                    End With
                Loop Until REC.Read = False

                CreateCSV.CloseCsv()
            Else
                MessageBox.Show(String.Format(MSG0231W, "取引先マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", "登録なし")
                Return False
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(String.Format(MSG0231W, "取引先マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Me.ResumeLayout()
        End Try

    End Function
    Public Function fn_CreateCSV_Delete() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV_Delete
        'Parameter      :
        'Description    :KFGP008(学校マスタメンテ(削除))印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2010/03/16
        'Update         :
        '============================================================================
        Dim i As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            '------------------------------------------------
            'ＣＳＶファイル作成
            '------------------------------------------------
            Dim CreateCSV As New KFGP008
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim SQL As New StringBuilder(128)
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

            SQL.Append("SELECT *")
            SQL.Append(" FROM GAKMAST2,GAKMAST1,TENMAST")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(CType(oTORI(GetIndex("GAKKOU_CODE_T")), TextBox).Text.Trim))
            SQL.Append(" AND GAKKOU_CODE_T = GAKKOU_CODE_G(+)")
            SQL.Append(" AND GAKUNEN_CODE_G = 1")
            SQL.Append(" AND TKIN_NO_T = KIN_NO_N(+)")
            SQL.Append(" AND TSIT_NO_T = SIT_NO_N(+)")
            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
                CreateCSV.OutputCsvData(Today, True)                                              'システム日付
                CreateCSV.OutputCsvData(NowTime, True)                                            'タイムスタンプ
                CreateCSV.OutputCsvData(GCom.GetUserID, True)                                     'ログイン名
                CreateCSV.OutputCsvData(Environment.MachineName, True)                            '端末名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_CODE_T")), True)              '学校コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("ITAKU_CODE_T")), True)               '委託者コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_NNAME_G")), True)             '学校名漢字
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("GAKKOU_KNAME_G")), True)            '学校名カナ
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TKIN_NO_T")), True)                  '取扱金融機関コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("KIN_NNAME_N")), True)                '取扱金融機関名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TSIT_NO_T")), True)                  '取扱支店コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("SIT_NNAME_N")), True)                '取扱支店名
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("KAMOKU_T"), 0), True)       '科目
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("KOUZA_T")), True)                    '口座番号
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 0), True)  '媒体コード
                Select Case GCom.NzInt(REC.Item("SFURI_SYUBETU_T"))
                    Case 0
                        CreateCSV.OutputCsvData("なし", True)    '再振種別
                        CreateCSV.OutputCsvData("なし", True)    '持越種別
                    Case 1
                        CreateCSV.OutputCsvData("あり", True)    '再振種別
                        CreateCSV.OutputCsvData("なし", True)    '持越種別
                    Case 2
                        CreateCSV.OutputCsvData("あり", True)    '再振種別
                        CreateCSV.OutputCsvData("あり", True)    '持越種別
                    Case 3
                        CreateCSV.OutputCsvData("なし", True)    '再振種別
                        CreateCSV.OutputCsvData("あり", True)    '持越種別
                End Select
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("TAKO_KBN_T"), 0), True)     '他行区分
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("FURI_DATE_T")), True)                '振替日
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("SFURI_DATE_T")), True)                 '再振日
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("FURI_CODE_T")), True)                  '振替コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("KIGYO_CODE_T")), True, True)           '企業コード
            Else
                Return False
            End If
            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

    End Function
    Public Function fn_Print(ByVal PrintNo As Integer) As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV
        'Parameter      :PrintNo 1:登録 2:更新 3:削除
        'Description    :帳票印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2010/03/16
        'Update         :
        '============================================================================
        Dim ErrMessage As String = ""
        Dim Param As String = ""
        Dim nRet As Integer = 0
        Dim Syori As String = ""

        Select Case PrintNo
            Case 1
                Syori = "登録"
            Case 2
                Syori = "更新"
            Case 3
                Syori = "削除"
        End Select
        Try
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            Param = GCom.GetUserID & "," & strCSV_FILE_NAME & "," & _
                    LW.ToriCode
            Select Case PrintNo
                Case 1
                    nRet = ExeRepo.ExecReport("KFGP035.EXE", Param)
                Case 2
                    nRet = ExeRepo.ExecReport("KFGP007.EXE", Param)
                Case 3
                    nRet = ExeRepo.ExecReport("KFGP008.EXE", Param)
            End Select

            If nRet <> 0 Then
                '印刷失敗：戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case -1
                        ErrMessage = String.Format(MSG0106W, "学校マスタメンテ(" & Syori & ")")
                    Case Else
                        ErrMessage = String.Format(MSG0004E, "学校マスタメンテ(" & Syori & ")")
                End Select

                MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            MessageBox.Show(String.Format(MSG0014I, "学校マスタメンテ(" & Syori & ")"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            ErrMessage = String.Format(MSG0004E, "学校マスタメンテ(" & Syori & ")")
            MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校マスタメンテ(" & Syori & ")印刷", "失敗")
            Return False
        End Try

    End Function
#End Region

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
    DataGridView1.RowPostPaint, DataGridView2.RowPostPaint, DataGridView3.RowPostPaint, _
    DataGridView4.RowPostPaint, DataGridView5.RowPostPaint, DataGridView6.RowPostPaint, _
    DataGridView7.RowPostPaint, DataGridView8.RowPostPaint, DataGridView9.RowPostPaint

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub

End Class