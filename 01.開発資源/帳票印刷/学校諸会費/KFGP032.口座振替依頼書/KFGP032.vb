Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP032
    Inherits CAstReports.ClsReportBase

    Private GAKKOU_CODE As String       '学校コード
    Private TKIN_NO As String           '取扱金融機関コード
    Private KIN_NAME As String          '取扱金融機関名
    Private TSIT_NO As String           '取扱支店名
    Private SIT_NAME As String          '取扱支店名
    Private ITAKU_CODE As String        '委託者コード
    Private ITAKU_NAME As String        '学校漢字名
    Private NS_KBN As String            '入出金区分
    Private TORIMATOME_SIT As String    '取りまとめ店
    Private TORIMATOME_NAME As String   '取りまとめ店名
    Private MEISAI_OUT As String        'ソート順
    Private TAISYOU_GAKUNEN As String   '2010/10/12.Sakon　対象学年

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP032"

        ' 定義体名セット
        ReportBaseName = "KFGP032_口座振替依頼書.rpd"
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
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            'パラメータから取得
            GAKKOU_CODE = GakkouCode            '学校コード
            NS_KBN = NSKbn                      '入出金区分
            TAISYOU_GAKUNEN = TaisyouGakunen     '2010/10/12.Sakon　対象学年追加

            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT ")
            SQL.Append(" TKIN_NO_T")                                '金融機関コード
            SQL.Append(",T1.KIN_NNAME_N AS TKIN_NAME")              '金融機関名
            SQL.Append(",TSIT_NO_T")                                '支店コード
            SQL.Append(",T1.SIT_NNAME_N AS TSIT_NAME")              '支店名
            SQL.Append(",ITAKU_CODE_T")                             '委託者コード
            SQL.Append(",TORIMATOME_SIT_T")                         '取りまとめ店
            SQL.Append(",T2.SIT_NNAME_N AS TMAT_NAME")              '取りまとめ店名
            SQL.Append(",MEISAI_OUT_T")                             '依頼書出力順
            SQL.Append(" FROM GAKMAST2,TENMAST T1,TENMAST T2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(GakkouCode))
            SQL.Append(" AND T1.KIN_NO_N = TKIN_NO_T")
            SQL.Append(" AND T1.SIT_NO_N = TSIT_NO_T")
            SQL.Append(" AND T2.KIN_NO_N = TKIN_NO_T")
            SQL.Append(" AND T2.SIT_NO_N = TORIMATOME_SIT_T")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    TKIN_NO = GCOM.NzStr(oraReader.GetString("TKIN_NO_T"))                  '金融機関コード
                    KIN_NAME = GCOM.NzStr(oraReader.GetString("TKIN_NAME"))                 '金融機関名
                    TSIT_NO = GCOM.NzStr(oraReader.GetString("TSIT_NO_T"))                  '支店コード
                    SIT_NAME = GCOM.NzStr(oraReader.GetString("TSIT_NAME"))                 '支店名
                    ITAKU_CODE = GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T"))            '委託者コード
                    TORIMATOME_SIT = GCOM.NzStr(oraReader.GetString("TORIMATOME_SIT_T"))    '取りまとめ店
                    TORIMATOME_NAME = GCOM.NzStr(oraReader.GetString("TMAT_NAME"))          '取りまとめ店名
                    MEISAI_OUT = GCOM.NzStr(oraReader.GetString("MEISAI_OUT_T"))            '帳票出力順

                    If SetSEITOMAST() = False Then
                        Return False
                    End If

                    oraReader.NextRead()

                End While

                If AllRecordCnt = 0 Then
                    BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                    RecordCnt = -1
                    Return False
                End If
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
    '
    ' 機能　 ： 依頼書の内容を書き出す
    '
    ' 備考　 ： 
    '
    Private Function SetSEITOMAST() As Boolean

        RecordCnt = 0
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim i As Integer '2010/10/12.Sakon　追加

        Try

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append(" SELECT")
            SQL.Append(" GAKKOU_NNAME_G")   '学校名
            SQL.Append(",NENDO_O")          '入学年度
            SQL.Append(",TUUBAN_O")         '通番
            SQL.Append(",GAKUNEN_CODE_O")   '学年
            SQL.Append(",CLASS_CODE_O")     'クラス
            SQL.Append(",SEITO_NO_O")       '生徒番号
            SQL.Append(",TKIN_NO_O")        '契約金融機関コード
            SQL.Append(",KIN_NNAME_N")      '契約金融機関名
            SQL.Append(",TSIT_NO_O")        '契約支店コード
            SQL.Append(",SIT_NNAME_N")      '契約支店名
            SQL.Append(",KAMOKU_O")         '契約科目
            SQL.Append(",KOUZA_O")          '契約口座
            SQL.Append(",MEIGI_KNAME_O")    '名義人名カナ
            SQL.Append(",MEIGI_NNAME_O")    '名義人名漢字
            SQL.Append(",SEITO_KNAME_O")    '生徒名カナ
            SQL.Append(",SEITO_NNAME_O")    '生徒名漢字

            SQL.Append(" FROM GAKMAST1,SEITOMAST,TENMAST")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_O")
            SQL.Append(" AND GAKUNEN_CODE_G = GAKUNEN_CODE_O")
            SQL.Append(" AND KIN_NO_N = TKIN_NO_O")
            SQL.Append(" AND SIT_NO_N = TSIT_NO_O")
            SQL.Append(" AND KAIYAKU_FLG_O = '0'")                  '解約済みでない
            SQL.Append(" AND TUKI_NO_O = '04'")
            SQL.Append(" AND GAKKOU_CODE_O = " & SQ(GAKKOU_CODE))

            '2010/10/12.Sakon　対象学年を指定 +++++++++++++++++++++++++++++++++++++++++++++++++
            For i = 0 To TAISYOU_GAKUNEN.Length - 1
                If i = 0 Then
                    SQL.Append(" AND (GAKUNEN_CODE_O = " & TAISYOU_GAKUNEN.Substring(i, 1))
                Else
                    SQL.Append(" OR GAKUNEN_CODE_O = " & TAISYOU_GAKUNEN.Substring(i, 1))
                End If
            Next
            SQL.Append(")")
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '2010/11/07 他の標準帳票と条件を合わせる
            'Select Case MEISAI_OUT
            '    Case "0"
            '        '学年・クラス・生徒番号
            '        '2010/10/20 GAKUEN_CODE_O⇒GAKUNEN_CODE_Oに修正
            '        'SQL.Append(" ORDER BY GAKUEN_CODE_O,CLASS_CODE_O,SEITO_NO_O")
            '        SQL.Append(" ORDER BY GAKUNEN_CODE_O,CLASS_CODE_O,SEITO_NO_O")
            '    Case "1"
            '        '入学年度・通番
            '        SQL.Append(" ORDER BY NENDO_O,TUUBAN_O")
            '    Case "2"
            '        'あいうえお
            '        SQL.Append(" ORDER BY MEIGI_KNAME_O")
            '        SQL.Append(" ORDER BY SEITO_KNAME_O")
            'End Select

            '2011/03/01 TASK)saitou MOD -------------------->
            'SQL文間違い修正
            SQL.Append(" ORDER BY")
            Select Case MEISAI_OUT
                Case "0"        '学年・クラス・生徒番号
                    SQL.Append(" SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.CLASS_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_NO_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case "1"        '入学年度・通番
                    SQL.Append(" SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case Else       'あいうえお
                    SQL.Append(" SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_KNAME_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
            End Select

            'Select Case MEISAI_OUT
            '    Case "0"    '学年・クラス・生徒番号
            '        SQL.Append(",G_MEIMAST.GAKUNEN_CODE_O")
            '        SQL.Append(",G_MEIMAST.CLASS_CODE_O")
            '        SQL.Append(",G_MEIMAST.SEITO_NO_O")
            '        SQL.Append(",G_MEIMAST.NENDO_O")
            '        SQL.Append(",G_MEIMAST.TUUBAN_O")
            '    Case "1"    '入学年度・通番
            '        SQL.Append(",G_MEIMAST.GAKUNEN_CODE_O")
            '        SQL.Append(",G_MEIMAST.NENDO_O")
            '        SQL.Append(",G_MEIMAST.TUUBAN_O")
            '    Case Else   'あいうえお
            '        SQL.Append(",G_MEIMAST.GAKUNEN_CODE_O")
            '        SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            '        SQL.Append(",G_MEIMAST.NENDO_O")
            '        SQL.Append(",G_MEIMAST.TUUBAN_O")
            'End Select
            '2011/03/01 TASK)saitou MOD --------------------<

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(TKIN_NO, True)                                                '取扱金融機関コード
                    OutputCsvData(TSIT_NO, True)                                                '取扱支店コード
                    OutputCsvData(KIN_NAME, True)                                               '取扱金融機関名
                    OutputCsvData(SIT_NAME, True)                                               '取扱支店名
                    OutputCsvData(ITAKU_CODE, True)                                             '委託者コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")), True)      '委託者名
                    Select Case NS_KBN                                                          '入出金区分
                        Case "0"
                            OutputCsvData("入金", True)
                        Case "1"
                            OutputCsvData("出金", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(TORIMATOME_SIT, True)                                         '取りまとめ店
                    OutputCsvData(TORIMATOME_NAME, True)                                        '取りまとめ店名
                    OutputCsvData(GAKKOU_CODE, True)                                            '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_O")), True)           '契約支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)         '契約支店名
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_O"))                     '科目
                        Case "02"
                            OutputCsvData("普通", True)
                        Case "01"
                            OutputCsvData("当座", True)
                        Case "37"
                            OutputCsvData("職員", True)
                        Case "05"
                            OutputCsvData("納税", True)
                        Case "04"
                            OutputCsvData("別段", True)
                        Case "99"
                            OutputCsvData("諸勘定", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O")), True)             '口座番号
                    If GCOM.NzStr(oraReader.GetString("MEIGI_NNAME_O")) = "" Then             '名義人名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_NNAME_O")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")), True)             '入学年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")), True)            '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")), True)      '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O")), True)        'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")), True)          '学年コード
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")) = "" Then               '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True, True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True, True)
                    End If

                    AllRecordCnt += 1
                    RecordCnt += 1

                    oraReader.NextRead()
                End While
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = 0
                Return True
            End If
        Catch ex As Exception
            Ret = -300
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        End Try
    End Function

End Class
