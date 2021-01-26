Option Explicit On 
Option Strict On

Imports System.Data.OracleClient

Public Class clsDataBase

    Private Const ThisModuleName As String = "clsDataBase.vb"

    ' �@�\�@�@�@: �����ꗗ��Ԃ�
    '
    ' �߂�l�@�@: �J�[�\���s�̗L��
    '
    ' �������@�@: ARG1 - Reader Object
    '
    ' ���l�@�@�@: �ėp
    '
    Public Function GetToriCode(ByRef OraReader As OracleDataReader) As Boolean
        Dim SQL As String = ""
        Try
            SQL = "SELECT TORIS_CODE"
            SQL &= ", TORIF_CODE"
            SQL &= ", ITAKU_CODE"
            SQL &= ", ITAKU_NNAME"
            SQL &= ", ITAKU_KNAME"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ENC_KBN = '1'"
            'SQL &= " WHERE FSYORI_KBN = '1'"
            'SQL &= " AND ENC_KBN = '1'"
            SQL &= " ORDER BY ITAKU_KNAME ASC"

            Return GCom.SetDynaset(SQL, OraReader)
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function

    ' �@�\�@�@�@: ������R�[�h�A���R�[�h�AF�����敪��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h(��L�[)
    '           : ARG2 - ������R�[�h(�Q�Ɠn��)
    '           : ARG3 - ����敛�R�[�h(�Q�Ɠn��)
    '           : ARG4 - F�����敪(�Q�Ɠn��)
    '
    ' ���l�@�@�@: 
    '
    Public Function GetFToriCode(ByVal itakuCode As String, ByVal Syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String, ByVal nFormatKbn As Integer) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            '�Ƃ肠�������U�݂̂�z��
            SQL = "SELECT NVL(TORIS_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(TORIF_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & Syubetu & "'"
            ElseIf nFormatKbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)

                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ� START
    ' �@�\�@�@�@: ������R�[�h�A���R�[�h�AF�����敪��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h(��L�[)
    '           : ARG2 - ������R�[�h(�Q�Ɠn��)
    '           : ARG3 - ����敛�R�[�h(�Q�Ɠn��)
    '           : ARG4 - F�����敪(�Q�Ɠn��)
    '           : ARG7 - �U�֓�(�Q�Ɠn��)
    '           : ARG8 - �Ԋ҃t���O(�Q�Ɠn��)
    '
    ' ���l�@�@�@: 
    '
    Public Function aGetFToriCode(ByVal itakuCode As String, ByVal Syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String, ByVal nFormatKbn As Integer, _
        Optional ByVal furiDate As String = "") As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            '�Ƃ肠�������U�݂̂�z��
            SQL = "SELECT NVL(TORIS_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(TORIF_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & Syubetu & "'"
            ElseIf nFormatKbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST, SCHMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND   TORIS_CODE_T = TORIS_CODE_S"
                SQL &= " AND   TORIF_CODE_T = TORIF_CODE_S"
                SQL &= " AND   FURI_DATE_S = '" & furiDate & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)

                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ�  END

    '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
    ' �@�\�@�@�@: ������R�[�h�A���R�[�h�AF�����敪��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h(��L�[)
    '           : ARG2 - ������R�[�h(�Q�Ɠn��)
    '           : ARG3 - ����敛�R�[�h(�Q�Ɠn��)
    '           : ARG4 - F�����敪(�Q�Ɠn��)
    '           : ARG5 - �}�̃R�[�h         
    '
    ' ���l�@�@�@: 
    '
    Public Function GetFToriCode(ByVal itakuCode As String, ByVal syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String _
        , ByRef baitaiCode As String, ByRef itakukanriCode As String, ByRef multikbn As String, Optional ByVal nFormatkbn As Integer = 0) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(TORIS_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(TORIF_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            SQL &= ", NVL(BAITAI_CODE_T, '00')"
            SQL &= ", ITAKU_KANRI_CODE_T"
            SQL &= ", MULTI_KBN_T"
            If nFormatKbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & syubetu & "'"
            ElseIf nFormatkbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)
                baitaiCode = onReader.GetString(3)

                itakukanriCode = onReader.GetString(4)
                multikbn = onReader.GetString(5)

                Return True
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                '' �Ǎ����s�� 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function

    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ� START
    '***ASTAR.S.S 2008.05.23 �}�̋敪�ǉ�       ***
    ' �@�\�@�@�@: ������R�[�h�A���R�[�h�AF�����敪��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h(��L�[)
    '           : ARG2 - ������R�[�h(�Q�Ɠn��)
    '           : ARG3 - ����敛�R�[�h(�Q�Ɠn��)
    '           : ARG4 - F�����敪(�Q�Ɠn��)
    '           : ARG5 - �}�̃R�[�h         
    '
    ' ���l�@�@�@: 
    '
    Public Function bGetFToriCode(ByVal itakuCode As String, ByVal syubetu As String, _
        ByRef toris As String, ByRef torif As String, ByRef fkbn As String _
        , ByRef baitaiCode As String, ByRef itakukanriCode As String, ByRef multikbn As String, Optional ByVal nFormatkbn As Integer = 0, _
        Optional ByVal furiDate As String = "") As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(TORIS_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(TORIF_CODE_T, '�Y���Ȃ�')"
            SQL &= ", NVL(FSYORI_KBN_T, '0')"
            SQL &= ", NVL(BAITAI_CODE_T, '00')"
            SQL &= ", ITAKU_KANRI_CODE_T"
            SQL &= ", MULTI_KBN_T"
            If nFormatkbn = 7 Then
                SQL &= " FROM S_TORIMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND SYUBETU_T = '" & syubetu & "'"
            ElseIf nFormatkbn = 3 Then
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE TORIS_CODE_T = '" & Mid(itakuCode, 1, 10) & "'"
                SQL &= " AND   TORIF_CODE_T = '" & Mid(itakuCode, 11, 2) & "'"
            Else
                SQL &= " FROM TORIMAST, SCHMAST"
                SQL &= " WHERE ITAKU_CODE_T = '" & itakuCode & "'"
                SQL &= " AND   TORIS_CODE_T = TORIS_CODE_S"
                SQL &= " AND   TORIF_CODE_T = TORIF_CODE_S"
                SQL &= " AND   FURI_DATE_S = '" & furiDate & "'"
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                toris = onReader.GetString(0)
                torif = onReader.GetString(1)
                fkbn = onReader.GetString(2)
                baitaiCode = onReader.GetString(3)

                itakukanriCode = onReader.GetString(4)
                multikbn = onReader.GetString(5)

                Return True
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                '' �Ǎ����s�� 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�����敪�E������E���R�[�h�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Return False
        End Try
    End Function
    '2010.03.18 �ߗ��M���J�X�^�}�C�Y �U�֓��A�Ԋ҃t���O�ǉ�  END

    ' �@�\�@�@�@: �ϑ��҃R�[�h��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h(�Q�Ɠn��)
    '           : ARG2 - ������R�[�h
    '           : ARG3 - ����敛�R�[�h
    '
    ' ���l�@�@�@: 
    '
    Public Function GetItakuCode(ByRef itakuCode As String, _
        ByVal toris As String, ByVal torif As String) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_CODE, '�Y���Ȃ�')"
            SQL &= " FROM TORI_VIEW"
            '*** �C�� mitsu 2008/09/01 �����C�� ***
            'SQL &= " WHERE TORI_S_CODE = '" & toris & "'"
            'SQL &= " AND TORI_F_CODE = " & torif & "'"
            SQL &= " WHERE TORIS_CODE = '" & toris & "'"
            SQL &= " AND TORIF_CODE = " & torif & "'"
            '**************************************

            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                itakuCode = onReader.GetString(0)
                Return True
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                '' �Ǎ����s�� 
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�ϑ��҃R�[�h�擾���s ������R�[�h�F" & toris & " ����敛�R�[�h�F" & torif
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ϑ��҃R�[�h�擾���s ������R�[�h�F" & toris & " ����敛�R�[�h�F" & torif & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function


    ' �@�\�@�@�@: �t�H�[�}�b�g�敪��Ԃ�
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '           : ARG2 - �t�H�[�}�b�g�敪(CMT�ϊ��d�l)
    '
    ' ���l�@�@�@: 
    '
    Public Function GetFormatKbn(ByVal itakuCode As String, _
        ByRef nFormatKbn As Integer) As Boolean

        Dim fkbn As String      ' �t�H�[�}�b�g�敪������
        Dim mkbn As String      ' �����敪������

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(FMT_KBN, '00'), NVL(MOTIKOMI_KBN,'0')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & itakuCode & "'"
            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                fkbn = onReader.GetString(0)
                mkbn = onReader.GetString(1)
            Else
                ' �Ǎ����s�� 
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�t�H�[�}�b�g�敪�擾���s �ϑ��҃R�[�h�F" & itakuCode
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�t�H�[�}�b�g�敪�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (mkbn = "1") Then
            nFormatKbn = 4          ' TODO �Z���^�[�����̐�����/���ʕ��̐U���ɑΉ�
            Return True
        End If
        Select Case fkbn
            Case "00"               ' �S��
                nFormatKbn = 0
            Case "01"               ' �n�������c��1 (350)
                nFormatKbn = 1
            Case "06"               ' �n�������c��2 (300) ����s
                nFormatKbn = 2
            Case "02"               ' ����
                nFormatKbn = 3
            Case "20"               ' SSS�͑S�∵��
                nFormatKbn = 0
        End Select
        Return True
    End Function

    ' �@�\�@�@�@: ���x���敪�̎擾
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '           : ARG2 - �t�H�[�}�b�g�敪(CMT�ϊ��d�l)
    '
    ' ���l�@�@�@: 
    '
    Public Function GetLabelKbn(ByVal itakuCode As String, _
        ByRef nLabelKbn As Integer) As Boolean

        Dim SQL As String = ""
        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(LABEL_KBN,0)"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & itakuCode & "'"
            If GCom.SetDynaset(SQL, onReader) Then
                ' �Ǎ�������
                onReader.Read()
                nLabelKbn = onReader.GetInt32(0)
            Else
                ' �Ǎ����s�� 
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "���x���敪�擾���s �ϑ��҃R�[�h�F" & itakuCode
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���x���敪�擾���s �ϑ��҃R�[�h�F" & itakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function


    ' �@�\�@�@�@: ����抿�����̎擾
    '
    ' �߂�l�@�@: ����抿����
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '
    ' ���l�@�@�@: 
    '
    Public Function GetItakuKanji(ByVal strItakuCode As String) As String
        Dim SQL As String = ""

        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_NNAME, ' -- ')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & strItakuCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' �s������Ƃ�
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                Return name
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�ϑ��Җ�(����)�擾���s �ϑ��҃R�[�h�F" & strItakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "�ϑ��Җ��擾"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ϑ��Җ�(����)�擾���s �ϑ��҃R�[�h�F" & strItakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "�ϑ��Җ������G���["

    End Function

    '*** �C�� mitsu 2008/07/17 �U�փR�[�h�E��ƃR�[�h��n���ꂽ�ꍇ ***
    Public Function GetItakuKanji(ByRef strItakuCode As String _
        , ByVal strFuriCode As String, ByVal strKigyoCode As String) As String

        Dim SQL As String = ""

        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_NNAME_T, ' -- '), ITAKU_CODE_T"
            SQL &= " FROM TORIMAST"
            SQL &= " WHERE KIGYO_CODE_T = '" & strKigyoCode & "'"
            SQL &= " AND FURI_CODE_T = '" & strFuriCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' �s������Ƃ�
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                strItakuCode = onReader.GetString(1)
                Return name
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�ϑ��Җ�(����)�擾���s �U�փR�[�h�F" & strFuriCode _
                '                 & " ��ƃR�[�h�F" & strKigyoCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ϑ��Җ�(����)�擾���s �U�փR�[�h�F" & strFuriCode _
                             & " ��ƃR�[�h�F" & strKigyoCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "�ϑ��Җ������G���["

    End Function
    '******************************************************************

    ' �@�\�@�@�@: �����J�i���̎擾
    '
    ' �߂�l�@�@: ����抿����
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '
    ' ���l�@�@�@: 
    '
    Public Function GetItakuKana(ByVal strItakuCode As String) As String
        Dim SQL As String = ""

        Try
            '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
            Dim onReader As OracleDataReader = Nothing
            'Dim onReader As OracleDataReader
            '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

            SQL = "SELECT NVL(ITAKU_KNAME, ' -- ')"
            SQL &= " FROM TORI_VIEW"
            SQL &= " WHERE ITAKU_CODE = '" & strItakuCode & "'"

            If GCom.SetDynaset(SQL, onReader) Then
                ' �s������Ƃ�
                onReader.Read()
                Dim name As String = onReader.GetString(0)
                Return name
            Else
                '*** �C�� mitsu 2008/09/08 �s�v ***
                'With GCom.GLog
                '    .Result = MenteCommon.clsCommon.NG
                '    .Discription = "�ϑ��Җ�(�J�i)�擾���s �ϑ��҃R�[�h�F" & strItakuCode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return " -- "
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�ϑ��Җ�(�J�i)�擾���s �ϑ��҃R�[�h�F" & strItakuCode & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return "�ϑ��Җ������G���["

    End Function

    ' �@�\�@ �F ���Z�@�֏��擾
    '
    ' ����   �F ARG1 - ���Z�@�փR�[�h
    '           ARG2 - �x�X�R�[�h
    '           ARG3 - ���Z�@�֖�(�Q�Ɠn��)
    '           ARG4 - �x�X��(�Q�Ɠn��)
    '
    ' �߂�l �F ���� true / ���s false
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function GetBankAndBranchName(ByVal BKCode As String, ByVal BRCode As String, _
        ByRef bankname As String, ByRef branchname As String) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Try
            If BKCode.Length = 0 Then
                Return False
            End If

            Dim SQL As String = "SELECT KIN_NNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode & "'"
            SQL &= " AND SIT_NO_N = '" & BRCode & "'"
            '*** �C�� mitsu 2008/09/01 ���������� ***
            SQL &= " AND ROWNUM = 1"
            '****************************************

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read() Then

                '���Z�@�֖�
                bankname = REC.GetString(0)
                '�x�X��
                branchname = REC.GetString(1)

                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "���Z�@�֖��E�x�X���擾���s ���Z�@�փR�[�h�F" & BKCode & " �x�X�R�[�h�F" & BRCode
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���Z�@�֖��E�x�X���擾���s ���Z�@�փR�[�h�F" & BKCode & " �x�X�R�[�h�F" & BRCode & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
        Return True
    End Function


    ' �@�\�@ �F ���Z�@�֏��擾
    '
    ' ����   �F ARG1 - ���Z�@�փR�[�h
    '           ARG2 - ���Z�@�֖�(�Q�Ɠn��)
    '
    ' �߂�l �F ���� true / ���s false
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function GetBankName(ByVal BKCode As String, ByRef bankname As String) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        Try
            If BKCode.Length = 0 Then
                Return False
            End If

            Dim SQL As String = "SELECT KIN_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode & "'"
            '*** �C�� mitsu 2008/09/01 ���������� ***
            SQL &= " AND ROWNUM = 1"
            '****************************************

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read() Then

                '���Z�@�֖�
                bankname = REC.GetString(0)
                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "���Z�@�֖��擾���s ���Z�@�փR�[�h�F" & BKCode
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "���Z�@�֖��擾���s ���Z�@�փR�[�h�F" & BKCode & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
        Return True
    End Function

    ' �@�\�@�@�@: �t�@�C�������݉񐔂̎擾
    '
    ' �߂�l�@�@: ���� true / ���s false
    '
    ' �������@�@: ARG1 - �ϑ��҃R�[�h
    '             ARG2 - �U�֓�
    '             ARG3 - �t�@�C��������(�������̃��R�[�h�̂����̍ő�l)
    ' ���l�@�@�@: 
    Public Function GetWriteCounter(ByVal itakucd As String, ByVal furidate As String, ByRef Counter As Integer) As Boolean
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Try
            Dim SQL As String = "SELECT NVL(MAX(WRITE_COUNTER), 0) FROM CMT_WRITE_TBL"
            SQL &= " WHERE FILE_SEQ = 1"
            SQL &= " AND ITAKU_CODE = '" & itakucd & "'"
            SQL &= " AND FURI_DATE = '" & furidate & "'"
            SQL &= " AND ERR_CD = 0"
            If GCom.SetDynaset(SQL, onReader) AndAlso onReader.Read Then
                Counter = onReader.GetInt32(0)
                Return True
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "�t�@�C�������񐔎擾���s �ϑ��҃R�[�h�F" & itakucd & " �U�֓��F" & furidate
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Counter = 0
                Return False
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "�t�@�C�������񐔎擾���s �ϑ��҃R�[�h�F" & itakucd & " �U�֓��F" & furidate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

            Counter = 0
            Return False

        End Try

    End Function
End Class
