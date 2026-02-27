using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    internal class DeterministicFiniteAutomaton
    {
        public HashSet<string> states;
        public HashSet<char> alphabet;
        public Dictionary<(string, char), string> transitionFunction;
        public string initialState;
        public HashSet<string> finalStates;

        public DeterministicFiniteAutomaton()
        {
            states = new HashSet<string>();
            alphabet = new HashSet<char>();
            transitionFunction = new Dictionary<(string, char), string>();
            initialState = string.Empty;
            finalStates = new HashSet<string>();
        }

        public DeterministicFiniteAutomaton(HashSet<string> states, HashSet<char> alphabet, Dictionary<(string, char), string> transitionFunction, string initialState, HashSet<string> finalStates)
        {
            this.states = states;
            this.alphabet = alphabet;
            this.transitionFunction = transitionFunction;
            this.initialState = initialState;
            this.finalStates = finalStates;
        }

        public bool VerifyAutomaton()
        {
            if (!states.Contains(initialState))
                return false;
            foreach (var transition in transitionFunction)
            {
                var (state, symbol) = transition.Key;
                var nextState = transition.Value;
                if (!states.Contains(state) || !states.Contains(nextState) || !alphabet.Contains(symbol))
                    return false;
                if (!alphabet.Contains(symbol))
                    return false;
            }
            return true;
        }

        public void PrintAutomaton()
        {
            Console.WriteLine("Tabelul functiei de tranzitie:");
            Console.Write("Stare/Simbol ");
            foreach (var symbol in alphabet)
                Console.Write(symbol + " ");
            Console.WriteLine();
            foreach (var state in states)
            {
                Console.Write(state + " ");
                foreach (var symbol in alphabet)
                {
                    if (transitionFunction.ContainsKey((state, symbol)))
                        Console.Write(transitionFunction[(state, symbol)] + " ");
                    else
                        Console.Write("- ");
                }
                Console.WriteLine();
            }
        }


        public bool CheckWord(string word)
        {
            string currentState = initialState;
            foreach (var symbol in word)
            {
                var key = (currentState, symbol);
                if (!transitionFunction.ContainsKey(key))
                {
                    Console.WriteLine($"Tranzitie inexistenta pentru starea '{currentState}' si simbolul '{symbol}'");
                    return false;
                }
                else
                    currentState = transitionFunction[key];
            }

            if (finalStates.Contains(currentState))
            {
                Console.WriteLine($"Cuvantul '{word}' este acceptat de automat (stare finala: '{currentState}')");
                return true;
            }
            else
            {
                Console.WriteLine($"Cuvantul '{word}' nu este acceptat de automat (stare finala: '{currentState}')");
                return false;
            }
        }

        public void addFinalState(string state)
        {
            addState(state);
            finalStates.Add(state);
        }

        public void addTransition(string fromState, char symbol, string toState)
        {
            addState(fromState);
            addState(toState);
            addSymbolToAlphabet(symbol);
            transitionFunction[(fromState, symbol)] = toState;
        }


        public void setInitialState(string state)
        {
            addState(state);
            initialState = state;
        }

        public void addState(string state)
        {
            states.Add(state);
        }

        public void addSymbolToAlphabet(char symbol)
        {
            alphabet.Add(symbol);
        }

    }
}
