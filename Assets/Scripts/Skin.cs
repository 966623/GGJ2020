using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkin", menuName = "Skin", order = 1)]
public class Skin : ScriptableObject
{
    public RuntimeAnimatorController idle;
    public RuntimeAnimatorController run;
    public RuntimeAnimatorController jump;
    public RuntimeAnimatorController fall;
    public RuntimeAnimatorController tape;
    public RuntimeAnimatorController untape;
    public RuntimeAnimatorController death;
}
