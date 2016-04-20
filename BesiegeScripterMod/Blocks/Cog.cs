﻿using System;
using System.Reflection;

namespace LenchScripterMod.Blocks
{
    /// <summary>
    /// Handler for all wheel and cog blocks.
    /// </summary>
    public class Cog : Block
    {
        private CogMotorController cmc;
        private FieldInfo input;

        private float setInputValue;
        private bool setInputFlag = false;

        internal override void Initialize(BlockBehaviour bb)
        {
            base.Initialize(bb);
            cmc = bb.GetComponent<CogMotorController>();
            input = cmc.GetType().GetField("input", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Invokes the block's action.
        /// Throws ActionNotFoundException if the block does not poses such action.
        /// </summary>
        /// <param name="actionName">Display name of the action.</param>
        public override void action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (actionName == "FORWARDS")
            {
                SetInput(1);
                return;
            }
            if (actionName == "REVERSE")
            {
                SetInput(-1);
                return;
            }
            throw new ActionNotFoundException("Block " + blockName + " has no " + actionName + " action.");
        }

        /// <summary>
        /// Sets the input value on the next LateUpdate.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        public void SetInput(float value = 1)
        {
            if (float.IsNaN(value))
                throw new ArgumentException("Value is not a number (NaN).");
            setInputValue = value;
            setInputFlag = true;
        }

        private void LateUpdate()
        {
            if (setInputFlag)
            {
                setInputFlag = false;
                input.SetValue(cmc, setInputValue);
            }
        }

        internal static bool isCog(BlockBehaviour bb)
        {
            return bb.GetComponent<CogMotorController>() != null;
        }
    }
}
