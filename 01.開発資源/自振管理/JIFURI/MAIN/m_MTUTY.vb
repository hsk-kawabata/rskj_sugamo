Imports System
Imports System.Runtime.InteropServices
Module m_MTUTY
    '-------------------------------------------------------------------------------------------
    '----- �l�s�A�N�Z�X�p�֐���`�i�i�o�b�Ђc�k�k�p�j----------------------------------------------
    '-------------------------------------------------------------------------------------------

    ' �V���W���[���p��`
    Public Declare Function mtinit Lib "MTDLL53.DLL" _
           (ByRef Mt_info As Integer) As Long
    '(ByRef Mt_info As Any) As Long
    '���u�̃X�e�[�^�X�ǂݎ��
    Public Declare Function mtstat Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�a�n�s�܂Ŋ����߂�
    Public Declare Function mtrewind Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�P�u���b�N�ǂݍ���
    'Public Declare Function mtrblock Lib "MTDLL53.DLL" _
    '(ByVal MtId As Integer, ByVal buff As String, ByVal count As Integer, ByVal bufflen As Long) As Long
    Public Declare Function mtrblock Lib "MTDLL53.DLL" _
       (ByVal MtId As Integer, ByRef buff As String, ByRef count As Integer, ByRef bufflen As Long) As Long
    '�P�u���b�N��������
    Public Declare Function mtwblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer, ByVal buff As String, ByVal blklen As Integer) As Long
    '�P�u���b�N�O�i
    Public Declare Function mtfblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�P�u���b�N���
    Public Declare Function mtbblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�e�[�v�}�[�N�����o����܂őO�i
    Public Declare Function mtffile Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�e�[�v�}�[�N�����o����܂Ō��
    Public Declare Function mtbfile Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�e�[�v�}�[�N�P���C�g
    Public Declare Function mtwtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�e�[�v�}�[�N�Q���C�g
    Public Declare Function mtwmtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�}���`�e�[�v�}�[�N�̌��o
    Public Declare Function mtsmtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�e�[�v�̃A�����[�h
    Public Declare Function mtunload Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '���u�̃I�����C��
    Public Declare Function mtonline Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '�d�a�b�c�h�b�R�[�h�ϊ�
    Public Declare Function mtebc Lib "MTDLL53.DLL" _
        (ByVal codeno As Integer) As Long
    '�l�s�̃C���[�Y
    Public Declare Function mters Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '----------------------------------------------------------------------------------------
    '----------------------------------------------------------------------------------------
    '�c�k�k���łi�o�b�֐����g�p����Ƃ��̈���
    Public mt_mtid As Integer
    <VBFixedString(32000)> Public mt_buffer As String
    Public mt_bufflen As Long
    Public mt_count As Integer
    Public mt_blklen As Integer
    Public mt_codeno As Integer
    Public mt_rtn As Long

    Structure MtINFO
        Public UnitNo As Byte
        Public HostNo As Byte
        Public TargetNo As Byte
        'Public Vender(0 To 7)   As Byte
        'Public Product(0 To 15) As Byte
        'Public Version(0 To 3)  As Byte
        Public Vender1 As Byte
        Public Vender2 As Byte
        Public Vender3 As Byte
        Public Vender4 As Byte
        Public Vender5 As Byte
        Public Vender6 As Byte
        Public Vender7 As Byte
        Public Vender8 As Byte
        Public Product1 As Byte
        Public Product2 As Byte
        Public Product3 As Byte
        Public Product4 As Byte
        Public Product5 As Byte
        Public Product6 As Byte
        Public Product7 As Byte
        Public Product8 As Byte
        Public Product9 As Byte
        Public Product10 As Byte
        Public Product11 As Byte
        Public Product12 As Byte
        Public Product13 As Byte
        Public Product14 As Byte
        Public Product15 As Byte
        Public Product16 As Byte
        Public Version1 As Byte
        Public Version2 As Byte
        Public Version3 As Byte
        Public Version4 As Byte
        '<VBFixedArray(7), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Vender() As Byte
        '<VBFixedArray(15), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Product() As Byte
        '<VBFixedArray(3), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Version() As Byte
        Public Reserve As Byte
    End Structure
    'Public T_Mtinfo(1 To 8) As MtINFO
    Public T_Mtinfo() As MtINFO
    '<VBFixedArray(7), MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> Public T_Mtinfo As MtINFO
    '�X�^���_�[�h���x����`
    '--------------------------------------
    ' �������� BOT + VOL1 + HDR1 + TM + TM
    '--------------------------------------
    '�{�����[�����x���iVOL1)
    <VBFixedString(3)> Public Const VOL1_���x�����ʖ� As String = "VOL"
    <VBFixedString(1)> Public Const VOL1_���x���ԍ� As String = "1"
    <VBFixedString(6)> Public VOL1_�{�����[�����ʖ� As String
    'Public Const VOL1_�A�N�Z�X����      As String * 1 = " "
    <VBFixedString(31)> Public Const �\��1 As String = " "
    <VBFixedString(10)> Public VOL1_���L�Ҏ��� As String
    <VBFixedString(29)> Public VOL1_�\��2 As String
    '�t�@�C�����x���iHDR1)
    <VBFixedString(3)> Public Const HDR1_���x�����ʖ� As String = "HDR"
    <VBFixedString(1)> Public Const HDR1_���x���ԍ� As String = "1"
    <VBFixedString(17)> Public HDR1_�t�@�C�����ʖ� As String
    <VBFixedString(6)> Public HDR1_�t�@�C���Z�b�g���ʖ� As String
    <VBFixedString(4)> Public Const HDR1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const HDR1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const HDR1_����ԍ� As String = "0001"
    <VBFixedString(2)> Public Const HDR1_����X�V�ԍ� As String = "00"
    <VBFixedString(6)> Public Const HDR1_�쐬���t As String = " 00000"
    <VBFixedString(6)> Public Const HDR1_�������t As String = " 00000"
    <VBFixedString(1)> Public Const HDR1_�A�N�Z�X���� As String = "0"
    <VBFixedString(6)> Public Const HDR1_�u���b�N�� As String = "000000"
    <VBFixedString(13)> Public Const HDR1_�V�X�e�����ʖ� As String = " "
    <VBFixedString(7)> Public Const HDR1_�\��1 As String = " "
    '�t�@�C�����x���iHDR2)
    <VBFixedString(3)> Public Const HDR2_���x�����ʖ� As String = "HDR"
    <VBFixedString(1)> Public Const HDR2_���x���ԍ� As String = "2"
    <VBFixedString(1)> Public Const HDR2_���R�[�h�ԍ� As String = "F"        '�Œ蒷
    <VBFixedString(5)> Public HDR2_�u���b�N�� As String
    <VBFixedString(5)> Public HDR2_���R�[�h�� As String
    <VBFixedString(1)> Public Const HDR2_�L�^���x As String = " "
    <VBFixedString(1)> Public Const HDR2_�{�����[����� As String = "0"      '�P�{�����[��
    <VBFixedString(21)> Public Const HDR2_�\��1 As String = " "
    <VBFixedString(1)> Public HDR2_�u���b�N���� As String
    <VBFixedString(41)> Public Const HDR2_�\��2 As String = " "
    '�t�@�C�����x���iEOF1)
    <VBFixedString(3)> Public Const EOF1_���x�����ʖ� As String = "EOF"
    <VBFixedString(1)> Public Const EOF1_���x���ԍ� As String = "1"
    <VBFixedString(17)> Public EOF1_�t�@�C�����ʖ� As String
    <VBFixedString(6)> Public EOF1_�t�@�C���Z�b�g���ʖ� As String
    <VBFixedString(4)> Public Const EOF1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const EOF1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const EOF1_����ԍ� As String = "0001"
    <VBFixedString(2)> Public Const EOF1_����X�V�ԍ� As String = "00"
    <VBFixedString(6)> Public Const EOF1_�쐬���t As String = " 00000"
    <VBFixedString(6)> Public Const EOF1_�������t As String = " 00000"
    <VBFixedString(1)> Public Const EOF1_�A�N�Z�X���� As String = "0"
    <VBFixedString(6)> Public Const EOF1_�u���b�N�� As String = "000000"
    <VBFixedString(13)> Public Const EOF1_�V�X�e�����ʖ� As String = " "
    <VBFixedString(7)> Public Const EOF1_�\��1 As String = " "
    '�t�@�C�����x���iEOF2)
    <VBFixedString(3)> Public Const EOF2_���x�����ʖ� As String = "EOF"
    <VBFixedString(1)> Public Const EOF2_���x���ԍ� As String = "2"
    <VBFixedString(1)> Public Const EOF2_���R�[�h�ԍ� As String = "F"        '�Œ蒷
    <VBFixedString(5)> Public EOF2_�u���b�N�� As String
    <VBFixedString(5)> Public EOF2_���R�[�h�� As String
    <VBFixedString(1)> Public Const EOF2_�L�^���x As String = " "
    <VBFixedString(1)> Public Const EOF2_�{�����[����� As String = "0"     '�P�{�����[��
    <VBFixedString(21)> Public Const EOF2_�\��1 As String = " "
    <VBFixedString(1)> Public EOF2_�u���b�N���� As String
    <VBFixedString(41)> Public Const EOF2_�\��2 As String = " "
    '�t�@�C�����x���iEOV1)
    <VBFixedString(3)> Public Const EOV1_���x�����ʖ� As String = "EOV"
    <VBFixedString(1)> Public Const EOV1_���x���ԍ� As String = "1"
    <VBFixedString(17)> Public EOV1_�t�@�C�����ʖ� As String
    <VBFixedString(6)> Public EOV1_�t�@�C���Z�b�g���ʖ� As String
    <VBFixedString(4)> Public Const EOV1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const EOV1_�t�@�C�������ԍ� As String = "0001"
    <VBFixedString(4)> Public Const EOV1_����ԍ� As String = "0001"
    <VBFixedString(2)> Public Const EOV1_����X�V�ԍ� As String = "00"
    <VBFixedString(6)> Public Const EOV1_�쐬���t As String = " 00000"
    <VBFixedString(6)> Public Const EOV1_�������t As String = " 00000"
    <VBFixedString(1)> Public Const EOV1_�A�N�Z�X���� As String = "0"
    <VBFixedString(6)> Public Const EOV1_�u���b�N�� As String = "000000"
    <VBFixedString(13)> Public Const EOV1_�V�X�e�����ʖ� As String = " "
    <VBFixedString(7)> Public Const EOV1_�\��1 As String = " "
    '�t�@�C�����x���iEOV2)
    <VBFixedString(3)> Public Const EOV2_���x�����ʖ� As String = "EOV"
    <VBFixedString(1)> Public Const EOV2_���x���ԍ� As String = "2"
    <VBFixedString(1)> Public Const EOV2_���R�[�h�ԍ� As String = "F"       '�Œ蒷
    <VBFixedString(5)> Public EOV2_�u���b�N�� As String
    <VBFixedString(5)> Public EOV2_���R�[�h�� As String
    <VBFixedString(1)> Public Const EOV2_�L�^���x As String = " "
    <VBFixedString(1)> Public Const EOV2_�{�����[����� As String = "0"     '�P�{�����[��
    <VBFixedString(21)> Public Const EOV2_�\��1 As String = " "
    <VBFixedString(1)> Public EOV2_�u���b�N���� As String
    <VBFixedString(41)> Public Const EOV2_�\��2 As String = " "


    Structure DEVICE_status
        <VBFixedString(1)> Public BIT_TMK As String
        <VBFixedString(1)> Public BIT_EOT As String
        <VBFixedString(1)> Public BIT_BOT As String
        <VBFixedString(1)> Public BIT_DEN0 As String
        <VBFixedString(1)> Public BIT_DEN1 As String
        <VBFixedString(1)> Public BIT_FIL1 As String
        <VBFixedString(1)> Public BIT_PRO As String
        <VBFixedString(1)> Public BIT_FIL2 As String
        <VBFixedString(1)> Public BIT_DTE As String
        <VBFixedString(1)> Public BIT_HDE As String
        <VBFixedString(1)> Public BIT_NRDY As String
        <VBFixedString(1)> Public BIT_ILC As String
        <VBFixedString(1)> Public BIT_SCE As String
        <VBFixedString(1)> Public BIT_UDC As String
        <VBFixedString(1)> Public BIT_FIL3 As String
        <VBFixedString(1)> Public BIT_CHG As String
    End Structure

    '�戵���f�[�^���R�[�h��`
    <VBFixedString(120)> Public KZFDT120 As String   '�S�⃌�R�[�h
    <VBFixedString(180)> Public KZFDT180 As String   'NTT�d�b��/�ꊇ�������R�[�h
    <VBFixedString(220)> Public KZFDT220 As String   '�n���́i�x�R���j
    <VBFixedString(350)> Public KZFDT350 As String   '�n���́i�O�d���j
    <VBFixedString(390)> Public KZFDT390 As String   '����


    Public Function mtCHGStatus(ByVal Fskj_mt_rtn As Long, ByRef Fskj_mt_BITrtn As DEVICE_status) As Integer
        Dim STATUS_BIT(16) As Integer
        Dim count As Integer

        mtCHGStatus = -1

        If Fskj_mt_rtn = 0 Then
            Fskj_mt_BITrtn.BIT_TMK = 0
            Fskj_mt_BITrtn.BIT_EOT = 0
            Fskj_mt_BITrtn.BIT_BOT = 0
            Fskj_mt_BITrtn.BIT_DEN0 = 0
            Fskj_mt_BITrtn.BIT_DEN1 = 0
            Fskj_mt_BITrtn.BIT_FIL1 = 0
            Fskj_mt_BITrtn.BIT_PRO = 0
            Fskj_mt_BITrtn.BIT_FIL2 = 0
            Fskj_mt_BITrtn.BIT_DTE = 0
            Fskj_mt_BITrtn.BIT_HDE = 0
            Fskj_mt_BITrtn.BIT_NRDY = 0
            Fskj_mt_BITrtn.BIT_ILC = 0
            Fskj_mt_BITrtn.BIT_SCE = 0
            Fskj_mt_BITrtn.BIT_UDC = 0
            Fskj_mt_BITrtn.BIT_FIL3 = 0
            Fskj_mt_BITrtn.BIT_CHG = 0
            'Fskj_mt_BITrtn = "0000000000000000"
            GoTo �����I��
        End If
        If Fskj_mt_rtn = -1 Then
            Fskj_mt_BITrtn.BIT_TMK = 1
            Fskj_mt_BITrtn.BIT_EOT = 1
            Fskj_mt_BITrtn.BIT_BOT = 1
            Fskj_mt_BITrtn.BIT_DEN0 = 1
            Fskj_mt_BITrtn.BIT_DEN1 = 1
            Fskj_mt_BITrtn.BIT_FIL1 = 1
            Fskj_mt_BITrtn.BIT_PRO = 1
            Fskj_mt_BITrtn.BIT_FIL2 = 1
            Fskj_mt_BITrtn.BIT_DTE = 1
            Fskj_mt_BITrtn.BIT_HDE = 1
            Fskj_mt_BITrtn.BIT_NRDY = 1
            Fskj_mt_BITrtn.BIT_ILC = 1
            Fskj_mt_BITrtn.BIT_SCE = 1
            Fskj_mt_BITrtn.BIT_UDC = 1
            Fskj_mt_BITrtn.BIT_FIL3 = 1
            Fskj_mt_BITrtn.BIT_CHG = 1
            'Fskj_mt_BITrtn = "1111111111111111"
            GoTo �����I��
        End If
        '�r�b�g --> �o�C�g�ϊ�
        'Fskj_mt_BITrtn = Null
        For count = 0 To 15
            STATUS_BIT(count) = Fskj_mt_rtn Mod 2
            'Fskj_mt_BITrtn = Fskj_mt_BITrtn & CStr(Abs(STATUS_BIT(count)))
            Fskj_mt_rtn = Fskj_mt_rtn \ 2
        Next
        Fskj_mt_BITrtn.BIT_TMK = CStr(Math.Abs(STATUS_BIT(0)))
        Fskj_mt_BITrtn.BIT_EOT = CStr(Math.Abs(STATUS_BIT(1)))
        Fskj_mt_BITrtn.BIT_BOT = CStr(Math.Abs(STATUS_BIT(2)))
        Fskj_mt_BITrtn.BIT_DEN0 = CStr(Math.Abs(STATUS_BIT(3)))
        Fskj_mt_BITrtn.BIT_DEN1 = CStr(Math.Abs(STATUS_BIT(4)))
        Fskj_mt_BITrtn.BIT_FIL1 = CStr(Math.Abs(STATUS_BIT(5)))
        Fskj_mt_BITrtn.BIT_PRO = CStr(Math.Abs(STATUS_BIT(6)))
        Fskj_mt_BITrtn.BIT_FIL2 = CStr(Math.Abs(STATUS_BIT(7)))
        Fskj_mt_BITrtn.BIT_DTE = CStr(Math.Abs(STATUS_BIT(8)))
        Fskj_mt_BITrtn.BIT_HDE = CStr(Math.Abs(STATUS_BIT(9)))
        Fskj_mt_BITrtn.BIT_NRDY = CStr(Math.Abs(STATUS_BIT(10)))
        Fskj_mt_BITrtn.BIT_ILC = CStr(Math.Abs(STATUS_BIT(11)))
        Fskj_mt_BITrtn.BIT_SCE = CStr(Math.Abs(STATUS_BIT(12)))
        Fskj_mt_BITrtn.BIT_UDC = CStr(Math.Abs(STATUS_BIT(13)))
        Fskj_mt_BITrtn.BIT_FIL3 = CStr(Math.Abs(STATUS_BIT(14)))
        Fskj_mt_BITrtn.BIT_CHG = CStr(Math.Abs(STATUS_BIT(15)))

�����I��:
        If Err.Number <> 0 Then
            mtCHGStatus = Err.Number
        Else
            mtCHGStatus = 0
        End If
    End Function




End Module
