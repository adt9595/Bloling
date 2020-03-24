using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text coinText;
    public GameObject ball;
    public GameObject pinContainer;
    public CharController player;
    public CamController cam;
    public float explosionRange = 4f;
    public float explosionForce = 100f;

    private ScoreManager scoreManager;
    private Vector3 initialPinContainerPosition;
    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
    private Pin[] pins;
    private bool countingScore = false;
    private bool countingTimeOut = false;

    private float timeoutCounter = 0;
    private float timeoutLimit = 6f;
    private float countScoreTimer = 0;
    private float countScoreTimeLimit = 3.4f;

    private float laneOffset = 4.592f;
    private float pinRotationOffset = 270f;

    private List<int> remainingPins;
    private int throwIndex = 0;
    private int laneIndex = 0;
    private int frameIndex = 0;
    private int bonusThrows = 0;
    private int totalPinCount = 0;
    private int money = 0;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        pins = new Pin[10];
        int index = 0;
        foreach (Transform pin in pinContainer.transform)
        {
            pins[index] = pin.GetComponent<Pin>();
            index++;
        }
        initialPinContainerPosition = pinContainer.transform.position;
        initialPositions = new Vector3[11];
        initialRotations = new Quaternion[11];
        for(int i=0;i<10;i++) {
            initialPositions[i] = pins[i].transform.position;
            initialRotations[i] = pins[i].transform.rotation;
        }
        initialPositions[10] = ball.transform.position;
        initialRotations[10] = ball.transform.rotation;
    }

    void LateUpdate()
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit, 50f)) {
        //        createExplosion(hit.point);
        //    }
        //}
        //if (Input.GetButtonDown("Reset")) {
        //    laneIndex = 0;
        //    throwIndex = 1;
        //    reset(null);
        //}
        if (countingTimeOut) {
            timeoutCounter += Time.deltaTime;
            if (timeoutCounter >= timeoutLimit) {
                checkPins();
            }
        }
        if (countingScore) {
            if (countingTimeOut) {
                countingTimeOut = false;
                timeoutCounter = 0;
            }
            countScoreTimer += Time.deltaTime;
            if (countScoreTimer >= countScoreTimeLimit) {
                checkPins();
            }
        }
    }

    void checkPins() {
        int pinCount = 0;
        remainingPins = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            if (pins[i].transform.localEulerAngles.x > pinRotationOffset + 10f || pins[i].transform.localEulerAngles.x < pinRotationOffset - 10f)
            {
                if (pins[i].gameObject.activeSelf)
                {
                    pinCount++;
                    totalPinCount++;
                }
                
            }
            else
            {
                remainingPins.Add(i);
            }
            //else if (pins[i].transform.localEulerAngles.z > 10f || pins[i].transform.localEulerAngles.z < -10f) {
            //    Debug.Log("z: " + pins[i].transform.localEulerAngles.z);
            //    totalPinCount++;
            //}
        }

        scoreManager.updateBox(pinCount, frameIndex, throwIndex);

        Debug.Log("Frame: " + frameIndex + ", Throw number: " + throwIndex + ", Total pins hit: " + totalPinCount);

        if (totalPinCount < 10 && throwIndex == 0)
        {
            reset(remainingPins);
        }
        else if (frameIndex == 9 && throwIndex == 1)
        {
            if (totalPinCount >= 10)
            {
                throwIndex = 2;
                if (scoreManager.getLastFrameBonus() && remainingPins.Count > 0)
                {
                    reset(remainingPins);
                }
                else {
                    reset(null);
                }
                Debug.Log("ONE MORE TRY YO");
            }
        }
        else if (throwIndex == 0)
        {
            throwIndex = 1;
            reset(null);
        }
        else
        {
            reset(null);
        }
    }

    // Lol
    void createExplosion(Vector3 position) {
        Collider[] colliders = Physics.OverlapSphere(position, explosionRange);
        foreach (Collider c in colliders) {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r == null || r.gameObject.tag == "Pin") {
                continue;
            }
            r.AddExplosionForce(explosionForce, position, explosionRange);
        }
    }


    private void reset(List<int> remainingPins)
    {
        // Reset conditions
        if (throwIndex == 0)
        {
            throwIndex = 1;
        }
        else{
            //laneIndex++;
            if (frameIndex != 9) {
                frameIndex++;
                throwIndex = 0;
                totalPinCount = 0;
            }
        }
        countingScore = false;
        countingTimeOut = false;
        countScoreTimer = 0;
        timeoutCounter = 0;

        // Reset ball
        Rigidbody r;
        r = ball.GetComponent<Rigidbody>();
        Ball b = ball.GetComponent<Ball>();
        r.isKinematic = true;
        r.velocity = Vector3.zero;
        r.angularVelocity = Vector3.zero;
        ball.transform.position = initialPositions[10] - Vector3.forward*(laneIndex*laneOffset);
        ball.transform.rotation = initialRotations[10];
        b.setThrown(false);
        b.setThrowing(false);
        b.setTouchingFloor(false);


        // Reset pins
        pinContainer.transform.position = initialPinContainerPosition;
        if (remainingPins == null)
        {
            for (int i = 0; i < 10; i++)
            {
                pins[i].gameObject.SetActive(true);
                r = pins[i].GetComponent<Rigidbody>();
                r.velocity = Vector3.zero;
                r.angularVelocity = Vector3.zero;
                pins[i].transform.position = initialPositions[i];
                pins[i].transform.rotation = initialRotations[i];
                pins[i].setOverrideHitSound(false);
            }
        }
        else {
            for (int i = 0; i < 10; i++)
            {
                pins[i].setOverrideHitSound(false);
                if (remainingPins.Contains(i))
                {
                    r = pins[i].GetComponent<Rigidbody>();
                    r.velocity = Vector3.zero;
                    r.angularVelocity = Vector3.zero;
                    pins[i].transform.position = initialPositions[i];
                    pins[i].transform.rotation = initialRotations[i];
                }
                else
                {
                    pins[i].gameObject.SetActive(false);
                }
            }
        }
        pinContainer.transform.position = initialPinContainerPosition - Vector3.forward * (laneIndex * laneOffset);

        // Reset camera
        cam.setMoving(true);
        cam.setTarget(ball.transform);
        cam.resetPosition();
    }

    public void endGame() {
        scoreManager.centreScoreCard();

    }

    /* Gets & Sets */
    public void changeMoney(int amount) {
        money += amount;
        coinText.text = money.ToString();
    }

    public bool isCountingScore()
    {
        return countingScore;
    }

    public void setCountingScore(bool _countingScore)
    {
        countingScore = _countingScore;
    }

    public bool isCountingTimeout()
    {
        return countingTimeOut;
    }

    public void setCountingTimeout(bool _countingTimeOut)
    {
        countingTimeOut = _countingTimeOut;
    }

    public int getFrameIndex() {
        return frameIndex;
    }

    public int getRemainingPinCount()
    {
        return remainingPins.Count;
    }
}
