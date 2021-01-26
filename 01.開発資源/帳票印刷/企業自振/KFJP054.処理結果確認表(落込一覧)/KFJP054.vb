Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP054

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP054"

        ' 定義体名セット
        ReportBaseName = "KFJP054_処理結果確認表(落込一覧).rpd"
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

            SQL.Append(" SELECT ")
            SQL.Append(" * ")
            SQL.Append(" FROM SCHMAST, TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T ")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T ")
            SQL.Append(" AND TOUROKU_DATE_S = '" & SyoriDate & "'")
            SQL.Append(" ORDER BY BAITAI_CODE_T,FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S ")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    OutputCsvData(oraReader.GetItem("FURI_DATE_S"), True) '振替日
                    OutputCsvData(mMatchingDate, True)    '処理日
                    OutputCsvData(mMatchingTime, True)    'タイムスタンプ
                    OutputCsvData(SyoriDate, True)        '受付日
                    OutputCsvData(oraReader.GetItem("TORIS_CODE_S"), True) '取引先主コード
                    OutputCsvData(oraReader.GetItem("TORIF_CODE_S"), True) '取引先副コード
                    OutputCsvData(oraReader.GetItem("ITAKU_NNAME_T"), True) '取引先名
                    OutputCsvData(oraReader.GetItem("ITAKU_CODE_S"), True) '委託者コード
                    Dim BaitaiName As String = ""
                    If CInt(oraReader.GetItem("BAITAI_CODE_T")) = 7 Then
                        OutputCsvData("学校", True) '媒体
                    Else
                        OutputCsvData(GetBaitaiName(oraReader.GetItem("BAITAI_CODE_T")), True) '媒体
                    End If
                    Dim SyoriKen As Decimal = oraReader.GetItem("SYORI_KEN_S")
                    Dim SyoriKin As Decimal = oraReader.GetItem("SYORI_KIN_S")
                    Dim ErrKen As Decimal = oraReader.GetItem("ERR_KEN_S")
                    Dim ErrKin As Decimal = oraReader.GetItem("ERR_KIN_S")

                    OutputCsvData(SyoriKen.ToString) '依頼件数
                    OutputCsvData(SyoriKin.ToString) '依頼金額
                    OutputCsvData((SyoriKen - ErrKen).ToString) '処理件数
                    OutputCsvData((SyoriKin - ErrKin).ToString) '処理金額
                    OutputCsvData(oraReader.GetItem("KIGYO_CODE_T"), True) '企業コード
                    OutputCsvData(oraReader.GetItem("FURI_CODE_T"), True, True) '振替コード

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

    Private Function GetBaitaiName(ByVal BaitaiCode As String) As String

        Dim ret As String = ""

        Dim sr As StreamReader = Nothing

        Try
            Dim TXTFileName As String = Path.Combine(GCom.GetTXTFolder, "Common_媒体コード.TXT")

            sr = New StreamReader(TXTFileName, Encoding.GetEncoding(932))
            Dim ReadLine As String = ""

            While sr.EndOfStream = False
                ReadLine = sr.ReadLine()
                If Not ReadLine Is Nothing Then
                    Dim Item As String() = ReadLine.Split(","c)

                    If CInt(Item(0)) = CInt(BaitaiCode) Then
                        ret = Item(1)
                        Exit While
                    End If
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ret

        Catch ex As Exception
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

        Return ret

    End Function
End Class
