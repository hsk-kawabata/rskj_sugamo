Option Explicit On 
Option Strict On

Imports System.Data.OracleClient

Public Class clsDataBase

    Private Const ThisModuleName As String = "clsDataBase.vb"

    ' 機能　　　: 取引先一覧を返す
    '
    ' 戻り値　　: カーソル行の有無
    '
    ' 引き数　　: ARG1 - Reader Object
    '
    ' 備考　　　: 汎用
    '
    Public Function GetToriCode(ByRef OraReader As OracleDataReader) As Boolean
        Dim SQL As String = ""
        Try
            SQL = "SELECT TORIS_CODE"
            SQL &= ", TORIF_CODE"
            SQL &= ", ITAKU_CODE"
            SQL &= ", ITAKU_NNAME"
            SQL &= ", ITAKU_KNAME"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ENC_KBN = '1'"
            'SQL &= " WHERE FSYORI_KBN = '1'"
            'SQL &= " AND ENC_KBN = '1'"
            SQL &= " ORDER BY ITAKU_KNAME ASC"

            Return GCom.SetDynaset(SQL, OraReader)
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function

    ' 機能　　　: 取引先主コード、副コード、F処理区分を返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード(主キー)
    '           : ARG2 - 取引先主コード(参照渡し)
    '           : ARG3 - 取引先副コード(参照渡し)
    '           : ARG4 - F処理区分(参照渡し)
    '
    ' 備考　　　: 
    '
    Public Function GetFToriCode(ByVal itakuCode As String, ByVal Syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String, ByVal nFormatKbn As Integer) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            'とりあえず自振のみを想定
            SQL = "SELECT NVL(TORIS_CODE_T, '該当なし')"
            SQL &= ", NVL(TORIF_CODE_T, '該当なし')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & Syubetu & "'"
            ElseIf nFormatKbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)

                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加 START
    ' 機能　　　: 取引先主コード、副コード、F処理区分を返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード(主キー)
    '           : ARG2 - 取引先主コード(参照渡し)
    '           : ARG3 - 取引先副コード(参照渡し)
    '           : ARG4 - F処理区分(参照渡し)
    '           : ARG7 - 振替日(参照渡し)
    '           : ARG8 - 返還フラグ(参照渡し)
    '
    ' 備考　　　: 
    '
    Public Function aGetFToriCode(ByVal itakuCode As String, ByVal Syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String, ByVal nFormatKbn As Integer, _
        Optional ByVal furiDate As String = "") As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            'とりあえず自振のみを想定
            SQL = "SELECT NVL(TORIS_CODE_T, '該当なし')"
            SQL &= ", NVL(TORIF_CODE_T, '該当なし')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & Syubetu & "'"
            ElseIf nFormatKbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST, SCHMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND   TORIS_CODE_T = TORIS_CODE_S"
                SQL &= " AND   TORIF_CODE_T = TORIF_CODE_S"
                SQL &= " AND   FURI_DATE_S = '" & furiDate & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)

                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加  END

    '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
    ' 機能　　　: 取引先主コード、副コード、F処理区分を返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード(主キー)
    '           : ARG2 - 取引先主コード(参照渡し)
    '           : ARG3 - 取引先副コード(参照渡し)
    '           : ARG4 - F処理区分(参照渡し)
    '           : ARG5 - 媒体コード         
    '
    ' 備考　　　: 
    '
    Public Function GetFToriCode(ByVal itakuCode As String, ByVal syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String _
        , ByRef baitaiCode As String, ByRef itakukanriCode As String, ByRef multikbn As String, Optional ByVal nFormatkbn As Integer = 0) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(TORIS_CODE_T, '該当なし')"
            SQL &= ", NVL(TORIF_CODE_T, '該当なし')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            SQL &= ", NVL(BAITAI_CODE_T, '00')"
            SQL &= ", ITAKU_KANRI_CODE_T"
            SQL &= ", MULTI_KBN_T"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & syubetu & "'"
            ElseIf nFormatkbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)
                baitaiCode = onReader.GetString(3)

                itakukanriCode = onReader.GetString(4)
                multikbn = onReader.GetString(5)

                Return True
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                '' 読込失敗時 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function

    '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加 START
    '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
    ' 機能　　　: 取引先主コード、副コード、F処理区分を返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード(主キー)
    '           : ARG2 - 取引先主コード(参照渡し)
    '           : ARG3 - 取引先副コード(参照渡し)
    '           : ARG4 - F処理区分(参照渡し)
    '           : ARG5 - 媒体コード         
    '
    ' 備考　　　: 
    '
    Public Function bGetFToriCode(ByVal itakuCode As String, ByVal syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String _
        , ByRef baitaiCode As String, ByRef itakukanriCode As String, ByRef multikbn As String, Optional ByVal nFormatkbn As Integer = 0, _
        Optional ByVal furiDate As String = "") As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(TORIS_CODE_T, '該当なし')"
            SQL &= ", NVL(TORIF_CODE_T, '該当なし')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            SQL &= ", NVL(BAITAI_CODE_T, '00')"
            SQL &= ", ITAKU_KANRI_CODE_T"
            SQL &= ", MULTI_KBN_T"
            If nFormatkbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & syubetu & "'"
            ElseIf nFormatkbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST, SCHMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND   TORIS_CODE_T = TORIS_CODE_S"
                SQL &= " AND   TORIF_CODE_T = TORIF_CODE_S"
                SQL &= " AND   FURI_DATE_S = '" & furiDate & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)
                baitaiCode = onReader.GetString(3)

                itakukanriCode = onReader.GetString(4)
                multikbn = onReader.GetString(5)

                Return True
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                '' 読込失敗時 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "処理区分・取引先主・副コード取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加  END

    ' 機能　　　: 委託者コードを返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード(参照渡し)
    '           : ARG2 - 取引先主コード
    '           : ARG3 - 取引先副コード
    '
    ' 備考　　　: 
    '
    Public Function GetItakuCode(ByRef itakuCode As String, _
        ByVal toris As String, ByVal torif As String) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_CODE, '該当なし')"
            SQL &= " FROM TORI_VIEW"
            '*** 修正 mitsu 2008/09/01 条件修正 ***
            'SQL &= " WHERE TORI_S_CODE = '" & toris & "'"
            'SQL &= " AND TORI_F_CODE = " & torif & "'"
            SQL &= " WHERE TORIS_CODE = '" & toris & "'"
            SQL &= " AND TORIF_CODE = " & torif & "'"
            '**************************************

            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                itakuCode = onReader.GetString(0)
                Return True
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                '' 読込失敗時 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "委託者コード取得失敗 取引先主コード：" & toris & " 取引先副コード：" & torif
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "委託者コード取得失敗 取引先主コード：" & toris & " 取引先副コード：" & torif & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function


    ' 機能　　　: フォーマット区分を返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード
    '           : ARG2 - フォーマット区分(CMT変換仕様)
    '
    ' 備考　　　: 
    '
    Public Function GetFormatKbn(ByVal itakuCode As String, _
        ByRef nFormatKbn As Integer) As Boolean

        Dim fkbn As String      ' フォーマット区分文字列
        Dim mkbn As String      ' 持込区分文字列

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(FMT_KBN, '00'), NVL(MOTIKOMI_KBN,'0')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & itakuCode & "'"
            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                fkbn = onReader.GetString(0)
                mkbn = onReader.GetString(1)
            Else
                ' 読込失敗時 
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "フォーマット区分取得失敗 委託者コード：" & itakuCode
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "フォーマット区分取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (mkbn = "1") Then
            nFormatKbn = 4          ' TODO センター持込の請求分/結果分の振分に対応
            Return True
        End If
        Select Case fkbn
            Case "00"               ' 全銀
                nFormatKbn = 0
            Case "01"               ' 地方公共団体1 (350)
                nFormatKbn = 1
            Case "06"               ' 地方公共団体2 (300) 岡崎市
                nFormatKbn = 2
            Case "02"               ' 国税
                nFormatKbn = 3
            Case "20"               ' SSSは全銀扱い
                nFormatKbn = 0
        End Select
        Return True
    End Function

    ' 機能　　　: ラベル区分の取得
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード
    '           : ARG2 - フォーマット区分(CMT変換仕様)
    '
    ' 備考　　　: 
    '
    Public Function GetLabelKbn(ByVal itakuCode As String, _
        ByRef nLabelKbn As Integer) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(LABEL_KBN,0)"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & itakuCode & "'"
            If GCom.SetDynaset(SQL, onReader) Then
                ' 読込成功時
                onReader.Read()
                nLabelKbn = onReader.GetInt32(0)
            Else
                ' 読込失敗時 
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "ラベル区分取得失敗 委託者コード：" & itakuCode
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ラベル区分取得失敗 委託者コード：" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function


    ' 機能　　　: 取引先漢字名の取得
    '
    ' 戻り値　　: 取引先漢字名
    '
    ' 引き数　　: ARG1 - 委託者コード
    '
    ' 備考　　　: 
    '
    Public Function GetItakuKanji(ByVal strItakuCode As String) As String
        Dim SQL As String = ""

        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_NNAME, ' -- ')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & strItakuCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' 行があるとき
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                Return name
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "委託者名(漢字)取得失敗 委託者コード：" & strItakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "委託者名取得"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "委託者名(漢字)取得失敗 委託者コード：" & strItakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "委託者名検索エラー"

    End Function

    '*** 修正 mitsu 2008/07/17 振替コード・企業コードを渡された場合 ***
    Public Function GetItakuKanji(ByRef strItakuCode As String _
        , ByVal strFuriCode As String, ByVal strKigyoCode As String) As String

        Dim SQL As String = ""

        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_NNAME_T, ' -- '), ITAKU_CODE_T"
            SQL &= " FROM TORIMAST"
            SQL &= " WHERE KIGYO_CODE_T = '" & strKigyoCode & "'"
            SQL &= " AND FURI_CODE_T = '" & strFuriCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' 行があるとき
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                strItakuCode = onReader.GetString(1)
                Return name
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "委託者名(漢字)取得失敗 振替コード：" & strFuriCode _
                '                 & " 企業コード：" & strKigyoCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "委託者名(漢字)取得失敗 振替コード：" & strFuriCode _
                             & " 企業コード：" & strKigyoCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "委託者名検索エラー"

    End Function
    '******************************************************************

    ' 機能　　　: 取引先カナ名の取得
    '
    ' 戻り値　　: 取引先漢字名
    '
    ' 引き数　　: ARG1 - 委託者コード
    '
    ' 備考　　　: 
    '
    Public Function GetItakuKana(ByVal strItakuCode As String) As String
        Dim SQL As String = ""

        Try
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_KNAME, ' -- ')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & strItakuCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' 行があるとき
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                Return name
            Else
                '*** 修正 mitsu 2008/09/08 不要 ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "委託者名(カナ)取得失敗 委託者コード：" & strItakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "委託者名(カナ)取得失敗 委託者コード：" & strItakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "委託者名検索エラー"

    End Function

    ' 機能　 ： 金融機関情報取得
    '
    ' 引数   ： ARG1 - 金融機関コード
    '           ARG2 - 支店コード
    '           ARG3 - 金融機関名(参照渡し)
    '           ARG4 - 支店名(参照渡し)
    '
    ' 戻り値 ： 成功 true / 失敗 false
    '
    ' 備考　 ： なし
    '
    Public Function GetBankAndBranchName(ByVal BKCode As String, ByVal BRCode As String, _
        ByRef bankname As String, ByRef branchname As String) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Try
            If BKCode.Length = 0 Then
                Return False
            End If

            Dim SQL As String = "SELECT KIN_NNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode & "'"
            SQL &= " AND SIT_NO_N = '" & BRCode & "'"
            '*** 修正 mitsu 2008/09/01 処理高速化 ***
            SQL &= " AND ROWNUM = 1"
            '****************************************

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read() Then

                '金融機関名
                bankname = REC.GetString(0)
                '支店名
                branchname = REC.GetString(1)

                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "金融機関名・支店名取得失敗 金融機関コード：" & BKCode & " 支店コード：" & BRCode
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "金融機関名・支店名取得失敗 金融機関コード：" & BKCode & " 支店コード：" & BRCode & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
        Return True
    End Function


    ' 機能　 ： 金融機関情報取得
    '
    ' 引数   ： ARG1 - 金融機関コード
    '           ARG2 - 金融機関名(参照渡し)
    '
    ' 戻り値 ： 成功 true / 失敗 false
    '
    ' 備考　 ： なし
    '
    Public Function GetBankName(ByVal BKCode As String, ByRef bankname As String) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            If BKCode.Length = 0 Then
                Return False
            End If

            Dim SQL As String = "SELECT KIN_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode & "'"
            '*** 修正 mitsu 2008/09/01 処理高速化 ***
            SQL &= " AND ROWNUM = 1"
            '****************************************

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read() Then

                '金融機関名
                bankname = REC.GetString(0)
                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "金融機関名取得失敗 金融機関コード：" & BKCode
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "金融機関名取得失敗 金融機関コード：" & BKCode & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
        Return True
    End Function

    ' 機能　　　: ファイル書込み回数の取得
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ARG1 - 委託者コード
    '             ARG2 - 振替日
    '             ARG3 - ファイル書込回数(複数件のレコードのうちの最大値)
    ' 備考　　　: 
    Public Function GetWriteCounter(ByVal itakucd As String, ByVal furidate As String, ByRef Counter As Integer) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Try
            Dim SQL As String = "SELECT NVL(MAX(WRITE_COUNTER), 0) FROM CMT_WRITE_TBL"
            SQL &= " WHERE FILE_SEQ = 1"
            SQL &= " AND ITAKU_CODE = '" & itakucd & "'"
            SQL &= " AND FURI_DATE = '" & furidate & "'"
            SQL &= " AND ERR_CD = 0"
            If GCom.SetDynaset(SQL, onReader) AndAlso onReader.Read Then
                Counter = onReader.GetInt32(0)
                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "ファイル書込回数取得失敗 委託者コード：" & itakucd & " 振替日：" & furidate
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Counter = 0
                Return False
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ファイル書込回数取得失敗 委託者コード：" & itakucd & " 振替日：" & furidate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Counter = 0
            Return False

        End Try

    End Function
End Class
