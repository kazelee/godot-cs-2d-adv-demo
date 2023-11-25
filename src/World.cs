using Godot;

// namespace Adventure;

namespace Adventure; 

public partial class World : Node2D
{
    private TileMap _tileMap;
    private Camera2D _camera2D;

    public override void _Ready() {
        _tileMap = GetNode<TileMap>("TileMap");
        _camera2D = GetNode<Camera2D>("Player/Camera2D");

        var used = _tileMap.GetUsedRect().Grow(-1);
        var tileSize = _tileMap.TileSet.TileSize;

        _camera2D.LimitLeft = used.Position.X * tileSize.X;
        _camera2D.LimitTop = used.Position.Y * tileSize.Y;
        _camera2D.LimitRight = used.End.X * tileSize.X;
        _camera2D.LimitBottom = used.End.Y * tileSize.Y;
		
        _camera2D.ResetSmoothing();
    }
}