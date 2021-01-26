Imports System

''' <summary>
''' 配信データ作成メインモジュール
''' </summary>
''' <remarks></remarks>
Module ModMain

#Region "モジュール定数"
    Public MainLOG As New CASTCommon.BatchLOG("KFJ030", "センターカットデータ作成")

#End Region

#Region "パブリックメソッド"

    Public Function Main(ByVal CmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG
        Dim HaisinClass As New ClsHAISIN

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "配信データ(開始)", "成功") 
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            ELog.Write("開始")



            '初期処理
            If HaisinClass.FirstInit(CmdArgs) = False Then
                Return -1
            End If

            ' 主処理
            ret = HaisinClass.Main(CmdArgs(0))

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write("0000000000-00", "00000000", "配信データ","失敗", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            ELog.Write(ex.Message)
            Return -1
        Finally
            ELog.Write("終了")
        End Try
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "配信データ(終了)", "成功")
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Return ret

    End Function

#End Region

End Module
