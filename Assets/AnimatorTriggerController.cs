//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AnimatorTriggerController : MonoBehaviour
//{
//    public Animator animator1; // Assign your first object's animator in the inspector
//    public Animator animator2; // Assign your second object's animator in the inspector

//    private void Update()
//    {
//        // Check if the space key was pressed down in this frame
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            // Set isClicked to true for both animators
//            animator1.SetBool("isclicked", true);
//            animator2.SetBool("isclicked", true);

//            Invoke(nameof(ResetIsClicked), 0.5f);
//        }
//    }

//    private void ResetIsClicked()
//    {
//        // Set isClicked to false for both animators
//        animator1.SetBool("isclicked", false);
//        animator2.SetBool("isclicked", false);
//    }
//}