using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class ListExtensions 
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count -1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i +1);

                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}

