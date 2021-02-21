/size 262
/isize 256
/map
{Atlas 98}--
{main:Proc}--
    {Loop 1}--
             binary   32:32               , --
        {kubun:Select Bin}--
            @32  binary   4                   , --
        {Case X'20202020'}--
                 binary   4:4                 , --
                 ank      0:3                 , --
        {ElseCase}--
                 packzone u7:u7               , --
        {EndSelect}--
        {kubun2:Select Bin}--
            @36  binary   4                   , --
        {Case X'20202020'}--
                 binary   4:4                 , --
                 ank      0:3                 , --
        {ElseCase}--
                 packzone u7:u7               , --
        {EndSelect}--
             binary   216:216             , --
    {EndLoop}--
{EndProc}--
