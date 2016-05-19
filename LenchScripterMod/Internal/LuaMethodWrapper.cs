﻿using System;
using System.Collections.Generic;
using UnityEngine;
using LenchScripter.Blocks;

namespace LenchScripter.Internal
{
    /// <summary>
    /// Used as a wrapper for all Lua accessible functions.
    /// Instantiated at the start of the simulation.
    /// </summary>
    internal class LuaMethodWrapper
    {
        // Measuring time
        private System.Diagnostics.Stopwatch stopwatch;
        private float startTime;

        // List of placed marks
        private List<Mark> marks;

        /// <summary>
        /// Instantiates the interface that is passed to Lua as besiege object.
        /// </summary>
        internal LuaMethodWrapper()
        {
            marks = new List<Mark>();
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            startTime = Time.time;
        }
  
        /// <summary>
        /// Returns the block's handler.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>Block object.</returns>
        public Block getBlock(string blockId)
        {
            try
            {
                return Scripter.Instance.GetBlock(new Guid(blockId));
            }
            catch (FormatException)
            {
                return Scripter.Instance.GetBlock(blockId);
            }
        }

        /// <summary>
        /// Returns true if the block has RigidBody.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>Boolean value.</returns>
        public bool exists(string blockId)
        {
            Block b = getBlock(blockId);
            return b.exists();
        }

        /// <summary>
        /// Logs the string into the debug console.
        /// </summary>
        /// <param name="msg">Message string to be logged.</param>
        public void log(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// Returns the time in seconds from the start of the simulation.
        /// Independent of the in-game time-scale slider.
        /// Useful for benchmarking your script's time of execution.
        /// </summary>
        /// <returns>Float value.</returns>
        public float getTime()
        {
            return stopwatch.ElapsedMilliseconds / 1000f;
        }

        /// <summary>
        /// Returns the the time in seconds from the start of the simulation.
        /// Consistent with the in-game time-scale slider.
        /// Useful for calculating the rate of change (speed).
        /// </summary>
        /// <returns>Float value.</returns>
        public float getScaledTime()
        {
            return (Time.time - startTime);
        }

        /// <summary>
        /// Adds a global variable to the watchlist.
        /// </summary>
        /// <param name="name">Name of the global variable.</param>
        public void watch(string name)
        {
            ScripterMod.Watchlist.AddToWatchlist(name, null, true);
        }

        /// <summary>
        /// Adds a value to watchlist under the specified display name.
        /// </summary>
        /// <param name="name">Display name of the variable.</param>
        /// <param name="value">Variable value to be reported.</param>
        public void watch(string name, System.Object value)
        {
            ScripterMod.Watchlist.AddToWatchlist(name, value, false);
        }

        /// <summary>
        /// Clears all entries from the watchlist.
        /// </summary>
        public void clearWatchlist()
        {
            ScripterMod.Watchlist.ClearWatchlist();
        }

        /// <summary>
        /// Toggles all functions to return angles in degrees.
        /// </summary>
        public void useDegrees()
        {
            Block.UseDegrees();
        }

        /// <summary>
        /// Toggles all functions to returns angles in radians.
        /// </summary>
        public void useRadians()
        {
            Block.UseRadians();
        }

        /// <summary>
        /// Invokes the block's action.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="actionName">Display name of the action.</param>
        public void action(string blockId, string actionName)
        {
            Block b = getBlock(blockId);
            b.action(actionName);
        }

        /// <summary>
        /// Sets the toggle mode of the block, specified by the toggle display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="toggleName">Toggle property to be set.</param>
        /// <param name="value">Boolean value to be set.</param>
        public void setToggleMode(string blockId, string toggleName, bool value)
        {
            Block b = getBlock(blockId);
            b.setToggleMode(toggleName, value);
        }

        /// <summary>
        /// Sets the slider value of the block, specified by the slider display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="sliderName">Slider value to be set.</param>
        /// <param name="value">Float value to be set.</param>
        public void setSliderValue(string blockId, string sliderName, float value)
        {
            Block b = getBlock(blockId);
            b.setSliderValue(sliderName, value);
        }

        /// <summary>
        /// Returns the toggle mode of the block, specified by the toggle display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="toggleName">Toggle property to be returned.</param>
        /// <returns>Boolean value.</returns>
        public bool getToggleMode(string blockId, string toggleName)
        {
            Block b = getBlock(blockId);
            return b.getToggleMode(toggleName);
        }

        /// <summary>
        /// Returns the slider value of the block, specified by the slider display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="sliderName">Toggle property to be returned.</param>
        /// <returns>Float value.</returns>
        public float getSliderValue(string blockId, string sliderName)
        {
            Block b = getBlock(blockId);
            return b.getSliderValue(sliderName);
        }

        /// <summary>
        /// Returns the key mapper's minimum slider value, specified by the slider display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="sliderName">Minimum slider value to be returned.</param>
        /// <returns>Float value.</returns>
        public float getSliderMin(string blockId, string sliderName)
        {
            Block b = getBlock(blockId);
            return b.getSliderMin(sliderName);
        }

        /// <summary>
        /// Returns the key mapper's maximum slider value, specified by the slider display name.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="sliderName">Maximum slider value to be returned.</param>
        /// <returns>Float value.</returns>
        public float getSliderMax(string blockId, string sliderName)
        {
            Block b = getBlock(blockId);
            return b.getSliderMax(sliderName);
        }

        /// <summary>
        /// Adds key to the specified key bind.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="keyName">Key bind to add the key to.</param>
        /// <param name="key">Key value to be added.</param>
        public void addKey(string blockId, string keyName, KeyCode key)
        {
            Block b = getBlock(blockId);
            b.addKey(keyName, key);
        }

        /// <summary>
        /// Replaces the first key bound to the specified key bind.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="keyName">Key bind to be replaced.</param>
        /// <param name="key">Key value to be replaced with.</param>
        public void replaceKey(string blockId, string keyName, KeyCode key)
        {
            Block b = getBlock(blockId);
            b.replaceKey(keyName, key);
        }

        /// <summary>
        /// Returns the first key value bound of the specified key bind.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="keyName">Key bind to be returned.</param>
        /// <returns>Integer value.</returns>
        public KeyCode getKey(string blockId, string keyName)
        {
            Block b = getBlock(blockId);
            return b.getKey(keyName);
        }

        /// <summary>
        /// Clears all keys of the specified key bind.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <param name="keyName"></param>
        public void clearKeys(string blockId, string keyName)
        {
            Block b = getBlock(blockId);
            b.clearKeys(keyName);
        }

        /// <summary>
        /// Returns the block's forward vector.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getForward(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getForward();
        }

        /// <summary>
        /// Returns the block's up vector.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getUp(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getUp();
        }

        /// <summary>
        /// Returns the block's right vector.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getRight(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getRight();
        }

        /// <summary>
        /// Returns the block's position vector.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getPosition(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getPosition();
        }

        /// <summary>
        /// Returns the block's velocity vector.
        /// Throws NoRigidBodyException if the block has no RigidBody.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getVelocity(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getVelocity();
        }

        /// <summary>
        /// Returns the block's mass.
        /// Throws NoRigidBodyException if the block has no RigidBody.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public float getMass(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getMass();
        }

        /// <summary>
        /// Returns the center of mass of the block, relative to the block's position.
        /// Throws NoRigidBodyException if the block has no RigidBody.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getCenterOfMass(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getCenterOfMass();
        }

        /// <summary>
        /// Returns the mass of the machine.
        /// </summary>
        /// <returns>Float value representing total mass.</returns>
        public float getMachineMass()
        {
            return Machine.Active().Mass;
        }

        /// <summary>
        /// Returns the center of mass of the machine in the world.
        /// </summary>
        /// <returns>Vector3 position of world COM.</returns>
        public Vector3 getMachineCenterOfMass()
        {
            Vector3 center = new Vector3(0, 0, 0);
            for (int i = 0; i < Machine.Active().Blocks.Count; i++)
            {
                Rigidbody body = Machine.Active().Blocks[i].GetComponent<Rigidbody>();
                if(body != null)
                    center += body.worldCenterOfMass * body.mass;
            }
            return center / Machine.Active().Mass;
        }

        /// <summary>
        /// Returns the block's rotation in the form of it's Euler angles.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getEulerAngles(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getEulerAngles();
        }

        /// <summary>
        /// Returns the block's angular velocity.
        /// Throws NoRigidBodyException if the block has no RigidBody.
        /// </summary>
        /// <param name="blockId">Block identifier string.</param>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public Vector3 getAngularVelocity(string blockId = "STARTING BLOCK 1")
        {
            Block b = getBlock(blockId);
            return b.getAngularVelocity();
        }

        /// <summary>
        /// Uses raycast to find out where mouse cursor is pointing.
        /// </summary>
        /// <returns>Returns an x, y, z positional vector of the hit.</returns>
        public Vector3 getRaycastHit()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }
            throw new Exception("Your raycast does not intersect with a collider.");
        }

        /// <summary>
        /// Casts ray defined by origin and direction vectors.
        /// </summary>
        /// <param name="origin">Origin vector of the raycast.</param>
        /// <param name="direction">Direction vector of the raycast.</param>
        /// <returns>Returns position of the hit.</returns>
        public Vector3 getRaycastHit(Vector3 origin, Vector3 direction)
        {
            RaycastHit hit;
            Ray ray = new Ray(origin, direction.normalized);
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }
            throw new Exception("Your raycast does not intersect with a collider.");
        }

        /// <summary>
        /// Uses raycast to find out what collider the mouse cursor is pointing at.
        /// If not sucessfull, returns zero vector.
        /// </summary>
        /// <returns>Returns an x, y, z positional vector of the hit.</returns>
        public TrackedCollider getRaycastCollider()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                return new TrackedCollider(hit.collider, hit.point);
            }
            throw new Exception("Your raycast does not intersect with a collider.");
        }

        /// <summary>
        /// Casts ray defined by origin and direction vectors.
        /// </summary>
        /// <param name="origin">Origin vector of the raycast.</param>
        /// <param name="direction">Direction vector of the raycast.</param>
        /// <returns>Returns TrackedCollider object of the hit.</returns>
        public TrackedCollider getRaycastCollider(Vector3 origin, Vector3 direction)
        {
            RaycastHit hit;
            Ray ray = new Ray(origin, direction.normalized);
            if (Physics.Raycast(ray, out hit))
            {
                return new TrackedCollider(hit.collider, hit.point);
            }
            throw new Exception("Your raycast does not intersect with a collider.");
        }

        /// <summary>
        /// Creates a mark at a given position.
        /// </summary>
        /// <param name="pos">Vector3 specifying position.</param>
        /// <returns>Reference to the mark.</returns>
        public Mark createMark(Vector3 pos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = "Mark";
            obj.transform.parent = Scripter.Instance.transform;
            Mark m = obj.AddComponent<Mark>();
            m.move(pos);
            marks.Add(m);
            return m;
        }

        /// <summary>
        /// Clears all marks.
        /// Called by user or at the end of the simulation.
        /// </summary>
        public void clearMarks(bool manual_call = true)
        {
            foreach (Mark m in marks)
            {
                m.clear(manual_call);
            }
            marks.Clear();
        }
    }
}