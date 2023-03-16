using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] Vector2 forceDir;
	[SerializeField] float force=5;
	[SerializeField] bool isShawAttack;
	[SerializeField] bool isStabAttack;
	[SerializeField] bool isGossamerStorm;
	[SerializeField] GameObject strikePs;
	[SerializeField] float offset=15;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("EnemyAttack") && isShawAttack)
		{
			p.ShawRetreat();
		}
		if (other.CompareTag("Enemy"))
		{
			Enemy target = other.GetComponent<Enemy>();
			Vector2 temp = (other.transform.position - transform.position).normalized;
			float angleZ = 
				Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;
			Instantiate(
				strikePs, 
				other.transform.position, 
				Quaternion.Euler(0,0,angleZ + offset * temp.x)
			);

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
