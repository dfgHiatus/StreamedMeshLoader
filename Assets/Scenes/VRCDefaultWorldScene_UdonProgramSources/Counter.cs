
using UdonSharp;
using UnityEngine;

public class Counter : UdonSharpBehaviour
{
    [SerializeField]
    private BaseUdonCoroutine r;

    void Start()
    {
        r.StartUdonCoroutine();
    }
}
