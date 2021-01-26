'*** Str Add 2015/12/01 SO)荒木 for ジョブ多重実行（新規作成） ***
Imports System
Imports System.Diagnostics

' DBロッククラス
Public Class CDBLock

    Private mJobTrubanLockFlg As Boolean = False
    Private mWebRirekiLockFlg As Boolean = False
    Private mJobExecLock_Flg As Boolean = False

    Private LOG As CASTCommon.BatchLOG


    Public Sub New()

        LOG = New CASTCommon.BatchLOG("ClsDBLock", "CDBLock")

    End Sub


    ' 機能　 ： ジョブマスタに対するINSERTロックを行う
    ' 引数　 ： db ：DBコネクション
    '           waittime ： 待ち時間（秒）
    ' 復帰値 ： True: ロック成功   Flase: ロック待ちタイムアウト
    '           タイムアウト以外の例外時は、例外をスローする
    Public Function InsertJOBMAST_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertJOBMAST_Lock", "waittime=" & waittime)

            ' 既にロック済みの場合はNOP
            If mJobTrubanLockFlg = True Then
                rtn = True
                Return rtn
            End If

            ' ロック用SQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK TUUBAN_J FROM JOBMAST' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' タイムアウトの場合
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' その他のエラー
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMASTにLOCK_KEY（LOCK TUUBAN_J FROM JOBMAST）が登録されていません")
                End If
            End If

            ' ロック済みフラグ設定
            mJobTrubanLockFlg = True

            rtn = True
            Return rtn

        Catch ex As Exception
            LOG.Write_Err("ClsDBLock.InsertJOBMAST_Lock", ex)
            Throw

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If

            LOG.Write_Exit3(sw, "ClsDBLock.InsertJOBMAST_Lock", "rtn=" & rtn)
        End Try

    End Function


    ' 機能　 ： ジョブマスタに対するINSERTロック解除を行う（将来拡張用で実装なし）
    ' 引数　 ： db ：DBコネクション
    ' 備考　 ： 実際のロック解除は、呼出し元でDBコネクションに対し
    '           コミット／ロールバックを実行したタイミングとなる
    '
    Public Sub InsertJOBMAST_UnLock(ByVal db As CASTCommon.MyOracle)

        If mJobTrubanLockFlg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertJOBMAST_UnLock")

            ' ロック済みフラグクリア
            mJobTrubanLockFlg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.InsertJOBMAST_UnLock")
        End Try

    End Sub


    ' 機能　 ： WEB_RIREKIMASTに対するINSERTロックを行う
    '           waittime ： 待ち時間（秒）
    ' 引数　 ： db ：DBコネクション
    ' 復帰値 ： True: ロック成功   Flase: ロック待ちタイムアウト
    '           タイムアウト以外の例外時は、例外をスローする
    Public Function InsertWEB_RIREKIMAST_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertWEB_RIREKIMAST_Lock", "waittime=" & waittime)

            ' 既にロック済みの場合はNOP
            If mWebRirekiLockFlg = True Then
                rtn = True
                Return rtn
            End If

            ' ロック用SQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK SEQ_NO_W FROM WEB_RIREKIMAST' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' タイムアウトの場合
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' その他のエラー
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMASTにLOCK_KEY（LOCK SEQ_NO_W FROM WEB_RIREKIMAST）が登録されていません")
                End If
            End If

            ' ロック済みフラグ設定
            mWebRirekiLockFlg = True

            rtn = True
            Return rtn

        Catch ex As Exception
            LOG.Write_Err("ClsDBLock.InsertWEB_RIREKIMAST_Lock", ex)
            Throw

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If

            LOG.Write_Exit3(sw, "ClsDBLock.InsertWEB_RIREKIMAST_Lock", "rtn=" & rtn)
        End Try

    End Function


    ' 機能　 ： WEB_RIREKIMASTに対するINSERTロック解除を行う（将来拡張用で実装なし）
    ' 引数　 ： db ：DBコネクション
    ' 備考　 ： 実際のロック解除は、呼出し元でDBコネクションに対し
    '           コミット／ロールバックを実行したタイミングとなる
    '
    Public Sub InsertWEB_RIREKIMAST_UnLock(ByVal db As CASTCommon.MyOracle)

        If mWebRirekiLockFlg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertWEB_RIREKIMAST_UnLock")

            ' ロック済みフラグクリア
            mWebRirekiLockFlg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.InsertWEB_RIREKIMAST_UnLock")
        End Try

    End Sub


    ' 機能　 ： 実行中のEXE名（ジョブID）で実行ロックを行う
    ' 引数　 ： db ：DBコネクション
    '           waittime ： 待ち時間（秒）
    ' 復帰値 ： True: ロック成功   Flase: ロック待ちタイムアウト
    '           タイムアウト以外の例外時は、例外をスローする
    Public Function Job_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.Job_Lock", "waittime=" & waittime)

            ' 既にロック済みの場合はNOP
            If mJobExecLock_Flg = True Then
                rtn = True
                Return rtn
            End If

            ' EXE名からジョブIDを取得
            Dim jobID As String = Process.GetCurrentProcess.ProcessName
            jobID = jobID.ToUpper

            ' VB.NETからのデバッグ時はEXE名に".VSHOST"が付くので取り除く
            jobID = jobID.Replace(".VSHOST", "")

            ' ロック用SQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK JOBEXEC JOBID=" & jobID & _
                                    "' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' タイムアウトの場合
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' その他のエラー
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMASTにLOCK_KEY（LOCK JOBEXEC JOBID=" & jobID & "）が登録されていません")
                End If
            End If

            ' ロック済みフラグ設定
            mJobExecLock_Flg = True

            rtn = True
            Return rtn

        Catch ex As Exception
            LOG.Write_Err("ClsDBLock.Job_Lock", ex)
            Throw

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If

            LOG.Write_Exit3(sw, "ClsDBLock.Job_Lock", "rtn=" & rtn)
        End Try

    End Function


    ' 機能　 ： 実行中のEXE名（ジョブID）で実行ロック解除を行う（将来拡張用で実装なし）
    ' 引数　 ： db ：DBコネクション
    ' 備考　 ： 実際のロック解除は、呼出し元でDBコネクションに対し
    '           コミット／ロールバックを実行したタイミングとなる
    '
    Public Sub Job_UnLock(ByVal db As CASTCommon.MyOracle)

        If mJobExecLock_Flg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.Job_UnLock")

            ' ロック済みフラグクリア
            mJobExecLock_Flg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.Job_UnLock")
        End Try

    End Sub

End Class
'*** End Add 2015/12/01 SO)荒木 for ジョブ多重実行（新規作成） ***
