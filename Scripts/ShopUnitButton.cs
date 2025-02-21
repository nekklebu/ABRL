using Godot;
using System;

public partial class ShopUnitButton : Button
{
	private Label _unitNameLabel;
	public string UnitType { get; private set; }

	public override void _Ready()
	{
		_unitNameLabel = GetNode<Label>("UnitName");
		Connect("pressed", new Callable(this, nameof(OnPressed)));
	}

	public void SetUnitType(string unitType)
	{
		if (null == _unitNameLabel) {
			_unitNameLabel = GetNode<Label>("UnitName");
		}
		UnitType = unitType;
		_unitNameLabel.Text = UnitType;
		Text = $"Buy {UnitType} (10g)";
	}

	private void OnPressed()
	{
		EmitSignal(nameof(UnitPurchased), UnitType);
	}

	[Signal]
	public delegate void UnitPurchasedEventHandler(string unitType);
}
