using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightProbesMover : MonoBehaviour
{
    [SerializeField] private float xDistanceToUpdate = 36 * 2;
    private Transform mainCam;

    private float distanceMoved;
    private float lastX;
    
    private LightProbes lightProbes;
    private List<Vector3> probePositions;

    private void Start()
    {
        lightProbes = LightProbes.GetInstantiatedLightProbesForScene(gameObject.scene);
        mainCam = Camera.main.transform;
        lastX = mainCam.position.x;
        probePositions = lightProbes.GetPositionsSelf().ToList();
    }

    private void Update()
    {
        var currentDistance = lastX - mainCam.position.x;

        if (Mathf.Abs(currentDistance) >= xDistanceToUpdate)
        {
            
            lastX = mainCam.position.x;

            Move(mainCam.position.x);
        }
    }
    
    public void Move(float xValue)
    {
        var positions = lightProbes.GetPositionsSelf();
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i].x = xValue + probePositions[i].x;
        }
        
        lightProbes.SetPositionsSelf(positions, true);
        LightProbes.Tetrahedralize();
    }
}