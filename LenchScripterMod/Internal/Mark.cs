﻿using UnityEngine;

namespace LenchScripter.Internal
{
    /// <summary>
    /// Mark script attached to primitive sphere objects.
    /// Used to mark locations.
    /// </summary>
    public class Mark : MonoBehaviour
    {
        private Renderer renderer;

        /// <summary>
        /// Should the mark be destroyed at the end of the simulation.
        /// </summary>
        internal bool DestroyOnSimulationStop { get; set; } = true;

        private void Awake()
        {
            renderer = GetComponent<Renderer>();
            Color color = Color.red;
            color.a = 0.5f;
            renderer.material.color = color;
            renderer.material.shader = Shader.Find("Transparent/Diffuse");
            Destroy(GetComponent<SphereCollider>());
        }

        /// <summary>
        /// Implicit conversion to Vector3.
        /// </summary>
        /// <param name="m"></param>
        public static implicit operator Vector3(Mark m)
        {
            return m.transform.position;
        }

        /// <summary>
        /// Explicit conversion to string.
        /// </summary>
        public override string ToString()
        {
            return transform.position.ToString();
        }

        /// <summary>
        /// Moves the mark to the target position.
        /// </summary>
        /// <param name="target">Vector3 target position</param>
        public void Move(Vector3 target)
        {
            transform.position = target;
        }

        /// <summary>
        /// Sets the color of the mark.
        /// </summary>
        /// <param name="c">UnityEngine.Color</param>
        public void SetColor(Color c)
        {
            c.a = 0.6f;
            renderer.material.color = c;
        }

        /// <summary>
        /// Clears the mark.
        /// </summary>
        public void Clear(bool manual_call = true)
        {
            if (DestroyOnSimulationStop || manual_call)
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }
}
