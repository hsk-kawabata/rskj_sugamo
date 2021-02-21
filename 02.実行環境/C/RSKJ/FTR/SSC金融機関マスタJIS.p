/size 380
/isize 380
/map
{Atlas 98}--
{main:Proc}--
    {Loop 1}--
        {データ区分:Select Bin}--
            @0  binary   1:1                 , --
        {Case X'31'}--
                binary   380:380             , --
        {Case X'32'}--
            {データ種別:Select Bin}--
                @1  binary   1:1                 , --
            {Case X'31'}--
                    binary   26:26               , --
                    kanji    30:30               , --
                    binary   15:15               , --
                    kanji    30:30               , --
                    binary   279:279             , --
            {Case X'32'}--
                    binary   27:27               , --
                    kanji    30:30               , --
                    binary   91:91               , --
                    kanji    110:110             , --
                    binary   57:57               , --
                    kanji    20:20               , --
                    binary   45:45               , --
            {ElseCase}--
                    binary   380:380             , --
            {EndSelect}--
        {ElseCase}--
                binary   380:380             , --
        {EndSelect}--
    {EndLoop}--
{EndProc}--
