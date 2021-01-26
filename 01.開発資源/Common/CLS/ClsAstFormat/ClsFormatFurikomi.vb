Option Strict On
Option Explicit On

Imports CASTCommon.ModPublic

' 全銀 データ（振込）フォーマットクラス
Public Class CFormatFurikomi
    ' データフォーマット基本クラス
    Inherits CFormatZengin

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 120

    '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal len As Integer)
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = len

    End Sub
    '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END

    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Public Overrides Function CheckRecord1() As String
        Dim sRet As String = MyBase.CheckRecord1

        If sRet <> "H" Then
            Return sRet
        End If

        If sRet <> "ERR" Then
            If MyBase.CheckHeaderRecord(1) = False Then
                Return "ERR"
            End If
        End If

        ' 全銀振込フォーマット独自チェック
        If Not mInfoComm Is Nothing Then
            ' 入出金区分 種別コードチェック
            Select Case mInfoComm.INFOToriMast.NS_KBN_T
                Case "1"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "11", "12", "21", "41", "43", "44", "45", "71", "72"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ入出金区分、種別", "不一致", "種別：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ入出金区分、種別不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "9"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "91"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ入出金区分、種別", "不一致", "種別：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ入出金区分、種別不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
            End Select

            '  種別コードチェック
            Select Case mInfoComm.INFOToriMast.SYUBETU_T
                Case "91"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "91"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "21"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "21", "41", "43", "44", "45"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "12"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "12"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "11"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "11"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
            End Select

            '2018/10/07 saitou 広島信金(RSV2標準) DEL （ヘッダ口座情報チェック対応） ------------------ START
            'ここで異常を返しても、次のヘッダレコードのチェックで結果を塗り替えられてしまうため、処理を移動。
            ''2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（口座情報チェック）------------------ START
            'If INI_S_KDBMAST_CHK = "1" AndAlso mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" Then
            '    Dim bRet As Boolean = ChkKDBMAST(InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
            '    If bRet = False Then
            '        Dim MSG As String = String.Format("支店コード：{0} 科目：{1} 口座：{2}", InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
            '        WriteBLog("ヘッダ口座情報チェック", "口座なし", MSG)
            '        DataInfo.Message = "ヘッダ口座情報チェック不一致 " & MSG

            '        Dim InError As INPUTERROR = Nothing
            '        InError.ERRINFO = "口座情報なし(ヘッダー)"
            '        InErrorArray.Add(InError)

            '        Return "IJO"
            '    End If
            'End If
            ''2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（口座情報チェック）------------------ END
            '2018/10/07 saitou 広島信金(RSV2標準) DEL ------------------------------------------------- END
        End If

        Return "H"
    End Function

    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Protected Overrides Function CheckRecord2() As String
        Dim sRet As String = MyBase.CheckRecord2

        If sRet <> "D" Then
            Return sRet
        End If

        Return "D"
    End Function

    Protected Overrides Function CheckDataRecord() As Boolean
        Dim InError As INPUTERROR = Nothing
        '2018/10/04 saitou 広島信金(RSV2標準) ADD （金額0円チェック） ------------------------------ START
        Dim INI_RSV2_DATA_KINGAKUZERO As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KINGAKUZERO")
        '2018/10/04 saitou 広島信金(RSV2標準) ADD -------------------------------------------------- END

        InError.DATA = InfoMeisaiMast

        InErrorArray = New ArrayList

        If mInfoComm Is Nothing Then
            Return True
        End If

        '支店・口座読替対応(変更前情報の保持)
        InfoMeisaiMast.OLD_KIN_NO = InfoMeisaiMast.KEIYAKU_KIN
        InfoMeisaiMast.OLD_SIT_NO = InfoMeisaiMast.KEIYAKU_SIT
        InfoMeisaiMast.OLD_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA

        '規定外文字チェックをインプットエラーとして出力
        Dim kiteiRem As Long = CheckRegularString()

        If kiteiRem <> -1 Then
            ' 規定外文字異常
            InfoMeisaiMast.FURIKETU_CODE = 9
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kiteigaimoji)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 規定外文字：" & kiteiRem & "バイト目"
        End If

        '金融機関マスタ存在チェックを行うかどうかを判定するフラグ
        Dim TenMastExistCheck_Flg As Boolean = True

        '金融機関コード数値チェック
        If IsDecimal(InfoMeisaiMast.KEIYAKU_KIN) = False OrElse _
            InfoMeisaiMast.KEIYAKU_KIN.Equals("0000") = True OrElse _
            InfoMeisaiMast.KEIYAKU_KIN.Equals("9999") = True Then
            ' 銀行コード数値異常
            InfoMeisaiMast.FURIKETU_CODE = 9

            '銀行コード異常
            InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN

            '数値異常の場合、フラグをFALSEにする
            TenMastExistCheck_Flg = False

        ElseIf IsDecimal(InfoMeisaiMast.KEIYAKU_SIT) = False OrElse _
            InfoMeisaiMast.KEIYAKU_SIT.Equals("999") = True Then

            'ゆうちょ銀行の場合は"000"と"999"を異常としない
            If InfoMeisaiMast.KEIYAKU_KIN.Equals("9900") = False Then
                ' 店番数値異常
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Tenban)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 支店コード：" & InfoMeisaiMast.KEIYAKU_SIT

                '数値異常の場合、フラグをFALSEにする
                TenMastExistCheck_Flg = False
            End If
        End If

        '科目コードチェック
        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "11", "12"
                '科目チェックに9を許す
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 9
                    Case Else
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- START
                        '' 科目異常
                        'InfoMeisaiMast.FURIKETU_CODE = 9

                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        Select Case mInfoComm.INFOToriMast.SYUBETU_T
                            Case "11"
                                If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_11").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' 科目異常
                                    InfoMeisaiMast.FURIKETU_CODE = 9

                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                            Case "12"
                                If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_12").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' 科目異常
                                    InfoMeisaiMast.FURIKETU_CODE = 9

                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                        End Select
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- END
                End Select

            Case "21"
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 4, 9
                    Case Else
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- START
                        '' 科目異常
                        'InfoMeisaiMast.FURIKETU_CODE = 9

                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_21").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' 科目異常
                            InfoMeisaiMast.FURIKETU_CODE = 9

                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- END
                End Select
        End Select

        ' 新規コードチェック
        '新規コードに空白を許す
        Select Case InfoMeisaiMast.SINKI_CODE
            Case " ", "0", "1", "2"
            Case Else
                ' 新規コード異常
                InfoMeisaiMast.FURIKETU_CODE = 9
                InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 新規コード：" & InfoMeisaiMast.SINKI_CODE
        End Select

        Dim KouzaCheck As Boolean = True

        '口座番号内のハイフンは省き、頭0埋めする
        InfoMeisaiMast.KEIYAKU_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA.Replace("-"c, "").PadLeft(7, "0"c)
 
        '科目が9かつ口座番号がALL9の時は口座チェックなし
        Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
            Case 9
                Select Case InfoMeisaiMast.KEIYAKU_KOUZA.Trim
                    Case "9999999"
                        KouzaCheck = False
                End Select
        End Select

        '口座番号チェックディジットチェック処理時も、KOUZACHECKフラグを参照するよう修正
        If KouzaCheck = True Then
            If mCheckDigitFlag = "1" Then
                '口座番号チェックデジットチェック
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    If CheckDigitCheck() = False Then
                        ' 口座番号異常
                        InfoMeisaiMast.FURIKETU_CODE = 9
                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
                End If
            End If
        End If
        
        If KouzaCheck = True Then
            If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA) = False Then
                ' 口座番号異常
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                KouzaCheck = False
            End If
        End If

        '口座番号ALL0は不能事由コード2をセットする
        If KouzaCheck = True AndAlso InfoMeisaiMast.KEIYAKU_KOUZA = "0000000" Then
            ' 口座番号異常
            Select Case InfoMeisaiMast.KEIYAKU_KOUZA
                Case "0000000"
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
            End Select
        End If

        '支店読み替え
        With InfoMeisaiMast
            If CASTCommon.GetFSKJIni("YOMIKAE", "TENPO") = "1" Then
                '支店読み替え対象
                Call fn_TENPO_YOMIKAE(.KEIYAKU_KIN, .KEIYAKU_SIT, .KEIYAKU_KIN, .KEIYAKU_SIT)
            End If
        End With

        '口座読み替え
        With InfoMeisaiMast
            If CASTCommon.GetFSKJIni("YOMIKAE", "KOUZA") = "1" Then
                '支店読み替え対象
                Call fn_KOUZA_YOMIKAE(.KEIYAKU_SIT, .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                                .KEIYAKU_SIT, .KEIYAKU_KOUZA, .IDOU_DATE)
            End If
        End With

        '金融機関コード数値チェックでエラーとなった場合、金融機関存在チェックは行わない

        '金融機関コード存在チェック
        Dim nRet As Integer
        ' 2016/10/18 タスク）綾部 CHG 【PG】UI_11-1-15(飯田信金<振込依頼データの金融機関・支店名編集対応>) -------------------- START
        'If TenMastExistCheck_Flg = True Then
        '    nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
        'Else '金融機関コード数値チェック失敗
        '    nRet = 9
        'End If
        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SFURI_TENNAME") = "1" Then
            Select Case mInfoComm.INFOToriMast.FSYORI_KBN_T
                Case "3"
                    nRet = CAstExternal.GetTENMASTExistsCustom(OraDB, _
                                                               InfoMeisaiMast.KEIYAKU_KIN, _
                                                               InfoMeisaiMast.KEIYAKU_SIT, _
                                                               InfoMeisaiMast2.KEIYAKU_KIN_KNAME, _
                                                               InfoMeisaiMast2.KEIYAKU_SIT_KNAME, _
                                                               InfoMeisaiMast.FURIKAE_DATE, _
                                                               InfoMeisaiMast.YOBI1, _
                                                               InfoMeisaiMast.YOBI2)
                    If TenMastExistCheck_Flg = False Then
                        '金融機関コード数値チェック失敗
                        nRet = 9
                    End If
                Case Else
                    If TenMastExistCheck_Flg = True Then
                        nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
                    Else '金融機関コード数値チェック失敗
                        nRet = 9
                    End If
            End Select
        Else
            If TenMastExistCheck_Flg = True Then
                nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
            Else '金融機関コード数値チェック失敗
                nRet = 9
            End If
        End If
        ' 2016/09/01 タスク）綾部 CHG 【PG】UI_11-1-15(飯田信金<振込依頼データの金融機関・支店名編集対応>) -------------------- END

        '金融機関取得処理時のエラー制御を変更(以下の通り)
        '===================================================================
        '0:金融機関取得失敗(GetTENMASTExistで例外発生)
        '1:金融機関あり支店なし
        '2:金融機関あり支店あり(正常終了)
        '3:振込日が削除日より後(店舗統廃合)
        '9:金融機関コード数値チェック失敗
        '===================================================================
        Select Case nRet
            Case 0 '金融機関なしの場合
                '金融機関なし
                InfoMeisaiMast.FURIKETU_CODE = 2
                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
            Case 1 '金融機関あり，支店なし
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    '自行で支店なしの場合は自行店番異常
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    
                    InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT

                Else
                    '他行で支店なしの場合は他行店番異常
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                End If
            Case 2 '金融機関あり，支店あり
                '自行で店番000の場合は自行店番異常
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO AndAlso InfoMeisaiMast.KEIYAKU_SIT = "000" Then

                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
               
                    '正常の場合、コードと名称の整合性が取れているかチェックする
                ElseIf Not CheckTenMast(InfoMeisaiMast, InfoMeisaiMast2, InError.ERRINFO) Then
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- START
                    InfoMeisaiMast.KinTenSoui = True
                    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- END
                End If

                '正常終了のため処理なし

            Case 3 '振込日が削除日より後(店舗統廃合)
                InfoMeisaiMast.FURIKETU_CODE = 2
                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
            Case 9 '金融機関コード数値チェックで失敗した場合
            Case Else '例外
        End Select

        '受取人名が空白の場合はエラーとする
        If InfoMeisaiMast.KEIYAKU_KNAME.Trim = "" Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kana)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 受取人名なし"
        End If

        '金額チェック
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
        Else
            If InfoMeisaiMast.FURIKIN < 0 Then
                ' マイナス金額
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
            End If

            '金額０円
            '2018/10/04 saitou 広島信金(RSV2標準) UPD （金額0円チェック） ------------------------------ START
            If INI_RSV2_DATA_KINGAKUZERO = "1" Then
                If InfoMeisaiMast.FURIKIN = 0 Then
                    InfoMeisaiMast.FURIKETU_CODE = 9

                    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
                End If
            End If
            'If InfoMeisaiMast.FURIKIN = 0 Then
            '    InfoMeisaiMast.FURIKETU_CODE = 9

            '    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
            '    InErrorArray.Add(InError)
            '    DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
            'End If
            '2018/10/04 saitou 広島信金(RSV2標準) UPD -------------------------------------------------- END
        End If

        '2018/03/15 saitou 広島信金(RSV2標準) DEL 振込手数料計算処理削除 ------------------------------ START
        '消費税対応が入る前の旧ロジックなので、標準版より削除。（この処理により意図しない結果が発生しないように）
        '落し込み時に手数料の計算を行いたかったら、落し込みの明細マスタ作成時に計算ロジックを埋め込むように、
        'カスタマイズ対応を行う。
        'If InfoMeisaiMast.FURIKETU_CODE = 0 Then
        '    'エラーが無ければ振込手数料計算
        '    If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
        '        '振込金融機関が，自金庫の場合
        '        If InfoMeisaiMast.KEIYAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T Then
        '            ' 振込支店がとりまとめ店と一致する場合，自店内
        '            If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '                '０円より大きい かつ １万円未満
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A1_T
        '            ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '                '１万円以上 かつ ３万円未満
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A2_T
        '            ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '                '３万円以上
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A3_T
        '            End If
        '        Else
        '            '振込支店がとりまとめ店と一致しない場合，本支店
        '            If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '                '０円より大きい かつ １万円未満
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B1_T
        '            ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '                '１万円以上 かつ ３万円未満
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B2_T

        '            ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '                '３万円以上
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B3_T
        '            End If
        '        End If
        '    Else
        '        '他行
        '        If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '            '０円より大きい かつ １万円未満
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C1_T
        '        ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '            '０円より大きい かつ ３万円未満
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C2_T
        '        ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '            '３万円以上
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C3_T
        '        End If
        '    End If
        'End If
        '2018/03/15 saitou 広島信金(RSV2標準) DEL ----------------------------------------------------- END

        InErrorArray.TrimToSize()

        If InErrorArray.Count > 0 Then
            Return False
        End If
        Return True
    End Function

    Private Function CheckTenMast(ByVal InfMei As CAstFormat.CFormat.MEISAI, ByRef InfMei2 As CAstFormat.CFormat.MEISAI2, ByRef ERRINFO As String) As Boolean

        Dim ret As Boolean = False
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Dim KinName As String = ""
        Dim RyakuKinName As String = ""
        Dim SitName As String = ""

        Try
            ' 2016/01/28 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            ' ＤＢ接続が存在しない場合，処理を行わない(標準バグ修正)
            If OraDB Is Nothing Then
                Return False
            End If
            ' 2016/01/28 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

            '2016/12/16 saitou RSV2 ADD 金融機関名チェック有無対応 ---------------------------------------- START
            '金融機関名チェック有無が0(しない)の場合のみ処理を正常で抜ける
            Dim KINNAMECHK As String = CASTCommon.GetFSKJIni("KAWASE", "KINNAMECHK")
            If KINNAMECHK.Equals("0") = True Then
                Return True
            End If
            '2016/12/16 saitou RSV2 ADD ------------------------------------------------------------------- END

            '金融機関名称チェック
            '金融機関コードで検索してｶﾅ名が一致するかどうか
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.AppendLine(" SELECT KIN_KNAME_N, RYAKU_KIN_KNAME_N FROM KIN_INFOMAST")
            SQL.AppendLine(" WHERE KIN_NO_N = " & SQ(InfMei.KEIYAKU_KIN))
            '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- START
            SQL.AppendLine(" ORDER BY KIN_FUKA_N DESC ")
            '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

            '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
            Dim ReplaceKinName As String = InfMei2.KEIYAKU_KIN_KNAME
            For Each de As DictionaryEntry In ReplaceKinNamePattern
                If ReplaceKinName.IndexOf(de.Key.ToString) <> -1 Then
                    ReplaceKinName = ReplaceKinName.Replace(de.Key.ToString, de.Value.ToString)
                    'Exit For
                End If
            Next
            '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

            If OraReader.DataReader(SQL) Then
                '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- START
                InfMei2.TENMAST_SIT_KNAME = OraReader.GetString("KIN_KNAME_N")
                '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

                While OraReader.EOF = False
                    KinName = OraReader.GetString("KIN_KNAME_N")
                    RyakuKinName = OraReader.GetString("RYAKU_KIN_KNAME_N")

                    '金融機関名が一致したら正常
                    If KinName = InfMei2.KEIYAKU_KIN_KNAME.Trim OrElse
                        (RyakuKinName <> "" AndAlso (RyakuKinName = InfMei2.KEIYAKU_KIN_KNAME.Trim)) Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
                    ElseIf KinName = ReplaceKinName.Trim OrElse
                        (RyakuKinName <> "" AndAlso (RyakuKinName = ReplaceKinName.Trim)) Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END
                    End If

                    OraReader.NextRead()
                End While
            End If

            If ret = False Then
                '金融機関名で一致しない場合
                ERRINFO = "銀行名相違(正常扱)"
                Return False
            End If

            ret = False

            '支店名称チェック
            '金融機関コード、支店コードで検索
            OraReader.Close()

            SQL.Length = 0
            SQL.AppendLine(" SELECT SIT_KNAME_N FROM SITEN_INFOMAST")
            SQL.AppendLine(" WHERE KIN_NO_N = " & SQ(InfMei.KEIYAKU_KIN))
            SQL.AppendLine(" AND SIT_NO_N = " & SQ(InfMei.KEIYAKU_SIT))
            '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- START
            SQL.AppendLine(" ORDER BY KIN_FUKA_N DESC ,SIT_FUKA_N ASC ")
            '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- END


            '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
            Dim ReplaceSitName As String = InfMei2.KEIYAKU_SIT_KNAME
            For Each de As DictionaryEntry In ReplaceSitNamePattern
                If ReplaceSitName.IndexOf(de.Key.ToString) <> -1 Then
                    ReplaceSitName = ReplaceSitName.Replace(de.Key.ToString, de.Value.ToString)
                    'Exit For
                End If
            Next
            '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

            If OraReader.DataReader(SQL) Then
                '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- START
                InfMei2.TENMAST_SIT_KNAME = OraReader.GetString("SIT_KNAME_N")
                '2018/10/02 maeda 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

                While OraReader.EOF = False
                    SitName = OraReader.GetString("SIT_KNAME_N")

                    '支店名が一致したら正常
                    If SitName = InfMei2.KEIYAKU_SIT_KNAME.Trim Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
                    ElseIf SitName = ReplaceSitName.Trim Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END
                    End If

                    OraReader.NextRead()
                End While
            End If

            If ret = False Then
                '支店名で一致しない場合
                ERRINFO = "支店名相違(正常扱)"
                Return False
            End If

        Catch
            Throw
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return ret
    End Function

    '2018/10/07 saitou 広島信金(RSV2標準) DEL （ヘッダ口座情報チェック対応） ------------------ START
    '処理移動のため関数不要。
    '''' <summary>
    '''' 口座情報マスタに指定口座が存在するかチェックする
    '''' </summary>
    '''' <param name="astrSIT_NO">支店コード</param>
    '''' <param name="astrKAMOKU">科目コード</param>
    '''' <param name="astrKOUZA">口座番号</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function ChkKDBMAST(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String) As Boolean
    '    Dim SQL As New System.Text.StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader = Nothing
    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(OraDB)

    '        astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

    '        SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
    '        SQL.Append(" WHERE OLD_TSIT_NO_D = '" & astrSIT_NO & "'")
    '        SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
    '        SQL.Append(" AND OLD_KOUZA_D = '" & astrKOUZA & "'")

    '        return OraReader.DataReader(SQL)

    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then OraReader.Close()
    '    End Try

    'End Function
    '2018/10/07 saitou 広島信金(RSV2標準) DEL ------------------------------------------------- END

End Class
