using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeParticles : MonoBehaviour
{
    private ParticleSystem thisParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        thisParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.playerScript.isCharging)
            Destroy(gameObject);

        ParticleSystem.VelocityOverLifetimeModule vel = thisParticleSystem.velocityOverLifetime;
        if (GameManager.Instance.playerScript.facing == -1)         // Check key input for left side (Movement)
        {
            // Flip the sprite to the proper direction
            thisParticleSystem.transform.position = GameManager.Instance.player.transform.position + new Vector3(1, -1.8f, -2);
            thisParticleSystem.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (GameManager.Instance.playerScript.facing == 1)     // Check key input for right side (Movement)
        {
            // Flip the sprite to the proper direction
            thisParticleSystem.transform.position = GameManager.Instance.player.transform.position + new Vector3(-1, -1.8f, -2);
            thisParticleSystem.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (GameManager.Instance.playerScript.chargeTime > 3)
        {
            float temp = -Time.deltaTime + Mathf.Abs(vel.orbitalZ.constant);
            vel.orbitalZ = -temp;
        }
    }
}
