Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections

' �������Ϗ������ʊm�F�\
Class ClsPrnKessaiSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    ' ���O�����N���X
    Private LOG As New CASTCommon.BatchLOG("�������σf�[�^�쐬", "KESSAI")

    Public CsvData(43) As String

    Private BasePath As String              ' INI�t�@�C���̎������σt�@�C���p�X
    Private KessaiFileName As String        ' �������σt�@�C����
    Private strKESSAI_DATE As String        ' ���ϓ�

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFKP001"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFKP001_�������ʊm�F�\(��������).rpd"

        CsvData(0) = "00010101"                                     ' ������
        CsvData(1) = "00010101"                                     ' ���ϓ�
        CsvData(2) = "00010101"                                     ' �^�C���X�^���v
        CsvData(3) = ""                                             ' ������R�[�h
        CsvData(4) = ""                                             ' ����敛�R�[�h
        CsvData(5) = ""                                             ' ����於
        CsvData(6) = ""                                             ' �U�փR�[�h
        CsvData(7) = ""                                             ' ��ƃR�[�h
        CsvData(8) = "00010101"                                     ' �U�֓�
        CsvData(9) = ""                                             ' ���ϋ敪�i�����j
        CsvData(10) = ""                                            ' ���ϋ��Z�@�փR�[�h
        CsvData(11) = ""                                            ' ���ϋ��Z�@�֎x�X�R�[�h
        CsvData(12) = ""                                            ' ���ωȖ�
        CsvData(13) = ""                                            ' ���ό����ԍ�
        CsvData(14) = ""                                            ' �萔�������敪
        CsvData(15) = ""                                            ' �萔���������@
        CsvData(16) = ""                                            ' ���܂ƂߓX�R�[�h
        CsvData(17) = ""                                            ' ���܂ƂߓX��
        CsvData(18) = "0"                                           ' ��������
        CsvData(19) = "0"                                           ' �������z
        CsvData(20) = "0"                                           ' �s�\����
        CsvData(21) = "0"                                           ' �s�\���z
        CsvData(22) = "0"                                           ' �U�֌���
        CsvData(23) = "0"                                           ' �U�֋��z
        CsvData(24) = "0"                                           ' �萔��
        CsvData(25) = "0"                                           ' �萔������|���U
        CsvData(26) = "0"                                           ' �萔������|�U��
        CsvData(27) = "0"                                           ' �萔������|���̑�
        CsvData(28) = "0"                                           ' ����������
        CsvData(29) = "0"                                           ' ���������z
        CsvData(30) = ""                                            ' �ȖڃR�[�h
        CsvData(31) = ""                                            ' �I�y�R�[�h
        CsvData(32) = ""                                            ' �I�y���[�V������
        CsvData(33) = "0"                                           ' ���o���z�i�I�y�R�[�h�P�ʁj
        CsvData(34) = "0"                                           ' �萔���z�i�I�y�R�[�h�P�ʁj
        CsvData(35) = "0"                                           ' �W�v�t���O
        CsvData(36) = "0"                                           ' ���G���^�쐬�敪
        CsvData(37) = ""                                            ' ���ϋ��Z�@�֖�
        CsvData(38) = ""                                            ' ���ώx�X��
        CsvData(39) = ""
        CsvData(40) = ""                                            ' �{���ʒi�����ԍ�
        CsvData(41) = ""                                            ' �C�Ӎ���
        CsvData(42) = "0"                                           ' ���R�[�h�ԍ�
        CsvData(43) = "1"                                           ' �쐬�敪(1:���o�b�`)
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("���ϓ�")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("���ϋ敪")
        CSVObject.Output("���ϋ��Z�@�փR�[�h")
        CSVObject.Output("���ώx�X�R�[�h")
        CSVObject.Output("���ωȖ�")
        CSVObject.Output("���ό����ԍ�")
        CSVObject.Output("�萔�������敪")
        CSVObject.Output("�萔���������@")
        CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
        CSVObject.Output("�Ƃ�܂ƂߓX��")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("�s�\����")
        CSVObject.Output("�s�\���z")
        CSVObject.Output("�U�֌���")
        CSVObject.Output("�U�֋��z")
        CSVObject.Output("�萔��")
        CSVObject.Output("���U�萔��")
        CSVObject.Output("�U���萔��")
        CSVObject.Output("���̑��萔��")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("�ȖڃR�[�h")
        CSVObject.Output("�I�y�R�[�h")
        CSVObject.Output("�I�y���[�V������")
        CSVObject.Output("���o���z")
        CSVObject.Output("�萔���z")
        CSVObject.Output("�W�v�t���O")
        CSVObject.Output("�쐬�敪")
        CSVObject.Output("���ϋ��Z�@�֖�")
        CSVObject.Output("���ώx�X��")
        CSVObject.Output("���b�Z�[�W")
        CSVObject.Output("�{���ʒi�����ԍ�")
        CSVObject.Output("�C�Ӎ���")
        CSVObject.Output("���R�[�h�ԍ�")
        CSVObject.Output("�쐬�敪", False, True)
        Return file

    End Function

    ''' <summary>
    ''' �������ʊm�F�\(�������σf�[�^�쐬)��CSV���o�͂��܂��B
    ''' </summary>
    ''' <param name="ary"></param>
    ''' <param name="jikinko"></param>
    ''' <param name="strDate"></param>
    ''' <param name="strTime"></param>
    ''' <param name="strKessaiDate"></param>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' <remarks>2013/09/17 ��_�M�� ���o�b�`�Ή�</remarks>
    Public Function OutputCSVKekka(ByVal ary As ArrayList, _
                                   ByVal jikinko As String, _
                                   ByVal strDate As String, _
                                   ByVal strTime As String, _
                                   ByVal strKessaiDate As String, _
                                   ByVal db As CASTCommon.MyOracle, _
                                   ByVal aryMSG As ArrayList, _
                                   ByRef aryHimokuName As ArrayList) As Integer

        Dim KData As CAstFormKes.ClsFormKes.KessaiDataKinBatch = Nothing
        Dim YData As ClsKessaiDataCreateKinBatch.msgDATA = Nothing
        Dim newkey As String = ""
        Dim oldkey As String = ""

        Dim cnt As Integer = ary.Count - 1 '���[�v��

        Dim lngSagaku As Long
        Dim strNyuukin As String

        Try

            For i As Integer = 0 To cnt

                KData.Init()

                KData = CType(ary.Item(i), CAstFormKes.ClsFormKes.KessaiDataKinBatch)

                YData.Init()
                YData = CType(aryMSG.Item(i), ClsKessaiDataCreateKinBatch.msgDATA)

                newkey = KData.TorisCode & KData.TorifCode & KData.FuriDate

                ' �b�r�u�f�[�^�ݒ�
                CsvData(0) = strDate                  ' ������
                CsvData(1) = strKessaiDate                              ' ���
                CsvData(2) = strTime                                    ' �^�C���X�^���v
                CsvData(3) = KData.TorisCode                             ' ������R�[�h
                CsvData(4) = KData.TorifCode                             ' ����敛�R�[�h
                CsvData(5) = KData.ToriNName.Trim                        ' ����於
                CsvData(6) = KData.FuriCode                              ' �U�փR�[�h
                CsvData(7) = KData.KigyoCode                             ' ��ƃR�[�h
                CsvData(8) = KData.FuriDate                                     ' �U�֓�

                Select Case KData.KessaiKbn
                    Case "00"
                        CsvData(9) = "�a����"
                    Case "01"
                        CsvData(9) = "��������"                                            ' ���ϋ敪�i�����j
                    Case "02"
                        CsvData(9) = "�ב֐U��"
                    Case "03"
                        CsvData(9) = "�ב֕t��"
                    Case "04"
                        CsvData(9) = "�ʒi�o���̂�"
                    Case "05"
                        CsvData(9) = "���ʊ��"
                    Case "99"
                        CsvData(9) = "���ϑΏۊO"
                End Select

                CsvData(10) = KData.KesKinCode                                            ' ���ϋ��Z�@�փR�[�h
                CsvData(11) = KData.KesSitCode                                            ' ���ϋ��Z�@�֎x�X�R�[�h
                CsvData(12) = KData.KesKamoku                                            ' ���ωȖ�
                CsvData(13) = KData.KesKouza                                ' ���ό����ԍ�

                Select Case KData.TesTyoKbn
                    Case "0"    '�s�x�����̏ꍇ
                        CsvData(14) = "�s�x����"                               ' �萔�������敪
                    Case "1"    '�ꊇ�����̏ꍇ
                        CsvData(14) = "�ꊇ����"
                    Case "2"    '���ʖƏ��̏ꍇ
                        CsvData(14) = "���ʖƏ�"

                    Case "3"    '�ʓr�����̏ꍇ
                        CsvData(14) = "�ʓr����"
                    Case Else
                        CsvData(14) = "�萔��������"
                End Select

                Select Case KData.TesTyohh
                    Case "0"
                        CsvData(15) = "����"                               ' �萔���������@
                    Case "1"
                        CsvData(15) = "����"                               ' �萔���������@
                    Case Else
                        CsvData(15) = ""                                    ' �萔���������@
                End Select

                CsvData(16) = KData.TorimatomeSit                           ' ���܂ƂߓX�R�[�h
                CsvData(17) = GetTenmast(jikinko, KData.TorimatomeSit, MainDB)                                            ' ���܂ƂߓX��
                CsvData(18) = KData.SyoriKen.Trim                                            ' ��������
                CsvData(19) = KData.Syorikin.Trim                                            ' �������z
                CsvData(20) = KData.FunouKen.Trim                                            ' �s�\����
                CsvData(21) = KData.FunouKin.Trim                                            ' �s�\���z
                CsvData(22) = KData.FuriKen.Trim                                            ' �U�֌���
                CsvData(23) = KData.FuriKin.Trim                                           ' �U�֋��z
                CsvData(24) = KData.TesuuKin.Trim                                           ' �萔��
                CsvData(25) = KData.JifutiTesuuKin.Trim                                      ' �萔������|���U
                CsvData(26) = KData.FurikomiTesuukin.Trim                                    ' �萔������|�U��
                CsvData(27) = KData.SonotaTesuuKin.Trim                                     ' �萔������|���̑�
                CsvData(28) = KData.NyukinKen.Trim                                           ' ����������

                ' ���������z�̐ݒ�
                ' �U�֍ϋ��z - �萔�����z > 0 and �萔�������敪 = 0�F�s�x���� and �萔���������@=0�F��������
                lngSagaku = CLng(KData.FuriKin.Trim) - CLng(KData.TesuuKin.Trim)
                If lngSagaku > 0 And KData.TesTyoKbn = "0" And KData.TesTyohh = "0" Then
                    strNyuukin = lngSagaku.ToString
                Else
                    strNyuukin = KData.FuriKin.Trim
                End If

                CsvData(29) = strNyuukin                                           ' ���������z
                CsvData(30) = KData.OpeCode.Substring(0, 2)                             ' �ȖڃR�[�h
                CsvData(31) = KData.OpeCode.Substring(2, 3)                             ' �I�y�R�[�h
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                Dim strBikou As String = ""
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                Select Case KData.OpeCode
                    Case "04099"
                        CsvData(32) = "�ʒi�x��"                                                        ' �I�y���[�V������
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = "�{���ʒi�����ԍ�:" & KData.HonbuKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "01010"
                        CsvData(32) = "��������"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "02019"
                        CsvData(32) = "���ʓ���(NB)"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "04019"
                        CsvData(32) = "�ʒi����"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "99019"
                        CsvData(32) = "���������"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        '�萔���łȂ����̂݃Z�b�g
                        If CLng(KData.ope_nyukin.Trim) <> 0 Then
                            strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                            strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                            strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        End If
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "48100"
                        CsvData(32) = "�ב֐U��"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "48500"
                        CsvData(32) = "�ב֕t��"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "48600"
                        CsvData(32) = "�ב֐���"
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        strBikou = GetHimokuName(aryHimokuName.Item(i)) & KData.KesKinCode & "-" & KData.KesSitCode & "  "
                        strBikou &= GetTenmast(KData.KesKinCode, "", MainDB, True).PadRight(15) & "  " & GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False).PadRight(15) & "  "
                        strBikou &= KData.KesKamoku & "-" & KData.KesKouza
                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                    Case "99418"
                        CsvData(32) = "�萔������(�A��)"
                    Case "99419"
                        CsvData(32) = "������A������"
                End Select

                CsvData(33) = KData.ope_nyukin.Trim                                          ' ���o���z�i�I�y�R�[�h�P�ʁj
                CsvData(34) = KData.ope_tesuu.Trim                                           ' �萔���z�i�I�y�R�[�h�P�ʁj

                If newkey = oldkey Then
                    CsvData(35) = "0"                                           ' �W�v�t���O �W�v���Ȃ�
                Else
                    CsvData(35) = "1"                                          ' �W�v�t���O �W�v����
                End If

                Select Case KData.ToriKbn                     ' 0�F�������ςƎ萔�������̗����̐�A1�F�������ς̂ݐ�A2�F�萔�������̂ݐ�
                    Case "0"
                        CsvData(36) = "���ρE�萔"
                    Case "1"
                        CsvData(36) = "����"
                    Case "2"
                        CsvData(36) = "�萔"
                End Select

                CsvData(37) = GetTenmast(KData.KesKinCode, "", MainDB, True)                        '���ϋ��Z�@�֖�
                CsvData(38) = GetTenmast(KData.KesKinCode, KData.KesSitCode, MainDB, False)         '���ώx�X��
                CsvData(39) = YData.msg_DATA
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                '�{���ʒi�����ԍ��ǉ�
                CsvData(40) = KData.HonbuKouza
                '�I�y�R�[�h�P�ʂ̏����I�y�R�[�h���ɏo��
                CsvData(41) = strBikou
                '���R�[�h�ԍ��ǉ�
                CsvData(42) = (i + 1).ToString
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                CsvData(43) = "1"

                '�b�r�u�o�͏���
                If CSVObject.Output(CsvData) = 0 Then
                    Return -1
                End If

                oldkey = KData.TorisCode & KData.TorifCode & KData.FuriDate

            Next

        Catch ex As Exception
            LOG.Write("�������Ϗ������ʊm�F�\�b�r�u�o��", "���s", ex.Message)
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")

            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try



    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle, ByVal KIN As Boolean) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strKinName As String = ""

        Try
            If KIN = True Then
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' ORDER BY SIT_NO_N")
            Else
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")
            End If


            If orareader.DataReader(sql) = True Then
                If KIN = True Then
                    strKinName = orareader.GetString("KIN_KNAME_N")
                Else
                    strKinName = orareader.GetString("SIT_KNAME_N")
                End If
                Return strKinName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function
    
    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
    Private Function GetHimokuName(ByVal HimokuName As Object) As String
        Dim retHimokuName As String = CStr(HimokuName).Trim
        If retHimokuName = "" Then
            Return ""
        Else
            Return Microsoft.VisualBasic.StrConv(retHimokuName.PadRight(10, " "c), Microsoft.VisualBasic.VbStrConv.Wide).Substring(0, 10)
        End If
    End Function
    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

End Class
