using System;
using System.IO;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using NativeUI;

public class SeatbeltMenu: Script
{
	private Ped playerPed = Game.Player.Character;
	private MenuPool _menuPool;
	private ScriptSettings config;
	private Keys OpenMenu;
	private bool CantFallFromBike_;
	private bool CantBeDraggedOutOfVehicle_;
	private bool CantFlyThroughWindscreen_;

	public void PlayerMenu(UIMenu menu)
	{
		UIMenuItem CantFallFromBike = new UIMenuCheckboxItem("Can't fall from bike", CantFallFromBike_, "You can't fall from your bike when enabled");
		UIMenuItem CantBeDraggedOutOfVehicle = new UIMenuCheckboxItem("Can't be dragged out of vehicle", CantBeDraggedOutOfVehicle_, "You can't be dragged out of your vehicle when enabled");
		UIMenuItem CantFlyThroughWindscreen = new UIMenuCheckboxItem("Can't fly through windscreen", CantFlyThroughWindscreen_, "Crashing at high speed won't make you fly through the windscreen");
		menu.AddItem(CantFallFromBike);
		menu.AddItem(CantBeDraggedOutOfVehicle);
		menu.AddItem(CantFlyThroughWindscreen);
		menu.OnCheckboxChange += (sender, item, checked_) =>
		{
			if (item == CantFallFromBike)
			{
				CantFallFromBike_ = checked_;
				config.SetValue<bool>("Options", "CantFallFromBike", checked_);
			}
			if (item == CantBeDraggedOutOfVehicle)
			{
				CantBeDraggedOutOfVehicle_ = checked_;
				config.SetValue<bool>("Options", "CantBeDraggedOutOfVehicle", checked_);
			}
			if (item == CantFlyThroughWindscreen)
			{
				CantFlyThroughWindscreen_ = checked_;
				config.SetValue<bool>("Options", "CantFlyThroughWindscreen", checked_);
			}
			config.Save();
		};
	}

	public SeatbeltMenu()
	{
		string fileName = "scripts\\Seatbelt Menu.ini";
		config = ScriptSettings.Load(fileName);
		if (!File.Exists(fileName))
		{
			config.SetValue<string>("Options", "OpenMenu", "F7");
			config.SetValue<bool>("Options", "CantFallFromBike", false);
			config.SetValue<bool>("Options", "CantBeDraggedOutOfVehicle", false);
			config.SetValue<bool>("Options", "CantFlyThroughWindscreen", false);
			config.Save();
		}
		OpenMenu = (Keys)Enum.Parse(typeof(Keys), config.GetValue<string>("Options", "OpenMenu", "F7"), true);
		CantFallFromBike_ = config.GetValue<bool>("Options", "CantFallFromBike", false);
		CantBeDraggedOutOfVehicle_ = config.GetValue<bool>("Options", "CantBeDraggedOutOfVehicle", false);
		CantFlyThroughWindscreen_ = config.GetValue<bool>("Options", "CantFlyThroughWindscreen", false);
		_menuPool = new MenuPool();
		UIMenu mainMenu = new UIMenu("Seatbelt Menu", "~b~Mod by ShadoFax! ~r~V 1.1");
		_menuPool.Add(mainMenu);
		PlayerMenu(mainMenu);
		_menuPool.RefreshIndex();
		Tick += delegate(object o, EventArgs e)
		{
			_menuPool.ProcessMenus();
			Function.Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, playerPed, CantFallFromBike_); // this is actually inverted
			Function.Call(Hash.SET_PED_CAN_BE_DRAGGED_OUT, playerPed, !CantBeDraggedOutOfVehicle_);
			Function.Call(Hash.SET_PED_CONFIG_FLAG, playerPed, 32, !CantFlyThroughWindscreen_);
		};
		KeyDown += delegate(object o, KeyEventArgs e)
		{
			if (e.KeyCode == OpenMenu)
			{
				mainMenu.Visible = !mainMenu.Visible;
			}
		};
	}
}
