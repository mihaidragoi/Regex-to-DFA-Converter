Regex to DFA Converter
A console application developed in C# that parses Regular Expressions (Regex) and converts them step-by-step into Deterministic Finite Automata (DFA).

🚀 Main Features
Syntax Tree Construction: Uses a SyntaxTree data structure to parse the grammar and operator precedence of the regular expression.

Step-by-Step Conversion: Processes the expression through a Non-Deterministic Finite Automaton (NonDeterministicFiniteAutomaton) phase to reach the final, minimized Deterministic Finite Automaton (DeterministicFiniteAutomaton).

Automated Engine: The core conversion logic is efficiently encapsulated within the RegexToDFA class.

File Processing: Automatically reads input regular expressions from an input.txt file for rapid testing.

📁 Project Structure
The code is organized using solid Object-Oriented Programming (OOP) principles:

Automata Models: DeterministicFiniteAutomaton.cs and NonDeterministicFiniteAutomaton.cs (define states, transitions, and alphabets).

Syntax Analysis: SyntaxTree.cs (evaluates and breaks down the expression).

Conversion Engine: RegexToDFA.cs (links the components together and executes the algorithm).

Entry Point: Program.cs (runs the application and outputs the results to the console).

🛠️ Required Dependencies
To compile and run this project, you will need:

.NET SDK / .NET Framework installed on your system.

A C# compatible IDE, such as Visual Studio or JetBrains Rider (to open and build the .csproj project file).
