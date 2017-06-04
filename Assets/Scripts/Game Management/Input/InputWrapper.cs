using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputWrapper {

	public abstract bool MenuPress ();
	public abstract bool MenuConfirm ();
	public abstract bool MenuOpen ();
	public abstract bool MenuExit ();

	public abstract bool Continue ();

	public abstract float GetHorizontalAxis ();
	public abstract float GetVerticalAxis ();

	public abstract bool IsSlashAnalog ();

	public abstract float GetHorizontalSlashAxis ();
	public abstract float GetVerticalSlashAxis ();

	public abstract bool SlashDown ();
	public abstract bool SlashUp ();
}
