Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic

' �ʗ������ݏ���
Public Class ClsTouroku

    '2013/12/24 saitou �W���� ���g�p DEL -------------------------------------------------->>>>
    'Private clsFusion As New clsFUSION.clsMain
    '2013/12/24 saitou �W���� ���g�p DEL --------------------------------------------------<<<<
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
                CP.FURI_DATE = para(1)                      '�U�֓�
                CP.CODE_KBN = para(2)                       '�R�[�h�敪
                CP.FMT_KBN = para(3).PadLeft(2, "0"c)       '�t�H�[�}�b�g�敪
                CP.BAITAI_CODE = para(4).PadLeft(2, "0"c)   '�}�̃R�[�h
                CP.LABEL_KBN = para(5)                      '���x���敪

                ' 2017/04/12 �^�X�N�j���� CHG �yME�z(RSV2�W���@�\�Ή�) -------------------- START
                'CP.RENKEI_FILENAME = ""                     '�A�g�t�@�C����
                'CP.ENC_KBN = ""                             '�Í��������敪
                'CP.ENC_KEY1 = ""                            '�Í����L�[�P
                'CP.ENC_KEY2 = ""                            '�Í����L�[�Q
                'CP.ENC_OPT1 = ""                            '�`�d�r�I�v�V����
                'CP.CYCLENO = ""                             '�T�C�N����
                'CP.JOBTUUBAN = Integer.Parse(para(6))       '�W���u�ʔ�
                Select Case para.Length
                    Case 7
                        CP.RENKEI_FILENAME = ""                     '�A�g�t�@�C����
                        CP.ENC_KBN = ""                             '�Í��������敪
                        CP.ENC_KEY1 = ""                            '�Í����L�[�P
                        CP.ENC_KEY2 = ""                            '�Í����L�[�Q
                        CP.ENC_OPT1 = ""                            '�`�d�r�I�v�V����
                        CP.CYCLENO = ""                             '�T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(6))       '�W���u�ʔ�
                    Case 8
                        CP.RENKEI_FILENAME = para(6).TrimEnd        '�A�g�t�@�C����
                        CP.ENC_KBN = ""                             '�Í��������敪
                        CP.ENC_KEY1 = ""                            '�Í����L�[�P
                        CP.ENC_KEY2 = ""                            '�Í����L�[�Q
                        CP.ENC_OPT1 = ""                            '�`�d�r�I�v�V����
                        CP.CYCLENO = ""                             '�T�C�N����
                        CP.JOBTUUBAN = Integer.Parse(para(7))       '�W���u�ʔ�
                End Select
                ' 2017/04/12 �^�X�N�j���� CHG �yME�z(RSV2�W���@�\�Ή�) -------------------- END

            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    Dim mUserID As String = ""                      '���[�U�h�c
    Dim mJobMessage As String = ""                  '�W���u���b�Z�[�W
    Private mDataFileName As String                 '�˗��f�[�^�t�@�C����
    Private mArgumentData As CommData               '�N���p�����[�^ ���ʏ��
    Public MainDB As CASTCommon.MyOracle            '�p�u���b�N�c�a
    Private ArrayPrnNenkin As New ArrayList(128)    '�N�� ���l�ʐU�����ו\ �o�̓��X�g
    Private mErrMessage As String = ""              '�G���[���b�Z�[�W(�������ʊm�F�\����p)
    Private ArrayTenbetu As New ArrayList           '�X�ʏW�v�\�o�͑Ώ� �i�[���X�g

    ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- START
    Private INI_RSV2_TENBETUSYUKEI As String = ""
    ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- END

    ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Private INI_RSV2_EDITION As String = ""
    Private INI_RSV2_SYORIKEKKA_TOUROKU As String = ""
    ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
    '��t�����`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Private INI_UKETUKE_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU_CHK")
    '���������`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Private INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
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

            ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- START
            '--------------------------------------------
            ' INI̧�ُ��ݒ�
            '--------------------------------------------
            INI_RSV2_TENBETUSYUKEI = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TENBETUSYUKEI")
            ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- END
            ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            INI_RSV2_SYORIKEKKA_TOUROKU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_TOUROKU")
            ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

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
                        BatchLog.Write_Err("�又��", "���s", "���U�˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        BatchLog.UpdateJOBMASTbyErr("���U�˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        Return -1
                    End If
                End If
                ' 2016/02/08 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
                '�������ʊm�F�\�o�́i�V�X�e���G���[)
                Dim oRenkei As New ClsRenkei(mArgumentData)

                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(mKoParam.CP.FSYORI_KBN,
                        mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE,
                        mKoParam.CP.JOBTUUBAN, Path.GetFileName(mKoParam.CP.RENKEI_FILENAME), mErrMessage, MainDB) = False Then
                    BatchLog.Write("�������ʊm�F�\", "���s", "�t�@�C����:" & Prn.FileName & " ���b�Z�[�W:" & Prn.ReportMessage)
                End If

                Prn = Nothing

            Else
                '��������I��
                Dim DestFile As String = ""
                '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- START
                Dim IRAI_BK_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_BKUP_MODE")
                '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�˗��f�[�^�̑ޔ�ݒ�j---------------------- END
                Try

                    Select Case InfoParam.BAITAI_CODE
                        ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
                        'Case "00"                       '2010.02.04 �w�Z��DAT��
                        Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END

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
                            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                        Case "01", "07", "11", "12", "13", "14", "15"
                            'Case "01", "07", "11", "12", "13", "14"                 '2010.02.04 �w�Z��DAT��       ''2012/7/3  mubuchi "11"(DVD)�ǉ�
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
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
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
                                Case Else
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
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
                            ' 2016/01/27 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                        Case "SA"   '�ĐU�p����}�̋敪
                            DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "S" & mKoParam.CP.TORI_CODE & ".DAT"
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
                            '2009/12/09 �ǉ� ================================
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

            ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- START
            'If bRet = True Then
            '    '�����U�֓X�ʏW�v�\�i�o�͏����Ƃ��ĕs�\���׋敪���Q�Ƃ���j ���@2009.11.11 �Q�Ƃ��Ȃ�
            '    Dim ExeRepo As New CAstReports.ClsExecute '���|�G�[�W�F���g���
            '    For i As Integer = 0 To ArrayTenbetu.Count - 1
            '        Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
            '        'Select Case prnTenbetu(0)
            '        '    Case "1", "2", "3"
            '        Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP019.EXE", Param)
            '        If nRet <> 0 Then
            '            BatchLog.Write("�����U�֓X�ʏW�v�\�o��", "���s", "���A�l�F" & nRet)
            '        End If
            '        'End Select
            '    Next
            'End If
            If INI_RSV2_TENBETUSYUKEI = "1" Then
                '--------------------------------------------
                ' �����U�֓X�ʏW�v�\���
                '--------------------------------------------
                If bRet = True Then
                    Dim ExeRepo As New CAstReports.ClsExecute
                    For i As Integer = 0 To ArrayTenbetu.Count - 1
                        Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                        Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
                        Dim nRet As Integer = ExeRepo.ExecReport("KFJP019.EXE", Param)
                        If nRet <> 0 Then
                            BatchLog.Write("�����U�֓X�ʏW�v�\�o��", "���s", "���A�l�F" & nRet)
                        End If
                    Next
                End If
            End If
            ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-03(RSV2�Ή�) -------------------- END

            ' 2016/01/28 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            If bRet = True Then
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
            End If
            ' 2016/01/28 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

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
                Call mArgumentData.GetTORIMAST(mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE)

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
            'If mKoParam.CP.FMT_KBN <> "04" And mKoParam.CP.FMT_KBN <> "05" Then
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
                'oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
                '*** Str Add 2015/12/26 sys)mori for �G���[�̂��߁A�C�� ***
                oReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                'oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

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
            '    'mErrMessage = "�}�̓ǂݍ��ݎ��s"�@�֐����Őݒ�
            '    Return False
            'End If
            If sReadFile = "" Then
                oReadFMT.Close()
                BatchLog.Write("(�o�^���C������)", "���s", "�}�̓ǂݍ��݁F" & oReadFMT.Message)
                Return False
            Else
                '--------------------------------------------
                ' �U�ֈ˗��f�[�^(���ԃt�@�C��)�̑ޔ�
                '--------------------------------------------
                Dim BKUP_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_CHECK_IN")
                If BKUP_CHECK <> "err" Then
                    If BKUP_CHECK = "1" Then
                        Dim BKUP_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_BAITAI_IN")

                        If BKUP_BAITAI.IndexOf(mKoParam.CP.BAITAI_CODE) >= 0 Then
                            Dim BKUP_FOLDER As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_FOLDER_IN")
                            Dim BKUP_FILENAME As String = BKUP_FOLDER & Path.GetFileNameWithoutExtension(sReadFile) & "_" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & ".DAT"

                            Try
                                File.Copy(sReadFile, BKUP_FILENAME, True)
                            Catch ex As Exception
                                BatchLog.Write("(�o�^���C������)", "���s", "�U�ֈ˗��f�[�^�ޔ��F" & ex.Message)
                            End Try
                        End If
                    End If
                End If
            End If
            ' 2016/05/16 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�_�򕌋@�\�����R��) -------------------- END

            '--------------------------------------------
            '�f�[�^�`�F�b�N
            '--------------------------------------------
            '�`���C�e�c�C�l�s�C�b�l�s,�c�u�c
            '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
            '�\�[�X���P
            Select Case mKoParam.CP.BAITAI_CODE
                ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- START
                'Case "00", "01", "05", "06", "04", "09", "07", "10", "11", "12", "13", "14", "15", "SA"
                Case "00", "01", "05", "06", "04", "09", "07", "10", "11", "12", "13", "14", "15", "SA",
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    ' 2016/11/21 �^�X�N�j���� CHG �yPG�zUI_99-99(�ѓc�M�� �`���n�}�̒ǉ�(30�`39) -------------------- END

                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
                    If mKoParam.CP.CODE_KBN = "2" OrElse mKoParam.CP.CODE_KBN = "3" Then
                        '���R�[�h�����ς�邽�ߍĐݒ肷��(120��������...)
                        oReadFMT = New CAstFormat.CFormatZengin
                    End If
                    '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END

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
                            BatchLog.Write("(�o�^���C������)", "����", "�K��O��������F" &
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


            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "04" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "09" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "07" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "10" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "11" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "12" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "13" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "14" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "SA" Then '�}��"04","09","SA","07"�ǉ�     '20120618 mubuchi "11"(DVD)�ǉ�
            '    '2012/06/30 �W���Ł@WEB�`���Ή��@�}��"10"�ǉ�
            '    '20130606 maeda ���̑��}�̒ǉ�


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
            '=====================================================

            '2012/06/30 �W���Ł@WEB�`���A�g-------------------------->
            If mKoParam.CP.BAITAI_CODE = "10" Then
                If UpdateWEB_RIREKIMAST() = False Then
                    BatchLog.Write("WEB�����}�X�^�X�V", "���s", "")

                    oReadFMT.Close()
                    '���[���o�b�N
                    MainDB.Rollback()
                    Return False
                End If
            End If
            '--------------------------------------------------------<

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
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            SQL.Append(" FOR UPDATE WAIT " & LockWaitTime)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                    If oReader.Message <> "" Then
                        Dim errmsg As String
                        If oReader.Message.StartsWith("ORA-30006") Then
                            errmsg = "�������ݏ����Ŏ��s�҂��^�C���A�E�g"
                        Else
                            errmsg = "�������ݏ������b�N�ُ�"
                        End If

                        BatchLog.Write_Err("�������ݏ���", "���s", errmsg & " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                        Call BatchLog.UpdateJOBMASTbyErr(errmsg & " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                        mErrMessage = errmsg
                        Return False
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                    BatchLog.Write("�X�P�W���[���Ȃ�", "���s", " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
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
                    BatchLog.Write("�X�P�W���[��", "���s", "�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & "�����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("�X�P�W���[��:�������ݏ����� �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "�������ݏ�����"
                    Return False
                End If

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                ' �}���`�敪�̏ꍇ
                ' ����̎����啛�R�[�h���}���`�敪=1�̎����}�X�^���������A�����}�X�^�́u��\�ϑ��҃R�[�h(ITAKU_KANRI_CODE_T)�v
                ' �Ɠ����U�֓��̎����̃X�P�W���[�������b�N����B�i�Ώۂ�0���̏ꍇ�́A���̂܂܌㑱�������s�B�j
                If mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                    oReader.Close()

                    ' ��\�ϑ��҃R�[�h�擾
                    SQL = New StringBuilder(256)
                    SQL.Append("SELECT ")
                    SQL.Append(" ITAKU_KANRI_CODE_T")
                    SQL.Append(" FROM TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mKoParam.CP.TORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(mKoParam.CP.TORIF_CODE))
                    SQL.Append(" AND MULTI_KBN_T  = '1'")

                    If oReader.DataReader(SQL) = True Then
                        Dim itaku_kanri_code As String = oReader.GetString("ITAKU_KANRI_CODE_T")

                        oReader.Close()

                        ' �}���`�敪���̃X�P�W���[�����b�N
                        SQL = New StringBuilder(256)
                        SQL.Append("SELECT FURI_DATE_S")
                        SQL.Append(" FROM SCHMAST , TORIMAST")
                        SQL.Append(" WHERE ITAKU_KANRI_CODE_T = " & SQ(itaku_kanri_code))
                        SQL.Append(" AND TORIS_CODE_T       = TORIS_CODE_S")
                        SQL.Append(" AND TORIF_CODE_T       = TORIF_CODE_S")
                        SQL.Append(" AND FURI_DATE_S        = " & SQ(mKoParam.CP.FURI_DATE))
                        SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & LockWaitTime)
                        If oReader.DataReader(SQL) = False Then
                            If oReader.Message <> "" Then
                                Dim errmsg As String
                                If oReader.Message.StartsWith("ORA-30006") Then
                                    errmsg = "�������ݏ����Ŏ��s�҂��^�C���A�E�g"
                                Else
                                    errmsg = "�������ݏ������b�N�ُ�"
                                End If

                                BatchLog.Write_Err("�������ݏ���", "���s", errmsg & " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                                Call BatchLog.UpdateJOBMASTbyErr(errmsg & " �U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                                mErrMessage = errmsg
                                Return False
                            End If
                        End If
                    End If
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***


                Return True

            Catch ex As Exception
                BatchLog.Write("�X�P�W���[������", "���s", "�U�֓��F" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy�NMM��dd��"))
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
        Dim MediaRead As New clsMediaRead

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

                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        'nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
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

                                nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
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
                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

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

                        ' ''�l�s�t�@�C���Ǎ��������s
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
                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
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
                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        'nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

                    Case "07"       '2010.02.04 �ǉ�

                        Dim REC_LENGTH As Integer = 120
                        '�˗��t�@�C���R�s�[��ݒ�
                        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DAT") & "D" & .TORI_CODE & ".DAT"
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)

                        '******20120618 mubuchi DVD�Ή�*********************************************
                        '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                    Case "11", "12", "13", "14", "15"
                        'Case "11", "12", "13", "14"       'DVD
                        '2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<

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


                        'DVD�Ǎ��������s

                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''�R�[�h�敪�`�F�b�N�p�̃��[�N�t�@�C���p�X�p
                        'Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)


                        ''2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                        ''�}�̃R�[�h�������ɒǉ����A�}�̃R�[�h�ɂ���đΏۃp�X��ύX����悤�ɏ�����ύX
                        'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                        '                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)
                        ''nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle, readfmt)
                        ''2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<

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


                                '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                                '�}�̃R�[�h�������ɒǉ����A�}�̃R�[�h�ɂ���đΏۃp�X��ύX����悤�ɏ�����ύX
                                nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH,
                                                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)
                                'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle, readfmt)
                                '2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<

                                '2009/11/30 �R�[�h�敪�`�F�b�N�ǉ�===
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
                                '===================================
                        End Select
                        ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        '------------------------------------------------------------
                        '���̕����R�[�h�̃f�[�^���ĕϊ����ď����ɏ悹��
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If
                        '******20120618 mubuchi DVD�Ή� END*********************************************

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

                            ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
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
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName_Edition2)
                                Case Else
                                    If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                        Else
                            ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            '2014/05/21 saitou �W���� MODIFY ----------------------------------------------->>>>
                            ''�`���̏ꍇ�A�����}�X�^�̃t�@�C�����͎g�p���Ȃ��i���ɖ߂��j
                            'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            'Else
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
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
                                    ' 2017/04/12 �^�X�N�j���� CHG �yME�z(RSV2�W���@�\�Ή�) -------------------- START
                                    'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                    'Else
                                    '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                    'End If
                                    If mKoParam.CP.RENKEI_FILENAME.Trim = "" Then
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                                    ' 2017/04/12 �^�X�N�j���� CHG �yME�z(RSV2�W���@�\�Ή�) -------------------- END
                            End Select
                            ' 2016/01/11 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

                            ''2010/09/08.Sakon�@�����}�X�^�̃t�@�C������D�悵�Ďg�p���� ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            'If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
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
                            ''If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            ''    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            ''Else
                            ''    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            ''End If
                            ''+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            ''mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            '2014/05/21 saitou �W���� MODIFY -----------------------------------------------<<<<
                        End If

                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

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
                        '2011/06/15 �W���ŏC�� �ĐU�̏ꍇ,�R�[�h�敪���`�F�b�N�����Ȃ�-----------START
                        '2009/11/30 �R�[�h�敪�`�F�b�N�ǉ�===
                        'If nRetRead = 0 Then
                        If nRetRead = 0 And Baitai_Code <> "SA" Then
                            '2011/06/15 �W���ŏC�� �ĐU�̏ꍇ,�R�[�h�敪���`�F�b�N�����Ȃ�-----------END
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
                '2010/10/07.Sakon�@�C���v�b�g�G���[��z�M���邩(���ʃR�[�h��9���Z�b�g���邩) +++++
                '�C���v�b�g�G���[�z�M�ۃt���O�`�F�b�N
                If aReadFmt.SouInputErr = "ERR" Then
                    BatchLog.Write("�C���v�b�g�G���[�z�M�۔���", "���s", aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr("�C���v�b�g�G���[�z�M�۔��莸�s")
                    mErrMessage = "�C���v�b�g�G���[�z�M�۔��莸�s"
                    Return False
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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

                            ''���z�ُ� �t�H�[�}�b�g�G���[

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
                                    BatchLog.Write("���׃}�X�^�o�^����", "���s", nRecordCount.ToString & "�s�� ��\�ϑ��҃R�[�h�s��v" &
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "�s�� ��\�ϑ��҃R�[�h�s��v" &
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

                        '0:��Ώ�,1:�X�ԃ\�[�g,2:��\�[�g,3:�G���[���̂�,4�ȍ~�g���p
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
                            If aReadFmt.InfoMeisaiMast.KEIYAKU_KIN <> aReadFmt.InfoMeisaiMast.OLD_KIN_NO OrElse
                               aReadFmt.InfoMeisaiMast.KEIYAKU_SIT <> aReadFmt.InfoMeisaiMast.OLD_SIT_NO OrElse
                               aReadFmt.InfoMeisaiMast.KEIYAKU_KOUZA <> aReadFmt.InfoMeisaiMast.OLD_KOUZA Then
                                '2010/02/04 ���Ńf�[�^�̏ꍇ�̓f�[�^�敪3�̂ݏo�͑Ώۂɂ���
                                If mKoParam.CP.FMT_KBN = "02" Then
                                    If aReadFmt.RecordData.Substring(0, 1) = "3" Then
                                        '���񏈗�(����t���O�𗧂Ă�^�b�r�u�t�@�C�����쐬����)
                                        If bSitenYomikaePrint = False Then
                                            bSitenYomikaePrint = True
                                            PrnSitenYomikae.CreateCsvFile()
                                        End If
                                        '�f�[�^�����o��
                                        PrnSitenYomikae.OutputCsvData(aReadFmt)
                                    End If
                                Else
                                    '���񏈗�(����t���O�𗧂Ă�^�b�r�u�t�@�C�����쐬����)
                                    If bSitenYomikaePrint = False Then
                                        bSitenYomikaePrint = True
                                        PrnSitenYomikae.CreateCsvFile()
                                    End If
                                    '�f�[�^�����o��
                                    PrnSitenYomikae.OutputCsvData(aReadFmt)
                                End If
                            End If
                        End If

                        '���łłȂ��B�܂��̓f�[�^�敪���R�i���ł��o��j
                        If aReadFmt.ToriData.INFOToriMast.FMT_KBN_T <> "02" OrElse aReadFmt.InfoMeisaiMast.DATA_KBN = "3" Then

                            '��t���ׂb�r�u�o��
                            If Not PrnMeisai Is Nothing Then
                                ' 0:��Ώ�,1:�X�ԃ\�[�g,2:��\�[�g,3:�G���[���̂�
                                ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- START
                                'Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
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
                                        Dim UketukeErrOut As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST010_��t���ו\�o�͋敪.TXT"),
                                                                                                    aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T,
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
                    End If

                    ' ��d�����`�F�b�N(�T�C�N���l�����s��Ȃ��B���U�ɂ͕K�v�ɂȂ邩���H)
                    'If aReadFmt.ToriData.INFOToriMast.CYCLE_T = "1" Then
                    If nRecordNumber <= 6 Then
                        ' �ŏ��̂U��
                        ArrayMeiData.Add(aReadFmt.RecordData)
                    ElseIf sCheckRet = "T" Then
                        ' �g���[�����R�[�h
                        ArrayMeiData.Add(aReadFmt.RecordData)
                    End If
                    'End If


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
                            '���ڎ����t�H�[�}�b�g�ȊO
                            '�G���h���R�[�h�����݂��Ȃ��ꍇ������̂Ń`�F�b�N���Ȃ�
                            If aReadFmt.ToriData.INFOParameter.FMT_KBN <> "TO" Then
                                BatchLog.Write("�t�H�[�}�b�g�G���[", "���s", (nRecordCount + 1).ToString & "�s�� �G���h���R�[�h������܂���")
                                BatchLog.UpdateJOBMASTbyErr((nRecordCount + 1).ToString & "�s�� �G���h���R�[�h������܂���")
                                mErrMessage = (nRecordCount + 1).ToString & "�s�� ����ں��ނȂ�"
                                bRet = False
                                Exit Do
                            End If
                        End If

                        If PrnFlag = True Then

                            ' ���׃}�X�^���������C�Q�d�����̌������s��
                            Dim bDupicate As Boolean = False
                            'bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData) '���U�p
                            If bDupicate = True Then
                                mJobMessage = "��d��������"
                            End If

                            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
                            If INI_UKETUKE_KIJITU_CHK = "1" Then
                                '��t�����`�F�b�N
                                MotikomiOver = aReadFmt.CheckKaisiDate(aReadFmt.ToriData.INFOParameter.FURI_DATE)
                                If MotikomiOver = False Then
                                    mJobMessage = "��t�J�n�O"
                                    mArgumentData.Syorikekka_Bikou = mJobMessage    '�������ʊm�F�\�̔��l���ɏo��
                                    BatchLog.Write("��t�����`�F�b�N����", "����", mJobMessage)
                                End If
                            End If

                            If INI_MOTIKOMI_KIJITU_CHK = "1" Then
                                '��t�����`�F�b�N���G���[�łȂ���΃`�F�b�N����
                                If MotikomiOver = True Then
                                    '���������`�F�b�N
                                    MotikomiOver = aReadFmt.CheckMotikomiDate()
                                    If MotikomiOver = False Then
                                        mJobMessage = "������������"
                                        mArgumentData.Syorikekka_Bikou = mJobMessage    '�������ʊm�F�\�̔��l���ɏo��
                                        BatchLog.Write("���������`�F�b�N����", "����", mJobMessage)
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
                                        PrnInErrList.HostCsvName = "KFJP061_"
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnInErrList.HostCsvName &= "0000.CSV"
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
                                    Dim SortParam As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST010_��t���ו\�o�͋敪.TXT"),
                                                                                            aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T,
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
                                        PrnMeisai.HostCsvName = "KFJP003_"
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnMeisai.HostCsvName &= "0000.CSV"
                                        ArrayPrnMeisai.Add(PrnMeisai)
                                        PrnMeisai = Nothing
                                    End If
                                    IOfileStream.Close()
                                    IOfileStream = Nothing
                                    'ArrayPrnMeisai.Add(PrnMeisai)
                                    'PrnMeisai = Nothing
                                    ' 2016/03/03 �^�X�N�j��� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
                                End If
                            End If

                            '�X�ʏW�v�\����p���X�g�Ɋi�[(����敪,������R�[�h,����敛�R�[�h)
                            ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," &
                                             aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," &
                                             aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)

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
                        BatchLog.Write_Err("�又��", "���s", "���U�˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
                        BatchLog.UpdateJOBMASTbyErr("���U�˗��f�[�^���������Ŏ��s�҂��^�C���A�E�g")
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
                                PrnInErrList.HostCsvName = "KFJP061_"
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                PrnInErrList.HostCsvName &= "0000.CSV"
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
                                    BatchLog.Write("�C���v�b�g�G���[���X�g�b�r�u�o�͏���", "���s", ex.Message)
                                End Try
                            End If
                            'Try
                            '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            '    DestName &= PrnInErrList.HostCsvName
                            '    File.Copy(PrnInErrList.FileName, DestName, True)
                            '    BatchLog.Write("�C���v�b�g�G���[�b�r�u�o�͏���", "����", DestName)
                            '    PrnInErrList = Nothing
                            'Catch ex As Exception
                            '    BatchLog.Write("�C���v�b�g�G���[���X�g�b�r�u�o�͏���", "���s", ex.Message)
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

                '�����G���h���R�[�h��ʂ�
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

                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnMeisai.HostCsvName

                    '    If File.Exists(PrnMeisai.FileName) = True Then
                    '        File.Copy(PrnMeisai.FileName, DestName, True)
                    '    Else
                    '        BatchLog.Write("��t���ו\�o�� ���ׂO��", "����", "�o�͒��[�Ȃ�")
                    '    End If

                    '    PrnMeisai = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("��t���ו\�b�r�u�o�͏���", "���s", ex.Message)
                    'End Try

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
                    '�������ʊm�F�\���������
                    BatchLog.Write("�C���v�b�g�`�F�b�N", "���s", ErrText)
                    BatchLog.UpdateJOBMASTbyErr("�C���v�b�g�`�F�b�N�G���[ " & ErrText)
                    mErrMessage = ErrText
                    'BatchLog.JobMastMessage = ErrText
                    Return False
                End If

                '�������ʊm�F�\���������
                ' 2016/01/13 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                'If Not PrnSyoKekka Is Nothing Then
                '    If PrnSyoKekka.ReportExecute() = False Then
                '        BatchLog.Write("�������ʊm�F�\�o��", "���s", "�t�@�C����:" & PrnSyoKekka.FileName & " ���b�Z�[�W:" & PrnSyoKekka.ReportMessage)
                '    End If
                'End If
                '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
                '��t�����^���������`�F�b�N�G���[�̏ꍇ�͏o�͂���i�e�`�F�b�N�@�\���L���̏ꍇ�j
                If INI_RSV2_SYORIKEKKA_TOUROKU = "1" OrElse MotikomiOver = False Then
                    'If INI_RSV2_SYORIKEKKA_TOUROKU = "1" Then
                    '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("�������ʊm�F�\�o��", "���s", "�t�@�C����:" & PrnSyoKekka.FileName & " ���b�Z�[�W:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If
                End If
                ' 2016/01/13 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

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
            BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")

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
            '2010/01/19 ���v�Ɣԍ���ϊ��������̂��i�[
            'SQL.Append(",' '")                                      'YOBI4_K    
            SQL.Append(",:YOBI4")                                   'YOBI4_K    
            '=========================================

            '2010/09/21.Sakon�@�\���T�ɂ̓G���[���b�Z�[�W���i�[ ++++++++++++++++++++++++++++++++++++++++++++++++
            SQL.Append(",:YOBI5")                                   'YOBI5_K
            'SQL.Append(",' '")                                      'YOBI5_K
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '*** Str Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
            'SQL.Append(",' '")                                      'YOBI6_K
            'SQL.Append(",' '")                                      'YOBI7_K
            'SQL.Append(",' '")                                      'YOBI8_K
            'SQL.Append(",' '")                                      'YOBI9_K
            'SQL.Append(",' '")                                      'YOBI10_K
            SQL.Append(",:YOBI6")                                   'YOBI6_K
            SQL.Append(",:YOBI7")                                   'YOBI7_K
            SQL.Append(",:YOBI8")                                   'YOBI8_K
            SQL.Append(",:YOBI9")                                   'YOBI9_K
            SQL.Append(",:YOBI10")                                  'YOBI10_K
            '*** End Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

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
        Finally
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
                .Item("FURI_DATA") = aReadFmt.RecordDataNoChange
                .Item("RECORD_NO") = stMei.RECORD_NO.ToString
                .Item("SAKUSEI_DATE") = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
                .Item("YOBI1") = stMei.YOBI1
                .Item("YOBI2") = stMei.YOBI2
                .Item("YOBI3") = stMei.YOBI3
                '2010/01/19 ���v�Ɣԍ���ϊ����ē���
                Dim W_JYUYOUKA As String = ""
                Dim IN_JYUYOUKA As String = stMei.JYUYOKA_NO.PadRight(24)
                For LOO As Integer = 1 To 24
                    Select Case Asc(Mid(IN_JYUYOUKA, LOO, 1))
                        Case 48 To 57, 65 To 90, 40 To 41, 32
                            W_JYUYOUKA &= Mid(IN_JYUYOUKA, LOO, 1)
                        Case Else
                            W_JYUYOUKA &= "0"
                    End Select
                Next
                .Item("YOBI4") = W_JYUYOUKA
                '===================================

                '2010/09/21.Sakon�@��t���א����o�͑Ή��F�\���T�ɃG���[�����i�[ ++++++++++++++++++++++++++++++
                If aReadFmt.InErrorArray.Count = 0 OrElse stMei.DATA_KBN <> "2" Then
                    .Item("YOBI5") = ""
                Else
                    .Item("YOBI5") = CType(aReadFmt.InErrorArray(0), CAstFormat.CFormat.INPUTERROR).ERRINFO
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                .Item("YOBI6") = stMei.YOBI6
                .Item("YOBI7") = stMei.YOBI7
                .Item("YOBI8") = stMei.YOBI8
                .Item("YOBI9") = stMei.YOBI9
                .Item("YOBI10") = stMei.YOBI10
                '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

                If stTori.FSYORI_KBN_T = "1" Then

                    ' ���U�̏ꍇ
                    If aReadFmt.BinMode Then
                        .Item("BIN") = aReadFmt.RecordDataBin
                    End If

                End If

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("���׃}�X�^�o�^", "���s", MainDB.Message)
                    Return False
                End If

                '2013/12/24 saitou �W���� DEL -------------------------------------------------->>>>
                '�N���̓Z���^�[���ڎ����ɕύX
                ''�N���̏ꍇ�́CNENKINMAST �ւ�INSERT ����
                'If aReadFmt.ToriData.INFOParameter.FMT_KBN = "03" Then
                '    If InsertNENKINMAST(aReadFmt) = False Then
                '        Return False
                '    End If
                'End If
                '2013/12/24 saitou �W���� DEL --------------------------------------------------<<<<

            End With

        Catch ex As Exception
            BatchLog.Write("���׃}�X�^�o�^", "���s", ex.Message)
            mErrMessage = "���׃}�X�^�o�^���s"
            Return False
        Finally
        End Try

        Return True
    End Function

    '2013/12/24 saitou �W���� DEL -------------------------------------------------->>>>
    '�N���̓Z���^�[���ڎ����ɕύX
    '' �@�\�@ �F �N���}�X�^ INSERT
    ''
    '' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ''
    '' ���l�@ �F ��InsertMEIMAST�Ƃقړ���
    ''
    'Private Function InsertNENKINMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
    '    Dim SQL As StringBuilder         ' �r�p�k

    '    Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' �������
    '    Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' ���׃}�X�^���

    '    Try
    '        SQL = New System.Text.StringBuilder("INSERT INTO ", 2048)
    '        SQL.Append(" NENKINMAST(")
    '        SQL.Append(" FSYORI_KBN_K")
    '        SQL.Append(",TORIS_CODE_K")
    '        SQL.Append(",TORIF_CODE_K")
    '        SQL.Append(",FURI_DATE_K")
    '        SQL.Append(",KIGYO_CODE_K")
    '        SQL.Append(",KIGYO_SEQ_K")
    '        SQL.Append(",ITAKU_KIN_K")
    '        SQL.Append(",ITAKU_SIT_K")
    '        SQL.Append(",ITAKU_KAMOKU_K")
    '        SQL.Append(",ITAKU_KOUZA_K")
    '        SQL.Append(",KEIYAKU_KIN_K")
    '        SQL.Append(",KEIYAKU_SIT_K")
    '        SQL.Append(",KEIYAKU_NO_K")
    '        SQL.Append(",KEIYAKU_KNAME_K")
    '        SQL.Append(",KEIYAKU_KAMOKU_K")
    '        SQL.Append(",KEIYAKU_KOUZA_K")
    '        SQL.Append(",FURIKIN_K")
    '        SQL.Append(",FURIKETU_CODE_K")
    '        SQL.Append(",FURIKETU_CENTERCODE_K")
    '        SQL.Append(",SINKI_CODE_K")
    '        SQL.Append(",NS_KBN_K")
    '        SQL.Append(",TEKIYO_KBN_K")
    '        SQL.Append(",KTEKIYO_K")
    '        SQL.Append(",NTEKIYO_K")
    '        SQL.Append(",JYUYOUKA_NO_K")
    '        SQL.Append(",MINASI_K")
    '        SQL.Append(",TEISEI_SIT_K")
    '        SQL.Append(",TEISEI_KAMOKU_K")
    '        SQL.Append(",TEISEI_KOUZA_K")
    '        SQL.Append(",TEISEI_AKAMOKU_K")
    '        SQL.Append(",TEISEI_AKOUZA_K")
    '        SQL.Append(",DATA_KBN_K")
    '        SQL.Append(",FURI_DATA_K")
    '        SQL.Append(",RECORD_NO_K")
    '        SQL.Append(",SORT_KEY_K")
    '        SQL.Append(",SAKUSEI_DATE_K")
    '        SQL.Append(",KOUSIN_DATE_K")
    '        SQL.Append(",YOBI1_K")
    '        SQL.Append(",YOBI2_K")
    '        SQL.Append(",YOBI3_K")
    '        SQL.Append(",YOBI4_K")
    '        SQL.Append(",YOBI5_K")
    '        SQL.Append(",YOBI6_K")
    '        SQL.Append(",YOBI7_K")
    '        SQL.Append(",YOBI8_K")
    '        SQL.Append(",YOBI9_K")
    '        SQL.Append(",YOBI10_K")

    '        SQL.Append(")")
    '        SQL.Append(" VALUES(")
    '        SQL.Append(" " & SQ(stTori.FSYORI_KBN_T))                               ' FSYORI_KBN_K
    '        SQL.Append("," & SQ(stTori.TORIS_CODE_T))                               ' TORIS_CODE_K             
    '        SQL.Append("," & SQ(stTori.TORIF_CODE_T))                               ' TORIF_CODE_K
    '        SQL.Append("," & SQ(stMei.FURIKAE_DATE))                                ' FURI_DATE_K
    '        SQL.Append("," & SQ(stTori.KIGYO_CODE_T))                               ' KIGYO_CODE_K
    '        SQL.Append("," & SQ(stMei.KIGYO_SEQ))                                   ' KIGYO_SEQ_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KIN))                                   ' ITAKU_KIN_K
    '        SQL.Append("," & SQ(stMei.ITAKU_SIT))                                   ' ITAKU_SIT_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KAMOKU))                                ' ITAKU_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KOUZA))                                 ' ITAKU_KOUZA_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KIN))                                 ' KEIYAKU_KIN_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_SIT))                                 ' KEIYAKU_SIT_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_NO))                                  ' KEIYAKU_NO_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KNAME))                               ' KEIYAKU_KNAME_K
    '        SQL.Append("," & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU))) ' KEIYAKU_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KOUZA))                               ' KEIYAKU_KOUZA_K
    '        SQL.Append("," & stMei.FURIKIN)                                         ' FURIKIN_K
    '        SQL.Append("," & stMei.FURIKETU_CODE)                                   ' FURIKETU_CODE_K 
    '        SQL.Append("," & stMei.FURIKETU_CENTERCODE)                             ' FURIKETU_SENTERCODE_K
    '        SQL.Append("," & SQ(stMei.SINKI_CODE))                                  ' SINKI_CODE_K
    '        SQL.Append("," & SQ(stTori.NS_KBN_T))                                   ' NS_KBN_K
    '        SQL.Append("," & SQ(stTori.TEKIYOU_KBN_T))                              ' TEKIYO_KBN_K
    '        SQL.Append("," & SQ(stMei.KTEKIYO))                                     ' KTEKIYO_K
    '        SQL.Append("," & SQ(stTori.NTEKIYOU_T))                                 ' NTEKIYO_K
    '        SQL.Append("," & SQ(stMei.JYUYOKA_NO))                                  ' JYUYOUKA_NO_K
    '        SQL.Append(",'0'")                                                      ' MINASI_K
    '        SQL.Append("," & SQ(stMei.TEISEI_SIT))                                  ' TEISEI_SIT_K
    '        SQL.Append("," & SQ(stMei.TEISEI_KAMOKU))                               ' TEISEI_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.TEISEI_KOUZA))                                ' TEISEI_KOUZA_K
    '        SQL.Append(",'00'")                                                     ' TEISEI_AKAMOKU_K
    '        SQL.Append(",'0000000'")                                                ' TEISEI_AKOUZA_K
    '        SQL.Append("," & SQ(stMei.DATA_KBN))                                    ' DATA_KBN_K
    '        SQL.Append("," & SQ(aReadFmt.RecordDataNoChange))                       ' FURI_DATA_K
    '        SQL.Append("," & stMei.RECORD_NO.ToString)                              ' RECORD_NO_K
    '        SQL.Append(",' '")                                                      ' SORT_KEY_K
    '        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))      ' SAKUSEI_DATE_K
    '        SQL.Append(",'00000000'")                                               ' KOUSIN_DATE_K
    '        SQL.Append("," & SQ(stMei.YOBI1))                                       ' YOBI1_K
    '        SQL.Append("," & SQ(stMei.YOBI2))                                       ' YOBI2_K
    '        SQL.Append("," & SQ(stMei.YOBI3))                                       ' YOBI3_K
    '        SQL.Append(",' '")                                                      ' YOBI4_K
    '        SQL.Append(",' '")                                                      ' YOBI5_K
    '        SQL.Append(",' '")                                                      ' YOBI6_K
    '        SQL.Append(",' '")                                                      ' YOBI7_K
    '        SQL.Append(",' '")                                                      ' YOBI8_K
    '        SQL.Append(",' '")                                                      ' YOBI9_K
    '        SQL.Append(",' '")                                                      ' YOBI10_K
    '        SQL.Append(")")

    '        If MainDB.ExecuteNonQuery(SQL) <= 0 Then
    '            BatchLog.Write("�N���}�X�^�o�^", "���s", MainDB.Message)
    '            Return False
    '        End If

    '    Catch ex As Exception
    '        BatchLog.Write("�N���}�X�^�o�^", "���s", ex.Message)
    '        Return False
    '    End Try

    '    Return True
    'End Function
    '2013/12/24 saitou �W���� DEL --------------------------------------------------<<<<

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
        Finally
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
            BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")

            For I As Integer = 1 To 2
                Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
                If I = 1 Then
                    SQL.Append("UPDATE ENTMAST SET")
                Else
                    SQL.Append("UPDATE FUKU_ENTMAST SET")
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
        Finally
        End Try
        Return True
    End Function
    '' �@�\�@ �F ���Ł@�����擾
    ''
    '' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ''
    '' ���l�@ �F 
    ''
    'Private Function GetKokuzeiTORIMAST() As String
    '    Dim SQL As New StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader

    '    Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
    '    Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
    '    Call KokuzeiFmt.FirstRead(oRenkei.InFileName)
    '    KokuzeiFmt.GetFileData(KokuzeiFmt.KOKUZEI_REC1.Data)
    '    Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
    '    KokuzeiFmt.Close()
    '    oRenkei = Nothing

    '    Dim ToriCode As String
    '    '�ȖڃR�[�h�@020:�\��������, 300:����ŋy�n�������
    '    ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
    '    If ToriCode <> "err" Then
    '        Return ToriCode
    '    End If

    '    '��{�I�ɂ́C��L�̂h�m�h�t�@�C������擾�ƂȂ邪�C�h�m�h��������Ȃ��ꍇ�c�a���猟��
    '    SQL.Append("SELECT ")
    '    SQL.Append(" TORIS_CODE_T")
    '    SQL.Append(",TORIF_CODE_T")
    '    SQL.Append(" FROM TORIMAST")

    '    '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
    '    'SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mKoParam.CP.RENKEI_KBN))
    '    'SQL.Append("   AND FMT_KBN_T = '02'")
    '    SQL.Append(" WHERE FMT_KBN_T = '02'")
    '    '+++++++++++++++++++++++++++++++++++++++++++++++

    '    SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
    '    SQL.Append(" ORDER BY TORIS_CODE_T ASC, TORIF_CODE_T ASC")
    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(MainDB)
    '        If OraReader.DataReader(SQL) = True Then
    '            If Kamoku <> "020" Then
    '                OraReader.NextRead()
    '                If OraReader.EOF = True Then
    '                    OraReader.DataReader(SQL)
    '                End If
    '            End If
    '            ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
    '            Return ToriCode.PadRight(9, " "c)
    '        End If
    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then
    '            OraReader.Close()
    '        End If
    '        OraReader = Nothing
    '    End Try

    '    Return New String(" "c, 9)
    'End Function

    '2013/12/24 saitou �W���� DEL -------------------------------------------------->>>>
    '�N���̓Z���^�[���ڎ����ɕύX
    '' �@�\�@ �F �N���@�����擾
    ''
    '' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ''
    '' ���l�@ �F 
    ''
    'Private Function GetNenkinTORIMAST() As String
    '    Dim SQL As New StringBuilder(128)
    '    Dim NenkinFmt As New CAstFormat.CFormatNenkin
    '    Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
    '    Dim Kamoku As String = ""
    '    Dim ToriCode As String

    '    Call NenkinFmt.FirstRead(oRenkei.InFileName)
    '    NenkinFmt.GetFileData(NenkinFmt.NENKIN_REC1.Data)

    '    ' �N����ʂ��画�f 61:�������N��,62:���D���N��,63:�������N��,64:�J�ДN��,65:�V�����N���E�����N��,66:�V�D���N��,67:�������N���Z��
    '    Kamoku = NenkinFmt.NENKIN_REC1.NK2
    '    NenkinFmt.Close()
    '    oRenkei = Nothing

    '    ' �ȖڃR�[�h�@
    '    If Kamoku.Trim <> "" Then
    '        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Kamoku)
    '        If ToriCode <> "err" Then
    '            Return ToriCode
    '        End If
    '    End If

    '    Return New String(" "c, 9)
    'End Function
    '2013/12/24 saitou �W���� DEL --------------------------------------------------<<<<

    '2013/12/24 saitou �W���� DEL -------------------------------------------------->>>>
    '���Ɏ����̗������ݏ����Ȃ̂ŁA�����ŃZ���^�[���ڎ������̎������Q�Ƃ��邱�Ƃ͂Ȃ����߃R�����g�A�E�g
    '' �@�\�@ �F �Z���^�[���ڎ������݁@�����擾
    ''
    '' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ''
    '' ���l�@ �F 
    ''
    'Private Sub GetCenterTORIMAST(ByVal furiCode As String, ByVal kigyoCode As String)
    '    Dim SQL As New StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader = Nothing

    '    SQL.Append("SELECT ")
    '    SQL.Append(" TORIS_CODE_T")
    '    SQL.Append(",TORIF_CODE_T")
    '    SQL.Append(" FROM TORIMAST")
    '    SQL.Append(" WHERE FURI_CODE_T = " & SQ(furiCode))
    '    SQL.Append("   AND KIGYO_CODE_T = " & SQ(kigyoCode))
    '    SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
    '    SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(MainDB)
    '        If OraReader.DataReader(SQL) = True Then
    '            mKoParam.CP.TORIS_CODE = OraReader.GetString("TORIS_CODE_T")
    '            mKoParam.CP.TORIF_CODE = OraReader.GetString("TORIF_CODE_T")
    '            mKoParam.CP.TORI_CODE = mKoParam.CP.TORIS_CODE & mKoParam.CP.TORIF_CODE
    '        End If
    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then
    '            OraReader.Close()
    '        End If
    '        OraReader = Nothing
    '    End Try
    'End Sub
    '2013/12/24 saitou �W���� DEL --------------------------------------------------<<<<

    '2013/12/24 saitou �W���� DEL -------------------------------------------------->>>>
    '�����Ƒ�K�͔ł̋@�\�Ȃ̂ŃR�����g�A�E�g
    '' �@�\�@ �F �ʑO����
    ''
    '' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    ''
    '' ���l�@ �F 
    ''
    'Private Sub KobetuExecute(ByVal fileName As String, ByVal key As CommData)
    '    Dim StrReader As StreamReader = Nothing
    '    Try
    '        Dim MaeSyoriKey As String
    '        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
    '        Dim SQL As New StringBuilder(128)
    '        Dim TxtName As String

    '        SQL.Append("SELECT MAE_SYORI_T FROM TORIMAST WHERE")
    '        SQL.Append("   �@TORIS_CODE_T = " & SQ(key.INFOToriMast.TORIS_CODE_T))
    '        SQL.Append(" AND TORIF_CODE_T = " & SQ(key.INFOToriMast.TORIF_CODE_T))

    '        '������Ȃ��ꍇ
    '        If OraReader.DataReader(SQL) = False Then
    '            BatchLog.Write("�ʑO���� �����}�X�^�o�^�Ȃ�", "����", "�����R�[�h�F" & _
    '                       key.INFOToriMast.TORIS_CODE_T & "-" & key.INFOToriMast.TORIF_CODE_T)
    '            Return
    '        End If

    '        '�o�^���Ă��Ȃ��ꍇ
    '        If OraReader.GetItem("MAE_SYORI_T").Trim = "" Then
    '            BatchLog.Write("�ʑO���� �o�^�Ȃ�", "����", "")
    '            Return
    '        End If

    '        '�ʑO�����ԍ��擾
    '        MaeSyoriKey = OraReader.GetString("MAE_SYORI_T")
    '        OraReader.Close()

    '        TxtName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "134_MAE_SYORI_T.TXT")
    '        StrReader = New StreamReader(TxtName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

    '        Dim Line As String = StrReader.ReadLine()

    '        Do Until Line = Nothing
    '            Dim pos As Integer = Line.IndexOf(","c)

    '            If pos >= 0 Then
    '                If CAInt32(Line.Substring(0, pos).Trim).ToString("00") = MaeSyoriKey Then
    '                    ' ��v����ԍ���������

    '                    If pos >= 0 AndAlso Line.Length >= pos Then
    '                        pos = Line.IndexOf(","c, pos + 1)

    '                        Dim ParaPos As Integer = Line.ToUpper.IndexOf(".EXE", pos + 1)
    '                        If ParaPos < 0 Then
    '                            BatchLog.Write("�ʑO���� �d�w�d�o�^�Ȃ�", "����", "")
    '                            Exit Sub
    '                        End If

    '                        Dim Exe As String = Line.Substring(pos + 1, ParaPos - pos + 3).Trim
    '                        Exe = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), Exe)

    '                        Dim Para As String = ""
    '                        If ParaPos >= 0 AndAlso Line.Length >= ParaPos Then
    '                            Para = Line.Substring(ParaPos + 4).Trim
    '                        End If

    '                        ' �ʑO���� �N��
    '                        Dim ProcInfo As New ProcessStartInfo
    '                        ProcInfo.FileName = Exe

    '                        If File.Exists(ProcInfo.FileName) = True Then

    '                            Dim OutPath As String = CASTCommon.GetFSKJIni("OTHERSYS", "MAESYORI")
    '                            If OutPath <> "err" Then
    '                                If Directory.Exists(OutPath) = False Then
    '                                    Directory.CreateDirectory(OutPath)
    '                                End If
    '                            End If

    '                            If Directory.Exists(OutPath) = False Then
    '                                BatchLog.Write("�ʑO���� �A�g�t�H���_�Ȃ�", "���s", OutPath)
    '                                Return
    '                            End If

    '                            Dim OutName As String = Path.GetFileName(Exe) & "_" & Para & "."
    '                            OutName = Path.Combine(OutPath, OutName)

    '                            For i As Integer = 1 To 999
    '                                Try
    '                                    ' �t�@�C�����쐬�ł���܂ŌJ��Ԃ�
    '                                    If File.Exists(OutName & i.ToString("000")) = False Then
    '                                        File.Copy(fileName, OutName & i.ToString("000"), False)
    '                                        Exit For
    '                                    Else
    '                                        Threading.Thread.Sleep(100)
    '                                    End If
    '                                Catch ex As Exception

    '                                End Try
    '                            Next i

    '                            ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
    '                            ProcInfo.Arguments = Para
    '                            Process.Start(ProcInfo)
    '                            BatchLog.Write("�ʑO���� ���s", "����")
    '                        Else
    '                            BatchLog.Write("�ʑO���� ���s���W���[���Ȃ�", "���s", Exe)
    '                        End If

    '                        ' �ʏ��������s�����ł̔�����
    '                        Return
    '                    End If

    '                    BatchLog.Write("�ʑO���� ���s���W���[���̎w��Ȃ�", "���s")
    '                    Exit Do
    '                End If

    '                Line = StrReader.ReadLine()
    '            End If
    '        Loop

    '        BatchLog.Write("�ʑO���� TXT��v�f�[�^�Ȃ�", "����", MaeSyoriKey)

    '        StrReader.Close()
    '        StrReader = Nothing
    '    Catch ex As Exception
    '        BatchLog.Write("�ʑO����", "���s", ex.Message)
    '    Finally
    '        If Not StrReader Is Nothing Then
    '            StrReader.Close()
    '        End If
    '    End Try
    'End Sub
    '2013/12/24 saitou �W���� DEL --------------------------------------------------<<<<

    '2012/06/30 �W���Ł@WEB�`���A�g
    ' �@�\�@ �F WEB�����}�X�^ UPDATE
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F WEB�����}�X�^�X�V�p
    '
    Private Function UpdateWEB_RIREKIMAST() As Boolean

        Try
            BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")

            Dim SQL As New StringBuilder(128)
            SQL.Append("UPDATE WEB_RIREKIMAST SET")
            SQL.Append(" STATUS_KBN_W = '1' ")
            SQL.Append(" WHERE FSYORI_KBN_W = '1'")
            SQL.Append(" AND TORIS_CODE_W = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_W = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_W = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND SEQ_NO_W = (SELECT MAX(SEQ_NO_W) FROM WEB_RIREKIMAST ")
            SQL.Append(" WHERE TORIS_CODE_W = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_W = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND   FURI_DATE_W = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND   FSYORI_KBN_W = '1'")
            SQL.Append(" )")


            '2017/04/19 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            'WEB�����}�X�^�Ƀ��R�[�h���Ȃ��ꍇ�ɃG���[�ƂȂ�悤�ɏC��
            If MainDB.ExecuteNonQuery(SQL) < 1 Then
                'If MainDB.ExecuteNonQuery(SQL) < 0 Then
                '2017/04/19 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
                BatchLog.Write("WEB�����}�X�^�X�V", "���s", MainDB.Message)
                Return False
            End If

        Catch ex As Exception
            BatchLog.Write("WEB�����}�X�^�X�V", "���s", ex.Message)
            Return False
        Finally
        End Try

        Return True
    End Function

    ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
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
            If mArgumentData.INFOToriMast.FSYORI_KBN_T = "1" Then
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

            For i As Integer = 0 To FileList.Length - 1 Step 1
                If Path.GetFileName(FileList(i)).IndexOf(CheckFileName) = 0 Then
                    FileName = Path.GetFileName(FileList(i))
                    Exit For
                End If
            Next

            If FileName = "" Then
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
            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "", ex)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            BatchLog.Write("", "0000000000-00", "00000000", "�e�L�X�g�t�@�C���Ǎ�", "�I��", "")
        End Try

    End Function
    ' 2016/01/11 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

End Class
