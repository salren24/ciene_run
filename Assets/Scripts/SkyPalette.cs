using UnityEngine;

[CreateAssetMenu(fileName = "SkyPalette", menuName = "CIENE RUN/Sky Palette")]
public class SkyPalette : ScriptableObject
{
    [Header("Cielo (tiempo 0 = horizonte, tiempo 1 = cenit)")]
    public Gradient skyGradient;

    [Header("Niebla / atmosfera (para tenir capas lejanas de parallax)")]
    public Color fogColor = Color.white;

    [Header("Color base de camara (horizonte) - evita bordes visibles en cualquier aspect ratio")]
    public Color cameraBaseColor = Color.white;
}
