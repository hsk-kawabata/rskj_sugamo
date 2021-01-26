Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CASTCommon.MyOracle
'*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

' �ʗ������ݏ���
Public Class ClsTouroku

    Private clsFusion As New clsFUSION.clsMain
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    Private vbDLL As New CMTCTRL.ClsFSKJ
    'Private vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

    ' �ʋN���p�����[�^ �\����
    Structure TourokuParam
        Dim CP As CAstBatch.CommData.stPARAMETER

        Public WriteOnly Property Data() As String      '�Œ蒷�f�[�^�����p�v���p�e�B
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                CP.TORI_CODE = ""                           '�����R�[�h
                CP.FURI_DATE = para(0)                      '�U�֓�
                CP.CODE_KBN = "4"                           '�R�[�h�敪
                CP.FMT_KBN = "TO"                           '�t�H�[�}�b�g�敪
                CP.BAITAI_CODE = para(1).PadLeft(2, "0"c)   '�}�̃R�[�h
                '2010/01/27 add
                'CP.LABEL_KBN = ""                           '���x���敪
                CP.LABEL_KBN = "2"                           '���x���敪
                CP.RENKEI_FILENAME = ""                     '�A�g�t�@�C����
                CP.ENC_KBN = ""                             '�Í��������敪
                CP.ENC_KEY1 = ""                            '�Í����L�[�P
                CP.ENC_KEY2 = ""                            '�Í����L�[�Q
                CP.ENC_OPT1 = ""                            '�`�d�r�I�v�V����
                CP.CYCLENO = ""                             '�T�C�N����
                CP.JOBTUUBAN = Integer.Parse(para(2))       '�W���u�ʔ�
            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    ' �b�n�l�o�k�n�b�j�Í������
    Private Structure stCOMPLOCK
        Dim AES As String
        Dim KEY As String
        Dim IVKEY As String
        Dim RECLEN As String
    End Structure

    Dim Complock As stCOMPLOCK

    Dim mUserID As String = ""                      '���[�U�h�c
    Dim mJobMessage As String = ""                  '�W���u���b�Z�[�W
    Private mDataFileName As String                 '�˗��f�[�^�t�@�C����
    Private mArgumentData As CommData               '�N���p�����[�^ ���ʏ��
    Public MainDB As CASTCommon.MyOracle            '�p�u���b�N�c�a
    Private mErrMessage As String = ""              '�G���[���b�Z�[�W(�������ʊm�F�\����p)
    Private ArrayTenbetu As New ArrayList           '�X�ʏW�v�\�o�͑Ώ� �i�[���X�g

    ' New
    Public Sub New()
    End Sub

    '=================================================================
    ' �@�\�@ �F �����又���@���C��
    ' �����@ �F command - �N���p�����[�^
    ' �߂�l �F True - ����CFalse - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/07
    ' �X�V�� �F 
    '=================================================================
    Function Main(ByVal command As String) As Integer
        Try
            '--------------------------------------------
            ' �p�����[�^�擾
            '--------------------------------------------
            '���C�������ݒ�
            mKoParam.Data = command

            '�W���u�ʔԐݒ�
            BatchLog.Write("0000000000-00", "00000000", "(�p�����[�^�擾)�J�n", "����")
            BatchLog.JobTuuban = mKoParam.CP.JOBTUUBAN
            BatchLog.ToriCode = mKoParam.CP.TORI_CODE
            BatchLog.FuriDate = mKoParam.CP.FURI_DATE
        Catch ex As Exception
            BatchLog.Write("(�p�����[�^�擾)", "���s", "�p�����[�^�擾���s[" & command & "]�F" & ex.Message)

            Return 1
        End Try

        Try
            '--------------------------------------------
            '�p�����[�^���ݒ�
            '--------------------------------------------
            Dim bRet As Boolean                         '��������
            Dim InfoParam As New CommData.stPARAMETER   '�p�����[�^���
            'Dim ErrFileName As String               '�t�@�C����

            '�I���N��
            MainDB = New CASTCommon.MyOracle

            '�N���p�����[�^���ʏ��
            mArgumentData = New CommData(MainDB)

            '�p�����[�^����ݒ�
            InfoParam.TORI_CODE = mKoParam.CP.TORI_CODE
            InfoParam.BAITAI_CODE = mKoParam.CP.BAITAI_CODE
            InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
            InfoParam.FURI_DATE = mKoParam.CP.FURI_DATE
            InfoParam.CODE_KBN = mKoParam.CP.CODE_KBN
            InfoParam.RENKEI_FILENAME = mKoParam.CP.RENKEI_FILENAME
            InfoParam.ENC_KBN = mKoParam.CP.ENC_KBN
            InfoParam.ENC_KEY1 = mKoParam.CP.ENC_KEY1
            InfoParam.ENC_KEY2 = mKoParam.CP.ENC_KEY2
            InfoParam.ENC_OPT1 = mKoParam.CP.ENC_OPT1
            InfoParam.CYCLENO = mKoParam.CP.CYCLENO
            InfoParam.JOBTUUBAN = mKoParam.CP.JOBTUUBAN
            InfoParam.TIME_STAMP = DateTime.Now.ToString("HHmmss")
            mArgumentData.INFOParameter = InfoParam

            '--------------------------------------------
            '�����������s
            '--------------------------------------------
            bRet = TourokuMain()

            '--------------------------------------------
            '��������E���[�o��
            '--------------------------------------------
            If bRet = False Then
                '�������ʊm�F�\�o�́i�V�X�e���G���[)
                Dim oRenkei As New ClsRenkei(mArgumentData)

                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(mKoParam.CP.FSYORI_KBN, _
                        mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE, _
                        mKoParam.CP.JOBTUUBAN, Path.GetFileName(mKoParam.CP.RENKEI_FILENAME), mErrMessage, MainDB) = False Then
                    BatchLog.Write("�������ʊm�F�\", "���s", "�t�@�C����:" & Prn.FileName & " ���b�Z�[�W:" & Prn.ReportMessage)

                End If

                Prn = Nothing

            Else
                '��������I��
                Dim DestFile As String = ""
                Try
                    '�`���E�w�Z�̏ꍇ�͈˗��t�@�C����ޔ�
                    ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
                    'Select Case InfoParam.BAITAI_CODE
                    '    Case "00", "07"
                    '        DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                    '        '�O��t�@�C�����폜
                    '        If File.Exists(DestFile) = True Then
                    '            File.Delete(DestFile)
                    '        End If

                    '        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    '    Case Else

                    '        DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                    '        '�O��t�@�C�����폜
                    '        If File.Exists(DestFile) = True Then
                    '            File.Delete(DestFile)
                    '        End If

                    '        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    'End Select
                    Select Case InfoParam.BAITAI_CODE
                        Case "00", "07", _
                             "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '�O��t�@�C�����폜
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                        Case Else

                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '�O��t�@�C�����폜
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    End Select
                    ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END

                Catch ex As Exception
                    BatchLog.Write("����t�H���_�ړ�", "���s", mKoParam.CP.RENKEI_FILENAME & " -> " & DestFile)
                End Try
            End If

            MainDB.Close()

            'RV-A45 �X�V�����ʒu�ύX
            ' JOB�}�X�^�X�V
            If Not bRet = False Then
                If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
                    bRet = False
                End If
            End If
            '========================
            '---------------------------------------------------------------------------------

            BatchLog.Write("�I��", "����", bRet.ToString)


            If bRet = False Then
                Return 2
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write("(�������݃��C������)", "���s", ex.Message)

        End Try
    End Function

    '=================================================================
    ' �@�\�@ �F �o�^���C������
    ' �����@ �F �Ȃ�
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/08
    ' �X�V�� �F 
    '=================================================================
    Private Function TourokuMain() As Boolean

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        Try
            BatchLog.Write("(�o�^���C������)�J�n", "����")


            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                BatchLog.Write_Err("(�o�^���C������)", "���s", "�Z���^�[���ڎ������������Ŏ��s�҂��^�C���A�E�g")
                BatchLog.UpdateJOBMASTbyErr("�Z���^�[���ڎ������������Ŏ��s�҂��^�C���A�E�g")
                mErrMessage = "���s�҂��^�C���A�E�g"
                Return False
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '--------------------------------------------
            '�}�̓ǂݍ��݁i�t�H�[�}�b�g�@�j
            '--------------------------------------------
            Dim oReadFMT As CAstFormat.CFormat
            Dim sReadFile As String = ""

            Try
                '�t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
                'oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
                oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)

                If oReadFMT Is Nothing Then
                    BatchLog.Write("(�o�^���C������)", "���s", "�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j")

                    BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j")
                    mErrMessage = "�t�H�[�}�b�g�擾���s"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(�o�^���C������)", "���s", "�t�H�[�}�b�g�擾�F" & ex.Message)
                BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾")
                mErrMessage = "�t�H�[�}�b�g�擾���s"
                Return False
            End Try

            'SJIS ���s�Ȃ��t�@�C���ɕϊ����ēǂݍ���(�}�̓Ǎ��̎��s)
            sReadFile = ReadMediaData(oReadFMT)
            If sReadFile = "" Then
                oReadFMT.Close()
                ' 2016/02/08 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- START
                'BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾���s")
                Select Case mErrMessage
                    Case ""
                        BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾���s")
                    Case Else
                        BatchLog.UpdateJOBMASTbyErr(mErrMessage)
                End Select
                ' 2016/02/08 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- END
                BatchLog.Write("(�o�^���C������)", "���s", "�}�̓ǂݍ��݁F" & oReadFMT.Message)
                mErrMessage = "�t�H�[�}�b�g�擾���s"
                Return False
            End If

            '--------------------------------------------
            '�f�[�^�`�F�b�N
            '--------------------------------------------
            '*****20120704 mubuchi  "11"(DVD)�ǉ�***********>>>>
            ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "11" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" Then
            '    '*****20120704 mubuchi  "11"(DVD)�ǉ�***********<<<<
            '    Dim nPos As Long = -1
            '    Dim nLine As Long = 0
            '    Dim nErrorCount As Long = 0
            '    Dim FirstError As String = ""

            '    oReadFMT.FirstRead(sReadFile)

            '    Do Until oReadFMT.EOF
            '        nLine += 1

            '        '1���R�[�h�@�f�[�^�擾
            '        If oReadFMT.GetFileData() = 0 Then
            '        End If

            '        '�K�蕶���`�F�b�N
            '        nPos = oReadFMT.CheckRegularString()

            '        If nPos >= 0 Then
            '            nErrorCount += 1
            '            BatchLog.Write("(�o�^���C������)", "����", "�K��O��������F" & _
            '                                                        nLine.ToString & "�s�ڂ�" & (nPos + 1).ToString & "�o�C�g�ڂɕs���ȕ����������Ă��܂�")

            '            If FirstError Is Nothing Then
            '                FirstError = nLine.ToString & "�s�ڂ�" & (nPos + 1).ToString & "�o�C�g�ڂɕs���ȕ����������Ă��܂�"
            '            End If

            '            If nErrorCount >= 10 Then
            '                Exit Do
            '            End If
            '        End If
            '    Loop

            '    oReadFMT.Close()

            'End If
            Select Case mKoParam.CP.BAITAI_CODE
                Case "00", "01", "05", "06", _
                     "11", "12", "13", "14", "15", _
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    oReadFMT.FirstRead(sReadFile)

                    Do Until oReadFMT.EOF
                        nLine += 1

                        '1���R�[�h�@�f�[�^�擾
                        If oReadFMT.GetFileData() = 0 Then
                        End If

                        '�K�蕶���`�F�b�N
                        nPos = oReadFMT.CheckRegularString()

                        If nPos >= 0 Then
                            nErrorCount += 1
                            BatchLog.Write("(�o�^���C������)", "����", "�K��O��������F" & _
                                                                        nLine.ToString & "�s�ڂ�" & (nPos + 1).ToString & "�o�C�g�ڂɕs���ȕ����������Ă��܂�")

                            If FirstError Is Nothing Then
                                FirstError = nLine.ToString & "�s�ڂ�" & (nPos + 1).ToString & "�o�C�g�ڂɕs���ȕ����������Ă��܂�"
                            End If

                            If nErrorCount >= 10 Then
                                Exit Do
                            End If
                        End If
                    Loop

                    oReadFMT.Close()
            End Select
            ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END

            '--------------------------------------------
            ' ���׃}�X�^�o�^����
            '--------------------------------------------
            If MakingMeiMast(oReadFMT) = False Then
                oReadFMT.Close()

                If File.Exists(sReadFile) = True Then
                    File.Delete(sReadFile)
                End If

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                ' ���[���o�b�N
                MainDB.Rollback()
                Return False
            End If

            oReadFMT.Close()
            If File.Exists(sReadFile) = True Then
                File.Delete(sReadFile)
            End If

            'RV-A45 �X�V�����ʒu�ύX
            ' JOB�}�X�^�X�V
            'If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
            '    ' ���[���o�b�N
            '    MainDB.Rollback()
            '    Return False
            'End If
            '========================

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            ' �c�a�R�~�b�g
            MainDB.Commit()

            Return True

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            'RV-A45 ���[���o�b�N��ǉ�
            MainDB.Rollback()
            '=================
            BatchLog.Write("(�o�^���C������)", "���s", ex.Message)
            Return False

        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)

            ' ���[���o�b�N
            MainDB.Rollback()
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            BatchLog.Write("(�o�^���C������)�I��", "����")

        End Try
    End Function

    '=================================================================
    ' �@�\�@ �F �p�����[�^�`�F�b�N
    ' �����@ �F �Ȃ�
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/08
    ' �X�V�� �F 
    '=================================================================
    Function CheckParameter() As Boolean
        Dim SQL As StringBuilder = New StringBuilder(256)

        Try
            BatchLog.Write("(�p�����[�^�`�F�b�N)�J�n", "����")


            '---------------------------------------------------------
            '�t�H�[�}�b�g�敪�̃`�F�b�N
            '---------------------------------------------------------
            If mKoParam.CP.FMT_KBN <> mArgumentData.INFOToriMast.FMT_KBN_T Then
                BatchLog.Write("(�p�����[�^�`�F�b�N)", "���s", "�t�H�[�}�b�g�敪�ُ�")
                Call BatchLog.UpdateJOBMASTbyErr("�����}�X�^�o�^�ُ�F�t�H�[�}�b�g�敪")
                mErrMessage = "�t�H�[�}�b�g�敪�ُ�"
                Return False
            End If

            '---------------------------------------------------------
            '�}�̃R�[�h�̃`�F�b�N
            '---------------------------------------------------------
            If mKoParam.CP.BAITAI_CODE <> mArgumentData.INFOToriMast.BAITAI_CODE_T Then
                BatchLog.Write("(�p�����[�^�`�F�b�N)", "���s", "�}�̃R�[�h�ُ�")
                Call BatchLog.UpdateJOBMASTbyErr("�����}�X�^�o�^�ُ�F�}�̃R�[�h")
                mErrMessage = "�}�̃R�[�h�ُ�"
                Return False
            End If

            '---------------------------------------------------------
            '�X�P�W���[���̗L���̃`�F�b�N�^�������ݏ����������̊m�F
            '---------------------------------------------------------
            Dim oReader As CASTCommon.MyOracleReader = Nothing

            SQL.Append("SELECT ")
            SQL.Append(" TYUUDAN_FLG_S")
            SQL.Append(",TOUROKU_FLG_S")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(mKoParam.CP.FURI_DATE))

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    BatchLog.Write("�X�P�W���[���Ȃ�", "���s", "�U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���Ȃ� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    mErrMessage = "�X�P�W���[���Ȃ�"
                    Return False
                Else
                    '�o�^�t���O�^���f�t���O�̊m�F
                    'Do While oReader.EOF = False
                    If oReader.GetString("TOUROKU_FLG_S") <> "0" AndAlso oReader.GetString("TYUUDAN_FLG_S") = "0" Then
                        BatchLog.Write("�X�P�W���[��", "���s", "�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                        Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                        mErrMessage = "�������ݏ�����"
                    End If
                    'oReader.NextRead()
                    'Loop
                End If

                If oReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                    BatchLog.Write("�X�P�W���[��", "���s", "���f�t���O�ݒ�� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                   Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:���f�t���O�ݒ�� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                    mErrMessage = "���f�t���O�ݒ�ς�"
                    Return False
                End If

                If oReader.GetString("TOUROKU_FLG_S") <> "0" Then
                    BatchLog.Write("�X�P�W���[��", "���s", "�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"), "���f", "�����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "�������ݏ�����"
                    Return False
                End If

                Return True

            Catch ex As Exception
                BatchLog.Write("�X�P�W���[������", "���s", " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���������s �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                mErrMessage = "�X�P�W���[���������s"
                Return False

            Finally
                If Not oReader Is Nothing Then
                    oReader.Close()
                End If
            End Try

            Return True

        Catch ex As Exception
            Call BatchLog.UpdateJOBMASTbyErr("�p�����[�^�`�F�b�N�F�V�X�e���G���[�i���O�Q�Ɓj")
            BatchLog.Write("(�p�����[�^�`�F�b�N)", "���s", ex.Message)

            mErrMessage = "�p�����[�^�`�F�b�N���s"
            Return False

        Finally
            BatchLog.Write("(�p�����[�^�`�F�b�N)�I��", "����")
        End Try
    End Function

    '=================================================================
    ' �@�\�@ �F �˗��f�[�^�Ǎ�����
    ' �����@ �F readfmt - �t�H�[�}�b�g�N���X
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/08
    ' �X�V�� �F 
    '=================================================================
    Public Function ReadMediaData(ByVal readfmt As CAstFormat.CFormat) As String
        Dim nRetRead As Integer = 0     '�}�̓ǂݍ��݌���
        Dim FTranName As String         'F*TRAN�t�@�C����
        Dim strKekka As String

        Try
            BatchLog.Write("(�˗��f�[�^�Ǎ�)�J�n", "����")

            '�˗��f�[�^�Ǎ��^�t�H�[�}�b�g�`�F�b�N���s
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData) '�A�g

            '------------------------------------------------------------
            '�}�̓ǂ�
            '------------------------------------------------------------
            With mKoParam.CP

                '���ԃt�@�C���쐬��w��
                oRenkei.OutFileName = CASTCommon.GetFSKJIni("COMMON", "DATBK") & "iKINKO_WATASHI.DAT"

                'F*TRAN�t�@�C�������擾
                If .CODE_KBN = "4" And .BAITAI_CODE = "01" Then
                    FTranName = readfmt.FTRANIBMP
                    'FTranName = readfmt.FTRANIBMBINARYP
                Else
                    FTranName = readfmt.FTRANP
                End If

                '------------------------------------------------------------
                '�}�̕ʁ@�t�@�C���捞���s
                '------------------------------------------------------------
                '2009/12/09 CMT/MT ���ڑ��Ή�
                Dim Baitai_Code As String
                Select Case mKoParam.CP.BAITAI_CODE
                    Case "05"
                        If CASTCommon.GetFSKJIni("COMMON", "MT") = "1" Then 'MT���ڑ�
                            Baitai_Code = "MT"  '��p�}�̃R�[�h
                        Else
                            Baitai_Code = mKoParam.CP.BAITAI_CODE
                        End If
                    Case "06"
                        If CASTCommon.GetFSKJIni("COMMON", "CMT") = "1" Then 'CMT���ڑ�
                            Baitai_Code = "CMT"  '��p�}�̃R�[�h
                        Else
                            Baitai_Code = mKoParam.CP.BAITAI_CODE
                        End If
                    Case Else
                        Baitai_Code = mKoParam.CP.BAITAI_CODE
                End Select
                'Select mKoParam.CP.BAITAI_CODE
                Select Case Baitai_Code
                    '===================================

                    Case "01"       '�R�D�T�e�c
                        BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�}�̃R�[�h�s��")

                        BatchLog.UpdateJOBMASTbyErr("�}�̃R�[�h�s��")
                        Return ""

                    Case "05"       '�l�s

                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        '�l�s�t�@�C���Ǎ��������s
                        strKekka = vbDLL.cmtCPYtoDISK(0, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                        'Dim intblk_size As Short
                        'Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        'Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''�l�s�t�@�C���Ǎ��������s
                        'strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        '�}�̓Ǎ����s�F�����I��
                        If strKekka <> "" Then
                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�l�s�Ǎ��F" & oRenkei.Message)
                            BatchLog.UpdateJOBMASTbyErr("�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        End If

                    Case "06"       '�b�l�s

                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        '�b�l�s�t�@�C���R�s�[��ݒ�
                        strKekka = vbDLL.cmtCPYtoDISK(0, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 0, " ", oRenkei.InFileName, 0, 0)

                        'Dim intblk_size As Short
                        'Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        'Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''�b�l�s�t�@�C���R�s�[��ݒ�
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        'strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 0, " ", oRenkei.InFileName, 0, 0)
                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        '�}�̓Ǎ����s�F�����I��
                        If strKekka <> "" Then
                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�b�l�s�Ǎ��F" & oRenkei.Message)
                            BatchLog.UpdateJOBMASTbyErr("�b�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                            Return ""
                        End If
                        '2010/04/26 �O���M���Ή� �o�C�i���œǂݍ���ŕϊ�����
                        Dim REC_LENGTH As Integer
                        REC_LENGTH = readfmt.RecordLen
                        nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, oRenkei.InFileName, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        '======================================================
                    Case Else       '���̑�(�`�� etc)

                        Dim REC_LENGTH As Integer
                        Select Case .CODE_KBN
                            Case "2"
                                '119�o�C�g�i�h�r���s����
                                REC_LENGTH = 119
                            Case "3"
                                '118�o�C�g�i�h�r���s����
                                REC_LENGTH = 118
                            Case Else
                                '���̑�
                                REC_LENGTH = readfmt.RecordLen
                        End Select

                        '�˗��t�@�C���R�s�[��ݒ�
                        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "KINKO_WATASHI.DAT"

                        '2017/01/23 saitou ���t�M��(RSV2�W��) ADD ��e�ʃf���o���Ή� ---------------------------------------- START
                        Dim CENTER_DELIVERY As String = String.Empty
                        CENTER_DELIVERY = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "CENTER_DELIVERY")
                        If CENTER_DELIVERY = "1" Then
                            '��e�ʃf���o��
                            If Not Me.fn_CopyBatchSVToDen(mKoParam.CP.RENKEI_FILENAME) Then
                                Return ""
                            End If
                        Else
                            '�`��
                            If CENTER_DELIVERY = "err" OrElse CENTER_DELIVERY = Nothing Then
                                '�ݒ�l�~�X�i�ʏ�̓`���Ƃ��ď����j
                                BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�ݒ�t�@�C���擾���s [RSV2_V1.0.0]CENTER_DELIVERY �ݒ�Ȃ��̂��ߓ`���Ƃ��ď���")
                            End If
                        End If
                        '2017/01/23 saitou ���t�M��(RSV2�W��) ADD ----------------------------------------------------------- END

                        If GetFSKJIni("CENTER", "COMPLOCK") = "1" Then

                            'CompLock
                            Dim strOfileName As String = CASTCommon.GetFSKJIni("COMMON", "DEN") & "AKINKO_WATASHI.DAT"

                            If GetKeyInfo() = False Then
                                BatchLog.Write("���������擾", "���s", mErrMessage)
                                mErrMessage = "���������擾���s"
                                Return ""
                            End If

                            Dim cmpErr As Long
                            cmpErr = EncodeComplock(Complock, mKoParam.CP.RENKEI_FILENAME, strOfileName)
                            If cmpErr <> 0 Then   '�߂�l�b��
                                BatchLog.Write("����������", "���s", "�G���[�R�[�h:" & cmpErr & " " & mErrMessage)
                                mErrMessage = "�������������s"
                                Return ""
                            End If

                            mKoParam.CP.RENKEI_FILENAME = strOfileName

                        End If

                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ' 2016/02/01 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- START
                        Select Case CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                            Case "2"
                                If File.Exists(oRenkei.InFileName) = False Then
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "���̓t�@�C���Ȃ� : " & oRenkei.InFileName)
                                    mErrMessage = "���̓t�@�C���Ȃ� : " & oRenkei.InFileName
                                    Return ""
                                End If
                            Case Else
                                ' NOP
                        End Select
                        ' 2016/02/01 �^�X�N�j���� CHG �yIT�zUI_B-14-99(RSV2�Ή�(�e�������R��)) -------------------- END

                        'nRetRead = oRenkei.CopyToDisk(readfmt)
                        nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

                End Select
            End With

            '----------------------------------------------------------------------------------------------------
            'Return         :0=�����A50=�t�@�C���Ȃ��A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
            '               :300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
            '----------------------------------------------------------------------------------------------------
            Select Case nRetRead
                Case 0
                    Return oRenkei.OutFileName
                Case 10
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���`���ُ�A�t�@�C�����F" & oRenkei.InFileName)
                    Return ""
                Case 50
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�������F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���Ȃ��A�t�@�C�����F" & oRenkei.InFileName)
                    Return ""
                Case 100
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�ϊ�)�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�R�[�h�ϊ��j")
                    Return ""
                Case 200
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�敪�ُ�(JIS���s����))�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�R�[�h�敪�ُ�iJIS���s����j�j")
                    Return ""
                Case 300
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                    Return ""
                Case 400
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�o�̓t�@�C���쐬)�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�o�̓t�@�C���쐬�j")
                    Return ""
                Case Else
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�s���Ȗ߂�l�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i���O�Q�Ɓj")
                    Return ""
            End Select

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ex.Message)

            Return ""

        Finally
            BatchLog.Write("(�˗��f�[�^�Ǎ�)�I��", "����")
        End Try
    End Function

    '=================================================================
    ' �@�\�@ �F ���׃}�X�^�o�^����
    ' �����@ �F readfmt - �t�H�[�}�b�g�N���X / aReadFile - �Ǎ��t�@�C����
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/08
    ' �X�V�� �F 
    '=================================================================
    Private Function MakingMeiMast(ByVal aReadFmt As CAstFormat.CFormat) As Boolean
        Dim nRecordCount As Integer = 0                         '�t�@�C���S�̂̃��R�[�h�J�E���g
        Dim nRecordNumber As Integer = 0                        '�w�b�_�P�ʂ̃��R�[�h�J�E���g
        Dim sCheckRet As String = ""                            '�`�F�b�N��������
        Dim bRet As Boolean                                     '��������
        Dim PrnInErrList As ClsPrnInputError = Nothing          '�C���v�b�g�G���[���X�g�N���X
        Dim PrnSyoKekka As ClsPrnSyorikekkaKakuninhyo = Nothing '�������ʊm�F�\
        Dim PrnFlag As Boolean = False
        Dim PrnSchErrList As ClsPrnSchError = Nothing          '�X�P�W���[���G���[���X�g�N���X
        Dim PrnToriErrList As ClsPrnToriError = Nothing          '�C���v�b�g�G���[���X�g�N���X
        'Dim PrnMeisai As ClsPrnUketukeMeisai = Nothing          '��t���ו\
        'Dim ArrayPrnMeisai As New ArrayList(128)
        Dim ArrayPrnInErrList As New ArrayList(128)
        'Dim DaihyouItakuCode As String = ""                     '��\�ϑ��҃R�[�h

        Dim SyoriFlg As Boolean = True      '�G���[���X�g�̂��߂ɏ������s�t���O

        Try
            BatchLog.Write("(���׃}�X�^�o�^)�J�n", "����")


            '�����̏����t�H�[�}�b�g�N���X�֓n��
            aReadFmt.Oracle = MainDB
            aReadFmt.LOG = BatchLog

            '�N���p�����[�^����n��
            aReadFmt.ToriData = mArgumentData

            Dim ArrayMeiData As New ArrayList           '�d���`�F�b�N�J�E���^
            Dim EndFlag As Boolean = False              '�G���h���R�[�h���݃t���O
            Dim ErrFlag As Boolean = False              '�C���v�b�g�G���[�ُ폈���t���O
            Dim ErrText As String = ""
            'Dim SitenYomikae As String                  '�x�X�Ǒֈ���敪(1:����Ώ� / ���̑�:�����Ώ�)
            'SitenYomikae = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")

            '--------------------------------------------
            '�S�f�[�^�`�F�b�N
            '--------------------------------------------
            Try
                If aReadFmt.FirstRead() = 0 Then
                    BatchLog.Write("�t�@�C���I�[�v��", "���s", aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���I�[�v�����s")
                    mErrMessage = "�t�@�C���I�[�v�����s"
                    Return False
                End If

                '�o�C���h�ϐ��Ή��N�G������
                If SetInsertMEIMAST(aReadFmt) = False Then
                    BatchLog.UpdateJOBMASTbyErr("MEIMAST�̃C���T�[�g�p�N�G���������s")
                    mErrMessage = "���דo�^���s"
                    Return False
                End If

                Do Until aReadFmt.EOF
                    nRecordCount += 1
                    nRecordNumber += 1

                    ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                    sCheckRet = aReadFmt.CheckDataFormat()

                    Select Case sCheckRet

                        Case "ERR"
                            '�t�H�[�}�b�g�G���[
                            Dim nPos As Long
                            nPos = aReadFmt.CheckRegularString

                            If nPos > 0 Then
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� " & aReadFmt.Message)
                            Else
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            End If

                            mErrMessage = aReadFmt.Message

                            bRet = False
                            Exit Do

                        Case "IJO"
                            '�C���v�b�g�G���[

                            If PrnInErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                PrnInErrList = New ClsPrnInputError        ' �C���v�b�g�G���[���X�g�N���X
                                PrnInErrList.CreateCsvFile()
                            End If

                            Call PrnInErrList.OutputData(aReadFmt, aReadFmt.ToriData)

                        Case "NS"   '�X�P�W���[���Ȃ�
                            BatchLog.Write("�X�P�W���[���G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 1)

                            SyoriFlg = False

                        Case "TS"   '�X�P�W���[�����f
                            BatchLog.Write("�X�P�W���[���G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 2)

                            SyoriFlg = False

                        Case "SS"   '�X�P�W���[���������ݍ�
                            BatchLog.Write("�X�P�W���[���G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 3)

                            SyoriFlg = False

                        Case "NT"   '�������Ȃ�
                            BatchLog.Write("�����G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)

                            If PrnToriErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                PrnToriErrList = New ClsPrnToriError
                                PrnToriErrList.CreateCsvFile()
                            End If

                            Call PrnToriErrList.OutputData(aReadFmt)

                            SyoriFlg = False

                        Case "H"
                            EndFlag = False

                        Case "D"

                        Case "T"

                        Case "E"
                            EndFlag = True

                        Case ""
                            Exit Do

                    End Select

                    If SyoriFlg = True Then

                        '�w�b�_���R�[�h
                        If aReadFmt.IsHeaderRecord() = True Then

                            '�J�E���^������
                            nRecordNumber = 1
                            aReadFmt.InfoMeisaiMast.FILE_SEQ += 1
                            PrnFlag = True

                            ' ��d
                            ArrayMeiData.Clear()
                        End If


                        '���׃}�X�^���ڐݒ� ���R�[�h�ԍ�
                        '�����G���h�̃��R�[�h�͖��׃}�X�^��INSERT���Ȃ�
                        If sCheckRet <> "99" Then
                            aReadFmt.InfoMeisaiMast.RECORD_NO = nRecordNumber
                        End If

                        '�����G���h�̃��R�[�h�͖��׃}�X�^��INSERT���Ȃ�
                        '���׃}�X�^�o�^
                        If sCheckRet <> "99" Then
                            ' ���׃}�X�^�o�^
                            bRet = InsertMEIMAST(aReadFmt)
                            If bRet = False Then
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�� ")
                                BatchLog.UpdateJOBMASTbyErr("MEIMAST�̃C���T�[�g���s " & nRecordCount.ToString & "�s�� ")
                                mErrMessage = "����Ͻ��o�^���s " & nRecordCount.ToString & "�s��"
                                Exit Do
                            End If
                        End If


                        ' �g���[���[���R�[�h
                        If aReadFmt.IsTrailerRecord() = True Or aReadFmt.EOF = True Then

                            If PrnFlag = True Then

                                ' ���׃}�X�^���������C�Q�d�����̌������s��
                                Dim bDupicate As Boolean = False
                                'bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData) '���U�p
                                If bDupicate = True Then
                                    mJobMessage = "��d��������"
                                End If

                                If bDupicate = False Then
                                    ' �Q�d�o�^���́C�C���v�b�g�G���[���o�͂��Ȃ�
                                    ' �C���v�b�g�G���[�b�r�u���o�͂���
                                    If Not PrnInErrList Is Nothing Then
                                        PrnInErrList.CloseCsv()

                                        ' ���폈���̂��ƂɈ�����邽�߁C���X�g�ɕۑ�
                                        ArrayPrnInErrList.Add(PrnInErrList)
                                        PrnInErrList = Nothing
                                    End If
                                End If

                                ' �������ʊm�F�\�b�r�u�t�@�C���쐬(����I�����ɂ��o�͂���)
                                If PrnSyoKekka Is Nothing Then
                                    PrnSyoKekka = New ClsPrnSyorikekkaKakuninhyo
                                    PrnSyoKekka.CreateCsvFile()
                                End If

                                If Not PrnSyoKekka Is Nothing Then
                                    Call PrnSyoKekka.OutputCSVKekka(aReadFmt, mArgumentData)
                                End If

                                '�X�ʏW�v�\����p���X�g�Ɋi�[(����敪,������R�[�h,����敛�R�[�h)
                                ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," & _
                                                 aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," & _
                                                 aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)

                                ' �X�P�W���[���}�X�^���X�V����
                                bRet = aReadFmt.UpdateSCHMAST
                                If bRet = False Then
                                    BatchLog.Write("�X�P�W���[���X�V", "���s")
                                    BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���X�V���s")
                                    Exit Do
                                End If
                            End If

                            PrnFlag = False
                        End If

                    End If

                Loop


                If Not PrnSchErrList Is Nothing Then
                    If PrnSchErrList.ReportExecute() = False Then
                        BatchLog.Write("�X�P�W���[���G���[���X�g", "���s", "�t�@�C����:" & PrnSchErrList.FileName & " ���b�Z�[�W:" & PrnSchErrList.ReportMessage)
                    End If
                    mErrMessage = "�X�P�W���[���G���[�ϑ��҂���"
                End If

                If Not PrnToriErrList Is Nothing Then
                    If PrnToriErrList.ReportExecute() = False Then
                        BatchLog.Write("�����}�X�^�G���[���X�g", "���s", "�t�@�C����:" & PrnToriErrList.FileName & " ���b�Z�[�W:" & PrnToriErrList.ReportMessage)
                    End If
                    mErrMessage = "�����G���[�ϑ��҂���"
                End If

                If SyoriFlg = True Then

                    If bRet = False Then

                        If aReadFmt.IsTrailerRecord() = True Then
                            ' �C���v�b�g�G���[���X�g���������
                            If Not PrnInErrList Is Nothing Then
                                PrnInErrList.CloseCsv()

                                ' ���폈���̂��ƂɈ�����邽�߁C���X�g�ɕۑ�
                                ArrayPrnInErrList.Add(PrnInErrList)
                                PrnInErrList = Nothing
                            End If

                            For i As Integer = 0 To ArrayPrnInErrList.Count - 1
                                PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                                BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "�Ώ�", PrnInErrList.FileName)

                                If PrnInErrList.ReportExecute() = False Then
                                    BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "���s", "�t�@�C����:" & PrnInErrList.FileName & " ���b�Z�[�W:" & PrnInErrList.ReportMessage)
                                End If
                                PrnInErrList = Nothing
                            Next i

                            '�C���v�b�g�G���[�o�͎��̓p�g���C�g�_��
                            If ArrayPrnInErrList.Count > 0 Then
                                Try
                                    Dim ELog As New CASTCommon.ClsEventLOG
                                    ELog.Write("�C���v�b�g�G���[���X�g�o�͂���", EventLogEntryType.Error)
                                Catch ex As Exception
                                End Try
                            End If
                        End If

                        '�C���v�b�g�G���[�o�͎��Ɉُ�I���Ƃ���ꍇ�̓R�����g�����Ȃ� +++
                        'mErrMessage = "�C���v�b�g�G���[���X�g�o�͂���"
                        Return False
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    End If

                    ' �C���v�b�g�G���[���X�g���������
                    For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                        PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                        BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "�Ώ�", PrnInErrList.FileName)

                        If PrnInErrList.ReportExecute() = False Then
                            BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "���s", "�t�@�C����:" & PrnInErrList.FileName & " ���b�Z�[�W:" & PrnInErrList.ReportMessage)
                        End If

                    Next i

                    '�C���v�b�g�G���[�o�͎��̓p�g���C�g�_��
                    If ArrayPrnInErrList.Count > 0 Then
                        Try
                            Dim ELog As New CASTCommon.ClsEventLOG
                            ELog.Write("�C���v�b�g�G���[���X�g�o�͂���", EventLogEntryType.Error)
                        Catch ex As Exception

                        End Try
                    End If

                    '�������݃`�F�b�N
                    If ErrFlag = True Then
                        '�������ʊm�F�\���������
                        BatchLog.Write("�C���v�b�g�`�F�b�N", "���s", ErrText)
                        BatchLog.UpdateJOBMASTbyErr("�C���v�b�g�`�F�b�N�G���[ " & ErrText)
                        mErrMessage = ErrText
                        'BatchLog.JobMastMessage = ErrText
                        Return False
                    End If

                    '�������ʊm�F�\���������
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("�������ʊm�F�\�o��", "���s", "�t�@�C����:" & PrnSyoKekka.FileName & " ���b�Z�[�W:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If

                Else

                    '�X�P�W���[���G���[���X�g�������͎����G���[���X�g���ł��̂�����G���[
                    Return False

                End If

            Catch ex As Exception
                BatchLog.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
                BatchLog.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")
                mErrMessage = "�V�X�e���G���[�i���O�Q�Ɓj"
                Return False
            End Try

            Return True

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�o�^����", "���s", ex.Message & ":" & ex.StackTrace)
            BatchLog.UpdateJOBMASTbyErr("�V�X�e���G���[�i���O�Q�Ɓj")
            mErrMessage = "�V�X�e���G���[�i���O�Q�Ɓj"
            Return False
        Finally
            BatchLog.Write("(���׃}�X�^�o�^)�I��", "����")
        End Try
    End Function

    ' �@�\�@ �F ���׃}�X�^ INSERT�N�G���Z�b�g
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function SetInsertMEIMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim SQL As StringBuilder        ' �r�p�k

        Dim stTori As CAstBatch.CommData.stTORIMAST ' �������
        stTori = aReadFmt.ToriData.INFOToriMast

        Try
            SQL = New System.Text.StringBuilder("INSERT INTO ", 2048)
            SQL.Append(" MEIMAST(")
            SQL.Append(" FSYORI_KBN_K")
            SQL.Append(",TORIS_CODE_K")
            SQL.Append(",TORIF_CODE_K")
            SQL.Append(",FURI_DATE_K")
            SQL.Append(",FURI_CODE_K")
            SQL.Append(",KIGYO_CODE_K")
            SQL.Append(",KIGYO_SEQ_K")
            SQL.Append(",ITAKU_KIN_K")
            SQL.Append(",ITAKU_SIT_K")
            SQL.Append(",ITAKU_KAMOKU_K")
            SQL.Append(",ITAKU_KOUZA_K")
            SQL.Append(",KEIYAKU_KIN_K")
            SQL.Append(",KEIYAKU_SIT_K")
            SQL.Append(",KEIYAKU_NO_K")
            SQL.Append(",KEIYAKU_KNAME_K")
            SQL.Append(",KEIYAKU_KAMOKU_K")
            SQL.Append(",KEIYAKU_KOUZA_K")
            SQL.Append(",FURIKIN_K")
            SQL.Append(",FURIKETU_CODE_K")
            SQL.Append(",FURIKETU_CENTERCODE_K")
            SQL.Append(",SINKI_CODE_K")
            SQL.Append(",NS_KBN_K")
            SQL.Append(",TEKIYO_KBN_K")
            SQL.Append(",KTEKIYO_K")
            SQL.Append(",NTEKIYO_K")
            SQL.Append(",JYUYOUKA_NO_K")
            SQL.Append(",MINASI_K")
            SQL.Append(",TEISEI_SIT_K")
            SQL.Append(",TEISEI_KAMOKU_K")
            SQL.Append(",TEISEI_KOUZA_K")
            SQL.Append(",TEISEI_AKAMOKU_K")
            SQL.Append(",TEISEI_AKOUZA_K")
            SQL.Append(",DATA_KBN_K")
            SQL.Append(",FURI_DATA_K")
            SQL.Append(",RECORD_NO_K")
            SQL.Append(",SORT_KEY_K")
            SQL.Append(",SAKUSEI_DATE_K")
            SQL.Append(",KOUSIN_DATE_K")
            SQL.Append(",YOBI1_K")
            SQL.Append(",YOBI2_K")
            SQL.Append(",YOBI3_K")
            SQL.Append(",YOBI4_K")
            SQL.Append(",YOBI5_K")
            SQL.Append(",YOBI6_K")
            SQL.Append(",YOBI7_K")
            SQL.Append(",YOBI8_K")
            SQL.Append(",YOBI9_K")
            SQL.Append(",YOBI10_K")

            If stTori.FSYORI_KBN_T = "3" Then
                SQL.Append(",CYCLE_NO_K")
            ElseIf aReadFmt.BinMode Then
                SQL.Append(",BIN_DATA_K")
            End If

            SQL.Append(")")
            SQL.Append(" VALUES(")
            SQL.Append(" :FSYORI_KBN")
            SQL.Append(",:TORIS_CODE")                              'TORIS_CODE_K             
            SQL.Append(",:TORIF_CODE")                              'TORIF_CODE_K
            SQL.Append(",:FURI_DATE")                               'FURI_DATE_K
            SQL.Append(",:FURI_CODE")                               'FURI_CODE_K
            SQL.Append(",:KIGYO_CODE")                              'KIGYO_CODE_K
            SQL.Append(",:KIGYO_SEQ")                               'KIGYO_SEQ_K
            SQL.Append(",:ITAKU_KIN")                               'ITAKU_KIN_K
            SQL.Append(",:ITAKU_SIT")                               'ITAKU_SIT_K
            SQL.Append(",:ITAKU_KAMOKU")                            'ITAKU_KAMOKU_K
            SQL.Append(",:ITAKU_KOUZA")                             'ITAKU_KOUZA_K
            SQL.Append(",:KEIYAKU_KIN")                             'KEIYAKU_KIN_K
            SQL.Append(",:KEIYAKU_SIT")                             'KEIYAKU_SIT_K
            SQL.Append(",:KEIYAKU_NO")                              'KEIYAKU_NO_K
            SQL.Append(",:KEIYAKU_KNAME")                           'KEIYAKU_KNAME_K
            SQL.Append(",:KEIYAKU_KAMOKU")                          'KEIYAKU_KAMOKU_K
            SQL.Append(",:KEIYAKU_KOUZA")                           'KEIYAKU_KOUZA_K
            SQL.Append(",:FURIKIN")                                 'FURIKIN_K
            SQL.Append(",:FURIKETU_CODE")                           'FURIKETU_CODE_K 
            SQL.Append(",:FURIKETU_CENTERCODE")                     'FURIKETU_SENTERCODE_K
            SQL.Append(",:SINKI_CODE")                              'SINKI_CODE_K
            SQL.Append(",:NS_KBN")                                  'NS_KBN_K
            SQL.Append(",:TEKIYOU_KBN")                             'TEKIYO_KBN_K
            SQL.Append(",:KTEKIYO")                                 'KTEKIYO_K
            SQL.Append(",:NTEKIYOU")                                'NTEKIYO_K
            SQL.Append(",:JYUYOKA_NO")                              'JYUYOUKA_NO_K
            SQL.Append(",'0'")                                      'MINASI_K
            SQL.Append(",:TEISEI_SIT")                              'TEISEI_SIT_K
            SQL.Append(",:TEISEI_KAMOKU")                           'TEISEI_KAMOKU_K
            SQL.Append(",:TEISEI_KOUZA")                            'TEISEI_KOUZA_K
            SQL.Append(",'00'")                                     'TEISEI_AKAMOKU_K
            SQL.Append(",'0000000'")                                'TEISEI_AKOUZA_K
            SQL.Append(",:DATA_KBN")                                'DATA_KBN_K
            SQL.Append(",:FURI_DATA")                               'FURI_DATA_K
            SQL.Append(",:RECORD_NO")                               'RECORD_NO_K
            SQL.Append(",' '")                                      'SORT_KEY_K
            SQL.Append(",:SAKUSEI_DATE")                            'SAKUSEI_DATE_K
            SQL.Append(",'00000000'")                               'KOUSIN_DATE_K
            SQL.Append(",:YOBI1")                                   'YOBI1_K
            SQL.Append(",:YOBI2")                                   'YOBI2_K
            SQL.Append(",:YOBI3")                                   'YOBI3_K
            SQL.Append(",' '")                                      'YOBI4_K
            SQL.Append(",' '")                                      'YOBI5_K
            SQL.Append(",' '")                                      'YOBI6_K
            SQL.Append(",' '")                                      'YOBI7_K
            SQL.Append(",' '")                                      'YOBI8_K
            SQL.Append(",' '")                                      'YOBI9_K
            SQL.Append(",' '")                                      'YOBI10_K

            If aReadFmt.BinMode Then
                '���U����EBCDIC�̏ꍇ
                SQL.Append(",:BIN")                                 'BIN_DATA_K
                SQL.Append(")")
            Else
                '���U�̏ꍇ
                SQL.Append(")")
            End If

            MainDB.CommandText = SQL.ToString

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�o�^�N�G������", "���s", ex.Message)
            Return False
        End Try

        Return True
    End Function

    ' �@�\�@ �F ���׃}�X�^ INSERT
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertMEIMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' �������
        Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' ���׃}�X�^���

        Try
            With MainDB
                .Item("FSYORI_KBN") = stTori.FSYORI_KBN_T
                .Item("TORIS_CODE") = stTori.TORIS_CODE_T
                .Item("TORIF_CODE") = stTori.TORIF_CODE_T
                .Item("FURI_DATE") = stMei.FURIKAE_DATE
                .Item("FURI_CODE") = stTori.FURI_CODE_T
                .Item("KIGYO_CODE") = stTori.KIGYO_CODE_T
                .Item("KIGYO_SEQ") = stMei.KIGYO_SEQ
                .Item("ITAKU_KIN") = stMei.ITAKU_KIN
                .Item("ITAKU_SIT") = stMei.ITAKU_SIT
                .Item("ITAKU_KAMOKU") = stMei.ITAKU_KAMOKU
                .Item("ITAKU_KOUZA") = stMei.ITAKU_KOUZA
                .Item("KEIYAKU_KIN") = stMei.KEIYAKU_KIN
                .Item("KEIYAKU_SIT") = stMei.KEIYAKU_SIT
                .Item("KEIYAKU_NO") = stMei.KEIYAKU_NO
                .Item("KEIYAKU_KNAME") = stMei.KEIYAKU_KNAME
                .Item("KEIYAKU_KAMOKU") = CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)
                .Item("KEIYAKU_KOUZA") = stMei.KEIYAKU_KOUZA
                .Item("FURIKIN") = stMei.FURIKIN
                .Item("FURIKETU_CODE") = stMei.FURIKETU_CODE
                .Item("FURIKETU_CENTERCODE") = stMei.FURIKETU_CENTERCODE
                .Item("SINKI_CODE") = stMei.SINKI_CODE
                .Item("NS_KBN") = stTori.NS_KBN_T
                .Item("TEKIYOU_KBN") = stTori.TEKIYOU_KBN_T
                .Item("KTEKIYO") = stMei.KTEKIYO
                .Item("NTEKIYOU") = stTori.NTEKIYOU_T
                .Item("JYUYOKA_NO") = stMei.JYUYOKA_NO
                .Item("TEISEI_SIT") = stMei.TEISEI_SIT
                .Item("TEISEI_KAMOKU") = stMei.TEISEI_KAMOKU
                .Item("TEISEI_KOUZA") = stMei.TEISEI_KOUZA
                .Item("DATA_KBN") = stMei.DATA_KBN
                '2010.06.08 oni �Z���^�[���ڎ������݂͕ϊ���(NULL�΍�)���Z�b�g start
                .Item("FURI_DATA") = aReadFmt.RecordData
                '.Item("FURI_DATA") = aReadFmt.RecordDataNoChange
                '2010.06.08 oni �Z���^�[���ڎ������݂͕ϊ���(NULL�΍�)���Z�b�g end
                .Item("RECORD_NO") = stMei.RECORD_NO.ToString
                .Item("SAKUSEI_DATE") = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
                .Item("YOBI1") = stMei.YOBI1
                .Item("YOBI2") = stMei.YOBI2
                .Item("YOBI3") = stMei.YOBI3

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("���׃}�X�^�o�^", "���s", MainDB.Message)
                    Return False
                End If

                '�N���̏ꍇ�́CNENKINMAST �ւ�INSERT ����
                'If aReadFmt.ToriData.INFOParameter.FMT_KBN = "03" Then
                '    If InsertNENKINMAST(aReadFmt) = False Then
                '        Return False
                '    End If
                'End If

            End With

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�o�^", "���s", ex.Message)
            Return False
        End Try

        Return True
    End Function

    ' �@�\�@ �F ���׃}�X�^��ƃV�[�P���X UPDATE
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F �Z���^�[���ڎ����t�H�[�}�b�g���Ńf�[�^�p
    '
    Private Function UpdateKigyoSEQ(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' �������
        Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' ���׃}�X�^���

        '�f�[�^���R�[�h�̂ݍX�V����
        If stMei.DATA_KBN <> "2" Then
            Return True
        End If

        Try
            Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
            SQL.Append("UPDATE MEIMAST SET")
            SQL.Append(" KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
            SQL.Append(", KOUSIN_DATE_K = " & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))
            SQL.Append(" WHERE FSYORI_KBN_K = '1'")
            SQL.Append(" AND TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
            SQL.Append(" AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
            SQL.Append(" AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
            SQL.Append(" AND DATA_KBN_K = '3'")
            SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
            SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
            SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
            SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
            SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
            '�S�p�����Ή�
            SQL.Append(" AND SUBSTRB(FURI_DATA_K, 4, 5) = " & SQ(stMei.JYUYOKA_NO.Substring(0, 5))) '�Ǐ��ԍ�
            SQL.Append(" AND SUBSTRB(FURI_DATA_K, 53, 8) = " & SQ(stMei.JYUYOKA_NO.Substring(5, 8))) ' �����ԍ�

            '��ӂ̖��ׂ��X�V�o���Ȃ���΃G���[�Ƃ���
            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                BatchLog.Write("���׃}�X�^�i��ƃV�[�P���X�j�X�V", "���s", "���׍X�V��������ӂłȂ� " & MainDB.Message)
                Return False
            End If

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�i��ƃV�[�P���X�j�X�V", "���s", ex.Message)
            Return False
        End Try

        Return True
    End Function


    ' �@�\�@ �F �Z���^�[���ڎ������݁@�����擾
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Sub GetCenterTORIMAST(ByVal furiCode As String, ByVal kigyoCode As String)
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        SQL.Append("SELECT ")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE FURI_CODE_T = " & SQ(furiCode))
        SQL.Append("   AND KIGYO_CODE_T = " & SQ(kigyoCode))
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                mKoParam.CP.TORIS_CODE = OraReader.GetString("TORIS_CODE_T")
                mKoParam.CP.TORIF_CODE = OraReader.GetString("TORIF_CODE_T")
                mKoParam.CP.TORI_CODE = mKoParam.CP.TORIS_CODE & mKoParam.CP.TORIF_CODE
            End If
        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try
    End Sub

    ' �@�\�@ �F �ʑO����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Sub KobetuExecute(ByVal fileName As String, ByVal key As CommData)
        Dim StrReader As StreamReader = Nothing

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        Try
            Dim MaeSyoriKey As String
            '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            'Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            Dim SQL As New StringBuilder(128)
            Dim TxtName As String

            SQL.Append("SELECT MAE_SYORI_T FROM TORIMAST WHERE")
            SQL.Append("   �@TORIS_CODE_T = " & SQ(key.INFOToriMast.TORIS_CODE_T))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(key.INFOToriMast.TORIF_CODE_T))

            '������Ȃ��ꍇ
            If OraReader.DataReader(SQL) = False Then
                BatchLog.Write("�ʑO���� �����}�X�^�o�^�Ȃ�", "����", "�����R�[�h�F" & _
                           key.INFOToriMast.TORIS_CODE_T & "-" & key.INFOToriMast.TORIF_CODE_T)
                
                Return
            End If

            '�o�^���Ă��Ȃ��ꍇ
            If OraReader.GetItem("MAE_SYORI_T").Trim = "" Then
                BatchLog.Write("�ʑO���� �o�^�Ȃ�", "����", "")
                Return
            End If

            '�ʑO�����ԍ��擾
            MaeSyoriKey = OraReader.GetString("MAE_SYORI_T")
            OraReader.Close()

            TxtName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "134_MAE_SYORI_T.TXT")
            StrReader = New StreamReader(TxtName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim Line As String = StrReader.ReadLine()

            Do Until Line = Nothing
                Dim pos As Integer = Line.IndexOf(","c)

                If pos >= 0 Then
                    If CAInt32(Line.Substring(0, pos).Trim).ToString("00") = MaeSyoriKey Then
                        ' ��v����ԍ���������

                        If pos >= 0 AndAlso Line.Length >= pos Then
                            pos = Line.IndexOf(","c, pos + 1)

                            Dim ParaPos As Integer = Line.ToUpper.IndexOf(".EXE", pos + 1)
                            If ParaPos < 0 Then
                                BatchLog.Write("�ʑO���� �d�w�d�o�^�Ȃ�", "����", "")
                                Exit Sub
                            End If

                            Dim Exe As String = Line.Substring(pos + 1, ParaPos - pos + 3).Trim
                            Exe = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), Exe)

                            Dim Para As String = ""
                            If ParaPos >= 0 AndAlso Line.Length >= ParaPos Then
                                Para = Line.Substring(ParaPos + 4).Trim
                            End If

                            ' �ʑO���� �N��
                            Dim ProcInfo As New ProcessStartInfo
                            ProcInfo.FileName = Exe

                            If File.Exists(ProcInfo.FileName) = True Then

                                Dim OutPath As String = CASTCommon.GetFSKJIni("OTHERSYS", "MAESYORI")
                                If OutPath <> "err" Then
                                    If Directory.Exists(OutPath) = False Then
                                        Directory.CreateDirectory(OutPath)
                                    End If
                                End If

                                If Directory.Exists(OutPath) = False Then
                                    BatchLog.Write("�ʑO���� �A�g�t�H���_�Ȃ�", "���s", OutPath)
                                    Return
                                End If

                                Dim OutName As String = Path.GetFileName(Exe) & "_" & Para & "."
                                OutName = Path.Combine(OutPath, OutName)

                                For i As Integer = 1 To 999
                                    Try
                                        ' �t�@�C�����쐬�ł���܂ŌJ��Ԃ�
                                        If File.Exists(OutName & i.ToString("000")) = False Then
                                            File.Copy(fileName, OutName & i.ToString("000"), False)
                                            Exit For
                                        Else
                                            Threading.Thread.Sleep(100)
                                        End If
                                    Catch ex As Exception

                                    End Try
                                Next i

                                ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
                                ProcInfo.Arguments = Para
                                Process.Start(ProcInfo)
                                BatchLog.Write("�ʑO���� ���s", "����")
                            Else
                                BatchLog.Write("�ʑO���� ���s���W���[���Ȃ�", "���s", Exe)

                            End If

                            ' �ʏ��������s�����ł̔�����
                            Return
                        End If
                        BatchLog.Write("�ʑO���� ���s���W���[���̎w��Ȃ�", "���s")

                        Exit Do
                    End If

                    Line = StrReader.ReadLine()
                End If
            Loop
            BatchLog.Write("�ʑO���� TXT��v�f�[�^�Ȃ�", "����", MaeSyoriKey)


            StrReader.Close()
            StrReader = Nothing
        Catch ex As Exception
            BatchLog.Write("�ʑO����", "���s", ex.Message)
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            If Not StrReader Is Nothing Then
                StrReader.Close()
            End If
        End Try
    End Sub

    ' �@�\�@ �F �b�n�l�o�k�n�b�j���g�p���ăt�@�C�����Í���
    '
    ' ���l�@ �F 
    '
    Private Function EncodeComplock(ByVal complock As stCOMPLOCK, ByVal infile As String, ByVal outfile As String) As Long
        Dim Arg As String
        Dim intIDPROCESS As Integer = Nothing

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            mErrMessage = "COMPLOCK�v���O������������܂���"
            Return -100
        End If

        ' �����g�ݗ���
        Dim DQ As String = """"
        Arg = "-I "
        Arg &= DQ & infile & DQ
        'Arg &= " -l " & DQ & complock.RECLEN & DQ
        Arg &= " -O " & DQ & outfile & DQ ' �o�͐�
        'If complock.AES = "0" Then
        '    ' �`�d�r�Ȃ�
        '    Arg &= " -a 5"
        '    Arg &= " -n 256"
        'Else
        '    ' �`�d�r
        '    Arg &= " -a  8"             ' -rl ���w�肵�Ȃ��ꍇ��, -a 6 �ƂȂ�
        '    Arg &= " -m  1 "
        '    Arg &= " -ak 1 "
        '    Arg &= " -p  1"
        'End If
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' ��
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -rl " & DQ & complock.RECLEN & DQ
        Arg &= " -lf 0"
        'Arg &= " -t 0"
        'Arg &= " -g 1"

        ' ���������s
        Dim ProcFT As New Process
        Try
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(sComplockPath, "decode.exe")
            ProcInfo.Arguments = Arg
            ProcInfo.WorkingDirectory = sComplockPath
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            'If ProcFT.ExitCode <> 0 Then
            '    Return ProcFT.ExitCode
            'End If
        Catch ex As Exception
            mErrMessage = ex.Message
            Return -1
        End Try

        Return ProcFT.ExitCode

    End Function

    Private Function GetKeyInfo() As Boolean

        Complock.AES = GetFSKJIni("CENTER", "AES")
        Complock.KEY = GetFSKJIni("CENTER", "KEY")
        Complock.IVKEY = GetFSKJIni("CENTER", "IVKEY")
        Complock.RECLEN = GetFSKJIni("CENTER", "RECLEN")

    End Function

    ''' <summary>
    ''' ���ɓn���t�@�C�����o�b�`SV����DEN�t�H���_�ɃR�s�[����
    ''' </summary>
    ''' <param name="CMTFileName"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/01/23 saitou ���t�M��(RSV2�W��) added for ��e�ʃf���o���Ή�</remarks>
    Private Function fn_CopyBatchSVToDen(ByVal CMTFileName As String) As Boolean

        Dim ret As Boolean = False

        Try
            '�R�s�[���t�@�C����
            Dim SrcFileName As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "CENTER_FILE")
            If SrcFileName.ToUpper = "ERR" Then
                BatchLog.UpdateJOBMASTbyErr("���ɓn���t�@�C���擾 ���s ")
                BatchLog.Write("���ɓn���t�H���_�擾", "���s", "[RSV2_V1.0.0].CENTER_FILE")
                mErrMessage = "���ɓn���t�@�C���擾 ���s [RSV2_V1.0.0].CENTER_FILE"
                Exit Try
            End If

            '�t�@�C�����ɋ��ɃR�[�h�A�o�^��������ꍇ�̍l��
            SrcFileName = Format(SrcFileName.Replace("{0}", CASTCommon.GetFSKJIni("COMMON", "KINKOCD")))
            SrcFileName = Format(SrcFileName.Replace("{1}", mKoParam.CP.FURI_DATE))

            If Not File.Exists(SrcFileName) Then
                BatchLog.UpdateJOBMASTbyErr("���ɓn���t�@�C���擾 ���s �t�@�C�������F" & SrcFileName)
                BatchLog.Write("���ɓn���t�@�C���擾", "���s", "�t�@�C�������F" & SrcFileName)
                mErrMessage = "���ɓn���t�@�C���擾 ���s �t�@�C�������F" & SrcFileName
                Exit Try
            End If

            Dim FpExePath As String = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If FpExePath.ToUpper = "ERR" Then
                BatchLog.UpdateJOBMASTbyErr("���ɓn���t�@�C���擾 ���s ")
                BatchLog.Write("���ɓn���t�@�C���擾", "���s", "[COMMON].FTRANP")
                mErrMessage = "���ɓn���t�@�C���擾 ���s [COMMON].FTRANP"
                Exit Try
            End If

            '�R�s�[��t�@�C����
            Dim DestFileName As String = CMTFileName

            '�o�b�`�T�[�o����DEN�t�H���_�ɃR�s�[
            Dim Proc As Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(FpExePath, "FP")
            '�p�����[�^
            ProcInfo.Arguments = " /nwd/ putrand " & SrcFileName & " " & DestFileName
            ProcInfo.Arguments &= " /size 644 /isize 640 /map @4 binary 640:640 "

            ProcInfo.WorkingDirectory = FpExePath

            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()

            If Proc.ExitCode = 0 Then
                ret = True
            End If

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("���ɓn���t�@�C���擾 ���s")
            BatchLog.Write("���ɓn���t�@�C���擾", "���s", ex.Message)
            mErrMessage = "���ɓn���t�@�C���擾 ���s "
            Exit Try
        End Try

        Return ret

    End Function

End Class
