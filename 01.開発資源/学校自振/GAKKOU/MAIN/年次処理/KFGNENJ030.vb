Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGNENJ030

#Region " 共通変数定義 "
    Public strDIR As String        '現在のパス
    Public intSinTuuban As Integer '進学生徒入力用通番
#End Region

#Region " 構造体定義 "
    '***********************************************
    '学校ごと（進学先学校含む）の入力項目情報構造体
    '***********************************************
    Public Structure structureGakkouData
        Public cmbKana As ComboBox       'カナコンボボックス
        Public cmbGakkouName As ComboBox '学校名コンボボックス
        Public txtGakkouCode As TextBox  '学校コードテキストボックス
        Public labGakkouName As Label    '学校名表示ラベル
        Public strGakkouCode() As String '学校コード取得用配列
        Public strKigyoCode As String    '企業コード
        Public strFuriCode As String     '振替コード
    End Structure
    Public GakkouData(6) As structureGakkouData

    '***********************************************
    '生徒マスタの最高学年生徒（進学生徒）情報構造体
    '***********************************************
    Public Structure structureSeitoData
        '生徒マスタの内容を全て取得する
        Public GAKKOU_CODE As String
        Public NENDO As String
        Public TUUBAN As Integer
        Public GAKUNEN_CODE As Integer
        Public CLASS_CODE As Integer
        Public SEITO_NO As String
        Public SEITO_KNAME As String
        Public SEITO_NNAME As String
        Public SEIBETU As String
        Public TKIN_NO As String
        Public TSIT_NO As String
        Public KAMOKU As String
        Public KOUZA As String
        Public MEIGI_KNAME As String
        Public MEIGI_NNAME As String
        Public FURIKAE As String
        Public KEIYAKU_NJYU As String
        Public KEIYAKU_DENWA As String
        Public KAIYAKU_FLG As String
        Public SINKYU_KBN As String
        Public HIMOKU_ID As String
        Public TYOUSI_FLG As Integer
        Public TYOUSI_NENDO As String
        Public TYOUSI_TUUBAN As Integer
        Public TYOUSI_GAKUNEN As Integer
        Public TYOUSI_CLASS As Integer
        Public TYOUSI_SEITONO As String
        Public TUKI_NO As String
        Public SEIKYU01 As String
        Public KINGAKU01 As Integer
        Public SEIKYU02 As String
        Public KINGAKU02 As Integer
        Public SEIKYU03 As String
        Public KINGAKU03 As Integer
        Public SEIKYU04 As String
        Public KINGAKU04 As Integer
        Public SEIKYU05 As String
        Public KINGAKU05 As Integer
        Public SEIKYU06 As String
        Public KINGAKU06 As Integer
        Public SEIKYU07 As String
        Public KINGAKU07 As Integer
        Public SEIKYU08 As String
        Public KINGAKU08 As Integer
        Public SEIKYU09 As String
        Public KINGAKU09 As Integer
        Public SEIKYU10 As String
        Public KINGAKU10 As Integer
        Public SEIKYU11 As String
        Public KINGAKU11 As Integer
        Public SEIKYU12 As String
        Public KINGAKU12 As Integer
        Public SEIKYU13 As String
        Public KINGAKU13 As Integer
        Public SEIKYU14 As String
        Public KINGAKU14 As Integer
        Public SEIKYU15 As String
        Public KINGAKU15 As Integer
        Public SAKUSEI_DATE As String
        Public KOUSIN_DATE As String
        Public YOBI1 As String
        Public YOBI2 As String
        Public YOBI3 As String
    End Structure
    Public SeitoData() As structureSeitoData
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'ループ用変数

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        STR_SYORI_NAME = "進学データ作成処理"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '構造体に各フォームを設定する
        PSUB_StructureSet()

        '学校コンボ設定（全学校）
        For i As Integer = 1 To 6
            If GFUNC_DB_COMBO_SET(GakkouData(i).cmbKana, GakkouData(i).cmbGakkouName) = False Then
                Call GSUB_LOG(0, "コンボボックス設定")
                Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")
                Exit Sub
            Else
                'コンボボックスより学校を指定する際の配列の設定(配列は学校コード一覧)
                If STR_GCOAD Is Nothing Then
                    '学校が登録されていない
                    ReDim STR_GCOAD(0)
                Else
                    '学校コード一覧を取得
                    GakkouData(i).strGakkouCode = STR_GCOAD.Clone
                End If
            End If
        Next

        '入学年度表示
        '===2008/04/13 年度ラベル→テキストボックスに変更=============
        'If Format(Month(Now), "00") >= 4 Then
        '    '４～１２月
        '    labNendo.Text = Format(Year(Now), "0000") + 1
        'Else
        '    '１～３月
        '    labNendo.Text = Format(Year(Now), "0000")
        'End If
        If Format(Month(Now), "00") >= 4 Then
            '４～１２月
            txtNendo.Text = Format(Year(Now), "0000") + 1
        Else
            '１～３月
            txtNendo.Text = Format(Year(Now), "0000")
        End If
        '==========================================================

        '現在のパスを取得
        strDIR = CurDir()

        txtGakkouCode1.Focus()

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '==========================
        '実行ボタン
        '==========================

        '入力チェック
        If PFUNC_CommonCheck() = False Then
            Exit Sub
        End If

        '新入生マスタ取得
        If PFUNC_SEITOMAST2_Check() = False Then
            Exit Sub
        End If

        If MessageBox.Show("処理しますか？", STR_SYORI_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        End If

        '進学生徒情報取得
        If PFUNC_SEITOMAST_Check() = False Then
            Exit Sub
        End If

        '進学生徒登録処理
        If PFUNC_InsSinnyusei() = False Then
            Exit Sub
        End If

        '帳票印刷処理
        If PFUNC_PrintMeisai() = False Then
            Exit Sub
        End If

        ''2008/03/09 仕様変更によりコメント
        ''ＣＳＶファイル出力
        'If PFUNC_CSVFileWrite() = False Then
        '    Exit Sub
        'End If

        GSUB_LOG_OUT(GakkouData(6).txtGakkouCode.Text, "", "進学データ作成", "データ作成処理", "成功", "")  '2008/03/23 処理成功ログ追加
        GSUB_MESSAGE_INFOMATION("登録しました")

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        '取消しボタン（画面クリア）

        'ループ用変数

        '各欄初期化
        For i As Integer = 1 To 6

            '学校コード・学校名・カナコンボ初期化
            GakkouData(i).txtGakkouCode.Text = ""
            GakkouData(i).labGakkouName.Text = ""
            GakkouData(i).cmbKana.SelectedIndex = -1

            If GFUNC_DB_COMBO_SET(GakkouData(i).cmbKana, GakkouData(i).cmbGakkouName) = False Then
                Call GSUB_LOG(0, "コンボボックス設定")
                Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")
                Exit Sub
            Else
                'コンボボックスより学校を指定する際の配列の設定(配列は学校コード一覧)
                If STR_GCOAD Is Nothing Then
                    '学校が登録されていない
                    ReDim STR_GCOAD(0)
                Else
                    '学校コード一覧を取得
                    GakkouData(i).strGakkouCode = STR_GCOAD.Clone
                End If
            End If
        Next

        '入学年度表示
        '===2008/04/13 ラベル→テキストボックスに変更=============
        'If Format(Month(Now), "00") >= 4 Then
        '    '４～１２月
        '    labNendo.Text = Format(Year(Now), "0000") + 1
        'Else
        '    '１～３月
        '    labNendo.Text = Format(Year(Now), "0000")
        'End If
        If Format(Month(Now), "00") >= 4 Then
            '４～１２月
            txtNendo.Text = Format(Year(Now), "0000") + 1
        Else
            '１～３月
            txtNendo.Text = Format(Year(Now), "0000")
        End If
        '=========================================================


    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '終了ボタン

        Me.Close()
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GakNameGet(ByVal intIndex As Integer) As Integer
        '学校名の設定
        PFUNC_GakNameGet = 0

        STR_SQL = "SELECT GAKKOU_NNAME_G,SINKYU_NENDO_T,YOBI1_T,YOBI2_T FROM KZFMAST.GAKMAST1,KZFMAST.GAKMAST2 "
        STR_SQL += "WHERE GAKKOU_CODE_G = GAKKOU_CODE_T "
        STR_SQL += "AND GAKKOU_CODE_G = " & " '" & GakkouData(intIndex).txtGakkouCode.Text & "'"

        '学校マスタ存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            GSUB_MESSAGE_WARNING("学校マスタに登録されていません")
            GakkouData(intIndex).labGakkouName.Text = ""
            GakkouData(intIndex).txtGakkouCode.Focus()
            PFUNC_GakNameGet = 1
            Exit Function
        End If

        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        '進級処理チェック
        '===2008/04/13 年度ラベル→テキストボックスに変更==================================================================
        If CInt(OBJ_DATAREADER.Item("SINKYU_NENDO_T")) >= CInt(txtNendo.Text) Then
            'GSUB_MESSAGE_WARNING( "すでに進級処理済みの学校です")       '2008/03/09　修正
            GSUB_MESSAGE_WARNING(GakkouData(intIndex).labGakkouName.Text.Trim & "　は既に進級処理済みの学校です")
        End If
        '==================================================================================================================

        '学校名表示
        GakkouData(intIndex).labGakkouName.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        '企業コード・振替コード取得
        GakkouData(intIndex).strKigyoCode = OBJ_DATAREADER.Item("YOBI1_T")
        GakkouData(intIndex).strFuriCode = OBJ_DATAREADER.Item("YOBI2_T")

        OBJ_DATAREADER.Close()

    End Function

    Private Function PFUNC_SEITOMAST_Check() As Boolean
        '=============================================
        '進学生徒のチェック・取得
        '=============================================
        PFUNC_SEITOMAST_Check = False

        Dim j As Integer

        Try
            For i As Integer = 1 To 5
                If GakkouData(i).txtGakkouCode.Text.Trim <> "" Then
                    '生徒マスタから最高学年の生徒(進学する生徒)を取得
                    STR_SQL = "SELECT * FROM KZFMAST.SEITOMAST "
                    STR_SQL += "WHERE GAKKOU_CODE_O = '" & GakkouData(i).txtGakkouCode.Text.Trim & "' "
                    STR_SQL += "AND GAKUNEN_CODE_O = (SelectSAIKOU_GAKUNEN_T FROM GAKMAST2 "
                    STR_SQL += "WHERE GAKKOU_CODE_T = '" & GakkouData(i).txtGakkouCode.Text.Trim & "') "
                    STR_SQL += " AND KAIYAKU_FLG_O = '0' "      '2008/03/23 解約済み生徒ははずす
                    STR_SQL += " AND SINKYU_KBN_O = '0' "       '2008/03/23 進級しない生徒ははずす
                    STR_SQL += "AND TUKI_NO_O = '04'" '４月分のみ採取
                    STR_SQL += " ORDER BY CLASS_CODE_O , SEITO_NO_O "   '2008/03/23 クラス・生徒番号順に新通番を採番

                    '生徒マスタ／進学生徒存在チェック
                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        'GSUB_MESSAGE_WARNING( "進学する生徒が存在しません")     '2008/03/09 修正
                        GSUB_MESSAGE_WARNING(GakkouData(i).labGakkouName.Text.Trim & " には進学する生徒が存在しません")
                        Exit Function
                    End If

                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

                    While (OBJ_DATAREADER.Read = True)
                        '構造体配列の再定義（要素数の更新）
                        ReDim Preserve SeitoData(j)

                        '通番設定
                        intSinTuuban += 1

                        '進学生徒情報を格納
                        With SeitoData(j)
                            .GAKKOU_CODE = GakkouData(6).txtGakkouCode.Text '学校CODE：進学先学校
                            '===2008/04/13 年度ラベル→テキストボックスに変更======================================
                            '.NENDO = labNendo.Text.Trim                     '入学年度：表示されている値を設定
                            .NENDO = txtNendo.Text.Trim                     '入学年度：表示されている値を設定
                            '======================================================================================
                            .TUUBAN = intSinTuuban                          '通　　番：採番しなおす
                            .GAKUNEN_CODE = 0                               '学　　年：０固定(新入生のため)
                            .CLASS_CODE = 1                                 'ク ラ ス：１固定
                            .SEITO_NO = Format(intSinTuuban, "0000000")     '生徒番号：採番しなおす
                            .SEITO_KNAME = OBJ_DATAREADER.Item("SEITO_KNAME_O")
                            .SEITO_NNAME = PFUNC_NullCheckST(OBJ_DATAREADER.Item("SEITO_NNAME_O"))
                            .SEIBETU = OBJ_DATAREADER.Item("SEIBETU_O")
                            .TKIN_NO = OBJ_DATAREADER.Item("TKIN_NO_O")
                            .TSIT_NO = OBJ_DATAREADER.Item("TSIT_NO_O")
                            .KAMOKU = OBJ_DATAREADER.Item("KAMOKU_O")
                            .KOUZA = OBJ_DATAREADER.Item("KOUZA_O")
                            .MEIGI_KNAME = OBJ_DATAREADER.Item("MEIGI_KNAME_O")
                            .MEIGI_NNAME = PFUNC_NullCheckST(OBJ_DATAREADER.Item("MEIGI_NNAME_O"))
                            .FURIKAE = OBJ_DATAREADER.Item("FURIKAE_O")
                            .KEIYAKU_NJYU = PFUNC_NullCheckST(OBJ_DATAREADER.Item("KEIYAKU_NJYU_O"))
                            .KEIYAKU_DENWA = PFUNC_NullCheckST(OBJ_DATAREADER.Item("KEIYAKU_DENWA_O"))
                            .KAIYAKU_FLG = OBJ_DATAREADER.Item("KAIYAKU_FLG_O")
                            .SINKYU_KBN = OBJ_DATAREADER.Item("SINKYU_KBN_O")
                            .HIMOKU_ID = "001"                              '費　　目：１固定
                            .TYOUSI_FLG = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_FLG_O"))
                            .TYOUSI_NENDO = PFUNC_NullCheckST(OBJ_DATAREADER.Item("TYOUSI_NENDO_O"))    '2007/10/15修正
                            .TYOUSI_TUUBAN = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_TUUBAN_O"))
                            .TYOUSI_GAKUNEN = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_GAKUNEN_O"))
                            .TYOUSI_CLASS = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_CLASS_O"))
                            .TYOUSI_SEITONO = OBJ_DATAREADER.Item("TYOUSI_SEITONO_O")
                            .TUKI_NO = OBJ_DATAREADER.Item("TUKI_NO_O")
                            .SEIKYU01 = "0"                                 '個別FLG ：０固定(費目１～１５)
                            .KINGAKU01 = 0                                  '個別金額：０固定(費目１～１５)
                            .SEIKYU02 = "0"
                            .KINGAKU02 = 0
                            .SEIKYU03 = "0"
                            .KINGAKU03 = 0
                            .SEIKYU04 = "0"
                            .KINGAKU04 = 0
                            .SEIKYU05 = "0"
                            .KINGAKU05 = 0
                            .SEIKYU06 = "0"
                            .KINGAKU06 = 0
                            .SEIKYU07 = "0"
                            .KINGAKU07 = 0
                            .SEIKYU08 = "0"
                            .KINGAKU08 = 0
                            .SEIKYU09 = "0"
                            .KINGAKU09 = 0
                            .SEIKYU10 = "0"
                            .KINGAKU10 = 0
                            .SEIKYU11 = "0"
                            .KINGAKU11 = 0
                            .SEIKYU12 = "0"
                            .KINGAKU12 = 0
                            .SEIKYU13 = "0"
                            .KINGAKU13 = 0
                            .SEIKYU14 = "0"
                            .KINGAKU14 = 0
                            .SEIKYU15 = "0"
                            .KINGAKU15 = 0
                            .SAKUSEI_DATE = Now.ToString("yyyyMMdd")        '登録日付：システム日付
                            .KOUSIN_DATE = ""                               '更新日付：なし
                            '予備１～３に旧データを保持する
                            '予備１：旧学校コード
                            .YOBI1 = GakkouData(i).txtGakkouCode.Text.Trim
                            '予備２：旧企業コード・旧振替コード
                            .YOBI2 = GakkouData(i).strKigyoCode.Trim & GakkouData(i).strFuriCode.Trim
                            '予備３：旧クラス・旧生徒番号
                            .YOBI3 = OBJ_DATAREADER.Item("CLASS_CODE_O") & OBJ_DATAREADER.Item("SEITO_NO_O")
                        End With

                        j += 1 'カウントアップ

                    End While
                    OBJ_DATAREADER.Close()
                End If
            Next

            PFUNC_SEITOMAST_Check = True

        Catch ex As Exception
            Call GSUB_LOG(0, "進学生徒情報取得")
            GSUB_MESSAGE_WARNING("進学生徒情報の取得に失敗しました" & vbCrLf & ex.Message)
            OBJ_DATAREADER.Close()
            Exit Function
        End Try

    End Function

    Private Function PFUNC_SEITOMAST2_Check() As Boolean
        '=============================================
        '新入生マスタの通番取得
        '=============================================
        PFUNC_SEITOMAST2_Check = False

        STR_SQL = "SELECT MAX(TUUBAN_O) FROM KZFMAST.SEITOMAST2 "
        STR_SQL += "WHERE GAKKOU_CODE_O = '" & GakkouData(6).txtGakkouCode.Text & "' "
        '===2008/04/13 年度ラベル→テキストボックスに変更=======
        'STR_SQL += "AND NENDO_O = '" & labNendo.Text & "'"
        STR_SQL += "AND NENDO_O = '" & txtNendo.Text & "'"
        '=======================================================

        Try
            OBJ_COMMAND.Connection = OBJ_CONNECTION
            OBJ_COMMAND.CommandText = STR_SQL
            OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

            OBJ_DATAREADER.Read()

            '新入生通番の取得
            If TypeOf OBJ_DATAREADER.Item("MAX(TUUBAN_O)") Is DBNull Then
                '登録なし（ＤＢＮＵＬＬ）
                intSinTuuban = 0
            Else
                '通番の最大値を取得
                intSinTuuban = OBJ_DATAREADER.Item("MAX(TUUBAN_O)")
            End If

            OBJ_DATAREADER.Close()
            PFUNC_SEITOMAST2_Check = True

        Catch ex As Exception
            Call GSUB_LOG(0, "新入生マスタ取得")
            GSUB_MESSAGE_WARNING("新入生マスタの取得に失敗しました" & vbCrLf & ex.Message)
            OBJ_DATAREADER.Close()
            Exit Function
        End Try

    End Function

    Private Function PFUNC_CommonCheck() As Boolean
        '===================================
        '入力チェック
        '===================================
        PFUNC_CommonCheck = False

        Dim j As Integer

        '===2008/04/13===========================================
        '-------------------------------------------
        '進学先学校コード入力チェック
        '-------------------------------------------
        If txtNendo.Text = "" Then
            GSUB_MESSAGE_WARNING("入学年度が未入力です")
            txtNendo.Focus()
            Exit Function
        End If
        '========================================================

        '-------------------------------------------
        '進学先学校コード入力チェック
        '-------------------------------------------
        If GakkouData(6).txtGakkouCode.Text = "" Then
            GSUB_MESSAGE_WARNING("進学先学校が未入力です")
            GakkouData(6).txtGakkouCode.Focus()
            Exit Function
        End If

        '-------------------------------------------
        '同じ学校コードが入力されていないかチェック
        '-------------------------------------------
        For i As Integer = 1 To 5
            '空欄の場合はチェックしない
            If GakkouData(i).txtGakkouCode.Text.Trim <> "" Then
                For j = i + 1 To 6
                    If GakkouData(i).txtGakkouCode.Text.Trim = GakkouData(j).txtGakkouCode.Text.Trim Then
                        '同じ学校が２つ以上入力されている
                        GSUB_MESSAGE_WARNING("同じ学校が２つ以上入力されています")
                        PFUNC_CommonCheck = False
                        GakkouData(i).txtGakkouCode.Focus()
                        Exit Function
                    Else
                        '１件でも有効な学校が入力されていた場合
                        PFUNC_CommonCheck = True
                    End If
                Next
            End If
        Next

        '-------------------------------------------
        '進学元学校コード入力チェック
        '-------------------------------------------
        If PFUNC_CommonCheck = False Then
            '学校コードが一件も入力されていない
            GSUB_MESSAGE_WARNING("進学元学校は最低１件必要です")
            GakkouData(1).txtGakkouCode.Focus()
        End If

    End Function

    Private Function PFUNC_InsSinnyusei() As Boolean
        '===================================
        '新入生マスタ登録処理
        '===================================
        PFUNC_InsSinnyusei = False

        Dim intTuki As Integer

        'トランザクション開始
        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        For i As Integer = 0 To SeitoData.Length - 1
            '-----------------------------------
            '構造体から値を抽出、ＳＱＬ文を作成
            '-----------------------------------
            For intTuki = 1 To 12   '１２ヶ月分
                With SeitoData(i)
                    STR_SQL = "INSERT INTO SEITOMAST2 VALUES ('"
                    STR_SQL += .GAKKOU_CODE & "','"
                    STR_SQL += .NENDO & "',"
                    STR_SQL += .TUUBAN & ","
                    STR_SQL += .GAKUNEN_CODE & ","
                    STR_SQL += .CLASS_CODE & ",'"
                    STR_SQL += .SEITO_NO & "','"
                    STR_SQL += .SEITO_KNAME & "','"
                    STR_SQL += .SEITO_NNAME & "','"
                    STR_SQL += .SEIBETU & "','"
                    STR_SQL += .TKIN_NO & "','"
                    STR_SQL += .TSIT_NO & "','"
                    STR_SQL += .KAMOKU & "','"
                    STR_SQL += .KOUZA & "','"
                    STR_SQL += .MEIGI_KNAME & "','"
                    STR_SQL += .MEIGI_NNAME & "','"
                    STR_SQL += .FURIKAE & "','"
                    STR_SQL += .KEIYAKU_NJYU & "','"
                    STR_SQL += .KEIYAKU_DENWA & "','"
                    STR_SQL += .KAIYAKU_FLG & "','"
                    STR_SQL += .SINKYU_KBN & "','"
                    STR_SQL += .HIMOKU_ID & "',"
                    STR_SQL += .TYOUSI_FLG & ",'"
                    STR_SQL += .TYOUSI_NENDO & "',"
                    STR_SQL += .TYOUSI_TUUBAN & ","
                    STR_SQL += .TYOUSI_GAKUNEN & ","
                    STR_SQL += .TYOUSI_CLASS & ",'"
                    STR_SQL += .TYOUSI_SEITONO & "','"
                    STR_SQL += Format(intTuki, "00") & "','"      '月（２桁表示）
                    STR_SQL += .SEIKYU01 & "',"
                    STR_SQL += .KINGAKU01 & ",'"
                    STR_SQL += .SEIKYU02 & "',"
                    STR_SQL += .KINGAKU02 & ",'"
                    STR_SQL += .SEIKYU03 & "',"
                    STR_SQL += .KINGAKU03 & ",'"
                    STR_SQL += .SEIKYU04 & "',"
                    STR_SQL += .KINGAKU04 & ",'"
                    STR_SQL += .SEIKYU05 & "',"
                    STR_SQL += .KINGAKU05 & ",'"
                    STR_SQL += .SEIKYU06 & "',"
                    STR_SQL += .KINGAKU06 & ",'"
                    STR_SQL += .SEIKYU07 & "',"
                    STR_SQL += .KINGAKU07 & ",'"
                    STR_SQL += .SEIKYU08 & "',"
                    STR_SQL += .KINGAKU08 & ",'"
                    STR_SQL += .SEIKYU09 & "',"
                    STR_SQL += .KINGAKU09 & ",'"
                    STR_SQL += .SEIKYU10 & "',"
                    STR_SQL += .KINGAKU10 & ",'"
                    STR_SQL += .SEIKYU11 & "',"
                    STR_SQL += .KINGAKU11 & ",'"
                    STR_SQL += .SEIKYU12 & "',"
                    STR_SQL += .KINGAKU12 & ",'"
                    STR_SQL += .SEIKYU13 & "',"
                    STR_SQL += .KINGAKU13 & ",'"
                    STR_SQL += .SEIKYU14 & "',"
                    STR_SQL += .KINGAKU14 & ",'"
                    STR_SQL += .SEIKYU15 & "',"
                    STR_SQL += .KINGAKU15 & ",'"
                    STR_SQL += .SAKUSEI_DATE & "','"
                    STR_SQL += .KOUSIN_DATE & "','"
                    STR_SQL += .YOBI1 & "','"
                    STR_SQL += .YOBI2 & "','"
                    STR_SQL += .YOBI3 & "')"
                End With

                'データベース登録
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    Call GSUB_LOG(0, "新入生マスタ登録")
                    Call GSUB_MESSAGE_WARNING("新入生マスタ登録に失敗しました")
                    Exit Function
                End If
            Next
        Next

        'ＣＯＭＭＩＴ処理
        GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
        PFUNC_InsSinnyusei = True

    End Function

    Private Function PFUNC_PrintMeisai() As Boolean
        '===================================
        '進学データ作成明細表出力
        '===================================
        PFUNC_PrintMeisai = False

        Dim strReportPath As String '帳票のパス
        Dim strPrintSQL As String   '帳票出力条件ＳＱＬ文
        'ループ用変数

        'クリレポ変数
        'Dim CRXApplication As New CRAXDDRT.Application
        'Dim CRXReport As CRAXDDRT.Report
        'Dim CPProperty As CRAXDDRT.ConnectionProperty
        'Dim DBTable As CRAXDDRT.DatabaseTable
        'Dim CRX_FORMULA As CRAXDDRT.FormulaFieldDefinition
        'Dim strFormulaName As String

        Try
            '帳票のパスを取得
            strReportPath = STR_LST_PATH
            strReportPath += "進学データ明細表.RPT"

            If System.IO.File.Exists(strReportPath) = False Then
                Call GSUB_MESSAGE_WARNING("レポート定義ファイルが未登録です。(" & strReportPath & ")")
                Exit Function
            End If

            '出力条件設定
            strPrintSQL = "SELECT * FROM GAKMAST1,SEITOMAST2 "
            strPrintSQL += "WHERE GAKKOU_CODE_O = '" & Trim(GakkouData(6).txtGakkouCode.Text) & "' "
            strPrintSQL += "AND GAKUNEN_CODE_G = 1 "
            strPrintSQL += "AND TUKI_NO_O = 4 "
            strPrintSQL += "AND GAKKOU_CODE_G = GAKKOU_CODE_O "
            strPrintSQL += "AND YOBI1_O IS NOT NULL " '予備欄１（旧学校コード）が未入力でない（進学生徒）場合
            strPrintSQL += "ORDER BY TUUBAN_O"

            'クリレポ出力設定
            'CRXReport = CRXApplication.OpenReport(strReportPath, 1)
            'DBTable = CRXReport.Database.Tables(1)
            'CPProperty = DBTable.ConnectionProperties("Password")
            'CPProperty.Value = "KZFMAST"
            'CRXReport.SQLQueryString = strPrintSQL

            ''学校１～５の情報を付与
            'For i As Integer = 1 To CRXReport.FormulaFields.Count
            '    CRX_FORMULA = CRXReport.FormulaFields.Item(i)
            '    strFormulaName = CRX_FORMULA.FormulaFieldName

            '    Select Case strFormulaName
            '        Case "学校名１"
            '            CRX_FORMULA.Text = "'" & GakkouData(1).labGakkouName.Text & "'"
            '        Case "学校コード１"
            '            CRX_FORMULA.Text = "'" & GakkouData(1).txtGakkouCode.Text & "'"
            '        Case "学校名２"
            '            CRX_FORMULA.Text = "'" & GakkouData(2).labGakkouName.Text & "'"
            '        Case "学校コード２"
            '            CRX_FORMULA.Text = "'" & GakkouData(2).txtGakkouCode.Text & "'"
            '        Case "学校名３"
            '            CRX_FORMULA.Text = "'" & GakkouData(3).labGakkouName.Text & "'"
            '        Case "学校コード３"
            '            CRX_FORMULA.Text = "'" & GakkouData(3).txtGakkouCode.Text & "'"
            '        Case "学校名４"
            '            CRX_FORMULA.Text = "'" & GakkouData(4).labGakkouName.Text & "'"
            '        Case "学校コード４"
            '            CRX_FORMULA.Text = "'" & GakkouData(4).txtGakkouCode.Text & "'"
            '        Case "学校名５"
            '            CRX_FORMULA.Text = "'" & GakkouData(5).labGakkouName.Text & "'"
            '        Case "学校コード５"
            '            CRX_FORMULA.Text = "'" & GakkouData(5).txtGakkouCode.Text & "'"
            '    End Select
            'Next

            ''帳票出力
            'CRXReport.PrintOut(False, 1)
            Call ChDir(strDIR)

        Catch ex As Exception
            Call GSUB_LOG(0, "進学データ作成明細表作成")
            Call GSUB_MESSAGE_WARNING("進学データ作成明細表の作成に失敗しました" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

        PFUNC_PrintMeisai = True

    End Function

    Private Function PFUNC_CSVFileWrite() As Boolean
        '===================================
        'ＣＳＶファイル出力
        '===================================
        PFUNC_CSVFileWrite = False

        Dim strCSVOutFile As String     '出力ＣＳＶファイル名
        Dim fnbr As Integer             'ファイル番号
        Dim strCSV_NName As String      'ＣＳＶ出力用生徒名漢字
        Dim strCSV_OldClass As String   'ＣＳＶ出力用旧クラス
        Dim strCSV_OldSeitoNo As String 'ＣＳＶ出力用旧生徒番号

        Try
            'ファイル保存のダイアログボックスの設定
            SaveFileDialog1.InitialDirectory = STR_CSV_PATH
            SaveFileDialog1.Filter = "csvﾌｧｲﾙ(*.csv)|*.csv|全てのﾌｧｲﾙ(*.*)|*.*"
            SaveFileDialog1.FilterIndex = 1
            SaveFileDialog1.FileName = "CLASS_SINGAKU" & GakkouData(6).txtGakkouCode.Text & ".csv"

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                strCSVOutFile = SaveFileDialog1.FileName
            Else
                Call ChDir(strDIR)
                PFUNC_CSVFileWrite = True 'キャンセル時も処理完了とみなす
                Exit Function
            End If

            'ＣＳＶファイルのオープン
            fnbr = FreeFile()
            FileOpen(fnbr, strCSVOutFile, OpenMode.Output)

            'タイトル出力
            WriteLine(fnbr, "削除フラグ", "仮ＩＤ（クラス）", "仮ＩＤ（生徒番号）", "学校コード", "年度", "通番", "生徒名カナ", "生徒名", "学年", "旧学校名", "旧クラス", "旧生徒番号", "新クラス", "新生徒番号")

            '進学生徒データの読込み（新入生は除く）
            STR_SQL = ""
            'STR_SQL += "SELECT * FROM KZFMAST.SEITOMAST2 "     '2008/03/10 旧学校名を取得追加
            STR_SQL += "SELECT SEITOMAST2.* , GAKKOU_NNAME_G FROM KZFMAST.SEITOMAST2 , GAKMAST1 "
            STR_SQL += "WHERE GAKKOU_CODE_O   = " & "'" & GakkouData(6).txtGakkouCode.Text & "' "
            STR_SQL += "and YOBI1_O = GAKKOU_CODE_G "   '2008/03/10 条件追加　 
            STR_SQL += "AND TUKI_NO_O  ='04' "
            STR_SQL += "AND YOBI1_O IS NOT NULL " '予備欄１（旧学校コード）が未入力でない（進学生徒）場合
            STR_SQL += "AND YOBI2_O IS NOT NULL " '予備欄２
            STR_SQL += "AND YOBI3_O IS NOT NULL " '予備欄３
            STR_SQL += "ORDER BY TUUBAN_O"

            OBJ_COMMAND.Connection = OBJ_CONNECTION
            OBJ_COMMAND.CommandText = STR_SQL
            OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

            While (OBJ_DATAREADER.Read = True)
                With OBJ_DATAREADER
                    If IsDBNull(.Item("SEITO_NNAME_O")) = False Then
                        strCSV_NName = Trim(.Item("SEITO_NNAME_O"))
                    Else
                        strCSV_NName = ""
                    End If

                    '予備３から旧クラスコード・旧生徒番号取得（クラスコード２桁対応）
                    If Trim(OBJ_DATAREADER.Item("YOBI3_O")).Length = 8 Then
                        strCSV_OldClass = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(0, 1)
                        strCSV_OldSeitoNo = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(1, 7)
                    Else
                        strCSV_OldClass = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(0, 2)
                        strCSV_OldSeitoNo = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(2, 7)
                    End If

                    '進学データ出力 
                    WriteLine(fnbr, "0", .Item("CLASS_CODE_O"), _
                        Trim(.Item("SEITO_NO_O")), _
                        Trim(.Item("GAKKOU_CODE_O")), _
                        Trim(.Item("NENDO_O")), _
                        .Item("TUUBAN_O"), _
                        Trim(.Item("SEITO_KNAME_O")), _
                        strCSV_NName, _
                        "1", _
                        Trim(.Item("GAKKOU_NNAME_G")), _
                        strCSV_OldClass, _
                        strCSV_OldSeitoNo, " ", " ")

                End With
            End While

            OBJ_DATAREADER.Close()

            'ＣＳＶファイルクローズ
            FileClose(fnbr)
            GSUB_LOG_OUT("", "", "進学データ作成", "移出", "成功", "")

        Catch ex As Exception
            Call GSUB_LOG(0, "ＣＳＶファイル出力")
            Call GSUB_MESSAGE_WARNING("ＣＳＶファイルの作成に失敗しました" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

        Call ChDir(strDIR)
        PFUNC_CSVFileWrite = True

    End Function

    Private Function PFUNC_NullCheck(ByVal objData As Object) As Integer
        '===========================================
        '数値型のデータベース項目のＮｕｌｌチェック
        '===========================================
        Try
            'Ｎｕｌｌの場合は０を返す
            If objData.GetType.Name = "DBNull" Then
                PFUNC_NullCheck = 0
            Else
                PFUNC_NullCheck = objData
            End If
        Catch ex As Exception
            Call GSUB_MESSAGE_WARNING("データのＮｕｌｌチェックに失敗しました" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

    End Function

    Private Function PFUNC_NullCheckST(ByVal objData As Object) As String
        '===========================================
        '数値型のデータベース項目のＮｕｌｌチェック
        '===========================================
        Try
            'Ｎｕｌｌの場合は空白を返す
            If objData.GetType.Name = "DBNull" Then
                PFUNC_NullCheckST = ""
            Else
                PFUNC_NullCheckST = objData
            End If
        Catch ex As Exception
            Call GSUB_MESSAGE_WARNING("データのＮｕｌｌチェックに失敗しました" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Return ""
        End Try

    End Function

#End Region

#Region " Private Sub "
    '****************************
    'Private Function
    '****************************
    Private Sub PSUB_ComboNameSet(ByVal intIndex As Integer)
        '===================================
        'コンボボックスに学校名をセットする
        '===================================
        If GakkouData(intIndex).cmbKana.Text = "" Then
            Exit Sub
        End If

        '学校検索
        Call GFUNC_DB_COMBO_SET(GakkouData(intIndex).cmbKana, GakkouData(intIndex).cmbGakkouName)

    End Sub

    Private Sub PSUB_TextNameSet(ByVal intIndex As Integer)
        '=====================================
        'テキストボックスに学校名をセットする
        '=====================================
        If GakkouData(intIndex).cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        GakkouData(intIndex).txtGakkouCode.Text = _
            GakkouData(intIndex).strGakkouCode(GakkouData(intIndex).cmbGakkouName.SelectedIndex())

        '学校コードにカーソル設定
        GakkouData(intIndex).labGakkouName.Text = GakkouData(intIndex).cmbGakkouName.Text
        GakkouData(intIndex).txtGakkouCode.Focus()

    End Sub

    Private Sub PSUB_StructureSet()
        '=====================================
        '構造体に各フォームを設定する
        '=====================================
        GakkouData(1).cmbKana = cmbKana1
        GakkouData(1).cmbGakkouName = cmbGakkouName1
        GakkouData(1).txtGakkouCode = txtGakkouCode1
        GakkouData(1).labGakkouName = labGakkouName1

        GakkouData(2).cmbKana = cmbKana2
        GakkouData(2).cmbGakkouName = cmbGakkouName2
        GakkouData(2).txtGakkouCode = txtGakkouCode2
        GakkouData(2).labGakkouName = labGakkouName2

        GakkouData(3).cmbKana = cmbKana3
        GakkouData(3).cmbGakkouName = cmbGakkouName3
        GakkouData(3).txtGakkouCode = txtGakkouCode3
        GakkouData(3).labGakkouName = labGakkouName3

        GakkouData(4).cmbKana = cmbKana4
        GakkouData(4).cmbGakkouName = cmbGakkouName4
        GakkouData(4).txtGakkouCode = txtGakkouCode4
        GakkouData(4).labGakkouName = labGakkouName4

        GakkouData(5).cmbKana = cmbKana5
        GakkouData(5).cmbGakkouName = cmbGakkouName5
        GakkouData(5).txtGakkouCode = txtGakkouCode5
        GakkouData(5).labGakkouName = labGakkouName5

        GakkouData(6).cmbKana = cmbKana6
        GakkouData(6).cmbGakkouName = cmbGakkouName6
        GakkouData(6).txtGakkouCode = txtGakkouCode6
        GakkouData(6).labGakkouName = labGakkouName6

    End Sub

#End Region

    Private Sub txtGakkouCode1_Validating(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    txtGakkouCode1.Validating, _
    txtGakkouCode2.Validating, _
    txtGakkouCode3.Validating, _
    txtGakkouCode4.Validating, _
    txtGakkouCode5.Validating, _
    txtGakkouCode6.Validating

        '学校コード
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then

                Select Case .Name
                    Case "txtGakkouCode1"
                        '学校名の取得
                        If PFUNC_GakNameGet(1) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode2"
                        '学校名の取得
                        If PFUNC_GakNameGet(2) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode3"
                        '学校名の取得
                        If PFUNC_GakNameGet(3) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode4"
                        '学校名の取得
                        If PFUNC_GakNameGet(4) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode5"
                        '学校名の取得
                        If PFUNC_GakNameGet(5) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode6"
                        '学校名の取得
                        If PFUNC_GakNameGet(6) <> 0 Then
                            Exit Sub
                        End If
                End Select
            Else
                Select Case .Name
                    Case "txtGakkouCode1"
                        labGakkouName1.Text = ""
                    Case "txtGakkouCode2"
                        labGakkouName2.Text = ""
                    Case "txtGakkouCode3"
                        labGakkouName3.Text = ""
                    Case "txtGakkouCode4"
                        labGakkouName4.Text = ""
                    Case "txtGakkouCode5"
                        labGakkouName5.Text = ""
                    Case "txtGakkouCode6"
                        labGakkouName6.Text = ""
                End Select
            End If
        End With
    End Sub

#Region " SELECT edIndexChanged(ComboBox) "
    '****************************
    'SelectedIndexChanged
    '****************************
    Private Sub cmbKana1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana1.SelectedIndexChanged
        PSUB_ComboNameSet(1)
        GakkouData(1).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName1.SelectedIndexChanged
        PSUB_TextNameSet(1)
    End Sub

    Private Sub cmbKana2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana2.SelectedIndexChanged
        PSUB_ComboNameSet(2)
        GakkouData(2).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName2.SelectedIndexChanged
        PSUB_TextNameSet(2)
    End Sub

    Private Sub cmbKana3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana3.SelectedIndexChanged
        PSUB_ComboNameSet(3)
        GakkouData(3).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName3.SelectedIndexChanged
        PSUB_TextNameSet(3)
    End Sub

    Private Sub cmbKana4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana4.SelectedIndexChanged
        PSUB_ComboNameSet(4)
        GakkouData(4).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName4.SelectedIndexChanged
        PSUB_TextNameSet(4)
    End Sub

    Private Sub cmbKana5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana5.SelectedIndexChanged
        PSUB_ComboNameSet(5)
        GakkouData(5).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName5.SelectedIndexChanged
        PSUB_TextNameSet(5)
    End Sub

    Private Sub cmbKana6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana6.SelectedIndexChanged
        PSUB_ComboNameSet(6)
        GakkouData(6).strGakkouCode = STR_GCOAD.Clone '学校コード取得用配列の再定義
    End Sub

    Private Sub cmbGakkouName6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName6.SelectedIndexChanged
        PSUB_TextNameSet(6)
    End Sub

#End Region

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        '==========================
        '実行ボタン
        '==========================

        '入力チェック
        If PFUNC_CommonCheck() = False Then
            Exit Sub
        End If

        '新入生マスタ取得
        If PFUNC_SEITOMAST2_Check() = False Then
            Exit Sub
        End If

        If MessageBox.Show("再印刷しますか？", STR_SYORI_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        End If

        '処理はしない
        ''進学生徒情報取得
        'If PFUNC_SEITOMAST_Check() = False Then
        '    Exit Sub
        'End If

        ''進学生徒登録処理
        'If PFUNC_InsSinnyusei() = False Then
        '    Exit Sub
        'End If

        '帳票印刷処理
        If PFUNC_PrintMeisai() = False Then
            Exit Sub
        End If

        ''2008/03/09 仕様変更によりコメント
        ''ＣＳＶファイル出力
        'If PFUNC_CSVFileWrite() = False Then
        '    Exit Sub
        'End If

        GSUB_LOG_OUT(GakkouData(6).txtGakkouCode.Text, "", "進学データ作成", "帳票再印刷処理", "成功", "")  '2008/03/23 処理成功ログ追加
        GSUB_MESSAGE_INFOMATION("再印刷しました")

    End Sub
End Class
