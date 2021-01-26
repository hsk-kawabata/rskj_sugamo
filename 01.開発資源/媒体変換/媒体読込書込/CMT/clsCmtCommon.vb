Option Explicit On 
Option Strict On

Imports System.IO
Imports CASTCommon
Imports System.text
Imports System.Data.OracleClient
Imports System.Math
Imports CMT.ClsCMTCTRL

' �@�@�\ : CMT�ϊ��@�\�p���ʊ֐��N���X
'
' ���@�l : �Ȃ�ׂ����̋��ʊ֐��̂��̂𗘗p���A�g������̂������ꍇ�ɂ̂�
'       �@ �����ɒǉ�����B
'
Public Class clsCmtCommon
    Public gstrSysDate As String    ' �N�������̃V�X�e�����t(yyyymmdd�̌`��)
    Public gstrStationNo As String   ' ��tPC�̋@��
    'Public gstrStationName As String ' PC��

    Private Const ThisModuleName As String = "clsCmtCommon.vb"

    ' �@�\�@ �F �������֐�
    '
    ' �����@ �F �Ȃ�
    ' �߂�l �F
    Public Sub New()
        'gstrStationName = System.Environment.MachineName()
    End Sub

    ' �@�@�\ : CMT�Ǎ�����
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - �t�H�[�}�b�g�敪
    ' �@�@�@   ARG2 - �����Ǎ��t���O
    '          ARG3 - ListView(�Q�Ɠn)
    ' �@�@�@   ARG4 - �Ǎ�CMT�{��
    '
    ' �� �l  : 
    Public Function CmtReader(ByVal nFormatKbn As Integer _
        , ByVal bOverrideFlag As Boolean _
        , ByRef listview As ListView _
        , ByVal nReadQuantity As Integer _
        ) As Boolean

        ' ���������p�ϐ��̐錾
        Dim nReceptionNo, nFirstReceptionNo As Integer      ' ��t�ԍ�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim strSQL As String                                ' SQL���i�[�p�ϐ�
        Dim strPreRecord As String = ""                     ' ��O�̃��R�[�h�̃f�[�^�敪
        Dim cmtReadData(nReadQuantity) As clsCmtData
        Dim filename As String = ""
        Dim serverpath As String = ""
        Dim bJSFlag As Boolean = False                      ' ���U/���U�̃t���O
        Dim bCMTExist As Boolean = False                    ' CMT����{�ł����݂�����True

        Try
            GCom.GLog.Job2 = "CMT�Ǎ�"

            For i As Integer = 0 To nReadQuantity - 1
                cmtReadData(i) = New clsCmtData
            Next i

            ' �t�H�[�}�b�g�敪�ɉ����ăf�[�^�ێ��N���X�̃C���X�^���X����
            If Not GetFormat(nFormatKbn, cmtFormat) Then
                Return False
            End If

            ' ���[�J���t�H���_�̍폜
            If Not Me.LocalFileDelete(True) Then
                MessageBox.Show("CMT�ǎ�@�̈ꎞ�t�@�C���p�t�H���_�̏����Ɏ��s���܂����B", "CMT�ǎ掸�s" _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If Not ReadStacker(11, MAXSTACKER) Then ' CMT�Ǎ��֐����Ăяo��
                Return False
            End If

            ' ��t�ԍ�(nReceptionNo)���擾
            If Not CmtCom.GetReceptionNo(True, nReceptionNo) Then
                ' ��t�ԍ��擾���s
                ' ���O�o�͂͊֐������ōs���Ă��邽�߉������Ȃ��ŏI��
                Return False
            End If

            nFirstReceptionNo = nReceptionNo ' �擪�̎�t�ԍ�����Ŏg�����߃R�s�[

            For i As Integer = 0 To nReadQuantity - 1
                With cmtReadData(i)
                    .ReadSucceedFlag = False ' CMT�Ǎ��t���O�����炩����False�ŏ�����
                    .bListUpFlag = False
                    If GetCmtReadResult(i + 1) <> 1 Then
                        ' ListView�ɕ\�����ׂ����b�Z�[�W�����݂���Ƃ�(CMT����ȊO)
                        ' CMT�Ǎ��f�[�^�ێ��N���X�̏�����
                        .Init(nReceptionNo, i + 1, False, True)
                        .bListUpFlag = True

                        If GetCmtReadResult(i + 1) = 0 Then
                            ' CMT�t�@�C���̐���Ǎ�����
                            .ReadSucceedFlag = True
                            bCMTExist = True

                            ' CMT�̓Ǎ�.��͂��Ȃ���f�[�^�ێ��N���X�ɗ��ߍ���
                            Try
                                '*** �C�� mitsu 2009/04/?? �Z���^�[���ڎ����Í���CMT�Ή� ***
                                If nFormatKbn = 4 Then
                                    Dim AngouFileName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DATBK"), "COMPLOCK")
                                    Dim FukugouFileName As String = Path.Combine(MakeServerFileName(True, True, nFormatKbn), "NCOMPLOCK.dat")

                                    'DATBK�ɈÍ����t�@�C�����R�s�[����
                                    File.Copy(gstrCMTReadFileName(i), AngouFileName, True)

                                    '������EXE���W���u�o�^����
                                    Dim clsFUSION As clsFUSION.clsMain = New clsFUSION.clsMain
                                    clsFUSION.fn_INSERT_JOBMAST("T104", GCom.GetUserID, AngouFileName)

                                    '�������t�@�C�����o����܂őҋ@����(3���҂��Ă��I���Ȃ���΃G���[)
                                    Dim waitCnt As Integer = 0
                                    While File.Exists(FukugouFileName) = False
                                        Threading.Thread.Sleep(1000)
                                        waitCnt += 1

                                        If waitCnt > 180 Then
                                            MessageBox.Show("CMT�t�@�C����������܂���B" & vbCrLf & "�t�@�C���̕������Ɏ��s���Ă���\��������܂��B", _
                                                "CMT�Ǎ�", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                            Return False
                                        End If
                                    End While

                                    '�t�@�C�������[�J����CMT�t�H���_�Ɉړ����Ēʏ��CMT�t�@�C���Ɠ��l�ɏ�������
                                    File.Delete(gstrCMTReadFileName(i))
                                    File.Move(FukugouFileName, gstrCMTReadFileName(i))
                                End If
                                '***********************************************************

                                If (nFormatKbn = 5) Then
                                    ' �Z���^�[����(���ʃf�[�^�̏ꍇ)
                                    .InitEmptyItakuDataNoErr(True)
                                    .ErrCode = 0
                                    .bListUpFlag = True
                                    bJSFlag = True
                                ElseIf cmtFormat.FirstRead(gstrCMTReadFileName(i)) = 0 Then ' 1.CMT�t�@�C���̕����I�Ǎ�
                                    ' �Ǎ����s
                                    .InitEmptyItakuData(True)
                                    ' �G���[���e��DB�ɕۑ�
                                    strSQL = .GetInsertSQL(0) ' �t�@�C��SEQ�P�ʂ�SQL������
                                    GCom.DBExecuteProcess(4, strSQL)
                                    Exit Try
                                Else ' �����I�Ǎ�����
                                    ' 2.�t�H�[�}�b�g�̉�͊J�n
                                    ParseCMTFormat(nFormatKbn, cmtFormat, cmtReadData(i), True)

                                    ' 3.���U�E���U�̎�������
                                    If .ItakuCounter >= 0 Then
                                        GetJSFlag(.ItakuData(0), cmtReadData(i), bJSFlag)

                                        '***Astar���� 2008/05/30
                                        .JSFlag = bJSFlag
                                        '***

                                        '*** ASTAR.S.S 2008.05.23 �}�̋敪����Ή�  ***
                                        If .NotError() Then
                                            '*** �C�� mitsu 2008/10/21 .ItakuData(0)�̕\�L�ȗ� ***
                                            Dim id As clsItakuData = .ItakuData(0)
                                            '*****************************************************

                                            If id.strBaitaiCode <> "06" Then
                                                '*** �C�� mitsu 2008/10/21 �Z���^�[���ڎ����t�H�[�}�b�g���Ńf�[�^�Ή� ***
                                                '�Z���^�[���ڎ��������ł̏ꍇ�͔}�̋敪����𖳎�����
                                                If Not (nFormatKbn = 4 AndAlso _
                                                    (id.strToriSCode & id.strToriFCode = GetFSKJIni("TOUROKU", "KOKUZEI020") Or _
                                                     id.strToriSCode & id.strToriFCode = GetFSKJIni("TOUROKU", "KOKUZEI300"))) Then
                                                    '********************************************************************
                                                    cmtReadData(i).setError(9, "�}�̋敪����")
                                                With GCom.GLog
                                                        .Result = MenteCommon.clsCommon.NG
                                                    .Discription = "�}�̋敪���� �ϑ��҃R�[�h�F" & id.strItakuCode & ", �}�̋敪�F" & id.strBaitaiCode
                                                End With
                                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                                End If
                                                '*** �C�� mitsu 2008/10/21 �Z���^�[���ڎ����t�H�[�}�b�g���Ńf�[�^�Ή� ***
                                            End If
                                            '****************************************************************************
                                        End If
                                    Else
                                        ' �ϑ��҂����݂��Ȃ��ꍇ = ���R�[�h���[��
                                        .ComplockInit("", "")     ' �ǎ�ł��Ȃ��������߁A�ϑ��ҕs���Ńf�[�^����
                                        .ComplockFlag = False
                                        .Override = False
                                        .bListUpFlag = True
                                        .setError(9, "CMT�Ǎ����s")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "CMT�t�@�C�����̈ϑ��Ґ��O CMT�t�@�C�����Ƀw�b�_���R�[�h�����݂��Ȃ��\��������܂�"
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    End If
                                End If

                                If .NotError() Then
                                    ' 4.�t�@�C���̃A�b�v���[�h����(.tmp�ŃR�s�[)
                                    ' -- �t�@�C�����쐬

                                    '***�`�������̃t�@�C������z��
                                    If nFormatKbn <> 7 Then
                                        If .ItakuData(0).strMultiKbn = "0" Then
                                            filename = "D" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode
                                        Else
                                            filename = "D" & .ItakuData(0).strItakuKanriCode
                                        End If
                                    Else
                                        If .ItakuData(0).strMultiKbn = "0" Then
                                            filename = "SD" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode
                                        Else
                                            filename = "SD" & .ItakuData(0).strItakuKanriCode
                                        End If
                                    End If


                                    serverpath = MakeServerFileName(True, bJSFlag, nFormatKbn)

                                    If serverpath = "ERR" Then
                                        .setError(6, "�T�[�o�ւ̃A�b�v���[�h�Ɏ��s���܂����B")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "�T�[�o�̃t�H���_�������Ɏ��s���܂��� ���U���U�敪�F" & bJSFlag.ToString & ", �t�H�[�}�b�g�敪�F" & nFormatKbn.ToString
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        .bListUpFlag = True
                                    Else
                                        .FileName = filename & ".dat" ' �t�@�C�����ݒ�

                                        ' -- �A�b�v���[�h - 
                                        If (nFormatKbn <> 5) Then
                                            .bUploadSucceedFlg = upload(gstrCMTReadFileName(i), serverpath & filename & ".tmp", cmtReadData(i), cmtFormat.RecordLen)
                                        Else ' �Z���^�[����(���ʃf�[�^)
                                            .bUploadSucceedFlg = upload(gstrCMTReadFileName(i), serverpath & filename & ".tmp", cmtReadData(i), 165)
                                        End If

                                        If Not .bUploadSucceedFlg Then
                                            .setError(6, "�T�[�o�ւ̃A�b�v���[�h���s")
                                            With GCom.GLog
                                                .Result = MenteCommon.clsCommon.NG
                                                .Discription = "�T�[�o��ւ̃A�b�v���[�h���s " & cmtReadData(i).FileName & ".tmp���� .dat"
                                            End With
                                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        Else
                                            If Not Me.RenameFileName(serverpath & filename & ".tmp", serverpath & filename & ".dat") Then
                                                cmtReadData(i).setError(6, "�T�[�o�ւ̃A�b�v���[�h�Ɏ��s���܂����B")
                                                With GCom.GLog
                                                    .Result = MenteCommon.clsCommon.NG
                                                    .Discription = "�T�[�o��ł̃��l�[�����s " & cmtReadData(i).FileName & ".tmp���� .dat"
                                                End With
                                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                                DeleteFile(serverpath & filename & ".tmp") ' ���l�[�����s�t�@�C���̍폜
                                            End If
                                        End If
                                    End If

                                    ' cmtReadData(i)�̓��e���e�[�u���ɏ�����
                                    For j As Integer = 0 To .ItakuCounter
                                        strSQL = .GetInsertSQL(j) ' �t�@�C��SEQ�P�ʂ�SQL������
                                        GCom.DBExecuteProcess(4, strSQL)
                                    Next
                                Else
                                    ' �G���[���e��DB�ɏo��
                                    strSQL = .GetInsertSQL(0) ' �t�@�C��SEQ�P�ʂ�SQL������
                                    GCom.DBExecuteProcess(4, strSQL)
                                End If
                                cmtFormat.Close()
                            Catch ex As Exception
                                With GCom.GLog
                                    .Job2 = "CMT�Ǎ�"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = ex.Message
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                'GCom.DBExecuteProcess(16) ' rollback
                            End Try
                        End If
                        .bListUpFlag = True
                        nReceptionNo += 1
                    Else
                        ' CMT����A���邢��CTM���@�����Ńt�@�C�������� ���@�G���[�ł͂Ȃ��̂ł��̂܂ܑf�ʂ�
                        .bListUpFlag = False
                    End If
                End With
            Next i

            ' ListView �ɍ��ڂ�ǉ�
            For i As Integer = 0 To nReadQuantity - 1
                If cmtReadData(i).bListUpFlag Then
                    Call AddListView(listview, cmtReadData(i), nFirstReceptionNo)
                    nFirstReceptionNo += 1
                End If
            Next i

            ' cmtFormat�C���X�^���X�̍폜
            cmtFormat.Close()
            cmtFormat.Dispose()

            For i As Integer = 0 To nReadQuantity - 1 ' cmtReadData�C���X�^���X�̍폜
                cmtReadData(i).Dispose()
            Next i

        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT�Ǎ�"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return True
    End Function

    ' �@�@�\ : CMT���}������Ă��Ȃ��Ƃ��G���[���b�Z�[�W��\��
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - CMT�L���t���O
    '
    ' �� �l  :
    Private Sub CMTNotExistErr()
        MessageBox.Show("CMT���[�_��CMT���Z�b�g����Ă��܂���", "CMT�ǎ�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)
        With GCom.GLog
            .Result = MenteCommon.clsCommon.NG
            .Discription = "CMT����(�X�^�b�J���Z�b�g,CMT���}��)"
        End With
        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    End Sub

    ' �@�@�\ : ComplockCMT�Ǎ�����
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - �ϑ��҃R�[�h
    '          ARG2 - ������R�[�h
    '          ARG3 - ����敛�R�[�h
    '          ARG4 - ListView(�Q�Ɠn)
    '
    ' �� �l  : 
    Public Function ComplockCmtReader(ByVal strItakuCd As String _
        , ByVal strToriSCd As String _
        , ByVal strToriFCd As String _
        , ByRef listview As ListView _
        ) As Boolean

        ' ���������p�ϐ��̐錾
        Dim nReceptionNo As Integer                         ' ��t�ԍ�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim strSQL As String                                ' SQL���i�[�p�ϐ�
        Dim cmtReadData As clsCmtData = New clsCmtData
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String                    ' F�����敪, ������R�[�h�A���R�[�h
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim bJSFlag As Boolean
        Dim serverpath As String                            ' �A�b�v���[�h��̃p�X
        Dim filename As String                              ' �A�b�v���[�h��̃t�@�C����(�g���q����)
        Dim nFormatKbn As Integer                           ' �t�H�[�}�b�g�敪
        Dim cmtc As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        Try
            If Not DB.GetFormatKbn(strItakuCd, nFormatKbn) Then
                Return False
            End If
            GCom.GLog.Job2 = "Complock�Ǎ�"

            If Not DB.GetFToriCode(strItakuCd, "", toris, torif, fkbn, nFormatKbn) Then
                '*** �C�� mitsu 2008/09/01 ���b�Z�[�W�{�b�N�X�� ***
                'With GCom.GLog
                '    .Result = "�ϑ��҃R�[�h�ɊY������ϑ��҂Ȃ�"
                '    .Discription = "�ϑ��҃R�[�h:" & strItakuCd
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("�ϑ��҃R�[�h�ɊY������ϑ��҂Ȃ�")
                '**************************************************
                Return False
            Else
                Select Case fkbn
                    Case "1" '���U
                        bJSFlag = True
                    Case "3" ' ���U
                        bJSFlag = False
                    Case Else
                        '*** �C�� mitsu 2008/09/01 ���b�Z�[�W�{�b�N�X�� ***
                        'With GCom.GLog
                        '    .Result = "F�����敪��1,3�ȊO"
                        '    .Discription = "F�����敪:" & fkbn
                        'End With
                        'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        MessageBox.Show("�����敪��1,3�ȊO")
                        '**************************************************
                        Return False
                End Select
            End If

            LocalFileDelete(True) ' ���[�J��PC�̃t�@�C���폜

            ' �t�H�[�}�b�g�敪�ɉ����ăf�[�^�ێ��N���X�̃C���X�^���X����
            GetFormat(nFormatKbn, cmtFormat)

            '*** �C�� mitsu 2008/09/01 CMT�ڑ��m�F ***
            If Not CheckConnectCMT() Then 'CMT���ڑ�����Ă��Ȃ��ꍇ
                Return False
            End If
            '*****************************************

            If Me.IsExistStacker(cmtc, True) Then ' �X�^�b�J�̗L�����`�F�b�N.
                Return False
            ElseIf Not ReadStacker(12, 1) Then ' CMT�Ǎ��֐����Ăяo��
                Return False '�Ǎ����s���ُ͈�I��
            ElseIf Not CmtCom.GetReceptionNo(True, nReceptionNo) Then ' ��t�ԍ�(nReceptionNo)���擾
                ' ��t�ԍ��擾���s
                ' ���O�o�͂͊֐������ōs���Ă��邽�߉������Ȃ��ŏI��
                Return False
            End If

            With cmtReadData
                .ReadSucceedFlag = False ' CMT�Ǎ��t���O�����炩����False�ŏ�����
                .bListUpFlag = False
                If GetCmtReadResult(1) = 1 Then
                    ' CMT����
                    Me.CMTNotExistErr()
                    Return False
                ElseIf GetCmtReadResult(1) = 0 Then
                    ' ListView�ɕ\�����ׂ����b�Z�[�W�����݂���Ƃ�(CMT����ȊO)
                    ' CMT�Ǎ��f�[�^�ێ��N���X�̏�����
                    .Init(nReceptionNo, 1, False, True)
                    .bListUpFlag = True

                    ' CMT�t�@�C���̐���Ǎ�����
                    .ReadSucceedFlag = True

                    ' CMT�̓Ǎ�.��͂��Ȃ���f�[�^�ێ��N���X�ɗ��ߍ���
                    Try
                        ' �t�@�C���̃A�b�v���[�h����(.tmp�ŃR�s�[)
                        ' -- �t�@�C���쐬
                        filename = "C" & CmtCom.gstrStationNo & CmtCom.gstrSysDate & (nReceptionNo.ToString).PadLeft(5, "0"c) _
                            & "." & fkbn & "." & toris & "." & torif ' �g���q�����̃t�@�C�����쐬
                        serverpath = MakeServerFileName(True, bJSFlag, nFormatKbn)
                        .FileName = filename & ".dat"
                        ' -- �t�@�C���R�s�[
                        upload(gstrCMTReadFileName(0), serverpath & filename & ".tmp", cmtReadData, cmtFormat.RecordLen)
                        ' �T�[�o�ɃA�b�v���[�h�����t�@�C����.tmp��.dat�Ƀ��l�[��
                        If .NotError Then
                            ' ����ɓǂݎ�ꂽ�ꍇ�݂̂��A�b�v���[�h
                            If Not Me.RenameFileName(serverpath & filename & ".tmp", serverpath & filename & ".dat") Then
                                ' ���l�[�����s��
                                cmtReadData.setError(6, "�T�[�o�ւ̃A�b�v���[�h�Ɏ��s���܂����B")
                                DeleteFile(serverpath & filename & ".tmp") ' ���l�[�����s�t�@�C���̍폜
                            End If
                        End If
                    Catch ex As Exception
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "�t�@�C���R�s�[���s " & ex.Message
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End Try
                Else
                    ' CMT���ُ�
                    .bListUpFlag = True ' �G���[��\�����邽��True�ɐݒ�
                    CheckCmtReadResult(1, cmtReadData)
                End If
                ' �G���[�̗L�����킸�A�\���p�EDB�ۑ��p�̃f�[�^���쐬
                .ComplockInit(strItakuCd, "00000000")
                .ComplockFlag = True
                ' cmtReadData(i)�̓��e���e�[�u���ɏ�����
                strSQL = .GetInsertSQL(0) ' �t�@�C��SEQ�P�ʂ�SQL������
                GCom.DBExecuteProcess(4, strSQL)
            End With

            ' ListView �ɍ��ڂ�ǉ�
            If cmtReadData.bListUpFlag Then
                Call AddListView(listview, cmtReadData, nReceptionNo)
            End If

            If listview.Items.Count > 0 AndAlso _
                MessageBox.Show("��������s���܂����H", "ComplockCMT�Ǎ����ʈ��", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Me.PrintButton(listview, "ComplockCMT�Ǎ�����")
            End If

            ' cmtFormat�C���X�^���X�̍폜
            cmtFormat.Close()
            cmtFormat.Dispose()
            cmtReadData.Dispose()
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "Complock�Ǎ�"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return True
    End Function


    ' �@�@�\ : TORIMAST���玩�U�E���U�̃t���O���擾
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - clsItakuData�̃C���X�^���X
    '        : ARG2 - �G���[���i�[���邽�߂�clsCmtData�̃C���X�^���X
    '          ARG3 - �擾���ʂ��i�[����Bool�l�t���O
    '
    ' �� �l  :
    Private Function GetJSFlag(ByVal itakudata As clsItakuData, ByVal cmtd As clsCmtData, ByRef bJSFlag As Boolean) As Boolean
        Select Case itakudata.strFSyoriKbn
            Case "1"
                bJSFlag = True
            Case "3"
                bJSFlag = False
            Case Else
                'cmtd.setError(9, "�f�[�^�敪�s��")         ' 2008/04/06 �O�c �C��
                cmtd.setError(9, "�����}�X�^�Ȃ�")        ' 2008/04/06 �O�c �C��
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�����敪�s�� �ϑ��҃R�[�h�F" & itakudata.strItakuCode & ", �����敪�F" & itakudata.strFSyoriKbn
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
        End Select
        Return True
    End Function


    ' �@  �\ : Listview�ǉ�
    '
    ' �߂�l : �G���[�� True / �G���[�L False
    '
    ' ������ : ARG1 - �ǉ���ListView
    '          ARG2 - clsCmtData�C���X�^���X
    '          ARG3 - ��t�ԍ�
    '          ARG4 - ���X�g�J�E���^
    Private Function AddListView(ByVal listv As ListView, ByVal cmtd As clsCmtData, ByVal nReceptionNo As Integer) As Boolean
        Try
            With cmtd
                For j As Integer = 0 To .ItakuCounter
                    Dim lv As New ListViewItem(nReceptionNo.ToString.PadLeft(5, "0"c))
                    .getListViewItem(lv, j)
                    If Not .NotError() Then
                        lv.BackColor = Color.Pink
                    Else
                        lv.BackColor = Color.White
                    End If
                    listv.Items.AddRange(New ListViewItem() {lv})
                Next j
            End With
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListView�ǉ����s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' �@  �\ : CMT�Ǎ����ʂ̃`�F�b�N
    '
    ' �߂�l : �G���[�� True / �G���[�L False
    '
    ' ������ : ARG1 - �X�^�b�J�ԍ�
    '          ARG2 - clsCmtData�̃C���X�^���X
    ' �� �l  : ����`�F�b�N���V�X�e�����t��薢���ł��邱�Ƃ��`�F�b�N
    Private Function CheckCmtReadResult(ByVal nStackerNo As Integer, ByVal cmtd As clsCmtData) As Boolean
        If nStackerNo < 1 Or nStackerNo > MAXSTACKER Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�w�肳�ꂽ�X�^�b�J�ԍ��͕s���ł� �X�^�b�J�ԍ��F" & nStackerNo.ToString
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        With cmtd
            Select Case GetCmtReadResult(nStackerNo)
                Case 2
                    .setError(9, "CMT����ł�")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT����ł� err code 9"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Case 3
                    .setError(4, "CMT�ǎ�@�̒��Ɋ��ɓ����̃t�@�C�������݂��Ă��܂�")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "���[�J��PC�ɐ�s�t�@�C������ err code 4"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Case 4
                    .setError(9, "CMT�ǎ�G���[")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT�ǎ�G���[ err code 4"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            End Select
        End With
        Return True
    End Function

    ' �@�@�\ : �U�֓��̃`�F�b�N
    '
    ' �߂�l : �G���[�� True / �G���[�L False
    '
    ' ������ : ARG1 - �U�֓��t(yyyymmdd�`��)
    '          ARG2 - �V�X�e�����t(yyyymmdd�`��)
    ' �� �l  : ����`�F�b�N���V�X�e�����t��薢���ł��邱�Ƃ��`�F�b�N
    Public Function CheckFuriDate(ByVal strFuriDate As String, ByVal strSysDate As String, ByVal bShowMsg As Boolean) As Boolean
        Dim furi As Date
        Dim sys As Date

        Try
            furi = DateValue(strFuriDate.Substring(0, 4) & "/" & strFuriDate.Substring(4, 2) & "/" & strFuriDate.Substring(6, 2))
            sys = DateValue(strSysDate.Substring(0, 4) & "/" & strSysDate.Substring(4, 2) & "/" & strSysDate.Substring(6, 2))
            'If furi <= sys Then
            '    ' �V�X�e�����t���ȑO�̓��t���U�֓��Ƃ��Ďw�肳��Ă���
            '    If bShowMsg Then
            '        MessageBox.Show("�V�X�e�����t:" & strSysDate & ", �U�֓�:" & strFuriDate, "�V�X�e�����t���ȑO�̓��t���U�֓��Ƃ��Ďw��", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    End If

            '    With GCom.GLog
            '        .Result = "�V�X�e�����t���ȑO�̓��t���U�֓��Ƃ��Ďw��"
            '        .Discription = "�V�X�e�����t:" & strSysDate & ", �U�֓�:" & strFuriDate
            '    End With
            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '    Return False
            'End If
        Catch ex As InvalidCastException
            If bShowMsg Then
                MessageBox.Show("�V�X�e�����t�F" & strSysDate & ", �U�֓��F" & strFuriDate, "�U�֓��A�V�X�e�����t������ł͂���܂���", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            '*** �C�� mitsu 2008/09/01 �s�v ***
            'With GCom.GLog
            '    .Result = "�U�֓��A�V�X�e�����t������ł͂���܂���"
            '    .Discription = "�V�X�e�����t:" & strSysDate & ", �U�֓�:" & strFuriDate
            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            'End With
            '**********************************
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "����`�F�b�N�G���[ �V�X�e�����t�F" & strSysDate & ", �U�֓��F" & strFuriDate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' �@�@�\ : ����`�F�b�N
    '
    ' �߂�l : �G���[�� True / �G���[�L False
    '
    ' ������ : ARG1 - �U�֓��t(yyyymmdd�`��)
    '          
    ' �� �l  : ����`�F�b�N�̂�
    Public Function CheckDate(ByVal strFuriDate As String) As Boolean
        Dim furi As Date

        Try
            furi = DateValue(strFuriDate.Substring(0, 4) & "/" & strFuriDate.Substring(4, 2) & "/" & strFuriDate.Substring(6, 2))
        Catch ex As InvalidCastException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�U�֓�������ł͂���܂��� �U�֓��F" & strFuriDate & " " & ex.Message
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            End With
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "����`�F�b�N�G���[ �U�֓��F" & strFuriDate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    ' �@�@�\ : TORIMAST���玩�U�E���U�̃t���O���擾
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - F�����敪
    '          ARG2 - �擾���ʂ��i�[����Bool�l�t���O
    ' �� �l  : �����Ƃ��Ĉϑ��҃R�[�h��n���ꍇ
    Private Function GetJSFlag(ByVal fkbn As String, ByRef bJSFlag As Boolean) As Boolean
        Select Case fkbn
            Case "1"
                bJSFlag = True
            Case "3"
                bJSFlag = False
            Case Else
                '*** �C�� mitsu 2008/09/01 �s�v ***
                'With GCom.GLog
                '    .Result = "�ϑ��҃R�[�h��F�����敪�s��"
                '    .Discription = "F�����敪�l:" & fkbn
                'End With
                '**********************************
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
        End Select
        Return True
    End Function

    ' �@�@�\ : �T�[�o�Ƀt�@�C�����A�b�v���[�h���ď������ʂ����O�ɋL�^
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - ���t�@�C����
    '          ARG2 - ��t�@�C����
    '          ARG3 - �G���[���b�Z�[�W�ݒ��clsCmtData
    '          ARG4 - ���R�[�h��
    ' �� �l  : �����Ƃ��Ĉϑ��҃R�[�h��n���ꍇ
    Private Function upload(ByVal source As String, ByVal distination As String, _
            ByVal cmtd As clsCmtData, ByVal rlen As Integer) As Boolean
        Dim nRet As Integer = CmtCom.BinaryCopy(source, distination, rlen)
        If nRet = 0 Then
            cmtd.bUploadSucceedFlg = True
            Return True
        End If
        ' �T�[�o�ւ�.tmp�t�@�C���̃A�b�v���[�h���s
        Select Case nRet
            Case 1
                cmtd.setError(6, "�T�[�o�ւ̃A�b�v���[�h�Ɏ��s���܂����B")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�T�[�o�ւ̃A�b�v���[�h���s ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 2
                cmtd.setError(7, "�t�@�C���������s")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�t�@�C���������s ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 3
                cmtd.setError(9, "�u���b�N���s��")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�u���b�N���s�� ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Select
        '*** �C�� mitsu 2008/09/01 �s�v ***
        'With GCom.GLog
        '    .Result = "�T�[�o�ւ̃A�b�v���[�h���s"
        '    .Discription = "��:" & source & ", ��:" & distination
        'End With
        'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '**********************************
        Return False
    End Function

    ' �@�@�\ : �T�[�o����t�@�C�����_�E�����[�h���ď������ʂ����O�ɋL�^
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - ���t�@�C����
    '          ARG2 - ��t�@�C����
    '          ARG3 - �G���[���b�Z�[�W�ݒ��clsCmtData
    '          ARG4 - ���R�[�h��
    ' �� �l  : �����Ƃ��Ĉϑ��҃R�[�h��n���ꍇ
    Private Function download(ByVal source As String, ByVal distination As String, ByVal cmtd As clsCmtData, ByVal rlen As Integer) As Boolean
        ' 3.�T�[�o���烍�[�J��PC�����p�t�H���_�ւ̃R�s�[

        Select Case BinaryCopy(source, distination, rlen)
            ' �߂�l : 0:����, 1:�t�@�C���Ȃ�, 2:�����ݐ悪���ɑ���, 3:�t�@�C�������s��
        Case 0
                ' �_�E�����[�h����
                cmtd.bUploadSucceedFlg = True
                Return True
            Case 1
                ' �_�E�����[�h���s
                cmtd.bUploadSucceedFlg = False
                cmtd.setError(9, "�ԋp�t�@�C���R�s�[���s")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�ԋp�t�@�C���R�s�[���s ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 2
                cmtd.bListUpFlag = False
                cmtd.setError(9, "�ԋp�t�@�C���R�s�[���s")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�ԋp�t�@�C���R�s�[���s ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 3
                cmtd.bListUpFlag = False
                cmtd.setError(9, "�ԋp�t�@�C���̃��R�[�h�����s���ł�")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�ԋp�t�@�C�����R�[�h���s�� ���t�@�C�����F" & source & ", ��t�@�C�����F" & distination & " ���R�[�h���F" & rlen.ToString
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Select
        Return False

    End Function

    ' �@�@�\ : CMT�t�@�C���̃`�F�b�N
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - CMT�X���b�g�ԍ�
    '          ARG2 - �t�H�[�}�b�g�敪
    '          ARG3 - ���ʂ��o�͂���clsCmtReadData�̃C���X�^���X
    '          ARG4 - �t�H�[�}�b�g�N���X�̃C���X�^���X
    '          ARG5 - CMT�C���X�^���X
    ' �� �l  :
    Private Function CheckCMTFile(ByVal nSlotNo As Integer, ByVal nFormatKbn As Integer _
        , ByRef aCmtReadData As clsCmtData, ByRef cmtFormat As CAstFormat.CFormat, ByRef cmt As CMT.ClsCMTCTRL) As Boolean

        Try
            'CMT�t�@�C���̓Ǎ�()
            If Not cmt.ReadCmt(CByte(nSlotNo)) Then
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�X�^�b�J�ԍ��F" & nSlotNo & "�œǍ����s"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT�t�@�C���`�F�b�N�G���[ �X�^�b�J�ԍ��F" & nSlotNo & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (cmtFormat.FirstRead(gstrCMTReadFileName(nSlotNo - 1)) = 0) Then
            aCmtReadData.InitEmptyItakuData(False)
            Return False
        End If

        ' CMT�t�@�C���̉��
        Try
            If (Not ParseCMTFormat(nFormatKbn, cmtFormat, aCmtReadData, True)) AndAlso _
                aCmtReadData.ErrCode = 0 Then
                aCmtReadData.setError(9, "�t�@�C���̃t�H�[�}�b�g�Ɉُ킪����܂�")
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT�t�@�C���`�F�b�N�G���[ �X�^�b�J�ԍ��F" & nSlotNo & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        '***ASTAR SUSUKI 2008.06.12         ***
        '***�w�b�_���x���������ݑΉ�
        Call LocalHeadCopy(nSlotNo)
        '**************************************

        Return True
    End Function

    ' �@�@�\ : �T�[�o�ԋp�t�@�C���̃`�F�b�N
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - �ϑ��҃R�[�h
    '          ARG2 - �U�֓�
    '          ARG3 - cmtWriteData
    '          ARG4 - cmtFormat�̃C���X�^���X
    '          ARG5 - �T�[�o�ԋp�t�@�C���̖��O
    '          ARG6 - �t�H�[�}�b�g�敪
    '
    ' �� �l  : 
    Private Function ParseServerFile(ByVal strFileName As String _
        , ByRef aCmtWriteData As clsCmtData, ByRef cmtFormat As CAstFormat.CFormat, ByVal nFormatKbn As Integer) As Boolean

        If Not File.Exists(strFileName) Then
            ' �����ΏۂƂȂ�ԋp�t�@�C�������݂���
            MessageBox.Show("�����ΏۂƂȂ�ԋp�t�@�C�������݂��܂���", "�ԋp�t�@�C���`�F�b�N")
            Return False
        End If

        If (cmtFormat.FirstRead() = 1) Then ' �Ǎ��ɐ������Ă��邩�`�F�b�N
            ' CMT�t�@�C���̉��
            If Not ParseCMTFormat(nFormatKbn, cmtFormat, aCmtWriteData, False) Then
                If aCmtWriteData.ErrCode = 0 Then
                    aCmtWriteData.setError(9, "�ԋp�t�@�C���ǎ�G���[")
                    Return False
                End If
            End If
        Else
            aCmtWriteData.InitEmptyItakuData(False)
            Return False
        End If

        Return True
    End Function


    ' �@�@�\ : CMT��������
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - �t�H�[�}�b�g�敪
    ' �@�@�@   ARG2 - �����Ǎ��t���O
    '          ARG3 - ListView(�Q�Ɠn)
    '          ARG4 - ����CMT�{��
    ' �� �l  :
    Public Function CmtWriter(ByVal nFormatKbn As Integer _
        , ByVal bOverrideFlag As Boolean _
        , ByRef listview As ListView _
        , ByVal nWriteQuantity As Integer _
        ) As Boolean

        ' ���������p�ϐ��̐錾
        Dim nReceptionNo, nFirstReceptionNo As Integer          ' ��t�ԍ�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing           ' �t�H�[�}�b�g�N���X
        Dim cmtfServer As CAstFormat.CFormat = Nothing          ' �t�H�[�}�b�g�N���X
        'Dim cmtFormat As CAstFormat.CFormat                     ' �t�H�[�}�b�g�N���X
        'Dim cmtfServer As CAstFormat.CFormat                    ' �t�H�[�}�b�g�N���X
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim strSQL As String                                    ' SQL���i�[�p�ϐ�
        Dim strPreRecord As String = ""                         ' ��O�̃��R�[�h�̃f�[�^�敪
        Dim cmtReadData(nWriteQuantity) As clsCmtData           ' CMT�Ǎ����ʂ̉�͌���
        Dim cmtWriteData(nWriteQuantity) As clsCmtData          ' �T�[�o�ԋp�t�@�C���̉�͌���
        Dim strWriteFileName(nWriteQuantity) As String          ' �T�[�o�ԋp�t�@�C����(�t���p�X)
        Dim cmtCtrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nWriteCounter As Integer = 0
        Dim bRet As Boolean
        Dim bJSFlag As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        '***ASTAR 2008.08.07 �u���b�N�T�C�Y�Ή� >>
        If nFormatKbn = 0 Then
            '�S��̏ꍇ�́C�u���b�N�T�C�Y�P�W�O�O
            cmtCtrl.BlockSize = 1800
            '*** �C�� mitsu 2008/09/12 �e��t�H�[�}�b�g�ɑΉ� ***
        ElseIf nFormatKbn = 1 Then
            cmtCtrl.BlockSize = 2100
        ElseIf nFormatKbn = 2 Then
            cmtCtrl.BlockSize = 3000
        ElseIf nFormatKbn = 3 Then
            cmtCtrl.BlockSize = 3900
            '�Z���^�[���ږ��Ή��I
            '****************************************************
        ElseIf nFormatKbn = 6 Then  'NHK
            cmtCtrl.BlockSize = 1800
        Else
            cmtCtrl.BlockSize = -1
        End If
        '***ASTAR 2008.08.07 �u���b�N�T�C�Y�Ή� <<

        Try
            GCom.GLog.Job2 = "CMT����"

            For i As Integer = 0 To nWriteQuantity - 1
                cmtReadData(i) = New clsCmtData
                cmtWriteData(i) = New clsCmtData
            Next i

            ' �t�H�[�}�b�g�敪�ɉ����ăf�[�^�ێ��N���X�̃C���X�^���X����
            If Not GetFormat(nFormatKbn, cmtFormat) Or Not GetFormat(nFormatKbn, cmtfServer) Then
                MessageBox.Show("�����݃{�^���������ɃG���[���������܂����B", "��������", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ' ���[�J���t�H���_�̑|��
            If (Not LocalFileDelete(True)) Or (Not LocalFileDelete(False)) Then
                MessageBox.Show("���[�J��PC���̕s�v�t�@�C���̍폜�Ɏ��s���܂����B", "��������", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If


            ' ��t�ԍ�(nReceptionNo)���擾
            If Not CmtCom.GetReceptionNo(False, nReceptionNo) Then
                ' ��t�ԍ��擾���s
                ' ���O�o�͂͊֐������ōs���Ă��邽�߉������Ȃ��ŏI��
                Return False
            End If
            nFirstReceptionNo = nReceptionNo

            If Not Me.CheckConnectCMT() Then ' CMT���ڑ�����Ă��Ȃ��ꍇ�͏I��
                Return False
            End If

            ' CMT�荷���̌��o
            If (cmtCtrl.SelectCmt(1)) Then ' 1�{�ڂ����݂��邩�`�F�b�N
                '***�C�� ���� �������݂������Ȃ��̂ŏC�� 2008.11.13 start
                'If Not Me.IsExistStacker(cmtCtrl, False) Then ' 2�{�ȏ㑶�݂��Ȃ������`�F�b�N
                'CMT��1�{�����̏ꍇ
                nWriteQuantity = 1 ' �ǎ�{������{�����ɂ���
                'End If
                '***�C�� ���� �������݂������Ȃ��̂ŏC�� 2008.11.13 end
            End If

            ' CMT�X�^�b�J����
            For i As Integer = 0 To nWriteQuantity - 1
                Try
                    nWriteCounter = 0
                    With cmtReadData(i)
                        .Init(i + 1, i + 1, False, True)
                        If cmtCtrl.SelectCmt(CByte(i + 1)) Then
                            ' cmt�����f�[�^�ێ��C���X�^���X�̏�����
                            cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                            cmtWriteData(i).bListUpFlag = True
                            cmtWriteData(i).bUploadSucceedFlg = False

                            bRet = CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl)

                            If bRet AndAlso .ItakuCounter >= 0 Then
                                ' CMT�t�@�C���̓Ǎ��ɐ���
                                .ReadSucceedFlag = True

                                ' �ԋp�t�@�C�����쐬
                                If .ItakuData(0).strMultiKbn = "0" Then
                                    strWriteFileName(i) = MakeServerFileName(False, True, nFormatKbn) & "O" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode & ".dat"
                                Else
                                    strWriteFileName(i) = MakeServerFileName(False, True, nFormatKbn) & "O" & .ItakuData(0).strItakuKanriCode & ".dat"
                                End If

                                If File.Exists(strWriteFileName(i)) Then ' �ԋp�t�@�C���̑��݃`�F�b�N
                                    ' �Ή�����ԋp�t�@�C��������

                                    cmtWriteData(i).FileName = "N" & .ItakuData(0).strItakuCode & .ItakuData(0).strFuriDate & ".dat"
                                    cmtWriteData(i).bListUpFlag = True

                                    Try
                                        ' 1.�ԋp�t�@�C���̌����Ɖ��
                                        bRet = ParseServerFile(strWriteFileName(i), cmtWriteData(i), cmtFormat, nFormatKbn)
                                        cmtWriteData(i).WriteCounter = nWriteCounter
                                    Catch ex As Exception
                                        cmtWriteData(i).setError(5, "�T�[�o��̕ԋp�t�@�C��������������܂���")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "�T�[�o��̕ԋp�t�@�C��������������܂��� �t�@�C�����F" & strWriteFileName(i) & ex.Message
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    End Try

                                    If Not bRet Then
                                        ' �������Ȃ��Ƃ��͉������Ȃ�

                                    ElseIf (cmtfServer.FirstRead(strWriteFileName(i)) <> 1) Then ' �擪���R�[�h��Ǎ�
                                        ' �ԋp�t�@�C���̓Ǎ����s
                                        cmtWriteData(i).InitEmptyItakuData(False)

                                    ElseIf CompareCMTFile(nFormatKbn, cmtFormat, cmtfServer, cmtWriteData(i)) AndAlso _
                                            DB.GetWriteCounter(cmtWriteData(i).ItakuData(0).strItakuCode, _
                                                cmtWriteData(i).ItakuData(0).strFuriDate, nWriteCounter) Then
                                        ' 2�ԋp�t�@�C����CMT�t�@�C���̔�r
                                        '   a.�N���X���m�̔�r
                                        '   b.�擪5���R�[�h���m�̔�r
                                        ' 3.CMT��������TBL���Q�Ƃ��A�ߋ��ɉ��񏑂�����ł��邩���`�F�b�N����
                                        '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ� START
                                        'DB.GetFToriCode(cmtWriteData(i).ItakuData(0).strItakuCode, "", toris, torif, fkbn, nFormatKbn)
                                        DB.aGetFToriCode(cmtWriteData(i).ItakuData(0).strItakuCode, "", toris, torif, fkbn, nFormatKbn, cmtWriteData(i).ItakuData(0).strFuriDate)
                                        '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ�  END
                                        Call GetJSFlag(fkbn, bJSFlag)
                                        If Not bJSFlag Then
                                            cmtWriteData(i).setError(9, "�����U�f�[�^�ɂ͑Ή����Ă��܂���")
                                            cmtWriteData(i).bListUpFlag = True
                                            With GCom.GLog
                                                .Result = MenteCommon.clsCommon.NG
                                                .Discription = "�����U�f�[�^ �t�@�C�����F" & cmtWriteData(i).FileName
                                            End With
                                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        Else
                                            '2��ȏ㏑�����܂�Ă��Ă��A��������OK�Ή� start
                                            'If (nWriteCounter >= 2) Then
                                            '    cmtWriteData(i).setError(9, "�����CMT�ԋp�t�@�C�������ɓ��ȏ㏑���ݍς݂ł�")
                                            '    With GCom.GLog
                                            '        .Result = MenteCommon.clsCommon.NG
                                            '        .Discription = "����t�@�C�������ݍ� �t�@�C�����F" & cmtWriteData(i).FileName
                                            '    End With
                                            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                            'Else
                                            Dim Ret As Boolean
                                            ' 3.�T�[�o���烍�[�J��PC�����p�t�H���_�ւ̃R�s�[
                                            Ret = download(strWriteFileName(i), gstrCMTWriteFileName(i), cmtWriteData(i), cmtFormat.RecordLen)

                                            ' 4.�ԋp�t�@�C����CMT�ւ̏���
                                            If Not cmtCtrl.WriteCmt(CByte(i + 1)) Then
                                                cmtWriteData(i).setError(9, "CMT�����݃G���[")
                                                Ret = False
                                            End If

                                            '5.�}�̏������݌���
                                            If Ret Then
                                                If nFormatKbn <> 6 Then 'NHK�ȊO
                                                    Dim BkCmtWriteData As clsCmtData = cmtWriteData(i)
                                                    Dim ItakuCounter As Integer = cmtWriteData(i).ItakuCounter
                                                    .Init(i + 1, i + 1, False, True)
                                                    cmtCtrl.SelectCmt(CByte(i + 1))
                                                    cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                    cmtWriteData(i).bListUpFlag = True
                                                    cmtWriteData(i).bUploadSucceedFlg = False
                                                    File.Delete(cmtFormat.FileName)
                                                    If CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl) = False OrElse _
                                                        ChkCMTFile(nFormatKbn, cmtFormat, cmtWriteData(i)) = False Then
                                                        With GCom.GLog
                                                            .Result = MenteCommon.clsCommon.NG
                                                            .Discription = "�}�̏������݌��؎��s"
                                                        End With
                                                        cmtWriteData(i) = BkCmtWriteData
                                                        cmtWriteData(i).ErrCode = 9
                                                        cmtWriteData(i).ItakuCounter = ItakuCounter
                                                        cmtWriteData(i).setError(9, "�}�̏������݌��؎��s")
                                                    Else
                                                        cmtWriteData(i) = BkCmtWriteData
                                                        cmtWriteData(i).ItakuCounter = ItakuCounter
                                                    End If
                                                    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �v���e�N�g�������I�� START
                                                End If
                                            Else
                                                MessageBox.Show(MSG0033E, "CMT��������", _
                                                           MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                cmtWriteData(i).ErrCode = 9
                                                cmtWriteData(i).setError(9, "���������ݎ��s")
                                                GoTo NEXT_RECORD
                                                '2010.03.18 �ߗ��M���J�X�^�}�C�Y �v���e�N�g�������I��  END
                                            End If
                                            '����������
                                            If MessageBox.Show(String.Format(MSG0069I, toris, torif, .ItakuData(0).strItakuKana), "CMT��������", _
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                                                Dim BkCmtWriteData As clsCmtData = cmtWriteData(i)
                                                Dim ItakuCounter As Integer = cmtWriteData(i).ItakuCounter
                                                Try
                                                    cmtCtrl.UnloadCmt()
                                                    MessageBox.Show(MSG0070I, "CMT��������", _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                    cmtCtrl.ReadCmt(CByte(1))
                                                    'CMT���̃t�@�C����ǂݍ���
                                                    .Init(i + 1, i + 1, False, True)
                                                    cmtCtrl.SelectCmt(CByte(i + 1))
                                                    cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                    cmtWriteData(i).bListUpFlag = True
                                                    cmtWriteData(i).bUploadSucceedFlg = False
                                                    File.Delete(cmtFormat.FileName)
                                                    Ret = CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl)
                                                Catch ex As Exception
                                                    Ret = False
                                                End Try

                                                If Ret AndAlso .ItakuCounter >= 0 AndAlso _
                                                    CompareCMTFile(nFormatKbn, cmtFormat, cmtfServer, cmtWriteData(i)) Then
                                                    ' 2�ԋp�t�@�C����CMT�t�@�C���̔�r

                                                    If File.Exists(gstrCMTWriteFileName(i)) Then
                                                        File.Delete(gstrCMTWriteFileName(i))
                                                    End If

                                                    ' 3.�T�[�o���烍�[�J��PC�����p�t�H���_�ւ̃R�s�[
                                                    download(strWriteFileName(i), gstrCMTWriteFileName(i), cmtWriteData(i), cmtFormat.RecordLen)

                                                    ' 4.�ԋp�t�@�C����CMT�ւ̏���
                                                    If Not cmtCtrl.WriteCmt(CByte(i + 1)) Then
                                                        cmtWriteData(i).setError(9, "CMT�����݃G���[")
                                                        Ret = False
                                                    End If

                                                    '5.�}�̏������݌���
                                                    If Ret Then
                                                        If nFormatKbn <> 6 Then 'NHK�ȊO
                                                            .Init(i + 1, i + 1, False, True)
                                                            cmtCtrl.SelectCmt(CByte(i + 1))
                                                            cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                            cmtWriteData(i).bListUpFlag = True
                                                            cmtWriteData(i).bUploadSucceedFlg = False
                                                            File.Delete(cmtFormat.FileName)
                                                            If CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl) = False OrElse _
                                                                ChkCMTFile(nFormatKbn, cmtFormat, cmtWriteData(i)) = False Then
                                                                With GCom.GLog
                                                                    .Result = MenteCommon.clsCommon.NG
                                                                    .Discription = "���}�̏������݌��؎��s"
                                                                End With
                                                                cmtWriteData(i) = BkCmtWriteData
                                                                cmtWriteData(i).ErrCode = 9
                                                                cmtWriteData(i).ItakuCounter = ItakuCounter
                                                                cmtWriteData(i).setError(9, "���}�̏������݌��؎��s")
                                                            Else
                                                                cmtWriteData(i) = BkCmtWriteData
                                                                cmtWriteData(i).ItakuCounter = ItakuCounter
                                                            End If
                                                            nWriteCounter += 1
                                                        End If
                                                    Else
                                                        If Not Ret AndAlso nFormatKbn <> 6 Then
                                                            MessageBox.Show(MSG0033E, "CMT��������", _
                                                                       MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                            cmtWriteData(i) = BkCmtWriteData
                                                            cmtWriteData(i).ErrCode = 9
                                                            cmtWriteData(i).ItakuCounter = ItakuCounter
                                                            cmtWriteData(i).setError(9, "���������ݎ��s")
                                                        End If
                                                    End If
                                                Else
                                                    MessageBox.Show(MSG0033E, "CMT��������", _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    cmtWriteData(i) = BkCmtWriteData
                                                    cmtWriteData(i).ErrCode = 9
                                                    cmtWriteData(i).ItakuCounter = ItakuCounter
                                                    cmtWriteData(i).setError(9, "���ǂݍ��ݎ��s")
                                                End If

                                            End If
                                            '2��ȏ㏑�����܂�Ă��Ă��A��������OK�Ή� start
                                            nWriteCounter += 1
                                            'End If
                                            '2��ȏ㏑�����܂�Ă��Ă��A��������OK�Ή� end
                                            End If
                                        cmtWriteData(i).WriteCounter = nWriteCounter
                                        cmtfServer.Close()
                                    End If
                                Else
                                    ' �Ή�����ԋp�t�@�C����������
                                    With GCom.GLog
                                        .Result = MenteCommon.clsCommon.NG
                                        .Discription = "�ԋp�t�@�C���Ȃ� �t�@�C�����F" & strWriteFileName(i)
                                    End With
                                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    cmtWriteData(i).InitEmptyItakuData(False)
                                    cmtWriteData(i).setError(9, "�ԋp�t�@�C������")
                                End If
                            Else
                                ' CMT�Ǎ����s
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMT�Ǎ��G���[ �X�^�b�J�ԍ��F" & (i + 1).ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                cmtWriteData(i).InitEmptyItakuData(True)
                            End If
                            cmtFormat.Close()

                            nReceptionNo += 1
                            ' 6.CMT��������TBL�ɏ����� - ���펞�E�G���[���Ƃ��ɏ�����
                            For j As Integer = 0 To cmtWriteData(i).ItakuCounter
                                strSQL = cmtWriteData(i).GetWInsertSQL(j) ' �t�@�C��SEQ�P�ʂ�SQL������
                                GCom.DBExecuteProcess(4, strSQL)
                            Next j
                        End If
                    End With
                    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �v���e�N�g�������I�� START
NEXT_RECORD:
                    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �v���e�N�g�������I��  END
                    ' 7.ListView�Ɍ��ʂ�\��
                    If cmtWriteData(i).bListUpFlag Then
                        Me.AddListView(listview, cmtWriteData(i), nFirstReceptionNo)
                        nFirstReceptionNo += 1
                    Else
                        'MessageBox.Show("�X�^�b�J�ԍ�:" & (i + 1).ToString & "�͕\������")
                    End If
                Catch ex As Exception
                    With GCom.GLog
                        .Job2 = "CMT����"
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = ex.Message
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End Try
            Next i

            If nWriteQuantity = 1 Then
                cmtCtrl.UnloadCmt()
            Else
                cmtCtrl.UnloadCmt()
                cmtCtrl.SelectCmt(22) ' CMT�X�^�b�J�r�o
            End If

            cmtFormat.Close()
            cmtfServer.Close()
            cmtFormat.Dispose()
            cmtfServer.Dispose()


            For i As Integer = 0 To nWriteQuantity - 1
                '2��ڈȍ~��DENBK��move����B start
                'If cmtWriteData(i).WriteCounter > 1 Then
                '    ' �����J�E���^��2�ȏ�̂Ƃ��폜
                '    Me.DeleteFile(strWriteFileName(i))
                'End If
                If cmtWriteData(i).ErrCode = 0 Then
                    If cmtWriteData(i).WriteCounter > 0 Then
                        ' �����J�E���^��1�ȏ�̂Ƃ�DENBK��move
                        Dim INI_COMMON_DENBK As String = ""
                        INI_COMMON_DENBK = CASTCommon.GetFSKJIni("COMMON", "DENBK")
                        '�O��t�@�C�����폜
                        If File.Exists(INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i))) = True Then
                            File.Delete(INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i)))
                        End If
                        File.Move(strWriteFileName(i), INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i)))
                    End If
                    '2��ڈȍ~��DENBK��move����B end
                End If
            Next i

        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT����"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

    ' �@�@�\ : CMT����������������(Complock/��Complock��������)
    '
    ' �߂�l : �������� True / �������s False
    '
    ' ������ : ARG1 - �ϑ��҃R�[�h
    '          ARG2 - ������R�[�h
    ' �@�@�@   ARG3 - ����敛�R�[�h
    '          ARG4 - �U�֓�
    '          ARG5 - ListView(�Q�Ɠn)
    '          ARG6 - Complock(True)��������(false)
    ' �� �l  : ��Complock�̋��������ɑΉ�
    Public Function ComplockCmtWriter(ByVal strItakuCode As String _
        , ByVal strToriSCode As String _
        , ByVal strToriFCode As String _
        , ByVal strFuriDate As String _
        , ByRef listview As ListView _
        , ByVal bComplock As Boolean _
        ) As Boolean

        ' ���������p�ϐ��̐錾
        Dim nReceptionNo As Integer             ' ��t�ԍ�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing     ' �t�H�[�}�b�g�N���X
        'Dim cmtFormat As CAstFormat.CFormat     ' �t�H�[�}�b�g�N���X
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim strSQL As String                    ' SQL���i�[�p�ϐ��@
        Dim strPreRecord As String = ""         ' ��O�̃��R�[�h�̃f�[�^�敪
        Dim cmtReadData As clsCmtData           ' CMT�Ǎ����ʂ̉�͌���
        Dim cmtWriteData As clsCmtData          ' �T�[�o�ԋp�t�@�C���̉�͌���
        Dim strWriteFileName As String          ' �T�[�o�ԋp�t�@�C����(�t���p�X)
        Dim cmtCtrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nWriteCounter As Integer = 0
        Dim nFormatKbn As Integer
        Dim cmtc As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nLabelKbn As Integer

        If (bComplock) Then
            GCom.GLog.Job2 = "CMT�Í�������"
        Else
            GCom.GLog.Job2 = "��������"
        End If

        If (strFuriDate.Length < 8) Then
            MessageBox.Show("�U�֓����s���ł��B(" & strFuriDate & ").�L���ȓ��t����͂��Ă�������", "�U�֓��G���[")
            '*** �C�� mitsu 2008/09/01 �s�v ***
            'With GCom.GLog
            '    .Result = "�U�֓����s���ł�"
            '    .Discription = strFuriDate
            'End With
            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '**********************************
            Return False
        End If
        cmtReadData = New clsCmtData
        cmtWriteData = New clsCmtData

        '*** �C�� mitsu 2008/09/01 CMT�ڑ��m�F ***
        If Not Me.CheckConnectCMT() Then ' CMT���ڑ�����Ă��Ȃ��ꍇ�͏I��
            Return False
        End If
        '*****************************************

        ' TORI_VIEW����t�H�[�}�b�g�敪�̎擾
        If Not DB.GetFormatKbn(strItakuCode, nFormatKbn) Then
            '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
            MessageBox.Show("�t�H�[�}�b�g�敪�擾���s" & vbCrLf & "�ϑ��҃R�[�h�F" & strItakuCode, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        '*** �C�� mitsu 2008/09/12 �u���b�N�T�C�Y�Ή� ***
        Select Case nFormatKbn
            Case 0
                cmtCtrl.BlockSize = 1800
            Case 1
                cmtCtrl.BlockSize = 2100
            Case 2
                cmtCtrl.BlockSize = 3000
            Case 3
                cmtCtrl.BlockSize = 3900
            Case Else
                cmtCtrl.BlockSize = -1
        End Select
        '************************************************

        ' TORI_VIEW���烉�x���敪�̎擾
        If Not DB.GetFormatKbn(strItakuCode, nLabelKbn) Then
            '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
            MessageBox.Show("���x���敪�擾���s" & vbCrLf & "�ϑ��҃R�[�h�F" & strItakuCode, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        ' �t�H�[�}�b�g�敪�ɉ����ăf�[�^�ێ��N���X�̃C���X�^���X����
        If Not GetFormat(nFormatKbn, cmtFormat) Then
            Return False
        End If

        ' ���[�J���t�H���_�̑|��
        If (Not LocalFileDelete(True)) Or (Not LocalFileDelete(False)) Then
            '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
            MessageBox.Show("���[�J��PC���̕s�v�t�@�C���̍폜�Ɏ��s���܂����B", "��������", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        ' ��t�ԍ�(nReceptionNo)���擾
        If Not CmtCom.GetReceptionNo(False, nReceptionNo) Then
            ' ��t�ԍ��擾���s
            ' ���O�o�͂͊֐������ōs���Ă��邽�߉������Ȃ��ŏI��
            Return False
        End If

        Try
            ' CMT��1�{�����O��
            nWriteCounter = 0
            With cmtReadData
                .Init(1, 1, False, True)
                If Not IsExistStacker(cmtCtrl, True) Then
                    ' cmt�����f�[�^�ێ��C���X�^���X�̏�����
                    cmtWriteData.Init(nReceptionNo, 1, False, False)
                    cmtWriteData.bListUpFlag = True
                    cmtWriteData.bUploadSucceedFlg = False
                    cmtWriteData.Override = True

                    ' �ԋp�t�@�C�����쐬
                    If (bComplock) Then
                        strWriteFileName = MakeServerFileName(False, True, nFormatKbn) & "C" & strItakuCode & strFuriDate & ".dat"
                    Else
                        strWriteFileName = MakeServerFileName(False, True, nFormatKbn) & "N" & strItakuCode & strFuriDate & ".dat"
                    End If
                    If (bComplock) Then
                        cmtWriteData.ComplockInit(strItakuCode, strFuriDate)
                        cmtWriteData.ComplockFlag = bComplock
                    Else
                        If (cmtFormat.FirstRead(strWriteFileName) = 1) Then
                            If Not (Me.ParseCMTFormat(nFormatKbn, cmtFormat, cmtWriteData, False)) Then
                                Return False
                            End If
                            cmtWriteData.ComplockFlag = bComplock
                        Else
                            cmtWriteData.InitEmptyItakuData(False)
                        End If
                    End If
                    ' �ԋp�t�@�C���̑��݃`�F�b�N

                    If File.Exists(strWriteFileName) Then
                        ' �Ή�����ԋp�t�@�C��������

                        If (bComplock) Then
                            cmtWriteData.FileName = "C" & strItakuCode & strFuriDate & ".dat"
                        Else
                            cmtWriteData.FileName = "N" & strItakuCode & strFuriDate & ".dat"
                        End If

                        cmtWriteData.bListUpFlag = True

                        ' CMT��������TBL���Q�Ƃ��A�ߋ��ɉ��񏑂�����ł��邩���`�F�b�N����
                        Call DB.GetWriteCounter(strItakuCode, strFuriDate, nWriteCounter)

                        If (nWriteCounter >= 2) Then
                            MessageBox.Show("���ɓ����ԋp�t�@�C�������ȏ�CMT�ɏ����݂���Ă��܂�", "CMT�����G���[")
                            cmtWriteData.setError(9, "�����CMT�ԋp�t�@�C�������ɓ��ȏ㏑���ݍς݂ł�")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "����t�@�C�������ݍ� �t�@�C�����F" & cmtWriteData.FileName
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        Else
                            ' �T�[�o���烍�[�J��PC�����p�t�H���_�ւ̃R�s�[
                            If (download(strWriteFileName, gstrCMTWriteFileName(0), cmtWriteData, cmtFormat.RecordLen)) Then
                            Else
                                Return False
                            End If

                            ' �ԋp�t�@�C����CMT�ւ̏���
                            CMT.PutCMTIni("LABEL-EXIST", "1", nLabelKbn.ToString) ' ���x���敪�̏�����
                            If Not cmtc.WriteCmt(1) Then
                                Dim result As String = CMT.GetCMTIni("WRITE-RESULT", "1")
                                cmtWriteData.bUploadSucceedFlg = False
                                cmtWriteData.setError(9, "CMT�������s")
                            Else
                                cmtWriteData.bUploadSucceedFlg = True
                                nWriteCounter += 1
                                If (nWriteCounter >= 2) Then
                                    Me.DeleteFile(strWriteFileName) ' �����������ɃT�[�o�̃t�@�C�����폜
                                End If
                            End If
                        End If

                        cmtWriteData.WriteCounter = nWriteCounter

                        ' CMT��������TBL�ɏ�����
                        strSQL = cmtWriteData.GetWInsertSQL(0) ' �t�@�C��SEQ�P�ʂ�SQL������
                        GCom.DBExecuteProcess(4, strSQL)

                        ' ListView�Ɍ��ʂ�\��
                        If cmtWriteData.bListUpFlag Then
                            Try
                                Dim lv As New ListViewItem(nReceptionNo.ToString)
                                cmtWriteData.getListViewItem(lv, 0)
                                listview.Items.AddRange(New ListViewItem() {lv})
                            Catch ex As Exception
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "ListView�ǉ����s " & ex.Message
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                Return False
                            End Try
                        End If
                    Else
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "�T�[�o�ԋp�t�@�C���Ȃ� �t�@�C�����F" & strWriteFileName
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End If
                End If
            End With
        Catch ex As Exception
            With GCom.GLog
                If bComplock Then
                    .Job2 = "ComplockCMT����"
                Else
                    .Job2 = "CMT��������"
                End If
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If listview.Items.Count > 0 AndAlso _
            MessageBox.Show("��������s���܂����H", "CMT�Ǎ����ʈ��", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            If bComplock Then
                Me.PrintButton(listview, "ComplockCMT����")
            Else
                Me.PrintButton(listview, "CMT��������")
            End If
        End If

        cmtCtrl.UnloadCmt()
        cmtFormat.Dispose()

    End Function


    ' �@�\   �F �X�^�b�J�ɕ�����CMT�����݂��邩�`�F�b�N
    '
    ' ����    : ARG1 - �Ȃ�
    ' �߂�l  : �X�^�b�J�ɕ�����CMT�����ݑ��� / �X�^�b�J���� false
    Private Function IsExistStacker(ByVal cmtCtrl As CMT.ClsCMTCTRL, ByVal bComplock As Boolean) As Boolean
        Dim nSlotPosition As Byte        ' �X�^�b�J�ʒu
        Dim nCMTSum As Integer              ' CMT�{��

        If cmtCtrl.SelectCmt(21) Then
            cmtCtrl.ChkCmtStat(nSlotPosition, nCMTSum)
            If (nCMTSum > 1) Then
                If bComplock Then ' Complock�̏ꍇ�̓G���[���o��
                    MessageBox.Show("Complock�����@�\/CMT���������݂�CMT��{���ɏ������s���܂��B�X�^�b�J�ɂ͑Ή����Ă��܂���B", _
                        "�X�^�b�J�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.OK
                        .Discription = "������CMT�������ɃZ�b�g CMT�Z�b�g���F" & nCMTSum.ToString
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    cmtCtrl.SelectCmt(22) ' CMT Eject
                End If
                Return True
            Else
                If (nCMTSum = 0) Then
                    Me.CMTNotExistErr()
                    Return False
                End If
            End If
        Else
            Return True
        End If
    End Function


    ' �@�\   �F CMT���ڑ�����Ă��邩�`�F�b�N���A�G���[��Ԃ�
    '
    ' ����    : ARG1 - �Ȃ�
    ' �߂�l  : ���� true / ���s false
    Private Function CheckConnectCMT() As Boolean
        Dim CMTSCSIReader As New CMT.ClsCMTCTRL
        If Not CMTSCSIReader.ChkLoader() Then
            MessageBox.Show("CMT���ڑ�����Ă��Ȃ����A���邢�͓d���������Ă��܂���B", "CMT�ڑ��G���[", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT���[�_�̓d����OFF���A�P�[�u�����O��Ă���\��������܂�"
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Else
            Return True
        End If
    End Function


    ' �@�\   �F �X�^�b�J����CMT�t�@�C���̓Ǎ�
    '
    ' ����    : ARG1 - nRead
    ' �߂�l  : ���� true / ���s false
    Private Function ReadStacker(ByVal nRead As Integer, ByVal nMax As Integer) As Boolean
        ' CMT�Ǎ��֐����Ăяo��
        Dim CMTSCSIReader As New CMT.ClsCMTCTRL
        Dim nCmtRet As Integer
        Dim bStackerEmpty As Boolean = True
        Dim bNoCmt As Boolean = True
        Dim bRet As Boolean = True
        Try
            If Not CheckConnectCMT() Then 'CMT���ڑ�����Ă��Ȃ��ꍇ
                Return False
            End If

            ' CMT�N���X�̓Ǎ��֐�
            nCmtRet = CMTSCSIReader.CmtCtrl(nRead)

            ' CMT�X�^�b�J��`�F�b�N [READ-RESULT]���S��1�̂Ƃ��X�^�b�J����
            For i As Integer = 1 To nMax Step 1
                If CMT.GetCMTIni("READ-RESULT", i.ToString) <> "1" Then ' �e�X���b�g��CMT����ł͂Ȃ����`�F�b�N
                    bStackerEmpty = False
                End If
            Next i

            If bStackerEmpty Then ' �荷����CMT����{�������Ă��Ȃ�
                Me.CMTNotExistErr() 'CMT����̃G���[
                bRet = False
            End If

            If nCmtRet <> 0 Then
                If nMax = 1 Then
                    MessageBox.Show("CMT�̓Ǎ��Ɏ��s���܂���", "CMT�Ǎ��G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMT�Ǎ����s CMT.ClsCMTCTRL.CmtCtrl(11)�̕Ԃ�l:" & nCmtRet
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                bRet = False
            End If

            CMTSCSIReader.UnloadCmt()
            'CMTSCSIReader.SelectCmt(22) ' CMT Eject

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT�Ǎ����s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return bRet
    End Function


    ' �@�\   �F �t�H�[�}�b�g�敪�ɉ����ăf�[�^�ێ��N���X�̃C���X�^���X����
    '
    ' ����    : ARG1 - �t�H�[�}�b�g�敪
    '         : ARG2 - �t�H�[�}�b�g�̃C���X�^���X
    ' �߂�l  : ���� true / ���s false
    Private Function GetFormat(ByVal nkbn As Integer, ByRef cmtFormat As CAstFormat.CFormat) As Boolean
        Try
            Select Case nkbn
                Case 0, 6, 7
                    ' �S��t�H�[�}�b�g(�S��ENHK�E���U)
                    cmtFormat = New CAstFormat.CFormatZengin
                    Return True
                Case 1
                    ' �n�������c��1
                    cmtFormat = New CAstFormat.CFormatZeikin350
                    Return True
                Case 2
                    ' �n�������c��2 ����s
                    cmtFormat = New CAstFormat.CFormatZeikin300
                    Return True
                Case 3
                    ' ����
                    cmtFormat = New CAstFormat.CFormatKokuzei
                    Return True
                Case 4
                    ' �Z���^�[����(������)
                    cmtFormat = New CAstFormat.CFormatTokCenter
                    Return True
                Case 5
                    ' �Z���^�[����(���ʕ�)�̃t�H�[�}�b�g.���ۂɂ͎g��Ȃ����A���삳���邽�ߎw��
                    cmtFormat = New CAstFormat.CFormatTokCenter
                    Return True
                Case Else
                    ' ���̑��t�H�[�}�b�g��I��
                    MessageBox.Show("�������̃t�H�[�}�b�g���I������Ă��܂�", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Return False
            End Select
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�C���X�^���X�������s �t�H�[�}�b�g�敪�F" & nkbn.ToString & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

    End Function

    ' �@�\   �F �S��/�n��1/2/����/�Z���^�[�����t�H�[�}�b�g�̉��
    '
    ' ����    : ARG1 - CAstFormat.CFormat�̃C���X�^���X
    '         : ARG2 - ��͌��ʂ��i�[���邽�߂�clsCmtReadData�̃C���X�^���X
    '
    ' �߂�l  : ���� true / ���s false
    ' ���l    : �G���[�`�F�b�N�̎����R���ǉ�, �S��ȊO�̃t�H�[�}�b�g�ɑΉ� 2007/12/10
    Private Function ParseCMTFormat(ByVal nFormatKbn As Integer, ByRef cmtFormat As CAstFormat.CFormat, _
        ByRef cmtData As clsCmtData, ByVal bCheckZumiFlag As Boolean) As Boolean
        Dim strRecordKbn As String
        Dim nRecordCounter As Integer = 0

        Try
            With cmtData
                Do Until cmtFormat.EOF = True ' �t�H�[�}�b�g�̉�͊J�n
                    strRecordKbn = cmtFormat.CheckDataFormat()

                    '*** ASTAR.S.S �ُ펞�Ή�       ***
                    If cmtFormat.IsHeaderRecord = True Then
                        Call cmtFormat.CheckRecord1()

                        '***ASTAR SUSUKI 2008.06.12                     ***
                        If nFormatKbn = 3 Then
                            ' ���ł̏ꍇ
                            strRecordKbn = "H"
                            Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
                            KokuzeiFmt.KOKUZEI_REC1.Data = cmtFormat.RecordData
                            Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
                            KokuzeiFmt.Close()
                            Dim TORICODE As String
                            ' �ȖڃR�[�h�@020:�\��������, 300:����ŋy�n�������
                            TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
                            If TORICODE = "err" Then
                                TORICODE = ""
                            End If
                            cmtFormat.InfoMeisaiMast.ITAKU_CODE = TORICODE
                            'Dim MainDB As New CASTCommon.MyOracle
                            'cmtFormat.ToriData = New CAstBatch.CommData(MainDB)
                            'Call cmtFormat.GetTorimastFromToriCode(TORICODE, MainDB)
                            'If Not cmtFormat.ToriData Is Nothing Then
                            '    cmtFormat.InfoMeisaiMast.ITAKU_CODE = cmtFormat.ToriData.INFOToriMast.ITAKU_CODE_T
                            'End If
                            'MainDB.Close()
                        End If
                        '**************************************************

                        cmtFormat.InfoMeisaiMast.FILE_SEQ += 1
                        '***ASTAR SUSUKI 2008.06.12                         ***
                        '***���ł̏ꍇ�̓��R�[�h�敪�R�̂݃J�E���g �܂��� ���ňȊO
                        If nFormatKbn = 3 Then
                            .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, CASTCommon.ConvertDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO, "yyyyMMdd") _
                                , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", "", nFormatKbn)
                            '***************************************************

                            '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����̏ꍇ�͐U�փR�[�h�E��ƃR�[�h���n�� ***
                        ElseIf nFormatKbn = 4 Then
                            .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO _
                                , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                      , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT _
                                      , CType(cmtFormat, CAstFormat.CFormatTokCenter).TOKCENTER_REC1.TC.TC5 _
                                      , CType(cmtFormat, CAstFormat.CFormatTokCenter).TOKCENTER_REC1.TC.TC6 _
                                      , "", nFormatKbn)
                            '**********************************************************************************
                        Else
                            '*** �C�� mitsu 2008/07/23 ���t���ǂݎ��Ȃ��ꍇ�Ή� ***
                            If cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO.Length < 8 Then
                                .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE _
                                    , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                    , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", cmtFormat.InfoMeisaiMast.SYUBETU_CODE, nFormatKbn)
                            Else
                                .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO _
                                                      , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                                      , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", cmtFormat.InfoMeisaiMast.SYUBETU_CODE, nFormatKbn)
                            End If
                            '********************************************************
                        End If
                        nRecordCounter += 1
                        If Not Me.CheckFuriDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE, Me.gstrSysDate, False) Then
                            '*** �C�� mitsu 2008/09/01 �s�v ***
                            'With GCom.GLog
                            '    .Result = "CMT�t�H�[�}�b�g�敪��͏���"
                            '    .Discription = "�U�֓����s���ł�" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '**********************************
                            Return False
                        End If

                    ElseIf cmtFormat.IsDataRecord = True Then
                        ' �f�[�^���R�[�h�̒l��ݒ�
                        '***ASTAR SUSUKI 2008.06.12                         ***
                        '***���ł̏ꍇ�̓��R�[�h�敪�R�̂݃J�E���g �܂��� ���ňȊO
                        If nFormatKbn <> 3 Or (nFormatKbn = 3 And cmtFormat.InfoMeisaiMast.DATA_KBN = "3") Then
                            .ItakuData(.ItakuCounter).SetData(cmtFormat.InfoMeisaiMast.FURIKIN)
                            nRecordCounter += 1
                        End If
                        '******************************************************

                    ElseIf cmtFormat.IsTrailerRecord = True Then
                        If Not (.AddTrailerRecord(cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KIN _
                            , cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN _
                            , cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN, cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN)) Then
                            '***ASTAR SUSUKI 2008.06.12                     ***
                            '***�g���[���`�F�b�N���͂���
                            '.setError(9, "�g���[���������z�ƍ��G���[")
                            'With GCom.GLog
                            '    .Result = "�g���[���������z�ƍ��G���["
                            '    .Discription = "�U�֓����s���ł�" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            'Return False
                            '**************************************************
                        End If
                        If bCheckZumiFlag AndAlso ((cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN > 0)) Then
                            '***ASTAR SUSUKI 2008.06.12                     ***
                            '***�g���[���`�F�b�N���͂���
                            '.setError(9, "�g���[���������z�ƍ��G���[")
                            'With cmtFormat.InfoMeisaiMast
                            '    GCom.GLog.Result = "�g���[���ɕs�\���E�ό����܂܂�Ă��܂�"
                            '    GCom.GLog.Discription = "�s�\��:" & .TOTAL_FUNO_KEN.ToString & ", �s�\��:" & .TOTAL_FUNO_KIN.ToString & _
                            '        "�ό���:" & .TOTAL_ZUMI_KEN.ToString & "�ϋ��z" & .TOTAL_ZUMI_KIN.ToString
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            'Return False
                            '**************************************************
                        End If
                        nRecordCounter += 1

                    ElseIf cmtFormat.IsEndRecord = True Then
                        ' �Ȃɂ����Ȃ�
                        nRecordCounter += 1
                    End If
                    '**********************************

                    Select Case strRecordKbn
                        Case "ERR"
                            If cmtFormat.ErrorNumber = 1 Then
                                cmtData.setError(9, "CMT�ǎ�G���[")
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMT���R�[�h�敪�G���[ �G���[�ԍ��F" & cmtFormat.ErrorNumber.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                Return False
                            End If
                        Case "H", "D", "T", "E"
                            'Case "H"
                            '    If Not Me.CheckFuriDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE, Me.gstrSysDate, False) Then
                            '        With GCom.GLog
                            '            .Result = "CMT�t�H�[�}�b�g�敪��͏���"
                            '            .Discription = "�U�֓����s���ł�" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    cmtFormat.InfoMeisaiMast.FILE_SEQ += 1
                            '    .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE _
                            '        , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                            '        , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT)
                            '    nRecordCounter += 1
                            'Case "D"
                            '    ' �f�[�^���R�[�h�̒l��ݒ�
                            '    .ItakuData(.ItakuCounter).SetData(cmtFormat.InfoMeisaiMast.FURIKIN)
                            '    nRecordCounter += 1
                            'Case "T"
                            '    If Not (.AddTrailerRecord(cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KIN _
                            '        , cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN _
                            '        , cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN, cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN)) Then
                            '        .setError(9, "�g���[���������z�ƍ��G���[")
                            '        With GCom.GLog
                            '            .Result = "�g���[���������z�ƍ��G���["
                            '            .Discription = "�U�֓����s���ł�" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    If bCheckZumiFlag AndAlso ((cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN > 0)) Then
                            '        .setError(9, "�g���[���������z�ƍ��G���[")
                            '        With cmtFormat.InfoMeisaiMast
                            '            GCom.GLog.Result = "�g���[���ɕs�\���E�ό����܂܂�Ă��܂�"
                            '            GCom.GLog.Discription = "�s�\��:" & .TOTAL_FUNO_KEN.ToString & ", �s�\��:" & .TOTAL_FUNO_KIN.ToString & _
                            '                "�ό���:" & .TOTAL_ZUMI_KEN.ToString & "�ϋ��z" & .TOTAL_ZUMI_KIN.ToString
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    nRecordCounter += 1
                            'Case "E"
                            '    ' �Ȃɂ����Ȃ�
                            '    nRecordCounter += 1
                        Case "IJO"
                            cmtData.setError(9, "CMT�ǎ�G���[")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "CMT�t�@�C����͎��Ɉُ팟�o"
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            Return False
                        Case "99"   '*** ASTAR SUSUKI 2008.06.05 9���R�[�h�������ς��ɑΉ�

                        Case Else
                            ' CMT�t�@�C���̓Ǎ��͏o�������A�t�@�C�����j�����Ă��đS����͏o���Ȃ������ꍇ�̏���
                            .setError(9, "CMT�ǎ�G���[")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "�s���ȃ��R�[�h�敪�����o ���R�[�h�敪�F" & strRecordKbn
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            Return False
                    End Select
                Loop
            End With
        Catch ex As Exception
            cmtData.setError(9, "CMT�ǎ�G���[")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT��͎��s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (nRecordCounter < 4) Then
            cmtData.setError(9, "CMT�ǎ�G���[(���R�[�h�ُ킠��)")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�s���S�ȃt�@�C���ł��B���R�[�h�����s�����Ă��܂��B ���R�[�h���F" & nRecordCounter
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End If

        Return True
    End Function

    '*** �C�� mitsu 2008/09/01 �����������̂��ߍ�蒼�� ***
#Region "��CompareCMTFile"
    'Private Function CompareCMTFile(ByVal nFormatKbn As Integer, ByRef cmtfSrc As CAstFormat.CFormat, ByRef cmtfDst As CAstFormat.CFormat, ByRef cmtData As clsCmtData) As Boolean
    '    Dim strSrcRecordKbn As String               ' ��r���̃��R�[�h�敪
    '    Dim strDstRecordKbn As String               ' ��r��̃��R�[�h�敪
    '    Dim nRecordCounter As Integer = 1           ' ���R�[�h���̃J�E���^
    '    Dim tempsrc, tempdst As String               ' �����񑀍�p�e���|�����ϐ�

    '    Try
    '        With cmtData
    '            Do Until cmtfSrc.EOF = True ' �t�H�[�}�b�g�̉�͊J�n
    '                strSrcRecordKbn = cmtfSrc.CheckDataFormat()
    '                If Not cmtfDst.EOF Then
    '                    strDstRecordKbn = cmtfDst.CheckDataFormat()
    '                Else
    '                    .setError(9, "���R�[�h�ُ킠��")
    '                    With GCom.GLog
    '                        .Result = "���R�[�h��������"
    '                        .Discription = "�t�H�[�}�b�g�敪:" & nFormatKbn.ToString & ", ���R�[�h���J�E���^:" & nRecordCounter.ToString
    '                    End With
    '                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                    Return False
    '                End If

    '                Select Case strSrcRecordKbn
    '                    Case "ERR"
    '                        If cmtfSrc.ErrorNumber = 1 Then
    '                            .setError(9, "CMT�ǎ�G���[")
    '                            With GCom.GLog
    '                                .Result = "CMT�擪�����f�[�^�敪�G���["
    '                                .Discription = "�G���[�ԍ�:" & cmtfSrc.ErrorNumber.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            Return False
    '                        Else
    '                            .setError(9, "CMT�ǎ�G���[")
    '                            With GCom.GLog
    '                                .Result = "CMT�t�H�[�}�b�g�敪��͏���"
    '                                .Discription = "�敪�G���["
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            Return False
    '                        End If
    '                    Case "H" ' �w�b�_���R�[�h
    '                        If nRecordCounter < 6 AndAlso cmtfSrc.RecordData <> cmtfDst.RecordData Then
    '                            With GCom.GLog
    '                                .Result = "CMT�t�H�[�}�b�g�敪��͏���"
    '                                .Discription = "�w�b�_���R�[�h�̓��ꐫ�ᔽ, ���R�[�h�J�E���^:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "����t�@�C���ł͂���܂���")
    '                            Return False
    '                        End If
    '                    Case "D" ' �f�[�^���R�[�h
    '                        If nRecordCounter < 6 Then
    '                            ' ���ʃt���O���J�b�g�Ŗ��߂�
    '                            Select Case nFormatKbn
    '                                Case 0 ' �S��t�H�[�}�b�g
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 111)
    '                                Case 1 ' �n�������c��(350byte)
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 91)
    '                                Case 2 ' �n�������c��(300byte) ����s
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 91)
    '                                Case 3 ' ����
    '                                    ' TODO �d�l���킩��Ȃ��̂Ń`�F�b�N���͂���
    '                                    'CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 111)
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                                Case 4 ' �Z���^�[����(������)
    '                                    ' TODO �Z���^�[����(������)�̃`�F�b�N
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                                Case 5 ' �Z���^�[����(���ʕ�)
    '                                    ' TODO �Z���^�[����(���ʕ�)�̃`�F�b�N
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                            End Select
    '                            ' ���ʃt���O�ȊO�̃f�[�^���R�[�h���r
    '                            If tempsrc <> tempdst Then
    '                                With GCom.GLog
    '                                    .Result = "CMT�t�H�[�}�b�g�敪��͏���"
    '                                    .Discription = "�f�[�^���R�[�h�̓��ꐫ�ᔽ, ���R�[�h�J�E���^:" & nRecordCounter.ToString
    '                                End With
    '                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                                .setError(7, "����t�@�C���ł͂���܂���")
    '                                Return False
    '                            End If
    '                        End If
    '                    Case "T"
    '                        Select Case nFormatKbn
    '                            Case 0 ' �S��t�H�[�}�b�g
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 1 ' �n�������c��(350byte)
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 2 ' �n�������c��(300byte) ����s
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 3 ' ����
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 69, 124)
    '                            Case 4 ' �Z���^�[����(������)
    '                                ' TODO �Z���^�[����(������)�̃`�F�b�N�̎���
    '                                tempsrc = ""
    '                                tempdst = ""
    '                            Case 5 ' �Z���^�[����(���ʕ�)
    '                                ' TODO �Z���^�[����(���ʕ�)�̃`�F�b�N�̎���
    '                                tempsrc = ""
    '                                tempdst = ""
    '                        End Select
    '                        ' �����ρE�s�\���ȊO�̃g���[�����R�[�h���r
    '                        If tempsrc <> tempdst Then
    '                            With GCom.GLog
    '                                .Result = "CMT�t�H�[�}�b�g�敪��͏���"
    '                                .Discription = "�g���[�����R�[�h�̓��ꐫ�ᔽ, ���R�[�h�J�E���^:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "����t�@�C���ł͂���܂���")
    '                            Return False
    '                        End If
    '                    Case "E"
    '                        If nRecordCounter < 6 AndAlso cmtfSrc.RecordData <> cmtfDst.RecordData Then
    '                            With GCom.GLog
    '                                .Result = "CMT�t�H�[�}�b�g�敪��͏���"
    '                                .Discription = "�G���h���R�[�h�̓��ꐫ�ᔽ, ���R�[�h�J�E���^:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "����t�@�C���ł͂���܂���")
    '                            Return False
    '                        End If
    '                    Case "IJO"
    '                        .setError(9, "CMT�ǎ�G���[")
    '                        With GCom.GLog
    '                            .Result = "CMT�t�@�C����͎��Ɉُ팟�o"
    '                            .Discription = "CMT�ǎ掸�s"
    '                        End With
    '                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                        Return False
    '                    Case Else
    '                        .setError(9, "CMT�ǎ�G���[")
    '                        With GCom.GLog
    '                            .Result = "cmtFormat.CheckDataFormat()����z�肳��Ă��Ȃ��l���Ԃ���܂���"
    '                            .Discription = "���R�[�h�敪:(" & strSrcRecordKbn & ")"
    '                        End With
    '                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                        Return False
    '                End Select
    '                nRecordCounter += 1
    '            Loop
    '        End With
    '    Catch ex As Exception
    '        cmtData.setError(9, "CMT�ǎ�G���[")
    '        With GCom.GLog
    '            .Result = "CMT��r���ɗ�O����"
    '            .Discription = ex.Message & ex.StackTrace
    '        End With
    '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '        Return False
    '    End Try
    '    Return True
    'End Function
#End Region

    ' �@�\   �F �S��/�n��1/2/����/�Z���^�[�����t�H�[�}�b�g�̐擪5���R�[�h�̔�r
    '
    ' ����    : ARG1 - �t�H�[�}�b�g�敪
    '         : ARG2 - ��r����CAstFormat.CFormat�̃C���X�^���X
    '         : ARG3 - ��r���CAstFormat.CFormat�̃C���X�^���X
    '         : ARG3 - ��͌��ʂ��i�[���邽�߂�clsCmtReadData�̃C���X�^���X
    '
    ' �߂�l  : ���� true / ���s false
    ' ���l    : �G���[�`�F�b�N�̎����R���ǉ�, �S��ȊO�̃t�H�[�}�b�g�ɑΉ� 2007/12/12
    '         : �w�b�_�A�g���[�����R�[�h�̂݃`�F�b�N���� �K��O�����A�����G���h���R�[�h�Ή� 2008/09/01

    Private Function CompareCMTFile(ByVal nFormatKbn As Integer, ByRef cmtfSrc As CAstFormat.CFormat, ByRef cmtfDst As CAstFormat.CFormat, ByRef cmtData As clsCmtData) As Boolean
        Dim SrcQue As New ArrayList                 ' ��r���̃��R�[�h�L���[
        Dim DstQue As New ArrayList                 ' ��r��̃��R�[�h�L���[
        Dim SrcEntry, DstEntry As DictionaryEntry   ' �L���[�ɒǉ�����G���g���[
        Dim SrcTR(1), DstTR(1) As Decimal           ' �G���g���[�ɒǉ�����g���[�������A���z
        Dim nRecordCounter As Integer = 1           ' ���R�[�h���̃J�E���^

        Try
            If nFormatKbn = 6 Then  'NHK�̏ꍇ�A��Έ�v���Ȃ�
                Return True
            End If
            With cmtData
                '�t�@�C�����J���Ȃ����A�X�g���[��������
                cmtfSrc.FirstRead()
                cmtfDst.FirstRead()

                '��r�����R�[�h�L���[�쐬
                Do Until cmtfSrc.EOF = True
                    SrcEntry.Key = cmtfSrc.CheckDataFormat()

                    '���ɉ�͍ς݂Ȃ̂Ń��R�[�h�`�F�b�N�͂��Ȃ�
                    Select Case SrcEntry.Key.ToString
                        Case "H" ' �w�b�_���R�[�h
                            '�K��O������S�ċ󔒂ɒu�����L���[�ɒǉ�
                            SrcEntry.Value = cmtfSrc.ReplaceString(cmtfSrc.RecordData, -1)
                            SrcQue.Add(SrcEntry)

                        Case "T"
                            '�˗������A���z���L���[�ɒǉ�
                            SrcTR(0) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KEN
                            SrcTR(1) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KIN
                            SrcEntry.Value = SrcTR
                            SrcQue.Add(SrcEntry)
                    End Select
                Loop

                '��r�惌�R�[�h�L���[�쐬
                Do Until cmtfDst.EOF = True
                    DstEntry.Key = cmtfDst.CheckDataFormat()

                    '���ɉ�͍ς݂Ȃ̂Ń��R�[�h�`�F�b�N�͂��Ȃ�
                    Select Case DstEntry.Key.ToString
                        Case "H" ' �w�b�_���R�[�h
                            '�K��O������S�ċ󔒂ɒu�����L���[�ɒǉ�
                            DstEntry.Value = cmtfDst.ReplaceString(cmtfDst.RecordData, -1)
                            DstQue.Add(DstEntry)

                        Case "T"
                            '�˗������A���z���L���[�ɒǉ�
                            DstTR(0) = cmtfDst.InfoMeisaiMast.TOTAL_IRAI_KEN
                            DstTR(1) = cmtfDst.InfoMeisaiMast.TOTAL_IRAI_KIN
                            DstEntry.Value = DstTR
                            DstQue.Add(DstEntry)
                    End Select

                    nRecordCounter += 1
                Loop

                '���R�[�h�����`�F�b�N
                If SrcQue.Count <> DstQue.Count Then
                    .setError(9, "���R�[�h�ُ킠��")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "���R�[�h������ �t�H�[�}�b�g�敪�F" & nFormatKbn.ToString & ", ���R�[�h���F" & nRecordCounter.ToString
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If

                '�w�b�_�E�g���[�����R�[�h�̂ݔ�r����
                For i As Integer = 0 To SrcQue.Count - 1
                    SrcEntry = CType(SrcQue.Item(i), DictionaryEntry)
                    DstEntry = CType(DstQue.Item(i), DictionaryEntry)

                    '���R�[�h�敪�`�F�b�N(�O�̂���)
                    If SrcEntry.Key.ToString <> DstEntry.Key.ToString Then
                        .setError(9, "���R�[�h�ُ킠��")
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "���R�[�h�敪���� �t�H�[�}�b�g�敪�F" & nFormatKbn.ToString & ", ���R�[�h���F" & i.ToString
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        Return False
                    End If

                    Select Case SrcEntry.Key.ToString
                        Case "H" ' �w�b�_���R�[�h
                            If SrcEntry.Value.ToString <> DstEntry.Value.ToString Then
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "�w�b�_���R�[�h�̓��ꐫ�ᔽ, ���R�[�h���F" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "����t�@�C���ł͂���܂���")
                                Return False
                            End If

                        Case "T" ' �g���[�����R�[�h
                            SrcTR = CType(SrcEntry.Value, Decimal())
                            DstTR = CType(DstEntry.Value, Decimal())

                            If SrcTR(0) <> DstTR(0) OrElse _
                               SrcTR(1) <> DstTR(1) Then
                                With GCom.GLog
                                    .Job2 = "CMT�t�@�C����r"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "�g���[�����R�[�h�̓��ꐫ�ᔽ, ���R�[�h���F" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "����t�@�C���ł͂���܂���")
                                Return False
                            End If
                    End Select
                Next
            End With

        Catch ex As Exception
            cmtData.setError(9, "CMT�ǎ�G���[")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT��r���s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        Finally
            '�Ō�Ƀt�@�C�����������
            cmtfSrc.Close()
            cmtfDst.Close()
        End Try
        Return True
    End Function
    ' �@�\   �F �S��/�n��1/2/����/�Z���^�[�����t�H�[�}�b�g�̐擪5���R�[�h�̔�r
    '
    ' ����    : ARG1 - �t�H�[�}�b�g�敪
    '         : ARG2 - ��r����CAstFormat.CFormat�̃C���X�^���X
    '         : ARG3 - ��͌��ʂ��i�[���邽�߂�clsCmtReadData�̃C���X�^���X
    '
    ' �߂�l  : ���� true / ���s false
    ' ���l    : �G���[�`�F�b�N�̎����R���ǉ�, �S��ȊO�̃t�H�[�}�b�g�ɑΉ� 2007/12/12
    '         : �w�b�_�A�g���[�����R�[�h�̂݃`�F�b�N���� �K��O�����A�����G���h���R�[�h�Ή� 2008/09/01

    Private Function ChkCMTFile(ByVal nFormatKbn As Integer, ByVal cmtfSrc As CAstFormat.CFormat, ByVal cmtData As clsCmtData) As Boolean
        Dim SrcQue As New ArrayList                 ' ��r���̃��R�[�h�L���[
        Dim SrcEntry As DictionaryEntry             ' �L���[�ɒǉ�����G���g���[
        Dim SrcTR(5) As Decimal                     ' �G���g���[�ɒǉ�����g���[�������A���z
        Dim nRecordCounter As Integer = 1           ' ���R�[�h���̃J�E���^

        Try
            With cmtData
                '�t�@�C�����J���Ȃ����A�X�g���[��������
                cmtfSrc.FirstRead()

                '��r�����R�[�h�L���[�쐬
                Do Until cmtfSrc.EOF = True
                    SrcEntry.Key = cmtfSrc.CheckDataFormat()

                    '���ɉ�͍ς݂Ȃ̂Ń��R�[�h�`�F�b�N�͂��Ȃ�
                    Select Case SrcEntry.Key.ToString
                        Case "H" ' �w�b�_���R�[�h
                            '�K��O������S�ċ󔒂ɒu�����L���[�ɒǉ�
                            SrcEntry.Value = cmtfSrc.ReplaceString(cmtfSrc.RecordData, -1)
                            SrcQue.Add(SrcEntry)

                        Case "T"
                            '�˗������A���z���L���[�ɒǉ�
                            SrcTR(0) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KEN
                            SrcTR(1) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KIN
                            SrcTR(2) = cmtfSrc.InfoMeisaiMast.TOTAL_ZUMI_KEN
                            SrcTR(3) = cmtfSrc.InfoMeisaiMast.TOTAL_ZUMI_KIN
                            SrcTR(4) = cmtfSrc.InfoMeisaiMast.TOTAL_FUNO_KEN
                            SrcTR(5) = cmtfSrc.InfoMeisaiMast.TOTAL_FUNO_KIN


                            SrcEntry.Value = SrcTR
                            SrcQue.Add(SrcEntry)
                    End Select
                Loop


                '�w�b�_�E�g���[�����R�[�h�̂ݔ�r����
                For i As Integer = 0 To SrcQue.Count - 1
                    SrcEntry = CType(SrcQue.Item(i), DictionaryEntry)

                    Select Case SrcEntry.Key.ToString
                        Case "T" ' �g���[�����R�[�h
                            SrcTR = CType(SrcEntry.Value, Decimal())

                            If SrcTR(0) <> SrcTR(2) + SrcTR(4) OrElse _
                               SrcTR(1) <> SrcTR(3) + SrcTR(5) Then
                                With GCom.GLog
                                    .Job2 = "CMT�t�@�C����r"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "�g���[�����R�[�h�̓��ꐫ�ᔽ, ���R�[�h���F" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "����t�@�C���ł͂���܂���")
                                Return False
                            End If
                    End Select
                Next
            End With

        Catch ex As Exception
            cmtData.setError(9, "CMT�ǎ�G���[")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT��r���s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        Finally
            '�Ō�Ƀt�@�C�����������
            cmtfSrc.Close()
        End Try
        Return True
    End Function
    '******************************************************

    ' �@�\   �F �T�[�o�̃p�X����Ԃ�
    '
    ' ����    : ARG1 - Read(true)/Write(false)
    '         : ARG2 - ���U(True)/���U(False)�t���O
    '         : ARG3 - �t�H�[�}�b�g�敪(Integer)
    ' �߂�l  : �t�@�C�����ł͂Ȃ��Ď��ۂɂ̓p�X����Ԃ�
    Private Function MakeServerFileName(ByVal bRWFlag As Boolean, ByVal bJSFlag As Boolean, ByVal fkbn As Integer) As String
        Dim filename As String

        If bRWFlag Then
            filename = gstrCMTServerRead
        Else
            filename = gstrCMTServerWrite
        End If
        'If bJSFlag Then
        '    filename &= "JIFURI\"
        'Select Case fkbn
        '    Case 0 ' �S��t�H�[�}�b�g
        '        filename &= "JFR120\"
        '    Case 1 ' �n�������c��350byte
        '        filename &= "JFR350\"
        '    Case 2 ' �n�������c��300byte
        '        filename &= "JFR300\"
        '    Case 3 ' ����
        '        filename &= "JFR390\"
        '    Case 4 ' �Z���^�[���ڎ���(�����f�[�^)
        '        filename &= "JFR640\"
        '    Case 5 ' �Z���^�[���ڎ���(���ʃf�[�^)
        '        filename &= "JFR165\"
        '    Case Else
        '        With GCom.GLog
        '            .Result = MenteCommon.clsCommon.NG
        '            .Discription = "���Ή��̃t�@�C���敪�ԍ� ���U���U�敪�F" & bJSFlag.ToString & ", CMT�t�H�[�}�b�g�敪�F" & fkbn.ToString
        '        End With
        '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '        Return "ERR"
        'End Select
        'Else
        'filename &= "SOUKYUFURI\"
        'Select Case fkbn
        '    Case 0
        '        filename &= "SFR120\"
        '    Case 1
        '        filename &= "SFR350\"
        '    Case 2
        '        filename &= "SFR300\"
        '    Case 3
        '        filename &= "SFR390\"
        '    Case Else
        '        With GCom.GLog
        '            .Result = MenteCommon.clsCommon.NG
        '            .Discription = "���Ή��̃t�@�C���敪�ԍ� ���U���U�敪�F" & bJSFlag.ToString & ", CMT�t�H�[�}�b�g�敪�F" & fkbn.ToString
        '        End With
        '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '        Return "ERR"
        'End Select
        'End If

        Return filename
    End Function


    ' �@�\   �F ���s�����̃T�[�o�̃p�X����Ԃ�
    '
    ' ����    : ARG1 �������� = true / ���ʓǎ� = false
    '
    ' �߂�l  : �t�@�C�����ł͂Ȃ��Ď��ۂɂ̓p�X����Ԃ�
    Private Function MakeOtherFileName(ByRef bRWflag As Boolean) As String
        Dim filename As String
        '*** �C�� mitsu 2008/09/01 �s�v ***
        'Try
        '**********************************
        If bRWflag Then
            filename = gstrCMTServerWrite
        Else
            filename = gstrCMTServerRead
        End If

        ' �S��̂�
        filename &= "TAKOU\"

        Return filename
    End Function


    ' �@�\�@ �F ��t�ԍ��̎擾
    '
    ' �����@ �F ARG1 - true �� CMT_READ_TBL����Ǎ�
    '                  false �� CMT_WRITE_TBL����Ǎ�
    '           ARG2   OracleDataReader(�Q�Ɠn)
    '           ARG3   ��t�ԍ�(�Q�Ɠn) 
    ' �߂�l �F ����I�� = True
    ' �@�@�@ �@ �ُ�I�� = False
    '
    ' ���l�@ �F 2007.11.14 �ǉ�
    Private Function GetReceptionNo(ByVal bRWFlag As Boolean, ByRef nReceptionNo As Integer) As Boolean
        Dim strSQL As String
        Dim strTbl As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim oraReader As OracleDataReader = Nothing
        'Dim oraReader As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        If bRWFlag Then
            strTbl = "CMT_READ_TBL"
        Else
            strTbl = "CMT_WRITE_TBL"
        End If

        ' ��t�ԍ�(nReceptionNo)���擾
        strSQL = "SELECT NVL(MAX(RECEPTION_NO),0)"
        strSQL &= " FROM " & strTbl
        strSQL &= " WHERE SYORI_DATE = '" & gstrSysDate & "'"
        strSQL &= " AND STATION_NO = '" & Me.gstrStationNo & "'"

        If GCom.SetDynaset(strSQL, oraReader) Then
            ' ��t�ԍ��́A�e�[�u�����̍ő�l�{�P����X�^�[�g
            oraReader.Read()
            nReceptionNo = oraReader.GetInt32(0) + 1
            Return True
        Else
            '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
            'With GCom.GLog
            '    .Result = strTbl & "�ŃG���[����"
            '    .Discription = "��t�ԍ��̎擾���s"
            'End With
            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            MessageBox.Show("��t�ԍ��̎擾�Ɏ��s���܂���" & vbCrLf & "�e�[�u�����F" & strTbl, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If
    End Function

    ' �@�@�\ : �t�@�C���������l�[��
    '
    ' ������ : ARG1 - �ύX���t�@�C����
    ' �@�@�@   ARG2 - �ύX��t�@�C����
    '
    ' �߂�l : True = ���� / False = ���s
    '
    ' ���l   : ���s�����ꍇ�Ɋe�탍�O���o�͂���
    Private Function RenameFileName(ByVal strSource As String, ByVal strDistination As String) As Boolean
        If Not (File.Exists(strSource)) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ύX���̃t�@�C���������݂��Ȃ����߁A���l�[�����s ���t�@�C�����F" & strSource & ", ��t�@�C�����F" & strDistination
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        If File.Exists(strDistination) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ύX��̃t�@�C���������ɑ��݂��Ă��邽�߁A���l�[�����s ���t�@�C�����F" & strSource & ", ��t�@�C�����F" & strDistination
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        End If
        Try
            FileSystem.Rename(strSource, strDistination)
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���l�[�����s ���t�@�C�����F" & strSource & ", ��t�@�C�����F" & strDistination & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' �@�@�\ : �t�@�C�����폜
    '
    ' ������ : ARG1 - �폜�t�@�C����
    '
    ' �߂�l : True = ���� / False = ���s
    '
    Private Function DeleteFile(ByVal filename As String) As Boolean
        Const strResult As String = "�t�@�C���폜���s"
        Try
            File.Delete(filename)
        Catch ex As UnauthorizedAccessException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �w�肳�ꂽ�p�X�̓f�B���N�g���ł��邩�A�A�N�Z�X����������܂���ł��B�p�X���F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As ArgumentException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �t�@�C�������󔒂ł��B�t�@�C�����F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As System.Security.SecurityException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �폜�����s���錠��������܂���B�t�@�C�����F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As DirectoryNotFoundException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �w�肵���f�B���N�g�����݂���܂���B �f�B���N�g�����F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As IOException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �w�肵���t�@�C�����g�p���ł��B �t�@�C�����F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " �t�@�C���폜���ɕs���ȃG���[���������܂����B �t�@�C�����F" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function

    ' �@�@�\ : �o�C�i���Ńt�@�C�����R�s�[
    '
    ' ������ : ARG1 - �Ǎ����t�@�C����
    ' �@�@�@   ARG2 - ������t�@�C����
    '          ARG3 - ���R�[�h��
    '
    ' �߂�l : 0:����, 1:�t�@�C���Ȃ�, 2:�����ݐ悪���ɑ���, 3:�t�@�C�������s��
    '
    ' ���l   : �t�@�C���T�C�Y��2G���ƂȂ�ꍇ�������ł���悤��Long�^�ŏ������Ă���
    Private Function BinaryCopy(ByVal ReadFileName As String, ByVal WriteFileName As String, ByVal nRecordLen As Integer) As Integer
        Dim bytesTemp(nRecordLen) As Byte
        Dim i As Long
        Dim len As Long
        Dim divr As Long
        Dim imax As Long

        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim rfs As FileStream = Nothing
        Dim wfs As FileStream = Nothing
        Dim rs As BinaryReader = Nothing
        Dim ws As BinaryWriter = Nothing
        'Dim rfs As FileStream
        'Dim wfs As FileStream
        'Dim rs As BinaryReader
        'Dim ws As BinaryWriter
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        Try
            Try
                rfs = New FileStream(ReadFileName, FileMode.Open)
                rs = New BinaryReader(rfs)
            Catch rext As FileNotFoundException
                ' �t�@�C���������݂��܂���
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�R�s�[���t�@�C���Ȃ� �t�@�C�����F" & ReadFileName & " " & rext.Message
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return 1
            End Try

            Try
                wfs = New FileStream(WriteFileName, FileMode.CreateNew)
                ws = New BinaryWriter(wfs)
            Catch wex As IOException
                ' �t�@�C���������ɑ��݂��Ă��܂�
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�����ݐ�̃t�@�C�������ɑ��݂��Ă��܂� �t�@�C�����F" & WriteFileName & " " & wex.Message
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                rs.Close()
                rfs.Close()
                Return 2
            End Try

            len = rfs.Length()
            imax = Math.DivRem(len, nRecordLen, divr)

            ' �w�肵�����R�[�h���̐����{�̒���������
            For i = 1 To imax
                bytesTemp = rs.ReadBytes(nRecordLen)    ' �w�肵�����R�[�h�����Ǎ�
                ws.Write(bytesTemp, 0, nRecordLen)      ' �w�肵�����R�[�h��������
            Next i

            If (divr > 0) Then
                ' �]�蕪��������
                bytesTemp = rs.ReadBytes(CInt(divr))
                ws.Write(bytesTemp, 0, CInt(divr))
                rs.Close()
                ws.Close()
                rfs.Close()
                wfs.Close()
                Return 0 ' CR�̗L�������邽�߁A�]��͖���
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�o�C�i���t�@�C���̃R�s�[���s " & ReadFileName & "����" & WriteFileName & "�ւ̃R�s�[ " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            rs.Close()
            ws.Close()
            rfs.Close()
            wfs.Close()
            Return 3
        End Try

        rs.Close()
        ws.Close()
        rfs.Close()
        wfs.Close()

        Return 0
    End Function


    ' �@�\�@ �F �@�Ԑݒ�
    '
    ' �����@ �F ARG1 - �Ȃ�
    '
    ' �߂�l �F ����I�� = True
    ' �@�@�@ �@ �ُ�I�� = False
    '
    ' ���l�@ �F 2007.11.16 Insert By Astar
    '
    Public Function SetStationNo() As Boolean

        gstrStationNo = GCom.GetStationNo().Substring(1, 1)

        If (gstrStationNo = "NULL") Then
            With GCom.GLog
                .Job2 = "�@�Ԏ擾"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ""
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Else
            Return True
        End If

    End Function


    ' �@�\�@ �F CMT.INI����ǎ�ǎ挋�ʂ̎擾
    '           �ǂݎ�茳: CMT.INI
    '
    ' �����@ �F ARG1 - �X�^�b�J�ԍ�
    '
    ' �߂�l �F �X�e�[�^�X(Integer)
    '
    ' ���l�@ �F 2007.11.21 �쐬
    Protected Function GetCmtReadResult(ByVal nStackerNo As Integer) As Integer
        Dim str As String = ""
        If (nStackerNo < 1) Or (nStackerNo > 10) Then

            With GCom.GLog
                .Job2 = "CMT.INI�Ǎ�"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�s���ȃX�^�b�J�ԍ� �ԍ��F" & nStackerNo.ToString
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return 4
        End If

        str = CMT.GetCMTIni("READ-RESULT", nStackerNo.ToString)
        If str = "err" Then
            Return 4
        Else
            Return Convert.ToInt32(str)
        End If
    End Function


    ' �@�\�@ �F CMT.INI���烉�x���L���̎擾
    '           �ǂݎ�茳: CMT.INI
    '
    ' �����@ �F ARG1 - �X�^�b�J�ԍ�
    '
    ' �߂�l �F �X�e�[�^�X(Integer)
    '
    ' ���l�@ �F 2007.11.21 �쐬
    Protected Function GetCmtLabelExist(ByVal nStackerNo As Integer) As Boolean
        Dim str As String = ""
        '        Dim cmt As CMT.ClsCMTCTRL

        If (nStackerNo < 0) Or (nStackerNo > 9) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT.INI�Ǎ� �s���ȃX�^�b�J�ԍ����w�肳��܂��� �ԍ��F" & nStackerNo.ToString
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        Try
            If CMT.GetCMTIni("LABEL-EXIST", (nStackerNo + 1).ToString) = "1" Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT.INI�Ǎ�"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT.INI�Ǎ� ���ړǎ�G���[ [LABEL-EXIST]�A�X�^�b�J�ԍ��F" & nStackerNo.ToString & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
    End Function


    ' �@�@�\ : ���[�J��PC�̃t�@�C���̍폜
    '
    ' �߂�l : ���� true / ���s false
    '
    ' ������ : ARG1 - �Ǎ��t�H���_�폜 = True / �����t�H���_�폜 = False
    Protected Function LocalFileDelete(ByVal bRWflag As Boolean) As Boolean
        Dim nCnt As Integer
        Dim path As String
        Dim head As String
        Dim endlabel As String
        Dim filename As String

        Try
            If bRWflag Then
                filename = CMT.GetCMTIni("FILE-NAME", "READ")
            Else
                filename = CMT.GetCMTIni("FILE-NAME", "WRITE")
            End If
            head = CMT.GetCMTIni("FILE-NAME", "HEAD")
            endlabel = CMT.GetCMTIni("FILE-NAME", "END")
            For nCnt = 1 To 10 Step 1
                If (bRWflag) Then
                    path = CMT.GetCMTIni("READ-DIRECTORY", nCnt.ToString)
                Else
                    path = CMT.GetCMTIni("WRITE-DIRECTORY", nCnt.ToString)
                End If

                ' �t�@�C���폜
                If File.Exists(path & "\" & filename) = True Then
                    File.Delete(path & "\" & filename)
                End If
                ' �w�b�_���x���폜
                If File.Exists(path & "\" & head) = True Then
                    File.Delete(path & "\" & head)
                End If
                ' �G���h���x���폜
                If File.Exists(path & "\" & endlabel) = True Then
                    File.Delete(path & "\" & endlabel)
                End If
            Next nCnt
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���[�J���t�@�C���폜���s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    '***ASTAR SUSUKI 2008.06.12                     ***
    '***�ǉ�
    ' �@�@�\ : ���[�J��PC�̃w�b�_�t�@�C�����R�s�[ Read ���� Write ��
    '
    ' �߂�l : ���� true / ���s false
    '
    ' ������ : ARG1 - �Ǎ��t�H���_�폜 = True / �����t�H���_�폜 = False
    Protected Function LocalHeadCopy(ByVal lot As Integer) As Boolean
        Dim nCnt As Integer
        Dim ReadPath As String
        Dim WritePath As String
        Dim head As String
        Dim endlabel As String

        Try
            head = CMT.GetCMTIni("FILE-NAME", "HEAD")
            endlabel = CMT.GetCMTIni("FILE-NAME", "END")
            nCnt = lot
            ReadPath = CMT.GetCMTIni("READ-DIRECTORY", nCnt.ToString)
            WritePath = CMT.GetCMTIni("WRITE-DIRECTORY", nCnt.ToString)

            ' �w�b�_���x���R�s�[
            If File.Exists(ReadPath & "\" & head) = True Then
                If File.Exists(WritePath & "\" & head) = False Then
                    File.Copy(ReadPath & "\" & head, WritePath & "\" & head)
                End If
            End If
            ' �G���h���x���R�s�[
            If File.Exists(ReadPath & "\" & endlabel) = True Then
                If File.Exists(WritePath & "\" & endlabel) = False Then
                    File.Copy(ReadPath & "\" & endlabel, WritePath & "\" & endlabel)
                End If
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�w�b�_�t�@�C���R�s�[���s �X�^�b�J�ԍ��F" & lot & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function
    '***

    ' �@�@�\ : ���уe�[�u������ListView����
    '
    ' ��  �� : ARG1 - �Ɖ�ʂ��i�[����e�[�u��(CMT_READ_TBL = True / CMT_WRITE_TBL = False)
    '          ARG2 - �e�[�u���I���t���O �Ǎ�����=True/��������=False
    '          ARG3 - ���U=True/�����U=False �t���O
    '          ARG4 - �U�֓�
    '          ARG5 - �ϑ��҃R�[�h
    '
    ' �߂�l : ���� true / ���s false
    '
    Public Function getRWList(ByRef listview As ListView, ByVal bRWFlg As Boolean, ByVal bJSFlg As Boolean, ByVal strFuriDate As String _
         , ByVal strItakuCD As String, ByVal bJifuri As Boolean, ByVal bsoukyufuri As Boolean) As Boolean
        Dim count As Integer
        Dim SQL As String
        Dim strTableName As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        If bRWFlg Then
            strTableName = "CMT_READ_TBL"
        Else
            strTableName = "CMT_WRITE_TBL"
        End If

        Try
            ' �s���J�E���g
            SQL = "SELECT COUNT(*)"
            SQL &= " FROM " & strTableName ' �e�[�u����
            If strFuriDate <> "" Then
                '*** 2008.06.10�C�� nishida �����p�^�[���ύX�@�U�֓�->������ START***
                SQL &= " WHERE SYORI_DATE = '" & strFuriDate & "'"
                'SQL &= " WHERE FURI_DATE = '" & strFuriDate & "'"
                '*** 2008.06.10�C�� nishida �����p�^�[���ύX�@�U�֓�->������ END***
                If strItakuCD <> "" Then
                    SQL &= " AND ITAKU_CODE = '" & strItakuCD & "'"
                End If
            Else
                If strItakuCD <> "" Then
                    SQL &= " WHERE ITAKU_CODE = '" & strItakuCD & "'"
                End If
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                onReader.Read()
                count = onReader.GetInt32(0)
            End If

            If count = 0 Then
                ' �\�����ږ���
                Return True
            End If

            ' SQL������
            SQL = "SELECT "
            SQL &= "   NVL(ITAKU_CODE, '0000000000')"           ' 00.�ϑ��҃R�[�h
            SQL &= " , NVL(FURI_DATE,'00000000')"               ' 01.�U�֓�
            SQL &= " , NVL(ERR_CD, 0)           "               ' 02.�G���[�R�[�h
            SQL &= " , NVL(ERR_INFO,' -- ')     "               ' 03.�G���[��
            SQL &= " , RECEPTION_NO"                            ' 04.��tNo
            SQL &= " , FILE_SEQ"                                ' 05.FILE_SEQ
            SQL &= " , NVL(SYORI_DATE, ' -- ')"                 ' 06.CMT������
            SQL &= " , NVL(STACKER_NO, 0)"                      ' 07.�X�^�b�J�ԍ�
            SQL &= " , NVL(STATION_NO, '-')"                    ' 08.CMT�Ǎ��@��
            SQL &= " , NVL(SYORI_KEN, 0)"                       ' 09.����
            SQL &= " , NVL(SYORI_KIN, 0)"                       ' 10.���z
            SQL &= " , NVL(FURI_KEN, 0)"                        ' 11.�U�֍ό���
            SQL &= " , NVL(FURI_KIN, 0)"                        ' 12.�U�֍ϋ��z
            SQL &= " , NVL(FUNOU_KEN, 0)"                       ' 13.�s�\����
            SQL &= " , NVL(FUNOU_KIN, 0)"                       ' 14.�s�\���z
            SQL &= " , NVL(FILE_NAME, ' -- ')"                  ' 15.�t�@�C����
            If Not bRWFlg Then
                SQL &= " , NVL(WRITE_COUNTER, 0)"               ' 16a.�t�@�C��������
            Else
                SQL &= " , NVL(JS_FLG,'-')"                     ' 16b.���U/���U�t���O
            End If
            SQL &= " , NVL(COMPLOCK_FLG,'-')"                   ' 17.�Í����t���O
            SQL &= " , CREATE_DATE"                             ' 18.������
            SQL &= " , UPDATE_DATE"                             ' 19.�X�V��
            If Not bRWFlg Then
                SQL &= " , OVERRIDE_FLG"                        ' 20.���������t���O
            End If

            SQL &= " FROM " & strTableName                      ' �e�[�u����

            If strFuriDate <> "" Then
                '*** 2008.06.10�C�� nishida �����p�^�[���ύX�@�U�֓�->������ START***
                SQL &= " WHERE SYORI_DATE = '" & strFuriDate & "'"
                'SQL &= " WHERE FURI_DATE = '" & strFuriDate & "'"
                '*** 2008.06.10�C�� nishida �����p�^�[���ύX�@�U�֓�->������ END***
                If strItakuCD <> "" Then
                    SQL &= " AND ITAKU_CODE = '" & strItakuCD & "'"
                End If
                If Not bJifuri Then
                    SQL &= " AND JS_FLG = '0'"
                Else
                    If Not bsoukyufuri Then
                        SQL &= " AND JS_FLG = '1'"
                    End If
                End If
            Else
                If strItakuCD <> "" Then
                    SQL &= " WHERE ITAKU_CODE = '" & strItakuCD & "'"
                    If Not bJifuri Then
                        SQL &= " AND JS_FLG = '0'"
                    Else
                        If Not bsoukyufuri Then
                            SQL &= " AND JS_FLG = '1'"
                        End If
                    End If
                Else
                    If Not bJifuri Then
                        SQL &= " WHERE JS_FLG = '0'"
                    Else
                        If Not bsoukyufuri Then
                            SQL &= " WHERE JS_FLG = '1'"
                        End If
                    End If
                End If
            End If
            SQL &= " ORDER BY RECEPTION_NO"

            If GCom.SetDynaset(SQL, onReader) Then
                For i As Integer = 0 To count - 1
                    onReader.Read()
                    Ora2List(onReader, listview, bRWFlg)
                Next i
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���уe�[�u������ListView�������s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
    End Function

    ' �@�@�\ : �G���[�̂���e�[�u���̃��X�g��Ԃ�
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �Ώ�ListView
    ' ���@�l : 
    '    
    Public Function getErrorList(ByRef listview As ListView) As Boolean
        Dim count As Integer
        Dim SQL As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        Try
            ' �s���J�E���g
            SQL = "SELECT COUNT(*) "
            SQL &= " FROM"
            SQL &= " ( SELECT " _
                 & "   ITAKU_CODE" _
                 & ", FURI_DATE" _
                 & ", MIN(ERR_CD) ERROR" _
                 & " FROM CMT_READ_TBL" _
                 & " GROUP BY ITAKU_CODE, FURI_DATE)"
            SQL &= " WHERE ERROR > 0"

            If GCom.SetDynaset(SQL, onReader) Then
                onReader.Read()
                count = onReader.GetInt32(0)
            End If

            If count = 0 Then
                ' �\�����ږ���
                Return True
            End If

            ' �e�[�u���̓��e���R�s�[
            SQL = "SELECT * "
            SQL &= "FROM ("
            SQL &= " SELECT"
            SQL &= "   NVL(ITAKU_CODE,'0000000000')" ' 0.�ϑ��҃R�[�h 
            SQL &= ",  NVL(FURI_DATE,'00000000')"    ' 1.�U�֓�
            SQL &= ",  MIN(ERR_CD) ERROR"    ' 2.�G���[�R�[�h
            SQL &= " , NVL(ERR_INFO, ' -- ')" '3.�G���[��
            SQL &= " , RECEPTION_NO"         ' 4.��tNo
            SQL &= " , FILE_SEQ"             ' 5.FILE_SEQ
            SQL &= " , SYORI_DATE"           ' 6.CMT������
            SQL &= " , NVL(STACKER_NO, 0)"   ' 7.�X�^�b�J�ԍ�
            SQL &= " , NVL(STATION_NO, '-')" ' 8.CMT�Ǎ��@��
            SQL &= " , NVL(SYORI_KEN, 0)"    ' 9.����
            SQL &= " , NVL(SYORI_KIN, 0)"    ' 10.���z
            SQL &= " , NVL(FURI_KEN, 0)"     ' 11.�U�֍ό���
            SQL &= " , NVL(FURI_KIN, 0)"     ' 12.�U�֍ϋ��z
            SQL &= " , NVL(FUNOU_KEN, 0)"    ' 13.�s�\����
            SQL &= " , NVL(FUNOU_KIN, 0)"    ' 14.�s�\���z
            SQL &= " , NVL(FILE_NAME, ' -- ')" ' 15.�t�@�C����
            SQL &= " , NVL(JS_FLG, '-')"     ' 16.���U/���U�t���O
            SQL &= " , NVL(COMPLOCK_FLG, '-')" ' 17.�Í����t���O
            SQL &= " , CREATE_DATE"          ' 18.������
            SQL &= " , UPDATE_DATE"          ' 19.�X�V��
            SQL &= " FROM CMT_READ_TBL"
            SQL &= " GROUP BY"
            SQL &= "  ITAKU_CODE"            ' 0.�ϑ��҃R�[�h 
            SQL &= ",  FURI_DATE"            ' 1.�U�֓�
            ' 2.�G���[�R�[�h
            SQL &= " , RECEPTION_NO"         ' 4.��tNo
            SQL &= " , FILE_SEQ"             ' 5.FILE_SEQ
            SQL &= " , SYORI_DATE"           ' 6.CMT������
            SQL &= " , STACKER_NO"           ' 7.�X�^�b�J�ԍ�
            SQL &= " , STATION_NO"           ' 8.CMT�Ǎ��@��
            SQL &= " , SYORI_KEN"            ' 9.����
            SQL &= " , SYORI_KIN"            ' 10.���z
            SQL &= " , FURI_KEN"             ' 11.�U�֍ό���
            SQL &= " , FURI_KIN"             ' 12.�U�֍ϋ��z
            SQL &= " , FUNOU_KEN"            ' 13.�s�\����
            SQL &= " , FUNOU_KIN"            ' 14.�s�\���z
            SQL &= " , FILE_NAME"            ' 15.�t�@�C����
            SQL &= " , ERR_INFO"             ' 3.�G���[��
            SQL &= " , JS_FLG"               ' 16.���U/���U�t���O
            SQL &= " , COMPLOCK_FLG"         ' 17.�Í����t���O
            SQL &= " , CREATE_DATE"          ' 18.������
            SQL &= " , UPDATE_DATE)"         ' 19.�X�V��
            SQL &= " WHERE ERROR > 0"        ' �G���[�R�[�h��0�ȊO�ł���Ƃ�
            If GCom.SetDynaset(SQL, onReader) Then
                
                For i As Integer = 0 To count - 1
                    onReader.Read()
                    Ora2List(onReader, listview, True)
                Next i
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListView�������s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function

    ' �@�@�\ : OracleDataReader����ListView�𐶐�
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - OracleReader
    '        : ARG2 - �Ώ�ListView
    '        : ARG3 - RW�t���O (True = CMT�Ǎ�����TBL / False = CMT��������TBL)
    ' ���@�l : 
    '    
    Public Function Ora2List(ByRef onReader As OracleDataReader, ByRef listview As ListView, ByVal bRWFlg As Boolean) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String ' F�敪, ������R�[�h, ����敛�R�[�h�i�[�p�ϐ�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        DB.GetFToriCode(onReader.GetString(0), "", toris, torif, fkbn, 0)

        Dim lv As New ListViewItem(onReader.GetInt32(4).ToString.PadLeft(5, "0"c))  ' 0.��tNo
        If (onReader.GetInt32(2) > 0) Then
            lv.BackColor = Color.Pink
        Else
            lv.BackColor = Color.White
        End If
        lv.SubItems.Add(onReader.GetInt32(5).ToString.PadLeft(2, "0"c))             ' 1.FILE_SEQ
        lv.SubItems.Add(onReader.GetString(6))                      ' 2.������
        lv.SubItems.Add(onReader.GetInt32(7).ToString.PadLeft(2, "0"c))            ' 3.�X�^�b�J�ԍ�
        If Not bRWFlg Then
            lv.SubItems.Add(onReader.GetInt32(16).ToString)         ' 4.�t�@�C��������
        Else
            lv.SubItems.Add("--")                                   ' 4.�t�@�C��������
        End If
        lv.SubItems.Add(onReader.GetString(0))                      ' 5.�ϑ��҃R�[�h(FILE_SEQ��)
        lv.SubItems.Add(onReader.GetString(1))                      ' 6.�U�֓�(FILE_SEQ��)
        lv.SubItems.Add(DB.GetItakuKana(onReader.GetString(0)))     ' 7.�ϑ��҃J�i
        lv.SubItems.Add(DB.GetItakuKanji(onReader.GetString(0)))    ' 8.�ϑ��Ҋ���
        lv.SubItems.Add(onReader.GetInt32(9).ToString)              ' 9.����
        lv.SubItems.Add(onReader.GetInt32(10).ToString)             ' 10.���z
        lv.SubItems.Add(" -- ")                                     ' 11.TR����
        lv.SubItems.Add(" -- ")                                     ' 12.TR���z
        lv.SubItems.Add(onReader.GetInt32(2).ToString)              ' 13.�G���[�R�[�h
        lv.SubItems.Add(onReader.GetString(3))                      ' 14.�G���[��
        lv.SubItems.Add(" -- ")                                     ' 15.���Z�@�փR�[�h
        lv.SubItems.Add(" -- ")                                     ' 16.���Z�@�֖�
        lv.SubItems.Add(" -- ")                                     ' 17.�x�X�R�[�h
        lv.SubItems.Add(" -- ")                                     ' 18.�x�X��
        If bRWFlg Then
            If (onReader.GetString(16) = "1") Then                  ' 19.���U/���U�t���O
                lv.SubItems.Add("��")
            Else
                lv.SubItems.Add("��")
            End If
        Else
            lv.SubItems.Add("��")
        End If
        lv.SubItems.Add(onReader.GetInt32(11).ToString)             ' 20.�U�֍ό���
        lv.SubItems.Add(onReader.GetInt32(12).ToString)             ' 21.�U�֍ϋ��z
        lv.SubItems.Add(onReader.GetInt32(13).ToString)             ' 22.�s�\����
        lv.SubItems.Add(onReader.GetInt32(14).ToString)             ' 23.�s�\���z
        lv.SubItems.Add(fkbn)                                       ' 24.F�����敪
        lv.SubItems.Add(toris)                                      ' 25.������R�[�h
        lv.SubItems.Add(torif)                                      ' 26.����敛�R�[�h
        lv.SubItems.Add(onReader.GetString(8))                      ' 27.CMT�ǎ�@��
        lv.SubItems.Add(onReader.GetString(15))                     ' 28.�t�@�C����
        If Not bRWFlg Then
            If (onReader.GetString(20) = "1") Then                  ' 29.���������t���O�t���O
                lv.SubItems.Add("����")                             '  - ����
            Else
                lv.SubItems.Add("�ʏ�")                              ' - �ʏ�
            End If
        Else
            lv.SubItems.Add(" - ")                                  '�@�Ǎ��Ȃ̂Ŋ֌W�Ȃ�
        End If
        If onReader.GetString(17) = "1" Then                        ' 30.�Í����L��
            lv.SubItems.Add("C")                                    '  - �Í�������
        Else
            lv.SubItems.Add("N")                                    '  - �Í����Ȃ�
        End If
        lv.SubItems.Add(onReader.GetOracleDateTime(18).ToString)    ' 31.������
        lv.SubItems.Add(onReader.GetOracleDateTime(19).ToString)    ' 32.�X�V��

        listview.Items.AddRange(New ListViewItem() {lv})
    End Function

    ' �@�@�\ : �T�[�o�Ɏc������ԋp�t�@�C���̈ꗗ��Ԃ�
    '          
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �Ώ�ListView
    ' ���@�l : 
    '
    Public Function getServerExistFiles(ByRef listview As ListView) As Boolean
        Dim serverpath As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim cmtd As clsCmtData
        Dim compflag As Boolean
        Dim itakucd As String

        GCom.GLog.Job2 = "CMT����������������"

        Try
            For nFormatKbn As Integer = 0 To 4 Step 1
                serverpath = MakeServerFileName(False, True, nFormatKbn)
                GetFormat(nFormatKbn, cmtFormat)

                If Not Directory.Exists(serverpath) Then ' �t�H���_�����݂��邩�ǂ�������
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "�T�[�o�ԋp�t�H���_���݂���܂��� �t�H���_���F" & serverpath
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If

                ' �t�H���_�����݂���ꍇ
                Dim di As New DirectoryInfo(serverpath) ' �T�[�o�p�X�̃f�B���N�g�����𐶐�
                Dim fi As FileInfo() = di.GetFiles()    ' �t�@�C�����̔z��𐶐�

                Dim fiTemp As FileInfo
                Dim i As Integer = 0

                Dim wcounter As Integer = 0
                For Each fiTemp In fi
                    ' �ԋp�t�@�C�����p�[�X����ListView�𐶐�

                    ' �t�@�C�����̃`�F�b�N
                    If fiTemp.Name.Length = 23 AndAlso fiTemp.Name.Substring(19, 4) = ".dat" Then
                        itakucd = fiTemp.Name.Substring(1, 10)
                        Dim furidate As String = fiTemp.Name.Substring(11, 8)
                        If (fiTemp.Name.Substring(0, 1) = "C") Then
                            compflag = True
                        Else
                            compflag = False
                        End If

                        If (GCom.NzLong(itakucd, 10) >= 0L) And _
                            (GCom.NzLong(itakucd, 10) <= 9999999999L) And _
                            (GCom.NzLong(furidate, 8) >= 19000000L) And _
                            (GCom.NzLong(furidate, 8) <= 99999999L) Then

                            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
                            Dim fkbn As String = String.Empty
                            Dim toris As String = String.Empty
                            Dim torif As String = String.Empty
                            'Dim fkbn, toris, torif As String ' F�����敪,������E���R�[�h
                            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
                            If DB.GetFToriCode(itakucd, "", toris, torif, fkbn, nFormatKbn) Then
                                ' ������E���R�[�h��������

                                cmtd = New clsCmtData
                                cmtd.Init(0, 1, False, False)

                                DB.GetFormatKbn(itakucd, nFormatKbn)

                                If (compflag) Then
                                    ' Complock�ňÍ�������Ă���ꍇ�̏���
                                    cmtd.ComplockInit(itakucd, furidate)
                                Else
                                    ' �����̏���
                                    If (cmtFormat.FirstRead(serverpath & fiTemp.Name) = 1) Then
                                        Me.ParseCMTFormat(nFormatKbn, cmtFormat, cmtd, False)
                                    Else
                                        cmtd.InitEmptyItakuData(False)
                                    End If

                                    '*** �C�� mitsu 2008/09/08 �t�@�C����������� ***
                                    Try : cmtFormat.Close() : Finally : End Try
                                    '************************************************
                                End If

                                If cmtd.NotError Then
                                    DB.GetWriteCounter(itakucd, furidate, wcounter)
                                    cmtd.WriteCounter = wcounter
                                End If

                                With cmtd
                                    ' �t�@�C���j���̃G���[���܂߂đS���\��
                                    For j As Integer = 0 To .ItakuCounter() Step 1
                                        Dim lv As New ListViewItem((i + 1).ToString.PadLeft(5, "0"c))
                                        If Not .NotError() Then
                                            lv.BackColor = Color.LightPink
                                        Else
                                            lv.BackColor = Color.White
                                        End If
                                        .getListViewItem(lv, j)
                                        listview.Items.AddRange(New ListViewItem() {lv})
                                    Next j
                                End With
                                cmtd.Dispose()
                                i += 1
                            Else
                                ' �T�[�o��̃t�@�C�����������}�X�^�ɖ��o�^
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "�T�[�o��̃t�H���_�Ɏ����}�X�^���o�^�̃f�[�^��������܂��� �t�@�C�����F" & serverpath
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            End If
                        Else
                            ' �T�[�o��̃t�@�C�����������}�X�^�ɖ��o�^
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "�T�[�o��̃t�H���_�Ɏ����}�X�^���o�^�̃f�[�^��������܂��� �t�H���_���F" & serverpath
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        End If
                    Else
                        ' �T�[�o��̃t�@�C�����������}�X�^�ɖ��o�^
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "�T�[�o��ɗL���łȂ��t�@�C�����̃f�[�^��������܂��� �t�H���_���F" & serverpath
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End If
                Next fiTemp
            Next nFormatKbn
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�T�[�o�ԋp�t�@�C���������s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return True
    End Function

    ' �@�@�\ : Listview����CSV�ւ̕ϊ�
    '
    ' �߂�l : �쐬���� true / ���s false
    '
    ' ������ : ARG1 - �Ώ�ListView
    '        : ARG2 - ��������CSV�t�@�C��
    ' ���@�l :
    '
    Public Function MakeCsvFile(ByVal listview As ListView, ByRef strCSVpath As String, ByRef strCSVfile As String, ByVal strTitle As String) As Boolean
        Dim sw As StreamWriter
        Dim onString As String

        ' ListView�Ɉ���Ώۂ̍��ڂ����邩�ǂ����`�F�b�N
        If listview.Items.Count = 0 Then
            MessageBox.Show("����Ώۂ̃f�[�^������܂���.", "����@�\")
            Return False
        End If

        Try
            strCSVpath = GCom.GetPRTFolder
            If strCSVpath = "err" Then
                MessageBox.Show("�ݒ�t�@�C���ɖ�肪����܂��B�V�X�e���Ǘ��҂ɂ��A����������", "����G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "fskj.ini�t�@�C����[COMMON]��PRT���ڂ��ݒ肳��Ă��Ȃ��\��������܂��B"
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
            strCSVfile = "CMT�ϊ�_" & String.Format("{0:yyyyMMddhhmmss}", Date.Now) & ".csv"
            sw = New StreamWriter(strCSVpath & strCSVfile, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            onString = listview.Columns(0).Text.ToString
            For Col As Integer = 1 To listview.Columns.Count - 1 Step 1
                onString &= "," & listview.Columns(Col).Text
            Next Col
            onString &= ",�^�C�g��"
            sw.WriteLine(onString)

            For row As Integer = 0 To listview.Items.Count - 1
                onString = listview.Items.Item(0).Text.ToString
                For Col As Integer = 1 To listview.Items(row).SubItems.Count - 1 Step 1
                    onString &= "," & listview.Items.Item(row).SubItems(Col).Text.ToString
                Next Col
                onString &= "," & strTitle
                sw.WriteLine(onString)
            Next row
            sw.Close()
            sw = Nothing

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "����pCSV�t�@�C���쐬���s " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            sw = Nothing
        End Try
        Return True
    End Function

    ' �@�@�\ : Listview�̈��
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �Ώ�ListView
    '
    ' ���@�l : 
    '
    Public Function PrintButton(ByVal lv As ListView, ByVal title As String) As Boolean
        GCom.GLog.Job2 = "����{�^��"
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim path As String = String.Empty
        Dim csvname As String = String.Empty
        'Dim path As String
        'Dim csvname As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        Try
            If Not CmtCom.MakeCsvFile(lv, path, csvname, title) Then   ' ���[�pCSV�𐶐�
                Return False
            End If

            Dim repoAgent As New CAstReports.RAX
            repoAgent.ReportName = "CMT�ϊ����ʈꗗ.rpd"                                ' ���|�[�g��`�����w��
            repoAgent.CsvPath = path
            repoAgent.CsvName() = csvname
            If repoAgent.PrintOut() = False Then
                Dim MSG As String
                MSG = String.Format("{0}{2}{1}", "����ł��܂���ł���", repoAgent.Message, Environment.NewLine)
                MessageBox.Show(MSG, "����G���[", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "���CSV�t�@�C�����F" & repoAgent.CsvPath & ", ���|�G�[�W�F���g���b�Z�[�W�F" & repoAgent.CsvName & " " & repoAgent.Message
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "����pCSV�t�@�C���쐬���s " & ex.StackTrace
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    ' �@�@�\ : ���s���������f�[�^��CMT�ւ̏���
    '
    ' �߂�l : ���� true / ���s false
    '
    ' ������ : �Ȃ�
    '
    ' ���@�l : 
    '
    Public Function WriteOtherCMT() As Boolean
        Dim serverpath As String
        Dim bankcd As String                                ' ���Z�@�փR�[�h
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim bankname As String = String.Empty               ' ���Z�@�֖�
        'Dim bankname As String                              ' ���Z�@�֖�
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        '***ASTAR 2008.08.07 �u���b�N�T�C�Y�Ή� >>
        '�u���b�N�T�C�Y�P�W�O�O
        cmtctrl.BlockSize = 1800
        '***ASTAR 2008.08.07 �u���b�N�T�C�Y�Ή� <<

        GCom.GLog.Job2 = "���s�������f�[�^CMT����"

        serverpath = MakeOtherFileName(True)    ' �T�[�o�̕ԋp�t�@�C���̒T��path�𐶐�
        If Not Directory.Exists(serverpath) Then ' �t�H���_�����݂��邩�ǂ�������
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�T�[�o�ԋp�t�H���_���݂���܂��� �t�H���_���F" & serverpath
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        ' �t�H���_�����݂���ꍇ
        Dim di As New DirectoryInfo(serverpath) ' �T�[�o�p�X�̃f�B���N�g�����𐶐�
        Dim fi As FileInfo() = di.GetFiles()    ' �t�@�C�����̔z��𐶐�
        Dim fiTemp As FileInfo

        '*** �C�� mitsu 2008/09/01 ��������0�l�� ***
        If fi.Length = 0 Then
            MessageBox.Show("�����Ώۃt�@�C���͂���܂���", "���s�������f�[�^CMT����", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True
        End If
        '*******************************************

        '*** �C�� mitsu 2008/09/01 CMT�ڑ��m�F ***
        If Not Me.CheckConnectCMT() Then ' CMT���ڑ�����Ă��Ȃ��ꍇ�͏I��
            Return False
        End If
        '*****************************************

        For Each fiTemp In fi
            ' ���[�J���t�@�C���̍폜
            If (Not Me.LocalFileDelete(True)) Or (Not Me.LocalFileDelete(False)) Then
                '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
                'With GCom.GLog
                '    .Result = "�G���["
                '    .Discription = "���[�J��PC�̃t�H���_���������s"
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("CMT�����@�̈ꎞ�t�@�C���p�t�H���_�̏����Ɏ��s���܂����B", GCom.GLog.Job2 _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**************************************************
                Return False
            End If

            ' �t�@�C�����̃`�F�b�N
            If fiTemp.Name.Length > 3 AndAlso _
                (GCom.NzInt(fiTemp.Name.Substring(0, 4)) > 0 And GCom.NzInt(fiTemp.Name.Substring(0, 4)) < 9999) Then
                bankcd = fiTemp.Name.Substring(0, 4) ' ���Z�@�փR�[�h�̐؂�o��
                If Not DB.GetBankName(bankcd, bankname) Then ' ���Z�@�փ}�X�^�ɑ��݃`�F�b�N
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "���Z�@�փR�[�h���݂Ȃ� ���Z�@�փR�[�h�F" & bankcd
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                ElseIf MessageBox.Show("���Z�@�փR�[�h�F" & bankcd & ", ���Z�@�֖��F" & bankname.TrimEnd & _
                    " �̐����f�[�^��CMT�ɏ������݂܂���?", "���s�������f�[�^CMT����", MessageBoxButtons.YesNo, MessageBoxIcon.Question) _
                        = DialogResult.No Then
                    ' �������܂Ȃ��ꍇ�͉����������s��Ȃ�
                ElseIf Not Me.IsEmptyCmt() AndAlso MessageBox.Show("CMT����ł͂���܂���B�㏑�����܂����H", "���s�������f�[�^CMT����", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then ' �������s
                    ' CMT����ł͂Ȃ��ꍇ
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT���t�H�[�}�b�g�ς݂ł͂Ȃ����ߒ��f"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    cmtctrl.UnloadCmt()
                    Exit For ' ���[�v�𔲂���
                ElseIf Me.BinaryCopy(serverpath & fiTemp.Name, gstrCMTWriteFileName(0), 120) = 0 Then
                    ' ��������
                    '*** ASTAR 2008.08.07 �����Ɏ��s�����ꍇ�Ƀ��b�Z�[�W��\������ >>
                    'Me.DeleteFile(serverpath & fiTemp.Name)
                    'cmtctrl.WriteCmt(1)
                    If cmtctrl.WriteCmt(1) = False Then
                        Call MessageBox.Show("���Z�@�փR�[�h:" & bankcd & ", ���Z�@�֖�:" & bankname & Environment.NewLine & Environment.NewLine & _
                    "�����Ɏ��s���܂����B", "���s�������f�[�^CMT����", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    Else
                        If MessageBox.Show(MSG0061I, "���s�������f�[�^CMT����", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                            cmtctrl.UnloadCmt()
                            MessageBox.Show(MSG0070I, "���s�������f�[�^CMT����", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                            File.Delete(gstrCMTWriteFileName(0))
                            File.Delete(gstrCMTReadFileName(0))
                            If Not Me.IsEmptyCmt() AndAlso MessageBox.Show("CMT����ł͂���܂���B�㏑�����܂����H", "���s�������f�[�^CMT����", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then ' �������s
                                ' CMT����ł͂Ȃ��ꍇ
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMT���t�H�[�}�b�g�ς݂ł͂Ȃ����ߒ��f"
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                cmtctrl.UnloadCmt()
                                Exit For ' ���[�v�𔲂���
                            ElseIf Me.BinaryCopy(serverpath & fiTemp.Name, gstrCMTWriteFileName(0), 120) = 0 Then
                                If cmtctrl.WriteCmt(1) = False Then
                                    Call MessageBox.Show("���Z�@�փR�[�h:" & bankcd & ", ���Z�@�֖�:" & bankname & Environment.NewLine & Environment.NewLine & _
                                                         "�����Ɏ��s���܂����B", "���s�������f�[�^CMT����", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Return False
                                End If
                            End If
                        End If
                        Me.DeleteFile(serverpath & fiTemp.Name)
                        End If
                        '*** ASTAR 2008.08.07 �����Ɏ��s�����ꍇ�Ƀ��b�Z�[�W��\������ <<
                        cmtctrl.UnloadCmt()
                End If
            End If
        Next fiTemp
        MessageBox.Show("��������", "���s�������f�[�^CMT����", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Return True
    End Function

    ' �@�@�\ : �}������Ă���CMT���t�H�[�}�b�g�ς݂��ǂ����`�F�b�N
    '
    ' �߂�l : ���� true / ���s false
    '
    ' ������ : �Ȃ�
    '
    ' ���@�l : 
    '
    Protected Function IsEmptyCmt() As Boolean
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        Try
            ' CMT�荷���̌��o
            If (cmtctrl.SelectCmt(1)) Then ' 1�{�ڂ����݂��邩�`�F�b�N
                If Not Me.IsExistStacker(cmtctrl, False) Then ' 2�{�ȏ㑶�݂��Ȃ������`�F�b�N
                    ' CMT��1�{�����̏ꍇ
                    'MessageBox.Show("��{�������o")
                Else
                    ' 2�{�ȏ゠��ꍇ���ُ�Ƃ݂Ȃ�
                    MessageBox.Show("CMT��2�{�ȏ�}������Ă��܂��B��{�����������ł��܂���", "���s�������f�[�^CMT������", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    cmtctrl.SelectCmt(22) ' Eject
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT�{���G���[ �����{��CMT���}���ς�"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If
            Else
                MessageBox.Show("CMT���}������Ă��܂���B", "���s�������f�[�^CMT������", MessageBoxButtons.OK, MessageBoxIcon.Information)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMT���}���G���["
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If

            cmtctrl.ReadCmt(1) ' CMT�t�@�C���Ǎ�

            If File.Exists(gstrCMTReadFileName(0)) Then
                ' CMT����łȂ��Ƃ�
                'cmtctrl.UnloadCmt()
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMT���t�H�[�}�b�g�ς݂ł͂���܂���ł���"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

    ' �@�@�\ : ���s�������ʃf�[�^��CMT����̓ǎ�
    '
    ' �߂�l : ���� true / ���s false
    '
    ' ������ : �Ȃ�
    '
    ' ���@�l : 
    '
    Public Function ReadOtherCMT() As Boolean
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim filename As String
        Dim serverpath As String
        Const msgtitle As String = "���s�����ʃf�[�^�ǎ�"   '*** 2008.05.29 ���s���������s��

        Try
            ' ���[�J���t�@�C���̍폜
            If (Not Me.LocalFileDelete(True)) Or (Not Me.LocalFileDelete(False)) Then
                '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
                'With GCom.GLog
                '    .Result = "�G���["
                '    .Discription = "���[�J��PC�̃t�H���_���������s"
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("CMT�ǎ�@�̈ꎞ�t�@�C���p�t�H���_�̏����Ɏ��s���܂����B", GCom.GLog.Job2 _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**************************************************
                Return False
            End If

            serverpath = MakeOtherFileName(False)
            If Not Directory.Exists(serverpath) Then ' �t�H���_�����݂��邩�ǂ�������
                '*** �C�� mitsu 2008/09/01 �G���[���b�Z�[�W�\�� ***
                'With GCom.GLog
                '    .Result = "�T�[�o�A�b�v���[�h�t�H���_���݂���܂���"
                '    .Discription = "�t�H���_��:" & serverpath
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("�T�[�o�A�b�v���[�h�t�H���_���݂���܂���" & vbCrLf & _
                                 "�t�H���_���F" & serverpath, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**********************************
                Return False
            End If

            '*** �C�� mitsu 2008/09/01 CMT�ڑ��m�F ***
            If Not Me.CheckConnectCMT() Then ' CMT���ڑ�����Ă��Ȃ��ꍇ�͏I��
                Return False
            End If
            '*****************************************

            If (Me.IsExistStacker(cmtctrl, False)) Then
                ' �X�^�b�J�ɂ͔�Ή�
                MessageBox.Show("�X�^�b�J�ɂ͑Ή����Ă��܂���", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                With GCom.GLog
                    .Job2 = msgtitle
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�X�^�b�J�����݂��Ă������߃G���[�Ƃ��ď���"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                cmtctrl.UnloadCmt()
                cmtctrl.SelectCmt(22) ' Eject
                Return False
            End If

            filename = serverpath & "0005" ' �O�H����UFJ��s���ߑł�
            If Me.ReadStacker(11, 1) Then ' CMT�t�@�C���̓Ǎ�
                ' �Ǎ�����
                '*** ASTAR 2008.05.29 ���Z�@�֌��߂������͂���     �荷���̂݉\�i�d�l�j ***
                Dim MSG As String
                Dim FMT As New CAstFormat.CFormatZengin
                If FMT.FirstRead(gstrCMTReadFileName(0)) = 1 Then
                    Call FMT.CheckDataFormat()
                    Call FMT.CheckRecord1()
                    filename = serverpath & FMT.InfoMeisaiMast.ITAKU_KIN
                    MSG = "���Z�@�փR�[�h �F " & FMT.InfoMeisaiMast.ITAKU_KIN
                    MSG &= Environment.NewLine & Environment.NewLine
                    MSG &= "��낵���ł����H"
                    If MessageBox.Show(MSG, msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                        FMT.Close()
                        Return False
                    End If
                End If
                FMT.Close()
                FMT = Nothing
                Dim nRet As Integer
                nRet = Me.BinaryCopy(gstrCMTReadFileName(0), filename, 120)
                '**********************************************************
                If nRet = 0 Then
                    MessageBox.Show(filename & "�ւ̃A�b�v���[�h��������", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    Return True
                    '*** ASTAR 2008.05.29 �G���[�����ǉ�                ***
                ElseIf nRet = 2 Then
                    MessageBox.Show("�t�@�C�������ɑ��݂��܂��B �����𒆒f���܂��B" & Environment.NewLine & Environment.NewLine & filename, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                    '**********************************************************
                Else
                    MessageBox.Show("�T�[�o�A�b�v���[�h���s", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "�T�[�o�A�b�v���[�h���s �t�@�C�����F" & filename
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If
            Else
                MessageBox.Show("CMT�Ǎ����s", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                cmtctrl.UnloadCmt()
                '*** �C�� mitsu 2008/09/01 �s�v ***
                'With GCom.GLog
                '    .Result = "CMT�Ǎ����s"
                '    .Discription = " -- "
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT�Ǎ�"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

End Class
