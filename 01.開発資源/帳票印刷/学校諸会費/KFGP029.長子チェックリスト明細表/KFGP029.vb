Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP029

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP029"

        ' 定義体名セット
        ReportBaseName = "KFGP029_長子チェックリスト明細表.rpd"
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
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT")
            SQL.Append(" SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMAST.CLASS_CODE_O")
            SQL.Append(",SEITOMAST.SEITO_NO_O")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",SEITOMAST.SEIBETU_O")
            SQL.Append(",SEITOMAST.TSIT_NO_O")
            SQL.Append(",SEITOMAST.KAMOKU_O")
            SQL.Append(",SEITOMAST.KOUZA_O")
            SQL.Append(",SEITOMAST.NENDO_O")
            SQL.Append(",SEITOMAST.TUUBAN_O")
            SQL.Append(",SEITOMAST.TYOUSI_NENDO_O")
            SQL.Append(",SEITOMAST.TYOUSI_GAKUNEN_O")
            SQL.Append(",SEITOMAST.TYOUSI_CLASS_O")
            SQL.Append(",SEITOMAST.TYOUSI_SEITONO_O")
            SQL.Append(",SEITOMAST.TYOUSI_FLG_O")
            SQL.Append(",SEITOMAST_1.SEITO_KNAME_O TYOUSI_KNAME_O")
            SQL.Append(",SEITOMAST_1.TUUBAN_O TYOUSI_TUUBAN_O")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")

            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.SEITOMAST SEITOMAST_1")
            SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O=SEITOMAST_1.GAKKOU_CODE_O (+)")
            SQL.Append(" AND SEITOMAST.TYOUSI_NENDO_O=SEITOMAST_1.NENDO_O (+)")
            SQL.Append(" AND SEITOMAST.TYOUSI_TUUBAN_O=SEITOMAST_1.TUUBAN_O (+)")
            SQL.Append(" AND SEITOMAST.TYOUSI_GAKUNEN_O=SEITOMAST_1.GAKUNEN_CODE_O (+)")
            SQL.Append(" AND SEITOMAST.TYOUSI_CLASS_O=SEITOMAST_1.CLASS_CODE_O (+)")
            SQL.Append(" AND SEITOMAST.TYOUSI_SEITONO_O=SEITOMAST_1.SEITO_NO_O (+)")
            SQL.Append(" AND SEITOMAST.TUKI_NO_O=SEITOMAST_1.TUKI_NO_O (+)")
            SQL.Append(" AND SEITOMAST.TUKI_NO_O = '04'")
            SQL.Append(" AND SEITOMAST.TYOUSI_FLG_O <> 0")
            If GakkouCode <> "9999999999" Then
                SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O = " & SQ(GakkouCode))
            End If
            SQL.Append(" ORDER BY SEITOMAST.GAKKOU_CODE_O,SEITOMAST.GAKUNEN_CODE_O")
            Select Case PrintSort
                Case "0"
                    SQL.Append(",SEITOMAST.CLASS_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_NO_O")
                    SQL.Append(",SEITOMAST.NENDO_O DESC")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case "1"
                    SQL.Append(",SEITOMAST.NENDO_O DESC")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case Else
                    SQL.Append(",SEITOMAST.SEITO_KNAME_O")
                    SQL.Append(",SEITOMAST.NENDO_O DESC")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_O")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))               '入学年度(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))              '通番(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")))        '学年(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O")))          'クラス(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")))            '生徒番号(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)   '生徒名(非長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_NENDO_O")))        '入学年度(長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_TUUBAN_O")))       '通番(長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_GAKUNEN_O")))      '学年(長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_CLASS_O")))        'クラス(長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_SEITONO_O")))      '生徒番号(長子)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TYOUSI_KNAME_O")), True)  '生徒名(長子)
                    If GCOM.NzStr(oraReader.GetString("TYOUSI_FLG_O")) <> "0" AndAlso _
                        GCOM.NzStr(oraReader.GetString("TYOUSI_KNAME_O")).Trim = "" Then    '備考
                        OutputCsvData("長子卒業", False, True)
                    Else
                        OutputCsvData("", False, True)
                    End If

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
