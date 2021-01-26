Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CAstFormat
'*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

' �ʗ������ݏ���
Public Class ClsTouroku

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
                CP.TORI_CODE = para(0)                      '�����R�[�h
                CP.FURI_DATE = para(1)                      '�U����
                CP.CODE_KBN = para(2)                       '�R�[�h�敪
                CP.FMT_KBN = para(3).PadLeft(2, "0"c)       '�t�H�[�}�b�g�敪
                CP.BAITAI_CODE = para(4).PadLeft(2, "0"c)   '�}�̃R�[�h
                CP.LABEL_KBN = para(5)                      '���x���敪

                Select Case para.Length
                    Case 7
                        CP.RENKEI_FILENAME = ""                  ' �A�g�t�@�C����
                        CP.ENC_KBN = ""                          ' �Í��������敪
                        CP.ENC_KEY1 = ""                         ' �Í����L�[�P
                        CP.ENC_KEY2 = ""                         ' �Í����L�[�Q
                        CP.ENC_OPT1 = ""                         ' �`�d�r�I�v�V����
                        CP.CYCLENO = ""                          ' �T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(6))    '�W���u�ʔ�
                    Case 8
                        CP.RENKEI_FILENAME = para(6).TrimEnd     ' �A�g�t�@�C����
                        CP.ENC_KBN = ""                          ' �Í��������敪
                        CP.ENC_KEY1 = ""                         ' �Í����L�[�P
                        CP.ENC_KEY2 = ""                         ' �Í����L�[�Q
                        CP.ENC_OPT1 = ""                         ' �`�d�r�I�v�V����
                        CP.CYCLENO = ""                          ' �T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(7))    ' �W���u�ʔ�
                    Case 9
                        CP.RENKEI_FILENAME = ""                  ' �A�g�t�@�C����
                        CP.ENC_KBN = ""                          ' �Í��������敪
                        CP.ENC_KEY1 = ""                         ' �Í����L�[�P
                        CP.ENC_KEY2 = ""                         ' �Í����L�[�Q
                        CP.ENC_OPT1 = ""                         ' �`�d�r�I�v�V����
                        CP.CYCLENO = ""                          ' �T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(8))    ' �W���u�ʔ�
                    Case 13
                        CP.RENKEI_FILENAME = para(6)             ' �A�g�t�@�C����
                        CP.ENC_KBN = para(7)                     ' �Í��������敪
                        CP.ENC_KEY1 = para(8)                    ' �Í����L�[�P
                        CP.ENC_KEY2 = para(9)                    ' �Í����L�[�Q
                        CP.ENC_OPT1 = para(10)                   ' �`�d�r�I�v�V����
                        CP.CYCLENO = para(11)                    ' �T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(12))   ' �W���u�ʔ�
                End Select
            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    Dim mUserID As String = ""                      '���[�U�h�c
    Dim mJobMessage As String = ""                  '�W���u���b�Z�[�W
    Private mDataFileName As String                 '�˗��f�[�^�t�@�C����
    Private mArgumentData As CommData               '�N���p�����[�^ ���ʏ��
    Public MainDB As CASTCommon.MyOracle            '�p�u���b�N�c�a
    Private mErrMessage As String = ""              '�G���[���b�Z�[�W(�������ʊm�F�\����p)
    Private ArrayTenbetu As New ArrayList           '�X�ʏW�v�\�o�͑Ώ� �i�[���X�g

    ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Private INI_RSV2_EDITION As String = ""
    ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
    Dim INI_RSV2_S_SYORIKEKKA_TOUROKU As String     ' �������ʊm�F�\����v��
    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
    '���������`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Protected INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
    '��t���ԃ`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Protected INI_UKETUKE_JIKAN_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN_CHK")
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

    ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
    Private dblock As CASTCommon.CDBLock = Nothing
    Private LockWaitTime As Integer = 600
    ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

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
            BatchLog.Write("", "0000000000-00", "00000000", "(�p�����[�^�擾)�J�n", "����", "")
            BatchLog.JobTuuban = mKoParam.CP.JOBTUUBAN
            BatchLog.ToriCode = mKoParam.CP.TORI_CODE
            BatchLog.FuriDate = mKoParam.CP.FURI_DATE

        Catch ex As Exception
            BatchLog.Write("(�p�����[�^�擾)", "���s", "�p�����[�^�擾���s[" & command & "]�F" & ex.Message)
            Return 1
        End Try

        Try
            ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            '--------------------------------------------
            ' INI̧�ُ��ݒ�
            '--------------------------------------------
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

            ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
            INI_RSV2_S_SYORIKEKKA_TOUROKU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SYORIKEKKA_TOUROKU")
            ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

            '--------------------------------------------
            '�p�����[�^���ݒ�
            '--------------------------------------------
            Dim bRet As Boolean                         '��������
            Dim InfoParam As New CommData.stPARAMETER   '�p�����[�^���

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
            ' 2016/10/17 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            If INI_RSV2_EDITION = "2" Then
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("�又��", "���s", "�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        BatchLog.UpdateJOBMASTbyErr("�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        Return -1
                    End If
                End If
            End If
            ' 2016/10/17 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

            bRet = TourokuMain()

            '--------------------------------------------
            '��������E���[�o��
            '--------------------------------------------
            If bRet = False Then
                ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("�又��", "���s", "�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        BatchLog.UpdateJOBMASTbyErr("�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        Return -1
                    End If
                End If
                ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
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
                '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                Dim IRAI_BK_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_BKUP_MODE_S")
                '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                Try
                    Select Case InfoParam.BAITAI_CODE
                        ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
                        'Case "00"
                        Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END
                            '���V�X�e���A�g����N�����ꂽ�ꍇ���l������
                            Dim R_FILENAME As String = Path.Combine(Path.GetDirectoryName(mKoParam.CP.RENKEI_FILENAME), InfoParam.RENKEI_FILENAME)

                            If File.Exists(R_FILENAME) = True Then
                                '���V�X�e���A�g�̏ꍇ�̃t�@�C������
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(R_FILENAME)

                                '�O��t�@�C�����폜
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If

                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(R_FILENAME, DestFile, True)
                                    File.Delete(R_FILENAME)
                                Else
                                    File.Move(R_FILENAME, DestFile)
                                End If
                                'File.Move(R_FILENAME, DestFile)
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END

                            Else
                                '�ʏ�̏ꍇ�̃t�@�C������
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                                '�O��t�@�C�����폜
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If

                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END

                            End If

                            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                        Case "01", "11", "12", "13", "14", "15"
                            'Case "01"
                            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<

                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '�O��t�@�C�����폜
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                            If IRAI_BK_MODE = "1" Then
                                File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                File.Delete(mKoParam.CP.RENKEI_FILENAME)
                            Else
                                File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END

                        Case "04", "09" '(�˗����E�`�[��ǉ�)
                            ' 2016/01/27 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
                            ''�O��t�@�C�����폜
                            'If File.Exists(DestFile) = True Then
                            '    File.Delete(DestFile)
                            'End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            ''2009/12/09 �ǉ� ================================
                            '�O��t�@�C�����폜
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                    If File.Exists(DestFile) = True Then
                                        File.Delete(DestFile)
                                    End If
                                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                    If IRAI_BK_MODE = "1" Then
                                        File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                        File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                    Else
                                        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    End If
                                    'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                                Case Else
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
                                    If File.Exists(DestFile) = True Then
                                        File.Delete(DestFile)
                                    End If
                                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                    If IRAI_BK_MODE = "1" Then
                                        File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                        File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                    Else
                                        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    End If
                                    'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                            End Select
                            ' 2016/01/27 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                        Case "05"
                            If CASTCommon.GetFSKJIni("COMMON", "MT") = "1" Then '���ڑ�
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                '�O��t�@�C�����폜
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                            End If
                        Case "06"
                            If CASTCommon.GetFSKJIni("COMMON", "CMT") = "1" Then '���ڑ�
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                '�O��t�@�C�����폜
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                            End If
                            '===============================================
                            '2012/06/30 �W���Ł@WEB�`���Ή�
                        Case "10"
                            DestFile = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV_BK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '�O��t�@�C�����폜
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                            If IRAI_BK_MODE = "1" Then
                                File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                File.Delete(mKoParam.CP.RENKEI_FILENAME)
                            Else
                                File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                    End Select
                Catch ex As Exception
                    BatchLog.Write("����t�H���_�ړ�", "���s", mKoParam.CP.RENKEI_FILENAME & " -> " & DestFile)
                End Try
            End If

            MainDB.Close()

            If bRet = True Then
                '���U�W���œX�ʏW�v�\�͖��Ή�
                '�����U���X�ʏW�v�\
                'Dim ExeRepo As New CAstReports.ClsExecute '���|�G�[�W�F���g���
                'For i As Integer = 0 To ArrayTenbetu.Count - 1
                '    Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                '    Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
                '    Dim nRet As Integer = ExeRepo.ExecReport("KFSPxxx.EXE", Param)
                '    If nRet <> 0 Then
                '        BatchLog.Write("�����U���X�ʏW�v�\�o��", "���s", "���A�l�F" & nRet)
                '    End If
                'Next
                ' 2016/01/28 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                Dim readFmt As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                Dim ToriCode(ArrayTenbetu.Count - 1) As String
                For i As Integer = 0 To ArrayTenbetu.Count - 1
                    Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                    ToriCode(i) = prnTenbetu(1) & prnTenbetu(2)
                Next

                If readFmt.CallTourokuExit(ToriCode, mArgumentData.INFOParameter.FURI_DATE) = False Then
                    BatchLog.Write("�������ݗp�o�^�o�����\�b�h����", "���s", "")
                End If
                readFmt.Close()
                ' 2016/01/28 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
            End If
            BatchLog.Write("�I��", "����", bRet.ToString)

            If bRet = False Then
                Return 2
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write("(�������݃��C������)", "���s", ex.Message)
        Finally
            ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            ' �W���u���s�A�����b�N
            System.Threading.Thread.Sleep(100)
            dblock.Job_UnLock(MainDB)
            ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
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
        Try
            BatchLog.Write("(�o�^���C������)�J�n", "����")

            '--------------------------------------------
            '�������擾
            '--------------------------------------------
            Try
                '�������擾
                Call mArgumentData.SelectTORIMAST("3", mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE)

                If mArgumentData.INFOToriMast.EOF = True Then
                    Dim line1 As String = ""

                    Try
                        Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
                        Dim readFmt As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)

                        '�e�t�H�[�}�b�g���Ƃɒl���擾�^�ݒ�
                        If readFmt.FirstRead(oRenkei.InFileName) = 1 Then
                            readFmt.GetFileData()
                            line1 = readFmt.RecordData
                            readFmt.Close()
                        Else
                            BatchLog.Write(readFmt.Message, "���s", oRenkei.InFileName)

                            readFmt.Close()

                            Dim ReadF As New FileStream(oRenkei.InFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                            Dim Arr(100) As Byte

                            ReadF.Read(Arr, 0, 100)
                            line1 = Encoding.GetEncoding("SHIFT-JIS").GetString(Arr)
                            ReadF.Close()
                        End If

                    Catch ex As Exception
                        line1 = ex.Message
                    End Try
                    BatchLog.Write("(�o�^���C������)", "���s", "�������擾�F" & line1)
                    BatchLog.UpdateJOBMASTbyErr("�������擾���s " & line1)
                    mErrMessage = "�������擾���s"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(�o�^���C������)", "���s", ex.Message)
                BatchLog.UpdateJOBMASTbyErr("�������擾���s")
                mErrMessage = "�������擾���s"
                Return False
            End Try

            '--------------------------------------------
            ' �p�����[�^����ݒ�
            '--------------------------------------------
            Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter

            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            mArgumentData.INFOParameter = InfoParam

            '�p�����[�^�̃`�F�b�N�i�N���t�H�[�}�b�g�ȊO�j
            If mKoParam.CP.FMT_KBN <> "03" Then
                If CheckParameter() = False Then
                    Return False
                End If
            End If

            '--------------------------------------------
            '�}�̓ǂݍ��݁i�t�H�[�}�b�g�@�j
            '--------------------------------------------
            Dim oReadFMT As CAstFormat.CFormat
            Dim sReadFile As String = ""

            Try
                '�t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
                oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)

                If oReadFMT Is Nothing Then
                    BatchLog.Write("(�o�^���C������)", "���s", "�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j")
                    '*** Str Add 2016/3/5 sys)mori for ���b�Z�[�W�C�� ***
                    BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾���s�i�K��O�t�H�[�}�b�g�j")
                    'BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾�i�K��O�t�H�[�}�b�g�j")
                    '*** end Add 2016/3/5 sys)mori for ���b�Z�[�W�C�� ***
                    mErrMessage = "�t�H�[�}�b�g�擾���s"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(�o�^���C������)", "���s", "�t�H�[�}�b�g�擾�F" & ex.Message)
                '*** Str Add 2016/3/5 sys)mori for ���b�Z�[�W�C�� ***
                BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾���s")
                'BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�擾")
                '*** end Add 2016/3/5 sys)mori for ���b�Z�[�W�C�� ***
                mErrMessage = "�t�H�[�}�b�g�擾���s"
                Return False
            End Try

            'SJIS ���s�Ȃ��t�@�C���ɕϊ����ēǂݍ���(�}�̓Ǎ��̎��s)
            sReadFile = ReadMediaData(oReadFMT)
            ' 2016/05/16 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�_�򕌋@�\�����R��) -------------------- START
            'If sReadFile = "" Then
            '    oReadFMT.Close()
            '    BatchLog.Write("(�o�^���C������)", "���s", "�}�̓ǂݍ��݁F" & oReadFMT.Message)
            '    Return False
            'End If
            If sReadFile = "" Then
                oReadFMT.Close()
                BatchLog.Write("(�o�^���C������)", "���s", "�}�̓ǂݍ��݁F" & oReadFMT.Message)
                Return False
            Else
                '--------------------------------------------
                ' �U���˗��f�[�^(���ԃt�@�C��)�̑ޔ�
                '--------------------------------------------
                Dim BKUP_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_CHECK_IN")
                If BKUP_CHECK <> "err" Then
                    If BKUP_CHECK = "1" Then
                        Dim BKUP_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_BAITAI_IN")

                        If BKUP_BAITAI.IndexOf(mKoParam.CP.BAITAI_CODE) >= 0 Then
                            Dim BKUP_FOLDER As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_FOLDER_IN")
                            Dim BKUP_FILENAME As String = BKUP_FOLDER & Path.GetFileNameWithoutExtension(sReadFile) & "_" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & ".DAT"

                            Try
                                File.Copy(sReadFile, BKUP_FILENAME, True)
                            Catch ex As Exception
                                BatchLog.Write("(�o�^���C������)", "���s", "�U���˗��f�[�^�ޔ��F" & ex.Message)
                            End Try
                        End If
                    End If
                End If
            End If
            ' 2016/05/16 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�_�򕌋@�\�����R��) -------------------- END

            '--------------------------------------------
            '�f�[�^�`�F�b�N
            '--------------------------------------------
            '�`���C�e�c�C�l�s�C�b�l�s
            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
            Select Case mKoParam.CP.BAITAI_CODE
                ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
                'Case "00", "01", "05", "06", "04", "09", "10", "SA", "11", "12", "13", "14", "15"
                Case "00", "01", "05", "06", "04", "09", "10", "SA", "11", "12", "13", "14", "15", _
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END
                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    '2018/03/05 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                    If mKoParam.CP.CODE_KBN = "2" OrElse mKoParam.CP.CODE_KBN = "3" Then
                        '���ԃt�@�C���̃��R�[�h����120�o�C�g�Ȃ̂ŉ��s�Ȃ��̃t�H�[�}�b�g�ŊJ���Ȃ���
                        oReadFMT = New CAstFormat.CFormatFurikomi
                    End If
                    '2018/03/05 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END

                    oReadFMT.FirstRead(sReadFile)

                    Do Until oReadFMT.EOF
                        nLine += 1

                        '1���R�[�h�@�f�[�^�擾
                        If oReadFMT.GetFileData() = 0 Then
                        End If

                        '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                        Try
                            '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

                            '�K�蕶���`�F�b�N
                            nPos = oReadFMT.CheckRegularString()

                            '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                        Catch ex As Exception
                            oReadFMT.Close()
                            BatchLog.Write_Err("(�o�^���C������)", "���s", ex)
                            BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�ϊ����s")
                            mErrMessage = "�t�H�[�}�b�g�ϊ����s"
                            Return False
                        End Try
                        '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

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

                Case Else

            End Select

            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "04" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "09" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "10" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "SA" Then '�}��"04","09","SA"�ǉ�
            '    '2012/06/30 �W���Ł@WEB�`���Ή��@�}��"10"�ǉ�

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
            '                                nLine.ToString & "�s�ڂ�" & (nPos + 1).ToString & "�o�C�g�ڂɕs���ȕ����������Ă��܂�")

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
            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<

            '--------------------------------------------
            ' ���׃}�X�^�o�^����
            '--------------------------------------------
            If MakingMeiMast(oReadFMT) = False Then
                oReadFMT.Close()

                If File.Exists(sReadFile) = True Then
                    File.Delete(sReadFile)
                End If

                ' ���[���o�b�N
                MainDB.Rollback()
                Return False
            End If

            '2010/02/18 �˗����̏ꍇ�A�Ή���������̈˗����̐V�K�R�[�h��1����0�ɍX�V����
            If mKoParam.CP.BAITAI_CODE = "04" Then
                If UpdateENTMAST() = False Then
                    oReadFMT.Close()
                    ' ���[���o�b�N
                    MainDB.Rollback()
                    Return False
                End If
            End If

            oReadFMT.Close()
            If File.Exists(sReadFile) = True Then
                File.Delete(sReadFile)
            End If

            ' JOB�}�X�^�X�V
            If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
                ' ���[���o�b�N
                MainDB.Rollback()
                Return False
            End If

            ' �c�a�R�~�b�g
            MainDB.Commit()

            Return True

        Catch ex As Exception
            BatchLog.Write("(�o�^���C������)", "���s", ex.Message)
            Return False
        Finally
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

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
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
            '�ĐU�̏ꍇ�̓`�F�b�N���Ȃ� 2009/10/21 �ǉ�
            If mKoParam.CP.BAITAI_CODE <> mArgumentData.INFOToriMast.BAITAI_CODE_T _
                 AndAlso mKoParam.CP.BAITAI_CODE <> "SA" Then
                BatchLog.Write("(�p�����[�^�`�F�b�N)", "���s", "�}�̃R�[�h�ُ�")
                Call BatchLog.UpdateJOBMASTbyErr("�����}�X�^�o�^�ُ�F�}�̃R�[�h")
                mErrMessage = "�}�̃R�[�h�ُ�"
                Return False
            End If

            '---------------------------------------------------------
            '�R�[�h�敪�̃`�F�b�N
            '---------------------------------------------------------
            If mKoParam.CP.CODE_KBN <> mArgumentData.INFOToriMast.CODE_KBN_T Then
                BatchLog.Write("(�p�����[�^�`�F�b�N)", "���s", "�R�[�h�敪�ُ�")
                Call BatchLog.UpdateJOBMASTbyErr("�����}�X�^�o�^�ُ�F�R�[�h�敪")
                mErrMessage = "�R�[�h�敪�ُ�"
                Return False
            End If

            ' �˗����E�`�[�łȂ��ꍇ�A�����񎝍����� ���� �����Ǘ����Ȃ��ꍇ�̓X�P�W���[���̑��݃`�F�b�N���s��Ȃ�
            If mArgumentData.INFOToriMast.BAITAI_CODE_T <> "04" AndAlso mArgumentData.INFOToriMast.BAITAI_CODE_T <> "09" Then
                If mArgumentData.INFOToriMast.CYCLE_T = "1" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                    Return True
                End If
            End If

            '---------------------------------------------------------
            '�X�P�W���[���̗L���̃`�F�b�N�^�������ݏ����������̊m�F
            '---------------------------------------------------------
            Dim oReader As CASTCommon.MyOracleReader = Nothing

            SQL.Append("SELECT ")
            SQL.Append(" TYUUDAN_FLG_S")
            SQL.Append(",TOUROKU_FLG_S")
            SQL.Append(" FROM S_SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(mKoParam.CP.FURI_DATE))

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    ' �����񎝍��Ȃ� ���� �����Ǘ����Ȃ��ꍇ
                    If mArgumentData.INFOToriMast.CYCLE_T = "0" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                        Return True
                    End If
                    BatchLog.Write("�X�P�W���[���Ȃ�", "���s", " �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���Ȃ� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    mErrMessage = "�X�P�W���[���Ȃ�"
                    Return False
                Else
                    If mArgumentData.INFOToriMast.CYCLE_T = "0" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                        ' �����Ǘ��Ȃ��̏ꍇ�ŁC�����񎝂����݂��Ȃ��ꍇ�C
                        ' �����X�P�W���[�������݂���ꍇ�G���[
                        Do While oReader.EOF = False
                            '�o�^�t���O�^���f�t���O�̊m�F
                            If oReader.GetString("TOUROKU_FLG_S") <> "0" AndAlso oReader.GetString("TYUUDAN_FLG_S") = "0" Then
                                BatchLog.Write("�X�P�W���[��", "���s", "�������ݏ����� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                                Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:�������ݏ����� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                                mErrMessage = "�������ݏ�����"
                            End If
                            oReader.NextRead()
                        Loop

                        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                        'Return True
                        Return SCHMAST_Lock(oReader, LockWaitTime)
                        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                    End If
                End If

                If oReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                    BatchLog.Write("�X�P�W���[��", "���s", "���f�t���O�ݒ�� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:���f�t���O�ݒ�� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                    mErrMessage = "���f�t���O�ݒ�ς�"
                    Return False
                End If

                If oReader.GetString("TOUROKU_FLG_S") <> "0" Then
                    BatchLog.Write("�X�P�W���[��", "���s", "�������ݏ����� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & "�����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:�������ݏ����� �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "�������ݏ�����"
                    Return False
                End If

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                'Return True
                Return SCHMAST_Lock(oReader, LockWaitTime)
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            Catch ex As Exception
                BatchLog.Write("�X�P�W���[������", "���s", "�U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���������s �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
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


    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
    Private Function SCHMAST_Lock(ByRef oReader As CASTCommon.MyOracleReader, ByVal LockWaitTime As Integer) As Boolean

        Dim SQL As StringBuilder = New StringBuilder(256)

        ' ��\�ϑ��҃R�[�h�擾
        SQL.Append("SELECT ")
        SQL.Append(" ITAKU_KANRI_CODE_T")
        SQL.Append(" FROM S_TORIMAST")
        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mKoParam.CP.TORIS_CODE))
        SQL.Append(" AND TORIF_CODE_T = " & SQ(mKoParam.CP.TORIF_CODE))
        SQL.Append(" AND MULTI_KBN_T  = '1'")

        If oReader.DataReader(SQL) = True Then
            Dim itaku_kanri_code As String = oReader.GetString("ITAKU_KANRI_CODE_T")

            oReader.Close()

            ' �}���`�敪���̃X�P�W���[�����b�N
            SQL = New StringBuilder(256)
            SQL.Append("SELECT FURI_DATE_S")
            SQL.Append(" FROM S_SCHMAST , S_TORIMAST")
            SQL.Append(" WHERE ITAKU_KANRI_CODE_T = " & SQ(itaku_kanri_code))
            SQL.Append(" AND TORIS_CODE_T       = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T       = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S        = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND TOUROKU_FLG_S      = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S      = '0'")
            SQL.Append(" FOR UPDATE OF S_SCHMAST.TORIS_CODE_S WAIT " & LockWaitTime)
            If oReader.DataReader(SQL) = False Then
                If oReader.Message <> "" Then
                    Dim errmsg As String
                    If oReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g"
                    Else
                        errmsg = "�U���˗��f�[�^�����������b�N�ُ�"
                    End If

                    BatchLog.Write_Err("�������ݏ���", "���s", errmsg & " �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                    Call BatchLog.UpdateJOBMASTbyErr(errmsg & " �U�����F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
                    mErrMessage = errmsg
                    Return False
                End If
            End If
        End If

        Return True

    End Function
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***


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
        Dim MediaRead As New clsMediaRead

        '2010/04/26 �p�����[�^�t�@�C�������l������
        Dim ParamInFileName As String = Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

        Try
            BatchLog.Write("(�˗��f�[�^�Ǎ�)�J�n", "����")

            '�˗��f�[�^�Ǎ��^�t�H�[�}�b�g�`�F�b�N���s
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData) '�A�g

            '------------------------------------------------------------
            '�}�̓ǂ�
            '------------------------------------------------------------
            With mKoParam.CP

                '���ԃt�@�C���쐬��w��
                oRenkei.OutFileName = CASTCommon.GetFSKJIni("COMMON", "DATBK") & "i" & .TORI_CODE & ".DAT"

                'F*TRAN�t�@�C�������擾
                If .CODE_KBN = "4" And .BAITAI_CODE = "01" Then
                    'FTranName = readfmt.FTRANIBMP
                    FTranName = readfmt.FTRANIBMBINARYP
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

                        '�e�c�t�@�C���Ǎ��������s

                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''2010/04/26 �p�����[�^�t�@�C�������l������
                        ''nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'If ParamInFileName = "" Then
                        '    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'Else
                        '    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, ParamInFileName, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'End If

                        ''2009/11/30 �R�[�h�敪�`�F�b�N�ǉ�===
                        'If nRetRead = 0 Then
                        '    Dim ErrMessage As String = ""
                        '    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                        '        Case 0  '�ُ�Ȃ�
                        '        Case 10 '10�FShift-jis
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                        '            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                        '            Return ""
                        '        Case 20 ' 20:EBCDIC�ُ�
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                        '            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                        '            Return ""
                        '        Case -1 '�������s
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                        '            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                        '            Return ""
                        '    End Select
                        'End If
                        ''===================================
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s:")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:")
                                    mErrMessage = "�t�@�C�����\�z���s"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)


                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                '�˗��t�@�C�����R�s�[����O�ɁA�˗��t�@�C���̓ǂݎ���p��������������
                                If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                                    Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                        fiCopyFile.Attributes = FileAttributes.Normal
                                    End If
                                End If

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If
                            Case Else
                                If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                                Else
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                                End If

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                If ParamInFileName = "" Then
                                    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                                Else
                                    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, ParamInFileName, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                                End If

                                If nRetRead = 0 Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If
                        End Select
                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        '------------------------------------------------------------
                        '���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If

                    Case "05"       '�l�s

                        '2018/10/05 saitou �L���M��(RSV2�W��) UPD ---------------------------------------- START
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "�t�@�C�����\�z���s"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If

                            Case Else
                                '�l�s�t�@�C���Ǎ��������s
                                strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                                '�}�̓Ǎ����s�F�����I��
                                If strKekka <> "" Then
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�l�s�Ǎ��F" & oRenkei.Message)
                                    BatchLog.UpdateJOBMASTbyErr("�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                                    Return ""
                                End If

                                '------------------------------------------------------------
                                '���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                                '------------------------------------------------------------
                                If nRetRead = 0 Then
                                    nRetRead = oRenkei.CopyToDisk(readfmt)
                                End If
                        End Select

                        ''****�΂��Ȃ薢�Ή� 2009.10.23

                        ''2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        ''�l�s�t�@�C���Ǎ��������s
                        'strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''Dim intblk_size As Short = CShort(readfmt.BLOCKSIZE)
                        ''Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        ''Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''�l�s�t�@�C���Ǎ��������s
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        ''�}�̓Ǎ����s�F�����I��
                        'If strKekka <> "" Then
                        '    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�l�s�Ǎ��F" & oRenkei.Message)
                        '    BatchLog.UpdateJOBMASTbyErr("�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                        '    Return ""
                        'End If

                        ''**************************
                        ''------------------------------------------------------------
                        ''���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                        ''------------------------------------------------------------
                        'If nRetRead = 0 Then
                        '    nRetRead = oRenkei.CopyToDisk(readfmt)
                        'End If
                        ''**************************
                        '2018/10/05 saitou �L���M��(RSV2�W��) UPD ---------------------------------------- END

                    Case "06"       '�b�l�s

                        '2018/10/05 saitou �L���M��(RSV2�W��) UPD ---------------------------------------- START
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "�t�@�C�����\�z���s"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If

                            Case Else

                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))

                                '�b�l�s�t�@�C���R�s�[��ݒ�
                                strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                                '�}�̓Ǎ����s�F�����I��
                                If strKekka <> "" Then
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�b�l�s�Ǎ��F" & oRenkei.Message)
                                    BatchLog.UpdateJOBMASTbyErr("�b�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                                    Return ""
                                End If

                                '------------------------------------------------------------
                                '���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                                '------------------------------------------------------------
                                If nRetRead = 0 Then
                                    nRetRead = oRenkei.CopyToDisk(readfmt)
                                End If
                        End Select

                        ''****�΂��Ȃ薢�Ή� 2009.10.23

                        ''2018/02/19 saitou �L���M��(RSV2�W��) DEL �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        ''Dim intblk_size As Short = CShort(readfmt.BLOCKSIZE)
                        ''Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        ''Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)
                        ''2018/02/19 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------- END

                        ''***************************
                        'mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        ''***************************

                        ''�b�l�s�t�@�C���R�s�[��ݒ�
                        ''2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        'strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ' ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        ''�}�̓Ǎ����s�F�����I��
                        'If strKekka <> "" Then
                        '    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�b�l�s�Ǎ��F" & oRenkei.Message)
                        '    BatchLog.UpdateJOBMASTbyErr("�b�l�s�Ǎ����s�A�t�@�C�����F" & oRenkei.InFileName)
                        '    Return ""
                        'End If

                        ''**************************
                        ''------------------------------------------------------------
                        ''���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                        ''------------------------------------------------------------
                        'If nRetRead = 0 Then
                        '    nRetRead = oRenkei.CopyToDisk(readfmt)
                        'End If
                        ''**************************
                        '2018/10/05 saitou �L���M��(RSV2�W��) UPD ---------------------------------------- END

                    Case "04", "09" '�˗���/�`�[

                        Dim REC_LENGTH As Integer = 120
                        '�˗��t�@�C���R�s�[��ݒ�
                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "�t�@�C�����\�z���s"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "ETC"), FileName_Edition2)
                            Case Else
                                mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        End Select
                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        'nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

                        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD -------------------------------------------------->>>>
                        '�O���}��(11�`15)�ǉ�
                    Case "11", "12", "13", "14", "15"

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

                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''�R�[�h�敪�`�F�b�N�p�̃��[�N�t�@�C���p�X�p
                        'Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)

                        'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                        '                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)

                        ''2009/11/30 �R�[�h�敪�`�F�b�N�ǉ�===
                        'If nRetRead = 0 Then
                        '    Dim ErrMessage As String = ""
                        '    Select Case oRenkei.CheckCode(CodeCHKFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                        '        Case 0  '�ُ�Ȃ�
                        '        Case 10 '10�FShift-jis
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                        '            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                        '            Return ""
                        '        Case 20 ' 20:EBCDIC�ُ�
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                        '            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                        '            Return ""
                        '        Case -1 '�������s
                        '            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                        '            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                        '            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                        '            Return ""
                        '    End Select
                        'End If
                        ''===================================
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "�t�@�C�����\�z���s"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                '�˗��t�@�C�����R�s�[����O�ɁA�˗��t�@�C���̓ǂݎ���p��������������
                                If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                                    Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                        fiCopyFile.Attributes = FileAttributes.Normal
                                    End If
                                End If

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If
                            Case Else
                                If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                                Else
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                                End If

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                '�R�[�h�敪�`�F�b�N�p�̃��[�N�t�@�C���p�X�p
                                Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)

                                nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                                                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)

                                If nRetRead = 0 Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(CodeCHKFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '�ُ�Ȃ�
                                        Case 10 '10�FShift-jis
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC�ُ�
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                            mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                            Return ""
                                        Case -1 '�������s
                                            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                            mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                            Return ""
                                    End Select
                                End If
                        End Select
                        ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        '------------------------------------------------------------
                        '���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If
                        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD --------------------------------------------------<<<<

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
                        If mKoParam.CP.BAITAI_CODE = "SA" Then
                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "S" & .TORI_CODE & ".DAT"
                            '2012/06/30 �W���Ł@WEB�`���Ή�-------------------------------------------------------------
                        ElseIf mKoParam.CP.BAITAI_CODE = "10" Then
                            ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            '    End If
                            'End If
                            ''--------------------------------------------------------------------------------------------
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    Dim FileName_Edition2 As String = ""
                                    If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                        BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                        BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                        mErrMessage = "�t�@�C�����\�z���s"
                                        Return ""
                                    End If
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName_Edition2)
                                Case Else
                                    If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                        Else
                            '2010/09/08.Sakon�@�����}�X�^�̃t�@�C������D�悵�Ďg�p���� ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                            '    'TORIMAST.FILE_NAME_T���󔒂łȂ����TORIMAST.FILE_NAME_T���t�@�C���Ƃ��Đݒ肷��B
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    'TORIMAST.FILE_NAME_T���󔒂ł���Ώ]���̃t�@�C������ݒ肷��B
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            '    End If
                            'End If
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    Dim FileName_Edition2 As String = ""
                                    If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                        BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�t�@�C�����\�z���s")
                                        BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�����\�z���s:" & Baitai_Code & "-" & FileName_Edition2)
                                        mErrMessage = "�t�@�C�����\�z���s"
                                        Return ""
                                    End If
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), FileName_Edition2)
                                Case Else
                                    If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/12 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                            'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            'Else
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            'End If
                            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        End If

                        '2010/04/26 �p�����[�^�t�@�C�������l������
                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        If ParamInFileName = "" Then
                            oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        Else
                            oRenkei.InFileName = Path.Combine(Path.GetDirectoryName(mKoParam.CP.RENKEI_FILENAME), ParamInFileName)
                        End If

                        '2012/01/13 saitou �W���C�� ADD ---------------------------------------->>>>
                        '�˗��t�@�C�����R�s�[����O�ɁA�˗��t�@�C���̓ǂݎ���p��������������
                        If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                            Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                            If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                fiCopyFile.Attributes = FileAttributes.Normal
                            End If
                        End If
                        '2012/01/13 saitou �W���C�� ADD ----------------------------------------<<<<

                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        '2009/11/30 �R�[�h�敪�`�F�b�N�ǉ�===
                        If nRetRead = 0 Then
                            Dim ErrMessage As String = ""
                            Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                Case 0  '�ُ�Ȃ�
                                Case 10 '10�FShift-jis
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(JIS) �t�@�C����:" & oRenkei.InFileName)
                                    mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(JIS)"
                                    Return ""
                                Case 20 ' 20:EBCDIC�ُ�
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�R�[�h�敪�ُ�(EBCDIC) �t�@�C����:" & oRenkei.InFileName)
                                    mErrMessage = "�f�[�^�t�@�C�� �R�[�h�敪�ُ�(EBCDIC)"
                                    Return ""
                                Case -1 '�������s
                                    BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
                                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ErrMessage)
                                    mErrMessage = "�V�X�e���G���[(���O�Q��)"
                                    Return ""
                            End Select
                        End If
                        '===================================

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
                    mErrMessage = "̧�ٌ`���ُ큨" & oRenkei.InFileName
                    Return ""
                Case 50
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C�������F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���Ȃ��A�t�@�C�����F" & oRenkei.InFileName)
                    mErrMessage = "̧�قȂ���" & oRenkei.InFileName
                    Return ""
                Case 100
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�ϊ�)�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�R�[�h�ϊ��j")
                    mErrMessage = "̧�َ捞���s�i���ޕϊ��j"
                    Return ""
                Case 200
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�敪�ُ�(JIS���s����))�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�R�[�h�敪�ُ�iJIS���s����j�j")
                    mErrMessage = "̧�َ捞���s(���ދ敪�ُ�(JIS���s����))"
                    Return ""
                Case 300
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�R�[�h�敪�ُ�(JIS���s�Ȃ�))�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s(�R�[�h�敪�ُ�(JIS���s�Ȃ�))")
                    mErrMessage = "̧�َ捞���s(���ދ敪�ُ�(JIS���s�Ȃ�))"
                    Return ""
                Case 400
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(�o�̓t�@�C���쐬)�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i�o�̓t�@�C���쐬�j")
                    mErrMessage = "̧�َ捞���s(�o��̧�ٍ쐬)"
                    Return ""
                Case 999
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�t�@�C���捞(���[�U�L�����Z��)�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i���[�U�L�����Z���j")
                    mErrMessage = "̧�َ捞���s(հ�޷�ݾ�)"
                    Return ""
                Case Else
                    BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", "�s���Ȗ߂�l�F" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("�t�@�C���捞���s�i���O�Q�Ɓj")
                    mErrMessage = "̧�َ捞���s(۸ގQ��)"
                    Return ""
            End Select

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("�˗��f�[�^�Ǎ������F�V�X�e���G���[�i���O�Q�Ɓj")
            BatchLog.Write("(�˗��f�[�^�Ǎ�)", "���s", ex.Message)
            mErrMessage = "�V�X�e���G���[(���O�Q��)"
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
        Dim PrnMeisai As ClsPrnUketukeMeisai = Nothing          '��t���ו\
        Dim ArrayPrnMeisai As New ArrayList(128)
        Dim ArrayPrnInErrList As New ArrayList(128)
        Dim PrnSitenYomikae As New ClsPrnSitenYomikae           '�x�X�Ǒ֖��ו\
        Dim bSitenYomikaePrint As Boolean = False               '�x�X�Ǒ֏o�͖��חL��
        Dim DaihyouItakuCode As String = ""                     '��\�ϑ��҃R�[�h
        ' 2016/03/03 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
        Dim IOfileStream As FileStream = Nothing
        ' 2016/03/03 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        Dim MotikomiOver As Boolean = True  '���������t���O(True:����������)
        Dim DateOver As Boolean = True      '��t���ԊO�t���O(True:��t���ԓ�)
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

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
            Dim SitenYomikae As String                  '�x�X�Ǒֈ���敪(1:����Ώ� / ���̑�:�����Ώ�)
            SitenYomikae = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")

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

                    '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                    Try
                        '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

                        ' �f�[�^��ǂݍ���ŁC�t�H�[�}�b�g�`�F�b�N���s��
                        sCheckRet = aReadFmt.CheckDataFormat()

                        '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                    Catch ex As Exception
                        BatchLog.Write_Err("(�o�^���C������)", "���s", ex)
                        BatchLog.UpdateJOBMASTbyErr("�t�H�[�}�b�g�ϊ����s")
                        mErrMessage = "�t�H�[�}�b�g�ϊ����s"
                        Return False
                    End Try
                    '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

                    Select Case sCheckRet

                        Case "ERR"
                            '�t�H�[�}�b�g�G���[
                            Dim nPos As Long
                            If aReadFmt.RecordData.Length > 0 Then nPos = aReadFmt.CheckRegularString

                            If nPos > 0 Then
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�ځC" & (nPos + 1).ToString & "�o�C�g�� " & aReadFmt.Message)
                            Else
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                            End If

                            mErrMessage = nRecordCount.ToString & "�s�� " & aReadFmt.Message

                            bRet = False
                            Exit Do

                        Case "IJO"
                            '�������Ƃ߂Ȃ� 2009.10.17
                            ''���z�ُ� �t�H�[�}�b�g�G���[ 
                            'If ErrFlag = False AndAlso aReadFmt.InErrorArray.Count > 0 Then
                            '    For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1

                            '        Dim InError As CAstFormat.CFormat.INPUTERROR
                            '        InError = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

                            '        '���s�X�Ԉُ�ȊO�^���s�X�Ԉُ�ȊO�^�����ԍ��ُ�ȊO�̃G���[
                            '        If InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.JikouTenban) AndAlso _
                            '           InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.TakouTenban) AndAlso _
                            '           InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.Kouza) Then

                            '            '�ُ폈�����s��
                            '            ErrFlag = True
                            '            ErrText = nRecordCount.ToString & "�s�� " & aReadFmt.Message
                            '            Exit For
                            '        End If
                            '    Next i
                            'End If

                            If PrnInErrList Is Nothing Then
                                '�ŏ��̃t�@�C���쐬
                                '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
                                PrnInErrList = New ClsPrnInputError(INI_RSV2_EDITION)        ' �C���v�b�g�G���[���X�g�N���X
                                PrnInErrList.CreateMemFile()
                                'PrnInErrList = New ClsPrnInputError        ' �C���v�b�g�G���[���X�g�N���X
                                'PrnInErrList.CreateCsvFile()
                                '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END
                            End If

                            '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
                            Call PrnInErrList.OutputMemData(aReadFmt, aReadFmt.ToriData)
                            'Call PrnInErrList.OutputData(aReadFmt, aReadFmt.ToriData)
                            '2017/02/27 �^�X�N�j���� CHG �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

                        Case "H"
                            EndFlag = False

                        Case "D"

                        Case "T"

                        Case "E"
                            EndFlag = True

                        Case "99"
                            '�G���h���R�[�h�����������Ă����̂܂ܒʂ�
                            EndFlag = True

                        Case "1A" '2010.01.19 1A �Ή�
                            EndFlag = True

                        Case ""
                            Exit Do

                    End Select

                    '�w�b�_���R�[�h
                    If aReadFmt.IsHeaderRecord() = True Then

                        '�}���`�`�F�b�N
                        If DaihyouItakuCode = "" Then
                            '�P�@�T�@�ځF��\�ϑ��҃R�[�h��ێ�
                            DaihyouItakuCode = aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T
                        Else
                            '�Q�T�ڈȍ~�F�}���`�f�[�^�`�F�b�N
                            Select Case True    'True = �G���[����

                                Case aReadFmt.ToriData.INFOToriMast.MULTI_KBN_T = "0"
                                    '�}���`�敪�`�F�b�N
                                    BatchLog.Write("���׃}�X�^�o�^����", "���s", nRecordCount.ToString & "�s�� �}���`�敪�ُ� : 0")
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� �}���`�敪�ُ� : 0")
                                    mKoParam.CP.TORIS_CODE = aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                    mKoParam.CP.TORIF_CODE = aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                    mErrMessage = "�}���`�敪�ُ�"
                                    Return False

                                Case DaihyouItakuCode <> aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T
                                    '��\�ϑ��҃R�[�h�`�F�b�N
                                    BatchLog.Write("���׃}�X�^�o�^����", "���s", nRecordCount.ToString & "�s�� ��\�ϑ��҃R�[�h�s��v" & _
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� ��\�ϑ��҃R�[�h�s��v" & _
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    mKoParam.CP.TORIS_CODE = aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                    mKoParam.CP.TORIF_CODE = aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                    mErrMessage = "��\�ϑ��҃R�[�h�s��v"
                                    Return False
                            End Select
                        End If

                        '�J�E���^������
                        nRecordNumber = 1
                        aReadFmt.InfoMeisaiMast.FILE_SEQ += 1
                        PrnFlag = True

                        '0:��Ώ�,1:�X�ԃ\�[�g,2:��\�[�g,3:�G���[���̂� 4�ȍ~�g���p
                        Dim ret As Integer = ".123456789".IndexOf(CType(aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, Char))
                        If ret >= 1 Then
                            If aReadFmt.ToriData.INFOParameter.FMT_KBN <> "03" Then
                                '�N���ȊO�̏o�͑Ώ�
                                PrnMeisai = New ClsPrnUketukeMeisai
                                PrnMeisai.OraDB = MainDB
                                PrnMeisai.CreateCsvFile()
                            End If
                        End If

                        ' ��d
                        ArrayMeiData.Clear()
                    End If

                    '���׃}�X�^���ڐݒ� ���R�[�h�ԍ�
                    '�����G���h�̃��R�[�h�͖��׃}�X�^��INSERT���Ȃ�
                    If sCheckRet <> "99" Then
                        aReadFmt.InfoMeisaiMast.RECORD_NO = nRecordNumber
                    End If

                    ' �f�[�^���R�[�h
                    If aReadFmt.IsDataRecord = True Then

                        '�x�X�Ǒւb�r�u�o��
                        If SitenYomikae = "1" Then  'INI�t�@�C���̏o�͑Ώۂ��u1�v�̏ꍇ�̂�
                            If aReadFmt.InfoMeisaiMast.KEIYAKU_KIN <> aReadFmt.InfoMeisaiMast.OLD_KIN_NO OrElse _
                               aReadFmt.InfoMeisaiMast.KEIYAKU_SIT <> aReadFmt.InfoMeisaiMast.OLD_SIT_NO OrElse _
                               aReadFmt.InfoMeisaiMast.KEIYAKU_KOUZA <> aReadFmt.InfoMeisaiMast.OLD_KOUZA Then

                                '���񏈗�(����t���O�𗧂Ă�^�b�r�u�t�@�C�����쐬����)
                                If bSitenYomikaePrint = False Then
                                    bSitenYomikaePrint = True
                                    PrnSitenYomikae.CreateCsvFile()
                                End If

                                '�f�[�^�����o��
                                PrnSitenYomikae.OutputCsvData(aReadFmt)
                            End If
                        End If

                        '��t���ׂb�r�u�o��
                        If Not PrnMeisai Is Nothing Then
                            ' 0:��Ώ�,1:�X�ԃ\�[�g,2:��\�[�g,3:�G���[���̂�
                            ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- START
                            ' �C�������Ӑ}���s�������邽�ߖ߂����łɁA�A�b�v�O���[�h
                            'Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
                            'Select Case "1"
                            '    Case "1", "2"
                            '        PrnMeisai.OutputCsvData(aReadFmt)
                            '    Case "3"
                            '        If sCheckRet = "IJO" Then
                            '            PrnMeisai.OutputCsvData(aReadFmt)
                            '        End If
                            'End Select
                            Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
                                Case "0"
                                Case Else
                                    Dim UketukeErrOut As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010_��t���ו\�o�͋敪.TXT"), _
                                                                                                    aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, _
                                                                                                    2)
                                    Select Case UketukeErrOut.Trim
                                        Case "E"
                                            If sCheckRet = "IJO" Then
                                                PrnMeisai.OutputCsvData(aReadFmt)
                                            End If
                                        Case Else
                                            PrnMeisai.OutputCsvData(aReadFmt)
                                    End Select
                            End Select
                            ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- END
                        End If
                    End If

                    ' ��d�����`�F�b�N
                    If aReadFmt.ToriData.INFOToriMast.CYCLE_T = "1" Then
                        If nRecordNumber <= 6 Then
                            ' �ŏ��̂U��
                            ArrayMeiData.Add(aReadFmt.RecordData)
                        ElseIf sCheckRet = "T" Then
                            ' �g���[�����R�[�h
                            ArrayMeiData.Add(aReadFmt.RecordData)
                        End If
                    End If

                    '�����G���h�̃��R�[�h�͖��׃}�X�^��INSERT���Ȃ�
                    '2010.01.19 1A �Ή�
                    '���׃}�X�^�o�^
                    If sCheckRet <> "99" And sCheckRet <> "1A" Then
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
                        If aReadFmt.EOF = True AndAlso EndFlag = False Then
                            BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", (nRecordCount + 1).ToString & "�s�� �G���h���R�[�h������܂���")
                            BatchLog.UpdateJOBMASTbyErr((nRecordCount + 1).ToString & "�s�� �G���h���R�[�h������܂���")
                            mErrMessage = (nRecordCount + 1).ToString & "�s�� ����ں��ނȂ�"
                            bRet = False
                            Exit Do
                        End If

                        If PrnFlag = True Then

                            ' ���׃}�X�^���������C�Q�d�����̌������s��
                            Dim bDupicate As Boolean = False
                            '2017/12/06 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�d�����R�[�h�`�F�b�N�j-------------- START
                            Dim blnChofuku As Boolean = False
                            '2017/12/06 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�d�����R�[�h�`�F�b�N�j-------------- END
                            bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData)
                            If bDupicate = True Then
                                mJobMessage = "��d��������"
                                '2017/12/06 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�d�����R�[�h�`�F�b�N�j-------------- START
                            Else
                                If GetRSKJIni("RSV2_V1.0.0", "CHOFUKU_CHK") = "1" Then
                                    '����˗��f�[�^�����݂���ꍇ�A�d�����R�[�h�`�F�b�N�G���[�Ƃ���B
                                    If aReadFmt.fn_Meisai_FukusuuIrai_Check(ArrayMeiData) = True Then
                                        mJobMessage = "�d�����R�[�h����"
                                        blnChofuku = True
                                    End If
                                End If
                                '2017/12/06 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�d�����R�[�h�`�F�b�N�j-------------- END
                            End If

                            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
                            If INI_MOTIKOMI_KIJITU_CHK = "1" Then
                                '���������`�F�b�N
                                MotikomiOver = aReadFmt.CheckMotikomiDate()
                                If MotikomiOver = False Then
                                    mJobMessage = "������������"
                                    mArgumentData.Syorikekka_Bikou = mJobMessage    '�������ʊm�F�\�̔��l���ɏo��
                                    BatchLog.Write("���������`�F�b�N����", "����", mJobMessage)
                                End If
                            End If

                            If INI_UKETUKE_JIKAN_CHK = "1" Then
                                '���������G���[�̏ꍇ�Ƀ`�F�b�N����
                                If MotikomiOver = False Then
                                    DateOver = aReadFmt.CheckInDatetime(aReadFmt.ToriData.INFOParameter.FURI_DATE)
                                    If DateOver = False Then
                                        mJobMessage = "��t���Ԓ���"
                                        mArgumentData.Syorikekka_Bikou = mJobMessage    '�������ʊm�F�\�̔��l���ɏo��
                                        BatchLog.Write("��t���ԊO�`�F�b�N����", "����", mJobMessage)
                                    End If
                                End If
                            End If
                            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

                            If bDupicate = False Then
                                ' �Q�d�o�^���́C�C���v�b�g�G���[���o�͂��Ȃ�
                                ' �C���v�b�g�G���[�b�r�u���o�͂���
                                If Not PrnInErrList Is Nothing Then
                                    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
                                    ' CSV�o��
                                    Call PrnInErrList.OutputCsvData(aReadFmt)

                                    If INI_RSV2_EDITION = "2" Then
                                        PrnInErrList.HostCsvName = "KFSP035_"
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnInErrList.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                                    End If
                                    '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

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

                            If bDupicate = False Then
                                ' �Q�d�o�^���́C��t���ו\���o�͂��Ȃ�
                                ' ��t���ו\���������
                                If Not PrnMeisai Is Nothing Then
                                    PrnMeisai.CloseCsv()

                                    ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- START
                                    '' 0:��Ώ�,1:�X�ԃ\�[�g,2:��\�[�g,3:�G���[���̂�
                                    'If aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T = "1" Then
                                    '    ' �X�ԃ\�[�g
                                    '    Call PrnMeisai.SortData()
                                    'End If
                                    Dim SortParam As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010_��t���ו\�o�͋敪.TXT"), _
                                                                                            aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, _
                                                                                            3)
                                    If SortParam.Trim <> "" Then
                                        Call PrnMeisai.SortData(SortParam.Trim)
                                    End If
                                    ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- END

                                    ' ��ň��
                                    ' 2016/03/03 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                                    'CSV�t�@�C����0�o�C�g�̏ꍇ�A����Ώۂɏ悹�Ȃ�
                                    IOfileStream = New FileStream(PrnMeisai.FileName, FileMode.Open, FileAccess.Read, FileShare.None)
                                    If IOfileStream.Length <> 0 Then
                                        '2011/06/16 �W���ŏC�� ��t���ו\��������ɂ���� ------------------START
                                        Dim Print_CNT As Integer = 0
                                        Dim OraReader As CASTCommon.MyOracleReader = Nothing
                                        Dim SQL As StringBuilder = New StringBuilder(256)
                                        Try
                                            '�f�o�b�O�p
                                            'BatchLog.Write("��������擾�J�n", "����", "�����R�[�h" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                            'aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)
                                            OraReader = New CASTCommon.MyOracleReader(MainDB)

                                            SQL.Append("SELECT PRTNUM_T FROM S_TORIMAST ")
                                            SQL.Append(" WHERE TORIS_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "'")
                                            SQL.Append(" AND TORIF_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "'")
                                            If OraReader.DataReader(SQL) = True Then
                                                Print_CNT = OraReader.GetInt("PRTNUM_T")
                                            Else
                                                Print_CNT = 1
                                            End If
                                            For intPrintNumber As Integer = 1 To Print_CNT
                                                PrnMeisai.HostCsvName = "KFSP002_"
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                                PrnMeisai.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                                                ArrayPrnMeisai.Add(PrnMeisai)
                                            Next
                                        Catch ex As Exception
                                            BatchLog.Write("��������擾", "���s", "�����R�[�h" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                                           aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & " " & ex.Message)

                                        Finally
                                            OraReader.Close()
                                        End Try
                                        'ArrayPrnMeisai.Add(PrnMeisai)
                                        '2011/06/16 �W���ŏC�� ��t���ו\��������ɂ���� ------------------END
                                    End If
                                    IOfileStream.Close()
                                    IOfileStream = Nothing
                                    ''2011/06/16 �W���ŏC�� ��t���ו\��������ɂ���� ------------------START
                                    'Dim Print_CNT As Integer = 0
                                    'Dim OraReader As CASTCommon.MyOracleReader = Nothing
                                    'Dim SQL As StringBuilder = New StringBuilder(256)
                                    'Try
                                    '    '�f�o�b�O�p
                                    '    'BatchLog.Write("��������擾�J�n", "����", "�����R�[�h" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                    '    'aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)
                                    '    OraReader = New CASTCommon.MyOracleReader(MainDB)

                                    '    SQL.Append("SELECT PRTNUM_T FROM S_TORIMAST ")
                                    '    SQL.Append(" WHERE TORIS_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "'")
                                    '    SQL.Append(" AND TORIF_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "'")
                                    '    If OraReader.DataReader(SQL) = True Then
                                    '        Print_CNT = OraReader.GetInt("PRTNUM_T")
                                    '    Else
                                    '        Print_CNT = 1
                                    '    End If
                                    '    For intPrintNumber As Integer = 1 To Print_CNT
                                    '        ArrayPrnMeisai.Add(PrnMeisai)
                                    '    Next
                                    'Catch ex As Exception
                                    '    BatchLog.Write("��������擾", "���s", "�����R�[�h" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                    '                   aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & " " & ex.Message)

                                    'Finally
                                    '    OraReader.Close()
                                    'End Try
                                    ''ArrayPrnMeisai.Add(PrnMeisai)
                                    ''2011/06/16 �W���ŏC�� ��t���ו\��������ɂ���� ------------------END
                                    ' 2016/03/03 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

                                    PrnMeisai = Nothing



                                End If
                            End If

                            '�X�ʏW�v�\����p���X�g�Ɋi�[(����敪,������R�[�h,����敛�R�[�h)
                            ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," & _
                                             aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," & _
                                             aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "," & _
                                             aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ)

                            ' �X�P�W���[���}�X�^���X�V����
                            bRet = aReadFmt.UpdateSCHMAST
                            If bRet = False Then
                                BatchLog.Write("�X�P�W���[���X�V", "���s")
                                BatchLog.UpdateJOBMASTbyErr("�X�P�W���[���X�V���s")
                                mErrMessage = "�X�P�W���[���X�V���s"
                                Exit Do
                            End If
                        End If

                        PrnFlag = False
                    End If
                Loop

                ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("�又��", "���s", "�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        BatchLog.UpdateJOBMASTbyErr("�U���˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        Return False
                    End If
                End If
                ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

                If bRet = False Then

                    If aReadFmt.IsTrailerRecord() = True Then
                        ' �C���v�b�g�G���[���X�g���������
                        If Not PrnInErrList Is Nothing Then
                            '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- START
                            ' CSV�o��
                            Call PrnInErrList.OutputCsvData(aReadFmt)

                            If INI_RSV2_EDITION = "2" Then
                                PrnInErrList.HostCsvName = "KFSP035_"
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                PrnInErrList.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                            End If
                            '2017/02/27 �^�X�N�j���� ADD �ѓc�M�� �J�X�^�}�C�Y�Ή�(�C���v�b�g�G���[���X�g�C��) -------------------- END

                            PrnInErrList.CloseCsv()

                            ' ���폈���̂��ƂɈ�����邽�߁C���X�g�ɕۑ�
                            ArrayPrnInErrList.Add(PrnInErrList)
                            PrnInErrList = Nothing
                        End If

                        For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                            PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                            If PrnInErrList.ReportExecute() = False Then
                                BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "���s", "�t�@�C����:" & PrnInErrList.FileName & " ���b�Z�[�W:" & PrnInErrList.ReportMessage)
                            End If


                            '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
                            '��K�͔ł̂݃R�s�[����
                            If INI_RSV2_EDITION = "2" Then
                                Try
                                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                                    DestName &= PrnInErrList.HostCsvName
                                    File.Copy(PrnInErrList.FileName, DestName, True)
                                    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "����", DestName)
                                    PrnInErrList = Nothing
                                Catch ex As Exception
                                    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "���s", ex.Message)
                                End Try
                            End If
                            'Try
                            '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            '    DestName &= PrnInErrList.HostCsvName
                            '    File.Copy(PrnInErrList.FileName, DestName, True)
                            '    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "����", DestName)
                            '    PrnInErrList = Nothing
                            'Catch ex As Exception
                            '    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "���s", ex.Message)
                            'End Try
                            '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END

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

                '2010.01.19 1A �Ή�
                If sCheckRet <> "E" AndAlso sCheckRet <> "T" AndAlso sCheckRet <> "" AndAlso sCheckRet <> "99" AndAlso sCheckRet <> "1A" Then
                    BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", nRecordCount.ToString & "�s�� " & aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� �t�@�C�����R�[�h�i�G���h�敪�j�ُ�")
                    mErrMessage = "�G���h�敪�ُ�"
                    Return False
                End If

                '�x�X�Ǒ֖��ו\���������
                If bSitenYomikaePrint = True Then
                    If PrnSitenYomikae.ReportExecute() = False Then
                        BatchLog.Write("�x�X�Ǒ֖��ו\", "���s", "�t�@�C����:" & PrnSitenYomikae.FileName & " ���b�Z�[�W:" & PrnSitenYomikae.ReportMessage)
                    End If
                End If

                ' ��t���ו\���������
                For i As Integer = 0 To ArrayPrnMeisai.Count - 1
                    PrnMeisai = CType(ArrayPrnMeisai(i), ClsPrnUketukeMeisai)

                    If PrnMeisai.ReportExecute() = False Then
                        BatchLog.Write("��t���ו\�o��", "���s", "�t�@�C����:" & PrnMeisai.FileName & " ���b�Z�[�W:" & PrnMeisai.ReportMessage)
                    End If

                    '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
                    '��K�͔ł̂݃R�s�[����
                    If INI_RSV2_EDITION = "2" Then
                        Try
                            Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            DestName &= PrnMeisai.HostCsvName
                            File.Copy(PrnMeisai.FileName, DestName, True)
                            BatchLog.Write("��t���ו\�b�r�u�o�͏���", "����", DestName)
                            PrnMeisai = Nothing
                        Catch ex As Exception
                            BatchLog.Write("��t���ו\�b�r�u�o�͏���", "���s", ex.Message)
                        End Try
                    End If
                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnMeisai.HostCsvName
                    '    File.Copy(PrnMeisai.FileName, DestName, True)
                    '    BatchLog.Write("��t���ו\�b�r�u�o�͏���", "����", DestName)
                    '    PrnMeisai = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("��t���ו\�b�r�u�o�͏���", "���s", ex.Message)
                    'End Try
                    '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END

                Next i

                ' �C���v�b�g�G���[���X�g���������
                For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                    PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)

                    If PrnInErrList.ReportExecute() = False Then
                        BatchLog.Write("�C���v�b�g�G���[���X�g�o��", "���s", "�t�@�C����:" & PrnInErrList.FileName & " ���b�Z�[�W:" & PrnInErrList.ReportMessage)
                    End If

                    '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
                    '��K�͔ł̂݃R�s�[����
                    If INI_RSV2_EDITION = "2" Then
                        Try
                            Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            DestName &= PrnInErrList.HostCsvName
                            File.Copy(PrnInErrList.FileName, DestName, True)
                            BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "����", DestName)
                            PrnInErrList = Nothing
                        Catch ex As Exception
                            BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "���s", ex.Message)
                        End Try
                    End If
                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnInErrList.HostCsvName
                    '    File.Copy(PrnInErrList.FileName, DestName, True)
                    '    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "����", DestName)
                    '    PrnInErrList = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "���s", ex.Message)
                    'End Try
                    '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END

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
                    BatchLog.Write("�C���v�b�g�`�F�b�N", "���s", ErrText)
                    BatchLog.UpdateJOBMASTbyErr("�C���v�b�g�`�F�b�N�G���[ " & ErrText)
                    mErrMessage = ErrText
                    Return False
                End If

                '�������ʊm�F�\���������
                ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
                'If Not PrnSyoKekka Is Nothing Then
                '    If PrnSyoKekka.ReportExecute() = False Then
                '        BatchLog.Write("�������ʊm�F�\�o��", "���s", "�t�@�C����:" & PrnSyoKekka.FileName & " ���b�Z�[�W:" & PrnSyoKekka.ReportMessage)
                '    End If
                'End If
                '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
                '��������/��t���ԊO�`�F�b�N�G���[���͏o�͂���i�e�`�F�b�N�@�\���L���̏ꍇ�j
                If INI_RSV2_S_SYORIKEKKA_TOUROKU = "1" OrElse MotikomiOver = False OrElse DateOver = False Then
                    'If INI_RSV2_S_SYORIKEKKA_TOUROKU = "1" Then
                    '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("�������ʊm�F�\�o��", "���s", "�t�@�C����:" & PrnSyoKekka.FileName & " ���b�Z�[�W:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If
                End If
                ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

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
            SQL.AppendLine(" S_MEIMAST(")
            SQL.AppendLine(" FSYORI_KBN_K")
            SQL.AppendLine(",TORIS_CODE_K")
            SQL.AppendLine(",TORIF_CODE_K")
            SQL.AppendLine(",FURI_DATE_K")
            SQL.AppendLine(",MOTIKOMI_SEQ_K")
            SQL.AppendLine(",SYUBETU_K")
            SQL.AppendLine(",FURI_CODE_K")
            SQL.AppendLine(",KIGYO_CODE_K")
            SQL.AppendLine(",ITAKU_KIN_K")
            SQL.AppendLine(",ITAKU_SIT_K")
            SQL.AppendLine(",ITAKU_KAMOKU_K")
            SQL.AppendLine(",ITAKU_KOUZA_K")
            SQL.AppendLine(",KEIYAKU_KIN_K")
            SQL.AppendLine(",KEIYAKU_SIT_K")
            SQL.AppendLine(",KEIYAKU_NO_K")
            SQL.AppendLine(",KEIYAKU_KNAME_K")
            SQL.AppendLine(",KEIYAKU_KAMOKU_K")
            SQL.AppendLine(",KEIYAKU_KOUZA_K")
            SQL.AppendLine(",FURIKIN_K")
            SQL.AppendLine(",TESUU_KIN_K")
            SQL.AppendLine(",FURIKETU_CODE_K")
            SQL.AppendLine(",FURIKETU_CENTERCODE_K")
            SQL.AppendLine(",SINKI_CODE_K")
            SQL.AppendLine(",NS_KBN_K")
            SQL.AppendLine(",TEKIYO_KBN_K")
            SQL.AppendLine(",KTEKIYO_K")
            SQL.AppendLine(",NTEKIYO_K")
            SQL.AppendLine(",JYUYOUKA_NO_K")
            SQL.AppendLine(",TEISEI_SIT_K")
            SQL.AppendLine(",TEISEI_KAMOKU_K")
            SQL.AppendLine(",TEISEI_KOUZA_K")
            SQL.AppendLine(",TEISEI_AKAMOKU_K")
            SQL.AppendLine(",TEISEI_AKOUZA_K")
            SQL.AppendLine(",DATA_KBN_K")
            SQL.AppendLine(",FURI_DATA_K")
            SQL.AppendLine(",RECORD_NO_K")
            SQL.AppendLine(",SORT_KEY_K")
            SQL.AppendLine(",SAKUSEI_DATE_K")
            SQL.AppendLine(",KOUSIN_DATE_K")
            SQL.AppendLine(",YOBI1_K")
            SQL.AppendLine(",YOBI2_K")
            SQL.AppendLine(",YOBI3_K")
            SQL.AppendLine(",YOBI4_K")
            SQL.AppendLine(",YOBI5_K")
            SQL.AppendLine(",YOBI6_K")
            SQL.AppendLine(",YOBI7_K")
            SQL.AppendLine(",YOBI8_K")
            SQL.AppendLine(",YOBI9_K")
            SQL.AppendLine(",YOBI10_K")

            If aReadFmt.BinMode Then
                SQL.AppendLine(",BIN_DATA_K")
            End If

            SQL.AppendLine(")")
            SQL.AppendLine(" VALUES(")
            SQL.AppendLine(" :FSYORI_KBN")
            SQL.AppendLine(",:TORIS_CODE")                              'TORIS_CODE_K             
            SQL.AppendLine(",:TORIF_CODE")                              'TORIF_CODE_K
            SQL.AppendLine(",:FURI_DATE")                               'FURI_DATE_K
            SQL.AppendLine(",:MOTIKOMI_SEQ")                            'MOTIKOMI_SEQ_K
            SQL.AppendLine(",:SYUBETU")                                 'SYUBETU_K
            SQL.AppendLine(",:FURI_CODE")                               'FURI_CODE_K
            SQL.AppendLine(",:KIGYO_CODE")                              'KIGYO_CODE_K
            SQL.AppendLine(",:ITAKU_KIN")                               'ITAKU_KIN_K
            SQL.AppendLine(",:ITAKU_SIT")                               'ITAKU_SIT_K
            SQL.AppendLine(",:ITAKU_KAMOKU")                            'ITAKU_KAMOKU_K
            SQL.AppendLine(",:ITAKU_KOUZA")                             'ITAKU_KOUZA_K
            SQL.AppendLine(",:KEIYAKU_KIN")                             'KEIYAKU_KIN_K
            SQL.AppendLine(",:KEIYAKU_SIT")                             'KEIYAKU_SIT_K
            SQL.AppendLine(",:KEIYAKU_NO")                              'KEIYAKU_NO_K
            SQL.AppendLine(",:KEIYAKU_KNAME")                           'KEIYAKU_KNAME_K
            SQL.AppendLine(",:KEIYAKU_KAMOKU")                          'KEIYAKU_KAMOKU_K
            SQL.AppendLine(",:KEIYAKU_KOUZA")                           'KEIYAKU_KOUZA_K
            SQL.AppendLine(",:FURIKIN")                                 'FURIKIN_K
            SQL.AppendLine(",:TESUU_KIN")                               'TESUU_KIN_K
            SQL.AppendLine(",:FURIKETU_CODE")                           'FURIKETU_CODE_K 
            SQL.AppendLine(",:FURIKETU_CENTERCODE")                     'FURIKETU_SENTERCODE_K
            SQL.AppendLine(",:SINKI_CODE")                              'SINKI_CODE_K
            SQL.AppendLine(",:NS_KBN")                                  'NS_KBN_K
            SQL.AppendLine(",:TEKIYOU_KBN")                             'TEKIYO_KBN_K
            SQL.AppendLine(",:KTEKIYO")                                 'KTEKIYO_K
            SQL.AppendLine(",:NTEKIYOU")                                'NTEKIYO_K
            SQL.AppendLine(",:JYUYOKA_NO")                              'JYUYOUKA_NO_K
            SQL.AppendLine(",:TEISEI_SIT")                              'TEISEI_SIT_K
            SQL.AppendLine(",:TEISEI_KAMOKU")                           'TEISEI_KAMOKU_K
            SQL.AppendLine(",:TEISEI_KOUZA")                            'TEISEI_KOUZA_K
            SQL.AppendLine(",'00'")                                     'TEISEI_AKAMOKU_K
            SQL.AppendLine(",'0000000'")                                'TEISEI_AKOUZA_K
            SQL.AppendLine(",:DATA_KBN")                                'DATA_KBN_K
            SQL.AppendLine(",:FURI_DATA")                               'FURI_DATA_K
            SQL.AppendLine(",:RECORD_NO")                               'RECORD_NO_K
            SQL.AppendLine(",' '")                                      'SORT_KEY_K
            SQL.AppendLine(",:SAKUSEI_DATE")                            'SAKUSEI_DATE_K
            SQL.AppendLine(",'00000000'")                               'KOUSIN_DATE_K
            SQL.AppendLine(",:YOBI1")                                   'YOBI1_K
            SQL.AppendLine(",:YOBI2")                                   'YOBI2_K
            SQL.AppendLine(",:YOBI3")                                   'YOBI3_K
            '2010/01/19 ���v�Ɣԍ���ϊ��������̂��i�[
            SQL.AppendLine(",:YOBI4")                                   'YOBI4_K
            '=========================================
            '*** Str Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
            'SQL.AppendLine(",' '")                                      'YOBI5_K
            'SQL.AppendLine(",' '")                                      'YOBI6_K
            'SQL.AppendLine(",' '")                                      'YOBI7_K
            'SQL.AppendLine(",' '")                                      'YOBI8_K
            'SQL.AppendLine(",' '")                                      'YOBI9_K
            'SQL.AppendLine(",' '")                                      'YOBI10_K
            SQL.AppendLine(",:YOBI5")                                   'YOBI5_K
            SQL.AppendLine(",:YOBI6")                                   'YOBI6_K
            SQL.AppendLine(",:YOBI7")                                   'YOBI7_K
            SQL.AppendLine(",:YOBI8")                                   'YOBI8_K
            SQL.AppendLine(",:YOBI9")                                   'YOBI9_K
            SQL.AppendLine(",:YOBI10")                                  'YOBI10_K
            '*** End Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

            If aReadFmt.BinMode Then
                'EBCDIC�̏ꍇ
                SQL.AppendLine(",:BIN")                                 'BIN_DATA_K
                SQL.AppendLine(")")
            Else
                SQL.AppendLine(")")
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

                If aReadFmt.IsEndRecord = True AndAlso stMei.SCH_UPDATE_FLG = True Then
                    ' �G���h���R�[�h�̏ꍇ�́C�P�O�̎���SEQ���g�p����
                    .Item("MOTIKOMI_SEQ") = stMei.MOTIKOMI_SEQ - 1
                Else
                    .Item("MOTIKOMI_SEQ") = stMei.MOTIKOMI_SEQ
                End If

                .Item("SYUBETU") = stTori.SYUBETU_T
                .Item("FURI_CODE") = stTori.FURI_CODE_T
                .Item("KIGYO_CODE") = stTori.KIGYO_CODE_T
                .Item("ITAKU_KIN") = stMei.ITAKU_KIN
                .Item("ITAKU_SIT") = stMei.ITAKU_SIT
                .Item("ITAKU_KAMOKU") = stMei.ITAKU_KAMOKU
                .Item("ITAKU_KOUZA") = stMei.ITAKU_KOUZA
                .Item("KEIYAKU_KIN") = stMei.KEIYAKU_KIN
                .Item("KEIYAKU_SIT") = stMei.KEIYAKU_SIT
                .Item("KEIYAKU_NO") = stMei.KEIYAKU_NO
                .Item("KEIYAKU_KNAME") = stMei.KEIYAKU_KNAME
                '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------START
                .Item("KEIYAKU_KAMOKU") = CASTCommon.ConvertKamoku1TO2_K(stMei.KEIYAKU_KAMOKU)
                '.Item("KEIYAKU_KAMOKU") = CASTCommon.onvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)
                '2011/06/28 �W���ŏC�� ���U�̏ꍇ�A�Ȗڂ��̑��́A09�ɕύX ------------------END
                .Item("KEIYAKU_KOUZA") = stMei.KEIYAKU_KOUZA
                .Item("FURIKIN") = stMei.FURIKIN
                .Item("TESUU_KIN") = stMei.TESUU_KIN
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
                .Item("FURI_DATA") = aReadFmt.RecordDataNoChange
                .Item("RECORD_NO") = stMei.RECORD_NO.ToString
                .Item("SAKUSEI_DATE") = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
                .Item("YOBI1") = stMei.YOBI1
                .Item("YOBI2") = stMei.YOBI2
                .Item("YOBI3") = stMei.YOBI3
                '2010/01/19 ���v�Ɣԍ���ϊ����ē���
                Dim wJYUYOUKA As New StringBuilder(24)
                For Each c As Char In stMei.JYUYOKA_NO.PadRight(24)
                    Select Case c
                        '0-9�AA-Z�A(�A)�ASPACE
                        Case "0"c To "9"c, "A"c To "Z"c, "("c To ")"c, " "c
                            wJYUYOUKA.Append(c)
                        Case Else
                            wJYUYOUKA.Append("0"c)
                    End Select
                Next
                .Item("YOBI4") = wJYUYOUKA.ToString
                '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                .Item("YOBI5") = stMei.YOBI5
                .Item("YOBI6") = stMei.YOBI6
                .Item("YOBI7") = stMei.YOBI7
                .Item("YOBI8") = stMei.YOBI8
                .Item("YOBI9") = stMei.YOBI9
                .Item("YOBI10") = stMei.YOBI10
                '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                '===================================

                If aReadFmt.BinMode Then
                    .Item("BIN") = aReadFmt.RecordDataBin
                End If

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("���׃}�X�^�o�^", "���s", MainDB.Message)
                    Return False
                End If
            End With

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�o�^", "���s", ex.Message)
            mErrMessage = "���׃}�X�^�o�^���s"
            Return False
        End Try

        Return True
    End Function

    '2010/02/18 �Ή�����˗����}�X�^�̐V�K�R�[�h���X�V����
    ' �@�\�@ �F �V�K�R�[�h UPDATE
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F �˗����}�X�^�V�K�R�[�h�X�V�p
    '
    Private Function UpdateENTMAST() As Boolean

        Try
            For I As Integer = 1 To 2
                Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
                If I = 1 Then
                    SQL.Append("UPDATE S_ENTMAST SET")
                Else
                    SQL.Append("UPDATE S_FUKU_ENTMAST SET")
                End If

                SQL.Append(" SINKI_CODE_E = '0'")
                SQL.Append(" WHERE TORIS_CODE_E = " & SQ(mKoParam.CP.TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(mKoParam.CP.TORIF_CODE))
                SQL.Append(" AND FURI_DATE_E = " & SQ(mKoParam.CP.FURI_DATE))
                SQL.Append(" AND SINKI_CODE_E = '1'")
                SQL.Append(" AND KAISI_DATE_E <= " & SQ(mKoParam.CP.FURI_DATE)) '�J�n�N�������r

                If MainDB.ExecuteNonQuery(SQL) < 0 Then
                    BatchLog.Write("�˗����}�X�^�i�V�K�R�[�h�j�X�V", "���s", MainDB.Message)
                    Return False
                End If
            Next

        Catch ex As Exception
            BatchLog.Write("�˗����}�X�^�i�V�K�R�[�h�j�X�V", "���s", ex.Message)
            Return False
        End Try
        Return True
    End Function

    ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    '================================
    ' �t�@�C�����\�z
    '================================
    Private Function GetFileName_Edition2(ByVal BaitaiCode As String, ByRef FileName As String) As Boolean

        Dim CheckFileName As String = ""

        Try
            BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "�J�n")

            FileName = ""

            '--------------------------------
            ' �Ɩ�
            '--------------------------------
            If mKoParam.CP.FSYORI_KBN = "1" Then
                CheckFileName = "J_"
            Else
                CheckFileName = "S_"
            End If

            '--------------------------------
            ' �}��
            '--------------------------------
            CheckFileName &= GetTextFileInfo(CASTCommon.GetFSKJIni("COMMON", "TXT") & "�}�̖����K��.txt", BaitaiCode) & "_"

            '--------------------------------
            ' �}���`�敪
            '--------------------------------
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                CheckFileName &= "S_" & mKoParam.CP.TORI_CODE & "_"
            Else
                '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- START
                '�}���`�ł��}�̂��˗����A�`�[�̏ꍇ�̓V���O�������Ƃ���
                Select Case BaitaiCode
                    Case "04", "09"
                        CheckFileName &= "S_" & mKoParam.CP.TORI_CODE & "_"
                    Case Else
                        CheckFileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
                End Select
                'CheckFileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
                '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- END
            End If

            '--------------------------------
            ' �t�H�[�}�b�g�敪
            '--------------------------------
            CheckFileName &= mArgumentData.INFOToriMast.FMT_KBN_T & "_"

            '--------------------------------
            ' �w���(�U�֓��܂��͐U����)
            '--------------------------------
            CheckFileName &= mKoParam.CP.FURI_DATE

            Dim FileList() As String = Nothing
            Select Case BaitaiCode
                '2017/04/10 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
                ' �`���n�}�̒ǉ�(30�`39)
                Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                'Case "00"
                '2017/04/10 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "DEN"))
                Case "01", "05", "06", "11", "12", "13", "14", "15"
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "BAITAIREAD"))
                Case "04", "09"
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "ETC"))
                Case "10"
                    FileList = Directory.GetFiles(GetFSKJIni("WEB_DEN", "WEB_REV"))
            End Select

            BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "����", "�`�F�b�N�t�@�C����:" & CheckFileName)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                If Path.GetFileName(FileList(i)).IndexOf(CheckFileName) = 0 Then
                    FileName = Path.GetFileName(FileList(i))
                    BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "����", "�擾�t�@�C����(" & i & "):" & FileName)
                    Exit For
                End If
            Next

            If FileName = "" Then
                BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "���s", "�擾�t�@�C�����Ȃ�")
                Return False
            Else
                Return True
            End If

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "", ex.Message)
            Return False
        Finally
            BatchLog.Write("", "0000000000-00", "00000000", "�t�@�C�����\�z", "�I��")
        End Try

    End Function

    '================================
    ' �e�L�X�g�t�@�C�����擾
    '================================
    Private Function GetTextFileInfo(ByVal TextFileName As String, ByVal KeyInfo As String) As String

        Dim sr As StreamReader = Nothing

        Try
            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "�J�n", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = KeyInfo Then
                    Return strLineData(1).Trim
                End If
            End While

            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "", "�Y���Ȃ�")
            Return "NON"

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "", ex.Message)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "�I��", "")
        End Try

    End Function
    ' 2016/01/12 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

End Class
