using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System; 

public class ParamFirework : MonoBehaviour
{
	public int _band;
	public float _startScale, _scaleMultiplier;
    public AudioPeer audioPeer;
    // Start is called before the first frame update
    [SerializeField]
	private VisualEffect visualEffect;

	[SerializeField, Range(10,80)]
	private float explosionSize = 10;

	[SerializeField]
	private float rocketLifetime = 5;

	[SerializeField]
	private int fireWorkSpawnRate = 1;
    void Start()
    {
        _band = 6;
    }

    // Update is called once per frame
    void Update()
    {
    	explosionSize = (audioPeer.bandBuffer[_band] * _scaleMultiplier) + _startScale;
        visualEffect.SetFloat("ExplosionSize", explosionSize);

        if((audioPeer.bandBuffer[_band] * _scaleMultiplier) + _startScale > 17)
        {
        	rocketLifetime = 0;
        }
        else
        {
        	rocketLifetime = 5;
        }
        visualEffect.SetFloat("RocketLifetime", rocketLifetime);

        fireWorkSpawnRate = (int)Math.Truncate(audioPeer.bandBuffer[_band] + _startScale);
        visualEffect.SetInt("FireWorkSpawnRate", fireWorkSpawnRate);
    }
}

