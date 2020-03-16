using System;
using System.Reflection;
using UnityEngine;

namespace Lench.Scripter.Blocks
{
    /// <summary>
    ///     Handler for the Spring and Rope blocks.
    /// </summary>
    public class Spring : Block
    {
        private readonly SpringCode _sc;
        
        private MethodInfo winchContract,winchUnwind;
        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Spring(BlockBehaviour bb) : base(bb)
        {
            _sc = bb.GetComponent<SpringCode>();

            Type type = _sc.GetType();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            winchContract = type.GetMethod("WinchContract", flags);
            winchUnwind = type.GetMethod("WinchUnwind", flags);
        }

        /// <summary>
        ///     Invokes the block's action.
        ///     Throws ActionNotFoundException if the block does not posess such action.
        /// </summary>
        /// <param name="actionName">Display name of the action.</param>
        public override void Action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (_sc.winchMode)
            {
                switch (actionName)
                {
                    case "WIND":
                        Wind();
                        return;
                    case "UNWIND":
                        Unwind();
                        return;
                }
            }
            else
            {
                switch (actionName)
                {
                    case "CONTRACT":
                        Contract();
                        return;
                }
            }

            throw new ActionNotFoundException($"Block {BlockName} has no {actionName} action.");
        }

        /// <summary>
        ///     Controls the spring / winch. Contracts if rate is positive and unwinds if negative.
        ///     Springs cannot be unwound.
        /// </summary>
        /// <param name="rate">Rate of movement.</param>
        public void SetInput(float rate = 1)
        {
            if (Mathf.Abs(rate) < 0.02) return;
            if (_sc.winchMode)
                if (rate > 0)
                    //_sc.WinchContract(rate);
                    Wind(rate);
                else
                    //_sc.WinchUnwind(-rate);
                    Unwind(-rate);
            else
                //_sc.Contract(rate);
                Contract(rate);
        }

        /// <summary>
        ///     Contracts the spring.
        /// </summary>
        public void Contract(float rate = 1)
        {
            //_sc.Contract(rate);
            winchContract.Invoke(_sc, new object[] { rate });
        }

        /// <summary>
        ///     Winds the winch.
        /// </summary>
        public void Wind(float rate = 1)
        {
            //_sc.WinchContract(rate);
            winchContract.Invoke(_sc, new object[] { rate });
        }

        /// <summary>
        ///     Unwinds the winch.
        /// </summary>
        public void Unwind(float rate = 1)
        {
            //_sc.WinchUnwind(rate); 
            winchUnwind.Invoke(_sc, new object[] { rate });
        }
    }
}