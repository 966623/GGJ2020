using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformPreview : MonoBehaviour
{

    public Platform instance;
    private void OnRenderObject()
    {
        //if (instance.IsFixedInitial)
        //{
        //    instance.renderer.sprite = instance.skin.fixedSprite;
        //    instance.renderer.transform.localPosition = instance.skin.offset;
        //}
        //else
        //{
        //    instance.renderer.sprite = instance.skin.rippedSprite;
        //    instance.renderer.transform.localPosition = instance.skin.rippedOffset;
        //}
        //instance.renderer.transform.localScale = new Vector3(instance.skin.spriteScale, instance.skin.spriteScale, 1);
        //instance.collider.size = new Vector2(2, instance.skin.colliderScale);
        //instance.collider.offset = new Vector2(0, 1 - instance.skin.colliderScale / 2);
    }

}
