using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsGridSystem : MonoBehaviour
{
    [Header("windows网格")] 
    public RectTransform mainWindows;
    public int rows = 6;
    public int colums = 12;

    private Vector2[] gridCenters;
    private Rect windowRect;  
    
    // Start is called before the first frame update
    void Awake()
    {
        // 计算网格中心点
        CalculateGridCenters();
    }

    void CalculateGridCenters()
    {
        
        windowRect = mainWindows.rect;
        
        float cellWidth = windowRect.width / colums;
        float cellHeight = windowRect.height / rows;
        
        
        gridCenters = new Vector2[rows * colums];
        
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < colums; col++)
            {
                
                float x = (col * cellWidth) + (cellWidth / 2) - (windowRect.width / 2);
                float y = -(row * cellHeight) - (cellHeight / 2) + (windowRect.height / 2-10);
                
                
                Vector2 localCenter = new Vector2(x, y);
                Vector2 worldCenter = mainWindows.TransformPoint(localCenter);
                
                
                gridCenters[row * colums + col] = worldCenter;
            }
        }
    }
    public Vector2 GetNearestGridCenter(Vector2 targetPosition)
    {
        if (gridCenters == null || gridCenters.Length == 0)
        {
            CalculateGridCenters();
        }
        
        Vector2 nearestCenter = gridCenters[0];
        float minDistance = Vector2.Distance(targetPosition, nearestCenter);
        
        foreach (Vector2 center in gridCenters)
        {
            float distance = Vector2.Distance(targetPosition, center);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCenter = center;
            }
        }
        
        return nearestCenter;
    }

    
    void OnDrawGizmos()
    {
        if (mainWindows == null) return;
        
        Gizmos.color = Color.blue;
        windowRect = mainWindows.rect;
        
        
        float cellWidth = windowRect.width / colums;
        float cellHeight = windowRect.height / rows;
        
        
        for (int col = 0; col <= colums; col++)
        {
            float x = (col * cellWidth) - (windowRect.width / 2);
            Vector2 start = mainWindows.TransformPoint(new Vector2(x, windowRect.height / 2));
            Vector2 end = mainWindows.TransformPoint(new Vector2(x, -windowRect.height / 2));
            Gizmos.DrawLine(start, end);
        }
        
      
        for (int row = 0; row <= rows; row++)
        {
            float y = -(row * cellHeight) + (windowRect.height / 2);
            Vector2 start = mainWindows.TransformPoint(new Vector2(-windowRect.width / 2, y));
            Vector2 end = mainWindows.TransformPoint(new Vector2(windowRect.width / 2, y));
            Gizmos.DrawLine(start, end);
        }
        
        Gizmos.color = Color.red;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < colums; col++)
            {
                float x = (col * cellWidth) + (cellWidth / 2) - (windowRect.width / 2);
                float y = -(row * cellHeight) - (cellHeight / 2) + (windowRect.height / 2-13);
                Vector2 center = mainWindows.TransformPoint(new Vector2(x, y));
                Gizmos.DrawSphere(center, 5f);
            }
        }
    }

}
