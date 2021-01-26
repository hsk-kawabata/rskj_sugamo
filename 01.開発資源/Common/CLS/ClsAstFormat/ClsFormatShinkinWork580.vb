Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' しんきんフォーマット（ワーク）クラス
Public Class CFormatShinkinWork580
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 580

    ' ------------------------------------------
    ' しんきんフォーマット
    ' ------------------------------------------
    '----------------
    'ヘッダーレコード
    '----------------
    Structure JF_Record1
        Implements CFormat.IFormat

        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(12)> Public TC As String      '取引先コード
        '<VBFixedString(9)> Public TC As String      '取引先コード
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        <VBFixedString(28)> Public FKN As String    '振替企業名     '2007/10/03 mitsu 28桁に変更
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String     '金庫コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(4)> Public JF4 As String     '持込み金庫コード
        <VBFixedString(1)> Public JF5 As String     '返還区分
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(123)> Public JF6 As String   '予備
        <VBFixedString(389)> Public JF400 As String '予備
        '<VBFixedString(115)> Public JF6 As String   '予備           '2007/10/03 mitsu 115桁に変更
        '<VBFixedString(400)> Public JF400 As String   '予備
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                Return String.Concat(New String() _
                                   { _
                                    SubData(TC, 12), _
                                    SubData(FKN, 28), _
                                    SubData(KNM, 15), _
                                    SubData(JF1, 4), _
                                    SubData(JF2, 3), _
                                    SubData(JF3, 1), _
                                    SubData(JF4, 4), _
                                    SubData(JF5, 1), _
                                    SubData(JF6, 123), _
                                    SubData(JF400, 389) _
                                   })
                'Return String.Concat(New String() _
                '                   { _
                '                    SubData(TC, 9), _
                '                    SubData(FKN, 28), _
                '                    SubData(KNM, 15), _
                '                    SubData(JF1, 4), _
                '                    SubData(JF2, 3), _
                '                    SubData(JF3, 1), _
                '                    SubData(JF4, 4), _
                '                    SubData(JF5, 1), _
                '                    SubData(JF6, 115), _
                '                    SubData(JF400, 400) _
                '                   })
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Get
            Set(ByVal value As String)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                TC = CuttingData(value, 12)
                'TC = CuttingData(value, 9)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                FKN = CuttingData(value, 28)
                KNM = CuttingData(value, 15)
                JF1 = CuttingData(value, 4)
                JF2 = CuttingData(value, 3)
                JF3 = CuttingData(value, 1)
                JF4 = CuttingData(value, 4)
                JF5 = CuttingData(value, 1)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                JF6 = CuttingData(value, 123)
                JF400 = CuttingData(value, 389)
                'JF6 = CuttingData(value, 115)
                'JF400 = CuttingData(value, 400)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Set
        End Property
    End Structure
    Public JF_DATA1 As JF_Record1

    '--------------
    'データレコード
    '--------------
    Structure JF_Record2
        Implements CFormat.IFormat

        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(12)> Public TC As String      '取引先コード
        '<VBFixedString(9)> Public TC As String      '取引先コード
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        <VBFixedString(30)> Public FKN As String    '振替企業名
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String     '金庫コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(2)> Public JF4 As String     '科目コード
        <VBFixedString(7)> Public JF5 As String     '口座番号
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(8)> Public JF6 As String     '自振指定日
        <VBFixedString(13)> Public JF7 As String    '金額
        '<VBFixedString(6)> Public JF6 As String     '自振指定日
        '<VBFixedString(10)> Public JF7 As String    '金額
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        <VBFixedString(1)> Public JF8 As String     '入出金区分
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(5)> Public JF9 As String     '企業コード
        <VBFixedString(8)> Public JF10 As String    '企業シーケンス
        '<VBFixedString(4)> Public JF9 As String     '企業コード
        '<VBFixedString(7)> Public JF10 As String    '企業シーケンス
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        <VBFixedString(2)> Public JF11 As String    '口座内優先コード
        <VBFixedString(3)> Public JF12 As String    '振替コード
        <VBFixedString(2)> Public JF13 As String    '振替相手科目
        <VBFixedString(7)> Public JF14 As String    '振替相手口番
        <VBFixedString(4)> Public JF15 As String    '開始年月
        <VBFixedString(4)> Public JF16 As String    '終了年月
        <VBFixedString(1)> Public JF17 As String    '摘要設定区分
        <VBFixedString(13)> Public JF18 As String   'カナ摘要
        <VBFixedString(12)> Public JF19 As String   '漢字摘要
        <VBFixedString(24)> Public JF20 As String   '需要家番号
        <VBFixedString(7)> Public JF21 As String    '特定顧客番号
        <VBFixedString(1)> Public JF22 As String    '休日指定コード
        <VBFixedString(1)> Public JF23 As String    '予備
        <VBFixedString(2)> Public FMT As String     'フォーマット区分
        <VBFixedString(3)> Public YOBI1 As String   '取引先マスタ 予備１（摘要コード）
        <VBFixedString(3)> Public YOBI2 As String   '取引先マスタ 予備２（内訳コード）
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(6)> Public RECORDNO As String   'レコード番号
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(376)> Public JF400 As String '予備
        '<VBFixedString(392)> Public JF400 As String '予備
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                Return String.Concat(New String() _
                                      { _
                                          SubData(TC, 12), _
                                          SubData(FKN, 30), _
                                          SubData(KNM, 15), _
                                          SubData(JF1, 4), _
                                          SubData(JF2, 3), _
                                          SubData(JF3, 1), _
                                          SubData(JF4, 2), _
                                          SubData(JF5, 7), _
                                          SubData(JF6, 8), _
                                          SubData(JF7, 13), _
                                          SubData(JF8, 1), _
                                          SubData(JF9, 5), _
                                          SubData(JF10, 8), _
                                          SubData(JF11, 2), _
                                          SubData(JF12, 3), _
                                          SubData(JF13, 2), _
                                          SubData(JF14, 7), _
                                          SubData(JF15, 4), _
                                          SubData(JF16, 4), _
                                          SubData(JF17, 1), _
                                          SubData(JF18, 13), _
                                          SubData(JF19, 12), _
                                          SubData(JF20, 24), _
                                          SubData(JF21, 7), _
                                          SubData(JF22, 1), _
                                          SubData(JF23, 1), _
                                          SubData(FMT, 2), _
                                          SubData(YOBI1, 3), _
                                          SubData(YOBI2, 3), _
                                          SubData(RECORDNO, 6), _
                                          SubData(JF400, 376) _
                                      })
                'Return String.Concat(New String() _
                '                      { _
                '                          SubData(TC, 9), _
                '                          SubData(FKN, 30), _
                '                          SubData(KNM, 15), _
                '                          SubData(JF1, 4), _
                '                          SubData(JF2, 3), _
                '                          SubData(JF3, 1), _
                '                          SubData(JF4, 2), _
                '                          SubData(JF5, 7), _
                '                          SubData(JF6, 6), _
                '                          SubData(JF7, 10), _
                '                          SubData(JF8, 1), _
                '                          SubData(JF9, 4), _
                '                          SubData(JF10, 7), _
                '                          SubData(JF11, 2), _
                '                          SubData(JF12, 3), _
                '                          SubData(JF13, 2), _
                '                          SubData(JF14, 7), _
                '                          SubData(JF15, 4), _
                '                          SubData(JF16, 4), _
                '                          SubData(JF17, 1), _
                '                          SubData(JF18, 13), _
                '                          SubData(JF19, 12), _
                '                          SubData(JF20, 24), _
                '                          SubData(JF21, 7), _
                '                          SubData(JF22, 1), _
                '                          SubData(JF23, 1), _
                '                          SubData(FMT, 2), _
                '                          SubData(YOBI1, 3), _
                '                          SubData(YOBI2, 3), _
                '                          SubData(JF400, 392) _
                '                      })
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Get
            Set(ByVal Value As String)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                TC = CuttingData(Value, 12)
                'TC = CuttingData(Value, 9)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                FKN = CuttingData(Value, 30)
                KNM = CuttingData(Value, 15)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 7)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                JF6 = CuttingData(Value, 8)
                JF7 = CuttingData(Value, 13)
                'JF6 = CuttingData(Value, 6)
                'JF7 = CuttingData(Value, 10)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                JF8 = CuttingData(Value, 1)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                JF9 = CuttingData(Value, 5)
                JF10 = CuttingData(Value, 8)
                'JF9 = CuttingData(Value, 4)
                'JF10 = CuttingData(Value, 7)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                JF11 = CuttingData(Value, 2)
                JF12 = CuttingData(Value, 3)
                JF13 = CuttingData(Value, 2)
                JF14 = CuttingData(Value, 7)
                JF15 = CuttingData(Value, 4)
                JF16 = CuttingData(Value, 4)
                JF17 = CuttingData(Value, 1)
                JF18 = CuttingData(Value, 13)
                JF19 = CuttingData(Value, 12)
                JF20 = CuttingData(Value, 24)
                JF21 = CuttingData(Value, 7)
                JF22 = CuttingData(Value, 1)
                JF23 = CuttingData(Value, 1)
                FMT = CuttingData(Value, 2)
                YOBI1 = CuttingData(Value, 3)
                YOBI2 = CuttingData(Value, 3)
                '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                RECORDNO = CuttingData(Value, 6)
                '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                JF400 = CuttingData(Value, 376)
                'JF400 = CuttingData(Value, 392)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Set
        End Property
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        Public Sub INIT()
            TC = ""
            FKN = ""
            KNM = ""    '契約者氏名
            JF1 = ""     '金庫コード
            JF2 = ""     '店舗コード
            JF3 = ""     'レコード種別
            JF4 = ""     '科目コード
            JF5 = ""     '口座番号
            JF6 = ""     '自振指定日
            JF7 = ""    '金額
            JF8 = ""     '入出金区分
            JF9 = ""     '企業コード
            JF10 = ""    '企業シーケンス
            JF11 = ""    '口座内優先コード
            JF12 = ""    '振替コード
            JF13 = ""    '振替相手科目
            JF14 = ""    '振替相手口番
            JF15 = ""    '開始年月
            JF16 = ""    '終了年月
            JF17 = ""    '摘要設定区分
            JF18 = ""   'カナ摘要
            JF19 = ""   '漢字摘要
            JF20 = ""   '需要家番号
            JF21 = ""    '特定顧客番号
            JF22 = ""    '休日指定コード
            JF23 = ""    '予備
            FMT = ""     'フォーマット区分
            YOBI1 = ""   '取引先マスタ 予備１（摘要コード）
            YOBI2 = ""   '取引先マスタ 予備２（内訳コード）
            RECORDNO = ""   'レコード番号
            JF400 = "" '予備
        End Sub
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
    End Structure
    Public JF_DATA2 As JF_Record2

    '--------------
    'エンドレコード
    '--------------
    Structure JF_Record3
        Implements CFormat.IFormat

        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(12)> Public TC As String      '取引先コード
        '<VBFixedString(9)> Public TC As String      '取引先コード
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        <VBFixedString(30)> Public FKN As String    '振替企業名
        <VBFixedString(15)> Public KNM As String    '契約者氏名
        <VBFixedString(4)> Public JF1 As String     '金庫コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(118)> Public JF4 As String   '予備
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        <VBFixedString(397)> Public JF400 As String '予備
        '<VBFixedString(400)> Public JF400 As String '予備
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                Return String.Concat(New String() _
                                { _
                                    SubData(TC, 12), _
                                    SubData(FKN, 30), _
                                    SubData(KNM, 15), _
                                    SubData(JF1, 4), _
                                    SubData(JF2, 3), _
                                    SubData(JF3, 1), _
                                    SubData(JF4, 118), _
                                    SubData(JF400, 397) _
                                })
                'Return String.Concat(New String() _
                '                { _
                '                    SubData(TC, 9), _
                '                    SubData(FKN, 30), _
                '                    SubData(KNM, 15), _
                '                    SubData(JF1, 4), _
                '                    SubData(JF2, 3), _
                '                    SubData(JF3, 1), _
                '                    SubData(JF4, 118), _
                '                    SubData(JF400, 400) _
                '                })
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Get
            Set(ByVal Value As String)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                TC = CuttingData(Value, 12)
                'TC = CuttingData(Value, 9)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
                FKN = CuttingData(Value, 30)
                KNM = CuttingData(Value, 15)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 118)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
                JF400 = CuttingData(Value, 397)
                'JF400 = CuttingData(Value, 400)
                '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            End Set
        End Property
    End Structure
    Public JF_DATA3 As JF_Record3

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"0", "9"}

        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        FtranPfile = "136.P"    'Pファイルは現行にも存在しないので未使用か？
        'FtranPfile = "128.P"
        '2018/02/01 タスク）西野 CHG 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END

        HeaderKubun = New String() {"0"}
        DataKubun = New String() {"1", "2", "3", "4"}
        TrailerKubun = New String() {"9"}
    End Sub

End Class
