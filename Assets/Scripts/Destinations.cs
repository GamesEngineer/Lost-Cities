using UnityEngine;

[CreateAssetMenu()]
public class Destinations : ScriptableObject
{
    public static readonly Color[] colors = { Color.white, Color.yellow, Color.blue, Color.green, Color.red };

    public Sprite[] cardArtwork;
}
