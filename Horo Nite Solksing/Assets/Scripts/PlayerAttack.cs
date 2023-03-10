using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] Vector2 forceDir;
	[SerializeField] float force=5;
	[SerializeField] bool isShawAttack;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
		{
			Enemy target = other.GetComponent<Enemy>();
			if (target != null) 
			{
				target.TakeDamage(
					p.atkDmg, 
					new Vector2(p.model.localScale.x * forceDir.x, forceDir.y),
					force
				);
				if (isShawAttack)
					p.ShawRetreat();
				p.SetSilk(1);
			}
		}
	}
}
