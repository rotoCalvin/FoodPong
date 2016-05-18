using UnityEngine;
using System.Collections;

public class BoundingBox2D {

    public Vector2 topLeft { get; private set; }
    public Vector2 bottomLeft { get; private set; }
    public Vector2 topRight { get; private set; }
    public Vector2 bottomRight { get; private set; }

    public Vector2 middle { get; private set; }

    public BoundingBox2D(GameObject obj)
    {
        var objTransform = obj.transform;

        var halfSizeX = objTransform.localScale.x / 2;
        var halfSizeY = objTransform.localScale.x / 2;

        topLeft = new Vector2(objTransform.position.x - halfSizeX, objTransform.position.y - halfSizeY);
        bottomLeft = new Vector2(objTransform.position.x - halfSizeX, objTransform.position.y + halfSizeY);
        topRight = new Vector2(objTransform.position.x + halfSizeX, objTransform.position.y - halfSizeY);
        bottomRight = new Vector2(objTransform.position.x + halfSizeX, objTransform.position.y + halfSizeY);

        middle = new Vector2(objTransform.position.x, objTransform.position.y);
    }

}
