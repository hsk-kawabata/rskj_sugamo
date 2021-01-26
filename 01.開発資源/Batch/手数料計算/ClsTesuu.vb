Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports System.Data
Imports CASTCommon.ModPublic
Imports CASTCommon
' 手数料計算処理
Public Class ClsTesuu
    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    Friend Structure INI_PARAM
        Dim LOG_FOLDER As String
        Dim ZEI_RITU As String
        Dim EXE_FOLDER As String
        Dim LST_FOLDER As String
        Dim TXT_FOLDER As String
        Dim KINKO_CODE As String
        Dim CENTER_CODE As String
        '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        Dim ZEIKIJUN As String
        '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
    End Structure
    Private strcINI_PARAM As INI_PARAM

    '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

    '2013/11/13 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
    'Private Const strTESUU_TABLE_FILE_NAME As String = "KFJMAST010_振込手数料基準ID.TXT"
    '2013/11/13 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
    '手数料計算用
    Structure TESUU_TABLE
        Dim strKIJYUN_ID_CODE As String         '手数料基準ID
        Dim strKIJYUN_ID_TEXT As String         '手数料基準ID名
        Dim lng10000UNDER_JITEN As Long         '手数料（自店１万円未満）
        Dim lng30000UNDER_JITEN As Long         '手数料（自店１万以上３万円未満）
        Dim lng30000OVER_JITEN As Long          '手数料（自店３万円以上）
        Dim lng10000UNDER_HONSITEN As Long      '手数料（本支店１万円未満）
        Dim lng30000UNDER_HONSITEN As Long      '手数料（本支店１万以上３万円未満）
        Dim lng30000OVER_HONSITEN As Long       '手数料（本支店３万円以上）
        Dim lng10000UNDER_TAKOU As Long         '手数料（他行１万未満）
        Dim lng30000UNDER_TAKOU As Long         '手数料（他行１万以上３万円未満）
        Dim lng30000OVER_TAKOU As Long          '手数料（他行３万円以上）

        Public Sub Init()
            strKIJYUN_ID_CODE = String.Empty
            strKIJYUN_ID_TEXT = String.Empty
            lng10000UNDER_JITEN = 0
            lng30000UNDER_JITEN = 0
            lng30000OVER_JITEN = 0
            lng10000UNDER_HONSITEN = 0
            lng30000UNDER_HONSITEN = 0
            lng30000OVER_HONSITEN = 0
            lng10000UNDER_TAKOU = 0
            lng30000UNDER_TAKOU = 0
            lng30000OVER_TAKOU = 0
        End Sub
    End Structure
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
    ' 手数料テーブルの件数を100件までに変更
    'Public TESUU_TABLE_DATA(10) As TESUU_TABLE
    Public TESUU_TABLE_DATA(100) As TESUU_TABLE
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

    Private Const msgTitle As String = "手数料計算処理(KFJ070)"

    ' 機能　 ： 手数料計算 メイン処理
    '
    ' 引数　 ： ARG1 - 振替日
    '           ARG2 - 再計算フラグ
    '           ARG3 - ジョブ通番
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal furiDate As String, _
                         ByVal reCalcFlag As Integer, _
                         ByVal tuuban As Integer) As Integer


        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 60
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 60
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        Try

            LOG = New CASTCommon.BatchLOG("KFJ070", "手数料計算")

            LOG.JobTuuban = tuuban
            LOG.FuriDate = furiDate

            LOG.Write("手数料計算処理開始", "成功", "振替日：" & furiDate & " 計算フラグ：" & reCalcFlag.ToString)

            'INIファイル読み込み
            If ReadIni() = False Then
                Return 1
            End If

            '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            Me.TAX = New CASTCommon.ClsTAX
            '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            '2013/11/13 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            'ここでは読み込みを行わない。
            ''手数料テーブルファイルの読込
            'Dim strTESUU_TABLE_FILE As String = ""
            'Dim intFILE_NO As Integer = 0

            'strTESUU_TABLE_FILE = Path.Combine(strcINI_PARAM.TXT_FOLDER, strTESUU_TABLE_FILE_NAME)
            'intFILE_NO = FreeFile()
            ''読取アクセスで開く
            'FileOpen(intFILE_NO, strTESUU_TABLE_FILE, OpenMode.Input)

            'Dim strKIJYUN_ID_CODE As String = ""
            'Dim strKIJYUN_ID_TEXT As String = ""
            'Dim intIndex As Integer = 0

            'Do Until EOF(intFILE_NO)
            '    Input(intFILE_NO, strKIJYUN_ID_CODE)
            '    If strKIJYUN_ID_CODE = "" Then
            '        Exit Do
            '    End If
            '    Input(intFILE_NO, strKIJYUN_ID_TEXT)
            '    intIndex = CInt(strKIJYUN_ID_CODE)
            '    If intIndex = -1 Then
            '    Else
            '        If intIndex > 0 And intIndex < 10 Then
            '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_CODE = strKIJYUN_ID_CODE
            '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_TEXT = strKIJYUN_ID_TEXT
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_JITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_JITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_JITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_HONSITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_HONSITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_HONSITEN)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_TAKOU)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_TAKOU)
            '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_TAKOU)
            '        End If
            '    End If
            'Loop
            'FileClose(intFILE_NO)
            '2013/11/13 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

            MainDB = New CASTCommon.MyOracle

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                LOG.Write_Err("手数料計算処理", "失敗", "手数料計算処理で実行待ちタイムアウト")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyErr("手数料計算処理で実行待ちタイムアウト")
                End If
                Return 1
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '再計算フラグが有効なら、手数料再計算する
            If reCalcFlag = 1 Then
                Call KekkaReBorn(furiDate)
            End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            If CalcTesuu(furiDate, reCalcFlag) = 0 Then
                MainDB.Commit()
                LOG.Write("手数料計算処理終了", "成功")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyOK("")
                End If
            Else
                MainDB.Rollback()
                LOG.Write("手数料計算処理終了", "失敗")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyErr("手数料計算 失敗")
                End If
                Return 1
            End If

            Return 0

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "手数料計算処理", "失敗", ex.Message)
            Return 1

        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            If Not MainDB Is Nothing Then 
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                ' ロールバック
                MainDB.Rollback()
                ' DBクローズ
                MainDB.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        End Try
    End Function

    ' 機能　 ： 手数料計算処理
    '
    ' 引数　 ： ARG1 - 振替日
    '           ARG2 - 再計算フラグ ０：通常，１：再計算
    '           ARG3 - ジョブ通番
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function CalcTesuu(ByVal furiDate As String, ByVal reCalcFlag As Integer) As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        ' 自金庫コード
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        '2013/11/13 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
        '' 消費税
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/13 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

        '2013/11/13 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
        '型変換（Decimal→Long）
        Dim TesuuKin1 As Long = 0                   ' 手数料金額１
        Dim TesuuKin2 As Long = 0                   ' 手数料金額２
        Dim TesuuKin3 As Long = 0                   ' 手数料金額３
        'Dim TesuuKin1 As Decimal = 0                    ' 手数料金額１
        'Dim TesuuKin2 As Decimal = 0                    ' 手数料金額２
        'Dim TesuuKin3 As Decimal = 0                    ' 手数料金額３
        '2013/11/13 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<

        Dim JifuriKin As Decimal = 0                    ' 自振金額

        Dim nSyoriKen As Integer = 0                    ' 処理件数

        Dim intKIJYUN_ID As Integer = 0                 '振込手数料基準ＩＤ

        Try
            ' スケジュールマスタ選択
            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_SV1")               '取引先主コード
            SQL.Append(", TORIF_CODE_SV1")              '取引先副コード
            SQL.Append(", FURI_DATE_SV1")               '振替日
            SQL.Append(", TESUUTYO_KBN_T")                   ' 手数料徴求区分
            '2013/11/13 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
            'SQLでの引落手数料の計算は行わず、計算に必要な項目の取得に変更する。
            SQL.Append(", SYORI_KEN_SV1")               '請求件数
            SQL.Append(", FURI_KEN_SV1")                '振替件数
            'SQL.Append(", TRUNC(")
            ''***** 2009/10/22 kakiwaki *****
            ''   SQL.Append(" ((KIHTESUU_TV1 * DECODE(SEIKYU_KBN_TV1,'0',SYORI_KEN_SV1,FURI_KEN_SV1) / 100)")
            'SQL.Append(" ((KIHTESUU_TV1 * DECODE(SEIKYU_KBN_TV1,'0',SYORI_KEN_SV1,FURI_KEN_SV1))")
            ''*** 2008/05/21 nishida 修正：案件No.418 固定手数料1/固定手数料2　=Nullの場合の対応 START
            ''SQL.Append("   + KOTEI_TESUU1_T")
            ''SQL.Append("   + KOTEI_TESUU2_T)")
            'SQL.Append(" + NVL(KOTEI_TESUU1_T,0)")
            'SQL.Append(" + NVL(KOTEI_TESUU2_T,0))")
            ''*** 2008/05/21 nishida 修正：案件No.418 固定手数料1/固定手数料2　=Nullの場合の対応 END
            'SQL.Append(" * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' 引落手数料
            '2013/11/13 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
            SQL.Append(", SOURYO_TV1")                       ' 送料
            SQL.Append(", FURI_KIN_SV1")                     ' 振替済み金額
            'SQL.Append(", TESUU1_TV1")                       ' 基準別手数料１
            'SQL.Append(", TESUU2_TV1")                       ' 基準別手数料２
            'SQL.Append(", TESUU3_TV1")                       ' 基準別手数料３
            'SQL.Append(", TESUU4_TV1")                       ' 基準別手数料４
            'SQL.Append(", TESUU5_TV1")                       ' 基準別手数料５
            'SQL.Append(", TESUU6_T TESUU6_TV1")              ' 基準別手数料６
            SQL.Append(", TUKEKIN_NO_TV1")                   ' 決済金融機関
            SQL.Append(", TUKESIT_NO_TV1")                   ' 決済支店
            'SQL.Append(", TORIMATOME_SIT_NO_T")              ' とりまとめ店
            SQL.Append(", TORIMATOME_SIT_T")                 ' とりまとめ店
            '*** 修正 mitsu 2009/05/28 合計件数に対する手数料対応 ***
            SQL.Append(", TESUUMAT_PATN_T")                  ' 手数料集計方法
            SQL.Append(", SYOUHI_KBN_T")                     ' 消費税区分
            SQL.Append(", KIHTESUU_TV1")                     ' 振替手数料単価
            SQL.Append(", SEIKYU_KBN_TV1")                   ' 請求引落区分
            SQL.Append(", TESUU_YDATE_SV1")                  ' 手数料徴求予定日
            SQL.Append(", NVL(KOTEI_TESUU1_T,0) KOTEI_TESUU1_T") ' 固定手数料１
            SQL.Append(", NVL(KOTEI_TESUU2_T,0) KOTEI_TESUU2_T") ' 固定手数料２
            SQL.Append(", TESUU_TABLE_ID_T")                '振込手数料基準ＩＤ
            '********************************************************
            SQL.Append(", BAITAI_CODE_T")   '2010/03/25 媒体コード追加
            '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            SQL.Append(", KESSAI_YDATE_SV1")
            '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            SQL.Append(" FROM V1_KESMAST, TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_SV1 = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_SV1 = TORIF_CODE_T")
            SQL.Append(" AND FSYORI_KBN_SV1 = FSYORI_KBN_T")
            SQL.Append(" AND FURI_DATE_SV1  = " & SQ(furiDate))       ' 振替日
            SQL.Append(" AND TESUUKEI_FLG_SV1 = " & SQ(reCalcFlag))   ' 手数料計算済フラグ 未処理
            SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")     ' 手数料徴求済フラグ 未処理
            SQL.Append(" AND FUNOU_FLG_SV1 = '1'")        ' 不能済フラグ 処理済み
            SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")      ' 中断フラグ
            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False
                    nSyoriKen += 1

                    '*** 修正 mitsu 2009/05/28 処理高速化 ***
                    'SQL = New StringBuilder(128)
                    SQL.Length = 0
                    '****************************************

                    '*** 修正 mitsu 2009/05/28 合計件数に対する手数料合計 ***
                    If OraReader.GetString("TESUUMAT_PATN_T") = "1" Then
                        '2013/11/13 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
                        '一時的にコメントアウト（要仕様検討）
                        'CalcGoukeiTesuu(OraReader, Jikinko, sTax)
                        '2013/11/13 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
                    Else
                        '****************************************************
                        If OraReader.GetString("TESUUTYO_KBN_T") <> "2" Then
                            ' 手数料徴求区分が特別免除以外の場合

                            '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                            Dim strKijunDate As String = String.Empty
                            If strcINI_PARAM.ZEIKIJUN.Equals("0") = True Then
                                '振替日基準
                                strKijunDate = OraReader.GetString("FURI_DATE_SV1")
                            Else
                                '決済日基準
                                strKijunDate = OraReader.GetString("KESSAI_YDATE_SV1")
                            End If

                            '税率取得
                            Me.TAX.GetZeiritsu(strKijunDate)
                            If Me.TAX.ZEIRITSU.Equals("err") = True Then
                                LOG.Write("税率取得", "失敗", "基準日：" & strKijunDate)
                                Return 1
                            End If

                            '振込手数料マスタ読み込み
                            If Me.GetJifuriTesuuTable(Me.TAX.ZEIRITSU_ID) = False Then
                                LOG.Write("振込手数料マスタ読込", "失敗", "基準日：" & strKijunDate)
                                Return 1
                            End If
                            '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                            '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
                            '印紙税取得
                            Me.TAX.GetInshizei(strKijunDate)
                            If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                                LOG.Write("印紙税取得", "失敗", "基準日：" & strKijunDate)
                                Return 1
                            End If
                            '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

                            '2013/11/13 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                            '引落手数料は別途計算する
                            If Me.CalcTesuuKin1(TesuuKin1, OraReader) = False Then
                                Return 1
                            End If
                            'TesuuKin1 = OraReader.GetInt64("TESUU_KIN1")        ' 引落手数料
                            '2013/11/13 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
                            TesuuKin2 = OraReader.GetInt64("SOURYO_TV1")        ' 送料
                            JifuriKin = OraReader.GetInt64("FURI_KIN_SV1") - (TesuuKin1 + TesuuKin2)
                            TesuuKin3 = 0

                            '振込手数料基準ＩＤを設定
                            If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                                intKIJYUN_ID = -1
                            Else
                                intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                                If Me.TESUU_TABLE_DATA(intKIJYUN_ID).strKIJYUN_ID_CODE = String.Empty Then
                                    LOG.Write("振込手数料マスタ取得", "失敗", "基準日：" & strKijunDate & " 手数料ID：" & intKIJYUN_ID.ToString)
                                    Return 1
                                End If
                            End If

                            '2014/03/04 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                            If intKIJYUN_ID = -1 Then
                                TesuuKin3 = 0
                            Else
                                If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                                    ' 決済金融機関が，自金庫の場合
                                    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                                        ' 決済支店がとりまとめ店と一致する場合，自店内
                                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                                        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                                        End If
                                    Else
                                        ' 決済支店がとりまとめ店と一致しない場合，本支店
                                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                                        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                                        End If
                                    End If
                                Else
                                    ' 他行
                                    If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                                    ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                                    ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                                    End If
                                End If
                            End If

                            ''2013/12/27 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                            'If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            '    ' 決済金融機関が，自金庫の場合
                            '    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            '        ' 決済支店がとりまとめ店と一致する場合，自店内
                            '        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '            '    ０円より大きい かつ ○円未満
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                            '        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '            '    ○円以上 かつ △円未満
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                            '        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '            '    △円以上
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                            '        End If
                            '    Else
                            '        ' 決済支店がとりまとめ店と一致しない場合，本支店
                            '        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '            '    ０円より大きい かつ ○円未満
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                            '        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '            '    ○円以上 かつ △円未満
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                            '        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '            '    △円以上
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                            '        End If
                            '    End If
                            'Else
                            '    ' 他行
                            '    If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '        '    ０円より大きい かつ ○円未満
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                            '    ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '        '    ○円以上 かつ △円未満
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                            '    ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '        '    △円以上
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                            '    End If
                            'End If

                            ''If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            ''    ' 決済金融機関が，自金庫の場合
                            ''    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            ''        ' 決済支店がとりまとめ店と一致する場合，自店内
                            ''        If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''            '    ０円より大きい かつ １万円未満
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                            ''        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''            '    １万円以上 かつ ３万円未満
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                            ''        ElseIf 30000 <= JifuriKin Then
                            ''            '    ３万円以上
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                            ''        End If
                            ''    Else
                            ''        ' 決済支店がとりまとめ店と一致しない場合，本支店
                            ''        If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''            '    ０円より大きい かつ １万円未満
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                            ''        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''            '    １万円以上 かつ ３万円未満
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                            ''        ElseIf 30000 <= JifuriKin Then
                            ''            '    ３万円以上
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                            ''        End If
                            ''    End If
                            ''Else
                            ''    ' 他行
                            ''    If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''        '    ０円より大きい かつ １万円未満
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                            ''    ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''        '    １万円以上 かつ ３万円未満
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                            ''    ElseIf 30000 <= JifuriKin Then
                            ''        '    ３万円以上
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                            ''    End If
                            ''End If
                            ''2013/12/27 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                            '2014/03/04 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<

                        Else
                            ' 免除
                            '*** 修正　nishida 2008/05/20
                            TesuuKin1 = 0
                            TesuuKin2 = 0
                            '*** 修正　nishida 2008/05/20
                            TesuuKin3 = 0
                        End If

                        ' スケジュールマスタ更新
                        SQL.Append("UPDATE SCHMAST SET")
                        SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                        SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                        SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                        SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                        SQL.Append(", TESUUKEI_FLG_S = '1'")
                        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_SV1")))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(OraReader.GetString("TORIF_CODE_SV1")))
                        SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_SV1")))
                        Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                        LOG.ToriCode = OraReader.GetString("TORIS_CODE_SV1") & OraReader.GetString("TORIF_CODE_SV1")

                        '2010/03/25 学校スケジュールマスタ更新
                        If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                            SQL = New StringBuilder
                            SQL.Append("UPDATE G_SCHMAST SET")
                            SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                            SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                            SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                            SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_SV1")))
                            Select Case OraReader.GetString("TORIF_CODE_SV1").Trim
                                Case "01"
                                    SQL.Append(" AND FURI_KBN_S = '0'")
                                Case "02"
                                    SQL.Append(" AND FURI_KBN_S = '1'")
                                Case "03"
                                    SQL.Append(" AND FURI_KBN_S = '2'")
                                Case "04"
                                    SQL.Append(" AND FURI_KBN_S = '3'")
                                Case Else
                                    SQL.Append(" AND FURI_KBN_S = '0'")
                            End Select
                            SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_SV1")))
                            nRet = MainDB.ExecuteNonQuery(SQL)
                        End If
                        '=====================================
                        '*** 修正 mitsu 2009/05/28 合計件数に対する手数料合計 ***
                    End If
                    '************************************************************

                    OraReader.NextRead()
                Loop
            End If
            OraReader.Close()

            LOG.Write("手数料計算", "成功", "件数：" & nSyoriKen.ToString)

        Catch ex As Exception
            '*** 修正 mitsu 2009/07/29 障害対応 パトライト点灯 ***
            Try
                Dim ELog As New CASTCommon.ClsEventLOG
                If LOG.ToriCode Is Nothing OrElse LOG.ToriCode = "" Then
                    ELog.Write("手数料計算に異常発生：" & ex.Message, EventLogEntryType.Error)
                Else
                    ELog.Write("手数料計算に異常発生：取引先コード：" & LOG.ToriCode & " " & ex.Message, EventLogEntryType.Error)
                End If
            Catch
            End Try
            '*****************************************************
            LOG.Write("手数料計算", "失敗", ex.Message)


            OraReader.Close()

            Return 1

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        End Try

        Return 0
    End Function

#Region "合計手数料に対する手数料計算"
    '*** 修正 mitsu 2009/05/28 合計件数に対する手数料対応 ***
    Private Function CalcGoukeiTesuu(ByRef OraReader As CASTCommon.MyOracleReader, ByVal Jikinko As String, ByVal sTax As String) As Integer
        Dim SchReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder = New StringBuilder(128)
        Dim nRet As Integer = 0

        SchReader = New CASTCommon.MyOracleReader

        Try
            '取引先マスタ情報取得
            Dim TorisCode As String = OraReader.GetString("TORIS_CODE_SV1")
            Dim TorifCode As String = OraReader.GetString("TORIF_CODE_SV1")
            Dim FuriDate As String = OraReader.GetString("FURI_DATE_SV1")
            Dim SyouhiKbn As String = OraReader.GetString("SYOUHI_KBN_T")
            Dim KihonTesuu As Integer = OraReader.GetInt("KIHTESUU_TV1")
            Dim SeikyuKbn As String = OraReader.GetString("SEIKYU_KBN_TV1")
            Dim TesuuYdate As String = OraReader.GetString("TESUU_YDATE_SV1")
            Dim KoteiTesuu1 As Integer = OraReader.GetInt("KOTEI_TESUU1_T")
            Dim KoteiTesuu2 As Integer = OraReader.GetInt("KOTEI_TESUU2_T")

            '手数料計算用
            Dim TesuuKin1 As Long = 0
            Dim TesuuKin2 As Integer = OraReader.GetInt("SOURYO_TV1")
            Dim TesuuKin3 As Integer = 0
            Dim TesuuKen As Long = 0            '基本手数料対象件数
            Dim SchKen As Integer = 0           'スケジュール件数

            Dim intKIJYUN_ID As Integer = 0     '振込手数料基準ＩＤ

            If OraReader.GetString("TESUUTYO_KBN_T") <> "2" Then
                ' 手数料徴求区分が特別免除以外の場合
                SchReader = New CASTCommon.MyOracleReader(MainDB)

                '同一手数料徴求日のスケジュールの手数料をリセットする
                SQL.Append("UPDATE SCHMAST SET")
                SQL.Append(" TESUU_KIN_S = 0")
                SQL.Append(", TESUU_KIN1_S = 0")
                SQL.Append(", TESUU_KIN2_S = 0")
                SQL.Append(", TESUU_KIN3_S = 0")
                SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(TorifCode))
                SQL.Append(" AND TESUU_YDATE_S = " & SQ(TesuuYdate))
                SQL.Append(" AND TESUUTYO_FLG_S = '0'")
                SQL.Append(" AND FUNOU_FLG_S = '1'")
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                MainDB.ExecuteNonQuery(SQL)

                '各スケジュールの情報取得と振込手数料計算
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append(" FURI_DATE_SV1")
                Select Case SeikyuKbn
                    Case "0" : SQL.Append(", SYORI_KEN_SV1 TESUU_KEN")
                    Case Else : SQL.Append(", FURI_KEN_SV1 TESUU_KEN")
                End Select
                SQL.Append(", FURI_KIN_SV1")
                SQL.Append(" FROM V1_KESMAST")
                SQL.Append(" WHERE TORIS_CODE_SV1 = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_SV1 = " & SQ(TorifCode))
                SQL.Append(" AND TESUU_YDATE_SV1  = " & SQ(TesuuYdate))
                SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")
                SQL.Append(" AND FUNOU_FLG_SV1 = '1'")
                SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")
                SQL.Append(" ORDER BY FURI_DATE_SV1")

                If SchReader.DataReader(SQL) = True Then
                    While SchReader.EOF = False
                        TesuuKen += SchReader.GetInt64("TESUU_KEN")
                        SchKen += 1

                        '振込手数料を求めるための自振手数料計算
                        '       '*** 修正 mitsu 2009/06/09 小数点以下は切り捨て ***
                        '       'TesuuKin1 = CLng(KihonTesuu * SchReader.GetInt64("TESUU_KEN") / 100) + KoteiTesuu1 + KoteiTesuu2
                        '   TesuuKin1 = CLng(Math.Floor(KihonTesuu * SchReader.GetInt64("TESUU_KEN") / 100)) + KoteiTesuu1 + KoteiTesuu2
                        '       '**************************************************
                        TesuuKin1 = CLng(Math.Floor(KihonTesuu * SchReader.GetInt64("TESUU_KEN"))) + KoteiTesuu1 + KoteiTesuu2

                        Dim JifuriKin As Long = SchReader.GetInt64("FURI_KIN_SV1") - (TesuuKin1 + TesuuKin2)

                        '振込手数料基準ＩＤを設定
                        If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                            intKIJYUN_ID = 0
                        Else
                            intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                        End If

                        '振込手数料計算
                        If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            ' 決済金融機関が，自金庫の場合
                            If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                                ' 決済支店がとりまとめ店と一致する場合，自店内
                                If 0 < JifuriKin And JifuriKin < 10000 Then
                                    '    ０円より大きい かつ １万円未満
                                    TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                                ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                    '    １万円以上 かつ ３万円未満
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                                ElseIf 30000 <= JifuriKin Then
                                    ' ３万円以上
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                                End If
                            Else
                                ' 決済支店がとりまとめ店と一致しない場合，本支店
                                If 0 < JifuriKin And JifuriKin < 10000 Then
                                    '    ０円より大きい かつ １万円未満
                                    TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                                ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                    '    １万円以上 かつ ３万円未満
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                                ElseIf 30000 <= JifuriKin Then
                                    ' ３万円以上
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                                End If
                            End If
                        Else
                            ' 他行
                            If 0 < JifuriKin And JifuriKin < 10000 Then
                                '    ０円より大きい かつ １万円未満
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                            ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                ' ０円より大きい かつ ３万円未満
                                TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                            ElseIf 30000 <= JifuriKin Then
                                ' ３万円以上
                                TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                            End If
                        End If

                        SchReader.NextRead()
                    End While
                End If

                '消費税区分より
                If SyouhiKbn <> "0" Then
                    sTax = "1"
                End If

                '引落手数料計算
                '*** 修正 mitsu 2009/06/09 小数点以下は切り捨て ***
                'TesuuKin1 = CLng((KihonTesuu * TesuuKen / 100 + SchKen * (KoteiTesuu1 + KoteiTesuu2)) * Double.Parse(sTax))
                TesuuKin1 = CLng(Math.Floor((KihonTesuu * TesuuKen / 100 + SchKen * (KoteiTesuu1 + KoteiTesuu2)) * Double.Parse(sTax)))
                '**************************************************
                TesuuKin2 = SchKen * TesuuKin2
            Else
                TesuuKin1 = 0
                TesuuKin2 = 0
                TesuuKin3 = 0
            End If

            ' スケジュールマスタ更新
            SQL.Length = 0
            SQL.Append("UPDATE SCHMAST SET")
            SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
            SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
            SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
            SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
            SQL.Append(", TESUUKEI_FLG_S = '1'")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(TorifCode))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(FuriDate))
            nRet = MainDB.ExecuteNonQuery(SQL)
            LOG.ToriCode = TorisCode & TorifCode
            '2010/03/25 学校スケジュールマスタ更新
            If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                SQL = New StringBuilder
                SQL.Append("UPDATE G_SCHMAST SET")
                SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(TorisCode))
                Select Case TorifCode.Trim
                    Case "01"
                        SQL.Append(" AND FURI_KBN_S = '0'")
                    Case "02"
                        SQL.Append(" AND FURI_KBN_S = '1'")
                    Case "03"
                        SQL.Append(" AND FURI_KBN_S = '2'")
                    Case "04"
                        SQL.Append(" AND FURI_KBN_S = '3'")
                    Case Else
                        SQL.Append(" AND FURI_KBN_S = '0'")
                End Select
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
                nRet = MainDB.ExecuteNonQuery(SQL)
            End If
            '====================================
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            '*** 修正 mitsu 2009/06/09 Nothingの場合を考慮 ***
            If Not SchReader Is Nothing Then
                SchReader.Close()
            End If
            '*************************************************
        End Try

        Return nRet
    End Function
    '********************************************************
#End Region

    ' 機能　 ： スケジュールマスタ，済・不能件数金額再計算
    '
    ' 引数　 ： ARG1 - 振替日
    '
    ' 備考　 ： 
    '
    Private Sub KekkaReBorn(ByVal furiDate As String)
        Dim SQL As New StringBuilder(256)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim ToriSCode As String
        Dim ToriFCode As String
        Dim MotikomiKubun As String
        Dim UpdateFlg As Boolean = False

        Try
            LOG.Write("手数料計算", "成功", "手数料再計算(開始) 振替日:" & furiDate)

            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(", TORIF_CODE_T")
            SQL.Append(", FMT_KBN_T")
            SQL.Append(", MOTIKOMI_KBN_T")
            SQL.Append(", BAITAI_CODE_T")  '2010/03/25 媒体コード追加
            SQL.Append(" FROM SCHMAST")
            SQL.Append(", TORIMAST")
            SQL.Append(" WHERE FURI_DATE_S = '" & furiDate & "'")
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND FSYORI_KBN_T = '1'")
            SQL.Append(" AND KESSAI_FLG_S = '0'")   '2010/03/25 HENKAN_FLG_S→KESSAI_FLG_S
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    ToriSCode = OraReader.GetString("TORIS_CODE_T")
                    ToriFCode = OraReader.GetString("TORIF_CODE_T")
                    MotikomiKubun = OraReader.GetString("MOTIKOMI_KBN_T")

                    Select Case strcINI_PARAM.CENTER_CODE
                        Case "4"
                            UpdateFlg = True
                        Case Else
                            If MotikomiKubun = "0" Then
                                UpdateFlg = True
                            Else
                                UpdateFlg = False
                            End If
                    End Select

                    If UpdateFlg = True Then
                        Dim FunoKensu As Decimal = 0
                        Dim FunoKingaku As Decimal = 0
                        Dim ZumiKensu As Decimal = 0
                        Dim ZumiKingaku As Decimal = 0

                        '----------------------
                        '不能件数、金額取得
                        '----------------------
                        Dim FunoReader As New CASTCommon.MyOracleReader(MainDB)
                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT")
                        SQL.Append(" COUNT(FURIKIN_K) KEN")
                        SQL.Append(", SUM(FURIKIN_K) KIN")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = '" & furiDate & "'")
                        If OraReader.GetItem("FMT_KBN_T") = "02" Then
                            ' 国税は、データ区分が３のレコード
                            SQL.Append(" AND DATA_KBN_K = '3'")
                        Else
                            SQL.Append(" AND DATA_KBN_K = '2'")
                        End If
                        SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                        ' 2008.03.14 振替金額が０円のものは含まない
                        SQL.Append(" AND FURIKIN_K > 0")
                        SQL.Append(" AND TORIS_CODE_K = '" & ToriSCode & "'")
                        SQL.Append(" AND TORIF_CODE_K = '" & ToriFCode & "'")
                        If FunoReader.DataReader(SQL) = True Then
                            FunoKensu = FunoReader.GetInt64("KEN")
                            FunoKingaku = FunoReader.GetInt64("KIN")
                        End If
                        FunoReader.Close()

                        '----------------------
                        '振替済件数、金額取得
                        '----------------------
                        Dim ZumiReader As New CASTCommon.MyOracleReader(MainDB)
                        If ZumiReader.DataReader(SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")) = True Then
                            ZumiKensu = ZumiReader.GetInt64("KEN")
                            ZumiKingaku = ZumiReader.GetInt64("KIN")
                        End If
                        ZumiReader.Close()

                        '-------------------------------------------
                        'スケジュールマスタの更新
                        '-------------------------------------------
                        SQL = New StringBuilder(256)
                        SQL.Append("UPDATE SCHMAST SET")
                        SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                        SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                        SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                        SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                        SQL.Append(" WHERE TORIS_CODE_S = '" & ToriSCode & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & ToriFCode & "'")
                        SQL.Append(" AND FURI_DATE_S = '" & furiDate & "'")
                        Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                        LOG.ToriCode = ToriSCode & ToriFCode
                        LOG.FuriDate = furiDate
                        '2010/03/25 学校スケジュールマスタ更新
                        If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                            '特別振替日対応
                            Dim OraGakReader As CASTCommon.MyOracleReader = Nothing

                            Try
                                '--------------------------------------------------
                                '学校スケジュールマスタのレコード件数抽出
                                '--------------------------------------------------
                                Dim MultiGScheduleFlg As Boolean = True

                                With SQL
                                    .Length = 0
                                    .Append("select count(*) as COUNTER from G_SCHMAST")
                                    .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                    Select Case ToriFCode.Trim
                                        Case "01" : .Append(" and FURI_KBN_S = '0'")
                                        Case "02" : .Append(" and FURI_KBN_S = '1'")
                                        Case "03" : .Append(" and FURI_KBN_S = '2'")
                                        Case "04" : .Append(" and FURI_KBN_S = '3'")
                                        Case Else : .Append(" and FURI_KBN_S = '0'")
                                    End Select
                                    .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                End With

                                OraGakReader = New CASTCommon.MyOracleReader(MainDB)
                                If OraGakReader.DataReader(SQL) = True Then
                                    'スケジュールが1レコード
                                    If OraGakReader.GetInt("COUNTER") = 1 Then
                                        MultiGScheduleFlg = False
                                    End If
                                End If

                                OraGakReader.Close()

                                If MultiGScheduleFlg = False Then
                                    SQL = New StringBuilder
                                    SQL.Append("UPDATE G_SCHMAST SET")
                                    SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                                    SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                                    SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                                    SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                                    SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(ToriSCode))
                                    Select Case ToriFCode.Trim
                                        Case "01"
                                            SQL.Append(" AND FURI_KBN_S = '0'")
                                        Case "02"
                                            SQL.Append(" AND FURI_KBN_S = '1'")
                                        Case "03"
                                            SQL.Append(" AND FURI_KBN_S = '2'")
                                        Case "04"
                                            SQL.Append(" AND FURI_KBN_S = '3'")
                                        Case Else
                                            SQL.Append(" AND FURI_KBN_S = '0'")
                                    End Select
                                    SQL.Append(" AND FURI_DATE_S = " & SQ(furiDate))
                                    nRet = MainDB.ExecuteNonQuery(SQL)

                                Else
                                    '更新件数初期化
                                    nRet = 0

                                    '--------------------------------------------------
                                    '学校スケジュールマスタ抽出
                                    '--------------------------------------------------
                                    With SQL
                                        .Length = 0
                                        .Append("select ")
                                        .Append(" GAKUNEN1_FLG_S")
                                        .Append(",GAKUNEN2_FLG_S")
                                        .Append(",GAKUNEN3_FLG_S")
                                        .Append(",GAKUNEN4_FLG_S")
                                        .Append(",GAKUNEN5_FLG_S")
                                        .Append(",GAKUNEN6_FLG_S")
                                        .Append(",GAKUNEN7_FLG_S")
                                        .Append(",GAKUNEN8_FLG_S")
                                        .Append(",GAKUNEN9_FLG_S")
                                        .Append(" from G_SCHMAST")
                                        .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                        Select Case ToriFCode.Trim
                                            Case "01" : .Append(" and FURI_KBN_S = '0'")
                                            Case "02" : .Append(" and FURI_KBN_S = '1'")
                                            Case "03" : .Append(" and FURI_KBN_S = '2'")
                                            Case "04" : .Append(" and FURI_KBN_S = '3'")
                                            Case Else : .Append(" and FURI_KBN_S = '0'")
                                        End Select
                                        .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                    End With

                                    If OraGakReader.DataReader(SQL) = True Then
                                        While OraGakReader.EOF = False

                                            ZumiKensu = 0
                                            ZumiKingaku = 0
                                            FunoKensu = 0
                                            FunoKingaku = 0

                                            '--------------------------------------------------
                                            '学年フラグが立っている明細に対して集計を行う
                                            '--------------------------------------------------
                                            For i As Integer = 1 To 9
                                                If OraGakReader.GetString("GAKUNEN" & i.ToString & "_FLG_S") = "1" Then
                                                    With SQL
                                                        .Length = 0
                                                        .Append("select ")
                                                        .Append(" sum(decode(FURIKETU_CODE_M, 0, 1, 0)) as FURI_KEN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, SEIKYU_KIN_M)) as FURI_KIN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, 1)) as FUNO_KEN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, SEIKYU_KIN_M)) as FUNO_KIN")
                                                        .Append(" from G_MEIMAST")
                                                        .Append(" where GAKKOU_CODE_M = " & SQ(ToriSCode))
                                                        Select Case ToriFCode.Trim
                                                            Case "01" : .Append(" and FURI_KBN_M = '0'")
                                                            Case "02" : .Append(" and FURI_KBN_M = '1'")
                                                            Case "03" : .Append(" and FURI_KBN_M = '2'")
                                                            Case "04" : .Append(" and FURI_KBN_M = '3'")
                                                            Case Else : .Append(" and FURI_KBN_M = '0'")
                                                        End Select
                                                        .Append(" and FURI_DATE_M = " & SQ(furiDate))
                                                        .Append(" and GAKUNEN_CODE_M = " & i.ToString)
                                                    End With

                                                    '不能オラクルリーダー再利用
                                                    FunoReader.Close()
                                                    If FunoReader.DataReader(SQL) = True Then
                                                        ZumiKensu += FunoReader.GetInt("FURI_KEN")
                                                        ZumiKingaku += FunoReader.GetInt64("FURI_KIN")
                                                        FunoKensu += FunoReader.GetInt("FUNO_KEN")
                                                        FunoKingaku += FunoReader.GetInt64("FUNO_KIN")
                                                    End If
                                                End If
                                            Next

                                            '--------------------------------------------------
                                            '学校スケジュールマスタ更新
                                            '--------------------------------------------------
                                            With SQL
                                                .Length = 0
                                                .Append("update G_SCHMAST set")
                                                .Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                                                .Append(",FURI_KIN_S = " & ZumiKingaku.ToString)
                                                .Append(",FUNOU_KEN_S = " & FunoKensu.ToString)
                                                .Append(",FUNOU_KIN_S = " & FunoKingaku.ToString)
                                                .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                                Select Case ToriFCode.Trim
                                                    Case "01" : .Append(" and FURI_KBN_S = '0'")
                                                    Case "02" : .Append(" and FURI_KBN_S = '1'")
                                                    Case "03" : .Append(" and FURI_KBN_S = '2'")
                                                    Case "04" : .Append(" and FURI_KBN_S = '3'")
                                                    Case Else : .Append(" and FURI_KBN_S = '0'")
                                                End Select
                                                .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                                .Append(" and GAKUNEN1_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN1_FLG_S")))
                                                .Append(" and GAKUNEN2_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN2_FLG_S")))
                                                .Append(" and GAKUNEN3_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN3_FLG_S")))
                                                .Append(" and GAKUNEN4_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN4_FLG_S")))
                                                .Append(" and GAKUNEN5_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN5_FLG_S")))
                                                .Append(" and GAKUNEN6_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN6_FLG_S")))
                                                .Append(" and GAKUNEN7_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN7_FLG_S")))
                                                .Append(" and GAKUNEN8_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN8_FLG_S")))
                                                .Append(" and GAKUNEN9_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN9_FLG_S")))
                                            End With

                                            nRet += MainDB.ExecuteNonQuery(SQL)

                                            OraGakReader.NextRead()
                                        End While
                                    End If
                                End If

                            Catch ex As Exception
                                LOG.Write("手数料計算", "失敗", "手数料再計算 振替日:" & furiDate & "、err = " & ex.Message)

                            Finally
                                If Not OraGakReader Is Nothing Then
                                    OraGakReader.Close()
                                    OraGakReader = Nothing
                                End If
                            End Try

                            'SQL = New StringBuilder
                            'SQL.Append("UPDATE G_SCHMAST SET")
                            'SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                            'SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                            'SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                            'SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                            'SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(ToriSCode))
                            'Select Case ToriFCode.Trim
                            '    Case "01"
                            '        SQL.Append(" AND FURI_KBN_S = '0'")
                            '    Case "02"
                            '        SQL.Append(" AND FURI_KBN_S = '1'")
                            '    Case "03"
                            '        SQL.Append(" AND FURI_KBN_S = '2'")
                            '    Case "04"
                            '        SQL.Append(" AND FURI_KBN_S = '3'")
                            '    Case Else
                            '        SQL.Append(" AND FURI_KBN_S = '0'")
                            'End Select
                            'SQL.Append(" AND FURI_DATE_S = " & SQ(furiDate))
                            'nRet = MainDB.ExecuteNonQuery(SQL)
                            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                        End If
                        '====================================
                        LOG.Write("スケジュールマスタ 済/不能 件数金額再集計", "成功", "件数：" & nRet.ToString)

                    End If
                    OraReader.NextRead()
                End While
            Else
                LOG.Write("スケジュールマスタ 済/不能 件数金額再集計", "成功", "０件")
            End If

            OraReader.Close()
            LOG.Write("手数料計算", "成功", "手数料再計算(終了) 振替日:" & furiDate)
        Catch ex As Exception
            LOG.Write("手数料計算", "失敗", "手数料再計算 振替日:" & furiDate & "、err = " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' INIファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function ReadIni() As Boolean
        Try
            strcINI_PARAM.LOG_FOLDER = CASTCommon.GetFSKJIni("COMMON", "LOG")
            If strcINI_PARAM.LOG_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - LOG)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - LOG)")
                Return False
            End If

            strcINI_PARAM.ZEI_RITU = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
            If strcINI_PARAM.ZEI_RITU = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - ZEIRITU)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - ZEIRITU)")
                Return False
            End If

            strcINI_PARAM.EXE_FOLDER = CASTCommon.GetFSKJIni("COMMON", "EXE")
            If strcINI_PARAM.EXE_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - EXE)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - EXE)")
                Return False
            End If

            strcINI_PARAM.LST_FOLDER = CASTCommon.GetFSKJIni("COMMON", "LST")
            If strcINI_PARAM.LST_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - LST)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - LST)")
                Return False
            End If

            strcINI_PARAM.TXT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If strcINI_PARAM.TXT_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - TXT)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - TXT)")
                Return False
            End If

            strcINI_PARAM.KINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If strcINI_PARAM.KINKO_CODE = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - KINKOCD)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - KINKOCD)")
                Return False
            End If

            strcINI_PARAM.CENTER_CODE = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If strcINI_PARAM.CENTER_CODE = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - CENTER)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(COMMON - CENTER)")
                Return False
            End If

            '2013/11/13 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            strcINI_PARAM.ZEIKIJUN = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
            If strcINI_PARAM.ZEIKIJUN = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(JIFURI - ZEIKIJUN)")
                LOG.UpdateJOBMASTbyErr("イニシャルファイルからの取得失敗(JIFURI - ZEIKIJUN)")
                Return False
            End If
            '2013/11/13 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            LOG.Write("イニシャルファイル読み込み", "失敗", ex.Message)

            Return False
        End Try
    End Function

    ''' <summary>
    ''' 引落手数料を計算します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <returns>引落手数料</returns>
    ''' <remarks>2013/11/13 標準版 消費税対応</remarks>
    Private Function CalcTesuuKin1(ByRef TesuuKin1 As Long, _
                                   ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            '-------------------------------------------------------
            '引落手数料の計算
            '-------------------------------------------------------
            Dim intKen As Integer = 0
            Select Case oraReader.GetString("SEIKYU_KBN_TV1")
                Case "0" : intKen = oraReader.GetInt("SYORI_KEN_SV1")
                Case "1" : intKen = oraReader.GetInt("FURI_KEN_SV1")
                Case Else   '請求区分が他にあれば設定
            End Select

            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "0" : TesuuKin1 = CLng(Math.Floor((oraReader.GetInt("KIHTESUU_TV1") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")) * Double.Parse(Me.TAX.ZEIRITSU)))
                Case "1" : TesuuKin1 = oraReader.GetInt("KIHTESUU_TV1") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")
                Case Else
            End Select

            Return True

        Catch ex As Exception
            LOG.Write("引落手数料計算", "失敗", ex.Message)

            Return False
        End Try

    End Function

    ''' <summary>
    ''' 振込手数料マスタを読み込みます。
    ''' </summary>
    ''' <param name="TAX_ID">税率ID</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 標準版 消費税対応</remarks>
    Private Function GetJifuriTesuuTable(ByVal TAX_ID As String) As Boolean

        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            For i As Integer = 0 To Me.TESUU_TABLE_DATA.Length - 1
                Me.TESUU_TABLE_DATA(i).Init()
            Next

            With SQL
                .Append("select * from TESUUMAST")
                .Append(" where TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                    'If oraReader.GetInt("TESUU_TABLE_ID_C") > 0 AndAlso oraReader.GetInt("TESUU_TABLE_ID_C") < 10 Then
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    'End If
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            LOG.Write("振込手数料マスタ読み込み", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

End Class
