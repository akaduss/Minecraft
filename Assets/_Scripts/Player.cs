using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public int ActiveBlockTypeIndex;
    private int numberOfBlockTypesInHotbar;
    public MeshUtils.BlockTypes[] HotbarBlockTypes;

    private void Awake()
    {
        numberOfBlockTypesInHotbar = HotbarBlockTypes.Length;
    }

    private void Start()
    {
        ActiveBlockTypeIndex = 0;
        _input = GetComponent<StarterAssetsInputs>();
        Signals.OnActiveBlockTypeChanged?.Invoke(ActiveBlockTypeIndex);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        SetBuildingBlockType();
        LeftClick();
        RightClick();
    }

    private void LeftClick()
    {
        if (_input.leftClick )
        {
            _input.leftClick = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                Vector3Int hitBlock = Vector3Int.RoundToInt(hit.point - (hit.normal / 2));
                Chunk thisChunk = hit.transform.GetComponent<Chunk>();
                int bx = (int)(hitBlock.x - thisChunk.location.x);
                int by = (int)(hitBlock.y - thisChunk.location.y);
                int bz = (int)(hitBlock.z - thisChunk.location.z);
                int i = bx + World.ChunkDimensions.x * (by + World.ChunkDimensions.z * bz);

                thisChunk.chunkData[i] = MeshUtils.BlockTypes.Air;

                DestroyImmediate(thisChunk.GetComponent<MeshFilter>());
                DestroyImmediate(thisChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(thisChunk.GetComponent<Collider>());

                thisChunk.CreateChunk(thisChunk.location, false);
            }

        }
    }

    private void RightClick()
    {
        if (_input.rightClick)
        {
            _input.rightClick = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                Vector3Int hitBlock = Vector3Int.RoundToInt(hit.point + (hit.normal / 2));
                Chunk thisChunk = hit.transform.GetComponent<Chunk>();

                Signals.OnPlayerRightClick?.Invoke(hitBlock, thisChunk);
            }

        }
    }

    public void SetBuildingBlockType()
    {
        if (_input.mouseScrollY == 0) return;
        ActiveBlockTypeIndex = _input.mouseScrollY > 0 ? (ActiveBlockTypeIndex - 1 + numberOfBlockTypesInHotbar) % numberOfBlockTypesInHotbar : (ActiveBlockTypeIndex + 1) % numberOfBlockTypesInHotbar;
        Signals.OnActiveBlockTypeChanged?.Invoke(ActiveBlockTypeIndex);
    }
}