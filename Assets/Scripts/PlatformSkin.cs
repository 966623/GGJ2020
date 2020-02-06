using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlatformSkin", menuName = "Platform Skin", order = 2)]
public class PlatformSkin : ScriptableObject
{
    public Sprite fixedSprite;
    public Sprite rippedSprite;
    public Vector2 offset;
    public Vector2 rippedOffset;
    public float spriteScale = 2f;
    public float colliderScale = 1f;
}
