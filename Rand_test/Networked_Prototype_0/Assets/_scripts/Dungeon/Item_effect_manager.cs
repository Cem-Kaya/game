using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
	
public class Item_effect_manager : NetworkBehaviour
{
	public uint  delay_dec ;
	public int speed_up ;

	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D hitObject)
	{		
		if (hitObject.gameObject.layer == 3)
		{
			box_mover player = hitObject.gameObject.GetComponent<box_mover>();
			player.dec_fire_rate_delay(delay_dec );
			player.inc_speed(speed_up);
		}
		
		
	}


}