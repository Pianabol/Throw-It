using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private Ball ballPrefab; 
    
    [Header(" Settings ")]
    [SerializeField] private float throwForce = 15f; 

    private ObjectPool<Ball> ballPool;

    private void Awake()
    {
        // Havuzu inşa ediyoruz
        ballPool = new ObjectPool<Ball>(CreateBall, OnTakeBall, OnReturnBall, OnDestroyBall, true, 10, 20);
    }

    private void OnEnable()
    {
        InputManager.onScreenTapped += Fire;
    }

    private void OnDisable()
    {
        InputManager.onScreenTapped -= Fire;
    }

    private void Fire(Vector3 targetPoint)
    {
        // 1. Atış yönünü hesapla (Shooter objesinden -> Tıklanan noktaya)
        Vector3 throwDirection = (targetPoint - transform.position).normalized;

        // 2. Havuzdan topu çek ve Shooter'ın tam olduğu noktaya koy
        Ball newBall = ballPool.Get();
        newBall.transform.position = transform.position;
        newBall.transform.rotation = Quaternion.identity;

        // 3. Topu fırlat
        newBall.Launch(throwDirection * throwForce);
    }

    #region Pool Methods
    private Ball CreateBall()
    {
        Ball ball = Instantiate(ballPrefab);
        ball.SetPool(ballPool);
        return ball;
    }
    private void OnTakeBall(Ball ball) => ball.gameObject.SetActive(true);
    private void OnReturnBall(Ball ball) => ball.gameObject.SetActive(false);
    private void OnDestroyBall(Ball ball) => Destroy(ball.gameObject);
    #endregion
}