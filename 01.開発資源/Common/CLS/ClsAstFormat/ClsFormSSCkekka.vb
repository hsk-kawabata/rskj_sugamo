Imports System
Imports CASTCommon.ModPublic

Public Class ClsFormatSSCKekka
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 210

#Region "ヘッダレコード"
    Public Structure T_HEAD
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' データ区分
        Dim DATA_CODE As String         ' データコード
        Dim CODE_KBN As String          ' コード区分
        Dim DUMMY As String             ' ダミー
        Dim CYCLE_NO As String          ' サイクル番号
        Dim KURIKOSI As String          ' 繰越表示
        Dim SOUSINBI As String          ' 送信日
        Dim KIN_NO As String            ' 仕向金融機関コード
        Dim DUMMY2 As String            ' ダミー

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' データ取得
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(DATA_CODE, 4), _
                    SubData(CODE_KBN, 1), _
                    SubData(DUMMY, 1), _
                    SubData(CYCLE_NO, 2), _
                    SubData(KURIKOSI, 1), _
                    SubData(SOUSINBI, 4), _
                    SubData(KIN_NO, 4), _
                    SubData(DUMMY2, 192) _
                    })
            End Get

            ' データ設定
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                DATA_CODE = CuttingData(Value, 4)
                CODE_KBN = CuttingData(Value, 1)
                DUMMY = CuttingData(Value, 1)
                CYCLE_NO = CuttingData(Value, 2)
                KURIKOSI = CuttingData(Value, 1)
                SOUSINBI = CuttingData(Value, 4)
                KIN_NO = CuttingData(Value, 4)
                DUMMY2 = CuttingData(Value, 192)
            End Set
        End Property
    End Structure
#End Region

#Region "データレコード"
    Public Structure T_DATA
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' データ区分
        Dim FILENAME As String          ' ファイル名
        Dim ERROR_FILENO As String      ' エラーファイル番号
        Dim DATA_RECORD As String       ' データレコード
        Dim CYCLE_NO As String          ' サイクル番号
        Dim ERR_CODE() As String        ' エラーコード１～２０
        Dim ERR_RECO() As String        ' エラーレコード番号１～２０
        Dim DUMMY As String             ' ダミー

        ' 初期化
        Public Sub Init()
            ERR_CODE = CType(Array.CreateInstance(GetType(String), 20), String())
            ERR_RECO = CType(Array.CreateInstance(GetType(String), 20), String())
        End Sub

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' データ取得
            Get
                Dim RetString As String
                RetString = String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(FILENAME, 12), _
                    SubData(ERROR_FILENO, 2), _
                    SubData(DATA_RECORD, 4), _
                    SubData(CYCLE_NO, 2) _
                    })

                For i As Integer = 0 To 19
                    RetString &= SubData(ERR_CODE(i), 2)
                    RetString &= SubData(ERR_RECO(i), 2)
                Next i

                RetString &= SubData(DUMMY, 49)

                Return RetString
            End Get

            ' データ設定
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                FILENAME = CuttingData(Value, 12)
                ERROR_FILENO = CuttingData(Value, 2)
                DATA_RECORD = CuttingData(Value, 4)
                CYCLE_NO = CuttingData(Value, 2)
                For i As Integer = 0 To 19
                    ERR_CODE(i) = CuttingData(Value, 2)
                    ERR_RECO(i) = CuttingData(Value, 5)
                Next i
                DUMMY = CuttingData(Value, 49)
            End Set
        End Property
    End Structure
#End Region

#Region "トレーラレコード"
    Public Structure T_TRAILER
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' データ区分
        Dim TOTAL_KEN As String         ' 合計件数
        Dim ERROR_TOTAL_KEN As String   ' エラー合計件数
        Dim DUMMY As String             ' ダミー

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' データ取得
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(TOTAL_KEN, 6), _
                    SubData(ERROR_TOTAL_KEN, 6), _
                    SubData(DUMMY, 197) _
                    })
            End Get

            ' データ設定
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                TOTAL_KEN = CuttingData(Value, 6)
                ERROR_TOTAL_KEN = CuttingData(Value, 6)
                DUMMY = CuttingData(Value, 197)
            End Set
        End Property
    End Structure
#End Region

#Region "エンドレコード"
    Public Structure T_END
        Implements CFormat.IFormat

        Dim DATA_KBN As String          ' データ区分
        Dim DUMMY As String             ' ダミー

        Public Property Data() As String Implements CFormat.IFormat.Data
            ' データ取得
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(DATA_KBN, 1), _
                    SubData(DUMMY, 209) _
                    })
            End Get

            ' データ設定
            Set(ByVal Value As String)
                DATA_KBN = CuttingData(Value, 1)
                DUMMY = CuttingData(Value, 209)
            End Set
        End Property
    End Structure
#End Region

End Class
