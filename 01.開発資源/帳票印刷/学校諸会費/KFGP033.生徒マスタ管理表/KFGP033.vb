Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon

Public Class KFGP033

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP033"
        ' 定義体名セット
        ReportBaseName = "KFGP033_生徒マスタ管理表.rpd"

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
        Dim SeitoReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim GAK_Info(11) As String
        Dim SEITO_Info(,) As String
        Dim SeitoNo As Integer = 0

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT DISTINCT")
            SQL.Append(" FURI_CODE_T ")
            SQL.Append(", KIGYO_CODE_T ")
            SQL.Append(", TORIMATOME_SIT_T ")
            SQL.Append(", SYURYOU_DATE_T ")
            SQL.Append(", SKYU_CODE_T ")
            SQL.Append(", FURI_DATE_T ")
            SQL.Append(", SFURI_DATE_T ")
            SQL.Append(", TKIN_NO_T ")
            SQL.Append(", TSIT_NO_T ")
            SQL.Append(", KAMOKU_T ")
            SQL.Append(", KOUZA_T ")
            SQL.Append(", GAKKOU_NNAME_G ")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(" FROM GAKMAST2,TENMAST,GAKMAST1")
            SQL.Append(" WHERE ")
            SQL.Append(" GAKMAST1.GAKKOU_CODE_G  = GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND GAKMAST2.TKIN_NO_T = TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND GAKMAST2.TSIT_NO_T = TENMAST.SIT_NO_N (+)")
            SQL.Append(" AND GAKMAST2.GAKKOU_CODE_T = " & SQ(GakkouCode))

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    '学校情報取得
                    GAK_Info(0) = GCOM.NzStr(oraReader.GetString("FURI_CODE_T"))          '振替コード
                    GAK_Info(1) = GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T"))         '企業コード
                    GAK_Info(2) = GCOM.NzStr(oraReader.GetString("TSIT_NO_T"))             'とりまとめ店
                    GAK_Info(3) = GCOM.NzStr(oraReader.GetString("SIT_NNAME_N"))             'とりまとめ店名
                    GAK_Info(4) = GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G"))             '提携先名
                    GAK_Info(5) = GCOM.NzStr(oraReader.GetString("SYURYOU_DATE_T"))   '振替終了年月
                    Select Case GCOM.NzStr(oraReader.GetString("SKYU_CODE_T"))      '休日条件
                        Case "0"
                            GAK_Info(6) = "１後営業日"
                        Case "1"
                            GAK_Info(6) = "１前営業日"
                    End Select
                    GAK_Info(7) = GCOM.NzStr(oraReader.GetString("SFURI_DATE_T"))       '再振日
                    GAK_Info(8) = GCOM.NzStr(oraReader.GetString("TSIT_NO_T"))       '付替店
                    GAK_Info(9) = GCOM.NzStr(oraReader.GetString("KAMOKU_T"))       '科目
                    GAK_Info(10) = GCOM.NzStr(oraReader.GetString("KOUZA_T"))       '口番
                    GAK_Info(11) = GCOM.NzStr(oraReader.GetString("FURI_DATE_T"))   '振替日

                    SeitoReader = New CASTCommon.MyOracleReader(oraDB)
                    SQL = New StringBuilder(128)

                    SQL.Append("SELECT * FROM SEITOMASTVIEW")
                    SQL.Append(" WHERE ")
                    SQL.Append(" GAKKOU_CODE_O = '" & GakkouCode & "'")
                    SQL.Append(" ORDER BY GAKUNEN_CODE_O, CLASS_CODE_O, SEITO_NO_O, NENDO_O, TUUBAN_O, TUKI_NO_O")

                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                    ReDim SEITO_Info(SEITO_COUNT, 38)
                    ReDim Himoku_NAME(14)
                    'ReDim SEITO_Info(SEITO_COUNT, 28)
                    'ReDim Himoku_NAME(9)
                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    SeitoNo = 0

                    If SeitoReader.DataReader(SQL) = True Then
                        While SeitoReader.EOF = False
                            '生徒情報取得

                            If SeitoNo = 0 Then     '初回のみ費目名取得
                                ''費目名取得
                                'If fn_Get_HimokuName(GakkouCode, GCOM.NzStr(SeitoReader.GetString("GAKUNEN_CODE_O")), oraDB) Then
                                '    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目名取得", "失敗", "学年：" & GCOM.NzStr(SeitoReader.GetString("GAKUNEN_CODE_O")))
                                'End If
                            End If
                            SEITO_Info(SeitoNo, 0) = GCOM.NzStr(SeitoReader.GetString("SEITO_NO_O"))    '生徒番号

                            If GCOM.NzStr(SeitoReader.GetString("SEITO_NNAME_O")) = "" Then             '生徒名
                                SEITO_Info(SeitoNo, 1) = GCOM.NzStr(SeitoReader.GetString("SEITO_KNAME_O"))
                            Else
                                SEITO_Info(SeitoNo, 1) = GCOM.NzStr(SeitoReader.GetString("SEITO_NNAME_O"))
                            End If
                            SEITO_Info(SeitoNo, 2) = GCOM.NzStr(SeitoReader.GetString("TSIT_NO_O"))         '店番
                            Select Case GCOM.NzStr(SeitoReader.GetString("KAMOKU_O"))                       '科目
                                Case "02"
                                    SEITO_Info(SeitoNo, 3) = "普通"
                                Case "01"
                                    SEITO_Info(SeitoNo, 3) = "当座"
                                Case Else
                                    SEITO_Info(SeitoNo, 3) = ""
                            End Select
                            SEITO_Info(SeitoNo, 4) = GCOM.NzStr(SeitoReader.GetString("KOUZA_O"))           '口座番号

                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                            For No As Integer = 1 To 15 '費目名、費目金額取得
                                'For No As Integer = 1 To 10 '費目名、費目金額取得
                                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                                If GCOM.NzStr(SeitoReader.GetString("SEIKYU" & No.ToString("00") & "_O")) = "0" Then       '請求区分により分岐
                                    SEITO_Info(SeitoNo, No + 4) = GCOM.NzStr(SeitoReader.GetString("HIMOKU_KINGAKU" & No.ToString("00") & "_O")) '費目金額5～14
                                Else
                                    SEITO_Info(SeitoNo, No + 4) = GCOM.NzStr(SeitoReader.GetString("KINGAKU" & No.ToString("00") & "_O")) '個別金額5～14
                                End If
                                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                                SEITO_Info(SeitoNo, No + 19) = GCOM.NzStr(SeitoReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_O")) '費目名15～24
                                'SEITO_Info(SeitoNo, No + 14) = GCOM.NzStr(SeitoReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_O")) '費目名15～24
                                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            Next
                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                            SEITO_Info(SeitoNo, 35) = GCOM.NzStr(SeitoReader.GetString("NENDO_O"))      '年度
                            SEITO_Info(SeitoNo, 36) = GCOM.NzStr(SeitoReader.GetString("TUUBAN_O"))     '通番
                            SEITO_Info(SeitoNo, 37) = GCOM.NzStr(SeitoReader.GetString("GAKUNEN_CODE_O"))      '学年
                            SEITO_Info(SeitoNo, 38) = GCOM.NzStr(SeitoReader.GetString("CLASS_CODE_O"))     'クラス
                            'SEITO_Info(SeitoNo, 25) = GCOM.NzStr(SeitoReader.GetString("NENDO_O"))      '年度
                            'SEITO_Info(SeitoNo, 26) = GCOM.NzStr(SeitoReader.GetString("TUUBAN_O"))     '通番
                            'SEITO_Info(SeitoNo, 27) = GCOM.NzStr(SeitoReader.GetString("GAKUNEN_CODE_O"))      '学年
                            'SEITO_Info(SeitoNo, 28) = GCOM.NzStr(SeitoReader.GetString("CLASS_CODE_O"))     'クラス
                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

                            SeitoNo += 1
                            SeitoReader.NextRead()
                        End While

                        'CSV作成
                        Dim Class_Name As String = ""
                        Dim TempClass_Name As String = ""

                        Dim count As Double = 0
                        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For j As Integer = 0 To 14
                            'For j As Integer = 0 To 9
                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            For count = 0 To SeitoNo - 1
                                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                                '費目名が未設定の場合は出力しない
                                If SEITO_Info(count, 20 + j) <> "" Then
                                    'If SEITO_Info(count, 15 + j) <> "" Then
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                                    OutputCsvData(mMatchingDate)                                            '処理日
                                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                                    OutputCsvData(GAK_Info(0))         '振替コード
                                    OutputCsvData(GAK_Info(1))         '企業コード
                                    OutputCsvData(GAK_Info(2))         'とりまとめ店
                                    OutputCsvData(GAK_Info(3))         'とりまとめ店名
                                    OutputCsvData(GAK_Info(4))         '提携先名
                                    OutputCsvData(GAK_Info(5))         '振替終了年月
                                    OutputCsvData(GAK_Info(6))         '休日条件
                                    OutputCsvData(GAK_Info(7))         '再振日
                                    OutputCsvData(GAK_Info(8))         '付替店
                                    OutputCsvData(GAK_Info(9))         '科目
                                    OutputCsvData(GAK_Info(10))        '口番
                                    OutputCsvData(GAK_Info(11))        '振替日
                                    OutputCsvData(SEITO_Info(count, 0))       '生徒番号
                                    OutputCsvData(SEITO_Info(count, 1))       '生徒名
                                    OutputCsvData(SEITO_Info(count, 2))       '店番
                                    OutputCsvData(SEITO_Info(count, 3))       '科目
                                    OutputCsvData(SEITO_Info(count, 4))       '口座番号
                                    OutputCsvData((j + 1).ToString("00"))  '費目NO
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                                    OutputCsvData(SEITO_Info(count, 20 + j))  '費目名
                                    'OutputCsvData(SEITO_Info(count, 15 + j))  '費目名
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                                    For i As Double = 1 To 12
                                        OutputCsvData(SEITO_Info(count, 5 + j))       '金額
                                        count += 1
                                    Next
                                    count -= 1
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                                    OutputCsvData(SEITO_Info(count, 35))    '年度
                                    OutputCsvData(SEITO_Info(count, 36))    '通番
                                    OutputCsvData(SEITO_Info(count, 37))    '学年
                                    OutputCsvData(SEITO_Info(count, 38))    'クラス
                                    'OutputCsvData(SEITO_Info(count, 25))    '年度
                                    'OutputCsvData(SEITO_Info(count, 26))    '通番
                                    'OutputCsvData(SEITO_Info(count, 27))    '学年
                                    'OutputCsvData(SEITO_Info(count, 28))    'クラス
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                                    'クラス名取得
                                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                                    If fn_Get_ClassName(GakkouCode, SEITO_Info(count, 37), SEITO_Info(count, 38), Class_Name, oraDB) = False Then
                                        'If fn_Get_ClassName(GakkouCode, SEITO_Info(count, 27), SEITO_Info(count, 28), Class_Name, oraDB) = False Then
                                        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                                        Class_Name = ""
                                    End If
                                    OutputCsvData(Class_Name)    'クラス
                                    OutputCsvData("", False, True)          '改行用ダミー
                                    RecordCnt += 1
                                Else
                                    count += 11
                                End If
                                'count += 1
                            Next
                        Next
                    Else '生徒が存在しない場合は、0件で印刷処理
                        OutputCsvData(mMatchingDate)                                            '処理日
                        OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                        OutputCsvData(GAK_Info(0))         '振替コード
                        OutputCsvData(GAK_Info(1))         '企業コード
                        OutputCsvData(GAK_Info(2))         'とりまとめ店
                        OutputCsvData(GAK_Info(3))         'とりまとめ店名
                        OutputCsvData(GAK_Info(4))         '提携先名
                        OutputCsvData(GAK_Info(5))         '振替終了年月
                        OutputCsvData(GAK_Info(6))         '休日条件
                        OutputCsvData(GAK_Info(7))         '再振日
                        OutputCsvData(GAK_Info(8))         '付替店
                        OutputCsvData(GAK_Info(9))         '科目
                        OutputCsvData(GAK_Info(10))        '口番
                        OutputCsvData(GAK_Info(11))        '振替日
                        OutputCsvData("0")       '生徒番号
                        OutputCsvData("")       '生徒名
                        OutputCsvData("")       '店番
                        OutputCsvData("")       '科目
                        OutputCsvData("")       '口座番号
                        OutputCsvData("")       '費目名
                        For i As Double = 1 To 12
                            OutputCsvData("0")       '金額
                        Next
                        OutputCsvData("")  '年度
                        OutputCsvData("0")  '通番
                        OutputCsvData("0")  '学年
                        OutputCsvData("")  'クラス
                        OutputCsvData("")  'クラス名
                        OutputCsvData("", False, True)                                          '改行用ダミー
                        RecordCnt += 1
                    End If
                    oraReader.NextRead()
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
    Function fn_Get_ClassName(ByVal GAKKOU_CODE As String, ByVal GkunenCode As String, ByVal ClassCode As String, ByRef ClassName As String, ByVal db As CASTCommon.MyOracle) As Boolean

        Dim PartReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        fn_Get_ClassName = False
        Try
            Dim intCnt As Integer
            Dim chkCLASS As Boolean = False '2006/10/11　クラス存在チェック

            PartReader = New CASTCommon.MyOracleReader(db)

            SQL = New StringBuilder(128)
            SQL.Append(" SELECT * FROM GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_G ='" & GAKKOU_CODE & "'")
            SQL.Append(" AND GAKUNEN_CODE_G =" & GkunenCode)

            If Not PartReader.DataReader(SQL) Then
                ClassName = ""
            Else
                '入力したクラスと一致するクラスをDB上より取得
                '一致したクラスコードのクラス名称を取得する
                For intCnt = 1 To 20
                    '設定されていないNULL値の場合はクラス名設定を抜ける
                    'クラスをとびとびで登録した場合の対処追加
                    If PartReader.GetItem("CLASS_CODE1" & intCnt.ToString("00") & "_G") = Nothing OrElse _
                                                                        PartReader.GetItem("CLASS_CODE1" & intCnt.ToString("00") & "_G") = "" Then
                        'クラス飛ばし対応のため処理なし
                    Else
                        '画面上で入力されているクラスコードと一致するクラスコードのクラス名称をラベルに設定
                        If ClassCode = GCOM.NzInt((PartReader.GetItem("CLASS_CODE1" & intCnt.ToString("00") & "_G"))) Then
                            ClassName = PartReader.GetItem("CLASS_NAME1" & intCnt.ToString("00") & "_G")
                            chkCLASS = True
                            Exit For
                        End If
                    End If
                Next intCnt

                'クラスが検出されなかった場合、クラス名ラベルは空白にする
                If chkCLASS = False Then
                    ClassName = ""
                End If
            End If

            fn_Get_ClassName = True

        Catch ex As Exception
            BatchLog.Write("例外発生", "失敗", ex.Message)
            fn_Get_ClassName = False

        Finally
            PartReader.Close()
            PartReader = Nothing
        End Try


    End Function


    'Function fn_Get_HimokuName(ByVal GAKKOU_CODE As String, ByVal GkunenCode As String,  ByVal db As CASTCommon.MyOracle) As Boolean
    '    '=====================================================================================
    '    'NAME           :fn_Select_SAIFURI
    '    'Parameter      :GAKKOU_CODE－学校コード／GAKUNEN_CODE－学年コード
    '    '               :HIMOKU_NAME()－費目名／HIMOKU_TUKEKOUZA－費目別付替先口座／HIMO_MEIGI－付替先顧客名
    '    'Description    :費目マスタ検索
    '    'Return         :True=OK(検索ヒット),False=NG（検索失敗）
    '    'Create         :2010/10/27
    '    'Update         :
    '    '=====================================================================================
    '    Dim PartReader As CASTCommon.MyOracleReader = Nothing
    '    Dim SQL As New StringBuilder(128)
    '    fn_Get_HimokuName = False
    '    PartReader = New CASTCommon.MyOracleReader(db)
    '    Try
    '        SQL = New StringBuilder(128)

    '        SQL.Append(" SELECT ")
    '        SQL.Append("  HIMOKU_NAME01_H ")
    '        SQL.Append(", HIMOKU_NAME02_H ")
    '        SQL.Append(", HIMOKU_NAME03_H ")
    '        SQL.Append(", HIMOKU_NAME04_H ")
    '        SQL.Append(", HIMOKU_NAME05_H ")
    '        SQL.Append(", HIMOKU_NAME06_H ")
    '        SQL.Append(", HIMOKU_NAME07_H ")
    '        SQL.Append(", HIMOKU_NAME08_H ")
    '        SQL.Append(", HIMOKU_NAME09_H ")
    '        SQL.Append(", HIMOKU_NAME10_H ")
    '        SQL.Append(" FROM HIMOMAST ")
    '        SQL.Append(" WHERE GAKKOU_CODE_H= " & SQ(GAKKOU_CODE))
    '        SQL.Append(" AND GAKUNEN_CODE_H  = " & SQ(GkunenCode))
    '        SQL.Append(" AND HIMOKU_ID_H      = '000'")

    '        '読込のみ
    '        If PartReader.DataReader(SQL) = False Then
    '            fn_Get_HimokuName = False
    '            Exit Function
    '        Else
    '            For no As Integer = 1 To 10
    '                Himoku_NAME(no - 1) = PartReader.GetInt("HIMOKU_NAME" & no.ToString("00") & "_H")
    '            Next

    '            fn_Get_HimokuName = True

    '        End If
    '    Catch ex As Exception
    '        BatchLog.Write("例外発生", "失敗", ex.Message)
    '        Return False

    '    Finally
    '        PartReader.Close()
    '    End Try

    '    Return True
    'End Function

End Class
