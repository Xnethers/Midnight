﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float length = 3;
    public float speed = 0.1f;
    public float IntervalTime = 2;


    private Vector3 originPosition;
    private bool IsDisappear = false;
    private float alpha = 0;

    void Start()
    {
        originPosition = transform.position;
    }

    void Update()
    {
        if (!IsDisappear)
        {
            alpha += speed;
            Vector3 newPosition = originPosition + new Vector3(length, 0, 0);
            if (Vector3.Distance(transform.position, newPosition) < 0.1f)
            { StartCoroutine("TimerAndReset"); }
            else
            { OnMove(newPosition); }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        { collision.transform.parent = this.transform; }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        { collision.transform.parent = null; }
    }
    #region private function
    private void OnMove(Vector3 newPos)
    {
        transform.position = Vector3.Lerp(transform.position, newPos, alpha * Time.deltaTime);
    }
    IEnumerator TimerAndReset()
    {
        IsDisappear = true;
        transform.position = originPosition;
        alpha = 0;
        yield return new WaitForSeconds(IntervalTime);
        IsDisappear = false;
    }
    #endregion
}