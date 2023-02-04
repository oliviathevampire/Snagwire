using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour{
    public enum MovementState {
        Idle=0,
        Walk=1,
        Crawl=2,
        Run=3,
        Dash=4,
        Jump=5,
        Fall=6
    }
    
    //Different components needed
    private Rigidbody2D _rb;
    private Collider2D _bc;
    private SpriteRenderer _sr;
    private Animator _animator;
    
    //Horizontal movement
    private float _moveX;
    
    //Movement states
    private MovementState _currentState;
    private MovementState _lastState;
    
    [Header("Gravity Values")]
    public float fallGravity;
    public float defaultGravity;
    public float dashGravity;

    [Header("Start Information")]
    public LayerMask groundLayer;
    
    [Header("Speeds")]
    public float walkSpeed;
    public float runSpeed;
    
    [Header("Jumping")]
    public float idleJumpForce;
    public float walkJumpForce;
    public float runJumpForce;
    
    [Header("Coyote")]
    public float coyoteTime;
    private float _currentCoyoteTime;
    
    //Animation names
    private static readonly int State = Animator.StringToHash("State");

    [Header("Air Jumps")]
    public int airJumpAmount;
    private int _currentAirJumps;

    [Header("Dashing")]
    [FormerlySerializedAs("dashSpeed")] public float dashForce;
    public float dashTime;
    public float dashCoolDown;
    public TrailRenderer trail;
    private bool _canDash = true;

    public GameObject plug;
    public FixedJoint2D plugJoint;
    
    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _bc = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        _currentAirJumps = airJumpAmount;
        _currentCoyoteTime = coyoteTime;
    }

    private void Update() {
        if (_currentState is MovementState.Dash) return;
        
        //Check if the player can dash
        if (Input.GetKeyDown(KeyCode.LeftControl) && _canDash) StartCoroutine(Dash());
    
        var velocity = _rb.velocity;
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        
        //Cancel jump
        if (Input.GetKeyUp(KeyCode.Space) && _rb.velocity.y > 0)
            _rb.velocity = new Vector2(_rb.velocity.x, velocity.y / 2);

        if (IsGrounded()) {
            _currentAirJumps = airJumpAmount;
            _currentCoyoteTime = coyoteTime;
            _rb.gravityScale = defaultGravity;
        } else if (_currentCoyoteTime > 0) {
            _currentCoyoteTime -= Time.deltaTime;
        }

        if (_rb.velocity.y < -0.1 && !IsGrounded()) {
            _rb.gravityScale = fallGravity;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            TogglePickup();
        }

        DoAnimations();
    }

    private void TogglePickup() {
        if (plugJoint.enabled) {
            plugJoint.enabled = false;
            plugJoint.GetComponent<Rigidbody2D>().AddForce(_rb.velocity.normalized * 5, ForceMode2D.Impulse);
            return;
        }

        Vector3 plugPos = plug.transform.position;
        if (Vector3.Distance(plugPos, transform.position) > 1) {
            return;
        }

        if (plugPos.y <= transform.position.y) {
            plugPos.y = transform.position.y + 0.01f;
            plug.transform.position = plugPos;
        }
        plugJoint.enabled = true;
    }

    private void FixedUpdate() {
        if (_currentState is MovementState.Dash) return;
        
        var velocity = _rb.velocity;
        
        _moveX = Input.GetAxis("Horizontal");
        //Movement
        if (Input.GetKey(KeyCode.LeftShift)) {
            _rb.velocity = new Vector2(_moveX * runSpeed, velocity.y);
            _currentState = MovementState.Run;
        } else {
            _rb.velocity = new Vector2(_moveX * walkSpeed, velocity.y);
            _currentState = MovementState.Walk;
        }
    }

    private void Jump() {
        var velocity = _rb.velocity;
        
        if (_currentAirJumps <= 0 && _currentCoyoteTime <= 0) return;

        if (IsGrounded() || _currentCoyoteTime > 0) {
            _rb.velocity = _currentState switch {
                MovementState.Idle => new Vector2(velocity.x, idleJumpForce),
                MovementState.Walk => new Vector2(velocity.x, walkJumpForce),
                MovementState.Run => new Vector2(velocity.x, runJumpForce),
                _ => _rb.velocity
            };
        }
        else {
            if (_currentAirJumps > 0) {
                _currentAirJumps--;
                _rb.velocity = _currentState switch {
                    MovementState.Idle => new Vector2(velocity.x, idleJumpForce),
                    MovementState.Walk => new Vector2(velocity.x, walkJumpForce),
                    MovementState.Run => new Vector2(velocity.x, runJumpForce),
                    _ => _rb.velocity
                };
            }
        }

        _currentCoyoteTime = 0;
    }

    private void DoAnimations() {
        if (_currentState == MovementState.Dash) return;
        
        switch (_moveX) {
            case 0: _currentState = MovementState.Idle; break;
            case < 0: _sr.flipX = true; break;
            case > 0: _sr.flipX = false; break;
        }

        // if (_currentState == _lastState) return;
        // _animator.SetInteger(State, (int)_currentState);
        // _lastState = _currentState;
    }

    private bool IsGrounded() {
        var bounds = _bc.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    }
    
    public void SetState(MovementState movementState) {
        _lastState = _currentState;
        _currentState = movementState;
    }

    public void SmoothStop() {
        StartCoroutine(SmoothStop(0.5f));
    }

    private IEnumerator SmoothStop(float duration) {
        var initialSpeed = _rb.velocity.x;
        var elapsedTime = 0f;
        float smoothX;

        while (elapsedTime < duration) {
            smoothX = Mathf.SmoothStep(initialSpeed, 0f, elapsedTime / duration);
            _rb.velocity = new Vector2(smoothX, _rb.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        smoothX = 0f;
        _rb.velocity = new Vector2(smoothX, _rb.velocity.y);
    }

    private IEnumerator Dash() {
        var tempState = _currentState;
        _currentState = MovementState.Dash;
        
        _canDash = false;
        trail.emitting = true;
        
        var orgGravity = _rb.gravityScale;
        _rb.gravityScale = dashGravity;

        var direction = _sr.flipX ? -1 : 1;
        _rb.velocity = new Vector2(dashForce * direction, 0f);
        yield return new WaitForSeconds(dashTime);

        //Reset variables to original values
        _rb.gravityScale = orgGravity;
        trail.emitting = false;
        _currentState = tempState;
        
        //Wait on cooldown
        yield return new WaitForSeconds(dashCoolDown);
        _canDash = true;
    }
}