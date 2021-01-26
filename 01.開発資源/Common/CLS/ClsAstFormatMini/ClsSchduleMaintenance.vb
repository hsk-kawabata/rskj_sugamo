Option Explicit On 
Option Strict On

Imports System.Data.OracleClient
Imports System.Text
Imports CASTCommon

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
        Dim FSYORI_KBN As String                '�U�������敪
        Dim TORIS_CODE As String                '������R�[�h
        Dim TORIF_CODE As String                '����敛�R�[�h
        Dim BAITAI_CODE As String               '�}�̃R�[�h
        Dim ITAKU_KANRI_CODE As String          '��\�ϑ��҃R�[�h
        Dim FMT_KBN As Integer                  '�t�H�[�}�b�g�敪
        Dim SYUBETU As String                   '���
        Dim ITAKU_CODE As String                '�ϑ��҃R�[�h
        Dim ITAKU_KNAME As String               '�ϑ��҃J�i��
        Dim TKIN_NO As String                   '�戵���Z�@��
        Dim TSIT_NO As String                   '�戵�X��
        Dim SOUSIN_KBN As String                '���M�敪
        Dim CYCLE As Integer                    '�T�C�N���Ǘ�
        Dim KIJITU_KANRI As Integer             '�����Ǘ��v��
        Dim FURI_CODE As String                 '�U�փR�[�h
        Dim KIGYO_CODE As String                '��ƃR�[�h
        Dim ITAKU_NNAME As String               '�ϑ��Ҋ�����
        Dim FURI_KYU_CODE As Integer            '�U���x���R�[�h
        Dim DATEN() As Integer                  'N���̗L���^����
        Dim MONTH_FLG As Integer                '�Y�����̒l�H
        Dim KEIYAKU_DATE As String              '�_���
        Dim MOTIKOMI_KIJITSU As Integer         '��������
        Dim IRAISHO_YDATE As Integer            '�����^����i�˗����j
        Dim IRAISHO_KIJITSU As String           '���t�敪�i�˗����j
        Dim IRAISHO_KYU_CODE As String          '�˗����x���V�t�g
        Dim KESSAI_KBN As Integer               '���ϋ敪
        Dim KESSAI_DAY As Integer               '�����^����i���ρj
        Dim KESSAI_KIJITSU As Integer           '���t�敪�i���ρj
        Dim KESSAI_KYU_CODE As Integer          '���ϓ��x���V�t�g
        Dim TESUUTYO_KBN As Integer             '�萔�������敪
        Dim TESUUTYO_PATN As Integer            '�萔���������@
        Dim TESUUMAT_NO As Integer              '�萔���W�v����
        Dim TESUUTYO_DAY As Integer             '�萔�����������^���
        Dim TESUUTYO_KIJITSU As Integer         '�萔�����������敪
        Dim TESUU_KYU_CODE As Integer           '�萔���������x���R�[�h
        Dim TESUUMAT_MONTH As Integer           '�W�v���
        Dim TESUUMAT_ENDDAY As Integer          '�W�v�I����
        Dim TESUUMAT_KIJYUN As Integer          '�W�v�

    End Structure
    Public TR() As TORIMAST_RECORD

    '�o�^�^���茋�ʏ��
    Public Structure SCHMAST_Data
        Dim KFURI_DATE As String            '�_��U����
        Dim FURI_DATE As String             '�U����
        Dim NFURI_DATE As String            '�ύX�U����
        Dim MOTIKOMI_SEQ As Integer         '����SEQ
        Dim IRAISYOK_YDATE As String        '�˗�������\���
        Dim KAKUHO_YDATE As String          '�����m�ۗ\���
        Dim HASSIN_YDATE As String          '���M�����\���
        Dim KESSAI_YDATE As String          '���ϗ\���
        Dim TESUU_YDATE As String           '�萔�������\���
        Dim MOTIKOMI_DATE As String         '��������
    End Structure
    Public SCH As SCHMAST_Data

    Public Enum OPT
        OptionNothing = 0               '�ʓo�^���
        OptionAddNew = 1                '�V�K�E�č쐬
        OptionAppend = 2                '�ǉ��쐬
    End Enum

    '�ǂ̋Ɩ�����Ăяo����邩�̎��ʕϐ�
    Private SchTable As Integer = 0

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
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT COLUMN_NAME")
            SQL.AppendLine(", DATA_TYPE")
            SQL.AppendLine(", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE")
            SQL.AppendLine(" FROM ALL_TAB_COLUMNS")
            SQL.AppendLine(" WHERE UPPER(OWNER) = '" & CASTCommon.DB.USER & "'")
            SQL.AppendLine(" AND TABLE_NAME = 'S_SCHMAST'")
            SQL.AppendLine(" ORDER BY COLUMN_ID ASC")

            If GCom.SetDynaset(SQL.ToString, REC) Then
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
                SQL.Length = 0
                SQL.AppendLine("SELECT COLUMN_NAME")
                SQL.AppendLine(", DATA_TYPE")
                SQL.AppendLine(", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE")
                SQL.AppendLine(" FROM ALL_TAB_COLUMNS")
                SQL.AppendLine(" WHERE UPPER(OWNER) = '" & CASTCommon.DB.USER & "'")
                SQL.AppendLine(" AND TABLE_NAME = 'S_SCHMAST_SUB'")
                SQL.AppendLine(" ORDER BY COLUMN_ID ASC")
                If GCom.SetDynaset(SQL.ToString, REC) Then
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
    ' �����@ �F ARG1 - �U�����f�[�^(��ʎw��)
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F ����O�ɒ~�ς��邱��
    '
    Public Sub SetKyuzituInformation(Optional ByVal FormFuriDate As Date = MenteCommon.clsCommon.BadResultDate)
        Try
            Select Case FormFuriDate
                Case MenteCommon.clsCommon.BadResultDate
                    '�S�x������~�ς���B
                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1)
                Case Else
                    '�Y�����O��̋x�����̂�����~�ς���B
                    Dim ProviousMonthDate As Date = FormFuriDate.AddMonths(-1)
                    Dim NextMonthDate As Date = FormFuriDate.AddMonths(1)

                    Dim SQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)" & _
                                        " IN ('" & String.Format("{0:yyyyMM}", ProviousMonthDate) & "'" & _
                                        ", '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'" & _
                                        ", '" & String.Format("{0:yyyyMM}", NextMonthDate) & "')"

                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1, SQL)
            End Select
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' �@�\�@ �F �X�P�W���[���쐬�Ώۂ̎����R�[�h�𒊏o
    '
    ' �����@ �F ARG1 - �U�����N��
    ' �@�@�@ �@ ARG2 - ������R�[�h
    ' �@�@�@ �@ ARG3 - ����敛�R�[�h
    ' �@�@�@ �@ ARG4 - �ďo�����ʎw��
    '
    ' �߂�l �F OK = True, NG = False(�Ώێ����Ȃ�)
    '
    ' ���l�@ �F �Y���̌����Ώۂ̂��̂ŐU�������J�n�^�I���̊Ԃɂ������
    '
    Public Function GET_SELECT_TORIMAST(ByVal FormFuriDate As Date, _
            ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, _
            Optional ByVal SEL_OPTION As Integer = OPT.OptionNothing) As Boolean

        Dim REC As OracleDataReader = Nothing
        Try
            '�Y����
            Dim MonthColumn As String = "TUKI" & FormFuriDate.Month.ToString & "_T"

            Dim SQL As StringBuilder = New StringBuilder(SetToriMastSelectBaseSql(MonthColumn))
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '3'")

            Select Case SEL_OPTION
                Case OPT.OptionNothing
                    '�ʓo�^���
                    SQL.AppendLine(" AND TORIS_CODE_T = '" & TORIS_CODE & "'")
                    SQL.AppendLine(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")
                Case OPT.OptionAddNew, OPT.OptionAppend
                    '���ԃX�P�W���[���쐬
                    SQL.AppendLine(" AND NOT " & MonthColumn & " = '0'")
                    SQL.AppendLine(" AND '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'")
                    SQL.AppendLine(" BETWEEN SUBSTR(KAISI_DATE_T, 1, 6)")
                    SQL.AppendLine(" AND SUBSTR(SYURYOU_DATE_T, 1, 6)")
                    SQL.AppendLine(" AND KIJITU_KANRI_T = '1'")

                    '�ǉ��̏ꍇ�ɂ͎����}�X�^�̕ύX������������
                    If SEL_OPTION = OPT.OptionAppend Then
                        SQL.AppendLine(" AND NOT KOUSIN_SIKIBETU_T = '2'")
                    End If
            End Select

            If GCom.SetDynaset(SQL.ToString, REC) Then

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
        Try
            Dim SQL As StringBuilder = New StringBuilder(128)
            SQL.AppendLine("SELECT FSYORI_KBN_T")               '�U�������敪
            SQL.AppendLine(", TORIS_CODE_T")                    '������R�[�h
            SQL.AppendLine(", TORIF_CODE_T")                    '����敛�R�[�h
            SQL.Append(", " & MonthColumn & " MONTH")           '�ݒ茎
            For Index As Integer = 1 To 31 Step 1
                SQL.Append(", DATE" & Index.ToString & "_T")
            Next Index
            SQL.AppendLine()
            SQL.AppendLine(", SOUSIN_KBN_T")                    '���M�敪
            SQL.AppendLine(", FURI_KYU_CODE_T")                 '�x���R�[�h
            SQL.AppendLine(", FMT_KBN_T")                       '�t�H�[�}�b�g�敪
            SQL.AppendLine(", TESUUTYO_KIJITSU_T")              '�萔�����������敪
            SQL.AppendLine(", TESUUTYO_KBN_T")                  '�萔�������敪
            SQL.AppendLine(", TESUUTYO_DAY_T")                  '�萔�����������^���
            SQL.AppendLine(", TESUU_KYU_CODE_T")                '�萔���������x���R�[�h
            SQL.AppendLine(", KESSAI_KIJITSU_T")                '���ϓ��w��敪
            SQL.AppendLine(", KESSAI_DAY_T")                    '���ϓ������^���
            SQL.AppendLine(", KESSAI_KYU_CODE_T")               '���ϋx���R�[�h
            SQL.AppendLine(", CYCLE_T")                         '�T�C�N���Ǘ�
            SQL.AppendLine(", KIJITU_KANRI_T")                  '�����Ǘ��v��
            SQL.AppendLine(", FURI_CODE_T")                     '�U�փR�[�h
            SQL.AppendLine(", KIGYO_CODE_T")                    '��ƃR�[�h
            SQL.AppendLine(", ITAKU_CODE_T")                    '�ϑ��҃R�[�h
            SQL.AppendLine(", TKIN_NO_T")                       '�戵���Z�@��
            SQL.AppendLine(", TSIT_NO_T")                       '�戵�X��
            SQL.AppendLine(", BAITAI_CODE_T")                   '�}�̃R�[�h
            SQL.AppendLine(", SYUBETU_T")                       '��ʃR�[�h
            SQL.AppendLine(", ITAKU_KNAME_T")                   '�ϑ��҃J�i��
            SQL.AppendLine(", ITAKU_NNAME_T")                   '�ϑ��Ҋ�����
            SQL.AppendLine(", MOTIKOMI_KIJITSU_T")              '��������
            SQL.AppendLine(", TESUUMAT_NO_T")                   '�萔���W�v����
            SQL.AppendLine(", TESUUMAT_MONTH_T")                '�W�v���
            SQL.AppendLine(", TESUUMAT_ENDDAY_T")               '�W�v�I����
            SQL.AppendLine(", TESUUMAT_KIJYUN_T")               '�W�v�
            SQL.AppendLine(", TESUUTYO_PATN_T")                 '�萔���������@
            SQL.AppendLine(", KESSAI_KBN_T")                    '���ϋ敪
            SQL.AppendLine(" FROM S_TORIMAST")

            Return SQL.ToString

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
        Try
            Dim Temp As String = ""

            With TR(Counter)
                .FSYORI_KBN = GCom.NzDec(REC.Item("FSYORI_KBN_T"), "")              '�U�������敪
                .TORIS_CODE = GCom.NzStr(REC.Item("TORIS_CODE_T"))              '������R�[�h
                .TORIF_CODE = GCom.NzDec(REC.Item("TORIF_CODE_T"), "")              '����敛�R�[�h
                .ITAKU_CODE = GCom.NzDec(REC.Item("ITAKU_CODE_T"), "")              '�ϑ��҃R�[�h
                .ITAKU_KNAME = GCom.NzStr(REC.Item("ITAKU_KNAME_T"))                '�ϑ��҃J�i��
                .ITAKU_NNAME = GCom.NzStr(REC.Item("ITAKU_NNAME_T"))                '�ϑ��Ҋ�����
                .MONTH_FLG = GCom.NzInt(REC.Item("MONTH"), 0)                       '�Y���w�茎�̒l

                '�U���Y�����̗L���^����
                ReDim TR(Counter).DATEN(31)
                For Index As Integer = 1 To 31 Step 1
                    Temp = "DATE" & Index.ToString & "_T"
                    TR(Counter).DATEN(Index) = GCom.NzInt(REC.Item(Temp), 0)
                Next Index

                .SOUSIN_KBN = GCom.NzDec(REC.Item("SOUSIN_KBN_T"), "")              '���M�敪
                .FURI_KYU_CODE = GCom.NzInt(REC.Item("FURI_KYU_CODE_T"), 0)         '�x���R�[�h
                .FMT_KBN = GCom.NzInt(REC.Item("FMT_KBN_T"), 0)                     '�t�H�[�}�b�g�敪
                .CYCLE = GCom.NzInt(REC.Item("CYCLE_T"), 0)                         '�T�C�N���Ǘ�
                .KIJITU_KANRI = GCom.NzInt(REC.Item("KIJITU_KANRI_T"), 0)           '�����Ǘ��v��
                .FURI_CODE = GCom.NzDec(REC.Item("FURI_CODE_T"), "")                '�U�փR�[�h
                .KIGYO_CODE = GCom.NzDec(REC.Item("KIGYO_CODE_T"), "")              '��ƃR�[�h
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
                .TESUUMAT_NO = GCom.NzInt(REC.Item("TESUUMAT_NO_T"), 0)             '�萔���W�v����
                If .TESUUMAT_NO > 12 Then
                    .TESUUMAT_NO = 12
                End If
                .TESUUMAT_MONTH = GCom.NzInt(REC.Item("TESUUMAT_MONTH_T"), 0)       '�W�v���
                .TESUUMAT_ENDDAY = GCom.NzInt(REC.Item("TESUUMAT_ENDDAY_T"), 0)     '�W�v�I����
                .TESUUMAT_KIJYUN = GCom.NzInt(REC.Item("TESUUMAT_KIJYUN_T"), 0)     '�W�v�
                .TESUUTYO_PATN = GCom.NzInt(REC.Item("TESUUTYO_PATN_T"), 0)         '�萔���������@
                .KESSAI_KBN = GCom.NzInt(REC.Item("KESSAI_KBN_T"), 0)               '���ϋ敪

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
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim SQL As StringBuilder = New StringBuilder(512)
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

            Dim KYUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
            Dim SOUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)
            Dim HASSHIN As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "HASSIN"), 0)

            '�����m�ۗ\���(��̌��ϗ\����Z�o�����ŏ���������)
            Select Case TR(Index).SYUBETU
                Case Is = "11"
                    '���^�U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, KYUFURI, 1)
                Case Is = "12"
                    '�ܗ^�U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, KYUFURI, 1)
                Case Is = "21"
                    '�����U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, SOUFURI, 1)
                Case Else
            End Select

            '���M�\���
            Select Case TR(Index).SYUBETU
                Case Is = "11"
                    '���^�U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Is = "12"
                    '�ܗ^�U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Is = "21"
                    '�����U��
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Else
            End Select

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

            If TR(Index).KESSAI_DAY = 0 Then
                TR(Index).KESSAI_DAY = 1
            End If

            Select Case TR(Index).KESSAI_KIJITSU
                Case 0
                    '�c�Ɠ����w��
                    Dim FrontBackType As Integer = 0
                    FrontBackType = 1
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KESSAI_YDATE, TR(Index).KESSAI_DAY, FrontBackType)
                Case 1
                    '����w��(�����␳����)
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

                    '�U��������O�̏ꍇ�ɂ͗����֍Čv�Z
                    If SCH.FURI_DATE > SCH.KESSAI_YDATE Then
                        '2010.03.01 �_��U�����ɕύX start
                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        '2010.03.01 �_��U�����ɕύX end
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

            SCH.KAKUHO_YDATE = SCH.KESSAI_YDATE
            SCH.KESSAI_YDATE = SCH.FURI_DATE
            If SCH.HASSIN_YDATE < SCH.KAKUHO_YDATE Then
                SCH.HASSIN_YDATE = SCH.KAKUHO_YDATE
            End If

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
            'TESUUMAT_KIJYUN2   '�W�v�       (2008.03.06) (0:�U����, 1:���ϓ�)
            'TESUUTYO_PATN      '�萔���������@ (2008.03.06) (0:��������, 1:���ړ���)
            'KESSAI_KBN         '���ϋ敪       (2008.03.06) 
            '���ϋ敪 = (0:�a����, 1:��������, 2:�ב֐U��, 3:�ב֕t��, 4:���ʊ��, 5:���ϑΏۊO)
            '2008.03.06 FJH �d�l
            '�W�v�J�n�����(TESUUMAT_KIJYUN)�Ŋς�B
            '�W�v�ɓ���邩�ۂ���(TESUUMAT_KIJYUN2)�Ŕ��f�B�U�����͌_����ŁA���ϓ��͌v�Z�\����ōs���B
            '�W�v�ɓ���邩�ۂ��̊��(TESUUMAT_END)�ōs���B

            If TR(Index).TESUUTYO_DAY = 0 Then
                TR(Index).TESUUTYO_DAY = 1
            End If

            Select Case TR(Index).TESUUTYO_KBN

                Case 1

                    '�ꊇ����(���������͐�΂ɂȂ��B���d�l)

                    '�萔����������z�񉻂���
                    Dim ALL_Month(GCom.NzInt(12 \ TR(Index).TESUUMAT_NO)) As Integer

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
                            '�U�����̏ꍇ
                            onText(0) = GCom.NzInt(SCH.FURI_DATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.FURI_DATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.FURI_DATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                        Case Else
                            '���ϓ��̏ꍇ
                            onText(0) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                    End Select
                    Dim memMonth As Integer = onText(1)

                    '����萔�������������߂�
                    For Cnt = 1 To ALL_Month.GetUpperBound(0) Step 1

                        If onText(1) <= ALL_Month(Cnt) Then

                            onText(1) = ALL_Month(Cnt)
                            Exit For
                        End If
                    Next

                    '�萔���������z����̌��l���U�����l��ǂ��z���Ȃ������ꍇ
                    If Cnt > ALL_Month.GetUpperBound(0) Then
                        '���N�x�̍ŏ��̒�������ݒ肷��
                        onText(0) += 1
                        onText(1) = ALL_Month(1)
                    Else
                        '�W�v������ϓ��̏ꍇ���l��
                        If (TR(Index).TESUUMAT_KIJYUN = 0 AndAlso GCom.NzInt(SCH.KFURI_DATE.Substring(4, 2), 0) = memMonth) OrElse _
                           (TR(Index).TESUUMAT_KIJYUN <> 0 AndAlso GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0) = memMonth) Then
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

                    '�萔���������̐U�������Z�o����B
                    Dim Temp_FURI_DATE As String = ""

                    '�ꊇ�����E�ʓr�������̒����\����v�Z
                    Select Case TR(Index).TESUUTYO_KBN
                        Case 1, 3
                            Temp = Temp.Substring(0, 6) & "15"
                    End Select

                    Select Case TR(Index).FURI_KYU_CODE
                        Case 0, 1
                            '�U����,�c�Ɠ�����(�y�E���E�j�Փ�����)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, TR(Index).FURI_KYU_CODE)
                        Case Else
                            '�U����,�c�Ɠ�����(�y�E���E�j�Փ�����)
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
                            SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
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

                                    '�U��������O�̏ꍇ�ɂ͗����֍Čv�Z
                                    If SCH.FURI_DATE > SCH.TESUU_YDATE Then
                                        '2010.03.01 �_��U�����ɕύX start
                                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                        '2010.03.01 �_��U�����ɕύX end
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
                                    SCH.TESUU_YDATE = SCH.KAKUHO_YDATE

                            End Select

                            '�萔�������\������m�ۓ������O�̏ꍇ
                            If SCH.TESUU_YDATE < SCH.KAKUHO_YDATE Then
                                SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
                            End If

                            '�萔�������敪���u�m�ۓ��w��v�̏ꍇ�A�萔�������\����������m�ۗ\����Ɠ����ɂ��܂��B
                            If TR(Index).TESUUTYO_KIJITSU = 4 Then
                                SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
                            End If

                    End Select
            End Select

            '�v�Z���W�b�N�݂̂Ŋ֐��𔲂���ꍇ(2008.03.18 By Astar)
            If CALC_ONLY Then
                Return True
            End If

            '------------------------------------------------------------------
            '�}�X�^�o�^���ڐݒ�(SQL��)
            '------------------------------------------------------------------
            SQL.AppendLine("INSERT INTO S_SCHMAST")
            
            For ColumnID1 = 0 To MaxColumn Step 1
                Select Case ColumnID1
                    Case 0
                        SQL.Append(" (" & ORANAME(ColumnID1))
                    Case Else
                        SQL.AppendLine(", " & ORANAME(ColumnID1))
                End Select
            Next ColumnID1
            SQL.Append(") VALUES")

            For ColumnID2 = 0 To MaxColumn Step 1

                If ColumnID2 = 0 Then
                    SQL.AppendLine(" (")
                Else
                    SQL.Append(", ")
                End If

                Select Case ORANAME(ColumnID2)
                    Case "FSYORI_KBN_S"
                        '�U�������敪1
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).FSYORI_KBN))
                    Case "TORIS_CODE_S"
                        '������R�[�h2
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TORIS_CODE))
                    Case "TORIF_CODE_S"
                        '����敛�R�[�h3
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TORIF_CODE))
                    Case "FURI_DATE_S"
                        '�U����4
                        SQL.AppendLine(SetItem(ColumnID2, SCH.FURI_DATE))
                    Case "KFURI_DATE_S"
                        '�_��U����5
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KFURI_DATE))
                    Case "SYUBETU_S"
                        '���6
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).SYUBETU))
                    Case "FURI_CODE_S"
                        '�U�փR�[�h7
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).FURI_CODE))
                    Case "KIGYO_CODE_S"
                        '��ƃR�[�h8
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).KIGYO_CODE))
                    Case "ITAKU_CODE_S"
                        '�ϑ��҃R�[�h9
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).ITAKU_CODE))
                    Case "TKIN_NO_S"
                        '�戵���Z�@��10
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TKIN_NO))
                    Case "TSIT_NO_S"
                        '�戵�X��11
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TSIT_NO))
                    Case "SOUSIN_KBN_S"
                        '���M�敪12
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).SOUSIN_KBN))
                    Case "BAITAI_CODE_S"
                        '�}�̃R�[�h13
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).BAITAI_CODE))
                    Case "MOTIKOMI_SEQ_S"
                        '����SEQ14
                        SQL.AppendLine(SetItem(ColumnID2, SCH.MOTIKOMI_SEQ.ToString))
                    Case "FILE_SEQ_S"
                        '�t�@�C��SEQ15
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KBN_S"
                        '�萔���v�Z�敪16
                        Select Case TR(Index).TESUUTYO_KBN
                            Case 0
                                '�萔�������敪 = �s�x����
                                SQL.AppendLine(SetItem(ColumnID2, "1"))
                            Case 1
                                '�萔�������敪 = �ꊇ����
                                Select Case TR(Index).MONTH_FLG
                                    Case 1, 3
                                        SQL.AppendLine(SetItem(ColumnID2, "2"))
                                    Case Else
                                        SQL.AppendLine(SetItem(ColumnID2, "3"))
                                End Select
                            Case Else
                                '�萔�������敪 = (2)���ʖƏ�, (3)�ʓr����
                                SQL.AppendLine(SetItem(ColumnID2, "0"))
                        End Select
                    Case "MOTIKOMI_DATE_S"
                        '�����\���17
                        SQL.AppendLine(SetItem(ColumnID2, SCH.MOTIKOMI_DATE))
                    Case "IRAISYO_DATE_S"
                        '�˗����쐬��18
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "IRAISYOK_YDATE_S"
                        '�˗�������\���19
                        SQL.AppendLine(SetItem(ColumnID2, SCH.IRAISYOK_YDATE))
                    Case "UKETUKE_DATE_S"
                        '��t��20
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "TOUROKU_DATE_S"
                        '�o�^��21
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "KAKUHO_YDATE_S"
                        '�����m�ۗ\���22
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KAKUHO_YDATE))
                    Case "KAKUHO_DATE_S"
                        '�����m�ۓ�23
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "HASSIN_YDATE_S"
                        '���M�\���24
                        SQL.AppendLine(SetItem(ColumnID2, SCH.HASSIN_YDATE))
                    Case "HASSIN_DATE_S"
                        '���M��25
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "SOUSIN_YDATE_S"
                        '���M�\���26
                        SQL.AppendLine(SetItem(ColumnID2, SCH.HASSIN_YDATE))
                    Case "SOUSIN_DATE_S"
                        '���M������27
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "KESSAI_YDATE_S"
                        '���ϗ\���28
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KESSAI_YDATE))
                    Case "KESSAI_DATE_S"
                        '���ϓ�29
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "TESUU_YDATE_S"
                        '�萔�������\���30
                        SQL.AppendLine(SetItem(ColumnID2, SCH.TESUU_YDATE))
                    Case "TESUU_DATE_S"
                        '�萔��������31
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "UKETORI_DATE_S"
                        '��揑�쐬��32
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "UKETUKE_FLG_S"
                        '��t�σt���O33
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TOUROKU_FLG_S"
                        '�o�^�σt���O34
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "KAKUHO_FLG_S"
                        '�m�ۍσt���O35
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "HASSIN_FLG_S"
                        '���M�σt���O36
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "SOUSIN_FLG_S"
                        '���M�σt���O37
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUUKEI_FLG_S"
                        '�萔���v�Z�σt���O38
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUUTYO_FLG_S"
                        '�萔�������σt���O39
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "KESSAI_FLG_S"
                        '���σt���O40
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TYUUDAN_FLG_S"
                        '���f�t���O41
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERROR_INF_S"
                        '�G���[���42
                        SQL.AppendLine("NULL")
                    Case "SYORI_KEN_S"
                        '��������43
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "SYORI_KIN_S"
                        '�������z44
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERR_KEN_S"
                        '�G���[����45
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERR_KIN_S"
                        '�G���[���z46
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN_S"
                        '�萔�����z47
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN1_S"
                        '�萔�����z�P48
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN2_S"
                        '�萔�����z�Q49
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN3_S"
                        '�萔�����z�R50
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FURI_KEN_S"
                        '�U���ό���51
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FURI_KIN_S"
                        '�U���ϋ��z52
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FUNOU_KEN_S"
                        '�s�\����53
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FUNOU_KIN_S"
                        '�s�\���z54
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "UFILE_NAME_S"
                        '��M�t�@�C����55
                        SQL.AppendLine("NULL")
                    Case "SFILE_NAME_S"
                        '���M�t�@�C����56
                        SQL.AppendLine("NULL")
                    Case "SAKUSEI_DATE_S"
                        '�쐬���t57
                        SQL.AppendLine("TO_CHAR(SYSDATE, 'yyyymmdd')")
                    Case "KAKUHO_TIME_STAMP_S"
                        '�m�ۃ^�C���X�^���v58
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "HASSIN_TIME_STAMP_S"
                        '���M�^�C���X�^���v59
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "KESSAI_TIME_STAMP_S"
                        '���σ^�C���X�^���v60
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "TESUU_TIME_STAMP_S"
                        '�萔�������^�C���X�^���v61
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "YOBI1_S"
                        '�\���P62
                        SQL.AppendLine("NULL")
                    Case "YOBI2_S"
                        '�\���Q63
                        SQL.AppendLine("NULL")
                    Case "YOBI3_S"
                        '�\���R64
                        SQL.AppendLine("NULL")
                    Case "YOBI4_S"
                        '�\���S65
                        SQL.AppendLine("NULL")
                    Case "YOBI5_S"
                        '�\���T66
                        SQL.AppendLine("NULL")
                    Case "YOBI6_S"
                        '�\���U67
                        SQL.AppendLine("NULL")
                    Case "YOBI7_S"
                        '�\���V68
                        SQL.AppendLine("NULL")
                    Case "YOBI8_S"
                        '�\���W69
                        SQL.AppendLine("NULL")
                    Case "YOBI9_S"
                        '�\���X70
                        SQL.AppendLine("NULL")
                    Case "YOBI10_S"
                        '�\���P�O71
                        SQL.AppendLine("NULL")
                    Case Else
                        SQL.AppendLine("NULL")
                End Select
            Next ColumnID2
            SQL.AppendLine(")")

            Dim SQLCode As Integer = 0
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, SEL)

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
    ' �@�\�@ �F �X�P�W���[���}�X�^����폜����
    '
    ' �����@ �F ARG1 - �U�����w��N��
    '
    ' �߂�l �F OK = True, NG = False
    '
    ' ���l�@ �F ���g�p�̊Y���N���X�P�W���[�����폜����(�_��U��������)
    '
    Public Function DELETE_SCHMAST(ByVal FormFuriDate As Date) As Boolean
        Try
            Dim StartDay As String = String.Format("{0:yyyyMM}", FormFuriDate) & "01"
            Dim onDate As Date = FormFuriDate.AddMonths(1)
            onDate = GCom.SET_DATE(String.Format("{0:yyyyMM}", onDate) & "01")
            onDate = onDate.AddDays(-1)
            Dim EndDay As String = String.Format("{0:yyyyMMdd}", onDate)

            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("DELETE FROM S_SCHMAST")
            SQL.AppendLine(" WHERE KFURI_DATE_S BETWEEN '" & StartDay & "' AND '" & EndDay & "'")
            SQL.AppendLine(" AND UKETUKE_FLG_S = '0'")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '0'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            
            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)

            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If SQLCode = 0 Then
                    Dim Dsql As New StringBuilder(128)
                    Dsql.Length = 0
                    Dsql.Append("DELETE FROM S_SCHMAST_SUB ")
                    Dsql.Append(" WHERE ")
                    Dsql.Append("     NOT EXISTS")
                    Dsql.Append("     (")
                    Dsql.Append("      SELECT")
                    Dsql.Append("          *")
                    Dsql.Append("      FROM")
                    Dsql.Append("          S_SCHMAST")
                    Dsql.Append("      WHERE")
                    Dsql.Append("          TORIS_CODE_S       = TORIS_CODE_SSUB")
                    Dsql.Append("      AND TORIF_CODE_S       = TORIF_CODE_SSUB")
                    Dsql.Append("      AND FURI_DATE_S        = FURI_DATE_SSUB")
                    Dsql.Append("      AND MOTIKOMI_SEQ_S     = MOTIKOMI_SEQ_SSUB")
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
        Try
            Dim SQL As StringBuilder = New StringBuilder(128)
            SQL.AppendLine("DELETE FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim SQLCode As Integer = 0
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)

            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If Ret = 1 AndAlso SQLCode = 0 Then
                    SQL.Length = 0
                    SQL.AppendLine("DELETE FROM S_SCHMAST_SUB")
                    SQL.AppendLine(" WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'")
                    SQL.AppendLine(" AND TORIS_CODE_SSUB   = '" & TR(0).TORIS_CODE & "'")
                    SQL.AppendLine(" AND TORIF_CODE_SSUB   = '" & TR(0).TORIF_CODE & "'")
                    SQL.AppendLine(" AND FURI_DATE_SSUB    = '" & SCH.FURI_DATE & "'")
                    SQL.AppendLine(" AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ)

                    SQLCode = 0
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)
                End If
            End If
            ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

            If cFlg = False Then Return True

            If Ret = 1 AndAlso SQLCode = 0 Then
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
        '               :astrNEW_FURI_DATE�F�V�U����
        'Description    :�V�X�P�W���[�������݂��Ȃ����Ƃ��m�F
        'Return         :True=OK(���݂���),False=NG�i���݂��Ȃ��j
        'Create         :2007/05/28
        'Update         :
        '============================================================================

        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            Dim BRet As Boolean

            SQL.AppendLine("SELECT COUNT(*) AS COUNTER FROM S_SCHMAST")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("TORIS_CODE_S = '" & TorisCode.Trim & "' AND ")
            SQL.AppendLine("TORIF_CODE_S = '" & TorifCode.Trim & "' AND ")
            SQL.AppendLine("FURI_DATE_S = '" & NewFuriDate.Trim & "'")

            BRet = GCom.SetDynaset(SQL.ToString, REC)
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
        Finally
            If Not REC Is Nothing Then REC.Close()
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
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT TORIS_CODE_S")
            SQL.AppendLine(" FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(Index).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(Index).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(Index).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
            If BRet AndAlso REC.Read Then
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
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("UPDATE S_TORIMAST")
            SQL.AppendLine(" SET KOUSIN_SIKIBETU_T = '2'")
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '" & TR(Index).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_T = '" & TR(Index).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_T = '" & TR(Index).TORIF_CODE & "'")

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, False)

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

            Dim SQL As StringBuilder = New StringBuilder()
            SQL.AppendLine("SELECT YASUMI_DATE_Y")
            SQL.AppendLine(", YASUMI_NAME_Y")
            SQL.AppendLine(", SAKUSEI_DATE_Y")
            SQL.AppendLine(", KOUSIN_DATE_Y")
            SQL.AppendLine(" FROM YASUMIMAST")
            SQL.AppendLine(" ORDER BY YASUMI_DATE_Y ASC")

            Dim Ret As Boolean = GCom.SetDynaset(SQL.ToString, REC)

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
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT * FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
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
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT * FROM S_SCHMAST_SUB")
            SQL.AppendLine(" WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_SSUB = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_SSUB = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_SSUB = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
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
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT COUNT(*) FROM JOBMAST")
            SQL.AppendLine(" WHERE TOUROKU_DATE_J = '" & Now.ToString("yyyyMMdd") & "'")
            SQL.AppendLine(" AND STATUS_J IN ('0','1')")

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
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
