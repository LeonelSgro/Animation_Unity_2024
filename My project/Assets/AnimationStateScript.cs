using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateScript : MonoBehaviour
{
    Animator animatons; 
    int Walkinghash;
    int Runninghash;
    // Start is called before the first frame update
    void Start()
    {
        animatons = GetComponent<Animator>();
        Walkinghash = Animator.StringToHash("isWalking");
        Runninghash = Animator.StringToHash("isRunning");

    }
    // Update is called once per frame
    void Update()
    {
               
        bool forewardPress = Input.GetKey(KeyCode.W);
        bool isWalking = animatons.GetBool(Walkinghash);

        bool Runpress = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = animatons.GetBool(Runninghash);
        
        if(!isWalking && forewardPress){
            animatons.SetBool(Walkinghash, true);
        }

        if(isWalking && !forewardPress){
            animatons.SetBool(Walkinghash, false);
        }

        if(!isRunning && (forewardPress || Runpress)){
            animatons.SetBool(Runninghash, true);
        }

        if(isRunning && !(forewardPress || Runpress)){
            animatons.SetBool(Runninghash, false);
        }











    }
}
