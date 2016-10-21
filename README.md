# FPU Comment Generator
Comment generator for FPU code.     

```Assembler
finit;              0~||
;                   
fld QWORD PTR x;    1~| x |
fmul st, st(0);     1~| x*x |
fld QWORD PTR y;    2~| x*x | y |
fmul st, st(0);     2~| x*x | y*y |
faddp st(1),st(0);  1~| x*x+y*y |
fsqrt;=radius;      1~| radius |
;                   
fld QWORD PTR u;    2~| radius | u |
fld QWORD PTR v;    3~| radius | u | v |
fld st(1);          4~| radius | u | v | u |
fadd st(0), st(1);  4~| radius | u | v | u+v |
fmul st(0), st(3);  4~| radius | u | v | (u+v)*radius |
fstp QWORD PTR u;   3~| radius | u | v |
fsubp st(1), st(0); 2~| radius | u-v |
fmul st(0), st(1);  2~| radius | (u-v)*radius |
fstp QWORD PTR v;   1~| radius |
fstp;               0~||
```