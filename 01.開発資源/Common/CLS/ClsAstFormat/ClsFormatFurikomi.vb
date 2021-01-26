Option Strict On
Option Explicit On

Imports CASTCommon.ModPublic

' �S�� �f�[�^�i�U���j�t�H�[�}�b�g�N���X
Public Class CFormatFurikomi
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormatZengin

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 120

    '2018/03/05 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal len As Integer)
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = len

    End Sub
    '2018/03/05 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END

    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Public Overrides Function CheckRecord1() As String
        Dim sRet As String = MyBase.CheckRecord1

        If sRet <> "H" Then
            Return sRet
        End If

        If sRet <> "ERR" Then
            If MyBase.CheckHeaderRecord(1) = False Then
                Return "ERR"
            End If
        End If

        ' �S��U���t�H�[�}�b�g�Ǝ��`�F�b�N
        If Not mInfoComm Is Nothing Then
            ' ���o���敪 ��ʃR�[�h�`�F�b�N
            Select Case mInfoComm.INFOToriMast.NS_KBN_T
                Case "1"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "11", "12", "21", "41", "43", "44", "45", "71", "72"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_���o���敪�A���", "�s��v", "��ʁF" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ޓ��o���敪�A��ʕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "9"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "91"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_���o���敪�A���", "�s��v", "��ʁF" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ޓ��o���敪�A��ʕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
            End Select

            '  ��ʃR�[�h�`�F�b�N
            Select Case mInfoComm.INFOToriMast.SYUBETU_T
                Case "91"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "91"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "21"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "21", "41", "43", "44", "45"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "12"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "12"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
                Case "11"
                    Select Case InfoMeisaiMast.SYUBETU_CODE
                        Case "11"
                            'OK
                        Case Else
                            'NG
                            WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                            DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                            Return "ERR"
                    End Select
            End Select

            '2018/10/07 saitou �L���M��(RSV2�W��) DEL �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
            '�����ňُ��Ԃ��Ă��A���̃w�b�_���R�[�h�̃`�F�b�N�Ō��ʂ�h��ւ����Ă��܂����߁A�������ړ��B
            ''2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�������`�F�b�N�j------------------ START
            'If INI_S_KDBMAST_CHK = "1" AndAlso mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" Then
            '    Dim bRet As Boolean = ChkKDBMAST(InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
            '    If bRet = False Then
            '        Dim MSG As String = String.Format("�x�X�R�[�h�F{0} �ȖځF{1} �����F{2}", InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
            '        WriteBLog("�w�b�_�������`�F�b�N", "�����Ȃ�", MSG)
            '        DataInfo.Message = "�w�b�_�������`�F�b�N�s��v " & MSG

            '        Dim InError As INPUTERROR = Nothing
            '        InError.ERRINFO = "�������Ȃ�(�w�b�_�[)"
            '        InErrorArray.Add(InError)

            '        Return "IJO"
            '    End If
            'End If
            ''2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�������`�F�b�N�j------------------ END
            '2018/10/07 saitou �L���M��(RSV2�W��) DEL ------------------------------------------------- END
        End If

        Return "H"
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Protected Overrides Function CheckRecord2() As String
        Dim sRet As String = MyBase.CheckRecord2

        If sRet <> "D" Then
            Return sRet
        End If

        Return "D"
    End Function

    Protected Overrides Function CheckDataRecord() As Boolean
        Dim InError As INPUTERROR = Nothing
        '2018/10/04 saitou �L���M��(RSV2�W��) ADD �i���z0�~�`�F�b�N�j ------------------------------ START
        Dim INI_RSV2_DATA_KINGAKUZERO As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KINGAKUZERO")
        '2018/10/04 saitou �L���M��(RSV2�W��) ADD -------------------------------------------------- END

        InError.DATA = InfoMeisaiMast

        InErrorArray = New ArrayList

        If mInfoComm Is Nothing Then
            Return True
        End If

        '�x�X�E�����Ǒ֑Ή�(�ύX�O���̕ێ�)
        InfoMeisaiMast.OLD_KIN_NO = InfoMeisaiMast.KEIYAKU_KIN
        InfoMeisaiMast.OLD_SIT_NO = InfoMeisaiMast.KEIYAKU_SIT
        InfoMeisaiMast.OLD_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA

        '�K��O�����`�F�b�N���C���v�b�g�G���[�Ƃ��ďo��
        Dim kiteiRem As Long = CheckRegularString()

        If kiteiRem <> -1 Then
            ' �K��O�����ُ�
            InfoMeisaiMast.FURIKETU_CODE = 9
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kiteigaimoji)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " �K��O�����F" & kiteiRem & "�o�C�g��"
        End If

        '���Z�@�փ}�X�^���݃`�F�b�N���s�����ǂ����𔻒肷��t���O
        Dim TenMastExistCheck_Flg As Boolean = True

        '���Z�@�փR�[�h���l�`�F�b�N
        If IsDecimal(InfoMeisaiMast.KEIYAKU_KIN) = False OrElse _
            InfoMeisaiMast.KEIYAKU_KIN.Equals("0000") = True OrElse _
            InfoMeisaiMast.KEIYAKU_KIN.Equals("9999") = True Then
            ' ��s�R�[�h���l�ُ�
            InfoMeisaiMast.FURIKETU_CODE = 9

            '��s�R�[�h�ُ�
            InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN

            '���l�ُ�̏ꍇ�A�t���O��FALSE�ɂ���
            TenMastExistCheck_Flg = False

        ElseIf IsDecimal(InfoMeisaiMast.KEIYAKU_SIT) = False OrElse _
            InfoMeisaiMast.KEIYAKU_SIT.Equals("999") = True Then

            '�䂤�����s�̏ꍇ��"000"��"999"���ُ�Ƃ��Ȃ�
            If InfoMeisaiMast.KEIYAKU_KIN.Equals("9900") = False Then
                ' �X�Ԑ��l�ُ�
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Tenban)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " �x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT

                '���l�ُ�̏ꍇ�A�t���O��FALSE�ɂ���
                TenMastExistCheck_Flg = False
            End If
        End If

        '�ȖڃR�[�h�`�F�b�N
        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "11", "12"
                '�Ȗڃ`�F�b�N��9������
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 9
                    Case Else
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- START
                        '' �Ȗڈُ�
                        'InfoMeisaiMast.FURIKETU_CODE = 9

                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        Select Case mInfoComm.INFOToriMast.SYUBETU_T
                            Case "11"
                                If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_11").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' �Ȗڈُ�
                                    InfoMeisaiMast.FURIKETU_CODE = 9

                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                            Case "12"
                                If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_12").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' �Ȗڈُ�
                                    InfoMeisaiMast.FURIKETU_CODE = 9

                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                        End Select
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- END
                End Select

            Case "21"
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 4, 9
                    Case Else
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- START
                        '' �Ȗڈُ�
                        'InfoMeisaiMast.FURIKETU_CODE = 9

                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "S_KEIYAKUKAMOKU_21").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' �Ȗڈُ�
                            InfoMeisaiMast.FURIKETU_CODE = 9

                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- END
                End Select
        End Select

        ' �V�K�R�[�h�`�F�b�N
        '�V�K�R�[�h�ɋ󔒂�����
        Select Case InfoMeisaiMast.SINKI_CODE
            Case " ", "0", "1", "2"
            Case Else
                ' �V�K�R�[�h�ُ�
                InfoMeisaiMast.FURIKETU_CODE = 9
                InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " �V�K�R�[�h�F" & InfoMeisaiMast.SINKI_CODE
        End Select

        Dim KouzaCheck As Boolean = True

        '�����ԍ����̃n�C�t���͏Ȃ��A��0���߂���
        InfoMeisaiMast.KEIYAKU_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA.Replace("-"c, "").PadLeft(7, "0"c)
 
        '�Ȗڂ�9�������ԍ���ALL9�̎��͌����`�F�b�N�Ȃ�
        Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
            Case 9
                Select Case InfoMeisaiMast.KEIYAKU_KOUZA.Trim
                    Case "9999999"
                        KouzaCheck = False
                End Select
        End Select

        '�����ԍ��`�F�b�N�f�B�W�b�g�`�F�b�N���������AKOUZACHECK�t���O���Q�Ƃ���悤�C��
        If KouzaCheck = True Then
            If mCheckDigitFlag = "1" Then
                '�����ԍ��`�F�b�N�f�W�b�g�`�F�b�N
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    If CheckDigitCheck() = False Then
                        ' �����ԍ��ُ�
                        InfoMeisaiMast.FURIKETU_CODE = 9
                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
                End If
            End If
        End If
        
        If KouzaCheck = True Then
            If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA) = False Then
                ' �����ԍ��ُ�
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                KouzaCheck = False
            End If
        End If

        '�����ԍ�ALL0�͕s�\���R�R�[�h2���Z�b�g����
        If KouzaCheck = True AndAlso InfoMeisaiMast.KEIYAKU_KOUZA = "0000000" Then
            ' �����ԍ��ُ�
            Select Case InfoMeisaiMast.KEIYAKU_KOUZA
                Case "0000000"
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
            End Select
        End If

        '�x�X�ǂݑւ�
        With InfoMeisaiMast
            If CASTCommon.GetFSKJIni("YOMIKAE", "TENPO") = "1" Then
                '�x�X�ǂݑւ��Ώ�
                Call fn_TENPO_YOMIKAE(.KEIYAKU_KIN, .KEIYAKU_SIT, .KEIYAKU_KIN, .KEIYAKU_SIT)
            End If
        End With

        '�����ǂݑւ�
        With InfoMeisaiMast
            If CASTCommon.GetFSKJIni("YOMIKAE", "KOUZA") = "1" Then
                '�x�X�ǂݑւ��Ώ�
                Call fn_KOUZA_YOMIKAE(.KEIYAKU_SIT, .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                                .KEIYAKU_SIT, .KEIYAKU_KOUZA, .IDOU_DATE)
            End If
        End With

        '���Z�@�փR�[�h���l�`�F�b�N�ŃG���[�ƂȂ����ꍇ�A���Z�@�֑��݃`�F�b�N�͍s��Ȃ�

        '���Z�@�փR�[�h���݃`�F�b�N
        Dim nRet As Integer
        ' 2016/10/18 �^�X�N�j���� CHG �yPG�zUI_11-1-15(�ѓc�M��<�U���˗��f�[�^�̋��Z�@�ցE�x�X���ҏW�Ή�>) -------------------- START
        'If TenMastExistCheck_Flg = True Then
        '    nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
        'Else '���Z�@�փR�[�h���l�`�F�b�N���s
        '    nRet = 9
        'End If
        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SFURI_TENNAME") = "1" Then
            Select Case mInfoComm.INFOToriMast.FSYORI_KBN_T
                Case "3"
                    nRet = CAstExternal.GetTENMASTExistsCustom(OraDB, _
                                                               InfoMeisaiMast.KEIYAKU_KIN, _
                                                               InfoMeisaiMast.KEIYAKU_SIT, _
                                                               InfoMeisaiMast2.KEIYAKU_KIN_KNAME, _
                                                               InfoMeisaiMast2.KEIYAKU_SIT_KNAME, _
                                                               InfoMeisaiMast.FURIKAE_DATE, _
                                                               InfoMeisaiMast.YOBI1, _
                                                               InfoMeisaiMast.YOBI2)
                    If TenMastExistCheck_Flg = False Then
                        '���Z�@�փR�[�h���l�`�F�b�N���s
                        nRet = 9
                    End If
                Case Else
                    If TenMastExistCheck_Flg = True Then
                        nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
                    Else '���Z�@�փR�[�h���l�`�F�b�N���s
                        nRet = 9
                    End If
            End Select
        Else
            If TenMastExistCheck_Flg = True Then
                nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
            Else '���Z�@�փR�[�h���l�`�F�b�N���s
                nRet = 9
            End If
        End If
        ' 2016/09/01 �^�X�N�j���� CHG �yPG�zUI_11-1-15(�ѓc�M��<�U���˗��f�[�^�̋��Z�@�ցE�x�X���ҏW�Ή�>) -------------------- END

        '���Z�@�֎擾�������̃G���[�����ύX(�ȉ��̒ʂ�)
        '===================================================================
        '0:���Z�@�֎擾���s(GetTENMASTExist�ŗ�O����)
        '1:���Z�@�ւ���x�X�Ȃ�
        '2:���Z�@�ւ���x�X����(����I��)
        '3:�U�������폜������(�X�ܓ��p��)
        '9:���Z�@�փR�[�h���l�`�F�b�N���s
        '===================================================================
        Select Case nRet
            Case 0 '���Z�@�ւȂ��̏ꍇ
                '���Z�@�ւȂ�
                InfoMeisaiMast.FURIKETU_CODE = 2
                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
            Case 1 '���Z�@�ւ���C�x�X�Ȃ�
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    '���s�Ŏx�X�Ȃ��̏ꍇ�͎��s�X�Ԉُ�
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    
                    InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT

                Else
                    '���s�Ŏx�X�Ȃ��̏ꍇ�͑��s�X�Ԉُ�
                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                End If
            Case 2 '���Z�@�ւ���C�x�X����
                '���s�œX��000�̏ꍇ�͎��s�X�Ԉُ�
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO AndAlso InfoMeisaiMast.KEIYAKU_SIT = "000" Then

                    InfoMeisaiMast.FURIKETU_CODE = 2
                    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"

                    InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
               
                    '����̏ꍇ�A�R�[�h�Ɩ��̂̐����������Ă��邩�`�F�b�N����
                ElseIf Not CheckTenMast(InfoMeisaiMast, InfoMeisaiMast2, InError.ERRINFO) Then
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- START
                    InfoMeisaiMast.KinTenSoui = True
                    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- END
                End If

                '����I���̂��ߏ����Ȃ�

            Case 3 '�U�������폜������(�X�ܓ��p��)
                InfoMeisaiMast.FURIKETU_CODE = 2
                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
            Case 9 '���Z�@�փR�[�h���l�`�F�b�N�Ŏ��s�����ꍇ
            Case Else '��O
        End Select

        '���l�����󔒂̏ꍇ�̓G���[�Ƃ���
        If InfoMeisaiMast.KEIYAKU_KNAME.Trim = "" Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kana)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���l���Ȃ�"
        End If

        '���z�`�F�b�N
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
        Else
            If InfoMeisaiMast.FURIKIN < 0 Then
                ' �}�C�i�X���z
                InfoMeisaiMast.FURIKETU_CODE = 9

                InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
            End If

            '���z�O�~
            '2018/10/04 saitou �L���M��(RSV2�W��) UPD �i���z0�~�`�F�b�N�j ------------------------------ START
            If INI_RSV2_DATA_KINGAKUZERO = "1" Then
                If InfoMeisaiMast.FURIKIN = 0 Then
                    InfoMeisaiMast.FURIKETU_CODE = 9

                    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
                End If
            End If
            'If InfoMeisaiMast.FURIKIN = 0 Then
            '    InfoMeisaiMast.FURIKETU_CODE = 9

            '    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
            '    InErrorArray.Add(InError)
            '    DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
            'End If
            '2018/10/04 saitou �L���M��(RSV2�W��) UPD -------------------------------------------------- END
        End If

        '2018/03/15 saitou �L���M��(RSV2�W��) DEL �U���萔���v�Z�����폜 ------------------------------ START
        '����őΉ�������O�̋����W�b�N�Ȃ̂ŁA�W���ł��폜�B�i���̏����ɂ��Ӑ}���Ȃ����ʂ��������Ȃ��悤�Ɂj
        '�������ݎ��Ɏ萔���̌v�Z���s������������A�������݂̖��׃}�X�^�쐬���Ɍv�Z���W�b�N�𖄂ߍ��ނ悤�ɁA
        '�J�X�^�}�C�Y�Ή����s���B
        'If InfoMeisaiMast.FURIKETU_CODE = 0 Then
        '    '�G���[��������ΐU���萔���v�Z
        '    If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
        '        '�U�����Z�@�ւ��C�����ɂ̏ꍇ
        '        If InfoMeisaiMast.KEIYAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T Then
        '            ' �U���x�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
        '            If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '                '�O�~���傫�� ���� �P���~����
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A1_T
        '            ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '                '�P���~�ȏ� ���� �R���~����
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A2_T
        '            ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '                '�R���~�ȏ�
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_A3_T
        '            End If
        '        Else
        '            '�U���x�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
        '            If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '                '�O�~���傫�� ���� �P���~����
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B1_T
        '            ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '                '�P���~�ȏ� ���� �R���~����
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B2_T

        '            ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '                '�R���~�ȏ�
        '                InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_B3_T
        '            End If
        '        End If
        '    Else
        '        '���s
        '        If 0 < InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 10000 Then
        '            '�O�~���傫�� ���� �P���~����
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C1_T
        '        ElseIf 10000 <= InfoMeisaiMast.FURIKIN And InfoMeisaiMast.FURIKIN < 30000 Then
        '            '�O�~���傫�� ���� �R���~����
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C2_T
        '        ElseIf 30000 <= InfoMeisaiMast.FURIKIN Then
        '            '�R���~�ȏ�
        '            InfoMeisaiMast.TESUU_KIN = mInfoComm.INFOToriMast.TESUU_C3_T
        '        End If
        '    End If
        'End If
        '2018/03/15 saitou �L���M��(RSV2�W��) DEL ----------------------------------------------------- END

        InErrorArray.TrimToSize()

        If InErrorArray.Count > 0 Then
            Return False
        End If
        Return True
    End Function

    Private Function CheckTenMast(ByVal InfMei As CAstFormat.CFormat.MEISAI, ByRef InfMei2 As CAstFormat.CFormat.MEISAI2, ByRef ERRINFO As String) As Boolean

        Dim ret As Boolean = False
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Dim KinName As String = ""
        Dim RyakuKinName As String = ""
        Dim SitName As String = ""

        Try
            ' 2016/01/28 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            ' �c�a�ڑ������݂��Ȃ��ꍇ�C�������s��Ȃ�(�W���o�O�C��)
            If OraDB Is Nothing Then
                Return False
            End If
            ' 2016/01/28 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

            '2016/12/16 saitou RSV2 ADD ���Z�@�֖��`�F�b�N�L���Ή� ---------------------------------------- START
            '���Z�@�֖��`�F�b�N�L����0(���Ȃ�)�̏ꍇ�̂ݏ����𐳏�Ŕ�����
            Dim KINNAMECHK As String = CASTCommon.GetFSKJIni("KAWASE", "KINNAMECHK")
            If KINNAMECHK.Equals("0") = True Then
                Return True
            End If
            '2016/12/16 saitou RSV2 ADD ------------------------------------------------------------------- END

            '���Z�@�֖��̃`�F�b�N
            '���Z�@�փR�[�h�Ō������ĶŖ�����v���邩�ǂ���
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.AppendLine(" SELECT KIN_KNAME_N, RYAKU_KIN_KNAME_N FROM KIN_INFOMAST")
            SQL.AppendLine(" WHERE KIN_NO_N = " & SQ(InfMei.KEIYAKU_KIN))
            '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- START
            SQL.AppendLine(" ORDER BY KIN_FUKA_N DESC ")
            '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

            '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
            Dim ReplaceKinName As String = InfMei2.KEIYAKU_KIN_KNAME
            For Each de As DictionaryEntry In ReplaceKinNamePattern
                If ReplaceKinName.IndexOf(de.Key.ToString) <> -1 Then
                    ReplaceKinName = ReplaceKinName.Replace(de.Key.ToString, de.Value.ToString)
                    'Exit For
                End If
            Next
            '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

            If OraReader.DataReader(SQL) Then
                '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- START
                InfMei2.TENMAST_SIT_KNAME = OraReader.GetString("KIN_KNAME_N")
                '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

                While OraReader.EOF = False
                    KinName = OraReader.GetString("KIN_KNAME_N")
                    RyakuKinName = OraReader.GetString("RYAKU_KIN_KNAME_N")

                    '���Z�@�֖�����v�����琳��
                    If KinName = InfMei2.KEIYAKU_KIN_KNAME.Trim OrElse
                        (RyakuKinName <> "" AndAlso (RyakuKinName = InfMei2.KEIYAKU_KIN_KNAME.Trim)) Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
                    ElseIf KinName = ReplaceKinName.Trim OrElse
                        (RyakuKinName <> "" AndAlso (RyakuKinName = ReplaceKinName.Trim)) Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END
                    End If

                    OraReader.NextRead()
                End While
            End If

            If ret = False Then
                '���Z�@�֖��ň�v���Ȃ��ꍇ
                ERRINFO = "��s������(���툵)"
                Return False
            End If

            ret = False

            '�x�X���̃`�F�b�N
            '���Z�@�փR�[�h�A�x�X�R�[�h�Ō���
            OraReader.Close()

            SQL.Length = 0
            SQL.AppendLine(" SELECT SIT_KNAME_N FROM SITEN_INFOMAST")
            SQL.AppendLine(" WHERE KIN_NO_N = " & SQ(InfMei.KEIYAKU_KIN))
            SQL.AppendLine(" AND SIT_NO_N = " & SQ(InfMei.KEIYAKU_SIT))
            '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- START
            SQL.AppendLine(" ORDER BY KIN_FUKA_N DESC ,SIT_FUKA_N ASC ")
            '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END


            '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
            Dim ReplaceSitName As String = InfMei2.KEIYAKU_SIT_KNAME
            For Each de As DictionaryEntry In ReplaceSitNamePattern
                If ReplaceSitName.IndexOf(de.Key.ToString) <> -1 Then
                    ReplaceSitName = ReplaceSitName.Replace(de.Key.ToString, de.Value.ToString)
                    'Exit For
                End If
            Next
            '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

            If OraReader.DataReader(SQL) Then
                '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- START
                InfMei2.TENMAST_SIT_KNAME = OraReader.GetString("SIT_KNAME_N")
                '2018/10/02 maeda �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

                While OraReader.EOF = False
                    SitName = OraReader.GetString("SIT_KNAME_N")

                    '�x�X������v�����琳��
                    If SitName = InfMei2.KEIYAKU_SIT_KNAME.Trim Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
                    ElseIf SitName = ReplaceSitName.Trim Then
                        ret = True
                        Exit While
                        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END
                    End If

                    OraReader.NextRead()
                End While
            End If

            If ret = False Then
                '�x�X���ň�v���Ȃ��ꍇ
                ERRINFO = "�x�X������(���툵)"
                Return False
            End If

        Catch
            Throw
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return ret
    End Function

    '2018/10/07 saitou �L���M��(RSV2�W��) DEL �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
    '�����ړ��̂��ߊ֐��s�v�B
    '''' <summary>
    '''' �������}�X�^�Ɏw����������݂��邩�`�F�b�N����
    '''' </summary>
    '''' <param name="astrSIT_NO">�x�X�R�[�h</param>
    '''' <param name="astrKAMOKU">�ȖڃR�[�h</param>
    '''' <param name="astrKOUZA">�����ԍ�</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function ChkKDBMAST(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String) As Boolean
    '    Dim SQL As New System.Text.StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader = Nothing
    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(OraDB)

    '        astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

    '        SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
    '        SQL.Append(" WHERE OLD_TSIT_NO_D = '" & astrSIT_NO & "'")
    '        SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
    '        SQL.Append(" AND OLD_KOUZA_D = '" & astrKOUZA & "'")

    '        return OraReader.DataReader(SQL)

    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then OraReader.Close()
    '    End Try

    'End Function
    '2018/10/07 saitou �L���M��(RSV2�W��) DEL ------------------------------------------------- END

End Class
