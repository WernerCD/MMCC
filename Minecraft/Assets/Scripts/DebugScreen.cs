using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    private Transform _cam;
    private World _world;
    private Text _text;

    private float _frameRate;
    private float _timer;
    private int _halfWorldSizeInVoxels;
    private int _halfWorldSizeInChunks;

    void Start()
    {
        _cam = GameObject.Find("Main Camera").transform;
        _world = GameObject.Find("World").GetComponent<World>();
        _text = GetComponent<Text>();

        _halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
        _halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
    }

    void Update()
    {
        if (_timer > 1f)
        {
            _frameRate = (int) (1f / Time.unscaledDeltaTime);
            _timer = 0;
        }
        else
        {
            _timer += Time.deltaTime;
        }

        StringBuilder debugText = new StringBuilder();
        debugText.AppendLine($"MMCC Tutorial");
        debugText.AppendLine($"FPS: {_frameRate}");

        if (_world._playerChunkCoord != null)
        {
            int cx = _world._playerChunkCoord.X / _halfWorldSizeInChunks;
            int cz = _world._playerChunkCoord.Z / _halfWorldSizeInChunks;
            debugText.AppendLine($"Chunk: {cx}, {cz}");
        }

        if (_world.Player != null)
        {
            int x = Mathf.FloorToInt(_world.Player.position.x) - _halfWorldSizeInVoxels;
            int y = Mathf.FloorToInt(_world.Player.position.y);
            int z = Mathf.FloorToInt(_world.Player.position.z) - _halfWorldSizeInVoxels;
            debugText.AppendLine($"XYZ: {x},{y},{z}");
        }

        _text.text = debugText.ToString();
    }
}
