using Godot;
using System;

public partial class Unit : Node2D
{
	[Export] public int Hp = 100;
	[Export] public int Attack = 10;
	[Export] public float Speed = 1.0f;
	[Export] public string Team = "player";
	[Export] public string UnitType = "Warrior";
	private bool _isDragging = false;
	private double _elapsedTime = 0.0f;
	private Label _label;
	private Area2D _clickArea;
	private Unit _target;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_clickArea = GetNode<Area2D>("ClickArea");
		Hp = (int)GD.RandRange(50, 150);
		Attack = (int)GD.RandRange(5, 20);
		Speed = (float)GD.RandRange(0.5, 1.5);
		_label.Text = $"{UnitType} HP: {Hp}";
		
		_clickArea.Connect("input_event", new Callable(this, nameof(OnInputEvent)));
	}

	public override void _Process(double delta)
	{
		_elapsedTime += delta;
		if (_isDragging)
			GlobalPosition = GetGlobalMousePosition();
			
		if (_elapsedTime > Speed) {
			ReadyAttack();
			_elapsedTime = 0.0f;
		}
	}

	private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed && Team == "player")
				_isDragging = true;
			else
				_isDragging = false;
		}
	}
	
	public void ReadyAttack() {
		if (null == _target)
			GD.PrintErr("Trying to attack before a valid target is assigned");
		_target.TakeDamage(Attack);
	}
	
	public void TakeDamage(int damage) {
		Hp -= damage;
		_label.Text = $"{UnitType} HP: {Hp}";
		if (Hp <= 0)
			QueueFree();
	}

	public void Target(Unit eUnit)
	{
		if (null != _target)
			return;
		_target = eUnit;
	}
}
