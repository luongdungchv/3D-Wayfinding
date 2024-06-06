using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DL.Utils
{
    public static class CollectionUtils
    {
        public static void Loop<T>(this T[,,] input, System.Action<int, int, int> executor)
        {
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    for (int z = 0; z < input.GetLength(2); z++)
                    {
                        executor?.Invoke(x, y, z);
                    }
                }
            }
        }
        public static void Loop<T>(this T[,,] input, System.Action<T, int, int, int> executor)
        {
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    for (int z = 0; z < input.GetLength(2); z++)
                    {
                        executor?.Invoke(input[x,y,z], x, y, z);
                    }
                }
            }
        }
        public static void Loop<T>(this T[,,] input, System.Action<T> executor)
        {
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    for (int z = 0; z < input.GetLength(2); z++)
                    {
                        executor?.Invoke(input[x,y,z]);
                    }
                }
            }
        }

        public static void Loop<T>(this T[,] input, System.Action<T> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    executor?.Invoke(input[x, y]);
                }
            }
        }
        public static void Loop<T>(this T[,] input, System.Action<T, int, int> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    executor?.Invoke(input[x, y], x, y);
                }
            }
        }

        public static void Loop<T>(this T[,] input, System.Action<int, int> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    executor?.Invoke(x, y);
                }
            }
        }

        public static void Loop<T>(this T[,] input, System.Func<T, LoopOp> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    var result = executor?.Invoke(input[x, y]);
                    if(result == LoopOp.Continue) continue;
                    else if(result == LoopOp.Break) break;
                }
            }
        }
        public static void Loop<T>(this T[,] input, System.Func<T, int, int, LoopOp> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    var result = executor?.Invoke(input[x, y], x, y);
                    if(result == LoopOp.Continue) continue;
                    else if(result == LoopOp.Break) break;
                }
            }
        }

        public static void Loop<T>(this T[,] input, System.Func<int, int, LoopOp> executor){
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    var result = executor?.Invoke(x, y);
                    if(result == LoopOp.Continue) continue;
                    else if(result == LoopOp.Break) break;
                }
            }
        }
        public static T[,] CloneMatrix<T>(this T[,] input){
            var width = input.GetLength(0);
            var height = input.GetLength(1);
            T[,] result = new T[width, height];
            input.Loop((item, x, y) => result[x, y] = item);
            return result;
        }
    }
    
    public enum LoopOp{
        None, Continue, Break
    }
}
