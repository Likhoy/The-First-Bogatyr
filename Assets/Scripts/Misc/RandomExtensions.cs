using System;
using System.Collections.Generic;

static class RandomExtensions
{
    public static void Shuffle<T>(this Random rng, IList<T> lst)
    {
        int n = lst.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = lst[n];
            lst[n] = lst[k];
            lst[k] = temp;
        }
    }
}
