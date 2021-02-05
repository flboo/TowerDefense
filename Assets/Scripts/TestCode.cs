using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestCode1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TestCode1()
    {
        // Logger(0 & 1);
        // Logger(0 & 2);
        // Logger(1 & 1);
        // Logger(1 & 2);
        // Logger(2 & 1);
        // Logger(2 & 2);
    }

    public static void Logger(object obj)
    {
        Debug.LogError(obj.ToString());
    }

}