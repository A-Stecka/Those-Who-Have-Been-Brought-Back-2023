using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowController : MonoBehaviour
{
    public Gradient gradient;
    public float time, timerSpeed;

    private SpriteRenderer _spriteRenderer;
    private float _timer;

    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _timer = time * timerSpeed;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > time)
        {
            _timer = 0.0f;
        }
            
        _spriteRenderer.color = gradient.Evaluate(_timer / time);
    }
}
