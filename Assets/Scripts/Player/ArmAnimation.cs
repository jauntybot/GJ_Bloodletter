using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAnimation : MonoBehaviour
{

    private Animator armAnimator;
    private bool bloodTrigger;

    // Start is called before the first frame update
    void Start()
    {
        armAnimator = GetComponent<Animator>();
        bloodTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (armAnimator != null)
        {
            if (Input.GetButtonDown("Bloodlet"))
            {
                if (bloodTrigger)
                {
                    armAnimator.SetTrigger("Bleed In");
                } else
                {
                    armAnimator.SetTrigger("Bleed Out");
                }

                bloodTrigger = !bloodTrigger;
            }
        }
    }
}
