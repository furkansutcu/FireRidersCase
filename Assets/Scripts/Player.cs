using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 20;
    public Text ScoreText;
    private bool Swing;
    private LineRenderer Line;
    private Vector3 Point;
    private Rigidbody rb;
    private bool die;
    private int score;
    private bool Started;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Line = GetComponent<LineRenderer>();
        Line.enabled = true;
        die = false;
        score = 0;
        Started = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit))
        {
            Point = hit.point;
        }
        StartCoroutine(InitialSwing());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Swing = true;
            Line.enabled = true;
            rb.useGravity = false;
            if (!Started)
            {
                Started = true;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward + transform.up, out hit))
                {
                    Point = hit.point;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Swing = false;
            rb.useGravity = true;
            Line.enabled = false;
        }
        else if (Swing || !Started)
        {
            Line.positionCount = 2;
            Line.SetPosition(0, transform.position);
            Line.SetPosition(1, Point);
            rb.velocity = Vector3.Cross(-transform.right, (transform.position - Point).normalized) * speed;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        die = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DoubleScore"))
        {
            score += 2;
        }
        else if (other.CompareTag("OneScore"))
        {
            score += 1;
        }

        foreach (Collider collider in other.transform.parent.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        StartCoroutine(ExplodeTarget(other.transform));
        ScoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator ExplodeTarget(Transform target)
    {
        float duration = .2f;
        float elapsedTime = 0f;
        float startTime = Time.time;
        Vector3 initialScale = target.localScale;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            target.localScale = Vector3.Lerp(initialScale, Vector3.one * 20, (elapsedTime / duration));
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator InitialSwing()
    {
        float initialSpeed = speed;
        float elapsedTime = 0;
        while (!Started)
        {
            speed = Mathf.PingPong(elapsedTime * initialSpeed, initialSpeed * 2) - initialSpeed;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        speed = initialSpeed;
    }
}