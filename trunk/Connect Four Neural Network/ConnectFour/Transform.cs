﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNet;
using System.Diagnostics;

namespace ConnectFour
{
    /// <summary>
    /// Contains the constant functions that transform Checkers/EndGameStates to double values to feed into neural network.
    /// </summary>
    public static class Transform
    {
        public static double ToValue(Checker checker) 
        {
            switch (checker)
            {
                case Checker.Blue: return -1.0; 
                case Checker.Green: return 1.0; 
                case Checker.Empty: return 0.0; 
                default: throw new Exception();
            }
        }

        public static double ToValue(GameResult result)
        {
            switch (result)
            {
                case GameResult.Loss: return -1.0;
                case GameResult.Win: return 1.0;
                case GameResult.Draw: return 0.0;
                default: throw new Exception();
            }
        }

        public static double ToNormalizedValue(Checker checker, Checker lastPlayerToGo)
        {
            if (checker == Checker.Empty)
                return ToValue(checker);
            else if (checker == lastPlayerToGo)
                return Math.Max(ToValue(Checker.Blue), ToValue(Checker.Green));
            else
                return Math.Min(ToValue(Checker.Blue), ToValue(Checker.Green));
        }


        /// <summary>
        /// Converts board to neurel net training example.  Returns example corresopnding to normalized board (current player's checker color corresponds to max value)
        /// </summary>
        /// <param name="lastPlayerToGo">Current Player which corresponds to last checker placed on board.</param>
        public static Example ToNormalizedExample(Board board, Checker lastPlayerToGo)
        {
            Debug.Assert(lastPlayerToGo != Checker.Empty);
            return new Example(board.Cells.Cast<Checker>().Select(c=>Transform.ToNormalizedValue(c, lastPlayerToGo)).ToList());
        }


        /// <summary>
        /// Verifies that some old transformer is equivalent to the current transformer function.
        /// </summary>
        /// <param name="check">Function to check validity of</param>
        public static bool Verify<T>(Func<T, double> check)
        {
            dynamic correct;
            if (typeof(T) == typeof(Checker))
                correct = (Func<Checker,double>) ToValue;
            else if (typeof(T) == typeof(GameResult))
                correct = (Func<GameResult,double>) ToValue;
            else
                throw new Exception();
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                if (check(val) != correct(val))
                    return false;
            }
            return true;
        }
    }
}