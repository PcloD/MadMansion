using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireObjects : MonoBehaviour {
	[SerializeField]
	private GameObject _fireParticlePrefab;
	[SerializeField]
	private float speedY=0.5f;
	[SerializeField]
	private float timeIntervel=1f;
	[SerializeField]
	private int firstCreate=40;
	[SerializeField]
	private int maxCount=100;
	[SerializeField]
	private float maxYPos=3f;
	[SerializeField]
	private float posRange=1f;
	private Queue<Transform> existingParticles = new Queue<Transform>();
	private Queue<Transform> nonexistingParticles = new Queue<Transform>();

	// Use this for initialization
	void Start () {
		StartCoroutine(SpawnNewParticle());
	}
	void Awake(){
		for (int index=0; firstCreate<20; index++ )
		{
			Transform newTransform=UnusedParticle;
		}
	}

	private Transform UnusedParticle {
		get {
			Vector3 randomPos = new Vector3(Random.Range(-0.3f,0.3f), 0,Random.Range(-0.3f,0.3f)) + transform.position;
			if (nonexistingParticles.Count > 0) {
				Transform reusedFireTransform = nonexistingParticles.Dequeue(); 
				existingParticles.Enqueue(reusedFireTransform);
				reusedFireTransform.gameObject.SetActive(true);
				reusedFireTransform.position = randomPos;
				return reusedFireTransform;
			} else {
				GameObject newFireParticleObject = Instantiate(_fireParticlePrefab, randomPos, Quaternion.identity) as GameObject;
				Transform newFireTransform = newFireParticleObject.transform;
				existingParticles.Enqueue(newFireTransform);
				return newFireTransform;
			}
		}
	}

	private void CleanupOldest () {
		Transform oldest = existingParticles.Dequeue();
		oldest.gameObject.SetActive(false);
		nonexistingParticles.Enqueue(oldest);
	}

	private void MoveParticleInOneFrame(Transform T){
		T.localScale=T.localScale*1f;
		T.position=T.position+new Vector3(Random.Range(-posRange,posRange),speedY,Random.Range(-posRange,posRange)) * Time.deltaTime;
		/*if (T.position.x>0.3f)
		{
			T.position=new Vector3(0.2f+Random.Range(-0.03f,0.03f),T.position.y,T.position.z);
		}
		else if (T.position.x<-0.3f)
		{
			T.position=new Vector3(-0.2f+Random.Range(-0.03f,0.03f),T.position.y,T.position.z);
		}
		if (T.position.z>0.3f)
		{
			T.position=new Vector3(T.position.x,T.position.y,0.2f+Random.Range(-0.03f,0.03f));
		}
		else if (T.position.z<-0.3f)
		{
			T.position=new Vector3(T.position.x,T.position.y,-0.2f+Random.Range(-0.03f,0.03f));
		}*/
		if (T.position.y>maxYPos)
		{
			CleanupOldest();
		}
	}

	private IEnumerator SpawnNewParticle () {
		while (true) {
			yield return new WaitForSeconds(timeIntervel);
			Transform newTransform = UnusedParticle;
		}
	}

	// Update is called once per frame
	void Update () {
		//move
		foreach (Transform F in existingParticles)
		{
			MoveParticleInOneFrame(F);

		}
			
		while (existingParticles.Count > maxCount)//a particle reaches the end of its life
		{
			CleanupOldest();
		}
	}
}
