using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] bool shawAttack;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
		{
			p.SetSilk(1);
			Enemy target = other.GetComponent<Enemy>();
			if (target != null) 
				target.TakeDamage(p.atkDmg, p.self);
		}
	}
}
