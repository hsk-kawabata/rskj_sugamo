Option Explicit On 
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Data.OracleClient

' �@�@�\ : CMT�t�@�C���̃f�[�^�A�N�Z�X�p�̃N���X�B
'            
'
'
Public Class clsCmtData
    Implements IDisposable

    Private Const ThisModuleName As String = "clsCmtData.vb"

    ' �e�[�u��(CMT_READ_TBL / CMT_WRITE_TBL)�ɗ����o�����鍀�ڂ����X�g�A�b�v
    '          �ϐ���      �ێ��p�ϐ��̌^   DB���ږ�
    Protected nReceptionNo As Integer       ' ��t�ԍ�
    Protected strSyoriDate As String        ' ������(�N�����I�v�V�����œn���ꂽ�V�X�e���ғ���)  
    Protected strStationNo As String        ' CMT�ǎ�@��
    'Protected gstrPCName As String          ' DB�ɂ͖������֗��Ȃ��ߕێ�
    Protected nErrCode As Integer           ' �G���[�ԍ�
    Protected strErrName As String          ' �G���[��
    Protected nStackerNo As Integer         ' �X�^�b�J�[�ԍ�
    Protected strFileName As String         ' �����t�@�C����
    Protected bComplockFlag As Boolean       ' �Í����t���O       
    Public ReadSucceedFlag As Boolean       ' �Ǎ������t���O
    Protected CreateDate As Date            ' �쐬��
    Protected UpdateDate As Date            ' �X�V��
    Protected bJSFlag As Boolean            ' ���U�����U�t���O
    Public bUploadSucceedFlg As Boolean     ' �A�b�v���[�h���ۃt���O
    Public bListUpFlag As Boolean           ' ListView�\���t���O
    Protected bRWFlag As Boolean            ' �Ǎ�/�����t���O

    ' �e�[�u���ɂ͖�������
    Protected nItakuCounter As Integer = -1 ' �S��t�@�C�����̃w�b�_�̐�
    Public ItakuData() As clsItakuData      ' �ϑ��Җ��̃f�[�^

    ' CmtWriteData�Œǉ����鍀��
    Protected nWriteCounter As Integer ' �t�@�C��������(�����̃t�@�C�������񏑂����񂾂����L�^)
    Protected bOverrideFlg As Boolean ' ���������݃t���O true�̂Ƃ����������ݎ��{

    ' �@�\�@�@�@: �R���X�g���N�^ 
    ' ����      : �Ȃ��A�̏ꍇ�̏���
    Public Sub New()
        Init(1, 1, False, True)
    End Sub

    ' �@�\�@�@�@: �R���X�g���N�^
    ' ����      : ARG1 - ��t�ԍ�
    '           : ARG2 - �X�^�b�J�ԍ�
    '           : ARG3 - ���x���L���t���O
    '           : ARG4 - �Í����t���O
    Public Sub New(ByVal nReceptionNo As Integer _
    , ByVal nStackerNo As Integer _
    , ByVal ComplockFlag As Boolean _
    , ByVal JSFlag As Boolean _
    , ByVal rwflg As Boolean)
        ' ����������
        Init(nReceptionNo, nStackerNo, ComplockFlag, rwflg)
    End Sub

    ' �@�\�@�@�@: ������
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - ��t�ԍ�
    '       �@�@: ARG2 - �X�^�b�J�ԍ�
    '       �@�@: ARG3 - ���x���L���t���O(True:���x���L / False:���x������)
    '       �@�@: ARG4 - �Í����t���O (False:���� / True:�Í�)
    '           : ARG5 - Read/Write�t���O (True : Read / False : Write)
    ' ���l�@�@�@: �Ȃ�
    Public Sub Init(ByVal recep As Integer _
        , ByVal nStackerNo As Integer _
        , ByVal compFlag As Boolean _
        , ByVal rwflg As Boolean)
        Me.nReceptionNo = recep
        Me.nStackerNo = nStackerNo
        Me.strStationNo = CmtCom.gstrStationNo
        Me.strSyoriDate = CmtCom.gstrSysDate
        Me.strFileName = "none"

        Me.nErrCode = 0
        Me.strErrName = "����"
        Me.bComplockFlag = compFlag
        Me.bUploadSucceedFlg = False
        Me.bListUpFlag = False
        Me.bOverrideFlg = False
        Me.nWriteCounter = 0
        Me.bJSFlag = True ' �f�t�H���g�ł͎��U
        Me.bRWFlag = rwflg

        nItakuCounter = -1
    End Sub

    ' ��t�ԍ���Ԃ�
    Property ReceptionNo() As Integer
        Get
            Return nReceptionNo
        End Get
        Set(ByVal Value As Integer)
            nReceptionNo = Value
        End Set
    End Property

    ' �w�b�_���X�L���������ۂɐݒ肷�鍀��
    ' �@�\�@�@�@: �w�b�_���X�L���������ۂɐݒ肷�鍀��
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - �U�֓�
    '         �@: ARG2 - �ϑ��҃R�[�h
    '
    ' ���l�@�@�@: �Ȃ�
    '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����̏ꍇ�͐U�փR�[�h�E��ƃR�[�h���n�� ***
    'Public Function AddHeaderRecord(ByVal fileseq As Integer, ByVal strFuriDate As String, ByVal strItakuCode As String, ByVal strItakuKana As String _
    '    , ByVal strBankCode As String, ByVal strBranchCode As String) As Boolean
    Public Function AddHeaderRecord(ByVal fileseq As Integer, ByVal strFuriDate As String, ByVal strItakuCode As String, ByVal strItakuKana As String _
       , ByVal strBankCode As String, ByVal strBranchCode As String _
        , Optional ByVal strFuriCode As String = "", Optional ByVal strKigyoCode As String = "", Optional ByVal Syubetu As String = "", Optional ByVal nFormatKbn As Integer = 0) As Boolean
        '******************************************************************************
        ' �ϑ��҃C���X�^���X�̒ǉ��Ɛݒ�
        Me.nItakuCounter += 1
        ReDim Preserve ItakuData(nItakuCounter)

        '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h���n�� ***
        'ItakuData(nItakuCounter) = New clsItakuData(fileseq, strFuriDate, strItakuCode, strItakuKana, strBankCode, strBranchCode)
        ItakuData(nItakuCounter) = New clsItakuData(fileseq, strFuriDate, strItakuCode, strItakuKana, strBankCode, strBranchCode, strFuriCode, strKigyoCode, Syubetu, nFormatKbn)
        '**********************************************************

        Return True
    End Function

    ' Complock��init
    ' �@�\�@�@�@: complock�ł̏������֐�
    '
    ' �߂�l�@�@: ����I�� true / �ُ�I�� false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '         �@: ARG2 - ������R�[�h
    '           : ARG3 - ����敛�R�[�h
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function ComplockInit(ByVal itakucd As String, ByVal furidate As String) As Boolean

        Me.bComplockFlag = True

        ' �ϑ��҃C���X�^���X�̒ǉ��Ɛݒ�
        nItakuCounter += 1
        ReDim Preserve ItakuData(nItakuCounter)
        ItakuData(nItakuCounter) = New clsItakuData(False)
        If (itakucd = "") Then
            ItakuData(nItakuCounter).strItakuCode = "0000000000"
            ItakuData(nItakuCounter).strItakuKanji = " -- "
            ItakuData(nItakuCounter).strItakuKana = " -- "
            ItakuData(nItakuCounter).strToriSCode = "0000000"
            ItakuData(nItakuCounter).strToriFCode = "00"
            ItakuData(nItakuCounter).strFSyoriKbn = "0"
            ItakuData(nItakuCounter).strFuriDate = "00000000"
            '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
            ItakuData(nItakuCounter).strBaitaiCode = "00"
            '**********************************************
        Else
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim str1 As String = String.Empty
            Dim str2 As String = String.Empty
            Dim fsyori As String = String.Empty
            'Dim str1, str2, fsyori As String
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
            '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
            Dim baitaiCode As String = ""
            Dim itakukanricode As String = ""
            Dim multikbn As String = ""
            '**********************************************
            Call DB.GetFToriCode(itakucd, "", str1, str2, fsyori, 0) ' �ϑ��҃R�[�h������ɐݒ肳��Ă��Ȃ��ꍇ���f�ʂ�

            ItakuData(nItakuCounter).strItakuCode = itakucd
            ItakuData(nItakuCounter).strItakuKanji = DB.GetItakuKanji(itakucd.PadLeft(10, "0"c))
            ItakuData(nItakuCounter).strItakuKana = DB.GetItakuKana(itakucd.PadLeft(10, "0"c))
            ItakuData(nItakuCounter).strToriSCode = str1
            ItakuData(nItakuCounter).strToriFCode = str2
            ItakuData(nItakuCounter).strFSyoriKbn = fsyori
            ItakuData(nItakuCounter).strFuriDate = furidate
            '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
            Call DB.GetFToriCode(itakucd, "", str1, str2, fsyori, baitaiCode, itakukanricode, multikbn) ' �ϑ��҃R�[�h������ɐݒ肳��Ă��Ȃ��ꍇ���f�ʂ�
            ItakuData(nItakuCounter).strBaitaiCode = baitaiCode
            '**********************************************
        End If
        Return True
    End Function

    ' �@�\�@�@�@: ��̈ϑ��f�[�^�𐶐�
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - RWFlag  (�Ǎ����ʁ�True / �������ʂ̕ێ� False)
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function InitEmptyItakuData(ByVal rw As Boolean) As Boolean
        Me.ComplockInit("", "")     ' �ǎ�ł��Ȃ��������߁A�ϑ��ҕs��
        If (rw) Then
            Me.setError(9, "CMT�ǎ�G���[")
        Else
            Me.setError(9, "�ԋp�t�@�C���ǎ�G���[")
        End If

        Me.ComplockFlag = False     ' �Í������ꂽ���̂͏����ΏۊO�̂��߁A�S��False
        Me.Override = False         ' �G���[�`�F�b�N�ň������������ۂ̏����̂��߁A�S��False
        Me.bListUpFlag = True       ' �G���[���e��\�����邽��true
        Me.RWFlag = rw
        With GCom.GLog
            .Result = "�s��"
            If (rw) Then
                .Discription = "CMT�t�@�C���ǎ�G���["
            Else
                .Discription = "�ԋp�t�@�C���ǎ�G���["
            End If
            .Discription &= " �t�@�C�����j�����Ă���\��������܂�"
        End With
        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    End Function

    ' �@�\�@�@�@: ��̈ϑ��f�[�^�𐶐�(�G���[�o�͂���)
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - RWFlag  (�Ǎ����ʁ�True / �������ʂ̕ێ� False)
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function InitEmptyItakuDataNoErr(ByVal rw As Boolean) As Boolean
        Me.ComplockInit("", "")     ' �ǎ�ł��Ȃ��������߁A�ϑ��ҕs��

        Me.ComplockFlag = False     ' �Í������ꂽ���̂͏����ΏۊO�̂��߁A�S��False
        Me.Override = False         ' �G���[�`�F�b�N�ň������������ۂ̏����̂��߁A�S��False
        Me.bListUpFlag = True       ' �������ʂ�\�����邽��true
        Me.RWFlag = rw
    End Function


    ' �@�\�@�@�@: �f�[�^���R�[�h���X�L���������ۂɐݒ肷�鍀��
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - �������Ƃ����z
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function AddDataRecord(ByVal nHikiKin As Decimal) As Boolean
        ' �������Ƃ����z���ϑ��҃C���X�^���X�ɍ��m
        Return ItakuData(nItakuCounter).SetData(nHikiKin)
    End Function

    ' �@�\�@�@�@: �g���[�����R�[�h���X�L���������ۂɐݒ肷�鍀��
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - ����
    '             ARG2 - ���z
    '             ARG3 - �U�֍ό���
    '             ARG4 - �U�֍ϋ��z
    '             ARG5 - �s�\����
    '             ARG6 - �s�\���z
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function AddTrailerRecord(ByVal ken As Decimal, ByVal kin As Decimal, _
           ByVal furiken As Decimal, ByVal furikin As Decimal, _
           ByVal funouken As Decimal, ByVal funoukin As Decimal) As Boolean
        ' �������Ƃ����z���ϑ��҃C���X�^���X�ɍ��m
        Return ItakuData(nItakuCounter).SetTrailer(ken, kin, furiken, furikin, funouken, funoukin)
    End Function

    ' �ϑ��J�E���^�̎擾
    Property ItakuCounter() As Integer
        Get
            Return nItakuCounter
        End Get
        Set(ByVal Value As Integer)
            nItakuCounter = Value
        End Set
    End Property

    ' �@�\�@�@�@: �t�@�C�������擾
    Property FileName() As String
        Get
            Return Me.strFileName
        End Get
        Set(ByVal Value As String)
            Me.strFileName = Value
        End Set
    End Property

    ' �@�\�@�@�@: �����񐔂̎擾
    Property WriteCounter() As Integer
        Get
            Return Me.nWriteCounter
        End Get
        Set(ByVal Value As Integer)
            Me.nWriteCounter = Value
        End Set
    End Property




    ' �@�\�@�@�@: �G���[����ݒ�
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - �G���[�ԍ�
    '             ARG2 - �G���[��
    ' ���l�@�@�@: �Ȃ�
    Public Sub setError(ByVal nerrCode As Integer, ByVal strerrName As String)
        Me.nErrCode = nerrCode
        Me.strErrName = strerrName

    End Sub

    ' �@�\�@�@�@: �G���[����
    '
    ' �߂�l�@�@: �G���[�ԍ���0�ŃG���[���Ȃ��Ƃ� True / �G���[�ԍ���0�ȊO�̂Ƃ� False
    '
    ' �������@�@: �Ȃ�
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function NotError() As Boolean
        If (nErrCode = 0) Then
            Return True
        Else
            Return False
        End If
    End Function

    ' �@�\�@�@�@: �N���X�j�����̏���
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: �Ȃ�
    '
    ' ���l�@�@�@: �Ȃ�
    Public Sub Dispose() Implements System.IDisposable.Dispose
        ' �N���X��j������Ƃ��ɍs���������L�q
    End Sub

    ' �@�\�@�@�@: CMT�Ǎ����уe�[�u���p��Insert�����s���邽�߂�SQL���𐶐�
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ���� true / ���s false
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function GetInsertSQL(ByVal nItakuCounter As Integer) As String
        '*** �C�� mitsu 2008/09/01 ���������� ***
        Dim strSQL As New StringBuilder(512)
        strSQL.Append("INSERT INTO CMT_READ_TBL(" _
            & "RECEPTION_NO" _
            & ", SYORI_DATE" _
            & ", FILE_SEQ" _
            & ", STATION_NO" _
            & ", FSYORI_KBN" _
            & ", TORIS_CODE" _
            & ", TORIF_CODE" _
            & ", ITAKU_CODE" _
            & ", FURI_DATE" _
            & ", SYORI_KEN" _
            & ", SYORI_KIN" _
            & ", FURI_KEN" _
            & ", FURI_KIN" _
            & ", FUNOU_KEN" _
            & ", FUNOU_KIN" _
            & ", ERR_CD" _
            & ", ERR_INFO" _
            & ", STACKER_NO" _
            & ", FILE_NAME" _
            & ", COMPLOCK_FLG" _
            & ", JS_FLG" _
            & ")")

        With ItakuData(nItakuCounter)
            strSQL.Append(" VALUES(")
            strSQL.Append(nReceptionNo)                                      ' 1.��tNo
            strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.�V�X�e��������
            strSQL.Append(", " & .nFileSeq)                                  ' 3.FILE_SEQ
            strSQL.Append(", '" & strStationNo & "' ")                       ' 4.��tPC�̋@��
            strSQL.Append(", '" & .strFSyoriKbn & "' ")                      ' 5.F�����敪
            strSQL.Append(", '" & .strToriSCode.PadLeft(7, "0"c) & "' ")     ' 6.������R�[�h
            strSQL.Append(", '" & .strToriFCode.PadLeft(2, "0"c) & "' ")     ' 7.����敛�R�[�h
            strSQL.Append(", '" & .strItakuCode.PadLeft(10, "0"c) & "' ")    ' 8.�ϑ��҃R�[�h
            strSQL.Append(", '" & .strFuriDate & "' ")                       ' 9.�U�֓�
            strSQL.Append(", " & .nSyoriKen.ToString)                        ' 10.����  
            strSQL.Append(", " & .nSyoriKin.ToString)                        ' 11.���z
            strSQL.Append(", " & .nFuriKen.ToString)                         ' 12.�U�֍ό���  
            strSQL.Append(", " & .nFuriKin.ToString)                         ' 13.�U�֍ϋ��z
            strSQL.Append(", " & .nFunouKen.ToString)                        ' 14.�s�\����  
            strSQL.Append(", " & .nFunouKin.ToString)                        ' 15.�s�\���z
            strSQL.Append(", " & nErrCode.ToString)                          ' 16.�G���[�R�[�h 
            strSQL.Append(", '" & strErrName & "' ")                         ' 17.�G���[��
            strSQL.Append(", " & nStackerNo.ToString)                        ' 18.�X�^�b�J�ԍ�
            strSQL.Append(", '" & strFileName & "' ")                        ' 19.�����t�@�C����
            If bComplockFlag Then                                       ' 20.�Í����t�@�C��
                strSQL.Append(", '1' ")                            ' �Í� 
            Else
                strSQL.Append(", '0' ")                            ' ����
            End If
            If Me.bJSFlag Then                                          ' 21.���U�E���U
                strSQL.Append(", '1' ")                            ' ���U
            Else
                '***Astar���� 2008/05/30
                strSQL.Append(", '0' ")                            ' ���U

                ''***Astar���� 2008/05/30
                ''strSQL.Append(", '0' ")                            ' ���U
                'strSQL.Append(", '3' ")                            ' ���U
                ''***
                '***
            End If
            strSQL.Append(")")
        End With

        Return strSQL.ToString
        '****************************************
    End Function

    ' �@�\�@�@�@: CMT�������уe�[�u���p��Insert�����s���邽�߂�SQL���𐶐�
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ���� true / ���s false
    '
    ' ���l�@�@�@: �Ȃ�
    Public Function GetWInsertSQL(ByVal nItakuCounter As Integer) As String
        '*** �C�� mitsu 2008/09/01 ���������� ***
        Dim strSQL As New StringBuilder(512)
        strSQL.Append("INSERT INTO CMT_WRITE_TBL(" _
            & "RECEPTION_NO" _
            & ", SYORI_DATE" _
            & ", FILE_SEQ" _
            & ", STATION_NO" _
            & ", FSYORI_KBN" _
            & ", TORIS_CODE" _
            & ", TORIF_CODE" _
            & ", ITAKU_CODE" _
            & ", FURI_DATE" _
            & ", SYORI_KEN" _
            & ", SYORI_KIN" _
            & ", FURI_KEN" _
            & ", FURI_KIN" _
            & ", FUNOU_KEN" _
            & ", FUNOU_KIN" _
            & ", ERR_CD" _
            & ", ERR_INFO" _
            & ", STACKER_NO" _
            & ", FILE_NAME" _
            & ", COMPLOCK_FLG" _
            & ", WRITE_COUNTER" _
            & ", OVERRIDE_FLG" _
            & ")")

        If (nItakuCounter >= 0) Then
            With ItakuData(nItakuCounter)
                strSQL.Append(" VALUES(")
                strSQL.Append(nReceptionNo)                                      ' 1.��tNo
                strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.�V�X�e��������
                strSQL.Append(", " & .nFileSeq)                                  ' 3.FILE_SEQ
                strSQL.Append(", '" & strStationNo & "' ")                       ' 4.��tPC�̋@��
                strSQL.Append(", '" & .strFSyoriKbn & "' ")                      ' 5.F�����敪
                strSQL.Append(", '" & .strToriSCode.PadLeft(7, "0"c) & "' ")     ' 6.������R�[�h
                strSQL.Append(", '" & .strToriFCode.PadLeft(2, "0"c) & "' ")     ' 7.����敛�R�[�h
                strSQL.Append(", '" & .strItakuCode & "' ")                      ' 8.�ϑ��҃R�[�h
                strSQL.Append(", '" & .strFuriDate & "' ")                       ' 9.�U�֓�
                strSQL.Append(", " & .nSyoriKen.ToString)                        ' 10.����  
                strSQL.Append(", " & .nSyoriKin.ToString)                        ' 11.���z
                strSQL.Append(", " & .nFuriKen.ToString)                         ' 12.�U�֍ό���  
                strSQL.Append(", " & .nFuriKin.ToString)                         ' 13.�U�֍ϋ��z
                strSQL.Append(", " & .nFunouKen.ToString)                        ' 14.�s�\����  
                strSQL.Append(", " & .nFunouKin.ToString)                        ' 15.�s�\���z
                strSQL.Append(", " & nErrCode.ToString)                          ' 16.�G���[�R�[�h 
                strSQL.Append(", '" & strErrName & "' ")                         ' 17.�G���[��
                strSQL.Append(", " & nStackerNo.ToString)                        ' 18.�X�^�b�J�ԍ�
                strSQL.Append(", '" & strFileName & "' ")                        ' 19.�����t�@�C����
                If (bComplockFlag) Then                                     ' 20.�Í����t�@�C��
                    strSQL.Append(", '1' ")                            ' �Í� 
                Else
                    strSQL.Append(", '0' ")                            ' ����
                End If
                strSQL.Append(", " & nWriteCounter.ToString)                     ' 21.������
                If (bOverrideFlg) Then                                      ' 22.���������t���O
                    strSQL.Append(", '1' ")                            ' ����
                Else
                    strSQL.Append(", '0' ")                            ' �ʏ�
                End If
                strSQL.Append(")")
            End With
        Else
            strSQL.Append(" VALUES(")
            strSQL.Append(nReceptionNo)                                      ' 1.��tNo
            strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.�V�X�e��������
            strSQL.Append(", 0")                                             ' 3.FILE_SEQ
            strSQL.Append(", '" & strStationNo & "' ")                       ' 4.��tPC�̋@��
            strSQL.Append(", '0' ")                                          ' 5.F�����敪
            strSQL.Append(", '0000000' ")                                    ' 6.������R�[�h
            strSQL.Append(", '00' ")                                         ' 7.����敛�R�[�h
            strSQL.Append(", '0000000000'")                                  ' 8.�ϑ��҃R�[�h
            strSQL.Append(", '00000000'")                                    ' 9.�U�֓�
            strSQL.Append(", 0")                                             ' 10.����  
            strSQL.Append(", 0")                                             ' 11.���z
            strSQL.Append(", 0")                                             ' 12.�U�֍ό���  
            strSQL.Append(", 0")                                             ' 13.�U�֍ϋ��z
            strSQL.Append(", 0")                                             ' 14.�s�\����  
            strSQL.Append(", 0")                                             ' 15.�s�\���z
            strSQL.Append(", " & nErrCode.ToString)                          ' 16.�G���[�R�[�h 
            strSQL.Append(", '" & strErrName & "' ")                         ' 17.�G���[��
            strSQL.Append(", " & nStackerNo.ToString)                        ' 18.�X�^�b�J�ԍ�
            strSQL.Append(", '" & strFileName & "' ")                        ' 19.�����t�@�C����
            If (bComplockFlag) Then                                     ' 20.�Í����t�@�C��
                strSQL.Append(", '1' ")                            ' �Í� 
            Else
                strSQL.Append(", '0' ")                            ' ����
            End If
            strSQL.Append(", " & nWriteCounter.ToString)                     ' 21.������
            If (bOverrideFlg) Then                                      ' 22.���������t���O
                strSQL.Append(", '1' ")                            ' ����
            Else
                strSQL.Append(", '0' ")                            ' �ʏ�
            End If
            strSQL.Append(")")
        End If

        Return strSQL.ToString
        '****************************************
    End Function

    ' �@�\�@�@�@: clsCMTData�̃����o�ϐ��̏�񂩂�ListViewItem��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ListViewItem
    ' ���l�@�@�@: �Ǎ�/�������Ή� 2007/12/07 �O�c
    Public Function getListViewItem(ByRef lv As ListViewItem, ByVal idx As Integer) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim createDate As String = String.Empty
        Dim updateDate As String = String.Empty
        'Dim createDate As String
        'Dim updateDate As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        If (idx < 0) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���R�[�h�Ɉϑ��ҏ�񖳂� ��t�ԍ��F" & Me.nReceptionNo.ToString
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        Try
            lv.SubItems.Add(ItakuData(idx).nFileSeq.ToString.PadLeft(2, "0"c)) ' 01.FILE_SEQ
            lv.SubItems.Add(Me.strSyoriDate)                                    ' 02.������
            lv.SubItems.Add(nStackerNo.ToString.PadLeft(2, "0"c))               ' 03.�X�^�b�J�ԍ�
            If Me.bRWFlag Then                                                  ' 04.������
                ' �Ǎ�
                lv.SubItems.Add("-")                                            '  - �Ǎ��Ȃ̂Ŗ���
            Else
                ' ����
                lv.SubItems.Add(nWriteCounter.ToString)                         '  - �����񐔂��o��
            End If

            lv.SubItems.Add(ItakuData(idx).strItakuCode.PadLeft(10, "0"c))  ' 05.�ϑ��҃R�[�h(FILE_SEQ��)
            lv.SubItems.Add(ItakuData(idx).strFuriDate)                     ' 06.�U�֓�(FILE_SEQ��)
            lv.SubItems.Add(ItakuData(idx).strItakuKana)                    ' 07.�ϑ��҃J�i
            lv.SubItems.Add(ItakuData(idx).strItakuKanji)                   ' 08.�ϑ��҃J�i
            lv.SubItems.Add(ItakuData(idx).nSyoriKen.ToString)              ' 09.����
            lv.SubItems.Add(ItakuData(idx).nSyoriKin.ToString)              ' 10.���z
            lv.SubItems.Add(ItakuData(idx).nCheckTotalKen.ToString)         ' 11.�f�[�^���R�[�h���v����
            lv.SubItems.Add(ItakuData(idx).nCheckTotalKin.ToString)         ' 12.�f�[�^���R�[�h���v���z
            lv.SubItems.Add(nErrCode.ToString.PadLeft(1, "0"c))             ' 13.�G���[�R�[�h
            lv.SubItems.Add(strErrName)                                     ' 14.�G���[��
            lv.SubItems.Add(ItakuData(idx).strBankCode.PadLeft(4, "0"c))    ' 15.���Z�@�փR�[�h
            lv.SubItems.Add(ItakuData(idx).strBankName)                     ' 16.���Z�@�֖�
            lv.SubItems.Add(ItakuData(idx).strBranchCode.PadLeft(3, "0"c))  ' 17.�x�X�R�[�h
            lv.SubItems.Add(ItakuData(idx).strBranchName)                   ' 18.�x�X��
            If (Me.bRWFlag) Then                                                                      ' 19.���U�E���U�t���O
                If (Me.bJSFlag) Then
                    lv.SubItems.Add("��")                                   '  - ���U
                Else
                    lv.SubItems.Add("��")                                   '  - ���U
                End If
            Else
                ' �����ɂ͎��U�����Ȃ�
                lv.SubItems.Add("��")                                       '  - ���U
            End If

            lv.SubItems.Add(ItakuData(idx).nFuriKen.ToString)               ' 20.�U�֍ό���
            lv.SubItems.Add(ItakuData(idx).nFuriKin.ToString)               ' 21.�U�֍ϋ��z
            lv.SubItems.Add(ItakuData(idx).nFunouKen.ToString)              ' 22.�s�\����
            lv.SubItems.Add(ItakuData(idx).nFunouKin.ToString)              ' 23.�s�\���z
            lv.SubItems.Add(ItakuData(idx).strFSyoriKbn)                    ' 24.F�����敪
            lv.SubItems.Add(ItakuData(idx).strToriSCode)                    ' 25.������R�[�h
            lv.SubItems.Add(ItakuData(idx).strToriFCode)                    ' 26.����敛�R�[�h
            lv.SubItems.Add(Me.strStationNo)                                ' 27.CMT�ǎ�@��
            lv.SubItems.Add(Me.FileName)                                 ' 28�t�@�C����
            If Not bRWFlag Then                                                 ' 29.���������݃t���O
                ' ������
                If Me.bOverrideFlg Then
                    lv.SubItems.Add("����")                                 '  - ����
                Else
                    lv.SubItems.Add("�ʏ�")                                 '  - �ʏ�
                End If
            Else
                ' �Ǎ���
                lv.SubItems.Add(" - ")                                      '  - ���g�p
            End If

            If Me.bComplockFlag Then                                                                  ' 30.�Í����t���O
                lv.SubItems.Add("C")
            Else
                lv.SubItems.Add("N")                                        '  - N:����
            End If

            Me.GetCreateDate(Me.ItakuData(idx).nFileSeq, createDate, updateDate)
            lv.SubItems.Add(createDate)                                     ' 31.�쐬��
            lv.SubItems.Add(updateDate)                                     ' 32.�X�V��

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListViewItem�������� " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return True
    End Function

    ' �@�\�@�@�@: CMT�Ǎ�����TBL/CMT��������TBL�̐������E�X�V�����擾
    '
    ' �߂�l�@�@: �擾���� true / �擾���s false
    '
    ' ������    : ARG1 FILE_SEQ
    '           : ARG2 ������(�Q�Ɠn��)
    '           : ARG3 �X�V��(�Q�Ɠn��)
    ' ���l�@�@�@: 
    Private Function GetCreateDate(ByVal fileseq As Integer, ByRef strCreateDate As String, ByRef strUpdateDate As String) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim strTbl As String

        If Me.bRWFlag Then
            strTbl = "CMT_READ_TBL"
        Else
            strTbl = "CMT_WRITE_TBL"
        End If

        If fileseq = 0 Then ' FILE_SEQ���[���̂Ƃ������͎��s���Ȃ�
            strUpdateDate = " --- "
            strCreateDate = " --- "
            Return False
        End If

        Try
            Dim SQL As String = "SELECT "
            SQL &= "  CREATE_DATE"
            SQL &= ", UPDATE_DATE"
            SQL &= "  FROM " & strTbl
            SQL &= "  WHERE RECEPTION_NO = " & Me.nReceptionNo.ToString()
            SQL &= "  AND FILE_SEQ = " & fileseq.ToString
            SQL &= "  AND SYORI_DATE = '" & Me.strSyoriDate & "'"
            SQL &= "  AND STATION_NO = '" & CmtCom.gstrStationNo & "'"
            If GCom.SetDynaset(SQL, onReader) AndAlso onReader.Read Then
                strCreateDate = onReader.GetOracleDateTime(0).ToString
                strUpdateDate = onReader.GetOracleDateTime(1).ToString
                Return True
            Else
                '*** �C�� mitsu 2008/09/01 �s�v ***
                'With GCom.GLog
                '    .Result = "�������E�X�V���擾���s"
                '    .Discription = "SQL��:" & SQL
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                strUpdateDate = " --- "
                strCreateDate = " --- "
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�������E�X�V���擾���s " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            strUpdateDate = " -- "
            strCreateDate = " -- "
            Return False
        End Try
    End Function

    ' �@�\�@�@�@: ����CMT�f�[�^�\���̂Ɣ�r
    '             �������ƈϑ������̃J�E���^���r������A�ϑ��f�[�^�̔�r���s���B
    '
    ' �߂�l�@�@: ���v true / �s��v false
    '
    ' �������@�@: clsCmtData�̃C���X�^���X
    ' ���l�@�@�@: 
    Public Function CompareWR(ByRef cmtData As clsCmtData) As Boolean

        If Me.strSyoriDate = cmtData.strSyoriDate And _
            Me.nItakuCounter = cmtData.nItakuCounter And _
            Me.bJSFlag = cmtData.bJSFlag Then
            For i As Integer = 0 To Me.nItakuCounter - 1
                With cmtData.ItakuData(i)
                    If Me.ItakuData(i).nCheckTotalKen = .nCheckTotalKen And _
                        Me.ItakuData(i).nCheckTotalKin = .nCheckTotalKin And _
                        Me.ItakuData(i).nSyoriKen = .nSyoriKen And _
                        Me.ItakuData(i).nSyoriKin = .nSyoriKin And _
                        Me.ItakuData(i).strBankCode = .strBankCode And _
                        Me.ItakuData(i).strBranchCode = .strBranchCode And _
                        Me.ItakuData(i).strItakuCode = .strItakuCode And _
                        Me.ItakuData(i).strItakuKana = .strItakuKana Then
                        Return True
                    Else
                        Return False
                    End If
                End With
            Next
            Return True
        Else
            Return False
        End If
    End Function


    ' ���������݃t���O�̐ݒ�
    Property Override() As Boolean
        Get
            Return bOverrideFlg
        End Get
        Set(ByVal Value As Boolean)
            bOverrideFlg = Value
        End Set
    End Property

    ' ���U/���U�t���O�̐ݒ�
    Property JSFlag() As Boolean
        Get
            Return bJSFlag
        End Get
        Set(ByVal Value As Boolean)
            bJSFlag = Value
        End Set
    End Property

    ' �Ǐ��t���O�̐ݒ�
    Property RWFlag() As Boolean
        Get
            Return bRWFlag
        End Get
        Set(ByVal Value As Boolean)
            bRWFlag = Value
        End Set
    End Property

    ' �Í����t���O�̐ݒ�
    Property ComplockFlag() As Boolean
        Get
            Return Me.bComplockFlag
        End Get
        Set(ByVal Value As Boolean)
            Me.bComplockFlag = Value
        End Set
    End Property

    ' �G���[�R�[�h�擾
    Property ErrCode() As Integer
        Get
            Return Me.nErrCode
        End Get
        Set(ByVal Value As Integer)
            Me.nErrCode = Value
        End Set
    End Property

    ' �G���[���̐ݒ�
    Property ErrorName() As String
        Get
            Return Me.strErrName
        End Get
        Set(ByVal Value As String)
            Value = Me.strErrName
        End Set
    End Property
End Class
