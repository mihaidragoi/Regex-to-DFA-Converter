🚀 From Logic to Machine: The Regex-to-DFA Journey
Have you ever wondered how a simple string like (a|b)*abb actually gets "understood" by a computer? It’s not magic—it’s a beautiful pipeline of mathematical transformations!

This project is my personal deep-dive into the world of Formal Languages. I've built a converter that takes a "human-readable" Regular Expression and transforms it into a hyper-efficient Deterministic Finite Automaton (DFA).

🎢 The Pipeline (How it works)Think of this as a factory assembly line:
The Blueprint (Regex → Postfix): We turn the expression inside out using the Shunting-yard algorithm so the computer knows exactly what operation to do first.
The Skeleton (Postfix → NFA): Using Thompson's rules, we create a "fuzzy" machine that can be in multiple places at once (Epsilon transitions! 👻).
The Final Form (NFA → DFA): We calculate $\epsilon$-closures to remove the "fuzziness," resulting in a lean, mean, string-matching machine.
