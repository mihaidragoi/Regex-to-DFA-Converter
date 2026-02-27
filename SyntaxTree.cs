using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class SyntaxTree
    {
        public char Value { get; set; }
        public SyntaxTree Left { get; set; }
        public SyntaxTree Right { get; set; }

        public SyntaxTree(char value)
        {
            Value = value;
            Left = null;
            Right = null;
        }

        public static SyntaxTree Build(string regex)
        {
            Stack<SyntaxTree> stack = new Stack<SyntaxTree>();
            foreach (char token in regex)
            {
                SyntaxTree node = new SyntaxTree(token);
                if (Char.IsLetterOrDigit(token))
                {
                    stack.Push(node);
                }
                else
                    if (token == '*')
                {
                    if (stack.Count < 0)
                        throw new InvalidOperationException("* error");
                    node.Left = stack.Pop();
                    stack.Push(node);
                }
                else if (token == '.' || token == '|')
                {
                    if (stack.Count < 2)
                        throw new InvalidOperationException("binary operator error");
                    node.Right = stack.Pop();
                    node.Left = stack.Pop();
                    stack.Push(node);
                }
            }
            if (stack.Count != 1)
                throw new InvalidOperationException("polish form invalid");
            return stack.Pop();
        }


        public string TraversePostorder()
        {
            string result = "";
            if (Left != null)
                result += Left.TraversePostorder();
            if (Right != null)
                result += Right.TraversePostorder();
            result += Value;
            return result;
        }


        public string TraverseInorder()
        {
            string result = "";
            if (Left != null)
                result += Left.TraverseInorder();
            result += Value;
            if (Right != null)
                result += Right.TraverseInorder();
            return result;
        }

        public string TraversePreorder()
        {
            string result = "";
            result += Value;
            if (Left != null)
                result += Left.TraversePreorder();
            if (Right != null)
                result += Right.TraversePreorder();
            return result;
        }
    }
}

