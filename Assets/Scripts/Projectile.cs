using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float speed;

    private float _direction;
    private bool _hit;
    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private float _lifeTime;
    
    private static readonly int Explode = Animator.StringToHash("Explode");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_hit) return;
        float movementSpeed = speed * Time.deltaTime * _direction;
        transform.Translate(movementSpeed, 0, 0);

        _lifeTime += Time.deltaTime;
        if(_lifeTime > 5) 
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _hit = true;
        _boxCollider.enabled = false;
        _animator.SetTrigger(Explode);
    }

    public void SetDirection(float direction)
    {
        _lifeTime = 0;
        _direction = direction;
        gameObject.SetActive(true);
        _hit = false;
        _boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
