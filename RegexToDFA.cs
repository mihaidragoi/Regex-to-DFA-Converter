using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class RegexToDFA
    {
        private int stateCounter = 0;

        public string GetNewState()
        {
            stateCounter++;
            return "q" + stateCounter;
        }

        public string PolishForm(string regex)
        {
            string regexFormatted = FormatRegex(regex);

            List<char> polish= new List<char>();
            Stack<char> stack = new Stack<char>();

            var priority = new Dictionary<char, int>
            {
                {'(', 0},
                {'|', 1},
                {'.', 2},
                {'*', 3}
            };

            foreach (var current in regexFormatted)
            {
                if (Char.IsLetterOrDigit(current))
                    polish.Add(current);
                else
                    if (current == '(')
                    stack.Push(current);
                else
                    if (current == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        polish.Add(stack.Pop());
                    }
                    if (stack.Count > 0 && stack.Peek() == '(')
                        stack.Pop();
                }
                else
                {
                    while (stack.Count > 0 && priority.ContainsKey(stack.Peek()) && priority[stack.Peek()] >= priority[current])
                    {
                        polish.Add(stack.Pop());
                    }
                    stack.Push(current);
                }
            }
            while(stack.Count > 0)
            {
                polish.Add(stack.Pop());
            }
            return string.Join("", polish);
        }

        public string FormatRegex(string regex)
        {
            StringBuilder expanded = new StringBuilder();
            for(int i=0;i< regex.Length;i ++)
            {
                char current = regex[i];
                if(current == '+')
                {
                    int start = i - 1;
                    if (start < 0)
                        throw new InvalidOperationException("+ cannot be at the beginning");
                    if (regex[start] == ')')
                    {
                        int balance = 1;
                        start--;
                        while (start >= 0 && balance > 0)
                        {
                            if (regex[start] == ')') balance++;
                            else if (regex[start] == '(') balance--;
                            start--;
                        }
                        start++;

                        string segment = regex.Substring(start, i - start);
                        expanded.Append(segment).Append('*');
                    }
                    else if(Char.IsLetterOrDigit(regex[start]) || regex[start] == '*')
                    {
                        char segment = regex[start];
                        expanded.Append(segment).Append('*');
                    }
                    else
                    {
                        throw new InvalidOperationException("+ applied to invalid operator.");
                    }
                }

                else
                {
                    expanded.Append(current);
                }

            }

            string regexExpanded = expanded.ToString();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < regexExpanded.Length; i++)
            {
                char current = regexExpanded[i];
                result.Append(current);
                if (i +1  < regexExpanded.Length)
                {
                    char next = regexExpanded[i + 1];
                    if ((Char.IsLetterOrDigit(current) || current == '*' || current == ')') && (Char.IsLetterOrDigit(next) || next == '('))
                    {
                        result.Append('.');
                    }
                }
            }
            return result.ToString();
        }

        public DeterministicFiniteAutomaton BuildDFAFromRegex(string regex)
        {
            string polish = PolishForm(regex);
            NonDeterministicFiniteAutomaton nfa = ConstructNFA(polish);
            return ConvertNFAtoDFA(nfa);
        }

        private NonDeterministicFiniteAutomaton ConstructNFA(string regexPostfix)
        {
            var stack = new Stack<NonDeterministicFiniteAutomaton>();

            foreach (char token in regexPostfix)
            {
                if (char.IsLetterOrDigit(token))
                {
                    stack.Push(CreateBaseNFA(token));
                }
                else if (token == '*')
                {
                    var nfa = stack.Pop();
                    stack.Push(CreateStarNFA(nfa));
                }
                else if (token == '.')
                {
                    var nfa2 = stack.Pop();
                    var nfa1 = stack.Pop();
                    stack.Push(ConcatenateNFAs(nfa1, nfa2));
                }
                else if (token == '|')
                {
                    var nfa2 = stack.Pop();
                    var nfa1 = stack.Pop();
                    stack.Push(UnionNFAs(nfa1, nfa2));
                }
            }
            return stack.Pop();
        }

        private NonDeterministicFiniteAutomaton CreateBaseNFA(char symbol)
        {
            var nfa= new NonDeterministicFiniteAutomaton();
            var startState = GetNewState();
            var acceptState = GetNewState();
            nfa.initialState = startState;
            nfa.finalStates.Add(acceptState);
            nfa.addTransition(startState, symbol, acceptState);
            return nfa;
        }

        private NonDeterministicFiniteAutomaton ConcatenateNFAs(NonDeterministicFiniteAutomaton nfa1, NonDeterministicFiniteAutomaton nfa2)
        {
            foreach (var finalState in nfa1.finalStates)
            {
                nfa1.addTransition(finalState, NonDeterministicFiniteAutomaton.lambda, nfa2.initialState);
            }

            foreach (var kvp in nfa2.transitionFunction)
            {
                nfa1.transitionFunction[kvp.Key] = kvp.Value;
            }
            nfa1.states.UnionWith(nfa2.states);
            nfa1.alphabet.UnionWith(nfa2.alphabet);
            nfa1.finalStates = nfa2.finalStates; 

            return nfa1;
        }

        private NonDeterministicFiniteAutomaton UnionNFAs(NonDeterministicFiniteAutomaton nfa1, NonDeterministicFiniteAutomaton nfa2)
        {
            var newNfa = new NonDeterministicFiniteAutomaton();
            string startState = GetNewState();
            string endState = GetNewState();

            newNfa.initialState = startState;
            newNfa.finalStates.Add(endState);

            MergeNFAContent(newNfa, nfa1);
            MergeNFAContent(newNfa, nfa2);

            newNfa.addState(startState);
            newNfa.addState(endState);

            newNfa.addTransition(startState, NonDeterministicFiniteAutomaton.lambda, nfa1.initialState);
            newNfa.addTransition(startState, NonDeterministicFiniteAutomaton.lambda, nfa2.initialState);

            foreach (var final in nfa1.finalStates) newNfa.addTransition(final, NonDeterministicFiniteAutomaton.lambda, endState);
            foreach (var final in nfa2.finalStates) newNfa.addTransition(final, NonDeterministicFiniteAutomaton.lambda, endState);

            return newNfa;
        }

        private NonDeterministicFiniteAutomaton CreateStarNFA(NonDeterministicFiniteAutomaton nfa)
        {
            var newNfa = new NonDeterministicFiniteAutomaton();
            string startState = GetNewState();
            string endState = GetNewState();

            newNfa.initialState = startState;
            newNfa.finalStates.Add(endState);

            MergeNFAContent(newNfa, nfa);
            newNfa.addState(startState);
            newNfa.addState(endState);

            newNfa.addTransition(startState, NonDeterministicFiniteAutomaton.lambda, endState);

            newNfa.addTransition(startState, NonDeterministicFiniteAutomaton.lambda, nfa.initialState);

            foreach (var final in nfa.finalStates)
            {
                newNfa.addTransition(final, NonDeterministicFiniteAutomaton.lambda, nfa.initialState);
                newNfa.addTransition(final, NonDeterministicFiniteAutomaton.lambda, endState);
            }

            return newNfa;
        }

        private void MergeNFAContent(NonDeterministicFiniteAutomaton target, NonDeterministicFiniteAutomaton source)
        {
            target.states.UnionWith(source.states);
            target.alphabet.UnionWith(source.alphabet);
            foreach (var transition in source.transitionFunction)
            {
                target.transitionFunction[transition.Key] = transition.Value;
            }
        }

        public DeterministicFiniteAutomaton ConvertNFAtoDFA(NonDeterministicFiniteAutomaton nfa)
        {
            DeterministicFiniteAutomaton dfa = new DeterministicFiniteAutomaton();

            Dictionary<string, HashSet<string>> dfaStatesMap = new Dictionary<string, HashSet<string>>();
            Queue<string> queue = new Queue<string>();
            int dfaStateCounter = 0;

            var initialSet = nfa.EClosure(new HashSet<string> { nfa.initialState });
            string startStateName = "S" + (dfaStateCounter++);

            dfa.setInitialState(startStateName);
            dfaStatesMap[startStateName] = initialSet;
            queue.Enqueue(startStateName);

            if (initialSet.Overlaps(nfa.finalStates))
                dfa.addFinalState(startStateName);

            while (queue.Count > 0)
            {
                string currentStateName = queue.Dequeue();
                HashSet<string> currentSet = dfaStatesMap[currentStateName];

                foreach (char symbol in nfa.alphabet)
                {
                    var moveResult = nfa.Move(currentSet, symbol);
                    var nextSet = nfa.EClosure(moveResult);

                    if (nextSet.Count == 0) continue; 

                    string nextStateName = null;
                    foreach (var existing in dfaStatesMap)
                    {
                        if (existing.Value.SetEquals(nextSet))
                        {
                            nextStateName = existing.Key;
                            break;
                        }
                    }

                    if (nextStateName == null)
                    {
                        nextStateName = "S" + (dfaStateCounter++);
                        dfaStatesMap[nextStateName] = nextSet;
                        queue.Enqueue(nextStateName);

                        if (nextSet.Overlaps(nfa.finalStates))
                            dfa.addFinalState(nextStateName);
                    }

                    dfa.addTransition(currentStateName, symbol, nextStateName);
                }
            }

            return dfa;
        }

        public SyntaxTree BuildSyntaxTree(string regex)
        {
            string postfix = PolishForm(regex);
            return SyntaxTree.Build(postfix);
        }

        public void SyntaxTreeDisplay(string regex)
        {
            SyntaxTree root = BuildSyntaxTree(regex);
            Console.WriteLine("Postordine: " + root.TraversePostorder());
            Console.WriteLine("Inordine: " + root.TraverseInorder());
            Console.WriteLine("Preordine: " + root.TraversePreorder());
        }

    }
}
