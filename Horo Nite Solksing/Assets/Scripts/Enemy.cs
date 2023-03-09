using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] int hp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

	public void TakeDamage(int dmg, Transform opponent)
	{
		StartCoroutine( TakeDamageCo(dmg, opponent) );
	}

	IEnumerator TakeDamageCo(int dmg, Transform opponent)
	{
		hp -= dmg;
		yield return new WaitForSeconds(0.1f);
	}
}
