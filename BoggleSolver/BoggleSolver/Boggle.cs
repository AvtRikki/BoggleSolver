using System.Text;

namespace BoggleSolver
{
    /// <summary>
    /// Small comments on the API proposed in the problem:
    /// 1. int as input type for boardWidth and boardHeight not make sense as we expect a positive number, my proposal to change it to uint
    /// 2. IEnumerable<string> as a input type also not a good choice because IEnumerable<T> is quite common interface and as a result if 
    ///     a user will pass IQueryable<T> to the function we can potentially have one more round trip on the database. 
    ///     On the other hand, the user may pass by mistake BlockingCollection<T> which in turn can greatly affect the performance of this method.
    ///     My proposal to change  IEnumerable<string> ->  IReadOnlyList<string>
    /// </summary>
    public class Boggle
    {
        private readonly List<(int, int)> _movingDirections = new() 
        { 
            ( 0,  1),  // Right 
            ( 0, -1),  // Left
            ( 1,  0),  //Down
            (-1,  0),  //Up
            ( 1,  1),  //Down-Right
            (-1,  1),  //Up-Right
            ( 1, -1),  //Down-Left
            (-1, -1)   //Up-Left
           };

        private Trie? _legalWords;
        private int _boardWidth;
        private int _boardHeight;
        private string? _boardLetters;

        /// <summary>
        /// This method is used in the game to set the list of legal words that players can form from the letters on the game board.
        /// Prior to using this method, it is necessary to load the list of valid words so that players can form words that will be considered during the game.
        /// The method overwrites the previous list of legal words, so the provided collection should contain the complete and up-to-date list of words.
        /// If players form words during the game that are not included in the list of legal words, such words will be considered invalid and will not be counted towards the score.
        /// </summary>
        /// <param name="allWords">A collection of strings, alphabetically-sorted, that represents the list of all valid words that can be used in the game. Each string represents a single word.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when allWords is set to null</exception>
        public void SetLegalWords(IEnumerable<string> allWords)
        {
            if (allWords is null)
            {
                throw new ArgumentNullException(nameof(allWords));
            }

            _legalWords = new Trie();

            foreach (string word in allWords)
            {
                _legalWords.Insert(word);
            }
        }

        /// <summary>
        /// This method is used in the game to solve the game board and find all possible words that can be formed from the letters provided on the game board.
        /// The method solves the game board based on the specified dimensions and letters, and returns a list of all possible words that can be formed
        /// The board is solved based on specific rules, such as forming words by connecting neighboring letters (vertically, horizontally, or diagonally), each letter can only be used once, and words must be valid and present in the list of legal words
        /// The list of legal words must be set in advance using the SetLegalWords() method
        /// The result is a collection of all the valid words found on the game board
        /// </summary>
        /// <param name="boardWidth">The width of the game board, specified as an integer. It determines the number of columns on the game board</param>
        /// <param name="boardHeight">The height of the game board, specified as an integer. It determines the number of rows on the game board</param>
        /// <param name="boardLetters">A string containing the letters, without spaces or separators, representing the letters arranged on the game board from left to right and top to bottom</param>
        /// <returns>A collection of strings containing all the possible words found on the game board. Each string represents a single word</returns>
        /// <exception cref="System.ArgumentException">Thrown when boardWidth or boardHeight is set to be less than 1.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when boardLetters is set to null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when boardLetters size less than the size of the board (boardWidth * boardHeight)</exception>
        public IEnumerable<string> SolveBoard(int boardWidth, int boardHeight, string boardLetters)
        {
            if (_legalWords == null)
            {
                return Enumerable.Empty<string>();
            }

            if (boardWidth <= 0 || boardHeight <= 0)
            {
                throw new ArgumentException("Invalid board width. The board width must be greater than zero", nameof(boardWidth));
            }

            if (boardLetters == null)
            {
                throw new ArgumentNullException(nameof(boardLetters));
            }

            var boardLettersCount = boardWidth * boardHeight;
            if (boardLetters.Length < boardLettersCount)
            {
                throw new ArgumentOutOfRangeException("Insufficient string size to fill the board. The string size must be equal to or greater than the size of the board (boardWidth * boardHeight).");
            }

            _boardLetters = boardLetters;
            _boardHeight = boardHeight;
            _boardWidth = boardWidth;

            HashSet<(int, int)> checkedCellsPositions = new HashSet<(int, int)>();
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            StringBuilder word = new();
            for (int row = 0; row < boardHeight; row++)
            {
                for (int column = 0; column < boardWidth; column++)
                {
                    char letter = boardLetters[TranslateBoardCoordsToArrayIndex(row, column)];
                    if (!_legalWords.Root.Children.TryGetValue(Char.ToUpperInvariant(letter), out TrieNode currentNode))
                    { 
                        continue; 
                    }

                    (string part, TrieNode nextNode) = GetNextNode(letter, currentNode);
                    word.Append(part);
                    checkedCellsPositions.Add((row, column));
                    SearchWords(row, column, checkedCellsPositions, nextNode, word, result);
                    word.Remove(word.Length - part.Length, part.Length);
                    checkedCellsPositions.Remove((row, column));
                }
            }

            return result;
        }

        private void SearchWords(int row, int column, HashSet<(int, int)> checkedCellsPositions, TrieNode root, StringBuilder word, HashSet<string> foundWords)
        {
            if (root.EndOfWord)
            {
                foundWords.Add(word.ToString());
            }

            foreach ((int cellX, int cellY) in _movingDirections)
            {
                int cellIndexX = cellX + row;
                int cellIndexY = cellY + column;

                if (checkedCellsPositions.Contains((cellIndexX, cellIndexY))) 
                {
                    continue;
                }

                if (cellIndexX < _boardHeight && cellIndexX >= 0 && cellIndexY < _boardWidth && cellIndexY >= 0)
                {
                    char letter = _boardLetters[TranslateBoardCoordsToArrayIndex(cellIndexX, cellIndexY)];
                    if (root.Children.TryGetValue(Char.ToUpperInvariant(letter), out TrieNode currentNode))
                    {
                        (string part, TrieNode nextNode) = GetNextNode(letter, currentNode);
                        checkedCellsPositions.Add((cellIndexX, cellIndexY));
                        word.Append(part);
                        SearchWords(cellIndexX, cellIndexY, checkedCellsPositions, nextNode, word, foundWords);
                        checkedCellsPositions.Remove((cellIndexX, cellIndexY));
                        word.Remove(word.Length - part.Length, part.Length);
                    }
                }
            }
        }

        private (string part, TrieNode node) GetNextNode(char letter, TrieNode currentNode)
        {
            TrieNode nextNode = currentNode;
            string part = letter.ToString();
            if (letter.Equals('q'))
            {
                nextNode = WalkThruSpecialNode(nextNode);
                part = "qu";
            }

            return (part, nextNode);
        }

        private TrieNode WalkThruSpecialNode(TrieNode rootNode)
        {
            return rootNode.Children['Q'].Children['U'];
        }

        private int TranslateBoardCoordsToArrayIndex(int row, int column)
        {
            return (row * _boardWidth) + column;
        }

        private class TrieNode
        {
            public TrieNode() : this('\0')
            {
            }

            public TrieNode(char letter)
            {
                Children = new Dictionary<char, TrieNode>();
                EndOfWord = false;
                Letter = letter;
            }

            public Dictionary<char, TrieNode> Children { get; }

            public char Letter { get; }

            public bool EndOfWord { get; set; }
        }

        private class Trie
        {
            private readonly TrieNode _root;

            public Trie()
            {
                _root = new TrieNode();
            }

            public TrieNode Root => _root;

            public void Insert(string word)
            {
                TrieNode current = _root;

                foreach (char letter in word)
                {
                    var upperLetter = Char.ToUpperInvariant(letter);
                    if (!current.Children.TryGetValue(upperLetter, out TrieNode node))
                    {
                        node = new TrieNode(upperLetter);
                        current.Children.Add(upperLetter, node);
                    }

                    current = node;
                }

                current.EndOfWord = true;
            }
        }
    }
}
