using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeManager : MonoBehaviour {

    SpriteRenderer spriteRenderer;

    public void InitPrototype (Sprite initSprite) {
        spriteRenderer = transform.Find("SpriteRenderer").GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = initSprite;
        spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
        Invoke("SetPoints", 0.1f);
    }

    private void SetPoints () {
        Transform prototypeTransform = transform.Find("PrototypeTransform");
        prototypeTransform.gameObject.AddComponent<PolygonCollider2D>();
        prototypeTransform.GetComponent<PolygonCollider2D>().usedByEffector = true;

        Vector2 vectorScale = new Vector2();

        vectorScale.x = ( prototypeTransform.GetComponent<RectTransform>().rect.width * 100 ) /
            spriteRenderer.GetComponent<SpriteRenderer>().sprite.rect.size.x;
        vectorScale.y = ( prototypeTransform.GetComponent<RectTransform>().rect.height * 100) /
            spriteRenderer.GetComponent<SpriteRenderer>().sprite.rect.size.y;

        //Debug.Log(vectorScale);

        Debug.Log(spriteRenderer.GetComponent<PolygonCollider2D>().points.Length);
        prototypeTransform.GetComponent<PolygonCollider2D>().points =
            ScalePoints(spriteRenderer.GetComponent<PolygonCollider2D>().points, vectorScale);
    }

    private Vector2[] ScalePoints (Vector2[] points, Vector2 scale) {
        for (int i = 0 ; i < points.Length ; i++) {
            Vector2 point = points[i];
            point.x = point.x * scale.x;
            point.y = point.y * scale.y;
            points[i] = point;
        }
        return points;
    }
}
