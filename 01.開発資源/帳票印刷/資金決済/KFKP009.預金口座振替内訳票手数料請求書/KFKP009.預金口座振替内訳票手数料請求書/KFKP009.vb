Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFKP009

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP009"

        ' 定義体名セット
        ReportBaseName = "KFKP009_預金口座振替票手数料請求書.rpd"
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
        Dim KINKO_NAME As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")

        Dim ken As Decimal = 0      '消費税計算用

        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        Dim TAX As New CASTCommon.ClsTAX
        Dim ZEIKIJUN As String = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
        If ZEIKIJUN.Equals("err") OrElse ZEIKIJUN = String.Empty Then
            BatchLog.Write("預金口座振替内訳票手数料請求書印刷", "失敗", "[JIFURI]ZEIKIJUN 設定なし")
            Return False
        End If
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

        Try
            '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            'Dim zei As Decimal = CType(CASTCommon.GetFSKJIni("COMMON", "ZEIRITU"), Double) - 1
            '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT ")
            SQL.Append(" TORIS_CODE_T")             '取引先主コード
            SQL.Append(",TORIF_CODE_T")             '取引先副コード
            SQL.Append(",ITAKU_NNAME_T")            '委託者漢字名

            SQL.Append(",SEIKYU_KBN_T")             '手数料請求区分
            SQL.Append(",KIHTESUU_T")               '振替手数料単価
            SQL.Append(",KOTEI_TESUU1_T")           '固定手数料１
            SQL.Append(",KOTEI_TESUU2_T")           '固定手数料２
            SQL.Append(",SYOUHI_KBN_T")             '消費税区分
            SQL.Append(",TESUUTYO_PATN_T")          '手数料徴求方法

            SQL.Append(",FURI_DATE_S")              '振替日
            SQL.Append(",SYORI_KEN_S")               '請求件数
            SQL.Append(",SYORI_KIN_S")               '請求金額
            SQL.Append(",FUNOU_KEN_S")               '不能件数
            SQL.Append(",FUNOU_KIN_S")               '不能金額
            SQL.Append(",FURI_KEN_S")               '振替件数
            SQL.Append(",FURI_KIN_S")               '振替金額
            SQL.Append(",TESUU_KIN_S")              '手数料金額
            SQL.Append(",TESUU_KIN1_S")              '手数料金額
            SQL.Append(",TESUU_KIN2_S")              '手数料金額
            SQL.Append(",TESUU_KIN3_S")              '手数料金額
            SQL.Append(",TESUUTYO_KBN_T")              '手数料徴求区分 2010.02.09
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            SQL.Append(",KESSAI_YDATE_S")           '決済予定日
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE　FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            If LW.ToriCode <> "0".PadLeft(12, "0"c) Then

                SQL.Append(" AND TORIS_CODE_S = " & SQ(LW.ToriCode.Substring(0, 10)))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(LW.ToriCode.Substring(10, 2)))

            End If

            SQL.Append(" AND FURI_DATE_S = '" & FURI_DATE & "'")        '振替日がパラメータ日
            SQL.Append(" AND TESUUKEI_FLG_S = '1'")                       '手数料計算済
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                       '0:有効
            
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    ken = 0

                    'GCOM.NzStrはNULL値対策

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)               '取引先名
                    OutputCsvData(mMatchingDate, True)                                                  '処理日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)               '振替日
                    OutputCsvData(oraReader.GetInt64("SYORI_KEN_S"), True)               '請求件数
                    OutputCsvData(oraReader.GetInt64("SYORI_KIN_S"), True)                '請求金額
                    OutputCsvData(oraReader.GetInt64("FUNOU_KEN_S"), True)                '不能件数
                    OutputCsvData(oraReader.GetInt64("FUNOU_KIN_S"), True)               '不能金額
                    OutputCsvData(oraReader.GetInt64("FURI_KEN_S"), True)                 '振替件数
                    OutputCsvData(oraReader.GetInt64("FURI_KIN_S"), True)                 '振替金額
                    OutputCsvData(oraReader.GetInt64("KIHTESUU_T"), True)                  '振替手数料単価

                    '請求方法区分で分岐 0, 請求分徴求　1, 振替分徴求
                    If oraReader.GetString("SEIKYU_KBN_T") = "0" Then
                        OutputCsvData(oraReader.GetInt64("SYORI_KEN_S"), True)                 '手数料対象件数

                        ken = oraReader.GetInt64("SYORI_KEN_S")

                    Else
                        OutputCsvData(oraReader.GetInt64("FURI_KEN_S"), True)                 '手数料対象件数

                        '2014/05/01 saitou 標準修正 MODIFY ----------------------------------------------->>>>
                        ken = oraReader.GetInt64("FURI_KEN_S")
                        'ken = oraReader.GetInt64("SYORI_KEN_S")
                        '2014/05/01 saitou 標準修正 MODIFY -----------------------------------------------<<<<

                    End If

                    OutputCsvData(oraReader.GetInt64("TESUU_KIN1_S"), True)                 '振替手数料額
                    OutputCsvData(oraReader.GetInt64("TESUU_KIN3_S"), True)                 '振込手数料
                    OutputCsvData(oraReader.GetInt64("TESUU_KIN2_S"), True)                 '郵送料

                    '消費税区分で分岐　0, 外税　1, 内税
                    '2010.02.09 手数料が特別免除なら印字しない
                    If oraReader.GetString("TESUUTYO_KBN_T") = "2" Then

                        OutputCsvData("0", True)       '消費税額

                    Else
                        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                        If ZEIKIJUN.Equals("0") Then
                            TAX.GetZeiritsu(oraReader.GetString("FURI_DATE_S"))
                        Else
                            TAX.GetZeiritsu(oraReader.GetString("KESSAI_YDATE_S"))
                        End If

                        If IsNumeric(TAX.ZEIRITSU) = False Then
                            BatchLog.Write("税率取得", "失敗", "取得値：" & TAX.ZEIRITSU)
                            Return False
                        End If

                        Dim zei As Decimal = CType(TAX.ZEIRITSU, Double) - 1
                        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                        If oraReader.GetString("SYOUHI_KBN_T") = "0" Then

                            OutputCsvData(Math.Floor(((oraReader.GetInt64("KIHTESUU_T") * ken) + _
                                                        oraReader.GetInt64("KOTEI_TESUU1_T") + oraReader.GetInt64("KOTEI_TESUU2_T")) * zei), True)                      '消費税額 切捨て

                        Else

                            OutputCsvData(oraReader.GetInt64("TESUU_KIN1_S") - (Math.Ceiling(oraReader.GetInt64("TESUU_KIN1_S") / (zei + 1))), True)       '消費税額

                        End If

                    End If

                    OutputCsvData(KINKO_NAME, True, True)                                               '金庫名

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
End Class
