Imports MenteCommon

Module M_FUSION

    Public GCom As MenteCommon.clsCommon

    Public MainForm As KFUMENU020
    Public GOwnerForm As Form

    '���O�C�����[�U�p
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    'Biware���O�t�@�C���t�H�[�}�b�g
    Structure gstrBiware_LOG
        <VBFixedString(10)> Public LOG1 As String   '�`�����t
        <VBFixedString(1)> Public SP1 As String     '�Z�p���[�^
        <VBFixedString(8)> Public LOG2 As String    '�J�n����
        <VBFixedString(1)> Public SP2 As String     '�Z�p���[�^
        <VBFixedString(4)> Public LOG3 As String    '����M
        <VBFixedString(1)> Public SP3 As String     '�Z�p���[�^
        <VBFixedString(1)> Public LOG4 As String    '�T�C�N���ԍ�(�H)
        <VBFixedString(1)> Public SP4 As String     '�Z�p���[�^
        <VBFixedString(20)> Public LOG5 As String   '�����Z���^��
        <VBFixedString(1)> Public SP5 As String     '�Z�p���[�^
        <VBFixedString(4)> Public LOG6 As String    '����M
        <VBFixedString(1)> Public SP6 As String     '�Z�p���[�^
        <VBFixedString(12)> Public LOG7 As String   '�t�@�C���h�c
        <VBFixedString(1)> Public SP7 As String     '�Z�p���[�^
        <VBFixedString(256)> Public LOG8 As String  '�t�@�C���p�X��
        <VBFixedString(1)> Public SP8 As String     '�Z�p���[�^
        <VBFixedString(5)> Public LOG9 As String    '�e�L�X�g��
        <VBFixedString(1)> Public SP9 As String     '�Z�p���[�^
        <VBFixedString(8)> Public LOG10 As String   '���R�[�h��
        <VBFixedString(1)> Public SP10 As String     '�Z�p���[�^
        <VBFixedString(7)> Public LOG11 As String  '�G���[�R�[�h
        <VBFixedString(1)> Public SP11 As String     '�Z�p���[�^
        <VBFixedString(10)> Public LOG12 As String   '�I����
        <VBFixedString(1)> Public SP12 As String     '�Z�p���[�^
        <VBFixedString(8)> Public LOG13 As String  '�I������
        <VBFixedString(1)> Public SP13 As String     '�Z�p���[�^
    End Structure
    Public gstrBiware As gstrBiware_LOG
    Structure gstrBiware_LOG_366
        <VBFixedString(366)> Public LOG As String    '�f�[�^�敪(=2)
    End Structure
    Public gstrBiware366 As gstrBiware_LOG_366

End Module
