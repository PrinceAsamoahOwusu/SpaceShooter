using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	//-4, 1 = y
	//-11.33487, 11.4826 = x 11.36284
	[SerializeField]
	private float _speed = 5.0f;
	private float _speedMultiplier = 2f;

	[SerializeField]
	private GameObject _laserPrefab;
	[SerializeField]
	private GameObject _tripleShotPrefab;

	[SerializeField]
	private float _fireRate = 0.15f;
	private float _canFire = -1f;
	[SerializeField]
	private int _lives = 3;
	private SpawnManager _spawnManager;
	private bool _isTripleShotActive = false;
	private bool _isSpeedBoostActive = false;
	private bool _isShieldActive = false;

	[SerializeField]
	private GameObject shieldVisualizer;
    // Start is called before the first frame update
    void Start()
    {
		transform.position = new Vector3(0, -3.63f, 0);
		_spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

		if (_spawnManager == null)
		{
			Debug.LogError("The Spawn Manager is NULL.");
		}
    }

    // Update is called once per frame
    void Update()
    {
		CalculateMovement();

		if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
		{
			FireLaser();
		}
    }

	void CalculateMovement()
	{
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");

		Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
			transform.Translate(direction * _speed * Time.deltaTime);

		if (transform.position.y >= 1)
		{
			transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
		}

		if (transform.position.y <= -5.0f)
		{
			transform.position = new Vector3(transform.position.x, -5.0f, transform.position.z);
		}

		if (transform.position.x >= 11.36284f)
		{
			transform.position = new Vector3(-11.3348f, transform.position.y, transform.position.z);
		}

		if (transform.position.x <= -11.33487f)
		{
			transform.position = new Vector3(11.4826f, transform.position.y, transform.position.z);
		}
	}

	void FireLaser()
	{
		_canFire = Time.time + _fireRate;

		if (_isTripleShotActive == true)
		{
			Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
		}
		else
		{
			Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
		}
		
	}

	public void Damage()
	{
		if (_isShieldActive == true)
		{
			_isShieldActive = false;
			shieldVisualizer.SetActive(false);
			return;
		}
		_lives--;

		if (_lives < 1)
		{
			_spawnManager.OnPlayerDeath();
			Destroy(this.gameObject);
		}
	}

	public void TripleShotActive()
	{
		_isTripleShotActive = true;
		StartCoroutine(TripleShotPowerDownRoutine());
	}

	IEnumerator TripleShotPowerDownRoutine()
	{
		yield return new WaitForSeconds(5.0f);
		_isTripleShotActive = false;
	}

	public void SpeedBoostActive()
	{
		_isSpeedBoostActive = true;
		_speed *= _speedMultiplier;
		StartCoroutine(SpeedBoostPowerDownRoutine());
	}

	IEnumerator SpeedBoostPowerDownRoutine()
	{
		yield return new WaitForSeconds(5.0f);
		_isSpeedBoostActive = false;
		_speed /= _speedMultiplier;
	}

	public void ShieldActive()
	{
		_isShieldActive = true;
		shieldVisualizer.SetActive(true);
	}
}
