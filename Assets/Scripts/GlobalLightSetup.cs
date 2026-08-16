using UnityEngine;
using UnityEngine.Rendering.Universal;

// Salvaguarda: si en el futuro se agrega una sorting layer nueva y nadie actualiza el
// Light2D global a mano, esto la vuelve a incluir automaticamente al arrancar la escena.
[RequireComponent(typeof(Light2D))]
public class GlobalLightSetup : MonoBehaviour
{
    void Awake()
    {
        Light2D light = GetComponent<Light2D>();

        SortingLayer[] layers = SortingLayer.layers;
        int[] ids = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            ids[i] = layers[i].id;

        light.targetSortingLayers = ids;
    }
}
