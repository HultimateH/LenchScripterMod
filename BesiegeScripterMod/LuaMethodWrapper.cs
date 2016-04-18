﻿using System.Collections.Generic;
using UnityEngine;
using NLua.Exceptions;
using LenchScripterMod.Blocks;

namespace LenchScripterMod
{

    /// <summary>
    /// Used as a wrapper for all Lua accessible functions.
    /// Instantiated at the start of the simulation.
    /// </summary>
    public class LuaMethodWrapper
    {
        // Using radians or degrees
        private float convertToDegrees;
        private float convertToRadians;
        // Measuring time
        private System.Diagnostics.Stopwatch stopwatch;
        private float startTime;
        // List of placed marks
        private List<Mark> marks;
        // Action calls enabled
        internal static bool actionCallsEnabled = true;

        /// <summary>
        /// Instantiates the interface that is passed to Lua as besiege object.
        /// </summary>
        internal LuaMethodWrapper()
        {
            marks = new List<Mark>();
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            startTime = Time.time;
            convertToDegrees = Mathf.Rad2Deg;
            convertToRadians = 1;
        }

        public Block getBlock(string blockId)
        {
            BlockBehaviour bb = ScripterMod.scripter.GetBlockBehaviour(blockId);
            if (Cog.isCog(bb))
                return new Cog(bb);
            if (Steering.isSteering(bb))
                return new Steering(bb);
            if (Cannon.isCannon(bb))
                return new Cannon(bb);
            return new Block(bb);
        }

        /// <summary>
        /// Returns BlockScript object of the modded block.
        /// If the block is not a block mod, throws an exception.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public System.Object getBlockScript(string blockId)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            System.Object bs = b.GetComponent(ScripterMod.blockScriptType);

            if(bs != null) {
                return bs;
            }

            throw new LuaException("Block " + blockId + " is not a block mod.");
        }

        /// <summary>
        /// Returns true if a block exists in a physical form.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <returns>Boolean value.</returns>
        public bool exists(string blockId)
        {
            BlockBehaviour b;
            try
            {
                b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            }
            catch (LuaException)
            {
                return false;
            }
                
            if (b == null) return false;
            if (b.transform == null) return false;
            if (b.GetComponent<Rigidbody>() == null) return false;
            return true;
        }

        /// <summary>
        /// Logs the message into the mod-loader's console.
        /// </summary>
        /// <param name="msg"></param>
        public void log(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// Gets simulation time, independent of in-game time scaling.
        /// </summary>
        /// <returns></returns>
        public long getTime()
        {
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Gets simulation time, consistent with in-game time scaling.
        /// </summary>
        /// <returns></returns>
        public float getScaledTime()
        {
            return (Time.time - startTime) * 1000;
        }

        /// <summary>
        /// Reports the value of the variable to the watchlist.
        /// </summary>
        public void watch(string name, System.Object value)
        {
            ScripterMod.watchlist.AddToWatchlist(name, value, false);
        }

        /// <summary>
        /// Clears the watchlist.
        /// </summary>
        public void clearWatchlist()
        {
            ScripterMod.watchlist.ClearWatchlist();
        }

        /// <summary>
        /// Makes any future calls to angle functions return degrees.
        /// </summary>
        public void useDegrees()
        {
            convertToDegrees = Mathf.Rad2Deg;
            convertToRadians = 1;
        }

        /// <summary>
        /// Makes any future calls to angle functions return radians.
        /// </summary>
        public void useRadians()
        {
            convertToDegrees = 1;
            convertToRadians = Mathf.Deg2Rad;
        }

        /// <summary>
        /// Makes a block do something.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="actionName"></param>
        public void action(string blockId, string actionName)
        {
            if (!actionCallsEnabled)
                return;
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            bool keyAdded = false;
            foreach (MKey m in b.Keys)
            {
                if (m.DisplayName.ToUpper() == actionName.ToUpper())
                {
                    for (int i = 0; i < m.KeyCode.Count; i++)
                        if (m.KeyCode[i] == KeyCode.None)
                        {
                            m.AddOrReplaceKey(i, InputManager.actionKeyCode);
                            keyAdded = true;
                            break;
                        }
                    if (!keyAdded)
                        m.AddKey(InputManager.actionKeyCode);
                    ScripterMod.scripter.activatedBlocks.Add(b);
                    return;
                }
            }
            throw new LuaException("Action " + actionName + " not found.");
        }

        /// <summary>
        /// Used to toggle various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="toggleName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <param name="value">Boolean value to be set.</param>
        public void setToggleMode(string blockId, string toggleName, bool value)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MToggle m in b.Toggles)
            {
                if (m.DisplayName.ToUpper() == toggleName.ToUpper())
                {
                    m.IsActive = value;
                    return;
                }
            }
            throw new LuaException("Toggle " + toggleName + " not found.");
        }

        /// <summary>
        /// Used to set slider value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="sliderName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <param name="value">Float value to be set.</param>
        public void setSliderValue(string blockId, string sliderName, float value)
        {
            if (float.IsNaN(value))
                throw new LuaException("Value is not a number (NaN).");
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MSlider m in b.Sliders)
            {
                if (m.DisplayName.ToUpper() == sliderName.ToUpper())
                {
                    m.Value = value;
                    return;
                }
            }
            throw new LuaException("Slider " + sliderName + " not found.");
        }

        /// <summary>
        /// Used to set limit value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="limitName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <param name="value">Float value to be set.</param>
        public void setLimitValue(string blockId, string limitName, float value)
        {
            if (float.IsNaN(value))
                throw new LuaException("Value is not a number (NaN).");
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MLimits m in b.Limits)
            {
                if (m.DisplayName.ToUpper() == limitName.ToUpper())
                {
                    m.MaxValue = value;
                    return;
                }
            }
            throw new LuaException("Slider " + limitName + " not found.");
        }

        /// <summary>
        /// Used to get the toggle value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="toggleName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <returns>Returns the toggle value of a specified property.</returns>
        public bool getToggleMode(string blockId, string toggleName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MToggle m in b.Toggles)
            {
                if (m.DisplayName.ToUpper() == toggleName.ToUpper())
                {
                    return m.IsActive;
                }
            }
            throw new LuaException("Toggle " + toggleName + " not found.");
        }

        /// <summary>
        /// Used to get the slider value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="sliderName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <returns>Returns the float value of a specified property.</returns>
        public float getSliderValue(string blockId, string sliderName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MSlider m in b.Sliders)
            {
                if (m.DisplayName.ToUpper() == sliderName.ToUpper())
                {
                    return m.Value;
                }
            }
            throw new LuaException("Slider " + sliderName + " not found.");
        }

        /// <summary>
        /// Used to get the limit value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="limitName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <returns>Returns the float value of a specified property.</returns>
        public float getLimitValue(string blockId, string limitName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MLimits m in b.Limits)
            {
                if (m.DisplayName.ToUpper() == limitName.ToUpper())
                {
                    return m.MaxValue;
                }
            }
            throw new LuaException("Slider " + limitName + " not found.");
        }

        /// <summary>
        /// Used to get the minimum slider value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="sliderName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <returns>Returns the float value of a specified property.</returns>
        public float getSliderMin(string blockId, string sliderName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MSlider m in b.Sliders)
            {
                if (m.DisplayName.ToUpper() == sliderName.ToUpper())
                {
                    return m.Min;
                }
            }
            throw new LuaException("Slider " + sliderName + " not found.");
        }

        /// <summary>
        /// Used to get the maximum slider value of various block properties.
        /// Throws an exception if the property is not found.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier.</param>
        /// <param name="sliderName">Case insensitive string specifying the property to be set.
        /// Usually identical to in-game label.</param>
        /// <returns>Returns the float value of a specified property.</returns>
        public float getSliderMax(string blockId, string sliderName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MSlider m in b.Sliders)
            {
                if (m.DisplayName.ToUpper() == sliderName.ToUpper())
                {
                    return m.Max;
                }
            }
            throw new LuaException("Slider " + sliderName + " not found.");
        }

        /// <summary>
        /// Used to add blocks key controls.
        /// Throws an exception if the key name is not found.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <param name="keyName">Key display name.</param>
        /// <param name="keyValue">New key to be assigned.</param>
        public void addKey(string blockId, string keyName, int keyValue)
        {
            KeyCode key = (KeyCode)keyValue;
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MKey m in b.Keys)
            {
                if (m.DisplayName.ToUpper() == keyName.ToUpper())
                {
                    for (int i = 0; i < m.KeyCode.Count; i++)
                        if(m.KeyCode[i] == KeyCode.None)
                        {
                            m.AddOrReplaceKey(i, key);
                            return;
                        }
                    m.AddKey(key);
                    return;
                }
            }
            throw new LuaException("Key " + keyName + " not found.");
        }

        /// <summary>
        /// Used to replace blocks key controls.
        /// Throws an exception if the key name is not found.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <param name="keyName">Key display name.</param>
        /// <param name="keyValue">New key to be assigned.</param>
        public void replaceKey(string blockId, string keyName, int keyValue)
        {
            KeyCode key = (KeyCode)keyValue;
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MKey m in b.Keys)
            {
                if (m.DisplayName.ToUpper() == keyName.ToUpper())
                {
                    m.AddOrReplaceKey(0, key);
                }
            }
            throw new LuaException("Key " + keyName + " not found.");
        }

        /// <summary>
        /// Used to replaced blocks key controls.
        /// Throws an exception if the key name is not found.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <param name="keyName">Key display name.</param>
        public int getKey(string blockId, string keyName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MKey m in b.Keys)
            {
                if (m.DisplayName.ToUpper() == keyName.ToUpper())
                {
                    return (int)m.KeyCode[0];
                }
            }
            throw new LuaException("Key " + keyName + " not found.");
        }

        /// <summary>
        /// Used to remove all of the blocks key controls.
        /// Throws an exception if the key name is not found.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <param name="keyName">Key display name.</param>
        public void clearKeys(string blockId, string keyName)
        {
            BlockBehaviour b = ScripterMod.scripter.GetBlockBehaviour(blockId);
            foreach (MKey m in b.Keys)
            {
                if (m.DisplayName.ToUpper() == keyName.ToUpper())
                {
                    for (int i = 0; i < m.KeyCode.Count; i++)
                        m.AddOrReplaceKey(i, KeyCode.None);
                    return;
                }
            }
            throw new LuaException("Key " + keyName + " not found.");
        }

        /// <summary>
        /// Returns the forward vector of the specified block.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public Vector3 getForward(string blockId = "STARTING BLOCK 1")
        {
            return ScripterMod.scripter.GetBlockBehaviour(blockId).transform.forward;
        }

        /// <summary>
        /// Returns the up vector of the specified block.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public Vector3 getUp(string blockId = "STARTING BLOCK 1")
        {
            return ScripterMod.scripter.GetBlockBehaviour(blockId).transform.up;
        }

        /// <summary>
        /// Returns the right vector of the specified block.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public Vector3 getRight(string blockId = "STARTING BLOCK 1")
        {
            return ScripterMod.scripter.GetBlockBehaviour(blockId).transform.right;
        }

        /// <summary>
        /// Returns the position vector of the specified block.
        /// If no argument is used, starting block is used.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <returns>Vector3 object.</returns>
        public Vector3 getPosition(string blockId = "STARTING BLOCK 1")
        {
            return ScripterMod.scripter.GetBlockBehaviour(blockId).transform.position;
        }

        /// <summary>
        /// Returns the velocity vector of the specified block
        /// in units per second.
        /// If no argument is used, starting block is used.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <returns>Vector3 object.</returns>
        public Vector3 getVelocity(string blockId = "STARTING BLOCK 1")
        {
            Rigidbody body = ScripterMod.scripter.GetBlockBehaviour(blockId).GetComponent<Rigidbody>();
            if (body != null)
                return body.velocity;
            throw new LuaException("Block " + blockId + " has no rigid body.");
        }

        /// <summary>
        /// Returns mass of the block.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public float getMass(string blockId = "STARTING BLOCK 1")
        {
            Rigidbody body = ScripterMod.scripter.GetBlockBehaviour(blockId).GetComponent<Rigidbody>();
            if (body != null)
                return body.mass;
            throw new LuaException("Block " + blockId + " has no rigid body.");
        }

        /// <summary>
        /// Returns the center of mass relative to the blocks position.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        public Vector3 getCenterOfMass(string blockId = "STARTING BLOCK 1")
        {
            Rigidbody body = ScripterMod.scripter.GetBlockBehaviour(blockId).GetComponent<Rigidbody>();
            if (body != null)
                return body.centerOfMass;
            throw new LuaException("Block " + blockId + " has no rigid body.");
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
        /// Returns the euler angles vector of the specified block,
        /// respective to the blocks forward, right, up vectors.
        /// If no argument is used, starting block is used.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <returns>Vector3 object.</returns>
        public Vector3 getEulerAngles(string blockId = "STARTING BLOCK 1")
        {
            Vector3 d2r = new Vector3(convertToRadians, convertToRadians, convertToRadians);
            Vector3 euler = ScripterMod.scripter.GetBlockBehaviour(blockId).transform.eulerAngles;
            euler.Scale(d2r);
            return euler;
        }

        /// <summary>
        /// Returns the angular velocity vector of the specified block.
        /// If no argument is used, starting block is used.
        /// </summary>
        /// <param name="blockId">Block identifier.</param>
        /// <returns>Vector3 object.</returns>
        public Vector3 getAngularVelocity(string blockId = "STARTING BLOCK 1")
        {
            Rigidbody body = ScripterMod.scripter.GetBlockBehaviour(blockId).GetComponent<Rigidbody>();
            if (body != null)
            {
                Vector3 convertUnits = new Vector3(convertToDegrees, convertToDegrees, convertToDegrees);
                Vector3 angularVelocity = body.angularVelocity;
                angularVelocity.Scale(convertUnits);
                return angularVelocity;
            }
            throw new LuaException("Block " + blockId + " has no rigid body.");
        }

        /// <summary>
        /// Calculates the heading of the specified block.
        /// Works the same as GetYaw.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier. Default is starting block.</param>
        /// <returns>Float value ranging from 0 to 360 or 2*PI.</returns>
        public float getHeading(string blockId = "STARTING BLOCK 1")
        {
            Quaternion q = ScripterMod.scripter.GetBlockBehaviour(blockId).transform.rotation;
            float jaw = Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z) * convertToDegrees;
            return jaw < 0 ? jaw + 2 * Mathf.PI * convertToDegrees : jaw;
        }

        /// <summary>
        /// Returns directed yaw angle of the specified block in degrees.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier. Default is starting block.</param>
        /// <returns>Float value ranging from -180 to 180.</returns>
        public float getYaw(string blockId = "STARTING BLOCK 1")
        {
            Quaternion q = ScripterMod.scripter.GetBlockBehaviour(blockId).transform.rotation;
            return Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z) * convertToDegrees;
        }

        /// <summary>
        /// Returns directed pitch angle of the specified block in degrees.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier. Default is starting block.</param>
        /// <returns>Float value ranging from -180 to 180.</returns>
        public float getPitch(string blockId = "STARTING BLOCK 1")
        {
            Quaternion q = ScripterMod.scripter.GetBlockBehaviour(blockId).transform.rotation;
            return -Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z) * convertToDegrees;
        }

        /// <summary>
        /// Returns directed roll angle of the specified block in degrees.
        /// </summary>
        /// <param name="blockId">Blocks unique identifier. Default is starting block.</param>
        /// <returns>Float value ranging from -180 to 180.</returns>
        public float getRoll(string blockId = "STARTING BLOCK 1")
        {
            Quaternion q = ScripterMod.scripter.GetBlockBehaviour(blockId).transform.rotation;
            return -Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Uses raycast to find out where mouse cursor is pointing.
        /// If not sucessfull, returns zero vector.
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
            throw new LuaException("Your raycast does not intersect with a collider.");
        }

        /// <summary>
        /// Creates a mark at a given position.
        /// </summary>
        /// <param name="pos">Vector3 specifying position.</param>
        /// <returns>Reference to the mark.</returns>
        public Mark createMark(Vector3 pos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mark m = obj.AddComponent<Mark>();
            m.move(pos);
            marks.Add(m);
            return m;
        }

        /// <summary>
        /// Clears all marks.
        /// Called by user or at the end of the simulation.
        /// </summary>
        public void clearMarks()
        {
            foreach (Mark m in marks)
            {
                m.clear();
            }
            marks.Clear();
        }
    }

    /// <summary>
    /// Mark script attached to primitive sphere objects.
    /// Used to mark locations through Lua script.
    /// </summary>
    public class Mark : MonoBehaviour
    {

        void Start()
        {
            GetComponent<Renderer>().material.color = Color.red;
            Destroy(GetComponent<SphereCollider>());
        }

        /// <summary>
        /// Moves the mark to the target position.
        /// </summary>
        /// <param name="target">Vector3 target position.</param>
        public void move(Vector3 target)
        {
            transform.position = target;
        }

        /// <summary>
        /// Sets the color of the mark.
        /// </summary>
        /// <param name="c">UnityEngine.Color object.</param>
        public void setColor(Color c)
        {
            GetComponent<Renderer>().material.color = c;
        }

        /// <summary>
        /// Clears the mark.
        /// </summary>
        public void clear()
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
