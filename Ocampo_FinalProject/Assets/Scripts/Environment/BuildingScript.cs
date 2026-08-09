using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _roofSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStatsScript>() != null)
        {
            StopAllCoroutines();
            StartCoroutine(OnPlayerEnter());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStatsScript>() != null)
        {
            StopAllCoroutines();
            StartCoroutine(OnPlayerExit());
        }
    }

    private IEnumerator OnPlayerEnter()
    {

        while (_roofSprite.color.a > 0)
        {
            yield return null;

            Color color = _roofSprite.color;
            color.a = color.a >= 0 ? color.a - Time.deltaTime * 4 : 0;
            _roofSprite.color = color;
        }
    }

    private IEnumerator OnPlayerExit()
    {

        while (_roofSprite.color.a < 1)
        {
            yield return null;

            Color color = _roofSprite.color;
            color.a = color.a <= 1 ? color.a + Time.deltaTime * 4 : 1;
            _roofSprite.color = color;
        }
    }
}
