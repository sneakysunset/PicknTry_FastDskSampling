using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // mettre sur myges

    [SerializeField] private Transform cratere;
    [SerializeField] private float r;
    [SerializeField] private float k;
    [SerializeField] private int size;

    private List<Transform> grid;



    void Start()
    {
        grid = new List<Transform>();
        for (int i = 0; i < size * size; i++)
        {
            grid.Add(null);
        }

        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        List<int> activeIndices = new List<int>();

        int firstIndex = Random.Range(0, size * size);
        activeIndices.Add(firstIndex);
        grid[firstIndex] = Instantiate(cratere, new Vector3((firstIndex % size) * r, 0, (firstIndex / size) * r), Quaternion.identity);

        int maxCount = 9999, count = 0;
        while (activeIndices.Count > 0 && count < maxCount)
        {
            int selectedIndex = Random.Range(0, activeIndices.Count);
            int activeIndex = activeIndices[selectedIndex];

            Vector3 samplePos = Vector3.zero;
            int samples = 0;
            bool valid = false; 
            while(!valid && samples < k)
            {
                float radius = Random.Range(r, 2 * r);
                Vector2 randomVec = Random.insideUnitCircle * radius;
                samplePos = grid[activeIndex].position + new Vector3(randomVec.x, 0, randomVec.y);

                valid = IsSampleValid(samplePos);

                samples++;

                yield return null;
            }

            if (valid)
            {
                int sampleIndex = GetGridIndex(samplePos);

                grid[sampleIndex] = Instantiate(cratere, samplePos, Quaternion.identity);
                activeIndices.Add(sampleIndex);
            }
            else
            {
                activeIndices.RemoveAt(selectedIndex);
            }

            count++;
        }

    }

    bool IsSampleValid(Vector3 samplePos)
    {
        int sampleIndex = GetGridIndex(samplePos);
        if (sampleIndex >= 0) // Inside grid
        {
            // Test if current and surrounding cells are free or contain an object that is far enough (> r)
            if (IsValid(sampleIndex, samplePos)
                && IsValid(sampleIndex + 1, samplePos)
                && IsValid(sampleIndex - 1, samplePos)
                && IsValid(sampleIndex + size, samplePos)
                && IsValid(sampleIndex + size + 1, samplePos)
                && IsValid(sampleIndex + size - 1, samplePos)
                && IsValid(sampleIndex - size, samplePos)
                && IsValid(sampleIndex - size - 1, samplePos)
                && IsValid(sampleIndex - size + 1, samplePos))
            {
                return true;
            }
        }
        return false;
    }

    bool IsValid(int index, Vector3 samplePos)
    {
        if (index >= 0 && index < size * size)
        {
            if (grid[index])
            {
                return Vector3.Distance(grid[index].position, samplePos) > r;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    int GetGridIndex(Vector3 pos)
    {
        int i = Mathf.FloorToInt(pos.z / r);
        int j = Mathf.FloorToInt(pos.x / r);

        if (i >= 0 && i < size && j >= 0 && j < size) // Make sure it's inside the bounds of the grid
        {
            return i * size + j;
        }
        else
        {
            return -1;
        }
    }
}
