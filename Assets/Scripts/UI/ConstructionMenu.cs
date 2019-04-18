using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : MonoBehaviour
{
    public Animator anim;
    public string defaultAnimation;
    public string updateAnimation;
    public bool state = false;
    public bool open = false;

    private bool startCount = false;
    private float timer = 1;

    private void Start()
    {
        this.gameObject.SetActive(this.state);
    }

    private void Update()
    {
        if (this.startCount)
        {
            this.timer -= Time.deltaTime;

            if (this.timer <= 0)
            {
                this.gameObject.SetActive(this.state);
                this.timer = 1;
            }
        }
    }

    public void ChangeState(bool sleepMode = false)
    {
        if (sleepMode && !this.open)
        {
            this.open = true;
            this.gameObject.SetActive(!this.state);
        }
        this.state = !this.state;
    }

    public void UpdateAnimator(Animator animator)
    {
        if (state)
            animator.Play(defaultAnimation);
        else
        {
            animator.Play(updateAnimation);
            this.startCount = true;
            this.open = false;
        }
    }

    public void SetState(bool state)
    {
        this.gameObject.SetActive(this.state);
        this.state = state;
    }
}
