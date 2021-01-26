Imports System.IO
Imports System.Text

''' <summary>
''' 日次ファイル精査
''' </summary>
''' <remarks>
''' 2016/11/29 saitou 近畿産業信組 added for RSV2性能向上
''' 　フォームアプリケーションからコンソールアプリケーションに変更
''' </remarks>
Module ModMain

#Region "モジュール変数"

    Private ReadOnly Delete_FILE_ini As String = "Delete_FILE.ini"

    Private BatchLog As New CASTCommon.BatchLOG("DelFile", "過去ログ削除")
    Private Structure LogWrite
        Dim UserID As String        'ユーザID
        Dim ToriCode As String      '取引先コード
        Dim FuriDate As String      '振替日
    End Structure
    Private LW As LogWrite

    Private Structure IniInfo
        Dim DIR_NAME As String      'フォルダパス
        Dim DEL_MONTH As Integer    '削除対象月数
        Dim DIR_DELFLG As String    'フォルダ削除フラグ
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
            Dim IniFileName As String = Path.Combine(Application.StartupPath, Delete_FILE_ini)
            sr = New StreamReader(IniFileName, Encoding.GetEncoding(932))
            Dim ReadLine As String = ""
            While sr.EndOfStream = False
                ReadLine = sr.ReadLine()
                If Not ReadLine Is Nothing Then
                    Dim Item As String() = ReadLine.Split(","c)
                    If Item.Length = 3 Then
                        If IsNumeric(Trim(Item(1))) = True Then

                            DelInfo.DIR_NAME = Trim(Item(0))
                            DelInfo.DEL_MONTH = CInt(Trim(Item(1)))
                            DelInfo.DIR_DELFLG = Trim(Item(2))

                            DelArray.Add(DelInfo)
                        End If
                    End If
                End If
            End While

            sr.Close()
            sr = Nothing

            '----------------------------------------
            '過去ログの削除
            '----------------------------------------
            Dim SysDate As Date = DateTime.Today
            Dim strKijyunDate As String = ""
            For Each Info As IniInfo In DelArray
                If Directory.Exists(Info.DIR_NAME) = True Then
                    strKijyunDate = SysDate.AddMonths(Info.DEL_MONTH).ToString("yyyyMMdd")
                    If DelFile(Info.DIR_NAME, strKijyunDate, Info.DIR_DELFLG) = False Then
                        Return
                    End If
                Else
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "フォルダ検索", "失敗", "フォルダ名:" & Info.DIR_NAME)
                End If
            Next

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "メイン処理", "失敗", ex.Message)

        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(メイン処理)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 過去ファイルの削除を行います。
    ''' </summary>
    ''' <param name="DIR">対象フォルダ</param>
    ''' <param name="delete_date">削除基準日</param>
    ''' <param name="DFLG">フォルダ削除フラグ</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function DelFile(ByVal DIR As String, ByVal delete_date As String, ByVal DFLG As String) As Boolean
        Try
            Dim strDir As String
            Dim count As Long = 0
            If DFLG = "1" Then
                If System.IO.Directory.Exists(DIR) = True Then
                    For Each strDir In System.IO.Directory.GetDirectories(DIR)
                        If CInt(delete_date) > CInt(System.IO.Directory.GetLastWriteTime(strDir).ToString("yyyyMMdd")) Then
                            System.IO.Directory.Delete(strDir, True)
                            count += 1
                        End If
                    Next
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & DIR & ")削除", "成功", count & "件")
                Else
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & DIR & ")", "未存在", DIR & "が存在しません")
                End If
            Else
                If System.IO.Directory.Exists(DIR) = True Then
                    For Each strDir In System.IO.Directory.GetFiles(DIR)
                        If CInt(delete_date) > CInt(System.IO.File.GetLastWriteTime(strDir).ToString("yyyyMMdd")) Then
                            System.IO.File.Delete(strDir)
                            count += 1
                        End If
                    Next
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & DIR & ")削除", "成功", count & "件")
                Else
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & DIR & ")", "未存在", DIR & "が存在しません")
                End If
            End If

            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & DIR & "-ファイル削除)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

#End Region

End Module
