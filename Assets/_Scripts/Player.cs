using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public static Action<int> OnActiveBlockTypeChanged;
    public static Action<Vector3Int, Chunk, MeshUtils.BlockTypes> OnPlayerRightClick;

    private StarterAssetsInputs _input;
    public int ActiveBlockTypeIndex;
    public int numberOfBlockTypesInHotbar;
    public MeshUtils.BlockTypes[] HotbarBlockTypes;
    public MeshUtils.BlockTypes ActiveBlockType;

    private void Awake()
    {
        numberOfBlockTypesInHotbar = HotbarBlockTypes.Length;
    }

    private void Start()
    {
        ActiveBlockTypeIndex = 0;
        ActiveBlockType = HotbarBlockTypes[ActiveBlockTypeIndex];
        _input = GetComponent<StarterAssetsInputs>();
        OnActiveBlockTypeChanged?.Invoke(ActiveBlockTypeIndex);
    }

    private void Update()
    {
        SetBuildingBlockType();
        LeftClick();
        RightClick();
    }

    private void LeftClick()
    {
        if(_input.leftClick)
        {
            _input.leftClick = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                Vector3Int hitBlock = Vector3Int.RoundToInt(hit.point - (hit.normal / 2));
                Chunk hitChunk = hit.transform.GetComponent<Chunk>();
                int bx = hitBlock.x - hitChunk.location.x;
                int by = hitBlock.y - hitChunk.location.y;
                int bz = hitBlock.z - hitChunk.location.z;
                int i = bx + hitChunk.width * (by + hitChunk.depth * bz);

                hitChunk.crackData[i]++;
                if (hitChunk.crackData[i] == MeshUtils.BlockTypes.None + MeshUtils.blockTypeHealths[(int)hitChunk.chunkData[i]])
                {
                    hitChunk.chunkData[i] = MeshUtils.BlockTypes.Air;
                }

                DestroyImmediate(hitChunk.GetComponent<MeshFilter>());
                DestroyImmediate(hitChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(hitChunk.GetComponent<Collider>());

                hitChunk.CreateChunk(hitChunk.location , false);
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
                Chunk hitChunk = hit.collider.transform.GetComponent<Chunk>();
                OnPlayerRightClick?.Invoke(hitBlock, hitChunk, ActiveBlockType);
            }

        }
    }

    public void SetBuildingBlockType()
    {
        if(_input.mouseScrollY == 0) return;
        ActiveBlockTypeIndex = _input.mouseScrollY > 0 ? (ActiveBlockTypeIndex - 1 + numberOfBlockTypesInHotbar) % numberOfBlockTypesInHotbar : (ActiveBlockTypeIndex + 1) % numberOfBlockTypesInHotbar;
        ActiveBlockType = HotbarBlockTypes[ActiveBlockTypeIndex];
        OnActiveBlockTypeChanged?.Invoke(ActiveBlockTypeIndex);
    }
}
