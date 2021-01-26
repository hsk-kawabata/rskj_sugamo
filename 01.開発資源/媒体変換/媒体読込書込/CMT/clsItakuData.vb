Option Explicit On 
Option Strict On

' �@�@�\ : �S��t�H�[�}�b�g�̈ϑ��Җ��̃f�[�^��ێ�����N���X
'
'
Public Class clsItakuData

    Private Const ThisModuleName As String = "clsItakuData.vb"

    ' �S��f�[�^�̃w�b�_�A�f�[�^�A�g���C���[����擾�ł�����
    Public nFileSeq As Integer = 0      ' FILE_SEQ
    Public strFuriDate As String = ""   ' �U�֓�
    Public strItakuCode As String = ""  ' �ϑ��҃R�[�h 
    Public strItakuKana As String = ""  ' �ϑ��Җ��J�i(DB�ɂ͖������\���p�ɕ֗��Ȃ��ߕێ�)
    Public strItakuKanji As String = "" ' �ϑ��Ҋ�����(DB�ɂ͖������\���p�ɕ֗��Ȃ��ߕێ�)
    Public strBankCode As String = ""   ' ���Z�@�փR�[�h(DB�ɂ͖������\���p�ɕ֗��Ȃ��ߕێ�)
    Public strBankName As String = ""   ' ���Z�@�֖�
    Public strBranchCode As String = "" ' �x�X�R�[�h
    Public strBranchName As String = "" ' �x�X��
    Public strAccountShubetu As String = "" ' �������
    Public strAccountNo As String = ""  ' �����ԍ�
    Public nSyoriKin As Decimal = 0D    ' �������z
    Public nSyoriKen As Decimal = 0D    ' ��������
    Public nFuriKen As Decimal = 0D     ' �U�֍ό���
    Public nFuriKin As Decimal = 0D     ' �U�֍ϋ��z
    Public nFunouKen As Decimal = 0D    ' �s�\����
    Public nFunouKin As Decimal = 0D    ' �s�\���z

    ' ��L��񂩂瑼�e�[�u�����Q�Ƃ��Đ����������
    Public strFSyoriKbn As String        ' F�����敪
    Public strToriSCode As String        ' ������R�[�h
    Public strToriFCode As String        ' ����敛�R�[�h

    '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
    Public strBaitaiCode As String      ' �}�̃R�[�h
    '**********************************************

    '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h�ǉ� ***
    Public strFuriCode As String
    Public strKigyoCode As String
    '********************************************************
    Public strSyubetu As String
    Public strItakuKanriCode As String
    Public strMultiKbn As String

    Public nCheckTotalKin As Decimal = 0D    ' �f�[�^���R�[�h�̈������Ƃ����z�̍��v�l 
    Public nCheckTotalKen As Decimal = 0D    ' �f�[�^���R�[�h�̌����̍��v�l 
    Public noBankFlg As Boolean = False  ' ���Z�@�֖��E�x�X�����擾���Ȃ��ꍇ��true
    ' 
    ' �@�\�@�@�@: �N���X�̃C���X�^���X�̏�����
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: �Ȃ�
    '
    ' ���l�@�@�@: init���ĂԂ����ŉ������Ȃ�
    Public Sub New()
        ' �S�Ă̕ϐ���������
        init()
    End Sub

    Public Sub New(ByVal nobank As Boolean)
        ' �S�Ă̕ϐ���������
        init()
        Me.noBankFlg = nobank
    End Sub

    '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����̏ꍇ�͐U�փR�[�h�E��ƃR�[�h���n�� ***
    'Public Sub New(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String _
    '    , ByVal itakukana As String, ByVal bankcode As String, ByVal branchcode As String)
    Public Sub New(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String _
        , ByVal itakukana As String, ByVal bankcode As String, ByVal branchcode As String _
        , Optional ByVal furicode As String = "", Optional ByVal kigyocode As String = "", Optional ByVal syubetu As String = "", Optional ByVal nFormatkbn As Integer = 0)
        '******************************************************************************

        ' �S�Ă̕ϐ���������
        init()

        '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h���n�� ***
        'SetHeader(fileseq, furiDate, itaku, itakukana, bankcode, branchcode)
        SetHeader(fileseq, furiDate, itaku, itakukana, bankcode, branchcode, furicode, kigyocode, syubetu, nFormatkbn)
        '**********************************************************
    End Sub

    ' �@�\�@�@�@: �����o�ϐ��̏�����
    '
    ' �߂�l�@�@: ���� True / ���s False
    '
    ' �������@�@: �Ȃ�
    '
    ' ���l�@�@�@: 
    Public Function init() As Boolean
        nFileSeq = 0                ' FILE_SEQ
        strFuriDate = ""            ' �U�֓�
        strItakuCode = ""           ' �ϑ��҃R�[�h ������
        strItakuKana = ""           ' �ϑ��Җ��J�i(DB�ɂ͖������\���p�ɕ֗��Ȃ��ߕێ�)������
        strItakuCode = ""           ' �ϑ��҃R�[�h������
        strToriSCode = ""           ' ������R�[�h������
        strToriFCode = ""           ' ����敛�R�[�h������
        nSyoriKin = 0D              ' �������z������
        nSyoriKen = 0D              ' ��������������
        nFuriKen = 0D               ' �U�֍ό���������
        nFuriKin = 0D               ' �U�֍ϋ��z������
        nFunouKen = 0D              ' �s�\����������
        nFunouKin = 0D              ' �s�\���z������

        strFSyoriKbn = ""            ' F�����敪
        strToriSCode = ""            ' ������R�[�h
        strToriFCode = ""            ' ����敛�R�[�h

        '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h�ǉ� ***
        strFuriCode = ""
        strKigyoCode = ""
        '********************************************************

        nCheckTotalKin = 0D         ' �f�[�^���R�[�h�̈������Ƃ����z���v�l
        nCheckTotalKen = 0D         ' �f�[�^���R�[�h�����̍��v�l
        Return False
    End Function

    ' �@�\�@�@�@: �w�b�_���̐ݒ�
    '
    ' �߂�l�@�@: ���� True / ���s False
    '
    ' �������@�@: ARG1 - FILE_SEQ
    '             ARG2 - �U�֓�
    '             ARG3 - �ϑ��҃R�[�h,
    '             ARG4 - �ϑ��҃J�i��
    '             ARG5 - ���Z�@�փR�[�h
    '             ARG6 - �x�X�R�[�h
    '
    ' ���l�@�@�@: 
    '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����̏ꍇ�͐U�փR�[�h�E��ƃR�[�h���n�� ***
    'Public Function SetHeader(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String, ByVal itakuKana As String, ByVal bankcode As String, ByVal branchcode As String) As Boolean
    Public Function SetHeader(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String, ByVal itakuKana As String, ByVal bankcode As String, ByVal branchcode As String _
        , Optional ByVal furicode As String = "", Optional ByVal kigyocode As String = "", Optional ByVal syubetu As String = "", Optional ByVal nFormatkbn As Integer = 0) As Boolean
        '******************************************************************************
        Me.strItakuKana = itakuKana
        Me.nFileSeq = fileseq
        Me.strFuriDate = furiDate
        Me.strItakuCode = itaku
        Me.strBankCode = bankcode
        Me.strBranchCode = branchcode

        '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h�ǉ� ***
        Me.strFuriCode = furicode
        Me.strKigyoCode = kigyocode
        '********************************************************

        nCheckTotalKin = 0D         ' �f�[�^���R�[�h�̈������Ƃ����z���v�l
        nCheckTotalKen = 0D         ' �f�[�^���R�[�h�����̍��v�l

        CmtCom.CheckDate(strFuriDate)

        ' F�����敪, ������R�[�h, ����敛�R�[�h�̎擾

        '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h��n���ꂽ�ꍇ ***
        'strItakuKanji = DB.GetItakuKanji(strItakuCode)
        '�Z���^�[���ڎ����Ή�
        If strFuriCode = "" AndAlso strKigyoCode = "" Then
            strItakuKanji = DB.GetItakuKanji(strItakuCode)
        Else
            strItakuKanji = DB.GetItakuKanji(itaku, strFuriCode, strKigyoCode)
            '�ϑ��҃R�[�h�𐳂����l�ɓǑ�
            Me.strItakuCode = itaku
        End If
        '******************************************************************
        '*** ASTAR.S.S �}�̃R�[�h�ǉ�   2008.05.23  ***
        '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ� START
        'If Not DB.GetFToriCode(itaku, syubetu, strToriSCode, strToriFCode, strFSyoriKbn, strBaitaiCode, strItakuKanriCode, strMultiKbn, nFormatkbn) Then
        If Not DB.bGetFToriCode(itaku, syubetu, strToriSCode, strToriFCode, strFSyoriKbn, strBaitaiCode, strItakuKanriCode, strMultiKbn, nFormatkbn, furiDate) Then
            '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ�  END
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�����敪�A������R�[�h�A���R�[�h�̎擾���s �ϑ��҃R�[�h�F" & itaku
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        ' ���Z�@�֊�����, �x�X�������̎擾
        If Not noBankFlg Then
            If DB.GetBankAndBranchName(bankcode, branchcode, Me.strBankName, Me.strBranchName) Then
                'MessageBox.Show("���Z�@�֖�:" & Me.strBankName & ", �x�X��:" & Me.strBranchName)
            Else
                Me.strBankName = " -- "
                Me.strBranchName = " -- "
                ' MessageBox.Show("���Z���ԏ��擾���s")
                '***ASTAR SUSUKI 2008.06.13                 ***
                '***���Z�@�փ`�F�b�N���͂���
                'With GCom.GLog
                '    .Result = "���Z�@�֖��A�x�X���擾���s"
                '    .Discription = "���Z�@�փR�[�h:" & bankcode & ", �x�X�R�[�h:" & branchcode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                'Return False
                '***                                        ***
            End If
        End If
        Return True
    End Function

    ' �@�\�@�@�@: �f�[�^���R�[�h�̌����̃J�E���g�Ƌ��z�̐ݒ�
    '
    ' �߂�l�@�@: ���� True / ���s False
    '
    ' �������@�@: ARG1 - �������z
    '
    ' ���l�@�@�@: 
    Public Function SetData(ByVal hikikin As Decimal) As Boolean
        '
        If (hikikin < 0) Then
            Return False
        End If
        nCheckTotalKin += hikikin
        nCheckTotalKen += 1
        Return True
    End Function

    ' �@�\�@�@�@: �g���[�����̐ݒ�
    '
    ' �߂�l�@�@: ���� True / ���s False
    '
    ' �������@�@: ARG1 - ��������
    '             ARG2 - �������z
    '             ARG3 - �U�֍ό���
    '             ARG4 - �U�֍ϋ��z
    '             ARG5 - �s�\����
    '             ARG6 - �s�\���z
    '                     
    ' ���l�@�@�@: 
    Public Function SetTrailer(ByVal syoriken As Decimal, ByVal syorikin As Decimal _
        , ByVal furiken As Decimal, ByVal furikin As Decimal _
        , ByVal funouken As Decimal, ByVal funoukin As Decimal) As Boolean
        '  ���l�G���[�`�F�b�N
        Me.nSyoriKen = syoriken
        Me.nSyoriKin = syorikin
        Me.nFuriKen = furiken
        Me.nFuriKin = furikin
        Me.nFunouKen = funouken
        Me.nFunouKin = funoukin

        If (syoriken < 0) Or (syorikin < 0) Or (furiken < 0) Or (furikin < 0) _
            Or (funouken < 0) Or (funoukin < 0) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�f�[�^�g���[���s���� �w�b�_���������F" & nSyoriKen & ",���v�����F" & nCheckTotalKen & ", �w�b�_�������z�F" _
                & nSyoriKin & ",���v�������z�F" & nCheckTotalKin
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        ' �f�[�^���R�[�h�̍��v�l�ƃg���[�����R�[�h�̒l���r
        If (Me.nSyoriKen <> Me.nCheckTotalKen) Or (Me.nSyoriKin <> Me.nCheckTotalKin) Then
            ' �s��������
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�f�[�^�g���[���s���� �w�b�_���������F" & Me.nSyoriKen & ",���v�����F" & Me.nCheckTotalKen & ", �w�b�_�������z�F" _
                & Me.nSyoriKen & ",���v�������z�F" & Me.nCheckTotalKin
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        Return True
    End Function

End Class