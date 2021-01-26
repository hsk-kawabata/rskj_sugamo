Imports System.IO
Imports System.Text
Imports System.Windows.Forms

''' <summary>
''' 日次データベース精査
''' </summary>
''' <remarks>
''' 2016/11/29 saitou 近畿産業信組 added for RSV2性能向上
''' 　フォームアプリケーションからコンソールアプリケーションに変更
''' </remarks>
Module ModMain

#Region "モジュール変数"

    Private ReadOnly Delete_MAST_ini As String = "Delete_MAST.ini"

    Private BatchLog As New CASTCommon.BatchLOG("DelDB", "DBデータ削除")
    Private Structure LogWrite
        Dim UserID As String        'ユーザID
        Dim ToriCode As String      '取引先コード
        Dim FuriDate As String      '振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    Private Structure IniInfo
        Dim TABLE_NAME As String    'テーブル名
        Dim KEY_NAME As String      '削除キー
        Dim DEL_MONTH As Integer    '削除対象月数
    End Structure
    Private DelInfo As IniInfo

    Private DelArray As ArrayList

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' メイン処理を行います。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Main()

        Dim sr As StreamReader = Nothing

        Try
            '----------------------------------------
            'ログ書き込みに必要な情報の取得
            '----------------------------------------
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(メイン処理)開始", "成功", "")

            '----------------------------------------
            '設定情報の格納
            '----------------------------------------
            DelArray = New ArrayList
            Dim IniFileName As String = Path.Combine(Application.StartupPath, Delete_MAST_ini)
            sr = New StreamReader(IniFileName, Encoding.GetEncoding(932))
            Dim ReadLine As String = ""
            While sr.EndOfStream = False
                ReadLine = sr.ReadLine()
                If Not ReadLine Is Nothing Then
                    Dim Item As String() = ReadLine.Split(","c)
                    If Item.Length = 3 Then
                        If IsNumeric(Trim(Item(2))) = True Then

                            DelInfo.TABLE_NAME = Trim(Item(0))
                            DelInfo.KEY_NAME = Trim(Item(1))
                            DelInfo.DEL_MONTH = CInt(Trim(Item(2)))

                            DelArray.Add(DelInfo)
                        End If
                    End If
                End If
            End While

            sr.Close()
            sr = Nothing

            '----------------------------------------
            'マスタの削除
            '----------------------------------------
            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()
            For Each Info As IniInfo In DelArray
                If DelData(Info.TABLE_NAME, Info.DEL_MONTH, Info.KEY_NAME) = False Then
                    Return
                End If
            Next

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "メイン処理", "失敗", ex.Message)

        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If

            '最後まで残っていたらロールバック
            '処理の最後に必ず閉める
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(メイン処理)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テーブルの削除を行います。
    ''' </summary>
    ''' <param name="table">対象テーブル名</param>
    ''' <param name="month">削除対象月</param>
    ''' <param name="key">削除キー</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function DelData(ByVal table As String, ByVal month As Integer, ByVal key As String) As Boolean
        Dim SQL As New StringBuilder
        Dim strKijyunDate As String
        Dim delCnt As Integer

        Try
            '削除基準日取得
            strKijyunDate = DateTime.Today.AddMonths(month).ToString("yyyyMMdd")

            SQL.Append("DELETE FROM " & table & " ")
            SQL.Append("WHERE " & key & " < '" & strKijyunDate & "'")

            delCnt = MainDB.ExecuteNonQuery(SQL)

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, table & "削除", "成功", "削除件数: " & delCnt & " 件")

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, table & "削除", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Module
