using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class customMaterial
{
    public string name;
    public Dictionary<materialProperty, Texture2D> texturemaps;
    public Dictionary<materialProperty, double> values;
    public string shader;

}

public enum materialProperty
{
    diffuse,
    alpha,
    normal,
    roughness,
    metal,
    subsurface,
    specular
}