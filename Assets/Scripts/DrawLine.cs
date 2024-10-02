using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PikachuGame
{
    public class DrawLine : MonoBehaviour
    {
        public static DrawLine Instance;
        private LineRenderer lr;

        void Start()
        {
            Instance = this;
            lr = GetComponent<LineRenderer>();
        }

        public void DrawLink(Vector3[] vertexPositions)
        {
            lr.positionCount = vertexPositions.Length;
            lr.SetPositions(vertexPositions);
        }
    }
}

