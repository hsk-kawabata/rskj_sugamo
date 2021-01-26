Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections

' 資金決済処理結果確認表
Class ClsPrnKessaiSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    ' ログ処理クラス
    Private LOG As New CASTCommon.BatchLOG("資金決済データ作成", "KESSAI")

    Public CsvData(43) As String

    Private BasePath As String              ' INIファイルの資金決済ファイルパス
    Private KessaiFileName As String        ' 資金決済ファイル名
    Private strKESSAI_DATE As String        ' 決済日

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP001"

        ' 定義体名セット
        ReportBaseName = "KFKP001_処理結果確認表(資金決済).rpd"

        CsvData(0) = "00010101"                                     ' 処理日
        CsvData(1) = "00010101"                                     ' 決済日
        CsvData(2) = "00010101"                                     ' タイムスタンプ
        CsvData(3) = ""                                             ' 取引先主コード
        CsvData(4) = ""                                             ' 取引先副コード
        CsvData(5) = ""                                             ' 取引先名
        CsvData(6) = ""                                             ' 振替コード
        CsvData(7) = ""                                             ' 企業コード
        CsvData(8) = "00010101"                                     ' 振替日
        CsvData(9) = ""                                             ' 決済区分（漢字）
        CsvData(10) = ""                                            ' 決済金融機関コード
        CsvData(11) = ""                                            ' 決済金融機関支店コード
        CsvData(12) = ""                                            ' 決済科目
        CsvData(13) = ""                                            ' 決済口座番号
        CsvData(14) = ""                                            ' 手数料徴求区分
        CsvData(15) = ""                                            ' 手数料徴求方法
        CsvData(16) = ""                                            ' 取りまとめ店コード
        CsvData(17) = ""                                            ' 取りまとめ店名
        CsvData(18) = "0"                                           ' 請求件数
        CsvData(19) = "0"                                           ' 請求金額
        CsvData(20) = "0"                                           ' 不能件数
        CsvData(21) = "0"                                           ' 不能金額
        CsvData(22) = "0"                                           ' 振替件数
        CsvData(23) = "0"                                           ' 振替金額
        CsvData(24) = "0"                                           ' 手数料
        CsvData(25) = "0"                                           ' 手数料内訳－自振
        CsvData(26) = "0"                                           ' 手数料内訳－振込
        CsvData(27) = "0"                                           ' 手数料内訳－その他
        CsvData(28) = "0"                                           ' 入金分件数
        CsvData(29) = "0"                                           ' 入金分金額
        CsvData(30) = ""                                            ' 科目コード
        CsvData(31) = ""                                            ' オペコード
        CsvData(32) = ""                                            ' オペレーション名
        CsvData(33) = "0"                                           ' 入出金額（オペコード単位）
        CsvData(34) = "0"                                           ' 手数料額（オペコード単位）
        CsvData(35) = "0"                                           ' 集計フラグ
        CsvData(36) = "0"                                           ' リエンタ作成区分
        CsvData(37) = ""                                            ' 決済金融機関名
        CsvData(38) = ""                                            ' 決済支店名
        CsvData(39) = ""
        CsvData(40) = ""                                            ' 本部別段口座番号
        CsvData(41) = ""                                            ' 任意項目
        CsvData(42) = "0"                                           ' レコード番号
        CsvData(43) = "1"                                           ' 作成区分(1:金バッチ)
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("決済日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("振替日")
        CSVObject.Output("決済区分")
        CSVObject.Output("決済金融機関コード")
        CSVObject.Output("決済支店コード")
        CSVObject.Output("決済科目")
        CSVObject.Output("決済口座番号")
        CSVObject.Output("手数料徴求区分")
        CSVObject.Output("手数料徴求方法")
        CSVObject.Output("とりまとめ店コード")
        CSVObject.Output("とりまとめ店名")
        CSVObject.Output("請求件数")
        CSVObject.Output("請求金額")
        CSVObject.Output("不能件数")
        CSVObject.Output("不能金額")
        CSVObject.Output("振替件数")
        CSVObject.Output("振替金額")
        CSVObject.Output("手数料")
        CSVObject.Output("自振手数料")
        CSVObject.Output("振込手数料")
        CSVObject.Output("その他手数料")
        CSVObject.Output("入金件数")
        CSVObject.Output("入金金額")
        CSVObject.Output("科目コード")
        CSVObject.Output("オペコード")
        CSVObject.Output("オペレーション名")
        CSVObject.Output("入出金額")
        CSVObject.Output("手数料額")
        CSVObject.Output("集計フラグ")
        CSVObject.Output("作成区分")
        CSVObject.Output("決済金融機関名")
        CSVObject.Output("決済支店名")
        CSVObject.Output("メッセージ")
        CSVObject.Output("本部別段口座番号")
        CSVObject.Output("任意項目")
        CSVObject.Output("レコード番号")
        CSVObject.Output("作成区分", False, True)
        Return file

    End Function

    ''' <summary>
    ''' 処理結果確認表(資金決済データ作成)のCSVを出力します。
    ''' </summary>
    ''' <param name="ary"></param>
    ''' <param name="jikinko"></param>
    ''' <param name="strDate"></param>
    ''' <param name="strTime"></param>
    ''' <param name="strKessaiDate"></param>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' <remarks>2013/09/17 大垣信金 金バッチ対応</remarks>
    Public Function OutputCSVKekka(ByVal ary As ArrayList, _
                                   ByVal jikinko As String, _
                                   ByVal strDate As String, _
                                   ByVal strTime As String, _
                                   ByVal strKessaiDate As String, _
                                   ByVal db As CASTCommon.MyOracle, _
                                   ByVal aryMSG As ArrayList, _
                                   ByRef aryHimokuName As ArrayList) As Integer

        Dim KData As CAstFormKes.ClsFormKes.KessaiDataKinBatch = Nothing
        Dim YData As ClsKessaiDataCreateKinBatch.msgDATA = Nothing
        Dim newkey As String = ""
        Dim oldkey As String = ""

        Dim cnt As Integer = ary.Count - 1 'ループ回数

        Dim lngSagaku As Long
        Dim strNyuukin As String

        Try

            For i As Integer = 0 To cnt

                KData.Init()

                KData = CType(ary.Item(i), CAstFormKes.ClsFormKes.KessaiDataKinBatch)

                YData.Init()
                YData = CType(aryMSG.Item(i), ClsKessaiDataCreateKinBatch.msgDATA)

                newkey = KData.TorisCode & KData.TorifCode & KData.FuriDate

                ' ＣＳＶデータ設定
                CsvData(0) = strDate                  ' 処理日
                CsvData(1) = strKessaiDate                              ' 基準日
                CsvData(2) = strTime                                    ' タイムスタンプ
                CsvData(3) = KData.TorisCode                             ' 取引先主コード
                CsvData(4) = KData.TorifCode                             ' 取引先副コード
                CsvData(5) = KData.ToriNName.Trim                        ' 取引先名
                CsvData(6) = KData.FuriCode                              ' 振替コード
                CsvData(7) = KData.KigyoCode                             ' 企業コード
                CsvData(8) = KData.FuriDate                                     ' 振替日

                Select Case KData.KessaiKbn
                    Case "00"
                        CsvData(9) = "預け金"
                    Case "01"
                        CsvData(9) = "口座入金"                                            ' 決済区分（漢字）
                    Case "02"
                        CsvData(9) = "為替振込"
                    Case "03"
                        CsvData(9) = "為替付替"
                    Case "04"
                        CsvData(9) = "別段出金のみ"
                    Case "05"
                        CsvData(9) = "特別企業"
                    Case "99"
                        CsvData(9) = "決済対象外"
                End Select

                CsvData(10) = KData.KesKinCode                                            ' 決済金融機関コード
                CsvData(11) = KData.KesSitCode                                            ' 決済金融機関支店コード
                CsvData(12) = KData.KesKamoku                                            ' 決済科目
                CsvData(13) = KData.KesKouza                                ' 決済口座番号

                Select Case KData.TesTyoKbn
                    Case "0"    '都度徴求の場合
                        CsvData(14) = "都度徴求"                               ' 手数料徴求区分
                    Case "1"    '一括徴求の場合
                        CsvData(14) = "一括徴求"
                    Case "2"    '特別免除の場合
                        CsvData(14) = "特別免除"

                    Case "3"    '別途徴求の場合
                        CsvData(14) = "別途徴求"
                    Case Else
                        CsvData(14) = "手数料未徴求"
                End Select

                Select Case KData.TesTyohh
                    Case "0"
                        CsvData(15) = "差引"                               ' 手数料徴求方法
                    Case "1"
                        CsvData(15) = "直接"                               ' 手数料徴求方法
                    Case Else
                        CsvData(15) = ""                                    ' 手数料徴求方法
                End Select

                CsvData(16) = KData.TorimatomeSit                           ' 取りまとめ店コード
                CsvData(17) = GetTenmast(jikinko, KData.TorimatomeSit, MainDB)                                            ' 取りまとめ店名
                CsvData(18) = KData.SyoriKen.Trim                                            ' 請求件数
                CsvData(19) = KData.Syorikin.Trim                                            ' 請求金額
                CsvData(20) = KData.FunouKen.Trim                                            ' 不能件数
                CsvData(21) = KData.FunouKin.Trim                                            ' 不能金額
                CsvData(22) = KData.FuriKen.Trim                                            ' 振替件数
                CsvData(23) = KData.FuriKin.Trim                                           ' 振替金額
                CsvData(24) = KData.TesuuKin.Trim                                           ' 手数料
                CsvData(25) = KData.JifutiTesuuKin.Trim                                      ' 手数料内訳－自振
                CsvData(26) = KData.FurikomiTesuukin.Trim                                    ' 手数料内訳－振込
                CsvData(27) = KData.SonotaTesuuKin.Trim                                     ' 手数料内訳－その他
                CsvData(28) = KData.NyukinKen.Trim                                           ' 入金分件数

                ' 入金分金額の設定
                ' 振替済金額 - 手数料金額 > 0 and 手数料徴求区分 = 0：都度徴求 and 手数料徴求方法=0：差引入金
                lngSagaku = CLng(KData.FuriKin.Trim) - CLng(KData.TesuuKin.Trim)
                If lngSagaku > 0 And KData.TesTyoKbn = "0" And KData.TesTyohh = "0" Then
                    strNyuukin = lngSagaku.ToString
                Else
                    strNyuukin = KData.FuriKin.Trim
                End If

                CsvData(29) = strNyuukin                                           ' 入金分金額
                CsvData(30) = KData.OpeCode.Substring(0, 2)                             ' 科目コード
                CsvData(31) = KData.OpeCode.Substring(2, 3)                             ' オペコード
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                Dim strBikou As String = ""
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                Select Case KData.OpeCode
                    Case "04099"
                        CsvData(32) = "別段支払"                                                        ' オペレーション名
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = "本部別段口座番号:" & KData.HonbuKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "01010"
                        CsvData(32) = "当座入金"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "02019"
                        CsvData(32) = "普通入金(NB)"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "04019"
                        CsvData(32) = "別段入金"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "99019"
                        CsvData(32) = "諸勘定入金"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        '手数料でない時のみセット
                        If CLng(KData.ope_nyukin.Trim) <> 0 Then
                            strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                            strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                            strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        End If
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "48100"
                        CsvData(32) = "為替振込"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "48500"
                        CsvData(32) = "為替付替"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "48600"
                        CsvData(32) = "為替請求"
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                    Case "99418"
                        CsvData(32) = "手数料徴求(連動)"
                    Case "99419"
                        CsvData(32) = "諸勘定連動入金"
                End Select

                CsvData(33) = KData.ope_nyukin.Trim                                          ' 入出金額（オペコード単位）
                CsvData(34) = KData.ope_tesuu.Trim                                           ' 手数料額（オペコード単位）

                If newkey = oldkey Then
                    CsvData(35) = "0"                                           ' 集計フラグ 集計しない
                Else
                    CsvData(35) = "1"                                          ' 集計フラグ 集計する
                End If

                Select Case KData.ToriKbn                     ' 0：資金決済と手数料徴求の両方の先、1：資金決済のみ先、2：手数料徴求のみ先
                    Case "0"
                        CsvData(36) = "決済・手数"
                    Case "1"
                        CsvData(36) = "決済"
                    Case "2"
                        CsvData(36) = "手数"
                End Select

                CsvData(37) = GetTenmast(KData.KesKinCode, "", MainDB, True)                        '決済金融機関名
                CsvData(38) = GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False)         '決済支店名
                CsvData(39) = YData.msg_DATA
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                '本部別段口座番号追加
                CsvData(40) = KData.HonbuKouza
                'オペコード単位の情報をオペコード毎に出力
                CsvData(41) = strBikou
                'レコード番号追加
                CsvData(42) = (i + 1).ToString
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                CsvData(43) = "1"

                'ＣＳＶ出力処理
                If CSVObject.Output(CsvData) = 0 Then
                    Return -1
                End If

                oldkey = KData.TorisCode & KData.TorifCode & KData.FuriDate

            Next

        Catch ex As Exception
            LOG.Write("資金決済処理結果確認表ＣＳＶ出力", "失敗", ex.Message)
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")

            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try



    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle, ByVal KIN As Boolean) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strKinName As String = ""

        Try
            If KIN = True Then
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' ORDER BY SIT_NO_N")
            Else
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")
            End If


            If orareader.DataReader(sql) = True Then
                If KIN = True Then
                    strKinName = orareader.GetString("KIN_KNAME_N")
                Else
                    strKinName = orareader.GetString("SIT_KNAME_N")
                End If
                Return strKinName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function
    
    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
    Private Function GetHimokuName(ByVal HimokuName As Object) As String
        Dim retHimokuName As String = CStr(HimokuName).Trim
        If retHimokuName = "" Then
            Return ""
        Else
            Return Microsoft.VisualBasic.StrConv(retHimokuName.PadRight(10, " "c), Microsoft.VisualBasic.VbStrConv.Wide).Substring(0, 10)
        End If
    End Function
    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

End Class
