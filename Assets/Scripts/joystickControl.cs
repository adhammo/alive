using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joystickControl : MonoBehaviour
{
        public Animator anim;

        public Animator wheel_anim;

        public Animator self_anim;

        public void JoyStick()
        {
            anim.SetBool("Go",true); //cart
            wheel_anim.SetBool("GO",true); //cart wheel
            self_anim.SetBool("GO",false); // joy go idle
            self_anim.SetBool("IDLE",true);
            
        }
}
