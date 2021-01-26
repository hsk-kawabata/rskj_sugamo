Option Explicit Off
Option Strict Off

Public Class KFGOTHR020

#Region " 共通変数定義 "
    'Dim STR企業自振連携_INI As String
    Dim LNG読込件数 As Long
    Dim STR学校コード(9) As String
    Dim STR入学年度(9) As String
    Dim INT通番(9) As Integer
    Dim STR生徒氏名カナ(9) As String
    Dim STR生徒氏名漢字(9) As String
    Dim STR処理名 As String

    '2005/05/11
    Dim strSEITO_NENDO As String
    Dim strSEITO_TUUBAN As String
    '2006/10/13
    Public strTAKOU_FLG As String = "0"
    Public update_GAKUNEN As String = "0"

    Public UPD_SCH_KBN As String
    Public UPD_FURI_KBN As String
    Public UPD_FURI_DATE As String
    Public UPD_SFURI_DATE As String

    '2007/09/27
    Dim strJUYOUKA_NO As String

#End Region

#Region " Form Load "
    Private Sub KFGOTHR020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = False
        End With

        STR処理名 = "振替結果変更"

        STR_SYORI_NAME = "振替結果変更"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        'テキストファイルからコンボボックス設定
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbFURIKUBUN)")
            Exit Sub
        End If

        'テキストファイルからコンボボックス設定
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbKamoku)")
            Exit Sub
        End If

        'テキストファイルからコンボボックス設定
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbFuriketu)")
            Exit Sub
        End If

        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        '学校指定の場合
        If rdbgakkouSeq.Checked = True Then
            '金融機関コードから振替結果コード（グループ１）の表示抑止
            GroupBox1.Visible = False
            '学年コード
            txtGAKUNEN.Visible = True
            '"-"
            lbl1.Visible = True
            'クラスコード
            txtCLASS.Visible = True
            '"-"
            lbl2.Visible = True
            '生徒番号
            txtSEITONO.Visible = True

        End If

        '入力ボタン制御
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD.Open()

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '更新ボタン
        STR_COMMAND = "更新"
        STR_LOG_GAKKOU_CODE = txtGAKKOU_CODE.Text
        STR_LOG_FURI_DATE = txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text

        '振替結果コンボ選択チェック
        If cmbFuriketu.SelectedIndex < 0 Then
            Call GSUB_MESSAGE_WARNING("振替結果が選択されていません")
            '振替結果コンボにカーソル設定
            cmbFuriketu.Focus()

            Exit Sub
        End If

        If GFUNC_MESSAGE_QUESTION("更新しますか？") <> vbOK Then
            Exit Sub
        End If

        '蒲郡信金向け 蒲郡信金は他行も扱う 2007/09/27
        ''他行作成対象先フラグ取得 2006/10/13
        'If PFUC_TAKOUFLG_GET() = 1 Then
        '    Call GSUB_MESSAGE_WARNING( "学校検索に失敗しました")
        '    txtGAKKOU_CODE.Focus()
        '    txtGAKKOU_CODE.SelectionStart = 0
        '    txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
        '    Exit Sub
        'End If

        'トランザクション開始
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If

        ''口座番号指定の場合
        'If rdbKouza.Checked = True Then
        STR_SQL = ""
        STR_SQL += "UPDATE  KZFMAST.G_MEIMAST SET "
        '振替結果コード
        STR_SQL += " FURIKETU_CODE_M = " & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu)
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M = '" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M = '" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_M = '" & txtSitCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TKAMOKU_M = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKOUZA_M = '" & txtKouzaBan.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " RECORD_NO_M = " & CLng(txtRECNO.Text)
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M = " & CLng(txtKingaku.Text)

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '更新処理エラー
            GSUB_MESSAGE_WARNING("明細マスタの更新でエラーが発生しました。")
            'トラキャン
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)

            Call GSUB_LOG(0, "明細マスタの更新")

            Exit Sub
        End If

        '企業自振明細マスタ更新
        If PFUNC_KMEISAI_UPD() = False Then
            'トラキャン
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Call GSUB_LOG(0, "企業自振明細マスタ更新")

            Exit Sub
        End If

        'トランザクション終了（ＣＯＭＭＩＴ）
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Sub
        End If

        '------------------------------------------------------------
        'スケジュールの振替件数・金額、不能件数・金額の更新(2005/05/11)
        '------------------------------------------------------------
        'トランザクション開始
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If
        If PFUNC_GSCHAMST_UPD() = False Then
            'トラキャン
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Exit Sub
        End If
        'トランザクション終了（ＣＯＭＭＩＴ）
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Sub
        End If

        '蒲郡信金向け 他行SCHMASTは企業側で扱う 2007/09/27
        'If strTAKOU_FLG = "1" Then '他行作成対象先のみ
        '    '------------------------------------------------------------
        '    '他行スケジュールの振替件数・金額、不能件数・金額の更新(2006/10/13)
        '    '------------------------------------------------------------
        '    'トランザクション開始
        '    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
        '        Exit Sub
        '    End If
        '    If PFUNC_GSCHAMST_UPD_TAKOU() = False Then
        '        'トラキャン
        '        GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
        '        Exit Sub
        '    End If
        '    'トランザクション終了（ＣＯＭＭＩＴ）
        '    If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
        '        Exit Sub
        '    End If
        'End If

        '--------------------------------------------------------------

        Call GSUB_LOG(1, "振替結果更新")

        Call GSUB_MESSAGE_INFOMATION("更新が終了しました")

        '入力項目　抑止の解除
        PSUB_INPUTEnabled_True()

        txtGAKKOU_CODE.Focus()

        '入力ボタン制御
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnSansyou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyou.Click
        '参照ボタン

        '入力項目の共通チェック
        If PFUNC_COMMON_CHECK() = False Then
            Exit Sub
        End If

        'スケジュールマスタ検索
        If PFUNC_SCHMAST_GET() = False Then
            '学校コードにカーソル設定
            txtGAKKOU_CODE.Focus()
            Exit Sub
        End If

        '口座振替データ検索
        If PFUNC_MEISAI_GET() = False Then
            '学校コードにカーソル設定
            txtGAKKOU_CODE.Focus()
            Exit Sub
        End If

        'グループ領域の表示
        GroupBox1.Visible = True

        '入力項目制御
        PSUB_INPUTEnabled_False()
        cmbFuriketu.Focus()

        '入力ボタン制御
        btnAction.Enabled = True
        btnSansyou.Enabled = False
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        '取消ボタン

        cmbKana.SelectedIndex = -1
        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        '入力項目　抑止の解除
        PSUB_INPUTEnabled_True()

        '2006/10/16　追加：振替日・請求対象月をクリア
        txtFURINEN.Text = ""
        txtFURITUKI.Text = ""
        txtFURIHI.Text = ""
        txtSEIKYUNEN.Text = ""
        txtSEIKYUTUKI.Text = ""
        cmbFURIKUBUN.SelectedIndex = 0


        txtGAKUNEN.Text = ""
        txtCLASS.Text = ""
        txtSEITONO.Text = ""
        txtKinyuuCode.Text = ""
        txtSitCode.Text = ""

        cmbKamoku.SelectedIndex = 0

        txtKouzaBan.Text = ""
        txtKingaku.Text = ""
        lab請求月.Text = ""
        txtKeiyaku.Text = ""
        txtRECNO.Text = ""
        cmbFuriketu.SelectedIndex = 0

        '入力項目制御
        rdbgakkouSeq.Checked = True
        GroupBox1.Visible = False

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()
        txtGAKKOU_CODE.SelectionStart = 0
        txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        '入力ボタン制御
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        If OBJ_CONNECTION_DREAD Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
        End If

        '終了ボタン
        Me.Close()
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_INPUTEnabled_False()
        '入力項目の抑止

        cmbKana.Enabled = False
        cmbGakkouName.Enabled = False
        txtGAKKOU_CODE.Enabled = False
        txtFURINEN.Enabled = False
        txtFURITUKI.Enabled = False
        txtFURIHI.Enabled = False
        txtSEIKYUNEN.Enabled = False
        txtSEIKYUTUKI.Enabled = False
        cmbFURIKUBUN.Enabled = False
        txtGAKUNEN.Enabled = False
        txtCLASS.Enabled = False
        txtSEITONO.Enabled = False
        txtKinyuuCode.Enabled = False
        txtKinyuuCode.Enabled = False
        txtSitCode.Enabled = False
        cmbKamoku.Enabled = False
        txtKouzaBan.Enabled = False
        txtKingaku.Enabled = False
        txtKeiyaku.Enabled = False
        txtRECNO.Enabled = False
    End Sub
    Private Sub PSUB_INPUTEnabled_True()
        '入力項目の抑止

        cmbKana.Enabled = True
        cmbGakkouName.Enabled = True
        txtGAKKOU_CODE.Enabled = True
        txtFURINEN.Enabled = True
        txtFURITUKI.Enabled = True
        txtFURIHI.Enabled = True
        txtSEIKYUNEN.Enabled = True
        txtSEIKYUTUKI.Enabled = True
        cmbFURIKUBUN.Enabled = True
        txtGAKUNEN.Enabled = True
        txtCLASS.Enabled = True
        txtSEITONO.Enabled = True
        txtKinyuuCode.Enabled = True
        txtKinyuuCode.Enabled = True
        txtSitCode.Enabled = True
        cmbKamoku.Enabled = True
        txtKouzaBan.Enabled = True
        txtKingaku.Enabled = True
        txtKeiyaku.Enabled = True
        txtRECNO.Enabled = True
    End Sub
    Private Sub PSUB_Kanma_Set(ByVal pText As TextBox, ByVal pIndex As Integer)

        Select Case (pIndex)
            Case 0
                If pText.MaxLength = 12 Then
                    pText.MaxLength = pText.MaxLength + 3
                End If
                pText.Text = Format(CInt(pText.Text), "#,##0")
            Case 1
                If pText.MaxLength = 15 Then
                    pText.MaxLength = pText.MaxLength - 3
                End If
                pText.Text = Format(CInt(pText.Text), "###0")
        End Select
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "

    Private Function PFUC_TAKOUFLG_GET() As Integer
        PFUC_TAKOUFLG_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST2 WHERE"
        STR_SQL += "     GAKKOU_CODE_T  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '学校マスタ2他行フラグチェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            GSUB_MESSAGE_WARNING("学校マスタに登録されていません")
            txtGAKKOU_CODE.Focus()
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                '他行区分
                strTAKOU_FLG = .Item("TAKO_KBN_T")
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        PFUC_TAKOUFLG_GET = 0

    End Function

    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False

        '学校名の設定
        STR_SQL = " SELECT * FROM GAKMAST1"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'"

        '学校マスタ１存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("学校マスタに登録されていません")

            lab学校名.Text = ""
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        With OBJ_DATAREADER
            .Read()

            lab学校名.Text = .Item("GAKKOU_NNAME_G")
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_COMMON_CHECK() As Boolean

        PFUNC_COMMON_CHECK = False

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("学校コードの入力に誤りがあります")
            txtGAKKOU_CODE.Focus()
            Exit Function
        End If
        '振替年
        If Trim(txtFURINEN.Text) < "2004" Then
            Call GSUB_MESSAGE_WARNING("振替年の入力に誤りがあります")
            txtFURINEN.Focus()
            Exit Function
        End If
        '振替月
        If Trim(txtFURITUKI.Text) >= "01" And Trim(txtFURITUKI.Text) <= "12" Then
        Else
            Call GSUB_MESSAGE_WARNING("振替月の入力に誤りがあります")
            txtFURITUKI.Focus()
            Exit Function
        End If

        '振替日
        '月日関連チェック
        Select Case (Trim(txtFURITUKI.Text))
            Case "01", "03", "05", "07", "08", "10", "12"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "31" Then
                Else
                    Call GSUB_MESSAGE_WARNING("振替日の入力に誤りがあります")
                    txtFURIHI.Focus()
                    Exit Function
                End If
            Case "04", "06", "09", "11"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "30" Then
                Else
                    Call GSUB_MESSAGE_WARNING("振替日の入力に誤りがあります")
                    txtFURIHI.Focus()
                    Exit Function
                End If
            Case "02"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "29" Then
                Else
                    Call GSUB_MESSAGE_WARNING("振替日の入力に誤りがあります")
                    txtFURIHI.Focus()
                    Exit Function
                End If
        End Select

        '請求年
        If Trim(txtSEIKYUNEN.Text) < "2004" Then
            Call GSUB_MESSAGE_WARNING("請求年の入力に誤りがあります")
            txtSEIKYUNEN.Focus()
            Exit Function
        End If

        '請求月
        If Trim(txtSEIKYUTUKI.Text) >= "01" And Trim(txtSEIKYUTUKI.Text) <= "12" Then
        Else
            Call GSUB_MESSAGE_WARNING("振替月の入力に誤りがあります")
            txtSEIKYUTUKI.Focus()
            Exit Function
        End If

        '振替区分コンボ
        If cmbFURIKUBUN.SelectedIndex < 0 Then
            Call GSUB_MESSAGE_WARNING("振替区分が選択されていません")
            cmbFURIKUBUN.Focus()
            Exit Function
        End If

        '学校指定の場合
        If rdbgakkouSeq.Checked = True Then
            '学年
            If Trim(txtGAKUNEN.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("学年の入力に誤りがあります")
                txtGAKUNEN.Focus()
                Exit Function
            End If
            'クラス
            If Trim(txtCLASS.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("クラスの入力に誤りがあります")
                txtCLASS.Focus()
                Exit Function
            End If
            '生徒番号
            If Trim(txtSEITONO.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("生徒番号の入力に誤りがあります")
                txtSEITONO.Focus()
                Exit Function
            End If
        End If

        '口座番号指定の場合
        If rdbKouza.Checked = True Then
            '金融機関コード
            If Trim(txtKinyuuCode.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("金融機関コードの入力に誤りがあります")
                txtKinyuuCode.Focus()
                Exit Function
            End If
            '支店コード
            If Trim(txtSitCode.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("支店コードの入力に誤りがあります")
                txtSitCode.Focus()
                Exit Function
            End If
            '科目コンボ
            If cmbKamoku.SelectedIndex < 0 Then
                Call GSUB_MESSAGE_WARNING("科目が選択されていません")
                cmbKamoku.Focus()
                Exit Function
            End If
            '口座番号
            If Trim(txtKouzaBan.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("口座番号の入力に誤りがあります")
                txtKouzaBan.Focus()
                Exit Function
            End If
            '金額
            If Trim(txtKingaku.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("金額の入力に誤りがあります")
                txtKingaku.Focus()
                Exit Function
            End If
        End If

        PFUNC_COMMON_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean
        'スケジュールから対象データ検索する

        PFUNC_SCHMAST_GET = True

        '検索キーは、学校コード、請求年月、振替区分、振替日

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " NENGETUDO_S ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"

        'スケジュールマスタ存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("スケジュールが存在しません")
            PFUNC_SCHMAST_GET = False
            Exit Function
        End If

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                '不能結果更新フラグ
                If .Item("FUNOU_FLG_S") = "1" Then
                    '企業自振スケジュールマスタのフラグチェック
                    If PFUNC_KSCHMAST_GET() = False Then
                        PFUNC_SCHMAST_GET = False
                    End If
                Else
                    Call GSUB_MESSAGE_WARNING("不能結果がまだ更新されていません")
                    PFUNC_SCHMAST_GET = False
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

    End Function
    Private Function PFUNC_KSCHMAST_GET() As Boolean

        PFUNC_KSCHMAST_GET = False

        '企業自振スケジュールマスタのフラグチェック
        STR_SQL = " SELECT * FROM SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " TORIS_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '↓2005/03/26 修正
        'STR_SQL += " AND"
        'STR_SQL += " TORIF_CODE_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '↑2005/03/26 修正
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S = '" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"

        '企業自振スケジュールマスタ存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("企業自振スケジュールが存在しません")
            Exit Function
        End If

        'LNG読込件数 = 0

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '受付フラグ、落し込みフラグ、配信フラグ
                '↓2005/04/06 修正 企業自振は配信フラグのチェックは行わない
                'If .Item("UKETUKE_FLG_S") = "1" AND _
                '   .Item("TOUROKU_FLG_S") = "1" AND _
                '   .Item("HAISIN_FLG_S") = "0" Then
                If .Item("UKETUKE_FLG_S") = "1" And _
                   .Item("TOUROKU_FLG_S") = "1" Then
                    '↑2005/04/06 修正
                Else
                    If .Item("UKETUKE_FLG_S") <> "1" Or _
                       .Item("TOUROKU_FLG_S") <> "1" Then
                        Call GSUB_MESSAGE_WARNING("振替データが作成されていません")
                        Exit While
                    End If
                    '↓2005/04/06 修正 企業自振は配信フラグのチェックは行わない
                    'If .Item("HAISIN_FLG_S") <> "0" Then
                    '    Call GSUB_MESSAGE_WARNING( "配信済です")
                    '    Exit While
                    'End If
                    '↑2005/04/06 修正
                End If
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_KSCHMAST_GET = True

    End Function
    Private Function PFUNC_MEISAI_GET() As Boolean
        '口座振替明細データの検索

        PFUNC_MEISAI_GET = False

        '学校指定の場合
        If rdbgakkouSeq.Checked = True Then
            If PFUNC_MEISAI_GAKGET() = False Then
                Exit Function
            End If
        End If

        '口座番号指定の場合
        If rdbKouza.Checked = True Then
            If PFUNC_MEISAI_KOZGET() = False Then
                Exit Function
            End If
        End If

        PFUNC_MEISAI_GET = True

    End Function
    Private Function PFUNC_MEISAI_GAKGET() As Boolean
        '明細マスタの学校指定での検索


        PFUNC_MEISAI_GAKGET = False

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
        STR_SQL += " AND"
        STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
        STR_SQL += " AND"
        STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG読込件数 = 0

        '件数カウント
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                If LNG読込件数 > 9 Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("配列（１０）を超えました")

                    Exit Function
                End If

                '学校コード退避
                STR学校コード(LNG読込件数) = .Item("GAKKOU_CODE_M")

                '入学年度の退避
                STR入学年度(LNG読込件数) = .Item("NENDO_M")

                '通番の退避
                INT通番(LNG読込件数) = .Item("TUUBAN_M")

                '読込み件数のカウントアップ
                LNG読込件数 += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG読込件数 = 0 Then
            Call GSUB_MESSAGE_WARNING("明細マスタに該当レコードが存在しません")
            Exit Function
        End If

        '生徒マスタの検索
        Call PSUB_SEITONAME_STORE()

        Select Case (LNG読込件数)
            Case Is = 1
                STR_SQL = " SELECT * FROM G_MEIMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
                STR_SQL += " AND"
                STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                With OBJ_DATAREADER
                    .Read()

                    '金融機関コード
                    txtKinyuuCode.Text = .Item("TKIN_NO_M")
                    '支店コード
                    txtSitCode.Text = .Item("TSIT_NO_M")
                    '科目
                    cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))

                    '口座番号
                    txtKouzaBan.Text = .Item("TKOUZA_M")
                    '金額
                    If IsDBNull(.Item("SEIKYU_KIN_M")) = False Then
                        txtKingaku.Text = Format(CLng(.Item("SEIKYU_KIN_M")), "#,##0")
                    Else
                        txtKingaku.Text = 0
                    End If
                    '請求月編集
                    lab請求月.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "年" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "月"
                    '名義人カナ編集
                    txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                    'レコード番号編集
                    txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                    '振替結果コード編集
                    cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                    '2005/05/11
                    strSEITO_NENDO = .Item("NENDO_M")
                    strSEITO_TUUBAN = .Item("TUUBAN_M")
                    '2006/10/26
                    update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                    '2007/09/27
                    strJUYOUKA_NO = .Item("JUYOUKA_NO_M")

                End With

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            Case Is > 1

                STR_SQL = " SELECT * FROM G_MEIMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
                STR_SQL += " AND"
                STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

                '2006/10/23 データ件数のカウント
                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If
                Dim intREC_COUNT As Integer = 0
                While (OBJ_DATAREADER.Read = True)
                    intREC_COUNT += 1
                End While
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If


                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                Dim intCOUNT As Integer = 0
                Dim ICnt As Integer = 0
                While (OBJ_DATAREADER.Read = True)
                    intCOUNT += 1
                    With OBJ_DATAREADER
                        If intREC_COUNT = intCOUNT Then
                            '当該レコードの確定処理
                            '金融機関コード
                            txtKinyuuCode.Text = .Item("TKIN_NO_M")
                            '支店コード
                            txtSitCode.Text = .Item("TSIT_NO_M")
                            '科目
                            cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                            '口座番号
                            txtKouzaBan.Text = OBJ_DATAREADER.Item("TKOUZA_M")
                            '金額
                            txtKingaku.Text = CStr(OBJ_DATAREADER.Item("SEIKYU_KIN_M"))
                            '請求月編集
                            lab請求月.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "年" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "月"
                            '名義人カナ編集
                            txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                            'レコード番号編集
                            txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                            '振替結果コード編集
                            cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                            '2005/05/11
                            strSEITO_NENDO = .Item("NENDO_M")
                            strSEITO_TUUBAN = .Item("TUUBAN_M")
                            '2006/10/26
                            update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                            '2007/09/27
                            strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                        Else
                            '確認メッセージ　　　'生徒マスタの検索
                            If GFUNC_MESSAGE_YESNO("該当レコードが複数存在します" & vbCr & _
                                                                                  "以下のレコードを表示しますか？" & vbCr & _
                                                                                  "学年 = " & .Item("GAKUNEN_CODE_M") & vbCr & _
                                                                                  "クラス = " & .Item("CLASS_CODE_M") & vbCr & _
                                                                                  "生徒番号 = " & .Item("SEITO_NO_M") & vbCr & _
                                                                                  "生徒氏名（カナ） = " & STR生徒氏名カナ(ICnt) & vbCr & _
                                                                                  "生徒氏名（漢字）= " & STR生徒氏名漢字(ICnt) & vbCr & _
                                                                                  "請求月 = " & .Item("SEIKYU_TUKI_M") & vbCrLf & _
                                                                                  "次のレコードを検索する場合は「いいえ」を押してください") = vbYes Then
                                '当該レコードの確定処理
                                '金融機関コード
                                txtKinyuuCode.Text = .Item("TKIN_NO_M")
                                '支店コード
                                txtSitCode.Text = .Item("TSIT_NO_M")
                                '科目
                                cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                                '口座番号
                                txtKouzaBan.Text = OBJ_DATAREADER.Item("TKOUZA_M")
                                '金額
                                txtKingaku.Text = CStr(OBJ_DATAREADER.Item("SEIKYU_KIN_M"))
                                '請求月編集
                                lab請求月.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "年" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "月"
                                '名義人カナ編集
                                txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                                'レコード番号編集
                                txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                                '振替結果コード編集
                                cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                                '2005/05/11
                                strSEITO_NENDO = .Item("NENDO_M")
                                strSEITO_TUUBAN = .Item("TUUBAN_M")
                                '2006/10/26
                                update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                                '2007/09/27
                                strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                                Exit While
                            End If

                        End If
                    End With

                    ICnt += 1
                End While

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
        End Select

        PFUNC_MEISAI_GAKGET = True

    End Function
    Private Function PFUNC_MEISAI_KOZGET() As Boolean
        '明細マスタの口座番号指定での検索

        Dim sESC_SQL As String

        PFUNC_MEISAI_KOZGET = False

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M ='" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_M ='" & txtSitCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TKAMOKU_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) & "'"
        'STR_SQL += " TKAMOKU_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKOUZA_M ='" & txtKouzaBan.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M = " & CLng(txtKingaku.Text)

        sESC_SQL = STR_SQL

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG読込件数 = 0

        '件数カウント
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                If LNG読込件数 > 9 Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("配列（１０）を超えました")

                    Exit Function
                End If

                '学校コード退避
                STR学校コード(LNG読込件数) = .Item("GAKKOU_CODE_M")

                '入学年度の退避
                STR入学年度(LNG読込件数) = .Item("NENDO_M")

                '通番の退避
                INT通番(LNG読込件数) = .Item("TUUBAN_M")
            End With

            '読込み件数のカウントアップ
            LNG読込件数 += 1
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG読込件数 = 0 Then
            Call GSUB_MESSAGE_WARNING("明細マスタに該当レコードが存在しません")
            Exit Function
        End If

        '生徒マスタの検索
        Call PSUB_SEITONAME_STORE()

        Select Case (LNG読込件数)
            Case Is = 1
                STR_SQL = sESC_SQL

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                With OBJ_DATAREADER

                    .Read()

                    '金融機関コード
                    txtKinyuuCode.Text = .Item("TKIN_NO_M")
                    '支店コード
                    txtSitCode.Text = .Item("TSIT_NO_M")
                    '科目
                    cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                    '口座番号
                    txtKouzaBan.Text = .Item("TKOUZA_M")
                    '金額
                    txtKingaku.Text = CStr(.Item("SEIKYU_KIN_M"))
                    '請求月編集
                    lab請求月.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "年" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "月"
                    '名義人カナ編集
                    txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                    'レコード番号編集
                    txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                    '振替結果コード編集
                    cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                    '2005/05/11
                    strSEITO_NENDO = .Item("NENDO_M")
                    strSEITO_TUUBAN = .Item("TUUBAN_M")
                    '2006/10/26
                    update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                    '2007/09/27
                    strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                End With

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            Case Is > 1

                STR_SQL = sESC_SQL

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                Dim ICnt As Integer = 0

                While (OBJ_DATAREADER.Read = True)
                    With OBJ_DATAREADER
                        'Debug.WriteLine("READ")
                        '確認メッセージ
                        If GFUNC_MESSAGE_YESNO("該当レコードが複数存在します" & vbCr & _
                                                          "以下のレコードを表示しますか？" & vbCr & _
                                                          "学年 = " & .Item("GAKUNEN_CODE_M") & vbCr & _
                                                          "クラス = " & .Item("CLASS_CODE_M") & vbCr & _
                                                          "生徒番号 = " & .Item("SEITO_NO_M") & vbCr & _
                                                          "生徒氏名（カナ） = " & STR生徒氏名カナ(ICnt) & vbCr & _
                                                          "生徒氏名（漢字）= " & STR生徒氏名漢字(ICnt) & vbCr & _
                                                          "請求月 = " & .Item("SEIKYU_TUKI_M") & vbCrLf & _
                                                          "次のレコードを検索する場合は「いいえ」を押してください") = vbYes Then

                            '当該レコードの確定処理
                            '金融機関コード
                            txtKinyuuCode.Text = .Item("TKIN_NO_M")
                            '支店コード
                            txtSitCode.Text = .Item("TSIT_NO_M")
                            '科目
                            cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                            '口座番号
                            txtKouzaBan.Text = .Item("TKOUZA_M")
                            '金額
                            txtKingaku.Text = CStr(.Item("SEIKYU_KIN_M"))
                            '請求月編集
                            lab請求月.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "年" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "月"
                            '名義人カナ編集
                            txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                            'レコード番号編集
                            txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                            '振替結果コード編集
                            cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                            '2005/05/11
                            strSEITO_NENDO = .Item("NENDO_M")
                            strSEITO_TUUBAN = .Item("TUUBAN_M")
                            '2006/10/26
                            update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                            '2007/09/27
                            strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                            Exit While
                        End If
                    End With

                    ICnt += 1
                End While

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
        End Select

        PFUNC_MEISAI_KOZGET = True

    End Function
    Private Function PFUNC_KMEISAI_UPD() As Boolean

        PFUNC_KMEISAI_UPD = False

        '企業自振　明細マスタの更新
        STR_SQL = " UPDATE MEIMAST SET "
        '振替結果コード
        STR_SQL += " FURIKETU_CODE_K = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu) & "'"
        '検索キーは、学校コード、振替日、振替入出区分、金融機関コード、支店コード、科目コード、口座番号、請求金額
        STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        STR_SQL += " AND FURI_DATE_K ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FSYORI_KBN_K ='1'"
        STR_SQL += " AND KEIYAKU_KIN_K ='" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND KEIYAKU_SIT_K ='" & txtSitCode.Text & "'"
        STR_SQL += " AND KEIYAKU_KAMOKU_K ='" & ConvKamoku1To2(CStr(CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku)))) & "'"
        STR_SQL += " AND KEIYAKU_KOUZA_K ='" & txtKouzaBan.Text & "'"
        STR_SQL += " AND FURIKIN_K = " & CLng(txtKingaku.Text)
        STR_SQL += " AND"
        'STR_SQL += " SUBSTR(JYUYOUKA_NO_K,1,8) = '" & strSEITO_NENDO & strSEITO_TUUBAN.PadLeft(4, "0") & "'"
        '蒲郡信金向け 学校コード(7)＋学年(1)+クラス(2)+生徒番号(7)+請求月(2)+振替区分(1)　2007/09/04
        STR_SQL += " SUBSTR(JYUYOUKA_NO_K,1,20) = '" & strJUYOUKA_NO & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '更新処理エラー
            Call GSUB_MESSAGE_WARNING("企業自振明細マスタの更新でエラーが発生しました。")
            Exit Function
        End If

        PFUNC_KMEISAI_UPD = True

    End Function
    Private Function PSUB_SEITONAME_STORE() As Boolean
        '生徒マスタの検索


        For i As Integer = 0 To LNG読込件数 - 1

            STR_SQL = " SELECT * FROM SEITOMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_O ='" & STR学校コード(i) & "'"
            STR_SQL += " AND"
            STR_SQL += " NENDO_O ='" & STR入学年度(i) & "'"
            STR_SQL += " AND"
            STR_SQL += " TUUBAN_O =" & INT通番(i)

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With OBJ_DATAREADER
                If .Read() = True Then
                    STR生徒氏名カナ(i) = .Item("SEITO_KNAME_O")
                    STR生徒氏名漢字(i) = ConvNullToString(.Item("SEITO_NNAME_O"), "")
                Else
                    STR生徒氏名カナ(i) = ""
                    STR生徒氏名漢字(i) = ""
                End If
            End With

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        Next i

    End Function
    Private Function PFUNC_GSCHAMST_UPD() As Boolean

        Dim iGakunen_Flag() As Integer = Nothing
        Dim bFlg As Boolean
        Dim bLoopFlg As Boolean
        Dim iLcount As Integer

        PFUNC_GSCHAMST_UPD = False


        '更新学年からスケジュールの学年フラグを取得し、更新対象スケジュールを指定する
        '2006/10/26
        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND GAKUNEN" & update_GAKUNEN & "_FLG_S ='1'"
        '追加 2006/11/29
        STR_SQL += " AND FURI_KBN_S = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            Exit Function
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                UPD_SCH_KBN = .Item("SCH_KBN_S")
                UPD_FURI_KBN = .Item("FURI_KBN_S")
                UPD_FURI_DATE = .Item("FURI_DATE_S")
                UPD_SFURI_DATE = .Item("SFURI_DATE_S")
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        'スケジュールが年間、特別から
        Select Case (PFUNC_Get_Gakunen(txtGAKKOU_CODE.Text, iGakunen_Flag))
            Case -1
                'エラー
                Call GSUB_LOG(0, "指定学年取得でエラーが発生しました")

                Exit Function
            Case 0
                '全学年が対象
                bFlg = False
            Case Else
                '特定学年のみが対象
                bFlg = True
        End Select

        '---------------------------------------
        '振替済件数・金額の取得
        '---------------------------------------
        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        STR_SQL = " SELECT COUNT(SEIKYU_KIN_M),SUM(SEIKYU_KIN_M) FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M ='0'"
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If
        '追加 2006/11/29
        STR_SQL += " AND FURI_KBN_M ='" & UPD_FURI_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFURI_KEN = 0
            dblFURI_KIN = 0
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                dblFURI_KEN = .Item("COUNT(SEIKYU_KIN_M)")
                dblFURI_KIN = CDbl(ConvNullToString(.Item("SUM(SEIKYU_KIN_M)"), "0"))
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        bLoopFlg = False

        '---------------------------------------
        '不能件数・金額の取得
        '---------------------------------------
        Dim dblFUNOU_KEN As Double
        Dim dblFUNOU_KIN As Double
        STR_SQL = " SELECT count(SEIKYU_KIN_M),sum(SEIKYU_KIN_M) FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FURIKETU_CODE_M <> '0'"
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If
        '追加 2006/11/29
        STR_SQL += " AND FURI_KBN_M ='" & UPD_FURI_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFUNOU_KEN = 0
            dblFUNOU_KIN = 0
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                dblFUNOU_KEN = .Item("count(SEIKYU_KIN_M)")
                dblFUNOU_KIN = CDbl(ConvNullToString(.Item("sum(SEIKYU_KIN_M)"), "0"))
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '---------------------------------------
        'スケジュールマスタの更新
        '---------------------------------------
        '初振スケジュールマスタ更新
        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " FURI_KEN_S =" & dblFURI_KEN & ","
        STR_SQL += " FURI_KIN_S =" & dblFURI_KIN & ","
        STR_SQL += " FUNOU_KEN_S =" & dblFUNOU_KEN & ","
        STR_SQL += " FUNOU_KIN_S =" & dblFUNOU_KIN & ""
        STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND SFURI_DATE_S ='" & UPD_SFURI_DATE & "'"
        STR_SQL += " AND SCH_KBN_S = '" & UPD_SCH_KBN & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '更新処理エラー
            Call GSUB_MESSAGE_WARNING("スケジュールマスタの更新でエラーが発生しました。")

            Exit Function
        End If

        PFUNC_GSCHAMST_UPD = True

    End Function

    Private Function PFUNC_GSCHAMST_UPD_TAKOU() As Boolean
        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        Dim dblFUNOU_KEN As Double
        Dim dblFUNOU_KIN As Double
        Dim intGAKUNEN_CODE As Integer = 0
        Dim intFURI_KBN As Integer = 0
        Dim strTAKOU_CD As String = ""

        PFUNC_GSCHAMST_UPD_TAKOU = False

        STR_SQL = " SELECT SUM(decode(FURIKETU_CODE_M,'0',1,0)) AS FURIKEN ,SUM(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) AS FURIKIN,"
        STR_SQL += " SUM(DECODE(FURIKETU_CODE_M,'0',0,1)) AS FUNOUKEN ,SUM(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) AS FUNOUKIN,"
        STR_SQL += " GAKUNEN_CODE_M,FURI_KBN_M,TKIN_NO_M"
        STR_SQL += " FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND SEIKYU_KIN_M > 0"
        STR_SQL += " AND TKIN_NO_M <> '" & STR_JIKINKO_CODE & "'"
        STR_SQL += " GROUP BY GAKKOU_CODE_M,GAKUNEN_CODE_M,FURI_KBN_M,TKIN_NO_M"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFURI_KEN = 0
            dblFURI_KIN = 0
            intGAKUNEN_CODE = 0
            intFURI_KBN = 0
            strTAKOU_CD = ""
        Else
            While (OBJ_DATAREADER_DREAD.Read = True)

                With OBJ_DATAREADER_DREAD
                    dblFURI_KEN = .Item("FURIKEN")
                    dblFURI_KIN = CDbl(ConvNullToString(.Item("FURIKIN"), "0"))
                    dblFUNOU_KEN = .Item("FUNOUKEN")
                    dblFUNOU_KIN = CDbl(ConvNullToString(.Item("FUNOUKIN"), "0"))
                    intGAKUNEN_CODE = CInt(ConvNullToString(.Item("GAKUNEN_CODE_M"), 0))
                    intFURI_KBN = CInt(ConvNullToString(.Item("FURI_KBN_M"), 0))
                    strTAKOU_CD = .Item("TKIN_NO_M")
                    '---------------------------------------
                    'スケジュールマスタの更新
                    '---------------------------------------
                    '初振スケジュールマスタ更新
                    STR_SQL = " UPDATE  G_TAKOUSCHMAST SET "
                    STR_SQL += " FURI_KEN_U =" & dblFURI_KEN & ","
                    STR_SQL += " FURI_KIN_U =" & dblFURI_KIN & ","
                    STR_SQL += " FUNOU_KEN_U =" & dblFUNOU_KEN & ","
                    STR_SQL += " FUNOU_KIN_U =" & dblFUNOU_KIN
                    STR_SQL += " WHERE GAKKOU_CODE_U  ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND FURI_DATE_U ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                    STR_SQL += " AND FURI_KBN_U ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                    STR_SQL += " AND GAKUNEN_U  =" & intGAKUNEN_CODE
                    STR_SQL += " AND TKIN_NO_U  = '" & strTAKOU_CD & "'"

                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '更新処理エラー
                        Call GSUB_MESSAGE_WARNING("スケジュールマスタの更新でエラーが発生しました。")

                        Exit Function
                    End If

                End With
            End While

        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If


        PFUNC_GSCHAMST_UPD_TAKOU = True

    End Function
    Private Function PFUNC_Get_Gakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen() As Integer) As Integer

        Dim iLoopCount As Integer
        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen = -1

        '選択された学校の指定振替日で抽出
        '(全スケジュール区分が対象)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND CHECK_FLG_S ='1'"
        STR_SQL += " AND DATA_FLG_S ='1'"
        STR_SQL += " AND FUNOU_FLG_S ='1'"
        STR_SQL += " AND TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND SFURI_DATE_S ='" & UPD_SFURI_DATE & "'"
        STR_SQL += " AND FURI_KBN_S = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND SCH_KBN_S = '" & UPD_SCH_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read)
            With OBJ_DATAREADER_DREAD
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        '使用学年全てに学年フラグがある場合は全学年対象として扱う
        '学年
        For iLoopCount = 1 To iMaxGakunen
            Select Case (pSiyou_gakunen(iLoopCount))
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function


#End Region

#Region " CheckedChanged(RadioButton) "
    '****************************
    'CheckedChanged
    '****************************
    Private Sub rdbgakkouSeq_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbgakkouSeq.CheckedChanged
        '学校指定
        If rdbgakkouSeq.Checked = True Then
            txtGAKUNEN.Visible = True
            lbl1.Visible = True
            txtCLASS.Visible = True
            lbl2.Visible = True
            txtSEITONO.Visible = True
        Else
            txtGAKUNEN.Visible = False
            lbl1.Visible = False
            txtCLASS.Visible = False
            lbl2.Visible = False
            txtSEITONO.Visible = False
        End If
    End Sub
    Private Sub rdbKouza_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKouza.CheckedChanged
        '口座番号指定
        If rdbKouza.Checked = True Then
            GroupBox1.Visible = True
        Else
            GroupBox1.Visible = False
        End If
    End Sub
#End Region

#Region " LostFocus "
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校名の取得
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '学校名の取得
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    Private Sub txtKingaku_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtKingaku.Validating

        If btnSansyou.Enabled = True Then
            btnSansyou.Focus()
        Else
            cmbFuriketu.Focus()
        End If
    End Sub
#End Region

#Region " SELECT edIndexChanged(ComboBox) "
    '****************************
    'SelectedIndexChanged
    '****************************
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        If PFUNC_GAKNAME_GET() = False Then
            Exit Sub
        End If

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
#End Region

End Class
