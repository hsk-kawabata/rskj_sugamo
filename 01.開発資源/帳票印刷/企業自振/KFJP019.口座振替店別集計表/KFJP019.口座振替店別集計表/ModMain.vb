Imports System

''' <summary>
''' 口座振替店別集計表　メインモジュール
''' </summary>
''' <remarks>蒲郡信金 ハード更改にて改修</remarks>
Module ModMain

#Region "モジュール変数"
    'ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP019", "口座振替店別集計表")
    Public Structure LogWrite
        Dim UserID As String
        Dim ToriCode As String
        Dim FuriDate As String
    End Structure
    Public LW As LogWrite

    Public RecordCnt As Long = 0
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 口座振替店別集計表　メイン処理
    ''' </summary>
    ''' <param name="arg">起動パラメータ</param>
    ''' <returns>0 - 正常 , 0以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main(ByVal arg() As String) As Integer
        Try
            '---------------------------------------------------------
            'ログ書込みに必要な情報の取得
            '---------------------------------------------------------
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If arg.Length = 0 Then
                'パラメータ取得失敗
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Param() As String = arg(0).Split(",")
            If Param.Length <> 3 AndAlso Param.Length <> 4 AndAlso Param.Length <> 5 Then
                'パラメータ間違い
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & arg(0))
                Return -100
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "成功", "コマンドライン引数：" & arg(0))
            End If

            '---------------------------------------------------------
            '口座振替店別集計表印刷処理
            '---------------------------------------------------------
            Dim ret As Integer
            Dim List As New ClsTenbetushuukei
            List.TORI_CODE = Param(0)
            List.FURI_DATE = Param(1)
            List.HENKAN_FLG = Param(2)
            List.PRINTERNAME = ""
            '2012/06/30 標準版　WEB伝送対応------------->
            If Param.Length = 5 Then
                List.INVOKE_KBN = Param(4)
            End If
            '-------------------------------------------<

            ret = List.Main()

            If ret = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)", "成功", "")
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)", "失敗", "復帰値：" & ret.ToString)
            End If

            Return ret
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)", "失敗", ex.Message)
            Return -999
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
        End Try
    End Function

#End Region

End Module

#Region "過去の遺産"

'Imports System
'Imports System.IO
'Imports System.Diagnostics

'' 口座振替店別集計表 メインモジュール
'Module ModMain

'    ' ログ処理クラス
'    Public MainLOG As New CASTCommon.BatchLOG("KFJP019", "口座振替店別集計表")

'    '
'    ' 機能　 ： 口座振替店別集計表 メイン処理
'    '
'    ' 引数　 ： ARG1 - 起動パラメータ
'    '
'    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
'    '
'    ' 備考　 ：
'    '
'    Function Main(ByVal arg() As String) As Integer
'        ' 戻り値
'        Dim ret As Integer
'        Dim Tenbetushuukei As New ClsTenbetushuukei
'        If arg.Length <> 1 Then
'            MainLOG.Write("開始", "失敗", "引数まちがい")
'            Return -2
'        End If

'        Dim Param() As String = arg(0).Split(",")
'        Try
'            Tenbetushuukei.TORI_CODE = Param(0)     '取引先コード
'            Tenbetushuukei.FURI_DATE = Param(1)     '振替日
'            Tenbetushuukei.HENKAN_FLG = Param(2)    '返還フラグ(0:落込時出力/0以外返還時出力)

'            Tenbetushuukei.PRINTERNAME = ""
'            'If Param.Length = 3 Then
'            '    Tenbetushuukei.PRINTERNAME = Param(2)
'            'Else
'            '    Tenbetushuukei.PRINTERNAME = ""
'            'End If

'            MainLOG.Write("パラメータ取得", "成功", arg(0))
'        Catch ex As Exception
'            MainLOG.Write("パラメータ取得", "失敗", ex.Message)
'            Return -1
'        End Try

'        Dim ELog As New CASTCommon.ClsEventLOG
'        ELog.Write("処理開始")

'        ' 口座振替店別集計表落し込み処理
'        ' メイン処理
'        ret = Tenbetushuukei.Main()
'        If ret = 0 Then
'            MainLOG.Write("処理終了", "成功", )
'        Else
'            MainLOG.Write("処理終了", "失敗", "復帰値：" & ret)
'        End If

'        Return ret
'    End Function

'End Module

#End Region
