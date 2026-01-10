using UnityEngine;
using System.Reflection;

/// <summary>
/// Script para configurar la cámara para seguir al jugador.
/// Funciona con Cinemachine si está instalado, o usa seguimiento simple si no lo está.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private Camera mainCamera;
    
    [Header("Configuración (sin Cinemachine)")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothSpeed = 5f;
    
    // Referencias a Cinemachine usando reflexión
    private Component virtualCamera;
    private bool useCinemachine = false;
    
    void Start()
    {
        // Buscar jugador si no está asignado
        if (targetPlayer == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetPlayer = player.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró el jugador. Asegúrate de que tenga el tag 'Player'.");
                enabled = false;
                return;
            }
        }
        
        // Intentar encontrar y usar Cinemachine
        TrySetupCinemachine();
        
        // Si no hay Cinemachine, usar seguimiento simple
        if (!useCinemachine)
        {
            SetupSimpleCamera();
        }
    }
    
    void TrySetupCinemachine()
    {
        // Buscar Virtual Camera de Cinemachine usando reflexión
        try
        {
            System.Type virtualCameraType = System.Type.GetType("Cinemachine.CinemachineVirtualCamera, Cinemachine");
            if (virtualCameraType != null)
            {
                UnityEngine.Object[] objects = FindObjectsByType(virtualCameraType, FindObjectsSortMode.None);
                if (objects != null && objects.Length > 0)
                {
                    virtualCamera = objects[0] as Component;
                    if (virtualCamera != null)
                    {
                        useCinemachine = true;
                        
                        // Configurar Follow y LookAt usando reflexión
                        SetCinemachineFollow(targetPlayer);
                        SetCinemachineLookAt(targetPlayer);
                        
                        Debug.Log("Cámara de Cinemachine configurada para seguir al jugador.");
                    }
                }
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Cinemachine no está disponible. Usando seguimiento simple de cámara.");
            useCinemachine = false;
        }
    }
    
    void SetCinemachineFollow(Transform target)
    {
        if (virtualCamera == null || target == null) return;
        
        try
        {
            PropertyInfo followProperty = virtualCamera.GetType().GetProperty("Follow");
            if (followProperty != null)
            {
                followProperty.SetValue(virtualCamera, target, null);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo configurar Follow en Cinemachine: " + e.Message);
        }
    }
    
    void SetCinemachineLookAt(Transform target)
    {
        if (virtualCamera == null || target == null) return;
        
        try
        {
            PropertyInfo lookAtProperty = virtualCamera.GetType().GetProperty("LookAt");
            if (lookAtProperty != null)
            {
                lookAtProperty.SetValue(virtualCamera, target, null);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo configurar LookAt en Cinemachine: " + e.Message);
        }
    }
    
    void SetupSimpleCamera()
    {
        // Buscar cámara principal si no está asignada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("No se encontró ninguna cámara.");
            enabled = false;
            return;
        }
        
        Debug.Log("Usando seguimiento simple de cámara.");
    }
    
    void Update()
    {
        if (targetPlayer == null) return;
        
        if (useCinemachine)
        {
            // Verificar si el target se perdió en Cinemachine
            try
            {
                PropertyInfo followProperty = virtualCamera.GetType().GetProperty("Follow");
                if (followProperty != null)
                {
                    Transform currentFollow = followProperty.GetValue(virtualCamera) as Transform;
                    if (currentFollow == null && targetPlayer != null)
                    {
                        SetCinemachineFollow(targetPlayer);
                        SetCinemachineLookAt(targetPlayer);
                    }
                }
            }
            catch (System.Exception)
            {
                // Si hay error, desactivar Cinemachine y usar seguimiento simple
                useCinemachine = false;
                SetupSimpleCamera();
            }
        }
    }
    
    void LateUpdate()
    {
        // Seguimiento simple de cámara (solo si no hay Cinemachine)
        if (!useCinemachine && mainCamera != null && targetPlayer != null)
        {
            Vector3 desiredPosition = targetPlayer.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            mainCamera.transform.position = smoothedPosition;
        }
        // Si hay Cinemachine, el seguimiento se maneja automáticamente
    }
}