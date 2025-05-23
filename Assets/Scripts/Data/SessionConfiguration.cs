using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionConfiguration
{
    public int subject;
    public bool isCircular;
    public int[] a;
    public int[] w;
    public int trials;
    public int practice;

    public bool isValid()
    {
        return subject > 0 
            && trials >= 8
            && trials > practice 
            && practice >= 0 
            && a.Length > 0 && w.Length > 0;
    }
}
