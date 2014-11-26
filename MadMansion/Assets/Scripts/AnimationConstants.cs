using UnityEngine;
using System.Collections;

public class AnimationConstants : MonoBehaviour {
    public static int Idle { get; private set; }
    public static int Walking { get; private set; }

    public static int IsScared { get; private set; }
    public static int IsConfused { get; private set; }

    void Awake () {
    	Idle = Animator.StringToHash("Base.Idle");
    	Walking = Animator.StringToHash("Base.Walking");

    	IsScared = Animator.StringToHash("IsScared");
    	IsConfused = Animator.StringToHash("IsConfused");
    }
}