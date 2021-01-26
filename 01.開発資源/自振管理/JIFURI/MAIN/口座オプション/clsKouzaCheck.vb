Imports CASTCommon
Imports System.Text
Imports MenteCommon.clsCommon

Public Class clsKouzaCheck

    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
    Private INI_RSV2_KNAMECHK As String
    Private INI_RSV2_JIFURICHK As String
    Private INI_RSV2_IKAN_KNAMECHK As String
    Private INI_RSV2_IKAN_JIFURICHK As String
    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

    Sub New()

    End Sub
    '
    ' 機能　　　: 口座チェック関数
    '
    ' 戻り値　　: 0:異常なし 1:口座なし            2:自振契約なし  3:名義人名(カナ)相違  4:自振契約無-名義人名(カナ)相違  5:解約済 -1:異常 (順次追加)
    '　　　　　　　  　　　    (移管後) 10:移管済 12:自振契約なし 13:名義人名(カナ)相違 14:自振契約無-名義人名(カナ)相違 15:解約済
    ' 引き数　　: ARG1 - 支店コード
    ' 　　　 　　 ARG2 - 科目コード（2桁）
    '             ARG3 - 口座番号
    '             ARG4 - 振替コード
    '             ARG5 - 企業コード
    '             ARG6 - 種別
    '             ARG7 - 契約者カナ氏名
    '             ARG9 - KDBinfo (0):名義人　(1):移管店番　(2):移管口番　(3):エラーコード（戻り値とは違う。結果書込用）　(4):エラーメッセージ

    ' 備考　　　: 
    '
    Public Function KouzaChk(ByVal SitCode As String, ByVal Kamoku As String, _
                             ByVal Kouza As String, ByVal FuriCode As String, _
                             ByVal KigyoCode As String, _
                             ByVal Syubetu As String, ByVal KokyakuName As String, _
                             ByRef KDBinfo As String(), ByVal MainDB As CASTCommon.MyOracle, Optional ByVal IkanCheck As Boolean = True) As Integer

        Dim Ret As Integer = -1

        Dim OraReader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim KDB_FuriCode As String = ""
        Dim KDB_KigyoCode As String = ""
        Dim KDB_KatuKouza As String = ""
        Dim KDB_Name As String = ""
        Dim KDB_NewSitNo As String = ""
        Dim KDB_NewKouza As String = ""

        Dim clsMente As New MenteCommon.clsCommon

        Dim iStatus As Integer = -1

        Try

            ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
            INI_RSV2_KNAMECHK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KNAMECHK")
            INI_RSV2_JIFURICHK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIFURICHK")
            INI_RSV2_IKAN_KNAMECHK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IKAN_KNAMECHK")
            INI_RSV2_IKAN_JIFURICHK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IKAN_JIFURICHK")
            ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

            SQL.Append("SELECT FURI_CODE_D,KIGYOU_CODE_D,KOKYAKU_KNAME_D,KATU_KOUZA_D")
            SQL.Append(" FROM KDBMAST")
            SQL.Append(" WHERE TSIT_NO_D = " & SQ(SitCode))
            SQL.Append(" AND KAMOKU_D = " & SQ(Kamoku))
            SQL.Append(" AND KOUZA_D = " & SQ(Kouza))
            SQL.Append(" ORDER BY KATU_KOUZA_D DESC")

            If OraReader.DataReader(SQL) = True Then

                '口座有(正常)
                iStatus = 0
                KDBinfo(3) = "0"
                KDBinfo(4) = ""

                KDB_KatuKouza = clsMente.NzStr(OraReader.GetString("KATU_KOUZA_D")).Trim

                'ここで活口座か決定、先頭が0だったら後続は全部ゼロだから解約済みで終わり
                If KDB_KatuKouza = "0" Then
                    iStatus = 5
                    KDBinfo(3) = "1"
                    KDBinfo(4) = "解約済"
                Else

                    Do Until OraReader.EOF

                        KDB_FuriCode = clsMente.NzStr(OraReader.GetString("FURI_CODE_D")).Trim
                        KDB_KigyoCode = clsMente.NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim
                        KDB_Name = clsMente.NzStr(OraReader.GetString("KOKYAKU_KNAME_D")).Trim

                        '振替コードが一致して、企業コードが一致して、活口座だったら自振契約有り
                        ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
                        'If KDB_FuriCode = FuriCode AndAlso KDB_KigyoCode = KigyoCode AndAlso KDB_KatuKouza = "1" Then
                        '    '自振契約あり　まだ正常

                        '    '口座有(正常)
                        '    iStatus = 0
                        '    KDBinfo(3) = "0"
                        '    KDBinfo(4) = ""

                        '    Exit Do
                        'End If

                        ''最後の最後まで抜けれなかったから自振契約なし
                        'iStatus = 2
                        'KDBinfo(3) = "2"
                        'KDBinfo(4) = "自振契約なし"
                        Select Case INI_RSV2_JIFURICHK
                            Case "NO"
                            Case Else
                                If KDB_FuriCode = FuriCode AndAlso KDB_KigyoCode = KigyoCode AndAlso KDB_KatuKouza = "1" Then
                                    '自振契約あり　まだ正常

                                    '口座有(正常)
                                    iStatus = 0
                                    KDBinfo(3) = "0"
                                    KDBinfo(4) = ""

                                    Exit Do
                                End If

                                '最後の最後まで抜けれなかったから自振契約なし
                                iStatus = 2
                                KDBinfo(3) = "2"
                                KDBinfo(4) = "自振契約なし"
                        End Select
                        ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

                        OraReader.NextRead()

                    Loop

                    ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
                    ''最後のレコードで持っていたKDB名義人名で相違チェック　これは全レコード違わないこと前提
                    'If KDB_Name.PadRight(30, " "c).Substring(0, 30) <> KokyakuName.PadRight(30, " "c) Then

                    '    'さらにここで自振契約でによってエラー内容が変わる
                    '    If iStatus = 0 Then
                    '        iStatus = 3
                    '        KDBinfo(3) = "2"
                    '        KDBinfo(4) = "名義人ｶﾅ相違"
                    '        KDBinfo(0) = KDB_Name
                    '    ElseIf iStatus = 2 Then
                    '        iStatus = 4
                    '        KDBinfo(3) = "2"
                    '        KDBinfo(4) = "自契無-名義人ｶﾅ相違"
                    '        KDBinfo(0) = KDB_Name
                    '    End If

                    'End If
                    '最後のレコードで持っていたKDB名義人名で相違チェック　これは全レコード違わないこと前提
                    Select Case INI_RSV2_KNAMECHK
                        Case "NO"
                        Case Else
                            If KDB_Name.PadRight(30, " "c).Substring(0, 30) <> KokyakuName.PadRight(30, " "c) Then
                                'さらにここで自振契約でによってエラー内容が変わる
                                If iStatus = 0 Then
                                    iStatus = 3
                                    KDBinfo(3) = "2"
                                    KDBinfo(4) = "名義人ｶﾅ相違"
                                    KDBinfo(0) = KDB_Name
                                ElseIf iStatus = 2 Then
                                    iStatus = 4
                                    KDBinfo(3) = "2"
                                    KDBinfo(4) = "自契無-名義人ｶﾅ相違"
                                    KDBinfo(0) = KDB_Name
                                End If
                            End If
                    End Select
                    ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

                End If
                'ここまできてistatusが2、4の人は自振契約作成対象
            Else

                iStatus = 1
                KDBinfo(3) = "1"
                KDBinfo(4) = "口座なし"

            End If

            OraReader.Close()

            '以後移管チェックに続く
            '口座なしか解約済なら移管前口番でぶつける
            If IkanCheck = True Then

                If iStatus = 1 OrElse iStatus = 5 Then

                    KDB_FuriCode = ""
                    KDB_KigyoCode = ""
                    KDB_KatuKouza = ""
                    KDB_Name = ""

                    OraReader = New MyOracleReader(MainDB)
                    SQL = New StringBuilder(128)

                    '初回と違うのは店番と口番を取得するとこ
                    SQL.Append("SELECT TSIT_NO_D,KOUZA_D,FURI_CODE_D,KIGYOU_CODE_D,KOKYAKU_KNAME_D,KATU_KOUZA_D")
                    SQL.Append(" FROM KDBMAST")
                    SQL.Append(" WHERE OLD_TSIT_NO_D = " & SQ(SitCode))
                    SQL.Append(" AND KAMOKU_D = " & SQ(Kamoku))
                    SQL.Append(" AND OLD_KOUZA_D = " & SQ(Kouza))
                    SQL.Append(" ORDER BY KATU_KOUZA_D DESC")

                    If OraReader.DataReader(SQL) = True Then

                        'この時点で移管済確定
                        iStatus = 10
                        KDBinfo(3) = "1"
                        KDBinfo(4) = "移管済"
                        KDBinfo(1) = OraReader.GetString("TSIT_NO_D")
                        KDBinfo(2) = OraReader.GetString("KOUZA_D")

                        KDB_KatuKouza = clsMente.NzStr(OraReader.GetString("KATU_KOUZA_D")).Trim

                        'ここで活口座か決定、先頭が0だったら後続は全部ゼロだから解約済みで終わり
                        If KDB_KatuKouza = "0" Then
                            '(移管後)解約済
                            iStatus = 15
                            KDBinfo(3) = "1"
                            KDBinfo(4) = "(移管)解約済"
                        Else

                            Do Until OraReader.EOF

                                KDB_NewSitNo = clsMente.NzStr(OraReader.GetString("TSIT_NO_D")).Trim
                                KDB_NewKouza = clsMente.NzStr(OraReader.GetString("KOUZA_D")).Trim
                                KDB_FuriCode = clsMente.NzStr(OraReader.GetString("FURI_CODE_D")).Trim
                                KDB_KigyoCode = clsMente.NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim
                                KDB_Name = clsMente.NzStr(OraReader.GetString("KOKYAKU_KNAME_D")).Trim

                                '振替コードが一致して、企業コードが一致して、活口座だったら自振契約有り
                                ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
                                'If KDB_FuriCode = FuriCode AndAlso KDB_KigyoCode = KigyoCode AndAlso KDB_KatuKouza = "1" Then
                                '    '自振契約あり

                                '    iStatus = 10
                                '    KDBinfo(3) = "1"
                                '    KDBinfo(4) = "移管済"

                                '    Exit Do
                                'End If

                                ''最後の最後まで抜けれなかったから(移管後)自振契約なし
                                'iStatus = 12
                                'KDBinfo(3) = "2"
                                'KDBinfo(4) = "(移管)自振契約なし"
                                Select Case INI_RSV2_IKAN_JIFURICHK
                                    Case "NO"
                                    Case Else
                                        If KDB_FuriCode = FuriCode AndAlso KDB_KigyoCode = KigyoCode AndAlso KDB_KatuKouza = "1" Then
                                            '自振契約あり

                                            iStatus = 10
                                            KDBinfo(3) = "1"
                                            KDBinfo(4) = "移管済"

                                            Exit Do
                                        End If

                                        '最後の最後まで抜けれなかったから(移管後)自振契約なし
                                        iStatus = 12
                                        KDBinfo(3) = "2"
                                        KDBinfo(4) = "(移管)自振契約なし"
                                End Select
                                ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

                                OraReader.NextRead()

                            Loop

                            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- START
                            ''最後のレコードで持っていたKDB名義人名で相違チェック　これは全レコード違わないこと前提
                            'If KDB_Name.PadRight(30, " "c).Substring(0, 30) <> KokyakuName.PadRight(30, " "c) Then

                            '    'さらにここで自振契約をむしかえす
                            '    If iStatus = 10 Then
                            '        '(移管後)名義人名(カナ)相違
                            '        iStatus = 13
                            '        KDBinfo(3) = "2"
                            '        KDBinfo(4) = "(移管)名義人ｶﾅ相違"
                            '        KDBinfo(0) = KDB_Name
                            '    ElseIf iStatus = 12 Then
                            '        '(移管後)自振契約無-名義人名(カナ)相違
                            '        iStatus = 14
                            '        KDBinfo(3) = "2"
                            '        KDBinfo(4) = "(移管)自契無-名義人ｶﾅ相違"
                            '        KDBinfo(0) = KDB_Name
                            '    End If

                            'End If
                            '最後のレコードで持っていたKDB名義人名で相違チェック　これは全レコード違わないこと前提
                            Select Case INI_RSV2_IKAN_KNAMECHK
                                Case "NO"
                                Case Else
                                    If KDB_Name.PadRight(30, " "c).Substring(0, 30) <> KokyakuName.PadRight(30, " "c) Then
                                        'さらにここで自振契約をむしかえす
                                        If iStatus = 10 Then
                                            '(移管後)名義人名(カナ)相違
                                            iStatus = 13
                                            KDBinfo(3) = "2"
                                            KDBinfo(4) = "(移管)名義人ｶﾅ相違"
                                            KDBinfo(0) = KDB_Name
                                        ElseIf iStatus = 12 Then
                                            '(移管後)自振契約無-名義人名(カナ)相違
                                            iStatus = 14
                                            KDBinfo(3) = "2"
                                            KDBinfo(4) = "(移管)自契無-名義人ｶﾅ相違"
                                            KDBinfo(0) = KDB_Name
                                        End If
                                    End If
                            End Select
                            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-10,11(RSV2対応<小浜信金>) -------------------- END

                        End If

                        'ここまできてistatusが12、14の人は自振契約作成対象

                    Else

                        '初回チェックの情報を引き継げばいいのでなにもセットしない
                        'iStatus = 1
                        'KDBinfo(3) = "1"
                        'KDBinfo(4) = "口座なし"

                    End If

                    OraReader.Close()

                End If
            End If

            Return iStatus

        Catch ex As Exception
            Throw
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function

End Class
