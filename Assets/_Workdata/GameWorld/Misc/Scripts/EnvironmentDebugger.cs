using MyBox;
using UnityEngine;

public class EnvironmentDebugger : MonoBehaviour
{
   
#if UNITY_EDITOR
   [Separator("Gizmo settings")] [SerializeField]
   private bool drawGizmos = true;
   [Separator("Gizmos Side")]
   [SerializeField] private Color sideColor = Color.red;
   [SerializeField] private Transform leftBound;
   [SerializeField] private Transform rightBound;
   [SerializeField] [PositiveValueOnly] private float sideLength;
   [SerializeField] [PositiveValueOnly] private float sideWidth;
   [Separator("Gizmos Lava")]
   [SerializeField] private Color groundColor = Color.blue;
   [SerializeField] private Transform middleRow;
   [SerializeField] [PositiveValueOnly] private float groundLength;
   [SerializeField] [PositiveValueOnly] private float groundWidth;
   

   private void OnDrawGizmos()
   {
      if(leftBound == null || rightBound == null || middleRow == null)
         return;
      
      if (drawGizmos)
      {
         Gizmos.color = groundColor;
         Gizmos.DrawCube(middleRow.position.ChangeY(middleRow.position.y - 1), new Vector3(groundLength, 1, groundWidth));
         
         
         Gizmos.color = sideColor;
         // Left box
         Gizmos.DrawCube(new Vector3(leftBound.position.x + 1, leftBound.position.y, leftBound.position.z), new Vector3(sideLength, 10, sideWidth));
         // Right box
         Gizmos.DrawCube(new Vector3(rightBound.position.x + 1, rightBound.position.y, rightBound.position.z), new Vector3(sideLength, 10, sideWidth));
      }
   }
#endif
   
}
