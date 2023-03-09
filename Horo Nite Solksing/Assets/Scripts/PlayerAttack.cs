using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] bool isShawAttack;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
		{
			p.SetSilk(1);
			Enemy target = other.GetComponent<Enemy>();
			if (target != null) 
				target.TakeDamage(p.atkDmg, p.self);
			if (isShawAttack)
				p.ShawRetreat();
		}
	}
}
