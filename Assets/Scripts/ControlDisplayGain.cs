using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ControlDisplayGain
{
    public enum Type{ linear }
    public Type type;

    private float _sensitivity;

    public ControlDisplayGain(Type type, float sensitivity)
    {
        this.type = type;
        this._sensitivity = sensitivity;
    }

    public Vector2 GainedDelta(Vector2 delta)
    {
        return _sensitivity * delta;
    }

}
