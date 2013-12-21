﻿// Icosphere builder class for Emgen library.
// By Keijiro Takahashi, 2013
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Emgen
{
    public class IcosphereBuilder
    {
    #region Internal classes
        // A midpoint table class which provides midpoints of edges without duplication.
        class MidpointTable
        {
            VertexCache vc;
            Dictionary<int, int> table;

            // Generates a key for the table from a pair of indices.
            static int IndexPairToKey (int i1, int i2)
            {
                if (i1 < i2) {
                    return i1 | (i2 << 16);
                } else {
                    return (i1 << 16) | i2;
                }
            }

            // Constructor.
            public MidpointTable (VertexCache vc)
            {
                this.vc = vc;
                table = new Dictionary<int, int> ();
            }

            // Get a midpoint with retrieving an existing vertex or adding a new one.
            public int GetMidpoint (int i1, int i2)
            {
                var key = IndexPairToKey (i1, i2);
                if (table.ContainsKey (key)) {
                    return table [key];
                }
                var mid = (vc.vertices [i1] + vc.vertices [i2]) * 0.5f;
                var i = vc.AddVertex (mid.normalized);
                table [key] = i;
                return i;
            }
        }
    #endregion

    #region Public members
        public VertexCache vertexCache;
    #endregion

    #region Constructor
        public IcosphereBuilder ()
        {
            var t = (1.0f + Mathf.Sqrt (5.0f)) / 2.0f;

            vertexCache = new VertexCache ();

            vertexCache.AddVertex (new Vector3 (-1, +t, 0).normalized);
            vertexCache.AddVertex (new Vector3 (+1, +t, 0).normalized);
            vertexCache.AddVertex (new Vector3 (-1, -t, 0).normalized);
            vertexCache.AddVertex (new Vector3 (+1, -t, 0).normalized);

            vertexCache.AddVertex (new Vector3 (0, -1, +t).normalized);
            vertexCache.AddVertex (new Vector3 (0, +1, +t).normalized);
            vertexCache.AddVertex (new Vector3 (0, -1, -t).normalized);
            vertexCache.AddVertex (new Vector3 (0, +1, -t).normalized);

            vertexCache.AddVertex (new Vector3 (+t, 0, -1).normalized);
            vertexCache.AddVertex (new Vector3 (+t, 0, +1).normalized);
            vertexCache.AddVertex (new Vector3 (-t, 0, -1).normalized);
            vertexCache.AddVertex (new Vector3 (-t, 0, +1).normalized);

            vertexCache.AddTriangle (0, 11, 5);
            vertexCache.AddTriangle (0, 5, 1);
            vertexCache.AddTriangle (0, 1, 7);
            vertexCache.AddTriangle (0, 7, 10);
            vertexCache.AddTriangle (0, 10, 11);
        
            vertexCache.AddTriangle (1, 5, 9);
            vertexCache.AddTriangle (5, 11, 4);
            vertexCache.AddTriangle (11, 10, 2);
            vertexCache.AddTriangle (10, 7, 6);
            vertexCache.AddTriangle (7, 1, 8);
        
            vertexCache.AddTriangle (3, 9, 4);
            vertexCache.AddTriangle (3, 4, 2);
            vertexCache.AddTriangle (3, 2, 6);
            vertexCache.AddTriangle (3, 6, 8);
            vertexCache.AddTriangle (3, 8, 9);
        
            vertexCache.AddTriangle (4, 9, 5);
            vertexCache.AddTriangle (2, 4, 11);
            vertexCache.AddTriangle (6, 2, 10);
            vertexCache.AddTriangle (8, 6, 7);
            vertexCache.AddTriangle (9, 8, 1);
        }
    #endregion

    #region Mesh manipulators
        public void Subdivide ()
        {
            var vc = new VertexCache ();
            vc.vertices.AddRange (vertexCache.vertices);

            var midPoints = new MidpointTable (vc);
            foreach (var t in vertexCache.triangles) {
                var m1 = midPoints.GetMidpoint (t.i1, t.i2);
                var m2 = midPoints.GetMidpoint (t.i2, t.i3);
                var m3 = midPoints.GetMidpoint (t.i3, t.i1);
                vc.AddTriangle (t.i1, m1, m3);
                vc.AddTriangle (m1, t.i2, m2);
                vc.AddTriangle (m3, m2, t.i3);
                vc.AddTriangle (m1, m2, m3);
            }

            vertexCache = vc;
        }
    #endregion
    }
}