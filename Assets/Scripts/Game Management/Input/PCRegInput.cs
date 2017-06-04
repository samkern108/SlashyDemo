using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCRegInput : InputWrapper {

	public override bool MenuPress() {
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public override bool MenuConfirm() {
		return Input.GetKeyDown(KeyCode.KeypadEnter);
	}

	public override bool MenuExit() {
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public override bool MenuOpen () {
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public override bool Continue () {
		return Input.GetKeyDown(KeyCode.Space);
	}

	public override float GetHorizontalAxis()
	{
		return Input.GetAxis ("Horizontal");
	}

	public override float GetVerticalAxis()
	{
		return Input.GetAxis ("Vertical");
	}

	public override bool IsSlashAnalog () {
		return false;
	}

	public override float GetHorizontalSlashAxis ()
	{
		return Input.GetAxis ("Horizontal2");
	}

	public override float GetVerticalSlashAxis ()
	{
		return Input.GetAxis ("Vertical2");
	}

	public override bool SlashDown () {
		return Input.GetKeyDown(KeyCode.Space);
	}

	public override bool SlashUp () {
		return Input.GetKeyUp(KeyCode.Space);
	}
}
