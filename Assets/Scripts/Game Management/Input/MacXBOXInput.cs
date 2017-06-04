using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacXBOXInput : InputWrapper {

	public override bool MenuPress() {
		return Input.GetAxis ("Menu_XBOXMac") > 0;
	}

	public override bool MenuConfirm() {
		return false;
	}

	public override bool MenuExit() {
		return false;
	}

	public override bool MenuOpen () {
		return Input.GetAxis ("Menu_XBOXMac") > 0;
	}

	public override bool Continue () {
		return Input.GetAxis ("Slash_XBOXMac") > 0;
	}

	public override float GetHorizontalAxis()
	{
		Debug.Log ("Horiz2:  " + Input.GetAxis ("Horizontal_GAMEPAD"));
		return Input.GetAxis ("Horizontal_GAMEPAD");
	}

	public override float GetVerticalAxis()
	{
		Debug.Log ("Vert2:  " + Input.GetAxis ("Vertical_GAMEPAD"));
		return Input.GetAxis ("Vertical_GAMEPAD");
	}

	public override bool IsSlashAnalog () {
		return true;
	}
		
	public override float GetHorizontalSlashAxis ()
	{
		return Input.GetAxis ("Horizontal2_GAMEPAD");
	}

	public override float GetVerticalSlashAxis ()
	{
		return Input.GetAxis ("Vertical2_GAMEPAD");
	}

	public override bool SlashDown () {
		return (GetHorizontalSlashAxis () > .5f || GetVerticalSlashAxis () > .5f);
	}

	public override bool SlashUp () {
		return (GetHorizontalSlashAxis () <= .5f && GetVerticalSlashAxis () <= .5f);
	}
}
