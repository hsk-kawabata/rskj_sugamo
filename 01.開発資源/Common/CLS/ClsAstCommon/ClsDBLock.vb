'*** Str Add 2015/12/01 SO)�r�� for �W���u���d���s�i�V�K�쐬�j ***
Imports System
Imports System.Diagnostics

' DB���b�N�N���X
Public Class CDBLock

    Private mJobTrubanLockFlg As Boolean = False
    Private mWebRirekiLockFlg As Boolean = False
    Private mJobExecLock_Flg As Boolean = False

    Private LOG As CASTCommon.BatchLOG


    Public Sub New()

        LOG = New CASTCommon.BatchLOG("ClsDBLock", "CDBLock")

    End Sub


    ' �@�\�@ �F �W���u�}�X�^�ɑ΂���INSERT���b�N���s��
    ' �����@ �F db �FDB�R�l�N�V����
    '           waittime �F �҂����ԁi�b�j
    ' ���A�l �F True: ���b�N����   Flase: ���b�N�҂��^�C���A�E�g
    '           �^�C���A�E�g�ȊO�̗�O���́A��O���X���[����
    Public Function InsertJOBMAST_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertJOBMAST_Lock", "waittime=" & waittime)

            ' ���Ƀ��b�N�ς݂̏ꍇ��NOP
            If mJobTrubanLockFlg = True Then
                rtn = True
                Return rtn
            End If

            ' ���b�N�pSQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK TUUBAN_J FROM JOBMAST' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' �^�C���A�E�g�̏ꍇ
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' ���̑��̃G���[
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMAST��LOCK_KEY�iLOCK TUUBAN_J FROM JOBMAST�j���o�^����Ă��܂���")
                End If
            End If

            ' ���b�N�ς݃t���O�ݒ�
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


    ' �@�\�@ �F �W���u�}�X�^�ɑ΂���INSERT���b�N�������s���i�����g���p�Ŏ����Ȃ��j
    ' �����@ �F db �FDB�R�l�N�V����
    ' ���l�@ �F ���ۂ̃��b�N�����́A�ďo������DB�R�l�N�V�����ɑ΂�
    '           �R�~�b�g�^���[���o�b�N�����s�����^�C�~���O�ƂȂ�
    '
    Public Sub InsertJOBMAST_UnLock(ByVal db As CASTCommon.MyOracle)

        If mJobTrubanLockFlg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertJOBMAST_UnLock")

            ' ���b�N�ς݃t���O�N���A
            mJobTrubanLockFlg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.InsertJOBMAST_UnLock")
        End Try

    End Sub


    ' �@�\�@ �F WEB_RIREKIMAST�ɑ΂���INSERT���b�N���s��
    '           waittime �F �҂����ԁi�b�j
    ' �����@ �F db �FDB�R�l�N�V����
    ' ���A�l �F True: ���b�N����   Flase: ���b�N�҂��^�C���A�E�g
    '           �^�C���A�E�g�ȊO�̗�O���́A��O���X���[����
    Public Function InsertWEB_RIREKIMAST_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertWEB_RIREKIMAST_Lock", "waittime=" & waittime)

            ' ���Ƀ��b�N�ς݂̏ꍇ��NOP
            If mWebRirekiLockFlg = True Then
                rtn = True
                Return rtn
            End If

            ' ���b�N�pSQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK SEQ_NO_W FROM WEB_RIREKIMAST' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' �^�C���A�E�g�̏ꍇ
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' ���̑��̃G���[
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMAST��LOCK_KEY�iLOCK SEQ_NO_W FROM WEB_RIREKIMAST�j���o�^����Ă��܂���")
                End If
            End If

            ' ���b�N�ς݃t���O�ݒ�
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


    ' �@�\�@ �F WEB_RIREKIMAST�ɑ΂���INSERT���b�N�������s���i�����g���p�Ŏ����Ȃ��j
    ' �����@ �F db �FDB�R�l�N�V����
    ' ���l�@ �F ���ۂ̃��b�N�����́A�ďo������DB�R�l�N�V�����ɑ΂�
    '           �R�~�b�g�^���[���o�b�N�����s�����^�C�~���O�ƂȂ�
    '
    Public Sub InsertWEB_RIREKIMAST_UnLock(ByVal db As CASTCommon.MyOracle)

        If mWebRirekiLockFlg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.InsertWEB_RIREKIMAST_UnLock")

            ' ���b�N�ς݃t���O�N���A
            mWebRirekiLockFlg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.InsertWEB_RIREKIMAST_UnLock")
        End Try

    End Sub


    ' �@�\�@ �F ���s����EXE���i�W���uID�j�Ŏ��s���b�N���s��
    ' �����@ �F db �FDB�R�l�N�V����
    '           waittime �F �҂����ԁi�b�j
    ' ���A�l �F True: ���b�N����   Flase: ���b�N�҂��^�C���A�E�g
    '           �^�C���A�E�g�ȊO�̗�O���́A��O���X���[����
    Public Function Job_Lock(ByVal db As CASTCommon.MyOracle, ByVal waittime As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim rtn As Boolean = False

        Try
            sw = LOG.Write_Enter3("ClsDBLock.Job_Lock", "waittime=" & waittime)

            ' ���Ƀ��b�N�ς݂̏ꍇ��NOP
            If mJobExecLock_Flg = True Then
                rtn = True
                Return rtn
            End If

            ' EXE������W���uID���擾
            Dim jobID As String = Process.GetCurrentProcess.ProcessName
            jobID = jobID.ToUpper

            ' VB.NET����̃f�o�b�O����EXE����".VSHOST"���t���̂Ŏ�菜��
            jobID = jobID.Replace(".VSHOST", "")

            ' ���b�N�pSQL
            Dim LockSql As String = "SELECT LOCK_KEY FROM LOCKMAST WHERE LOCK_KEY='LOCK JOBEXEC JOBID=" & jobID & _
                                    "' FOR UPDATE WAIT " & waittime

            oraReader = New CASTCommon.MyOracleReader(db)
            If oraReader.DataReader(LockSql) = False Then
                If oraReader.Message <> "" Then
                    ' �^�C���A�E�g�̏ꍇ
                    If oraReader.Message.StartsWith("ORA-30006") Then
                        Return rtn    ' False

                    ' ���̑��̃G���[
                    Else
                        Throw New Exception(oraReader.Message)
                    End If
                Else
                    Throw New Exception("LOCKMAST��LOCK_KEY�iLOCK JOBEXEC JOBID=" & jobID & "�j���o�^����Ă��܂���")
                End If
            End If

            ' ���b�N�ς݃t���O�ݒ�
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


    ' �@�\�@ �F ���s����EXE���i�W���uID�j�Ŏ��s���b�N�������s���i�����g���p�Ŏ����Ȃ��j
    ' �����@ �F db �FDB�R�l�N�V����
    ' ���l�@ �F ���ۂ̃��b�N�����́A�ďo������DB�R�l�N�V�����ɑ΂�
    '           �R�~�b�g�^���[���o�b�N�����s�����^�C�~���O�ƂȂ�
    '
    Public Sub Job_UnLock(ByVal db As CASTCommon.MyOracle)

        If mJobExecLock_Flg = False Then
            Exit Sub
        End If

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = LOG.Write_Enter3("ClsDBLock.Job_UnLock")

            ' ���b�N�ς݃t���O�N���A
            mJobExecLock_Flg = False

        Finally
            LOG.Write_Exit3(sw, "ClsDBLock.Job_UnLock")
        End Try

    End Sub

End Class
'*** End Add 2015/12/01 SO)�r�� for �W���u���d���s�i�V�K�쐬�j ***
