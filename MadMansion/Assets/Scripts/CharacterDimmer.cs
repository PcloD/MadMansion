using UnityEngine;
using System.Collections;

public class CharacterDimmer : MonoBehaviour {

	[SerializeField]
	private Renderer _renderer;

	public void Dim() {
		for(int i = 0; i<_renderer.materials.Length; i++) {
			_renderer.materials[i].color = Color.black;
		}
	}
}
