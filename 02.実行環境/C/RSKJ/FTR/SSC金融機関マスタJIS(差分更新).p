/size 380
/isize 380
/map
{Atlas 98}--
{main:Proc}--
    {Loop 1}--
        {データ区分:Select Bin}--
            @0  binary   1:1                 , --
        {Case X'F1'}--
                ank      380:380             , --
        {Case X'F2'}--
            {データ種別:Select Bin}--
                @1  binary   1:1                 , --
            {Case X'F1'}--
                    ank      26:26               , --
                    kanji    30:30               , --
                    ank      15:15               , --
                    kanji    30:30               , --
                    ank      279:279             , --
            {Case X'F2'}--
                    ank      27:27               , --
                    kanji    30:30               , --
                    ank      91:91               , --
                    kanji    110:110             , --
                    ank      57:57               , --
                    kanji    20:20               , --
                    ank      45:45               , --
            {ElseCase}--
                    binary   380:380             , --
            {EndSelect}--
        {ElseCase}--
                ank      380:380             , --
        {EndSelect}--
    {EndLoop}--
{EndProc}--
