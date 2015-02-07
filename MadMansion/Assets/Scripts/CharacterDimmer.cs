using UnityEngine;
using System.Collections;

public class CharacterDimmer : MonoBehaviour
{

	[SerializeField]
	private Renderer
		_renderer;
	private Color[] _color;

	void Awake ()
	{
		_color = new Color[_renderer.materials.Length];
		for (int i = 0; i<_renderer.materials.Length; i++) {
			_color [i] = _renderer.materials [i].color;
		}
	}

	public void Dim ()
	{
		for (int i = 0; i<_renderer.materials.Length; i++) {
			_renderer.materials [i].color = Color.black;
		}
	}

	public void Undim ()
	{
		for (int i = 0; i<_renderer.materials.Length; i++) {
			_renderer.materials [i].color = _color [i];
		}
	}
}
