using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBlocker : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    [SerializeField] private float imageSize = 100f;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RawImage image;
    public void UpdateUVS(int blockID)
    {
        Rect rect = canvas.pixelRect;
        float yScale = rect.height / imageSize;
        float xScale = rect.width / imageSize;
        float scale = Mathf.Max(xScale, yScale);
        gameObject.transform.localScale = Vector3.one * scale;
        if(blockID == BlockID.Air)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Vector2[] vectors = BlockMesh.Get(blockID).front.GetUVs();
            Rect rectangle = new Rect(vectors[0].x, vectors[0].y, vectors[2].x - vectors[0].x, vectors[2].y - vectors[0].y);
            image.uvRect = rectangle;
            gameObject.SetActive(true);
            //image.color = new Color(image.color.r, image.color.g, image.color.b, 1.1f);
        }
    }
}
