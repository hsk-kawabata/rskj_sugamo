
'���� ���e������`���W���[��
'�I���N����`
Public Class DB
    Public Shared ReadOnly USER As String = "KZFMAST"               '���[�U
    Public Shared ReadOnly PASSWORD As String = "KZFMAST"           '�p�X���[�h
    Public Shared ReadOnly SOURCE As String = "RSKJ_LSNR"           '�f�[�^�\�[�X��
    '�ڑ�������
    Public Shared ReadOnly CONNECT As String = String.Format("User={0};Password={1};Data Source={2};Pooling=false;", _
                                                USER, _
                                                PASSWORD, _
                                                SOURCE)

End Class
