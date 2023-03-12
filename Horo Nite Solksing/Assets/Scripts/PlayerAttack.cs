using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] Vector2 forceDir;
	[SerializeField] float force=5;
	[SerializeField] bool isShawAttack;
	[SerializeField] bool isStabAttack;
	[SerializeField] bool isGossamerStorm;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
		{
			Enemy target = other.GetComponent<Enemy>();
			if (target != null) 
			{
				int dmg = !isStabAttack ? (!isGossamerStorm ? p.atkDmg : p.gossamerDmg) : p.stabDmg;
				target.TakeDamage(
					dmg, 
					isGossamerStorm ? transform : null,
					new Vector2(p.model.localScale.x * forceDir.x, forceDir.y),
					force
				);
				if (isShawAttack)
					p.ShawRetreat();

				// simple attack
				if (!isStabAttack && !isGossamerStorm)
					p.SetSilk(1);
			}
		}
	}
}
