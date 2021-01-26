Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFGP107
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP107"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        '列名設定
        With CSVObject
            .Output("処理日")
            .Output("タイムスタンプ")
            .Output("ログイン名")
            .Output("端末名")
            .Output("学校コード")
            .Output("年度")
            .Output("通番")
            .Output("クラス")
            .Output("生徒番号")
            .Output("学校名")
            .Output("日本語項目名")
            .Output("ORACLE項目名")
            .Output("変更前")
            .Output("変更後", False, True)
        End With

        Return file
    End Function

    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    '2017/04/05 saitou 東春信金(RSV2標準) ADD 生徒マスタメンテ帳票改善 ---------------------------------------- START
    Public Class TableInfo
        Public Const HIMOKU_INFO_START_INDEX As Integer = 28
        Public Const KOUSIN_DATE_INDEX As Integer = 59

        Public Shared ReadOnly ColumnEnglishName() As String = { _
            "GAKKOU_CODE_O", _
            "NENDO_O", _
            "TUUBAN_O", _
            "GAKUNEN_CODE_O", _
            "CLASS_CODE_O", _
            "SEITO_NO_O", _
            "SEITO_KNAME_O", _
            "SEITO_NNAME_O", _
            "SEIBETU_O", _
            "TKIN_NO_O", _
            "TSIT_NO_O", _
            "KAMOKU_O", _
            "KOUZA_O", _
            "MEIGI_KNAME_O", _
            "MEIGI_NNAME_O", _
            "FURIKAE_O", _
            "KEIYAKU_NJYU_O", _
            "KEIYAKU_DENWA_O", _
            "KAIYAKU_FLG_O", _
            "SINKYU_KBN_O", _
            "HIMOKU_ID_O", _
            "TYOUSI_FLG_O", _
            "TYOUSI_NENDO_O", _
            "TYOUSI_TUUBAN_O", _
            "TYOUSI_GAKUNEN_O", _
            "TYOUSI_CLASS_O", _
            "TYOUSI_SEITONO_O", _
            "TUKI_NO_O", _
            "SEIKYU01_O", _
            "KINGAKU01_O", _
            "SEIKYU02_O", _
            "KINGAKU02_O", _
            "SEIKYU03_O", _
            "KINGAKU03_O", _
            "SEIKYU04_O", _
            "KINGAKU04_O", _
            "SEIKYU05_O", _
            "KINGAKU05_O", _
            "SEIKYU06_O", _
            "KINGAKU06_O", _
            "SEIKYU07_O", _
            "KINGAKU07_O", _
            "SEIKYU08_O", _
            "KINGAKU08_O", _
            "SEIKYU09_O", _
            "KINGAKU09_O", _
            "SEIKYU10_O", _
            "KINGAKU10_O", _
            "SEIKYU11_O", _
            "KINGAKU11_O", _
            "SEIKYU12_O", _
            "KINGAKU12_O", _
            "SEIKYU13_O", _
            "KINGAKU13_O", _
            "SEIKYU14_O", _
            "KINGAKU14_O", _
            "SEIKYU15_O", _
            "KINGAKU15_O", _
            "SAKUSEI_DATE_O", _
            "KOUSIN_DATE_O", _
            "YOBI1_O", _
            "YOBI2_O", _
            "YOBI3_O", _
            "YOBI4_O", _
            "YOBI5_O" _
            }

        Public Shared ReadOnly ColumnJapaneseName() As String = { _
            "学校コード", _
            "入学年度", _
            "通番", _
            "学年コード", _
            "クラスコード", _
            "生徒番号", _
            "生徒氏名(カナ)", _
            "生徒氏名(漢字)", _
            "性別", _
            "金融機関コード", _
            "支店コード", _
            "科目", _
            "口座番号", _
            "名義人(カナ)", _
            "名義人(漢字)", _
            "振替方法", _
            "契約住所", _
            "契約電話番号", _
            "解約区分", _
            "進級区分", _
            "費目ID", _
            "長子有無フラグ", _
            "長子入学年度", _
            "長子通番", _
            "長子学年", _
            "長子クラス", _
            "長子生徒番号", _
            "請求月", _
            "費目1の請求方法", _
            "費目1の請求金額", _
            "費目2の請求方法", _
            "費目2の請求金額", _
            "費目3の請求方法", _
            "費目3の請求金額", _
            "費目4の請求方法", _
            "費目4の請求金額", _
            "費目5の請求方法", _
            "費目5の請求金額", _
            "費目6の請求方法", _
            "費目6の請求金額", _
            "費目7の請求方法", _
            "費目7の請求金額", _
            "費目8の請求方法", _
            "費目8の請求金額", _
            "費目9の請求方法", _
            "費目9の請求金額", _
            "費目10の請求方法", _
            "費目10の請求金額", _
            "費目11の請求方法", _
            "費目11の請求金額", _
            "費目12の請求方法", _
            "費目12の請求金額", _
            "費目13の請求方法", _
            "費目13の請求金額", _
            "費目14の請求方法", _
            "費目14の請求金額", _
            "費目15の請求方法", _
            "費目15の請求金額", _
            "作成日付", _
            "更新日付", _
            "予備1", _
            "予備2", _
            "予備3", _
            "予備4", _
            "予備5" _
            }


        Public Shared Function GetColumnEnglishName(ByVal ColumnIndex As Integer) As String
            Try
                Return ColumnEnglishName(ColumnIndex)
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Shared Function GetColumnJapaneseName(ByVal ColumnIndex As Integer) As String
            Try
                Return ColumnJapaneseName(ColumnIndex)
            Catch ex As Exception
                Return ""
            End Try
        End Function

    End Class
    '2017/04/05 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------------- END

End Class
