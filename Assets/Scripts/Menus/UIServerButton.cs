using UnityEngine;
using System.Collections;

public class UIServerButton : UICheckbox
{
	public delegate void OnServerSelectionChange(bool state, string serverName);
	public OnServerSelectionChange onServerSelectionChange;

	public string serverName;

	void Awake()
	{
		base.onStateChange = FireSelectionChange;
	}

	void Destroy()
	{
		base.onStateChange = null;
	}

	private void FireSelectionChange(bool state)
	{
		if (onServerSelectionChange != null)
			onServerSelectionChange(base.isChecked, this.serverName);
	}

	void OnDoubleClick()
	{
		var menuController = GameObject.Find("MenuController").GetComponent<MenuController>();
		menuController.DoubleClickServer(this);
	}
}
