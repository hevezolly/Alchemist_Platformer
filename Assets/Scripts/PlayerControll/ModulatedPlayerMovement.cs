using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulatedPlayerMovement : MonoBehaviour
{
    public JumpOption jump;
    public List<MoveOption> moveOptions;
    private MoveOption currentOption;

    private void Start()
    {
        jump.SetUp(gameObject);
        foreach (var option in moveOptions)
        {
            option.SetUp(gameObject, jump);
        }
        if (moveOptions.Count > 0)
            currentOption = moveOptions[0];
    }

    // Update is called once per frame
    private void Update()
    {
        var isChousen = false;
        foreach (var option in moveOptions)
        {
            if (option.IsConditionsReached())
            {
                if (option != currentOption)
                {
                    currentOption.TurnOff();
                    currentOption = option;
                    currentOption.TurnOn();
                }
                isChousen = true;
                break;
            }
        }
        if (!isChousen && moveOptions.Count > 0)
        {
            if (moveOptions[0] != currentOption)
            {
                currentOption.TurnOff();
                currentOption = moveOptions[0];
                currentOption.TurnOn();
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var option in moveOptions)
            option.NeccessaryFixedUpdate();
        if (currentOption != null)
            currentOption.FixedUpdate();
        jump.FixedUpdate();
    }

    private void LateUpdate()
    {
        foreach (var option in moveOptions)
            option.NeccessaryUpdate();
        if (currentOption != null)
            currentOption.Update();
        jump.Update();
    }

    private void OnDrawGizmos()
    {
        foreach (var option in moveOptions)
        {
            option.OnDrawGizoms();
        }
    }
}
