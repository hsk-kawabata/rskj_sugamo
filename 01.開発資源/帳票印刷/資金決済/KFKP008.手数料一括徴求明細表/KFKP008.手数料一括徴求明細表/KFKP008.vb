Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFKP008

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP008"

        ' 定義体名セット
        ReportBaseName = "KFKP008_手数料徴求一括明細表.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '2009/12/03      '手数料テーブルファイルの読込
            '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            'Dim strTESUU_TABLE_FILE As String = ""
            'Dim intFILE_NO As Integer = 0
            '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
            Dim intKIJYUN_ID As Integer
            Dim JifuriKin As Long
            Dim JIFURI_Tesuu As Long
            Dim Soryo As Long
            Dim Furi_Tesuu As Long
            '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            'strTESUU_TABLE_FILE = Path.Combine(TXT_FOLDER, strTESUU_TABLE_FILE_NAME)
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
            '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
            '=============================================


            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT ")
            SQL.Append(" TESUU_YDATE_S")            '手数料徴求予定日
            SQL.Append(",TORIS_CODE_T")             '取引先主コード
            SQL.Append(",TORIF_CODE_T")             '取引先副コード
            SQL.Append(",ITAKU_NNAME_T")            '委託者漢字名
            SQL.Append(",FURI_CODE_T")              '振替コード
            SQL.Append(",KIGYO_CODE_T")             '企業コード
            '2009/12/02 項目追加 =================
            SQL.Append(",KOTEI_TESUU1_T") '固定手数料1
            SQL.Append(",KOTEI_TESUU2_T") '固定手数料2
            SQL.Append(",KIHTESUU_T") '振替手数料単価
            SQL.Append(",SOURYO_T") '送料
            SQL.Append(",TESUU_TABLE_ID_T") '振込手数料基準ID
            SQL.Append(",SYORI_KEN_S")                    '振替済金額
            SQL.Append(",FURI_KEN_S")                     '振替済件数
            SQL.Append(",FURI_KIN_S")                     '振替済金額
            '2013/11/14 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
            '引落手数料は別途計算する。
            SQL.Append(",SEIKYU_KBN_T")
            SQL.Append(",SYOUHI_KBN_T")
            'SQL.Append(", TRUNC(")
            'SQL.Append("((KIHTESUU_T * DECODE(SEIKYU_KBN_T,'0',SYORI_KEN_S,FURI_KEN_S))")
            'SQL.Append(" + NVL(KOTEI_TESUU1_T,0)")
            'SQL.Append(" + NVL(KOTEI_TESUU2_T,0))")
            'SQL.Append(" * DECODE(SYOUHI_KBN_T,'0'," & ZEI_RITU & ",1)) TESUU_KIN1")     ' 引落手数料
            '2013/11/14 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
            SQL.Append(",TUKEKIN_NO_T")                     '決済金融機関
            SQL.Append(",TUKESIT_NO_T")                     '決済支店
            SQL.Append(",TORIMATOME_SIT_T")                 '取りまとめ店
            '=====================================
            SQL.Append(",FURI_DATE_S")              '振替日
            SQL.Append(",TESUU_KIN_S")              '手数料金額
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            SQL.Append(",KESSAI_YDATE_S")
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE　FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TESUUKEI_FLG_S = '1' ")                          '1:手数料計算済
            '2009/12/02 範囲指定追加 ===========================
            'SQL.Append(" AND TESUU_YDATE_S = '" & TESUUTYO_YDATE & "'")     '手数料徴求予定日
            SQL.Append(" AND TESUU_YDATE_S >= '" & TESUUTYO_YDATE & "'")     '手数料徴求予定日
            SQL.Append(" AND TESUU_YDATE_S <= '" & TESUUTYO_YDATE2 & "'")
            '===================================================
            SQL.Append(" AND TESUUTYO_FLG_S = '0'")                         '0:手数料未徴求
            SQL.Append(" AND TESUUTYO_KBN_T = '1'")                         '1:一括徴求
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                           '0:有効
            SQL.Append(" AND TOUROKU_FLG_S = '1'")                          '1:登録済
            SQL.Append(" AND TESUU_KIN_S > 0")                              '手数料金額
            SQL.Append(" ORDER BY TESUU_YDATE_S,TORIS_CODE_T,TORIF_CODE_T,FURI_DATE_S")
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                                  '処理日
                    OutputCsvData(mMatchingTime, True)                                                  'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TESUU_YDATE_S")), True)               '手数料徴求予定日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)                '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)                '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)               '委託者漢字名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)                 '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)                '企業コード
                    '2009/12/03
                    '振込手数料基準ＩＤを設定
                    If oraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                        intKIJYUN_ID = 0
                    Else
                        intKIJYUN_ID = CInt(oraReader.GetString("TESUU_TABLE_ID_T"))
                    End If

                    '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                    Dim strKijunDate As String = String.Empty
                    If ZEIKIJUN.Equals("0") = True Then
                        '振替日基準
                        strKijunDate = oraReader.GetString("FURI_DATE_S")
                    Else
                        '決済日基準
                        strKijunDate = oraReader.GetString("KESSAI_YDATE_S")
                    End If

                    '税率取得
                    TAX.GetZeiritsu(strKijunDate)
                    If TAX.ZEIRITSU.Equals("err") = True Then
                        BatchLog.Write("税率取得", "失敗", "基準日：" & strKijunDate)
                        Return False
                    End If

                    '振込手数料マスタ読み込み
                    If Me.GetJifuriTesuuTable(TAX.ZEIRITSU_ID, oraDB) = False Then
                        Return False
                    End If
                    '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                    '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
                    '印紙税取得
                    TAX.GetInshizei(strKijunDate)
                    If TAX.INSHIZEI_ID.Equals("err") = True Then
                        BatchLog.Write("印紙税取得", "失敗", "基準日：" & strKijunDate)
                        Return False
                    End If
                    '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

                    '2013/11/14 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                    '引落手数料は別途計算する
                    If Me.CalcTesuuKin1(JIFURI_Tesuu, oraReader) = False Then
                        Return False
                    End If
                    'JIFURI_Tesuu = oraReader.GetInt64("TESUU_KIN1")        ' 引落手数料
                    '2013/11/14 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
                    Soryo = oraReader.GetInt64("SOURYO_T")        ' 送料
                    JifuriKin = oraReader.GetInt64("FURI_KIN_S") - (JIFURI_Tesuu + Soryo)
                    Furi_Tesuu = 0

                    '振込手数料計算
                    '2013/12/27 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                    If oraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                        ' 決済金融機関が，自金庫の場合
                        If oraReader.GetString("TUKESIT_NO_T") = oraReader.GetString("TORIMATOME_SIT_T") Then
                            ' 決済支店がとりまとめ店と一致する場合，自店内
                            If 0 < JifuriKin And JifuriKin < TAX.INSHIZEI1 Then
                                ' ０円より大きい かつ １万円未満
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                            ElseIf TAX.INSHIZEI1 < JifuriKin And JifuriKin < TAX.INSHIZEI2 Then
                                ' １万円より大きい かつ ３万円未満
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                            ElseIf TAX.INSHIZEI2 <= JifuriKin Then
                                ' ３万円以上
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                            End If
                        Else
                            ' 決済支店がとりまとめ店と一致しない場合，本支店
                            If 0 < JifuriKin And JifuriKin < TAX.INSHIZEI1 Then
                                ' ０円より大きい かつ １万円未満
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                            ElseIf TAX.INSHIZEI1 < JifuriKin And JifuriKin < TAX.INSHIZEI2 Then
                                ' １万円より大きい かつ ３万円未満
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                            ElseIf TAX.INSHIZEI2 <= JifuriKin Then
                                ' ３万円以上
                                Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                            End If
                        End If
                    Else
                        ' 他行
                        If 0 < JifuriKin And JifuriKin < TAX.INSHIZEI1 Then
                            ' ０円より大きい かつ １万円未満
                            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                        ElseIf TAX.INSHIZEI1 < JifuriKin And JifuriKin < TAX.INSHIZEI2 Then
                            ' １万円円より大きい かつ ３万円未満
                            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                        ElseIf TAX.INSHIZEI2 <= JifuriKin Then
                            ' ３万円以上
                            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                        End If
                    End If

                    'If oraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                    '    ' 決済金融機関が，自金庫の場合
                    '    If oraReader.GetString("TUKESIT_NO_T") = oraReader.GetString("TORIMATOME_SIT_T") Then
                    '        ' 決済支店がとりまとめ店と一致する場合，自店内
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            ' ０円より大きい かつ １万円未満
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                    '        ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '            ' １万円より大きい かつ ３万円未満
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' ３万円以上
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                    '        End If
                    '    Else
                    '        ' 決済支店がとりまとめ店と一致しない場合，本支店
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            ' ０円より大きい かつ １万円未満
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                    '        ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '            ' １万円より大きい かつ ３万円未満
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' ３万円以上
                    '            Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                    '        End If
                    '    End If
                    'Else
                    '    ' 他行
                    '    If 0 < JifuriKin And JifuriKin < 10000 Then
                    '        ' ０円より大きい かつ １万円未満
                    '        Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                    '    ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '        ' １万円円より大きい かつ ３万円未満
                    '        Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                    '    ElseIf 30000 <= JifuriKin Then
                    '        ' ３万円以上
                    '        Furi_Tesuu += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                    '    End If
                    'End If
                    '2013/12/27 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<

                    OutputCsvData(GCOM.NzStr(Soryo), True) '送料
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIHTESUU_T")), True)  '振替手数料単価
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOTEI_TESUU1_T")), True) '固定手数料1
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOTEI_TESUU2_T")), True) '固定手数料2
                    OutputCsvData(GCOM.NzStr(Furi_Tesuu), True)                         '振込手数料
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SYORI_KEN_S")), True) '請求件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_KEN_S")), True)  '振込件数
                    '====================================================================================
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)                 '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TESUU_KIN_S")), True, True)           '手数料金額
                    oraReader.NextRead()
                    RecordCnt += 1
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' 引落手数料を計算します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <returns>引落手数料</returns>
    ''' <remarks>2013/11/14 標準版 消費税対応</remarks>
    Private Function CalcTesuuKin1(ByRef TesuuKin1 As Long, _
                                   ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            '-------------------------------------------------------
            '引落手数料の計算
            '-------------------------------------------------------
            Dim intKen As Integer = 0
            Select Case oraReader.GetString("SEIKYU_KBN_T")
                Case "0" : intKen = oraReader.GetInt("SYORI_KEN_S")
                Case "1" : intKen = oraReader.GetInt("FURI_KEN_S")
                Case Else   '請求区分が他にあれば設定
            End Select

            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "0" : TesuuKin1 = CLng(Math.Floor((oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")) * Double.Parse(TAX.ZEIRITSU)))
                Case "1" : TesuuKin1 = oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")
                Case Else
            End Select

            Return True

        Catch ex As Exception
            BatchLog.Write("引落手数料計算", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 振込手数料マスタを読み込みます。
    ''' </summary>
    ''' <param name="TAX_ID">税率ID</param>
    ''' <returns>True or False</returns>
    ''' <remarks>
    ''' 2013/11/27 標準版 消費税対応
    ''' 2013/12/19 MODIFY 引数にオラクル接続追加
    ''' </remarks>
    Private Function GetJifuriTesuuTable(ByVal TAX_ID As String, _
                                         ByVal db As CASTCommon.MyOracle) As Boolean

        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(db)

        Try
            With SQL
                .Append("select * from TESUUMAST")
                .Append(" where TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
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
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            BatchLog.Write("振込手数料マスタ読み込み", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

End Class
