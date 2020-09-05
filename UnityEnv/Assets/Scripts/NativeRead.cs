﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_ANDROID

public class NativeRead
{
#if UNITY_IOS
	public const string libName = "__Internal";
#else
    public const string libName = "Utility";
#endif


    [DllImport(libName)]
    public static extern int ReadAssetsBytes(string name, ref IntPtr ptr);

    [DllImport(libName)]
    public static extern int ReadAssetsBytesWithOffset(string name, ref IntPtr ptr, int offset, int length);

    [DllImport(libName)]
    public static extern int Add(int a, int b);

    [DllImport(libName)]
    public static extern int ReadRawBytes(string name, ref IntPtr ptr);

    [DllImport(libName)]
    public static extern void ReleaseBytes(IntPtr ptr);

}
#endif