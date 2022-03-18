using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	static int s_DeadHash = Animator.StringToHash("Dead");
	static int s_RunStartHash = Animator.StringToHash("runStart");
	static int s_MovingHash = Animator.StringToHash("Moving");
	static int s_JumpingHash = Animator.StringToHash("Jumping");
	static int s_JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
	static int s_SlidingHash = Animator.StringToHash("Sliding");
	static int s_StartHash = Animator.StringToHash("Start");
	static int s_Win = Animator.StringToHash("Win");
	static int s_DanceDelay = Animator.StringToHash("Countdown");
	static int s_Hit = Animator.StringToHash("Hit");


	public float laneChangeSpeed = 1.0f;

    public int currentLife = 0;

    public bool isSliding;

    public float slideStart;

    public float isRunning;

	public int maxLife = 3;

	protected bool m_IsRunning;
	protected bool m_isChangeLane;
	protected float m_JumpStart;
	protected bool m_Jumping;

	protected bool m_Sliding;
	protected float m_SlideStart;

	public float jumpLength = 2.0f;     // Distance jumped
	public float jumpHeight = 1.2f;

	public float slideLength = 2.0f;

	protected const int k_StartingLane = 1;
	protected const float k_GroundingSpeed = 80f;
	protected const float k_ShadowRaycastDistance = 100f;
	protected const float k_ShadowGroundOffset = 0.01f;
	protected const float k_TrackSpeedToJumpAnimSpeedRatio = 0.6f;
	protected const float k_TrackSpeedToSlideAnimSpeedRatio = 0.9f;

	private int laneDistance = 5;
	private Rigidbody rigid;
	private Animator characterAnimator;

	protected Vector2 m_StartingTouch;
	protected bool m_IsSwiping = false;
    private Vector3 m_TargetPosition;
    private int m_CurrentLane;

	[SerializeField] private GameObject mainCamera;

	[SerializeField] private List<LevelConfig> levels = new List<LevelConfig>();

	private int currentLevel = 0;
	private LevelConfig currentLevelConfig;
	private bool isFinish = false;
	private int currentTrigger;
	private bool isCountdown;

	private CapsuleCollider characterCollider;
	private bool isGameover;

	private Vector2 clickPos;
	private Vector2 releasePos;



	private void Awake()
    {
		isGameover = false;
		currentLife = maxLife;
		isCountdown = true;
		currentTrigger = 0;
		m_CurrentLane = 1;
		m_TargetPosition = Vector3.zero;
		characterAnimator = this.GetComponent<Animator>();
		rigid = this.GetComponent<Rigidbody>();
		characterCollider = this.GetComponent<CapsuleCollider>();
		Debug.Log(characterAnimator);
	}

	private void ApplyRunCollider()
    {
		characterCollider.center = new Vector3(0, 0.7f, 0);
		characterCollider.radius = 0.2f;
		characterCollider.height = 1.36f;
		characterCollider.direction = 1;
	}

	private IEnumerator ApplyJumpCollider()
	{
		characterCollider.center = new Vector3(0, 1.3f, 0);
		characterCollider.radius = 0.2f;
		characterCollider.height = 1.36f;
		characterCollider.direction = 1;
		yield return new WaitForSeconds(1f);
		ApplyRunCollider();
	}

	private IEnumerator ApplySlideCollider()
	{
		characterCollider.center = new Vector3(0, 0.2f, 0);
		characterCollider.radius = 0.2f;
		characterCollider.height = 1.36f;
		characterCollider.direction = 2; 

		yield return new WaitForSeconds(1f);
		ApplyRunCollider();
	}

	private void Start()
    {
		currentLevel = PlayerDataManager.level;

		characterAnimator.SetBool(s_DanceDelay, true);

		currentLevelConfig = levels[currentLevel];

		this.RegisterListener(GameEvent.OnFinishCoundown, OnFinishCountdownHandler);
	}

    private void OnFinishCountdownHandler(object obj)
	{
		characterAnimator.SetBool(s_DanceDelay, false);
		isCountdown = false;
		StartRunning();
	}

	public void StartRunning()
	{
		characterAnimator.Play(s_StartHash);
		StartMoving();
		if (characterAnimator)
		{
			characterAnimator.Play(s_RunStartHash);
			characterAnimator.SetBool(s_MovingHash, true);
		}
	}

	public void StartMoving()
	{
		m_IsRunning = true;
	}

	public void StopMoving()
	{
		m_IsRunning = false;
		if (characterAnimator)
		{
			characterAnimator.SetBool(s_MovingHash, false);
		}
		rigid.velocity = Vector3.zero;
	}

	private void Update()
    {
		if (Input.GetKeyDown(KeyCode.G))
        {
			FinishLevel();
		}

		if (!isCountdown && !isGameover)
        {
#if UNITY_EDITOR || UNITY_STANDALONE

			if (Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetKeyDown(KeyCode.A)))
			{
				Debug.Log("Left");
				ChangeLane(-1);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetKeyDown(KeyCode.D)))
			{
				Debug.Log("Right");
				ChangeLane(1);
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.W)) || (Input.GetKeyDown(KeyCode.W)))
			{
				if (!m_Jumping)
				{
					Debug.Log("Jump");
					StartCoroutine(Jump(0.6f));
				}
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetKeyDown(KeyCode.S)))
			{
				if (!m_Sliding)
				{
					Debug.Log("Slide");
					StartCoroutine(Slide(1f));
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				clickPos = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(0))
			{
				releasePos = Input.mousePosition;

				if (Mathf.Abs(releasePos.x - clickPos.x) > Mathf.Abs(releasePos.y - clickPos.y))
                {
					if (releasePos.x > clickPos.x)
                    {
						ChangeLane(1);
                    }
                    else
                    {
						ChangeLane(-1);
                    }
                }
                else
                {
					if (releasePos.y > clickPos.y)
                    {
						StartCoroutine(Jump(0.6f));
					}
                    else
                    {
						StartCoroutine(Slide(1f));
					}
                }
			}

#else
        // Use touch input on mobile
        if (Input.touchCount == 1)
        {
			if(m_IsSwiping)
			{
				Vector2 diff = Input.GetTouch(0).position - m_StartingTouch;

				// Put difference in Screen ratio, but using only width, so the ratio is the same on both
                // axes (otherwise we would have to swipe more vertically...)
				diff = new Vector2(diff.x/Screen.width, diff.y/Screen.width);

				if(diff.magnitude > 0.01f) //we set the swip distance to trigger movement to 1% of the screen width
				{
					if(Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
					{
						if(diff.y < 0)
						{
							StartCoroutine(Slide(1f));
						}
						else
						{
							StartCoroutine(Jump(0.6f));
						}
					}
					else
					{
						if(diff.x < 0)
						{
							ChangeLane(-1);
						}
						else
						{
							ChangeLane(1);
						}
					}
						
					m_IsSwiping = false;
				}
            }

        	// Input check is AFTER the swip test, that way if TouchPhase.Ended happen a single frame after the Began Phase
			// a swipe can still be registered (otherwise, m_IsSwiping will be set to false and the test wouldn't happen for that began-Ended pair)
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				m_StartingTouch = Input.GetTouch(0).position;
				m_IsSwiping = true;
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				m_IsSwiping = false;
			}
        }
#endif

			if (!isFinish)
			{
				Movement();
			}
		}
    }

	private void FinishLevel()
    {
		isFinish = true;
		StopMoving();
		characterAnimator.SetTrigger(s_Win);

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.IncreaseLevel();
        }

        this.PostEvent(GameEvent.OnFinishLevel);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Milestone"))
		{
			StartCoroutine(CharacterRotation(currentLevelConfig.listAngle[currentTrigger], currentLevelConfig.time[currentTrigger]));
			currentTrigger++;
		}

		if (other.gameObject.CompareTag("Endpoint"))
		{
			FinishLevel();
		}

		if (other.gameObject.CompareTag("Obstacle"))
        {
			other.gameObject.SetActive(false);
			characterAnimator.SetTrigger(s_Hit);
			if (currentLife > 0)
            {
				currentLife--;
				this.PostEvent(GameEvent.OnColliderObstacle, other.transform.position);
			}
            else
            {
				GameOver();
			}	
        }

		if (other.gameObject.CompareTag("Reward"))
		{
			other.gameObject.SetActive(false);
			this.PostEvent(GameEvent.OnClaimStar, other.transform.position);
		}

	}

	private void GameOver()
    {
		characterAnimator.SetBool(s_DeadHash, true);
		isGameover = true;
		StopMoving();

		if (PlayerDataManager.Instance != null)
        {
			PlayerDataManager.Instance.ResetLevel();
		}

		this.PostEvent(GameEvent.OnGameOver);
    }

    public float currentAngle = 0;

    private IEnumerator CharacterRotation(float angle, float time)
    {
		float targerAngle = currentAngle + angle;
		float timeCurrent = 0f;
		float time_c = time;
		int lane = m_CurrentLane;

		if (m_CurrentLane != 1)
		{
			if (angle > 0)
			{
				if (m_CurrentLane == 2)
				{
					time = time_c * 0.8f;
				}
				else
				{
					time = time_c * 1.2f;
				}
			}
			else
			{
				if (m_CurrentLane == 2)
				{
					time = time_c * 1.2f;
				}
				else
				{
					time = time_c * 0.8f;
				}
			}
		}

		while (timeCurrent < time)
		{
			if (lane != m_CurrentLane)
            {
				if (angle > 0)
				{
					if (m_CurrentLane > lane)
					{
						if (m_CurrentLane == 1) time = time_c * 1f;
						if (m_CurrentLane == 2) time = time_c * 0.8f;
					}
					else
					{
						if (m_CurrentLane == 1) time = time_c * 1f;
						if (m_CurrentLane == 0) time = time_c * 1.2f;
					}
				}
				else
				{
					if (m_CurrentLane > lane)
					{
						if (m_CurrentLane == 1) time = time_c * 1f;
						if (m_CurrentLane == 2) time = time_c * 1.2f;
					}
					else
					{
						if (m_CurrentLane == 1) time = time_c * 1f;
						if (m_CurrentLane == 0) time = time_c * 0.8f;
					}
				}
				lane = m_CurrentLane;
			}
			                      

			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.x, targerAngle, transform.rotation.z),Mathf.Abs(angle) * Time.deltaTime / (time));
			timeCurrent += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		currentAngle = targerAngle;

		transform.rotation = Quaternion.Euler(transform.rotation.x, targerAngle, transform.rotation.z);

		yield break;

	}

	public IEnumerator Jump(float duration)
	{
		if (!m_IsRunning)
			yield return null;


        //this.PostEvent(GameEvent.OnCharacterJump);
		StartCoroutine(ApplyJumpCollider());

		if (!m_Jumping)
		{
			if (m_Sliding)
				StopSliding();

			float animSpeed = 1f;

			characterAnimator.SetFloat(s_JumpingSpeedHash, animSpeed);
			characterAnimator.SetBool(s_JumpingHash, true);
			m_Jumping = true;
		}

		yield return new WaitForSeconds(duration);


		StopJumping();
	}

	public void StopJumping()
	{
		if (m_Jumping)
		{
			characterAnimator.SetBool(s_JumpingHash, false);
			m_Jumping = false;
		}
	}

	public void Movement()
    {
		rigid.velocity = this.transform.forward * 15f;
	}

	public IEnumerator Slide(float duration)
	{
		if (!m_IsRunning)
			yield return null;

        //this.PostEvent(GameEvent.OnCharacterSlide);
		StartCoroutine(ApplySlideCollider());

        if (!m_Sliding)
		{

			if (m_Jumping)
				StopJumping();

			
			float animSpeed = 1f;

			characterAnimator.SetFloat(s_JumpingSpeedHash, animSpeed);
			characterAnimator.SetBool(s_SlidingHash, true);
			m_Sliding = true;

		}

		yield return new WaitForSeconds(duration);

		StopSliding();
	}

	public void StopSliding()
	{
		if (m_Sliding)
		{
			characterAnimator.SetBool(s_SlidingHash, false);
			m_Sliding = false;

		}
	}

	public void ChangeLane(int direction)
	{
        if (!m_IsRunning)
			return ;

		int targetLane = m_CurrentLane + direction;

        if (targetLane < 0 || targetLane > 2)
			// Ignore, we are on the borders.
			return;

		m_CurrentLane = targetLane;

		Vector3 startPosition;
		Vector3 targetPosition;

		targetPosition = this.transform.position + this.transform.right * direction * laneDistance;
		mainCamera.transform.position = mainCamera.transform.position - this.transform.right * direction * laneDistance;

		startPosition = this.transform.position;

		this.transform.position = targetPosition;
    }
}
