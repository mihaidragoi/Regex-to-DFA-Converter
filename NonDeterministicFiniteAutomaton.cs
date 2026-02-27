using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class NonDeterministicFiniteAutomaton
    {
        public HashSet<string> states;
        public HashSet<char> alphabet;
        public Dictionary<(string, char), HashSet<string>> transitionFunction;
        public string initialState;
        public HashSet<string> finalStates;
        public const char lambda = '\0';


        public NonDeterministicFiniteAutomaton()
        {
            states = new HashSet<string>();
            alphabet = new HashSet<char>();
            transitionFunction = new Dictionary<(string, char), HashSet<string>>();
            initialState = string.Empty;
            finalStates = new HashSet<string>();
        }

        public NonDeterministicFiniteAutomaton(HashSet<string> states, HashSet<char> alphabet, Dictionary<(string, char), HashSet<string>> transitionFunction, string initialState, HashSet<string> finalStates)
        {
            this.states = states;
            this.alphabet = alphabet;
            this.transitionFunction = transitionFunction;
            this.initialState = initialState;
            this.finalStates = finalStates;
        }

        public void addState(string state)
        {
            states.Add(state);
        }

        public void setInitialState(string state)
        {
            addState(state);
            initialState = state;
        }
        public void addSymbolToAlphabet(char symbol)
        {
            if (symbol != lambda)
                alphabet.Add(symbol);
        }

        public void addTransition(string fromState, char symbol, string toState)
        {
            if (!states.Contains(fromState)) states.Add(fromState);
            if (!states.Contains(toState)) states.Add(toState);

            if (symbol != lambda)
            {
                alphabet.Add(symbol);
            }

            var key = (fromState, symbol);
            if (!transitionFunction.ContainsKey(key))
            {
                transitionFunction[key] = new HashSet<string>();
            }
            transitionFunction[key].Add(toState);
        }

        public HashSet<string> EClosure(HashSet<string> states)
        {
            Stack<string> stack = new Stack<string>(states);
            HashSet<string> closure = new HashSet<string>(states);

            while (stack.Count > 0)
            {
                string currentState = stack.Pop();
                var key = (currentState, lambda);

                if (transitionFunction.ContainsKey(key))
                {
                    foreach (var nextState in transitionFunction[key])
                    {
                        if (!closure.Contains(nextState))
                        {
                            closure.Add(nextState);
                            stack.Push(nextState);
                        }
                    }
                }
            }
            return closure;
        }

        public HashSet<string> Move(HashSet<string> states, char symbol)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var state in states)
            {
                var key = (state, symbol);
                if (transitionFunction.ContainsKey(key))
                {
                    foreach (var nextState in transitionFunction[key])
                    {
                        result.Add(nextState);
                    }
                }
            }
            return result;
        }
    }
}
