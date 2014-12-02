using UnityEngine;
using System.Collections;

public class TVFurniture : MonoBehaviour, IFurniture {
	[SerializeField]
    private int _columns = 37;
    [SerializeField]
    private int _rows = 37;
    [SerializeField]
    private float _framesPerSecond = 12f;
    [SerializeField]
    private int _materialIndex = 0;
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private int _totalFrames = 100;

    //the current frame to display
    private int index = 0;

    public Vector3 Position {
    	get { return transform.position; }
    }

    void Start()
    {
        StartCoroutine(UpdateTiling());

        //set the tile size of the texture (in UV units), based on the _rows and _columns
        Vector2 size = new Vector2(1f / _columns, 1f / _rows);
        _renderer.materials[_materialIndex].SetTextureScale("_MainTex", size);
    }

    private IEnumerator UpdateTiling()
    {
        while (true) {
            //move to the next index
            index++;
            if (index >= _totalFrames)
                index = 0;

            //split into x and y indexes
            Vector2 offset = new Vector2((float)index / _columns - (index / _columns), //x index
                                          (index / _columns) / (float)_rows);          //y index

            _renderer.materials[_materialIndex].SetTextureOffset("_MainTex", offset);

            yield return new WaitForSeconds(1f / _framesPerSecond);
        }

    }
}