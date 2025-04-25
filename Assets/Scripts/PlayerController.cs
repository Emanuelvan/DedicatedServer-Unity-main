using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color connectedColor = Color.white;
    [SerializeField] private Color disconnectedColor = Color.gray;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // If SpriteRenderer is not in this object, try to find it in children
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        UpdateVisualState(false);
    }

    public void UpdateVisualState(bool connected)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = connected ? connectedColor : disconnectedColor;
        }
        else
        {
            Debug.LogWarning($"No SpriteRenderer found for player {playerId}");
        }
    }

    public void MovePlayer(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
