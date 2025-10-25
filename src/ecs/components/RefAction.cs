namespace HUI;

public delegate void RefAction<T>(ref T arg);
public delegate void RefAction<T1, T2>(ref T1 arg1, ref T2 arg2);
public delegate void SecondRefAction<T1, T2>(T1 arg1, ref T2 arg2);