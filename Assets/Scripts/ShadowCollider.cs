using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;

public class ShadowCollider : MonoBehaviour {

    //private EdgeCollider2D edgeCollider;
    private PolygonCollider2D edgeCollider;
    private Camera cam;

    private Vector2[] edgePoints;

    private LineRenderer lineRenderer;

    public float worldWidth = 13;
    public float worldTop = -8.25f;
    public float worldBottom = 2.5f;
    private float worldHeight;

    public float spacing = 0.5f;
    private int pixelSpacing = 14;



    // Use this for initialization
    void Start()
    {
        edgeCollider = GetComponent<PolygonCollider2D>();
        cam = GetComponentInParent<Camera>();

        List<Vector2> edgePointsList = new List<Vector2>();
        for (float x = 0; x <= worldWidth; x += spacing)
        {
            edgePointsList.Add(new Vector2(x, worldBottom));
        }

        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.SetColors(Color.magenta, Color.magenta);
        lineRenderer.SetWidth(0.2f, 0.2f);
        lineRenderer.SetVertexCount(edgePointsList.Count);
        lineRenderer.sortingLayerName = "Sign";
        for (int i = 0; i < edgePointsList.Count; i++)
        {
            lineRenderer.SetPosition(i, edgePointsList[i]);
        }


        edgePointsList.Add(new Vector2(worldWidth, worldBottom));
        edgePointsList.Add(new Vector2(0, worldBottom));


        edgePoints = edgePointsList.ToArray();
        edgeCollider.points = edgePoints;

        worldHeight = Mathf.Abs(worldBottom - worldTop);



    }

    // Update is called once per frame
    void Update()
    {

    }



    public void SetPointsFromTexture(Texture2D tex, float greyscaleThreshold)
    {
        float spaceConstant = spacing / worldWidth;
        pixelSpacing = (int)(spaceConstant * tex.width);

        bool foundSpot = false;

        int x = 0;
        for (int i = 0; i < edgePoints.Length - 2; i++)
        {
            foundSpot = false;
            x = i * pixelSpacing;
            for (int y = tex.height - 1; y >= 0; y -= pixelSpacing)
            {
                Color rgb = tex.GetPixel(x, y);
                if (rgb.grayscale < greyscaleThreshold)
                {
                    /*
                    Vector2 newPoint = new Vector2(edgePoints[i].x, GetWorldY(y));
                    edgePoints[i] = Vector2.MoveTowards(edgePoints[i], newPoint, 0.1f);
                    lineRenderer.SetPosition(i, newPoint);
                    */

                    // eventually change this move, instead of set
                    edgePoints[i].y = GetWorldY(y);
                    lineRenderer.SetPosition(i, new Vector2(edgePoints[i].x, edgePoints[i].y));
                    foundSpot = true;
                    break;
                }

                if (!foundSpot)
                {
                    edgePoints[i].y = worldBottom;
                    lineRenderer.SetPosition(i, new Vector2(edgePoints[i].x, edgePoints[i].y));
                }
                //tex.SetPixel(x, y, Color.red);
            }
        }
        edgeCollider.points = edgePoints;
    }



    public void ShowPointsFromTexture(Texture2D tex, float greyscaleThreshold)
    {
        float spaceConstant = spacing / worldWidth;
        pixelSpacing = (int)(spaceConstant * tex.width);

        bool foundSpot = false;

        int x = 0;
        for (int i = 0; i < edgePoints.Length - 2; i++)
        {
            x = i * pixelSpacing;
            foundSpot = false;
            for (int y = tex.height - 1; y >= 0; y -= pixelSpacing)
            {
                Color rgb = tex.GetPixel(x, y);
                if (rgb.grayscale < greyscaleThreshold)
                {
                    edgePoints[i].y = GetWorldY(y);
                    lineRenderer.SetPosition(i, new Vector2(edgePoints[i].x, edgePoints[i].y));
                    foundSpot = true;
                    break;
                }

                if (!foundSpot)
                {
                    edgePoints[i].y = worldBottom;
                    lineRenderer.SetPosition(i, new Vector2(edgePoints[i].x, edgePoints[i].y));
                }

                tex.SetPixel(x, y, Color.red);
            }
        }
        edgeCollider.points = edgePoints;
    }




    private float GetWorldY(int pixelY)
    {
        return (((float)pixelY / (float)cam.pixelHeight) * worldHeight) + worldBottom;
    }


}
