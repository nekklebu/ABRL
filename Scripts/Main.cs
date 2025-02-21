using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private PackedScene _unitScene = GD.Load<PackedScene>("res://Scenes/Unit.tscn");
	private PackedScene _shopUnitButtonScene = GD.Load<PackedScene>("res://Scenes/ShopUnitButton.tscn");
	private List<Unit> _playerUnits = new List<Unit>();
	private List<Unit> _enemyUnits = new List<Unit>();
	private int _wave = 1;
	private int _gold = 50;
	private Label _waveLabel;
	private Button _shopButton;
	private HBoxContainer _shopContainer;
	private string[] _shopUnits = { "Warrior", "Mage", "Rogue" };

	public override void _Ready()
	{
		_waveLabel = GetNode<Label>("UI/WaveLabel");
		_shopButton = GetNode<Button>("UI/ShopButton");
		_shopContainer = GetNode<HBoxContainer>("UI/ShopContainer");

		_waveLabel.Text = $"Wave: {_wave} | Gold: {_gold}";
		_shopButton.Connect("pressed", new Callable(this, nameof(RefreshShop)));

		SpawnPlayerUnit(new Vector2(300, 250));  // Close for testing combat
		SpawnEnemyUnit(new Vector2(300, 200));
		RefreshShop();
	}

	public override void _Process(double delta)
	{
		foreach (var pUnit in _playerUnits)
		{
			foreach (var eUnit in _enemyUnits)
			{
				if (pUnit.Position.DistanceTo(eUnit.Position) <= 50)
				{
					pUnit.Target(eUnit);
					eUnit.Target(pUnit);
				}
			}
		}

		_playerUnits.RemoveAll(u => !IsInstanceValid(u));
		_enemyUnits.RemoveAll(u => !IsInstanceValid(u));

		if (_enemyUnits.Count == 0)
			NextWave();
	}

	private void SpawnPlayerUnit(Vector2 pos)
	{
		var unit = _unitScene.Instantiate<Unit>();
		unit.Position = pos;
		unit.Team = "player";
		GetNode<Node2D>("Bench").AddChild(unit);
		_playerUnits.Add(unit);
	}

	private void SpawnEnemyUnit(Vector2 pos)
	{
		var unit = _unitScene.Instantiate<Unit>();
		unit.Position = pos;
		unit.Team = "enemy";
		GetNode<Node2D>("Battlefield").AddChild(unit);
		_enemyUnits.Add(unit);
	}

	private void NextWave()
	{
		_wave++;
		_gold += 10;
		_waveLabel.Text = $"Wave: {_wave} | Gold: {_gold}";
		SpawnEnemyUnit(new Vector2(400, 200));
		GD.Print($"Wave {_wave}, Gold: {_gold}");
	}

	private void RefreshShop()
	{
		foreach (Node child in _shopContainer.GetChildren())
			child.QueueFree();

		for (int i = 0; i < 3; i++)
		{
			var unitType = _shopUnits[GD.Randi() % _shopUnits.Length];
			var button = _shopUnitButtonScene.Instantiate<ShopUnitButton>();
			button.SetUnitType(unitType);
			button.Connect("UnitPurchased", new Callable(this, nameof(OnUnitPurchased)));
			_shopContainer.AddChild(button);
		}
	}

	private void OnUnitPurchased(string unitType)
	{
		if (_gold >= 10)
		{
			_gold -= 10;
			_waveLabel.Text = $"Wave: {_wave} | Gold: {_gold}";
			Vector2 spawnPos = new Vector2(300 + (_playerUnits.Count * 50), 600);
			var unit = _unitScene.Instantiate<Unit>();
			unit.Position = spawnPos;
			unit.Team = "player";
			unit.UnitType = unitType;
			GetNode<Node2D>("Bench").AddChild(unit);
			_playerUnits.Add(unit);
			GD.Print($"Bought {unitType}, Gold: {_gold}");
		}
		else
		{
			GD.Print("Not enough gold!");
		}
	}
}
