Option Explicit On 
Option Strict On

Imports System
Imports System.Data.OracleClient
Imports System.Text
Imports CASTCommon
Imports CAstExternal
'--------------------------------------------------------------------------------------------------
'' �X�P�W���[���}�X�^�쐬�֘A�֐� 2007.12.12 By K.Seto
'--------------------------------------------------------------------------------------------------

Public Class ClsSchduleMaintenanceClass

    '�X�P�W���[���}�X�^���e   
    Public MaxColumn As Integer = 0
    Public ORANAME() As String         'ORACLE Column Name
    Public ORATYPE() As String         'ORACLE Column TYPE
    Public ORASIZE() As Integer        'ORACLE Column SIZE
    Public SCHMAST() As String         'SCHMAST Current Column Value

    '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
    '�X�P�W���[���}�X�^�T�u���e
    Public MaxColumn_Sub As Integer = 0
    Public ORANAME_SUB() As String
    Public ORATYPE_SUB() As String
    Public ORASIZE_SUB() As Integer
    Public SCHMAST_SUB() As String
    '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END

    '�ꎟ���o�����}�X�^�z��
    Public Structure TORIMAST_RECORD
        Dim FSYORI_KBN As String                '�U�֏����敪
        Dim TORIS_CODE As String                '������R�[�h
        Dim TORIF_CODE As String                '����敛�R�[�h
        Dim BAITAI_CODE As String               '�}�̃R�[�h
        Dim ITAKU_KANRI_CODE As String          '��\�ϑ��҃R�[�h
        Dim FMT_KBN As Integer                  '�t�H�[�}�b�g�敪
        Dim SYUBETU As String                   '��ʃR�[�h
        Dim ITAKU_CODE As String                '�ϑ��҃R�[�h
        Dim ITAKU_KNAME As String               '�ϑ��҃J�i��
        Dim TKIN_NO As String                   '�戵���Z�@��
        Dim TSIT_NO As String                   '�戵�X��
        Dim MOTIKOMI_KBN As String              '�����敪
        Dim SOUSIN_KBN As String                '���M�敪
        Dim TAKO_KBN As Integer                 '���s�敪
        Dim FURI_CODE As String                 '�U�փR�[�h
        Dim KIGYO_CODE As String                '��ƃR�[�h
        Dim ITAKU_NNAME As String               '�ϑ��Ҋ�����
        Dim FURI_KYU_CODE As Integer            '�U�֋x���R�[�h
        Dim DATEN() As Integer                  'N���̗L���^����
        Dim MONTH_FLG As Integer                '�Y�����̒l�H
        Dim SFURI_FLG As Integer                '�ĐU�_��
        Dim SFURI_FCODE As String               '�ĐU���R�[�h
        Dim SFURI_DAY As Integer                '�����^����i�ĐU�j
        Dim SFURI_KIJITSU As Integer             '���t�敪�i�ĐU�j
        Dim SFURI_KYU_CODE As Integer            '�ĐU�x���V�t�g
        Dim KEIYAKU_DATE As String              '�_���
        Dim MOTIKOMI_KIJITSU As Integer         '��������
        Dim IRAISHO_YDATE As Integer            '�����^����i�˗����j
        Dim IRAISHO_KIJITSU As String           '���t�敪�i�˗����j
        Dim IRAISHO_KYU_CODE As String          '�˗����x���V�t�g
        Dim KESSAI_KBN As Integer               '���ϋ敪
        Dim KESSAI_DAY As Integer               '�����^����i���ρj
        Dim KESSAI_KIJITSU As Integer            '���t�敪�i���ρj
        Dim KESSAI_KYU_CODE As Integer           '���ϓ��x���V�t�g
        Dim TESUUTYO_KBN As Integer             '�萔�������敪
        Dim TESUUTYO_PATN As Integer            '�萔���������@(2008.03.06)
        Dim TESUUMAT_NO As Integer              '�萔���W�v����
        Dim TESUUTYO_DAY As Integer             '�萔�����������^���
        Dim TESUUTYO_KIJITSU As Integer         '�萔�����������敪
        Dim TESUU_KYU_CODE As Integer           '�萔���������x���R�[�h
        Dim TESUUMAT_MONTH As Integer           '�W�v���(2008.03.06)
        Dim TESUUMAT_ENDDAY As Integer          '�W�v�I����(2008.03.06)
        Dim TESUUMAT_KIJYUN As Integer          '�W�v�(2008.03.06)

    End Structure
    Public TR() As TORIMAST_RECORD

    '�o�^�^���茋�ʏ��
    Public Structure SCHMAST_Data
        Dim YUUKOU_FLG As Integer           '�L���t���O����
        Dim KFURI_DATE As String            '�_��U�֓�
        Dim FURI_DATE As String             '�U�֓�
        Dim NFURI_DATE As String            '�ύX�U�֓�
        Dim MOTIKOMI_SEQ As Integer         '����SEQ
        Dim IRAISYOK_YDATE As String        '�˗�������\���
        Dim HAISIN_YDATE As String          '�z�M�����\������s��
        Dim HAISIN_T1YDATE As String        '�z�M�����\������s��g����
        Dim HAISIN_T2YDATE As String        '�z�M�����\������s��g�O��
        Dim FUNOU_YDATE As String           '�s�\�����\������s��
        Dim FUNOU_T1YDATE As String         '�s�\�����\������s��g����
        Dim FUNOU_T2YDATE As String         '�s�\�����\������s��g�O��
        Dim HENKAN_YDATE As String          '�Ԋҗ\���
        Dim KESSAI_YDATE As String          '���ϗ\���
        Dim TESUU_YDATE As String           '�萔�������\���
        Dim KSAIFURI_DATE As String         '�ĐU�\���
        Dim MOTIKOMI_DATE As String         '��������

        'FJH����̎d�l�҂�
        Dim KAKUHO_YDATE_S As String        '�����m�ۗ\���
        Dim HASSIN_YDATE_S As String        '���M�\���

        Dim WRK_SFURI_YDATE As String            '�ĐU��2009.10.05
    End Structure
    Public SCH As SCHMAST_Data

    Public Enum OPT
        OptionNothing = 0               '�ʓo�^���
        OptionAddNew = 1                '�V�K�E�č쐬
        OptionAppend = 2                '�ǉ��쐬
    End Enum

    Private Const ThisModuleName As String = "ClsSchduleMaintenance.vb"

    '�ǂ̋Ɩ�����Ăяo����邩�̎��ʕϐ�
    Private SchTable As Integer = 0

    Public Enum APL
        JifuriApplication = 1
        SofuriApplication = 3
    End Enum

    Public WriteOnly Property SetSchTable() As Integer
        Set(ByVal Value As Integer)
            SchTable = Value
        End Set
    End Property

    '
    ' �@�\�@ �F SCHMAST���ږ��̒~��
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F 2007.12.12 ���ʉ�
    '
    Public Sub SetSchMastInformation()
        'With GCom.GLog
        '    .Job2 = "SCHMAST���ږ��~��"
        'End With
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = "SELECT COLUMN_NAME"
            SQL &= ", DATA_TYPE"
            SQL &= ", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE"
            SQL &= " FROM ALL_TAB_COLUMNS"
            SQL &= " WHERE UPPER(OWNER) = 'KZFMAST'"        '���� 2009.09.15 �Ƃ肠�������̂܂�
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " AND TABLE_NAME = 'SCHMAST'"
                Case Is = APL.SofuriApplication
                    SQL &= " AND TABLE_NAME = 'S_SCHMAST'"
            End Select
            SQL &= " ORDER BY COLUMN_ID ASC"
            Dim Ret As Boolean = GCom.SetDynaset(SQL, REC)
            If Ret Then
                Do While REC.Read
                    Select Case MaxColumn
                        Case 0
                            ReDim ORANAME(0)
                            ReDim ORATYPE(0)
                            ReDim ORASIZE(0)
                        Case Else
                            ReDim Preserve ORANAME(MaxColumn)
                            ReDim Preserve ORATYPE(MaxColumn)
                            ReDim Preserve ORASIZE(MaxColumn)
                    End Select
                    ORANAME(MaxColumn) = GCom.NzStr(REC.Item("COLUMN_NAME")).Trim.ToUpper
                    ORATYPE(MaxColumn) = GCom.NzStr(REC.Item("DATA_TYPE")).Trim.ToUpper
                    ORASIZE(MaxColumn) = GCom.NzInt(REC.Item("DATA_SIZE"), 0)
                    MaxColumn += 1
                Loop
                MaxColumn -= 1
                ReDim SCHMAST(MaxColumn)
            End If
            '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                SQL = "SELECT COLUMN_NAME"
                SQL &= ", DATA_TYPE"
                SQL &= ", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE"
                SQL &= " FROM ALL_TAB_COLUMNS"
                SQL &= " WHERE UPPER(OWNER) = 'KZFMAST'"
                Select Case SchTable
                    Case Is = APL.JifuriApplication
                        SQL &= " AND TABLE_NAME = 'SCHMAST_SUB'"
                    Case Is = APL.SofuriApplication
                        SQL &= " AND TABLE_NAME = 'S_SCHMAST_SUB'"
                End Select
                SQL &= " ORDER BY COLUMN_ID ASC"
                Ret = GCom.SetDynaset(SQL, REC)
                If Ret Then
                    Do While REC.Read
                        Select Case MaxColumn_Sub
                            Case 0
                                ReDim ORANAME_SUB(0)
                                ReDim ORATYPE_SUB(0)
                                ReDim ORASIZE_SUB(0)
                            Case Else
                                ReDim Preserve ORANAME_SUB(MaxColumn_Sub)
                                ReDim Preserve ORATYPE_SUB(MaxColumn_Sub)
                                ReDim Preserve ORASIZE_SUB(MaxColumn_Sub)
                        End Select
                        ORANAME_SUB(MaxColumn_Sub) = GCom.NzStr(REC.Item("COLUMN_NAME")).Trim.ToUpper
                        ORATYPE_SUB(MaxColumn_Sub) = GCom.NzStr(REC.Item("DATA_TYPE")).Trim.ToUpper
                        ORASIZE_SUB(MaxColumn_Sub) = GCom.NzInt(REC.Item("DATA_SIZE"), 0)
                        MaxColumn_Sub += 1
                    Loop
                    MaxColumn_Sub -= 1
                    ReDim SCHMAST_SUB(MaxColumn_Sub)
                End If
            End If
            '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END

        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Sub

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^���ڒl�z��Q��
    '
    ' �����@ �F ARG1 - ���ږ�=�R���g���[����
    ' �@�@�@ �@ ARG2 - �ԓ��]������
    '
    ' �߂�l �F �P��������l
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function GetColumnValue(ByVal ColumnName As String, ByVal ReturnType As String) As String
        Dim ReturnValue As String = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzStr(SCHMAST(Index)).Trim
                Exit For
            End If
        Next Index
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzStr(SCHMAST_SUB(Index)).Trim
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^���ڒl�z��Q��
    '
    ' �����@ �F ARG1 - ���ږ�=�R���g���[����
    '
    ' �߂�l �F ������^���l���
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function GetColumnValue(ByVal ColumnName As String) As String
        Dim ReturnValue As String = ""
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzDec(SCHMAST(Index), "")
                Exit For
            End If
        Next Index
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzDec(SCHMAST_SUB(Index), "")
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^���ڒl�z��Q��
    '
    ' �����@ �F ARG1 - ���ږ�=�R���g���[����
    ' �@�@�@ �@ ARG2 - �ԓ��]������
    '
    ' �߂�l �F ���l�^���ڒl
    '
    ' ���l�@ �F �����l(Integer)
    '
    Public Function GetColumnValue(ByVal ColumnName As String, ByVal ReturnType As Integer) As Integer
        Dim ReturnValue As Integer = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzInt(SCHMAST(Index), 0)
                Exit For
            End If
        Next Index
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzInt(SCHMAST_SUB(Index), 0)
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^���ڒl�z��Q��
    '
    ' �����@ �F ARG1 - ���ږ�=�R���g���[����
    ' �@�@�@ �@ ARG2 - �ԓ��]������
    ' �@�@�@ �@ ARG3 - �����̑傫�����ڎ���
    '
    ' �߂�l �F ���l�^���ڒl
    '
    ' ���l�@ �F �\�i���l(Decimal)
    '
    Public Function GetColumnValue(ByVal ColumnName As String, _
                ByVal ReturnType As Integer, ByVal DecimalType As Integer) As Decimal
        Dim ReturnValue As Decimal = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzDec(SCHMAST(Index), 0)
                Exit For
            End If
        Next Index
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzDec(SCHMAST_SUB(Index), 0)
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �x�����̒~��
    '
    ' �����@ �F ARG1 - �U�֓��f�[�^(��ʎw��)
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F ����O�ɒ~�ς��邱��
    '
    Public Sub SetKyuzituInformation(Optional ByVal FormFuriDate As Date = MenteCommon.clsCommon.BadResultDate)
        'With GCom.GLog
        '    .Job2 = "�x�����~�Ϗ���"
        'End With
        Try
            Select Case FormFuriDate
                Case MenteCommon.clsCommon.BadResultDate
                    '�S�x������~�ς���B
                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1)
                Case Else
                    '�Y�����O��̋x�����̂�����~�ς���B
                    Dim ProviousMonthDate As Date = FormFuriDate.AddMonths(-1)
                    Dim NextMonthDate As Date = FormFuriDate.AddMonths(1)

                    Dim SQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)"
                    SQL &= " IN ('" & String.Format("{0:yyyyMM}", ProviousMonthDate) & "'"
                    SQL &= ", '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'"
                    SQL &= ", '" & String.Format("{0:yyyyMM}", NextMonthDate) & "')"
                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1, SQL)
            End Select
        Catch ex As Exception

            Throw

        End Try
    End Sub

    '
    ' �@�\�@ �F �X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
    '
    ' �����@ �F ARG1 - �U�֓��N��
    ' �@�@�@ �@ ARG2 - ������R�[�h
    ' �@�@�@ �@ ARG3 - ����敛�R�[�h
    ' �@�@�@ �@ ARG4 - �ďo�����ʎw��
    '
    ' �߂�l �F OK = True, NG = False(�Ώێ����Ȃ�)
    '
    ' ���l�@ �F �Y���̌����Ώۂ̂��̂ŐU�֓����J�n�^�I���̊Ԃɂ������
    '
    Public Function GET_SELECT_TORIMAST(ByVal FormFuriDate As Date, _
            ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, _
            Optional ByVal SEL_OPTION As Integer = OPT.OptionNothing) As Boolean

        With GCom.GLog
            .Job2 = "�X�P�W���[���쐬�Ώێ����R�[�h���o"
        End With
        Dim REC As OracleDataReader = Nothing
        Try
            '�Y����
            Dim MonthColumn As String = "TUKI" & FormFuriDate.Month.ToString & "_T"

            Dim SQL As String = SetToriMastSelectBaseSql(MonthColumn)
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " WHERE FSYORI_KBN_T = '1'"
                Case Is = APL.SofuriApplication
                    SQL &= " WHERE FSYORI_KBN_T = '3'"
            End Select

            Select Case SEL_OPTION
                Case OPT.OptionNothing
                    '�ʓo�^���
                    SQL &= " AND TORIS_CODE_T = '" & TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_T = '" & TORIF_CODE & "'"
                Case OPT.OptionAddNew, OPT.OptionAppend
                    '���ԃX�P�W���[���쐬
                    SQL &= " AND NOT " & MonthColumn & " = '0'"
                    SQL &= " AND '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'"
                    SQL &= " BETWEEN SUBSTR(KAISI_DATE_T, 1, 6)"
                    SQL &= " AND SUBSTR(SYURYOU_DATE_T, 1, 6)"
                    SQL &= " AND NOT TO_NUMBER(NVL(BAITAI_CODE_T, '0')) = 7"

                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                        Case Is = APL.SofuriApplication
                            SQL &= " AND KIJITU_KANRI_T = '1'"
                    End Select

                    '�ǉ��̏ꍇ�ɂ͎����}�X�^�̕ύX������������
                    If SEL_OPTION = OPT.OptionAppend Then

                        SQL &= " AND NOT KOUSIN_SIKIBETU_T = '2'"
                    End If

            End Select

            If GCom.SetDynaset(SQL, REC) Then

                Dim Counter As Integer = 0

                Select Case SEL_OPTION
                    Case OPT.OptionNothing
                        '�ʓo�^���
                        If REC.Read Then

                            ReDim TR(0)

                            '�z��ϐ��Ɏ�芸�����Ώێ�����~��
                            Call GET_SELECT_TORIMAST_Sub(REC, Counter)

                            Return True
                        End If
                    Case Else
                        '���ԃX�P�W���[���쐬
                        Do While REC.Read

                            Select Case Counter
                                Case Is = 0
                                    ReDim TR(0)
                                Case Else
                                    ReDim Preserve TR(Counter)
                            End Select

                            '�z��ϐ��Ɏ�芸�����Ώێ�����~��
                            Call GET_SELECT_TORIMAST_Sub(REC, Counter)

                            Counter += 1
                        Loop
                        Return (Counter > 0)
                End Select
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function

    ' �@�\�@ �F �������ĐU�̎����}�X�^���`�F�b�N
    '
    ' �����@ �F 
    ' �@�@�@ �@ ARG1 - ������R�[�h
    ' �@�@�@ �@ ARG2 - ����敛�R�[�h
    '
    ' �߂�l �F OK = True, NG = False(�Ώێ����Ȃ�)
    '
    ' ���l�@ �F 
    '
    Public Function CHECK_SAIFURI_SELF(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String) As Boolean

        Dim REC As OracleDataReader = Nothing
        Dim sql As New StringBuilder(128)
        Try

            sql.Append("SELECT COUNT(*) AS COUNTER FROM ")
            sql.Append(" TORIMAST")
            sql.Append(" WHERE FSYORI_KBN_T = '1'")
            sql.Append(" AND TORIS_CODE_T = '" & TORIS_CODE & "'")
            sql.Append(" AND SFURI_FCODE_T = '" & TORIF_CODE & "'")
            sql.Append(" AND SFURI_FLG_T = '1'")

            If GCom.SetDynaset(sql.ToString, REC) Then
                If REC.Read = True AndAlso GCom.NzInt(REC.Item("COUNTER"), -1) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False

            End If

        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function


    '
    ' �@�\�@ �F �����}�X�^����SQL���̊�{�����Z�b�g����B
    '
    ' �����@ �F ARG1 - �ݒ茎�̍��ږ�
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F ���ʉ�
    '
    Private Function SetToriMastSelectBaseSql(ByVal MonthColumn As String) As String
        With GCom.GLog
            .Job2 = "�����}�X�^����SQL�쐬"
        End With
        Try
            Dim SQL As String = "SELECT FSYORI_KBN_T"   '�U�֏����敪
            SQL &= ", TORIS_CODE_T"                     '������R�[�h
            SQL &= ", TORIF_CODE_T"                     '����敛�R�[�h
            SQL &= ", " & MonthColumn & " MONTH"        '�ݒ茎
            For Index As Integer = 1 To 31 Step 1
                SQL &= ", DATE" & Index.ToString & "_T"
            Next Index
            SQL &= ", SOUSIN_KBN_T"                     '���M�敪
            SQL &= ", FURI_KYU_CODE_T"                       '�x���R�[�h
            SQL &= ", FMT_KBN_T"                        '�t�H�[�}�b�g�敪
            SQL &= ", TESUUTYO_KIJITSU_T"               '�萔�����������敪
            SQL &= ", TESUUTYO_KBN_T"                   '�萔�������敪
            SQL &= ", TESUUTYO_DAY_T"                    '�萔�����������^���
            SQL &= ", TESUU_KYU_CODE_T"                 '�萔���������x���R�[�h
            SQL &= ", KESSAI_KIJITSU_T"                 '���ϓ��w��敪
            SQL &= ", KESSAI_DAY_T"                      '���ϓ������^���
            SQL &= ", KESSAI_KYU_CODE_T"                '���ϋx���R�[�h
            SQL &= ", FURI_CODE_T"                      '�U�փR�[�h
            SQL &= ", ITAKU_CODE_T"                     '�ϑ��҃R�[�h
            SQL &= ", TKIN_NO_T"                        '�戵���Z�@��
            SQL &= ", TSIT_NO_T"                        '�戵�X��
            SQL &= ", BAITAI_CODE_T"                    '�}�̃R�[�h
            SQL &= ", SYUBETU_T"                   '��ʃR�[�h
            SQL &= ", ITAKU_KNAME_T"                    '�ϑ��҃J�i��
            SQL &= ", ITAKU_NNAME_T"                    '�ϑ��Ҋ�����
            SQL &= ", MOTIKOMI_KIJITSU_T"               '��������
            SQL &= ", TESUUMAT_NO_T"                    '�萔���W�v����(2008.03.06)
            SQL &= ", TESUUMAT_MONTH_T"                '�W�v���(2008.03.06)
            SQL &= ", TESUUMAT_ENDDAY_T"                   '�W�v�I����(2008.03.06)
            SQL &= ", TESUUMAT_KIJYUN_T"               '�W�v�(2008.03.06)
            SQL &= ", TESUUTYO_PATN_T"                  '�萔���������@(2008.03.06)
            SQL &= ", KESSAI_KBN_T"                     '���ϋ敪(2008.03.06)

            Select Case SchTable
                Case Is = APL.JifuriApplication
                    '���U�ŗL�̍���
                    SQL &= ", TAKO_KBN_T"                       '���s�敪
                    SQL &= ", SFURI_FLG_T"                      '�ĐU�t���O
                    SQL &= ", SFURI_KIJITSU_T"                      '�ĐU�敪
                    SQL &= ", SFURI_DAY_T"                     '�ĐU��
                    SQL &= ", SFURI_FCODE_T"                     '�ĐU��
                    SQL &= ", SFURI_KYU_CODE_T"                 '�ĐU�x���R�[�h
                    SQL &= ", KIGYO_CODE_T"                     '��ƃR�[�h
                    SQL &= ", MOTIKOMI_KBN_T"                   '�����敪
                    'SQL &= ", TOHO_CNT_CODE_T"                  '�`�������Z���^�[�m�F�R�[�h

                    SQL &= " FROM TORIMAST"

                Case Is = APL.SofuriApplication
                    '�����U�ɍ��ڂ��Ȃ�

                    SQL &= " FROM K_TORIMAST"
            End Select

            Return SQL
        Catch ex As Exception

            Throw

        End Try
        Return ""
    End Function

    '
    ' �@�\�@ �F �����}�X�^�������ʂ�z��փZ�b�g����B
    '
    ' �����@ �F ARG1 - ORACLE Data Reader Object
    ' �@�@�@ �@ ARG2 - �J�E���^�[�ϐ�
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F ���ʉ�(�z��ϐ��Ɏ�芸�����Ώێ�����~��)
    '
    Public Sub GET_SELECT_TORIMAST_Sub(ByVal REC As OracleDataReader, ByRef Counter As Integer)
        With GCom.GLog
            .Job2 = "�����}�X�^�������ʊi�["
        End With
        Try
            Dim Temp As String = ""

            With TR(Counter)
                .FSYORI_KBN = GCom.NzDec(REC.Item("FSYORI_KBN_T"), "")              '�U�֏����敪
                .TORIS_CODE = GCom.NzDec(REC.Item("TORIS_CODE_T"), "")              '������R�[�h
                .TORIF_CODE = GCom.NzDec(REC.Item("TORIF_CODE_T"), "")              '����敛�R�[�h
                .ITAKU_CODE = GCom.NzDec(REC.Item("ITAKU_CODE_T"), "")              '�ϑ��҃R�[�h
                .ITAKU_KNAME = GCom.NzStr(REC.Item("ITAKU_KNAME_T"))                '�ϑ��҃J�i��
                .ITAKU_NNAME = GCom.NzStr(REC.Item("ITAKU_NNAME_T"))                '�ϑ��Ҋ�����
                .MONTH_FLG = GCom.NzInt(REC.Item("MONTH"), 0)                       '�Y���w�茎�̒l

                '�U�֊Y�����̗L���^����
                ReDim TR(Counter).DATEN(31)
                For Index As Integer = 1 To 31 Step 1
                    Temp = "DATE" & Index.ToString & "_T"
                    TR(Counter).DATEN(Index) = GCom.NzInt(REC.Item(Temp), 0)
                Next Index

                .SOUSIN_KBN = GCom.NzDec(REC.Item("SOUSIN_KBN_T"), "")              '���M�敪
                .FURI_KYU_CODE = GCom.NzInt(REC.Item("FURI_KYU_CODE_T"), 0)         '�x���R�[�h
                .FMT_KBN = GCom.NzInt(REC.Item("FMT_KBN_T"), 0)                     '�t�H�[�}�b�g�敪
                .FURI_CODE = GCom.NzDec(REC.Item("FURI_CODE_T"), "")                '�U�փR�[�h
                .TKIN_NO = GCom.NzDec(REC.Item("TKIN_NO_T"), "")                    '�戵���Z�@��
                .TSIT_NO = GCom.NzDec(REC.Item("TSIT_NO_T"), "")                    '�戵�X��
                .BAITAI_CODE = GCom.NzDec(REC.Item("BAITAI_CODE_T"), "")            '�}�̃R�[�h
                .SYUBETU = GCom.NzDec(REC.Item("SYUBETU_T"), "")                    '��ʃR�[�h
                .TESUUTYO_KIJITSU = GCom.NzInt(REC.Item("TESUUTYO_KIJITSU_T"), 0)   '�萔�����������敪
                .TESUUTYO_KBN = GCom.NzInt(REC.Item("TESUUTYO_KBN_T"), 0)           '�萔�������敪
                .TESUUTYO_DAY = GCom.NzInt(REC.Item("TESUUTYO_DAY_T"), 0)           '�萔�����������^���
                .TESUU_KYU_CODE = GCom.NzInt(REC.Item("TESUU_KYU_CODE_T"), 0)       '�萔���������x���R�[�h
                .KESSAI_KIJITSU = GCom.NzInt(REC.Item("KESSAI_KIJITSU_T"), 0)       '���ϓ��w��敪
                .KESSAI_DAY = GCom.NzInt(REC.Item("KESSAI_DAY_T"), 0)               '���ϓ������^���
                .KESSAI_KYU_CODE = GCom.NzInt(REC.Item("KESSAI_KYU_CODE_T"), 0)     '���ϋx���R�[�h
                .MOTIKOMI_KIJITSU = GCom.NzInt(REC.Item("MOTIKOMI_KIJITSU_T"), 0)   '��������
                .TESUUMAT_NO = GCom.NzInt(REC.Item("TESUUMAT_NO_T"), 0)             '�萔���W�v����(2008.03.06)
                If .TESUUMAT_NO > 12 Then
                    .TESUUMAT_NO = 12
                End If
                .TESUUMAT_MONTH = GCom.NzInt(REC.Item("TESUUMAT_MONTH_T"), 0)     '�W�v���(2008.03.06)
                .TESUUMAT_ENDDAY = GCom.NzInt(REC.Item("TESUUMAT_ENDDAY_T"), 0)           '�W�v�I����(2008.03.06)
                .TESUUMAT_KIJYUN = GCom.NzInt(REC.Item("TESUUMAT_KIJYUN_T"), 0)   '�W�v�(2008.03.06)
                .TESUUTYO_PATN = GCom.NzInt(REC.Item("TESUUTYO_PATN_T"), 0)         '�萔���������@(2008.03.06)
                .KESSAI_KBN = GCom.NzInt(REC.Item("KESSAI_KBN_T"), 0)               '���ϋ敪(2008.03.06)

                Select Case SchTable
                    Case Is = APL.JifuriApplication
                        .TAKO_KBN = GCom.NzInt(REC.Item("TAKO_KBN_T"), 0)               '���s�敪
                        .KIGYO_CODE = GCom.NzDec(REC.Item("KIGYO_CODE_T"), "")          '��ƃR�[�h
                        .MOTIKOMI_KBN = GCom.NzDec(REC.Item("MOTIKOMI_KBN_T"), "")      '�����敪
                        .SFURI_FLG = GCom.NzInt(REC.Item("SFURI_FLG_T"), 0)             '�ĐU�t���O
                        .SFURI_FCODE = GCom.NzStr(REC.Item("SFURI_FCODE_T"))            '
                        .SFURI_DAY = GCom.NzInt(REC.Item("SFURI_DAY_T"), 0)           '�ĐU��
                        .SFURI_KYU_CODE = GCom.NzInt(REC.Item("SFURI_KYU_CODE_T"), 0)   '�ĐU�x���R�[�h
                        '2012/06/05 �W���ŏC���@���t�敪�i�ĐU�j�ǉ�
                        .SFURI_KIJITSU = GCom.NzInt(REC.Item("SFURI_KIJITSU_T"), 0)     '���t�敪�i�ĐU�j
                    Case Is = APL.SofuriApplication
                        '.TAKO_KBN = 0                                   '���s�敪
                        '.KIGYO_CODE = ""                                '��ƃR�[�h
                        '.MOTIKOMI_KBN = ""                              '�����敪
                        '.TOHO_CNT_CODE = ""                             '�`�������Z���^�[�m�F�R�[�h
                        '.SFURI_FLG = 0                                  '�ĐU�t���O
                        '.SFURI_KBN = 0                                  '�ĐU�敪
                        '.SFURI_DATE = 0                                 '�ĐU��
                        '.SFURI_KYU_CODE = 0                             '�ĐU�x���R�[�h
                End Select
            End With
        Catch ex As Exception

            Throw

        End Try
    End Sub

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^�ւ̓o�^
    '
    ' �����@ �F ARG1 - 0 = �ʃX�P�W���[���o�^, Else = ���ԃX�P�W���[���쐬
    '           ARG2 - ���O�o�͎���
    '           ARG3 - ���v�Z�����̋@�\���g�p����ꍇ=True
    '
    ' �߂�l �F ����I�� = True, �ُ�I�� = False
    '
    ' ���l�@ �F �ĐU�X�P�W���[���̌����␳
    '
    Public Function INSERT_NEW_SCHMAST(ByVal Index As Integer, _
            Optional ByVal SEL As Boolean = True, Optional ByVal CALC_ONLY As Boolean = False) As Boolean
        'With GCom.GLog
        '    .Job2 = "�X�P�W���[���}�X�^�o�^"
        'End With
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim SQL As String = ""
        Dim BRet As Boolean             '���t�]���֐��̖߂�l�󂯎M
        Dim Temp As String              '�ėp������ϐ�
        Dim onDate As Date              '�ėp���t�ϐ�
        Dim onText(2) As Integer        '�ėp���t�]�����z��
        Dim ColumnID1 As Integer
        Dim ColumnID2 As Integer
        Try
            '------------------------------------------------------------------
            '�������ؓ�(����)�����߂�
            '------------------------------------------------------------------
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.MOTIKOMI_DATE, TR(Index).MOTIKOMI_KIJITSU, 1)

            '------------------------------------------------------------------
            '�U���^�s�\�X�V�^�ԋp�\���
            '------------------------------------------------------------------
            '���s���z�M�\���
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HAISIN_YDATE, GCom.DayINI.HAISIN, 1)
            '���s���s�\���ʗ\���
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.FUNOU_YDATE, GCom.DayINI.FUNOU, 0)
            '------------------------------------------------------------------
            '�z�M�\��������߂�
            '------------------------------------------------------------------
            '���s�敪    : ��Ώ�    :          �Ώ�
            '------------------------------------------------------------------
            '�e�l�s�敪  : �S��      : SSS(��)    : SSS(�O)    :   ����ȊO
            '------------------------------------------------------------------
            '��          :       �p�^�[���P
            '------------------------------------------------------------------
            '���s�P      :    �~     :       �p�^�[���S        : �p�^�[���Q
            '------------------------------------------------------------------
            '���s�Q      :    �~     :     �~     : �p�^�[���T : �p�^�[���R
            '------------------------------------------------------------------
            '              �U�蕪���\��       �s�\�\��
            '�p�^�[��  1        - 3              + 1
            '          2        - 5              + 2
            '          3        - 6              + 3        �S��INI�t�@�C���w��
            '          4        - 5              + 3
            '          5        - 7              + 5
            '------------------------------------------------------------------
            '[JIFURI]  fskj.ini
            'FUNOU = 1
            'FUNOU_TAKOU_1 = 1
            'FUNOU_TAKOU_2 = 1
            'FUNOU_SSS_1 = 3
            'FUNOU_SSS_2 = 3
            'HAISIN = 3
            'HAISIN_TAKOU_1 = 3
            'HAISIN_TAKOU_2 = 3
            'HAISIN_SSS_1 = 6
            'HAISIN_SSS_2 = 7
            'KAISYU = 10
            'HITS = 4
            'HITQ = 3
            '------------------------------------------------------------------

            '���s�d�����L��(���s�敪), �t�H�[�}�b�g�敪 �Ō���
            Select Case TR(Index).FMT_KBN
                Case 20, 21
                    'SSS �͑��s�敪�Ɋ֌W�Ȃ��쐬����B
                    '20, �r�r�r(��g��)
                    '21, �r�r�r(��g�O)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.HAISIN_T1YDATE, GCom.DayINI.HAISIN_SSS_1, 1)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.HAISIN_T2YDATE, GCom.DayINI.HAISIN_SSS_2, 1)

                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.FUNOU_T1YDATE, GCom.DayINI.FUNOU_SSS_1, 0)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.FUNOU_T2YDATE, GCom.DayINI.FUNOU_SSS_2, 0)

                    Select Case TR(Index).FMT_KBN
                        Case 20
                            SCH.HENKAN_YDATE = SCH.FUNOU_T1YDATE    '�Ԋҗ\���
                            SCH.HAISIN_T2YDATE = New String("0"c, 8)
                            SCH.FUNOU_T2YDATE = New String("0"c, 8)
                        Case 21
                            SCH.HENKAN_YDATE = SCH.FUNOU_T2YDATE    '�Ԋҗ\���
                    End Select

                Case Else  '0, 1, 2, 3, 4, 5, 6
                    'SSS �ȊO�̃t�H�[�}�b�g�͑��s�敪�Ŕ��肷��
                    '00, �S��
                    '01, �n����(350)
                    '02, ����
                    '03, �N��
                    '04, �˗���
                    '05, �`�[
                    '06, �n����(300)
                    If TR(Index).TAKO_KBN = 0 Then
                        '0, ���s�f�[�^�쐬��Ώ�
                        SCH.HAISIN_T1YDATE = New String("0"c, 8)
                        SCH.HAISIN_T2YDATE = New String("0"c, 8)
                        SCH.FUNOU_T1YDATE = New String("0"c, 8)
                        SCH.FUNOU_T2YDATE = New String("0"c, 8)

                        SCH.HENKAN_YDATE = SCH.FUNOU_YDATE          '�Ԋҗ\���
                    Else
                        '1, ���s�f�[�^�쐬�Ώ�
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.HAISIN_T1YDATE, GCom.DayINI.HAISIN_TAKOU_1, 1)
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.HAISIN_T2YDATE, GCom.DayINI.HAISIN_TAKOU_2, 1)

                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.FUNOU_T1YDATE, GCom.DayINI.FUNOU_TAKOU_1, 0)
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.FUNOU_T2YDATE, GCom.DayINI.FUNOU_TAKOU_2, 0)

                        '���s�}�X�^�ɓ`���ȊO�����邩�ۂ�
                        Select Case CheckTakouMast(Index)
                            Case 0
                                '�`���̂�
                                SCH.HENKAN_YDATE = SCH.FUNOU_T1YDATE    '�Ԋҗ\���
                                SCH.HAISIN_T2YDATE = New String("0"c, 8)
                                SCH.FUNOU_T2YDATE = New String("0"c, 8)
                            Case Else
                                '�`���ȊO����
                                SCH.HENKAN_YDATE = SCH.FUNOU_T2YDATE    '�Ԋҗ\���
                        End Select
                    End If
            End Select

            '�����U�̏ꍇ�̏���
            If SchTable = APL.SofuriApplication Then

                Dim KYUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
                Dim SOUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)
                Dim HASSHIN As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "HASSIN"), 0)

                '�����m�ۗ\���(��̌��ϗ\����Z�o�����ŏ���������)2008.04.04 By Astar FJH���c���w��
                Select Case TR(Index).SYUBETU
                    Case Is = "11"
                        '���^�U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, KYUFURI, 1)
                    Case Is = "12"
                        '�ܗ^�U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, KYUFURI, 1)
                    Case Is = "21"
                        '�����U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, SOUFURI, 1)
                End Select

                '���M�\���
                Select Case TR(Index).SYUBETU
                    Case Is = "11"
                        '���^�U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                    Case Is = "12"
                        '�ܗ^�U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                    Case Is = "21"
                        '�����U��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                End Select

                '�Ԋҗ\���
                BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HENKAN_YDATE, 1, 0)
                SCH.HENKAN_YDATE = SCH.FURI_DATE
            End If

            '------------------------------------------------------------------
            '�˗�������\��������߂�
            '------------------------------------------------------------------
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.IRAISYOK_YDATE, GCom.DayINI.KAISYU, 1)

            '------------------------------------------------------------------
            '���ϗ\��������߂�
            '------------------------------------------------------------------
            'KESSAI_KIJITSU     '���ϊ����敪(0:�c�Ɠ����w��, 1:����w��, 2:��������w��)
            'KESSAI_DAY         '���ϓ����^���(1, 2, 3, 4, 6, 12 ���ꊇ�����̏ꍇ�̂ݗL��)
            'KESSAI_KYU_CODE    '���ϋx���R�[�h(0:���c�Ɠ��U��, 1:�O�c�Ɠ��U��)

            '2008.02.27 Insert By Astar
            If TR(Index).KESSAI_DAY = 0 Then
                TR(Index).KESSAI_DAY = 1
            End If

            Select Case TR(Index).KESSAI_KIJITSU
                Case 0
                    '�c�Ɠ����w��
                    Dim FrontBackType As Integer = 0
                    Select Case SchTable
                        Case APL.JifuriApplication
                            FrontBackType = 0
                        Case APL.SofuriApplication
                            FrontBackType = 1
                    End Select
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KESSAI_YDATE, TR(Index).KESSAI_DAY, FrontBackType)
                Case 1
                    '����w��(�����␳����)
                    '2008.04.21 Update By Astar
                    'onDate = GCom.SET_DATE(SCH.FURI_DATE)
                    onDate = GCom.SET_DATE(SCH.KFURI_DATE)
                    onText(0) = onDate.Year
                    onText(1) = onDate.Month
                    onText(2) = TR(Index).KESSAI_DAY

                    '����ŗ������
                    Ret = GCom.SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        '����łȂ��ꍇ�ɂ͌������Z�o
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                    BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)

                    '�U�֓�����O�̏ꍇ�ɂ͗����֍Čv�Z
                    If SCH.FURI_DATE > SCH.KESSAI_YDATE Then
                        '2010.03.01 �_��U�֓��ɕύX start
                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1) 
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        '2010.03.01 �_��U�֓��ɕύX end
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).KESSAI_DAY
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)
                    End If
                Case 2
                    '��������w��
                    '2008.04.21 Update By Astar
                    'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                    onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                    onText(0) = onDate.Year
                    onText(1) = onDate.Month
                    onText(2) = TR(Index).KESSAI_DAY
                    Ret = GCom.SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                    BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)
                Case 3
                    '�U�����w��
                    SCH.KESSAI_YDATE = SCH.FURI_DATE
            End Select

            If SchTable = APL.SofuriApplication Then

                SCH.KAKUHO_YDATE_S = SCH.KESSAI_YDATE
                SCH.KESSAI_YDATE = SCH.FURI_DATE
                If SCH.HASSIN_YDATE_S < SCH.KAKUHO_YDATE_S Then

                    SCH.HASSIN_YDATE_S = SCH.KAKUHO_YDATE_S
                End If
            End If

#If False Then
            If TR(Index).TORIS_CODE = "0000001" AndAlso TR(Index).TORIF_CODE = "01" Then Stop
#End If
            '------------------------------------------------------------------
            '�萔�������\��������߂�
            '------------------------------------------------------------------
            'TESUUTYO_KIJITSU   '�萔�����������敪(0:�c�Ɠ����w��, 1:����w��, 2:��������w��)
            'TESUUTYO_KBN       '�萔�������敪(0:�s�x����, 1:�ꊇ����, 2:���ʖƏ�, 3:�ʓr����(�ꊇ) 4:�ʓr����(�s�x))
            'TESUUTYO_NO        '�萔�����������^���
            'TESUU_KYU_CODE     '�萔���������x���R�[�h(0:���c�Ɠ��U��, 1:�O�c�Ɠ��U��)
            '�ȉ��A�ꊇ�����̏ꍇ�̂ݗL��
            'TESUUMAT_DAY        '�萔���W�v���� (2008.03.06) �m����(1,2,3,4,6,12)=12�̖�
            'TESUUMAT_MONTH    '�W�v���     (2008.03.06)
            'TESUUMAT_ENDDAY       '�W�v�I����     (2008.03.06)
            'TESUUMAT_KIJYUN2   '�W�v�       (2008.03.06) (0:�U�֓�, 1:���ϓ�)
            'TESUUTYO_PATN      '�萔���������@ (2008.03.06) (0:��������, 1:���ړ���)
            'KESSAI_KBN         '���ϋ敪       (2008.03.06) 
            '���ϋ敪 = (0:�a����, 1:��������, 2:�ב֐U��, 3:�ב֕t��, 4:���ʊ��, 5:���ϑΏۊO)
            '2008.03.06 FJH �d�l
            '�W�v�J�n�����(TESUUMAT_KIJYUN)�Ŋς�B
            '�W�v�ɓ���邩�ۂ���(TESUUMAT_KIJYUN2)�Ŕ��f�B�U�֓��͌_����ŁA���ϓ��͌v�Z�\����ōs���B
            '�W�v�ɓ���邩�ۂ��̊��(TESUUMAT_END)�ōs���B

            '2008.02.27 Insert By Astar
            If TR(Index).TESUUTYO_DAY = 0 Then
                TR(Index).TESUUTYO_DAY = 1
            End If

            Select Case TR(Index).TESUUTYO_KBN

                Case 1

                    '�ꊇ����(���������͐�΂ɂȂ��B���d�l)

                    '�萔����������z�񉻂���
                    Dim ALL_Month(GCom.NzInt(12 \ TR(Index).TESUUMAT_NO)) As Integer

                    '2008.04.12 Update By Astar
                    '�����܂ł��N�Ԃ�ʂ����T�C�N���ōl����
                    Select Case ALL_Month.GetUpperBound(0)
                        Case 2, 3, 4, 6
                            onText(1) = TR(Index).TESUUMAT_MONTH - 1
                            Do While onText(1) - TR(Index).TESUUMAT_NO > 0
                                onText(1) -= TR(Index).TESUUMAT_NO
                            Loop
                            If onText(1) <= 0 Then
                                onText(1) = TR(Index).TESUUMAT_NO
                            End If
                            ALL_Month(1) = onText(1)

                            For Cnt = 2 To ALL_Month.GetUpperBound(0) Step 1
                                onText(1) = ALL_Month(Cnt - 1) + TR(Index).TESUUMAT_NO
                                ALL_Month(Cnt) = onText(1)
                            Next Cnt
                        Case 12
                            For Cnt = 1 To ALL_Month.GetUpperBound(0) Step 1
                                ALL_Month(Cnt) = Cnt
                            Next Cnt
                        Case Else
                            If TR(Index).TESUUMAT_MONTH = 1 Then
                                ALL_Month(1) = 12
                            Else
                                ALL_Month(1) = TR(Index).TESUUMAT_MONTH - 1
                            End If
                    End Select

                    Dim CompDay As Integer
                    Select Case TR(Index).TESUUMAT_KIJYUN
                        Case 0
                            '�U�֓��̏ꍇ(�_��U�֓��Ŕ��f����B2008.04.11 By FJH)**** �� ���U�֓��ɕύX2008/01/04 nishida
                            onText(0) = GCom.NzInt(SCH.FURI_DATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.FURI_DATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.FURI_DATE.Substring(6, 2), 0)
                            'onText(0) = GCom.NzInt(SCH.KFURI_DATE.Substring(0, 4), 0)
                            'onText(1) = GCom.NzInt(SCH.KFURI_DATE.Substring(4, 2), 0)
                            'onText(2) = GCom.NzInt(SCH.KFURI_DATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                        Case Else
                            '���ϓ��̏ꍇ(�_�񌈍ϓ��Ŕ��f����B2008.04.11 By FJH)
                            onText(0) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(6, 2), 0)
                            '*** �C�� mitsu 2008/12/26 �萔�����������ł͂Ȃ����ϗ\��� ***
                            'CompDay = TR(Index).KESSAI_NO
                            CompDay = onText(2)
                            '**************************************************************
                    End Select
                    Dim memMonth As Integer = onText(1)

                    '����萔�������������߂�
                    For Cnt = 1 To ALL_Month.GetUpperBound(0) Step 1

                        If onText(1) <= ALL_Month(Cnt) Then

                            onText(1) = ALL_Month(Cnt)
                            Exit For
                        End If
                    Next

                    '�萔���������z����̌��l���U�֌��l��ǂ��z���Ȃ������ꍇ(2008.04.16 By Astar)
                    If Cnt > ALL_Month.GetUpperBound(0) Then
                        '���N�x�̍ŏ��̒�������ݒ肷��
                        onText(0) += 1
                        onText(1) = ALL_Month(1)
                    Else
                        '2008.04.18 Update By Astar
                        '*** �C�� mitsu 2008/12/26 �W�v������ϓ��̏ꍇ���l�� ***
                        'If GCom.NzInt(SCH.KFURI_DATE.Substring(4, 2), 0) = memMonth Then
                        If (TR(Index).TESUUMAT_KIJYUN = 0 AndAlso GCom.NzInt(SCH.KFURI_DATE.Substring(4, 2), 0) = memMonth) OrElse _
                           (TR(Index).TESUUMAT_KIJYUN <> 0 AndAlso GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0) = memMonth) Then
                            '******************************************************
                            '�I�����̊֌W�œ������ɐݒ�ł��Ȃ��ꍇ�ɂ͂P�T�C�N�����炷
                            If onText(1) <= memMonth AndAlso TR(Index).TESUUMAT_ENDDAY < CompDay Then
                                onText(1) += TR(Index).TESUUMAT_NO
                                If onText(1) > 12 Then
                                    onText(0) += 1
                                    onText(1) = onText(1) - 12
                                End If
                            End If
                        End If
                    End If

                    '2008.04.18 Update By Astar
                    Ret = SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        '����łȂ��ꍇ�ɂ͌������Z�o
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)

                    '�萔���������̐U�֓����Z�o����B
                    Dim Temp_FURI_DATE As String = ""

                    '*** �C�� mitsu 2008/12/31 ��������p �ꊇ�����E�ʓr�������̒����\����v�Z ***
                    Select Case TR(Index).TESUUTYO_KBN
                        Case 1, 3
                            Temp = Temp.Substring(0, 6) & "15"
                    End Select
                    '*****************************************************************************

                    Select Case TR(Index).FURI_KYU_CODE
                        Case 0, 1
                            '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, TR(Index).FURI_KYU_CODE)
                        Case Else
                            '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, 0)
                            If GCom.NzInt(Temp.Substring(0, 6), 0) < _
                                GCom.NzInt(Temp_FURI_DATE.Substring(0, 6), 0) Then

                                BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, 1, 1)
                            End If
                    End Select

                    '�萔�������֘A�w�荀�ڂɏ]���ĎZ�o����
                    Select Case TR(Index).TESUUTYO_KIJITSU
                        Case 0
                            '�c�Ɠ����w��
                            '*** �C�� mitsu 2008/12/26 ��������p �ꊇ�����E�ʓr�������̒����\����v�Z ***
                            'BRet = GCom.CheckDateModule(Temp_FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_NO, 0)
                            Select Case TR(Index).TESUUTYO_KBN
                                Case 1, 3
                                    '�W�v�I����(�����␳����)
                                    onText(0) = Integer.Parse(Temp_FURI_DATE.Substring(0, 4))
                                    onText(1) = Integer.Parse(Temp_FURI_DATE.Substring(4, 2))
                                    onText(2) = TR(Index).TESUUMAT_ENDDAY
                                    '�������
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        '����łȂ��ꍇ�ɂ͌������Z�o
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If

                                    '�W�v�I�������萔���������x���R�[�h�ŉc�Ɠ��␳����
                                    BRet = GCom.CheckDateModule(onDate.ToString("yyyyMMdd"), Temp_FURI_DATE, TR(Index).TESUU_KYU_CODE)
                                    '�W�v�I��������c�Ɠ������w�肷��
                                    BRet = GCom.CheckDateModule(Temp_FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                                Case Else
                                    BRet = GCom.CheckDateModule(Temp_FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                            End Select
                            '*****************************************************************************
                        Case 1
                            '����w��(�����␳����)
                            onDate = GCom.SET_DATE(Temp_FURI_DATE)
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = TR(Index).TESUUTYO_DAY

                            '����ŗ������
                            Ret = GCom.SET_DATE(onDate, onText)
                            If Not Ret = -1 Then
                                '����łȂ��ꍇ�ɂ͌������Z�o
                                onText(0) = onDate.Year
                                onText(1) = onDate.Month
                                onText(2) = 1
                                Ret = GCom.SET_DATE(onDate, onText)
                                onDate = onDate.AddDays(-1)
                            End If
                            Temp = String.Format("{0:yyyyMMdd}", onDate)
                            BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)

                            '2008.04.25 �p�~ By Astar
                            '�U�֓�����O�̏ꍇ�ɂ͎��̏W�v���֍Čv�Z
                            'If SCH.FURI_DATE > SCH.TESUU_YDATE Then
                            '    onDate = GCom.SET_DATE(Temp_FURI_DATE).AddMonths(1)
                            '    onText(0) = onDate.Year
                            '    onText(1) = onDate.Month
                            '    onText(2) = TR(Index).TESUUTYO_DAY
                            '    Ret = GCom.SET_DATE(onDate, onText)
                            '    If Not Ret = -1 Then
                            '        onText(0) = onDate.Year
                            '        onText(1) = onDate.Month
                            '        onText(2) = 1
                            '        Ret = GCom.SET_DATE(onDate, onText)
                            '        onDate = onDate.AddDays(-1)
                            '    End If
                            '    Temp = String.Format("{0:yyyyMMdd}", onDate)
                            '    BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                            'End If
                        Case 2
                            '��������w��
                            onDate = GCom.SET_DATE(Temp_FURI_DATE).AddMonths(1)
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = TR(Index).TESUUTYO_DAY
                            Ret = GCom.SET_DATE(onDate, onText)
                            If Not Ret = -1 Then
                                onText(0) = onDate.Year
                                onText(1) = onDate.Month
                                onText(2) = 1
                                Ret = GCom.SET_DATE(onDate, onText)
                                onDate = onDate.AddDays(-1)
                            End If
                            Temp = String.Format("{0:yyyyMMdd}", onDate)
                            BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                        Case 3
                            '�U�����w��
                            SCH.TESUU_YDATE = SCH.FURI_DATE
                        Case 4
                            '�m�ۓ��w��
                            SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                    End Select
                Case 2  '���ʖƏ�

                    SCH.TESUU_YDATE = "00000000"

                Case 3  '�ʓr����

                    SCH.TESUU_YDATE = SCH.KESSAI_YDATE

                Case Else
                    '�s�x����
                    Select Case TR(Index).TESUUTYO_PATN
                        Case 0
                            '��������
                            SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                        Case Else

                            '���ړ���
                            Select Case TR(Index).TESUUTYO_KIJITSU
                                Case 0
                                    '�c�Ɠ����w��
                                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                                Case 1
                                    '����w��(�����␳����)
                                    onDate = GCom.SET_DATE(SCH.FURI_DATE)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).TESUUTYO_DAY

                                    '����ŗ������
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        '����łȂ��ꍇ�ɂ͌������Z�o
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                                    BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)

                                    '�U�֓�����O�̏ꍇ�ɂ͗����֍Čv�Z
                                    If SCH.FURI_DATE > SCH.TESUU_YDATE Then
                                        '2010.03.01 �_��U�֓��ɕύX start
                                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                        '2010.03.01 �_��U�֓��ɕύX end
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = TR(Index).TESUUTYO_DAY
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        If Not Ret = -1 Then
                                            onText(0) = onDate.Year
                                            onText(1) = onDate.Month
                                            onText(2) = 1
                                            Ret = GCom.SET_DATE(onDate, onText)
                                            onDate = onDate.AddDays(-1)
                                        End If
                                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                                        BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                                    End If
                                Case 2
                                    '��������w��
                                    onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).TESUUTYO_DAY
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                                    BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                                Case 3
                                    '�U�����w��
                                    SCH.TESUU_YDATE = SCH.FURI_DATE
                                Case 4
                                    '�m�ۓ��w��
                                    SCH.TESUU_YDATE = SCH.KESSAI_YDATE

                            End Select

                            '2008.04.03 By Astar �ً}���I���u(�����}�X�^���񌒑S�̏ꍇ�̏��u)
                            If SCH.TESUU_YDATE < SCH.KESSAI_YDATE Then
                                SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                            End If

                            '*** kakinoki 2008/5/15 ***************************************************
                            '*** �萔�������敪���u�m�ۓ��w��v�̏ꍇ�A�萔�������\����������m�ۗ\���
                            '*** �Ɠ����ɂ��܂��B                                                     *
                            '************************************************************************** 
                            If SchTable = APL.SofuriApplication Then
                                If TR(Index).TESUUTYO_KIJITSU = 4 Then
                                    SCH.TESUU_YDATE = SCH.KAKUHO_YDATE_S
                                End If
                            End If
                            '*** kakinoki 2008/5/15 ***************************************************
                            '*** �萔�������敪���u�m�ۓ��w��v�̏ꍇ�A�萔�������\����������m�ۗ\���
                            '*** �Ɠ����ɂ��܂��B                                                     *
                            '************************************************************************** 

                    End Select
            End Select

            '------------------------------------------------------------------
            '�ĐU�\��������߂�
            '------------------------------------------------------------------
            'SFURI_FLG      '�ĐU�t���O(0:�ĐU�_��Ȃ�, 1:�ĐU�_�񂠂�)
            'SFURI_KBN      '�ĐU�敪(0:�c�Ɠ����w��, 1:����w��)
            'SFURI_DAY      '�ĐU�_���(�ĐU����̏ꍇ�A�ĐU�֓����w��@�U�֓�����̉c�Ɠ����@or ����w��)
            'SFURI_KYU_CODE '�ĐU�x���R�[�h(0:���c�Ɠ��U��, 1:�O�c�Ɠ��U��)
            If TR(Index).SFURI_FLG = 1 Then
                '�ĐU�t���O(�ĐU�L) 

                '2008.02.27 Insert By Astar
                If TR(Index).SFURI_DAY = 0 Then
                    TR(Index).SFURI_DAY = 1
                End If

                Select Case TR(Index).SFURI_KIJITSU
                    Case 0
                        '�c�Ɠ����w��
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KSAIFURI_DATE, TR(Index).SFURI_DAY, 0)
                    Case 1
                        '����w��(�����␳����)
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE)
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).SFURI_DAY

                        '����ŗ������
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            '����łȂ��ꍇ�ɂ͌������Z�o
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        Select Case TR(Index).SFURI_KYU_CODE
                            Case 0, 1
                                '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                                BRet = GCom.CheckDateModule(Temp, _
                                                SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)

                                '�U�֓�����O�̏ꍇ�ɂ͗����֍Čv�Z
                                If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                    '2012/06/05 �W���ŏC���@�ĐU���𗂌��ɂ��čČv�Z
                                    onDate = GCom.SET_DATE(SCH.KSAIFURI_DATE).AddMonths(1)
                                    'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).SFURI_DAY
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)

                                    '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                                    BRet = GCom.CheckDateModule(Temp, _
                                                    SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)
                                End If
                            Case Else
                                '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                                BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 0)

                                '�ĐU�x���R�[�h���u2�v���c�Ɠ��U��(���ׂ����O�c�Ɠ�)�ł͓����������Ƃ���B
                                If GCom.NzInt(SCH.FURI_DATE.Substring(0, 6), 0) < _
                                   GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 6), 0) Then

                                    Temp = SCH.KSAIFURI_DATE

                                    '�����قȂ�ꍇ�ɑO���ŏI�c�Ɠ��֕␳����B
                                    onText(0) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 4), 0)
                                    onText(1) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(4, 2), 0)
                                    onText(2) = 1
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    BRet = GCom.CheckDateModule(onDate.AddDays(-1).ToString("yyyyMMdd"), _
                                                                    SCH.KSAIFURI_DATE, 1)

                                    If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                        '�U�֓�����O�̏ꍇ�ɂ͍Čv�Z��j������

                                        SCH.KSAIFURI_DATE = Temp
                                    End If
                                End If
                        End Select
                    Case 2
                        '��������w��
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).SFURI_DAY
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        Select Case TR(Index).SFURI_KYU_CODE
                            Case 0, 1
                                '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                                BRet = GCom.CheckDateModule(Temp, _
                                                SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)
                            Case Else
                                '�U�֓�,�c�Ɠ�����(�y�E���E�j�Փ�����)
                                BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 0)

                                '�ĐU�x���R�[�h���u2�v���c�Ɠ��U��(���ׂ����O�c�Ɠ�)�ł͓����������Ƃ���B
                                onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                If GCom.NzInt(onDate.ToString("yyyyMM"), 0) < _
                                   GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 6), 0) Then

                                    Temp = SCH.KSAIFURI_DATE

                                    '�����قȂ�ꍇ�ɑO���ŏI�c�Ɠ��֕␳����B
                                    onText(0) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 4), 0)
                                    onText(1) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(4, 2), 0)
                                    onText(2) = 1
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    Temp = onDate.ToString("yyyyMMdd")
                                    BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 1, 1)

                                    If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                        '�U�֓�����O�̏ꍇ�ɂ͍Čv�Z��j������

                                        SCH.KSAIFURI_DATE = Temp
                                    End If
                                End If

                        End Select
                End Select
            Else
                '�ĐU�Ȃ�
                SCH.KSAIFURI_DATE = New String("0"c, 8)
            End If

            '�v�Z���W�b�N�݂̂Ŋ֐��𔲂���ꍇ(2008.03.18 By Astar)
            If CALC_ONLY Then
                Return True
            End If

            '------------------------------------------------------------------
            '�}�X�^�o�^���ڐݒ�(SQL��)
            '------------------------------------------------------------------
            Select Case SchTable
                Case Is = APL.JifuriApplication

                    SQL = "INSERT INTO SCHMAST"

                Case Is = APL.SofuriApplication

                    SQL = "INSERT INTO S_SCHMAST"
            End Select

            For ColumnID1 = 0 To MaxColumn Step 1
                Select Case ColumnID1
                    Case 0
                        SQL &= " (" & ORANAME(ColumnID1)
                    Case Else
                        SQL &= ", " & ORANAME(ColumnID1)
                End Select
            Next ColumnID1
            SQL &= ") VALUES"

            For ColumnID2 = 0 To MaxColumn Step 1

                If ColumnID2 = 0 Then
                    SQL &= " ("
                Else
                    SQL &= ", "
                End If

                Select Case ORANAME(ColumnID2)
                    Case "FSYORI_KBN_S"
                        '�U�֏����敪1
                        SQL &= SetItem(ColumnID2, TR(Index).FSYORI_KBN)
                    Case "TORIS_CODE_S"
                        '������R�[�h2
                        SQL &= SetItem(ColumnID2, TR(Index).TORIS_CODE)
                    Case "TORIF_CODE_S"
                        '����敛�R�[�h3
                        SQL &= SetItem(ColumnID2, TR(Index).TORIF_CODE)
                    Case "FURI_DATE_S"
                        '�U�֓�4
                        SQL &= SetItem(ColumnID2, SCH.FURI_DATE)
                    Case "KFURI_DATE_S"
                        '�_��U�֓�5
                        SQL &= SetItem(ColumnID2, SCH.KFURI_DATE)
                    Case "SAIFURI_DATE_S"
                        '�ĐU�֓�6
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "KSAIFURI_DATE_S"
                        '�_��ĐU�֗\��7
                        SQL &= SetItem(ColumnID2, SCH.KSAIFURI_DATE)
                    Case "FURI_CODE_S"
                        '�U�փR�[�h8
                        SQL &= SetItem(ColumnID2, TR(Index).FURI_CODE)
                    Case "KIGYO_CODE_S"
                        '��ƃR�[�h9
                        SQL &= SetItem(ColumnID2, TR(Index).KIGYO_CODE)
                    Case "ITAKU_CODE_S"
                        '�ϑ��҃R�[�h10
                        SQL &= SetItem(ColumnID2, TR(Index).ITAKU_CODE)
                    Case "TKIN_NO_S"
                        '�戵���Z�@��11
                        SQL &= SetItem(ColumnID2, TR(Index).TKIN_NO)
                    Case "TSIT_NO_S"
                        '�戵�X��12
                        SQL &= SetItem(ColumnID2, TR(Index).TSIT_NO)
                    Case "SOUSIN_KBN_S"
                        '���M�敪13
                        SQL &= SetItem(ColumnID2, TR(Index).SOUSIN_KBN)
                    Case "MOTIKOMI_KBN_S"
                        '�����敪14
                        SQL &= SetItem(ColumnID2, TR(Index).MOTIKOMI_KBN)
                    Case "BAITAI_CODE_S"
                        '�}�̃R�[�h15
                        SQL &= SetItem(ColumnID2, TR(Index).BAITAI_CODE)
                    Case "MOTIKOMI_SEQ_S"
                        '�����r�d�p16
                        SQL &= SetItem(ColumnID2, SCH.MOTIKOMI_SEQ.ToString)
                    Case "FILE_SEQ_S"
                        '�t�@�C���r�d�p17
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KBN_S"
                        '�萔���v�Z�敪18
                        '2008.03.28 By Astar ����g�p�ɑς��Ȃ��t���O
                        Select Case TR(Index).TESUUTYO_KBN
                            Case 0
                                '�萔�������敪 = �s�x����
                                SQL &= SetItem(ColumnID2, "1")
                                '*** �C�� mitsu 2008/12/26 ��������p �ʓr�������܂߂� ***
                                'Case 1
                            Case 1, 3
                                '*********************************************************
                                '�萔�������敪 = �ꊇ����
                                Select Case TR(Index).MONTH_FLG
                                    Case 1, 3
                                        SQL &= SetItem(ColumnID2, "2")
                                    Case Else
                                        SQL &= SetItem(ColumnID2, "3")
                                End Select
                            Case Else
                                '�萔�������敪 = (2)���ʖƏ�, (3)�ʓr����
                                SQL &= SetItem(ColumnID2, "0")
                        End Select
                    Case "MOTIKOMI_DATE_S"
                        '�����\���
                        SQL &= SetItem(ColumnID2, SCH.MOTIKOMI_DATE)
                    Case "IRAISYO_DATE_S"
                        '�˗����쐬��19
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "IRAISYOK_YDATE_S"
                        '�˗�������\���20
                        SQL &= SetItem(ColumnID2, SCH.IRAISYOK_YDATE)
                    Case "UKETUKE_DATE_S"
                        '��t��21
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "TOUROKU_DATE_S"
                        '�o�^��22
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "HAISIN_YDATE_S"
                        '�z�M�����\���23
                        SQL &= SetItem(ColumnID2, SCH.HAISIN_YDATE)
                    Case "HAISIN_DATE_S"
                        '�z�M������24
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "SOUSIN_YDATE_S"
                        '���M�\���25
                        SQL &= SetItem(ColumnID2, SCH.HAISIN_YDATE)
                    Case "SOUSIN_DATE_S"
                        '���M������26
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "FUNOU_YDATE_S"
                        '�s�\�����\���27
                        SQL &= SetItem(ColumnID2, SCH.FUNOU_YDATE)
                    Case "FUNOU_DATE_S"
                        '�s�\������28
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "KESSAI_YDATE_S"
                        '���ϗ\���29
                        SQL &= SetItem(ColumnID2, SCH.KESSAI_YDATE)
                    Case "KESSAI_DATE_S"
                        '���ϓ�30
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "TESUU_YDATE_S"
                        '�萔�������\���31
                        SQL &= SetItem(ColumnID2, SCH.TESUU_YDATE)
                    Case "TESUU_DATE_S"
                        '�萔��������32
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "HENKAN_YDATE_S"
                        '�Ԋҏ����\���33
                        SQL &= SetItem(ColumnID2, SCH.HENKAN_YDATE)
                    Case "HENKAN_DATE_S"
                        '�Ԋҏ�����34
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "UKETORI_DATE_S"
                        '��揑�쐬��35
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "UKETUKE_FLG_S"
                        '��t�σt���O36
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TOUROKU_FLG_S"
                        '�o�^�σt���O37
                        SQL &= SetItem(ColumnID2, "0")
                    Case "HAISIN_FLG_S"
                        '�z�M�σt���O38
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SAIFURI_FLG_S"
                        '�ĐU�σt���O39
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SOUSIN_FLG_S"
                        '���M�σt���O40
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_FLG_S"
                        '�s�\�σt���O41
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUUKEI_FLG_S"
                        '�萔���v�Z�σt���O42
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUUTYO_FLG_S"
                        '�萔�������σt���O43
                        SQL &= SetItem(ColumnID2, "0")
                    Case "KESSAI_FLG_S"
                        '���σt���O44
                        SQL &= SetItem(ColumnID2, "0")
                    Case "HENKAN_FLG_S"
                        '�Ԋҍσt���O45
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TYUUDAN_FLG_S"
                        '���f�t���O46
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TAKOU_FLG_S"
                        '���s�t���O47
                        SQL &= SetItem(ColumnID2, "0")
                    Case "NIPPO_FLG_S"
                        '����t���O48
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERROR_INF_S"
                        '�G���[���49
                        SQL &= "NULL"
                    Case "SYORI_KEN_S"
                        '��������50
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SYORI_KIN_S"
                        '�������z51
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERR_KEN_S"
                        '�C���v�b�g�G���[����52
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERR_KIN_S"
                        '�C���v�b�g�G���[���z53
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN_S"
                        '�萔�����z54
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN1_S"
                        '�萔�����z�P55
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN2_S"
                        '�萔�����z�Q56
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN3_S"
                        '�萔�����z�R57
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FURI_KEN_S"
                        '�U�֍ό���58
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FURI_KIN_S"
                        '�U�֍ϋ��z59
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_KEN_S"
                        '�s�\����60
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_KIN_S"
                        '�s�\���z61
                        SQL &= SetItem(ColumnID2, "0")
                    Case "UFILE_NAME_S"
                        '��M�t�@�C����62
                        SQL &= "NULL"
                    Case "SFILE_NAME_S"
                        '���M�t�@�C����63
                        SQL &= "NULL"
                    Case "SAKUSEI_DATE_S"
                        '�쐬���t64
                        SQL &= "TO_CHAR(SYSDATE, 'yyyymmdd')"
                    Case "JIFURI_TIME_STAMP_S"
                        '�z�M�^�C���X�^���v65
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "KESSAI_TIME_STAMP_S"
                        '���σ^�C���X�^���v66
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "TESUU_TIME_STAMP_S"
                        '�萔�������^�C���X�^���v67
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "YOBI1_S"
                        '�\���P68
                        SQL &= "NULL"
                    Case "YOBI2_S"
                        '�\���Q69
                        SQL &= "NULL"
                    Case "YOBI3_S"
                        '�\���R70
                        SQL &= "NULL"
                    Case "YOBI4_S"
                        '�\���S71
                        SQL &= "NULL"
                    Case "YOBI5_S"
                        '�\���T72
                        SQL &= "NULL"
                    Case "YOBI6_S"
                        '�\���U73
                        SQL &= "NULL"
                    Case "YOBI7_S"
                        '�\���V74
                        SQL &= "NULL"
                    Case "YOBI8_S"
                        '�\���W75
                        SQL &= "NULL"
                    Case "YOBI9_S"
                        '�\���X76
                        SQL &= "NULL"
                    Case "YOBI10_S"
                        '�\���P�O77
                        SQL &= "NULL"
                    Case Else
                        SQL &= "NULL"
                End Select
            Next ColumnID2
            SQL &= ")"

            Dim SQLCode As Integer = 0
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, SEL)
            If Ret = 1 AndAlso SQLCode = 0 Then
                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
                If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastSQL As New StringBuilder
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub_RetSQL(TR(Index).FSYORI_KBN, _
                                                                             TR(Index).TORIS_CODE, _
                                                                             TR(Index).TORIF_CODE, _
                                                                             SCH.FURI_DATE, _
                                                                             SCH.MOTIKOMI_SEQ, _
                                                                             ReturnMessage, _
                                                                             SubMastSQL)
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SubMastSQL.ToString, SQLCode, SEL)
                End If
                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
                Return True
            End If
        Catch ex As Exception

            Throw

        End Try

        Return False
    End Function

    '
    ' �@�\�@ �F �}�����x���֐�
    '
    ' �����@ �F ARG1 - ORACLE COLUMN_ID Index
    ' �@�@�@ �@ ARG2 - �ݒ�l
    '
    ' �߂�l �F SQL���}��������
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function SetItem(ByVal Index As Integer, ByVal aData As String) As String
        If aData.Length = 0 Then
            Return "NULL"
        Else
            Select Case ORATYPE(Index)
                Case "CHAR"
                    Return "'" & aData & "'"
                Case "NUMBER"
                    Return aData
                Case Else
                    Return "'" & aData & "'"
            End Select
        End If
    End Function

    '
    ' �@�\�@ �F ���s�}�X�^�̃`�F�b�N
    '
    ' �����@ �F ARG1 - �~�� TORIMAST-Index
    '
    ' �߂�l �F �`���ȊO�̑��茏��
    '
    ' ���l�@ �F �Q��ނ̕��@������(True�^False���t�]����Ε��@���ς���)
    '
    Private Function CheckTakouMast(ByVal Index As Integer) As Integer
        'With GCom.GLog
        '    .Job2 = "���s�}�X�^�Q�ƃ`�F�b�N����"
        'End With
        Dim Ret As Integer = 0
        Dim SQL As String
        Dim BRet As Boolean
        Dim REC As OracleDataReader = Nothing
        Try
            Select Case True
                Case True
                    '�`���ȊO�̔}�̃R�[�h���������Ă��邩�ۂ�
                    SQL = "SELECT COUNT(*) COUNTER"
                    SQL &= " FROM TAKOUMAST"
                    SQL &= " WHERE TORIS_CODE_V = '" & TR(Index).TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_V = '" & TR(Index).TORIF_CODE & "'"
                    SQL &= " AND NOT TO_NUMBER(NVL(BAITAI_CODE_V, '0')) = 0"
                    BRet = GCom.SetDynaset(SQL, REC)
                    If BRet AndAlso REC.Read Then
                        Ret = GCom.NzInt(REC.Item("COUNTER"), 0)
                    End If
                Case False
                    '�`���ȊO�̐U�蕪���}�̗̂L�����o
                    SQL = "SELECT TKIN_NO_V"
                    SQL &= " FROM TAKOUMAST"
                    SQL &= " WHERE TORIS_CODE_V = '" & TR(Index).TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_V = '" & TR(Index).TORIF_CODE & "'"
                    BRet = GCom.SetDynaset(SQL, REC)
                    If BRet Then
                        Do While REC.Read
                            Dim BKCode As String = GCom.NzDec(REC.Item("TKIN_NO_V"), "")
                            Dim BAITAI_CODE As String = CASTCommon.GetIni("FURIWAKE.INI", BKCode, "Baitai")
                            If Not GCom.NzInt(BAITAI_CODE, 0) = 0 Then
                                Ret += 1
                            End If
                        Loop
                    End If
            End Select
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^����폜����
    '
    ' �����@ �F ARG1 - �U�֓��w��N��
    '
    ' �߂�l �F OK = True, NG = False
    '
    ' ���l�@ �F ���g�p�̊Y���N���X�P�W���[�����폜����(�_��U�֓������ɕύX�B2007.12.07)
    '
    Public Function DELETE_SCHMAST(ByVal FormFuriDate As Date) As Boolean
        'With GCom.GLog
        '    .Job2 = "�U�֓��w��X�P�W���[���}�X�^���R�[�h�폜"
        'End With
        Try
            Dim StartDay As String = String.Format("{0:yyyyMM}", FormFuriDate) & "01"
            Dim onDate As Date = FormFuriDate.AddMonths(1)
            onDate = GCom.SET_DATE(String.Format("{0:yyyyMM}", onDate) & "01")
            onDate = onDate.AddDays(-1)
            Dim EndDay As String = String.Format("{0:yyyyMMdd}", onDate)

            Dim SQL As String = ""
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "DELETE FROM SCHMAST"
                Case Is = APL.SofuriApplication
                    SQL = "DELETE FROM S_SCHMAST"
            End Select

            '�_��U�֓������ɕύX�B2007.12.07 By Astar
            SQL &= " WHERE KFURI_DATE_S BETWEEN '" & StartDay & "' AND '" & EndDay & "'"
            SQL &= " AND UKETUKE_FLG_S = '0'"
            SQL &= " AND TOUROKU_FLG_S = '0'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " AND NIPPO_FLG_S = '0'"
                Case Is = APL.SofuriApplication
            End Select
            SQL &= " AND TYUUDAN_FLG_S = '0'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " AND NOT TO_NUMBER(NVL(BAITAI_CODE_S, '0')) = 7"
                Case Is = APL.SofuriApplication
            End Select

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If SQLCode = 0 Then
                    Dim Dsql As New StringBuilder(128)
                    Dsql.Length = 0
                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                            Dsql.Append("DELETE FROM SCHMAST_SUB ")
                        Case Is = APL.SofuriApplication
                            Dsql.Append("DELETE FROM S_SCHMAST_SUB ")
                    End Select
                    Dsql.Append(" WHERE ")
                    Dsql.Append("     NOT EXISTS")
                    Dsql.Append("     (")
                    Dsql.Append("      SELECT")
                    Dsql.Append("          *")
                    Dsql.Append("      FROM")
                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                            Dsql.Append("  SCHMAST")
                        Case Is = APL.SofuriApplication
                            Dsql.Append("  S_SCHMAST ")
                    End Select
                    Dsql.Append("      WHERE")
                    Dsql.Append("          TORIS_CODE_S       = TORIS_CODE_SSUB")
                    Dsql.Append("      AND TORIF_CODE_S       = TORIF_CODE_SSUB")
                    Dsql.Append("      AND FURI_DATE_S        = FURI_DATE_SSUB")
                    Select Case SchTable
                        Case Is = APL.SofuriApplication
                            Dsql.Append(" AND MOTIKOMI_SEQ_S  = MOTIKOMI_SEQ_SSUB")
                    End Select
                    Dsql.Append("     )")

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)
                End If
            End If
            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

            Return (SQLCode = 0)
        Catch ex As Exception

            Throw

        End Try

        Return False
    End Function

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^�폜
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F ����I�� = True, �ُ�I�� = False
    '
    ' ���l�@ �F �ʃX�P�W���[���o�^�p
    '
    Public Function DELETE_SCHMAST(Optional ByVal cFlg As Boolean = True) As Boolean
        'With GCom.GLog
        '    .Job2 = "�X�P�W���[���}�X�^���R�[�h�폜"
        'End With
        Try
            Dim SQL As String = ""
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "DELETE FROM SCHMAST"
                Case Is = APL.SofuriApplication
                    SQL = "DELETE FROM S_SCHMAST"
            End Select
            SQL &= " WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_S = '" & SCH.FURI_DATE & "'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                Case Is = APL.SofuriApplication
                    SQL &= " AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ
            End Select

            Dim SQLCode As Integer = 0
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If Ret = 1 And SQLCode = 0 Then
                    SQL = ""
                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                            SQL = "DELETE FROM SCHMAST_SUB"
                        Case Is = APL.SofuriApplication
                            SQL = "DELETE FROM S_SCHMAST_SUB"
                    End Select
                    SQL &= " WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'"
                    SQL &= " AND TORIS_CODE_SSUB   = '" & TR(0).TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_SSUB   = '" & TR(0).TORIF_CODE & "'"
                    SQL &= " AND FURI_DATE_SSUB    = '" & SCH.FURI_DATE & "'"
                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                        Case Is = APL.SofuriApplication
                            SQL &= " AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ
                    End Select

                    SQLCode = 0
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                End If
            End If
            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

            '***************************************
            '�ĐU2009.10.05
            If TR(0).SFURI_FLG = 1 Then
                If Ret = 1 And SQLCode = 0 Then
                    Dim Dsql As New StringBuilder(128)

                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                            Dsql.Append("DELETE FROM SCHMAST")
                            Dsql.Append(" WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'")
                            Dsql.Append(" AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'")
                            Dsql.Append(" AND TORIF_CODE_S = '" & TR(0).SFURI_FCODE & "'") '�ĐU���R�[�h
                            Dsql.Append(" AND FURI_DATE_S = '" & SCH.WRK_SFURI_YDATE & "'")
                            Dsql.Append(" AND UKETUKE_FLG_S ='0'")
                            Dsql.Append(" AND TOUROKU_FLG_S ='0'")
                            Dsql.Append(" AND HAISIN_FLG_S ='0'")
                            Dsql.Append(" AND SAIFURI_FLG_S ='0'")
                            Dsql.Append(" AND SOUSIN_FLG_S ='0'")
                            Dsql.Append(" AND FUNOU_FLG_S ='0'")
                            Dsql.Append(" AND TESUUKEI_FLG_S ='0'")
                            Dsql.Append(" AND TESUUTYO_FLG_S ='0'")
                            Dsql.Append(" AND KESSAI_FLG_S ='0'")
                            Dsql.Append(" AND HENKAN_FLG_S ='0'")
                            Dsql.Append(" AND TYUUDAN_FLG_S ='0'")
                            Dsql.Append(" AND TAKOU_FLG_S ='0'")
                            Dsql.Append(" AND NIPPO_FLG_S ='0'")

                            SQLCode = 0
                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)

                            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
                            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                                If Ret = 1 AndAlso SQLCode = 0 Then
                                    Dsql.Length = 0
                                    Dsql.Append("DELETE FROM SCHMAST_SUB")
                                    Dsql.Append(" WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'")
                                    Dsql.Append(" AND TORIS_CODE_SSUB   = '" & TR(0).TORIS_CODE & "'")
                                    Dsql.Append(" AND TORIF_CODE_SSUB   = '" & TR(0).SFURI_FCODE & "'") '�ĐU���R�[�h
                                    Dsql.Append(" AND FURI_DATE_SSUB    = '" & SCH.WRK_SFURI_YDATE & "'")

                                    SQLCode = 0
                                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)

                                End If
                            End If
                            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

                        Case Is = APL.SofuriApplication

                    End Select

                End If
            End If
            '**************************************

            If cFlg = False Then Return True

            If Ret = 1 AndAlso SQLCode = 0 Then
                'Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, "COMMIT WORK")
                Return True
            End If

        Catch ex As Exception

            Throw

        End Try
        Return False
    End Function

    Function SEARCH_NEW_SCHMAST(ByVal TorisCode As String, ByVal TorifCode As String, ByVal NewFuriDate As String) As Boolean
        '============================================================================
        'NAME           :fn_NEW_SCHMAST_KAKUNINN
        'Parameter      :astrTORIS_CODE�F������R�[�h�^astrTORIF_CODE�F����敛�R�[�h�^
        '               :astrNEW_FURI_DATE�F�V�U�֓�
        'Description    :�V�X�P�W���[�������݂��Ȃ����Ƃ��m�F
        'Return         :True=OK(���݂���),False=NG�i���݂��Ȃ��j
        'Create         :2007/05/28
        'Update         :
        '============================================================================

        Try
            Dim Ret As Integer = 0
            Dim SQL As String = ""
            Dim BRet As Boolean
            Dim REC As OracleDataReader = Nothing

            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "SELECT COUNT(*) AS COUNTER FROM SCHMAST"
                Case Is = APL.SofuriApplication
                    SQL = "SELECT COUNT(*) AS COUNTER FROM S_SCHMAST"
            End Select

            sql = sql & " WHERE "
            sql = sql & "TORIS_CODE_S = '" & Trim(TorisCode) & "' AND "
            sql = sql & "TORIF_CODE_S = '" & Trim(TorifCode) & "' AND "
            sql = sql & "FURI_DATE_S = '" & Trim(NewFuriDate) & "'"

            BRet = GCom.SetDynaset(sql, REC)
            If BRet Then
                Do While REC.Read
                    Dim CNT As Integer = GCom.NzInt(REC.Item("COUNTER"))
                    If CNT = 0 Then
                        Return True
                    Else
                        Return False
                    End If
                Loop
            End If
        Catch ex As Exception
            Throw
        End Try
    End Function



    '
    ' �@�\�@ �F ����SCHMAST�`�F�b�N
    '
    ' �����@ �F ARG1 - �����z��ʒu
    '
    ' �߂�l �F True = ������, False = ������
    '
    ' ���l�@ �F SCHMAST�ɃX�P�W���[�������݂��A�������ł��邩����
    '
    Public Function CHECK_SELECT_SCHMAST(ByVal Index As Integer) As Boolean
        With GCom.GLog
            .Job2 = "����SCHMAST�`�F�b�N"
        End With
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = "SELECT TORIS_CODE_S"   '�L���t���O
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " FROM SCHMAST"
                Case Is = APL.SofuriApplication
                    SQL &= " FROM S_SCHMAST"
            End Select
            SQL &= " WHERE FSYORI_KBN_S = '" & TR(Index).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_S = '" & TR(Index).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_S = '" & TR(Index).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_S = '" & SCH.FURI_DATE & "'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                Case Is = APL.SofuriApplication
                    SQL &= " AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ
            End Select

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet AndAlso REC.Read Then

                '�L���t���O���X�V����B
                'If GCom.NzInt(REC.Item("YUUKOU_FLG_S"), 0) = 0 Then

                '    Select Case SchTable
                '        Case Is = APL.JifuriApplication
                '            SQL = "UPDATE SCHMAST"
                '        Case Is = APL.SofuriApplication
                '            SQL = "UPDATE K_SCHMAST"
                '    End Select
                '    SQL &= " SET YUUKOU_FLG_S = '1'"
                '    SQL &= " WHERE FSYORI_KBN_S = '" & TR(Index).FSYORI_KBN & "'"
                '    SQL &= " AND TORIS_CODE_S = '" & TR(Index).TORIS_CODE & "'"
                '    SQL &= " AND TORIF_CODE_S = '" & TR(Index).TORIF_CODE & "'"
                '    SQL &= " AND FURI_DATE_S = '" & SCH.FURI_DATE & "'"
                '    Select Case SchTable
                '        Case Is = APL.JifuriApplication
                '        Case Is = APL.SofuriApplication
                '            SQL &= " AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ
                '    End Select

                '    Dim SQLCode As Integer = 0
                '    Dim Ret As Integer = GCom.DBExecuteProcess(GCom.enDB.DB_Execute, SQL, SQLCode)
                '    If Ret = 1 AndAlso SQLCode = 0 Then
                '    Else
                '        Throw New Exception("�X�P�W���[���}�X�^�L���t���O�X�V�G���[")
                '    End If
                'End If

                Return False
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return True
    End Function

    '
    ' �@�\�@ �F ���ԃX�P�W���[���쐬�������A�����}�X�^�� KOUSIN_SIKIBETU_T ��"2"�𗧂Ă�(�����t���O)
    '
    ' �����@ �F ARG1 - �������z��ʒu
    '
    ' �߂�l �F True = ������, False = ������
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function UPDATE_TORIMAST(ByVal Index As Integer) As Boolean
        'With GCom.GLog
        '    .Job2 = "���ԃX�P�W���[���쐬�����t���O�X�V"
        'End With
        Try
            Dim SQL As String = ""
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "UPDATE TORIMAST"
                Case Is = APL.SofuriApplication
                    SQL = "UPDATE K_TORIMAST"
            End Select
            SQL &= " SET KOUSIN_SIKIBETU_T = '2'"
            SQL &= " WHERE FSYORI_KBN_T = '" & TR(Index).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_T = '" & TR(Index).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_T = '" & TR(Index).TORIF_CODE & "'"

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, False)

            Return (Ret = 1)
        Catch ex As Exception

            Throw

        End Try

        Return False

    End Function

    '
    ' �@�\�@ �F �x���\���̈�̕`��
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Sub Set_Kyuuzitu_Monitor_Area(ByVal ListView1 As ListView)
        'With GCom.GLog
        '    .Job2 = "�x���\���̈�`��"
        'End With
        Dim REC As OracleDataReader = Nothing
        Try
            With ListView1
                .Clear()
                .Columns.Add("�a��", 210, HorizontalAlignment.Center)
                .Columns.Add("����", 100, HorizontalAlignment.Center)
                .Columns.Add(" ����", 175, HorizontalAlignment.Left)
                .Columns.Add("�쐬��", 95, HorizontalAlignment.Center)
                .Columns.Add("�X�V��", 95, HorizontalAlignment.Center)
            End With

            Dim SQL As String = "SELECT YASUMI_DATE_Y"
            SQL &= ", YASUMI_NAME_Y"
            SQL &= ", SAKUSEI_DATE_Y"
            SQL &= ", KOUSIN_DATE_Y"
            SQL &= " FROM YASUMIMAST"
            SQL &= "  ORDER BY YASUMI_DATE_Y ASC"

            Dim Ret As Boolean = GCom.SetDynaset(SQL, REC)
            If Ret Then
                Dim ROW As Integer = 0

                Do While REC.Read

                    Dim Data(4) As String

                    Dim Temp As String = String.Format("{0:00000000}", GCom.NzDec(REC.Item("YASUMI_DATE_Y"), 0))
                    Dim onText(2) As Integer
                    onText(0) = CType(Temp.Substring(0, 4), Integer)
                    onText(1) = CType(Temp.Substring(4, 2), Integer)
                    onText(2) = CType(Temp.Substring(6), Integer)
                    Dim onDate As Date
                    Call GCom.ChangeDate(onText, Temp, 1, onDate)
                    If Not onDate = MenteCommon.clsCommon.BadResultDate Then

                        '�a��
                        Data(0) = Temp

                        '����
                        Data(1) = String.Format("{0:yyyy-MM-dd}", onDate)

                        '�x������
                        Data(2) = " " & GCom.NzStr(REC.Item("YASUMI_NAME_Y")).Trim

                        '�쐬��
                        Data(3) = String.Format("{0:0000-00-00}", GCom.NzDec(REC.Item("SAKUSEI_DATE_Y"), 0))

                        '�X�V��
                        If GCom.NzDec(REC.Item("KOUSIN_DATE_Y")) > 0 Then
                            Data(4) = String.Format("{0:0000-00-00}", GCom.NzDec(REC.Item("KOUSIN_DATE_Y"), 0))
                        End If
                    End If

                    Dim LineColor As Color
                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                Loop

                If ListView1.Items.Count = 0 Then
                    Dim MSG As String = MSG0003E
                    Call MessageBox.Show(MSG, GCom.GLog.Job1, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Sub

    '
    ' �@�\�@ �F �X�P�W���[���}�X�^�̎Q��
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F �J�[�\���L��
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function READ_SCHMAST() As Boolean
        'With GCom.GLog
        '    .Job2 = "�X�P�W���[���}�X�^����"
        'End With
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = ""
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "SELECT * FROM SCHMAST"
                Case Is = APL.SofuriApplication
                    SQL = "SELECT * FROM S_SCHMAST"
            End Select
            SQL &= " WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_S = '" & SCH.FURI_DATE & "'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                Case Is = APL.SofuriApplication
                    SQL &= " AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ
            End Select

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet AndAlso REC.Read Then

                Array.Clear(SCHMAST, 0, SCHMAST.Length)

                For Index As Integer = 0 To MaxColumn Step 1
                    Select Case ORATYPE(Index)
                        Case "NUMBER"
                            SCHMAST(Index) = GCom.NzDec(REC.Item(Index), "")
                        Case Else
                            SCHMAST(Index) = GCom.NzStr(REC.Item(Index))
                    End Select
                Next Index

                Return True
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function

    '2017/12/05 saitou �L���M��(RSV2�W����) ADD ��K�͍\�z�Ή� ---------------------------------------- START
    ''' <summary>
    ''' �X�P�W���[���}�X�^�T�u�̎Q��
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function READ_SCHMAST_SUB() As Boolean
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = ""
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL = "SELECT * FROM SCHMAST_SUB"
                Case Is = APL.SofuriApplication
                    SQL = "SELECT * FROM S_SCHMAST_SUB"
            End Select
            SQL &= " WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_SSUB = '" & TR(0).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_SSUB = '" & TR(0).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_SSUB = '" & SCH.FURI_DATE & "'"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                Case Is = APL.SofuriApplication
                    SQL &= " AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ
            End Select

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet AndAlso REC.Read Then

                Array.Clear(SCHMAST_SUB, 0, SCHMAST_SUB.Length)

                For Index As Integer = 0 To MaxColumn_Sub Step 1
                    Select Case ORATYPE_SUB(Index)
                        Case "NUMBER"
                            SCHMAST_SUB(Index) = GCom.NzDec(REC.Item(Index), "")
                        Case Else
                            SCHMAST_SUB(Index) = GCom.NzStr(REC.Item(Index))
                    End Select
                Next Index

                Return True
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function
    '2017/12/05 saitou �L���M��(RSV2�W����) ADD ------------------------------------------------------- END

    '
    ' �@�\�@ �F ���s�X�P�W���[���}�X�^���݃`�F�b�N
    '
    ' �����@ �F ARG1 - �����}�X�^�z��ʒu
    '
    ' �߂�l �F ���݂��� = True, ���݂��Ȃ� = False
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function CHECK_TAKOSCHMAST(ByVal Index As Integer) As Boolean
        'With GCom.GLog
        '    .Job2 = "���s�X�P�W���[���}�X�^�L������"
        'End With
        Dim Temp As String = ""
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = "SELECT COUNT(*) COUNTER"
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " FROM TAKOSCHMAST"
                Case Is = APL.SofuriApplication
                    SQL &= " FROM K_TAKOSCHMAST"
            End Select
            SQL &= " WHERE TORIS_CODE_U = '" & TR(Index).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_U = '" & TR(Index).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_U = '" & SCH.FURI_DATE & "'"

            Dim Ret As Boolean = GCom.SetDynaset(SQL, REC)
            If Ret AndAlso REC.Read Then

                Return (GCom.NzInt(REC.Item("COUNTER"), 0) > 0)
            End If
        Catch ex As Exception

            Throw
 
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function

    '
    ' �@�\�@ �F �}�̑��t����уe�[�u���̏ƍ��t���O�� 0 �ɂ���B
    '
    ' �����@ �F ARG1 - �����敪(���U=1,�U��=3)
    ' �@�@�@ �@ ARG2 - ������R�[�h
    ' �@�@�@ �@ ARG3 - ����敛�R�[�h
    ' �@�@�@ �@ ARG4 - �U�֓�
    ' �@�@�@ �@ ARG5 - ����SEQ(���U�͏��=0)
    '
    ' �߂�l �F ����I�� = True, �ُ�I�� = False
    '
    ' ���l�@ �F �ėp�I�Ɏg�p�ł���悤�ɂ���B(������SCHMAST�̒l)
    ' �@�@�@ �@ �ƍ��ς݂̏ꍇ�ɂ̂ݏ���������B
    '
    'Public Function SET_MEDIA_ENTRY_TBL_CHECK_FLG(ByVal FSYORI_KBN As String, ByVal TORIS_CODE As String, _
    '    ByVal TORIF_CODE As String, ByVal FURI_DATE As String, ByVal MOTIKOMI_SEQ As Integer) As Boolean
    '    Try
    '        Dim SQL As String = "UPDATE MEDIA_ENTRY_TBL"
    '        SQL &= " SET CHECK_FLG = 0"
    '        SQL &= ", UPDATE_OP = '" & GCom.GetUserID & "'"
    '        SQL &= ", UPDATE_DATE = TO_CHAR(SYSDATE, 'yyyymmddHH24MIss')"
    '        SQL &= " WHERE FSYORI_KBN = '" & FSYORI_KBN & "'"
    '        SQL &= " AND TORIS_CODE = '" & TORIS_CODE & "'"
    '        SQL &= " AND TORIF_CODE = '" & TORIF_CODE & "'"
    '        SQL &= " AND FURI_DATE = '" & FURI_DATE & "'"
    '        SQL &= " AND NVL(CYCLE_NO, 0) = " & MOTIKOMI_SEQ.ToString
    '        SQL &= " AND NOT NVL(CHECK_FLG, 0) = 0"

    '        Dim SQLCode As Integer = 0
    '        Dim Ret As Integer = GCom.DBExecuteProcess(GCom.enDB.DB_Execute, SQL, SQLCode)

    '        Return (SQLCode = 0)
    '    Catch ex As Exception

    '        Throw

    '    End Try
    '    Return False
    'End Function

    '
    ' �@�\�@�@�@: �e�L�X�g�{�b�N�X�œ��͂��ꂽ���t�f�[�^��]������
    '
    ' �߂�l�@�@: ������t = OK(-1)
    ' �@�@�@�@�@  �ُ���t = ������ӏ� (0=�N, 1=��, 2=��)
    '
    ' �������@�@: ARG1(onDate) - ���t
    ' �@�@�@�@�@  ARG2(onData) - �N�������(�z��) (0=�N, 1=��, 2=��)
    '
    ' �@�\�����@: ���t�`�F�b�N�֐�
    '
    ' ���l�@�@�@: ���ʊ֐��ł͎g���ɂ����ׁA�኱�������Đ�p�֐��Ƃ���B
    '
    Private Function SET_DATE(ByRef onDate As Date, ByRef onData() As Integer) As Integer
        Try
            onDate = DateSerial(onData(0), onData(1), onData(2))

            Select Case False
                Case onDate.Year = onData(0)
                    Return 0
                Case onDate.Month = onData(1)
                    Return 1
                Case onDate.Day = onData(2)
                    Return 2
            End Select

            Return -1
        Catch ex As Exception

            Throw

        End Try

        Return 9
    End Function


    Public Function GetJobCount() As Integer
        Dim Cnt As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String
            SQL = "SELECT COUNT(*) FROM JOBMAST"
            SQL &= " WHERE TOUROKU_DATE_J = '" & Now.ToString("yyyyMMdd") & "'"
            SQL &= " AND STATUS_J IN ('0','1')"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet AndAlso REC.Read Then
                Cnt = REC.GetInt32(0)
            End If

        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Cnt
    End Function

End Class
